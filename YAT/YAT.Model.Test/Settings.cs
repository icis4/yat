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

using System.Diagnostics.CodeAnalysis;
using System.Net;

using MKY.IO.Serial.Socket;

using YAT.Domain;
using YAT.Settings.Model;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Settings
	{
		/// <summary></summary>
		public static TerminalSettingsRoot ToSettings(Domain.Settings.TerminalSettings terminalSettings)
		{
			var settings = new TerminalSettingsRoot();
			settings.Terminal = terminalSettings;
			settings.ClearChanged();
			return (settings);
		}

		#region SerialPort
		//------------------------------------------------------------------------------------------
		// SerialPort
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettingsRoot GetSerialPortSettings(TerminalType terminalType, string portId)
		{
			return (ToSettings(Domain.Test.Settings.GetSerialPortSettings(terminalType, portId)));
		}

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static TerminalSettingsRoot GetMTSicsSerialPortDeviceSettings(string portId)
		{
			return (ToSettings(Domain.Test.Settings.GetSerialPortMTSicsDeviceSettings(portId)));
		}

		#endregion

		#region Socket
		//------------------------------------------------------------------------------------------
		// Socket
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettingsRoot GetIPLoopbackSettings(TerminalType terminalType, SocketType type, IPAddress ipAddress)
		{
			return (ToSettings(Domain.Test.Settings.GetIPLoopbackSettings(terminalType, type, ipAddress)));
		}

		/// <summary></summary>
		public static TerminalSettingsRoot GetIPSpecificInterfaceSettings(TerminalType terminalType, SocketType type, string networkInterface)
		{
			return (ToSettings(Domain.Test.Settings.GetIPSpecificInterfaceSettings(terminalType, type, networkInterface)));
		}

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static TerminalSettingsRoot GetMTSicsIPDeviceSettings(int port)
		{
			return (ToSettings(Domain.Test.Settings.GetMTSicsIPDeviceSettings(port)));
		}

		#endregion

		#region USB Ser/HID
		//------------------------------------------------------------------------------------------
		// USB Ser/HID
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public static TerminalSettingsRoot GetUsbSerialHidSettings(TerminalType terminalType, string deviceInfo)
		{
			return (ToSettings(Domain.Test.Settings.GetUsbSerialHidSettings(terminalType, deviceInfo)));
		}

		/// <remarks>"MTSics" prepended for grouping and easier lookup.</remarks>
		public static TerminalSettingsRoot GetMTSicsUsbSerialHidDeviceSettings(string deviceInfo)
		{
			return (ToSettings(Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceSettings(deviceInfo)));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
