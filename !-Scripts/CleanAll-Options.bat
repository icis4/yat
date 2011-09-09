@ECHO OFF

PUSHD ..

ECHO Cleaning solution user options...
DEL /F /Q /A:H "*.suo"
ECHO ...successfully cleaned

ECHO Cleaning additional solution settings...
DEL /F /Q /A:H "*.xml"
ECHO ...successfully cleaned

ECHO Cleaning all project user options...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanFiles.bat" "%%I" "*.csproj.user"
ECHO ...successfully cleaned

POPD
