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

using System;
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.IO.Serial.SerialPort;
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
		private static TerminalSettings GetTextSettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Text;
			settings.UpdateTerminalTypeDependentSettings();
			settings.TextTerminal.ShowEol = true; // Required for easier test verification (char/byte count).
			return (settings);                    // Consider moving to each test instead.
		}

		private static TerminalSettings GetBinarySettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Binary;
			settings.UpdateTerminalTypeDependentSettings();
			return (settings);
		}

		private static TerminalSettings GetSettings(TerminalType terminalType)
		{
			switch (terminalType)
			{
				case TerminalType.Text:   return (GetTextSettings());
				case TerminalType.Binary: return (GetBinarySettings());
			}
			throw (new ArgumentOutOfRangeException("terminalType", terminalType, MessageHelper.InvalidExecutionPreamble + "'" + terminalType + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#region SerialPort
		//------------------------------------------------------------------------------------------
		// SerialPort
		//------------------------------------------------------------------------------------------

		internal static void ApplySerialPortSettings(TerminalSettings settings, string portId)
		{
			settings.IO.IOType = IOType.SerialPort;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.SerialPort.PortId = portId;
			settings.UpdateIOSettingsDependentSettings();
		}

		internal static TerminalSettings GetSerialPortSettings(TerminalType terminalType, string portId)
		{
			var settings = GetSettings(terminalType);
			ApplySerialPortSettings(settings, portId);
			return (settings);
		}

		internal static TerminalSettings GetSerialPortMTSicsDeviceASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
			{
				var settings = GetSerialPortSettings(TerminalType.Text, MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ApplyMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetSerialPortMTSicsDeviceBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
			{
				var settings = GetSerialPortSettings(TerminalType.Text, MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ApplyMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket
		//------------------------------------------------------------------------------------------
		// Socket
		//------------------------------------------------------------------------------------------

		internal static void ApplyIPLoopbackSettings(TerminalSettings settings, IOType type, string networkInterface)
		{
			IPNetworkInterfaceEx networkInterfaceCasted = networkInterface;

			settings.IO.IOType = type;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.RemoteHost     = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted);
			settings.IO.Socket.LocalInterface =                                 networkInterfaceCasted;
			settings.IO.Socket.LocalFilter    = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted);
			settings.UpdateIOSettingsDependentSettings();
		}

		internal static TerminalSettings GetIPLoopbackSettings(TerminalType terminalType, IOType type, string networkInterface)
		{
			var settings = GetSettings(terminalType);
			ApplyIPLoopbackSettings(settings, type, networkInterface);
			return (settings);
		}

		internal static void ApplyIPSpecificInterfaceSettings(TerminalSettings settings, IOType type, string networkInterface)
		{
			IPNetworkInterfaceEx networkInterfaceCasted = networkInterface;

			settings.IO.IOType = type;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.RemoteHost     = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted); // \ToDo: Complete specific interface based testing.
			settings.IO.Socket.LocalInterface =                                 networkInterfaceCasted;
			settings.IO.Socket.LocalFilter    = IPAddressEx.GetLoopbackOfFamily(networkInterfaceCasted); // \ToDo: Complete specific interface based testing.
			settings.UpdateIOSettingsDependentSettings();
		}

		internal static TerminalSettings GetIPSpecificInterfaceSettings(TerminalType terminalType, IOType type, string networkInterface)
		{
			var settings = GetSettings(terminalType);
			ApplyIPSpecificInterfaceSettings(settings, type, networkInterface);
			return (settings);
		}

		#region Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------
		// Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetTcpClientOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetTcpClientOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetTcpClientOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetTcpClientOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------
		// Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetTcpServerOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetTcpServerOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetTcpServerOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetTcpServerOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

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
		internal static TerminalSettings GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		/// <remarks>Explicitly using "Loopback", corresponding to  'Configuration.IPv6LoopbackIsAvailable'.</remarks>
		internal static TerminalSettings GetTcpAutoSocketOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetTcpAutoSocketOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetTcpAutoSocketOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		/// <remarks>Interface of <see cref="IPNetworkInterface.IPv4Loopback"/> is implicitly given.</remarks>
		internal static TerminalSettings GetTcpAutoSocketMTSicsDeviceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable)
			{
				var settings = GetIPLoopbackSettings(TerminalType.Text, IOType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback);

				int port = MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceTcpPortAsInt;
				settings.IO.Socket.LocalTcpPort = port;
				settings.IO.Socket.RemoteTcpPort = port;

				ApplyMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDevice' is not available, therefore this test is excluded. Ensure that 'MTSicsDevice' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetUdpClientOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetUdpClientOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetUdpClientOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetUdpClientOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetUdpServerOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetUdpServerOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetUdpServerOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetUdpServerOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------
		// Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetUdpPairSocketOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettings GetUdpPairSocketOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (GetIPLoopbackSettings(terminalType, IOType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettings GetUdpPairSocketOnIPv4SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetUdpPairSocketOnIPv6SpecificInterfaceSettings(TerminalType terminalType)
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetIPSpecificInterfaceSettings(terminalType, IOType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#endregion

		#region USB Ser/HID
		//------------------------------------------------------------------------------------------
		// USB Ser/HID
		//------------------------------------------------------------------------------------------

		internal static void ApplyUsbSerialHidSettings(TerminalSettings settings, string deviceInfo)
		{
			settings.IO.IOType = IOType.UsbSerialHid;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;
			settings.UpdateIOSettingsDependentSettings();
		}

		internal static TerminalSettings GetUsbSerialHidSettings(TerminalType terminalType, string deviceInfo)
		{
			var settings = GetSettings(terminalType);
			ApplyUsbSerialHidSettings(settings, deviceInfo);
			return (settings);
		}

		internal static TerminalSettings GetUsbSerialHidMTSicsDeviceASettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
			{
				var settings = GetUsbSerialHidSettings(TerminalType.Text, MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ApplyMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettings GetUsbSerialHidMTSicsDeviceBSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
			{
				var settings = GetUsbSerialHidSettings(TerminalType.Text, MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ApplyMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		#endregion

		#region MT-SICS
		//------------------------------------------------------------------------------------------
		// MT-SICS
		//------------------------------------------------------------------------------------------

		private static void ApplyMTSicsSettings(TerminalSettings settings)
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
