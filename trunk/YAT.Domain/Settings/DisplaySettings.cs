//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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
			_showTimeStamp      = rhs.ShowTimeStamp;
			_showLength         = rhs.ShowLength;
			_showConnectTime    = rhs.ShowConnectTime;
			_showCounters       = rhs.ShowCounters;
			_txMaxLineCount = rhs.TxMaxLineCount;
			_rxMaxLineCount = rhs.RxMaxLineCount;
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
			ShowTimeStamp      = ShowTimeStampDefault;
			ShowLength         = ShowLengthDefault;
			ShowConnectTime    = ShowConnectTimeDefault;
			ShowCounters       = ShowCountersDefault;
			TxMaxLineCount = MaxLineCountDefault;
			RxMaxLineCount = MaxLineCountDefault;
			DirectionLineBreakEnabled = DirectionLineBreakEnabledDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxRadix")]
		public bool SeparateTxRxRadix
		{
			get { return (_separateTxRxRadix); }
			set
			{
				if (_separateTxRxRadix != value)
				{
					_separateTxRxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxRadix")]
		public Radix TxRadix
		{
			get { return (_txRadix); }
			set
			{
				if (_txRadix != value)
				{
					_txRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxRadix")]
		public Radix RxRadix
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
				if (_rxRadix != value)
				{
					_rxRadix = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTimeStamp")]
		public bool ShowTimeStamp
		{
			get { return (_showTimeStamp); }
			set
			{
				if (_showTimeStamp != value)
				{
					_showTimeStamp = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowLength")]
		public bool ShowLength
		{
			get { return (_showLength); }
			set
			{
				if (_showLength != value)
				{
					_showLength = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowConnectTime")]
		public bool ShowConnectTime
		{
			get { return (_showConnectTime); }
			set
			{
				if (_showConnectTime != value)
				{
					_showConnectTime = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowCounters")]
		public bool ShowCounters
		{
			get { return (_showCounters); }
			set
			{
				if (_showCounters != value)
				{
					_showCounters = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxMaxLineCount")]
		public int TxMaxLineCount
		{
			get { return (_txMaxLineCount); }
			set
			{
				if (_txMaxLineCount != value)
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
		public int RxMaxLineCount
		{
			get { return (_rxMaxLineCount); }
			set
			{
				if (_rxMaxLineCount != value)
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
		public int BidirMaxLineCount
		{
			get { return (TxMaxLineCount + RxMaxLineCount); }
		}

		/// <summary></summary>
		[XmlElement("DirectionLineBreakEnabled")]
		public bool DirectionLineBreakEnabled
		{
			get { return (_directionLineBreakEnabled); }
			set
			{
				if (_directionLineBreakEnabled != value)
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
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_separateTxRxRadix.Equals  (value._separateTxRxRadix) &&
					_txRadix.Equals            (value._txRadix) &&
					_rxRadix.Equals            (value._rxRadix) &&
					_showTimeStamp.Equals      (value._showTimeStamp) &&
					_showLength.Equals         (value._showLength) &&
					_showConnectTime.Equals    (value._showConnectTime) &&
					_showCounters.Equals       (value._showCounters) &&
					_txMaxLineCount.Equals (value._txMaxLineCount) &&
					_rxMaxLineCount.Equals (value._rxMaxLineCount) &&
					_directionLineBreakEnabled.Equals(value._directionLineBreakEnabled)
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
