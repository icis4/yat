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
// MKY Version 1.0.27
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

using System.Text;
using System.Windows.Forms;

using MKY.Configuration;
using MKY.Windows.Forms;

using NUnit.Framework;

namespace MKY.Test.ConfigurationTemplate
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
			GenerateTemplate(DefaultSectionName);

			// Tell user how to proceed:
			var sb = new StringBuilder();
			foreach (var l in TemplateGenerator.DefaultInstructions_1through7)
				sb.AppendLine(l);

			sb.AppendLine(@" 8. Update the solution's effective configuration e.g.");
			sb.AppendLine(@"     "".\<Solution>.Test.config"" as required.");
			sb.AppendLine(@"     (The generic base configuration.)");
			sb.AppendLine(@" 9. Update the test assembly effective configurations in e.g.");
			sb.AppendLine(@"     ""..\!-TestConfig"" as required.");
			sb.AppendLine(@"     (The machine dependent configuration to be merged in.)");

			MessageBoxEx.Show(sb.ToString(), "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary></summary>
		public static void GenerateTemplate(string sectionName)
		{
			var c = TemplateGenerator.CreateEmpty("MKY.Test.config");
			{
				AddSectionGroups(c, sectionName);
			}
			c.Save(System.Configuration.ConfigurationSaveMode.Full, true);
		}

		/// <summary></summary>
		public static void AddSectionGroups(System.Configuration.Configuration configuration, string sectionName)
		{
			TemplateGenerator.AddSectionGroups
			(
				configuration,
				ConfigurationConstants.SelectionGroupName,
				ConfigurationConstants.SectionsGroupName,
				sectionName,
				new ConfigurationSection()
			);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
