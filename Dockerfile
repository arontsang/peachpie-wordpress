﻿ARG config=Release

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["app/app.csproj", "app/"]
COPY ["MyContent/MyContent.msbuildproj", "MyContent/"]
COPY ["global.json", "global.json"]
COPY ["Directory.Build.props", "Directory.Build.props"]

COPY ["app/packages.lock.json", "app/"]
COPY ["MyContent/packages.lock.json", "MyContent/"]

RUN dotnet restore "app/app.csproj" --locked-mode -v diag
COPY . .
WORKDIR "/src/app"
ARG config
RUN  dotnet build "app.csproj" -c $config --no-restore -o /app/build  

FROM build AS publish
ARG config
RUN dotnet publish "app.csproj" \
    -c $config  \
    -p:PublishReadyToRun=true  \
    -r linux-x64  \
    --self-contained  \
    -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:80
ENTRYPOINT ["./app"]
