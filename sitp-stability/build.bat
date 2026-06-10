@echo off
setlocal

REM Clean
if exist ".\build" (
  rmdir /s /q ".\build"
)

if exist ".\debug" (
  rmdir /s /q ".\debug"
)

if exist ".\agent\obj" (
  rmdir /s /q ".\agent\obj"
)

REM Build application
dotnet restore .\src\sitp-stability.csproj --force
dotnet build .\src\sitp-stability.csproj /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet

dotnet restore .\src\tryout.csproj --force
dotnet build .\src\tryout.csproj /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet