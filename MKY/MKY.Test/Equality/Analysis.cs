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
using System.Net;

using NUnit.Framework;

namespace MKY.Test.Equality
{
	// Difficulties with equality:
	//  > Reference types may need to deal with value equality on all virtual/inheritance levels.
	//    Overriding Equals() provides this functionality.
	//  > object.operators == and != are not virtual by default and therefore cannot take
	//    value equality of reference types into account.
	//  > operators (==/!=/...) cannot be applied within template methods.
	//  > IEquatable<T> should be implemented to improve performance.
	//  > null must be handled.

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only displays the general functionality of Equals() and ==/!=. It does neither perform any tests nor tests any MKY functionality.")]
	public class TraceEquality
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void TraceSequenceOfEqualityOfOwnReferenceType()
		{
			Trace.Indent();
			Trace.WriteLine("");
			Trace.WriteLine("Base without operators");
			{
				Types.ReferenceTypeNotIEquatableWithoutOperatorsBase a = new Types.ReferenceTypeNotIEquatableWithoutOperatorsBase(1);
				Types.ReferenceTypeNotIEquatableWithoutOperatorsBase b = new Types.ReferenceTypeNotIEquatableWithoutOperatorsBase(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived a = new Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived(1, 2);
				Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived b = new Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base IEquatable without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperatorsBase a = new Types.ReferenceTypeIEquatableWithoutOperatorsBase(1);
				Types.ReferenceTypeIEquatableWithoutOperatorsBase b = new Types.ReferenceTypeIEquatableWithoutOperatorsBase(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived IEquatable without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperatorsDerived a = new Types.ReferenceTypeIEquatableWithoutOperatorsDerived(1, 2);
				Types.ReferenceTypeIEquatableWithoutOperatorsDerived b = new Types.ReferenceTypeIEquatableWithoutOperatorsDerived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with operators");
			{
				Types.ReferenceTypeNotIEquatableWithOperatorsBase a = new Types.ReferenceTypeNotIEquatableWithOperatorsBase(1);
				Types.ReferenceTypeNotIEquatableWithOperatorsBase b = new Types.ReferenceTypeNotIEquatableWithOperatorsBase(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with operators");
			{
				Types.ReferenceTypeNotIEquatableWithOperatorsDerived a = new Types.ReferenceTypeNotIEquatableWithOperatorsDerived(1, 2);
				Types.ReferenceTypeNotIEquatableWithOperatorsDerived b = new Types.ReferenceTypeNotIEquatableWithOperatorsDerived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived a = new Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived(1, 2, 3);
				Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived b = new Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived(1, 2, 3);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with base operators");
			{
				Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase a = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase(1);
				Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase b = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with base operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with base operators");
			{
				Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived a = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived(1, 2);
				Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived b = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with base operators results in " + result);
			}
			Trace.Unindent();
		}

		#endregion
	}

		/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class AnalysisOfValueTypesWithoutOperators
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeNotIEquatableWithoutOperators()
		{
			Types.ValueTypeNotIEquatableWithoutOperators objToCompareAgainst = new Types.ValueTypeNotIEquatableWithoutOperators(1);
			Types.ValueTypeNotIEquatableWithoutOperators objEqual            = new Types.ValueTypeNotIEquatableWithoutOperators(1);
			Types.ValueTypeNotIEquatableWithoutOperators objNotEqual         = new Types.ValueTypeNotIEquatableWithoutOperators(2);

			Methods.Generic.TestEquals<object>                                      (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeNotIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			//// Operator ==/!= cannot be directly applied to value types without operators,
			////   neither to evaluate reference nor value equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeIEquatableWithoutOperators()
		{
			Types.ValueTypeIEquatableWithoutOperators objToCompareAgainst = new Types.ValueTypeIEquatableWithoutOperators(1);
			Types.ValueTypeIEquatableWithoutOperators objEqual            = new Types.ValueTypeIEquatableWithoutOperators(1);
			Types.ValueTypeIEquatableWithoutOperators objNotEqual         = new Types.ValueTypeIEquatableWithoutOperators(2);

			Methods.Generic.TestEquals<object>                                   (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeIEquatableWithoutOperators>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			//// Operator ==/!= cannot be directly applied to value types without operators,
			////   neither to evaluate reference nor value equality.
		}

		#endregion
	}

		/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class AnalysisOfValueTypesWithOperators
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemValueTypeWithOperators()
		{
			DateTime objToCompareAgainst = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objEqual            = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objNotEqual         = new DateTime(2001, 2, 3, 13, 4, 5);

			Methods.Generic.TestEquals<object>  (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<DateTime>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeDateTime.TestOperatorsForReferenceEquality (objToCompareAgainst, objEqual, objNotEqual);
			Methods.ValueTypeDateTime.TestOperatorsForValueEquality     (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeNotIEquatableWithOperators()
		{
			Types.ValueTypeNotIEquatableWithOperators objToCompareAgainst = new Types.ValueTypeNotIEquatableWithOperators(1);
			Types.ValueTypeNotIEquatableWithOperators objEqual            = new Types.ValueTypeNotIEquatableWithOperators(1);
			Types.ValueTypeNotIEquatableWithOperators objNotEqual         = new Types.ValueTypeNotIEquatableWithOperators(2);

			Methods.Generic.TestEquals<object>                                   (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeNotIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeNotIEquatableWithOperators.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ValueTypeNotIEquatableWithOperators.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeIEquatableWithOperators()
		{
			Types.ValueTypeIEquatableWithOperators objToCompareAgainst = new Types.ValueTypeIEquatableWithOperators(1);
			Types.ValueTypeIEquatableWithOperators objEqual            = new Types.ValueTypeIEquatableWithOperators(1);
			Types.ValueTypeIEquatableWithOperators objNotEqual         = new Types.ValueTypeIEquatableWithOperators(2);

			Methods.Generic.TestEquals<object>                                (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeIEquatableWithOperators>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeIEquatableWithOperators.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ValueTypeIEquatableWithOperators.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class AnalysisOfReferenceTypesWithoutOperators
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemReferenceTypeCollectionNotIEquatableWithoutOperators()
		{
			List<int> objToCompareAgainst = new List<int>(2); // Preset the required capacity to improve memory management.
			List<int> objEqual            = new List<int>(2); // Preset the required capacity to improve memory management.
			List<int> objNotEqual         = new List<int>(2); // Preset the required capacity to improve memory management.

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// List<int>.Equals() doesn't exist, and object.Equals() only evaluates reference equality.

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// List<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemReferenceTypeIEquatableWithoutOperators()
		{
			IPAddress objToCompareAgainst = new IPAddress(new byte[] { 0, 1, 2, 3 });
			IPAddress objEqual            = new IPAddress(new byte[] { 0, 1, 2, 3 });
			IPAddress objNotEqual         = new IPAddress(new byte[] { 0, 1, 2, 9 });

			Methods.Generic.TestEquals<object>   (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<IPAddress>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIPAddress.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// IPAddress.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionNotIEquatableWithoutOperators()
		{
			Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int> objToCompareAgainst = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();
			Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int> objEqual            = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();
			Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int> objNotEqual         = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.Equals() doesn't exist, and List<int>.Equals() only evaluates reference equality.

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionIEquatableWithoutOperators()
		{
			Types.ReferenceTypeCollectionIEquatableWithoutOperators<int> objToCompareAgainst = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();
			Types.ReferenceTypeCollectionIEquatableWithoutOperators<int> objEqual            = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();
			Types.ReferenceTypeCollectionIEquatableWithoutOperators<int> objNotEqual         = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			Methods.Generic.TestEquals<object>                                                      (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// EqualityTestData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForBase")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperatorsBase(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithoutOperatorsBase objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperatorsBase(b1);
			Types.ReferenceTypeNotIEquatableWithoutOperatorsBase objEqual            = new Types.ReferenceTypeNotIEquatableWithoutOperatorsBase(b2);
			Types.ReferenceTypeNotIEquatableWithoutOperatorsBase objNotEqual         = new Types.ReferenceTypeNotIEquatableWithoutOperatorsBase(b3);

			Methods.Generic.TestEquals<object>                                              (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperatorsBase>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperatorsBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperatorsDerived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived objEqual            = new Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived(b3, d3);

			Methods.Generic.TestEquals<object>                                                 (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForBase")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperatorsBase(int b1, int b2, int b3)
		{
			Types.ReferenceTypeIEquatableWithoutOperatorsBase objToCompareAgainst = new Types.ReferenceTypeIEquatableWithoutOperatorsBase(b1);
			Types.ReferenceTypeIEquatableWithoutOperatorsBase objEqual            = new Types.ReferenceTypeIEquatableWithoutOperatorsBase(b2);
			Types.ReferenceTypeIEquatableWithoutOperatorsBase objNotEqual         = new Types.ReferenceTypeIEquatableWithoutOperatorsBase(b3);

			Methods.Generic.TestEquals<object>                                           (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperatorsBase>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperatorsBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperatorsDerived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeIEquatableWithoutOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithoutOperatorsDerived(b1, d1);
			Types.ReferenceTypeIEquatableWithoutOperatorsDerived objEqual            = new Types.ReferenceTypeIEquatableWithoutOperatorsDerived(b2, d2);
			Types.ReferenceTypeIEquatableWithoutOperatorsDerived objNotEqual         = new Types.ReferenceTypeIEquatableWithoutOperatorsDerived(b3, d3);

			Methods.Generic.TestEquals<object>                                              (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture, Explicit("This test fixture only analyzes the general functionality of Equals() and ==/!=. It does neither perform any 'real' tests nor tests any MKY functionality.")]
	public class AnalysisOfReferenceTypesWithOperators
	{
		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemReferenceTypeIEquatableWithOperators()
		{
			Version objToCompareAgainst = new Version(1, 1);
			Version objEqual            = new Version(1, 1);
			Version objNotEqual         = new Version(1, 2);

			Methods.Generic.TestEquals<object> (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Version>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality (objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeVersion.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeVersion.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionIEquatableWithOperators()
		{
			Types.ReferenceTypeCollectionIEquatableWithOperators<int> objToCompareAgainst = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();
			Types.ReferenceTypeCollectionIEquatableWithOperators<int> objEqual            = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();
			Types.ReferenceTypeCollectionIEquatableWithOperators<int> objNotEqual         = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();

			objToCompareAgainst.AddRange(new int[] { 1, 1 });
			objEqual           .AddRange(new int[] { 1, 1 });
			objNotEqual        .AddRange(new int[] { 1, 2 });

			Methods.Generic.TestEquals<object>                                                   (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeCollectionIEquatableWithOperators<int>>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeCollectionIEquatableWithOperators.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeCollectionIEquatableWithOperators.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForBase")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsBase(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithOperatorsBase objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithOperatorsBase(b1);
			Types.ReferenceTypeNotIEquatableWithOperatorsBase objEqual            = new Types.ReferenceTypeNotIEquatableWithOperatorsBase(b2);
			Types.ReferenceTypeNotIEquatableWithOperatorsBase objNotEqual         = new Types.ReferenceTypeNotIEquatableWithOperatorsBase(b3);

			Methods.Generic.TestEquals<object>                                           (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperatorsBase>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperatorsBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithOperatorsBase.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsDerived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithOperatorsDerived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithOperatorsDerived objEqual            = new Types.ReferenceTypeNotIEquatableWithOperatorsDerived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithOperatorsDerived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithOperatorsDerived(b3, d3);

			Methods.Generic.TestEquals<object>                                              (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithOperatorsDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerivedDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithDerivedOperatorsDerived(int b1, int d1, int dd1, int b2, int d2, int dd2, int b3, int d3, int dd3)
		{
			Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived(b1, d1, dd1);
			Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived objEqual            = new Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived(b2, d2, dd2);
			Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived(b3, d3, dd3);

			Methods.Generic.TestEquals<object>                                                     (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithDerivedOperatorsDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForBase")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithBaseOperatorsBase(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase(b1);
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase objEqual            = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase(b2);
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase objNotEqual         = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase(b3);

			Methods.Generic.TestEquals<object>                                               (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithBaseOperatorsBase>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithBaseOperatorsBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithBaseOperatorsBase.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithBaseOperatorsDerived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived objEqual            = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived(b3, d3);

			Methods.Generic.TestEquals<object>                                                  (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithBaseOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithBaseOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithBaseOperatorsDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForBase")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithBaseOperatorsBase(int b1, int b2, int b3)
		{
			Types.ReferenceTypeIEquatableWithBaseOperatorsBase objToCompareAgainst = new Types.ReferenceTypeIEquatableWithBaseOperatorsBase(b1);
			Types.ReferenceTypeIEquatableWithBaseOperatorsBase objEqual            = new Types.ReferenceTypeIEquatableWithBaseOperatorsBase(b2);
			Types.ReferenceTypeIEquatableWithBaseOperatorsBase objNotEqual         = new Types.ReferenceTypeIEquatableWithBaseOperatorsBase(b3);

			Methods.Generic.TestEquals<object>                                            (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithBaseOperatorsBase>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithBaseOperatorsBase.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithBaseOperatorsBase.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCasesForDerived")]
		public virtual void AnalyzeOwnReferenceTypeDerivedIEquatableWithBaseOperatorsDerived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeIEquatableWithBaseOperatorsDerived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithBaseOperatorsDerived(b1, d1);
			Types.ReferenceTypeIEquatableWithBaseOperatorsDerived objEqual            = new Types.ReferenceTypeIEquatableWithBaseOperatorsDerived(b2, d2);
			Types.ReferenceTypeIEquatableWithBaseOperatorsDerived objNotEqual         = new Types.ReferenceTypeIEquatableWithBaseOperatorsDerived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithBaseOperatorsDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithBaseOperatorsDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithBaseOperatorsDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
