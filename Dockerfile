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
COPY /app/ /src/app/
COPY /MyContent/ /src/MyContent/
COPY /Peachpie.Library.Sqlite/ /src/Peachpie.Library.Sqlite/
COPY /Peachpie.Wordpress.Sqlite/ /src/Peachpie.Wordpress.Sqlite/
WORKDIR "/src/app"
ARG config

FROM build AS publish
ARG config
RUN dotnet publish "app.csproj" \
    -c $config  \
    -r linux-x64  \
    --self-contained  \
    -o /app/publish 

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS s6-overlay
ARG S6_OVERLAY_VERSION=3.2.0.2
RUN apt-get update && apt-get install -y xz-utils
RUN mkdir /rootfs
ADD https://github.com/just-containers/s6-overlay/releases/download/v${S6_OVERLAY_VERSION}/s6-overlay-noarch.tar.xz /tmp
RUN tar -C /rootfs -Jxpf /tmp/s6-overlay-noarch.tar.xz
ADD https://github.com/just-containers/s6-overlay/releases/download/v${S6_OVERLAY_VERSION}/s6-overlay-x86_64.tar.xz /tmp
RUN tar -C /rootfs -Jxpf /tmp/s6-overlay-x86_64.tar.xz

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS litestream
ARG LITESTREAM_VERSION=0.3.13
RUN apt-get update && apt-get install -y tar gzip
ADD https://github.com/benbjohnson/litestream/releases/download/v${LITESTREAM_VERSION}/litestream-v${LITESTREAM_VERSION}-linux-amd64.tar.gz /tmp/litestream.tar.gz
RUN mkdir /dist
RUN tar -C /dist -zxvf /tmp/litestream.tar.gz

FROM base AS final
COPY --from=s6-overlay /rootfs /
COPY --from=publish /app/publish /app
EXPOSE 8080
EXPOSE 8200

COPY /s6/etc/ /etc/
COPY --from=litestream /dist/litestream /opt/bin/litestream

CMD ["/init"]
