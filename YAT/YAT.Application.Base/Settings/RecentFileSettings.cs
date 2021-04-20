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
// YAT Version 2.4.1
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY.Collections;
using MKY.Collections.Specialized;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class RecentFileSettings : MKY.Settings.SettingsItem, IEquatable<RecentFileSettings>
	{
		/// <remarks>
		/// Limited to 8 for...
		/// <list type="bullet">
		/// <item><description>...allowing keyboard shortcuts 1..8 in the menu.</description></item>
		/// <item><description>..."nice" looking menu numbering (10 would "spoil" the list).</description></item>
		/// </list>
		/// Limitation in other applications:
		/// <list type="bullet">
		/// <item><description>Visual Studio = 10.</description></item>
		/// <item><description>Notepad++ = 10.</description></item>
		/// <item><description>Programmer's Notepad = 15.</description></item>
		/// </list>
		/// </remarks>
		public const int MaxFilePaths = 8;

		private RecentItemCollection<string> filePaths;

		/// <summary></summary>
		public RecentFileSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public RecentFileSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public RecentFileSettings(RecentFileSettings rhs)
			: base(rhs)
		{
			FilePaths = new RecentItemCollection<string>(rhs.FilePaths);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FilePaths = new RecentItemCollection<string>(MaxFilePaths);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("FilePaths")]
		public virtual RecentItemCollection<string> FilePaths
		{
			get { return (this.filePaths); }
			set
			{
				if (this.filePaths != value)
				{
					this.filePaths = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This property allows standard XML serialization which is not provided for
		/// generic collection <see cref="T:RecentItemCollection`1"/>.
		/// </remarks>
		[XmlElement("FilePathsCapacity")]
		public virtual int FilePathsCapacity
		{
			get { return (this.filePaths.Capacity); }
			set { this.filePaths.Capacity = value; }
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

				hashCode = (hashCode * 397) ^ (FilePaths != null ? FilePaths.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as RecentFileSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(RecentFileSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				IEnumerableEx.ItemsEqual(FilePaths, other.FilePaths)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(RecentFileSettings lhs, RecentFileSettings rhs)
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
		public static bool operator !=(RecentFileSettings lhs, RecentFileSettings rhs)
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
