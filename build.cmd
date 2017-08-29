@echo off
cls

.paket\paket.exe restore
if errorlevel 1 (
  .paket\paket.bootstrapper.exe
  .paket\paket.exe restore
)

packages\FAKE\tools\FAKE.exe build.fsx %*
