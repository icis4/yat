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
// MKY Development Version 1.0.18
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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.Collections
{
	/// <summary>
	/// Array utility methods.
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
		/// 
		/// Attention:
		/// Similar code also exists in <see cref="ArrayEx"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <returns>
		/// True if arrays have value equality, otherwise false.
		/// </returns>
		public static bool ElementsEqual(IEnumerable objA, IEnumerable objB)
		{
			if (ReferenceEquals(objA, objB)) return (true);
			if (ReferenceEquals(objA, null)) return (false);
			if (ReferenceEquals(objB, null)) return (false);

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

		/// <summary>
		/// Appends all elements of an enumerable object to a string and returns the string.
		/// Elements that are <c>null</c> are returned as [null].
		/// </summary>
		/// <remarks>
		/// Attention:
		/// Similar code also exists in <see cref="ArrayEx"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <returns>
		/// String containing all elements.
		/// </returns>
		public static string ElementsToString(IEnumerable array)
		{
			var sb = new StringBuilder();

			bool firstElement = true;
			foreach (object obj in array)
			{
				if (firstElement)
					firstElement = false;
				else
					sb.Append(", ");

				if (obj != null)
					sb.Append(obj.ToString());
				else
					sb.Append("[null]");
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
