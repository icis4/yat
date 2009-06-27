//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2009 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using MKY.Utilities.Settings;
using MKY.Utilities.Types;

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
		private const int _WaitEOL = 1000;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettingsRoot GetTextTCPSettings()
		{
			// create settings
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.TcpAutoSocket;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		internal static DocumentSettingsHandler<TerminalSettingsRoot> GetTextTCPSettingsHandler()
		{
			return (new DocumentSettingsHandler<TerminalSettingsRoot>(GetTextTCPSettings()));
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
					Assert.Fail("Transmission timeout. Try to re-run test case.");
			}
			while (terminalB.RxByteCount != terminalA.TxByteCount);

			// wait to allow EOL to be sent (EOL is sent a bit later than line contents)
			Thread.Sleep(_WaitEOL);
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
							"Expected = " + expectedLineLength + " elements, " +
							"A = " + lineA.Count + @" elements, " +
							"B = " + lineB.Count + @" elements. See ""Console.Out"" for details."
							);
					}
				}
			}
			else
			{
				StringBuilder sbA = new StringBuilder();
				StringBuilder sbB = new StringBuilder();

				foreach (List<Domain.DisplayElement> lineA in linesA)
					sbA.Append(XArray.ElementsToString(lineA.ToArray()));
				foreach (List<Domain.DisplayElement> lineB in linesB)
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
// End of $URL$
//==================================================================================================
