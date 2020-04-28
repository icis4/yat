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
// YAT Version 2.1.1 Development
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

	#region Types > File Paths
	//------------------------------------------------------------------------------------------
	// Types > File Paths
	//------------------------------------------------------------------------------------------

	/// <summary></summary>
	public class FilePaths
	{
		/// <summary></summary>
		public string Path { get; }

		/// <summary></summary>
		public Dictionary<StressTestCase, string> StressFilePaths { get; }

		/// <summary></summary>
		public FilePaths()
			: this(null)
		{
		}

		/// <summary></summary>
		public FilePaths(string directory)
		{
			// Traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>".
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// Set path to "<Root>\!-SendFiles\" or "<Root>\!-SendFiles\<Directory>\".
			if (string.IsNullOrEmpty(directory))
				Path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-SendFiles" + System.IO.Path.DirectorySeparatorChar;
			else
				Path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-SendFiles" + System.IO.Path.DirectorySeparatorChar + directory + System.IO.Path.DirectorySeparatorChar;

			StressFilePaths = new Dictionary<StressTestCase, string>();
		}

		/// <summary></summary>
		public void AddStressFileName(StressTestCase fileKey, string fileName)
		{
			StressFilePaths.Add(fileKey, Path + fileName);
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
		public static readonly FilePaths FilePaths_StressText;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly FilePaths FilePaths_StressBinary;

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
			FilePaths_StressText = new FilePaths();
			FilePaths_StressText.AddStressFileName(StressTestCase.Normal,     "Stress-1-Normal.txt");
			FilePaths_StressText.AddStressFileName(StressTestCase.Large,      "Stress-2-Large.txt");
			FilePaths_StressText.AddStressFileName(StressTestCase.EvenLarger, "Stress-3-EvenLarger.txt");
			FilePaths_StressText.AddStressFileName(StressTestCase.Huge,       "Stress-4-Huge.txt");
			FilePaths_StressText.AddStressFileName(StressTestCase.Enormous,   "Stress-5-Enormous.txt");

			// Stress binary:
			FilePaths_StressBinary = new FilePaths();
			FilePaths_StressBinary.AddStressFileName(StressTestCase.Normal,     "Stress-1-Normal.dat");
			FilePaths_StressBinary.AddStressFileName(StressTestCase.Large,      "Stress-2-Large.dat");
			FilePaths_StressBinary.AddStressFileName(StressTestCase.EvenLarger, "Stress-3-EvenLarger.dat");
			FilePaths_StressBinary.AddStressFileName(StressTestCase.Huge,       "Stress-4-Huge.dat");
			FilePaths_StressBinary.AddStressFileName(StressTestCase.Enormous,   "Stress-5-Enormous.dat");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
