﻿@ECHO OFF

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

PUSHD ..

ECHO Cleaning all \bin directories . . .
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "bin"
ECHO ...successfully cleaned

ECHO Cleaning all \obj directories . . .
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "obj"
ECHO ...successfully cleaned

ECHO Cleaning all temporary files in ALAZ source and demos . . .
PUSHD "ALAZ"
FOR /R %%I IN (.) DO CALL "..\!-Scripts\CleanFiles.bat" "%%I" "*.tmp"
POPD
ECHO . . . successfully cleaned

POPD

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
