//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//css_reference MKY.dll;
//css_reference MKY.IO.Ports.dll;
//css_reference MKY.IO.Serial.SerialPort.dll;
//css_reference MKY.IO.Serial.Socket.dll;

//css_reference YAT.Domain.dll;
//css_reference YAT.Model.Base.dll;
//css_reference YAT.Settings.Application.dll;
//css_reference YAT.Settings.Model.dll;

namespace YATTools.Settings.DocklightToYAT
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	using MKY.Net;
	using MKY.IO;
	using MKY.IO.Serial.SerialPort;
	using MKY.IO.Serial.Socket;
	using MKY.Settings;

	using YAT.Domain.Settings;
	using YAT.Model.Settings;
	using YAT.Model.Types;
	using YAT.Settings.Application;
	using YAT.Settings.Model;

	public static class Convert
	{
		/// <summary>
		/// The return value on success.
		/// </summary>
		public const int Success = 0;

		/// <summary>
		/// Script entry point.
		/// </summary>
		/// <param name="args">
		/// <list type="number">
		/// <item><description>The absolute or relative path to a Docklight settings file (.ptp)</description></item>
		/// <item><description>The absolute or relative path for converting the YAT terminal file (.yat) to</description></item>
		/// </list>
		/// </param>
		/// <returns>
		/// Script result:
		/// <list type="bullet">
		/// <item><description>Value of 0 (<see cref="Success"/>) = successful script run.</description></item>
		/// <item><description>Values above 0 = non-successful file handling or conversion.</description></item>
		/// <item><description>Values below 0 = invalid arguments.</description></item>
		/// </list>
		/// </returns>
		public static int Main(string[] args)
		{
			string inputFilePath, outputFilePath;
			int result = ProcessArgs(args, out inputFilePath, out outputFilePath);
			if (result != Success)
			{
				Console.WriteLine("Failed to process args!");
				return (result);
			}

			string version;
			string[] commSettings;
			string[] commChannels;
			List<string[]> sendButtons;
			if (!TryReadDocklightSections(inputFilePath, out version, out commSettings, out commChannels, out sendButtons))
			{
				Console.WriteLine(@"Failed to read Docklight sections from ""{0}""!", inputFilePath);
				return (1);
			}

			int versionAsInt;
			if (!int.TryParse(version, out versionAsInt))
			{
				Console.WriteLine(@"""{0}"" does not seem to be a Docklight settings file!", inputFilePath);
				return (2);
			}

			IOSettings ioSettings;
			if (!TryParseComm(commSettings, commChannels, out ioSettings))
			{
				Console.WriteLine(@"Failed to parse COMM* content from ""{0}""!", inputFilePath);
				return (3);
			}

			PredefinedCommandPage predefinedCommandPage;
			if (!TryParseSend(sendButtons, out predefinedCommandPage))
			{
				Console.WriteLine(@"Failed to parse SEND content from ""{0}""!", inputFilePath);
				return (4);
			}

		////if (!TryParseReceive(receive)) is not supported by this script (yet).

			ApplicationSettings.Create(ApplicationSettingsFileAccess.None); // Required to create a terminals settings object.

			var yatSettings = new TerminalSettingsRoot();
			yatSettings.IO = ioSettings;
			var predefinedCommand = new PredefinedCommandSettings();
			predefinedCommand.PageLayout = PredefinedCommandPageLayoutEx.GetMatchingItem(predefinedCommandPage.DefinedCommandCount);
			predefinedCommand.Pages.Add(predefinedCommandPage);
			yatSettings.PredefinedCommand = predefinedCommand;

			var yatSettingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>(yatSettings);
			yatSettingsHandler.SettingsFilePath = outputFilePath;
			yatSettingsHandler.Save();

			return (Success);
		}

		private static int ProcessArgs(string[] args, out string inputFilePath, out string outputFilePath)
		{
			inputFilePath = null;
			outputFilePath = null;

			if (args.Length < 2)
			{
				Console.WriteLine("Invalid number of args! Script must always be called with 2 args.");
				return (-1);
			}

			if (!File.Exists(args[0]))
			{
				Console.WriteLine(@"Invalid input file! File ""{0}"" does not exist!", args[0]);
				return (-2);
			}

			inputFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(args[0]);

			if (!PathEx.IsValid(args[1]))
			{
				Console.WriteLine(@"Invalid output path! Path ""{0}"" is not valid!", args[1]);
				return (-3);
			}

			outputFilePath = PathEx.GetNormalizedRootedExpandingEnvironmentVariables(args[1]);

			return (Success);
		}

		/// <remarks>
		/// Using explicit section parameters rather than a generic collection like e.g. a
		/// <see cref="System.Linq.Lookup{TKey, TElement}"/> or
		/// <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/> for two reasons;
		/// <list type="bullet">
		/// <item><description>More strictly typed interface, more obvious to use.</description></item>
		/// <item><description>Better suitable for handling different type of sections.</description></item>
		/// </list>
		/// </remarks>
		private static bool TryReadDocklightSections(string inputFilePath,
			out string version, out string[] commSettings, out string[] commChannels, out List<string[]> send)
		{
			version      = null;
			commSettings = null;
			commChannels = null;
			send         = new List<string[]>();

			using (var fs = new StreamReader(inputFilePath, Encoding.Default, false))
			{
				string sectionName = null;
				List<string> sectionContent = new List<string>();

				string line;
				while ((line = fs.ReadLine()) != null)
				{
					if (!line.IsEmptyOrWhiteSpace())
					{
						// Start a section or add content to it:
						if (sectionName == null)
							sectionName = line;
						else
							sectionContent.Add(line);
					}
					else
					{
						// Commit a completed section:
						if (sectionName != null)
						{
							if (sectionContent.Count < 1)
							{
								Console.WriteLine(@"Section ""{0}"" is incomplete!", sectionName);
								return (false);
							}

							switch (sectionName.ToUpperInvariant()) // Section names should already be UPPER, but one can never know...
							{
								case "VERSION":
								{
									if (sectionContent.Count == 1)
									{
										version = sectionContent[0];
										break;
									}
									else
									{
										Console.WriteLine(@"Section ""{0}"" is too long!", sectionName);
										return (false);
									}
								}

								case "COMMSETTINGS":
								{
									commSettings = sectionContent.ToArray();
									break;
								}

								case "COMMCHANNELS":
								{
									commChannels = sectionContent.ToArray();
									break;
								}

								case "SEND":
								{
									send.Add(sectionContent.ToArray());
									break;
								}

								default: // COMMDISPLAY, VERSATAP, CHANNELALIAS, RECEIVE
								{
									Console.WriteLine(@"Section ""{0}"" is not (yet) supported by this script and will be ignored.", sectionName);
									break;
								}
							}

							sectionName = null;
							sectionContent.Clear();
						}
					}
				}
			}

			return (true);
		}

		/// <remarks>
		/// Comprehensibility method.
		/// </remarks>
		private static bool IsEmptyOrWhiteSpace(this string str)
		{
			return (string.IsNullOrWhiteSpace(str));
		}

		private static bool TryParseComm(string[] commSettings, string[] commChannels, out IOSettings ioSettings)
		{
			MKY.IO.Ports.SerialPortId portId;                                    // applies to version 7
			if ((commChannels.Length == 0) || MKY.IO.Ports.SerialPortId.TryParse(commChannels[0], out portId))
			{                      // \\ applies to version 8
				if (commSettings.Length != 9)
				{
					Console.WriteLine("COMMSETTINGS section does not consist of 9 entries!");
					ioSettings = null;
					return (false);
				}
				else
				{
					// COMMSETTINGS
					// 0    -> mode (0 = send/receive => single port, 1 = monitoring => port pair)
					// COM1 -> main port
					// COM2 -> aux port (in use for monitoring)
					// 9600 -> baud rate
					// 2    -> parity (0..4 = even/mark/none/odd/space)
					// 0    -> partiy error char
					// 4    -> data bits (0..4 = 4/5/6/7/8)
					// 0    -> stop bits (0..2 = 1/1.5/2)
					// 0    -> flow control (0..4 = off/manual HW/HW/SW/RS485)

					int mode;
					if (!int.TryParse(commSettings[0], out mode))
					{
						Console.WriteLine("COMMSETTINGS entry #1 value = {0} is an invalid mode!", commSettings[0]);
						ioSettings = null;
						return (false);
					}

					if (mode != 0)
					{
						Console.WriteLine("Script (so far) only supports COMMSETTINGS entry #1 mode = 0 (send/receive) but mode = {0}!", commSettings[0]);
						ioSettings = null;
						return (false);
					}

					if (!MKY.IO.Ports.SerialPortId.TryParse(commSettings[1], out portId))
					{
						Console.WriteLine("COMMSETTINGS entry #2 value = {0} is an invalid serial COM port ID!", commSettings[1]);
						ioSettings = null;
						return (false);
					}

					// commSettings[2] 'aux port' does not need to be considered (yet).

					int baudRate;
					if (!int.TryParse(commSettings[3], out baudRate))
					{
						Console.WriteLine("COMMSETTINGS entry #4 value = {0} is an invalid baud rate value!", commSettings[3]);
						ioSettings = null;
						return (false);
					}

					int parityDocklight;
					if (!int.TryParse(commSettings[4], out parityDocklight) || (parityDocklight < 0) || (parityDocklight > 4))
					{
						Console.WriteLine("COMMSETTINGS entry #5 value = {0} is an invalid parity value!", commSettings[4]);
						ioSettings = null;
						return (false);
					}

					System.IO.Ports.Parity parity;
					switch (parityDocklight)
					{
						case 0: parity = System.IO.Ports.Parity.Even;  break;
						case 1: parity = System.IO.Ports.Parity.Mark;  break;
						case 2: parity = System.IO.Ports.Parity.None;  break;
						case 3: parity = System.IO.Ports.Parity.Odd;   break;
						case 4: parity = System.IO.Ports.Parity.Space; break;

						default: throw (new InvalidOperationException("'parityDocklight' cannot have value " + parityDocklight + " here!"));
					}

					// commSettings[5] 'parity error char' is not considered (yet).

					int dataBitsDocklight;
					if (!int.TryParse(commSettings[6], out dataBitsDocklight) || (dataBitsDocklight < 0) || (dataBitsDocklight > 4))
					{
						Console.WriteLine("COMMSETTINGS entry #7 value = {0} is an invalid data bits value!", commSettings[6]);
						ioSettings = null;
						return (false);
					}

					MKY.IO.Ports.DataBits dataBits;
					switch (dataBitsDocklight)
					{
						case 0:
						{
							Console.WriteLine("COMMSETTINGS entry #7 value = 0 (stop bits = 4) is not supported by YAT!");
							ioSettings = null;
							return (false);
						}

						case 1: dataBits = MKY.IO.Ports.DataBits.Five;  break;
						case 2: dataBits = MKY.IO.Ports.DataBits.Six;   break;
						case 3: dataBits = MKY.IO.Ports.DataBits.Seven; break;
						case 4: dataBits = MKY.IO.Ports.DataBits.Eight; break;

						default: throw (new InvalidOperationException("'dataBitsDocklight' cannot have value " + dataBitsDocklight + " here!"));
					}

					int stopBitsDocklight;
					if (!int.TryParse(commSettings[7], out stopBitsDocklight) || (stopBitsDocklight < 0) || (stopBitsDocklight > 2))
					{
						Console.WriteLine("COMMSETTINGS entry #8 value = {0} is an invalid stop bits value!", commSettings[7]);
						ioSettings = null;
						return (false);
					}

					System.IO.Ports.StopBits stopBits;
					switch (stopBitsDocklight)
					{
						case 0: stopBits = System.IO.Ports.StopBits.One;          break;
						case 1: stopBits = System.IO.Ports.StopBits.OnePointFive; break;
						case 2: stopBits = System.IO.Ports.StopBits.Two;          break;

						default: throw (new InvalidOperationException("'stopBitsDocklight' cannot have value " + stopBitsDocklight + " here!"));
					}

					int flowControlDocklight;
					if (!int.TryParse(commSettings[8], out flowControlDocklight) || (flowControlDocklight < 0) || (flowControlDocklight > 4))
					{
						Console.WriteLine("COMMSETTINGS entry #9 value = {0} is an invalid flowControl value!", commSettings[8]);
						ioSettings = null;
						return (false);
					}

					SerialFlowControl flowControl;
					switch (flowControlDocklight)
					{
						case 0: flowControl = SerialFlowControl.None;           break;
						case 1: flowControl = SerialFlowControl.ManualHardware; break;
						case 2: flowControl = SerialFlowControl.Hardware;       break;
						case 3: flowControl = SerialFlowControl.Software;       break;
						case 4: flowControl = SerialFlowControl.RS485;          break;

						default: throw (new InvalidOperationException("'flowControlDocklight' cannot have value " + flowControlDocklight + " here!"));
					}

					var communication = new SerialCommunicationSettings(baudRate, dataBits, parity, stopBits, flowControl);
					ioSettings = new IOSettings();
					ioSettings.SerialPort = new SerialPortSettings(portId, communication);
					return (true);
				}
			}
			else // TCP or UDP?
			{
				var split = commChannels[0].Split(':');
				if (split.Length > 1)
				{
					if (split[0].Equals("LOCALHOST", StringComparison.InvariantCultureIgnoreCase)) // LOCALHOST:10001
					{
						int remoteTcpPort;
						if (int.TryParse(split[1], out remoteTcpPort) && (remoteTcpPort >= 0) && (remoteTcpPort <= 65535))
						{
							ioSettings = new IOSettings();
							ioSettings.Socket = new SocketSettings(SocketType.TcpClient, (IPHostEx)IPHost.IPv4Localhost, remoteTcpPort, 0);
							return (true);
						}
						else
						{
							Console.WriteLine("LOCALHOST value {0} is an invalid TCP port!", split[1]);
							ioSettings = null;
							return (false);
						}
					}
					else if (split[0].Equals("SERVER", StringComparison.InvariantCultureIgnoreCase)) // SERVER:10001
					{
						int localTcpPort;
						if (int.TryParse(split[1], out localTcpPort) && (localTcpPort >= 0) && (localTcpPort <= 65535))
						{
							ioSettings = new IOSettings();
							ioSettings.Socket = new SocketSettings(SocketType.TcpServer, SocketSettings.RemoteHostDefault, 0, 0, SocketSettings.LocalInterfaceDefault, SocketSettings.LocalFilterDefault, localTcpPort, 0);
							return (true);
						}
						else
						{
							Console.WriteLine("SERVER value {0} is an invalid TCP port!", split[1]);
							ioSettings = null;
							return (false);
						}
					}
					else if (split[0].Equals("UDP", StringComparison.InvariantCultureIgnoreCase))
					{
						if (split.Length == 3) // UDP:LOCALHOST:5000
						{
							int remoteUdpPort;
							if (int.TryParse(split[2], out remoteUdpPort) && (remoteUdpPort >= 0) && (remoteUdpPort <= 65535))
							{
								ioSettings = new IOSettings();
								ioSettings.Socket = new SocketSettings(SocketType.UdpClient, SocketSettings.RemoteHostDefault, 0, remoteUdpPort, SocketSettings.LocalInterfaceDefault, SocketSettings.LocalFilterDefault, 0, 0);
								return (true);
							}
							else
							{
								Console.WriteLine("UDP value {0} is an invalid UDP port!", split[2]);
								ioSettings = null;
								return (false);
							}
						}
						else if (split.Length == 4) // UDP:LOCALHOST:5001:5002
						{
							int remoteUdpPort;
							if (int.TryParse(split[2], out remoteUdpPort) && (remoteUdpPort >= 0) && (remoteUdpPort <= 65535))
							{
								int localUdpPort;
								if (int.TryParse(split[3], out localUdpPort) && (localUdpPort  >= 0) && (localUdpPort <= 65535))
								{
									ioSettings = new IOSettings();
									ioSettings.Socket = new SocketSettings(SocketType.UdpPairSocket, SocketSettings.RemoteHostDefault, 0, remoteUdpPort, SocketSettings.LocalInterfaceDefault, SocketSettings.LocalFilterDefault, 0, localUdpPort);
									return (true);
								}
								else
								{
									Console.WriteLine("UDP value {0} is an invalid UDP port!", split[3]);
									ioSettings = null;
									return (false);
								}
							}
							else
							{
								Console.WriteLine("UDP value {0} is an invalid UDP port!", split[2]);
								ioSettings = null;
								return (false);
							}
						}
					}
				}
			}

			Console.WriteLine("Failed to parse I/O type from COMMCHANNELS!");
			ioSettings = null;
			return (false);
		}

		private static bool TryParseSend(List<string[]> sendButtons, out PredefinedCommandPage predefinedCommandPage)
		{
			predefinedCommandPage = new PredefinedCommandPage("Converted from Docklight");

			for (int i = 0; i < sendButtons.Count; i++)
			{
				var send = sendButtons[i];
				if (send.Length != 5)
				{
					Console.WriteLine("SEND section #{0} does not consist of 5 entries!", i);
					return (false);
				}
				else
				{
					// SEND
					// 0                             -> index
					// Ping                          -> description
					// 2D 2D 2D 2D 6F 20 50 69 6E 67 -> data
					// 0                             -> ???
					// 5                             -> ???
					//
					// SEND
					// 1                             -> index
					// Pong                          -> description
					// 6F 2D 2D 2D 2D 20 50 6F 6E 67 -> data
					// 0                             -> ???
					// 5                             -> ???

					int index;
					if (!int.TryParse(send[0], out index) && (index < 0) && (index >= PredefinedCommandPage.MaxCommandCapacityPerPage))
					{
						Console.WriteLine("SEND section #{0} entry #1 value = {1} does not define a valid index!", i, send[0]);
						return (false);
					}

					string description = send[1];
					string textLine = @"\h(" + send[2] + ")";

					var command = new Command(description, textLine);
					predefinedCommandPage.SetCommand(index, command);
				}
			}

			return (true);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
