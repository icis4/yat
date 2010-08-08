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
	public class LayoutSettings : MKY.Utilities.Settings.Settings, IEquatable<LayoutSettings>
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
		public LayoutSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.txMonitorPanelIsVisible    = rhs.TxMonitorPanelIsVisible;
			this.bidirMonitorPanelIsVisible = rhs.BidirMonitorPanelIsVisible;
			this.rxMonitorPanelIsVisible    = rhs.RxMonitorPanelIsVisible;
			this.monitorOrientation         = rhs.MonitorOrientation;
			this.txMonitorSplitterRatio     = rhs.TxMonitorSplitterRatio;
			this.rxMonitorSplitterRatio     = rhs.RxMonitorSplitterRatio;

			this.predefinedPanelIsVisible = rhs.PredefinedPanelIsVisible;
			this.predefinedSplitterRatio  = rhs.PredefinedSplitterRatio;

			this.sendCommandPanelIsVisible = rhs.SendCommandPanelIsVisible;
			this.sendFilePanelIsVisible = rhs.SendFilePanelIsVisible;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TxMonitorPanelIsVisible = false;
			BidirMonitorPanelIsVisible = true;
			RxMonitorPanelIsVisible = false;
			MonitorOrientation = Orientation.Vertical;
			TxMonitorSplitterRatio = (float)1 / 3;
			RxMonitorSplitterRatio = (float)1 / 2;

			PredefinedPanelIsVisible = true;
			PredefinedSplitterRatio = (float)3 / 4;

			SendCommandPanelIsVisible = true;
			SendFilePanelIsVisible = true;
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
			if (obj == null)
				return (false);

			LayoutSettings casted = obj as LayoutSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LayoutSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.txMonitorPanelIsVisible    == casted.txMonitorPanelIsVisible) &&
				(this.bidirMonitorPanelIsVisible == casted.bidirMonitorPanelIsVisible) &&
				(this.rxMonitorPanelIsVisible    == casted.rxMonitorPanelIsVisible) &&
				(this.monitorOrientation         == casted.monitorOrientation) &&
				(this.txMonitorSplitterRatio     == casted.txMonitorSplitterRatio) &&
				(this.rxMonitorSplitterRatio     == casted.rxMonitorSplitterRatio) &&

				(this.predefinedPanelIsVisible   == casted.predefinedPanelIsVisible) &&
				(this.predefinedSplitterRatio    == casted.predefinedSplitterRatio) &&

				(this.sendCommandPanelIsVisible  == casted.sendCommandPanelIsVisible) &&
				(this.sendFilePanelIsVisible     == casted.sendFilePanelIsVisible)
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
		public static bool operator ==(LayoutSettings lhs, LayoutSettings rhs)
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
		public static bool operator !=(LayoutSettings lhs, LayoutSettings rhs)
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
