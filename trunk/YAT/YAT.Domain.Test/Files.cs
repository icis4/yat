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

	/// <summary></summary>
	public enum FileType
	{
		Text,
		Binary
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
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Meaning is clear here; repeating 'File' makes little sense.")]
		public FileType Type { get; }

		/// <summary></summary>
		public string Path { get; }

		/// <remarks>Content byte count, including EOLs (text files), not including EOF.</remarks>
		public int ByteCount { get; }

		/// <remarks>Virtual lines in case of <see cref="FileType.Binary"/>.</remarks>
		public int LineCount { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public FileInfo(FileType type, string path, int byteCount, int lineCount)
		{
			Type      = type;
			Path      = path;
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
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Don't care, straightforward test implementation.")]
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
		public void Add(StressFile fileKey, FileType fileType, string fileName, int fileSize, int lineCount)
		{
			Item.Add(fileKey, new FileInfo(fileType, DirectoryPath + fileName, fileSize, lineCount));
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
		public static readonly FileGroup TextSendText;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of test implementation, especially adding new settings.")]
		public static readonly FileGroup BinarySendText;

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
			TextSendFile = new FileGroup(6);                                                                              // Including EOLs; not including EOF.
			TextSendFile.Add(StressFile.Normal,                FileType.Text,   "Stress-1-Normal.txt",                  8400,    300); //   28 bytes per line.
			TextSendFile.Add(StressFile.Large,                 FileType.Text,   "Stress-2-Large.txt",                  82500,   1500); //   55 bytes per line.
			TextSendFile.Add(StressFile.LargeWithLongLines,    FileType.Text,   "Stress-3-LargeWithLongLines.txt",     97300,    100); //  973 bytes per line.
			TextSendFile.Add(StressFile.Huge,                  FileType.Text,   "Stress-4-Huge.txt",                 1090000,  10000); //  109 bytes per line.
			TextSendFile.Add(StressFile.HugeWithVeryLongLines, FileType.Text,   "Stress-5-HugeWithVeryLongLines.txt", 999100,    100); // 9991 bytes per line.
			TextSendFile.Add(StressFile.Enormous,              FileType.Text,   "Stress-6-Enormous.txt",            16300000, 100000); //  163 bytes per line.

			BinarySendFile = new FileGroup(4);                                                                            // Not including EOF.
			BinarySendFile.Add(StressFile.Normal,              FileType.Binary, "Stress-1-Normal.dat",                  8192, (    8192 / 256)); // Virtual lines of 256 bytes each.
			BinarySendFile.Add(StressFile.Large,               FileType.Binary, "Stress-2-Large.dat",                  82432, (   82432 / 256));
			////               StressFile.LargeWithLongLines makes little sense for binary terminals
			BinarySendFile.Add(StressFile.Huge,                FileType.Binary, "Stress-4-Huge.dat",                 1089792, ( 1089792 / 256));
			////               StressFile.HugeWithVeryLongLines makes little sense for binary terminals
			BinarySendFile.Add(StressFile.Enormous,            FileType.Binary, "Stress-5-Enormous.dat",            16299776, (16299776 / 256));

			TextSendText = new FileGroup(3);                                                                              // Including EOL; not including EOF.
			TextSendText.Add(StressFile.LongLine,              FileType.Text,   "Stress-7-LongLine.txt",                 973, 1);
			TextSendText.Add(StressFile.VeryLongLine,          FileType.Text,   "Stress-8-VeryLongLine.txt",            9991, 1);
			TextSendText.Add(StressFile.EnormousLine,          FileType.Text,   "Stress-9-EnormousLine.txt",          500014, 1);

			BinarySendText = new FileGroup(3);                                                                            // Not including EOL; not including EOF.
			BinarySendText.Add(StressFile.LongLine,            FileType.Text,   "Stress-7-LongLine.txt",                 971, 1);
			BinarySendText.Add(StressFile.VeryLongLine,        FileType.Text,   "Stress-8-VeryLongLine.txt",            9989, 1);
			BinarySendText.Add(StressFile.EnormousLine,        FileType.Text,   "Stress-9-EnormousLine.txt",          500012, 1);

		////SendRaw = new FileGroup(3);                                                                                   // Not including EOF.
		////SendRaw.Add(StressFile.LongData,                   FileType.Text,   "Stress-7-LongData.txt",                 973, 1);
		////SendRaw.Add(StressFile.VeryLongData,               FileType.Text,   "Stress-8-VeryLongData.txt",            9991, 1);
		////SendRaw.Add(StressFile.EnormousData,               FileType.Text,   "Stress-9-EnormousData.txt",          500014, 1);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
