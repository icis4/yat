using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Array utility methods.
	/// </summary>
	public static class XArray
	{
		/// <summary>
		/// Determines whether the two arrays have value equality, i.e. all array elements have
		/// value equality.
		/// </summary>
		public static bool ValueEquals(Array objA, Array objB)
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
	}
}
