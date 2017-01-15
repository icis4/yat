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
:: Copyright © 2003-2017 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

PUSHD ..

ECHO Cleaning temporary setup directories . . .
RMDIR /S /Q "YAT.Setup\Debug"
RMDIR /S /Q "YAT.Setup\Release"
RMDIR /S /Q "YAT.Setup\Release with Redistributable"
ECHO . . . successfully cleaned

POPD

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
