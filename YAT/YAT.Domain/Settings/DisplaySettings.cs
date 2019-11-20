//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

using YAT.Domain.Utilities;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class DisplaySettings : MKY.Settings.SettingsItem, IEquatable<DisplaySettings>
	{
		/// <summary></summary>
		public const bool SeparateTxRxRadixDefault = false;

		/// <summary></summary>
		public const Radix RadixDefault = RadixEx.Default;

		/// <summary></summary>
		public const Radix RadixTextDefault = RadixEx.TextDefault;

		/// <summary></summary>
		public const Radix RadixBinaryDefault = RadixEx.BinaryDefault;

		/// <summary></summary>
		public const bool ShowRadixDefault = true;

		/// <summary></summary>
		public const bool ShowLineNumbersDefault = false;

		/// <summary></summary>
		public const LineNumberSelection LineNumberSelectionDefault = LineNumberSelectionEx.Default;

		/// <summary></summary>
		public const bool ShowTimeStampDefault = false;

		/// <summary></summary>
		public const string TimeStampFormatDefault = TimeStampFormatPresetEx.DefaultFormat;

		/// <summary></summary>
		public const bool TimeStampUseUtcDefault = false;

		/// <summary></summary>
		public const bool ShowTimeSpanDefault = false;

		/// <summary></summary>
		public const string TimeSpanFormatDefault = TimeSpanFormatPresetEx.DefaultFormat;

		/// <summary></summary>
		public const bool ShowTimeDeltaDefault = false;

		/// <summary></summary>
		public const string TimeDeltaFormatDefault = TimeDeltaFormatPresetEx.DefaultFormat;

		/// <summary></summary>
		public const bool ShowDeviceDefault = false;

		/// <summary></summary>
		public const bool ShowDirectionDefault = false;

		/// <summary></summary>
		public const bool ShowLengthDefault = false;

		/// <summary></summary>
		public const LengthSelection LengthSelectionDefault = LengthSelectionEx.Default;

		/// <summary></summary>
		public const LengthSelection LengthSelectionTextDefault = LengthSelectionEx.TextDefault;

		/// <summary></summary>
		public const LengthSelection LengthSelectionBinaryDefault = LengthSelectionEx.BinaryDefault;

		/// <summary></summary>
		public const bool ShowDurationDefault = false;

		/// <summary></summary>
		public const string TimeDurationFormatDefault = TimeDeltaFormatPresetEx.DefaultFormat;

		/// <remarks>
		/// Prepended "IO" for distiction with ASCII "Control" characters.
		/// </remarks>
		public const bool IncludeIOControlDefault = false;

		/// <summary></summary>
		public const int MaxLineCountDefault = 1000;

		/// <summary></summary>
		public const int MaxLineLengthDefault = 1000;

		/// <summary></summary>
		public const bool ShowCopyOfActiveLineDefault = false;

		/// <summary></summary>
		public const bool DeviceLineBreakEnabledDefault = true;

		/// <summary></summary>
		public const bool DirectionLineBreakEnabledDefault = true;

		/// <summary></summary>
		public const bool ChunkLineBreakEnabledDefault = false;

		/// <summary></summary>
		public static readonly string InfoSeparatorDefault = " "; // Space for better separability of elements.

		/// <summary></summary>
		public static readonly string InfoEnclosureDefault = "()"; // Parentheses for better recognizability of info elements.

		private bool   separateTxRxRadix;
		private Radix  txRadix;
		private Radix  rxRadix;
		private bool   showRadix;
		private bool   showLineNumbers;
		private LineNumberSelection lineNumberSelection;
		private bool   showTimeStamp;
		private string timeStampFormat;
		private bool   timeStampUseUtc;
		private bool   showTimeSpan;
		private string timeSpanFormat;
		private bool   showTimeDelta;
		private string timeDeltaFormat;
		private bool   showDevice;
		private bool   showDirection;
		private bool   showLength;
		private LengthSelection lengthSelection;
		private bool   showDuration;
		private string timeDurationFormat;
		private bool   includeIOControl;

		private bool deviceLineBreakEnabled;
		private bool directionLineBreakEnabled;
		private bool chunkLineBreakEnabled;

		private int    maxLineCount;
		private int    maxLineLength;
		private bool   showCopyOfActiveLine;

		private InfoSeparatorEx infoSeparator; // = null;
		private InfoEnclosureEx infoEnclosure; // = null;

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
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public DisplaySettings(DisplaySettings rhs)
			: base(rhs)
		{
			SeparateTxRxRadix   = rhs.SeparateTxRxRadix;
			TxRadix             = rhs.TxRadix;
			RxRadix             = rhs.RxRadix;
			ShowRadix           = rhs.ShowRadix;
			ShowLineNumbers     = rhs.ShowLineNumbers;
			LineNumberSelection = rhs.LineNumberSelection;
			ShowTimeStamp       = rhs.ShowTimeStamp;
			TimeStampFormat     = rhs.TimeStampFormat;
			TimeStampUseUtc     = rhs.TimeStampUseUtc;
			ShowTimeSpan        = rhs.ShowTimeSpan;
			TimeSpanFormat      = rhs.TimeSpanFormat;
			ShowTimeDelta       = rhs.ShowTimeDelta;
			TimeDeltaFormat     = rhs.TimeDeltaFormat;
			ShowDevice          = rhs.ShowDevice;
			ShowDirection       = rhs.ShowDirection;
			ShowLength          = rhs.ShowLength;
			LengthSelection     = rhs.LengthSelection;
			ShowDuration        = rhs.ShowDuration;
			TimeDurationFormat  = rhs.TimeDurationFormat;
			IncludeIOControl    = rhs.IncludeIOControl;

			DeviceLineBreakEnabled    = rhs.DeviceLineBreakEnabled;
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;
			ChunkLineBreakEnabled     = rhs.ChunkLineBreakEnabled;

			MaxLineCount         = rhs.MaxLineCount;
			MaxLineLength        = rhs.MaxLineLength;
			ShowCopyOfActiveLine = rhs.ShowCopyOfActiveLine;

			InfoSeparator = rhs.InfoSeparator;
			InfoEnclosure = rhs.InfoEnclosure;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxRadix   = SeparateTxRxRadixDefault;
			TxRadix             = RadixDefault;
			RxRadix             = RadixDefault;
			ShowRadix           = ShowRadixDefault;
			ShowLineNumbers     = ShowLineNumbersDefault;
			LineNumberSelection = LineNumberSelectionDefault;
			ShowTimeStamp       = ShowTimeStampDefault;
			TimeStampFormat     = TimeStampFormatDefault;
			TimeStampUseUtc     = TimeStampUseUtcDefault;
			ShowTimeSpan        = ShowTimeSpanDefault;
			TimeSpanFormat      = TimeSpanFormatDefault;
			ShowTimeDelta       = ShowTimeDeltaDefault;
			TimeDeltaFormat     = TimeDeltaFormatDefault;
			ShowDevice          = ShowDeviceDefault;
			ShowDirection       = ShowDirectionDefault;
			LengthSelection     = LengthSelectionDefault;
			ShowLength          = ShowLengthDefault;
			ShowDuration        = ShowDurationDefault;
			TimeDurationFormat  = TimeDurationFormatDefault;
			IncludeIOControl    = IncludeIOControlDefault;

			DeviceLineBreakEnabled    = DeviceLineBreakEnabledDefault;
			DirectionLineBreakEnabled = DirectionLineBreakEnabledDefault;
			ChunkLineBreakEnabled     = ChunkLineBreakEnabledDefault;

			MaxLineCount         = MaxLineCountDefault;
			MaxLineLength        = MaxLineLengthDefault;
			ShowCopyOfActiveLine = ShowCopyOfActiveLineDefault;

			InfoSeparator = InfoSeparatorDefault;
			InfoEnclosure = InfoEnclosureDefault;
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
				else // Rx redirects to Tx:
					return (this.txRadix);
			}
			set
			{
				if (this.rxRadix != value)
				{
					this.rxRadix = value;
					SetMyChanged();
				}

				// Do not redirect on 'set'. this would not be an understandable behavior.
				// It could even confuse the user, e.g. when temporarily separating the settings,
				// and then load them again from XML => temporary settings get lost.
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
		[XmlElement("ShowLineNumbers")]
		public virtual bool ShowLineNumbers
		{
			get { return (this.showLineNumbers); }
			set
			{
				if (this.showLineNumbers != value)
				{
					this.showLineNumbers = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LineNumberSelection")]
		public virtual LineNumberSelection LineNumberSelection
		{
			get { return (this.lineNumberSelection); }
			set
			{
				if (this.lineNumberSelection != value)
				{
					this.lineNumberSelection = value;
					SetMyChanged();
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
				if (this.showTimeStamp != value)
				{
					this.showTimeStamp = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeStampFormat")]
		public virtual string TimeStampFormat
		{
			get { return (this.timeStampFormat); }
			set
			{
				if (this.timeStampFormat != value)
				{
					this.timeStampFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// XML element is named "UTC" instead of .NET-style "Utc" for better readability.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop you are right, I don't like 'Utc' either...")]
		[XmlElement("TimeStampUseUTC")]
		public virtual bool TimeStampUseUtc
		{
			get { return (this.timeStampUseUtc); }
			set
			{
				if (this.timeStampUseUtc != value)
				{
					this.timeStampUseUtc = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTimeSpan")]
		public virtual bool ShowTimeSpan
		{
			get { return (this.showTimeSpan); }
			set
			{
				if (this.showTimeSpan != value)
				{
					this.showTimeSpan = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeSpanFormat")]
		public virtual string TimeSpanFormat
		{
			get { return (this.timeSpanFormat); }
			set
			{
				if (this.timeSpanFormat != value)
				{
					this.timeSpanFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowTimeDelta")]
		public virtual bool ShowTimeDelta
		{
			get { return (this.showTimeDelta); }
			set
			{
				if (this.showTimeDelta != value)
				{
					this.showTimeDelta = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeDeltaFormat")]
		public virtual string TimeDeltaFormat
		{
			get { return (this.timeDeltaFormat); }
			set
			{
				if (this.timeDeltaFormat != value)
				{
					this.timeDeltaFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		[XmlElement("ShowDevice")]
		public virtual bool ShowDevice
		{
			get { return (this.showDevice); }
			set
			{
				if (this.showDevice != value)
				{
					this.showDevice = value;
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

		/// <summary></summary>
		[XmlElement("LengthSelection")]
		public virtual LengthSelection LengthSelection
		{
			get { return (this.lengthSelection); }
			set
			{
				if (this.lengthSelection != value)
				{
					this.lengthSelection = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowDuration")]
		public virtual bool ShowDuration
		{
			get { return (this.showDuration); }
			set
			{
				if (this.showDuration != value)
				{
					this.showDuration = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeDurationFormat")]
		public virtual string TimeDurationFormat
		{
			get { return (this.timeDurationFormat); }
			set
			{
				if (this.timeDurationFormat != value)
				{
					this.timeDurationFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Prepended "IO" for distiction with ASCII "Control" characters.
		/// </remarks>
		[XmlElement("IncludeIOControl")]
		public virtual bool IncludeIOControl
		{
			get { return (this.includeIOControl); }
			set
			{
				if (this.includeIOControl != value)
				{
					this.includeIOControl = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		[XmlElement("DeviceLineBreakEnabled")]
		public virtual bool DeviceLineBreakEnabled
		{
			get { return (this.deviceLineBreakEnabled); }
			set
			{
				if (this.deviceLineBreakEnabled != value)
				{
					this.deviceLineBreakEnabled = value;
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

		/// <summary></summary>
		[XmlElement("ChunkLineBreakEnabled")]
		public virtual bool ChunkLineBreakEnabled
		{
			get { return (this.chunkLineBreakEnabled); }
			set
			{
				if (this.chunkLineBreakEnabled != value)
				{
					this.chunkLineBreakEnabled = value;
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
						throw (new ArgumentOutOfRangeException("value", value, "Line count must at least be 1!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

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
		/// <remarks>
		/// Named "length" rather than "char count" or "byte count" because it is either or,
		/// depending on the terminal type. Also, "length" seems the most appropriate term.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if length is below 1.</exception>
		[XmlElement("MaxLineLength")]
		public virtual int MaxLineLength
		{
			get { return (this.maxLineLength); }
			set
			{
				if (this.maxLineLength != value)
				{
					if (value < 1)
						throw (new ArgumentOutOfRangeException("value", value, "Line length must at least be 1!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

					this.maxLineLength = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Name only "Active" instead of "LastActive" for simplicity.
		/// </remarks>
		[XmlElement("ShowCopyOfActiveLine")]
		public virtual bool ShowCopyOfActiveLine
		{
			get { return (this.showCopyOfActiveLine); }
			set
			{
				if (this.showCopyOfActiveLine != value)
				{
					this.showCopyOfActiveLine = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public InfoSeparatorEx InfoSeparator
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
		public InfoEnclosureEx InfoEnclosure
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

				hashCode = (hashCode * 397) ^  SeparateTxRxRadix  .GetHashCode();
				hashCode = (hashCode * 397) ^  TxRadix            .GetHashCode();
				hashCode = (hashCode * 397) ^  RxRadix            .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowRadix          .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowLineNumbers    .GetHashCode();
				hashCode = (hashCode * 397) ^  LineNumberSelection.GetHashCode();
				hashCode = (hashCode * 397) ^  ShowTimeStamp      .GetHashCode();
				hashCode = (hashCode * 397) ^ (TimeStampFormat    != null ? TimeStampFormat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  TimeStampUseUtc    .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowTimeSpan       .GetHashCode();
				hashCode = (hashCode * 397) ^ (TimeSpanFormat     != null ? TimeSpanFormat .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  ShowTimeDelta      .GetHashCode();
				hashCode = (hashCode * 397) ^ (TimeDeltaFormat    != null ? TimeDeltaFormat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  ShowDevice         .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowDirection      .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowLength         .GetHashCode();
				hashCode = (hashCode * 397) ^  LengthSelection    .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowDuration       .GetHashCode();
				hashCode = (hashCode * 397) ^ (TimeDurationFormat != null ? TimeDurationFormat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  IncludeIOControl   .GetHashCode();

				hashCode = (hashCode * 397) ^  DeviceLineBreakEnabled   .GetHashCode();
				hashCode = (hashCode * 397) ^  DirectionLineBreakEnabled.GetHashCode();
				hashCode = (hashCode * 397) ^  ChunkLineBreakEnabled    .GetHashCode();

				hashCode = (hashCode * 397) ^  MaxLineCount        .GetHashCode();
				hashCode = (hashCode * 397) ^  MaxLineLength       .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowCopyOfActiveLine.GetHashCode();

				hashCode = (hashCode * 397) ^ (InfoSeparator != null ? InfoSeparator.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (InfoEnclosure != null ? InfoEnclosure.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as DisplaySettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(DisplaySettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				SeparateTxRxRadix                  .Equals(other.SeparateTxRxRadix)   &&
				TxRadix                            .Equals(other.TxRadix)             &&
				RxRadix                            .Equals(other.RxRadix)             &&
				ShowRadix                          .Equals(other.ShowRadix)           &&
				ShowLineNumbers                    .Equals(other.ShowLineNumbers)     &&
				LineNumberSelection                .Equals(other.LineNumberSelection) &&
				ShowTimeStamp                      .Equals(other.ShowTimeStamp)       &&
				StringEx.EqualsOrdinal(TimeStampFormat,    other.TimeStampFormat)     &&
				TimeStampUseUtc                    .Equals(other.TimeStampUseUtc)     &&
				ShowTimeSpan                       .Equals(other.ShowTimeSpan)        &&
				StringEx.EqualsOrdinal(TimeSpanFormat,     other.TimeSpanFormat)      &&
				ShowTimeDelta                      .Equals(other.ShowTimeDelta)       &&
				StringEx.EqualsOrdinal(TimeDeltaFormat,    other.TimeDeltaFormat)     &&
				ShowDevice                         .Equals(other.ShowDevice)          &&
				ShowDirection                      .Equals(other.ShowDirection)       &&
				ShowLength                         .Equals(other.ShowLength)          &&
				LengthSelection                    .Equals(other.LengthSelection)     &&
				ShowDuration                       .Equals(other.ShowDuration)        &&
				StringEx.EqualsOrdinal(TimeDurationFormat, other.TimeDurationFormat)  &&
				IncludeIOControl                   .Equals(other.IncludeIOControl)    &&

				DeviceLineBreakEnabled   .Equals(other.DeviceLineBreakEnabled)    &&
				DirectionLineBreakEnabled.Equals(other.DirectionLineBreakEnabled) &&
				ChunkLineBreakEnabled    .Equals(other.ChunkLineBreakEnabled)     &&

				MaxLineCount        .Equals(other.MaxLineCount)         &&
				MaxLineLength       .Equals(other.MaxLineLength)        &&
				ShowCopyOfActiveLine.Equals(other.ShowCopyOfActiveLine) &&

				ObjectEx.Equals(InfoSeparator, other.InfoSeparator) &&
				ObjectEx.Equals(InfoEnclosure, other.InfoEnclosure)
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

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
