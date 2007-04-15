using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class DisplaySettings : Utilities.Settings.Settings
	{
		public const Radix RadixDefault = Radix.String;
		public const bool ShowTimestampDefault = false;
		public const bool ShowLengthDefault = false;
		public const bool ShowCountersDefault = false;
		public const int MaximalLineCountDefault = 100;

		private Radix _radix;
		private bool _showTimestamp;
		private bool _showLength;
		private bool _showCounters;
		private int _txMaximalLineCount;
		private int _rxMaximalLineCount;

		public DisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public DisplaySettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public DisplaySettings(DisplaySettings rhs)
			: base(rhs)
		{
			Radix         = rhs.Radix;
			ShowTimestamp = rhs.ShowTimestamp;
			ShowLength    = rhs.ShowLength;
			ShowCounters  = rhs.ShowCounters;
			TxMaximalLineCount = rhs.TxMaximalLineCount;
			RxMaximalLineCount = rhs.RxMaximalLineCount;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			Radix         = RadixDefault;
			ShowTimestamp = ShowTimestampDefault;
			ShowLength    = ShowLengthDefault;
			ShowCounters  = ShowCountersDefault;
			TxMaximalLineCount = MaximalLineCountDefault;
			RxMaximalLineCount = MaximalLineCountDefault;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("Radix")]
		public Radix Radix
		{
			get { return (_radix); }
			set
			{
				if (_radix != value)
				{
					_radix = value;
					SetChanged();
				}
			}
		}

		[XmlElement("ShowTimestamp")]
		public bool ShowTimestamp
		{
			get { return (_showTimestamp); }
			set
			{
				if (_showTimestamp != value)
				{
					_showTimestamp = value;
					SetChanged();
				}
			}
		}

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

		[XmlIgnore]
		public int BidirMaximalLineCount
		{
			get { return (TxMaximalLineCount + RxMaximalLineCount); }
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
					_radix.Equals(value._radix) &&
					_showTimestamp.Equals(value._showTimestamp) &&
					_showLength.Equals(value._showLength) &&
					_showCounters.Equals(value._showCounters) &&
					_txMaximalLineCount.Equals(value._txMaximalLineCount) &&
					_rxMaximalLineCount.Equals(value._rxMaximalLineCount)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
