using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class TerminalSettings : Utilities.Settings.Settings
	{
		private TerminalType _terminalType;

		// type independent settings
		private IOSettings _io;
		private BufferSettings _buffer;
		private DisplaySettings _display;
		private TransmitSettings _transmit;

		// type dependet settings
		private TextTerminalSettings _textTerminal;
		private BinaryTerminalSettings _binaryTerminal;

		public TerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public TerminalSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public TerminalSettings(TerminalSettings rhs)
			: base(rhs)
		{
			TerminalType = rhs.TerminalType;

			IO = new IOSettings(rhs.IO);
			Buffer = new BufferSettings(rhs.Buffer);
			Display = new DisplaySettings(rhs.Display);
			Transmit = new TransmitSettings(rhs.Transmit);

			TextTerminal = new TextTerminalSettings(rhs.TextTerminal);
			BinaryTerminal = new BinaryTerminalSettings(rhs.BinaryTerminal);

			ClearChanged();
		}

		private void InitializeNodes()
		{
			IO = new IOSettings(SettingsType);
			Buffer = new BufferSettings(SettingsType);
			Display = new DisplaySettings(SettingsType);
			Transmit = new TransmitSettings(SettingsType);

			TextTerminal = new TextTerminalSettings(SettingsType);
			BinaryTerminal = new BinaryTerminalSettings(SettingsType);
		}

		protected override void SetMyDefaults()
		{
			TerminalType = TerminalType.Text;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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

		[XmlElement("Transmit")]
		public TransmitSettings Transmit
		{
			get { return (_transmit); }
			set
			{
				if (_transmit == null)
				{
					_transmit = value;
					AttachNode(_transmit);
				}
				else if (_transmit != value)
				{
					TransmitSettings old = _transmit;
					_transmit = value;
					ReplaceNode(old, _transmit);
				}
			}
		}

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
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalType.Equals(value._terminalType) &&
					base.Equals((Utilities.Settings.Settings)value) // compares all settings nodes
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