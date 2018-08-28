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
:: Copyright © 2003-2018 Matthias Kläy.
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

:: Change to 'TRUE' for verbose execution (e.g. in case of debugging):
SET verbose='FALSE'

:: Retrieve the first argument which is the PowerShell script to be called:
SET script=%1
IF %verbose%=='TRUE' ECHO Script is %script%

:: Check whether there are script arguments to forward:
if %2.==. (
    SET hasArgs='FALSE'
    IF %verbose%=='TRUE' ECHO Script has no args
) else (
    SET hasArgs='TRUE'

    :: Get the length of the first argument to prepare retrieving the remaining arguments:
    SET #=%1
    SET len=0
    :StrLenLoop
    IF DEFINED # (
        SET #=%#:~1%
        SET /A len += 1
        IF %verbose%=='TRUE' (
            ECHO Retrieving script path length... Currently %len%...
        )
        GOTO :StrLenLoop
    )
    IF %verbose%=='TRUE' ECHO Script path length is %len%

    :: Compensate for the space between the first and the remaining arguments:
    SET /A len += 1

    :: Retrieve the remaining arguments, retrieving all arguments and then skipping the first:
    SET args=%*
    CALL SET args=%%args:~%len%%%
    IF %verbose%=='TRUE' ECHO Script args are %args%

    :: Convert all quotes within the arguments from '"' to '"""' to ensure that they are properly
    :: forwarded to the PowerShell -Command argument string:
    SET args=%args:"="""%
    IF %verbose%=='TRUE' ECHO PowerShell args are %args%
)

:: Compose options and command to call PowerShell:
SET opts=-ExecutionPolicy Bypass -NoLogo -NoProfile
IF %hasArgs%=='FALSE' (
    SET cmd=PowerShell.exe %opts% -Command "& {%script%}"
) else (
    SET cmd=PowerShell.exe %opts% -Command "& {%script% %args%}"
)
IF %verbose%=='TRUE' ECHO Invoking PowerShell...
IF %verbose%=='TRUE' ECHO %cmd%

:: Invoke PowerShell executing the script with the given arguments:
%cmd%
IF %verbose%=='TRUE' ECHO.
IF %verbose%=='TRUE' ECHO ...done

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
