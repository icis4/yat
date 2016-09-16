//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Collections.ObjectModel
{
	/// <summary>
	/// ReadOnlyCollection utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ReadOnlyCollectionEx
	{
		/// <summary>
		/// Determines whether the two arrays have value equality, i.e. all array elements have
		/// value equality.
		/// </summary>
		/// <returns>
		/// True if arrays have value equality, otherwise false.
		/// </returns>
		/// <typeparam name="T">The type of the array's items.</typeparam>
		public static bool ValuesEqual<T>(ReadOnlyCollection<T> objA, ReadOnlyCollection<T> objB)
		{
			if (ReferenceEquals(objA, objB)) return (true);
			if (ReferenceEquals(objA, null)) return (false);
			if (ReferenceEquals(objB, null)) return (false);

			if (objA.Count != objB.Count)
				return (false);

			IEnumerator objAEnumerator = objA.GetEnumerator();
			IEnumerator objBEnumerator = objB.GetEnumerator();

			// Check element by element:
			while (objAEnumerator.MoveNext() && objBEnumerator.MoveNext())
			{
				if ((objAEnumerator.Current == null) && (objBEnumerator.Current == null))
					continue; // Both 'null' is equal.

				if ((objAEnumerator.Current == null) || (objBEnumerator.Current == null))
					return (false); // Only one 'null' is not equal.

				if (!objAEnumerator.Current.Equals(objBEnumerator.Current))
					return (false);
			}

			// Check that both enumerators are positioned after the last element:
			if (!objAEnumerator.MoveNext() && !objBEnumerator.MoveNext())
				return (true);
			else
				return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
