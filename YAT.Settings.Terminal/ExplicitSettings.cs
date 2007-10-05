using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	[Serializable]
	public class ExplicitSettings : MKY.Utilities.Settings.Settings, IEquatable<ExplicitSettings>
	{
		private Domain.Settings.TerminalSettings _terminal;
		private Gui.Settings.PredefinedCommandSettings _predefinedCommand;
		private Gui.Settings.FormatSettings _format;
		private Log.Settings.LogSettings _log;

		public ExplicitSettings()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();

			Terminal = new Domain.Settings.TerminalSettings(SettingsType);
			PredefinedCommand = new Gui.Settings.PredefinedCommandSettings(SettingsType);
			Format = new Gui.Settings.FormatSettings(SettingsType);
			Log = new Log.Settings.LogSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public ExplicitSettings(ExplicitSettings rhs)
			: base(rhs)
		{
			Terminal = new Domain.Settings.TerminalSettings(rhs.Terminal);
			PredefinedCommand = new Gui.Settings.PredefinedCommandSettings(rhs.PredefinedCommand);
			Format = new Gui.Settings.FormatSettings(rhs.Format);
			Log = new Log.Settings.LogSettings(rhs.Log);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			// nothing to do
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("Terminal")]
		public Domain.Settings.TerminalSettings Terminal
		{
			get { return (_terminal); }
			set
			{
				if (_terminal == null)
				{
					_terminal = value;
					AttachNode(_terminal);
				}
				else if (_terminal != value)
				{
					Domain.Settings.TerminalSettings old = _terminal;
					_terminal = value;
					ReplaceNode(old, _terminal);
				}
			}
		}

		[XmlElement("PredefinedCommand")]
		public Gui.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (_predefinedCommand); }
			set
			{
				if (_predefinedCommand == null)
				{
					_predefinedCommand = value;
					AttachNode(_predefinedCommand);
				}
				else if (_predefinedCommand != value)
				{
					Gui.Settings.PredefinedCommandSettings old = _predefinedCommand;
					_predefinedCommand = value;
					ReplaceNode(old, _predefinedCommand);
				}
			}
		}

		[XmlElement("Format")]
		public Gui.Settings.FormatSettings Format
		{
			get { return (_format); }
			set
			{
				if (_format == null)
				{
					_format = value;
					AttachNode(_format);
				}
				else if (_format != value)
				{
					Gui.Settings.FormatSettings old = _format;
					_format = value;
					ReplaceNode(old, _format);
				}
			}
		}

		[XmlElement("Log")]
		public Log.Settings.LogSettings Log
		{
			get { return (_log); }
			set
			{
				if (_log == null)
				{
					_log = value;
					AttachNode(_log);
				}
				else if (_log != value)
				{
					Log.Settings.LogSettings old = _log;
					_log = value;
					ReplaceNode(old, _log);
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
			if (obj is ExplicitSettings)
				return (Equals((ExplicitSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(ExplicitSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
				return (base.Equals((MKY.Utilities.Settings.Settings)value)); // compares all settings nodes

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
		public static bool operator ==(ExplicitSettings lhs, ExplicitSettings rhs)
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
		public static bool operator !=(ExplicitSettings lhs, ExplicitSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
