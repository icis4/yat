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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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
using System.Threading;

using MKY.Collections.Generic;
using MKY.Net;

using NUnit.Framework;

using YAT.Domain.Settings;

#endregion

namespace YAT.Domain.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <typeparam name="T">The (simple) settings type.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize the 'utility' nature of this delegate.")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Why not?")]
		public delegate TerminalSettings TerminalSettingsDelegate<T>(T arg);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize the 'utility' nature of this class.")]
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
						var tsm = new TerminalSettingsDelegate<string>(GetSerialPortTextSettings);
						var pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortA);
						var pB = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortB);
						string name = "SerialPortLoopbackPairs_" + ce.PortA + "_" + ce.PortB;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int WaitTimeoutForStateChange = 3000;

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForStateChange = 100;

		/// <summary></summary>
		public const int WaitTimeoutForIsSendingForSomeTime = (3 * Domain.Utilities.ForSomeTimeEventHelper.Threshold);

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForIsSendingForSomeTime = WaitIntervalForStateChange;

		/// <summary></summary>
		public const int WaitTimeoutForIsNoLongerSending = (3 * Domain.Utilities.ForSomeTimeEventHelper.Threshold);

		/// <remarks>
		/// Note that a shorter interval would increase debug output, spoiling the debug console.
		/// </remarks>
		public const int WaitIntervalForIsNoLongerSending = WaitIntervalForStateChange;

		/// <remarks>
		/// Timeout of 200 ms is too short for serial COM ports at 9600 baud, especially when
		/// debugger is connected. Measurements:
		///  > TripleLine (where timeout would be 3 * 200 ms = 600 ms) takes around 500 ms.
		///  > MultiLine (where timeout would be 26 * 200 ms = 5200 ms) takes around 5000 ms.
		///     => 300 ms seems defensive enough while still not too long to waste time.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public const int WaitTimeoutForLineTransmission = 300; // See remarks above.

		/// <remarks>
		/// Note that a longer interval would increase the wait time, thus increasing the test time.
		/// </remarks>
		public const int WaitIntervalForLineTransmission = 20;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > Text
		//------------------------------------------------------------------------------------------
		// Settings > Text
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetTextSettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Text;
			settings.UpdateTerminalTypeDependentSettings();
			settings.TextTerminal.ShowEol = true; // Required for easier test verification (char/byte count).
			return (settings);                    // Consider moving to each test instead.
		}

		internal static TerminalSettings GetSerialPortTextSettings(string portId)
		{
			var settings = GetTextSettings();
			settings.IO.IOType = IOType.SerialPort;
			settings.IO.SerialPort.PortId = portId;
			settings.UpdateIOTypeDependentSettings();
			settings.UpdateIOSettingsDependentSettings();
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketTextSettings(IPNetworkInterfaceEx networkInterface)
		{
			var settings = GetTextSettings();
			settings.IO.IOType = IOType.TcpAutoSocket;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.LocalInterface = networkInterface;
			settings.UpdateIOSettingsDependentSettings();
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketOnIPv4LoopbackTextSettings()
		{
			return (GetTcpAutoSocketTextSettings((IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		#endregion

		#region Settings > Binary
		//------------------------------------------------------------------------------------------
		// Settings > Binary
		//------------------------------------------------------------------------------------------

		internal static TerminalSettings GetBinarySettings()
		{
			var settings = new TerminalSettings();
			settings.TerminalType = TerminalType.Binary;
			settings.UpdateTerminalTypeDependentSettings();
			return (settings);
		}

		internal static TerminalSettings GetSerialPortBinarySettings(string portId)
		{
			var settings = GetBinarySettings();
			settings.IO.IOType = IOType.SerialPort;
			settings.IO.SerialPort.PortId = portId;
			settings.UpdateIOTypeDependentSettings();
			settings.UpdateIOSettingsDependentSettings();
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketBinarySettings(IPNetworkInterfaceEx networkInterface)
		{
			var settings = GetBinarySettings();
			settings.IO.IOType = IOType.TcpAutoSocket;
			settings.UpdateIOTypeDependentSettings();
			settings.IO.Socket.LocalInterface = networkInterface;
			settings.UpdateIOSettingsDependentSettings();
			return (settings);
		}

		internal static TerminalSettings GetTcpAutoSocketOnIPv4LoopbackBinarySettings()
		{
			return (GetTcpAutoSocketBinarySettings((IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		#endregion

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForConnection(Domain.Terminal terminalA, Domain.Terminal terminalB)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);

			Trace.WriteLine("...done, connected");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForDisconnection(Domain.Terminal terminal)
		{
			int waitTime = 0;
			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Disconnect timeout!");
				}
			}

			Trace.WriteLine("...done, disconnected");
		}

		internal static void WaitForIsSendingForSomeTime(Domain.Terminal terminal, int timeout = WaitTimeoutForIsSendingForSomeTime)
		{
			int waitTime = 0;
			while (!terminal.IsSendingForSomeTime)
			{
				Thread.Sleep(WaitIntervalForIsSendingForSomeTime);
				waitTime += WaitIntervalForIsSendingForSomeTime;

				Trace.WriteLine("Waiting for 'IsSendingForSomeTime', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("'IsSendingForSomeTime' timeout!");
				}
			}

			Trace.WriteLine("...done, 'IsSendingForSomeTime'");
		}

		internal static void WaitForIsNoLongerSending(Domain.Terminal terminal, int timeout = WaitTimeoutForIsNoLongerSending)
		{
			int waitTime = 0;
			while (terminal.IsSending)
			{
				Thread.Sleep(WaitIntervalForIsNoLongerSending);
				waitTime += WaitIntervalForIsNoLongerSending;

				Trace.WriteLine("Waiting for 'IsNoLongerSending', " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("'IsNoLongerSending' timeout!");
				}
			}

			Trace.WriteLine("...done, 'IsNoLongerSending'");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// </remarks>
		/// <remarks>
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		internal static void WaitForSendingAndVerifyCounts(Domain.Terminal terminalTx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = WaitTimeoutForLineTransmission;

			int txByteCount = 0;
			int txLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Trace.WriteLine("Waiting for sending, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data sent within expected interval.");
				}

				txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (txLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent lines = " + txLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCount));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");

			Trace.WriteLine("...done, sent and verified");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// </remarks>
		/// <remarks>
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		internal static void WaitForReceivingAndVerifyCounts(Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = WaitTimeoutForLineTransmission;

			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Trace.WriteLine("Waiting for receiving, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data received within expected interval.");
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount));

			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			Trace.WriteLine("...done, received and verified");
		}

		/// <remarks>
		/// There are similar utility methods in 'Model.Test.Utilities'.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines.
		/// </remarks>
		/// <remarks>
		/// Comparison against the completed number of lines is not (yet) possible, change #375
		/// "consider to migrate Byte/Line Count/Rate from model to domain" is required for this.
		/// </remarks>
		internal static void WaitForTransmissionAndVerifyCounts(Domain.Terminal terminalTx, Domain.Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount, int timeout = WaitTimeoutForLineTransmission)
		{
			// Attention:
			// Similar code exists in Model.Test.Utilities.WaitForTransmissionAndVerifyCounts().
			// Changes here may have to be applied there too.

			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Trace.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data transmitted within expected interval.");
				}

				txByteCount = terminalTx.GetRepositoryByteCount(RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(RepositoryType.Tx);
				if (txLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent lines = " + txLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCount) ||
			       (rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCount));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");
			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			Trace.WriteLine("...done, transmitted and verified");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
