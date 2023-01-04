ARG DOTNET_VERSION=7.0-alpine
ARG NODE_VERSION=14-alpine

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build-net

WORKDIR /build

# Install dotnet tools.
COPY .config/ ./.config/
RUN dotnet tool restore

# Run paket restore.
COPY paket.* ./
RUN dotnet paket restore

# Copy code and build.
COPY . ./

ARG TARGETPLATFORM
ARG CONFIGURATION=Release
RUN case "$TARGETPLATFORM" in \
      linux/arm64) runtime=linux-musl-arm64 ;; \
      linux/amd64) runtime=linux-musl-x64 ;; \
      *) printf 'Unsupported TARGETPLATFORM %s\n' "$TARGETPLATFORM"; exit 1; \
    esac && \
    \
    dotnet publish --nologo \
                   --runtime $runtime \
                   --self-contained \
                   --configuration ${CONFIGURATION} \
                   source/Web/Web.csproj && \
  \
  mv -- "./build/bin/Web/$runtime/publish/" /publish && \
  rm -rf -- /publish/wwwroot/scripts

FROM --platform=$BUILDPLATFORM node:${NODE_VERSION} AS build-node

WORKDIR /build

COPY package.json package-lock.json ./
RUN npm install

COPY . ./

RUN npm run rollup && \
    mv -- ./source/Web/wwwroot/scripts /publish && \
    find /publish -type f -name '*.ts' -delete

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}

EXPOSE 80

HEALTHCHECK --start-period=15s --interval=5s \
            CMD [ "wget", "-O", "/dev/null", "--header", "X-Health-Check: 1", "http://localhost/health" ]

WORKDIR /app

COPY --from=build-net /publish ./
COPY --from=build-node /publish ./wwwroot/scripts/

ENV DOTNET_ENVIRONMENT=Production
ENV Logging__Console__FormatterName=simple

ENTRYPOINT ["dotnet", "Web.dll"]
