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
:: Author(s): Matthias Klaey
:: ------------------------------------------------------------------------------------------------
:: Copyright � 2003-2018 Matthias Kl�y.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

:: ------------------------------------------------------------------------------------------------
:: This batch is a workaround to start .vspe settings 'As Admin'. Setup and procedure:
:: 1) A shortcut with 'As Admin' enabled refers to this batch.
:: 2) This batch is started via that shortcut with 'As Admin' enabled.
:: 3) This batch starts the according .vspe settings.
::
:: This workaround is necessary because Windows doesn't allow to enable 'As Admin' on a shortcut
:: pointing to a .exe/.com/.bin/.bat/.cmd file.
:: ------------------------------------------------------------------------------------------------

:: 0) Change into the directory where the batch is located:
::    (Note that the executing directory of the batch is %SystemRoot%\System32 when called as admin.)
cd /D %~dp0

:: 1) Retrieve the name of this batch (without extension):
SET NAME=%~n0

:: 2) Invoke the according VSPE settings:
::    (Unfortunately, VSPE doesn't properly start minimized with the /MIN option.)
START "VSPE" /B "%NAME%.vspe"

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================