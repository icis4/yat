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
:: Copyright � 2003-2021 Matthias Kl�y.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

:: Setup:
SET _source=.\Distribution Template\Setup.x64\!-ReadMe.txt
SET _target=.\Distribution Template\Setup.x64 with Prerequisites\
COPY /Y "%_source%" "%_target%"
SET _target=.\Distribution Template\Setup.x86\
COPY /Y "%_source%" "%_target%"
SET _target=.\Distribution Template\Setup.x86 with Prerequisites\
COPY /Y "%_source%" "%_target%"

:: Binaries:
SET _source=.\Distribution Template\x64 Binaries\!-ReadMe.txt
SET _target=.\Distribution Template\x86 Binaries\
COPY /Y "%_source%" "%_target%"

::==================================================================================================
:: End of
:: $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CleanAll.cmd $
::==================================================================================================
