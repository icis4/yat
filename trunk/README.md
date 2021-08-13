```
****************************************************************************************************
                                       YAT Project ReadMe.
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
```
[1]: https://sourceforge.net/projects/y-a-terminal/
[2]: http://www.gnu.org/licenses/lgpl.html


Download
====================================================================================================

The standard x64 installer package can be downloaded from [YAT's SourceForge main page](https://sourceforge.net/projects/y-a-terminal/).
The complete set of packages can be downloaded from [YAT's SourceForge file page](https://sourceforge.net/projects/y-a-terminal/files/):

* x64 installer
* x64 installer that includes the .NET runtime
* x64 binaries
* x86 installer
* x86 installer that includes the .NET runtime
* x86 binaries

How to chose the most appropriate package:

* For up-to-date systems, use a compact installer package "...(32-bit).zip" or "...(64-bit).zip".
  (Windows Installer and .NET are already installed on up-to-date systems.)
* For outdated systems or offline installation, use a full installer package "..._with_.NET...zip".
  (Windows Installer and .NET are included for installation.)
* Alternatively, use a binary distribution, but don't forget to manually install the monospaced
  'DejaVu' font used by YAT as well as assign the .yat/.yaw file extensions to "YAT.exe".


Installation
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

For use of a binary distribution, refer to the respective instructions.


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


Execution
====================================================================================================

"Start > Programs > YAT > YAT" or
start "C:\<Program Files>\YAT\YAT.exe" for normal execution.
Start "C:\<Program Files>\YAT\YATConsole.exe" for console execution.

In normal execution, infos/warnings/errors are shown on the user interface, whereas in console
execution, notifications are returned via stdout/stderr and there will be no blocking modal dialogs.


History of Changes
====================================================================================================

Refer to the [YAT Release Notes](https://sourceforge.net/projects/y-a-terminal/files/).


Roadmap
====================================================================================================

YAT 2
----------------------------------------------------------------------------------------------------
YAT 2 is in maintenance mode now, i.e. focus on bug fixes and minor changes, while work for YAT 4
is already ongoing.


(Version 3 will be skipped to prevent naming conflict with yat3 of Dieter Fauth that became
public around the same time as YAT. And, 4.0 buzzes more anyway (industry 4.0 and the like ;-))


YAT 4 with Scripting :: Expected in 2022
----------------------------------------------------------------------------------------------------
YAT 4.0 will feature the integration of a scripting environment, based on the CSScript engine.
Scripting will allow you to script YAT and automate repetitive tasks, use it for test automation,
implement protocol layers,... whatever you can think of. Examples and templates will be included.

It is also planned to demonstrate how to use YAT from a PowerShell script and along with NUnit.
All these features aim for providing a versatile automatic testing tool for serial communications.


Legal
====================================================================================================

Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
Copyright © 2003-2021 Matthias Kläy.
All rights reserved.

YAT is licensed under the [GNU LGPL](http://www.gnu.org/licenses/lgpl.html).

----------------------------------------------------------------------------------------------------

```
****************************************************************************************************
                                     End of YAT Project ReadMe.
****************************************************************************************************
```
