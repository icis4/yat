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
// MKY Version 1.0.30
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// <see cref="Object"/>/<see cref="object"/> utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]

	public static class ObjectEx
	{
		/// <summary>
		/// -1 is specified as the 'invalid comparison' result in .NET.
		/// </summary>
		public const int InvalidComparisonResult = -1;

		/// <summary>
		/// Determines whether the specified <see cref="object"/> instances are considered equal.
		/// </summary>
		/// <remarks>
		/// This method is simply a wrapper to <see cref="object.Equals(object, object)"/>. It can
		/// e.g. be used to implement an overloaded Equals() method. Calling this method is preferred
		/// over directly calling <see cref="object.Equals(object, object)"/> or the respective base
		/// method <see cref="System.Object.Equals(object, object)"/> to prevent code check from
		/// suggesting to simply call <see cref="Equals(object, object)"/> as that could result in
		/// an unintended call stack as soon as a class overloads that method.
		///
		/// Note the logic behind <see cref="object.Equals(object, object)"/>:
		///  - If both objects represent the same object reference, it returns true.
		///  - If either or object is <c>null</c>, it returns false.
		///  - Otherwise, it calls objA.Equals(objB) and returns the result.
		/// </remarks>
		/// <param name="objA">The first <see cref="object"/> to compare.</param>
		/// <param name="objB">The second <see cref="object"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Same naming as 'object.Equals()'.")]
		public static new bool Equals(object objA, object objB)
		{
			return (object.Equals(objA, objB));
		}

		/// <summary>
		/// Returns a deep-cloned instance of the given object.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the object, must be decorated with the <see cref="SerializableAttribute"/>.
		/// </typeparam>
		public static T CreateDeepCloneOfSerializableItem<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				bf.Serialize(ms, obj);
				ms.Position = 0;
				return ((T)bf.Deserialize(ms));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
