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

using System.Configuration;

using MKY.Configuration;

using NUnit.Framework;

namespace YAT.Test.ConfigurationTemplate
{
	/// <summary></summary>
	[TestFixture, Explicit("This test fixture has the sole purpose of generating the configuration template for this test project")]
	public class Generator
	{
		/// <summary></summary>
		[Test]
		public virtual void GenerateTemplate()
		{
			var c = File.CreateEmpty("YAT.Test.config");

			// Add default sections for each test assembly to the overall configuration:

			MKY.IO.Ports.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, "Template");
		////MKY.IO.Serial.SerialPort.Test
		////MKY.IO.Serial.Socket.Test
		////MKY.IO.Serial.Usb.Test
			MKY.IO.Usb.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, "Template");
			MKY.Net.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, "Template");
		////MKY.Test
		////MKY.Win32.Test
		////MKY.Windows.Forms.Test
		////YAT.Controller.Test
		////YAT.Domain.Test
		////YAT.View.Test
		////YAT.Model.Test
		////YAT.Settings.Test
		////YAT.Test

			c.Save(ConfigurationSaveMode.Full, true);

			// Proceed as follows to generate template as well as effective configuration files:
			//  1. Activate and build the "Debug Test" configuration.
			//  2. Start NUnit and execute explicit test case => template file gets created.
			//  3. Go to "<Project>\bin\Debug" and filter for "*.config".
			//  4. Clean template files from unnecessary information:
			//      a) Remove the following sections:
			//          > "appSettings"
			//          > "configProtectedData"
			//          > "connectionStrings"
			//          > "system.diagnostics"
			//          > "system.windows.forms"
			//      b) Remove the version information:
			//          > In all "<sectionGroup..." remove all content from ", System.Configuration, Version=..." up to the closing quote.
			//            Attention: Files including the assembly information "System.Configuration" result in TypeLoadException's! Why? No clue...
			//          > In all "<section..." remove all content from ", Version=..." up to the very last closing quote.
			//  5. Move template file to "<Project>\ConfigurationTemplate".
			//  6. Compare the new template file against the former template file.
			//  7. Update the effective solution file ".\YAT.Test.config" as required. (This is the generic base configuration.)
			//  8. Update the effective assembly files in e.g. "..\!-TestConfig" as required. (This is the user/machine dependent configuration to be merged with.)
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
