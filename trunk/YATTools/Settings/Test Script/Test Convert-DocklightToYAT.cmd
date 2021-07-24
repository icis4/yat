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

:: Run script with default args => will result in 0 files converted:
PUSHD ..
CALL ..\..\YAT\!-Scripts\CallPowerShellScript.cmd .\Convert-DocklightToYAT.ps1
POPD

:: Run script with args referring to the available Docklight examples => will result in 12 files converted:
PUSHD ..
CALL ..\..\YAT\!-Scripts\CallPowerShellScript.cmd .\Convert-DocklightToYAT.ps1 -InputFilePathPattern ".\Docklight*\*.ptp"
POPD

:: Let the user see the results:
PAUSE

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
