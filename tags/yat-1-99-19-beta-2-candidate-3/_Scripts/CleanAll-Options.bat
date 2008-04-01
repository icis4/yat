@ECHO OFF

PUSHD ..

ECHO Cleaning user options...
DEL /F /Q /A:H "*.suo"
ECHO ...successfully cleaned

ECHO Cleaning additional options...
DEL /F /Q /A:H "*.xml"
ECHO ...successfully cleaned

POPD
