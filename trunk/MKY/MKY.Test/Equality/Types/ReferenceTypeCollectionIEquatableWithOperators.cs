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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

using MKY.Collections;
using MKY.Diagnostics;

namespace MKY.Test.Equality.Types
{
	/// <typeparam name="T">The type the verify against.</typeparam>
	internal class ReferenceTypeCollectionIEquatableWithOperators<T> : List<T>, IEquatable<ReferenceTypeCollectionIEquatableWithOperators<T>>
	{
		#region Object Members
		//======================================================================================
		// Object Members
		//======================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return
			(
				Environment.NewLine + "      1:Base = " + base.ToString()
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ReferenceTypeCollectionIEquatableWithOperators<T>));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(ReferenceTypeCollectionIEquatableWithOperators<T> other)
		{
			if (Configuration.TraceCallingSequence) // Trace the calling sequence:
			{
				Trace.Indent();
				TraceEx.WriteLocation();

				if (Count != other.Count)
				{
					Trace.WriteLine("Results in 'False' since count doesn't match");
					Trace.Unindent();
					return (false);
				}

				if (!IEnumerableEx.ItemsEqual(this, other))
				{
					Trace.WriteLine("Results in 'False' since items don't equal");
					Trace.Unindent();
					return (false);
				}

				Trace.WriteLine("Results in True");
				Trace.Unindent();
				return (true);
			}
			else // Normal implementation:
			{
				if (Count != other.Count)
					return (false);

				return (IEnumerableEx.ItemsEqual(this, other));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ReferenceTypeCollectionIEquatableWithOperators<T> lhs, ReferenceTypeCollectionIEquatableWithOperators<T> rhs)
		{
			if (Configuration.TraceCallingSequence) // Trace the calling sequence:
			{
				Trace.Indent();
				TraceEx.WriteLocation();

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("ReferenceEquals() results in 'True'");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("ReferenceEquals() results in 'False' since 'lhs' is 'null'");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("ReferenceEquals() results in 'False' since 'rhs' is 'null'");
					Trace.Unindent();
					return (false);
				}

				object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
				bool result = obj.Equals(rhs); // that the virtual <Derived>.Equals() is called.
				Trace.WriteLine("Equals() results in " + result);
				Trace.Unindent();
				return (result);
			}
			else // Normal implementation:
			{
				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
				return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(ReferenceTypeCollectionIEquatableWithOperators<T> lhs, ReferenceTypeCollectionIEquatableWithOperators<T> rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
