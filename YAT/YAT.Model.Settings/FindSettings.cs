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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;
using MKY.Collections.Specialized;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class FindSettings : MKY.Settings.SettingsItem, IEquatable<FindSettings>
	{
		private const int MaxRecentFindPatterns = 12;

		private bool showFindField;
		private string activeFindPattern;
		private RecentItemCollection<string> recentFindPatterns;

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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public FindSettings(FindSettings rhs)
			: base(rhs)
		{
			ShowFindField      = rhs.ShowFindField;
			ActiveFindPattern  = rhs.ActiveFindPattern;
			RecentFindPatterns = new RecentItemCollection<string>(rhs.RecentFindPatterns);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ShowFindField      = false;
			ActiveFindPattern  = null;
			RecentFindPatterns = new RecentItemCollection<string>(MaxRecentFindPatterns);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("ShowFindField")]
		public bool ShowFindField
		{
			get { return (this.showFindField); }
			set
			{
				if (this.showFindField != value)
				{
					this.showFindField = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ActiveFindPattern")]
		public string ActiveFindPattern
		{
			get { return (this.activeFindPattern); }
			set
			{
				if (this.activeFindPattern != value)
				{
					this.activeFindPattern = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentFindPatterns")]
		public RecentItemCollection<string> RecentFindPatterns
		{
			get { return (this.recentFindPatterns); }
			set
			{
				if (this.recentFindPatterns != value)
				{
					this.recentFindPatterns = value;
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

				hashCode = (hashCode * 397) ^  ShowFindField                                  .GetHashCode();
				hashCode = (hashCode * 397) ^ (ActiveFindPattern  != null ? ActiveFindPattern .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RecentFindPatterns != null ? RecentFindPatterns.GetHashCode() : 0);

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

				ShowFindField        .Equals(                          other.ShowFindField)      &&
				StringEx             .EqualsOrdinal(ActiveFindPattern, other.ActiveFindPattern)  &&
				IEnumerableEx.ElementsEqual(       RecentFindPatterns, other.RecentFindPatterns)
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
