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

using System.Collections;

using NUnit.Framework;

namespace MKY.Test.Equality
{
	/// <summary></summary>
	public static class Data
	{
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
