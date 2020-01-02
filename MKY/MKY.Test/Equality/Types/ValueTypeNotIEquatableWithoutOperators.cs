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
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.Test.Equality.Types
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "This test class intentionally doesn't have equality operators.")]
	internal struct ValueTypeNotIEquatableWithoutOperators
	{
		/// <summary></summary>
		public int B { get; } // = 'Base'

		/// <summary></summary>
		public ValueTypeNotIEquatableWithoutOperators(int b)
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
			return ("B = " + B.ToString(CultureInfo.CurrentCulture));
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
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", "CA2231:OverloadOperatorEqualsOnOverridingValueTypeEquals", Justification = "This test class intentionally doesn't have equality operators.")]
		public override bool Equals(object obj)
		{
			if (obj is ValueTypeNotIEquatableWithoutOperators)
				return (B.Equals(((ValueTypeNotIEquatableWithoutOperators)obj).B));
			else
				return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
