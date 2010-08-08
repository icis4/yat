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

namespace YAT.Settings.Application
{
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot : MKY.Utilities.Settings.Settings, IEquatable<LocalUserSettingsRoot>
	{
		private string productVersion = System.Windows.Forms.Application.ProductVersion;
		private Settings.GeneralSettings general;
		private Settings.PathSettings paths;
		private Settings.AutoWorkspaceSettings autoAutoWorkspace;
		private Model.Settings.MainWindowSettings mainWindow;
		private Model.Settings.NewTerminalSettings newTerminal;
		private Model.Settings.RecentFileSettings recentFiles;

		public LocalUserSettingsRoot()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			General       = new Settings.GeneralSettings();
			Paths         = new Settings.PathSettings();
			AutoWorkspace = new Settings.AutoWorkspaceSettings();
			MainWindow    = new Model.Settings.MainWindowSettings();
			NewTerminal   = new Model.Settings.NewTerminalSettings();
			RecentFiles   = new Model.Settings.RecentFileSettings();

			ClearChanged();
		}

		public LocalUserSettingsRoot(LocalUserSettingsRoot rhs)
			: base(rhs)
		{
			General       = new Settings.GeneralSettings(rhs.General);
			Paths         = new Settings.PathSettings(rhs.Paths);
			AutoWorkspace = new Settings.AutoWorkspaceSettings(rhs.AutoWorkspace);
			MainWindow    = new Model.Settings.MainWindowSettings(rhs.MainWindow);
			NewTerminal   = new Model.Settings.NewTerminalSettings(rhs.NewTerminal);
			RecentFiles   = new Model.Settings.RecentFileSettings(rhs.RecentFiles);

			ClearChanged();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[XmlElement("FileType")]
		public virtual string FileType
		{
			get { return ("YAT local user settings"); }
			set { } // Do nothing.
		}

		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // Do nothing.
		}

		[XmlElement("Saved")]
		public virtual SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // Do nothing.
		}

		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (this.productVersion); }
			set { } // Do nothing.
		}

		[XmlElement("General")]
		public virtual Settings.GeneralSettings General
		{
			get { return (this.general); }
			set
			{
				if (this.general == null)
				{
					this.general = value;
					AttachNode(this.general);
				}
				else if (value != this.general)
				{
					Settings.GeneralSettings old = this.general;
					this.general = value;
					ReplaceNode(old, this.general);
				}
			}
		}

		[XmlElement("Paths")]
		public virtual Settings.PathSettings Paths
		{
			get { return (this.paths); }
			set
			{
				if (this.paths == null)
				{
					this.paths = value;
					AttachNode(this.paths);
				}
				else if (value != this.paths)
				{
					Settings.PathSettings old = this.paths;
					this.paths = value;
					ReplaceNode(old, this.paths);
				}
			}
		}

		[XmlElement("AutoWorkspace")]
		public virtual Settings.AutoWorkspaceSettings AutoWorkspace
		{
			get { return (this.autoAutoWorkspace); }
			set
			{
				if (this.autoAutoWorkspace == null)
				{
					this.autoAutoWorkspace = value;
					AttachNode(this.autoAutoWorkspace);
				}
				else if (value != this.autoAutoWorkspace)
				{
					Settings.AutoWorkspaceSettings old = this.autoAutoWorkspace;
					this.autoAutoWorkspace = value;
					ReplaceNode(old, this.autoAutoWorkspace);
				}
			}
		}

		[XmlElement("MainWindow")]
		public virtual Model.Settings.MainWindowSettings MainWindow
		{
			get { return (this.mainWindow); }
			set
			{
				if (this.mainWindow == null)
				{
					this.mainWindow = value;
					AttachNode(this.mainWindow);
				}
				else if (value != this.mainWindow)
				{
					Model.Settings.MainWindowSettings old = this.mainWindow;
					this.mainWindow = value;
					ReplaceNode(old, this.mainWindow);
				}
			}
		}

		[XmlElement("NewTerminal")]
		public virtual Model.Settings.NewTerminalSettings NewTerminal
		{
			get { return (this.newTerminal); }
			set
			{
				if (this.newTerminal == null)
				{
					this.newTerminal = value;
					AttachNode(this.newTerminal);
				}
				else if (value != this.newTerminal)
				{
					Model.Settings.NewTerminalSettings old = this.newTerminal;
					this.newTerminal = value;
					ReplaceNode(old, this.newTerminal);
				}
			}
		}

		[XmlElement("RecentFiles")]
		public virtual Model.Settings.RecentFileSettings RecentFiles
		{
			get { return (this.recentFiles); }
			set
			{
				if (this.recentFiles == null)
				{
					this.recentFiles = value;
					AttachNode(this.recentFiles);
				}
				else if (value != this.recentFiles)
				{
					Model.Settings.RecentFileSettings old = this.recentFiles;
					this.recentFiles = value;
					ReplaceNode(old, this.recentFiles);
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
			if (obj == null)
				return (false);

			LocalUserSettingsRoot casted = obj as LocalUserSettingsRoot;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LocalUserSettingsRoot casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.
				(this.productVersion == casted.productVersion)
			);
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
