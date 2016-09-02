//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
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
	public enum LocalUserSettingsTestCase
	{
		Dummy,
	}

	/// <summary></summary>
	public enum TerminalSettingsTestCase
	{
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_00_COM1_Closed_Default,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_00_COM2_Closed_Default,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_01_COM1_Open_Default,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_02_COM2_Open_Binary_115200,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_03_COM1_Closed_Predefined,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "COM", Justification = "As always, there are exceptions to the rules...")]
		T_05_COM1_Open_Recent,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "USB", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HID", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "VID", Justification = "As always, there are exceptions to the rules...")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PID", Justification = "As always, there are exceptions to the rules...")]
		T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		T_Empty,
	}

	/// <summary></summary>
	public enum WorkspaceSettingsTestCase
	{
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_04_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_06_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_08_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_Empty,
	}

	#pragma warning restore 1591

	#endregion

	#region Types > Settings File Paths
	//------------------------------------------------------------------------------------------
	// Types > Settings File Paths
	//------------------------------------------------------------------------------------------

	/// <summary></summary>
	public class SettingsFilePaths
	{
		private string path;
		private Dictionary<LocalUserSettingsTestCase, string> localUserFilePaths;
		private Dictionary<TerminalSettingsTestCase, string>  terminalFilePaths;
		private Dictionary<WorkspaceSettingsTestCase, string> workspaceFilePaths;

		/// <summary></summary>
		public SettingsFilePaths()
			: this(null)
		{
		}

		/// <summary></summary>
		public SettingsFilePaths(string directory)
		{
			// Traverse path from "<Root>\YAT\bin\[Debug|Release]\YAT.exe" to "<Root>".
			System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
			for (int i = 0; i < 3; i++)
				di = di.Parent;

			// Set path to "<Root>\!-Settings\" or "<Root>\!-Settings\<Directory>\".
			if (string.IsNullOrEmpty(directory))
				this.path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-Settings" + System.IO.Path.DirectorySeparatorChar;
			else
				this.path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-Settings" + System.IO.Path.DirectorySeparatorChar + directory + System.IO.Path.DirectorySeparatorChar;

			this.localUserFilePaths = new Dictionary<LocalUserSettingsTestCase, string>();
			this.terminalFilePaths  = new Dictionary<TerminalSettingsTestCase,  string>();
			this.workspaceFilePaths = new Dictionary<WorkspaceSettingsTestCase, string>();
		}

		/// <summary></summary>
		public string Path
		{
			get { return (this.path); }
		}

		/// <summary></summary>
		public Dictionary<LocalUserSettingsTestCase, string> LocalUserFilePaths
		{
			get { return (this.localUserFilePaths); }
		}

		/// <summary></summary>
		public Dictionary<TerminalSettingsTestCase, string> TerminalFilePaths
		{
			get { return (this.terminalFilePaths); }
		}

		/// <summary></summary>
		public Dictionary<WorkspaceSettingsTestCase, string> WorkspaceFilePaths
		{
			get { return (this.workspaceFilePaths); }
		}

		/// <summary></summary>
		public void AddLocalUserFileName(LocalUserSettingsTestCase fileKey, string fileName)
		{
			this.localUserFilePaths.Add(fileKey, Path + fileName);
		}

		/// <summary></summary>
		public void AddTerminalFileName(TerminalSettingsTestCase fileKey, string fileName)
		{
			this.terminalFilePaths.Add(fileKey, Path + fileName);
		}

		/// <summary></summary>
		public void AddWorkspaceFileName(WorkspaceSettingsTestCase fileKey, string fileName)
		{
			this.workspaceFilePaths.Add(fileKey, Path + fileName);
		}
	}

	#endregion

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "As always, there are exceptions to the rules...")]
	public static class SettingsFilesProvider
	{
		private const string UnderscoreSuppressionJustification = "As always, there are exceptions to the rules...";

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_12;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_13;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_17;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_18;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_19;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_20;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_22;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_24;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_25;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_26;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_28;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_30;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_32;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_33;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_34;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_50;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_Current;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_Empty;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static SettingsFilesProvider()
		{
			// V1.99.12
			FilePaths_V1_99_12 = new SettingsFilePaths("2007-04-15 - YAT 2.0 Beta 1 Version 1.99.12");

			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_12.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			// V1.99.13
			FilePaths_V1_99_13 = new SettingsFilePaths("2007-08-30 - YAT 2.0 Beta 2 Preliminary Version 1.99.13");

			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default,       "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_13.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined,  "03 - COM1 - Closed - Predefined.yat");

			FilePaths_V1_99_13.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.17
			FilePaths_V1_99_17 = new SettingsFilePaths("2008-02-11 - YAT 2.0 Beta 2 Candidate 1 Version 1.99.17");

			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_17.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_17.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.18
			FilePaths_V1_99_18 = new SettingsFilePaths("2008-03-17 - YAT 2.0 Beta 2 Candidate 2 Version 1.99.18");

			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_18.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_18.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.19
			FilePaths_V1_99_19 = new SettingsFilePaths("2008-04-01 - YAT 2.0 Beta 2 Candidate 3 Version 1.99.19");

			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open - Default.yat");
			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_19.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_19.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.20
			FilePaths_V1_99_20 = new SettingsFilePaths("2008-07-18 - YAT 2.0 Beta 2 Candidate 4 Version 1.99.20");

			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_20.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_20.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.22
			FilePaths_V1_99_22 = new SettingsFilePaths("2009-09-08 - YAT 2.0 Beta 3 Candidate 1 Version 1.99.22");

			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_22.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");

			// V1.99.24
			FilePaths_V1_99_24 = new SettingsFilePaths("2010-11-11 - YAT 2.0 Beta 3 Candidate 2 Version 1.99.24");

			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_24.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");

			// V1.99.25
			FilePaths_V1_99_25 = new SettingsFilePaths("2010-11-28 - YAT 2.0 Beta 3 Candidate 3 Version 1.99.25");

			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_25.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_25.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");

			// V1.99.26
			FilePaths_V1_99_26 = new SettingsFilePaths("2011-04-25 - YAT 2.0 Beta 3 Candidate 4 Version 1.99.26");

			FilePaths_V1_99_26.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_26.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_26.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_26.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_26.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_26.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_26.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_26.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.28
			FilePaths_V1_99_28 = new SettingsFilePaths("2011-12-05 - YAT 2.0 Beta 4 Candidate 1 Version 1.99.28");

			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_28.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_28.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_28.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_28.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.30
			FilePaths_V1_99_30 = new SettingsFilePaths("2013-02-02 - YAT 2.0 Beta 4 Candidate 2 Version 1.99.30");

			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_30.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_30.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_30.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_30.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.32
			FilePaths_V1_99_32 = new SettingsFilePaths("2015-06-01 - YAT 2.0 Gamma 1 Version 1.99.32");

			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_32.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_32.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_32.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_32.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.33
			FilePaths_V1_99_33 = new SettingsFilePaths("2015-06-07 - YAT 2.0 Gamma 1' Version 1.99.33");

			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_33.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_33.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_33.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_33.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.34
			FilePaths_V1_99_34 = new SettingsFilePaths("2015-06-13 - YAT 2.0 Gamma 1'' Version 1.99.34");

			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_34.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_34.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_34.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_34.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.50
			FilePaths_V1_99_50 = new SettingsFilePaths("2016-09-09 - YAT 2.0 Gamma 2 Version 1.99.50");

			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_50.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_50.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_50.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_50.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// Current
			FilePaths_Current = new SettingsFilePaths("!-Current");

			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_Current.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_Current.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_Current.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_Current.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// Empty
			FilePaths_Empty = new SettingsFilePaths("!-Empty");

			FilePaths_Empty.AddTerminalFileName(TerminalSettingsTestCase.T_Empty, "Empty.yat");
			FilePaths_Empty.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_Empty, "Empty.yaw");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
