//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Drawing;
using System.Xml.Serialization;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class FormatSettings : MKY.Settings.SettingsItem
	{
		/// <remarks>Color.Blue = 0000FF.</remarks>
		public readonly Color DefaultTxColor = Color.Blue;

		/// <remarks>Color.Purple = 800080.</remarks>
		public readonly Color DefaultRxColor = Color.Purple;

		/// <remarks>Color.DarkGreen = 006400.</remarks>
		public readonly Color DefaultInfoColor = Color.DarkGreen;

		/// <remarks>Color.Black = 000000.</remarks>
		public readonly Color DefaultWhiteSpacesColor = Color.Black;

		/// <remarks>Color.OrangeRed = FF4500.</remarks>
		public readonly Color DefaultErrorColor = Color.OrangeRed;

		private FontFormat font;
		private TextFormat txDataFormat;
		private TextFormat txControlFormat;
		private TextFormat rxDataFormat;
		private TextFormat rxControlFormat;
		private TextFormat timeStampFormat;
		private TextFormat directionFormat;
		private TextFormat lengthFormat;
		private TextFormat whiteSpacesFormat;
		private TextFormat errorFormat;

		/// <summary></summary>
		public FormatSettings()
		{
			SetMyDefaults();
			ClearChanged();
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
			TimeStampFormat   = new TextFormat(rhs.TimeStampFormat);
			DirectionFormat   = new TextFormat(rhs.DirectionFormat);
			LengthFormat      = new TextFormat(rhs.LengthFormat);
			WhiteSpacesFormat = new TextFormat(rhs.WhiteSpacesFormat);
			ErrorFormat       = new TextFormat(rhs.ErrorFormat);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FontFormat        = new FontFormat(FontFormat.NameDefault, FontFormat.SizeDefault, FontFormat.StyleDefault);
			TxDataFormat      = new TextFormat(DefaultTxColor,           true, false, false, false); // Bold.
			TxControlFormat   = new TextFormat(DefaultTxColor,          false, false, false, false);
			RxDataFormat      = new TextFormat(DefaultRxColor,           true, false, false, false); // Bold.
			RxControlFormat   = new TextFormat(DefaultRxColor,          false, false, false, false);
			TimeStampFormat   = new TextFormat(DefaultInfoColor,        false, false, false, false);
			DirectionFormat   = new TextFormat(DefaultInfoColor,        false, false, false, false);
			LengthFormat      = new TextFormat(DefaultInfoColor,        false, false, false, false);
			WhiteSpacesFormat = new TextFormat(DefaultWhiteSpacesColor, false, false, false, false);
			ErrorFormat       = new TextFormat(DefaultErrorColor,        true, false, false, false); // Bold.
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
				(TimeStampFormat   == other.TimeStampFormat) &&
				(DirectionFormat   == other.DirectionFormat) &&
				(LengthFormat      == other.LengthFormat) &&
				(WhiteSpacesFormat == other.WhiteSpacesFormat) &&
				(ErrorFormat       == other.ErrorFormat)
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
			return
			(
				base.GetHashCode() ^

				Font             .GetHashCode() ^
				TxDataFormat     .GetHashCode() ^
				TxControlFormat  .GetHashCode() ^
				RxDataFormat     .GetHashCode() ^
				RxControlFormat  .GetHashCode() ^
				TimeStampFormat  .GetHashCode() ^
				DirectionFormat  .GetHashCode() ^
				LengthFormat     .GetHashCode() ^
				WhiteSpacesFormat.GetHashCode() ^
				ErrorFormat      .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
