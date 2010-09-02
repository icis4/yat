//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
	public class FormatSettings : MKY.Utilities.Settings.Settings, IEquatable<FormatSettings>
	{
		private FontFormat font;
		private TextFormat txDataFormat;
		private TextFormat txControlFormat;
		private TextFormat rxDataFormat;
		private TextFormat rxControlFormat;
		private TextFormat timeStampFormat;
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
		public FormatSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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
			FontFormat        = new FontFormat(Types.FontFormat.NameDefault, Types.FontFormat.SizeDefault, Types.FontFormat.StyleDefault);
			TxDataFormat      = new TextFormat(Color.Blue, true, false, false, false);
			TxControlFormat   = new TextFormat(Color.Blue, false, false, false, false);
			RxDataFormat      = new TextFormat(Color.Purple, true, false, false, false);
			RxControlFormat   = new TextFormat(Color.Purple, false, false, false, false);
			TimeStampFormat   = new TextFormat(Color.DarkGreen, false, false, false, false);
			LengthFormat      = new TextFormat(Color.DarkGreen, false, false, false, false);
			WhiteSpacesFormat = new TextFormat(Color.Black, false, false, false, false);
			ErrorFormat       = new TextFormat(Color.OrangeRed, true, false, false, false);
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
				if (value != this.font)
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
				if (value != this.font.Font)
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
				if (value != this.txDataFormat)
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
				if (value != this.txControlFormat)
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
				if (value != this.rxDataFormat)
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
				if (value != this.rxControlFormat)
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
				if (value != this.timeStampFormat)
				{
					this.timeStampFormat = value;
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
				if (value != this.lengthFormat)
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
				if (value != this.whiteSpacesFormat)
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
				if (value != this.errorFormat)
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
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			FormatSettings casted = obj as FormatSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FormatSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

				(this.font              == other.font) &&
				(this.txDataFormat      == other.txDataFormat) &&
				(this.txControlFormat   == other.txControlFormat) &&
				(this.rxDataFormat      == other.rxDataFormat) &&
				(this.rxControlFormat   == other.rxControlFormat) &&
				(this.timeStampFormat   == other.timeStampFormat) &&
				(this.lengthFormat      == other.lengthFormat) &&
				(this.whiteSpacesFormat == other.whiteSpacesFormat) &&
				(this.errorFormat       == other.errorFormat)
			);
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
		public static bool operator ==(FormatSettings lhs, FormatSettings rhs)
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
