﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using MKY;
using MKY.CommandLine;
using MKY.IO.Usb;

using YAT.Application.Utilities;
using YAT.Model.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.Model
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class CommandLineArgs : ArgsHandler
	{
		private const string VisibilitySuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		#region Public Fields = Command Line Arguments
		//==========================================================================================
		// Public Fields = Command Line Arguments
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[ValueArg(Description = "Open the given workspace (.yaw) or terminal (.yat).")]
		[OptionArg(Name = "Open", ShortName = "o", Description = "Open the given workspace (.yaw) or terminal (.yat).")]
		public string RequestedFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "Recent", ShortName = "r", Description = "Open the most recent file.")]
		public bool MostRecentIsRequested;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		public string MostRecentFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "New", ShortName = "n", Description = "Create a new terminal according to the options given or the default values.")]
		public bool NewIsRequested;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Type", "TerminalType" }, ShortNames = new string[] { "t", "ty" }, Description =
			"The desired terminal type. Valid values are 'Text', 'T' or 'Binary', 'B'. The default value is 'Text'.")]
		public string TerminalType;

		/// <summary></summary>
		/// <remarks>
		/// This option is intentionally called 'PortType' because it shall match the name on the 'New Terminal' and 'Terminal Settings' dialog.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "PortType", ShortName = "pt", Description =
			"The desired port type. Valid values are:" + EnvironmentEx.NewLineConstWorkaround +
			"> 'COM' (Serial COM Port)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'TCPClient', 'TCPServer', 'TCPAutoSocket' (TCP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'UDPClient', 'UDPServer', 'UDPPairSocket' (UDP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'USBSerHID' (USB Ser/HID)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is 'COM'.")]
		public string IOType;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "SerialPort", ShortName = "p", Description =
			"The desired serial COM port. Valid values are 1 through 65536, given the port exists on the current machine. The default value is 1." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int SerialPort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Baud", "BaudRate" }, ShortName = "br", Description =
			"The desired baud rate. Must be a positive integral value that is supported by the selected serial COM port on the current machine. " +
			"Typical values are 2400, 4800, 9600, 19200, 38400, 57600 and 115200. The default value is 9600." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int BaudRate;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "DataBits", ShortName = "db", Description =
			"The desired number of data bits. Valid values are 4, 5, 6, 7 or 8. The default value is 8." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int DataBits;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "Parity", ShortName = "pa", Description =
			"The desired parity setting. Valid values are 'None', 'Odd', 'Even', 'Mark' and 'Space'. The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public string Parity;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "StopBits", ShortName = "sb", Description =
			"The desired number of stop bits. Valid values are 0, 1, 1.5 or 2. The default value is 1." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public double StopBits;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "FlowControl", ShortName = "fc", Description =
			"The desired method of flow control." + EnvironmentEx.NewLineConstWorkaround +
			"For serial COM ports, valid values are 'None', 'Hardware' (RFR/CTS), 'Software' (XOn/XOff), 'Combined' (RFR/CTS and XOn/XOff), " +
			"'ManualHardware', 'ManualSoftware', 'ManualCombined' and 'RS485' (RS-485 Transceiver Control). The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"For USB Ser/HID, valid values are 'None', 'Software' (XOn/XOff) and 'ManualSoftware'. The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"Does not apply to TCP/IP and UDP/IP.")]
		public string FlowControl;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "SerialPortAliveMonitor", ShortName = "spam", Description =
			"When device is connected, e.g. a USB/COM converter, monitor the port every given milliseconds. " +
			"Must be positive integral value equal or greater than 100. A common value is 500. " +
			"The special value 0 indicates disabled. " +
			"By default, this feature is enabled and set to 500 milliseconds." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int SerialPortAliveMonitor;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "SerialPortAutoReopen", ShortName = "spar", Description =
			"When device is no longer available, e.g. a USB/COM converter, try to reopen the port every given milliseconds. " +
			"Must be positive integral value equal or greater than 100. A common value is 2000. " +
			"The special value 0 indicates disabled. " +
			"By default, this feature is enabled and set to 2000 milliseconds." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int SerialPortAutoReopen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "RemoteHost", ShortName = "rh", Description =
			"The desired remote IP host. Must be a valid IPv4 or IPv6 address or an alias or reserved address like:" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Localhost>' (IPv4 or IPv6 localhost)" + EnvironmentEx.NewLineConstWorkaround +
			"> '127.0.0.1' (IPv4 localhost) or '::1' (IPv6 localhost)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is '<Localhost>'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP/IP and UDP/IP.")]
		public string RemoteHost;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="System.Net.IPEndPoint.MinPort"/>, <see cref="System.Net.IPEndPoint.MaxPort"/> and <see cref="MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "RemotePort", ShortName = "rp", Description =
			"The desired remote TCP or UDP port. Valid values are 0 through 65535. The default value is 0." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP/IP clients, TCP/IP AutoSockets, UDP/IP clients and UDP/IP PairSockets.")]
		public int RemotePort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "LocalInterface", ShortName = "li", Description =
			"The desired local IP interface. Must be a valid IPv4 or IPv6 address or an alias or reserved address like:" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Any>' (any IPv4 or IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Loopback>' (IPv4 or IPv6 loopback)" + EnvironmentEx.NewLineConstWorkaround +
			"> '0.0.0.0' (any IPv4 interface) or '::' (any IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '127.0.0.1' (IPv4 loopback) or '::1' (IPv6 loopback)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is '<Any>'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP/IP.")]
		public string LocalInterface;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "LocalFilter", ShortName = "lf", Description =
			"The desired local IP address filter. Must be a valid IPv4 or IPv6 address or an alias or reserved address like:" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Any>' (any IPv4 or IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Localhost>' (IPv4 or IPv6 localhost)" + EnvironmentEx.NewLineConstWorkaround +
			"> '0.0.0.0' (any IPv4 interface) or '::' (any IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '127.0.0.1' (IPv4 localhost) or '::1' (IPv6 localhost)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is '<Any>'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to UDP/IP servers and UDP/IP PairSockets.")]
		public string LocalFilter;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="System.Net.IPEndPoint.MinPort"/>, <see cref="System.Net.IPEndPoint.MaxPort"/> and <see cref="MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "LocalPort", ShortName = "lp", Description =
			"The desired local TCP or UDP port. Valid values are 0 through 65535. The default value is 0." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP/IP servers, TCP/IP AutoSockets, UDP/IP servers and UDP/IP PairSockets.")]
		public int LocalPort;

		/// <remarks>
		/// Name is intentionally written 'TCP' instead of 'Tcp' for better readability.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Tcp' like this...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "TCPAutoReconnect", ShortName = "tar", Description =
			"When connection is lost, try to reconnect every given milliseconds. " +
			"Must be a positive integral value equal or greater than 100. A common value is 500. " +
			"The special value 0 indicates disabled, which also is the default setting." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP/IP clients.")]
		public int TcpAutoReconnect;

		/// <remarks>
		/// Name is intentionally written 'UDP' instead of 'Udp' for better readability.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Udp' like this...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "UDPServerSendMode", ShortName = "ussm", Description =
			"The send mode of a UDP/IP Server: " + EnvironmentEx.NewLineConstWorkaround +
			"0 = none = send is disabled." + EnvironmentEx.NewLineConstWorkaround +
			"1 = first = send to the first active client." + EnvironmentEx.NewLineConstWorkaround +
			"2 = most recent = send to the most recently active client." + EnvironmentEx.NewLineConstWorkaround +
			"The default value 2 = most recent." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to UDP/IP servers.")]
		public int UdpServerSendMode = 2;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="MKY.IO.Usb.DeviceInfo.FirstVendorIdString"/> and <see cref="MKY.IO.Usb.DeviceInfo.LastVendorIdString"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "VendorID", ShortName = "VID", Description =
			"The desired USB device vendor ID (VID). Must be a hexadecimal value from 0000 to FFFF. The default value is the VID of the first device currently found." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string VendorId;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="MKY.IO.Usb.DeviceInfo.FirstProductIdString"/> and <see cref="MKY.IO.Usb.DeviceInfo.LastProductIdString"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "ProductID", ShortName = "PID", Description =
			"The desired USB device product ID (PID). Must be a hexadecimal value from 0000 to FFFF. The default value is the PID of the first device currently found." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string ProductId;

		/// <summary></summary>
		[CLSCompliant(false)]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Names = new string[] { "SerialString", "SerialNumber" }, ShortName = "SNR", Description =
			"The desired USB device serial string -aka- serial number (SNR). The default value is the SNR of the first device currently found." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string SerialString;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "FormatPreset", ShortName = "fp", Description =
			"One of the presets to specify the report format USB Ser/HID. Valid values are " + SerialHidReportFormatPresetEx.UserSummary + "." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string FormatPreset;

		/// <remarks>
		/// Name is intentionally written 'USB' instead of 'Usb' for better readability.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, StyleCop, read above regarding 'Usb' ...")]
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "NoUSBAutoOpen", ShortName = "nuao", Description =
			"When USB device is connected, don't automatically open it. By default, this feature is enabled." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public bool NoUsbAutoOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "OpenTerminal", "StartTerminal" }, ShortNames = new string[] { "ot", "st" }, Description = "Open/start the terminal.")]
		public bool StartTerminal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "KeepTerminalClosed", "KeepTerminalStopped" }, ShortNames = new string[] { "ktc", "kts" }, Description =
			"Keep terminal(s) closed/stopped, even if settings request to open/start." + EnvironmentEx.NewLineConstWorkaround +
			"This option overrides the 'OpenTerminal/StartTerminal' option if both options are given.")]
		public bool KeepTerminalStopped;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Log", "LogOn" }, ShortName = "ln", Description = "Switch logging on.")]
		public bool LogOn;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "KeepLogOff", ShortName = "klf", Description =
			"Keep log(s) switched off, even if settings request to switch on." + EnvironmentEx.NewLineConstWorkaround +
			"This option overrides the 'Log/LogOn' option if both options are given.")]
		public bool KeepLogOff;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Horizontal", "TileHorizontal" }, ShortName = "th", Description = "Tile the terminals horizontally after having openend a workspace.")]
		public bool TileHorizontal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Vertical", "TileVertical" }, ShortName = "tv", Description = "Tile the terminals vertically after having openend a workspace.")]
		public bool TileVertical;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "DynamicIndex", "DynamicTerminalIndex" }, ShortName = "di", Description =
			"Perform any requested operation on the terminal with the given dynamic index within the opening workspace." + EnvironmentEx.NewLineConstWorkaround +
			"Valid values are 1 based indices 1, 2, 3,... up to the number of open terminals. " +
			"0 indicates that the currently active terminal is used, which typically is the last terminal opened. " +
			"The default value is 0." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies when opening a workspace that contains more than one terminal." + EnvironmentEx.NewLineConstWorkaround + EnvironmentEx.NewLineConstWorkaround +
			"In addition, -1 may be used to indicate 'none', i.e. no operation is performed at all. " +
			"This setting is useful to temporarily switch off the requested operation without having to completely edit the command line.")]
		public int RequestedDynamicTerminalIndex = Indices.DefaultDynamicIndex;

		/// <remarks>Using term 'Transmit' to indicate potential 'intelligence' to send + receive/verify the data.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "TransmitText", ShortName = "tt", Description = "Automatically transmit the given text using the terminal specified.")]
		public string RequestedTransmitText;

		/// <remarks>Using term 'Transmit' to indicate potential 'intelligence' to send + receive/verify the data.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "TransmitFile", ShortName = "tf", Description = "Automatically transmit the given file using the terminal specified.")]
		public string RequestedTransmitFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "OperationDelay", ShortName = "od", Description =
			"Delay operation by the given milliseconds. Useful to keep the application open until the operation has completed. " +
			"The default value is 500." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies when performing an operation like 'TransmitText' or 'TransmitFile'.")]
		public int OperationDelay = 500;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "ExitDelay", ShortName = "ed", Description =
			"Delay exit by the given milliseconds. Useful to keep the application open until the operation has completed. " +
			"The default value is 500." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies when performing an operation like 'TransmitText' or 'TransmitFile' and 'KeepOpen' is not enabled.")]
		public int ExitDelay = 500;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "KeepOpen", ShortName = "kp", Description = "Keep " + ApplicationEx.ProductNameConstWorkaround + " open after performing the requested operation.")]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "KeepOpenOnError", ShortName = "ke", Description = "Keep " + ApplicationEx.ProductNameConstWorkaround + " open in case there is an error while performing the requested operation.")]
		public bool KeepOpenOnError;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "Empty", ShortName = "e", Description = "Start " + ApplicationEx.ProductNameConstWorkaround + " but neither show any dialog nor perform any operation.")]
		public bool Empty;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "NonInteractive", ShortName = "ni", Description = "Run the " + ApplicationEx.ProductNameConstWorkaround + " application without any user or other interaction, even in case of errors." + EnvironmentEx.NewLineConstWorkaround +
			"For " + ApplicationEx.ProductNameConstWorkaround + ".exe, interaction is enabled by default." + EnvironmentEx.NewLineConstWorkaround +
			"For " + ApplicationEx.ProductNameConstWorkaround + "Console.exe, interaction is always disabled, i.e. this option has no effect.")]
		public bool NonInteractive;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args, true, true, false)
		{
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Processes the command line options and validates them.
		/// </summary>
		protected override bool Validate()
		{
			bool isValid = base.Validate();

			// RequestedFilePath as value argument:
			if (string.IsNullOrEmpty(RequestedFilePath))
			{
				if (ValueArgsCount > 0)
					RequestedFilePath = ValueArgs[0];
			}

			// RequestedFilePath:
			if (!string.IsNullOrEmpty(RequestedFilePath))
			{
				string filePath = Environment.ExpandEnvironmentVariables(RequestedFilePath);
				if (File.Exists(filePath)) // May be absolute or relative to current directory.
				{
					if (!ExtensionHelper.IsWorkspaceFile(filePath) &&
						!ExtensionHelper.IsTerminalFile (filePath))
					{
						RequestedFilePath = null;
						Invalidate("Requested file is neither a workspace nor a terminal file");
						BooleanEx.ClearIfSet(ref isValid);
					}
				}
				else
				{
					RequestedFilePath = null;
					Invalidate("Requested file does not exist");
					BooleanEx.ClearIfSet(ref isValid);
				}
			}

			// Recent:
			if (MostRecentIsRequested)
			{
				// Application settings can only be accessed after they have been created/loaded.
				// However, this validation method may be called before the settings are available.
				if (ApplicationSettings.LocalUserSettingsAreAvailable)
				{
					ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();
					if (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0)
					{
						string mostRecent = ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[0];
						if (File.Exists(mostRecent))
						{
							if (ExtensionHelper.IsWorkspaceFile(mostRecent) ||
								ExtensionHelper.IsTerminalFile(mostRecent))
							{
								MostRecentFilePath = mostRecent;
							}
							else
							{
								Invalidate("Requested file is no workspace nor terminal file");
								BooleanEx.ClearIfSet(ref isValid);
							}
						}
						else
						{
							Invalidate("Requested file does not exist");
							BooleanEx.ClearIfSet(ref isValid);
						}
					}
				}
			}

			// TransmitText:
			if (!string.IsNullOrEmpty(RequestedTransmitText))
			{
				string errorMessage;
				if (!ValidationHelper.ValidateText("text to send", RequestedTransmitText, out errorMessage))
				{
					RequestedTransmitText = null;
					Invalidate(errorMessage);
					BooleanEx.ClearIfSet(ref isValid);
				}
			}

			// TransmitFile:
			if (!string.IsNullOrEmpty(RequestedTransmitFilePath))
			{
				string filePath = Environment.ExpandEnvironmentVariables(RequestedTransmitFilePath);
				if (!File.Exists(filePath)) // May be absolute or relative to current directory.
				{
					RequestedTransmitFilePath = null;
					Invalidate("Requested file does not exist");
					BooleanEx.ClearIfSet(ref isValid);
				}
			}

			// Tile:
			if (TileHorizontal && TileVertical) // Must be mutual exclusive.
			{
				TileHorizontal = false;
				TileVertical   = false;
				Invalidate("Tile horizontal and vertical simultaneously is not possible");
				BooleanEx.ClearIfSet(ref isValid);
			}

			return (isValid);
		}

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public override string GetHelpText(int maxWidth)
		{
			string anyWorkspace = "<Workspace>" + ExtensionHelper.WorkspaceFile;
			string anyTerminal  = "<Terminal>"  + ExtensionHelper.TerminalFile;

			string myWorkspace = "MyWorkspace" + ExtensionHelper.WorkspaceFile;
			string myTerminal  = "MyTerminal"  + ExtensionHelper.TerminalFile;

			string name = ApplicationEx.ProductName;
			StringBuilder helpText = new StringBuilder();

			helpText.AppendLine(                                 "Usage:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + "[.exe] [" + anyWorkspace + "|" + anyTerminal + "] [<Options>]"));
			helpText.AppendLine();
			helpText.AppendLine();
			helpText.AppendLine(                                 "Usage examples:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " " + myWorkspace));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " and open the given workspace."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " " + myTerminal));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " and open the given terminal."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " " + myTerminal + " /b=19200"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " with the given terminal, but change the baud rate to 19200 baud."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /r"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " and open the most recent file."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /n /p=1 /b=19200"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " and create a new terminal on COM1 using 19200 baud."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " and show the 'New Terminal' dialog."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /e"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,        "Start " + ApplicationEx.ProductName + " but neither show any dialog nor perform any operation."));
			helpText.AppendLine();

			helpText.Append(base.GetHelpText(maxWidth));

			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, "Files can be provided with an absolute or relative path. Relative paths are relative to the current working directory. Paths containing spaces must be surrounded with quotes."));
			helpText.AppendLine();

			return (helpText.ToString());
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns whether user or other interaction shall be permitted.
		/// </summary>
		public bool Interactive
		{
			get { return (!(NonInteractive)); }
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
