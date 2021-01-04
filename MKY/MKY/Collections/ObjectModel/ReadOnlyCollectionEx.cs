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
// MKY Version 1.0.28 Development
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

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="ReadOnlyCollection{T}"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ReadOnlyCollectionEx
	{
		/// <summary>
		/// Determines whether the two collections have value equality, i.e. all array items have
		/// value equality.
		/// </summary>
		/// <remarks>
		/// This method has intentionally been called "ItemsEqual()"...
		/// ...for similar naming as <see cref="object.ReferenceEquals(object, object)"/> and...
		/// ...to emphasize difference to "ReadOnlyCollection.Equals()" which is just "object.Equals()".
		/// </remarks>
		/// <returns>
		/// True if collections have value equality, otherwise false.
		/// </returns>
		/// <typeparam name="T">The type of the array's items.</typeparam>
		public static bool ItemsEqual<T>(ReadOnlyCollection<T> collectionA, ReadOnlyCollection<T> collectionB)
		{
			if (ReferenceEquals(collectionA, collectionB)) return (true);
			if (ReferenceEquals(collectionA, null))        return (false);
			if (ReferenceEquals(collectionB, null))        return (false);

			if (collectionA.Count != collectionB.Count)
				return (false);

			return (IEnumeratorEx.ItemsEqual(collectionA.GetEnumerator(), collectionB.GetEnumerator()));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
