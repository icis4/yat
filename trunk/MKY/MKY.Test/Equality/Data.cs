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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace MKY.Test.Equality
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Data
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the hierarchy.")]
		public static IEnumerable TestCases_Base
		{
			get
			{
				yield return (new TestCaseData(1, 1, 2).SetName("Equal : B == B / Not Equal : B != B"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the hierarchy.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the hierarchy.")]
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
