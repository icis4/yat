//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT/YAT.cs $
// $Author: maettu_this $
// $Date: 2010-04-11 19:35:51 +0200 (So, 11 Apr 2010) $
// $Revision: 285 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace YAT.Test
{
	/// <summary>
	/// Creates the defaults of the test settings in the solution directory.
	/// </summary>
	public class TestSettingsDefaults
	{
		/// <summary></summary>
		[STAThread]
		public static void Main(string[] args)
		{
			System.Configuration.ConfigurationManager

			/*

			// Configure test settings.
			MKY.Test.SettingsModeProvider.Mode = MKY.Test.SettingsMode.CreateDefaultSolutionFile;
			MKY.Test.SettingsDefaults.CreateConfiguration("Default");

			// Force initialization of all test settings.
			// MKY
			MKY.IO.Ports.Test.Settings.ForceStaticInitialization();
		////MKY.IO.Serial.Test.Settings.ForceStaticInitialization());
			MKY.IO.Usb.Test.Settings.ForceStaticInitialization();
		////MKY.Utilities.Test.Settings.ForceStaticInitialization();
		////MKY.Windows.Forms.Test.Settings.ForceStaticInitialization();

			// YAT
		////YAT.Controller.Test.Settings.ForceStaticInitialization();
		////YAT.Domain.Test.Settings.ForceStaticInitialization();
		////YAT.Model.Test.Settings.ForceStaticInitialization();
		////YAT.Settings.Test.Settings.ForceStaticInitialization();

			// Save default solution test settings.
			MKY.Test.SettingsDefaults.SaveToSolutionFile();
			
			*/
		}
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT/YAT.cs $
//==================================================================================================
