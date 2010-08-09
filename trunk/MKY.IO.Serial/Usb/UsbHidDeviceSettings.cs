//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.IO.Usb;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\USB for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	[Serializable]
	public class UsbHidDeviceSettings : MKY.Utilities.Settings.Settings, IEquatable<UsbHidDeviceSettings>
	{
		/// <summary></summary>
		public static readonly AutoRetry AutoReopenDefault = new AutoRetry(true, 2000);

		private DeviceInfo deviceInfo;
		private AutoRetry autoReopen;

		/// <summary></summary>
		public UsbHidDeviceSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public UsbHidDeviceSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public UsbHidDeviceSettings(UsbHidDeviceSettings rhs)
			: base(rhs)
		{
			// Attention: USB device info can be null (if no USB devices are available on system).
			if (rhs.DeviceInfo != null)
				DeviceInfo = new DeviceInfo(rhs.DeviceInfo);
			else
				DeviceInfo = null;

			this.autoReopen = rhs.autoReopen;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			DeviceInfo = null;
			AutoReopen = AutoReopenDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("DeviceInfo")]
		public DeviceInfo DeviceInfo
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
		[XmlElement("AutoReopen")]
		public AutoRetry AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (value != this.autoReopen)
				{
					this.autoReopen = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			UsbHidDeviceSettings casted = obj as UsbHidDeviceSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(UsbHidDeviceSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.deviceInfo == casted.deviceInfo) &&
				(this.autoReopen == casted.autoReopen)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(UsbHidDeviceSettings lhs, UsbHidDeviceSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(UsbHidDeviceSettings lhs, UsbHidDeviceSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
