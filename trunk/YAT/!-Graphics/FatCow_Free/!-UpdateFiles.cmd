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
:: Copyright © 2003-2018 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

PUSHD
::--------------------------------------------------------------------------------------------------
SET TARGET_FORM_TOOL=..\..\YAT.View.Forms\Resources\Tool
SET TARGET_CTRL_TOOL=..\..\YAT.View.Controls\Resources\Tool

DEL %TARGET_FORM_TOOL%\*.* /F /Q
DEL %TARGET_CTRL_TOOL%\*.* /F /Q
::--------------------------------------------------------------------------------------------------
CD 16x16

ECHO Tool images (16x16) . . .

SET TARGET=..\%TARGET_FORM_TOOL%
FORFILES /C "CMD /C COPY @FILE %TARGET%\Image_Tool_@FNAME_16x16.@EXT"

SET TARGET=..\%TARGET_CTRL_TOOL%
COPY lightning.png           %TARGET%\Image_Tool_lightning_16x16.png
COPY arrow_refresh_small.png %TARGET%\Image_Tool_arrow_refresh_small_16x16.png

CD..
::--------------------------------------------------------------------------------------------------
CD 32x32

ECHO Tool images (32x32) . . .

SET TARGET=..\%TARGET_FORM_TOOL%
COPY application_add.png    %TARGET%\Image_Tool_application_add_32x32.png
COPY control_pause_blue.png %TARGET%\Image_Tool_control_pause_blue_32x32.png
COPY control_play_blue.png  %TARGET%\Image_Tool_control_play_blue_32x32.png
COPY control_stop_blue.png  %TARGET%\Image_Tool_control_stop_blue_32x32.png
COPY help.png               %TARGET%\Image_Tool_help_32x32.png
COPY information.png        %TARGET%\Image_Tool_information_32x32.png

CD..
::--------------------------------------------------------------------------------------------------
POPD

ECHO.
ECHO . . . done. Check output above for success/failure.
ECHO.
PAUSE

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
