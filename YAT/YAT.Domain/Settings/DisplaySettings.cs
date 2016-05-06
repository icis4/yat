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
	/// <summary></summary>
	[Serializable]
	public class DisplaySettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool SeparateTxRxRadixDefault = false;

		/// <summary></summary>
		public const Radix RadixDefault = Radix.String;

		/// <summary></summary>
		public const bool ShowRadixDefault = true;

		/// <summary></summary>
		public const bool ShowLineNumbersDefault = false;

		/// <summary></summary>
		public const bool ShowDateDefault = false;

		/// <summary></summary>
		public const bool ShowTimeDefault = false;

		/// <summary></summary>
		public const bool ShowPortDefault = false;

		/// <summary></summary>
		public const bool ShowDirectionDefault = false;

		/// <summary></summary>
		public const bool ShowLengthDefault = false;

		/// <summary></summary>
		public const int MaxLineCountDefault = 1000;

		/// <summary></summary>
		public const bool PortLineBreakEnabledDefault = true;

		/// <summary></summary>
		public const bool DirectionLineBreakEnabledDefault = true;

		private bool separateTxRxRadix;
		private Radix txRadix;
		private Radix rxRadix;
		private bool showRadix;
		private bool showLineNumbers;
		private bool showDate;
		private bool showTime;
		private bool showPort;
		private bool showDirection;
		private bool showLength;
		private int txMaxLineCount;
		private int rxMaxLineCount;

		private bool portLineBreakEnabled;
		private bool directionLineBreakEnabled;

		/// <summary></summary>
		public DisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public DisplaySettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public DisplaySettings(DisplaySettings rhs)
			: base(rhs)
		{
			SeparateTxRxRadix = rhs.SeparateTxRxRadix;
			TxRadix           = rhs.TxRadix;
			RxRadix           = rhs.RxRadix;
			ShowRadix         = rhs.ShowRadix;
			ShowLineNumbers   = rhs.ShowLineNumbers;
			ShowDate          = rhs.ShowDate;
			ShowTime          = rhs.ShowTime;
			ShowPort          = rhs.ShowPort;
			ShowDirection     = rhs.ShowDirection;
			ShowLength        = rhs.ShowLength;
			TxMaxLineCount    = rhs.TxMaxLineCount;
			RxMaxLineCount    = rhs.RxMaxLineCount;

			PortLineBreakEnabled      = rhs.PortLineBreakEnabled;
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxRadix = SeparateTxRxRadixDefault;
			TxRadix           = RadixDefault;
			RxRadix           = RadixDefault;
			ShowRadix         = ShowRadixDefault;
			ShowLineNumbers   = ShowLineNumbersDefault;
			ShowDate          = ShowDateDefault;
			ShowTime          = ShowTimeDefault;
			ShowPort          = ShowPortDefault;
			ShowDirection     = ShowDirectionDefault;
			ShowLength        = ShowLengthDefault;
			TxMaxLineCount    = MaxLineCountDefault;
			RxMaxLineCount    = MaxLineCountDefault;

			PortLineBreakEnabled      = PortLineBreakEnabledDefault;
			DirectionLineBreakEnabled = DirectionLineBreakEnabledDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxRadix")]
		public virtual bool SeparateTxRxRadix
		{
			get { return (this.separateTxRxRadix); }
			set
			{
				if (this.separateTxRxRadix != value)
				{
					this.separateTxRxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxRadix")]
		public virtual Radix TxRadix
		{
			get { return (this.txRadix); }
			set
			{
				if (this.txRadix != value)
				{
					this.txRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxRadix")]
		public virtual Radix RxRadix
		{
			get
			{
				if (this.separateTxRxRadix)
					return (this.rxRadix);
				else
					return (this.txRadix);
			}
			set
			{
				if (this.rxRadix != value)
				{
					this.rxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowRadix")]
		public virtual bool ShowRadix
		{
			get { return (this.showRadix); }
			set
			{
				if (this.showRadix != value)
				{
					this.showRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowLineNumbers")]
		public virtual bool ShowLineNumbers
		{
			get { return (this.showLineNumbers); }
			set
			{
				if (this.showLineNumbers != value)
				{
					this.showLineNumbers = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowDate")]
		public virtual bool ShowDate
		{
			get { return (this.showDate); }
			set
			{
				if (this.showDate != value)
				{
					this.showDate = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTime")]
		public virtual bool ShowTime
		{
			get { return (this.showTime); }
			set
			{
				if (this.showTime != value)
				{
					this.showTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowPort")]
		public virtual bool ShowPort
		{
			get { return (this.showPort); }
			set
			{
				if (this.showPort != value)
				{
					this.showPort = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowDirection")]
		public virtual bool ShowDirection
		{
			get { return (this.showDirection); }
			set
			{
				if (this.showDirection != value)
				{
					this.showDirection = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowLength")]
		public virtual bool ShowLength
		{
			get { return (this.showLength); }
			set
			{
				if (this.showLength != value)
				{
					this.showLength = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxMaxLineCount")]
		public virtual int TxMaxLineCount
		{
			get { return (this.txMaxLineCount); }
			set
			{
				if (this.txMaxLineCount != value)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("value", value, "Line count must at least be 1."));

					this.txMaxLineCount = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxMaxLineCount")]
		public virtual int RxMaxLineCount
		{
			get { return (this.rxMaxLineCount); }
			set
			{
				if (this.rxMaxLineCount != value)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("value", value, "Line count must at least be 1."));

					this.rxMaxLineCount = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int BidirMaxLineCount
		{
			get { return (TxMaxLineCount + RxMaxLineCount); }
		}

		/// <summary></summary>
		[XmlElement("PortLineBreakEnabled")]
		public virtual bool PortLineBreakEnabled
		{
			get { return (this.portLineBreakEnabled); }
			set
			{
				if (this.portLineBreakEnabled != value)
				{
					this.portLineBreakEnabled = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DirectionLineBreakEnabled")]
		public virtual bool DirectionLineBreakEnabled
		{
			get { return (this.directionLineBreakEnabled); }
			set
			{
				if (this.directionLineBreakEnabled != value)
				{
					this.directionLineBreakEnabled = value;
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

			DisplaySettings other = (DisplaySettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(SeparateTxRxRadix == other.SeparateTxRxRadix) &&
				(TxRadix           == other.TxRadix) &&
				(RxRadix           == other.RxRadix) &&
				(ShowRadix         == other.ShowRadix) &&
				(ShowLineNumbers   == other.ShowLineNumbers) &&
				(ShowDate          == other.ShowDate) &&
				(ShowTime          == other.ShowTime) &&
				(ShowPort          == other.ShowPort) &&
				(ShowDirection     == other.ShowDirection) &&
				(ShowLength        == other.ShowLength) &&
				(TxMaxLineCount    == other.TxMaxLineCount) &&
				(RxMaxLineCount    == other.rxMaxLineCount) &&

				(PortLineBreakEnabled      == other.PortLineBreakEnabled) &&
				(DirectionLineBreakEnabled == other.DirectionLineBreakEnabled)
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

				hashCode = (hashCode * 397) ^ SeparateTxRxRadix.GetHashCode();
				hashCode = (hashCode * 397) ^ TxRadix          .GetHashCode();
				hashCode = (hashCode * 397) ^ RxRadix          .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowRadix        .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowLineNumbers  .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowDate         .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowTime         .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowPort         .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowDirection    .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowLength       .GetHashCode();
				hashCode = (hashCode * 397) ^ TxMaxLineCount   .GetHashCode();
				hashCode = (hashCode * 397) ^ RxMaxLineCount   .GetHashCode();

				hashCode = (hashCode * 397) ^ PortLineBreakEnabled     .GetHashCode();
				hashCode = (hashCode * 397) ^ DirectionLineBreakEnabled.GetHashCode();

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
