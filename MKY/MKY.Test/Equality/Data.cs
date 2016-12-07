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
		public static IEnumerable TestCases_Base
		{
			get
			{
				yield return (new TestCaseData(1, 1, 2).SetName("Equal : B == B / Not Equal : B != B"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases_Derived
		{
			get
			{
				yield return (new TestCaseData(1, 1, 1, 1, 2, 2).SetName("Equal : B == D / Not Equal : B == D"));

				yield return (new TestCaseData(1, 1, 1, 1, 1, 2).SetName("Equal : B == D / Not Equal : B != D (D)"));
				yield return (new TestCaseData(2, 2, 2, 2, 1, 2).SetName("Equal : B == D / Not Equal : B != D (B)"));
				yield return (new TestCaseData(1, 1, 1, 1, 2, 1).SetName("Equal : B == D / Not Equal : B != D (B)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 1).SetName("Equal : B == D / Not Equal : B != D (D)"));

				yield return (new TestCaseData(1, 2, 1, 2, 1, 1).SetName("Equal : B != D (D) / Not Equal : B == D"));
				yield return (new TestCaseData(1, 2, 1, 2, 2, 2).SetName("Equal : B != D (B) / Not Equal : B == D"));
				yield return (new TestCaseData(2, 1, 2, 1, 1, 1).SetName("Equal : B != D (B) / Not Equal : B == D"));
				yield return (new TestCaseData(2, 1, 2, 1, 2, 2).SetName("Equal : B != D (D) / Not Equal : B == D"));

				yield return (new TestCaseData(1, 2, 1, 2, 2, 1).SetName("Equal : B != D (D) / Not Equal : B != D (B)"));
				yield return (new TestCaseData(2, 1, 2, 1, 1, 2).SetName("Equal : B != D (B) / Not Equal : B != D (D)"));
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases_DerivedDerived
		{
			get
			{
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 2, 2, 2).SetName("Equal : B == D == DD / Not Equal : B == D == DD"));

				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 1, 2, 2).SetName("Equal : B == D == DD / Not Equal : B != D == DD (D)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 1, 2, 2).SetName("Equal : B == D == DD / Not Equal : B != D == DD (B)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 1, 2, 2).SetName("Equal : B == D == DD / Not Equal : B != D == DD (B, D)"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 2, 1, 1).SetName("Equal : B == D == DD / Not Equal : B != D == DD (B)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 2, 1, 1).SetName("Equal : B == D == DD / Not Equal : B != D == DD (D)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 2, 1, 1).SetName("Equal : B == D == DD / Not Equal : B != D == DD (B, D)"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 1, 1, 2).SetName("Equal : B == D == DD / Not Equal : B == D != DD (DD)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 1, 1, 2).SetName("Equal : B == D == DD / Not Equal : B == D != DD (D)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 1, 1, 2).SetName("Equal : B == D == DD / Not Equal : B == D != DD (D, DD)"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 2, 2, 1).SetName("Equal : B == D == DD / Not Equal : B == D != DD (D)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 2, 2, 1).SetName("Equal : B == D == DD / Not Equal : B == D != DD (DD)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 2, 2, 1).SetName("Equal : B == D == DD / Not Equal : B == D != DD (D, DD)"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 1, 2, 3).SetName("Equal : B == D == DD / Not Equal : B != D != DD (D, DD)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 1, 2, 3).SetName("Equal : B == D == DD / Not Equal : B != D != DD (B, DD)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 1, 2, 3).SetName("Equal : B == D == DD / Not Equal : B != D != DD (B, D)"));
				yield return (new TestCaseData(1, 1, 1, 1, 1, 1, 3, 2, 1).SetName("Equal : B == D == DD / Not Equal : B != D != DD (B, D)"));
				yield return (new TestCaseData(2, 2, 2, 2, 2, 2, 3, 2, 1).SetName("Equal : B == D == DD / Not Equal : B != D != DD (B, DD)"));
				yield return (new TestCaseData(3, 3, 3, 3, 3, 3, 3, 2, 1).SetName("Equal : B == D == DD / Not Equal : B != D != DD (D, DD)"));

				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 1, 1, 1).SetName("Equal : B != D == DD (D)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 2, 2, 2).SetName("Equal : B != D == DD (B)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 3, 3, 3).SetName("Equal : B != D == DD (B, D)  / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 1, 1, 2, 1, 1, 1, 1, 1).SetName("Equal : B != D == DD (B)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 1, 1, 2, 1, 1, 2, 2, 2).SetName("Equal : B != D == DD (D)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 1, 1, 2, 1, 1, 3, 3, 3).SetName("Equal : B != D == DD (B, D)  / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 1, 1, 1).SetName("Equal : B == D != DD (DD)    / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 2, 2, 2).SetName("Equal : B == D != DD (D)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 3, 3, 3).SetName("Equal : B == D != DD (D, DD) / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 2, 1, 2, 2, 1, 1, 1, 1).SetName("Equal : B == D != DD (D)     / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 2, 1, 2, 2, 1, 2, 2, 2).SetName("Equal : B == D != DD (DD)    / Not Equal : B == D == DD"));
				yield return (new TestCaseData(2, 2, 1, 2, 2, 1, 3, 3, 3).SetName("Equal : B == D != DD (D, DD) / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 1, 1, 1).SetName("Equal : B != D != DD (D, DD) / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 2, 2, 2).SetName("Equal : B != D != DD (B, DD) / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 3, 3).SetName("Equal : B != D != DD (B, D)  / Not Equal : B == D == DD"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 1, 1, 1).SetName("Equal : B != D != DD (B, D)  / Not Equal : B == D == DD"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 2, 2, 2).SetName("Equal : B != D != DD (B, DD) / Not Equal : B == D == DD"));
				yield return (new TestCaseData(3, 2, 1, 3, 2, 1, 3, 3, 3).SetName("Equal : B != D != DD (D, DD) / Not Equal : B == D == DD"));

				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 3, 3, 3).SetName("Equal : B != D == DD / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 3, 2, 2).SetName("Equal : B != D == DD / Not Equal : B != D == DD"));
				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 3, 3, 2).SetName("Equal : B != D == DD / Not Equal : B == D != DD"));
				yield return (new TestCaseData(1, 2, 2, 1, 2, 2, 3, 2, 1).SetName("Equal : B != D == DD / Not Equal : B != D != DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 3, 3, 3).SetName("Equal : B == D != DD / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 3, 2, 2).SetName("Equal : B == D != DD / Not Equal : B != D == DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 3, 3, 2).SetName("Equal : B == D != DD / Not Equal : B == D != DD"));
				yield return (new TestCaseData(1, 1, 2, 1, 1, 2, 3, 2, 1).SetName("Equal : B == D != DD / Not Equal : B != D != DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 3, 3).SetName("Equal : B != D != DD / Not Equal : B == D == DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 2, 2).SetName("Equal : B != D != DD / Not Equal : B != D == DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 3, 2).SetName("Equal : B != D != DD / Not Equal : B == D != DD"));
				yield return (new TestCaseData(1, 2, 3, 1, 2, 3, 3, 2, 1).SetName("Equal : B != D != DD / Not Equal : B != D != DD"));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
