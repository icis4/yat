@ECHO OFF

::==================================================================================================
:: YAT - Yet Another Terminal.
:: Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
:: Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
:: ------------------------------------------------------------------------------------------------
:: $URL$
:: $Revision$
:: $Date$
:: $Author$
:: ------------------------------------------------------------------------------------------------
:: See release notes for product version details.
:: See SVN change log for file revision details.
::
::     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
::     Attention: Windows/DOS requires that this file is encoded in ASCII/ANSI and not UTF-8!
::     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
::
:: Author(s): Matthias Klaey
:: ------------------------------------------------------------------------------------------------
:: Copyright © 2003-2019 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

SETLOCAL

SET _cmd="Start MT-SICS TCP-IP Device.cmd"

:: Verify that command is available via the system's PATH:
WHERE %_cmd% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO :NoCommand

:: Get executable directory (required below):
FOR /F "tokens=* USEBACKQ" %%A IN (`WHERE %_cmd%`) DO (
    SET _cmdPath=%%A
)
CALL :GetDirOfFile "%_cmdPath%" _cmdDir

:: Change to executable directory (limitation of executable):
CD /D %_cmdDir%

:: Invoke command:
CALL %_cmd%

GOTO :End

:NoCommand
ECHO.
ECHO The required %_cmd% is not available!
ECHO Make sure that it has been added to the system's PATH!
ECHO.
PAUSE
GOTO :End

:GetDirOfFile <IN_PathOfFile> <OUT_Result>
(
    SET "%~2=%~dp1"
    EXIT /B
)

:End

ENDLOCAL

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
