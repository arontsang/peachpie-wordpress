ARG config=Release

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000


FROM alpine AS plugins
RUN apk add unzip
ADD https://downloads.wordpress.org/plugin/sqlite-database-integration.2.1.16.zip /tmp/sqlite.zip
RUN mkdir /tmp/wp-content/plugins -p && unzip /tmp/sqlite.zip -d /tmp/wp-content/plugins/
RUN cp /tmp/wp-content/plugins/sqlite-database-integration/db.copy /tmp/wp-content/db.php

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["app/app.csproj", "app/"]
COPY ["MyContent/MyContent.msbuildproj", "MyContent/"]
COPY ["Peachpie.Library.Sqlite/Peachpie.Library.Sqlite.csproj", "Peachpie.Library.Sqlite/"]
COPY ["global.json", "global.json"]
COPY ["Directory.Build.props", "Directory.Build.props"]



RUN dotnet restore "app/app.csproj" -v diag
COPY . .
WORKDIR "/src/app"
COPY --from=plugins /tmp/wp-content/ /src/MyContent/
ARG config
RUN  dotnet build "app.csproj" -c $config --no-restore -o /app/build  

FROM build AS publish
ARG config
RUN dotnet publish "app.csproj" \
    -c $config  \
    -r linux-x64  \
    --self-contained  \
    -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "./app.dll"]
