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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Settings;

using NUnit.Framework;

using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

#endregion

namespace YAT.Settings.Test
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1630:DocumentationTextMustContainWhitespace", Justification = "Text is given by the test case identification/name.")]
	[TestFixture]
	public class FileVersionsTest
	{
		private const string UnderscoreSuppressionJustification = "As always, there are exceptions to the rules...";

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run.
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings.
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close temporary in-memory application settings.
			ApplicationSettings.Close();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > YAT 2.0 Beta 1 Version 1.99.12
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 1 Version 1.99.12
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_12_TerminalSettingsCase02()
		{
			// Ignore baud rate because it changed from enum to int from 1.99.12 to 1.99.13.
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200], true);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_12_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_12.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Preliminary Version 1.99.13
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_13_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_13_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_13.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_13_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_13.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 1 Version 1.99.17
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_17_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_17_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_17.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_17_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_17.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 2 Version 1.99.18
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_18_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_18_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_18.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_18_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_18.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 3 Version 1.99.19
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_19_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_19_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_19.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_19_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_19.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 2 Candidate 4 Version 1.99.20
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_20_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_20_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_20.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_20_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_20.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_22_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_22_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_22_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_22.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_24_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_24_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_24.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_24_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_25_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_25_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_25.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_25.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_25_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_26_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_26_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_28_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_28_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_28.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_28.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_28.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_30_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_30_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_30.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_30.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_30.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1 Version 1.99.32
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1 Version 1.99.32
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_32_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_32_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_32.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_32.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_32.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1' Version 1.99.33
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1' Version 1.99.33
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_33_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_33_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_33.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_33.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_33.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1'' Version 1.99.34
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1'' Version 1.99.34
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortAIsAvailableCategory]
		public virtual void Test_V1_99_34_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test, MKY.IO.Ports.Test.PortBIsAvailableCategory]
		public virtual void Test_V1_99_34_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_34.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_34.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2200_MK8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_34.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#endregion

		#region Set Up Settings
		//==========================================================================================
		// Set Up Settings
		//==========================================================================================

		private static DocumentSettingsHandler<TerminalSettingsRoot> SetupTerminalSettingsFromFilePath(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>();
			settingsHandler.SettingsFilePath = filePath;
			if (!settingsHandler.Load())
			{
				Assert.Fail("Terminal settings could not be loaded from " + settingsHandler.SettingsFilePath);
			}
			return (settingsHandler);
		}

		private static DocumentSettingsHandler<WorkspaceSettingsRoot> SetupWorkspaceSettingsFromFilePath(string filePath)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			settingsHandler.SettingsFilePath = filePath;
			if (!settingsHandler.Load())
			{
				Assert.Fail("Workspace settings could not be loaded from " + settingsHandler.SettingsFilePath);
			}
			return (settingsHandler);
		}

		#endregion

		#region Settings Cases
		//==========================================================================================
		// Settings Cases
		//==========================================================================================

		#region Settings Cases > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------
		// Settings Cases > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------

		private void ExecuteSettingsCase01(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				// Required if COM1 is not available.
				terminal.MessageInputRequest += new EventHandler<Model.MessageInputEventArgs>(terminal_MessageInputRequest);

				Assert.IsTrue(terminal.Start(), @"Failed to start """ + terminal.Caption + @"""");

				VerifySettingsCase01(terminal);
			}
		}

		#endregion

		#region Settings Cases > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------
		// Settings Cases > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------

		private void ExecuteSettingsCase02(string filePath)
		{
			ExecuteSettingsCase02(filePath, false);
		}

		private void ExecuteSettingsCase02(string filePath, bool ignoreBaudRate)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				// Required if COM2 is not available.
				terminal.MessageInputRequest += new EventHandler<Model.MessageInputEventArgs>(terminal_MessageInputRequest);

				Assert.IsTrue(terminal.Start(), @"Failed to start """ + terminal.Caption + @"""");

				VerifySettingsCase02(terminal, ignoreBaudRate);
			}
		}

		#endregion

		#region Settings Cases > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Cases > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase03(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				Assert.IsTrue(terminal.Start(), @"Failed to start """ + terminal.Caption + @"""");

				VerifySettingsCase03(terminal);
			}
		}

		#endregion

		#region Settings Cases > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Cases > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase04(string filePath)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase04(workspace);
			}
		}

		#endregion

		#region Settings Cases > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------
		// Settings Cases > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------

		private void ExecuteSettingsCase05(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				// Required if COM1 is not available.
				terminal.MessageInputRequest += new EventHandler<Model.MessageInputEventArgs>(terminal_MessageInputRequest);

				Assert.IsTrue(terminal.Start(), @"Failed to start """ + terminal.Caption + @"""");

				VerifySettingsCase05(terminal);
			}
		}

		#endregion

		#region Settings Cases > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------
		// Settings Cases > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase06(string filePath)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase06(workspace);
			}
		}

		#endregion

		#region Settings Cases > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------
		// Settings Cases > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase07(string filePath)
		{
			DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (Model.Terminal terminal = new Model.Terminal(settingsHandler))
			{
				Assert.IsTrue(terminal.Start(), @"Failed to start """ + terminal.Caption + @"""");

				VerifySettingsCase07(terminal);
			}
		}

		#endregion

		#region Settings Cases > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------
		// Settings Cases > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase08(string filePath)
		{
			DocumentSettingsHandler<WorkspaceSettingsRoot> settingsHandler = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (Model.Workspace workspace = new Model.Workspace(settingsHandler))
			{
				workspace.OpenTerminals();
				VerifySettingsCase08(workspace);
			}
		}

		#endregion

		#region Settings Cases > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings Cases > Event Handlers
		//------------------------------------------------------------------------------------------

		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = System.Windows.Forms.DialogResult.OK;
		}

		#endregion

		#endregion

		#region Settings Case Verifications
		//==========================================================================================
		// Settings Case Verifications
		//==========================================================================================

		#region Settings Case Verifications > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 01 :: Terminal :: COM1 / Open / Default
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase01(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");

			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA == "COM1")
			{
				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
					Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM1!");
				else
					Assert.Ignore("'PortA' is configured to 'COM1' but isn't available on this machine, therefore this test is excluded.");
					// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
			}
			else
			{
				Assert.Fail("This test case requires that 'PortA' is configured to 'COM1'!");
			}
		}

		#endregion

		#region Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase02(Model.Terminal terminal, bool ignoreBaudRate)
		{
			Assert.AreEqual(Domain.TerminalType.Binary, terminal.SettingsRoot.TerminalType, "Terminal isn't binary!");
			Assert.AreEqual(2, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM2!");

			if (!ignoreBaudRate) // Optionally ignore baud rate because it changed from enum to int from 1.99.12 to 1.99.13.
				Assert.AreEqual(115200, terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, "Serial port baud rate isn't set to 115200!");

			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB == "COM2")
			{
				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
					Assert.IsTrue(terminal.IsOpen, "Terminal is not open on COM2!");
				else
					Assert.Ignore("'PortB' is configured to 'COM2' but isn't available on this machine, therefore this test is excluded.");
					// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
			}
			else
			{
				Assert.Fail("This test case requires that 'PortB' is configured to 'COM2'!");
			}
		}

		#endregion

		#region Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase03(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");
			Assert.IsFalse(terminal.IsOpen, "Terminal is not closed on COM1!");

			Assert.AreEqual(2, terminal.SettingsRoot.PredefinedCommand.Pages.Count, "Predefined commands do not contain 2 pages!");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual("First Page", page.PageName, "First predefined command pages has wrong name!");
			Assert.AreEqual(4, page.Commands.Count, "First predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.AreEqual("1A", command.Description);
			Assert.AreEqual("1A", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("1B", command.Description);
			Assert.AreEqual("1B", command.CommandLines[0]);
			command = page.Commands[2];
			Assert.AreEqual("1C", command.Description);
			Assert.AreEqual("1C", command.CommandLines[0]);
			command = page.Commands[3];
			Assert.AreEqual("1D", command.Description);
			Assert.AreEqual("1D", command.CommandLines[0]);

			page = terminal.SettingsRoot.PredefinedCommand.Pages[1];
			Assert.AreEqual("Second Page", page.PageName, "Second predefined command pages has wrong name!");
			Assert.AreEqual(4, page.Commands.Count, "Second predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.AreEqual("21", command.Description); // Ensures that numbers are properly parsed as well.
			Assert.AreEqual("21", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("22", command.Description);
			Assert.AreEqual("22", command.CommandLines[0]);
			command = page.Commands[2];
			Assert.AreEqual("23", command.Description);
			Assert.AreEqual("23", command.CommandLines[0]);
			command = page.Commands[3];
			Assert.AreEqual("24", command.Description);
			Assert.AreEqual("24", command.CommandLines[0]);
		}

		#endregion

		#region Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase04(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");
		}

		#endregion

		#region Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase05(Model.Terminal terminal)
		{
			Assert.AreEqual(1, terminal.SettingsRoot.IO.SerialPort.PortId, "Serial port isn't set to COM1!");

			// \todo:
			// Add tests that verify that recent contains three commands.
			// Add tests that verify that recent files contains three files commands.
		}

		#endregion

		#region Settings Case Verifications > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 06 :: Workspace :: 2 TCP AutoSocket Terminals
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase06(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");

			// \todo:
			// Add tests that verify that terminals are interconnected.
		}

		#endregion

		#region Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2200) MK.8 / Closed
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase07(Model.Terminal terminal)
		{
			Assert.AreEqual(Domain.IOType.UsbSerialHid, terminal.SettingsRoot.IOType, "Terminal isn't USB Ser/HID!");

			// \todo:
			// Add tests that verify that USB device info is correct.
		}

		#endregion

		#region Settings Case Verifications > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 08 :: Workspace :: 2 TCP AutoSocket Terminals with Unicode Predefined
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase08(Model.Workspace workspace)
		{
			Assert.AreEqual(2, workspace.TerminalCount, "Workspace doesn't contain 2 terminals!");

			Model.Terminal terminal1 = workspace.Terminals[0];
			Model.Terminal terminal2 = workspace.Terminals[1];

			Assert.AreEqual(1, terminal1.SettingsRoot.PredefinedCommand.Pages.Count, "Predefined commands do not contain 1 page!");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal1.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.AreEqual("Page 1", page.PageName, "First predefined command pages has wrong name!");
			Assert.AreEqual(5, page.Commands.Count, "First predefined command page doesn't contain 5 commands!");
			command = page.Commands[0];
			Assert.AreEqual("abc", command.Description);
			Assert.AreEqual("abc", command.CommandLines[0]);
			command = page.Commands[1];
			Assert.AreEqual("äöü", command.Description);
			Assert.AreEqual("äöü", command.CommandLines[0]);
			command = page.Commands[2];
			Assert.AreEqual("ÄÖÜ", command.Description);
			Assert.AreEqual("ÄÖÜ", command.CommandLines[0]);
			command = page.Commands[3];
			Assert.AreEqual("$£€", command.Description);
			Assert.AreEqual("$£€", command.CommandLines[0]);
			command = page.Commands[4];
			Assert.AreEqual("čěř", command.Description);
			Assert.AreEqual("čěř", command.CommandLines[0]);

			// \todo:
			// Add tests that verify that terminals are interconnected.

			UnusedLocal.PreventAnalysisWarning(terminal2);

			// \todo:
			// Add tests that send the commands to terminal 2.

			// \todo:
			// Add tests that verify the commands at terminal 2.
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
