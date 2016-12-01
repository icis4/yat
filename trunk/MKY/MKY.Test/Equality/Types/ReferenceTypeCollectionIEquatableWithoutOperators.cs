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
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using MKY.Diagnostics;

namespace MKY.Test.Equality.Types
{
	/// <typeparam name="T">The type the verify against.</typeparam>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Naming shall be consistent throughout this test module.")]
	public class ReferenceTypeCollectionIEquatableWithoutOperators<T> : List<T>, IEquatable<ReferenceTypeCollectionIEquatableWithoutOperators<T>>
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
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ReferenceTypeCollectionIEquatableWithoutOperators<T>));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(ReferenceTypeCollectionIEquatableWithoutOperators<T> other)
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

				for (int i = 0; i < Count; i++)
				{
					if (!ObjectEx.Equals(this[i], other[i]))
					{
						Trace.WriteLine("Results in 'False' since elements don't equal");
						Trace.Unindent();
						return (false);
					}
				}

				Trace.WriteLine("Results in True");
				Trace.Unindent();
				return (true);
			}
			else // Normal implementation:
			{
				if (Count != other.Count)
					return (false);

				for (int i = 0; i < Count; i++)
				{
					if (!ObjectEx.Equals(this[i], other[i]))
						return (false);
				}

				return (true);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
