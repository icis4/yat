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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
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

		private bool sendCommandPanelIsVisible;
		private bool sendFilePanelIsVisible;

		/// <summary></summary>
		public LayoutSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public LayoutSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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

			SendCommandPanelIsVisible  = rhs.SendCommandPanelIsVisible;
			SendFilePanelIsVisible     = rhs.SendFilePanelIsVisible;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TxMonitorPanelIsVisible    = false;
			BidirMonitorPanelIsVisible = true;
			RxMonitorPanelIsVisible    = false;
			MonitorOrientation         = Orientation.Vertical;
			TxMonitorSplitterRatio     = (float)1 / 3;
			RxMonitorSplitterRatio     = (float)1 / 2;

			PredefinedPanelIsVisible   = true;
			PredefinedSplitterRatio    = (float)3 / 4;

			SendCommandPanelIsVisible  = true;
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
				if (value != this.txMonitorPanelIsVisible)
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
				if (value != this.bidirMonitorPanelIsVisible)
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
				if (value != this.rxMonitorPanelIsVisible)
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
				if (value != this.monitorOrientation)
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
				if (value != this.txMonitorSplitterRatio)
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
				if (value != this.rxMonitorSplitterRatio)
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
				if (value != this.predefinedPanelIsVisible)
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
				if (value != this.predefinedSplitterRatio)
				{
					this.predefinedSplitterRatio = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendCommandPanelIsVisible")]
		public virtual bool SendCommandPanelIsVisible
		{
			get { return (this.sendCommandPanelIsVisible); }
			set
			{
				if (value != this.sendCommandPanelIsVisible)
				{
					this.sendCommandPanelIsVisible = value;
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
				if (value != this.sendFilePanelIsVisible)
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

				(this.txMonitorPanelIsVisible    == other.txMonitorPanelIsVisible) &&
				(this.bidirMonitorPanelIsVisible == other.bidirMonitorPanelIsVisible) &&
				(this.rxMonitorPanelIsVisible    == other.rxMonitorPanelIsVisible) &&
				(this.monitorOrientation         == other.monitorOrientation) &&
				(this.txMonitorSplitterRatio     == other.txMonitorSplitterRatio) &&
				(this.rxMonitorSplitterRatio     == other.rxMonitorSplitterRatio) &&

				(this.predefinedPanelIsVisible   == other.predefinedPanelIsVisible) &&
				(this.predefinedSplitterRatio    == other.predefinedSplitterRatio) &&

				(this.sendCommandPanelIsVisible  == other.sendCommandPanelIsVisible) &&
				(this.sendFilePanelIsVisible     == other.sendFilePanelIsVisible)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.txMonitorPanelIsVisible   .GetHashCode() ^
				this.bidirMonitorPanelIsVisible.GetHashCode() ^
				this.rxMonitorPanelIsVisible   .GetHashCode() ^
				this.monitorOrientation        .GetHashCode() ^
				this.txMonitorSplitterRatio    .GetHashCode() ^
				this.rxMonitorSplitterRatio    .GetHashCode() ^

				this.predefinedPanelIsVisible  .GetHashCode() ^
				this.predefinedSplitterRatio   .GetHashCode() ^

				this.sendCommandPanelIsVisible .GetHashCode() ^
				this.sendFilePanelIsVisible    .GetHashCode()
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
