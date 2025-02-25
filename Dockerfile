ARG config=Release

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000




FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["app/app.csproj", "app/"]
COPY ["MyContent/MyContent.msbuildproj", "MyContent/"]
COPY ["Peachpie.Library.Sqlite/Peachpie.Library.Sqlite.csproj", "Peachpie.Library.Sqlite/"]
COPY ["Peachpie.Wordpress.Sqlite/Peachpie.Wordpress.Sqlite.msbuildproj", "Peachpie.Wordpress.Sqlite/"]
COPY ["global.json", "global.json"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["peachpie-wordpress.sln", "./"]



RUN dotnet restore "peachpie-wordpress.sln" -v diag
COPY . .
WORKDIR "/src/app"
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
