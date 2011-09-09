@ECHO OFF

PUSHD ..

ECHO Cleaning all \.svn directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" ".svn"
ECHO ...successfully cleaned

ECHO Cleaning all \.git directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" ".git"
ECHO ...successfully cleaned

ECHO Cleaning Ankh.Load...
DEL /F /Q /A:H "Ankh.Load"
ECHO ...successfully cleaned

POPD
