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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY;
using MKY.Net;
using MKY.Settings;

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
		/// <remarks>
		/// \todo:
		/// This test set class should be improved such that it can also handle expectations on the
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

		internal static TerminalSettingsRoot GetStartedTextSerialPortSettings(MKY.IO.Ports.SerialPortId portId)
		{
			// Create settings
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.SerialPort;
			settings.Terminal.IO.SerialPort.PortId = portId;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortSettingsHandler(MKY.IO.Ports.SerialPortId portId)
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortSettings(portId)));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.SerialPortAIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.SerialPortA));

			Assert.Ignore("'SerialPortA' is not available, therefore this test is ignored. Ensure that 'SerialPortA' is properly configured and available if passing this test is required.");
			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortASettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortASettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.SerialPortBIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.SerialPortB));

			Assert.Ignore("'SerialPortB' is not available, therefore this test is ignored. Ensure that 'SerialPortB' is properly configured and available if passing this test is required.");
			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortBSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortBSettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA));

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is ignored. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			return (null);
		}

	////internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextMTSicsDeviceASettingsHandler()
	////{
	////	return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextMTSicsDeviceASettings()));
	////}

	////internal static TerminalSettingsRoot GetStartedTextMTSicsDeviceBSettings()
	////{
	////	if (MKY.IO.Ports.Test.SettingsProvider.Settings.MTSicsDeviceBIsConnected)
	////		return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.SettingsProvider.Settings.MTSicsDeviceB));
	////
	////	Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is ignored. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
	////	return (null);
	////}

	////internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextMTSicsDeviceBSettingsHandler()
	////{
	////	return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextMTSicsDeviceBSettings()));
	////}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketSettings(IPNetworkInterface networkInterface)
		{
			// Create settings
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.TcpAutoSocket;
			settings.Terminal.IO.Socket.LocalInterface = networkInterface;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketSettingsHandler(IPNetworkInterface networkInterface)
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketSettings(networkInterface)));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterface)IPNetworkInterfaceType.IPv4Loopback));
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnIPv4LoopbackSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterface)IPNetworkInterfaceType.IPv6Loopback));
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnIPv6LoopbackSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.SpecificIPv4InterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.SpecificIPv4Interface));

			Assert.Ignore("'SpecificIPv4Interface' is not available, therefore this test is ignored. Ensure that 'SpecificIPv4Interface' is properly configured and available if passing this test is required.");
			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings()));
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.SpecificIPv6InterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.SpecificIPv6Interface));

			Assert.Ignore("'SpecificIPv6Interface' is not available, therefore this test is ignored. Ensure that 'SpecificIPv6Interface' is properly configured and available if passing this test is required.");
			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings()));
		}

		#endregion

		#region Settings > Combined
		//------------------------------------------------------------------------------------------
		// Settings > Combined
		//------------------------------------------------------------------------------------------

		internal static void GetStartedTextSettings(TransmissionType type, out TerminalSettingsRoot settingsA, out TerminalSettingsRoot settingsB)
		{
			switch (type)
			{
				case TransmissionType.SerialPort:
					settingsA = GetStartedTextSerialPortASettings();
					settingsB = GetStartedTextSerialPortBSettings();
					break;

				case TransmissionType.TcpAutoSocketOnIPv4Loopback:
					settingsA = GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings();
					settingsB = GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings();
					break;

				case TransmissionType.TcpAutoSocketOnIPv6Loopback:
					settingsA = GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings();
					settingsB = GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings();
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv4Interface:
					settingsA = GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings();
					settingsB = GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings();
					break;

				case TransmissionType.TcpAutoSocketOnSpecificIPv6Interface:
					settingsA = GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings();
					settingsB = GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings();
					break;

				default:
					throw (new InvalidOperationException("Program execution should never get here, item " + type.ToString() + " is unknown!"));
			}
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
