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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace YAT.Domain.Test
{
	#region Types
	//==========================================================================================
	// Types
	//==========================================================================================

	#region Types > Enums
	//------------------------------------------------------------------------------------------
	// Types > Enums
	//------------------------------------------------------------------------------------------

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum StressTestCase
	{
		Normal,
		Large,
		EvenLarger,
		Huge,
		Enormous
	}

	#pragma warning restore 1591

	#endregion

	#region Types > Files
	//------------------------------------------------------------------------------------------
	// Types > Files
	//------------------------------------------------------------------------------------------

	/// <summary></summary>
	public class Files
	{
		/// <summary></summary>
		public string DirectoryPath { get; }

		/// <summary></summary>
		public Dictionary<StressTestCase, Tuple<string, int, int>> StressFiles { get; }

		/// <summary></summary>
		public Files()
			: this(null)
		{
		}

		/// <summary></summary>
		public Files(string directory)
		{
			// Traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>".
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// Set path to "<Root>\!-SendFiles\" or "<Root>\!-SendFiles\<Directory>\".
			if (string.IsNullOrEmpty(directory))
				DirectoryPath = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-SendFiles" + System.IO.Path.DirectorySeparatorChar;
			else
				DirectoryPath = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-SendFiles" + System.IO.Path.DirectorySeparatorChar + directory + System.IO.Path.DirectorySeparatorChar;

			StressFiles = new Dictionary<StressTestCase, Tuple<string, int, int>>();
		}

		/// <summary></summary>
		public void AddStressFile(StressTestCase fileKey, string fileName, int fileSize, int lineCount)
		{
			StressFiles.Add(fileKey, new Tuple<string, int, int>(DirectoryPath + fileName, fileSize, lineCount));
		}
	}

	#endregion

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "As always, there are exceptions to the rules...")]
	public static class SendFilesProvider
	{
		private const string UnderscoreSuppressionJustification = "As always, there are exceptions to the rules...";

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly Files FilePaths_StressText;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly Files FilePaths_StressBinary;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static SendFilesProvider()
		{
			// Stress text:
			FilePaths_StressText = new Files();                                                            // Not including EOF.
			FilePaths_StressText.AddStressFile(StressTestCase.Normal,     "Stress-1-Normal.txt"    ,     8400,    300);
			FilePaths_StressText.AddStressFile(StressTestCase.Large,      "Stress-2-Large.txt"     ,    82500,   1500);
			FilePaths_StressText.AddStressFile(StressTestCase.EvenLarger, "Stress-3-EvenLarger.txt",   275000,   5000);
			FilePaths_StressText.AddStressFile(StressTestCase.Huge,       "Stress-4-Huge.txt"      ,  1090000,  10000);
			FilePaths_StressText.AddStressFile(StressTestCase.Enormous,   "Stress-5-Enormous.txt"  , 16300000, 100000);

			// Stress binary:
			FilePaths_StressBinary = new Files();
			FilePaths_StressBinary.AddStressFile(StressTestCase.Normal,     "Stress-1-Normal.dat"    ,     8192, -1);
			FilePaths_StressBinary.AddStressFile(StressTestCase.Large,      "Stress-2-Large.dat"     ,    82432, -1);
			FilePaths_StressBinary.AddStressFile(StressTestCase.EvenLarger, "Stress-3-EvenLarger.dat",   274944, -1);
			FilePaths_StressBinary.AddStressFile(StressTestCase.Huge,       "Stress-4-Huge.dat"      ,  1089792, -1);
			FilePaths_StressBinary.AddStressFile(StressTestCase.Enormous,   "Stress-5-Enormous.dat"  , 16299776, -1);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
