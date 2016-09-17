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
::  Copyright © 2003-2016 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

PUSHD ..

ECHO Cleaning solution user options...
DEL /F /Q /A:H "*.suo"
ECHO ...successfully cleaned

ECHO Cleaning additional solution settings...
DEL /F /Q /A:H "*.xml"
ECHO ...successfully cleaned

ECHO Cleaning all project user options...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanFiles.bat" "%%I" "*.csproj.user"
ECHO ...successfully cleaned

POPD

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
