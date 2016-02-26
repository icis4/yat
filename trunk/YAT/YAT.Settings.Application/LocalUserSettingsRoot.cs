//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using YAT.Application.Settings;
using YAT.Application.Utilities;

namespace YAT.Settings.Application
{
	/// <summary></summary>
	[Serializable]
	[XmlRoot("LocalUserSettings")]
	public class LocalUserSettingsRoot : MKY.Settings.SettingsItem, MKY.Xml.IAlternateXmlElementProvider
	{
		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.4.1";

		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private GeneralSettings general;
		private PathSettings paths;
		private ExtensionSettings extensions;
		private AutoWorkspaceSettings autoAutoWorkspace;
		private Model.Settings.MainWindowSettings mainWindow;
		private Model.Settings.NewTerminalSettings newTerminal;
		private Model.Settings.RecentFileSettings recentFiles;

		/// <summary></summary>
		public LocalUserSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			General       = new GeneralSettings();
			Paths         = new PathSettings();
			Extensions    = new ExtensionSettings();
			AutoWorkspace = new AutoWorkspaceSettings();
			MainWindow    = new Model.Settings.MainWindowSettings();
			NewTerminal   = new Model.Settings.NewTerminalSettings();
			RecentFiles   = new Model.Settings.RecentFileSettings();

			ClearChanged();
		}

		/// <summary></summary>
		public LocalUserSettingsRoot(LocalUserSettingsRoot rhs)
			: base(rhs)
		{
			General       = new GeneralSettings(rhs.General);
			Paths         = new PathSettings(rhs.Paths);
			Extensions    = new ExtensionSettings(rhs.Extensions);
			AutoWorkspace = new AutoWorkspaceSettings(rhs.AutoWorkspace);
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
			get { return (ApplicationEx.ProductName + " local user settings"); }
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
		public virtual GeneralSettings General
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
					GeneralSettings old = this.general;
					this.general = value;
					ReplaceNode(old, this.general);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Paths")]
		public virtual PathSettings Paths
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
					PathSettings old = this.paths;
					this.paths = value;
					ReplaceNode(old, this.paths);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Extensions")]
		public virtual ExtensionSettings Extensions
		{
			get { return (this.extensions); }
			set
			{
				if (value == null)
				{
					DetachNode(this.extensions);
					this.extensions = null;
				}
				else if (this.extensions == null)
				{
					this.extensions = value;
					AttachNode(this.extensions);
				}
				else if (this.extensions != value)
				{
					ExtensionSettings old = this.extensions;
					this.extensions = value;
					ReplaceNode(old, this.extensions);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoWorkspace")]
		public virtual AutoWorkspaceSettings AutoWorkspace
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
					AutoWorkspaceSettings old = this.autoAutoWorkspace;
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

		#endregion

		#region Alternate Elements
		//==========================================================================================
		// Alternate Elements
		//==========================================================================================

		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "See comment above.")]
		private static readonly MKY.Xml.AlternateXmlElement[] StaticAlternateXmlElements =
		{
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MainFiles",           new string[] { "TerminalFilesPath" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MainFilesPath",       new string[] { "WorkspaceFilesPath" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "SendFiles",           new string[] { "SendFilesPath" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "LogFiles",            new string[] { "LogFilesPath" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MonitorFiles",        new string[] { "MonitorFilesPath" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "NewTerminal" }, "SocketRemoteTcpPort", new string[] { "SocketRemotePort" } ),
		};

		/// <summary></summary>
		[XmlIgnore]
		public virtual MKY.Xml.AlternateXmlElement[] AlternateXmlElements
		{
			get { return (StaticAlternateXmlElements); }
		}

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
				base.GetHashCode() ^ // Get hash code of all settings nodes.

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
