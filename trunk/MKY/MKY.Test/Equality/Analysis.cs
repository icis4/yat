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
				Types.ReferenceTypeNotIEquatableWithoutOperators_Base a = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(1);
				Types.ReferenceTypeNotIEquatableWithoutOperators_Base b = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				Types.ReferenceTypeNotIEquatableWithoutOperators_Derived a = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(1, 2);
				Types.ReferenceTypeNotIEquatableWithoutOperators_Derived b = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base IEquatable without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperators_Base a = new Types.ReferenceTypeIEquatableWithoutOperators_Base(1);
				Types.ReferenceTypeIEquatableWithoutOperators_Base b = new Types.ReferenceTypeIEquatableWithoutOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived IEquatable without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperators_Derived a = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(1, 2);
				Types.ReferenceTypeIEquatableWithoutOperators_Derived b = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type IEquatable without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with operators");
			{
				Types.ReferenceTypeNotIEquatableWithOperators_Base a = new Types.ReferenceTypeNotIEquatableWithOperators_Base(1);
				Types.ReferenceTypeNotIEquatableWithOperators_Base b = new Types.ReferenceTypeNotIEquatableWithOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with operators");
			{
				Types.ReferenceTypeNotIEquatableWithOperators_Derived a = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(1, 2);
				Types.ReferenceTypeNotIEquatableWithOperators_Derived b = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived a = new Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(1, 2, 3);
				Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived b = new Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(1, 2, 3);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base with base operators");
			{
				Types.ReferenceTypeNotIEquatableWithBaseOperators_Base a = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Base(1);
				Types.ReferenceTypeNotIEquatableWithBaseOperators_Base b = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with base operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with base operators");
			{
				Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived a = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived(1, 2);
				Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived b = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived(1, 2);
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
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithoutOperators_Base objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b1);
			Types.ReferenceTypeNotIEquatableWithoutOperators_Base objEqual            = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b2);
			Types.ReferenceTypeNotIEquatableWithoutOperators_Base objNotEqual         = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                              (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithoutOperators_Derived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithoutOperators_Derived objEqual            = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithoutOperators_Derived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                 (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeIEquatableWithoutOperators_Base objToCompareAgainst = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b1);
			Types.ReferenceTypeIEquatableWithoutOperators_Base objEqual            = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b2);
			Types.ReferenceTypeIEquatableWithoutOperators_Base objNotEqual         = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                           (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= are called, thus only reference equality is evaluated too.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeIEquatableWithoutOperators_Derived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b1, d1);
			Types.ReferenceTypeIEquatableWithoutOperators_Derived objEqual            = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b2, d2);
			Types.ReferenceTypeIEquatableWithoutOperators_Derived objNotEqual         = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                              (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
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
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithBaseOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Base objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Base(b1);
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Base objEqual            = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Base(b2);
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Base objNotEqual         = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                                (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithBaseOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithBaseOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithBaseOperators_Base.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithBaseOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived objEqual            = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                   (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithBaseOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithBaseOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithBaseOperators_Derived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_DerivedDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(int b1, int d1, int dd1, int b2, int d2, int dd2, int b3, int d3, int dd3)
		{
			Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(b1, d1, dd1);
			Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived objEqual            = new Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(b2, d2, dd2);
			Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived(b3, d3, dd3);

			Methods.Generic.TestEquals<object>                                                             (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithDerivedOperators_DerivedDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeNotIEquatableWithOperators_Base objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b1);
			Types.ReferenceTypeNotIEquatableWithOperators_Base objEqual            = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b2);
			Types.ReferenceTypeNotIEquatableWithOperators_Base objNotEqual         = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                            (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeNotIEquatableWithOperators_Derived objToCompareAgainst = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b1, d1);
			Types.ReferenceTypeNotIEquatableWithOperators_Derived objEqual            = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b2, d2);
			Types.ReferenceTypeNotIEquatableWithOperators_Derived objNotEqual         = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeNotIEquatableWithOperators_Derived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithBaseOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeIEquatableWithBaseOperators_Base objToCompareAgainst = new Types.ReferenceTypeIEquatableWithBaseOperators_Base(b1);
			Types.ReferenceTypeIEquatableWithBaseOperators_Base objEqual            = new Types.ReferenceTypeIEquatableWithBaseOperators_Base(b2);
			Types.ReferenceTypeIEquatableWithBaseOperators_Base objNotEqual         = new Types.ReferenceTypeIEquatableWithBaseOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                             (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithBaseOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithBaseOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithBaseOperators_Base.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeDerivedIEquatableWithBaseOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeIEquatableWithBaseOperators_Derived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithBaseOperators_Derived(b1, d1);
			Types.ReferenceTypeIEquatableWithBaseOperators_Derived objEqual            = new Types.ReferenceTypeIEquatableWithBaseOperators_Derived(b2, d2);
			Types.ReferenceTypeIEquatableWithBaseOperators_Derived objNotEqual         = new Types.ReferenceTypeIEquatableWithBaseOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithBaseOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithBaseOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithBaseOperators_Derived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_DerivedDerived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithDerivedOperators_DerivedDerived(int b1, int d1, int dd1, int b2, int d2, int dd2, int b3, int d3, int dd3)
		{
			Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived(b1, d1, dd1);
			Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived objEqual            = new Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived(b2, d2, dd2);
			Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived objNotEqual         = new Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived(b3, d3, dd3);

			Methods.Generic.TestEquals<object>                                                          (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithDerivedOperators_DerivedDerived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperators_Base(int b1, int b2, int b3)
		{
			Types.ReferenceTypeIEquatableWithOperators_Base objToCompareAgainst = new Types.ReferenceTypeIEquatableWithOperators_Base(b1);
			Types.ReferenceTypeIEquatableWithOperators_Base objEqual            = new Types.ReferenceTypeIEquatableWithOperators_Base(b2);
			Types.ReferenceTypeIEquatableWithOperators_Base objNotEqual         = new Types.ReferenceTypeIEquatableWithOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                            (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperators_Base>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			Types.ReferenceTypeIEquatableWithOperators_Derived objToCompareAgainst = new Types.ReferenceTypeIEquatableWithOperators_Derived(b1, d1);
			Types.ReferenceTypeIEquatableWithOperators_Derived objEqual            = new Types.ReferenceTypeIEquatableWithOperators_Derived(b2, d2);
			Types.ReferenceTypeIEquatableWithOperators_Derived objNotEqual         = new Types.ReferenceTypeIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToCompareAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperators_Derived>(objToCompareAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperators_Derived.TestOperatorsForReferenceEquality(objToCompareAgainst, objEqual, objNotEqual);
			Methods.ReferenceTypeIEquatableWithOperators_Derived.TestOperatorsForValueEquality    (objToCompareAgainst, objEqual, objNotEqual);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
