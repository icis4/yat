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
// YAT 2.0 Gamma 2 Version 1.99.50
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

using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class LayoutSettings : MKY.Settings.SettingsItem
	{
		private bool txMonitorPanelIsVisible;
		private bool bidirMonitorPanelIsVisible;
		private bool rxMonitorPanelIsVisible;
		private Orientation monitorOrientation;
		private float txMonitorSplitterRatio;
		private float rxMonitorSplitterRatio;

		private bool predefinedPanelIsVisible;
		private float predefinedSplitterRatio;

		private bool sendTextPanelIsVisible;
		private bool sendFilePanelIsVisible;

		/// <summary></summary>
		public LayoutSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public LayoutSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public LayoutSettings(LayoutSettings rhs)
			: base(rhs)
		{
			TxMonitorPanelIsVisible    = rhs.TxMonitorPanelIsVisible;
			BidirMonitorPanelIsVisible = rhs.BidirMonitorPanelIsVisible;
			RxMonitorPanelIsVisible    = rhs.RxMonitorPanelIsVisible;
			MonitorOrientation         = rhs.MonitorOrientation;
			TxMonitorSplitterRatio     = rhs.TxMonitorSplitterRatio;
			RxMonitorSplitterRatio     = rhs.RxMonitorSplitterRatio;

			PredefinedPanelIsVisible   = rhs.PredefinedPanelIsVisible;
			PredefinedSplitterRatio    = rhs.PredefinedSplitterRatio;

			SendTextPanelIsVisible     = rhs.SendTextPanelIsVisible;
			SendFilePanelIsVisible     = rhs.SendFilePanelIsVisible;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TxMonitorPanelIsVisible    = false;
			BidirMonitorPanelIsVisible = true;
			RxMonitorPanelIsVisible    = false;
			MonitorOrientation         = Orientation.Vertical;
			TxMonitorSplitterRatio     = (float) 1 / 3;
			RxMonitorSplitterRatio     = (float) 1 / 2;

			PredefinedPanelIsVisible   = true;
			PredefinedSplitterRatio    = (float) 3 / 4;

			SendTextPanelIsVisible     = true;
			SendFilePanelIsVisible     = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TxMonitorPanelIsVisible")]
		public virtual bool TxMonitorPanelIsVisible
		{
			get { return (this.txMonitorPanelIsVisible); }
			set
			{
				if (this.txMonitorPanelIsVisible != value)
				{
					this.txMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BidirMonitorPanelIsVisible")]
		public virtual bool BidirMonitorPanelIsVisible
		{
			get { return (this.bidirMonitorPanelIsVisible); }
			set
			{
				if (this.bidirMonitorPanelIsVisible != value)
				{
					this.bidirMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxMonitorPanelIsVisible")]
		public virtual bool RxMonitorPanelIsVisible
		{
			get { return (this.rxMonitorPanelIsVisible); }
			set
			{
				if (this.rxMonitorPanelIsVisible != value)
				{
					this.rxMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MonitorOrientation")]
		public virtual Orientation MonitorOrientation
		{
			get { return (this.monitorOrientation); }
			set
			{
				if (this.monitorOrientation != value)
				{
					this.monitorOrientation = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxMonitorSplitterRatio")]
		public virtual float TxMonitorSplitterRatio
		{
			get { return (this.txMonitorSplitterRatio); }
			set
			{
				if (this.txMonitorSplitterRatio != value)
				{
					this.txMonitorSplitterRatio = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxMonitorSplitterRatio")]
		public virtual float RxMonitorSplitterRatio
		{
			get { return (this.rxMonitorSplitterRatio); }
			set
			{
				if (this.rxMonitorSplitterRatio != value)
				{
					this.rxMonitorSplitterRatio = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("PredefinedPanelIsVisible")]
		public virtual bool PredefinedPanelIsVisible
		{
			get { return (this.predefinedPanelIsVisible); }
			set
			{
				if (this.predefinedPanelIsVisible != value)
				{
					this.predefinedPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("PredefinedSplitterRatio")]
		public virtual float PredefinedSplitterRatio
		{
			get { return (this.predefinedSplitterRatio); }
			set
			{
				if (this.predefinedSplitterRatio != value)
				{
					this.predefinedSplitterRatio = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendTextPanelIsVisible")]
		public virtual bool SendTextPanelIsVisible
		{
			get { return (this.sendTextPanelIsVisible); }
			set
			{
				if (this.sendTextPanelIsVisible != value)
				{
					this.sendTextPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendFilePanelIsVisible")]
		public virtual bool SendFilePanelIsVisible
		{
			get { return (this.sendFilePanelIsVisible); }
			set
			{
				if (this.sendFilePanelIsVisible != value)
				{
					this.sendFilePanelIsVisible = value;
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

			LayoutSettings other = (LayoutSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(TxMonitorPanelIsVisible    == other.TxMonitorPanelIsVisible) &&
				(BidirMonitorPanelIsVisible == other.BidirMonitorPanelIsVisible) &&
				(RxMonitorPanelIsVisible    == other.RxMonitorPanelIsVisible) &&
				(MonitorOrientation         == other.MonitorOrientation) &&
				(TxMonitorSplitterRatio     == other.TxMonitorSplitterRatio) &&
				(RxMonitorSplitterRatio     == other.RxMonitorSplitterRatio) &&

				(PredefinedPanelIsVisible   == other.PredefinedPanelIsVisible) &&
				(PredefinedSplitterRatio    == other.PredefinedSplitterRatio) &&

				(SendTextPanelIsVisible     == other.SendTextPanelIsVisible) &&
				(SendFilePanelIsVisible     == other.SendFilePanelIsVisible)
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

				hashCode =                     TxMonitorPanelIsVisible   .GetHashCode();
				hashCode = (hashCode * 397) ^  BidirMonitorPanelIsVisible.GetHashCode();
				hashCode = (hashCode * 397) ^  RxMonitorPanelIsVisible   .GetHashCode();
				hashCode = (hashCode * 397) ^  MonitorOrientation        .GetHashCode();
				hashCode = (hashCode * 397) ^  TxMonitorSplitterRatio    .GetHashCode();
				hashCode = (hashCode * 397) ^  RxMonitorSplitterRatio    .GetHashCode();

				hashCode = (hashCode * 397) ^  PredefinedPanelIsVisible  .GetHashCode();
				hashCode = (hashCode * 397) ^  PredefinedSplitterRatio   .GetHashCode();

				hashCode = (hashCode * 397) ^  SendTextPanelIsVisible    .GetHashCode();
				hashCode = (hashCode * 397) ^  SendFilePanelIsVisible    .GetHashCode();

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
