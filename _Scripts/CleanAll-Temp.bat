@ECHO OFF

PUSHD ..

ECHO Cleaning all \bin directories...
FOR /R %%I IN (.) DO CALL "_Scripts\CleanDir.bat" "%%I" "bin"
ECHO ...successfully cleaned

ECHO Cleaning all \obj directories...
FOR /R %%I IN (.) DO CALL "_Scripts\CleanDir.bat" "%%I" "obj"
ECHO ...successfully cleaned

POPD
