@ECHO OFF

::==================================================================================================
:: YAT - Yet Another Terminal.
:: Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
:: Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
:: ------------------------------------------------------------------------------------------------
:: $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CleanDir.cmd $
:: $Revision: 1223 $
:: $Date: 2017-01-15 17:07:25 +0100 (So, 15 Jan 2017) $
:: $Author: maettu_this $
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

IF %1=="" GOTO ErrorHandler

::--------------------------------------------------------------------------------------------------
PUSHD %1

ECHO Do something . . .

POPD
GOTO End
::--------------------------------------------------------------------------------------------------

:ErrorHandler
ECHO Parameter 1 must be . . .
GOTO End

:End

::==================================================================================================
:: End of
:: $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CleanDir.cmd $
::==================================================================================================
