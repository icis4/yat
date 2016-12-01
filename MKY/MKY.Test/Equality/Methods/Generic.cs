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

using System.Diagnostics;

using MKY.Diagnostics;

using NUnit.Framework;

namespace MKY.Test.Equality.Methods
{
	internal static class Generic
	{
		public static void TestEquals<T>(T objToCompareAgainst, T objEqual, T objNotEqual)
		{
			Trace.Indent();
			TraceEx.WriteLocation();
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
