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
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY.IO.Usb;

namespace MKY.IO.Serial.Usb
{
	/// <summary></summary>
	[Serializable]
	public class SerialHidDeviceSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool AutoOpenDefault = true;

		private const string Undefined = "<Undefined>";

		private DeviceInfo deviceInfo;
		private bool autoOpen;

		/// <summary></summary>
		public SerialHidDeviceSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialHidDeviceSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SerialHidDeviceSettings(SerialHidDeviceSettings rhs)
			: base(rhs)
		{
			// Attention: USB device info can be null (if no USB devices are available on system).
			if (rhs.DeviceInfo != null)
				DeviceInfo = new DeviceInfo(rhs.DeviceInfo);
			else
				DeviceInfo = null;

			AutoOpen = rhs.autoOpen;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			DeviceInfo = new DeviceInfo(); // Required for XML serialization.

			AutoOpen = AutoOpenDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("DeviceInfo")]
		public virtual DeviceInfo DeviceInfo
		{
			get { return (this.deviceInfo); }
			set
			{
				if (value != this.deviceInfo)
				{
					this.deviceInfo = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoOpen")]
		public virtual bool AutoOpen
		{
			get { return (this.autoOpen); }
			set
			{
				if (value != this.autoOpen)
				{
					this.autoOpen = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SerialHidDeviceSettings other = (SerialHidDeviceSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(DeviceInfo == other.DeviceInfo) &&
				(AutoOpen   == other.AutoOpen)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			int deviceInfoHashCode = 0;
			if (DeviceInfo != null)
				deviceInfoHashCode = DeviceInfo.GetHashCode();

			return
			(
				base.GetHashCode() ^

				deviceInfoHashCode ^
				AutoOpen.GetHashCode()
			);
		}

		/// <summary></summary>
		public virtual string ToShortDeviceInfoString()
		{
			if (DeviceInfo != null)
				return (DeviceInfo.ToShortString());
			else
				return (Undefined);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
