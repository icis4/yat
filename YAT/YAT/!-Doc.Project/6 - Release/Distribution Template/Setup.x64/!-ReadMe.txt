
****************************************************************************************************
                                     YAT Installation ReadMe.
 --------------------------------------------------------------------------------------------------
                                    YAT - Yet Another Terminal.
     Engineering, testing and debugging of serial communications. Supports RS-232/422/423/485...
   ...as well as TCP/IP Client/Server/AutoSocket, UDP/IP Client/Server/PairSocket and USB Ser/HID.
 --------------------------------------------------------------------------------------------------
                    Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
                     Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
 --------------------------------------------------------------------------------------------------
                    Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
                                Copyright © 2003-2021 Matthias Kläy.
                                        All rights reserved.
 --------------------------------------------------------------------------------------------------
                                YAT is licensed under the GNU LGPL.
                   See http://www.gnu.org/licenses/lgpl.html for license details.
****************************************************************************************************


====================================================================================================
1. Installation
====================================================================================================

It is recommended to unzip the package to a temporary location before starting the installation.

YAT uses .NET 4.8. The installer packages ensure that .NET 4.8 is available on the target computer.
The installer packages also ensure that Windows Installer 4.5 is available on the target computer.

For installation, run the ".msi" if Windows Installer is installed, otherwise "setup.exe".
 1. Installer will check the prerequisites mentioned above and install what is missing.
 2. Installer will install YAT. Older versions of YAT are automatically replaced.

You can also download .NET and/or Windows Installer from <https://www.microsoft.com/download>
or by googling for "Download Microsoft .NET Framework 4.8" and/or "Windows Installer 4.5".
Installing .NET and/or Windows Installer requires administrator permissions.


x86 (32-bit) -vs- x64 (64-bit)
----------------------------------------------------------------------------------------------------

YAT can be installed as x86 or x64 application. x86 works on either 32-bit or 64-bit systems, given
a 64-bit system provides 32-bit compatibility. x64 also works on either 32-bit or 64-bit systems, as
YAT x64 is built as 'Any CPU' for providing compatibility with MSIL projects. By default, x86 is
installed to "\Program Files (x86)" whereas x64 is installed to "\Program Files".

It is not possible to install both distributions for the same user. When changing from x86 to x64 of
the same version of YAT, or vice versa, the installed distribution must first be uninstalled before
the other distribution can be installed. If this limitation is not acceptable, create a new feature
request ticket and describe the impacts/rationale/use case as detailed as possible. Or use binary
distributions, which may exist in parallel without restrictions.


====================================================================================================
2. Execution
====================================================================================================

"Start > Programs > YAT > YAT" or
start "C:\<Program Files>\YAT\YAT.exe" for normal execution.
Start "C:\<Program Files>\YAT\YATConsole.exe" for console execution.

In normal execution, infos/warnings/errors are shown on the user interface, whereas in console
execution, notifications are returned via stdout/stderr and there will be no blocking modal dialogs.


****************************************************************************************************
                                   End of YAT Installation ReadMe.
****************************************************************************************************
