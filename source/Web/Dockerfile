ARG NODE_VERSION
ARG DOTNET_SDK_VERSION

FROM --platform=$BUILDPLATFORM node:${NODE_VERSION:?Required} AS vue

WORKDIR /app

COPY source/Frontend/package*.json ./

RUN apt update && \
    apt install --yes \
                build-essential \
                libcairo2-dev \
                libpango1.0-dev \
                libjpeg-dev \
                libgif-dev \
                librsvg2-dev && \
                npm install

COPY source/Frontend ./

RUN npm run build

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION}-alpine AS dotnet

WORKDIR /build

# Install dotnet tools.
COPY .config/ ./.config/
RUN dotnet tool restore

# Run paket restore.
COPY paket.* ./
RUN dotnet paket restore

# Copy code and build.
COPY . ./

ARG CONFIGURATION=Release
ARG TARGETPLATFORM
RUN case "$TARGETPLATFORM" in \
      linux/arm64) runtime=linux-musl-arm64 ;; \
      linux/amd64) runtime=linux-musl-x64 ;; \
      *) printf 'Unsupported TARGETPLATFORM %s\n' "$TARGETPLATFORM"; exit 1; \
    esac && \
    \
    dotnet publish \
           --nologo \
           --runtime "$runtime" \
           --self-contained \
           --configuration "$CONFIGURATION" \
           --output /publish \
           source/Web/Web.csproj

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:${DOTNET_SDK_VERSION}-alpine

EXPOSE 8080

HEALTHCHECK --start-period=15s \
            --interval=5s \
            CMD [ "wget", "-O", "/dev/null", "--header", "X-Health-Check: 1", "http://localhost:8080/health" ]

# https://github.com/dotnet/SqlClient/issues/220
RUN apk add --no-cache icu-data-full icu-libs tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

COPY --from=dotnet /publish ./
COPY --from=vue /app/dist wwwroot/

ENV DOTNET_ENVIRONMENT=Production
ENV Logging__Console__FormatterName=simple

ENTRYPOINT ["dotnet", "Web.dll"]
