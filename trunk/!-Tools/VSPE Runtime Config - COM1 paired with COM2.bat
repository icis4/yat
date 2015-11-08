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
::  Copyright © 2003-2015 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

:: ------------------------------------------------------------------------------------------------
:: This batch is a work-around to start .vspe settings 'As Admin'. Setup and procedure:
:: 1) A shortcut with 'As Admin' enabled refers to this batch
:: 2) This batch is started via that shortcut with 'As Admin' enabled
:: 3) This batch starts the according .vspe settings
::
:: This work-around is necessary because Windows doesn't allow to enable 'As Admin' on a shortcut
:: pointing to a .exe/.com/.bin/.bat file.
:: ------------------------------------------------------------------------------------------------

:: 0) Change into the directory where the batch is located
::    Note that the executing directory of the batch is %SystemRoot%\System32 when called as admin
cd /D %~dp0

:: 1) Retrieve the name of this batch (without extension)
SET name=%~n0

:: 2) Invoke the according VSPE settings
::    Unfortunately, VSPE doesn't properly start minimized with the /MIN option
START "VSPE" /B "%name%.vspe"

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
