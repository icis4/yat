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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether the calling sequence should be output onto the console:
// - Uncomment output calling sequence
// - Comment out for normal operation
//#define OUTPUT_CALLING_SEQUENCE

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics;

using NUnit.Framework;

#endregion

namespace MKY.System.Test
{
	/// <summary>
	/// Difficulties with equality:
	/// a) Reference types need to deal with value equality on all virtual/inheritance levels.
	///    Overriding Equals() provides this functionality.
	/// b) object.operators == and != or not virtual by default and therefore cannot take
	///    value equality of reference types into account.
	/// c) <c>null</c> must be handled.
	/// d) <see cref="T:IEquatable`T"/> should be implemented to improve performance.
	/// e) operators (==/!=/...) cannot be applied within template methods.
	/// </summary>
	public static class EqualityTestData
	{
		#region Value Types
		//==========================================================================================
		// Value Types
		//==========================================================================================

		/// <summary></summary>
		public struct ValueTypeWithOperators
		{
			/// <summary></summary>
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
				return ("A = " + this.A.ToString());
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("ValueTypeWithOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				ValueTypeWithOperators other = (ValueTypeWithOperators)obj;
				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				ValueTypeWithOperators other = (ValueTypeWithOperators)obj;
				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
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

				Debug.Indent();
				Debug.WriteLine("ValueTypeWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Debug.WriteLine("Results in True within ReferenceEquals()");
					Debug.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Debug.WriteLine("Results in False since lhs is null");
					Debug.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Debug.WriteLine("Results in False since rhs is null");
					Debug.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Debug.WriteLine("Results in " + result + " within Equals()");
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Value type implementation of operator ==.
				// See MKY.System.Test.EqualityTest for details.

				if (ReferenceEquals(lhs, rhs)) return (true);
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
		public struct ValueTypeWithoutOperators
		{
			/// <summary></summary>
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
				return ("A = " + this.A.ToString());
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("ValueTypeWithoutOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				ValueTypeWithoutOperators other = (ValueTypeWithoutOperators)obj;
				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				ValueTypeWithoutOperators other = (ValueTypeWithoutOperators)obj;
				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
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
		public class BaseReferenceTypeNotIEquatableWithoutOperators
		{
			/// <summary></summary>
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
				return (Environment.NewLine + "    0:A    = " + this.A.ToString());
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				BaseReferenceTypeNotIEquatableWithoutOperators other = (BaseReferenceTypeNotIEquatableWithoutOperators)obj;
				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				BaseReferenceTypeNotIEquatableWithoutOperators other = (BaseReferenceTypeNotIEquatableWithoutOperators)obj;
				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class DerivedReferenceTypeNotIEquatableWithoutOperators : BaseReferenceTypeNotIEquatableWithoutOperators
		{
			/// <summary></summary>
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
					Environment.NewLine + "      1:B    = " + B.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Debug.WriteLine("Results in False within base.Equals()");
					Debug.Unindent();
					return (false);
				}

				DerivedReferenceTypeNotIEquatableWithoutOperators other = obj as DerivedReferenceTypeNotIEquatableWithoutOperators;
				bool result = (this.B == other.B);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				DerivedReferenceTypeNotIEquatableWithoutOperators other = obj as DerivedReferenceTypeNotIEquatableWithoutOperators;
				return (this.B == other.B);

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class BaseReferenceTypeIEquatableWithoutOperators : IEquatable<BaseReferenceTypeIEquatableWithoutOperators>
		{
			/// <summary></summary>
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
				return (Environment.NewLine + "    0:A    = " + this.A.ToString());
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as BaseReferenceTypeIEquatableWithoutOperators));
			}

			/// <summary></summary>
			public bool Equals(BaseReferenceTypeIEquatableWithoutOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeIEquatableWithoutOperators.Equals<BaseReferenceTypeIEquatableWithoutOperators>()");

				if (ReferenceEquals(other, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != other.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(other, null))
					return (false);

				if (GetType() != other.GetType())
					return (false);

				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class DerivedReferenceTypeIEquatableWithoutOperators : BaseReferenceTypeIEquatableWithoutOperators, IEquatable<DerivedReferenceTypeIEquatableWithoutOperators>
		{
			/// <summary></summary>
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
					Environment.NewLine + "      1:B    = " + B.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as DerivedReferenceTypeIEquatableWithoutOperators));
			}

			/// <summary></summary>
			public bool Equals(DerivedReferenceTypeIEquatableWithoutOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeIEquatableWithoutOperators.Equals<DerivedReferenceTypeIEquatableWithoutOperators>()");

				bool result = (base.Equals(other) && (this.B == other.B));

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				return (base.Equals(other) && (this.B == other.B));

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode() ^ B.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class BaseReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
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
				return (Environment.NewLine + "    0:A    = " + this.A.ToString());
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeNotIEquatableWithOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				BaseReferenceTypeNotIEquatableWithOperators other = (BaseReferenceTypeNotIEquatableWithOperators)obj;
				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				BaseReferenceTypeNotIEquatableWithOperators other = (BaseReferenceTypeNotIEquatableWithOperators)obj;
				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
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

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeNotIEquatableWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Debug.WriteLine("Results in True within ReferenceEquals()");
					Debug.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Debug.WriteLine("Results in False since lhs is null");
					Debug.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Debug.WriteLine("Results in False since rhs is null");
					Debug.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Debug.WriteLine("Results in " + result + " within Equals()");
				Debug.Unindent();
				return (result);


			#else

				// Normal implementation:

				if (ReferenceEquals(lhs, rhs)) return (true);
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
		public class DerivedReferenceTypeNotIEquatableWithOperators : BaseReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
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
					Environment.NewLine + "      1:B    = " + B.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeNotIEquatableWithOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Debug.WriteLine("Results in False within base.Equals()");
					Debug.Unindent();
					return (false);
				}

				DerivedReferenceTypeNotIEquatableWithOperators other = obj as DerivedReferenceTypeNotIEquatableWithOperators;
				bool result = (this.B == other.B);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				DerivedReferenceTypeNotIEquatableWithOperators other = obj as DerivedReferenceTypeNotIEquatableWithOperators;
				return (this.B == other.B);

			#endif
			}

			/// <summary></summary>
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

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeNotIEquatableWithOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Debug.WriteLine("Results in True within ReferenceEquals()");
					Debug.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Debug.WriteLine("Results in False since lhs is null");
					Debug.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Debug.WriteLine("Results in False since rhs is null");
					Debug.Unindent();
					return (false);
				}

				bool result = lhs.Equals(rhs);

				Debug.WriteLine("Results in " + result + " within Equals()");
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(lhs, rhs)) return (true);
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
		public class DerivedReferenceTypeNotIEquatableWithDerivedOperators : DerivedReferenceTypeNotIEquatableWithOperators
		{
			/// <summary></summary>
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
					Environment.NewLine + "        2:C    = " + C.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeNotIEquatableWithoutOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Debug.WriteLine("Results in False within base.Equals()");
					Debug.Unindent();
					return (false);
				}

				DerivedReferenceTypeNotIEquatableWithDerivedOperators other = obj as DerivedReferenceTypeNotIEquatableWithDerivedOperators;
				bool result = (this.B == other.B);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				DerivedReferenceTypeNotIEquatableWithDerivedOperators other = obj as DerivedReferenceTypeNotIEquatableWithDerivedOperators;
				return (this.B == other.B);

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class BaseReferenceTypeNotIEquatableWithBaseOperators
		{
			/// <summary></summary>
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
				return (Environment.NewLine + "    0:A    = " + this.A.ToString());
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeNotIEquatableWithBaseOperators.Equals<object>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				BaseReferenceTypeNotIEquatableWithBaseOperators other = (BaseReferenceTypeNotIEquatableWithBaseOperators)obj;
				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				BaseReferenceTypeNotIEquatableWithBaseOperators other = (BaseReferenceTypeNotIEquatableWithBaseOperators)obj;
				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
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

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeNotIEquatableWithBaseOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Debug.WriteLine("Results in True within ReferenceEquals()");
					Debug.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Debug.WriteLine("Results in False since lhs is null");
					Debug.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Debug.WriteLine("Results in False since rhs is null");
					Debug.Unindent();
					return (false);
				}

				// Ensure that object.Equals() is called.
				// Thus, ensure that potential <Derived>.Equals() is called.
				object obj = (object)lhs;
				bool result = obj.Equals(rhs);
				Debug.WriteLine("Results in " + result + " within Equals()");
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Base reference type implementation of operator ==.
				// See MKY.System.Test.EqualityTest for details.

				if (ReferenceEquals(lhs, rhs)) return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				// Ensure that object.Equals() is called.
				// Thus, ensure that potential <Derived>.Equals() is called.
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
		public class DerivedReferenceTypeNotIEquatableWithBaseOperators : BaseReferenceTypeNotIEquatableWithBaseOperators
		{
			/// <summary></summary>
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
					Environment.NewLine + "      1:B    = " + B.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeNotIEquatableWithBaseOperators.Equals<object>()");

				if (!base.Equals(obj))
				{
					Debug.WriteLine("Results in False within base.Equals()");
					Debug.Unindent();
					return (false);
				}

				DerivedReferenceTypeNotIEquatableWithBaseOperators other = obj as DerivedReferenceTypeNotIEquatableWithBaseOperators;
				bool result = (this.B == other.B);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (!base.Equals(obj))
					return (false);

				DerivedReferenceTypeNotIEquatableWithBaseOperators other = obj as DerivedReferenceTypeNotIEquatableWithBaseOperators;
				return (this.B == other.B);

			#endif
			}

			/// <summary></summary>
			public override int GetHashCode()
			{
				return (base.GetHashCode() ^ A.GetHashCode());
			}

			#endregion
		}

		/// <summary></summary>
		public class BaseReferenceTypeIEquatableWithBaseOperators : IEquatable<BaseReferenceTypeIEquatableWithBaseOperators>
		{
			/// <summary></summary>
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
				return (Environment.NewLine + "    0:A    = " + this.A.ToString());
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
				return (Equals(obj as BaseReferenceTypeIEquatableWithBaseOperators));
			}

			/// <summary></summary>
			public bool Equals(BaseReferenceTypeIEquatableWithBaseOperators other)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeIEquatableWithBaseOperators.Equals<BaseReferenceTypeIEquatableWithBaseOperators>()");

				if (ReferenceEquals(other, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != other.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				bool result = (this.A == other.A);

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(other, null))
					return (false);

				if (GetType() != other.GetType())
					return (false);

				return (this.A == other.A);

			#endif
			}

			/// <summary></summary>
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

				Debug.Indent();
				Debug.WriteLine("BaseReferenceTypeIEquatableWithBaseOperators.operator ==()");

				if (ReferenceEquals(lhs, rhs))
				{
					Debug.WriteLine("Results in True within ReferenceEquals()");
					Debug.Unindent();
					return (true);
				}

				if (ReferenceEquals(lhs, null))
				{
					Debug.WriteLine("Results in False since lhs is null");
					Debug.Unindent();
					return (false);
				}

				if (ReferenceEquals(rhs, null))
				{
					Debug.WriteLine("Results in False since rhs is null");
					Debug.Unindent();
					return (false);
				}

				// Ensure that object.Equals() is called.
				// Thus, ensure that potential <Derived>.Equals() is called.
				object obj = (object)lhs;
				bool result = obj.Equals(rhs);
				Debug.WriteLine("Results in " + result + " within Equals()");
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				// Base reference type implementation of operator ==.
				// See MKY.System.Test.EqualityTest for details.

				if (ReferenceEquals(lhs, rhs)) return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				// Ensure that object.Equals() is called.
				// Thus, ensure that potential <Derived>.Equals() is called.
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
		public class DerivedReferenceTypeDerivedIEquatableWithBaseOperators : BaseReferenceTypeIEquatableWithBaseOperators
		{
			/// <summary></summary>
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
					Environment.NewLine + "      1:B    = " + B.ToString()
					);
			}

			/// <summary></summary>
			public override bool Equals(object obj)
			{
			#if (OUTPUT_CALLING_SEQUENCE)

				// Test implementation to get details about the calling sequence:

				Debug.Indent();
				Debug.WriteLine("DerivedReferenceTypeDerivedIEquatableWithBaseOperators.Equals<DerivedReferenceTypeDerivedIEquatableWithBaseOperators>()");

				if (ReferenceEquals(obj, null))
				{
					Debug.WriteLine("Results in False since other is null");
					Debug.Unindent();
					return (false);
				}

				if (GetType() != obj.GetType())
				{
					Debug.WriteLine("Results in False since types don't match");
					Debug.Unindent();
					return (false);
				}

				DerivedReferenceTypeDerivedIEquatableWithBaseOperators other = (DerivedReferenceTypeDerivedIEquatableWithBaseOperators)obj;
				bool result = (base.Equals(other) && (this.B == other.B));

				Debug.WriteLine("Results in " + result);
				Debug.Unindent();
				return (result);

			#else

				// Normal implementation:

				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				DerivedReferenceTypeDerivedIEquatableWithBaseOperators other = (DerivedReferenceTypeDerivedIEquatableWithBaseOperators)obj;
				return (base.Equals(other) && (this.B == other.B));

			#endif
			}

			/// <summary></summary>
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
	[TestFixture, Explicit("This test fixture only displays the general functionality of Equals() and ==/!=. It does not test any MKY.System functionality.")]
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
			Debug.Indent();
			Debug.WriteLine("");
			Debug.WriteLine("Base without operators");
			{
				EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators a = new EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators(1);
				EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators b = new EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators(1);
				bool result = (a == b);
				Debug.WriteLine("a == b of base reference type without operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Derived without operators");
			{
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators a = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators(1, 2);
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators b = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators(1, 2);
				bool result = (a == b);
				Debug.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Base IEquatable without operators");
			{
				EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators a = new EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators(1);
				EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators b = new EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators(1);
				bool result = (a == b);
				Debug.WriteLine("a == b of base reference type IEquatable without operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Derived IEquatable without operators");
			{
				EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators a = new EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators(1, 2);
				EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators b = new EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators(1, 2);
				bool result = (a == b);
				Debug.WriteLine("a == b of derived reference type IEquatable without operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Base with operators");
			{
				EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators a = new EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators(1);
				EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators b = new EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators(1);
				bool result = (a == b);
				Debug.WriteLine("a == b of base reference type with operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Derived with operators");
			{
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators a = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators(1, 2);
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators b = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators(1, 2);
				bool result = (a == b);
				Debug.WriteLine("a == b of derived reference type with operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Derived without operators");
			{
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators a = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(1, 2, 3);
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators b = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(1, 2, 3);
				bool result = (a == b);
				Debug.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Base with base operators");
			{
				EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators a = new EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators(1);
				EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators b = new EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators(1);
				bool result = (a == b);
				Debug.WriteLine("a == b of base reference type with base operators results in " + result);
			}
			Debug.WriteLine("");
			Debug.WriteLine("Derived with base operators");
			{
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators a = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators(1, 2);
				EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators b = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators(1, 2);
				bool result = (a == b);
				Debug.WriteLine("a == b of derived reference type with base operators results in " + result);
			}
			Debug.Unindent();
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only tests the general functionality of Equals() and ==/!=. It does not test any MKY.System functionality.")]
	public class EqualityTestOfValueTypes
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void TestSystemValueType()
		{
			DateTime objToCompareAgainst = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objEqual            = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objNotEqual         = new DateTime(2001, 2, 3, 13, 4, 5);

			EqualityTestMethods.TestEquals<object>  (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<DateTime>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType(objToCompareAgainst, objEqual, objNotEqual);
			// Operator ==/!= cannot be used on System.ValueType itself.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDateTime (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDateTime     (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestOwnValueTypeWithoutOperators()
		{
			EqualityTestData.ValueTypeWithoutOperators objToCompareAgainst = new EqualityTestData.ValueTypeWithoutOperators(1);
			EqualityTestData.ValueTypeWithoutOperators objEqual            = new EqualityTestData.ValueTypeWithoutOperators(1);
			EqualityTestData.ValueTypeWithoutOperators objNotEqual         = new EqualityTestData.ValueTypeWithoutOperators(2);

			EqualityTestMethods.TestEquals<object>                                  (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.ValueTypeWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType(objToCompareAgainst, objEqual, objNotEqual);
			// Operator ==/!= cannot be used on System.ValueType itself.
			// Operator ==/!= cannot be directly applied to value types without operators,
			//   neither to evaluate reference nor value equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void TestOwnValueTypeWithOperators()
		{
			EqualityTestData.ValueTypeWithOperators objToCompareAgainst = new EqualityTestData.ValueTypeWithOperators(1);
			EqualityTestData.ValueTypeWithOperators objEqual            = new EqualityTestData.ValueTypeWithOperators(1);
			EqualityTestData.ValueTypeWithOperators objNotEqual         = new EqualityTestData.ValueTypeWithOperators(2);

			EqualityTestMethods.TestEquals<object>                               (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.ValueTypeWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueType             (objToCompareAgainst, objEqual, objNotEqual);
			// Operator ==/!= cannot be used on System.ValueType itself.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfValueTypeWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfValueTypeWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only tests the general functionality of Equals() and ==/!=. It does not test any MKY.System functionality.")]
	public class EqualityTestOfReferenceTypesWithoutOperators
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForBase")]
		public virtual void TestOwnBaseReferenceTypeNotIEquatableWithoutOperators(int a1, int a2, int a3)
		{
			EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst = new EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators(a1);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual            = new EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators(a2);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual         = new EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerived")]
		public virtual void TestOwnDerivedReferenceTypeNotIEquatableWithoutOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators(a1, b1);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual            = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators(a2, b2);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForBase")]
		public virtual void TestOwnBaseReferenceTypeIEquatableWithoutOperators(int a1, int a2, int a3)
		{
			EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst = new EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators(a1);
			EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objEqual            = new EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators(a2);
			EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual         = new EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerived")]
		public virtual void TestOwnDerivedReferenceTypeIEquatableWithoutOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators(a1, b1);
			EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objEqual            = new EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators(a2, b2);
			EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only tests the general functionality of Equals() and ==/!=. It does not test any MKY.System functionality.")]
	public class EqualityTestOfReferenceTypesWithOperators
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void TestSystemBaseReferenceTypeIEquatableWithOperators()
		{
			Version objToCompareAgainst = new Version(1, 1);
			Version objEqual            = new Version(1, 1);
			Version objNotEqual         = new Version(1, 2);

			EqualityTestMethods.TestEquals<object> (objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<Version>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject (objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfVersion(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfVersion    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForBase")]
		public virtual void TestOwnBaseReferenceTypeNotIEquatableWithOperators(int a1, int a2, int a3)
		{
			EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst = new EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators(a1);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objEqual            = new EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators(a2);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual         = new EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerived")]
		public virtual void TestOwnDerivedReferenceTypeNotIEquatableWithOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators(a1, b1);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objEqual            = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators(a2, b2);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerivedDerived")]
		public virtual void TestOwnDerivedReferenceTypeNotIEquatableWithDerivedOperators(int a1, int b1, int c1, int a2, int b2, int c2, int a3, int b3, int c3)
		{
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a1, b1, c1);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual            = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a2, b2, c2);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators(a3, b3, c3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForBase")]
		public virtual void TestOwnBaseReferenceTypeNotIEquatableWithBaseOperators(int a1, int a2, int a3)
		{
			EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst = new EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators(a1);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual            = new EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators(a2);
			EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual         = new EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerived")]
		public virtual void TestOwnDerivedReferenceTypeNotIEquatableWithBaseOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a1, b1);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual            = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a2, b2);
			EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForBase")]
		public virtual void TestOwnBaseReferenceTypeIEquatableWithBaseOperators(int a1, int a2, int a3)
		{
			EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst = new EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators(a1);
			EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objEqual            = new EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators(a2);
			EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual         = new EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators(a3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(EqualityTestData), "TestCasesForDerived")]
		public virtual void TestOwnDerivedReferenceTypeDerivedIEquatableWithBaseOperators(int a1, int b1, int a2, int b2, int a3, int b3)
		{
			EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst = new EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a1, b1);
			EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual            = new EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a2, b2);
			EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual         = new EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators(a3, b3);

			EqualityTestMethods.TestEquals<object>(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestEquals<EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators>(objToCompareAgainst, objEqual, objNotEqual);

			EqualityTestMethods.TestOperatorsForReferenceEqualityOfObject(objToCompareAgainst, objEqual, objNotEqual);
			// object.operators ==/!= only evaluate reference equality.
			EqualityTestMethods.TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(objToCompareAgainst, objEqual, objNotEqual);
			EqualityTestMethods.TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}

	internal static class EqualityTestMethods
	{
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
			Debug.Indent();
			Debug.WriteLine("TestEquals<" + typeof(T).FullName + ">");
			Debug.Indent();

			try
			{
				// Reference equal.

				Debug.WriteLine("Reference equal using Equals()");
				Debug.Indent();

				if (!objToCompareAgainst.Equals(objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using Equals()");

				Debug.Unindent();

				// Value equal.

				Debug.WriteLine("Value equal using Equals()");
				Debug.Indent();

				if (!objToCompareAgainst.Equals(objEqual))
					Assert.Fail("Value equal objects are not considered equal using Equals()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using Equals()");
				Debug.Indent();

				if (objToCompareAgainst.Equals(objNotEqual))
					Assert.Fail("Value not equal objects are considered equal using Equals()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		#endregion

		#region Static Test Methods > Value Types
		//------------------------------------------------------------------------------------------
		// Static Test Methods > Value Types
		//------------------------------------------------------------------------------------------

		public static void TestOperatorsForReferenceEqualityOfValueType(ValueType objToCompareAgainst, ValueType objEqual, ValueType objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfValueType");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		//                 TestOperatorsForValueEqualityOfValueType is useless since it never succeeds.

		public static void TestOperatorsForReferenceEqualityOfDateTime(DateTime objToCompareAgainst, DateTime objEqual, DateTime objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDateTime");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDateTime(DateTime objToCompareAgainst, DateTime objEqual, DateTime objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDateTime");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfValueTypeWithOperators(EqualityTestData.ValueTypeWithOperators objToCompareAgainst, EqualityTestData.ValueTypeWithOperators objEqual, EqualityTestData.ValueTypeWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfValueTypeWithOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfValueTypeWithOperators(EqualityTestData.ValueTypeWithOperators objToCompareAgainst, EqualityTestData.ValueTypeWithOperators objEqual, EqualityTestData.ValueTypeWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfValueTypeWithOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		#endregion

		#region Static Test Methods > Reference Types
		//------------------------------------------------------------------------------------------
		// Static Test Methods > Reference Types
		//------------------------------------------------------------------------------------------

		public static void TestOperatorsForReferenceEqualityOfObject(object objToCompareAgainst, object objEqual, object objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfObject");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		//                 TestOperatorsForValueEqualityOfObject is useless since it never succeeds.

		public static void TestOperatorsForReferenceEqualityOfVersion(Version objToCompareAgainst, Version objEqual, Version objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfVersion");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfVersion(Version objToCompareAgainst, Version objEqual, Version objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfVersion");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators(EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objEqual, EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithoutOperators(EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objEqual, EqualityTestData.BaseReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objEqual, EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objEqual, EqualityTestData.DerivedReferenceTypeIEquatableWithoutOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithDerivedOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithoutOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators(EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityTestData.BaseReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeNotIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators(EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objEqual, EqualityTestData.DerivedReferenceTypeNotIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeNotIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objEqual, EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfBaseReferenceTypeIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators(EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objEqual, EqualityTestData.BaseReferenceTypeIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfBaseReferenceTypeIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual, EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForReferenceEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Reference equal.

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				Debug.WriteLine("Reference equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objToCompareAgainst))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Reference equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objToCompareAgainst)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
			}
		}

		public static void TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators(EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objToCompareAgainst, EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objEqual, EqualityTestData.DerivedReferenceTypeDerivedIEquatableWithBaseOperators objNotEqual)
		{
			Debug.Indent();
			Debug.WriteLine("TestOperatorsForValueEqualityOfDerivedReferenceTypeDerivedIEquatableWithBaseOperators");
			Debug.Indent();

			try
			{
				// Value equal.

				Debug.WriteLine("Value equal using operator ==()");
				Debug.Indent();

				if (!(objToCompareAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value equal using operator !=()");
				Debug.Indent();

				if (objToCompareAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Debug.Unindent();

				// Value not equal.

				Debug.WriteLine("Value not equal using operator ==()");
				Debug.Indent();

				if (objToCompareAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Debug.Unindent();
				Debug.WriteLine("Value not equal using operator !=()");
				Debug.Indent();

				if (!(objToCompareAgainst != objNotEqual))
					Assert.Fail("Value not equal objects are considered not equal using operator !=()");

				Debug.Unindent();
			}
			catch (AssertionException ex)
			{
				Debug.Unindent();
				throw (ex);
			}
			finally
			{
				Debug.Unindent();
				Debug.Unindent();
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
