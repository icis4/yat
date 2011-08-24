@ECHO OFF

PUSHD ..

ECHO Cleaning all StyleCop temporaries...
FOR /R %%I IN (.) DO CALL "_Scripts\CleanDir.bat" "%%I" "*.StyleCop"
ECHO ...successfully cleaned

POPD
