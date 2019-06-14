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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY;
using MKY.Settings;
using MKY.Windows.Forms;

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
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Don't care, it's for debugging only...")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[TestFixtureSetUp]
		public virtual void TestFixtureSetUp()
		{
			// Create temporary in-memory application settings for this test run:
			ApplicationSettings.Create(ApplicationSettingsFileAccess.None);

			// Prevent auto-save of workspace settings:
			ApplicationSettings.LocalUserSettings.General.AutoSaveWorkspace = false;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			// Close and dispose of temporary in-memory application settings:
			ApplicationSettings.CloseAndDispose();
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > YAT 2.0 Final Version 2.0.0
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Final Version 2.0.0
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V2_0_0_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V2_0_0.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V2_0_0_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V2_0_0.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V2_0_0.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V2_0_0.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V2_0_0.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V2_0_0.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V2_0_0.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V2_0_0.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V2_0_0_WorkspaceSettingsCase09()
		{
			ExecuteSettingsCase09(SettingsFilesProvider.FilePaths_V2_0_0.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_09_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Epsilon Version 1.99.90
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Epsilon Version 1.99.90
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_90_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_90.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_90_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_90.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_90.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_90.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_90.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_90.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_90.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_90.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_90_WorkspaceSettingsCase09()
		{
			ExecuteSettingsCase09(SettingsFilesProvider.FilePaths_V1_99_90.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_09_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Delta Version 1.99.80
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Delta Version 1.99.80
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_80_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_80.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_80_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_80.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_80.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_80.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_80.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_80.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_80.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_80_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_80.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 3 Version 1.99.70
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 3 Version 1.99.70
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_70_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_70.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_70_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_70.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_TerminalSettingsCase03()
		{
			ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_70.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_70.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_70.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_70.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_70.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_70_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_70.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 2'' Version 1.99.52
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 2'' Version 1.99.52
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_52_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_52.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_52_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_52.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_TerminalSettingsCase03()
		{
		////Case03 of V1.99.5x fail (bugs #232 and #246 "Issues with [Alternate]TolerantXmlSerializer").
		////ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_52.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_52.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_52.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_52.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_52.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_52_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_52.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 2' Version 1.99.51
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 2' Version 1.99.51
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_51_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_51.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_51_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_51.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_TerminalSettingsCase03()
		{
		////Case03 of V1.99.5x fail (bugs #232 and #246 "Issues with [Alternate]TolerantXmlSerializer").
		////ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_51.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_51.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_51.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_51.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_51.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_51_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_51.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 2 Version 1.99.50
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 2 Version 1.99.50
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_50_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_50.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
		public virtual void Test_V1_99_50_TerminalSettingsCase02()
		{
			ExecuteSettingsCase02(SettingsFilesProvider.FilePaths_V1_99_50.TerminalFilePaths[TerminalSettingsTestCase.T_02_COM2_Open_Binary_115200]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_TerminalSettingsCase03()
		{
		////Case03 of V1.99.5x fail (bugs #232 and #246 "Issues with [Alternate]TolerantXmlSerializer").
		////ExecuteSettingsCase03(SettingsFilesProvider.FilePaths_V1_99_50.TerminalFilePaths[TerminalSettingsTestCase.T_03_COM1_Closed_Predefined]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_WorkspaceSettingsCase04()
		{
			ExecuteSettingsCase04(SettingsFilesProvider.FilePaths_V1_99_50.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_04_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_TerminalSettingsCase05()
		{
			ExecuteSettingsCase05(SettingsFilesProvider.FilePaths_V1_99_50.TerminalFilePaths[TerminalSettingsTestCase.T_05_COM1_Open_Recent]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_WorkspaceSettingsCase06()
		{
			ExecuteSettingsCase06(SettingsFilesProvider.FilePaths_V1_99_50.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_06_Matthias]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_TerminalSettingsCase07()
		{
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_50.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_50_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_50.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1'' Version 1.99.34
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1'' Version 1.99.34
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_34_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_34.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_34_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_34.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1' Version 1.99.33
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1' Version 1.99.33
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_33_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_33.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_33_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_33.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Gamma 1 Version 1.99.32
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Gamma 1 Version 1.99.32
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_32_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_32.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_32_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_32.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_30_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_30.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_30_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_30.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_28_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_28.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_28_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_28.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 4 Version 1.99.26
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_26_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_26.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test]
		public virtual void Test_V1_99_26_WorkspaceSettingsCase08()
		{
			ExecuteSettingsCase08(SettingsFilesProvider.FilePaths_V1_99_26.WorkspaceFilePaths[WorkspaceSettingsTestCase.W_08_Matthias]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 3 Version 1.99.25
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_25_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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
			ExecuteSettingsCase07(SettingsFilesProvider.FilePaths_V1_99_25.TerminalFilePaths[TerminalSettingsTestCase.T_07_USB_SerHID_VID0EB8_PID2303_YAT8_Closed]);
		}

		#endregion

		#region Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 2 Version 1.99.24
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_24_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_24.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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

		#region Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------
		// Tests > YAT 2.0 Beta 3 Candidate 1 Version 1.99.22
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortAIsAvailable' is probed in test method.
		public virtual void Test_V1_99_22_TerminalSettingsCase01()
		{
			ExecuteSettingsCase01(SettingsFilesProvider.FilePaths_V1_99_22.TerminalFilePaths[TerminalSettingsTestCase.T_01_COM1_Open_Default]);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = UnderscoreSuppressionJustification)]
		[Test] // Test is mandatory, it shall not be excludable. 'PortBIsAvailable' is probed in test method.
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

		#endregion

		#region Set Up Settings
		//==========================================================================================
		// Set Up Settings
		//==========================================================================================

		private static DocumentSettingsHandler<TerminalSettingsRoot> SetupTerminalSettingsFromFilePath(string filePath)
		{
			var sh = new DocumentSettingsHandler<TerminalSettingsRoot>();
			sh.SettingsFilePath = filePath;
			if (!sh.Load())
			{
				Assert.Fail("Terminal settings could not be loaded from " + sh.SettingsFilePath);
			}
			return (sh);
		}

		private static DocumentSettingsHandler<WorkspaceSettingsRoot> SetupWorkspaceSettingsFromFilePath(string filePath)
		{
			var sh = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			sh.SettingsFilePath = filePath;
			if (!sh.Load())
			{
				Assert.Fail("Workspace settings could not be loaded from " + sh.SettingsFilePath);
			}
			return (sh);
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
			var sh = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (var t = new Model.Terminal(sh))
			{
				// Required if COM1 is not available.
				t.MessageInputRequest += terminal_MessageInputRequest;

				Assert.That(t.Start(), Is.True, @"Failed to start """ + t.Caption + @"""");

				VerifySettingsCase01(t);
			}
		}

		#endregion

		#region Settings Cases > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------
		// Settings Cases > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------

		private void ExecuteSettingsCase02(string filePath)
		{
			var sh = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (var t = new Model.Terminal(sh))
			{
				// Required if COM2 is not available.
				t.MessageInputRequest += terminal_MessageInputRequest;

				Assert.That(t.Start(), Is.True, @"Failed to start """ + t.Caption + @"""");

				VerifySettingsCase02(t);
			}
		}

		#endregion

		#region Settings Cases > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Cases > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase03(string filePath)
		{
			var sh = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (var t = new Model.Terminal(sh))
			{
				Assert.That(t.Start(), Is.True, @"Failed to start """ + t.Caption + @"""");

				VerifySettingsCase03(t);
			}
		}

		#endregion

		#region Settings Cases > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Cases > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase04(string filePath)
		{
			var sh = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (var w = new Model.Workspace(sh))
			{
				w.OpenTerminals();
				VerifySettingsCase04(w);
			}
		}

		#endregion

		#region Settings Cases > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------
		// Settings Cases > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------

		private void ExecuteSettingsCase05(string filePath)
		{
			var sh = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (var t = new Model.Terminal(sh))
			{
				// Required if COM1 is not available.
				t.MessageInputRequest += terminal_MessageInputRequest;

				Assert.That(t.Start(), Is.True, @"Failed to start """ + t.Caption + @"""");

				VerifySettingsCase05(t);
			}
		}

		#endregion

		#region Settings Cases > 06 :: Workspace :: 2 TCP/IP AutoSocket Terminals
		//------------------------------------------------------------------------------------------
		// Settings Cases > 06 :: Workspace :: 2 TCP/IP AutoSocket Terminals
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase06(string filePath)
		{
			var sh = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (var w = new Model.Workspace(sh))
			{
				w.OpenTerminals();
				VerifySettingsCase06(w);
			}
		}

		#endregion

		#region Settings Cases > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2303) YAT.8 / Closed
		//------------------------------------------------------------------------------------------
		// Settings Cases > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2303) YAT.8 / Closed
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase07(string filePath)
		{
			var sh = SetupTerminalSettingsFromFilePath(filePath);

			// Create terminal from settings and check whether settings are correctly set.
			using (var t = new Model.Terminal(sh))
			{
				Assert.That(t.Start(), Is.True, @"Failed to start """ + t.Caption + @"""");

				VerifySettingsCase07(t);
			}
		}

		#endregion

		#region Settings Cases > 08 :: Workspace :: 2 TCP/IP AutoSocket Terminals / Unicode Predefined
		//------------------------------------------------------------------------------------------
		// Settings Cases > 08 :: Workspace :: 2 TCP/IP AutoSocket Terminals / Unicode Predefined
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase08(string filePath)
		{
			var sh = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (var w = new Model.Workspace(sh))
			{
				w.OpenTerminals();
				VerifySettingsCase08(w);
			}
		}

		#endregion

		#region Settings Cases > 09 :: Workspace :: 6 TCP/IP AutoSocket Terminals / Encodings
		//------------------------------------------------------------------------------------------
		// Settings Cases > 09 :: Workspace :: 6 TCP/IP AutoSocket Terminals / Encodings
		//------------------------------------------------------------------------------------------

		private static void ExecuteSettingsCase09(string filePath)
		{
			var sh = SetupWorkspaceSettingsFromFilePath(filePath);

			// Create workspace from settings and check whether settings are correctly set.
			using (var w = new Model.Workspace(sh))
			{
				w.OpenTerminals();
				VerifySettingsCase09(w);
			}
		}

		#endregion

		#region Settings Cases > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings Cases > Event Handlers
		//------------------------------------------------------------------------------------------

		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = DialogResult.OK;
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
			AssertSettingsCase01Preconditions();

			Assert.That(terminal.SettingsRoot.TerminalType, Is.EqualTo(Domain.TerminalType.Text), "Terminal isn't text!");
			Assert.That((int)terminal.SettingsRoot.IO.SerialPort.PortId, Is.EqualTo(1), "Serial port isn't set to COM1!");
			Assert.That(terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, Is.EqualTo(9600), "Serial port baud rate isn't set to 9600!");
			Assert.That(terminal.IsOpen, "Terminal is not open!");
		}

		private static void AssertSettingsCase01Preconditions()
		{
			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA != "COM1")
				Assert.Fail("This test case requires that 'PortA' is configured to 'COM1'!");

			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
				Assert.Ignore("'PortA' is configured to 'COM1' but isn't available on this machine, therefore this test is excluded.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		#endregion

		#region Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 02 :: Terminal :: COM2 / Open / Binary / 115200
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase02(Model.Terminal terminal)
		{
			AssertSettingsCase02Preconditions();

			Assert.That(terminal.SettingsRoot.TerminalType, Is.EqualTo(Domain.TerminalType.Binary), "Terminal isn't binary!");
			Assert.That((int)terminal.SettingsRoot.IO.SerialPort.PortId, Is.EqualTo(2), "Serial port isn't set to COM2!");
			Assert.That(terminal.SettingsRoot.IO.SerialPort.Communication.BaudRate, Is.EqualTo(115200), "Serial port baud rate isn't set to 115200!");
			Assert.That(terminal.IsOpen, "Terminal is not open!");
		}

		private static void AssertSettingsCase02Preconditions()
		{
			if ((MKY.IO.Ports.SerialPortId)MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB != "COM2")
				Assert.Fail("This test case requires that 'PortB' is configured to 'COM2'!");

			if (!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
				Assert.Ignore("'PortB' is configured to 'COM2' but isn't available on this machine, therefore this test is excluded.");
				//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.
		}

		#endregion

		#region Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 03 :: Terminal :: COM1 / Closed / Predefined
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase03(Model.Terminal terminal)
		{
			Assert.That((int)terminal.SettingsRoot.IO.SerialPort.PortId, Is.EqualTo(1), "Serial port isn't set to COM1!");
			Assert.That(terminal.IsOpen, Is.False, "Terminal is not closed on COM1!");

			Assert.That(terminal.SettingsRoot.PredefinedCommand.Pages.Count, Is.EqualTo(2), "Predefined commands do not contain 2 pages!");

			Model.Types.PredefinedCommandPage page;
			Model.Types.Command command;

			page = terminal.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.That(page.PageName, Is.EqualTo("First Page"), "First predefined command pages has wrong name!");
			Assert.That(page.Commands.Count, Is.EqualTo(4), "First predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.That(command.Description,  Is.EqualTo("1A"));
			Assert.That(command.TextLines[0], Is.EqualTo("1A"));
			command = page.Commands[1];
			Assert.That(command.Description,  Is.EqualTo("1B"));
			Assert.That(command.TextLines[0], Is.EqualTo("1B"));
			command = page.Commands[2];
			Assert.That(command.Description,  Is.EqualTo("1C"));
			Assert.That(command.TextLines[0], Is.EqualTo("1C"));
			command = page.Commands[3];
			Assert.That(command.Description,  Is.EqualTo("1D"));
			Assert.That(command.TextLines[0], Is.EqualTo("1D"));

			page = terminal.SettingsRoot.PredefinedCommand.Pages[1];
			Assert.That(page.PageName, Is.EqualTo("Second Page"), "Second predefined command pages has wrong name!");
			Assert.That(page.Commands.Count, Is.EqualTo(4), "Second predefined command page doesn't contain 4 commands!");
			command = page.Commands[0];
			Assert.That(command.Description,  Is.EqualTo("21")); // Ensures that numbers are properly parsed as well.
			Assert.That(command.TextLines[0], Is.EqualTo("21"));
			command = page.Commands[1];
			Assert.That(command.Description,  Is.EqualTo("22"));
			Assert.That(command.TextLines[0], Is.EqualTo("22"));
			command = page.Commands[2];
			Assert.That(command.Description,  Is.EqualTo("23"));
			Assert.That(command.TextLines[0], Is.EqualTo("23"));
			command = page.Commands[3];
			Assert.That(command.Description,  Is.EqualTo("24"));
			Assert.That(command.TextLines[0], Is.EqualTo("24"));
		}

		#endregion

		#region Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 04 :: Workspace :: 2 Terminals on COM1 / COM2
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase04(Model.Workspace workspace)
		{
			Assert.That(workspace.TerminalCount, Is.EqualTo(2), "Workspace doesn't contain 2 terminals!");
		}

		#endregion

		#region Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 05 :: Terminal :: COM1 / Open / Recent
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase05(Model.Terminal terminal)
		{
			Assert.That((int)terminal.SettingsRoot.IO.SerialPort.PortId, Is.EqualTo(1), "Serial port isn't set to COM1!");

			// \todo:
			// Add tests that verify that recent contains three commands.
			// Add tests that verify that recent files contains three files commands.
		}

		#endregion

		#region Settings Case Verifications > 06 :: Workspace :: 2 TCP/IP AutoSocket Terminals
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 06 :: Workspace :: 2 TCP/IP AutoSocket Terminals
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase06(Model.Workspace workspace)
		{
			Assert.That(workspace.TerminalCount, Is.EqualTo(2), "Workspace doesn't contain 2 terminals!");

			// \todo:
			// Add tests that verify that terminals are interconnected.
		}

		#endregion

		#region Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2303) YAT.8 / Closed
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 07 :: Terminal :: USB Ser/HID (VID0EB8) (PID2303) YAT.8 / Closed
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase07(Model.Terminal terminal)
		{
			Assert.That(terminal.SettingsRoot.IOType, Is.EqualTo(Domain.IOType.UsbSerialHid), "Terminal isn't USB Ser/HID!");

			// \todo:
			// Add tests that verify that USB device info is correct.
		}

		#endregion

		#region Settings Case Verifications > 08 :: Workspace :: 2 TCP/IP AutoSocket Terminals / Unicode Predefined
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 08 :: Workspace :: 2 TCP/IP AutoSocket Terminals / Unicode Predefined
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase08(Model.Workspace workspace)
		{
			Assert.That(workspace.TerminalCount, Is.EqualTo(2), "Workspace doesn't contain 2 terminals!");

			var t1 = workspace.Terminals[0];
			var t2 = workspace.Terminals[1];

			Assert.That(t1.SettingsRoot.PredefinedCommand.Pages.Count, Is.EqualTo(1), "Predefined commands do not contain 1 page!");

			Model.Types.PredefinedCommandPage p;
			Model.Types.Command c;

			p = t1.SettingsRoot.PredefinedCommand.Pages[0];
			Assert.That(p.PageName, Is.EqualTo("Page 1"), "First predefined command pages has wrong name!");
			Assert.That(p.Commands.Count, Is.EqualTo(5), "First predefined command page doesn't contain 5 commands!");
			c = p.Commands[0];
			Assert.That(c.Description,  Is.EqualTo("abc"));
			Assert.That(c.TextLines[0], Is.EqualTo("abc"));
			c = p.Commands[1];
			Assert.That(c.Description,  Is.EqualTo("äöü"));
			Assert.That(c.TextLines[0], Is.EqualTo("äöü"));
			c = p.Commands[2];
			Assert.That(c.Description,  Is.EqualTo("ÄÖÜ"));
			Assert.That(c.TextLines[0], Is.EqualTo("ÄÖÜ"));
			c = p.Commands[3];
			Assert.That(c.Description,  Is.EqualTo("$£€"));
			Assert.That(c.TextLines[0], Is.EqualTo("$£€"));
			c = p.Commands[4];
			Assert.That(c.Description,  Is.EqualTo("čěř"));
			Assert.That(c.TextLines[0], Is.EqualTo("čěř"));

			// \todo:
			// Add tests that verify that terminals are interconnected.

			UnusedLocal.PreventAnalysisWarning(t2);

			// \todo:
			// Add tests that send the commands to terminal 2.

			// \todo:
			// Add tests that verify the commands at terminal 2.
		}

		#endregion

		#region Settings Case Verifications > 09 :: Workspace :: 6 TCP/IP AutoSocket Terminals / Encodings
		//------------------------------------------------------------------------------------------
		// Settings Case Verifications > 09 :: Workspace :: 6 TCP/IP AutoSocket Terminals / Encodings
		//------------------------------------------------------------------------------------------

		private static void VerifySettingsCase09(Model.Workspace workspace)
		{
			Assert.That(workspace.TerminalCount, Is.EqualTo(6), "Workspace doesn't contain 6 terminals!");

			foreach (var t in workspace.Terminals)
			{
				if (t.IndicatedName.Contains("Binary Server"))
					break; // No test for binary server yet.

				Assert.That(t.SettingsRoot.PredefinedCommand.Pages.Count, Is.EqualTo(1), "Predefined commands do not contain 1 page!");

				Model.Types.PredefinedCommandPage p;
				Model.Types.Command c;

				p = t.SettingsRoot.PredefinedCommand.Pages[0];
				Assert.That(p.PageName, Is.EqualTo("Page 1"), "First predefined command pages has wrong name!");
				Assert.That(p.Commands.Count, Is.EqualTo(8), "First predefined command page doesn't contain 8 commands!"); // 7 + 1 dummy.
				c = p.Commands[0];
				Assert.That(c.Description,  Is.EqualTo("abc"));
				Assert.That(c.TextLines[0], Is.EqualTo("abc"));
				c = p.Commands[1];
				Assert.That(c.Description,  Is.EqualTo("äöü"));
				Assert.That(c.TextLines[0], Is.EqualTo("äöü"));
				c = p.Commands[2];
				Assert.That(c.Description,  Is.EqualTo("ÄÖÜ"));
				Assert.That(c.TextLines[0], Is.EqualTo("ÄÖÜ"));
				c = p.Commands[3];
				Assert.That(c.Description,  Is.EqualTo("$£€"));
				Assert.That(c.TextLines[0], Is.EqualTo("$£€"));
				c = p.Commands[4];
				Assert.That(c.Description,  Is.EqualTo("čěř"));
				Assert.That(c.TextLines[0], Is.EqualTo("čěř"));
				c = p.Commands[5];
				Assert.That(c.Description,  Is.EqualTo("一二州"));
				Assert.That(c.TextLines[0], Is.EqualTo("一二州"));
				c = p.Commands[6];
				Assert.That(c.Description,  Is.EqualTo("︙"));
				Assert.That(c.TextLines[0], Is.EqualTo("︙"));

				// \todo:
				// ...
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
