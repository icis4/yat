//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
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

using MKY.Utilities.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class TextTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<TextTerminalSettings>
	{
		/// <summary></summary>
		public static readonly string DefaultEol = (string)XEol.Parse(Environment.NewLine);
		/// <summary></summary>
		public static readonly int DefaultEncoding = (XEncoding)(System.Text.Encoding.Default);

		private bool              _separateTxRxEol;
		private string            _txEol;
		private string            _rxEol;
		private int               _encoding;
		private bool              _showEol;
		private TextLineSendDelay _lineSendDelay;
		private WaitForResponse   _waitForResponse;
		private CharSubstitution  _charSubstitution;

		/// <summary></summary>
		public TextTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public TextTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			_separateTxRxEol     = rhs.SeparateTxRxEol;
			_txEol               = rhs.TxEol;
			_rxEol               = rhs.RxEol;
			_encoding            = rhs.Encoding;
			_showEol             = rhs.ShowEol;
			_lineSendDelay       = rhs.LineSendDelay;
			_waitForResponse     = rhs.WaitForResponse;
			_charSubstitution    = rhs.CharSubstitution;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SeparateTxRxEol  = false;
			TxEol            = DefaultEol;
			RxEol            = DefaultEol;
			Encoding         = DefaultEncoding;
			ShowEol          = false;
			LineSendDelay    = new TextLineSendDelay(false, 500, 1);
			WaitForResponse  = new WaitForResponse(false, 500);
			CharSubstitution = CharSubstitution.None;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxEol")]
		public bool SeparateTxRxEol
		{
			get { return (_separateTxRxEol); }
			set
			{
				if (_separateTxRxEol != value)
				{
					_separateTxRxEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxEol")]
		public string TxEol
		{
			get { return (_txEol); }
			set
			{
				if (_txEol != value)
				{
					_txEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxEol")]
		public string RxEol
		{
			get
			{
				if (_separateTxRxEol)
					return (_rxEol);
				else
					return (_txEol);
			}
			set
			{
				if (_rxEol != value)
				{
					_rxEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Encoding")]
		public int Encoding
		{
			get { return (_encoding); }
			set
			{
				if (_encoding != value)
				{
					_encoding = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowEol")]
		public bool ShowEol
		{
			get { return (_showEol); }
			set
			{
				if (_showEol != value)
				{
					_showEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LineSendDelay")]
		public TextLineSendDelay LineSendDelay
		{
			get { return (_lineSendDelay); }
			set
			{
				if (_lineSendDelay != value)
				{
					_lineSendDelay = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("WaitForResponse")]
		public WaitForResponse WaitForResponse
		{
			get { return (_waitForResponse); }
			set
			{
				if (_waitForResponse != value)
				{
					_waitForResponse = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharSubstitution")]
		public CharSubstitution CharSubstitution
		{
			get { return (_charSubstitution); }
			set
			{
				if (_charSubstitution != value)
				{
					_charSubstitution = value;
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
			if (obj is TextTerminalSettings)
				return (Equals((TextTerminalSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TextTerminalSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_separateTxRxEol.Equals (value._separateTxRxEol) &&
					_txEol.Equals           (value._txEol) &&
					_rxEol.Equals           (value._rxEol) &&
					_encoding.Equals        (value._encoding) &&
					_showEol.Equals         (value._showEol) &&
					_lineSendDelay.Equals   (value._lineSendDelay) &&
					_waitForResponse.Equals (value._waitForResponse) &&
					_charSubstitution.Equals(value._charSubstitution)
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
		public static bool operator ==(TextTerminalSettings lhs, TextTerminalSettings rhs)
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
		public static bool operator !=(TextTerminalSettings lhs, TextTerminalSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
