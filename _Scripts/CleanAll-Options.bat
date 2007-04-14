@ECHO OFF

PUSHD ..

ECHO Cleaning user options...
DEL /F /Q /A:H "*.suo"
ECHO ...successfully cleaned

POPD
