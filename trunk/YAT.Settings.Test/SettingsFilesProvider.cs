//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
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

	public enum TerminalSettingsTestCases
	{
		T_01_COM1_Open_Default,
		T_02_COM2_Open_Binary_115200,
		T_03_COM1_Closed_Predefined,
	}

	public enum WorkspaceSettingsTestCases
	{
		W_04_Matthias,
	}

	#endregion

	public struct SettingsFilePaths
	{
		private readonly string _Path;

		public readonly Dictionary<TerminalSettingsTestCases,  string> TerminalFilePaths;
		public readonly Dictionary<WorkspaceSettingsTestCases, string> WorkspaceFilePaths;

		public SettingsFilePaths(string directory)
		{
			// traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>"
			DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// set path to "<Root>\_Settings\<directory>\"
			_Path = di.FullName + Path.DirectorySeparatorChar + "_Settings" + Path.DirectorySeparatorChar + directory + Path.DirectorySeparatorChar;

			TerminalFilePaths  = new Dictionary<TerminalSettingsTestCases,  string>();
			WorkspaceFilePaths = new Dictionary<WorkspaceSettingsTestCases, string>();
		}

		public void AddTerminalFileName(TerminalSettingsTestCases fileKey, string fileName)
		{
			TerminalFilePaths.Add(fileKey, _Path + fileName);
		}

		public void AddWorkspaceFileName(WorkspaceSettingsTestCases fileKey, string fileName)
		{
			WorkspaceFilePaths.Add(fileKey, _Path + fileName);
		}
	}

	#endregion

	public static class SettingsFilesProvider
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static SettingsFilePaths _settingsFilePaths_V1_99_12;
		private static SettingsFilePaths _settingsFilePaths_V1_99_13;
		private static SettingsFilePaths _settingsFilePaths_V1_99_17;
		private static SettingsFilePaths _settingsFilePaths_V1_99_18;
		private static SettingsFilePaths _settingsFilePaths_V1_99_19;
		private static SettingsFilePaths _settingsFilePaths_V1_99_20;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		static SettingsFilesProvider()
		{
			// V1.99.12
			_settingsFilePaths_V1_99_12 = new SettingsFilePaths("2007-04-15 - YAT 2.0 Beta 1 Version 1.99.12");

			_settingsFilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			_settingsFilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			// V1.99.13
			_settingsFilePaths_V1_99_13 = new SettingsFilePaths("2007-08-30 - YAT 2.0 Beta 2 Preliminary Version 1.99.13");

			_settingsFilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			_settingsFilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			_settingsFilePaths_V1_99_13.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.17
			_settingsFilePaths_V1_99_17 = new SettingsFilePaths("2008-02-11 - YAT 2.0 Beta 2 Candidate 1 Version 1.99.17");

			_settingsFilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			_settingsFilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			_settingsFilePaths_V1_99_17.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.18
			_settingsFilePaths_V1_99_18 = new SettingsFilePaths("2008-03-17 - YAT 2.0 Beta 2 Candidate 2 Version 1.99.18");

			_settingsFilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			_settingsFilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			_settingsFilePaths_V1_99_18.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.19
			_settingsFilePaths_V1_99_19 = new SettingsFilePaths("2008-04-01 - YAT 2.0 Beta 2 Candidate 3 Version 1.99.19");

			_settingsFilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			_settingsFilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			_settingsFilePaths_V1_99_19.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.20
			_settingsFilePaths_V1_99_20 = new SettingsFilePaths("2008-07-18 - YAT 2.0 Beta 2 Candidate 4 Version 1.99.20");

			_settingsFilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			_settingsFilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			_settingsFilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCases.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");

			_settingsFilePaths_V1_99_20.AddWorkspaceFileName(WorkspaceSettingsTestCases.W_04_Matthias, "04 - Matthias.yaw");
		}

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		public static SettingsFilePaths FilePaths_V1_99_12
		{
			get { return (_settingsFilePaths_V1_99_12); }
		}

		public static SettingsFilePaths FilePaths_V1_99_13
		{
			get { return (_settingsFilePaths_V1_99_13); }
		}

		public static SettingsFilePaths FilePaths_V1_99_17
		{
			get { return (_settingsFilePaths_V1_99_17); }
		}

		public static SettingsFilePaths FilePaths_V1_99_18
		{
			get { return (_settingsFilePaths_V1_99_18); }
		}

		public static SettingsFilePaths FilePaths_V1_99_19
		{
			get { return (_settingsFilePaths_V1_99_19); }
		}

		public static SettingsFilePaths FilePaths_V1_99_20
		{
			get { return (_settingsFilePaths_V1_99_20); }
		}

		public static SettingsFilePaths FilePaths_Current
		{
			get { return (_settingsFilePaths_V1_99_20); }
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
