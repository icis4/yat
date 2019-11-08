//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Time;
using MKY.Xml;

using YAT.Application.Settings;

#endregion

namespace YAT.Settings.Application
{
	/// <remarks>Root name is relevant for <see cref="AlternateXmlElements"/>.</remarks>
	/// <remarks>
	/// An explicit name makes little sense as this is the very root of the XML. But accidentally
	/// named the root explicitly. Should be renamed, but doesn't work because root is not properly
	/// handled by the alternate tolerant deserialization. To be solved using XML transformation.
	/// </remarks>
	[XmlRoot("LocalUserSettings")] // Attention, see remark above!
	public class LocalUserSettingsRoot : MKY.Settings.SettingsItem, IEquatable<LocalUserSettingsRoot>, IAlternateXmlElementProvider
	{
		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.7.0";

		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private GeneralSettings general;
		private PathSettings paths;
		private AutoWorkspaceSettings autoAutoWorkspace;
		private Model.Settings.MainWindowSettings mainWindow;
		private Model.Settings.NewTerminalSettings newTerminal;
		private Model.Settings.RecentFileSettings recentFiles;

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public LocalUserSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			General       = new GeneralSettings(MKY.Settings.SettingsType.Explicit);
			Paths         = new PathSettings(MKY.Settings.SettingsType.Explicit);
			AutoWorkspace = new AutoWorkspaceSettings(MKY.Settings.SettingsType.Explicit);
			MainWindow    = new Model.Settings.MainWindowSettings(MKY.Settings.SettingsType.Explicit);
			NewTerminal   = new Model.Settings.NewTerminalSettings(MKY.Settings.SettingsType.Explicit);
			RecentFiles   = new Model.Settings.RecentFileSettings(MKY.Settings.SettingsType.Explicit);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public LocalUserSettingsRoot(LocalUserSettingsRoot rhs)
			: base(rhs)
		{
			General       = new GeneralSettings(rhs.General);
			Paths         = new PathSettings(rhs.Paths);
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

		/// <remarks>Settings name is kind of a title, therefore capital 'L', 'U' and 'S'.</remarks>
		[XmlElement("SettingsName")]
		public virtual string SettingsName
		{
			get { return (ApplicationEx.ProductName + " Local User Settings"); } // Name shall differ for "YAT" and "YATConsole"
			set { } // Do nothing.                                               // in order to get separate user settings.
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
			get { return ("Modifying structure and/or content may cause undefined behavior!"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Mark")]
		public virtual UserTimeStamp Mark
		{
			get { return (new UserTimeStamp(DateTime.Now, Environment.UserName)); }
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
					this.general = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.paths = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.autoAutoWorkspace = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.mainWindow = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.newTerminal = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.recentFiles = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
		private static readonly AlternateXmlElement[] StaticAlternateXmlElements =
		{                                        // XML path:                                 local name of XML element:            alternate local name(s), i.e. former name(s) of XML element:
		/*	new AlternateXmlElement(new string[] { "#document"                            }, "Settings",            new string[] { "LocalUserSettings" } ), => Accidentally named the root explicitly. Should be renamed, but doesn't work because root is not properly handled by the alternate tolerant deserialization. To be solved using XML transformation. */
			new AlternateXmlElement(new string[] { "#document", "Settings"                }, "SettingsName",        new string[] { "FileType" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings"                }, "Mark",                new string[] { "Saved" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MainFiles",           new string[] { "TerminalFilesPath" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MainFilesPath",       new string[] { "WorkspaceFilesPath" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "SendFiles",           new string[] { "SendFilesPath" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "LogFiles",            new string[] { "LogFilesPath" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Paths"       }, "MonitorFiles",        new string[] { "MonitorFilesPath" } ),
		/*	new AlternateXmlElement(new string[] { "#document", "Settings", "MainWindow"  }, "AlwaysOnTop",         formerly located in workspace settings */
			new AlternateXmlElement(new string[] { "#document", "Settings", "NewTerminal" }, "SocketRemoteTcpPort", new string[] { "SocketRemotePort" } )
		};

		/// <summary></summary>
		[XmlIgnore]
		public virtual IEnumerable<AlternateXmlElement> AlternateXmlElements
		{
			get { return (StaticAlternateXmlElements); }
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as LocalUserSettingsRoot));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(LocalUserSettingsRoot other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(ProductVersion, other.ProductVersion)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LocalUserSettingsRoot lhs, LocalUserSettingsRoot rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
