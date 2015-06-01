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
REM  Copyright © 2003-2015 Matthias Kläy.
REM  All rights reserved.
REM  ----------------------------------------------------------------------------------------------
REM  This source code is licensed under the GNU LGPL.
REM  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
REM ================================================================================================

PUSHD ..

ECHO Cleaning solution user options...
DEL /F /Q /A:H "*.suo"
ECHO ...successfully cleaned

ECHO Cleaning additional solution settings...
DEL /F /Q /A:H "*.xml"
ECHO ...successfully cleaned

ECHO Cleaning all project user options...
FOR /R %%I IN (.) DO CALL "!-Scripts\CleanFiles.bat" "%%I" "*.csproj.user"
ECHO ...successfully cleaned

POPD

REM ================================================================================================
REM  End of
REM  $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY/DelegateEx.cs $
REM ================================================================================================
