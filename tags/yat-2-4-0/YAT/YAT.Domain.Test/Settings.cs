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
// Copyright © 2007-2021 Matthias Kläy.
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
using System.Net;

using MKY;
using MKY.IO.Serial.SerialPort;
using MKY.IO.Serial.Socket;
using MKY.IO.Serial.Usb;
using MKY.Net;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Settings
	{
		/// <remarks>Method instead of property for orthogonality with <see cref="GetSettings(TerminalType)"/> below.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public static TerminalSettings GetTextSettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Text;
			settings.UpdateTerminalTypeDependentSettings();
			settings.TextTerminal.ShowEol = true; // Required for easier test verification (char/byte count).
			return (settings);                    // Consider moving to each test instead.
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetSettings(TerminalType)"/> below.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public static TerminalSettings GetBinarySettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Binary;
			settings.UpdateTerminalTypeDependentSettings();
			return (settings);
		}

		/// <summary></summary>
		public static TerminalSettings GetSettings(TerminalType terminalType)
		{
			switch (terminalType)
			{
				case TerminalType.Text:   return (GetTextSettings());
				case TerminalType.Binary: return (GetBinarySettings());
			}
			throw (new ArgumentOutOfRangeException("terminalType", terminalType, MessageHelper.InvalidExecutionPreamble + "'" + terminalType + "' is a terminal type that is not (yet) supported here!" + System.Environment.NewLine + System.Environment.NewLine + MessageHelper.SubmitBug));
		}

		#region SerialPort
		//==========================================================================================
		// SerialPort
		//==========================================================================================

		/// <summary></summary>
		public static void ApplySerialPortSettings(TerminalSettings settings, string portId)
		{
			settings.IO.IOType = IOType.SerialPort;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.SerialPort.PortId = portId;
			settings.UpdateIOSettingsDependentSettings();
		}

		/// <summary></summary>
		public static TerminalSettings GetSerialPortSettings(TerminalType terminalType, string portId)
		{
			var settings = GetSettings(terminalType);
			ApplySerialPortSettings(settings, portId);
			return (settings);
		}

		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsSerialPortDeviceSettings(string portId)
		{
			var settings = GetSerialPortSettings(TerminalType.Text, portId);
			ApplyMTSicsSettings(settings);
			return (settings);
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetMTSicsSerialPortDeviceSettings(string)"/> above.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsSerialPortDeviceASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
				return (GetMTSicsSerialPortDeviceSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA));

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetMTSicsSerialPortDeviceSettings(string)"/> above.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsSerialPortDeviceBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				return (GetMTSicsSerialPortDeviceSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB));

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket
		//==========================================================================================
		// Socket
		//==========================================================================================

		/// <summary></summary>
		public static void ApplyIPLoopbackSettings(TerminalSettings settings, SocketType socketType, IPAddress ipAddress)
		{
			settings.IO.IOType = (IOTypeEx)socketType;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.RemoteHost     = IPAddressEx.GetLoopbackOfFamily(ipAddress);
			settings.IO.Socket.LocalInterface =           (IPNetworkInterfaceEx)ipAddress;
			settings.IO.Socket.LocalFilter    = IPAddressEx.GetLoopbackOfFamily(ipAddress);
			settings.UpdateIOSettingsDependentSettings();
		}

		/// <summary></summary>
		public static TerminalSettings GetIPSocketSettings(TerminalType terminalType, SocketType socketType, IPAddress ipAddress)
		{
			var settings = GetSettings(terminalType);
			ApplyIPLoopbackSettings(settings, socketType, ipAddress);
			return (settings);
		}

		/// <summary></summary>
		public static void ApplyIPSpecificInterfaceSettings(TerminalSettings settings, SocketType socketType, string networkInterface)
		{
			IPNetworkInterfaceEx networkInterfaceCasted = networkInterface;

			settings.IO.IOType = (IOTypeEx)socketType;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.RemoteHost     = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted); // \ToDo: Complete specific interface based testing.
			settings.IO.Socket.LocalInterface =                                 networkInterfaceCasted;
			settings.IO.Socket.LocalFilter    = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted); // \ToDo: Complete specific interface based testing.
			settings.UpdateIOSettingsDependentSettings();
		}

		/// <summary></summary>
		public static TerminalSettings GetIPSpecificInterfaceSettings(TerminalType terminalType, SocketType socketType, string networkInterface)
		{
			var settings = GetSettings(terminalType);
			ApplyIPSpecificInterfaceSettings(settings, socketType, networkInterface);
			return (settings);
		}

		#region Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------
		// Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettings GetTcpClientOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpClientOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpClientOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpClientOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------
		// Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettings GetTcpServerOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpServerOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpServerOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpServerOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------
		// Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------

		/// <remarks>Explicitly using "Loopback", corresponding to 'Configuration.IPv4LoopbackIsAvailable'.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv4...")]
		public static TerminalSettings GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <remarks>Explicitly using "Loopback", corresponding to  'Configuration.IPv6LoopbackIsAvailable'.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv6...")]
		public static TerminalSettings GetTcpAutoSocketOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpAutoSocketOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetTcpAutoSocketOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsIPDeviceSettings(int port)
		{
			var settings = GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType.Text);
			settings.IO.Socket.LocalTcpPort = port;
			settings.IO.Socket.RemoteTcpPort = port;
			ApplyMTSicsSettings(settings);
			return (settings);
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetMTSicsIPDeviceSettings(int)"/> above.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsIPDeviceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable)
				return (GetMTSicsIPDeviceSettings(MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceTcpPortAsInt));

			Assert.Ignore("'MTSicsDevice' is not available, therefore this test is excluded. Ensure that 'MTSicsDevice' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettings GetUdpClientOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpClientOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpClientOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpClientOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettings GetUdpServerOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpServerOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpServerOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpServerOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettings GetUdpPairSocketOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpPairSocketOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPSocketSettings(terminalType, SocketType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpPairSocketOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <summary></summary>
		public static TerminalSettings GetUdpPairSocketOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, SocketType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP General
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP General
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static void RevertSettingsIfUdpSocket(TerminalSettings settings)
		{
			if (settings != null)
			{
				if (settings.IO.IOTypeIsUdpSocket)
				{
					settings.TextTerminal.TxEol = TextTerminalSettings.EolDefault;
					settings.TextTerminal.RxEol = TextTerminalSettings.EolDefault;

					settings.TextTerminal.TxDisplay.ChunkLineBreakEnabled = false;
					settings.TextTerminal.RxDisplay.ChunkLineBreakEnabled = false;

					settings.BinaryTerminal.TxDisplay.ChunkLineBreakEnabled = false;
					settings.BinaryTerminal.RxDisplay.ChunkLineBreakEnabled = false;
				}
			}
		}

		#endregion

		#endregion

		#region USB Ser/HID
		//==========================================================================================
		// USB Ser/HID
		//==========================================================================================

		/// <summary></summary>
		public static void ApplyUsbSerialHidSettings(TerminalSettings settings, string deviceInfo)
		{
			settings.IO.IOType = IOType.UsbSerialHid;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;
			settings.UpdateIOSettingsDependentSettings();
		}

		/// <summary></summary>
		public static TerminalSettings GetUsbSerialHidSettings(TerminalType terminalType, string deviceInfo)
		{
			var settings = GetSettings(terminalType);
			ApplyUsbSerialHidSettings(settings, deviceInfo);
			return (settings);
		}

		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsUsbSerialHidDeviceSettings(string deviceInfo)
		{
			var settings = GetUsbSerialHidSettings(TerminalType.Text, deviceInfo);
			ApplyMTSicsSettings(settings);
			return (settings);
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetMTSicsUsbSerialHidDeviceSettings(string)"/> above.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsUsbSerialHidDeviceASettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
				GetMTSicsUsbSerialHidDeviceSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <remarks>Method instead of property for orthogonality with <see cref="GetMTSicsUsbSerialHidDeviceSettings(string)"/> above.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettings GetMTSicsUsbSerialHidDeviceBSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				GetMTSicsUsbSerialHidDeviceSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region MT-SICS
		//==========================================================================================
		// MT-SICS
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static void ApplyMTSicsSettings(TerminalSettings settings)
		{
			// MT-SICS devices use XOn/XOff by default:
			settings.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software;
			settings.IO.UsbSerialHidDevice      .FlowControl = SerialHidFlowControl.Software;
			settings.UpdateIOSettingsDependentSettings();

			// Set required USB Ser/HID format incl. dependent settings:
			var presetEx = new MKY.IO.Usb.SerialHidDeviceSettingsPresetEx(MKY.IO.Usb.SerialHidDeviceSettingsPreset.MT_SerHid);
			settings.IO.UsbSerialHidDevice.Preset        = presetEx;
			settings.IO.UsbSerialHidDevice.ReportFormat  = presetEx.ToReportFormat();
			settings.IO.UsbSerialHidDevice.RxFilterUsage = presetEx.ToRxFilterUsage();
		}

		#endregion

		#region Binary
		//==========================================================================================
		// Binary
		//==========================================================================================

		/// <summary></summary>
		public static void ConfigureSettingsIfBinary(TerminalSettings settings, int lengthLineBreakLength)
		{
			if (settings != null)
			{
				if (settings.TerminalType == TerminalType.Binary)
				{
					ConfigureSettingsIfBinary(settings.BinaryTerminal.TxDisplay, lengthLineBreakLength);
					ConfigureSettingsIfBinary(settings.BinaryTerminal.RxDisplay, lengthLineBreakLength);
				}
			}
		}

		private static void ConfigureSettingsIfBinary(BinaryDisplaySettings settings, int lengthLineBreakLength)
		{
			if (settings != null)
			{
				settings.ChunkLineBreakEnabled = false;

				var llb = settings.LengthLineBreak;
				llb.Enabled = true;
				llb.Length = lengthLineBreakLength;
				settings.LengthLineBreak = llb;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
