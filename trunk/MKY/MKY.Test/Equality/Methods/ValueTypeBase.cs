﻿//==================================================================================================
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using MKY.Diagnostics;

using NUnit.Framework;

namespace MKY.Test.Equality.Methods
{
	internal static class ValueTypeBase
	{
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objEqual", Justification = Helper.UnusedParameterSuppressionJustification)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "objNotEqual", Justification = Helper.UnusedParameterSuppressionJustification)]
		public static void TestOperatorsForReferenceEquality(ValueType objToCompareAgainst, ValueType objEqual, ValueType objNotEqual)
		{
			Trace.Indent();
			TraceEx.WriteLocation();
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

		// TestOperatorsForValueEquality() is useless since it never succeeds.
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
