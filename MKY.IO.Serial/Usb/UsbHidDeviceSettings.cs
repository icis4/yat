//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Serial/USB/UsbHidPortSettings.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
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
		public static readonly AutoRetry AutoReconnectDefault = new AutoRetry(true, 2000);

        private DeviceId _deviceId;
		private AutoRetry _autoReconnect;

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
            DeviceId       = new DeviceId(rhs.DeviceId);
			_autoReconnect = rhs._autoReconnect;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			AutoReconnect = AutoReconnectDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
        [XmlElement("DeviceId")]
        public DeviceId DeviceId
		{
			get { return (_deviceId); }
			set
			{
                if (_deviceId != value)
				{
                    _deviceId = value;
                    SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoReopen")]
		public AutoRetry AutoReconnect
		{
			get { return (_autoReconnect); }
			set
			{
				if (_autoReconnect != value)
				{
					_autoReconnect = value;
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
			if (obj is UsbHidDeviceSettings)
				return (Equals((UsbHidDeviceSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(UsbHidDeviceSettings value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return 
					(
                    _deviceId.Equals  (value._deviceId)   &&
					_autoReconnect.Equals(value._autoReconnect)
					);
			}
			return (false);
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
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Serial/USB/UsbHidPortSettings.cs $
//==================================================================================================
