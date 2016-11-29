//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Xml.Serialization;

using MKY.IO.Usb;

#endregion

namespace MKY.IO.Serial.Usb
{
	/// <summary></summary>
	[Serializable]
	public class SerialHidDeviceSettings : Settings.SettingsItem, IEquatable<SerialHidDeviceSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool MatchSerialDefault = IO.Usb.SerialHidDevice.MatchSerialDefault;

		/// <summary></summary>
		public const SerialHidFlowControl FlowControlDefault = SerialHidFlowControl.None;

		/// <summary></summary>
		public const bool AutoOpenDefault = IO.Usb.SerialHidDevice.AutoOpenDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		private DeviceInfo deviceInfo;
		private bool matchSerial;

		private SerialHidReportFormat reportFormat;
		private SerialHidRxIdUsage rxIdUsage;

		private SerialHidFlowControl flowControl;
		private bool autoOpen;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialHidDeviceSettings()
			: this(Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SerialHidDeviceSettings(Settings.SettingsType settingsType)
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

			MatchSerial  = rhs.MatchSerial;

			ReportFormat = new SerialHidReportFormat(rhs.ReportFormat);
			RxIdUsage    = new SerialHidRxIdUsage   (rhs.RxIdUsage);

			FlowControl = rhs.FlowControl;
			AutoOpen    = rhs.AutoOpen;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			DeviceInfo   = new DeviceInfo(); // Required for XML serialization.
			MatchSerial  = MatchSerialDefault;

			ReportFormat = new SerialHidReportFormat();
			RxIdUsage    = new SerialHidRxIdUsage();

			FlowControl  = FlowControlDefault;
			AutoOpen     = AutoOpenDefault;
		}

		#endregion

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
				if (this.deviceInfo != value)
				{
					this.deviceInfo = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MatchSerial")]
		public virtual bool MatchSerial
		{
			get { return (this.matchSerial); }
			set
			{
				if (this.matchSerial != value)
				{
					this.matchSerial = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReportFormat")]
		public virtual SerialHidReportFormat ReportFormat
		{
			get { return (this.reportFormat); }
			set
			{
				if (this.reportFormat != value)
				{
					this.reportFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		[XmlElement("RxIdUsage")]
		public virtual SerialHidRxIdUsage RxIdUsage
		{
			get { return (this.rxIdUsage); }
			set
			{
				if (this.rxIdUsage != value)
				{
					this.rxIdUsage = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FlowControl")]
		public virtual SerialHidFlowControl FlowControl
		{
			get { return (this.flowControl); }
			set
			{
				if (this.flowControl != value)
				{
					this.flowControl = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c> if flow control is active, i.e. the receiver can pause the sender.
		/// </summary>
		public virtual bool FlowControlIsActive
		{
			get { return (!FlowControlIsInactive); }
		}

		/// <summary>
		/// Returns <c>true</c> if flow control is inactive, i.e. the receiver cannot pause the sender.
		/// </summary>
		public virtual bool FlowControlIsInactive
		{
			get { return (this.flowControl == SerialHidFlowControl.None); }
		}

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
		/// flow control is active.
		/// </summary>
		public virtual bool FlowControlUsesXOnXOff
		{
			get
			{
				return ((this.flowControl == SerialHidFlowControl.Software) ||
						(this.flowControl == SerialHidFlowControl.ManualSoftware));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if XOn/XOff is managed manually.
		/// </summary>
		public virtual bool FlowControlManagesXOnXOffManually
		{
			get
			{
				return (this.flowControl == SerialHidFlowControl.ManualSoftware);
			}
		}

		/// <summary></summary>
		[XmlElement("AutoOpen")]
		public virtual bool AutoOpen
		{
			get { return (this.autoOpen); }
			set
			{
				if (this.autoOpen != value)
				{
					this.autoOpen = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToShortDeviceInfoString()
		{
			if (DeviceInfo != null)
				return (DeviceInfo.ToShortString());
			else
				return (Undefined);
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (DeviceInfo != null ? DeviceInfo.GetHashCode() : 0); // May be 'null' if no devices are available!
				hashCode = (hashCode * 397) ^  MatchSerial                    .GetHashCode();

				hashCode = (hashCode * 397) ^  ReportFormat                   .GetHashCode();
				hashCode = (hashCode * 397) ^  RxIdUsage                      .GetHashCode();

				hashCode = (hashCode * 397) ^  FlowControl                    .GetHashCode();
				hashCode = (hashCode * 397) ^  AutoOpen                       .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialHidDeviceSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialHidDeviceSettings other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (ReferenceEquals(this, other))
				return (true);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ObjectEx.Equals(DeviceInfo, other.DeviceInfo) &&
				MatchSerial.Equals(         other.MatchSerial) &&

				ObjectEx.Equals(ReportFormat, other.ReportFormat) &&
				ObjectEx.Equals(RxIdUsage,    other.RxIdUsage) &&

				FlowControl.Equals(other.FlowControl) &&
				AutoOpen.Equals(   other.AutoOpen)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialHidDeviceSettings lhs, SerialHidDeviceSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(SerialHidDeviceSettings lhs, SerialHidDeviceSettings rhs)
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
