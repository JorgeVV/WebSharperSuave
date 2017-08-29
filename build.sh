#!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net
  .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    .paket/paket.bootstrapper.exe
    .paket/paket.exe restore
  fi
  packages/FAKE/tools/FAKE.exe $@ --fsiargs build.fsx
else
  # use mono
  mono .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    mono .paket/paket.bootstrapper.exe
    mono .paket/paket.exe restore
  fi
  mono packages/FAKE/tools/FAKE.exe $@ --fsiargs -d:MONO build.fsx
fi
