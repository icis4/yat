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
::  Copyright © 2003-2017 Matthias Kläy.
::  All rights reserved.
::  -----------------------------------------------------------------------------------------------
::  This source code is licensed under the GNU LGPL.
::  See http:REMwww.gnu.org/licenses/lgpl.html for license details.
:: =================================================================================================

:: -------------------------------------------------------------------------------------------------
PUSHD

:: -------------------------------------------------------------------------------------------------
SET TARGET_FORM_TOOL=..\..\YAT.View.Forms\Resources\Tool
SET TARGET_CTRL_TOOL=..\..\YAT.View.Controls\Resources\Tool

DEL %TARGET_FORM_TOOL%\*.* /F /Q
DEL %TARGET_CTRL_TOOL%\*.* /F /Q
:: -------------------------------------------------------------------------------------------------
CD 16x16

ECHO Tool images (16x16)...

SET TARGET=..\%TARGET_FORM_TOOL%
FORFILES /C "CMD /C COPY @FILE %TARGET%\Image_Tool_@FNAME_16x16.@EXT"

SET TARGET=..\%TARGET_CTRL_TOOL%
COPY lightning.png           %TARGET%\Image_Tool_lightning_16x16.png
COPY arrow_refresh_small.png %TARGET%\Image_Tool_arrow_refresh_small_16x16.png

CD..
:: -------------------------------------------------------------------------------------------------
CD 32x32

ECHO Tool images (32x32)...

SET TARGET=..\%TARGET_FORM_TOOL%
COPY application_add.png %TARGET%\Image_Tool_application_add_32x32.png
COPY help.png            %TARGET%\Image_Tool_help_32x32.png
COPY information.png     %TARGET%\Image_Tool_information_32x32.png

CD..

POPD
GOTO End
:: -------------------------------------------------------------------------------------------------

:End

:: =================================================================================================
::  End of
::  $URL$
:: =================================================================================================
