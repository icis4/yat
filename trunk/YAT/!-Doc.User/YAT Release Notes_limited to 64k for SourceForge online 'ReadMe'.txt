
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
                                Copyright © 2003-2019 Matthias Kläy.
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

YAT uses .NET 3.5 SP1. The YAT installer verifies that .NET is installed on the target computer.
The YAT installer also verifies that Windows Installer 4.5 is installed on the target computer.

First, chose the most appropriate package:
 > For up-to-date systems, use the compact package "...(32-bit).zip" or "...(64-bit).zip".
   (Windows Installer and .NET are already installed on up-to-date systems.)
 > For offline installation, use a full package "..._with_.NET...zip".
   (Windows Installer and .NET are included for installation.)
 > For Windows XP, use a binary distribution.
   (The YAT installer is no longer compatible with Windows XP.)

It is recommended to unzip this package to a temporary location before starting installation.

Run the ".msi" if Windows Installer is installed, otherwise "setup.exe".
 > Installer checks prerequisites and installs what is missing.
 > Installer installs YAT. Older versions of YAT are automatically replaced.

For installation of a binary distribution, refer to the instructions inside that package.

You can also download .NET and/or Windows Installer from <https://www.microsoft.com/en-us/download>
or by googling for "Microsoft Download .NET Framework 3.5 SP1 Full" or "Windows Installer 4.5".
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
- Text terminals: Additional options to control line break:
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
- General imitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. 'Find Pattern' (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or 'Find' items (bug #347).
   > When [Send Text] or [Send File] is hidden, resizing the panel doesn't properly work (bug #412).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports while disconnect, undock or hibernate without closing the port may
     result in an 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401, #442). To prevent this issue, refrain from disconnecting a
     device while its port is open. Or, manually close the port after the device got disconnected.
- The \!(PortSettings()) keyword is yet limited to serial COM ports (feature request #71).
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Line content and EOL may be sent in two separate chunks, because the parts are handled slightly
  after each other. Delay could be eliminated but requires some refactoring (feature request #333).
- Automatic responses and actions work fine as long as the received chunks do not contain more than
  one trigger and do not spread across multiple lines. Limitation could be eliminated but requires
  some refactoring (feature request #366).
- Automatic actions 'Filter' and 'Suppress' do not get reapplied on refresh (feature request #367).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


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
- General imitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. 'Find Pattern' (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or 'Find' items (bug #347).
   > When [Send Text] or [Send File] is hidden, resizing the panel doesn't properly work (bug #412).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports while disconnect, undock or hibernate without closing the port may
     result in an 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401). To prevent this issue, refrain from disconnecting a device
     while its port is open. Or, manually close the port after the device got disconnected.
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Line content and EOL may be sent in two separate chunks, because the parts are handled slightly
  after each other. Delay could be eliminated but requires some refactoring (feature request #333).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


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
- General imitations of .NET Windows.Forms:
   > System display scaling other than 100% (96 DPI) results in minor distortions on Win 7 and
     before (bugs #85, #235, #375) and some blurring on Win 8 and above (feature request #310).
     The latter will be fixed with upgrading to .NET 4.7+ (feature request #229).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Tool strip combo box slightly flickers when updating item list, e.g. 'Find Pattern' (bug #402).
   > Combo box cannot restore some corner-case cursor positions (bug #403).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] or 'Find' items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
   > Vertical scrolling of monitors while a lot of data is being transmitted and while items are
     selected may lead to a severe drop of the overall performance (related to bug #383).
   > Unicode is limited to the basic multilingual plane (U+0000..U+FFFF) (feature request #329).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
   > Issue with layouting when closing an MDI child (bug #399).
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports while disconnect, undock or hibernate without closing the port may
     result in an 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387, #401). To prevent this issue, refrain from disconnecting a device
     while its port is open. Or, manually close the port after the device got disconnected.
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


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
- General imitations of .NET Windows.Forms:
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
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of serial COM ports while disconnect, undock or hibernate without closing the port may
     result in an 'ObjectDisposedException' or 'UnauthorizedAccessException'. It happens due to
     a bug in the .NET 'SerialPort' class for which Microsoft only has vague plans fixing. YAT is
     applying several patches to try working around the issue (bugs #224, #254, #293, #316, #317,
     #345, #382, #385, #387). To prevent this issue, refrain from disconnecting a device while
     its port is open. Or, manually close the port after the device got disconnected.
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


YAT 2.0 Gamma 3 Version 1.99.70 :: 2017-07-04
----------------------------------------------------------------------------------------------------

New:
- [Send Text] without EOL by [Ctrl+Enter] or [Ctrl+F3] (feature requests #281, #283, #285).
- Option to send data in Unicode notation as "\U+...." or C-style "\u...." or YAT-style "\U(....)
  as well as option to show data in Unicode notation "U+...." added (feature request #271).
- Option to disable BOM (Unicode encoding preamble) when logging in UTF8 added (bug #363).
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
- General imitations of .NET Windows.Forms:
   > System display scaling other than 100% results in partly clipped controls (bugs #85 and #235).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of USB/RS-232 converters (USB CDC) while disconnect, undock or hibernate without closing
     the serial COM port may require to reset the converters, i.e. disconnect and reconnect them,
     or restart the computer. In addition, it may be required to disconnect or reset any device
     sending continuous data. Otherwise, the related serial COM ports may no longer be opened by
     an application. Issue is being reported by many users of .NET, but Microsoft seems to have
     no plans fixing it. If an 'ObjectDisposedException' or 'UnauthorizedAccessException' still
     occurs, the serial COM port connection monitoring has to be disabled in the terminal settings.
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Breaking while sending files not possible yet (bug #305 and feature request #295).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


YAT 2.0 Gamma 2'' Version 1.99.52 :: 2016-09-30
----------------------------------------------------------------------------------------------------

Important changes:
- Preset of 256000 baud added to serial COM port settings (feature request #276).
- Binary terminals: Length line break enabled by default.

Fixed bugs:
- Binary terminals: Logging in 'Neat' format fixed (bug #359).
- Binary terminals: 'ObjectDisposedException' when changing length line break settings (bug #358).


YAT 2.0 Gamma 2' Version 1.99.51 :: 2016-09-17
----------------------------------------------------------------------------------------------------

Important changes:
- Distribution without .NET runtime restored to .zip due to limitation of Internet Explorer with
  .msi downloads (even though .msi has been created with a Microsoft tool...) (bugs #318 and #323).

Fixed bugs:
- Binary terminals: Radix 'String' and 'Char' fixed (bug #357).


YAT 2.0 Gamma 2 Version 1.99.50 :: 2016-09-16
----------------------------------------------------------------------------------------------------

New:
- UDP/IP functionality improved:
   > Possibility to receive from multiple sources when using 'UDP/IP Server' (feature request #248).
   > Dedicated settings for server, client and socket (related to feature request #248).
- USB Ser/HID can be configured to XOn/XOff flow control (feature request #226).
- USB Ser/HID discovery improved, serial string may optionally be ignored (feature request #261).
- Automatic response feature, YAT can automatically respond to a request (feature reqs. #176, #252).
- Keyword \!(LineInterval) implemented in addition to the existing \!(LineDelay); useful together
  with \!(LineRepeat), line will be sent in more or less accurate interval (feature request #255).
- Binary terminals now also support to break lines before a sequence (feat. reqs. #76, #140, #180).
- Selectable default parse radix, e.g. useful when sending binary data (feat. reqs. #238, #270).
- Options to define the enclosure and spacing of display elements added (related to bug #292).
- Option to display the port as e.g. "COM1" or "127.0.0.1:10000" (feature request #14).
- Option to display the direction as "<<" (Tx) and ">>" (Rx) (feature request #14).
- Option to display the date in addition to the time (related to feature request #14).
- Options to hide 0x00 and/or 0xFF (feature req. #243).
- Option to encode neat logs in terminal encoding instead of UTF-8 (related to feature request #69).
- Logging in RTF format implemented (feature request #233). Note that the log file's RTF structure
  is incomplete while logging is ongoing. The file can only be opened by an RTF reader (e.g. Word)
  after logging has finished, i.e. logging has been switched off or the terminal is closed.
- Logging in XML format implemented (feature requests #26 and #27). Note that the log file's XML
  structure is incomplete while logging is ongoing. The file can only be opened by an XML reader
  after logging has finished, i.e. logging has been switched off or the terminal is closed.
- Line numbers may indicate the total number of transmitted lines (feature request #265).
- Selectable background color added to format settings (feature request #250).
- Window can be configured to always stay on top (feature request #241).

Important changes:
- 'Send Command' renamed to [Send Text] (related to feature request #252).
- Active terminal is highlighted/emphasized (feature request #247).
- Automatic terminal layout added. Use [Window > ...] for dedicated layout (feature request #247).
- If YAT is not the active app, a single click is now sufficient to execute an action (bug #210).
- Icons migrated from glyfx to FatCow (much larger variety) as preparation for the items below.
- Additional log menu items and toolbar icons added, including indication whether logging is
  switched on or off, as well as opening/viewing of log file (feature requests #69, #103, #245).
- XML schema on [Monitor > Save To File...] aligned with XML schema of logging in XML. Schema
  is saved in additional .xsd file (related to feature requests #26 and #27).
- Several other menu items and toolbar icons added (includes feature request #222).
- Parser is now capable to handle contiguous values such as \h(01020304) (related to bug #326).
- Parser error message improved in terms of accuracy of the error location (bug #268).
- File handling as well as error messages improved and unified (bugs #320 and #338).
- Lookup of serial COM ports improved (also fixes bug #23).
- New option to limit the serial COM port output to the selected baud rate - 25%.
- Handling of serial COM port RFR and DTR state improved (feature request #236 and bugs #203, #294).
- Handling of serial COM port XOff state fixed and improved (feature req. #216 and bugs #255, #295).
- Serial COM port software flow control options to send XOn more frequently added (feature #206).
- Superfluous control pin events of e.g. internal COM ports is now being handled (bugs #271, #277).
- Settings related to XOn/XOff and ASCII control character replacement improved (bug #319).
- Synchronization among outgoing and incoming data improved, preventing data mix-ups and exceptions
  (related to feat. reqs. #14, #252 as well as bugs #132, #275, #292, #321, #331, #333, #334, #346).
- Performance in terms of CPU and RAM usage improved (bugs #189, #243, #263, #302, #336).
- Adaptive monitor update rate significantly improved, it is now dependent on the CPU load.
- Number of bytes per line limited, in order to keep performance even in case of enormous lines.
- Implementation of keyword \!(Clear) improved, it now shortly waits before clearing (bug #251).
- Additional hints (tool tips) added to the settings dialogs (feature request #189).
- Help window is now sizeable to display more/all content (feature request #237).
- Command line options fixed, extended and slightly changed (bug #335 and feature request #260).
- DejaVu fonts updated to 2.37
- Visual Studio upgraded to 2015 Community Edition, including installer (VSI).
- Distribution without .NET runtime simplified to .msi only (bugs #318 and #323).
- Test board (serial COM ports, USB devices,...) created and test cases adapted accordingly.

Fixed bugs:
- Use of USB/RS-232 converters (USB CDC) while disconnect, undock or hibernate without closing the
  serial COM port no longer leads to an 'ObjectDisposedException' or 'UnauthorizedAccessException'
  in most cases (bugs #224, #254, #293, #316, #317, #345). However, it may still be needed to reset
  the converter, i.e. disconnect and reconnect it, or restart the computer. In addition, it may be
  required to disconnect or reset any device sending continuous data. Otherwise, related serial COM
  ports may no longer be opened. Issue is being reported by many users of .NET, but Microsoft seems
  to have no plans fixing it. If an 'ObjectDisposedException' or 'UnauthorizedAccessException' still
  occurs, the serial COM port connection monitoring has to be disabled in the terminal settings.
- TCP/IP local interface setting fixed:
   > Setting is now properly applied to all TCP/IP sockets (bug #322).
   > Setting extended to interface description/address pair (bugs #328 and #355).
- UDP/IP functionality fixed:
   > Can now properly receive from a remote host (bug #327).
   > Dedicated settings for server, client and socket (related to bug #248).
- Subsequent log files are now properly named according to the configured settings. File names that
  include the time stamp now properly use the time stamp of the open operation (related to feature
  requests #69, #103, #233, #245).
- Long monitor content is now displayed properly even when horizontally scrolling (bug #125).
- 'ExternalException' "A generic error occurred in GDI+" while drawing has been fixed by reverting
  from GDI+ to GDI (bugs #191, #266, #284, #286 and #325).
- .NET Windows.Forms MDI issues with window list worked-around (bugs #119, #180 and #213).

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2015 on .NET 3.5 SP1 (feat. #229).
- General imitations of .NET Windows.Forms:
   > System display scaling other than 100% results in partly clipped controls (bugs #85 and #235).
   > System errors are output in local language, even though YAT is all-English (bug #66).
   > Combo box text is compared case insensitively against item list, e.g. "aa" is changed to "AA"
     if that is contained in the item list, e.g. the recent [Send Text] items (bug #347).
   > Automatic completion for e.g. [Send Text] is not feasible to implement (feature request #227).
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163).
- MDI limitations of .NET Windows.Forms:
   > Issues with frame (bugs #29 and #30).
   > Issue with window list (bug #31).
- Serial COM port limitations of .NET:
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101).
   > Use of USB/RS-232 converters (USB CDC) while disconnect, undock or hibernate without closing
     the serial COM port may require to reset the converters, i.e. disconnect and reconnect them,
     or restart the computer. In addition, it may be required to disconnect or reset any device
     sending continuous data. Otherwise, the related serial COM ports may no longer be opened by
     an application. Issue is being reported by many users of .NET, but Microsoft seems to have
     no plans fixing it. If an 'ObjectDisposedException' or 'UnauthorizedAccessException' still
     occurs, the serial COM port connection monitoring has to be disabled in the terminal settings.
- Serial COM port break states may not be supported on certain hardware, e.g. USB/RS-232 converters.
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119).
- Keyword \!(Delay(<TimeSpan>)) not yet implemented (feature request #139).
- Keyword \!(Repeat(<Item>, <Repetitions>, <Delay>)) not yet implemented (feature request #13).
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176).
- Breaking while sending files not possible yet (bug #305).
- Direct send text mode does not yet support special formats and commands (feature request #10).
- Running YAT for a long period, or creating many terminals, results in memory leaks, which result
  in a gradual increase of the memory consumption (RAM) (bugs #243, #263 and #336, root cause yet
  unknown, could even be a limitation of the memory management of the .NET 2.0 runtime).


YAT 2.0 Gamma 1'' Version 1.99.34 :: 2015-06-13
----------------------------------------------------------------------------------------------------

New:
- Serial COM ports can better be configured in terms of sending (feature request #235)
   > Output buffer size can be limited; useful when using hardware and/or software flow control
   > Send rate can be limited; useful if a device cannot process more than a certain amount of data

Important changes:
- Serial COM port send chunk size can no longer be configured (related to feature request #235)

Fixed bugs:
- 'SocketException' when resolving URL of a not yet available remote host fixed (bug #312)
- Invalid handling of remote host IP address fixed (bugs #313, #314 and #315)
- Local user settings (previous workspace/terminals, recent files, window location, preferences) are
  again loaded from the latest previous version (bug #311)


YAT 2.0 Gamma 1' Version 1.99.33 :: 2015-06-07
----------------------------------------------------------------------------------------------------

Fixed bugs:
- 'FormatException' when opening the terminal settings of a terminal that is based on an existing
  .yat file fixed, additional test cases added to regression testing (bugs #306 and #307)
- Issues with [Send File] fixed, handling of recent commands and shortcuts improved (bug #304)


YAT 2.0 Gamma 1 Version 1.99.32 :: 2015-06-01
----------------------------------------------------------------------------------------------------

New:
- Keyword \!(LineRepeat) added, count can be configured in advanced settings (feat. #13 and #162)
- Support for variants of USB Ser/HID (e.g TI MSP430 HID API) added, report format can be specified
  in the USB Ser/HID settings (feature requests #193 and #209)
- Write protected .yat files are marked '#' and are not attempted to be written automatically,
  write protected .yaw files are not attempted to be written either (feature request #194)
- Predefined commands can now be copied to 'Send Command' using shortcuts Alt+Shift+Fx (feat. #218)
- Tool tips added to command buttons (feature request #205, also related to feature request #220)
- Terminal window setting (manual/cascade/tile) is now stored in the workspace and re-applied when
  starting YAT or re-sizing the main form (feature requests #40, #141 and #170)

Important changes:
- \!(...) keywords can optionally be disabled (feature request #183)
- Retrieving serial COM port captions from the system can optionally be disabled in 'Preferences'
  Disabling can be useful on computers where discovery of serial COM ports takes too long
- Binary terminals no longer by default replace control characters (feature request #204)
- Serial COM port XOn/XOff flow control chars can optionally be hidden for 'Manual Software' and
  'Manual Combined' (feature request #190)
- Serial COM port XOn/XOff flow control state and count is only indicated for 'Manual Software' and
  'Manual Combined', this is due to a limitation of the .NET/Win32 serial port driver (bug #214)
- Serial COM port modem baud rates 28.8 and 33.6 added to defaults (related to feature request #193)
- USB Ser/HID serial string comparison is no longer case-sensitive (Windows behavior) (bug #279)
- Time stamps are now output in milliseconds to improve readability (feature request #196)
- Format exceptions while sending are now reported as error messages within the terminal (bug #270)
- Error handling in case of deleted .yaw and .yat files improved (related to feature request #194)
- Command line arguments can also be used to preset the settings of the initial 'New Terminal'
  dialog to fixed values, independent on the values that were selected before (related to bug #262)
- Command line console output is now channeled to stdout or stderr depending on whether it is normal
  or error information (feature request #192)
- More information on virtual serial port tools added to [Help > About] (feature request #198)
- DejaVu fonts updated to 2.34
- Visual Studio upgraded to 2013 Community Edition, incl. installer (VSI) (feature request #221)
- Installer split into separate packages for x86 and x64 (feature requests #175 and #191)
- Installer improved, now using YAT specific banner and texts (related to requests #175 and #191)
- NUnit updated to 2.6.4, other development tools (GhostDoc, StyleCop, FxCop) updated too

Fixed bugs:
- Duplicated and/or split display representation fixed (bugs #258, #264 and #265)
- Manually entered serial COM port, remote TCP/IP host or USB Ser/HID device is now properly
  restored when opening the terminal settings dialog (bug #259)
- Manually entered non-standard baud rate is properly restored in settings dialog again (bug #283)
- Shortcuts are now always routed to the correct terminal window (i.e. MDI child) (bug #152)
- Handling of recent commands in 'Send Command' improved, arrows up/down properly work again
- Representation of description of multi-line commands in 'Send Command' improved
- Issues with 'Send Command' fixed, additional test cases added (bugs #276, #282 and #291)
- Issues with line break fixed (bugs #269 and #281)
- Issue with escape sequences following a C-style escape fixed (bug #290)
- Exception while reading stream in YAT.Domain.Parser.Parser.TryParse() fixed (bug #261)
- Exception while changing settings of a UDP/IP terminal fixed (bugs #289 and #301)
- Exceptions and issues while closing terminal or application fixed (bugs #72, #267, #287, #288)
- Memory leak when subsequently creating and closing terminals fixed (bug #263)
- Excessive memory occupation after standby fixed (bug #243)
- 'MissingMethodException' of 'Boolean System.Threading.WaitHandle.WaitOne(Int32)' fixed by adding
  .NET 3.5 SP1 as prerequisite to setup/installer (bug #273)
- Start from command line no longer always generates a terminal (bug #262)
- 'InvalidOperationException' when starting YAT with /r command line option fixed (bug #260)
- 'NullReferenceException' when starting YAT with /tf command line option fixed (bug #280)
- 'ThreadException' upon copy of exception data to clipboard fixed (bug #245)

Limitations and known issues:
- x64 distributions are 'AnyCPU' builds due to limitations of VS2013 on .NET 3.5 SP1 (feat. #229)
- General imitations of .NET Windows.Forms
   > If YAT is not the active application, two clicks are required to execute an action. Reason:
     In .NET Windows.Forms the first click only activates the application but doesn't execute
     anything (bug #210)
   > System errors are output in local language, even though YAT is all-English (bug #66)
   > Automatic horizontal scrolling of monitors is not feasible to implement (feature request #163)
- MDI limitations of .NET Windows.Forms
   > Issues with frame (bugs #29 and #30)
   > Issues with window list (bugs #31, #119, #180 and #213)
- Long monitor content is not displayed properly when horizontally scrolling (bug #125)
- Serial COM port limitations of .NET
   > Support for ports named other than "COM..." isn't supported by .NET (feature request #101)
- Serial COM port limitations of certain hardware, e.g. certain USB/RS-232 converters (USB CDC)
   > RFR/CTS flow control 'Hardware' and 'Hardware Combined' may not be supported (bug #203)
   > Manual RFR/CTS flow control 'Manual Hardware' and 'Manual Hardware Combined' may not be
     supported on Win7/64 (bug #254)
   > Handling of serial COM port break states may not be supported (feature request #109)
- Use of USB/RS-232 converters (USB CDC) while hibernate or undock without closing the serial COM
  port leads to 'ObjectDisposedException' in the system internal infrastructure (bugs #293 and #316)
- Use of internal serial COM port may lead to slowdown of the application on certain computers; due
  to superfluous events triggered by the control pins (bugs #271 and #277); root cause yet unknown
- USB Ser/HID only runs on Windows; use of 'LibUsb'/'LibUsbDotNet' and significant migration work of
  implementation and test environment would be needed to run it on unixoids (feature request #119)
- Keyword \!(Delay(<TimeSpan>)) not yet implemented (feature request #139)
- Keyword \!(Repeat(<Item>, <Repetitions>, <Delay>)) not yet implemented (feature request #13)
- Wait for answer line (text terminals) not yet implemented (feature request #19 and bug #176)
- Breaking while sending files not possible yet (bug #305)
- Direct send text mode does not yet support special formats and commands (feature request #10)
- Logging is limited to text, RTF format not yet implemented (feature requests #233)
- XML logging not yet implemented, definition of schema pending (feature requests #26 and #27)


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
YAT 2.0 Beta 1 Version 1.99.12 :: 2007-04-15
YAT 2.0 Alpha 3 Version 1.99.8 :: 2007-02-25
YAT 2.0 Alpha 2 Version 1.99.3 :: 2007-02-07
YAT 2.0 Alpha 1 Version 1.99.0 :: 2007-01-23

Content of the above Alpha and Beta versions has been removed in order to compact this file such it
fits the SourceForge limitation of 64 KB for the online release notes.



====================================================================================================
4. History of Changes in XTerm232
====================================================================================================

XTerm232 1.0.2 :: 2003-10-31
XTerm232 1.0.1 :: 2003-10-30
XTerm232 1.0.0 :: 2003-10-14

Content of the above Alpha and Beta versions has been removed in order to compact this file such it
fits the SourceForge limitation of 64 KB for the online release notes.


====================================================================================================
5. Roadmap
====================================================================================================

YAT 2
----------------------------------------------------------------------------------------------------
YAT 2 is in maintenance mode now, i.e. focus on bug fixes and minor changes, while work for YAT 4
is already ongoing. Still, YAT 2.4 is intended to upgrade to the .NET 4.0 runtime and framework,
versions 2.2 and 2.3 will be skipped in favor of the digit 4.

(Major version 3 will be skipped to prevent naming conflict with yat3 of Dieter Fauth that became
public around the same time as YAT. And, 4.0 buzzes more anyway (industry 4.0 and the like ;-))


YAT 4 with Scripting :: Expected in 2020
----------------------------------------------------------------------------------------------------
YAT 4.0 will feature the integration of a scripting environment, based on the CSScript engine.
Scripting will allow you to script YAT and automate repetitive tasks, use it for test automation,
implement protocol layers,... whatever you can think of. Examples and templates will be included.

It is also planned to demonstrate how to use YAT from a PowerShell script and along with NUnit. All
these features aim for providing a versatile automatic testing tool for serial communications.


====================================================================================================
6. Legal
====================================================================================================

Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
Copyright © 2003-2019 Matthias Kläy.
All rights reserved.

YAT is licensed under the GNU LGPL.
See http://www.gnu.org/licenses/lgpl.html for license details.


****************************************************************************************************
                                      End of YAT Release Notes.
****************************************************************************************************
