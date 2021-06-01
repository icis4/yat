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
:: This helper batch refreshes 3rd party files that will be redistributed by extracting them from
:: the original archives, resulting in original file time stamps, resulting in original file time
:: when distributing and installing YAT.
::
:: Still, files are committed to SVN in order to keep solution buildable from SVN. Files are just
:: overwritten with extracted original files.
::==================================================================================================

SETLOCAL

SET _exe=7z.exe

:ProbeExe
WHERE %_exe% >NUL 2>&1
IF NOT %ERRORLEVEL% == 0 GOTO :NoExe

:DejaVu
PUSHD "..\YAT\!-Fonts\DejaVu\"
FOR /F "tokens=* usebackq" %%I IN (`DIR ".\!-Packages\dejavu-fonts-ttf-*" /B /S`) DO ( SET _zipFilePath=%%I )
ECHO.
ECHO DejaVu package is located in %_zipFilePath%
ECHO Extracting infos . . .
SET _iniDir="dejavu-fonts-ttf-*"
SET _outPath=".\"
%_exe% e %_zipFilePath% -o%_outPath% "%_iniDir%\*" -x!"%_iniDir%\*\" -y
ECHO Extracting monospaced fonts . . .
SET _outPath=".\ttf\"
%_exe% e %_zipFilePath% -o%_outPath% "%_iniDir%\ttf\*Mono*" -y
POPD

GOTO :End

:NoExe
ECHO.
ECHO The required 7-Zip executable is not available!
ECHO Make sure 7-Zip is available and has been added to the system's PATH!
ECHO.
PAUSE
GOTO :End

:End

ENDLOCAL

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
