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
// MKY Version 1.0.15
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether the calling sequence should be output onto the console:
// - Uncomment output calling sequence
// - Comment out for normal operation
//#define OUTPUT_CALLING_SEQUENCE

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using NUnit.Framework;

#endregion

namespace MKY.Test
{
	/// <summary>
	/// Difficulties with equality:
	/// a) Reference types need to deal with value equality on all virtual/inheritance levels.
	///    Overriding Equals() provides this functionality.
	/// b) object.operators == and != are not virtual by default and therefore cannot take
	///    value equality of reference types into account.
	/// c) <c>null</c> must be handled.
	/// d) <see cref="T:IEquatable`T"/> should be implemented to improve performance.
	/// e) operators (==/!=/...) cannot be applied within template methods.
	/// </summary>
	public static class EqualityAnalysisData
	{
		private const string UnusedParameterSuppressionJustification = "These test methods all have the same three parameters: Testee, Equal and NotEqual counterpart.";

		#region Value Types
		//==========================================================================================
		// Value Types
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public struct ValueTypeWithOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public ValueTypeWithOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return ("A = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("ValueTypeWithOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (ValueTypeWithOperators)obj;
				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (ValueTypeWithOperators)obj;
				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(ValueTypeWithOperators lhs, ValueTypeWithOperators rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("ValueTypeWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Value type implementation of operator ==.
				// See MKY.Test.EqualityAnalysis for details.

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				return (lhs.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(ValueTypeWithOperators lhs, ValueTypeWithOperators rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "This test class intentionally doesn't have equality operators.")]
		public struct ValueTypeWithoutOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public ValueTypeWithoutOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return ("A = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			[SuppressMessage("Microsoft.Usage", "CA2231:OverloadOperatorEqualsOnOverridingValueTypeEquals", Justification = "This test class intentionally doesn't have equality operators.")]
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("ValueTypeWithoutOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (ValueTypeWithoutOperators)obj;
				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (ValueTypeWithoutOperators)obj;
				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion
		}

		#endregion

		#region Reference Types
		//==========================================================================================
		// Reference Types
		//==========================================================================================

		/// <summary></summary>
		/// <typeparam name="T">The type the verify against.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Naming shall be consistent thoughout this test module.")]
		public class OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<T> : List<T>
			where T : IEquatable<T>
		{
		}

		/// <summary></summary>
		/// <typeparam name="T">The type the verify against.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Naming shall be consistent thoughout this test module.")]
		public class OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T> : List<T>, IEquatable<OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T>>
			where T : IEquatable<T>
		{
			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString()
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T>));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for this equality test.")]
			public virtual bool Equals(OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T> other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T>.Equals<OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<T>>()");

				bool result = (base.Equals(other) && (B == other.B));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Dedicated list implementation:

				if (Count != other.Count)
					return (false);

				for (int i = 0; i < Count; i++)
				{
					if (!this[i].Equals(other[i])) // <T> might not overload the ==/!= operators.
						return (false);
				}

				return (true);

			#endif
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
				return (base.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		/// <typeparam name="T">The type the verify against.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Naming shall be consistent thoughout this test module.")]
		public class OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> : List<T>, IEquatable<OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T>>
			where T : IEquatable<T>
		{
			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString()
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T>));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for this equality test.")]
			public virtual bool Equals(OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T>.Equals<OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T>>()");

				bool result = (base.Equals(other) && (B == other.B));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Dedicated list implementation:

				if (Count != other.Count)
					return (false);

				for (int i = 0; i < Count; i++)
				{
					if (!this[i].Equals(other[i])) // <T> might not overload the ==/!= operators.
						return (false);
				}

				return (true);

			#endif
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
				return (base.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for this equality test.")]
			public static bool operator ==(OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> lhs, OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T>.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				return (lhs.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for this equality test.")]
			public static bool operator !=(OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> lhs, OwnDerivedCollectionReferenceTypeIEquatableWithOperators<T> rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class BaseReferenceTypeNotIEquatableWithoutOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public BaseReferenceTypeNotIEquatableWithoutOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "    0:A    = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (BaseReferenceTypeNotIEquatableWithoutOperators)obj;
				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (BaseReferenceTypeNotIEquatableWithoutOperators)obj;
				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeNotIEquatableWithoutOperators : BaseReferenceTypeNotIEquatableWithoutOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int B;

			/// <summary></summary>
			public DerivedReferenceTypeNotIEquatableWithoutOperators(int a, int b)
				: base(a)
			{
				B = b;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString() +
					Environment.NewLine + "      1:B    = " + B.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Trace.WriteLine("Results in False within base.Equals()");
					Trace.Unindent();
					return (false);
				}

				var other = (obj as DerivedReferenceTypeNotIEquatableWithoutOperators);
				bool result = (B == other.B);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				var other = (obj as DerivedReferenceTypeNotIEquatableWithoutOperators);
				return (B == other.B);

			#endif
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
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class BaseReferenceTypeIEquatableWithoutOperators : IEquatable<BaseReferenceTypeIEquatableWithoutOperators>
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public BaseReferenceTypeIEquatableWithoutOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "    0:A    = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as BaseReferenceTypeIEquatableWithoutOperators));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public virtual bool Equals(BaseReferenceTypeIEquatableWithoutOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeIEquatableWithoutOperators.Equals<BaseReferenceTypeIEquatableWithoutOperators>()");

				if (ReferenceEquals(other, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != other.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(other, null))
					return (false);

				if (GetType() != other.GetType())
					return (false);

				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeIEquatableWithoutOperators : BaseReferenceTypeIEquatableWithoutOperators, IEquatable<DerivedReferenceTypeIEquatableWithoutOperators>
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int B;

			/// <summary></summary>
			public DerivedReferenceTypeIEquatableWithoutOperators(int a, int b)
				: base(a)
			{
				B = b;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString() +
					Environment.NewLine + "      1:B    = " + B.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as DerivedReferenceTypeIEquatableWithoutOperators));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public virtual bool Equals(DerivedReferenceTypeIEquatableWithoutOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeIEquatableWithoutOperators.Equals<DerivedReferenceTypeIEquatableWithoutOperators>()");

				bool result = (base.Equals(other) && (B == other.B));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				return (base.Equals(other) && (B == other.B));

			#endif
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

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class BaseReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public BaseReferenceTypeNotIEquatableWithOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "    0:A    = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeNotIEquatableWithOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (BaseReferenceTypeNotIEquatableWithOperators)obj;
				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (BaseReferenceTypeNotIEquatableWithOperators)obj;
				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(BaseReferenceTypeNotIEquatableWithOperators lhs, BaseReferenceTypeNotIEquatableWithOperators rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeNotIEquatableWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);


			#else

				// Normal implementation:

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				return (lhs.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(BaseReferenceTypeNotIEquatableWithOperators lhs, BaseReferenceTypeNotIEquatableWithOperators rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeNotIEquatableWithOperators : BaseReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int B;

			/// <summary></summary>
			public DerivedReferenceTypeNotIEquatableWithOperators(int a, int b)
				: base(a)
			{
				B = b;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString() +
					Environment.NewLine + "      1:B    = " + B.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeNotIEquatableWithOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Trace.WriteLine("Results in False within base.Equals()");
					Trace.Unindent();
					return (false);
				}

				var other = (obj as DerivedReferenceTypeNotIEquatableWithOperators);
				bool result = (B == other.B);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				var other = (obj as DerivedReferenceTypeNotIEquatableWithOperators);
				return (B == other.B);

			#endif
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
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(DerivedReferenceTypeNotIEquatableWithOperators lhs, DerivedReferenceTypeNotIEquatableWithOperators rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeNotIEquatableWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				return (lhs.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(DerivedReferenceTypeNotIEquatableWithOperators lhs, DerivedReferenceTypeNotIEquatableWithOperators rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeNotIEquatableWithDerivedOperators : DerivedReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int C;

			/// <summary></summary>
			public DerivedReferenceTypeNotIEquatableWithDerivedOperators(int a, int b, int c)
				: base(a, b)
			{
				C = c;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "        2:Base = " + base.ToString() +
					Environment.NewLine + "        2:C    = " + this.C.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Trace.WriteLine("Results in False within base.Equals()");
					Trace.Unindent();
					return (false);
				}

				var other = (obj as DerivedReferenceTypeNotIEquatableWithDerivedOperators);
				bool result = (B == other.B);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				var other = (obj as DerivedReferenceTypeNotIEquatableWithDerivedOperators);
				return (B == other.B);

			#endif
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
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class BaseReferenceTypeNotIEquatableWithBaseOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public BaseReferenceTypeNotIEquatableWithBaseOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "    0:A    = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeNotIEquatableWithBaseOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (BaseReferenceTypeNotIEquatableWithBaseOperators)obj;
				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (BaseReferenceTypeNotIEquatableWithBaseOperators)obj;
				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(BaseReferenceTypeNotIEquatableWithBaseOperators lhs, BaseReferenceTypeNotIEquatableWithBaseOperators rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeNotIEquatableWithBaseOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				// Ensure that potiential <Derived>.Equals() is called.
				// Thus, ensure that object.Equals() is called.
				object obj = (object)lhs;
				bool result = obj.Equals(rhs);
				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Base reference type implementation of operator ==.
				// See MKY.Test.EqualityAnalysis for details.

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				// Ensure that potiential <Derived>.Equals() is called.
				// Thus, ensure that object.Equals() is called.
				object obj = (object)lhs;
				return (obj.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(BaseReferenceTypeNotIEquatableWithBaseOperators lhs, BaseReferenceTypeNotIEquatableWithBaseOperators rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeNotIEquatableWithBaseOperators : BaseReferenceTypeNotIEquatableWithBaseOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int B;

			/// <summary></summary>
			public DerivedReferenceTypeNotIEquatableWithBaseOperators(int a, int b)
				: base(a)
			{
				B = b;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString() +
					Environment.NewLine + "      1:B    = " + B.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeNotIEquatableWithBaseOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Trace.WriteLine("Results in False within base.Equals()");
					Trace.Unindent();
					return (false);
				}

				var other = (obj as DerivedReferenceTypeNotIEquatableWithBaseOperators);
				bool result = (B == other.B);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				var other = (obj as DerivedReferenceTypeNotIEquatableWithBaseOperators);
				return (B == other.B);

			#endif
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
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class BaseReferenceTypeIEquatableWithBaseOperators : IEquatable<BaseReferenceTypeIEquatableWithBaseOperators>
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int A;

			/// <summary></summary>
			public BaseReferenceTypeIEquatableWithBaseOperators(int a)
			{
				A = a;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return (Environment.NewLine + "    0:A    = " + A.ToString(CultureInfo.InvariantCulture));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as BaseReferenceTypeIEquatableWithBaseOperators));
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public virtual bool Equals(BaseReferenceTypeIEquatableWithBaseOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeIEquatableWithBaseOperators.Equals<BaseReferenceTypeIEquatableWithBaseOperators>()");

				if (ReferenceEquals(other, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != other.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				bool result = (A == other.A);

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(other, null))
					return (false);

				if (GetType() != other.GetType())
					return (false);

				return (A == other.A);

			#endif
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
				return (A.GetHashCode());
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(BaseReferenceTypeIEquatableWithBaseOperators lhs, BaseReferenceTypeIEquatableWithBaseOperators rhs)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("BaseReferenceTypeIEquatableWithBaseOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Trace.WriteLine("Results in True within ReferenceEquals()");
					Trace.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Trace.WriteLine("Results in False since lhs is null");
					Trace.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Trace.WriteLine("Results in False since rhs is null");
					Trace.Unindent();
					return (false);
				}

				// Ensure that potiential <Derived>.Equals() is called.
				// Thus, ensure that object.Equals() is called.
				object obj = (object)lhs;
				bool result = obj.Equals(rhs);
				Trace.WriteLine("Results in " + result + " within Equals()");
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Base reference type implementation of operator ==.
				// See MKY.Test.EqualityAnalysis for details.

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				// Ensure that potiential <Derived>.Equals() is called.
				// Thus, ensure that object.Equals() is called.
				object obj = (object)lhs;
				return (obj.Equals(rhs));

			#endif
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(BaseReferenceTypeIEquatableWithBaseOperators lhs, BaseReferenceTypeIEquatableWithBaseOperators rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This class really belongs to the test data only.")]
		public class DerivedReferenceTypeDerivedIEquatableWithBaseOperators : BaseReferenceTypeIEquatableWithBaseOperators
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
			public readonly int B;

			/// <summary></summary>
			public DerivedReferenceTypeDerivedIEquatableWithBaseOperators(int a, int b)
				: base(a)
			{
				B = b;
			}

			#region Object Members

			/// <summary></summary>
			public override string ToString()
			{
				return
				(
					Environment.NewLine + "      1:Base = " + base.ToString() +
					Environment.NewLine + "      1:B    = " + B.ToString(CultureInfo.InvariantCulture)
				);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Trace.Indent();
				Trace.WriteLine("DerivedReferenceTypeDerivedIEquatableWithBaseOperators.Equals<DerivedReferenceTypeDerivedIEquatableWithBaseOperators>()");

				if (ReferenceEquals(obj, null))
				{
					Trace.WriteLine("Results in False since other is null");
					Trace.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Trace.WriteLine("Results in False since types don't match");
					Trace.Unindent();
					return (false);
				}

				var other = (DerivedReferenceTypeDerivedIEquatableWithBaseOperators)obj;
				bool result = (base.Equals(other) && (B == other.B));

				Trace.WriteLine("Results in " + result);
				Trace.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				var other = (DerivedReferenceTypeDerivedIEquatableWithBaseOperators)obj;
				return (base.Equals(other) && (B == other.B));

			#endif
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
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCasesForBase
		{
			get
			{
				yield return (new TestCaseData(1, 1, 2).SetName("Base"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesForDerived
		{
			get
			{
				yield return (new TestCaseData(1, 1, 1, 1, 2, 2).SetName("Equal : Base == Derived / Not Equal : Base == Derived"));

				yield return (new TestCaseData(1, 1, 1, 1, 1, 2).SetName("Equal : Base == Derived / Not Equal : Base != Derived / A"));
				yield return (new TestCaseData(1, 1, 1, 1, 2, 1).SetName("Equal : Base == Derived / Not Equal : Base != Derived / B"));

				yield return (new TestCaseData(1, 2, 1, 2, 1, 1).SetName("Equal : Base != Derived / Not Equal : Base == Derived / AA"));
				yield return (new TestCaseData(1, 2, 1, 2, 2, 2).SetName("Equal : Base != Derived / Not Equal : Base == Derived / AB"));
				yield return (new TestCaseData(2, 1, 2, 1, 1, 1).SetName("Equal : Base != Derived / Not Equal : Base == Derived / BA"));
				yield return (new TestCaseData(2, 1, 2, 1, 2, 2).SetName("Equal : Base != Derived / Not Equal : Base == Derived / BB"));

				yield return (new TestCaseData(1, 2, 1, 2, 2, 1).SetName("Equal : Base != Derived / Not Equal : Base != Derived / A"));
				yield return (new TestCaseData(2, 1, 2, 1, 1, 2).SetName("Equal : Base != Derived / Not Equal : Base != Derived / B"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCasesForDerivedDerived
		{
			get
			{
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 3, 3, 3).SetName("Equal : Base == Derived / Not Equal : Base == Derived"));

				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 1, 2, 3).SetName("Equal : Base == Derived / Not Equal : Base != Derived / A"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 3, 2, 1).SetName("Equal : Base == Derived / Not Equal : Base != Derived / B"));

				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 1, 1, 1).SetName("Equal : Base != Derived / Not Equal : Base == Derived / AA"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 3, 3).SetName("Equal : Base != Derived / Not Equal : Base == Derived / AB"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 1, 1, 1).SetName("Equal : Base != Derived / Not Equal : Base == Derived / BA"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 3, 3, 3).SetName("Equal : Base != Derived / Not Equal : Base == Derived / BB"));

				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 2, 1).SetName("Equal : Base != Derived / Not Equal : Base != Derived / A"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 1, 2, 3).SetName("Equal : Base != Derived / Not Equal : Base != Derived / B"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only displays the general functionality of Equals() and ==/!=. It does neither perform any tests nor tests any MKY functionality.")]
	public class DisplayEquality
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void DisplaySequenceOfEqualityOfOwnReferenceType()
		{
			Trace.Indent();
			Trace.WriteLine("");
			Trace.WriteLine("Base without operators");
			{
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators a = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators(1);
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators b = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators a = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators(1, 2);
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators b = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base IEquatable without operators");
			{
				EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators a = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators(1);
				EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators b = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived IEquatable without operators");
			{
				EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators a = new EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators(1, 2);
				EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators b = new EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with operators");
			{
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators a = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators(1);
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators b = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with operators");
			{
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators a = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators(1, 2);
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators b = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators a = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(1, 2, 3);
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators b = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(1, 2, 3);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with base operators");
			{
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators a = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators(1);
				EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators b = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with base operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with base operators");
			{
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators a = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators(1, 2);
				EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators b = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with base operators results in " + result);
			}
			Trace.Unindent();
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class EqualityAnalysisOfValueTypes
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemValueType()
		{
			DateTime objToCompareAgainst = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objEqual            = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objNotEqual         = new DateTime(2001, 2, 3, 13, 4, 5);

			EqualityTestMethods.TestEquals<object>  (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<DateTime>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDateTime (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDateTime     (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeWithoutOperators()
		{
			EqualityAnalysisData.ValueTypeWithoutOperators objToCompareAgainst = new EqualityAnalysisData.ValueTypeWithoutOperators(1);
			EqualityAnalysisData.ValueTypeWithoutOperators objEqual            = new EqualityAnalysisData.ValueTypeWithoutOperators(1);
			EqualityAnalysisData.ValueTypeWithoutOperators objNotEqual         = new EqualityAnalysisData.ValueTypeWithoutOperators(2);

			EqualityTestMethods.TestEquals<object>                                    (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.ValueTypeWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			//// Operator ==/!= cannot be directly applied to value types without operators,
			////   neither to evaluate reference nor value equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeWithOperators()
		{
			EqualityAnalysisData.ValueTypeWithOperators objToCompareAgainst = new EqualityAnalysisData.ValueTypeWithOperators(1);
			EqualityAnalysisData.ValueTypeWithOperators objEqual            = new EqualityAnalysisData.ValueTypeWithOperators(1);
			EqualityAnalysisData.ValueTypeWithOperators objNotEqual         = new EqualityAnalysisData.ValueTypeWithOperators(2);

			EqualityTestMethods.TestEquals<object>                                 (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.ValueTypeWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType             (objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueTypeWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfValueTypeWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class EqualityAnalysisOfReferenceTypesWithoutOperators
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemCollectionReferenceTypeNotIEquatableWithoutOperators()
		{
			List<int> objToCompareAgainst = new List<int>(2); // Preset the required capacity to improve memory management.
			List<int> objEqual            = new List<int>(2); // Preset the required capacity to improve memory management.
			List<int> objNotEqual         = new List<int>(2); // Preset the required capacity to improve memory management.

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// List<int>.Equals() doesn't exist, and object.Equals() only evaluates reference equality.

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfList  (objToCompareAgainst, objEqual, objNotEqual);
			//// List<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators()
		{
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int> objToCompareAgainst = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int> objEqual            = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int> objNotEqual         = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.Equals() doesn't exist, and List<int>.Equals() only evaluates reference equality.

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfList  (objToCompareAgainst, objEqual, objNotEqual);
			//// EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnDerivedCollectionReferenceTypeIEquatableWithoutOperators()
		{
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int> objToCompareAgainst = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int> objEqual            = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int> objNotEqual         = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfList  (objToCompareAgainst, objEqual, objNotEqual);
			//// EqualityTestData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForBase")]
		public virtual void AnalyzeOwnBaseReferenceTypeNotIEquatableWithoutOperators(int a1, int a2, int a3)
		{
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators(a1);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual            = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators(a2);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual         = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeNotIEquatableWithoutOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators(a1, b1);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators(a2, b2);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForBase")]
		public virtual void AnalyzeOwnBaseReferenceTypeIEquatableWithoutOperators(int a1, int a2, int a3)
		{
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators(a1);
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objEqual            = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators(a2);
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual         = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeIEquatableWithoutOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators(a1, b1);
			EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators(a2, b2);
			EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class EqualityAnalysisOfReferenceTypesWithOperators
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemBaseReferenceTypeIEquatableWithOperators()
		{
			Version objToCompareAgainst = new Version(1, 1);
			Version objEqual            = new Version(1, 1);
			Version objNotEqual         = new Version(1, 2);

			EqualityTestMethods.TestEquals<object> (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<Version>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject (objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfVersion(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfVersion    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnDerivedCollectionReferenceTypeIEquatableWithOperators()
		{
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objToCompareAgainst = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objEqual            = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int>();
			EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objNotEqual         = new EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int>>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForBase")]
		public virtual void AnalyzeOwnBaseReferenceTypeNotIEquatableWithOperators(int a1, int a2, int a3)
		{
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators(a1);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objEqual            = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators(a2);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual         = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeNotIEquatableWithOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators(a1, b1);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators(a2, b2);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerivedDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeNotIEquatableWithDerivedOperators(int a1, int b1, int c1, int a2, int b2, int c2, int a3, int b3, int c3)
		{
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a1, b1, c1);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a2, b2, c2);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a3, b3, c3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForBase")]
		public virtual void AnalyzeOwnBaseReferenceTypeNotIEquatableWithBaseOperators(int a1, int a2, int a3)
		{
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators(a1);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual            = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators(a2);
			EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual         = new EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeNotIEquatableWithBaseOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a1, b1);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a2, b2);
			EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForBase")]
		public virtual void AnalyzeOwnBaseReferenceTypeIEquatableWithBaseOperators(int a1, int a2, int a3)
		{
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators(a1);
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objEqual            = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators(a2);
			EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual         = new EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityAnalysisData), "TestCasesForDerived")]
		public virtual void AnalyzeOwnDerivedReferenceTypeDerivedIEquatableWithBaseOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst = new EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a1, b1);
			EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual            = new EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a2, b2);
			EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual         = new EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}

	internal static class EqualityTestMethods
	{
		private const string UnusedParameterSuppressionJustification = "These test methods all have the same three parameters: Testee, Equal and NotEqual counterpart.";

		#region Static Test Methods
		//==========================================================================================
		// Static Test Methods
		//==========================================================================================

		#region Static Test Methods > Equals<T>
		//------------------------------------------------------------------------------------------
		// Static Test Methods > Equals<T>
		//------------------------------------------------------------------------------------------

		public static void TestEquals<T>(T objToCompareAgainst, T objEqual, T objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestEquals<" + typeof(T).FullName + ">");
			Trace.Indent();

			try
			{
				// Reference equal:

				Trace.WriteLine("Reference equal using Equals()");
				Trace.Indent();

				if (!objToCompareAgainst.Equals(objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using Equals()");

				Trace.Unindent();

				// Value equal:

				Trace.WriteLine("Value equal using Equals()");
				Trace.Indent();

				if (!objToCompareAgainst.Equals(objEqual))
					Assert.Fail("Value equal objects are not considered equal using Equals()");

				Trace.Unindent();

				// Value not equal:

				Trace.WriteLine("Value not equal using Equals()");
				Trace.Indent();

				if (objToCompareAgainst.Equals(objNotEqual))
					Assert.Fail("Value not equal objects are considered equal using Equals()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		#endregion

		#region Static Test Methods > Operators for Value Types
		//------------------------------------------------------------------------------------------
		// Static Test Methods > Operators for Value Types
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfValueType(ValueType objToCompareAgainst, ValueType objEqual, ValueType objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfValueType");
			Trace.Indent();

			try
			{
				// Reference equal:

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		//                 TestOperatorsForValueEqualityOfValueType is useless since it never succeeds.

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDateTime(DateTime objToCompareAgainst, DateTime objEqual, DateTime objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDateTime");
			Trace.Indent();

			try
			{
				// Reference equal:

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDateTime(DateTime objToCompareAgainst, DateTime objEqual, DateTime objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDateTime");
			Trace.Indent();

			try
			{
				// Value equal:

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal:

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfValueTypeWithOperators(EqualityAnalysisData.ValueTypeWithOperators objToCompareAgainst, EqualityAnalysisData.ValueTypeWithOperators objEqual, EqualityAnalysisData.ValueTypeWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfValueTypeWithOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfValueTypeWithOperators(EqualityAnalysisData.ValueTypeWithOperators objToCompareAgainst, EqualityAnalysisData.ValueTypeWithOperators objEqual, EqualityAnalysisData.ValueTypeWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfValueTypeWithOperators");
			Trace.Indent();

			try
			{
				// Value equal:

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal:

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		#endregion

		#region Static Test Methods > Operators for Reference Types
		//------------------------------------------------------------------------------------------
		// Static Test Methods > Operators for Reference Types
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfObject(object objToCompareAgainst, object objEqual, object objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfObject");
			Trace.Indent();

			try
			{
				// Reference equal:

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		//                 TestOperatorsForValueEqualityOfObject is useless since it never succeeds.

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfVersion(Version objToCompareAgainst, Version objEqual, Version objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfVersion");
			Trace.Indent();

			try
			{
				// Reference equal:

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfVersion(Version objToCompareAgainst, Version objEqual, Version objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfVersion");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfList(List<int> objToCompareAgainst, List<int> objEqual, List<int> objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfList");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method exists for the sake of completeness.")]
		public static void TestOperatorsForValueEqualityOfList(List<int> objToCompareAgainst, List<int> objEqual, List<int> objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfList");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators(EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objToCompareAgainst, EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objEqual, EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators(EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objToCompareAgainst, EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objEqual, EqualityAnalysisData.OwnDerivedCollectionReferenceTypeIEquatableWithOperators<int> objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfOwnDerivedCollectionReferenceTypeIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method exists for the sake of completeness.")]
		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method exists for the sake of completeness.")]
		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators(EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objEqual, EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method exists for the sake of completeness.")]
		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithoutOperators(EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objEqual, EqualityAnalysisData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method exists for the sake of completeness.")]
		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityAnalysisData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objEqual, EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objEqual, EqualityAnalysisData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Trace.WriteLine("Reference equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Reference equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst, EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual, EqualityAnalysisData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual)
		{
			Trace.Indent();
			Trace.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators");
			Trace.Indent();

			try
			{
				// Value equal.

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal.

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Trace.Unindent();
			}
			catch (AssertionException)
			{
				Trace.Unindent();
				throw; // Re-throw!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
