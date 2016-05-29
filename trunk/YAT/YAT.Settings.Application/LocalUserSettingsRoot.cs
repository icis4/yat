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
			General       = new GeneralSettings(MKY.Settings.SettingsType.Explicit);
			Paths         = new PathSettings(MKY.Settings.SettingsType.Explicit);
			Extensions    = new ExtensionSettings(MKY.Settings.SettingsType.Explicit);
			AutoWorkspace = new AutoWorkspaceSettings(MKY.Settings.SettingsType.Explicit);
			MainWindow    = new Model.Settings.MainWindowSettings(MKY.Settings.SettingsType.Explicit);
			NewTerminal   = new Model.Settings.NewTerminalSettings(MKY.Settings.SettingsType.Explicit);
			RecentFiles   = new Model.Settings.RecentFileSettings(MKY.Settings.SettingsType.Explicit);

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
				if (this.general != value)
				{
					var oldNode = this.general;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.general = value;
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
				if (this.paths != value)
				{
					var oldNode = this.paths;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.paths = value;
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
				if (this.extensions != value)
				{
					var oldNode = this.extensions;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.extensions = value;
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
				if (this.autoAutoWorkspace != value)
				{
					var oldNode = this.autoAutoWorkspace;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.autoAutoWorkspace = value;
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
				if (this.mainWindow != value)
				{
					var oldNode = this.mainWindow;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.mainWindow = value;
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
				if (this.newTerminal != value)
				{
					var oldNode = this.newTerminal;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.newTerminal = value;
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
				if (this.recentFiles != value)
				{
					var oldNode = this.recentFiles;
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.recentFiles = value;
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ ProductVersion.GetHashCode();

				return (hashCode);
			}
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
