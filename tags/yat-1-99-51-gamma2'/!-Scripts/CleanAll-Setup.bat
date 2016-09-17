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

ECHO Cleaning temporary setup directories...
RMDIR /S /Q "YAT.Setup\Debug"
RMDIR /S /Q "YAT.Setup\Release"
RMDIR /S /Q "YAT.Setup\Release with Redistributable"
ECHO ...successfully cleaned

POPD

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
