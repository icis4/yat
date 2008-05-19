using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	internal static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		internal struct TestSet
		{
			public readonly Model.Types.Command Command;
			public readonly int ExpectedLineCount;
			public readonly int[] ExpectedLineLenghts;

			public TestSet(Model.Types.Command command)
			{
				Command = command;
				ExpectedLineCount = command.CommandLines.Length;

				ExpectedLineLenghts = new int[ExpectedLineCount];
				for (int i = 0; i < ExpectedLineCount; i++)
				{
					ExpectedLineLenghts[i] = command.CommandLines[i].Length + 1; // EOL results in one more element
				}
			}

			public TestSet(Model.Types.Command command, int expectedLineCount, int[] expectedLineLenghts)
			{
				Command = command;
				ExpectedLineCount = expectedLineCount;
				ExpectedLineLenghts = expectedLineLenghts;
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================
		private const int _Interval = 100;
		private const int _Timeout = 10000;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettingsRoot GetTCPSettings()
		{
			// create settings
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.Terminal.IO.IOType = MKY.IO.Serial.IOType.TcpAutoSocket;
			settings.TerminalIsOpen = true;
			return (settings);
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForConnection(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(_Interval);
				timeout += _Interval;

				if (timeout >= _Timeout)
					Assert.Fail("Connect timeout");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);
		}

		internal static void WaitForTransmission(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(_Interval);
				timeout += _Interval;

				if (timeout >= _Timeout)
					Assert.Fail("Transmission timeout");
			}
			while (terminalB.RxByteCount != terminalA.TxByteCount);
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		internal static void VerifyLines(List<List<Domain.DisplayElement>> linesA, List<List<Domain.DisplayElement>> linesB, TestSet testSet)
		{
			VerifyLines(linesA, linesB, testSet, 1);
		}

		internal static void VerifyLines(List<List<Domain.DisplayElement>> linesA, List<List<Domain.DisplayElement>> linesB, TestSet testSet, int cycle)
		{
			int expectedLineCount = testSet.ExpectedLineCount * cycle;

			if ((linesB.Count == linesA.Count) &&
				(linesB.Count == expectedLineCount))
			{
				for (int i = 0; i < linesA.Count; i++)
				{
					List<Domain.DisplayElement> lineA = linesA[i];
					List<Domain.DisplayElement> lineB = linesB[i];

					int commandIndex = i % testSet.ExpectedLineCount;
					int expectedLineLength = testSet.ExpectedLineLenghts[commandIndex];

					if ((lineB.Count == lineA.Count) &&
						(lineB.Count == expectedLineLength))
					{
						for (int j = 0; j < lineA.Count; j++)
							Assert.AreEqual(lineA[j].Text, lineB[j].Text);
					}
					else
					{
						Assert.Fail
							(
							"Line length mismatch: " +
							"Expected = " + expectedLineLength.ToString() + " elements, " +
							"A = " + lineA.Count.ToString() + " elements, " +
							"B = " + lineB.Count.ToString() + " elements."
							);
					}
				}
			}
			else
			{
				Assert.Fail
					(
					"Line count mismatch: " +
					"Expected = " + expectedLineCount.ToString() + " lines, " +
					"A = " + linesA.Count.ToString() + " lines, " +
					"B = " + linesB.Count.ToString() + " lines."
					);
			}
		}

		#endregion
	}
}
