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
	public enum StressFile
	{
		Normal,
		Large,
		LargeWithLongLines,
		Huge,
		HugeWithVeryLongLines,
		Enormous,

		LongLine,
		VeryLongLine,
		EnormousLine
	}

	#pragma warning restore 1591

	#endregion

	#region Types > FileInfo
	//------------------------------------------------------------------------------------------
	// Types > FileInfo
	//------------------------------------------------------------------------------------------

	/// <remarks>Could be migrated to an 'EnumEx', with e.g. 'IsLine' and the like.</remarks>
	public class FileInfo
	{
		/// <summary></summary>
		public string Path { get; }

		/// <remarks>Content byte count, including EOLs (text files), not including EOF.</remarks>
		public int ByteCount { get; }

		/// <summary></summary>
		public int LineCount { get; }

		/// <summary></summary>
		public FileInfo(string path, int byteCount, int lineCount)
		{
			Path = path;
			ByteCount = byteCount;
			LineCount = lineCount;
		}

		/// <remarks>Line byte count, including EOL (text files).</remarks>
		public int LineByteCount
		{
			get { return (ByteCount / LineCount); }
		}
	}

	#endregion

	#region Types > FileGroup
	//------------------------------------------------------------------------------------------
	// Types > FileGroup
	//------------------------------------------------------------------------------------------

	/// <summary></summary>
	public class FileGroup
	{
		/// <summary></summary>
		public string DirectoryPath { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		public Dictionary<StressFile, FileInfo> Item { get; }

		/// <summary></summary>
		public FileGroup(int capacity)
			: this(capacity, null)
		{
		}

		/// <summary></summary>
		public FileGroup(int capacity, string directory)
		{
			// Traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>".
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// Set path to "<Root>\!-TestFiles\" or "<Root>\!-TestFiles\<Directory>\".
			if (string.IsNullOrEmpty(directory))
				DirectoryPath = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-TestFiles" + System.IO.Path.DirectorySeparatorChar;
			else
				DirectoryPath = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-TestFiles" + System.IO.Path.DirectorySeparatorChar + directory + System.IO.Path.DirectorySeparatorChar;

			Item = new Dictionary<StressFile, FileInfo>(capacity);
		}

		/// <summary></summary>
		public void Add(StressFile fileKey, string fileName, int fileSize, int lineCount)
		{
			Item.Add(fileKey, new FileInfo(DirectoryPath + fileName, fileSize, lineCount));
		}
	}

	#endregion

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "As always, there are exceptions to the rules...")]
	public static class Files
	{
		private const string UnderscoreSuppressionJustification = "As always, there are exceptions to the rules...";

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

	/////// <summary></summary>
	////[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
	////[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
	////public static readonly FileGroup SendRaw;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly FileGroup SendText;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly FileGroup TextSendFile;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly FileGroup BinarySendFile;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic, and anyway, performance isn't an issue here.")]
		static Files()
		{                                // Preset the required capacity to improve memory management.
			TextSendFile = new FileGroup(9);                                                             // Including EOLs; not including EOF.
			TextSendFile.Add(StressFile.Normal,                "Stress-1-Normal.txt",                  8400,    300); //   28 bytes per line.
			TextSendFile.Add(StressFile.Large,                 "Stress-2-Large.txt",                  82500,   1500); //   55 bytes per line.
			TextSendFile.Add(StressFile.LargeWithLongLines,    "Stress-3-LargeWithLongLines.txt",     97300,    100); //  973 bytes per line.
			TextSendFile.Add(StressFile.Huge,                  "Stress-4-Huge.txt",                 1090000,  10000); //  109 bytes per line.
			TextSendFile.Add(StressFile.HugeWithVeryLongLines, "Stress-5-HugeWithVeryLongLines.txt", 999100,    100); // 9991 bytes per line.
			TextSendFile.Add(StressFile.Enormous,              "Stress-6-Enormous.txt",            16300000, 100000); //  163 bytes per line.

			BinarySendFile = new FileGroup(4);                                                           // Not including EOF.
			BinarySendFile.Add(StressFile.Normal,              "Stress-1-Normal.dat",                  8192, -1);
			BinarySendFile.Add(StressFile.Large,               "Stress-2-Large.dat",                  82432, -1);
			////               StressFile.LargeWithLongLines makes little sense for binary terminals
			BinarySendFile.Add(StressFile.Huge,                "Stress-4-Huge.dat",                 1089792, -1);
			////               StressFile.HugeWithVeryLongLines makes little sense for binary terminals
			BinarySendFile.Add(StressFile.Enormous,            "Stress-5-Enormous.dat",            16299776, -1);

			SendText = new FileGroup(3);                                                                 // Including EOL; not including EOF.
			SendText.Add(StressFile.LongLine,                  "Stress-7-LongLine.txt",                 973, 1);
			SendText.Add(StressFile.VeryLongLine,              "Stress-8-VeryLongLine.txt",            9991, 1);
			SendText.Add(StressFile.EnormousLine,              "Stress-9-EnormousLine.txt",          500014, 1);

		////SendRaw = new FileGroup(3);                                                                  // Not including EOF.
		////SendRaw.Add(StressFile.LongData,                   "Stress-7-LongData.txt",                 973, 1);
		////SendRaw.Add(StressFile.VeryLongData,               "Stress-8-VeryLongData.txt",            9991, 1);
		////SendRaw.Add(StressFile.EnormousData,               "Stress-9-EnormousData.txt",          500014, 1);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
