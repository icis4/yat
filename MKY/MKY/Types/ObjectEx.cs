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
// MKY Version 1.0.17
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

using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Object utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ObjectEx
	{
		/// <summary>
		/// An invalid comparison result is represented by -1.
		/// </summary>
		public const int InvalidComparisonResult = -1;

		/// <summary>
		/// Compares two specified objects.
		/// </summary>
		/// <remarks>
		/// This method is simply a wrapper to <see cref="object.Equals(object, object)"/>. It can
		/// be used to implement an overloaded Equals() method. Calling this method is preferred
		/// over directly calling <see cref="object.Equals(object, object)"/> or the respective base
		/// method <see cref="System.Object.Equals(object, object)"/> to prevent code check from
		/// suggesting to simply call <see cref="Equals(object, object)"/> as that could result in
		/// an unintended call stack as soon as a class overloads that method.
		/// </remarks>
		public static new bool Equals(object objA, object objB)
		{
			return (object.Equals(objA, objB));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
