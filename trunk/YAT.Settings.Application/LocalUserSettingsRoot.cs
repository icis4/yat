using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings.Application
{
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot : Utilities.Settings.Settings, IEquatable<LocalUserSettingsRoot>
	{
		private string _productVersion = System.Windows.Forms.Application.ProductVersion;
		private Settings.GeneralSettings _general;
		private Settings.PathSettings _paths;
		private Gui.Settings.MainWindowSettings _mainWindow;
		private Gui.Settings.NewTerminalSettings _newTerminal;
		private Gui.Settings.RecentFileSettings _recentFiles;

		public LocalUserSettingsRoot()
			: base(Utilities.Settings.SettingsType.Explicit)
		{
			General     = new Settings.GeneralSettings();
			Paths       = new Settings.PathSettings();
			MainWindow  = new Gui.Settings.MainWindowSettings();
			NewTerminal = new Gui.Settings.NewTerminalSettings();
			RecentFiles = new Gui.Settings.RecentFileSettings();

			ClearChanged();
		}

		public LocalUserSettingsRoot(LocalUserSettingsRoot rhs)
			: base(rhs)
		{
			General     = new Settings.GeneralSettings(rhs.General);
			Paths       = new Settings.PathSettings(rhs.Paths);
			MainWindow  = new Gui.Settings.MainWindowSettings(rhs.MainWindow);
			NewTerminal = new Gui.Settings.NewTerminalSettings(rhs.NewTerminal);
			RecentFiles = new Gui.Settings.RecentFileSettings(rhs.RecentFiles);

			ClearChanged();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("FileType")]
		public string FileType
		{
			get { return ("YAT local user settings"); }
			set { } // do nothing
		}

		[XmlElement("Warning")]
		public string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // do nothing
		}

		[XmlElement("Saved")]
		public SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // do nothing
		}

		[XmlElement("ProductVersion")]
		public string ProductVersion
		{
			get { return (_productVersion); }
			set { } // do nothing
		}

		[XmlElement("General")]
		public Settings.GeneralSettings General
		{
			get { return (_general); }
			set
			{
				if (_general == null)
				{
					_general = value;
					AttachNode(_general);
				}
				else if (_general != value)
				{
					Settings.GeneralSettings old = _general;
					_general = value;
					ReplaceNode(old, _general);
				}
			}
		}

		[XmlElement("Paths")]
		public Settings.PathSettings Paths
		{
			get { return (_paths); }
			set
			{
				if (_paths == null)
				{
					_paths = value;
					AttachNode(_paths);
				}
				else if (_paths != value)
				{
					Settings.PathSettings old = _paths;
					_paths = value;
					ReplaceNode(old, _paths);
				}
			}
		}

		[XmlElement("MainWindow")]
		public Gui.Settings.MainWindowSettings MainWindow
		{
			get { return (_mainWindow); }
			set
			{
				if (_mainWindow == null)
				{
					_mainWindow = value;
					AttachNode(_mainWindow);
				}
				else if (_mainWindow != value)
				{
					Gui.Settings.MainWindowSettings old = _mainWindow;
					_mainWindow = value;
					ReplaceNode(old, _mainWindow);
				}
			}
		}

		[XmlElement("NewTerminal")]
		public Gui.Settings.NewTerminalSettings NewTerminal
		{
			get { return (_newTerminal); }
			set
			{
				if (_newTerminal == null)
				{
					_newTerminal = value;
					AttachNode(_newTerminal);
				}
				else if (_newTerminal != value)
				{
					Gui.Settings.NewTerminalSettings old = _newTerminal;
					_newTerminal = value;
					ReplaceNode(old, _newTerminal);
				}
			}
		}

		[XmlElement("RecentFiles")]
		public Gui.Settings.RecentFileSettings RecentFiles
		{
			get { return (_recentFiles); }
			set
			{
				if (_recentFiles == null)
				{
					_recentFiles = value;
					AttachNode(_recentFiles);
				}
				else if (_recentFiles != value)
				{
					Gui.Settings.RecentFileSettings old = _recentFiles;
					_recentFiles = value;
					ReplaceNode(old, _recentFiles);
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
			if (obj is LocalUserSettingsRoot)
				return (Equals((LocalUserSettingsRoot)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LocalUserSettingsRoot value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_productVersion.Equals(value._productVersion) &&
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
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LocalUserSettingsRoot lhs, LocalUserSettingsRoot rhs)
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
		public static bool operator !=(LocalUserSettingsRoot lhs, LocalUserSettingsRoot rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}	
}
