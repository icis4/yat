
****************************************************************************************************
                                         YAT Release Notes.
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


Contents
========

1. Installation
2. Execution
3. History of changes in YAT
4. History of changes in XTerm232
5. Roadmap
6. Legal


====================================================================================================
1. Installation
====================================================================================================

YAT uses .NET 4.8. The YAT installer ensures that .NET 4.8 is available on the target computer.
The installer also ensures that Windows Installer 5.0 is available on the target computer.

First, chose the most appropriate package:
 > For up-to-date systems, use the compact package "...(32-bit).zip" or "...(64-bit).zip".
   (Windows Installer and .NET are already installed on up-to-date systems.)
 > For outdated systems or offline installation, use a full package "..._with_.NET...zip".
   (Windows Installer and .NET are included for installation.)
 > Alternatively, use a binary distribution, but don't forget to manually install the monospaced
   'DejaVu' font used by YAT as well as assign the .yat/.yaw file extensions to YAT.exe.

It is recommended to unzip this package to a temporary location before starting the installation.

Run the ".msi" if Windows Installer is installed, otherwise "setup.exe".
 1. Installer will check prerequisites and install what is missing.
 2. Installer will install YAT. Older versions of YAT are automatically replaced.

For installation of a binary distribution, refer to the instructions inside that package.

You can also download .NET and/or Windows Installer from <https://www.microsoft.com/download>
or by googling for "Download Microsoft .NET Framework 4.8" and/or "Windows Installer 5.0".
Installing .NET and/or Windows Installer requires administrator permissions.


x86 (32-bit) -vs- x64 (64-bit)
----------------------------------------------------------------------------------------------------

YAT can be installed as x86 or x64 application. x86 works on either 32-bit or 64-bit systems whereas
x64 can only be installed on 64-bit systems. By default, x86 is installed to "\Program Files (x86)"
whereas x64 is installed to "\Program Files".

It is not possible to install both distributions for the same user. When changing from x86 to x64 of
the same version of YAT, or vice versa, the installed distribution must first be uninstalled before
the other distribution can be installed. If this limitation is not acceptable for somebody, create a
new feature request ticket and describe the impacts/rationale/use case as detailed as possible.


====================================================================================================
2. Execution
====================================================================================================

Run YAT by selecting "Start > Programs > YAT > YAT".

Use "C:\<Program Files>\YAT\YAT.exe" to run YAT normally.
Use "C:\<Program Files>\YAT\YATConsole.exe" to run YAT from console.


====================================================================================================
3. History of Changes in YAT
====================================================================================================

YAT 2.4.0 :: 2021-0x-xx
----------------------------------------------------------------------------------------------------

New:
- USB Ser/HID now takes the HID usage page and ID into account, allowing working with devices that
  implement multiple usages (e.g. a standard interface and a firmware upgrade interface) (bug #480).
- Support for concurrent sending, i.e. multiple commands can be active simultaneously (feat. #387).
  Useful e.g. for sending a text command while a repeating text command is already ongoing.
  Can be enabled at [Terminal > Settings... > Advanced... > Send > Allow concurrent sending].
- Text terminals: Option to glue characters of a line together, enabled by default, resulting in
  fewer line breaks when sending and receiving in full-duplex (related to feat. #19 and bug #176).
- Text terminals: Option to wait for response, resulting in request and response being displayed
  adjacently. Useful for sending multi-line commands and files with ping-pong-style command sets
  (feature request #19 and bug #176).
  Note for serial COM ports: In order to only limit the send rate, YAT offers other settings, e.g.
  [Terminal > Settings... > Advanced... > Send > Buffer not more than baud rate permits] which
  limits the send rate and lets an active hardware or software flow control do its job even for
  small buffers and large amount of data.
- Content separator can now be configured. Useful for e.g. displaying or logging hex data without
  separating spaces, e.g. "414243", using [None]. Separator format can now also be configured.
- Additional keyword \!(TimeStamp) allowing injection of current date/time (feature request #400).
  Format according to [View > Format Settings... > Options > Time Stamp].
  Useful for e.g. sending "AT+DATE=20 01 01 12 00 00".
- Additional keyword \!(Port()) allowing to change the serial COM port on-the-fly (feat. req. #403).
  Especially useful as predefined command e.g. "\!(Port(10))\!(NoEOL())" to change port by shortcut.
- Additional keywords \!(RtsOn|Off|Toggle) and \!(DtrOn|Off|Toggle) allowing to change serial COM
  port signals on-the-fly (feature request #408). Also work with predefined commands and shortcuts.
- Additional option to disable warnings like "XOff state, retaining data..." on sending.
- Automatic actions and responses now support multiple triggers within a line or chunk.
- Automatic actions and responses now support text triggers, optionally incl. regular expression.
- Automatic actions and responses now list recent used trigger and response texts.
- Automatic responses now support injection of trigger captures, given regular expression is used.
- New automatic actions [Plot Byte Count/Rate] and [Plot Line Count/Rate].
- New automatic actions [Line Chart], [Scatter Plot] and [Histogram].

Important changes:
- High FTDI and Prolific baud rates added to list of standard baud rates (feature request #398).
- Using term "I/O" instead of "Port" for terminal, settings,... for reducing mix-up of term in YAT
  with TCP and UDP "Port" terminology, where a "Port" only is a part of the overall "I/O" subsystem.
- Text terminals: Now by default using UTF-8 instead of .NET's [Encoding.Default] that is limited to
  an ANSI code page, e.g. Windows-1252 in Western Europe (related to former bugs #427, #428, #429).
- Sending refactored (precondition for feature requests #19, #333, #387 and fix of bug #176).
- Text terminals: Line content and EOL is no longer sent in two separate chunks (feature req. #333).
- Element and line processing refactored (precondition for feature requests #19, #366, #367 and fix
  of bugs #176, #371, #477, #478 as well as preparing upcoming feature request #74).
- Unidirectional Tx and Rx panels now use a separate line detection mechanism and no longer break
  lines at the same location as the bidirectional panel (bugs #371 and #477).
- [Monitor Context Menu > Clear] no longer clears all panels (Tx/Bidir/Rx), just the selected. Use
  [Main Menu > Terminal > Clear [All]] or [Ctrl+L] to clear all panels (related to change above).
  [Main Menu > Terminal > All Terminals Clear] or [Ctrl+E] to clear all terminals (feat. req. #392).
- User saved workspace file name is indicated in title bar (feature request #416).
- Binary terminals: Lines are no longer broken each 16 bytes by default, they now by default are
  broken on every chunk (related to bug #477).
- Consequently, chunk line break settings are now located in the text/binary specific dialog.
- Comma and semicolon added to list of predefined EOL sequences (feature request #409).
- Option to hide undefined predefined commands (feature request #410).
- Option to [Copy Text / File Path to Clipboard] (bug #493).
- Possibility to clear lists of recent files and [Send Text|File] commands.
- Adaptive monitor update rate further improved.
- Calculation of byte/line rates improved.
- For Bin/Oct/Dec/Hex/Unicode radix:
   > Performance significanty improved (feature request #406).
   > ASCII control characters are no longer converted to mnemonics.
- Option to not send XOn when opending a serial COM port or USB Ser/HID terminal (feat. req. #393).
- TCP/IP connection state management improved in terms of stress resilience.
- TCP/IP segment payload size limited to the safe maximum of 516 (IPv4) or 1220 octets (IPv6).
- UDP/IP datagram payload size limited to the safe maximum of 508 (IPv4) or 1212 octets (IPv6).
- IPv6 support enabled for UDP/IP terminals (feature request #372).
- Changes on-the-fly by keywords like \!(Port()), \!(PortSettings()), \!(Baud()),... are now
  reflected in the terminal settings, i.e. also indicated by '*' (related to reqs. #71 and #403).
- [Terminal > Settings... > Advanced...] dialog rearranged for better fitting screen.
- Upgrade to .NET 4 Runtime and .NET 4.8 Framework (part of feature request #229, precondition for
  new automatic actions [Chart/Plot/Histogram] and upcoming feature request #74 [Scripting]).
  Consequently, x64 distributions no longer need to be 'AnyCPU' builds (former limitation).
- Improved error message in case the required version of .NET is missing.
- Additional command line option [--version]. Better command line message box (feature req. #383).
- Project/Assembly structure slightly refined (preparing upcoming feature request #74).
- Test coverage of sending and processing significantly increased (related to refactorings above).

Fixed bugs:
- Predefined command description gets updated when multi-line command gets changed (bug #481) and
  default description no longer gets saved in settings file.
- Predefined command description is kept when filled-in before command (bugs #472, #476, #499).
- Issue with not shown predefined commands when defining more than 12 predefined commands fixed.
- Layout of predefined commands can now also be changed on a new terminal that doesn't contain
  commands yet (related to previous feature requests #28, #257, #365).
- Import/Paste of a .yacp command page file that contains more predefined commands than currently
  configured now behaves correctly (bug #479, related to previous feature requests #28, #257, #365).
- Explicit default radix is no longer reset to [String] when multiple predefined command pages are
  being defined (bug #492).
- Recent TCP/IP and UDP/IP ports, remote hosts and local filters are remembered again (related to
  former feature request #273).
- TCP/IP client terminals now also try to automatically reconnect within configured timeout even in
  case the server resets an initial SYN and times out a subsequent SYN request (bug #487).
- Automatic actions and responses now work chunk independently (feature request #366).
- Automatic actions [Filter] and [Suppress] now also work for continuous data (bug #478) and on
  refresh (feature request #367).
- The time information of a line is no longer defined by hidden bytes like e.g. an initial XOn.
- Line break detection and processing continues even when exceeding the configured maximum number
  of characters/bytes per line (related to refactoring of element and line processing).
- Sending of very long lines now also works for TCP/IP based terminals (bug #340).
- Sending of very long lines in other terminal types (in case of e.g. serial COM ports longer than
  the software buffer, typically 2048 bytes) works again (bug #417).
- Handling of [Preferences... > ...take serial number/string into account] fixed (rel. to bug #480).
- Consequence of .NET upgrade (former limitations of .NET Runtime 2 and .NET Framework 3.5):
   > System display scaling other than 100% (96 DPI) no longer result in minor distortions on Win 7
     (bugs #85, #235, #375) nor some blurring on Win 8 and above (feature request #310).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port should
     no longer result in an 'ObjectDisposedException' or 'UnauthorizedAccessException' (bugs #224,
     #254, #293, #316, #317, #345, #382, #385, #387, #401, #442) nor should no longer result in
     deadlocks, though not all potential causes for this issue in the .NET 'SerialPort' class can
     be verified.
   > Running YAT for a long period, or creating many terminals, no longer results in memory leaks,
     previously resulting in gradual increase of memory consumption (RAM) (bugs #243, #263, #336).

Limitations and known issues:
- General limitations of .NET Framework:
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- General limitations of .NET Windows.Forms:
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Main window status bar tooltips may flicker in case window is maximized (bug #488).
   > Tool strip combo box slightly flickers when updating item list, e.g. [Find Pattern] (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or [Find] items (bug #347).
   > When [Send Text] or [Send File] is hidden, resizing the panel doesn't properly work (bug #412).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
   > Link label text rendering issues after plotting (bug #485).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port may lead
     to a deadlock. It happens due to a bug in the .NET 'SerialPort' class for which Microsoft has
     no plans fixing. To prevent this issue, refrain from disconnecting a device or undocking or
     hibernating while a port is open.
- The \!(PortSettings()) keyword is yet limited to serial COM ports (feature request #71).
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Automatic actions [Filter] and [Suppress] as well as automatic actions and responses based on a
  text trigger are constrained to handle complete lines. Thus, individual characters incl. control
  characters like <XOn> as well as incomplete lines will not be displayed until line is complete.
- Switching log off may take several seconds, during which YAT is unresponsive (bug #459).


(Versions 2.2 and 2.3 have been skipped to emphasize the update to .NET 4.x while still keeping the
option for releasing the 2.4+ features (except plotting) also on .NET 2.0 Runtime / 3.5 Framework.)


YAT 2.1.0 :: 2019-10-04
----------------------------------------------------------------------------------------------------

New:
- Predefined commands can now be reordered (feature requests #376 and #379), cut/copy/pasted via
  clipboard, exported/imported to a single page .yacp or complete set of pages .yacps file (feature
  reqs. #28, #257, #365), and even linked to one or multiple single page .yacp file(s) (feat. #29).
- Option to show more predefined commands (24, 36, 48, 72, 108) (feature requests #256, #344).
- Additional keyword \!(PortSettings()) that allows re-configuring the port settings before or after
  sending data; and dedicated keywords \!(Baud()), \!(DataBits()), \!(Parity()), \!(StopBits()) and
  \!(FlowControl()) to change a dedicated port setting on-the-fly (feature request #71, partly
  related to former request #321 implemented since YAT 1.99.80). Useful for e.g. sending commands
  that let a connected device change its baud rate. See [Help > Contents...] for example usage.
- Additional automatic actions [Filter] and [Suppress] added (feature requests #347 and #382).
  Useful to exclusively display a pattern in the monitor, or suppress a pattern.
- Additional automatic action [Clear Monitor on Subsequent Receiving] added (feature request #357).
  Useful for screen/page synchronization on terminal-emulation-like behavior.
- Additional option [View > Show Duration (Line)] (feature request #348).
- Additional terminal option [Include Port Control Events] (feature request #350).
- Additional log channel [Log Port Control] (related to feature request #350).
- Text terminals: Additional options to configure line break:
   > Length line break (feature request #224).
     Useful to limit the number of characters displayed per line, i.e. "word wrap".
     Useful to communicate with devices that do use text messages but no EOL sequence.
   > Timed line break (feature request #340).
     Also useful to communicate with devices that do use text messages but no EOL sequence.
- Text terminals: Additional option to let bell (0x07) beep (related to feature request #308).
- Text terminals: Backspaces (0x08) are treated as backspaces by default (feature request #308).
                  Note that tabs have already been treated as tabs by default since YAT 1.99.22.

Important changes:
- UDP/IP now supports sending to and receiving from broadcast addresses (feature request #370).
- Serial COM port again uses the term RTS (Ready To Send) as that is still most commonly used.
  In addition, notes/hints on terms RTR (Ready To Receive) as well as RFR (Ready For Receiving).
- Serial COM port and USB Ser/HID terminals by default hide XOn/XOff flow control characters for
  'Software' and 'Combined' (related to earlier feature request #190, #206, #226 and bug #319).
  Note that 'Manual Software' and 'Manual Combined' still show the XOn/XOff characters by default
  but can optionally be hidden (earlier feature request #190).
- USB Ser/HID device list no longer potentially shows weird manufacturer, product or serial strings;
  neither does it show duplicated devices anymore.
- Errors are now indicated in square "[...]" instead of angle "<...>" brackets (rel. to feat. #350).
- Prevention of potential handling error when copying data to clipboard (feature request #345).
- Unicode/Non-Unicode multi-byte encoding test coverage improved (related to bugs #427, #428, #429).
- No longer using [Ctrl+Alt+<CharOrDigit>] shortcuts to avoid conflicts with [Alt Gr] modifier as
  [Ctrl+Alt] results in [Alt Gr] on Windows (feature request #359). As a consequence:
   > Shortcuts to activate predefined command page changed from [Ctrl+Alt+1..9] to [Ctrl+1..9].
   > Shortcuts to change window layout changed from [Ctrl+Alt+...] to [Alt+Shift+...].
   > Shortcuts to [Find Next] changed from [Ctrl+Alt+F] to [Alt+Shift+N].
          and [Find Previous] changed from [Alt+Shift+F] to [Alt+Shift+P].
- Shortcuts to select panels no longer conflict with those to select menus (feature request #355).
   > Shortcut of [Send Text] changed to [Alt+E] to distinguish from shortcut of [Terminal] menu.
   > Shortcut of [Send File] changed to [Alt+I] to distinguish from shortcut of [File] menu.
   > Shortcut to [Monitor] added as [Alt+M].
- Text terminals: Refactoring and refinement of EOL handling (related to feature requests #224,
  #340 and #347). Any sequence and mix of received and sent characters (even quite exotic ones)
  should now be correctly handled and displayed. Corresponding test coverage enlarged.
- Text terminals: Option to show length as number of characters (related to feature request #224).

Fixed bugs:
- Non-Unicode multi-byte encodings (e.g. GBK, GB18030, Shift-JIS,...) fixed (bugs #427, #428, #429).
- 'OverflowException' in case of missing closing parentheses in [Send Text] fixed (bug #426).
- Exception when trying to open terminal on non-standard port (e.g. "COM", "ABC") fixed (bug #416).
- Handling of potential 'ObjectDisposedException' when disconnecting USB/RS-232 converters (USB CDC)
  without closing the serial COM port now also handles potential 'UnauthorizedAccessException'
  (bug #442, related to former bugs #224, #254, #293, #316, #317, #345, #382, #385, #387, #401).
- [Remote Host] and [Local Filter] selection again show the common items (rel. to feat. req. #370)).
- [Send > Skip Empty Line on [Send File]] no longer accidentally restricts [Send Text].
- Problem with escape sequences in automatic action/response fixed (bug #424).
- Tool bar items no more flicker on automatic action/response trigger (related to bug #424).
- 'ArgumentOutOfRangeException' when deleting a sole predefined command page fixed (bug #440).
- 'SplitterDistance' value related 'ArgumentException' fixed (bugs #414, #418, #419, #420, #421,
  #422, #423, #432, #434, #435, #441, #443, #444, #450, #452, #453, #456, #461, #467, #468, #474,
  #475) which further refines the fix to former bugs #408 and #409.
- Attempted to fix rare 'TargetInvocationException' caused by 'IndexOutOfRangeException' (bug #446).
- [View > Show Time Span] is no longer reset when changing terminal settings (bug #436).
- [View > Show Time Span/Delta] are no longer swapped (bugs #431, #449, #458).
- [Advanced Settings > Disable escapes on [Send Text]] no longer leads to an 'ArgumentException'
  in some cases (bug #454).
- Shortcuts [Ctrl+Shift+F1..F12] 'Copy to Send Text' fixed.
- Option to set default radix for predefined commands fixed.
- Command line option [TransmitText] again allows keywords (related to bug #454).
- Optional user name of terminal is again taken into account (bug #415).
- [Help > Release Notes] fixed for YATConsole.exe (bug #413).
- Binary terminals: Synchronization on timed line break added (related to feature request #340).

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General limitations of .NET Framework:
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- General limitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. [Find Pattern] (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or [Find] items (bug #347).
   > When [Send Text] or [Send File] is hidden, resizing the panel doesn't properly work (bug #412).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port may lead
     to a deadlock or 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401, #442). To prevent this issue, refrain from disconnecting a
     device while its port is open. Or, manually close the port after the device got disconnected.
- The \!(PortSettings()) keyword is yet limited to serial COM ports (feature request #71).
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Line content and EOL may be sent in two separate chunks, because the parts are handled slightly
  after each other. Delay could be eliminated but requires some refactoring (feature request #333).
- Automatic responses and actions work fine as long as the received chunks do not contain more than
  one trigger and do not spread across multiple lines. Limitation could be eliminated but requires
  some refactoring (feature request #366).
- Automatic actions [Filter] and [Suppress] do not get reapplied on refresh (feature request #367).
- Wait for response line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Switching log off may take several seconds, during which YAT is unresponsive (bug #459).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET Runtime).


YAT 2.0 Final Version 2.0.0 :: 2018-04-13
----------------------------------------------------------------------------------------------------

New:
- Option to break lines on each sent or received chunk (feature request 335).
  Useful for message- rather than stream-oriented communication, e.g. most use cases of UDP/IP.

Important changes:
- UDP/IP terminals by default now use "[None]" as EOL sequence (related to feature request #335).
- UDP/IP related settings 'break lines on each sent or received chunk' and 'EOL sequence' are
  automatically changed when the port type gets changed. This automatism is only applied if the
  settings are at their defaults, i.e. have not been changed by the user; otherwise, the user is
  asked whether the settings shall be changed (related to feature request #335).
- Simple find/search function now allows to explicitly enable/disable the regex option (feature
  request #332, related to feature requests #11 and #79 implemented in previous release).
- Option to enable/disable <...> and \... escape sequences has been separated for [Send Text] and
  [Send File]. By default, escapes are enabled for [Send Text] and disabled for [Send File].
- [Send File] now also sends (formatted) text files for binary terminals, including support for
  [Explicit Default Radix] and other send file related settings (bug #411).
- Location of some send related settings has changed (related to bug #411).
- The "DynamicIndex"/"DynamicTerminalIndex" command line option has been changed to "DynamicId"/
  "DynamicTerminalId" to emphasize that the value is a 1 (and not 0) based ID, incl. the option
  to use value 0 to select the currently active terminal.
- Binary distributions now contain DejaVu fonts for manual installation (feature request #331).

Fixed bugs:
- 'SplitterDistance' value related 'InvalidOperationException' on startup fixed (bug #409).
- 'SplitterDistance' value related 'ArgumentOutOfRangeException' on startup fixed (bug #408).
- 'NullReferenceException' under certain conditions when .yat file got deleted fixed (bug #407).

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General limitations of .NET Framework:
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- General limitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. [Find Pattern] (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or [Find] items (bug #347).
   > When [Send Text] or [Send File] is hidden, resizing the panel doesn't properly work (bug #412).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port may lead
     to a deadlock or 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401). To prevent this issue, refrain from disconnecting a device
     while its port is open. Or, manually close the port after the device got disconnected.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Line content and EOL may be sent in two separate chunks, because the parts are handled slightly
  after each other. Delay could be eliminated but requires some refactoring (feature request #333).
- Wait for response line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET Runtime).


YAT 2.0 Epsilon Version 1.99.90 :: 2018-01-12
----------------------------------------------------------------------------------------------------

New:
- Simple find/search function for monitor contents added (feature requests #11 and #79).
- Recent TCP/IP and UDP/IP ports, remote hosts and local filters are remembered (feat. req. #273).
- Option to show a copy of the active monitor line in an additional text box, allowing to select
  and copy/paste characters and words (feature request #313).
- Automatic action feature, YAT can automatically invoke an action when receiving a configurable
  trigger sequence (feature requests #11, #314, #320, #325).
- Local time can optionally be shown in the main status bar (feature request #328).

Important changes:
- Cursor behavior of [Send Text] improved:
   > Cursor position and text selection is remembered (related to bugs #391 and #395).
   > Cursor no longer jumps to the end of the input box when sending the text (bug #395).
   > Cursor no longer jumps to the beginning of the input box after edit of recent (rel. bug #391).
- Shortcut of [Clear] changed from [Shift+Delete] to [Ctrl+L] (related to change stated above).
- Shortcut of [Save to File] changed from [Ctrl+F] to [Ctrl+T] (consequence of feat. #11 and #79).
- Automatic response trigger is highlighted, same as automatic action trigger (rel. to feat. #320).
- Automatic response extended by selection to respond the received trigger (related to feature
  requests #176, #252 implemented in version 1.99.50).
  This option combined with setting the trigger to "[Any Line]" makes YAT an echo server.
- Switching among automatic/manual vertical scrolling in monitor further improved, especially while
  continuous data is being received (related to bug #394 and feature request #323 implemented in
  previous release).
- Location of a few settings refined (workspace -vs- local user -vs- roaming user).
- Additional option to inhibit warning if a port/interface/device is no longer available (bug #392).
- Additional command line options 'KeepTerminalClosed/Stopped' and 'KeepLogOff' (bug #392).
- Additional command line option alias 'StartTerminal' for 'OpenTerminal' (related to bug #392).

Fixed bugs:
- Monitors no longer scroll to top as soon as display buffer has been filled (bugs #394 and #398 as
  well as feature request #326; related to feature request #323 implemented in previous release).
- Focus no longer moves away from [Send Text] when switching applications (bug #391).
- Standard word selection shortcuts [Ctrl+Shift+Left|Right] also work when commands are predefined.
- Shortcuts to navigate command pages changed from [Ctrl+Shift+Left|Right] to [Ctrl+Alt+Left|Right]
  and [Ctrl+Shift+F1..F12] to [Ctrl+Alt+1..9] as well as shortcuts to 'Copy to Send Text' changed
  from [Alt+Shift+F1..F12] to [Ctrl+Shift+F1..F12] (consequence of change above).
- Endianness of multi-byte encoded characters fixed, UTF-8 no longer results in spurious warning
  messages (bug #400, regression of bug #343 fixed for version 1.99.70).
- Superfluous spaces for multi-byte LE encodings fixed (related to bug #400 and feat. request #271).
- Code page of UTF-32 LE/BE encodings fixed (related to bug #400).
- 'UnauthorizedAccessException' on no longer valid log file path fixed (bug #404).
- 'ArgumentOutOfRangeException' when command line arguments refer to an empty workspace fixed.

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General limitations of .NET Framework:
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- General limitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. [Find Pattern] (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or [Find] items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port may lead
     to a deadlock or 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401). To prevent this issue, refrain from disconnecting a device
     while its port is open. Or, manually close the port after the device got disconnected.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for response line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET Runtime).


YAT 2.0 Delta Version 1.99.80 :: 2017-10-15
----------------------------------------------------------------------------------------------------

New:
- Additional setting and keywords \!(FramingErrorsOn|Off|Restore) that allows configuring the serial
  COM port behavior on framing errors. Useful when e.g. changing baud rates (feature request #321).

Important changes:
- Additional workaround applied to prevent potential 'ObjectDisposedException' or
  'UnauthorizedAccessException' when disconnecting USB/RS-232 converters (USB CDC) without closing
  the serial COM port (bug #382).
  Workaround also applies when undocking or hibernating a computer running YAT without closing the
  serial COM port, and when restarting devices that implement a virtual serial COM port (USB CDC).
- Option to disable user interaction question when a serial COM port, a local network interface or
  a USB Ser/HID device is no longer available; terminal will silently stay closed (feat. req. #316).
- Improved speed on sending and receiving, preventing application freeze on fast data (bug #383).
- Improved responsiveness to send requests while receiving a lot of fast data, as well as
  responsiveness to receive data while sending a lot of fast data (rel. to bugs #305, #380, #383).
- Automatic vertical scrolling of monitor is not only suspended if one or more lines are selected
  but also, if the scroll bar is moved away from the bottom of the monitor (feature request #323).
- Break [Ctrl+B] can now also be used to break sending a file (bug #305 and feature request #295).
- Auto-reconnect of TCP/IP client terminals enabled by default.
- Text input now supports the [Ctrl+Backspace] shortcut and multi-line text input also the [Ctrl+A]
  shortcut (feature request #317 as workaround to limitation of .NET Windows.Forms).
- Warning on invalid multi-byte encoded byte sequences for string, character and Unicode radix.
- Option to display the date has been merged with option to display the time, but it is now possible
  to configure the format (feature requests #31, #291, #319 and related to former feature req. #14).
  In addition, options to display the time span as well as the time delta have been added.
- 'Comments' exclusion on sending has been migrated to 'Text' exclusion that is now supporting
  regex patterns instead of plain strings (feature request #307). The change adds the possibility
  to omit empty lines. Attention, former 'Comment indicators' settings will have to be reapplied.

Fixed bugs:
- Serial COM port switching again properly works (bug #376).
- Serial COM port sending at very low baud rates again properly works (bug #379).
- Issue with UDP/IP server after connection reset fixed, automatically receiving again (bug #381).
- Monitor can again be cleared when terminal is closed (bug #380).
- Calculation of buffer line numbers fixed (related to bug #380).
- Calculation of length of multi-byte encoded EOL sequences fixed.
- Erroneous error message when changing log file separator fixed (bug #378).
- 'InvalidOperationException' when defining user defined log file separator or monitor format
  enclosure or separator fixed (partly related to bug #378).
- Minor issues with predefined and send commands fixed (parts of bug #308).
- Description of predefined commands can be changed again (bugs #377 and #386).
- Context menu shortcuts are no longer executed when a dialog is open (bug #300).
- Predefined command page navigation shortcuts fixed (related to bug #300).
- Predefined command panel width change issue in case Tx or Rx panel is shown fixed (bug #384).
- Saving and restoring the location in case of manual terminal layout fixed (bug #252).
- System display scaling other than 100% mostly fixed for Win 7 and before (bugs #85, #235, #375).

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General limitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports on disconnect, undock or hibernate without closing the port may lead
     to a deadlock or 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387). To prevent this issue, refrain from disconnecting a device while
     its port is open. Or, manually close the port after the device got disconnected.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for response line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET Runtime).


YAT 2.0 Gamma 3 Version 1.99.70 :: 2017-07-04
----------------------------------------------------------------------------------------------------

New:
- [Send Text] without EOL by [Ctrl+Enter] or [Ctrl+F3] (feature requests #281, #283, #285).
- Option to send data in Unicode notation as "\U+...." or C-style "\u...." or YAT-style "\U(....)
  as well as option to show data in Unicode notation "U+...." added (feature request #271).
- Option to disable BOM (Unicode encoding preamble) when logging in UTF-8 added (bug #363).
- Option to disable formatting; useful when highest data throughput slows down the view (feat. #39).
- Keywords with arguments \!(Delay(<ms>)), \!(LineDelay(<ms>)), \!(LineInterval(<ms>)) and
  \!(LineRepeat(<n>)) added (feature requests #13, #139).
- Additional keyword \!(ReportID(<ID>)) allows changing the USB Ser/HID report ID while sending and
  thus, allows to sequentially write to and read from multiple report IDs (feature request #296).
- Additional USB Ser/HID preset 'Signal 11 HID API' and changed presets 'Plain' and 'Common' to
  cover more use cases related to raw binary data (feature request #297 and bug #367).

Important changes:
- Option to display non-payload USB Ser/HID data (related to feature req. #296, #297 and bug #367).
- Option to skip empty lines when sending a text file (feature request #298).
- No longer showing empty lines that had only contained a previous line's hidden EOL (feat. #299).
- No longer showing a potentially annoying message box if no serial COM ports or no local network
  interfaces or no HID capable USB devices are currently available.
- Serial COM port that are in use are no longer simply labelled "(in use)" but rather "(in use by
  this terminal)", "(... terminal<ID>)" or "(... other application)" (feature requests #201, #207).
- Notes and links regarding non-intrusive RS-232 monitor/sniffer/spy cables and devices added to
  'About' dialog (related to feature requests #152, #198, #288).
- Option to disable all \!(...) keywords (former feature request #183) has been migrated to an
  option to disable *all* escape sequences, i.e. all "<...>" and "\..." sequences (feat. #302).
- When nothing has changed in a terminal, its file (.yat) is no longer auto-saved (bug #365).
- When evaluating relative paths of workspace/terminal/send/log files, the current directory is
  always taken into account (feature request #292).
- Default file location has been refined from "<User>\Documents" to "<User>\Documents\YAT"
  (related to feature request #292).
- Warning in case of yet incomplete RTF or XML log files (bug #356).
- Automatically generated backup files are now placed into standard temporary folder (feat. #275).
- Internal event handling refined, resulting in increased stability on stopping/closing/exiting
  (related to feature request #173 and bugs #310, #339).
- SourceForge file structure simplified, split into 'Current' and 'Previous' (feature request #278).
- Binary distribution added because installer is no longer compatible with Win XP (bugs #318, #369).
- Started to migrate test environment from NUnit V2 to V3, but reverted to 2.6.4 again (feat. #293).
- NUnit tests migrated from classic to constraint model (related to feature request #293).
- NUnit framework assemblies placed into solution structure instead of referring to installation.

Fixed bugs:
- UDP/IP PairSocket enabled to communicate with a remote computer (bug #368).
- TCP/IP sockets no longer block when exiting too quickly (bug #341).
- Potential application freeze when clearing monitors/repositories fixed (bug #361).
- Potential 'NullReferenceException' in binary terminal settings fixed (bug #362).
- Option to change endianness of multi-byte encoded characters fixed (bug #343).
- Presets of serial COM port settings work again (bug #372).
- Wrong active/inactive state of log menu items fixed (bug #366).
- Catch-all of unhandled synchronous exceptions fixed (bug #310) as well as
  catch-all of unhandled asynchronous exceptions improved (feature request #173).

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General limitations of .NET Windows.Forms:
   > System display scaling other than 100% results in partly clipped controls (bugs #85 and #235).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
- Serial COM port limitations of .NET Framework:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of USB/RS-232 converters (USB CDC) on disconnect, undock or hibernate without closing
     the serial COM port may require to reset the converters, i.e. disconnect and reconnect them,
     or restart the computer. In addition, it may be required to disconnect or reset any device
     sending continuous data. Otherwise, the related serial COM ports may no longer be opened by
     an application. Issue is being reported by many users of .NET, but Microsoft seems to have
     no plans fixing it. If an 'ObjectDisposedException' or 'UnauthorizedAccessException' still
     occurs, the serial COM port connection monitoring has to be disabled in the terminal settings.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for response line (text terminals) not yet implemented (feature request #19 and bug #176).
- Breaking while sending files not possible yet (bug #305 and feature request #295).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET Runtime).


YAT 2.0 Gamma 2''          Version 1.99.52 :: 2016-09-30
YAT 2.0 Gamma 2'           Version 1.99.51 :: 2016-09-17
YAT 2.0 Gamma 2            Version 1.99.50 :: 2016-09-16
YAT 2.0 Gamma 1''          Version 1.99.34 :: 2015-06-13
YAT 2.0 Gamma 1'           Version 1.99.33 :: 2015-06-07
YAT 2.0 Gamma 1            Version 1.99.32 :: 2015-06-01
YAT 2.0 Beta 4 Candidate 2 Version 1.99.30 :: 2013-02-02
YAT 2.0 Beta 4 Candidate 1 Version 1.99.28 :: 2011-12-05
YAT 2.0 Beta 3 Candidate 4 Version 1.99.26 :: 2011-04-25
YAT 2.0 Beta 3 Candidate 3 Version 1.99.25 :: 2010-11-28
YAT 2.0 Beta 3 Candidate 2 Version 1.99.24 :: 2010-11-11
YAT 2.0 Beta 3 Candidate 1 Version 1.99.23 :: 2009-09-10
YAT 2.0 Beta 2 Candidate 4 Version 1.99.20 :: 2008-07-18
YAT 2.0 Beta 2 Candidate 3 Version 1.99.19 :: 2008-04-01
YAT 2.0 Beta 2 Candidate 2 Version 1.99.18 :: 2008-03-17
YAT 2.0 Beta 2 Candidate 1 Version 1.99.17 :: 2008-02-11
YAT 2.0 Beta 2 Preliminary Version 1.99.13 :: 2007-08-30
YAT 2.0 Beta 1             Version 1.99.12 :: 2007-04-15
YAT 2.0 Alpha 3            Version 1.99.8  :: 2007-02-25
YAT 2.0 Alpha 2            Version 1.99.3  :: 2007-02-07
YAT 2.0 Alpha 1            Version 1.99.0  :: 2007-01-23

Content of the above historical versions has been removed in order to compact this file such it fits
the SourceForge limitation of 64 KB for the online release notes.



====================================================================================================
4. History of Changes in XTerm232
====================================================================================================

XTerm232 1.0.2 :: 2003-10-31
XTerm232 1.0.1 :: 2003-10-30
XTerm232 1.0.0 :: 2003-10-14

Content of the above historical versions has been removed in order to compact this file such it fits
the SourceForge limitation of 64 KB for the online release notes.


====================================================================================================
5. Roadmap
====================================================================================================

YAT 2
----------------------------------------------------------------------------------------------------
YAT 2 is in maintenance mode now, i.e. focus on bug fixes and minor changes, while work for YAT 4
is already ongoing.


(Version 3 will be skipped to prevent naming conflict with yat3 of Dieter Fauth that became
public around the same time as YAT. And, 4.0 buzzes more anyway (industry 4.0 and the like ;-))
----------------------------------------------------------------------------------------------------


YAT 4 with Scripting :: Expected in 2021
----------------------------------------------------------------------------------------------------
YAT 4.0 will feature the integration of a scripting environment, based on the CSScript engine.
Scripting will allow you to script YAT and automate repetitive tasks, use it for test automation,
implement protocol layers,... whatever you can think of. Examples and templates will be included.

It is also planned to demonstrate how to use YAT from a PowerShell script and along with NUnit.
All these features aim for providing a versatile automatic testing tool for serial communications.


====================================================================================================
6. Legal
====================================================================================================

Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
Copyright © 2003-2021 Matthias Kläy.
All rights reserved.

YAT is licensed under the GNU LGPL.
See http://www.gnu.org/licenses/lgpl.html for license details.


****************************************************************************************************
                                      End of YAT Release Notes.
****************************************************************************************************
