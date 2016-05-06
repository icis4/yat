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
// MKY Development Version 1.0.14
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
	public class SerialHidDeviceSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

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
		{
			SetMyDefaults();
			ClearChanged();
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

			DeviceInfo = new DeviceInfo(); // Required for XML serialization.

			ReportFormat = new SerialHidReportFormat();
			RxIdUsage    = new SerialHidRxIdUsage();

			FlowControl = FlowControlDefault;
			AutoOpen    = AutoOpenDefault;
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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

				(DeviceInfo   == other.DeviceInfo) &&

				(ReportFormat == other.ReportFormat) &&
				(RxIdUsage    == other.RxIdUsage) &&
				(FlowControl  == other.FlowControl) &&
				(AutoOpen     == other.AutoOpen)
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (DeviceInfo != null ? DeviceInfo.GetHashCode() : 0); // May be 'null' if no devices are available!

				hashCode = (hashCode * 397) ^  ReportFormat                   .GetHashCode();
				hashCode = (hashCode * 397) ^  RxIdUsage                      .GetHashCode();
				hashCode = (hashCode * 397) ^  FlowControl                    .GetHashCode();
				hashCode = (hashCode * 397) ^  AutoOpen                       .GetHashCode();

				return (hashCode);
			}
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToShortDeviceInfoString()
		{
			if (DeviceInfo != null)
				return (DeviceInfo.ToShortString());
			else
				return (Undefined);
		}

		#endregion

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
