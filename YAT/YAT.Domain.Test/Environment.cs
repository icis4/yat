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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;

using MKY.IO.Serial.Socket;
using MKY.Net;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	public static class Environment
	{
		#region SerialPort
		//------------------------------------------------------------------------------------------
		// SerialPort
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns test case descriptors for serial COM port loopback pairs.
		/// </summary>
		public static IEnumerable<SerialPortPairDescriptor> SerialPortLoopbackPairs
		{
			get
			{
				foreach (MKY.IO.Ports.Test.SerialPortPairConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairs)
				{
					string name = "SerialPortLoopbackPair_" + ce.PortA + "_" + ce.PortB;
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
					yield return (new SerialPortPairDescriptor(ce.PortA, ce.PortB, name, cats));
				}
			}
		}

		/// <summary>
		/// Returns test case descriptors for serial COM port loopback selfs.
		/// </summary>
		public static IEnumerable<SerialPortDescriptor> SerialPortLoopbackSelfs
		{
			get
			{
				foreach (MKY.IO.Ports.Test.SerialPortConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfs)
				{
					string name = "SerialPortLoopbackSelf_" + ce.Port;
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
					yield return (new SerialPortDescriptor(ce.Port, name, cats));
				}
			}
		}

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static IEnumerable<SerialPortDescriptor> MTSicsSerialPortDevices
		{
			get
			{
				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ||
				   !MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					string name = "SerialPort_MTSicsDeviceA";
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsAvailable };
					string port = MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA;
					yield return (new SerialPortDescriptor(port, name, cats));
				}

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				{
					string name = "SerialPort_MTSicsDeviceB";
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsAvailable };
					string port = MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB;
					yield return (new SerialPortDescriptor(port, name, cats));
				}
			}
		}

		#endregion

		#region Socket
		//------------------------------------------------------------------------------------------
		// Socket
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns test case descriptors for TCP/IP and UDP/IP Client, Server and AutoSocket.
		/// </summary>
		/// <remarks>
		/// TCP/IP combinations Server/AutoSocket and AutoSocket/Client are skipped as they don't really offer additional test coverage.
		/// UPD/IP PairSocket is also skipped as that would require additional settings with different ports, and they are tested further below anyway.
		/// </remarks>
		public static IEnumerable<IPSocketTypePairDescriptor> IPLoopbackPairs
		{
			get
			{
				string name;
				string[] cats;

				var v4Loopback = (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback; // Convenience shortcut.
				var v6Loopback = (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback; // Convenience shortcut.            // \ToDo: Complete specific interface based testing.
				var v4Specific = v4Loopback; // IPNetworkInterfaceEx.Parse(MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface); // Convenience shortcut.
				var v6Specific = v6Loopback; // IPNetworkInterfaceEx.Parse(MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface); // Convenience shortcut.

				// TCP/IP Client/Server

				name = "TcpClientServer_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpClient, SocketType.TcpServer, v4Loopback, name, cats));

				name = "TcpClientServer_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpClient, SocketType.TcpServer, v6Loopback, name, cats));

				name = "TcpClientServer_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpClient, SocketType.TcpServer, v4Specific, name, cats));

				name = "TcpClientServer_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpClient, SocketType.TcpServer, v6Specific, name, cats));

				// TCP/IP Server/Client

				name = "TcpServerClient_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpServer, SocketType.TcpClient, v4Loopback, name, cats));

				name = "TcpServerClient_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpServer, SocketType.TcpClient, v6Loopback, name, cats));

				name = "TcpServerClient_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpServer, SocketType.TcpClient, v4Specific, name, cats));

				name = "TcpServerClient_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpServer, SocketType.TcpClient, v6Specific, name, cats));

				// TCP/IP AutoSocket

				name = "TcpAutoSocket_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpAutoSocket, SocketType.TcpAutoSocket, v4Loopback, name, cats));

				name = "TcpAutoSocket_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpAutoSocket, SocketType.TcpAutoSocket, v6Loopback, name, cats));

				name = "TcpAutoSocket_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpAutoSocket, SocketType.TcpAutoSocket, v4Specific, name, cats));

				name = "TcpAutoSocket_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.TcpAutoSocket, SocketType.TcpAutoSocket, v6Specific, name, cats));

				// UDP/IP Client/Server

				name = "UdpClientServer_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpClient, SocketType.UdpServer, v4Loopback, name, cats));

				name = "UdpClientServer_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpClient, SocketType.UdpServer, v6Loopback, name, cats));

				name = "UdpClientServer_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpClient, SocketType.UdpServer, v4Specific, name, cats));

				name = "UdpClientServer_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpClient, SocketType.UdpServer, v6Specific, name, cats));

				// UDP/IP Server/Client

				name = "UdpServerClient_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpServer, SocketType.UdpClient, v4Loopback, name, cats));

				name = "UdpServerClient_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpServer, SocketType.UdpClient, v6Loopback, name, cats));

				name = "UdpServerClient_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpServer, SocketType.UdpClient, v4Specific, name, cats));

				name = "UdpServerClient_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypePairDescriptor(SocketType.UdpServer, SocketType.UdpClient, v6Specific, name, cats));
			}
		}

		/// <summary>
		/// Returns test case descriptors for UDP/IP PairSocket.
		/// </summary>
		public static IEnumerable<IPSocketTypeDescriptor> IPLoopbackSelfs
		{
			get
			{
				string name;
				string[] cats;

				var v4Loopback = (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback; // Convenience shortcut.
				var v6Loopback = (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback; // Convenience shortcut.            // \ToDo: Complete specific interface based testing.
				var v4Specific = v4Loopback; // IPNetworkInterfaceEx.Parse(MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface); // Convenience shortcut.
				var v6Specific = v6Loopback; // IPNetworkInterfaceEx.Parse(MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface); // Convenience shortcut.

				// UDP/IP PairSocket

				name = "UdpPairSocket_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new IPSocketTypeDescriptor(SocketType.UdpPairSocket, v4Loopback, name, cats));

				name = "UdpPairSocket_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new IPSocketTypeDescriptor(SocketType.UdpPairSocket, v6Loopback, name, cats));

				name = "UdpPairSocket_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypeDescriptor(SocketType.UdpPairSocket, v4Specific, name, cats));

				name = "UdpPairSocket_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new IPSocketTypeDescriptor(SocketType.UdpPairSocket, v6Specific, name, cats));
			}
		}

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static IEnumerable<IPSocketDescriptor> MTSicsIPDevices
		{
			get
			{
				var v4Loopback = (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback; // Convenience shortcut.

				// Add device in any case => 'Ignore' is issued if device is not available.
				{
					string name = "TcpAutoSocket_MTSicsDevice";
					string[] cats = { MKY.Net.Test.ConfigurationCategoryStrings.MTSicsDeviceIsAvailable };
					int port = MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceTcpPortAsInt;
					yield return (new IPSocketDescriptor(SocketType.TcpAutoSocket, v4Loopback, port, name, cats));
				}
			}
		}

		#endregion

		#region USB Ser/HID
		//------------------------------------------------------------------------------------------
		// USB Ser/HID
		//------------------------------------------------------------------------------------------

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static IEnumerable<UsbSerialHidDescriptor> MTSicsUsbDevices
		{
			get
			{
				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ||
				   !MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					string name = "UsbSerialHid_MTSicsDeviceA";
					string[] cats = { MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsAvailable };
					string deviceInfo = MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA;
					yield return (new UsbSerialHidDescriptor(deviceInfo, name, cats));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				{
					string name = "UsbSerialHid_MTSicsDeviceB";
					string[] cats = { MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsAvailable };
					string deviceInfo = MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB;
					yield return (new UsbSerialHidDescriptor(deviceInfo, name, cats));
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
