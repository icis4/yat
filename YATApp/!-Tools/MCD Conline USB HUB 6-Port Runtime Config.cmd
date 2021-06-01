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
:: Copyright © 2003-2021 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

::==================================================================================================
:: Purpose
:: ------------------------------------------------------------------------------------------------
:: This helper batch invokes the MCD Conline USB HUB control in an appropriate sequence.
::==================================================================================================

::==================================================================================================
:: HowTo
:: ------------------------------------------------------------------------------------------------
:: 1. Install the MCD Conline USB HUB drivers.
:: 2. Add the installation's "\Tools\CommandLine" subdirectory, typically
::    "C:\Program Files (x86)\MCD USBHubMonitor\Tools\CommandLine", to the system's PATH.
::==================================================================================================

SETLOCAL

SET _exe=USBHubControl.exe

:: Verify that executable is available via the system's PATH:
WHERE %_exe% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO :NoExe

:: Reset both hubs:
ECHO.
ECHO Hub 1: Disabling devices . . .
%_exe% A6YJ5BDF 000000
ECHO Hub 2: Disabling devices . . .
%_exe% A6YJ5A78 000000
:: Time required for execution is below 3 seconds.
:: Time required to unload the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.Test.Devices.UsbHubControl'.

:: Start hub 1 'USB'
ECHO.
ECHO Hub 1: Enabling devices . . .
%_exe% A6YJ5BDF 010111
:: !!! "Out 6" is disabled (010111 instead of 110111) because TI LauchPad composite devices don't work concurrently !!!
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.Test.Devices.UsbHubControl'.

:: Start hub 2 'RS-232'
:: Ensure to activate devices subsequently (workaround to limitation of MCT or windows driver ??).
:: Must be done in reversed order, enumeration/configuration of higher devices otherwise may fail.
ECHO.
ECHO Hub 2: Enabling higher pair of devices . . .
%_exe% A6YJ5A78 001100
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.Test.Devices.UsbHubControl'.
ECHO.
ECHO Hub 2: Adding lower pair of devices . . .
%_exe% A6YJ5A78 001111
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.Test.Devices.UsbHubControl'.

GOTO :End

:NoExe
ECHO.
ECHO The required MCD Conline USB HUB control executable is not available!
ECHO Make sure that the "MCD Conline USB HUB" drivers are installed, and. . .
ECHO . . ."<MCD USBHubMonitor>\Tools\CommandLine" has been added to the system's PATH!
ECHO.
PAUSE
GOTO :End

:End

ENDLOCAL

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
