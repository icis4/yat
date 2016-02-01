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
::  Copyright © 2003-2015 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http://www.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

SET USB_HUB_CTRL_EXE=USBHubControl.exe

:: Verify that executable is available
WHERE %USB_HUB_CTRL_EXE% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO ERROR_EXE

:: Start Hub 1 'USB'
%USB_HUB_CTRL_EXE% A6YJ5BDF 110111

:: Start Hub 2 'RS-232'
:: Ensure to activate devices subsequently (work-around to limitation of MCT or windows driver ??)
:: Must be done in reversed order, enumeration/configuration of higher devices otherwise may fail ??
ECHO.
ECHO Enabling higher pair of devices...
%USB_HUB_CTRL_EXE% A6YJ5A78 001100
:: Time required for execution is approx 3 seconds
TIMEOUT 10
:: Time required to load the drivers is approx 6 seconds => 10 seconds timeout
ECHO.
ECHO Additionally enabling lower pair of devices...
%USB_HUB_CTRL_EXE% A6YJ5A78 001111
:: Time required for execution is approx 3 seconds
TIMEOUT 10
:: Time required to load the drivers is approx 6 seconds => 10 seconds timeout

GOTO END

:ERROR_EXE
ECHO.
ECHO The required %USB_HUB_CTRL_EXE% is not available!
ECHO Make sure that the "MCD Conline USB HUB" drivers are installed, and...
ECHO ..."<MCD>\Tools\CommandLine" has been added to the system's PATH!
ECHO.
PAUSE
GOTO END

:END

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
