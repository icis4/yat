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
// YAT 2.0 Gamma 2 Version 1.99.50
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
using System.Drawing;
using System.Xml.Serialization;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class FormatSettings : MKY.Settings.SettingsItem
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

			FormatSettings other = (FormatSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(Font              == other.Font) &&

				(TxDataFormat      == other.TxDataFormat) &&
				(TxControlFormat   == other.TxControlFormat) &&
				(RxDataFormat      == other.RxDataFormat) &&
				(RxControlFormat   == other.RxControlFormat) &&
				(DateFormat        == other.DateFormat) &&
				(TimeFormat        == other.TimeFormat) &&
				(PortFormat        == other.PortFormat) &&
				(DirectionFormat   == other.DirectionFormat) &&
				(LengthFormat      == other.LengthFormat) &&
				(WhiteSpacesFormat == other.WhiteSpacesFormat) &&
				(ErrorFormat       == other.ErrorFormat) &&

				(BackFormat        == other.BackFormat)
			);
		}

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

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityAnalysis for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
