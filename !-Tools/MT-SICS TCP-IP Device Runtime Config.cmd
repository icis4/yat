@ECHO OFF

:: =================================================================================================
::  YAT - Yet Another Terminal.
::  Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
::  Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
::  -----------------------------------------------------------------------------------------------
::  $URL$
::  $Author$
::  $Date$
::  $Revision$
::  -----------------------------------------------------------------------------------------------
::  See SVN change log for revision details.
::  See release notes for product version details.
::  -----------------------------------------------------------------------------------------------
::  Copyright © 2003-2017 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http://www.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

SET MT_SICS_EXE=ExampleBalance_RB_Evalboard_LPC1769.exe
SET MT_SICS_EXE_PATH="D:\MKY\Projekte\Software\YAT\Test Environment\MT RB ExampleBalance"

:: Verify that executable is available:
WHERE %MT_SICS_EXE% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO ERROR_EXE

:: Change to executable directory (limitation of executable):
CD /D %MT_SICS_EXE_PATH%

:: Start executable:
%MT_SICS_EXE%

GOTO END

:ERROR_EXE
ECHO.
ECHO The required %MT_SICS_EXE% is not available!
ECHO Make sure that it has been added to the system's PATH!
ECHO.
PAUSE
GOTO END

:END

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
