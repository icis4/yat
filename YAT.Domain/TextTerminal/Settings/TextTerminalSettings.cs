using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using HSR.Utilities.Text;

namespace HSR.YAT.Domain.Settings
{
	public class TextTerminalSettings : Utilities.Settings.Settings
	{
		public static readonly string DefaultEol = (string)XEol.FromString(Environment.NewLine);
		public static readonly int DefaultEncoding = (XEncoding)(System.Text.Encoding.Default);

		private bool              _separateTxRxEol;
		private string            _txEol;
		private string            _rxEol;
		private int               _encoding;
		private bool              _directionLineBreakEnabled;
		private bool              _showEol;
		private bool              _replaceControlChars;
		private ControlCharRadix  _controlCharRadix;
		private TextLineSendDelay _lineSendDelay;
		private WaitForResponse   _waitForResponse;
		private CharSubstitution  _charSubstitution;

		public TextTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public TextTerminalSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			SeparateTxRxEol     = rhs.SeparateTxRxEol;
			TxEol               = rhs.TxEol;
			RxEol               = rhs.RxEol;
			Encoding            = rhs.Encoding;
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;
			ShowEol             = rhs.ShowEol;
			ReplaceControlChars = rhs.ReplaceControlChars;
			ControlCharRadix    = rhs.ControlCharRadix;
			LineSendDelay       = rhs.LineSendDelay;
			WaitForResponse     = rhs.WaitForResponse;
			CharSubstitution    = rhs.CharSubstitution;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			SeparateTxRxEol     = false;
			TxEol               = DefaultEol;
			RxEol               = DefaultEol;
			Encoding            = DefaultEncoding;
			DirectionLineBreakEnabled = true;
			ShowEol             = false;
			ReplaceControlChars = true;
			ControlCharRadix    = ControlCharRadix.AsciiMnemonic;
			LineSendDelay       = new TextLineSendDelay(false, 500, 1);
			WaitForResponse     = new WaitForResponse(false, 500);
			CharSubstitution    = CharSubstitution.None;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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

		[XmlElement("DirectionLineBreakEnabled")]
		public bool DirectionLineBreakEnabled
		{
			get { return (_directionLineBreakEnabled); }
			set
			{
				if (_directionLineBreakEnabled != value)
				{
					_directionLineBreakEnabled = value;
					SetChanged();
				}
			}
		}

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

		[XmlElement("ReplaceControlChars")]
		public bool ReplaceControlChars
		{
			get { return (_replaceControlChars); }
			set
			{
				if (_replaceControlChars != value)
				{
					_replaceControlChars = value;
					SetChanged();
				}
			}
		}

		[XmlElement("ControlCharRadix")]
		public ControlCharRadix ControlCharRadix
		{
			get { return (_controlCharRadix); }
			set
			{
				if (_controlCharRadix != value)
				{
					_controlCharRadix = value;
					SetChanged();
				}
			}
		}

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
					_separateTxRxEol.Equals    (value._separateTxRxEol) &&
					_txEol.Equals              (value._txEol) &&
					_rxEol.Equals              (value._rxEol) &&
					_encoding.Equals           (value._encoding) &&
					_directionLineBreakEnabled.Equals(value._directionLineBreakEnabled) &&
					_showEol.Equals            (value._showEol) &&
					_replaceControlChars.Equals(value._replaceControlChars) &&
					_controlCharRadix.Equals   (value._controlCharRadix) &&
					_lineSendDelay.Equals      (value._lineSendDelay) &&
					_waitForResponse.Equals    (value._waitForResponse) &&
					_charSubstitution.Equals   (value._charSubstitution)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
