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

SET MT_SICS_CMD="Start MT-SICS TCP-IP Device.cmd"

:: Verify that command is available via the system's PATH:
WHERE %MT_SICS_CMD% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO :NoCommand

:: Get executable directory (required below):
FOR /F "tokens=* USEBACKQ" %%A IN (`WHERE %MT_SICS_CMD%`) DO (
    SET PATH_OF_CMD=%%A
)
CALL :GetDirOfFile "%PATH_OF_CMD%" DIR_OF_CMD

:: Change to executable directory (limitation of executable):
CD /D %DIR_OF_CMD%

:: Invoke command:
CALL %MT_SICS_CMD%

GOTO :End

:NoCommand
ECHO.
ECHO The required %MT_SICS_CMD% is not available!
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

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
