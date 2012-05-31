//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	/// <summary></summary>
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot : MKY.Settings.SettingsItem
	{
		private string productVersion = System.Windows.Forms.Application.ProductVersion;
		private Settings.GeneralSettings general;
		private Settings.PathSettings paths;
		private Settings.AutoWorkspaceSettings autoAutoWorkspace;
		private Model.Settings.MainWindowSettings mainWindow;
		private Model.Settings.NewTerminalSettings newTerminal;
		private Model.Settings.RecentFileSettings recentFiles;

		/// <summary></summary>
		public LocalUserSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			General       = new Settings.GeneralSettings();
			Paths         = new Settings.PathSettings();
			AutoWorkspace = new Settings.AutoWorkspaceSettings();
			MainWindow    = new Model.Settings.MainWindowSettings();
			NewTerminal   = new Model.Settings.NewTerminalSettings();
			RecentFiles   = new Model.Settings.RecentFileSettings();

			ClearChanged();
		}

		/// <summary></summary>
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

		/// <summary></summary>
		[XmlElement("FileType")]
		public virtual string FileType
		{
			get { return ("YAT local user settings"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (this.productVersion); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying this file may cause undefined behavior!"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Saved")]
		public virtual SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("General")]
		public virtual Settings.GeneralSettings General
		{
			get { return (this.general); }
			set
			{
				if (value == null)
				{
					this.general = value;
					DetachNode(this.general);
				}
				else if (this.general == null)
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

		/// <summary></summary>
		[XmlElement("Paths")]
		public virtual Settings.PathSettings Paths
		{
			get { return (this.paths); }
			set
			{
				if (value == null)
				{
					this.paths = value;
					DetachNode(this.paths);
				}
				else if (this.paths == null)
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

		/// <summary></summary>
		[XmlElement("AutoWorkspace")]
		public virtual Settings.AutoWorkspaceSettings AutoWorkspace
		{
			get { return (this.autoAutoWorkspace); }
			set
			{
				if (value == null)
				{
					this.autoAutoWorkspace = value;
					DetachNode(this.autoAutoWorkspace);
				}
				else if (this.autoAutoWorkspace == null)
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

		/// <summary></summary>
		[XmlElement("MainWindow")]
		public virtual Model.Settings.MainWindowSettings MainWindow
		{
			get { return (this.mainWindow); }
			set
			{
				if (value == null)
				{
					this.mainWindow = value;
					DetachNode(this.mainWindow);
				}
				else if (this.mainWindow == null)
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

		/// <summary></summary>
		[XmlElement("NewTerminal")]
		public virtual Model.Settings.NewTerminalSettings NewTerminal
		{
			get { return (this.newTerminal); }
			set
			{
				if (value == null)
				{
					this.newTerminal = value;
					DetachNode(this.newTerminal);
				}
				else if (this.newTerminal == null)
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

		/// <summary></summary>
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			LocalUserSettingsRoot other = (LocalUserSettingsRoot)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.
				(this.productVersion == other.productVersion)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^
				this.productVersion.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}	
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
