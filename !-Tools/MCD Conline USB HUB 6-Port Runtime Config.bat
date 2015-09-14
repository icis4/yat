@ECHO OFF

:: =================================================================================================
::  YAT - Yet Another Terminal.
::  Visit YAT at http:REMsourceforge.net/projects/y-a-terminal/.
::  Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
::  -----------------------------------------------------------------------------------------------
::  $URL$
::  $Author$
::  $Date$
::  $Revision$
::  -----------------------------------------------------------------------------------------------
::  MKY Development Version 1.0.10
::  -----------------------------------------------------------------------------------------------
::  See SVN change log for revision details.
::  See release notes for product version details.
::  -----------------------------------------------------------------------------------------------
::  Copyright © 2003-2015 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

SET USB_HUB_CTRL_EXE=USBHubControl.exe

:: Verify the hub control is available
WHERE %USB_HUB_CTRL_EXE% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO ERROR_EXE

:: Hub 1 'USB'
%USB_HUB_CTRL_EXE% A6YJ5BDF 110111

:: Hub 2 'RS-232'
%USB_HUB_CTRL_EXE% A6YJ5A78 001111

GOTO END

:ERROR_EXE
ECHO.
ECHO The required %USB_HUB_CTRL_EXE% is not available!
ECHO Make sure that the "MCD Conline USB HUB" drivers are installed, and...
ECHO ..."\Tools\CommandLine\USBHubControl.exe" has been added to the system's PATH!
ECHO.
PAUSE
GOTO END

:END

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
