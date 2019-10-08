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
// YAT Version 2.3.90 Development
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;
using MKY.Collections.Specialized;

using YAT.Model.Types;

#endregion

namespace YAT.Model.Settings
{
	/// <remarks>
	/// \remind (2017-11-19 / MKY), (2019-08-02 / MKY)
	/// Should be migrated to a separate 'YAT.Application.Settings' project. Not easily possible
	/// because of dependencies among 'YAT.*' and 'YAT.Application', e.g. 'ExtensionSettings'.
	/// Requires slight refactoring of project dependencies. Could be done when refactoring the
	/// projects on integration with Albatros.
	/// </remarks>
	public class FindSettings : MKY.Settings.SettingsItem, IEquatable<FindSettings>
	{
		private const int MaxRecentPatterns = 12;

		private string activePattern;
		private RecentItemCollection<string> recentPatterns;
		private FindOptions options;

		/// <summary></summary>
		public FindSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public FindSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public FindSettings(FindSettings rhs)
			: base(rhs)
		{
			ActivePattern  = rhs.ActivePattern;
			RecentPatterns = new RecentItemCollection<string>(rhs.RecentPatterns);
			Options        = rhs.Options;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ActivePattern  = null;
			RecentPatterns = new RecentItemCollection<string>(MaxRecentPatterns);
			Options        = new FindOptions(true, false, false);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// Using "Pattern" instead of "TextOrPattern" for simplicity.
		/// </remarks>
		[XmlElement("ActivePattern")]
		public string ActivePattern
		{
			get { return (this.activePattern); }
			set
			{
				if (this.activePattern != value)
				{
					this.activePattern = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Using "Pattern" instead of "TextOrPattern" for simplicity.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentPatterns")]
		public RecentItemCollection<string> RecentPatterns
		{
			get { return (this.recentPatterns); }
			set
			{
				if (this.recentPatterns != value)
				{
					this.recentPatterns = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Options")]
		public FindOptions Options
		{
			get { return (this.options); }
			set
			{
				if (this.options != value)
				{
					this.options = value;
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

				hashCode = (hashCode * 397) ^ (ActivePattern  != null ? ActivePattern .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RecentPatterns != null ? RecentPatterns.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  Options                                .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FindSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FindSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx          .EqualsOrdinal(ActivePattern, other.ActivePattern)  &&
				IEnumerableEx.ItemsEqual(       RecentPatterns, other.RecentPatterns) &&
				Options           .Equals(                      other.Options)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(FindSettings lhs, FindSettings rhs)
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
		public static bool operator !=(FindSettings lhs, FindSettings rhs)
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
