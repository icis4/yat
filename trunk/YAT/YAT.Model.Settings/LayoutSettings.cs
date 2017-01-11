﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
	public class LayoutSettings : MKY.Settings.SettingsItem, IEquatable<LayoutSettings>
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as LayoutSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(LayoutSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				TxMonitorPanelIsVisible   .Equals(other.TxMonitorPanelIsVisible)    &&
				BidirMonitorPanelIsVisible.Equals(other.BidirMonitorPanelIsVisible) &&
				RxMonitorPanelIsVisible   .Equals(other.RxMonitorPanelIsVisible)    &&
				MonitorOrientation        .Equals(other.MonitorOrientation)         &&
				TxMonitorSplitterRatio    .Equals(other.TxMonitorSplitterRatio)     &&
				RxMonitorSplitterRatio    .Equals(other.RxMonitorSplitterRatio)     &&

				PredefinedPanelIsVisible  .Equals(other.PredefinedPanelIsVisible)   &&
				PredefinedSplitterRatio   .Equals(other.PredefinedSplitterRatio)    &&

				SendTextPanelIsVisible    .Equals(other.SendTextPanelIsVisible)     &&
				SendFilePanelIsVisible    .Equals(other.SendFilePanelIsVisible)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LayoutSettings lhs, LayoutSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that the virtual <Derived>.Equals() is called.
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
