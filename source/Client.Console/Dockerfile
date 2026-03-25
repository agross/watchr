FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /build

# Restore NuGet packages.
COPY *.sln* Directory.*.props ./
COPY --parents **/*.*proj ./
RUN dotnet restore

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
    dotnet publish \
           --nologo \
           --runtime "$runtime" \
           --self-contained \
           --configuration "$CONFIGURATION" \
           --output /publish \
           source/Client.Console/Client.Console.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine

WORKDIR /app

COPY --from=build /publish ./

ENV DOTNET_ENVIRONMENT=Production

CMD ["./Client.Console"]
