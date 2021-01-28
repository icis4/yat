@ECHO OFF

::==================================================================================================
:: YAT - Yet Another Terminal.
:: Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
:: Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
:: ------------------------------------------------------------------------------------------------
:: $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CleanAll.cmd $
:: $Revision: 3584 $
:: $Date: 2021-01-04 16:40:12 +0100 (Mo., 04 Jan 2021) $
:: $Author: maettu_this $
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

:: Fonts:
SET _source=..\..\..\!-Fonts\DejaVu\ttf\DejaVuSansMono*.*
SET _target=.\x64 Binaries\DejaVu Fonts\
COPY /Y "%_source%" "%_target%"
SET _target=.\x86 Binaries\DejaVu Fonts\
COPY /Y "%_source%" "%_target%"

::==================================================================================================
:: End of
:: $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CleanAll.cmd $
::==================================================================================================
