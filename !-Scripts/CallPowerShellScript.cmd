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
:: Purpose and Use Cases
:: ------------------------------------------------------------------------------------------------
:: This helper batch simplifies calling a PowerShell script from the good-old-fashioned DOS/Windows
:: shell, i.e. 'cmd'. This includes calls from tools (e.g. IDEs).
::
:: Calling a PowerShell script from 'cmd' requires calling PowerShell.exe with additional arguments
:: as well as dealing with the handling of "" that is tricky in case of array arguments containing
:: strings with spaces. Using this helper batch, calls become much simpler.
::
:: Use cases include the Visual Studio "Tools > External Tools..." definitions in which case the
:: length of the command as well as arguments are limited to 250 characters. Calling a PowerShell
:: script requires additional arguments (which consume additional characters). In this case this
:: helper batch file also solves this limitation.
::==================================================================================================

SETLOCAL

:: Change to 'TRUE' for verbose execution (e.g. in case of debugging):
SET _verbose='FALSE'

:: Retrieve the first argument which is the PowerShell script to be called:
SET _script=%1
IF %_verbose%=='TRUE' ECHO Script is %_script%

:: Get the length of the first argument to prepare retrieving the subsequent arguments:
SET #=%1
SET _len=0
:StrLenLoop
IF DEFINED # (
    SET #=%#:~1%
    SET /A _len += 1
    IF %_verbose%=='TRUE' (
        ECHO Retrieving script path length... Currently at %_len%...
    )
    GOTO :StrLenLoop
)
IF %_verbose%=='TRUE' ECHO Script path length is %_len%

:: Check whether there are script arguments to forward:
IF "%~2"=="" (
    SET _hasArgs='FALSE'
    IF %_verbose%=='TRUE' ECHO Script has no args
) else (
    SET _hasArgs='TRUE'
    IF %_verbose%=='TRUE' ECHO Script has args
)

:: Compensate for the space between the first and the subsequent arguments:
IF %_hasArgs%=='TRUE' (
    SET /A _len += 1
)

:: Retrieve the subsequent arguments, retrieving all arguments and then skipping the first:
SET _args=%*
CALL SET _args=%%_args:~%_len%%%
:: CALL is required to let the args be correctly expanded!
IF %_verbose%=='TRUE' ECHO Script args are %_args%

:: Convert all quotes within the arguments from '"' to '"""' to ensure that they are properly
:: forwarded to the PowerShell -Command argument string:
SET _args=%_args:"="""%
:: SET must not be placed into an IF %_hasArgs%=='TRUE', for whatever reason!
IF %_verbose%=='TRUE' ECHO PowerShell args are %_args%

:: Also remove potential quotes in the script path (applies if path contains spaces) to ensure
:: that the PowerShell -Command string can properly be composed further below:
SET _script=%_script:"=%

:: Compose options and command to call PowerShell:
SET _opts=-ExecutionPolicy Bypass -NoLogo -NoProfile
IF %_hasArgs%=='FALSE' (
    SET _cmd=PowerShell.exe %_opts% -Command "& {&'%_script%'}"
) else (
    SET _cmd=PowerShell.exe %_opts% -Command "& {&'%_script%' %_args%}"
)
IF %_verbose%=='TRUE' ECHO Invoking PowerShell with...
IF %_verbose%=='TRUE' ECHO %_cmd%

:: Invoke PowerShell executing the script with the given arguments:
%_cmd%
IF %_verbose%=='TRUE' ECHO.
IF %_verbose%=='TRUE' ECHO ...done

ENDLOCAL

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
