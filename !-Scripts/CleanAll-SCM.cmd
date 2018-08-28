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
::
::     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
::     Attention: Windows/DOS requires that this file is encoded in ASCII/ANSI and not UTF-8!
::     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
::
:: Author(s): Matthias Klaey
:: ------------------------------------------------------------------------------------------------
:: Copyright � 2003-2018 Matthias Kl�y.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

PUSHD ..

ECHO Cleaning all \.svn directories . . .
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.cmd" "%%I" ".svn"
ECHO ...successfully cleaned

ECHO Cleaning all \.git directories . . .
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.cmd" "%%I" ".git"
ECHO . . . successfully cleaned

ECHO Cleaning Ankh.Load...
DEL /F /Q /A:H "Ankh.Load"
ECHO . . . successfully cleaned

POPD

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
