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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;
using MKY.Time;

using YAT.Model.Types;

#endregion

namespace YAT.Settings.Model
{
	/// <remarks>Root name is relevant for potential future 'AlternateXmlElements'.</remarks>
	/// <remarks>An explicit name makes little sense as this is the very root of the XML.</remarks>
	[XmlRoot("Settings")]
	public class CommandPagesSettingsRoot : MKY.Settings.SettingsItem, IEquatable<CommandPagesSettingsRoot>
	{
		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.0.2";

		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private PredefinedCommandPageCollection pages;

		/// <summary></summary>
		public CommandPagesSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public CommandPagesSettingsRoot(CommandPagesSettingsRoot rhs)
			: base(rhs)
		{
			Pages = new PredefinedCommandPageCollection(rhs.Pages);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Pages = new PredefinedCommandPageCollection();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>Settings name is kind of a title, therefore capital 'C', 'P' and 'D'.</remarks>
		[XmlElement("SettingsName")]
		public virtual string SettingsName
		{
			get { return (ApplicationEx.CommonName + " Command Pages Definition"); } // Name must *not* differ for "YAT" and "YATConsole"
			set { /* Do nothing, this meta XML element is read-only. */ }            // in order to allow exchanging settings!
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

		/// <remarks>
		/// Commands are intentionally organized as pages but not as subpages.
		/// Reason: Subpages are only a representation of the view, but not the settings.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Pages")]
		public PredefinedCommandPageCollection Pages
		{
			get { return (this.pages); }
			set
			{
				if (!IEnumerableEx.ItemsEqual(this.pages, value))
				{
					this.pages = new PredefinedCommandPageCollection(value); // Clone to ensure decoupling.
					SetMyChanged();
				}
			}
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
				hashCode = (hashCode * 397) ^ (Pages          != null ? Pages         .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as CommandPagesSettingsRoot));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(CommandPagesSettingsRoot other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(ProductVersion, other.ProductVersion) &&
				ObjectEx.Equals(                 Pages,          other.Pages)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(CommandPagesSettingsRoot lhs, CommandPagesSettingsRoot rhs)
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
		public static bool operator !=(CommandPagesSettingsRoot lhs, CommandPagesSettingsRoot rhs)
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
