//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Array utility methods.
	/// </summary>
	public static class ArrayEx
	{
		/// <summary>
		/// Creates an array using <see cref="Array.CreateInstance(Type, int)"/> and initializes
		/// the given number of array elements with the given initial value.
		/// </summary>
		public static T[] CreateAndInitializeInstance<T>(int length, T initalValue)
		{
			T[] a = (T[])Array.CreateInstance(typeof(T), length);

			for (int i = 0; i < length; i++)
				a[i] = initalValue;

			return (a);
		}

		/// <summary>
		/// Determines whether the two arrays have value equality, i.e. all array elements have
		/// value equality.
		/// </summary>
		/// <returns>
		/// True if arrays have value equality, otherwise false.
		/// </returns>
		public static bool ValuesEqual(Array objA, Array objB)
		{
			if (ReferenceEquals(objA, objB))
				return (true);

			if (objA.Length != objB.Length)
				return (false);

			IEnumerator objAEnumerator = objA.GetEnumerator();
			IEnumerator objBEnumerator = objB.GetEnumerator();

			// check element by element
			while (objAEnumerator.MoveNext() && (objAEnumerator.Current != null) &&
				   objBEnumerator.MoveNext() && (objBEnumerator.Current != null))
			{
				if (!objAEnumerator.Current.Equals(objBEnumerator.Current))
					return (false);
			}

			// check that both enumerators are positioned after the last element
			if (!objAEnumerator.MoveNext() && !objBEnumerator.MoveNext())
				return (true);

			return (false);
		}

		/// <summary>
		/// Appends all elements of an array to a string and returns the string.
		/// </summary>
		/// <returns>
		/// String containing all elements.
		/// </returns>
		public static string ElementsToString(Array array)
		{
			StringBuilder sb = new StringBuilder();

			bool firstElement = true;
			foreach (object obj in array)
			{
				if (firstElement)
					firstElement = false;
				else
					sb.Append(", ");

				sb.Append(obj.ToString());
			}

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
