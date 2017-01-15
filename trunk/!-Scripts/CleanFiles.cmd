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

IF %1=="" GOTO NoPath
IF %2=="" GOTO NoDirectory

:: -------------------------------------------------------------------------------------------------
PUSHD %1

ECHO Cleaning %2 in %1
DEL /F /Q %2

POPD
GOTO End
:: -------------------------------------------------------------------------------------------------

:NoPath
ECHO Parameter 1 must be a valid file path (wildcards allowed)
GOTO End

:NoDirectory
ECHO Parameter 2 must be a valid directory name
GOTO End

:End

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
