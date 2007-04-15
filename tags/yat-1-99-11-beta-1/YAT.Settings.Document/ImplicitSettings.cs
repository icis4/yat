using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Settings.Document
{
	public class ImplicitSettings : Utilities.Settings.Settings
	{
		private bool _terminalIsOpen;
		private bool _logIsOpen;
		private Gui.Settings.SendCommandSettings _sendCommand;
		private Gui.Settings.SendFileSettings _sendFile;
		private Gui.Settings.PredefinedSettings _predefined;
		private Gui.Settings.WindowSettings _window;
		private Gui.Settings.LayoutSettings _layout;

		public ImplicitSettings()
			: base(Utilities.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			SendCommand = new Gui.Settings.SendCommandSettings(SettingsType);
			SendFile = new Gui.Settings.SendFileSettings(SettingsType);
			Predefined = new Gui.Settings.PredefinedSettings(SettingsType);
			Window = new Gui.Settings.WindowSettings(SettingsType);
			Layout = new Gui.Settings.LayoutSettings(SettingsType);

			ClearChanged();
		}

		public ImplicitSettings(ImplicitSettings rhs)
			: base(rhs)
		{
			TerminalIsOpen = rhs.TerminalIsOpen;
			LogIsOpen = rhs.LogIsOpen;

			SendCommand = new Gui.Settings.SendCommandSettings(rhs.SendCommand);
			SendFile = new Gui.Settings.SendFileSettings(rhs.SendFile);
			Predefined = new Gui.Settings.PredefinedSettings(rhs.Predefined);
			Window = new Gui.Settings.WindowSettings(rhs.Window);
			Layout = new Gui.Settings.LayoutSettings(rhs.Layout);

			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			TerminalIsOpen = true;
			LogIsOpen = false;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TerminalIsOpen")]
		public bool TerminalIsOpen
		{
			get { return (_terminalIsOpen); }
			set
			{
				if (_terminalIsOpen != value)
				{
					_terminalIsOpen = value;
					SetChanged();
				}
			}
		}

		[XmlElement("LogIsOpen")]
		public bool LogIsOpen
		{
			get { return (_logIsOpen); }
			set
			{
				if (_logIsOpen != value)
				{
					_logIsOpen = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendCommand")]
		public Gui.Settings.SendCommandSettings SendCommand
		{
			get { return (_sendCommand); }
			set
			{
				if (_sendCommand == null)
				{
					_sendCommand = value;
					AttachNode(_sendCommand);
				}
				else if (_sendCommand != value)
				{
					Gui.Settings.SendCommandSettings old = _sendCommand;
					_sendCommand = value;
					ReplaceNode(old, _sendCommand);
				}
			}
		}

		[XmlElement("SendFile")]
		public Gui.Settings.SendFileSettings SendFile
		{
			get { return (_sendFile); }
			set
			{
				if (_sendFile == null)
				{
					_sendFile = value;
					AttachNode(_sendFile);
				}
				else if (_sendFile != value)
				{
					Gui.Settings.SendFileSettings old = _sendFile;
					_sendFile = value;
					ReplaceNode(old, _sendFile);
				}
			}
		}

		[XmlElement("Predefined")]
		public Gui.Settings.PredefinedSettings Predefined
		{
			get { return (_predefined); }
			set
			{
				if (_predefined == null)
				{
					_predefined = value;
					AttachNode(_predefined);
				}
				else if (_predefined != value)
				{
					Gui.Settings.PredefinedSettings old = _predefined;
					_predefined = value;
					ReplaceNode(old, _predefined);
				}
			}
		}

		[XmlElement("Window")]
		public Gui.Settings.WindowSettings Window
		{
			get { return (_window); }
			set
			{
				if (_window == null)
				{
					_window = value;
					AttachNode(_window);
				}
				else if (_window != value)
				{
					Gui.Settings.WindowSettings old = _window;
					_window = value;
					ReplaceNode(old, _window);
				}
			}
		}

		[XmlElement("Layout")]
		public Gui.Settings.LayoutSettings Layout
		{
			get { return (_layout); }
			set
			{
				if (_layout == null)
				{
					_layout = value;
					AttachNode(_layout);
				}
				else if (_layout != value)
				{
					Gui.Settings.LayoutSettings old = _layout;
					_layout = value;
					ReplaceNode(old, _layout);
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
			if (obj is ImplicitSettings)
				return (Equals((ImplicitSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(ImplicitSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalIsOpen.Equals(value._terminalIsOpen) &&
					_logIsOpen.Equals(value._logIsOpen) &&
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
		public static bool operator ==(ImplicitSettings lhs, ImplicitSettings rhs)
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
		public static bool operator !=(ImplicitSettings lhs, ImplicitSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
