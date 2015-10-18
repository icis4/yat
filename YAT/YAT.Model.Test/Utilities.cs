//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY;
using MKY.Collections.Generic;
using MKY.Net;

using NUnit.Framework;

using YAT.Settings.Terminal;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		public delegate TerminalSettingsRoot TerminalSettingsDelegate<T>(T arg);

		/// <summary></summary>
		public static class TransmissionSettings
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> SerialPortLoopbackPairs
			{
				get
				{
					foreach (MKY.IO.Ports.Test.SerialPortPairConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairs)
					{
						TerminalSettingsDelegate<string> tsm = new TerminalSettingsDelegate<string>(GetStartedTextSerialPortSettings);
						Pair<TerminalSettingsDelegate<string>, string> pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortA);
						Pair<TerminalSettingsDelegate<string>, string> pB = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortB);
						string name = "SerialPortLoopbackPairs_" + ce.PortA + "_" + ce.PortB;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> SerialPortLoopbackSelfs
			{
				get
				{
					foreach (MKY.IO.Ports.Test.SerialPortConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfs)
					{
						TerminalSettingsDelegate<string> tsm = new TerminalSettingsDelegate<string>(GetStartedTextSerialPortSettings);
						Pair<TerminalSettingsDelegate<string>, string> pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.Port);
						Pair<TerminalSettingsDelegate<string>, string> pB = new Pair<TerminalSettingsDelegate<string>, string>(null, null);
						string name = "SerialPortLoopbackSelf_" + ce.Port;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> IPLoopbacks
			{
				get
				{
					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));
				}
			}
		}

		/// <summary></summary>
		/// <remarks>
		/// \todo:
		/// This test set struct should be improved such that it can also handle expectations on the
		/// sender side (i.e. terminal A). Rationale: Testing of \!(Clear) behavior.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This struct really belongs to these test utilities only.")]
		public struct TestSet
		{
			private Model.Types.Command command;
			private int   expectedLineCount;
			private int[] expectedElementCounts;
			private int[] expectedDataCounts;
			private bool  expectedAlsoApplyToA;

			/// <summary></summary>
			public TestSet(Model.Types.Command command)
			{
				this.command = command;
				this.expectedLineCount = command.CommandLines.Length;

				this.expectedElementCounts = new int[this.expectedLineCount];
				this.expectedDataCounts    = new int[this.expectedLineCount];
				for (int i = 0; i < this.expectedLineCount; i++)
				{
					this.expectedElementCounts[i] = 2; // 1 data element + 1 Eol element.
					this.expectedDataCounts[i]    = command.CommandLines[i].Length;
				}

				this.expectedAlsoApplyToA = false;
			}

			/// <summary></summary>
			public TestSet(Model.Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedDataCounts, bool expectedAlsoApplyToA)
			{
				this.command = command;
				this.expectedLineCount     = expectedLineCount;
				this.expectedElementCounts = expectedElementCounts;
				this.expectedDataCounts    = expectedDataCounts;
				this.expectedAlsoApplyToA  = expectedAlsoApplyToA;
			}

			/// <summary></summary>
			public Model.Types.Command Command
			{
				get { return (this.command); }
			}

			/// <summary></summary>
			public int ExpectedLineCount
			{
				get { return (this.expectedLineCount); }
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedElementCounts
			{
				get { return (this.expectedElementCounts); }
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedDataCounts
			{
				get { return (this.expectedDataCounts); }
			}

			/// <summary></summary>
			public bool ExpectedAlsoApplyToA
			{
				get { return (this.expectedAlsoApplyToA); }
			}

			#region Object Members

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override bool Equals(object obj)
			{
				if (ReferenceEquals(obj, null))
					return (false);

				if (GetType() != obj.GetType())
					return (false);

				TestSet other = (TestSet)obj;
				return
				(
					(Command               == other.Command) &&
					(ExpectedLineCount     == other.ExpectedLineCount) &&
					(ExpectedElementCounts == other.ExpectedElementCounts) &&
					(ExpectedDataCounts    == other.ExpectedDataCounts) &&
					(ExpectedAlsoApplyToA  == other.ExpectedAlsoApplyToA)
				);
			}

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override int GetHashCode()
			{
				return
				(
					Command              .GetHashCode() ^
					ExpectedLineCount    .GetHashCode() ^
					ExpectedElementCounts.GetHashCode() ^
					ExpectedDataCounts   .GetHashCode() ^
					ExpectedAlsoApplyToA .GetHashCode()
				);
			}

			#endregion

			#region Comparison Operators

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestSet lhs, TestSet rhs)
			{
				// Value type implementation of operator ==.
				// See MKY.Test.EqualityTest for details.

				if (ReferenceEquals(lhs, rhs))  return (true);
				if (ReferenceEquals(lhs, null)) return (false);
				if (ReferenceEquals(rhs, null)) return (false);

				return (lhs.Equals(rhs));
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(TestSet lhs, TestSet rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int WaitInterval = 100;
		private const int WaitTimeout = 10000;
		private const int EolWaitInterval = 1000;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > Dedicated
		//------------------------------------------------------------------------------------------
		// Settings > Dedicated
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTextSerialPortSettings(string portId)
		{
			return (GetStartedTextSerialPortSettings((MKY.IO.Ports.SerialPortId)portId));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortSettings(MKY.IO.Ports.SerialPortId portId)
		{
			// Create settings:
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.SerialPort;
			settings.Terminal.IO.SerialPort.PortId = portId;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		internal static TerminalSettingsRoot GetStartedTextPortASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA));

			Assert.Ignore("'PortA' is not available, therefore this test is ignored. Ensure that 'PortA' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextPortASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextPortASettings());
		}

		internal static TerminalSettingsRoot GetStartedTextPortBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB));

			Assert.Ignore("'PortB' is not available, therefore this test is ignored. Ensure that 'PortB' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextPortBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextPortBSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA));

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is ignored. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextMTSicsDeviceASettings());
		}

		internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB));
		
			Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is ignored. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextMTSicsDeviceBSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketSettings(string networkInterface)
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterface)networkInterface));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketSettings(IPNetworkInterface networkInterface)
		{
			// Create settings:
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.TcpAutoSocket;
			settings.Terminal.IO.Socket.LocalInterface = networkInterface;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterface)IPNetworkInterfaceType.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterface)IPNetworkInterfaceType.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is ignored. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is ignored. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings());
		}

		#endregion

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForConnection(Model.Terminal terminalA)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail("Connect timeout!");
			}
			while (!terminalA.IsConnected);
		}

		internal static void WaitForConnection(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail("Connect timeout!");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);
		}

		internal static void WaitForTransmission(Model.Terminal terminalA, Model.Terminal terminalB, TestSet testSet)
		{
			WaitForTransmission(terminalA, terminalB, testSet.ExpectedLineCount);
		}

		internal static void WaitForTransmission(Model.Terminal terminalA, Model.Terminal terminalB, int expectedLineCountB)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				timeout += WaitInterval;

				if (timeout >= WaitTimeout)
					Assert.Fail("Transmission timeout! Try to re-run test case.");
			}
			while ((terminalB.RxByteCount != terminalA.TxByteCount) &&
			       (terminalB.RxLineCount != terminalA.TxLineCount) &&
				   (terminalB.RxLineCount != expectedLineCountB));

			// Wait to allow Eol to be sent (Eol is sent a bit later than line contents).
			Thread.Sleep(EolWaitInterval);
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		internal static void VerifyLines(List<Domain.DisplayLine> linesA, List<Domain.DisplayLine> linesB, TestSet testSet)
		{
			VerifyLines(linesA, linesB, testSet, 1);
		}

		internal static void VerifyLines(List<Domain.DisplayLine> linesA, List<Domain.DisplayLine> linesB, TestSet testSet, int cycle)
		{
			// Compare the expected line count at the receiver side.
			int  expectedLineCount       = (testSet.ExpectedLineCount * cycle);
			bool expectedLineCountMatchB = (linesB.Count == expectedLineCount);

			// If both sides are expected to show the same line count, compare the counts.
			// Otherwise, ignore the comparision.
			bool expectedLineCountMatchAB;
			if (testSet.ExpectedAlsoApplyToA)
				expectedLineCountMatchAB = (linesB.Count == linesA.Count);
			else
				expectedLineCountMatchAB = true;

			if (expectedLineCountMatchAB && expectedLineCountMatchB)
			{
				for (int i = 0; i < linesA.Count; i++)
				{
					int commandIndex         = i % testSet.ExpectedLineCount;
					int expectedElementCount =     testSet.ExpectedElementCounts[commandIndex];
					int expectedDataCount    =     testSet.ExpectedDataCounts[commandIndex];

					Domain.DisplayLine lineA = linesA[i];
					Domain.DisplayLine lineB = linesB[i];

					if ((lineB.Count     == lineA.Count) &&
						(lineB.Count     == expectedElementCount) &&
						(lineB.DataCount == lineA.DataCount) &&
						(lineB.DataCount == expectedDataCount))
					{
						for (int j = 0; j < lineA.Count; j++)
							Assert.AreEqual(lineA[j].Text, lineB[j].Text);
					}
					else
					{
						string strA = ArrayEx.ElementsToString(lineA.ToArray());
						string strB = ArrayEx.ElementsToString(lineB.ToArray());

						Console.Error.Write
						(
							"A:" + Environment.NewLine + strA + Environment.NewLine +
							"B:" + Environment.NewLine + strB + Environment.NewLine
						);

						Assert.Fail
						(
							"Length of line " + i + " mismatches:" + Environment.NewLine +
							"Expected = " + expectedElementCount + " elements, " +
							"A = " + lineA.Count + " elements, " +
							"B = " + lineB.Count + " elements," + Environment.NewLine +
							"Expected = " + expectedDataCount + " data, " +
							"A = " + lineA.DataCount + " data, " +
							"B = " + lineB.DataCount + " data." + Environment.NewLine +
							@"See ""Output"" for details."
						);
					}
				}
			}
			else
			{
				StringBuilder sbA = new StringBuilder();
				StringBuilder sbB = new StringBuilder();

				foreach (Domain.DisplayLine lineA in linesA)
					sbA.Append(ArrayEx.ElementsToString(lineA.ToArray()));
				foreach (Domain.DisplayLine lineB in linesB)
					sbB.Append(ArrayEx.ElementsToString(lineB.ToArray()));

				Console.Error.Write
				(
					"A:" + Environment.NewLine + sbA + Environment.NewLine +
					"B:" + Environment.NewLine + sbB + Environment.NewLine
				);

				Assert.Fail
				(
					"Line count mismatches: " + Environment.NewLine +
					"Expected = " + expectedLineCount + " lines, " +
					"A = " + linesA.Count + " lines, " +
					"B = " + linesB.Count + " lines." + Environment.NewLine +
					@"See ""Output"" for details."
				);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
