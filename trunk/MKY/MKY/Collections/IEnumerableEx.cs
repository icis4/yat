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
// MKY Version 1.0.29
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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.Collections
{
	/// <summary>
	/// <see cref="IEnumerable"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class IEnumerableEx
	{
		/// <summary>
		/// Determines whether the two enumerable objects have value equality, i.e. contains the
		/// same number of items, all items are equally sequenced and have value equality.
		/// </summary>
		/// <remarks>
		/// This method has intentionally been called "ItemsEqual()"...
		/// ...for similar naming as <see cref="object.ReferenceEquals(object, object)"/> and...
		/// ...to emphasize difference to "IEnumerable.Equals()" which is just "object.Equals()".
		/// </remarks>
		/// <returns>
		/// True if enumerable objects have value equality, otherwise false.
		/// </returns>
		public static bool ItemsEqual(IEnumerable objA, IEnumerable objB)
		{
			if (ReferenceEquals(objA, objB)) return (true);
			if (ReferenceEquals(objA, null)) return (false);
			if (ReferenceEquals(objB, null)) return (false);

			// Opposed to ArrayEx.ItemsEqual(), this IEnumerable based implementation
			// cannot compare the count of the objects.

			return (IEnumeratorEx.ItemsEqual(objA.GetEnumerator(), objB.GetEnumerator()));
		}

		/// <summary>
		/// Appends all items of an enumerable object to a comma separated string and returns it.
		/// Items that are <c>null</c> are returned as "(null)".
		/// An empty <paramref name="collection"/> is returned as "(empty)".
		/// </summary>
		/// <returns>
		/// String containing values of all items.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string ItemsToString(IEnumerable collection, string itemEnclosure = null)
		{
			// Attention:
			// Similar code exists in ArrayEx.ValuesToString().
			// Changes here may have to be applied there too.

			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (object item in collection)
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(", ");

				if (!string.IsNullOrEmpty(itemEnclosure))
					sb.Append(itemEnclosure);

				if (item != null)
					sb.Append(item.ToString());
				else
					sb.Append("(null)");

				if (!string.IsNullOrEmpty(itemEnclosure))
					sb.Append(itemEnclosure);
			}

			if (isFirst) // i.e. no items.
				sb.Append("(empty)");

			return (sb.ToString());
		}

		/// <summary>
		/// Appends all items of an enumerable object to a comma separated string and returns it.
		/// Items that are <c>null</c> are returned as "(null)".
		/// </summary>
		/// <returns>
		/// String containing values of all items.
		/// </returns>
		public static string ItemsToString(IEnumerable collection, char itemEnclosure)
		{
			// Attention:
			// Similar code exists in ArrayEx.ValuesToString().
			// Changes here may have to be applied there too.

			return (ItemsToString(collection, itemEnclosure.ToString()));
		}

		/// <summary>
		/// Serves as a hash function that iterates over all items within the given collection.
		/// </summary>
		public static int ItemsToHashCode(IEnumerable collection)
		{
			// Attention:
			// Similar code exists in ArrayEx.ValuesToHashCode().
			// Changes here may have to be applied there too.

			int hashCode = 0;

			foreach (object item in collection)
			{
				unchecked
				{
					hashCode = (hashCode * 397) ^ (item != null ? item.GetHashCode() : 0);
				}
			}

			return (hashCode);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
