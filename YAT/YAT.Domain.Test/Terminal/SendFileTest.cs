﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.2.0 Development
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

using NUnit.Framework;

#endregion

namespace YAT.Domain.Test.Terminal
{
	/// <summary></summary>
	[TestFixture]
	public class SendFileTest
	{
		/* PENDING	var file = FilesProvider.FilePaths_StressText.StressFiles[StressTestCase.Normal];
			var message = string.Format(CultureInfo.InvariantCulture, "Precondition: File line count must equal {0} but is {1}!", SendLineCount, file.Item3);
			Assert.That(file.Item3, Is.EqualTo(SendLineCount), message);
			terminalTx.SendFile(file.Item1);

			var subsequentLengthExpected = (subsequentLineText.Length + 2); // Adjust EOL.
			for (int i = 0; i < subsequentLineCount; i++)
				terminalTx.SendTextLine(subsequentLineText); // Immediately invoke sending of subsequent data.
			                                     // Includes EOLs.
			var expectedTotalByteCount = (file.Item2 + (subsequentLengthExpected * subsequentLineCount));
			var expectedTotalLineCount = (file.Item3 +                             subsequentLineCount);
			Utilities.WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1000); // See further above, sending takes 300..600 ms.
*/	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================