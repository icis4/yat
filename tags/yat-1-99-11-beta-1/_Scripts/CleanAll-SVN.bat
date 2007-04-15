@ECHO OFF

PUSHD ..

ECHO Cleaning all \.svn directories...
FOR /R %%I IN (.) DO CALL "_Scripts\CleanDir.bat" "%%I" ".svn"
ECHO ...successfully cleaned

ECHO Cleaning Ankh.Load...
DEL /F /Q /A:H "Ankh.Load"
ECHO ...successfully cleaned

POPD
