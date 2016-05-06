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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
	[Serializable]
	public class StatusSettings : MKY.Settings.SettingsItem
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
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public StatusSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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
		/// Set fields through properties to ensure correct setting of changed flag.
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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

			StatusSettings other = (StatusSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(ShowConnectTime      == other.ShowConnectTime) &&
				(ShowCountAndRate     == other.ShowCountAndRate) &&
				(ShowFlowControlCount == other.ShowFlowControlCount) &&
				(ShowBreakCount       == other.ShowBreakCount)
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

				hashCode = (hashCode * 397) ^ ShowConnectTime     .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowCountAndRate    .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowFlowControlCount.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowBreakCount      .GetHashCode();

				return (hashCode);
			}
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
