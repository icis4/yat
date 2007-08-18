using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class DisplaySettings : Utilities.Settings.Settings, IEquatable<DisplaySettings>
	{
		/// <summary></summary>
		public const Radix RadixDefault = Radix.String;
		/// <summary></summary>
		public const bool ShowTimeStampDefault = false;
		/// <summary></summary>
		public const bool ShowLengthDefault = false;
		/// <summary></summary>
		public const bool ShowCountersDefault = false;
		/// <summary></summary>
		public const int MaximalLineCountDefault = 100;

		private Radix _radix;
		private bool _showTimeStamp;
		private bool _showLength;
		private bool _showCounters;
		private int _txMaximalLineCount;
		private int _rxMaximalLineCount;

		/// <summary></summary>
		public DisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public DisplaySettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public DisplaySettings(DisplaySettings rhs)
			: base(rhs)
		{
			Radix         = rhs.Radix;
			ShowTimeStamp = rhs.ShowTimeStamp;
			ShowLength    = rhs.ShowLength;
			ShowCounters  = rhs.ShowCounters;
			TxMaximalLineCount = rhs.TxMaximalLineCount;
			RxMaximalLineCount = rhs.RxMaximalLineCount;
			ClearChanged();
		}

		/// <summary></summary>
		protected override void SetMyDefaults()
		{
			Radix         = RadixDefault;
			ShowTimeStamp = ShowTimeStampDefault;
			ShowLength    = ShowLengthDefault;
			ShowCounters  = ShowCountersDefault;
			TxMaximalLineCount = MaximalLineCountDefault;
			RxMaximalLineCount = MaximalLineCountDefault;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
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
					_showTimeStamp.Equals(value._showTimeStamp) &&
					_showLength.Equals(value._showLength) &&
					_showCounters.Equals(value._showCounters) &&
					_txMaximalLineCount.Equals(value._txMaximalLineCount) &&
					_rxMaximalLineCount.Equals(value._rxMaximalLineCount)
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
