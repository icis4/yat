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
// YAT Version 2.1.0
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
using System.Text;
using System.Windows.Forms;

using MKY.Configuration;
using MKY.Windows.Forms;

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
			const string DefaultSectionName = "Template";

			// Create dedicated configurations and save templates:
			{
				MKY.IO.Ports.Test.ConfigurationTemplate.Generator.GenerateTemplate(DefaultSectionName);
			////MKY.IO.Serial.SerialPort.Test
			////MKY.IO.Serial.Socket.Test
			////MKY.IO.Serial.Usb.Test
				MKY.IO.Usb.Test.ConfigurationTemplate.Generator.GenerateTemplate(DefaultSectionName);
				MKY.Net.Test.ConfigurationTemplate.Generator.GenerateTemplate(DefaultSectionName);
			////MKY.Test
			////MKY.Win32.Test
			////MKY.Windows.Forms.Test
			////YAT.Controller.Test
			////YAT.Domain.Test
			////YAT.View.Test
			////YAT.Model.Test
			////YAT.Settings.Test
			////YAT.Test
			}

			// Create overall configuration save template:
			var c = TemplateGenerator.CreateEmpty("YAT.Test.config");
			{
				MKY.IO.Ports.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, DefaultSectionName);
			////MKY.IO.Serial.SerialPort.Test
			////MKY.IO.Serial.Socket.Test
			////MKY.IO.Serial.Usb.Test
				MKY.IO.Usb.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, DefaultSectionName);
				MKY.Net.Test.ConfigurationTemplate.Generator.AddSectionGroups(c, DefaultSectionName);
			////MKY.Test
			////MKY.Win32.Test
			////MKY.Windows.Forms.Test
			////YAT.Controller.Test
			////YAT.Domain.Test
			////YAT.View.Test
			////YAT.Model.Test
			////YAT.Settings.Test
			////YAT.Test
			}
			c.Save(ConfigurationSaveMode.Full, true);

			// Tell user how to proceed:
			var sb = new StringBuilder();
			foreach (var l in TemplateGenerator.DefaultInstructions_1through7)
				sb.AppendLine(l);

			sb.AppendLine(@" 8. Update the solution's effective configuration e.g.");
			sb.AppendLine(@"     "".\YAT.Test.config"" as required.");
			sb.AppendLine(@"     (The generic base configuration.)");
			sb.AppendLine(@" 9. Update the test assembly effective configurations in e.g.");
			sb.AppendLine(@"     ""..\!-TestConfig"" as required.");
			sb.AppendLine(@"     (The machine dependent configuration to be merged in.)");

			MessageBoxEx.Show(sb.ToString(), "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
