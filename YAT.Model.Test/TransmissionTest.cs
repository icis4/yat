//==================================================================================================
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
using System.Text;

using NUnit.Framework;

using YAT.Settings.Terminal;

namespace YAT.Model.Test
{
	[TestFixture]
	public class TransmissionTest
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
		private readonly Utilities.TestSet _MixedEOLCommand;

		private readonly Utilities.TestSet _EOLPartsCommand;

		private readonly Utilities.TestSet _SingleNoEOLCommand;
		private readonly Utilities.TestSet _DoubleNoEOLCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public TransmissionTest()
		{
			_SingleLineCommand  = new Utilities.TestSet(new Types.Command(_TestCommandLines[0]));
			_DoubleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { _TestCommandLines[0], _TestCommandLines[1] } ));
			_TripleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { _TestCommandLines[0], _TestCommandLines[1], _TestCommandLines[2] }));
			_MultiLineCommand   = new Utilities.TestSet(new Types.Command(_TestCommandLines));

			_MultiEOLCommand    = new Utilities.TestSet(new Types.Command(@"A\!(EOL)B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 1, 1, 1 }); // EOL results in one element since ShowEOL is switched off
			_MixedEOLCommand    = new Utilities.TestSet(new Types.Command(@"A\!(EOL)BC<CR><LF>D"),         3, new int[] { 2, 2, 2    }, new int[] { 1, 2, 1    }); // EOL results in one element since ShowEOL is switched off

			_EOLPartsCommand    = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 3, 2, 3, 6 }, new int[] { 2, 1, 2, 5 });

			_SingleNoEOLCommand = new Utilities.TestSet(new Types.Command(@"A\!(NoEOL)"), 1, new int[] { 1 }, new int[] { 1 });                                 // There is always 1 line
			_DoubleNoEOLCommand = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEOL)", @"B\!(NoEOL)" }), 1, new int[] { 1 }, new int[] { 2 }); // There is always 1 line
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
		public virtual void TestSingleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_SingleLineCommand);
		}

		#endregion

		#region Tests > DoubleLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > DoubleLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestDoubleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_DoubleLineCommand);
		}

		[Test]
		public virtual void TestDoubleLineDoubleTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_DoubleLineCommand, 2);
		}

		#endregion

		#region Tests > TripleLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > TripleLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestTripleLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_TripleLineCommand);
		}

		[Test]
		public virtual void TestTripleLineTripleTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_TripleLineCommand, 3);
		}

		#endregion

		#region Tests > MultiLineTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MultiLineTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestMultiLineTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiLineCommand);
		}

		[Test]
		public virtual void TestMultiLineMultiTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiLineCommand, _TestCommandLines.Length);
		}

		#endregion

		#region Tests > MultiEOLTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MultiEOLTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestMultiEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MultiEOLCommand);
		}

		#endregion

		#region Tests > MixedEOLTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > MixedEOLTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestMixedEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_MixedEOLCommand);
		}

		#endregion

		#region Tests > EOLPartsTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > EOLPartsTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestEOLPartsTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_EOLPartsCommand);
		}

		#endregion

		#region Tests > NoEOLTransmissionTCP
		//------------------------------------------------------------------------------------------
		// Tests > NoEOLTransmissionTCP
		//------------------------------------------------------------------------------------------

		[Test]
		public virtual void TestSingleNoEOLTransmissionTCP()
		{
			PerformCommandTransmissionTCP(_SingleNoEOLCommand);
		}

		[Test]
		public virtual void TestDoubleNoEOLTransmissionTCP()
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
			PerformCommandTransmission(Utilities.GetTextTCPSettings(), Utilities.GetTextTCPSettings(), testSet, transmissionCount);
		}

		private void PerformCommandTransmission(TerminalSettingsRoot settingsA, TerminalSettingsRoot settingsB, Utilities.TestSet testSet, int transmissionCount)
		{
			// Create terminals from settings and check whether B receives from A
			using (Terminal terminalA = new Terminal(settingsA))
			{
				using (Terminal terminalB = new Terminal(settingsB))
				{
					// Start and open terminals
					terminalA.Start();
					terminalB.Start();
					Utilities.WaitForConnection(terminalA, terminalB);

					for (int i = 0; i < transmissionCount; i++)
					{
						// Send test command
						terminalA.SendCommand(testSet.Command);
						Utilities.WaitForTransmission(terminalA, terminalB);

						// Verify transmission
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
