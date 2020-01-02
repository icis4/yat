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
:: Copyright © 2003-2020 Matthias Kläy.
:: All rights reserved.
:: ------------------------------------------------------------------------------------------------
:: This source code is licensed under the GNU LGPL.
:: See http://www.gnu.org/licenses/lgpl.html for license details.
::==================================================================================================

SETLOCAL

PUSHD
::--------------------------------------------------------------------------------------------------
SET _targetFormsTool=..\..\YAT.View.Forms\Resources\Tool
SET _targetControlsTool=..\..\YAT.View.Controls\Resources\Tool

DEL %_targetFormsTool%\*.* /F /Q
DEL %_targetControlsTool%\*.* /F /Q
::--------------------------------------------------------------------------------------------------
CD 16x16

ECHO Tool images (16x16) . . .

SET _target=..\%_targetFormsTool%
FORFILES /C "CMD /C COPY @FILE %_target%\Image_Tool_@FNAME_16x16.@EXT"

SET _target=..\%_targetControlsTool%
COPY arrow_refresh_small.png %_target%\Image_Tool_arrow_refresh_small_16x16.png
COPY lightning.png           %_target%\Image_Tool_lightning_16x16.png

CD..
::--------------------------------------------------------------------------------------------------
CD 32x32

ECHO Tool images (32x32) . . .

SET _target=..\%_targetFormsTool%
COPY application_add.png    %_target%\Image_Tool_application_add_32x32.png
COPY control_pause_blue.png %_target%\Image_Tool_control_pause_blue_32x32.png
COPY control_play_blue.png  %_target%\Image_Tool_control_play_blue_32x32.png
COPY control_stop_blue.png  %_target%\Image_Tool_control_stop_blue_32x32.png
COPY help.png               %_target%\Image_Tool_help_32x32.png
COPY information.png        %_target%\Image_Tool_information_32x32.png

CD..
::--------------------------------------------------------------------------------------------------
POPD

ECHO.
ECHO . . . done. Check output above for success/failure.
ECHO.
PAUSE

ENDLOCAL

::==================================================================================================
:: End of
:: $URL$
::==================================================================================================
