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
// NUnitEx Version 1.0.23
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY.IO;

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary></summary>
	public static class TestCaseDataHelper
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void WriteToTempFile(Type test, TestCaseData data, bool append = false)
		{
			var filePath = Temp.MakeTempFilePath(test, data.TestName, ".log", acceptExistingFile: append);

			using (var sw = new StreamWriter(filePath, append: append))
			{                           // Values start at position 10.
				sw.WriteLine("Args:     {0}", data.Arguments.Length);

				for (int i = 0; i < data.Arguments.Length; i++)
				{
					string argAsString = null;

					var arg = data.Arguments[i];
					if (arg != null)
						argAsString = arg.ToString();
					                         //// Indices get placed at position 5..8 (more than 9999 args are very unlikely).
					if (argAsString == null) //  // Values start at position 10.
						sw.WriteLine("    #{0,4} (null)", i);
					else if (argAsString.Length <= 0)
						sw.WriteLine("    #{0,4} (empty)", i);
					else
						sw.WriteLine("    #{0,4} {1}", i, argAsString);
				}

				// Note that the above format is the same as used in a YAT script log file.
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
