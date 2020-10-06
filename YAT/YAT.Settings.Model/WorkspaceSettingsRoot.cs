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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Xml.Serialization;

using MKY;
using MKY.Time;
using MKY.Xml;

using YAT.Model.Settings;

#endregion

namespace YAT.Settings.Model
{
	/// <remarks>Root name is relevant for <see cref="AlternateXmlElements"/>.</remarks>
	/// <remarks>An explicit name makes little sense as this is the very root of the XML.</remarks>
	[XmlRoot("Settings")]
	public class WorkspaceSettingsRoot : MKY.Settings.SettingsItem, IEquatable<WorkspaceSettingsRoot>
	{
		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.2.3";

		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private bool autoSaved;
		private WorkspaceSettings workspace;

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public WorkspaceSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			Workspace = new WorkspaceSettings(MKY.Settings.SettingsType.Explicit);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public WorkspaceSettingsRoot(WorkspaceSettingsRoot rhs)
			: base(rhs)
		{
			Workspace = new WorkspaceSettings(rhs.Workspace);

			ClearChanged();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>Settings name is kind of a title, therefore capital 'W' and 'S'.</remarks>
		[XmlElement("SettingsName")]
		public virtual string SettingsName
		{
			get { return (ApplicationEx.CommonName + " Workspace Settings"); } // Name shall *not* differ for "YAT" and "YATConsole"
			set { /* Do nothing, this meta XML element is read-only. */ }      // in order to allow exchanging settings.
		}

		/// <summary></summary>
		[XmlElement("SettingsVersion")]
		public virtual string SettingsVersion
		{
			get { return (this.settingsVersion); }
			set { /* Do nothing, this meta XML element is read-only. */ }
		}

		/// <summary></summary>
		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (this.productVersion); }
			set { /* Do nothing, this meta XML element is read-only. */ }
		}

		/// <summary></summary>
		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying structure and/or content may cause undefined behavior!"); }
			set { /* Do nothing, this meta XML element is read-only. */ }
		}

		/// <summary></summary>
		[XmlElement("Mark")]
		public virtual UserTimeStamp Mark
		{
			get { return (new UserTimeStamp(DateTime.Now, Environment.UserName)); }
			set { /* Do nothing, this meta XML element is read-only. */ }
		}

		/// <summary>
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </summary>
		[XmlElement("AutoSaved")]
		public virtual bool AutoSaved
		{
			get { return (this.autoSaved); }
			set
			{
				if (this.autoSaved != value)
				{
					this.autoSaved = value;

					// Do not set changed.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Workspace")]
		public virtual WorkspaceSettings Workspace
		{
			get { return (this.workspace); }
			set
			{
				if (this.workspace != value)
				{
					var oldNode = this.workspace;
					this.workspace = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		#region Properties > Shortcuts
		//------------------------------------------------------------------------------------------
		// Properties > Shortcuts
		//------------------------------------------------------------------------------------------

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public GuidList<TerminalSettingsItem> TerminalSettings
		{
			get { return (this.workspace.TerminalSettings); }
		}

		#endregion

		#endregion

		#region Alternate Elements
		//==========================================================================================
		// Alternate Elements
		//==========================================================================================

		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		/// <remarks>
		/// \remind (2008-06-07 / MKY) (2 hours to the first Euro2008 game :-)
		/// Instead of this approach, an [AlternateXmlElementAttribute] based approach should be tried
		/// in a future version. Such approach would be beneficial in terms of modularity because the
		/// XML path wouldn't need to be considered, i.e. changes in the path could be handled. This is
		/// not the case currently.
		/// \remind (2011-10-09 / MKY) (no Euro2012 games with Switzerland :-(
		/// Cannot alternate 'Display.ShowConnectTime|ShowCounters' to 'Status.ShowConnectTime|ShowCountAndRate'
		/// due to limitation described above.
		/// \remind (2012-10-29 / MKY)
		/// Attention, the solution above is OK for the give use case, however, it wouldn't allow to
		/// alternate the depth of the path as well. Such alternate is required for the commented
		/// case with 'EolComment' below.
		/// \remind (2016-04-05 / MKY)
		/// Ideally, simple alternate elements should be definable right at the element. Example:
		/// "SocketSettings.HostType" got simplified to "SocketSettings.Type"
		/// The alternate name (i.e. the old name) should be definable in 'SocketSettings'.
		/// </remarks>
		private static readonly MKY.Xml.AlternateXmlElement[] StaticAlternateXmlElements =
		{                                        // XML path:                                                   local name of XML element:     alternate local name(s), i.e. former name(s) of XML element:
			new AlternateXmlElement(new string[] { "#document", "Settings"                                  }, "SettingsName", new string[] { "FileType" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings"                                  }, "Mark",         new string[] { "Saved" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Workspace", "TerminalSettings" }, "FixedId",      new string[] { "FixedIndex" } )
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

				hashCode = (hashCode * 397) ^ (ProductVersion != null ? ProductVersion.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as WorkspaceSettingsRoot));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(WorkspaceSettingsRoot other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(ProductVersion, other.ProductVersion)
				//// Do not consider 'AutoSaved' since that doesn't change value equality.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(WorkspaceSettingsRoot lhs, WorkspaceSettingsRoot rhs)
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
		public static bool operator !=(WorkspaceSettingsRoot lhs, WorkspaceSettingsRoot rhs)
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
