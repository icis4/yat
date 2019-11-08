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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using NUnit.Framework;

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "Short and emphasize the hierarchy (b = base, d = derived)."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Base(System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Base(System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Base(System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_Base(System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "d")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", MessageId = "b")]

// Justification = "Emphasize the hierarchy (base -vs- derived)."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Base(System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Base(System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithoutOperators.#AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Base(System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_Base(System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeIEquatableWithOperators_DerivedCastedToBase(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = " member", Target = "MKY.Test.Equality.AnalysisOfReferenceTypesWithOperators.#AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)")]

#endregion

namespace MKY.Test.Equality
{
	// Challenges with equality:
	//  > Reference types may need to deal with value equality on all virtual/inheritance levels.
	//    Overriding Equals() provides this functionality.
	//  > object.operators ==/!= do not take value equality into account.
	//  > operators ==/!=/... are not virtual and bound at compile time.
	//  > operators ==/!=/... cannot be applied within template methods.
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
		public virtual void TraceSequenceOfEqualityOfOwnReferenceTypes()
		{
			Trace.Indent();
			Trace.WriteLine("");
			Trace.WriteLine("Not IEquatable");
			Trace.Indent();
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
			Trace.WriteLine("Derived with operators of base only");
			{
				Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived a = new Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(1, 2);
				Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived b = new Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators of base only results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived-derived with operators of derived only");
			{
				Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived a = new Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(1, 2, 3);
				Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived b = new Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(1, 2, 3);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived-derived reference type with operators of derived only results in " + result);
			}
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
			Trace.Unindent();
			Trace.WriteLine("");
			Trace.WriteLine("IEquatable");
			Trace.Indent();
			Trace.WriteLine("");
			Trace.WriteLine("Base with operators");
			{
				Types.ReferenceTypeIEquatableWithOperators_Base a = new Types.ReferenceTypeIEquatableWithOperators_Base(1);
				Types.ReferenceTypeIEquatableWithOperators_Base b = new Types.ReferenceTypeIEquatableWithOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with operators");
			{
				Types.ReferenceTypeIEquatableWithOperators_Derived a = new Types.ReferenceTypeIEquatableWithOperators_Derived(1, 2);
				Types.ReferenceTypeIEquatableWithOperators_Derived b = new Types.ReferenceTypeIEquatableWithOperators_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived with operators of base only");
			{
				Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived a = new Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(1, 2);
				Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived b = new Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type with operators of base only results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived-derived with operators of derived only");
			{
				Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived a = new Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(1, 2, 3);
				Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived b = new Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(1, 2, 3);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived-derived reference type with operators of derived only results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Base without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperators_Base a = new Types.ReferenceTypeIEquatableWithoutOperators_Base(1);
				Types.ReferenceTypeIEquatableWithoutOperators_Base b = new Types.ReferenceTypeIEquatableWithoutOperators_Base(1);
				bool result = (a == b);
				Trace.WriteLine("a == b of base reference type without operators results in " + result);
			}
			Trace.WriteLine("");
			Trace.WriteLine("Derived without operators");
			{
				Types.ReferenceTypeIEquatableWithoutOperators_Derived a = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(1, 2);
				Types.ReferenceTypeIEquatableWithoutOperators_Derived b = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(1, 2);
				bool result = (a == b);
				Trace.WriteLine("a == b of derived reference type without operators results in " + result);
			}
			Trace.Unindent();
			Trace.Unindent();
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
			DateTime objToEqualAgainst = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objEqual          = new DateTime(2000, 1, 1, 12, 0, 0);
			DateTime objNotEqual       = new DateTime(2001, 2, 3, 13, 4, 5);

			Methods.Generic.TestEquals<object>  (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<DateTime>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeDateTime.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ValueTypeDateTime.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeNotIEquatableWithOperators()
		{
			Types.ValueTypeNotIEquatableWithOperators objToEqualAgainst = new Types.ValueTypeNotIEquatableWithOperators(1);
			Types.ValueTypeNotIEquatableWithOperators objEqual          = new Types.ValueTypeNotIEquatableWithOperators(1);
			Types.ValueTypeNotIEquatableWithOperators objNotEqual       = new Types.ValueTypeNotIEquatableWithOperators(2);

			Methods.Generic.TestEquals<object>                                   (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeNotIEquatableWithOperators>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeNotIEquatableWithOperators.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ValueTypeNotIEquatableWithOperators.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeIEquatableWithOperators()
		{
			Types.ValueTypeIEquatableWithOperators objToEqualAgainst = new Types.ValueTypeIEquatableWithOperators(1);
			Types.ValueTypeIEquatableWithOperators objEqual          = new Types.ValueTypeIEquatableWithOperators(1);
			Types.ValueTypeIEquatableWithOperators objNotEqual       = new Types.ValueTypeIEquatableWithOperators(2);

			Methods.Generic.TestEquals<object>                                (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeIEquatableWithOperators>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Methods.ValueTypeIEquatableWithOperators.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ValueTypeIEquatableWithOperators.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
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
			Types.ValueTypeNotIEquatableWithoutOperators objToEqualAgainst = new Types.ValueTypeNotIEquatableWithoutOperators(1);
			Types.ValueTypeNotIEquatableWithoutOperators objEqual          = new Types.ValueTypeNotIEquatableWithoutOperators(1);
			Types.ValueTypeNotIEquatableWithoutOperators objNotEqual       = new Types.ValueTypeNotIEquatableWithoutOperators(2);

			Methods.Generic.TestEquals<object>                                      (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeNotIEquatableWithoutOperators>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Assert.Ignore("Operator ==/!= cannot be applied to value types without operators, neither to evaluate reference nor value equality.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnValueTypeIEquatableWithoutOperators()
		{
			Types.ValueTypeIEquatableWithoutOperators objToEqualAgainst = new Types.ValueTypeIEquatableWithoutOperators(1);
			Types.ValueTypeIEquatableWithoutOperators objEqual          = new Types.ValueTypeIEquatableWithoutOperators(1);
			Types.ValueTypeIEquatableWithoutOperators objNotEqual       = new Types.ValueTypeIEquatableWithoutOperators(2);

			Methods.Generic.TestEquals<object>                                   (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ValueTypeIEquatableWithoutOperators>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ValueTypeBase.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// Operator ==/!= cannot be used on System.ValueType itself.
			Assert.Ignore("Operator ==/!= cannot be applied to value types without operators, neither to evaluate reference nor value equality.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
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
			var objToEqualAgainst = new Version(1, 1);
			var objEqual          = new Version(1, 1);
			var objNotEqual       = new Version(1, 2);

			Methods.Generic.TestEquals<object> (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Version>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeVersion.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeVersion.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionIEquatableWithOperators()
		{
			var objToEqualAgainst = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();
			var objEqual          = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();
			var objNotEqual       = new Types.ReferenceTypeCollectionIEquatableWithOperators<int>();

			objToEqualAgainst.AddRange(new int[] { 1, 1 });
			objEqual         .AddRange(new int[] { 1, 1 });
			objNotEqual      .AddRange(new int[] { 1, 2 });

			Methods.Generic.TestEquals<object>                                                   (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeCollectionIEquatableWithOperators<int>>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeCollectionIEquatableWithOperators.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeCollectionIEquatableWithOperators.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Base(int b1, int b2, int b3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                            (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperators_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperators_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeNotIEquatableWithOperators_Derived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperators_DerivedCastedToBase(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                            (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeNotIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                         (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeNotIEquatableWithOperatorsOfBaseOnly_Derived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_DerivedDerived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(int b1, int d1, int dd1, int b2, int d2, int dd2, int b3, int d3, int dd3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b1, d1, dd1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b2, d2, dd2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b3, d3, dd3);

			Methods.Generic.TestEquals<object>                                                                   (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeNotIEquatableWithOperatorsOfDerivedOnly_DerivedDerived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperators_Base(int b1, int b2, int b3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithOperators_Base(b1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithOperators_Base(b2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                         (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                            (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperators_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperators_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeIEquatableWithOperators_Derived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperators_DerivedCastedToBase(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                         (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeIEquatableWithOperators_Base.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                      (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeIEquatableWithOperatorsOfBaseOnly_Derived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_DerivedDerived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(int b1, int d1, int dd1, int b2, int d2, int dd2, int b3, int d3, int dd3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b1, d1, dd1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b2, d2, dd2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived(b3, d3, dd3);

			Methods.Generic.TestEquals<object>                                                                (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Methods.ReferenceTypeIEquatableWithOperatorsOfDerivedOnly_DerivedDerived.TestOperatorsForValueEquality    (objToEqualAgainst, objEqual, objNotEqual);
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
			var objToEqualAgainst = new List<int>(2); // Preset the required capacity to improve memory management.
			var objEqual          = new List<int>(2); // Preset the required capacity to improve memory management.
			var objNotEqual       = new List<int>(2); // Preset the required capacity to improve memory management.

			objToEqualAgainst.AddRange(new int[] { 1, 1 });
			objEqual         .AddRange(new int[] { 1, 1 });
			objNotEqual      .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// List<int>.Equals() doesn't exist, and object.Equals() only evaluates reference equality.

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("List<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeSystemReferenceTypeIEquatableWithoutOperators()
		{
			var objToEqualAgainst = new IPAddress(new byte[] { 0, 1, 2, 3 });
			var objEqual          = new IPAddress(new byte[] { 0, 1, 2, 3 });
			var objNotEqual       = new IPAddress(new byte[] { 0, 1, 2, 9 });

			Methods.Generic.TestEquals<object>   (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<IPAddress>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIPAddress.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("IPAddress.operators ==/!= don't exist, thanks Microsoft guys...");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionNotIEquatableWithoutOperators()
		{
			var objToEqualAgainst = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();
			var objEqual          = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();
			var objNotEqual       = new Types.ReferenceTypeCollectionNotIEquatableWithoutOperators<int>();

			objToEqualAgainst.AddRange(new int[] { 1, 1 });
			objEqual         .AddRange(new int[] { 1, 1 });
			objNotEqual      .AddRange(new int[] { 1, 2 });

			// object.Equals() only evaluates reference equality.
			// EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.Equals() doesn't exist, and List<int>.Equals() only evaluates reference equality.

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("EqualityTestData.OwnDerivedCollectionReferenceTypeWithIEquatableTypeWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test]
		public virtual void AnalyzeOwnReferenceTypeCollectionIEquatableWithoutOperators()
		{
			var objToEqualAgainst = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();
			var objEqual          = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();
			var objNotEqual       = new Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>();

			objToEqualAgainst.AddRange(new int[] { 1, 1 });
			objEqual         .AddRange(new int[] { 1, 1 });
			objNotEqual      .AddRange(new int[] { 1, 2 });

			Methods.Generic.TestEquals<object>                                                      (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeCollectionIEquatableWithoutOperators<int>>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeList.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("EqualityTestData.OwnDerivedCollectionReferenceTypeIEquatableWithoutOperators<int>.operators ==/!= don't exist, and object.operators ==/!= only evaluate reference equality.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Base(int b1, int b2, int b3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithoutOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                               (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                                  (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperators_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperators_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeNotIEquatableWithoutOperators_DerivedCastedToBase(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeNotIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeNotIEquatableWithoutOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeNotIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Base")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Base(int b1, int b2, int b3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithoutOperators_Base(b3);

			Methods.Generic.TestEquals<object>                                            (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperators_Derived(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                               (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperators_Derived>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperators_Derived.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(Data), "TestCases_Derived")]
		public virtual void AnalyzeOwnReferenceTypeIEquatableWithoutOperators_DerivedCastedToBase(int b1, int d1, int b2, int d2, int b3, int d3)
		{
			var objToEqualAgainst = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b1, d1);
			var objEqual          = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b2, d2);
			var objNotEqual       = new Types.ReferenceTypeIEquatableWithoutOperators_Derived(b3, d3);

			Methods.Generic.TestEquals<object>                                            (objToEqualAgainst, objEqual, objNotEqual);
			Methods.Generic.TestEquals<Types.ReferenceTypeIEquatableWithoutOperators_Base>(objToEqualAgainst, objEqual, objNotEqual);

			Methods.ReferenceTypeObject.TestOperatorsForReferenceEquality(objToEqualAgainst);
			//// object.operators ==/!= only evaluate reference equality.
			Methods.ReferenceTypeIEquatableWithoutOperators_Base.TestOperatorsForReferenceEquality(objToEqualAgainst);
			Assert.Ignore("object.operators ==/!= are called, thus only reference equality is evaluated.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
