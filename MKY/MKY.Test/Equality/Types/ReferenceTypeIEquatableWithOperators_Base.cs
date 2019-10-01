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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Globalization;

using MKY.Diagnostics;

namespace MKY.Test.Equality.Types
{
	/// <summary></summary>
	internal class ReferenceTypeIEquatableWithOperators_Base : IEquatable<ReferenceTypeIEquatableWithOperators_Base>
	{
		/// <summary></summary>
		public int B { get; } // = 'Base'

		/// <summary></summary>
		public ReferenceTypeIEquatableWithOperators_Base(int b)
		{
			B = b;
		}

		#region Object Members
		//======================================================================================
		// Object Members
		//======================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (Environment.NewLine + "    0:B    = " + B.ToString(CultureInfo.CurrentCulture));
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return (B.GetHashCode());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ReferenceTypeIEquatableWithOperators_Base));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ReferenceTypeIEquatableWithOperators_Base other)
		{
			if (Configuration.TraceCallingSequence) // Trace the calling sequence:
			{
				Trace.Indent();
				TraceEx.WriteLocation();

				if (ReferenceEquals(other, null))
				{
					Trace.WriteLine("ReferenceEquals() results in 'False' since 'other' is 'null'");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(this, other))
				{
					Trace.WriteLine("ReferenceEquals() results in 'True'");
					Trace.Unindent();
					return (true);
				}

				if (GetType() != other.GetType())
				{
					Trace.WriteLine("Type comparison results in 'False'");
					Trace.Unindent();
					return (false);
				}

				bool result = (B.Equals(other.B));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);
			}
			else // Normal implementation:
			{
				if (ReferenceEquals(other, null)) return (false);
				if (ReferenceEquals(this, other)) return (true);
				if (GetType() != other.GetType()) return (false);

				return (B.Equals(other.B));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ReferenceTypeIEquatableWithOperators_Base lhs, ReferenceTypeIEquatableWithOperators_Base rhs)
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
		public static bool operator !=(ReferenceTypeIEquatableWithOperators_Base lhs, ReferenceTypeIEquatableWithOperators_Base rhs)
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
