@echo off
setlocal

REM Clean
if exist ".\release" (
  rmdir /s /q ".\release"
)

if exist ".\debug" (
  rmdir /s /q ".\debug"
)

if exist ".\src\obj" (
  rmdir /s /q ".\src\obj"
)

REM Build application
dotnet restore .\src\sitp-stability.csproj --force
dotnet build .\src\sitp-stability.csproj /p:Configuration=Debug /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet

dotnet restore .\src\sitp-stability.csproj --force
dotnet build .\src\sitp-stability.csproj /p:Configuration=Release /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet

dotnet restore .\src\tryout.csproj --force
dotnet build .\src\tryout.csproj /p:Configuration=Debug /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet

dotnet restore .\src\tryout.csproj --force
dotnet build .\src\tryout.csproj /p:Configuration=Release /p:WarningLevel=0 /nologo /t:Rebuild /verbosity:quiet