#!/usr/bin/env bash

set -euo pipefail

# Useful for debugging.
# env

image_name=watchr
dirs=(source/Web source/Client.Console)
tags=(web client)

get-nodejs-version-from() {
  local version_file="${1:?Need path to version file}"

  local version

  version="$(cat "$version_file")"

  printf '%s' "${version%.*}"
}

get-dotnet-sdk-version-from() {
  local global_json="${1:?Need path to global.json}"

  if ! hash jq 2> /dev/null; then
    >&2 printf 'Please install jq using your package manager.\n'
    exit 1
  fi

  local version
  version="$(jq --raw-output \
                --exit-status \
                .sdk.version \
                "$global_json")"

  printf '%s' "${version%.*}"
}

nodejs_version="NODE_VERSION=$(get-nodejs-version-from .node-version)"
dotnet_sdk_version="DOTNET_SDK_VERSION=$(get-dotnet-sdk-version-from global.json)"

for index in "${!dirs[@]}"; do
  docker buildx \
        build \
        --build-arg "$nodejs_version" \
        --build-arg "$dotnet_sdk_version" \
        --tag "$image_name-${tags[index]}" \
        --load \
        --file "${dirs[index]}/Dockerfile" \
        "$@" \
        .
done
