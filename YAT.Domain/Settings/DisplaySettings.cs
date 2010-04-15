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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class DisplaySettings : MKY.Utilities.Settings.Settings, IEquatable<DisplaySettings>
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
		public DisplaySettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.separateTxRxRadix  = rhs.SeparateTxRxRadix;
			this.txRadix            = rhs.TxRadix;
			this.rxRadix            = rhs.RxRadix;
			this.showRadix          = rhs.ShowRadix;
			this.showTimeStamp      = rhs.ShowTimeStamp;
			this.showLength         = rhs.ShowLength;
			this.showConnectTime    = rhs.ShowConnectTime;
			this.showCounters       = rhs.ShowCounters;
			this.txMaxLineCount     = rhs.TxMaxLineCount;
			this.rxMaxLineCount     = rhs.RxMaxLineCount;
			this.directionLineBreakEnabled = rhs.DirectionLineBreakEnabled;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SeparateTxRxRadix  = SeparateTxRxRadixDefault;
			TxRadix            = RadixDefault;
			RxRadix            = RadixDefault;
			ShowRadix          = ShowRadixDefault;
			ShowTimeStamp      = ShowTimeStampDefault;
			ShowLength         = ShowLengthDefault;
			ShowConnectTime    = ShowConnectTimeDefault;
			ShowCounters       = ShowCountersDefault;
			TxMaxLineCount     = MaxLineCountDefault;
			RxMaxLineCount     = MaxLineCountDefault;
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
			if (obj is DisplaySettings)
				return (Equals((DisplaySettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(DisplaySettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.separateTxRxRadix         == value.separateTxRxRadix) &&
					(this.txRadix                   == value.txRadix) &&
					(this.rxRadix                   == value.rxRadix) &&
					(this.showRadix                 == value.showRadix) &&
					(this.showTimeStamp             == value.showTimeStamp) &&
					(this.showLength                == value.showLength) &&
					(this.showConnectTime           == value.showConnectTime) &&
					(this.showCounters              == value.showCounters) &&
					(this.txMaxLineCount            == value.txMaxLineCount) &&
					(this.rxMaxLineCount            == value.rxMaxLineCount) &&
					(this.directionLineBreakEnabled == value.directionLineBreakEnabled)
					);
			}
			return (false);
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
		public static bool operator ==(DisplaySettings lhs, DisplaySettings rhs)
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
		public static bool operator !=(DisplaySettings lhs, DisplaySettings rhs)
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
