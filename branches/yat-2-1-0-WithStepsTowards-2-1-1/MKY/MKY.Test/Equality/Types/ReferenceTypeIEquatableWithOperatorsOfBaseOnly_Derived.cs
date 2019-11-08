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
	internal class ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived : ReferenceTypeIEquatableWithOperators_Base, IEquatable<ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived>
	{
		/// <summary></summary>
		public int D { get; } // = 'Derived'

		/// <summary></summary>
		public ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(int b, int d)
			: base(b)
		{
			D = d;
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
			return
			(
				Environment.NewLine + "      1:Base = " + base.ToString() +
				Environment.NewLine + "      1:D    = " + D.ToString(CultureInfo.CurrentCulture)
			);
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
			return (base.GetHashCode() ^ B.GetHashCode());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived other)
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

				bool result = (base.Equals(other) && D.Equals(other.D));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);
			}
			else // Normal implementation:
			{
				if (ReferenceEquals(other, null)) return (false);
				if (ReferenceEquals(this, other)) return (true);
				if (GetType() != other.GetType()) return (false);

				return (base.Equals(other) && D.Equals(other.D));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
