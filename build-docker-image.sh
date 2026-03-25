#!/usr/bin/env bash

set -euo pipefail

# Useful for debugging.
# env

image_name=watchr
dirs=(source/Web source/Client.Console)
tags=(web client)

for index in "${!dirs[@]}"; do
  docker buildx \
        build \
        --tag "$image_name-${tags[index]}" \
        --load \
        --file "${dirs[index]}/Dockerfile" \
        "$@" \
        .
done
