@echo off

if not exist .paket\paket.exe (
  .paket\paket.bootgstrapper.exe
  if errorlevel 1 (
    exit /b %errorlevel%
  )
)

.paket\paket.exe %*
