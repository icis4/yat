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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
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
		public const SerialHidDeviceSettingsPreset PresetDefault = IO.Usb.SerialHidDevice.PresetDefault;

		/// <summary></summary>
		public static readonly SerialHidReportFormat ReportFormatDefault = IO.Usb.SerialHidDevice.ReportFormatDefault;

		/// <summary></summary>
		public static readonly SerialHidRxFilterUsage RxFilterUsageDefault = IO.Usb.SerialHidDevice.RxFilterUsageDefault;

		/// <summary></summary>
		public const SerialHidFlowControl FlowControlDefault = SerialHidFlowControl.None;

		/// <summary></summary>
		public const bool AutoOpenDefault = IO.Usb.SerialHidDevice.AutoOpenDefault;

		/// <summary></summary>
		public const bool IncludeNonPayloadDataDefault = IO.Usb.SerialHidDevice.IncludeNonPayloadDataDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		private DeviceInfo deviceInfo;
		private bool matchSerial;

		private SerialHidDeviceSettingsPreset preset;
		private SerialHidReportFormat reportFormat;
		private SerialHidRxFilterUsage rxFilterUsage;

		private SerialHidFlowControl flowControl;
		private bool autoOpen;

		private bool includeNonPayloadData;

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
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
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

			MatchSerial = rhs.MatchSerial;

			Preset        = rhs.Preset;
			ReportFormat  = rhs.ReportFormat;
			RxFilterUsage = rhs.RxFilterUsage;

			FlowControl = rhs.FlowControl;
			AutoOpen    = rhs.AutoOpen;

			IncludeNonPayloadData = rhs.IncludeNonPayloadData;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			DeviceInfo  = new DeviceInfo(); // Required for XML serialization.
			MatchSerial = MatchSerialDefault;

			Preset        = PresetDefault;
			ReportFormat  = ReportFormatDefault;
			RxFilterUsage = RxFilterUsageDefault;

			FlowControl = FlowControlDefault;
			AutoOpen    = AutoOpenDefault;

			IncludeNonPayloadData = IncludeNonPayloadDataDefault;
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
		[XmlElement("Preset")]
		public virtual SerialHidDeviceSettingsPreset Preset
		{
			get { return (this.preset); }
			set
			{
				if (this.preset != value)
				{
					this.preset = value;
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
		[XmlElement("RxFilterUsage")]
		public virtual SerialHidRxFilterUsage RxFilterUsage
		{
			get { return (this.rxFilterUsage); }
			set
			{
				if (this.rxFilterUsage != value)
				{
					this.rxFilterUsage = value;
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
		/// Returns <c>true</c> if flow control is in use.
		/// </summary>
		public virtual bool FlowControlIsInUse
		{
			get { return (this.flowControl != SerialHidFlowControl.None); }
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

		/// <summary></summary>
		[XmlElement("IncludeNonPayloadData")]
		public virtual bool IncludeNonPayloadData
		{
			get { return (this.includeNonPayloadData); }
			set
			{
				if (this.includeNonPayloadData != value)
				{
					this.includeNonPayloadData = value;
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

				hashCode = (hashCode * 397) ^  Preset                         .GetHashCode();
				hashCode = (hashCode * 397) ^  ReportFormat                   .GetHashCode();
				hashCode = (hashCode * 397) ^  RxFilterUsage                  .GetHashCode();

				hashCode = (hashCode * 397) ^  FlowControl                    .GetHashCode();
				hashCode = (hashCode * 397) ^  AutoOpen                       .GetHashCode();

				hashCode = (hashCode * 397) ^  IncludeNonPayloadData          .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialHidDeviceSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialHidDeviceSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ObjectEx.Equals(DeviceInfo, other.DeviceInfo)  &&
				MatchSerial.Equals(         other.MatchSerial) &&

				Preset       .Equals(other.Preset)        &&
				ReportFormat .Equals(other.ReportFormat)  &&
				RxFilterUsage.Equals(other.RxFilterUsage) &&

				FlowControl.Equals(other.FlowControl) &&
				AutoOpen   .Equals(other.AutoOpen)    &&

				IncludeNonPayloadData.Equals(other.IncludeNonPayloadData)
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

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
