@ECHO OFF

REM ================================================================================================
REM  YAT - Yet Another Terminal.
REM  Visit YAT at http:REMsourceforge.net/projects/y-a-terminal/.
REM  Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
REM  ----------------------------------------------------------------------------------------------
REM  $URL: https:REMsvn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY/DelegateEx.cs $
REM  $Author: maettu_this $
REM  $Date: 2013-02-03 20:59:28 +0100 (So, 03 Feb 2013) $
REM  $Revision: 628 $
REM  ----------------------------------------------------------------------------------------------
REM  MKY Development Version 1.0.10
REM  ----------------------------------------------------------------------------------------------
REM  See SVN change log for revision details.
REM  See release notes for product version details.
REM  ----------------------------------------------------------------------------------------------
REM  Copyright © 2010-2013 Matthias Kläy.
REM  All rights reserved.
REM  ----------------------------------------------------------------------------------------------
REM  This source code is licensed under the GNU LGPL.
REM  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
REM ================================================================================================

PUSHD ..

ECHO Cleaning all \bin directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "bin"
ECHO ...successfully cleaned

ECHO Cleaning all \obj directories...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanDir.bat" "%%I" "obj"
ECHO ...successfully cleaned

ECHO Cleaning all temporary files in ALAZ source and demos...
PUSHD "ALAZ"
FOR /R %%I IN (.) DO CALL "..\!-Scripts\CleanFiles.bat" "%%I" "*.tmp"
POPD
ECHO ...successfully cleaned

POPD

REM ================================================================================================
REM  End of
REM  $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY/DelegateEx.cs $
REM ================================================================================================
