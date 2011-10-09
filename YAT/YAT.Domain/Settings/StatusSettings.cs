//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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

		private bool showConnectTime;
		private bool showCountAndRate;

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

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public StatusSettings(StatusSettings rhs)
			: base(rhs)
		{
			ShowConnectTime  = rhs.ShowConnectTime;
			ShowCountAndRate = rhs.ShowCountAndRate;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			ShowConnectTime  = ShowConnectTimeDefault;
			ShowCountAndRate = ShowCountAndRateDefault;
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
				if (value != this.showConnectTime)
				{
					this.showConnectTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowCountAndRate")]
		public virtual bool ShowCountAndRate
		{
			get { return (this.showCountAndRate); }
			set
			{
				if (value != this.showCountAndRate)
				{
					this.showCountAndRate = value;
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			StatusSettings other = (StatusSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.showConnectTime  == other.showConnectTime) &&
				(this.showCountAndRate == other.showCountAndRate)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.showConnectTime .GetHashCode() ^
				this.showCountAndRate.GetHashCode()
			);
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
