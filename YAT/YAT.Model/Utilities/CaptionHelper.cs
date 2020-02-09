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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using MKY;
using MKY.Settings;

using YAT.Settings.Model;

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class CaptionHelper
	{
		/// <summary></summary>
		public static string ComposeInvariant(string indicatedName, string info)
		{
			return (ComposeInvariant(indicatedName, new string[] { info }));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "infos", Justification = "Plural of 'info'.")]
		public static string ComposeInvariant(string indicatedName, IEnumerable<string> infos)
		{
			var sb = new StringBuilder();

			// Attention:
			// Similar "[IndicatedName] - Info - Info - Info" as in...
			// ...Compose() below.
			// ...Workspace.ActiveTerminalInfoText{get}.
			// Changes here may have to be applied there too.

			sb.Append("[");
			sb.Append(indicatedName);
			sb.Append("]");

			foreach (var info in infos)
			{
				sb.Append(" - ");
				sb.Append(info);
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		public static string Compose(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, TerminalSettingsRoot settingsRoot, Domain.Terminal terminal,
		                             string indicatedName, bool isStarted, bool isOpen, bool isConnected)
		{
			var sb = new StringBuilder();

			// Attention:
			// Similar "[IndicatedName] - Info - Info - Info" as in...
			// ...ComposeInvariant() above.
			// ...Workspace.ActiveTerminalInfoText{get}.
			// Changes here may have to be applied there too.

			if ((settingsHandler == null) || (settingsRoot == null))
			{
				sb.Append("[");
				sb.Append(indicatedName);
				sb.Append("]");
			}
			else
			{
				sb.Append("[");
				{
					if (settingsHandler.SettingsFileIsReadOnly)
						sb.Append("#");

					sb.Append(indicatedName);

					if (settingsHandler.SettingsFileIsReadOnly)
						sb.Append("#");

					if (settingsRoot.ExplicitHaveChanged)
						sb.Append(" *");
				}
				sb.Append("]");

				switch (settingsRoot.IOType)
				{
					case Domain.IOType.SerialPort:
					{
						string portNameAndCaption;
						bool autoReopenEnabled;

						var port = (terminal.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
						if (port != null) // Effective settings from port object:
						{
							var s = port.Settings;
							portNameAndCaption = s.PortId.ToNameAndCaptionString();
							autoReopenEnabled  = s.AutoReopen.Enabled;
						}
						else // Fallback to settings object tree:
						{
							var s = settingsRoot.IO.SerialPort;
							portNameAndCaption = s.PortId.ToNameAndCaptionString();
							autoReopenEnabled  = s.AutoReopen.Enabled;
						}

						sb.Append(" - ");
						sb.Append(portNameAndCaption);
						sb.Append(" - ");

						if (isStarted)
						{
							if (isOpen)
							{
								sb.Append("Open");
								sb.Append(" - ");
								sb.Append(isConnected ? "Connected" : "Disconnected"); // Break?
							}
							else if (autoReopenEnabled)
							{
								sb.Append("Closed - Waiting for reconnect");
							}
							else
							{
								sb.Append("Closed");
							}
						}
						else
						{
							sb.Append("Closed");
						}

						break;
					}

					case Domain.IOType.TcpClient:
					{
						var s = settingsRoot.IO.Socket;

						sb.Append(" - ");
						sb.Append(s.RemoteEndPointString);
						sb.Append(" - ");

						if (isConnected)
							sb.Append("Connected");
						else if (isStarted && s.TcpClientAutoReconnect.Enabled)
							sb.Append("Disconnected - Waiting for reconnect");
						else
							sb.Append("Disconnected");

						break;
					}

					case Domain.IOType.TcpServer:
					{
						var s = settingsRoot.IO.Socket;

						sb.Append(" - ");
						sb.Append("Server:");
						sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
						sb.Append(" - ");

						if (isStarted)
							sb.Append(isConnected ? "Connected" : "Listening");
						else
							sb.Append("Closed");

						break;
					}

					case Domain.IOType.TcpAutoSocket:
					{
						var s = settingsRoot.IO.Socket;
						if (isStarted)
						{
							bool isClient = false;
							bool isServer = false;

							var socket = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpAutoSocket);
							if (socket != null)
							{
								isClient = socket.IsClient;
								isServer = socket.IsServer;
							}

							if (isClient)
							{
								sb.Append(" - ");
								sb.Append(s.RemoteEndPointString);
								sb.Append(" - ");
								sb.Append(isConnected ? "Connected" : "Disconnected");
							}
							else if (isServer)
							{
								sb.Append(" - ");
								sb.Append("Server:");
								sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								sb.Append(" - ");
								sb.Append(isConnected ? "Connected" : "Listening");
							}
							else
							{
								sb.Append(" - ");
								sb.Append("Starting on port ");
								sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							}
						}
						else
						{
							sb.Append(" - ");
							sb.Append("AutoSocket:");
							sb.Append(s.RemotePort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							sb.Append(" - ");
							sb.Append("Disconnected");
						}

						break;
					}

					case Domain.IOType.UdpClient:
					{
						var s = settingsRoot.IO.Socket;
						sb.Append(" - ");
						sb.Append(s.RemoteEndPointString);
						sb.Append(" - ");
						sb.Append(isOpen ? "Open" : "Closed");
						break;
					}

					case Domain.IOType.UdpServer:
					{
						var s = settingsRoot.IO.Socket;
						sb.Append(" - ");
						sb.Append("Receive:");
						sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
						sb.Append(" - ");
						sb.Append(isOpen ? "Open" : "Closed");
						break;
					}

					case Domain.IOType.UdpPairSocket:
					{
						var s = settingsRoot.IO.Socket;
						sb.Append(" - ");
						sb.Append(s.RemoteEndPointString);
						sb.Append(" - ");
						sb.Append("Receive:");
						sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
						sb.Append(" - ");
						sb.Append(isOpen ? "Open" : "Closed");
						break;
					}

					case Domain.IOType.UsbSerialHid:
					{
						var s = settingsRoot.IO.UsbSerialHidDevice;
						sb.Append(" - ");
						var device = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice);
						if (device != null)
							sb.Append(device.InfoString);
						else
							s.DeviceInfo.ToString(true, false);

						sb.Append(" - ");

						if (isStarted)
						{
							if (isConnected)
							{
								if (isOpen)
									sb.Append("Connected - Open");
								else if (device.Settings.AutoOpen)
									sb.Append("Connected - Waiting for reopen");
								else
									sb.Append("Connected - Closed");
							}
							else if (device.Settings.AutoOpen)
							{
								sb.Append("Disconnected - Waiting for reconnect");
							}
							else
							{
								sb.Append("Disconnected - Closed");
							}
						}
						else
						{
							sb.Append("Closed");
						}

						break;
					}

					default:
					{
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + settingsRoot.IOType + "' is an I/O type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				} // switch (I/O type)
			} // if (settings available)

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
