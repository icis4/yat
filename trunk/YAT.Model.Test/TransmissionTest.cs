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
	/// <summary></summary>
	[TestFixture]
	public class TransmissionTest
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private readonly string[] TestCommandLines = new string[]
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

		private readonly Utilities.TestSet SingleLineCommand;
		private readonly Utilities.TestSet DoubleLineCommand;
		private readonly Utilities.TestSet TripleLineCommand;
		private readonly Utilities.TestSet MultiLineCommand;

		private readonly Utilities.TestSet MultiEolCommand;
		private readonly Utilities.TestSet MixedEolCommand;

		private readonly Utilities.TestSet EolPartsCommand;

		private readonly Utilities.TestSet SingleNoEolCommand;
		private readonly Utilities.TestSet DoubleNoEolCommand;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TransmissionTest()
		{
			SingleLineCommand  = new Utilities.TestSet(new Types.Command(TestCommandLines[0]));
			DoubleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1] } ));
			TripleLineCommand  = new Utilities.TestSet(new Types.Command(new string[] { TestCommandLines[0], TestCommandLines[1], TestCommandLines[2] }));
			MultiLineCommand   = new Utilities.TestSet(new Types.Command(TestCommandLines));

			MultiEolCommand    = new Utilities.TestSet(new Types.Command(@"A\!(Eol)B<CR><LF>C<CR><LF>D"), 4, new int[] { 2, 2, 2, 2 }, new int[] { 1, 1, 1, 1 }); // Eol results in one element since ShowEol is switched off
			MixedEolCommand    = new Utilities.TestSet(new Types.Command(@"A\!(Eol)BC<CR><LF>D"),         3, new int[] { 2, 2, 2    }, new int[] { 1, 2, 1    }); // Eol results in one element since ShowEol is switched off

			EolPartsCommand    = new Utilities.TestSet(new Types.Command(@"A<CR><CR><LF>B<CR><LF><LF>C<CR><LF>D<CR>E<LF>F"), 4, new int[] { 3, 2, 3, 6 }, new int[] { 2, 1, 2, 5 });

			SingleNoEolCommand = new Utilities.TestSet(new Types.Command(@"A\!(NoEol)"), 1, new int[] { 1 }, new int[] { 1 });                                 // There is always 1 line
			DoubleNoEolCommand = new Utilities.TestSet(new Types.Command(new string[] { @"A\!(NoEol)", @"B\!(NoEol)" }), 1, new int[] { 1 }, new int[] { 2 }); // There is always 1 line
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > SingleLineTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > SingleLineTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSingleLineTransmissionTcp()
		{
			PerformCommandTransmissionTcp(SingleLineCommand);
		}

		#endregion

		#region Tests > DoubleLineTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > DoubleLineTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestDoubleLineTransmissionTcp()
		{
			PerformCommandTransmissionTcp(DoubleLineCommand);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestDoubleLineDoubleTransmissionTcp()
		{
			PerformCommandTransmissionTcp(DoubleLineCommand, 2);
		}

		#endregion

		#region Tests > TripleLineTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > TripleLineTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestTripleLineTransmissionTcp()
		{
			PerformCommandTransmissionTcp(TripleLineCommand);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestTripleLineTripleTransmissionTcp()
		{
			PerformCommandTransmissionTcp(TripleLineCommand, 3);
		}

		#endregion

		#region Tests > MultiLineTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > MultiLineTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestMultiLineTransmissionTcp()
		{
			PerformCommandTransmissionTcp(MultiLineCommand);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestMultiLineMultiTransmissionTcp()
		{
			PerformCommandTransmissionTcp(MultiLineCommand, TestCommandLines.Length);
		}

		#endregion

		#region Tests > MultiEolTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > MultiEolTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestMultiEolTransmissionTcp()
		{
			PerformCommandTransmissionTcp(MultiEolCommand);
		}

		#endregion

		#region Tests > MixedEolTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > MixedEolTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestMixedEolTransmissionTcp()
		{
			PerformCommandTransmissionTcp(MixedEolCommand);
		}

		#endregion

		#region Tests > EolPartsTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > EolPartsTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestEolPartsTransmissionTcp()
		{
			PerformCommandTransmissionTcp(EolPartsCommand);
		}

		#endregion

		#region Tests > NoEolTransmissionTcp
		//------------------------------------------------------------------------------------------
		// Tests > NoEolTransmissionTcp
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSingleNoEolTransmissionTcp()
		{
			PerformCommandTransmissionTcp(SingleNoEolCommand);
		}

		/// <summary></summary>
		[Test]
		public virtual void TestDoubleNoEolTransmissionTcp()
		{
			PerformCommandTransmissionTcp(DoubleNoEolCommand);
		}

		#endregion

		#endregion

		#region Transmission
		//==========================================================================================
		// Transmission
		//==========================================================================================

		private void PerformCommandTransmissionTcp(Utilities.TestSet testSet)
		{
			PerformCommandTransmissionTcp(testSet, 1);
		}

		private void PerformCommandTransmissionTcp(Utilities.TestSet testSet, int transmissionCount)
		{
			PerformCommandTransmission(Utilities.GetTextTcpSettings(), Utilities.GetTextTcpSettings(), testSet, transmissionCount);
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
