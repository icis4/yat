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
:: Author(s): Matthias Klaey
:: ------------------------------------------------------------------------------------------------
:: Copyright © 2003-2017 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

SET USB_HUB_CTRL_EXE=USBHubControl.exe

:: Verify that executable is available:
WHERE %USB_HUB_CTRL_EXE% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO ERROR_EXE

:: Reset both hubs:
ECHO.
ECHO Hub 1: Disabling devices . . .
%USB_HUB_CTRL_EXE% A6YJ5BDF 000000
ECHO Hub 2: Disabling devices . . .
%USB_HUB_CTRL_EXE% A6YJ5A78 000000
:: Time required for execution is below 3 seconds.
:: Time required to unload the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.IO.Ports.Test.UsbHubControl'.

:: Start hub 1 'USB'
ECHO.
ECHO Hub 1: Enabling devices . . .
%USB_HUB_CTRL_EXE% A6YJ5BDF 110111
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 2 seconds.
::  => 5 seconds timeout:
TIMEOUT 5
:: Note that the timeouts also need to be configured in 'MKY.IO.Ports.Test.UsbHubControl'.

:: Start hub 2 'RS-232'
:: Ensure to activate devices subsequently (workaround to limitation of MCT or windows driver ??).
:: Must be done in reversed order, enumeration/configuration of higher devices otherwise may fail.
ECHO.
ECHO Hub 2: Enabling higher pair of devices . . .
%USB_HUB_CTRL_EXE% A6YJ5A78 001100
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.IO.Ports.Test.UsbHubControl'.
ECHO.
ECHO Hub 2: Adding lower pair of devices . . .
%USB_HUB_CTRL_EXE% A6YJ5A78 001111
:: Time required for execution is below 3 seconds.
:: Time required to load the drivers is below 6 seconds.
::  => 9 seconds timeout:
TIMEOUT 9
:: Note that the timeouts also need to be configured in 'MKY.IO.Ports.Test.UsbHubControl'.

GOTO END

:ERROR_EXE
ECHO.
ECHO The required %USB_HUB_CTRL_EXE% is not available!
ECHO Make sure that the "MCD Conline USB HUB" drivers are installed, and. . .
ECHO . . ."<MCD>\Tools\CommandLine" has been added to the system's PATH!
ECHO.
PAUSE
GOTO END

:END

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
