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
// MKY Version 1.0.27
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Guid for consistency with the System.Guid class.
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
		public virtual void AddOrReplace(T item)
		{
			// replace or add if not contained yet
			if (!Replace(item))
				Add(item);
		}

		/// <summary>
		/// Replaces the item that has the same <see cref="Guid"/> as item.
		/// </summary>
		public virtual bool Replace(T item)
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
		/// Returns the first item within the list that has the specified <see cref="Guid"/>,
		/// <c>null</c>  if no item with the specified <see cref="Guid"/> exists.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual T Find(Guid guid)
		{
			foreach (T item in this)
			{
				if (item.Guid == guid)
					return (item);
			}

			return (default(T));
		}

		/// <summary>
		/// Removes the first item within the list that has the specified <see cref="Guid"/>,
		/// and returns it, <c>null</c> if no item with the specified <see cref="Guid"/> exists.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public virtual T Remove(Guid guid)
		{
			T item = Find(guid);

			Remove(item);

			return (item);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
