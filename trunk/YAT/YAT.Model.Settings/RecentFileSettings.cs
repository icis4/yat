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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY.Recent;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class RecentFileSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int MaxFilePaths = 8;

		private RecentItemCollection<string> filePaths;

		/// <summary></summary>
		public RecentFileSettings()
			: base()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public RecentFileSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public RecentFileSettings(RecentFileSettings rhs)
			: base(rhs)
		{
			FilePaths = new RecentItemCollection<string>(rhs.FilePaths);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
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
				if (value != this.filePaths)
				{
					this.filePaths = value;
					SetChanged();
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

			RecentFileSettings other = (RecentFileSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(FilePaths == other.FilePaths)
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

				FilePaths.GetHashCode()
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
