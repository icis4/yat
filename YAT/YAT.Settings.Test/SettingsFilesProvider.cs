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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
	public enum RoamingUserSettingsTestCase
	{
		Dummy,
	}

	/// <summary></summary>
	public enum LocalUserSettingsTestCase
	{
		Dummy,
	}

	/// <summary></summary>
	public enum TerminalSettingsTestCase
	{
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		T_Empty,

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
	}

	/// <summary></summary>
	public enum WorkspaceSettingsTestCase
	{
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_Empty,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_04_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_06_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_08_Matthias,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "As always, there are exceptions to the rules...")]
		W_09_Matthias,
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
		/// <summary></summary>
		public string Path { get; }

		/// <summary></summary>
		public Dictionary<RoamingUserSettingsTestCase, string> RoamingUserFilePaths { get; }

		/// <summary></summary>
		public Dictionary<LocalUserSettingsTestCase, string> LocalUserFilePaths { get; }

		/// <summary></summary>
		public Dictionary<TerminalSettingsTestCase, string> TerminalFilePaths { get; }

		/// <summary></summary>
		public Dictionary<WorkspaceSettingsTestCase, string> WorkspaceFilePaths { get; }

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
				Path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-Settings" + System.IO.Path.DirectorySeparatorChar;
			else
				Path = di.FullName + System.IO.Path.DirectorySeparatorChar + "!-Settings" + System.IO.Path.DirectorySeparatorChar + directory + System.IO.Path.DirectorySeparatorChar;

			RoamingUserFilePaths = new Dictionary<RoamingUserSettingsTestCase, string>();
			LocalUserFilePaths   = new Dictionary<LocalUserSettingsTestCase,   string>();
			TerminalFilePaths    = new Dictionary<TerminalSettingsTestCase,    string>();
			WorkspaceFilePaths   = new Dictionary<WorkspaceSettingsTestCase,   string>();
		}

		/// <summary></summary>
		public void AddTerminalFileName(TerminalSettingsTestCase fileKey, string fileName)
		{
			TerminalFilePaths.Add(fileKey, Path + fileName);
		}

		/// <summary></summary>
		public void AddWorkspaceFileName(WorkspaceSettingsTestCase fileKey, string fileName)
		{
			WorkspaceFilePaths.Add(fileKey, Path + fileName);
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
		public static readonly SettingsFilePaths FilePaths_Empty;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_Current;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V2_0_0;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_90;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_80;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_70;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_52;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_51;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_50;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_34;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_33;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_32;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_30;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_28;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_26;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_25;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_24;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Ease of implementation, especially adding new settings.")]
		public static readonly SettingsFilePaths FilePaths_V1_99_22;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future test cases may have to implement more logic in the constructor, and anyway, performance isn't an issue here.")]
		static SettingsFilesProvider()
		{
			// Empty
			FilePaths_Empty = new SettingsFilePaths("!-Empty");

			FilePaths_Empty.AddTerminalFileName(TerminalSettingsTestCase.T_Empty, "Empty.yat");
			FilePaths_Empty.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_Empty, "Empty.yaw");

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
			FilePaths_Current.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_09_Matthias, "09 - Matthias.yaw");

			// V2.0.0
			FilePaths_V2_0_0 = new SettingsFilePaths("2018-04-13 - YAT 2.0 Final Version 2.0.0");

			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V2_0_0.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V2_0_0.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V2_0_0.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V2_0_0.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");
			FilePaths_V2_0_0.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_09_Matthias, "09 - Matthias.yaw");

			// V1.99.90
			FilePaths_V1_99_90 = new SettingsFilePaths("2018-01-12 - YAT 2.0 Epsilon Version 1.99.90");

			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_90.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_90.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_90.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_90.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");
			FilePaths_V1_99_90.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_09_Matthias, "09 - Matthias.yaw");

			// V1.99.80
			FilePaths_V1_99_80 = new SettingsFilePaths("2017-10-15 - YAT 2.0 Delta Version 1.99.80");

			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_80.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_80.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_80.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_80.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.70
			FilePaths_V1_99_70 = new SettingsFilePaths("2017-07-04 - YAT 2.0 Gamma 3 Version 1.99.70");

			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_70.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_70.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_70.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_70.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.52
			FilePaths_V1_99_52 = new SettingsFilePaths("2016-09-30 - YAT 2.0 Gamma 2'' Version 1.99.52");

			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_52.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_52.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_52.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_52.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.51
			FilePaths_V1_99_51 = new SettingsFilePaths("2016-09-17 - YAT 2.0 Gamma 2' Version 1.99.51");

			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM1_Closed_Default, "00 - COM1 - Closed - Default.yat");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_00_COM2_Closed_Default, "00 - COM2 - Closed - Default.yat");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_51.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_51.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_51.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");
			FilePaths_V1_99_51.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_08_Matthias, "08 - Matthias.yaw");

			// V1.99.50
			FilePaths_V1_99_50 = new SettingsFilePaths("2016-09-16 - YAT 2.0 Gamma 2 Version 1.99.50");

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

			// V1.99.25
			FilePaths_V1_99_25 = new SettingsFilePaths("2010-11-28 - YAT 2.0 Beta 3 Candidate 3 Version 1.99.25");

			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_25.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");
			FilePaths_V1_99_25.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_06_Matthias, "06 - Matthias.yaw");
			FilePaths_V1_99_25.AddTerminalFileName(TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed, "07 - USB SerHID (VID0EB8) (PID2303) YAT.8 - Closed.yat");

			// V1.99.24
			FilePaths_V1_99_24 = new SettingsFilePaths("2010-11-11 - YAT 2.0 Beta 3 Candidate 2 Version 1.99.24");

			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_24.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
			FilePaths_V1_99_24.AddTerminalFileName(TerminalSettingsTestCase.T_05_COM1_Open_Recent, "05 - COM1 - Open - Recent.yat");

			// V1.99.22
			FilePaths_V1_99_22 = new SettingsFilePaths("2009-09-08 - YAT 2.0 Beta 3 Candidate 1 Version 1.99.22");

			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_01_COM1_Open_Default, "01 - COM1 - Open.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200, "02 - COM2 - Open - Binary - 115200.yat");
			FilePaths_V1_99_22.AddTerminalFileName(TerminalSettingsTestCase.T_03_COM1_Closed_Predefined, "03 - COM1 - Closed - Predefined.yat");
			FilePaths_V1_99_22.AddWorkspaceFileName(WorkspaceSettingsTestCase.W_04_Matthias, "04 - Matthias.yaw");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================