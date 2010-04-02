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

		private bool _separateTxRxRadix;
		private Radix _txRadix;
		private Radix _rxRadix;
		private bool _showRadix;
		private bool _showTimeStamp;
		private bool _showLength;
		private bool _showConnectTime;
		private bool _showCounters;
		private int _txMaxLineCount;
		private int _rxMaxLineCount;
		private bool _directionLineBreakEnabled;

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
			_separateTxRxRadix  = rhs.SeparateTxRxRadix;
			_txRadix            = rhs.TxRadix;
			_rxRadix            = rhs.RxRadix;
			_showRadix          = rhs.ShowRadix;
			_showTimeStamp      = rhs.ShowTimeStamp;
			_showLength         = rhs.ShowLength;
			_showConnectTime    = rhs.ShowConnectTime;
			_showCounters       = rhs.ShowCounters;
			_txMaxLineCount     = rhs.TxMaxLineCount;
			_rxMaxLineCount     = rhs.RxMaxLineCount;
			_directionLineBreakEnabled = rhs.DirectionLineBreakEnabled;

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
			get { return (_separateTxRxRadix); }
			set
			{
				if (value != _separateTxRxRadix)
				{
					_separateTxRxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxRadix")]
		public virtual Radix TxRadix
		{
			get { return (_txRadix); }
			set
			{
				if (value != _txRadix)
				{
					_txRadix = value;
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
				if (_separateTxRxRadix)
					return (_rxRadix);
				else
					return (_txRadix);
			}
			set
			{
				if (value != _rxRadix)
				{
					_rxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowRadix")]
		public virtual bool ShowRadix
		{
			get { return (_showRadix); }
			set
			{
				if (value != _showRadix)
				{
					_showRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTimeStamp")]
		public virtual bool ShowTimeStamp
		{
			get { return (_showTimeStamp); }
			set
			{
				if (value != _showTimeStamp)
				{
					_showTimeStamp = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowLength")]
		public virtual bool ShowLength
		{
			get { return (_showLength); }
			set
			{
				if (value != _showLength)
				{
					_showLength = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowConnectTime")]
		public virtual bool ShowConnectTime
		{
			get { return (_showConnectTime); }
			set
			{
				if (value != _showConnectTime)
				{
					_showConnectTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowCounters")]
		public virtual bool ShowCounters
		{
			get { return (_showCounters); }
			set
			{
				if (value != _showCounters)
				{
					_showCounters = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxMaxLineCount")]
		public virtual int TxMaxLineCount
		{
			get { return (_txMaxLineCount); }
			set
			{
				if (value != _txMaxLineCount)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("TxMaxLineCount", "Line count must at least be 1"));

					_txMaxLineCount = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxMaxLineCount")]
		public virtual int RxMaxLineCount
		{
			get { return (_rxMaxLineCount); }
			set
			{
				if (value != _rxMaxLineCount)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("RxMaxLineCount", "Line count must at least be 1"));

					_rxMaxLineCount = value;
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
			get { return (_directionLineBreakEnabled); }
			set
			{
				if (value != _directionLineBreakEnabled)
				{
					_directionLineBreakEnabled = value;
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
					(_separateTxRxRadix         == value._separateTxRxRadix) &&
					(_txRadix                   == value._txRadix) &&
					(_rxRadix                   == value._rxRadix) &&
					(_showRadix                 == value._showRadix) &&
					(_showTimeStamp             == value._showTimeStamp) &&
					(_showLength                == value._showLength) &&
					(_showConnectTime           == value._showConnectTime) &&
					(_showCounters              == value._showCounters) &&
					(_txMaxLineCount            == value._txMaxLineCount) &&
					(_rxMaxLineCount            == value._rxMaxLineCount) &&
					(_directionLineBreakEnabled == value._directionLineBreakEnabled)
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
