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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Xml.Serialization;

using MKY;

using YAT.Format.Types;

namespace YAT.Format.Settings
{
	/// <summary></summary>
	public class FormatSettings : MKY.Settings.SettingsItem, IEquatable<FormatSettings>
	{
		/// <remarks>Color.Blue = 0000FF.</remarks>
		public static readonly Color TxColorDefault = Color.Blue;

		/// <remarks>Color.Purple = 800080.</remarks>
		public static readonly Color RxColorDefault = Color.Purple;

		/// <remarks>Color.Green = 008000.</remarks>
		public static readonly Color InfoColorDefault = Color.Green;

		/// <remarks>Color.Black = 000000.</remarks>
		public static readonly Color WhiteSpacesColorDefault = Color.Black;

		/// <remarks>Color.Goldenrod = FFD700.</remarks>
		public static readonly Color IOControlColorDefault = Color.Gold;

		/// <remarks>Color.DarkOrange = FF8C00.</remarks>
		public static readonly Color ErrorColorDefault = Color.DarkOrange;

		/// <remarks><see cref="SystemColors.Window"/>.</remarks>
		public static readonly Color BackColorDefault = SystemColors.Window;

		private FontFormat font;

		private bool formattingEnabled;

		private TextFormat txDataFormat;
		private TextFormat txControlFormat;
		private TextFormat rxDataFormat;
		private TextFormat rxControlFormat;
		private TextFormat timeStampFormat;
		private TextFormat timeSpanFormat;
		private TextFormat timeDeltaFormat;
		private TextFormat timeDurationFormat;
		private TextFormat deviceFormat;
		private TextFormat directionFormat;
		private TextFormat lengthFormat;
		private TextFormat ioControlFormat;
		private TextFormat errorFormat;
		private TextFormat whiteSpacesFormat;

		private BackFormat backFormat;

		/// <summary></summary>
		public FormatSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public FormatSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public FormatSettings(FormatSettings rhs)
			: base(rhs)
		{
			FontFormat        = new FontFormat(rhs.FontFormat);

			FormattingEnabled = rhs.FormattingEnabled;

			TxDataFormat       = new TextFormat(rhs.TxDataFormat);
			TxControlFormat    = new TextFormat(rhs.TxControlFormat);
			RxDataFormat       = new TextFormat(rhs.RxDataFormat);
			RxControlFormat    = new TextFormat(rhs.RxControlFormat);
			TimeStampFormat    = new TextFormat(rhs.TimeStampFormat);
			TimeSpanFormat     = new TextFormat(rhs.TimeSpanFormat);
			TimeDeltaFormat    = new TextFormat(rhs.TimeDeltaFormat);
			TimeDurationFormat = new TextFormat(rhs.TimeDurationFormat);
			DeviceFormat       = new TextFormat(rhs.DeviceFormat);
			DirectionFormat    = new TextFormat(rhs.DirectionFormat);
			LengthFormat       = new TextFormat(rhs.LengthFormat);
			IOControlFormat    = new TextFormat(rhs.IOControlFormat);
			ErrorFormat        = new TextFormat(rhs.ErrorFormat);
			WhiteSpacesFormat  = new TextFormat(rhs.WhiteSpacesFormat);

			BackFormat         = new BackFormat(rhs.BackFormat);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FontFormat = new FontFormat(FontFormat.NameDefault, FontFormat.SizeDefault, FontFormat.StyleDefault);

			FormattingEnabled = true;

			TxDataFormat       = new TextFormat(TxColorDefault,           true, false, false, false); // Bold.
			TxControlFormat    = new TextFormat(TxColorDefault,          false, false, false, false);
			RxDataFormat       = new TextFormat(RxColorDefault,           true, false, false, false); // Bold.
			RxControlFormat    = new TextFormat(RxColorDefault,          false, false, false, false);
			TimeStampFormat    = new TextFormat(InfoColorDefault,        false, false, false, false);
			TimeSpanFormat     = new TextFormat(InfoColorDefault,        false, false, false, false);
			TimeDeltaFormat    = new TextFormat(InfoColorDefault,        false, false, false, false);
			TimeDurationFormat = new TextFormat(InfoColorDefault,        false, false, false, false);
			DeviceFormat       = new TextFormat(InfoColorDefault,        false, false, false, false);
			DirectionFormat    = new TextFormat(InfoColorDefault,        false, false, false, false);
			LengthFormat       = new TextFormat(InfoColorDefault,        false, false, false, false);
			IOControlFormat    = new TextFormat(IOControlColorDefault,    true, false, false, false); // Bold.
			ErrorFormat        = new TextFormat(ErrorColorDefault,        true, false, false, false); // Bold.
			WhiteSpacesFormat  = new TextFormat(WhiteSpacesColorDefault, false, false, false, false);

			BackFormat = new BackFormat(BackColorDefault);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Font")]
		public FontFormat FontFormat
		{
			get { return (this.font); }
			set
			{
				if (this.font != value)
				{
					this.font = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public Font Font
		{
			get { return (this.font.Font); }
			set
			{
				if (this.font.Font != value)
				{
					this.font.Font = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FormattingEnabled")]
		public bool FormattingEnabled
		{
			get { return (this.formattingEnabled); }
			set
			{
				if (this.formattingEnabled != value)
				{
					this.formattingEnabled = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxDataFormat")]
		public TextFormat TxDataFormat
		{
			get { return (this.txDataFormat); }
			set
			{
				if (this.txDataFormat != value)
				{
					this.txDataFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxControlFormat")]
		public TextFormat TxControlFormat
		{
			get { return (this.txControlFormat); }
			set
			{
				if (this.txControlFormat != value)
				{
					this.txControlFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxDataFormat")]
		public TextFormat RxDataFormat
		{
			get { return (this.rxDataFormat); }
			set
			{
				if (this.rxDataFormat != value)
				{
					this.rxDataFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxControlFormat")]
		public TextFormat RxControlFormat
		{
			get { return (this.rxControlFormat); }
			set
			{
				if (this.rxControlFormat != value)
				{
					this.rxControlFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeStampFormat")]
		public TextFormat TimeStampFormat
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

		/// <summary></summary>
		[XmlElement("TimeSpanFormat")]
		public TextFormat TimeSpanFormat
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
		[XmlElement("TimeDeltaFormat")]
		public TextFormat TimeDeltaFormat
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

		/// <summary></summary>
		[XmlElement("TimeDurationFormat")]
		public TextFormat TimeDurationFormat
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

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		[XmlElement("DeviceFormat")]
		public TextFormat DeviceFormat
		{
			get { return (this.deviceFormat); }
			set
			{
				if (this.deviceFormat != value)
				{
					this.deviceFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DirectionFormat")]
		public TextFormat DirectionFormat
		{
			get { return (this.directionFormat); }
			set
			{
				if (this.directionFormat != value)
				{
					this.directionFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LengthFormat")]
		public TextFormat LengthFormat
		{
			get { return (this.lengthFormat); }
			set
			{
				if (this.lengthFormat != value)
				{
					this.lengthFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IOControlFormat")]
		public TextFormat IOControlFormat
		{
			get { return (this.ioControlFormat); }
			set
			{
				if (this.ioControlFormat != value)
				{
					this.ioControlFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ErrorFormat")]
		public TextFormat ErrorFormat
		{
			get { return (this.errorFormat); }
			set
			{
				if (this.errorFormat != value)
				{
					this.errorFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// "WhiteSpaces" is a bit misleading, as the format also applies to non-white-spaced
		/// separators. But for a YAT monitor perspective, those are also considered "white-space".
		/// </remarks>
		[XmlElement("WhiteSpacesFormat")]
		public TextFormat WhiteSpacesFormat
		{
			get { return (this.whiteSpacesFormat); }
			set
			{
				if (this.whiteSpacesFormat != value)
				{
					this.whiteSpacesFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BackFormat")]
		public BackFormat BackFormat
		{
			get { return (this.backFormat); }
			set
			{
				if (this.backFormat != value)
				{
					this.backFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public Color BackColor
		{
			get { return (this.backFormat.Color); }
			set
			{
				if (this.backFormat.Color != value)
				{
					this.backFormat.Color = value;
					SetMyChanged();
				}
			}
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

				hashCode = (hashCode * 397) ^ (Font               != null ? Font              .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^                                FormattingEnabled.GetHashCode();

				hashCode = (hashCode * 397) ^ (TxDataFormat       != null ? TxDataFormat      .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TxControlFormat    != null ? TxControlFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxDataFormat       != null ? RxDataFormat      .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxControlFormat    != null ? RxControlFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TimeStampFormat    != null ? TimeStampFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TimeSpanFormat     != null ? TimeSpanFormat    .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TimeDeltaFormat    != null ? TimeDeltaFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TimeDurationFormat != null ? TimeDurationFormat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DeviceFormat       != null ? DeviceFormat      .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DirectionFormat    != null ? DirectionFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LengthFormat       != null ? LengthFormat      .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IOControlFormat    != null ? IOControlFormat   .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ErrorFormat        != null ? ErrorFormat       .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WhiteSpacesFormat  != null ? WhiteSpacesFormat .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ (BackFormat         != null ? BackFormat        .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FormatSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FormatSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ObjectEx.Equals(Font,               other.Font) &&

				FormattingEnabled.Equals(           other.FormattingEnabled) &&

				ObjectEx.Equals(TxDataFormat,       other.TxDataFormat)       &&
				ObjectEx.Equals(TxControlFormat,    other.TxControlFormat)    &&
				ObjectEx.Equals(RxDataFormat,       other.RxDataFormat)       &&
				ObjectEx.Equals(RxControlFormat,    other.RxControlFormat)    &&
				ObjectEx.Equals(TimeStampFormat,    other.TimeStampFormat)    &&
				ObjectEx.Equals(TimeSpanFormat,     other.TimeSpanFormat)     &&
				ObjectEx.Equals(TimeDeltaFormat,    other.TimeDeltaFormat)    &&
				ObjectEx.Equals(TimeDurationFormat, other.TimeDurationFormat) &&
				ObjectEx.Equals(DeviceFormat,       other.DeviceFormat)       &&
				ObjectEx.Equals(DirectionFormat,    other.DirectionFormat)    &&
				ObjectEx.Equals(LengthFormat,       other.LengthFormat)       &&
				ObjectEx.Equals(IOControlFormat,    other.IOControlFormat)    &&
				ObjectEx.Equals(ErrorFormat,        other.ErrorFormat)        &&
				ObjectEx.Equals(WhiteSpacesFormat,  other.WhiteSpacesFormat)  &&

				ObjectEx.Equals(BackFormat,         other.BackFormat)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(FormatSettings lhs, FormatSettings rhs)
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
		public static bool operator !=(FormatSettings lhs, FormatSettings rhs)
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
