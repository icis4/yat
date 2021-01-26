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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;
using MKY.Test.Equality;
using MKY.Test.Equality.Methods;

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Settings.Model.Test
{
	/// <summary></summary>
	[TestFixture]
	public class EqualityTest
	{
		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());

			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		/// <summary></summary>
		[Test]
		public virtual void TestTerminalSettingsEquality()
		{
			var objToEqualAgainst = new TerminalSettingsRoot();
			var objEqual          = new TerminalSettingsRoot();
			var objNotEqual       = new TerminalSettingsRoot();

			// Change a single setting, somewhere deep in the tree:
			objNotEqual.IO.SerialPort.Communication.DataBits = MKY.IO.Ports.DataBits.Seven;

			// Perform equality test:
			TestEqualsWithObjectAndType                (objToEqualAgainst, objEqual, objNotEqual);
			TestOperatorsForReferenceEqualityWithObject(objToEqualAgainst);
			TestOperatorsForReferenceEquality          (objToEqualAgainst);
			TestOperatorsForValueEquality              (objToEqualAgainst, objEqual, objNotEqual);

			// Restore the setting, but change the settings at the other objects instead:
			objToEqualAgainst.IO.SerialPort.Communication.DataBits = MKY.IO.Ports.DataBits.Seven;
			objEqual         .IO.SerialPort.Communication.DataBits = MKY.IO.Ports.DataBits.Seven;
			objNotEqual      .IO.SerialPort.Communication.DataBits = MKY.IO.Ports.DataBitsEx.Default;

			// Perform equality test again:
			TestEqualsWithObjectAndType                (objToEqualAgainst, objEqual, objNotEqual);
			TestOperatorsForReferenceEqualityWithObject(objToEqualAgainst);
			TestOperatorsForReferenceEquality          (objToEqualAgainst);
			TestOperatorsForValueEquality              (objToEqualAgainst, objEqual, objNotEqual);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static void TestEqualsWithObjectAndType<T>(T objToEqualAgainst, T objEqual, T objNotEqual)
		{
			Generic.TestEquals<object>(objToEqualAgainst, objEqual, objNotEqual);
			Generic.TestEquals<T>     (objToEqualAgainst, objEqual, objNotEqual);
		}

		private static void TestOperatorsForReferenceEqualityWithObject(object obj)
		{
			ReferenceTypeObject.TestOperatorsForReferenceEquality(obj);
		}

		private static void TestOperatorsForReferenceEquality(TerminalSettingsRoot obj)
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

		private static void TestOperatorsForValueEquality(TerminalSettingsRoot objToEqualAgainst, TerminalSettingsRoot objEqual, TerminalSettingsRoot objNotEqual)
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
				throw; // Rethrow!
			}
			finally
			{
				Trace.Unindent();
				Trace.Unindent();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
