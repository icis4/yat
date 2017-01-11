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
::  See SVN change log for revision details.
::  See release notes for product version details.
::  -----------------------------------------------------------------------------------------------
::  Copyright © 2003-2017 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

PUSHD ..

ECHO Cleaning all \bin directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "bin"
ECHO ...successfully cleaned

ECHO Cleaning all \obj directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "obj"
ECHO ...successfully cleaned

ECHO Cleaning all temporary files in ALAZ source and demos...
PUSHD "ALAZ"
FOR /R %%I IN (.) DO CALL "..\!-Scripts\CleanFiles.bat" "%%I" "*.tmp"
POPD
ECHO ...successfully cleaned

POPD

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
