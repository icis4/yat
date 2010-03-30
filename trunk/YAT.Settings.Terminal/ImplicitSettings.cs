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

namespace YAT.Settings.Terminal
{
	[Serializable]
	public class ImplicitSettings : MKY.Utilities.Settings.Settings, IEquatable<ImplicitSettings>
	{
		private bool _terminalIsStarted;
		private bool _logIsStarted;

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
			_terminalIsStarted = rhs.TerminalIsStarted;
			_logIsStarted      = rhs.LogIsStarted;

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
			TerminalIsStarted = true;
			LogIsStarted = false;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[XmlElement("TerminalIsStarted")]
		public virtual bool TerminalIsStarted
		{
			get { return (_terminalIsStarted); }
			set
			{
				if (value != _terminalIsStarted)
				{
					_terminalIsStarted = value;
					SetChanged();
				}
			}
		}

		[XmlElement("LogIsStarted")]
		public virtual bool LogIsStarted
		{
			get { return (_logIsStarted); }
			set
			{
				if (value != _logIsStarted)
				{
					_logIsStarted = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendCommand")]
		public virtual Model.Settings.SendCommandSettings SendCommand
		{
			get { return (_sendCommand); }
			set
			{
				if (_sendCommand == null)
				{
					_sendCommand = value;
					AttachNode(_sendCommand);
				}
				else if (value != _sendCommand)
				{
					Model.Settings.SendCommandSettings old = _sendCommand;
					_sendCommand = value;
					ReplaceNode(old, _sendCommand);
				}
			}
		}

		[XmlElement("SendFile")]
		public virtual Model.Settings.SendFileSettings SendFile
		{
			get { return (_sendFile); }
			set
			{
				if (_sendFile == null)
				{
					_sendFile = value;
					AttachNode(_sendFile);
				}
				else if (value != _sendFile)
				{
					Model.Settings.SendFileSettings old = _sendFile;
					_sendFile = value;
					ReplaceNode(old, _sendFile);
				}
			}
		}

		[XmlElement("Predefined")]
		public virtual Model.Settings.PredefinedSettings Predefined
		{
			get { return (_predefined); }
			set
			{
				if (_predefined == null)
				{
					_predefined = value;
					AttachNode(_predefined);
				}
				else if (value != _predefined)
				{
					Model.Settings.PredefinedSettings old = _predefined;
					_predefined = value;
					ReplaceNode(old, _predefined);
				}
			}
		}

		[XmlElement("Window")]
		public virtual Model.Settings.WindowSettings Window
		{
			get { return (_window); }
			set
			{
				if (_window == null)
				{
					_window = value;
					AttachNode(_window);
				}
				else if (value != _window)
				{
					Model.Settings.WindowSettings old = _window;
					_window = value;
					ReplaceNode(old, _window);
				}
			}
		}

		[XmlElement("Layout")]
		public virtual Model.Settings.LayoutSettings Layout
		{
			get { return (_layout); }
			set
			{
				if (_layout == null)
				{
					_layout = value;
					AttachNode(_layout);
				}
				else if (value != _layout)
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
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_terminalIsStarted.Equals(value._terminalIsStarted) &&
					_logIsStarted.Equals(value._logIsStarted) &&
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
