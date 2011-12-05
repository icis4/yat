@ECHO OFF

PUSHD ..

ECHO Cleaning all \bin directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "bin"
ECHO ...successfully cleaned

ECHO Cleaning all \obj directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "obj"
ECHO ...successfully cleaned

ECHO Cleaning all temporary files in ALAZ source and demos...
PUSHD "ALAZ"
FOR /R %%I IN (.) DO CALL "..\!-Scripts\CleanFiles.bat" "%%I" "*.tmp"
POPD
ECHO ...successfully cleaned

POPD
