ARG DOTNET_SDK_VERSION

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION}-alpine AS build

WORKDIR /build

# Restore NuGet packages.
COPY *.sln Directory.Packages.props ./
COPY */*/*.*proj ./
RUN dotnet sln list | \
    tail -n +3 | \
    xargs -I {} sh -c \
    'target="{}"; dir="${target%/*}"; file="${target##*/}"; mkdir -p -- "$dir"; mv -- "$file" "$target"'
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

FROM mcr.microsoft.com/dotnet/runtime-deps:${DOTNET_SDK_VERSION}-alpine

WORKDIR /app

COPY --from=build /publish ./

ENV DOTNET_ENVIRONMENT=Production

CMD ["./Client.Console"]
