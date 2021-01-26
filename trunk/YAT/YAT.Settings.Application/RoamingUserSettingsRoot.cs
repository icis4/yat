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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	[XmlRoot("RoamingUserSettings")] // Attention, see remark above!
	public class RoamingUserSettingsRoot : MKY.Settings.SettingsItem, IEquatable<RoamingUserSettingsRoot>, IAlternateXmlElementProvider
	{
		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.2.0";

		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private SocketSettings socket;
		private FindSettings find;
		private AutoActionSettings autoAction;
		private AutoResponseSettings autoResponse;
		private ViewSettings view;
		private PlotSettings plot;
		private ExtensionSettings extensions;

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public RoamingUserSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			Socket       = new SocketSettings(MKY.Settings.SettingsType.Explicit);
			Find         = new FindSettings(MKY.Settings.SettingsType.Explicit);
			AutoAction   = new AutoActionSettings(MKY.Settings.SettingsType.Explicit);
			AutoResponse = new AutoResponseSettings(MKY.Settings.SettingsType.Explicit);
			View         = new ViewSettings(MKY.Settings.SettingsType.Explicit);
			Plot         = new PlotSettings(MKY.Settings.SettingsType.Explicit);
			Extensions   = new ExtensionSettings(MKY.Settings.SettingsType.Explicit);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public RoamingUserSettingsRoot(RoamingUserSettingsRoot rhs)
			: base(rhs)
		{
			Socket       = new SocketSettings(rhs.Socket);
			Find         = new FindSettings(rhs.Find);
			AutoAction   = new AutoActionSettings(rhs.AutoAction);
			AutoResponse = new AutoResponseSettings(rhs.AutoResponse);
			View         = new ViewSettings(rhs.View);
			Plot         = new PlotSettings(rhs.Plot);
			Extensions   = new ExtensionSettings(rhs.Extensions);

			ClearChanged();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>Settings name is kind of a title, therefore capital 'R', 'U' and 'S'.</remarks>
		[XmlElement("SettingsName")]
		public virtual string SettingsName
		{
			get { return (ApplicationEx.ProductName + " Roaming User Settings"); } // Name shall differ for "YAT" and "YATConsole"
			set { /* Do nothing, this meta XML element is read-only. */ }          // in order to get separate user settings.
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

		/// <summary></summary>
		[XmlElement("Socket")]
		public virtual SocketSettings Socket
		{
			get { return (this.socket); }
			set
			{
				if (this.socket != value)
				{
					var oldNode = this.socket;
					this.socket = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Find")]
		public virtual FindSettings Find
		{
			get { return (this.find); }
			set
			{
				if (this.find != value)
				{
					var oldNode = this.find;
					this.find = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoAction")]
		public virtual AutoActionSettings AutoAction
		{
			get { return (this.autoAction); }
			set
			{
				if (this.autoAction != value)
				{
					var oldNode = this.autoAction;
					this.autoAction = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoResponse")]
		public virtual AutoResponseSettings AutoResponse
		{
			get { return (this.autoResponse); }
			set
			{
				if (this.autoResponse != value)
				{
					var oldNode = this.autoResponse;
					this.autoResponse = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("View")]
		public virtual ViewSettings View
		{
			get { return (this.view); }
			set
			{
				if (this.view != value)
				{
					var oldNode = this.view;
					this.view = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Plot")]
		public virtual PlotSettings Plot
		{
			get { return (this.plot); }
			set
			{
				if (this.plot != value)
				{
					var oldNode = this.plot;
					this.plot = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
					this.extensions = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

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
		{                                        // XML path:                                     local name of XML element:              alternate local name(s), i.e. former name(s) of XML element:
		/*	new AlternateXmlElement(new string[] { "#document"             },                    "Settings",              new string[] { "RoamingUserSettings" } ), => Accidentally named the root explicitly. Should be renamed, but doesn't work because root is not properly handled by the alternate tolerant deserialization. To be solved using XML transformation. */
			new AlternateXmlElement(new string[] { "#document", "Settings" },                    "SettingsName",          new string[] { "FileType" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings" },                    "Mark",                  new string[] { "Saved" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Find", "Options" }, "EnableRegex",           new string[] { "UseRegex" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "View" },            "FindIsVisible",         new string[] { "FindVisible" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "View" },            "AutoActionIsVisible",   new string[] { "AutoActionVisible" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "View" },            "AutoResponseIsVisible", new string[] { "AutoResponseVisible" } ),
			new AlternateXmlElement(new string[] { "#document", "Settings", "Extensions" },      "ControlLogFiles",       new string[] { "PortLogFiles" } )
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
			return (Equals(obj as RoamingUserSettingsRoot));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(RoamingUserSettingsRoot other)
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
		public static bool operator ==(RoamingUserSettingsRoot lhs, RoamingUserSettingsRoot rhs)
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
		public static bool operator !=(RoamingUserSettingsRoot lhs, RoamingUserSettingsRoot rhs)
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
