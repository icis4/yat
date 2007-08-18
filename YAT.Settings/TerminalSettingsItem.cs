using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings
{
	[Serializable]
	public class TerminalSettingsItem : Utilities.Settings.Settings, IEquatable<TerminalSettingsItem>
	{
		private Guid _guid;
		private string _filePath;
		private Gui.Settings.WindowSettings _window;

		public TerminalSettingsItem()
			: base(Utilities.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			Window = new Gui.Settings.WindowSettings(SettingsType);

			ClearChanged();
		}

		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			Window = new Gui.Settings.WindowSettings(rhs.Window);

			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			_guid = Guid.Empty;
			_filePath = "";
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlIgnore]
		public Guid Guid
		{
			get { return (_guid); }
			set
			{
				if (_guid != value)
				{
					_guid = value;
					SetChanged();
				}
			}
		}

		[XmlElement("FilePath")]
		public string FilePath
		{
			get { return (_filePath); }
			set
			{
				if (_filePath != value)
				{
					_filePath = value;
					SetChanged();
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

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TerminalSettingsItem)
				return (Equals((TerminalSettingsItem)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TerminalSettingsItem value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
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
		public static bool operator ==(TerminalSettingsItem lhs, TerminalSettingsItem rhs)
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
		public static bool operator !=(TerminalSettingsItem lhs, TerminalSettingsItem rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
