using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace YAT.Settings
{
	[Serializable]
	public class TerminalSettingsItem : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettingsItem>
	{
		private string _filePath;
		private Guid _guid;
		private Gui.Settings.WindowSettings _window;

		public TerminalSettingsItem()
			: base(MKY.Utilities.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			Window = new Gui.Settings.WindowSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			_filePath = rhs.FilePath;
			_guid = rhs.Guid;

			Window = new Gui.Settings.WindowSettings(rhs.Window);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			FilePath = "";
			Guid = Guid.Empty;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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
				
				// set GUID from file path if needed
				if ((_guid == Guid.Empty) && File.Exists(_filePath))
				{
					string fileName = Path.GetFileNameWithoutExtension(_filePath);
					string guidString = fileName.Substring(GeneralSettings.AutoSaveTerminalFileNamePrefix.Length);
					try
					{
						_guid = new Guid(guidString);
					}
					catch (Exception)
					{
						_guid = Guid.Empty;
					}
				}
			}
		}

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
					_filePath.Equals(value._filePath) &&
					_guid.Equals(value._guid) &&
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
