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

IF %1=="" GOTO NoPath
IF %2=="" GOTO NoDirectory

REM ---
PUSHD %1

ECHO Cleaning %2 in %1
DEL /F /Q %2

POPD
GOTO End
REM ---

:NoPath
ECHO Parameter 1 must be a valid file path (wildcards allowed)
GOTO End

:NoDirectory
ECHO Parameter 2 must be a valid directory name
GOTO End

:End

REM ================================================================================================
REM  End of
REM  $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/MKY/MKY/DelegateEx.cs $
REM ================================================================================================
