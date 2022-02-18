ARG VERSION=6.0-alpine

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${VERSION} AS build

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
                   source/Client.Console/Client.Console.csproj && \
  \
  mv -- "/build/build/bin/Client.Console/$runtime/publish/" /publish

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/runtime-deps:${VERSION}

WORKDIR /app

COPY --from=build /publish ./

ENV DOTNET_ENVIRONMENT=Production

CMD ["./Client.Console"]
