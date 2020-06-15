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
// YAT Version 2.2.0 Development
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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using YAT.Settings.Model;

namespace YAT.Model.Utilities
{
	/// <summary></summary>
	public static class IOStatusHelper
	{
		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		public static string Compose(TerminalSettingsRoot settingsRoot, Domain.Terminal terminal,
		                             bool isStarted, bool isOpen, bool isConnected)
		{
			var sb = new StringBuilder();

			if (settingsRoot != null)
			{
				switch (settingsRoot.IOType)
				{
					case Domain.IOType.SerialPort:
					{
						string portNameAndCaption;
						string communication;
						bool autoReopenEnabled;

						var port = (terminal.UnderlyingIOProvider as MKY.IO.Serial.SerialPort.SerialPort);
						if (port != null) // Effective settings from port object:
						{
							var s = port.Settings;
							portNameAndCaption = s.PortId.ToNameAndCaptionString();
							communication      = s.Communication.ToString();
							autoReopenEnabled  = s.AutoReopen.Enabled;
						}
						else // Fallback to settings object tree;
						{
							var s = settingsRoot.IO.SerialPort;
							portNameAndCaption = s.PortId.ToNameAndCaptionString();
							communication      = s.Communication.ToString();
							autoReopenEnabled  = s.AutoReopen.Enabled;
						}

						sb.Append("Serial port "); // Not adding "COM" as the port name will already state that.
						sb.Append(portNameAndCaption);
						sb.Append(" (" + communication + ")");

						if (isStarted)
						{
							if (isOpen)
							{
								sb.Append(" is open and ");
								sb.Append(isConnected ? "connected" : "disconnected");
							}
							else if (autoReopenEnabled)
							{
								sb.Append(" is closed and waiting for reconnect");
							}
							else
							{
								sb.Append(" is closed");
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						break;
					}

					case Domain.IOType.TcpClient:
					{
						MKY.IO.Serial.Socket.SocketSettings s = settingsRoot.IO.Socket;
						sb.Append("TCP/IP client");

						if (isConnected)
							sb.Append(" is connected to ");
						else if (isStarted && s.TcpClientAutoReconnect.Enabled)
							sb.Append(" is disconnected and waiting for reconnect to ");
						else
							sb.Append(" is disconnected from ");

						sb.Append(s.RemoteEndPointString);
						break;
					}

					case Domain.IOType.TcpServer:
					{
						var s = settingsRoot.IO.Socket;
						sb.Append("TCP/IP server");
						if (isStarted)
						{
							if (isConnected)
							{
								int count = 0;

								var server = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.TcpServer);
								if (server != null)
									count = server.ConnectedClientCount;

								sb.Append(" is connected");
								if (count == 1)
								{
									sb.Append(" to a client");
								}
								else
								{
									sb.Append(" to ");
									sb.Append(count.ToString(CultureInfo.CurrentCulture));
									sb.Append(" clients");
								}
							}
							else
							{
								sb.Append(" is listening");
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						sb.Append(" on local port ");
						sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
						break;
					}

					case Domain.IOType.TcpAutoSocket:
					{
						var s = settingsRoot.IO.Socket;
						sb.Append("TCP/IP AutoSocket");
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
								sb.Append(" is connected to ");
								sb.Append(s.RemoteEndPointString);
							}
							else if (isServer)
							{
								sb.Append(isConnected ? " is connected" : " is listening");
								sb.Append(" on local port ");
								sb.Append(s.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							}
							else
							{
								sb.Append(" is starting to connect to ");
								sb.Append(s.RemoteEndPointString);
							}
						}
						else
						{
							sb.Append(" is disconnected from ");
							sb.Append(s.RemoteEndPointString);
						}

						break;
					}

					case Domain.IOType.UdpClient:
					{
						sb.Append("UDP/IP client");
						if (isOpen)
						{
							sb.Append(" is open");
							var socket = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
							if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.Client))
							{
								sb.Append(" for sending to ");
								sb.Append(socket.RemoteEndPoint.ToString());

								int localPort = socket.LocalPort;
								if (localPort != 0)
								{
									sb.Append(" and receiving on local port ");
									sb.Append(localPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
								}
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						break;
					}

					case Domain.IOType.UdpServer:
					{
						sb.Append("UDP/IP server is ");
						if (isOpen)
						{
							sb.Append(" is open");
							var socket = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
							if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.Server))
							{
								sb.Append(" for receiving on local port ");
								sb.Append(socket.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!

								System.Net.IPEndPoint remoteEndPoint = socket.RemoteEndPoint;
								if ((remoteEndPoint != null) && (MKY.Net.IPAddressEx.NotEqualsNone(remoteEndPoint.Address)))
								{
									sb.Append(" and sending to ");
									sb.Append(socket.RemoteEndPoint.ToString());
								}
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						break;
					}

					case Domain.IOType.UdpPairSocket:
					{
						sb.Append("UDP/IP PairSocket");
						if (isOpen)
						{
							sb.Append(" is open");

							var socket = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Socket.UdpSocket);
							if ((socket != null) && (socket.SocketType == MKY.IO.Serial.Socket.UdpSocketType.PairSocket))
							{
								sb.Append(" for sending to ");
								sb.Append(socket.RemoteEndPoint.ToString());
								sb.Append(" and receiving on local port ");
								sb.Append(socket.LocalPort.ToString(CultureInfo.InvariantCulture)); // 'InvariantCulture' for TCP and UDP ports!
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						break;
					}

					case Domain.IOType.UsbSerialHid:
					{
						var s = settingsRoot.IO.UsbSerialHidDevice;
						sb.Append("USB HID device '");

						var device = (terminal.UnderlyingIOProvider as MKY.IO.Serial.Usb.SerialHidDevice);
						if (device != null)
							sb.Append(device.InfoString);
						else
							s.DeviceInfo.ToString(true, false);

						sb.Append("'");

						if (isStarted)
						{
							if (isConnected)
							{
								if (isOpen)
									sb.Append(" is connected and open");
								else if (device.Settings.AutoOpen)
									sb.Append(" is connected but waiting for reopen");
								else
									sb.Append(" is connected but closed");
							}
							else if (device.Settings.AutoOpen)
							{
								sb.Append(" is disconnected and waiting for reconnect");
							}
							else
							{
								sb.Append(" is disconnected and closed");
							}
						}
						else
						{
							sb.Append(" is closed");
						}

						break;
					}

					default:
					{
						// Do nothing.
						break;
					}
				}
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
