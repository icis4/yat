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

using System.Diagnostics.CodeAnalysis;

using YAT.Domain;
using YAT.Settings.Model;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Settings
	{
		/// <remarks>
		/// Required because <see cref="TerminalSettingsRoot"/> cannot be used as a test argument
		/// as creating an object thereof requires the application settings being created/loaded.
		/// </remarks>
		/// <remarks>
		/// Until YAT 2.1.0, a rather complicated delegate/callback mechanism was used to defer
		/// settings creation into the test case itself. With YAT 2.2.0 the mechanism was simplified
		/// to use domain rather than model settings where test generation is needed.
		/// </remarks>
		public static TerminalSettingsRoot Create(Domain.Settings.TerminalSettings terminalSettings)
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

		/// <remarks>Method instead of property same as <see cref="Domain.Test.Settings.GetMTSicsSerialPortDeviceASettings()"/>.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettingsRoot GetMTSicsSerialPortDeviceASettings()
		{
			return (Create(Domain.Test.Settings.GetMTSicsSerialPortDeviceASettings()));
		}

		/// <remarks>Method instead of property same as <see cref="Domain.Test.Settings.GetMTSicsSerialPortDeviceBSettings()"/>.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettingsRoot GetMTSicsSerialPortDeviceBSettings()
		{
			return (Create(Domain.Test.Settings.GetMTSicsSerialPortDeviceBSettings()));
		}

		#endregion

		#region Socket
		//------------------------------------------------------------------------------------------
		// Socket
		//------------------------------------------------------------------------------------------

		/// <remarks>Explicitly using "Loopback", corresponding to 'Configuration.IPv4LoopbackIsAvailable'.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv4...")]
		public static TerminalSettingsRoot GetTcpAutoSocketOnIPv4LoopbackSettings(TerminalType terminalType)
		{
			return (Create(Domain.Test.Settings.GetTcpAutoSocketOnIPv4LoopbackSettings(terminalType)));
		}

		/// <remarks>Explicitly using "Loopback", corresponding to  'Configuration.IPv6LoopbackIsAvailable'.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Well, 'Pv' is just a part of IPv6...")]
		public static TerminalSettingsRoot GetTcpAutoSocketOnIPv6LoopbackSettings(TerminalType terminalType)
		{
			return (Create(Domain.Test.Settings.GetTcpAutoSocketOnIPv6LoopbackSettings(terminalType)));
		}

		/// <remarks>Method instead of property same as <see cref="Domain.Test.Settings.GetMTSicsIPDeviceSettings()"/>.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettingsRoot GetMTSicsIPDeviceSettings()
		{
			return (Create(Domain.Test.Settings.GetMTSicsIPDeviceSettings()));
		}

		#endregion

		#region USB Ser/HID
		//------------------------------------------------------------------------------------------
		// USB Ser/HID
		//------------------------------------------------------------------------------------------

		/// <remarks>Method instead of property same as <see cref="Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceASettings()"/>.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettingsRoot GetMTSicsUsbSerialHidDeviceASettings()
		{
			return (Create(Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceASettings()));
		}

		/// <remarks>Method instead of property same as <see cref="Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceBSettings()"/>.</remarks>
		/// <remarks>"MTSics" prepended (instead of inserted as "*MTSicsDevices") for grouping and easier lookup.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'MT-SICS' is a name.")]
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sics", Justification = "'MT-SICS' is a name.")]
		public static TerminalSettingsRoot GetMTSicsUsbSerialHidDeviceBSettings()
		{
			return (Create(Domain.Test.Settings.GetMTSicsUsbSerialHidDeviceBSettings()));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
