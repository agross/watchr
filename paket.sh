#!/bin/bash

maybe_mono=mono
if [ "$OS" == "Windows_NT" ]; then
  # Use .NET
  maybe_mono=
fi

if [ ! -f .paket/paket.exe ]; then
  $maybe_mono .paket/paket.bootstrapper.exe
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    exit $exit_code
  fi
fi

$maybe_mono .paket/paket.exe $*
exit_code=$?
if [ $exit_code -ne 0 ]; then
  exit $exit_code
fi
