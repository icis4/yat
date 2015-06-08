﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the Sytem.Guid class.
namespace MKY
{
	/// <summary>
	/// List with additional methods to handle items providing a <see cref="Guid"/>.
	/// </summary>
	/// <typeparam name="T">Type that implements <see cref="IGuidProvider"/>.</typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This actually IS a list, not just any kind of collection.")]
	[Serializable]
	public class GuidList<T> : List<T>
		where T : IGuidProvider
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public GuidList()
			: base()
		{
		}

		/// <summary></summary>
		public GuidList(IEnumerable<T> collection)
			: base(collection)
		{
		}

		/// <summary></summary>
		public GuidList(int capacity)
			: base(capacity)
		{
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Add or replaces the item that has the same <see cref="Guid"/> as item.
		/// </summary>
		public virtual void AddOrReplaceGuidItem(T item)
		{
			// replace or add if not contained yet
			if (!ReplaceGuidItem(item))
				Add(item);
		}

		/// <summary>
		/// Replaces the item that has the same <see cref="Guid"/> as item.
		/// </summary>
		public virtual bool ReplaceGuidItem(T item)
		{
			GuidList<T> clone = new GuidList<T>(this);
			for (int i = 0; i < clone.Count; i++)
			{
				if (this[i].Guid == item.Guid)
				{
					this[i] = item;
					return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Returns first item within the list that has the specified <see cref="Guid"/>,
		/// <c>null</c> otherwise.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual T GetGuidItem(Guid guid)
		{
			foreach (T item in this)
			{
				if (item.Guid == guid)
					return (item);
			}
			return (default(T));
		}

		/// <summary>
		/// Removes all items that have the specified <see cref="Guid"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual void RemoveGuid(Guid guid)
		{
			GuidList<T> obsoleteItems = new GuidList<T>();

			foreach (T item in this)
			{
				if (item.Guid == guid)
					obsoleteItems.Add(item);
			}

			foreach (T item in obsoleteItems)
			{
				Remove(item);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
