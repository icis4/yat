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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class DisplaySettings : MKY.Settings.Settings
	{
		/// <summary></summary>
		public const bool SeparateTxRxRadixDefault = false;

		/// <summary></summary>
		public const Radix RadixDefault = Radix.String;

		/// <summary></summary>
		public const bool ShowRadixDefault = true;

		/// <summary></summary>
		public const bool ShowTimeStampDefault = false;

		/// <summary></summary>
		public const bool ShowLengthDefault = false;

		/// <summary></summary>
		public const bool ShowConnectTimeDefault = false;

		/// <summary></summary>
		public const bool ShowCountersDefault = false;

		/// <summary></summary>
		public const int MaxLineCountDefault = 1000;

		/// <summary></summary>
		public const bool DirectionLineBreakEnabledDefault = true;

		private bool separateTxRxRadix;
		private Radix txRadix;
		private Radix rxRadix;
		private bool showRadix;
		private bool showTimeStamp;
		private bool showLength;
		private bool showConnectTime;
		private bool showCounters;
		private int txMaxLineCount;
		private int rxMaxLineCount;
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

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public DisplaySettings(DisplaySettings rhs)
			: base(rhs)
		{
			SeparateTxRxRadix = rhs.SeparateTxRxRadix;
			TxRadix           = rhs.TxRadix;
			RxRadix           = rhs.RxRadix;
			ShowRadix         = rhs.ShowRadix;
			ShowTimeStamp     = rhs.ShowTimeStamp;
			ShowLength        = rhs.ShowLength;
			ShowConnectTime   = rhs.ShowConnectTime;
			ShowCounters      = rhs.ShowCounters;
			TxMaxLineCount    = rhs.TxMaxLineCount;
			RxMaxLineCount    = rhs.RxMaxLineCount;
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SeparateTxRxRadix = SeparateTxRxRadixDefault;
			TxRadix           = RadixDefault;
			RxRadix           = RadixDefault;
			ShowRadix         = ShowRadixDefault;
			ShowTimeStamp     = ShowTimeStampDefault;
			ShowLength        = ShowLengthDefault;
			ShowConnectTime   = ShowConnectTimeDefault;
			ShowCounters      = ShowCountersDefault;
			TxMaxLineCount    = MaxLineCountDefault;
			RxMaxLineCount    = MaxLineCountDefault;
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
				if (value != this.separateTxRxRadix)
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
				if (value != this.txRadix)
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
				if (value != this.rxRadix)
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
				if (value != this.showRadix)
				{
					this.showRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTimeStamp")]
		public virtual bool ShowTimeStamp
		{
			get { return (this.showTimeStamp); }
			set
			{
				if (value != this.showTimeStamp)
				{
					this.showTimeStamp = value;
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
				if (value != this.showLength)
				{
					this.showLength = value;
					SetChanged();
				}
			}
		}

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
		[XmlElement("ShowCounters")]
		public virtual bool ShowCounters
		{
			get { return (this.showCounters); }
			set
			{
				if (value != this.showCounters)
				{
					this.showCounters = value;
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
				if (value != this.txMaxLineCount)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("TxMaxLineCount", "Line count must at least be 1"));

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
				if (value != this.rxMaxLineCount)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("RxMaxLineCount", "Line count must at least be 1"));

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
		[XmlElement("DirectionLineBreakEnabled")]
		public virtual bool DirectionLineBreakEnabled
		{
			get { return (this.directionLineBreakEnabled); }
			set
			{
				if (value != this.directionLineBreakEnabled)
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

				(this.separateTxRxRadix         == other.separateTxRxRadix) &&
				(this.txRadix                   == other.txRadix) &&
				(this.rxRadix                   == other.rxRadix) &&
				(this.showRadix                 == other.showRadix) &&
				(this.showTimeStamp             == other.showTimeStamp) &&
				(this.showLength                == other.showLength) &&
				(this.showConnectTime           == other.showConnectTime) &&
				(this.showCounters              == other.showCounters) &&
				(this.txMaxLineCount            == other.txMaxLineCount) &&
				(this.rxMaxLineCount            == other.rxMaxLineCount) &&
				(this.directionLineBreakEnabled == other.directionLineBreakEnabled)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.separateTxRxRadix        .GetHashCode() ^
				this.txRadix                  .GetHashCode() ^
				this.rxRadix                  .GetHashCode() ^
				this.showRadix                .GetHashCode() ^
				this.showTimeStamp            .GetHashCode() ^
				this.showLength               .GetHashCode() ^
				this.showConnectTime          .GetHashCode() ^
				this.showCounters             .GetHashCode() ^
				this.txMaxLineCount           .GetHashCode() ^
				this.rxMaxLineCount           .GetHashCode() ^
				this.directionLineBreakEnabled.GetHashCode()
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