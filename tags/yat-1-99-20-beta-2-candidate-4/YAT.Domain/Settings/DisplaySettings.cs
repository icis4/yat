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
		public const int MaximalLineCountDefault = 1000;
		/// <summary></summary>
		public const bool DirectionLineBreakEnabledDefault = true;

		private bool _separateTxRxRadix;
		private Radix _txRadix;
		private Radix _rxRadix;
		private bool _showTimeStamp;
		private bool _showLength;
		private bool _showConnectTime;
		private bool _showCounters;
		private int _txMaximalLineCount;
		private int _rxMaximalLineCount;
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
			_txMaximalLineCount = rhs.TxMaximalLineCount;
			_rxMaximalLineCount = rhs.RxMaximalLineCount;
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
			TxMaximalLineCount = MaximalLineCountDefault;
			RxMaximalLineCount = MaximalLineCountDefault;
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
		[XmlElement("TxMaximalLineCount")]
		public int TxMaximalLineCount
		{
			get { return (_txMaximalLineCount); }
			set
			{
				if (_txMaximalLineCount != value)
				{
					_txMaximalLineCount = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxMaximalLineCount")]
		public int RxMaximalLineCount
		{
			get { return (_rxMaximalLineCount); }
			set
			{
				if (_rxMaximalLineCount != value)
				{
					_rxMaximalLineCount = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public int BidirMaximalLineCount
		{
			get { return (TxMaximalLineCount + RxMaximalLineCount); }
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
			// ensure that object.operator!=() is called
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
					_txMaximalLineCount.Equals (value._txMaximalLineCount) &&
					_rxMaximalLineCount.Equals (value._rxMaximalLineCount) &&
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
