//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.CommandLine;

using YAT.Settings;
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
		[ValueArg(Description = "Open the given YAT workspace (.yaw) or terminal (.yat).")]
		[OptionArg(Name = "Open", ShortName = "o", Description = "Open the given YAT workspace (.yaw) or terminal (.yat).")]
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
		[OptionArg(Name = "TerminalType", ShortName = "ty", Description =
			"The desired terminal type. Valid values are 'Text'/'T' or 'Binary'/'B'. The default value is 'Text'." + EnvironmentEx.NewLineConstWorkaround +
			"Example: 'TerminalType=Text' or 'TerminalType=T' or 'ty=t'")]
		public string TerminalType;

		/// <summary></summary>
		/// <remarks>
		/// This option is intentionally called 'PortType' because it shall match the name on the 'New Terminal' and 'Terminal Settings' dialog.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "PortType", ShortName = "pt", Description =
			"The desired port type. Valid values are:" + EnvironmentEx.NewLineConstWorkaround +
			"- 'COM' (Serial Port)" + EnvironmentEx.NewLineConstWorkaround +
			"- 'TCPClient', 'TCPServer', 'TCPAutoSocket' (TCP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"- 'UDP' (UDP/IP Socket)" + EnvironmentEx.NewLineConstWorkaround +
			"- 'USBSerHID' (USB Ser/HID)" + EnvironmentEx.NewLineConstWorkaround +
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
			"The desired method of flow control. Valid values are 'None', 'Hardware' (RFR/CTS), 'Software' (XOn/XOff), 'Combined' (RFR/CTS and XOn/XOff), " +
			"'ManualHardware', 'ManualSoftware', 'ManualCombined' and 'RS485' (RS-485 Transceiver Control). The default value is 'None'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to serial COM ports.")]
		public string FlowControl;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "RemoteHost", ShortName = "rh", Description =
			"The desired remote IP host. Must be a valid IPv4 or IPv6 address or an alias like 'localhost'. The default value is 'localhost'." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP or UDP terminals.")]
		public string RemoteHost;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="System.Net.IPEndPoint.MinPort"/>, <see cref="System.Net.IPEndPoint.MaxPort"/> and <see cref="MKY.IO.Serial.Socket.SocketSettings.DefaultPort"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "RemotePort", ShortName = "rp", Description =
			"The desired remote TCP or UDP port. Valid values are 0 through 65535. The default value is 0." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP clients, TCP AutoSockets and UDP.")]
		public int RemotePort;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "LocalInterface", ShortName = "li", Description =
			"The desired local IP interface. Must be a valid IPv4 or IPv6 address or an alias or reserved address like:" + EnvironmentEx.NewLineConstWorkaround +
			"- '<Any>' (any IPv4 or IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"- '0.0.0.0' (any IPv4 interface) or '::' (any IPv6 interface)" + EnvironmentEx.NewLineConstWorkaround +
			"- '127.0.0.1' (IPv4 loopback) or '::1' (IPv6 loopback)" + EnvironmentEx.NewLineConstWorkaround +
			"The default value is '<Any>'. Only applies to TCP or UDP terminals.")]
		public string LocalInterface;

		/// <remarks>
		/// The values in the description must be provided directly instead of referencing their respective code items
		/// <see cref="System.Net.IPEndPoint.MinPort"/>, <see cref="System.Net.IPEndPoint.MaxPort"/> and <see cref="MKY.IO.Serial.Socket.SocketSettings.DefaultPort"/>
		/// because attribute arguments must be constant.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "LocalPort", ShortName = "lp", Description =
			"The desired local TCP or UDP port. Valid values are 0 through 65535. The default value is 0." + EnvironmentEx.NewLineConstWorkaround +
			"Only applies to TCP servers, TCP AutoSockets and UDP.")]
		public int LocalPort;

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
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "SerialPortAutoReopen", ShortName = "npar", Description =
			"When device is lost, e.g. a USB/Serial converter, try to reopen the port every given milliseconds. Must be positive integral value equal or greater than 100. A common value is 2000. Special value 0 means disabled." + EnvironmentEx.NewLineConstWorkaround +
			"By default, this feature is enabled and set to 2000 milliseconds. Only applies to serial COM ports.")]
		public int SerialPortAutoReopen;

		/// <remarks>
		/// Name is intentionally written 'TCP' instead of 'Tcp' for better readability.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "TCPAutoReconnect", ShortName = "tar", Description =
			"When connection is lost, try to reconnect every given milliseconds. Must be positive integral value equal or greater than 100. A common value is 500. Special value 0 means disabled." + EnvironmentEx.NewLineConstWorkaround +
			"By default, this feature is disabled. Only applies to TCP clients.")]
		public int TcpAutoReconnect;

		/// <remarks>
		/// Name is intentionally written 'USB' instead of 'Usb' for better readability.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "NoUSBAutoOpen", ShortName = "nuao", Description =
			"When USB device is connected, don't automatically open it." + EnvironmentEx.NewLineConstWorkaround +
			"By default, this feature is enabled. Only applies to USB Ser/HID.")]
		public bool NoUsbAutoOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "OpenTerminal", ShortName = "ot", Description = "Open the terminal.")]
		public bool OpenTerminal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Log", "BeginLog" }, ShortName = "bl", Description = "Begin logging.")]
		public bool BeginLog;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Horizontal", "TileHorizontal" }, ShortName = "th", Description = "Tile the terminals horizontal after having openend a workspace.")]
		public bool TileHorizontal;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Vertical", "TileVertical" }, ShortName = "tv", Description = "Tile the terminals vertical after having openend a workspace.")]
		public bool TileVertical;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "Terminal", ShortName = "t", Description =
			"Perform any requested operation on the terminal with the given dynamic index within the opening workspace." + EnvironmentEx.NewLineConstWorkaround +
			"Valid values are 1 based indices 1, 2, 3,... up to the number of open terminals. " +
			"0 indicates that the currently active terminal is used, which typically is the last terminal opened. " +
			"-1 indicates 'none', i.e. no operation is performed at all. The default value is -1." + EnvironmentEx.NewLineConstWorkaround +
			"This option is useful to temporarily switch off any option without having to edit the command line. " +
			"This option only has an effect when opening a workspace that contains more than one terminal.")]
		public int RequestedDynamicTerminalIndex = Indices.InvalidDynamicIndex;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "TransmitFile", ShortName = "tf", Description = "Automatically transmit the given file using the terminal specified.")]
		public string RequestedTransmitFilePath;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "Empty", ShortName = "e", Description = "Start YAT but neither show any dialog nor perform any operation.")]
		public bool Empty;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "KeepOpen", ShortName = "kp", Description = "Keep YAT open after performing the requested operation.")]
		public bool KeepOpen;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "KeepOpenOnError", ShortName = "ke", Description = "Keep YAT open in case there is an error while performing the requested operation.")]
		public bool KeepOpenOnError;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = VisibilitySuppressionJustification)]
		[OptionArg(Name = "NonInteractive", ShortName = "ni", Description = "Run the YAT application without any user or other interaction, even in case of errors." + EnvironmentEx.NewLineConstWorkaround +
			"For YAT.exe, interaction is enabled by default." + EnvironmentEx.NewLineConstWorkaround +
			"For YATConsole.exe, interaction is disabled by default.")]
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
				if (File.Exists(RequestedFilePath))
				{
					if (!ExtensionSettings.IsWorkspaceFile(RequestedFilePath) &&
						!ExtensionSettings.IsTerminalFile (RequestedFilePath))
					{
						RequestedFilePath = null;
						Invalidate("Requested file is no workspace nor terminal file");
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
				ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();
				if (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0)
				{
					string mostRecent = ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[0];
					if (File.Exists(mostRecent))
					{
						if (ExtensionSettings.IsWorkspaceFile(mostRecent) ||
							ExtensionSettings.IsTerminalFile (mostRecent))
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

			// TransmitFile:
			if (!string.IsNullOrEmpty(RequestedTransmitFilePath))
			{
				if (!File.Exists(RequestedTransmitFilePath))
				{
					RequestedTransmitFilePath = null;
					Invalidate("Requested file does not exist");
					BooleanEx.ClearIfSet(ref isValid);
				}
			}

			// Tile:
			if ((TileHorizontal == true) && (TileVertical == true)) // Must be mutual exclusive.
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
			string name = Application.ProductName;
			StringBuilder helpText = new StringBuilder();

			helpText.AppendLine(                                "Usage:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + "[.exe] [<Workspace>.yaw|<Terminal>.yat] [<Options>]"));
			helpText.AppendLine();
			helpText.AppendLine();
			helpText.AppendLine(                                "Usage examples:");
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " MyWorkspace.yaw"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT and open the given workspace."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " MyTerminal.yat"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT and open the given terminal."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " MyTerminal.yat /b=19200"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT with the given terminal, but change the baud rate to 19200 baud."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /r"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT and open the most recent file."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /n /p=1 /b=19200"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT and create a new terminal on COM1 using 19200 baud."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT and show the 'New Terminal' dialog."));
			helpText.AppendLine();
			helpText.Append(SplitIntoLines(maxWidth, MinorIndent, name + " /e"));
			helpText.Append(SplitIntoLines(maxWidth, MajorIndent,         "Start YAT but neither show any dialog nor perform any operation."));
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
