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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <remarks>
	/// The settings 'ShowConnectTime' and 'ShowCounters' were initially located in
	/// <see cref="DisplaySettings"/>. They were relocated here to better differentiate between
	/// settings that need a reload of the monitors and those which don't, i.e. those below.
	/// Now it is possible to show/hide the status labels without reloading the monitors, thus
	/// saving time, especially in case of large data.
	/// </remarks>
	public class StatusSettings : MKY.Settings.SettingsItem, IEquatable<StatusSettings>
	{
		/// <summary></summary>
		public const bool ShowConnectTimeDefault = false;

		/// <summary></summary>
		public const bool ShowCountAndRateDefault = false;

		/// <summary></summary>
		public const bool ShowFlowControlCountDefault = false;

		/// <summary></summary>
		public const bool ShowBreakCountDefault = false;

		private bool showConnectTime;
		private bool showCountAndRate;
		private bool showFlowControlCount;
		private bool showBreakCount;

		/// <summary></summary>
		public StatusSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public StatusSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public StatusSettings(StatusSettings rhs)
			: base(rhs)
		{
			ShowConnectTime      = rhs.ShowConnectTime;
			ShowCountAndRate     = rhs.ShowCountAndRate;
			ShowFlowControlCount = rhs.ShowFlowControlCount;
			ShowBreakCount       = rhs.ShowBreakCount;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ShowConnectTime      = ShowConnectTimeDefault;
			ShowCountAndRate     = ShowCountAndRateDefault;
			ShowFlowControlCount = ShowFlowControlCountDefault;
			ShowBreakCount       = ShowBreakCountDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("ShowConnectTime")]
		public virtual bool ShowConnectTime
		{
			get { return (this.showConnectTime); }
			set
			{
				if (this.showConnectTime != value)
				{
					this.showConnectTime = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>Data count is singular even that there is a count per direction.</remarks>
		[XmlElement("ShowCountAndRate")]
		public virtual bool ShowCountAndRate
		{
			get { return (this.showCountAndRate); }
			set
			{
				if (this.showCountAndRate != value)
				{
					this.showCountAndRate = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>Signal count is singular even that there are multiple counts per port.</remarks>
		[XmlElement("ShowFlowControlCount")]
		public virtual bool ShowFlowControlCount
		{
			get { return (this.showFlowControlCount); }
			set
			{
				if (this.showFlowControlCount != value)
				{
					this.showFlowControlCount = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>Break count is singular even that there are multiple counts per port.</remarks>
		[XmlElement("ShowBreakCount")]
		public virtual bool ShowBreakCount
		{
			get { return (this.showBreakCount); }
			set
			{
				if (this.showBreakCount != value)
				{
					this.showBreakCount = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode = (hashCode * 397) ^ ShowConnectTime     .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowCountAndRate    .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowFlowControlCount.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowBreakCount      .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as StatusSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(StatusSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ShowConnectTime     .Equals(other.ShowConnectTime) &&
				ShowCountAndRate    .Equals(other.ShowCountAndRate) &&
				ShowFlowControlCount.Equals(other.ShowFlowControlCount) &&
				ShowBreakCount      .Equals(other.ShowBreakCount)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(StatusSettings lhs, StatusSettings rhs)
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
		public static bool operator !=(StatusSettings lhs, StatusSettings rhs)
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
