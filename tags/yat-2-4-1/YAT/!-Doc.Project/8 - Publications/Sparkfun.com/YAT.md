YAT - Yet Another Terminal (Windows)
===============================

[YAT](http://sourceforge.net/projects/y-a-terminal/) is a user-friendly and feature-rich serial terminal. It features text as well as binary communication, predefined commands, a multiple-document user interface and lots of extras.

YAT is available to download at [SourceForge](http://sourceforge.net/projects/y-a-terminal/files/).

>> 01 - Welcome to YAT.png <<


Appearance
----------------
YAT features a multiple-document user interface (MDI) that consists of a single workspace with one or more terminals.

>> 06 - Workspace.png <<

Each terminal can be configured according to the device it shall be communicating with. These extra features make a terminal especially easy to use:

* Text command console
* File command list
* Unlimited number of predefined commands
* Drop-down of recent commands

Each terminal has its own monitor to display outgoing and incoming data. The view can be configured as desired:

* Time stamp
* Line number
* End-of-line sequence
* Line length
* Line and bytes transmission rate
* Chronometer

Most of these features can be enabled and configured, or hidden for a cleaner and simpler user interface.

>> 04 - Detailed Monitor.png <<
>> 05 - Monitor Status.png <<


Terminal Settings
------------------------
* Text or binary communication
* Communication port type:
	* Serial Port (COM)
	* TCP/IP Client, Server or AutoSocket
	* UDP/IP Socket
	* USB serial HID
* Specifc settings depending on port type

>> 07 - Serial COM Port Settings.png <<
>> 08 - TCP and UDP Settings.png <<
>> 09 - USB Serial HID Settings.png <<


Text Terminal Settings
------------------------------
* Full support of any known ASCII and Unicode encoding
* End-of-line configuration
	* Predefined and free-text sequences
	* Possibility to define separate EOL for Tx and Rx
* Send and receive timing options
* Character substituion
* Comment exclusion

>> 10 - Text Terminal Settings.png <<


Binary Terminal Settings
---------------------------------
* Configuration of protocol and line representation
* Possibility to define separate settings for Tx and Rx

>> 11 - Binary Terminal Settings.png <<


Advanced Settings
------------------------
* Various display options
* Various advanced communication options
* Specialized communication options for serial ports (COM)

>> 12 - Advanced Settings.png <<


Extras
--------
* Escapes for bin/oct/dec/hex like `\h(4F 4B)`
* Escapes for ASCII controls like `<CR><LF>` as well as C-style `\r\n`
* Special commands such as `\!(Delay)`, `\!(LineDelay)` and `\!(LineRepeat)`
* Versatile monitoring and logging of sent and received data
* Formatting options for excellent readability
* Powerful keyboard operation including shortcuts for the most important features
* Versatile shell/PowerShell command line
* x86 (32-bit) and x64 (64-bit) distribution

>> 02 - Predefined Commands.png <<
>> 13 - Log Settings.png <<
>> 14 - Monitor Format.png <<


Change Management and Support
---------------------------------------------
YAT is fully hosted on SourceForge. [Feature Requests](http://sourceforge.net/p/y-a-terminal/feature-requests/) and [Bug Reports](http://sourceforge.net/p/y-a-terminal/bugs/) can be entered into the according tracker. Both trackers can be filtered and sorted, either using the predefined searches or the list view.
Support is provided by a few simple helps integrated into the application, some screenshots on the SourceForge page, and the project's email if none of the above can help.

Development
------------------
YAT is implemented in C#.NET using Windows.Forms. The source code is implemented in a very modular way. Utilities and I/O sub-systems can also be used independent on YAT, e.g. for any other .NET based application that needs serial communication, command line handling or just a couple of convenient utilities.
Testing is done using an [NUnit](http://www.nunit.org/) based test suite. Project documentation is done in [OpenOffice](http://www.openoffice.org/). For more details and contributions to YAT, refer to *Help > About*.
