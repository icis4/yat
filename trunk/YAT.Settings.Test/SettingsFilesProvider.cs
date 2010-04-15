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
using System.IO;

namespace YAT.Settings.Test
{
	#region Types
	//==========================================================================================
	// Types
	//==========================================================================================

	#region Types > Settings Enums
	//------------------------------------------------------------------------------------------
	// Types > Settings Enums
	//------------------------------------------------------------------------------------------

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum TerminalSettingsTestCases
	{
		T_01_COM1_Open_Default,
		T_02_COM2_Open_Binary_115200,
		T_03_COM1_Closed_Predefined,
	}

	/// <summary></summary>
	public enum WorkspaceSettingsTestCases
	{
		W_04_Matthias,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	public struct SettingsFilePaths
	{
		private readonly string FilePath;

		/// <summary></summary>
		public readonly Dictionary<TerminalSettingsTestCases, string> TerminalFilePaths;

		/// <summary></summary>
		public readonly Dictionary<WorkspaceSettingsTestCases, string> WorkspaceFilePaths;

		/// <summary></summary>
		public SettingsFilePaths(string directory)
		{
			// Traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>".
			DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// Set path to "<Root>\this.Settings\<directory>\".
			FilePath = di.FullName + Path.DirectorySeparatorChar + "_Settings" + Path.DirectorySeparatorChar + directory + Path.DirectorySeparatorChar;

			TerminalFilePaths  = new Dictionary<TerminalSettingsTestCases,  string>();
			WorkspaceFilePaths = new Dictionary<WorkspaceSettingsTestCases, string>();
		}

		/// <summary></summary>
		public void AddTerminalFileName(TerminalSettingsTestCases fileKey, string fileName)
		{
			TerminalFilePaths.Add(fileKey, FilePath + fileName);
		}

		/// <summary></summary>
		public void AddWorkspaceFileName(WorkspaceSettingsTestCases fileKey, string fileName)
		{
			WorkspaceFilePaths.Add(fileKey, FilePath + fileName);
		}
	}

	#endregion

	/// <summary></summary>
	public static class SettingsFilesProvider
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_12;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_13;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_17;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_18;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_19;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_20;

		/// <summary></summary>
		public static readonly SettingsFilePaths FilePaths_V1_99_22;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		static SettingsFilesProvider()
		{
			// V1.99.12
			FilePaths_V1_99_12 = new SettingsFilePaths("2007-04-15 - YAT 2.0 Beta 1 Version 1.99.12");

			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			// V1.99.13
			FilePaths_V1_99_13 = new SettingsFilePaths("2007-08-30 - YAT 2.0 Beta 2 Preliminary Version 1.99.13");

			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_13.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.17
			FilePaths_V1_99_17 = new SettingsFilePaths("2008-02-11 - YAT 2.0 Beta 2 Candidate 1 Version 1.99.17");

			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_17.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.18
			FilePaths_V1_99_18 = new SettingsFilePaths("2008-03-17 - YAT 2.0 Beta 2 Candidate 2 Version 1.99.18");

			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_18.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.19
			FilePaths_V1_99_19 = new SettingsFilePaths("2008-04-01 - YAT 2.0 Beta 2 Candidate 3 Version 1.99.19");

			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_19.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.20
			FilePaths_V1_99_20 = new SettingsFilePaths("2008-07-18 - YAT 2.0 Beta 2 Candidate 4 Version 1.99.20");

			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_20.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.22
			FilePaths_V1_99_22 = new SettingsFilePaths("2009-09-08 - YAT 2.0 Beta 3 Candidate 1 Version 1.99.22");

			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_22.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");
		}

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary></summary>
		public static SettingsFilePaths FilePaths_Current
		{
			get { return (FilePaths_V1_99_22); }
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
