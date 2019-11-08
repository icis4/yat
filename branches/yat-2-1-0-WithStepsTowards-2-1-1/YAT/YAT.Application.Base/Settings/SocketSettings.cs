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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY.Collections;
using MKY.Collections.Specialized;
using MKY.Net;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class SocketSettings : MKY.Settings.SettingsItem, IEquatable<SocketSettings>
	{
		private const int MaxRecentItems = 12;

		private RecentIPHostCollection recentRemoteHosts;
		private RecentIPFilterCollection recentLocalFilters;
		private RecentItemCollection<int> recentPorts;

		/// <summary></summary>
		public SocketSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SocketSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SocketSettings(SocketSettings rhs)
			: base(rhs)
		{
			RecentRemoteHosts  = new RecentIPHostCollection(rhs.RecentRemoteHosts);
			RecentLocalFilters = new RecentIPFilterCollection(rhs.RecentLocalFilters);
			RecentPorts        = new RecentItemCollection<int>(rhs.RecentPorts);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			RecentRemoteHosts = new RecentIPHostCollection(MaxRecentItems);
			//// Standard hosts are automatically added by the collection.

			RecentLocalFilters = new RecentIPFilterCollection(MaxRecentItems);
			//// Standard hosts are automatically added by the collection.

			RecentPorts = new RecentItemCollection<int>(MaxRecentItems);
			RecentPorts.Add(MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault);
			RecentPorts.Add(MKY.IO.Serial.Socket.SocketSettings.LocalPortDefault + 1); // Higher value shall be on top.
			RecentPorts.Add(MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault);
			RecentPorts.Add(MKY.IO.Serial.Socket.SocketSettings.RemotePortDefault + 1); // Remote value shall be on very top.
			SetChanged(); // Manual change required because underlying collection is modified.
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentRemoteHosts")]
		public RecentIPHostCollection RecentRemoteHosts
		{
			get { return (this.recentRemoteHosts); }
			set
			{
				if (this.recentRemoteHosts != value)
				{
					this.recentRemoteHosts = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentLocalFilters")]
		public RecentIPFilterCollection RecentLocalFilters
		{
			get { return (this.recentLocalFilters); }
			set
			{
				if (this.recentLocalFilters != value)
				{
					this.recentLocalFilters = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentPorts")]
		public RecentItemCollection<int> RecentPorts
		{
			get { return (this.recentPorts); }
			set
			{
				if (this.recentPorts != value)
				{
					this.recentPorts = value;
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

				hashCode = (hashCode * 397) ^ (RecentRemoteHosts  != null ? RecentRemoteHosts .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RecentLocalFilters != null ? RecentLocalFilters.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RecentPorts        != null ? RecentPorts       .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SocketSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SocketSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				IEnumerableEx.ItemsEqual(RecentRemoteHosts,  other.RecentRemoteHosts)  &&
				IEnumerableEx.ItemsEqual(RecentLocalFilters, other.RecentLocalFilters) &&
				IEnumerableEx.ItemsEqual(RecentPorts,        other.RecentPorts)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SocketSettings lhs, SocketSettings rhs)
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
		public static bool operator !=(SocketSettings lhs, SocketSettings rhs)
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
