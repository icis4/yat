using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	[TestFixture]
	public class ModelTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private readonly string[] _TestCommandLines = new string[]
			{
				@"1a2a3a4a5a6a7a8a9a0",
				@"1b2b3b4b5b6b7b8b9b0",
				@"1c2c3c4c5c6c7c8c9c0",
				@"1d2d3d4d5d6d7d8d9d0",
				@"1e2e3e4e5e6e7e8e9e0",
				@"1f2f3f4f5f6f7f8f9f0",
				@"1g2g3g4g5g6g7g8g9g0",
				@"1h2h3h4h5h6h7h8h9h0",
				@"1i2i3i4i5i6i7i8i9i0",
				@"1j2j3j4j5j6j7j8j9j0",
			};

		private readonly Utilities.TestSet _SingleLineCommand;
		private readonly Utilities.TestSet _DoubleLineCommand;
		private readonly Utilities.TestSet _TripleLineCommand;
		private readonly Utilities.TestSet _MultiLineCommand;

		private readonly Utilities.TestSet _MultiEOLCommand;

		private readonly Utilities.TestSet _SingleNoEOLCommand;
		private readonly Utilities.TestSet _DoubleNoEOLCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public ModelTest()
		{
			_SingleLineCommand  = new Utilities.TestSet(new Model.Types.Command(_TestCommandLines[0]));
			_DoubleLineCommand  = new Utilities.TestSet(new Model.Types.Command(new string[] { _TestCommandLines[0], _TestCommandLines[1] } ));
			_TripleLineCommand  = new Utilities.TestSet(new Model.Types.Command(new string[] { _TestCommandLines[0], _TestCommandLines[1], _TestCommandLines[2] }));
			_MultiLineCommand   = new Utilities.TestSet(new Model.Types.Command(_TestCommandLines));

			_MultiEOLCommand    = new Utilities.TestSet(new Model.Types.Command(@"A<CR><LF>B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }); // EOL results in one more element

			_SingleNoEOLCommand = new Utilities.TestSet(new Model.Types.Command(@"A\!(NoEOL)"), 0, new int[] { 1 });
			_DoubleNoEOLCommand = new Utilities.TestSet(new Model.Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 0, new int[] { 2 });
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

		#region Tests > NoEOLTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > NoEOLTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestSingleNoEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_SingleNoEOLCommand);
		}

		[Test]
		public void TestDoubleNoEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_DoubleNoEOLCommand);
		}

		#endregion

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private void PerformCommandTransmissionTCP(Utilities.TestSet testSet)
		{
			PerformCommandTransmissionTCP(testSet, 1);
		}

		private void PerformCommandTransmissionTCP(Utilities.TestSet testSet, int transmissionCount)
		{
			PerformCommandTransmission(Utilities.GetTCPSettings(), Utilities.GetTCPSettings(), testSet, transmissionCount);
		}

		private void PerformCommandTransmission(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// create terminals from settings and check whether B receives from A
			using (Model.Terminal terminalA = new Model.Terminal(settingsA))
			{
				using (Model.Terminal terminalB = new Model.Terminal(settingsB))
				{
					// start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// send test command
						terminalA.SendCommand(testSet.Command);
						Utilities.WaitForTransmission(terminalA, terminalB);

						// verify transmission
						Utilities.VerifyLines(terminalA.RepositoryToDisplayLines(Domain.RepositoryType.Tx),
							                  terminalB.RepositoryToDisplayLines(Domain.RepositoryType.Rx),
									          testSet, i + 1);
					}
				}
			}
		}

		#endregion
	}
}
