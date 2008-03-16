using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	[Serializable]
	public class ImplicitSettings : MKY.Utilities.Settings.Settings, IEquatable<ImplicitSettings>
	{
		private bool _terminalIsOpen;
		private bool _logIsOpen;
		private Model.Settings.SendCommandSettings _sendCommand;
		private Model.Settings.SendFileSettings _sendFile;
		private Model.Settings.PredefinedSettings _predefined;
		private Model.Settings.WindowSettings _window;
		private Model.Settings.LayoutSettings _layout;

		public ImplicitSettings()
			: base(MKY.Utilities.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			SendCommand = new Model.Settings.SendCommandSettings(SettingsType);
			SendFile    = new Model.Settings.SendFileSettings(SettingsType);
			Predefined  = new Model.Settings.PredefinedSettings(SettingsType);
			Window      = new Model.Settings.WindowSettings(SettingsType);
			Layout      = new Model.Settings.LayoutSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public ImplicitSettings(ImplicitSettings rhs)
			: base(rhs)
		{
			_terminalIsOpen = rhs.TerminalIsOpen;
			_logIsOpen = rhs.LogIsOpen;

			SendCommand = new Model.Settings.SendCommandSettings(rhs.SendCommand);
			SendFile    = new Model.Settings.SendFileSettings(rhs.SendFile);
			Predefined  = new Model.Settings.PredefinedSettings(rhs.Predefined);
			Window      = new Model.Settings.WindowSettings(rhs.Window);
			Layout      = new Model.Settings.LayoutSettings(rhs.Layout);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TerminalIsOpen = true;
			LogIsOpen = false;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
		public Model.Settings.SendCommandSettings SendCommand
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
					Model.Settings.SendCommandSettings old = _sendCommand;
					_sendCommand = value;
					ReplaceNode(old, _sendCommand);
				}
			}
		}

		[XmlElement("SendFile")]
		public Model.Settings.SendFileSettings SendFile
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
					Model.Settings.SendFileSettings old = _sendFile;
					_sendFile = value;
					ReplaceNode(old, _sendFile);
				}
			}
		}

		[XmlElement("Predefined")]
		public Model.Settings.PredefinedSettings Predefined
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
					Model.Settings.PredefinedSettings old = _predefined;
					_predefined = value;
					ReplaceNode(old, _predefined);
				}
			}
		}

		[XmlElement("Window")]
		public Model.Settings.WindowSettings Window
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
					Model.Settings.WindowSettings old = _window;
					_window = value;
					ReplaceNode(old, _window);
				}
			}
		}

		[XmlElement("Layout")]
		public Model.Settings.LayoutSettings Layout
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
					Model.Settings.LayoutSettings old = _layout;
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
					base.Equals((MKY.Utilities.Settings.Settings)value) // compares all settings nodes
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
		/// Determines whether the two specified objects have reference or value equality.
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
