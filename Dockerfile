FROM docker.io/library/alpine:3.17 AS base
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
RUN  dotnet build "app.csproj" -c Release -o /app/build  --no-restore

FROM build AS publish
RUN dotnet publish "app.csproj" \
    -c Release  \
    -p:PublishReadyToRun=true  \
    -r linux-musl-x64  \
    --self-contained  \
    -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:80
ENTRYPOINT ["app"]
