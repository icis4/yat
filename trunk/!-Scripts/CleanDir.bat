@ECHO OFF

IF %1=="" GOTO NoPath
IF %2=="" GOTO NoDirectory

REM ---
PUSHD %1

ECHO Cleaning %2 in %1
RMDIR /S /Q %2

POPD
GOTO End
REM ---

:NoPath
ECHO Parameter 1 must be a valid path
GOTO End

:NoDirectory
ECHO Parameter 2 must be a valid directory name
GOTO End

:End
