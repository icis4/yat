//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettings>
	{
		private TerminalType _terminalType;

		// type independent settings
		private IOSettings _io;
		private BufferSettings _buffer;
		private DisplaySettings _display;
		private CharReplaceSettings _charReplace;
		private SendSettings _send;

		// type dependent settings
		private TextTerminalSettings _textTerminal;
		private BinaryTerminalSettings _binaryTerminal;

		/// <summary></summary>
		public TerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public TerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			IO = new IOSettings(SettingsType);
			Buffer = new BufferSettings(SettingsType);
			Display = new DisplaySettings(SettingsType);
			CharReplace = new CharReplaceSettings(SettingsType);
			Send = new SendSettings(SettingsType);

			TextTerminal = new TextTerminalSettings(SettingsType);
			BinaryTerminal = new BinaryTerminalSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TerminalSettings(TerminalSettings rhs)
			: base(rhs)
		{
			_terminalType = rhs.TerminalType;

			IO = new IOSettings(rhs.IO);
			Buffer = new BufferSettings(rhs.Buffer);
			Display = new DisplaySettings(rhs.Display);
			CharReplace = new CharReplaceSettings(rhs.CharReplace);
			Send = new SendSettings(rhs.Send);

			TextTerminal = new TextTerminalSettings(rhs.TextTerminal);
			BinaryTerminal = new BinaryTerminalSettings(rhs.BinaryTerminal);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalType = TerminalType.Text;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TerminalType")]
		public TerminalType TerminalType
		{
			get { return (_terminalType); }
			set
			{
				if (_terminalType != value)
				{
					_terminalType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IO")]
		public IOSettings IO
		{
			get { return (_io); }
			set
			{
				if (_io == null)
				{
					_io = value;
					AttachNode(_io);
				}
				else if (_io != value)
				{
					IOSettings old = _io;
					_io = value;
					ReplaceNode(old, _io);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Buffer")]
		public BufferSettings Buffer
		{
			get { return (_buffer); }
			set
			{
				if (_buffer == null)
				{
					_buffer = value;
					AttachNode(_buffer);
				}
				else if (_buffer != value)
				{
					BufferSettings old = _buffer;
					_buffer = value;
					ReplaceNode(old, _buffer);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Display")]
		public DisplaySettings Display
		{
			get { return (_display); }
			set
			{
				if (_display == null)
				{
					_display = value;
					AttachNode(_display);
				}
				else if (_display != value)
				{
					DisplaySettings old = _display;
					_display = value;
					ReplaceNode(old, _display);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharReplace")]
		public CharReplaceSettings CharReplace
		{
			get { return (_charReplace); }
			set
			{
				if (_charReplace == null)
				{
					_charReplace = value;
					AttachNode(_charReplace);
				}
				else if (_charReplace != value)
				{
					CharReplaceSettings old = _charReplace;
					_charReplace = value;
					ReplaceNode(old, _charReplace);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Send")]
		public SendSettings Send
		{
			get { return (_send); }
			set
			{
				if (_send == null)
				{
					_send = value;
					AttachNode(_send);
				}
				else if (_send != value)
				{
					SendSettings old = _send;
					_send = value;
					ReplaceNode(old, _send);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TextTerminal")]
		public TextTerminalSettings TextTerminal
		{
			get { return (_textTerminal); }
			set
			{
				if (_textTerminal == null)
				{
					_textTerminal = value;
					AttachNode(_textTerminal);
				}
				else if (_textTerminal != value)
				{
					TextTerminalSettings old = _textTerminal;
					_textTerminal = value;
					ReplaceNode(old, _textTerminal);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BinaryTerminal")]
		public BinaryTerminalSettings BinaryTerminal
		{
			get { return (_binaryTerminal); }
			set
			{
				if (_binaryTerminal == null)
				{
					_binaryTerminal = value;
					AttachNode(_binaryTerminal);
				}
				else if (_binaryTerminal != value)
				{
					BinaryTerminalSettings old = _binaryTerminal;
					_binaryTerminal = value;
					ReplaceNode(old, _binaryTerminal);
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
			if (obj is TerminalSettings)
				return (Equals((TerminalSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TerminalSettings value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalType.Equals(value._terminalType) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // compares all settings nodes
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
		public static bool operator ==(TerminalSettings lhs, TerminalSettings rhs)
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
		public static bool operator !=(TerminalSettings lhs, TerminalSettings rhs)
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
