@ECHO OFF

IF %1=="" GOTO NoPath
IF %2=="" GOTO NoDirectory

REM ---
PUSHD %1

ECHO Cleaning %2 in %1
DEL /F /Q %2

POPD
GOTO End
REM ---

:NoPath
ECHO Parameter 1 must be a valid file path (wildcards allowed)
GOTO End

:NoDirectory
ECHO Parameter 2 must be a valid directory name
GOTO End

:End
