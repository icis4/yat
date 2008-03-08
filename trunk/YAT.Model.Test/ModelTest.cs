using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using MKY.Utilities.Settings;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	[TestFixture]
	public class ModelTest
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
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

		private readonly string[] _TestCommandLines = new string[]
			{
				"1a2a3a4a5a6a7a8a9a0",
				"1b2b3b4b5b6b7b8b9b0",
				"1c2c3c4c5c6c7c8c9c0",
				"1d2d3d4d5d6d7d8d9d0",
				"1e2e3e4e5e6e7e8e9e0",
				"1f2f3f4f5f6f7f8f9f0",
				"1g2g3g4g5g6g7g8g9g0",
				"1h2h3h4h5h6h7h8h9h0",
				"1i2i3i4i5i6i7i8i9i0",
				"1j2j3j4j5j6j7j8j9j0",
			};

		private readonly TestSet _SingleLineCommand;
		private readonly TestSet _DoubleLineCommand;
		private readonly TestSet _TripleLineCommand;
		private readonly TestSet _MultiLineCommand;

		private readonly TestSet _MultiEOLCommand;

		private const int _IntervalTCP = 100;
		private const int _TimeoutTCP = 10000;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public ModelTest()
		{
			_SingleLineCommand = new TestSet(new Model.Types.Command("", _TestCommandLines[0]));
			_DoubleLineCommand = new TestSet(new Model.Types.Command("", new string[] { _TestCommandLines[0], _TestCommandLines[1] } ));
			_TripleLineCommand = new TestSet(new Model.Types.Command("", new string[] { _TestCommandLines[0], _TestCommandLines[1], _TestCommandLines[2] }));
			_MultiLineCommand  = new TestSet(new Model.Types.Command("", _TestCommandLines));

			_MultiEOLCommand   = new TestSet(new Model.Types.Command("", "A<CR><LF>B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }); // EOL results in one more element
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > SingleLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > SingleLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestSingleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_SingleLineCommand);
		}

		#endregion

		#region Tests > DoubleLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > DoubleLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestDoubleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_DoubleLineCommand);
		}

		#endregion

		#region Tests > DoubleLineDoubleTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > DoubleLineDoubleTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestDoubleLineDoubleTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_DoubleLineCommand, 2);
		}

		#endregion

		#region Tests > TripleLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > TripleLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestTripleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_TripleLineCommand);
		}

		#endregion

		#region Tests > TripleLineTripleTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > TripleLineTripleTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestTripleLineTripleTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_TripleLineCommand, 3);
		}

		#endregion

		#region Tests > MultiLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MultiLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestMultiLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiLineCommand);
		}

		#endregion

		#region Tests > MultiLineMultiTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MultiLineMultiTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestMultiLineMultiTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiLineCommand, _TestCommandLines.Length);
		}

		#endregion

		#region Tests > MultiEOLTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MultiEOLTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestMultiEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiEOLCommand);
		}

		#endregion

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private void PerformCommandTransmissionTCP(TestSet testSet)
		{
			PerformCommandTransmissionTCP(testSet, 1);
		}

		private void PerformCommandTransmissionTCP(TestSet testSet, int transmissionCount)
		{
			// create settings
			TerminalSettingsRoot settingsA = new TerminalSettingsRoot();
			settingsA.Terminal.IO.IOType = Domain.IOType.TcpAutoSocket;
			settingsA.TerminalIsOpen = true;

			// clone settings
			TerminalSettingsRoot settingsB = new TerminalSettingsRoot(settingsA);

			// create terminals from settings and check whether B receives from A
			using (Model.Terminal terminalA = new Model.Terminal(settingsA))
			{
				using (Model.Terminal terminalB = new Model.Terminal(settingsB))
				{
					// start and open terminals
					terminalA.Start();
					terminalB.Start();
					WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// send test command
						terminalA.SendCommand(testSet.Command);
						WaitForTransmission(terminalA, terminalB);

						// verify transmission
						VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
							        terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
									testSet, i + 1);
					}
				}
			}
		}

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		private void WaitForConnection(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(_IntervalTCP);
				timeout += _IntervalTCP;

				if (timeout >= _TimeoutTCP)
					Assert.Fail("Connect timeout");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);
		}

		private void WaitForTransmission(Model.Terminal terminalA, Model.Terminal terminalB)
		{
			int timeout = 0;
			do                         // initially wait to allow async send,
			{                          //   therefore, use do-while
				Thread.Sleep(_IntervalTCP);
				timeout += _IntervalTCP;

				if (timeout >= _TimeoutTCP)
					Assert.Fail("Transmission timeout");
			}
			while (terminalB.RxByteCount != terminalA.TxByteCount);
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		private void VerifyLines(List<List<Domain.DisplayElement>> linesA, List<List<Domain.DisplayElement>> linesB, TestSet testSet, int cycle)
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
