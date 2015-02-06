//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Settings.Application
{
	/// <summary></summary>
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot : MKY.Settings.SettingsItem
	{
		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.3.1";

		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string productVersion = Utilities.ApplicationInfo.ProductVersion;

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
		[XmlElement("SettingsVersion")]
		public virtual string SettingsVersion
		{
			get { return (this.settingsVersion); }
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
					DetachNode(this.general);
					this.general = null;
				}
				else if (this.general == null)
				{
					this.general = value;
					AttachNode(this.general);
				}
				else if (this.general != value)
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
					DetachNode(this.paths);
					this.paths = null;
				}
				else if (this.paths == null)
				{
					this.paths = value;
					AttachNode(this.paths);
				}
				else if (this.paths != value)
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
					DetachNode(this.autoAutoWorkspace);
					this.autoAutoWorkspace = null;
				}
				else if (this.autoAutoWorkspace == null)
				{
					this.autoAutoWorkspace = value;
					AttachNode(this.autoAutoWorkspace);
				}
				else if (this.autoAutoWorkspace != value)
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
					DetachNode(this.mainWindow);
					this.mainWindow = null;
				}
				else if (this.mainWindow == null)
				{
					this.mainWindow = value;
					AttachNode(this.mainWindow);
				}
				else if (this.mainWindow != value)
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
					DetachNode(this.newTerminal);
					this.newTerminal = null;
				}
				else if (this.newTerminal == null)
				{
					this.newTerminal = value;
					AttachNode(this.newTerminal);
				}
				else if (this.newTerminal != value)
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
				if (value == null)
				{
					DetachNode(this.recentFiles);
					this.recentFiles = null;
				}
				else if (this.recentFiles == null)
				{
					this.recentFiles = value;
					AttachNode(this.recentFiles);
				}
				else if (this.recentFiles != value)
				{
					Model.Settings.RecentFileSettings old = this.recentFiles;
					this.recentFiles = value;
					ReplaceNode(old, this.recentFiles);
				}
			}
		}

		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		/// <remarks>
		/// \remind (2013-01-02 / mky)
		/// There was the change in the NewTerminalSettings between 1.3.0 and 1.3.1 which was added
		/// below but not made available public because it would have introduced alternate elements
		/// to the local user settings, and the change is too minor to justify this.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "See comment above.")]
		private static readonly MKY.Xml.AlternateXmlElement[] StaticAlternateXmlElements =
			{
				new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "NewTerminal" }, "SocketRemoteTcpPort", new string[] { "SocketRemotePort" } ),
			};

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
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

				(ProductVersion == other.ProductVersion)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				ProductVersion.GetHashCode()
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
