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

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Collections
{
	/// <summary>
	/// <see cref="IEnumerator"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class IEnumeratorEx
	{
		/// <summary>
		/// Determines whether the two enumerated objects have value equality, i.e. contains the
		/// same number of items, all items are equally sequenced and have value equality.
		/// </summary>
		/// <remarks>
		/// This method has intentionally been called "ItemsEqual()"...
		/// ...for similar naming as <see cref="object.ReferenceEquals(object, object)"/> and...
		/// ...to emphasize difference to "IEnumerator.Equals()" which is just "object.Equals()".
		/// </remarks>
		/// <returns>
		/// True if enumerated objects have value equality, otherwise false.
		/// </returns>
		public static bool ItemsEqual(IEnumerator enumA, IEnumerator enumB)
		{
			while (true)
			{
				var enumAIsAtEnd = !enumA.MoveNext(); // Ensure that both enumerators are located
				var enumBIsAtEnd = !enumB.MoveNext(); // at the same position! This wouldn't be
				                                      // given in logical || or && condition!
				if         (enumAIsAtEnd || enumBIsAtEnd)
					return (enumAIsAtEnd && enumBIsAtEnd);

				if ((enumA.Current == null) && (enumB.Current == null))
					continue; // Both 'null' means that this element is equal.

				if ((enumA.Current == null) || (enumB.Current == null))
					return (false); // Only one 'null' means that enumerations are not equal.

				if (!enumA.Current.Equals(enumB.Current))
					return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
