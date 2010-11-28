@ECHO OFF

PUSHD ..

ECHO Cleaning temporary setup directories...
RMDIR /S /Q "YAT.Setup\Debug"
RMDIR /S /Q "YAT.Setup\Release"
RMDIR /S /Q "YAT.Setup\Release with Redistributable"
ECHO ...successfully cleaned

POPD
