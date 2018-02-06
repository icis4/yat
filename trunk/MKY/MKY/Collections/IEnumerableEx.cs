﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.24 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
		/// same number of elements, all elements are equally sequenced and have value equality.
		/// </summary>
		/// <remarks>
		/// This method has intentionally been called "ElementsEqual()"...
		/// ...for similar naming as <see cref="object.ReferenceEquals(object, object)"/> and...
		/// ...to emphasize difference to "IEnumerable.Equals()" which is just "object.Equals()".
		/// </remarks>
		/// <returns>
		/// True if enumerable objects have value equality, otherwise false.
		/// </returns>
		public static bool ElementsEqual(IEnumerable objA, IEnumerable objB)
		{
			if (ReferenceEquals(objA, objB)) return (true);
			if (ReferenceEquals(objA, null)) return (false);
			if (ReferenceEquals(objB, null)) return (false);

			// Opposed to ArrayEx.ElementsEqual(), this IEnumerable based implementation
			// cannot compare the count of the objects.

			return (IEnumeratorEx.ElementsEqual(objA.GetEnumerator(), objB.GetEnumerator()));
		}

		/// <summary>
		/// Appends all elements of an enumerable object to a string and returns the string.
		/// Elements that are <c>null</c> are returned as "[null]".
		/// </summary>
		/// <returns>
		/// String containing all elements.
		/// </returns>
		public static string ElementsToString(IEnumerable value)
		{
			// Attention:
			// Similar code exists in ArrayEx.ElementsToString().
			// Changes here may have to be applied there too.

			var sb = new StringBuilder();

			bool firstElement = true;
			foreach (object item in value)
			{
				if (firstElement)
					firstElement = false;
				else
					sb.Append(", ");

				if (item != null)
					sb.Append(item.ToString());
				else
					sb.Append("[null]");
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Serves as a hash function that iterates over all items within the given collection.
		/// </summary>
		public static int ElementsToHashCode(IEnumerable value)
		{
			// Attention:
			// Similar code exists in ArrayEx.ElementsToHashCode().
			// Changes here may have to be applied there too.

			int hashCode = 0;

			foreach (object item in value)
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
