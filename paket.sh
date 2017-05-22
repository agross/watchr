#!/usr/bin/env bash

mono=

if [[ "$OS" != "Windows_NT" ]]; then
  mono=mono
fi

$mono .paket/paket.exe "$@"
