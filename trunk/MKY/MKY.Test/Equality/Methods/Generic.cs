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
// MKY Version 1.0.29
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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using MKY.Diagnostics;

using NUnit.Framework;

namespace MKY.Test.Equality.Methods
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Generic
	{
		/// <typeparam name="T">The type being tested.</typeparam>
		public static void TestEquals<T>(T objToEqualAgainst, T objEqual, T objNotEqual)
		{
			if (Configuration.TraceCallingSequence) {
				Trace.Indent();
				TraceEx.WriteLocation();
				Trace.Indent();
			}

			try
			{
				// Reference equal:

				if (Configuration.TraceCallingSequence) {
					Trace.WriteLine("Reference equal using Equals()");
					Trace.Indent();
				}

				if (!objToEqualAgainst.Equals(objToEqualAgainst))
					Assert.Fail("Reference equal objects are not considered equal using Equals()");

				if (Configuration.TraceCallingSequence)
					Trace.Unindent();

				// Value equal:

				if (Configuration.TraceCallingSequence) {
					Trace.WriteLine("Value equal using Equals()");
					Trace.Indent();
				}

				if (!objToEqualAgainst.Equals(objEqual))
					Assert.Fail("Value equal objects are not considered equal using Equals()");

				if (Configuration.TraceCallingSequence)
					Trace.Unindent();

				// Value not equal:

				if (Configuration.TraceCallingSequence) {
					Trace.WriteLine("Value not equal using Equals()");
					Trace.Indent();
				}

				if (objToEqualAgainst.Equals(objNotEqual))
					Assert.Fail("Value not equal objects are considered equal using Equals()");

				if (Configuration.TraceCallingSequence)
					Trace.Unindent();
			}
			catch (AssertionException)
			{
				if (Configuration.TraceCallingSequence)
					Trace.Unindent();

				throw; // Rethrow!
			}
			finally
			{
				if (Configuration.TraceCallingSequence) {
					Trace.Unindent();
					Trace.Unindent();
				}
			}
		}

		// Note that generic methods like TestOperatorsForValueEquality<T>() where T : class and
		// TestOperatorsForReferenceEquality<T>() where T : class would be possible to implement but
		// would not work as expected since .NET does not call overloaded operators, as explained at
		// http://stackoverflow.com/questions/390900/cant-operator-be-applied-to-generic-types-in-c:
		//
		// .NET generics do not act like C++ templates. In C++ templates, overload resolution occurs
		// after the actual template parameters are known. In .NET generics (including C#), overload
		// resolution occurs without knowing the actual generic parameters. The only information the
		// compiler can use to choose the function to call comes from type constraints on the
		// generic parameters.
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
