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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class DisplaySettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool SeparateTxRxRadixDefault = false;

		/// <summary></summary>
		public const Radix RadixDefault = Radix.String;

		/// <summary></summary>
		public const bool ShowRadixDefault = true;

		/// <summary></summary>
		public const bool ShowBufferLineNumbersDefault = false;

		/// <summary></summary>
		public const bool ShowTotalLineNumbersDefault = false;

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
		public const int MaxBytePerLineCountDefault = 1000;

		/// <summary></summary>
		public const bool PortLineBreakEnabledDefault = true;

		/// <summary></summary>
		public const bool DirectionLineBreakEnabledDefault = true;

		/// <summary></summary>
		public static readonly string InfoSeparatorDefault = " "; // Space for better separability of elements.

		/// <summary></summary>
		public static readonly string InfoEnclosureDefault = "()"; // Parenthesis for better recognizability of info elements.

		private bool separateTxRxRadix;
		private Radix txRadix;
		private Radix rxRadix;
		private bool showRadix;
		private bool showBufferLineNumbers;
		private bool showTotalLineNumbers;
		private bool showDate;
		private bool showTime;
		private bool showPort;
		private bool showDirection;
		private bool showLength;
		private int maxLineCount;
		private int maxBytePerLineCount;

		private bool portLineBreakEnabled;
		private bool directionLineBreakEnabled;

		private InfoElementSeparatorEx infoSeparator; // = null;
		private InfoElementEnclosureEx infoEnclosure; // = null;

		private string infoSeparatorCache;      // = null;
		private string infoEnclosureLeftCache;  // = null;
		private string infoEnclosureRightCache; // = null;

		/// <summary></summary>
		public DisplaySettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
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
			SeparateTxRxRadix     = rhs.SeparateTxRxRadix;
			TxRadix               = rhs.TxRadix;
			RxRadix               = rhs.RxRadix;
			ShowRadix             = rhs.ShowRadix;
			ShowBufferLineNumbers = rhs.ShowBufferLineNumbers;
			ShowTotalLineNumbers  = rhs.ShowTotalLineNumbers;
			ShowDate              = rhs.ShowDate;
			ShowTime              = rhs.ShowTime;
			ShowPort              = rhs.ShowPort;
			ShowDirection         = rhs.ShowDirection;
			ShowLength            = rhs.ShowLength;
			MaxLineCount          = rhs.MaxLineCount;
			MaxBytePerLineCount   = rhs.MaxBytePerLineCount;

			PortLineBreakEnabled      = rhs.PortLineBreakEnabled;
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;

			InfoSeparator     = rhs.InfoSeparator;
			InfoEnclosure     = rhs.InfoEnclosure;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxRadix     = SeparateTxRxRadixDefault;
			TxRadix               = RadixDefault;
			RxRadix               = RadixDefault;
			ShowRadix             = ShowRadixDefault;
			ShowBufferLineNumbers = ShowBufferLineNumbersDefault;
			ShowTotalLineNumbers  = ShowTotalLineNumbersDefault;
			ShowDate              = ShowDateDefault;
			ShowTime              = ShowTimeDefault;
			ShowPort              = ShowPortDefault;
			ShowDirection         = ShowDirectionDefault;
			ShowLength            = ShowLengthDefault;
			MaxLineCount          = MaxLineCountDefault;
			MaxBytePerLineCount   = MaxBytePerLineCountDefault;

			PortLineBreakEnabled      = PortLineBreakEnabledDefault;
			DirectionLineBreakEnabled = DirectionLineBreakEnabledDefault;

			InfoSeparator     = InfoSeparatorDefault;
			InfoEnclosure     = InfoEnclosureDefault;
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Showable", Justification = "'Showable' is a correct English term.")]
		[XmlIgnore]
		public virtual bool TxRadixIsShowable
		{
			get
			{
				return
				(
					(TxRadix != Radix.None)   &&
					(TxRadix != Radix.String) &&
					(TxRadix != Radix.Char)
				);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Showable", Justification = "'Showable' is a correct English term.")]
		[XmlIgnore]
		public virtual bool RxRadixIsShowable
		{
			get
			{
				return
				(
					(RxRadix != Radix.None)   &&
					(RxRadix != Radix.String) &&
					(RxRadix != Radix.Char)
				);
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowBufferLineNumbers")]
		public virtual bool ShowBufferLineNumbers
		{
			get { return (this.showBufferLineNumbers); }
			set
			{
				if (this.showBufferLineNumbers != value)
				{
					this.showBufferLineNumbers = value;

					if (this.showTotalLineNumbers && value)
						this.showTotalLineNumbers = false; // Only one of the settings can be active at once.

					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTotalLineNumbers")]
		public virtual bool ShowTotalLineNumbers
		{
			get { return (this.showTotalLineNumbers); }
			set
			{
				if (this.showTotalLineNumbers != value)
				{
					this.showTotalLineNumbers = value;

					if (this.showBufferLineNumbers && value)
						this.showBufferLineNumbers = false; // Only one of the settings can be active at once.

					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <exception cref="ArgumentOutOfRangeException"> if count is below 1.</exception>
		[XmlElement("MaxLineCount")]
		public virtual int MaxLineCount
		{
			get { return (this.maxLineCount); }
			set
			{
				if (this.maxLineCount != value)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("value", value, "Line count must at least be 1!")); // Do not append 'MessageHelper.SubmitBug' as caller could rely on this exception text.

					this.maxLineCount = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Required to keep monitor update performance at a decent level. In case of very long
		/// lines, redrawing of a list box item may take way too long! It even looks as if there
		/// is a recursion within 'mscorlib.dll'.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if count is below 1.</exception>
		[XmlElement("MaxBytePerLineCount")]
		public virtual int MaxBytePerLineCount
		{
			get { return (this.maxBytePerLineCount); }
			set
			{
				if (this.maxBytePerLineCount != value)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("value", value, "Byte per line count must at least be 1!")); // Do not append 'MessageHelper.SubmitBug' as caller could rely on this exception text.

					this.maxBytePerLineCount = value;
					SetMyChanged();
				}
			}
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
					SetMyChanged();
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
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public InfoElementSeparatorEx InfoSeparator
		{
			get { return (this.infoSeparator); }
			set
			{
				if (this.infoSeparator != value)
				{
					this.infoSeparator = value;
					this.infoSeparatorCache  = this.infoSeparator.ToSeparator(); // For performance reasons.

					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("InfoSeparator")]
		public virtual string InfoSeparator_ForSerialization
		{
			get { return (InfoSeparator.ToSeparator()); } // Use separator string only!
			set { InfoSeparator = value;                }
		}

		/// <remarks>Available for performance reasons.</remarks>
		[XmlIgnore]
		public string InfoSeparatorCache
		{
			get { return (this.infoSeparatorCache); }
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public InfoElementEnclosureEx InfoEnclosure
		{
			get { return (this.infoEnclosure); }
			set
			{
				if (this.infoEnclosure != value)
				{
					this.infoEnclosure = value;
					this.infoEnclosureLeftCache  = this.infoEnclosure.ToEnclosureLeft();  // For performance reasons.
					this.infoEnclosureRightCache = this.infoEnclosure.ToEnclosureRight(); // For performance reasons.

					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("InfoEnclosure")]
		public virtual string InfoEnclosure_ForSerialization
		{
			get { return (InfoEnclosure.ToEnclosure()); } // Use enclosure string only!
			set { InfoEnclosure = value;                }
		}

		/// <remarks>Available for performance reasons.</remarks>
		[XmlIgnore]
		public string InfoEnclosureLeftCache
		{
			get { return (this.infoEnclosureLeftCache); }
		}

		/// <remarks>Available for performance reasons.</remarks>
		[XmlIgnore]
		public string InfoEnclosureRightCache
		{
			get { return (this.infoEnclosureRightCache); }
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

				hashCode = (hashCode * 397) ^ SeparateTxRxRadix    .GetHashCode();
				hashCode = (hashCode * 397) ^ TxRadix              .GetHashCode();
				hashCode = (hashCode * 397) ^ RxRadix              .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowRadix            .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowBufferLineNumbers.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowTotalLineNumbers .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowDate             .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowTime             .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowPort             .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowDirection        .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowLength           .GetHashCode();
				hashCode = (hashCode * 397) ^ MaxLineCount         .GetHashCode();
				hashCode = (hashCode * 397) ^ MaxBytePerLineCount  .GetHashCode();

				hashCode = (hashCode * 397) ^ PortLineBreakEnabled     .GetHashCode();
				hashCode = (hashCode * 397) ^ DirectionLineBreakEnabled.GetHashCode();

				hashCode = (hashCode * 397) ^ (InfoSeparator != null ? InfoSeparator.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (InfoEnclosure != null ? InfoEnclosure.GetHashCode() : 0);

				return (hashCode);
			}
		}

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

				(SeparateTxRxRadix     == other.SeparateTxRxRadix) &&
				(TxRadix               == other.TxRadix) &&
				(RxRadix               == other.RxRadix) &&
				(ShowRadix             == other.ShowRadix) &&
				(ShowBufferLineNumbers == other.ShowBufferLineNumbers) &&
				(ShowTotalLineNumbers  == other.ShowTotalLineNumbers) &&
				(ShowDate              == other.ShowDate) &&
				(ShowTime              == other.ShowTime) &&
				(ShowPort              == other.ShowPort) &&
				(ShowDirection         == other.ShowDirection) &&
				(ShowLength            == other.ShowLength) &&
				(MaxLineCount          == other.MaxLineCount) &&
				(MaxBytePerLineCount   == other.MaxBytePerLineCount) &&

				(PortLineBreakEnabled      == other.PortLineBreakEnabled) &&
				(DirectionLineBreakEnabled == other.DirectionLineBreakEnabled) &&

				(InfoSeparator     == other.InfoSeparator) &&
				(InfoEnclosure     == other.InfoEnclosure)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(DisplaySettings lhs, DisplaySettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
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
