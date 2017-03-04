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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Xml.Serialization;

using MKY;

using YAT.Model.Types;

namespace YAT.Model.Settings
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

		/// <remarks>'Orange' predefined by the color dialog = FF8000.</remarks>
		public static readonly Color ErrorColorDefault = Color.FromArgb(0xFF, 0x80, 0x00);

		/// <remarks><see cref="SystemColors.Window"/>.</remarks>
		public static readonly Color BackColorDefault = SystemColors.Window;

		private FontFormat font;

		private bool formattingEnabled;

		private TextFormat txDataFormat;
		private TextFormat txControlFormat;
		private TextFormat rxDataFormat;
		private TextFormat rxControlFormat;
		private TextFormat dateFormat;
		private TextFormat timeFormat;
		private TextFormat portFormat;
		private TextFormat directionFormat;
		private TextFormat lengthFormat;
		private TextFormat whiteSpacesFormat;
		private TextFormat errorFormat;

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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public FormatSettings(FormatSettings rhs)
			: base(rhs)
		{
			FontFormat        = new FontFormat(rhs.FontFormat);

			FormattingEnabled = rhs.FormattingEnabled;

			TxDataFormat      = new TextFormat(rhs.TxDataFormat);
			TxControlFormat   = new TextFormat(rhs.TxControlFormat);
			RxDataFormat      = new TextFormat(rhs.RxDataFormat);
			RxControlFormat   = new TextFormat(rhs.RxControlFormat);
			DateFormat        = new TextFormat(rhs.DateFormat);
			TimeFormat        = new TextFormat(rhs.TimeFormat);
			PortFormat        = new TextFormat(rhs.PortFormat);
			DirectionFormat   = new TextFormat(rhs.DirectionFormat);
			LengthFormat      = new TextFormat(rhs.LengthFormat);
			WhiteSpacesFormat = new TextFormat(rhs.WhiteSpacesFormat);
			ErrorFormat       = new TextFormat(rhs.ErrorFormat);

			BackFormat        = new BackFormat(rhs.BackFormat);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FontFormat        = new FontFormat(FontFormat.NameDefault, FontFormat.SizeDefault, FontFormat.StyleDefault);

			FormattingEnabled = true;

			TxDataFormat      = new TextFormat(TxColorDefault,           true, false, false, false); // Bold.
			TxControlFormat   = new TextFormat(TxColorDefault,          false, false, false, false);
			RxDataFormat      = new TextFormat(RxColorDefault,           true, false, false, false); // Bold.
			RxControlFormat   = new TextFormat(RxColorDefault,          false, false, false, false);
			DateFormat        = new TextFormat(InfoColorDefault,        false, false, false, false);
			TimeFormat        = new TextFormat(InfoColorDefault,        false, false, false, false);
			PortFormat        = new TextFormat(InfoColorDefault,        false, false, false, false);
			DirectionFormat   = new TextFormat(InfoColorDefault,        false, false, false, false);
			LengthFormat      = new TextFormat(InfoColorDefault,        false, false, false, false);
			WhiteSpacesFormat = new TextFormat(WhiteSpacesColorDefault, false, false, false, false);
			ErrorFormat       = new TextFormat(ErrorColorDefault,        true, false, false, false); // Bold.

			BackFormat        = new BackFormat(BackColorDefault);
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
		[XmlElement("DateFormat")]
		public TextFormat DateFormat
		{
			get { return (this.dateFormat); }
			set
			{
				if (this.dateFormat != value)
				{
					this.dateFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeFormat")]
		public TextFormat TimeFormat
		{
			get { return (this.timeFormat); }
			set
			{
				if (this.timeFormat != value)
				{
					this.timeFormat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("PortFormat")]
		public TextFormat PortFormat
		{
			get { return (this.portFormat); }
			set
			{
				if (this.portFormat != value)
				{
					this.portFormat = value;
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

				hashCode = (hashCode * 397) ^ (Font              != null ? Font             .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^                              FormattingEnabled.GetHashCode();

				hashCode = (hashCode * 397) ^ (TxDataFormat      != null ? TxDataFormat     .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TxControlFormat   != null ? TxControlFormat  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxDataFormat      != null ? RxDataFormat     .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxControlFormat   != null ? RxControlFormat  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DateFormat        != null ? DateFormat       .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TimeFormat        != null ? TimeFormat       .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (PortFormat        != null ? PortFormat       .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DirectionFormat   != null ? DirectionFormat  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LengthFormat      != null ? LengthFormat     .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WhiteSpacesFormat != null ? WhiteSpacesFormat.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ErrorFormat       != null ? ErrorFormat      .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ (BackFormat        != null ? BackFormat       .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FormatSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
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

				ObjectEx.Equals(Font,              other.Font) &&

				FormattingEnabled.Equals(          other.FormattingEnabled) &&

				ObjectEx.Equals(TxDataFormat,      other.TxDataFormat)      &&
				ObjectEx.Equals(TxControlFormat,   other.TxControlFormat)   &&
				ObjectEx.Equals(RxDataFormat,      other.RxDataFormat)      &&
				ObjectEx.Equals(RxControlFormat,   other.RxControlFormat)   &&
				ObjectEx.Equals(DateFormat,        other.DateFormat)        &&
				ObjectEx.Equals(TimeFormat,        other.TimeFormat)        &&
				ObjectEx.Equals(PortFormat,        other.PortFormat)        &&
				ObjectEx.Equals(DirectionFormat,   other.DirectionFormat)   &&
				ObjectEx.Equals(LengthFormat,      other.LengthFormat)      &&
				ObjectEx.Equals(WhiteSpacesFormat, other.WhiteSpacesFormat) &&
				ObjectEx.Equals(ErrorFormat,       other.ErrorFormat)       &&

				ObjectEx.Equals(BackFormat,        other.BackFormat)
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
