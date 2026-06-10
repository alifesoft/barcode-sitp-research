@echo off
setlocal

if exist ".\release" (
  rmdir /s /q ".\release"
)

if exist ".\debug" (
  rmdir /s /q ".\debug"
)

if exist ".\src\obj" (
  rmdir /s /q ".\src\obj"
)

if exist ".\output" (
  rmdir /s /q ".\output"
)
