//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY;
using MKY.CommandLine;

using YAT.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Model
{
	/// <summary></summary>
	public class CommandLineArgs : ArgsHandler
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[ValueArg(Description = "The YAT workspace (.yaw) or terminal (.yat) file to open.")]
		public string RequestedFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Recent", ShortName = "r", Description = "Open the most recent file.")]
		public bool MostRecentIsRequested;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "TerminalType", ShortName = "ty", Description =
			"The desired terminal type. Valid values are 'Text' or 'Binary'. The default value is 'Text'." + EnvironmentEx.NewLineConstWorkaround +
			"Example: 'TerminalType=Text' or 'ty=t'")]
		public Domain.TerminalTypeEx TerminalType;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "PortType", ShortName = "pt", Description =
			"The desired port type. Valid values are:" + EnvironmentEx.NewLineConstWorkaround +
			"> 'COM' (Serial Port)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'TCPClient', 'TCPServer', 'TCPAutoSocket' (TCP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'UDP' (UDP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"> 'USBSerHID' (USB Ser/HID)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is 'COM'.")]
		public Domain.IOTypeEx IOType;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "SerialPort", ShortName = "p", Description =
			"The desired serial COM port. Valid values are 1 through 65536, given the port exists on the current machine. The default value is 1." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int SerialPort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Baud", ShortName = "b", Description =
			"The desired baud rate. Must be a positive integral value that is supported by the selected serial COM port on the current machine. " +
			"Typical values are 2400, 4800, 9600, 19200, 38400, 57600 and 115200. The default value is 9600." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int Baud;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "DataBits", ShortName = "db", Description =
			"The desired number of data bits. Valid values are 4, 5, 6, 7 or 8. The default value is 8." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public int DataBits;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Parity", ShortName = "pa", Description =
			"The desired parity setting. Valid values are 'None', 'Odd', 'Even', 'Mark' and 'Space'. The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public string Parity;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "StopBits", ShortName = "sb", Description =
			"The desired number of stop bits. Valid values are 0, 1, 1.5 or 2. The default value is 1." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public double StopBits;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "FlowControl", ShortName = "fc", Description =
			"The desired method of flow control. Valid values are 'None', 'Hardware' (RTS/CTS), 'Software' (XOn/XOff), 'Combined' (RTS/CTS and XOn/XOff), " +
			"'Manual' and 'RS485' (RS-485 Transceiver Control). The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public MKY.IO.Serial.SerialFlowControl FlowControl;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "RemotePort", ShortName = "rp", Description =
			"The desired remote IP host. Must be a valid IPv4 or IPv6 address or an alias like 'localhost'. The default value is 'localhost'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP or UDP terminals.")]
		public string RemoteHost;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "RemotePort", ShortName = "rp", Description =
			"The desired remote TCP or UDP port. Valid values are 0 through 65535. The default value is 10000." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP clients, TCP AutoSockets and UDP.")]
		public int RemotePort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "LocalInterface", ShortName = "li", Description =
			"The desired local IP interface. Must be a valid IPv4 or IPv6 address or an alias or reserved address like:" + EnvironmentEx.NewLineConstWorkaround +
			"> '<Any>' (any IPv4 or IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '0.0.0.0' (any IPv4 interface) or '::' (any IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"> '127.0.0.1' (IPv4 loopback) or '::1' (IPv6 loopback)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is '<Any>'. Only applies to TCP or UDP terminals.")]
		public string LocalInterface;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "LocalPort", ShortName = "lp", Description =
			"The desired local TCP or UDP port. Valid values are 0 through 65535. The default value is 10000." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP servers, TCP AutoSockets and UDP.")]
		public int LocalPort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "VendorId", ShortName = "VID", Description =
			"The desired USB device vendor ID (VID). Must be a hexadecimal value from 0 to FFFF. The default value is the VID of the first device currently found." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string VendorId;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "ProductId", ShortName = "PID", Description =
			"The desired USB device product ID (PID). Must be a hexadecimal value from 0 to FFFF. The default value is the PID of the first device currently found." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to USB Ser/HID.")]
		public string ProductId;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "AutoReconnect", ShortName = "ar", Description =
			"When connection is lost, try to reconnect every given milliseconds. Must be positive integral value equal or greater than 100. A common value is 500." + EnvironmentEx.NewLineConstWorkaround +
			"By default, this feature is disabled. Only applies to serial COM ports and TCP clients.")]
		public int AutoReconnect;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "AutoOpen", ShortName = "ao", Description =
			"When device is connected, automatically open it." + EnvironmentEx.NewLineConstWorkaround +
			"By default, this feature is enabled. Only applies to USB Ser/HID.")]
		public bool AutoOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "Terminal", ShortName = "t", Description =
			"Perform any requested operation on the terminal with the given sequential index within the opening workspace." + EnvironmentEx.NewLineConstWorkaround +
			"Valid values are 1 based indices 1, 2, 3,... up to the number of open terminals. " +
			"0 indicates that the currently active terminal is used, which typically is the last terminal opened. " +
			"-1 indicates 'none', i.e. no operation is performed at all. This option is useful to temporarily switch off any option without having to edit the command line." + EnvironmentEx.NewLineConstWorkaround +
			"The default value is 0.")]
		public int RequestedSequentialTerminalIndex;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "TransmitFile", ShortName = "tf", Description = "Automatically transmit the given file using the terminal specified.")]
		public string RequestedTransmitFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "KeepOpen", ShortName = "kp", Description = "Keep YAT open after performing the requested operation.")]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This is a simple container for command line args.")]
		[OptionArg(Name = "KeepOpenOnError", ShortName = "ke", Description = "Keep YAT open in case there is an error while performing the requested operation.")]
		public bool KeepOpenOnError;

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args)
		{
		}

		/// <summary>
		/// Processes the command line options and validates them.
		/// </summary>
		protected override bool Validate()
		{
			bool isValid = true;

			// RequestedFilePath:
			if (File.Exists(RequestedFilePath))
			{
				if (!ExtensionSettings.IsWorkspaceFile(Path.GetExtension(RequestedFilePath)) &&
					!ExtensionSettings.IsTerminalFile (Path.GetExtension(RequestedFilePath)))
				{
					RequestedFilePath = null;
					BoolEx.ClearIfSet(ref isValid);
				}
			}
			else
			{
				RequestedFilePath = null;
				BoolEx.ClearIfSet(ref isValid);
			}

			// Recent:
			if (MostRecentIsRequested)
			{
				ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
				bool thereAreRecents = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
				if (thereAreRecents)
					RequestedFilePath = ApplicationSettings.LocalUser.RecentFiles.FilePaths[0].Item;
			}

			return (isValid);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
