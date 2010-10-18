//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using NUnit.Framework;

using MKY.System.Net;
using MKY.System.Settings;
using MKY.System.Types;

using YAT.Settings.Terminal;

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
		public struct TestSet
		{
			/// <summary></summary>
			public readonly Model.Types.Command Command;
			/// <summary></summary>
			public readonly int ExpectedLineCount;
			/// <summary></summary>
			public readonly int[] ExpectedElementCounts;
			/// <summary></summary>
			public readonly int[] ExpectedDataCounts;

			/// <summary></summary>
			public TestSet(Model.Types.Command command)
			{
				Command = command;
				ExpectedLineCount = command.CommandLines.Length;

				ExpectedElementCounts = new int[ExpectedLineCount];
				ExpectedDataCounts = new int[ExpectedLineCount];
				for (int i = 0; i < ExpectedLineCount; i++)
				{
					ExpectedElementCounts[i] = 2; // 1 data element + 1 Eol element
					ExpectedDataCounts[i]    = command.CommandLines[i].Length;
				}
			}

			/// <summary></summary>
			public TestSet(Model.Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedDataCounts)
			{
				Command = command;
				ExpectedLineCount = expectedLineCount;
				ExpectedElementCounts = expectedElementCounts;
				ExpectedDataCounts = expectedDataCounts;
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int Interval = 100;
		private const int Timeout = 10000;
		private const int WaitEol = 1000;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

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

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortSettingsHandler(MKY.IO.Ports.SerialPortId portId)
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortSettings(portId)));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortASettings()
		{
			return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortA));
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortASettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortASettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortBSettings()
		{
			return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.SettingsProvider.Settings.SerialPortB));
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextSerialPortBSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextSerialPortBSettings()));
		}

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

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnIPv6LoopbackSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.SettingsProvider.Settings.SpecificIPv4Interface));
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnSpecificIPv4InterfaceSettings()));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.SettingsProvider.Settings.SpecificIPv6Interface));
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetStartedTextTcpAutoSocketOnSpecificIPv6InterfaceSettings()));
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForConnection(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail("Connect timeout");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);
		}

		internal static void WaitForTransmission(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail("Transmission timeout. Try to re-run test case.");
			}
			while (terminalB.RxByteCount != terminalA.TxByteCount);

			// Wait to allow Eol to be sent (Eol is sent a bit later than line contents)
			Thread.Sleep(WaitEol);
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
			int expectedLineCount = testSet.ExpectedLineCount * cycle;

			if ((linesB.Count == linesA.Count) &&
				(linesB.Count == expectedLineCount))
			{
				for (int i = 0; i < linesA.Count; i++)
				{
					Domain.DisplayLine lineA = linesA[i];
					Domain.DisplayLine lineB = linesB[i];

					int commandIndex = i % testSet.ExpectedLineCount;
					int expectedElementCount = testSet.ExpectedElementCounts[commandIndex];
					int expectedDataCount = testSet.ExpectedDataCounts[commandIndex];

					if ((lineB.Count == lineA.Count) &&
						(lineB.Count == expectedElementCount) &&
						(lineB.DataCount == lineA.DataCount) &&
						(lineB.DataCount == expectedDataCount))
					{
						for (int j = 0; j < lineA.Count; j++)
							Assert.AreEqual(lineA[j].Text, lineB[j].Text);
					}
					else
					{
						string strA = XArray.ElementsToString(lineA.ToArray());
						string strB = XArray.ElementsToString(lineB.ToArray());

						Console.Write
							(
							"A:" + Environment.NewLine + strA + Environment.NewLine +
							"B:" + Environment.NewLine + strB + Environment.NewLine
							);

						Assert.Fail
							(
							"Line length mismatch: " +
							"Expected = " + expectedElementCount + " elements, " +
							"A = " + lineA.Count + @" elements, " +
							"B = " + lineB.Count + @" elements, " +
							"Expected = " + expectedDataCount + " data, " +
							"A = " + lineA.DataCount + @" data, " +
							"B = " + lineB.DataCount + @" data. See ""Console.Out"" for details."
							);
					}
				}
			}
			else
			{
				StringBuilder sbA = new StringBuilder();
				StringBuilder sbB = new StringBuilder();

				foreach (Domain.DisplayLine lineA in linesA)
					sbA.Append(XArray.ElementsToString(lineA.ToArray()));
				foreach (Domain.DisplayLine lineB in linesB)
					sbB.Append(XArray.ElementsToString(lineB.ToArray()));

				Console.Write
					(
					"A:" + Environment.NewLine + sbA + Environment.NewLine +
					"B:" + Environment.NewLine + sbB + Environment.NewLine
					);

				Assert.Fail
					(
					"Line count mismatch: " +
					"Expected = " + expectedLineCount + " lines, " +
					"A = " + linesA.Count + @" lines, " +
					"B = " + linesB.Count + @" lines. See ""Console.Out"" for details."
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
