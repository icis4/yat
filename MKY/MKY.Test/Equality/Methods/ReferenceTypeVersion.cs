﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;

using MKY.Diagnostics;

using NUnit.Framework;

namespace MKY.Test.Equality.Methods
{
	/// <summary></summary>
	public static class ReferenceTypeVersion
	{
		/// <summary></summary>
		public static void TestOperatorsForReferenceEquality(Version obj)
		{
			if (Configuration.TraceCallingSequence) {
				Trace.Indent();
				TraceEx.WriteLocation();
				Trace.Indent();
			}

			try
			{
				// Reference equal:

				#pragma warning disable 1718 // Disable "Comparison made to same variable; did you mean to compare something else?"

				if (Configuration.TraceCallingSequence) {
					Trace.WriteLine("Reference equal using operator ==()");
					Trace.Indent();
				}

				if (!(obj == obj))
					Assert.Fail("Reference equal objects are not considered equal using operator ==()");

				if (Configuration.TraceCallingSequence) {
					Trace.Unindent();
					Trace.WriteLine("Reference equal using operator !=()");
					Trace.Indent();
				}

				if (obj != obj)
					Assert.Fail("Reference equal objects are not considered not equal using operator !=()");

				if (Configuration.TraceCallingSequence)
					Trace.Unindent();

				#pragma warning restore 1718
			}
			catch (AssertionException)
			{
				if (Configuration.TraceCallingSequence)
					Trace.Unindent();

				throw; // Re-throw!
			}
			finally
			{
				if (Configuration.TraceCallingSequence) {
					Trace.Unindent();
					Trace.Unindent();
				}
			}
		}

		/// <summary></summary>
		public static void TestOperatorsForValueEquality(Version objToEqualAgainst, Version objEqual, Version objNotEqual)
		{
			Trace.Indent();
			TraceEx.WriteLocation();
			Trace.Indent();

			try
			{
				// Value equal:

				Trace.WriteLine("Value equal using operator ==()");
				Trace.Indent();

				if (!(objToEqualAgainst == objEqual))
					Assert.Fail("Value equal objects are not considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value equal using operator !=()");
				Trace.Indent();

				if (objToEqualAgainst != objEqual)
					Assert.Fail("Value equal objects are not considered not equal using operator !=()");

				Trace.Unindent();

				// Value not equal:

				Trace.WriteLine("Value not equal using operator ==()");
				Trace.Indent();

				if (objToEqualAgainst == objNotEqual)
					Assert.Fail("Value not equal objects are considered equal using operator ==()");

				Trace.Unindent();
				Trace.WriteLine("Value not equal using operator !=()");
				Trace.Indent();

				if (!(objToEqualAgainst != objNotEqual))
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
