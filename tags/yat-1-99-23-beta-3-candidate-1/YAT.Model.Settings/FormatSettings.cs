//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class FormatSettings : MKY.Utilities.Settings.Settings, IEquatable<FormatSettings>
	{
		private FontFormat _font;
		private TextFormat _txDataFormat;
		private TextFormat _txControlFormat;
		private TextFormat _rxDataFormat;
		private TextFormat _rxControlFormat;
		private TextFormat _timeStampFormat;
		private TextFormat _lengthFormat;
		private TextFormat _whiteSpacesFormat;
		private TextFormat _errorFormat;

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
			get { return (_font); }
			set
			{
				if (_font != value)
				{
					_font = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public Font Font
		{
			get { return (_font.Font); }
			set
			{
				if (_font.Font != value)
				{
					_font.Font = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxDataFormat")]
		public TextFormat TxDataFormat
		{
			get { return (_txDataFormat); }
			set
			{
				if (_txDataFormat != value)
				{
					_txDataFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxControlFormat")]
		public TextFormat TxControlFormat
		{
			get { return (_txControlFormat); }
			set
			{
				if (_txControlFormat != value)
				{
					_txControlFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxDataFormat")]
		public TextFormat RxDataFormat
		{
			get { return (_rxDataFormat); }
			set
			{
				if (_rxDataFormat != value)
				{
					_rxDataFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxControlFormat")]
		public TextFormat RxControlFormat
		{
			get { return (_rxControlFormat); }
			set
			{
				if (_rxControlFormat != value)
				{
					_rxControlFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimeStampFormat")]
		public TextFormat TimeStampFormat
		{
			get { return (_timeStampFormat); }
			set
			{
				if (_timeStampFormat != value)
				{
					_timeStampFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LengthFormat")]
		public TextFormat LengthFormat
		{
			get { return (_lengthFormat); }
			set
			{
				if (_lengthFormat != value)
				{
					_lengthFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("WhiteSpacesFormat")]
		public TextFormat WhiteSpacesFormat
		{
			get { return (_whiteSpacesFormat); }
			set
			{
				if (_whiteSpacesFormat != value)
				{
					_whiteSpacesFormat = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ErrorFormat")]
		public TextFormat ErrorFormat
		{
			get { return (_errorFormat); }
			set
			{
				if (_errorFormat != value)
				{
					_errorFormat = value;
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
			if (obj is FormatSettings)
				return (Equals((FormatSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FormatSettings value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_font.Equals             (value._font) &&
					_txDataFormat.Equals     (value._txDataFormat) &&
					_txControlFormat.Equals  (value._txControlFormat) &&
					_rxDataFormat.Equals     (value._rxDataFormat) &&
					_rxControlFormat.Equals  (value._rxControlFormat) &&
					_timeStampFormat.Equals  (value._timeStampFormat) &&
					_lengthFormat.Equals     (value._lengthFormat) &&
					_whiteSpacesFormat.Equals(value._whiteSpacesFormat) &&
					_errorFormat.Equals      (value._errorFormat)
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
