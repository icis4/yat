//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1' Version 1.99.33
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.IO;
using System.Text;
using System.Windows.Forms;

using MKY.IO;

using YAT.Utilities;

#endregion

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class ReleaseNotes : Form
	{
		private const string ReleaseNotesFileName = ApplicationInfo.ProductName + " Release Notes.txt";

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string ReleaseNotesFilePath = Application.StartupPath + Path.DirectorySeparatorChar + ReleaseNotesFileName;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is meant to be a constant.")]
		private readonly string ReleaseNotesDevelopmentRelativeFilePath =
			".." + Path.DirectorySeparatorChar +
			".." + Path.DirectorySeparatorChar +
			".." + Path.DirectorySeparatorChar +
			"!-Doc.User" + Path.DirectorySeparatorChar + ReleaseNotesFileName;

		/// <summary></summary>
		public ReleaseNotes()
		{
			InitializeComponent();

			// Get file path depending on development or installation:
			string filePath;
			switch (Path.GetFileName(Application.StartupPath))
			{
				case "Debug":
				case "Release":
					filePath = PathEx.CombineDirectoryAndFilePaths(Application.StartupPath, ReleaseNotesDevelopmentRelativeFilePath);
					break;

				default:
					filePath = ReleaseNotesFilePath;
					break;
			}

			// Set form title:
			Text = ApplicationInfo.ProductName + " Release Notes";

			// Open and fill release notes:
			textBox_ReleaseNotes.Text = "";
			if (File.Exists(filePath))
			{
				using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
				{
					if (sr != null)
						textBox_ReleaseNotes.Text = sr.ReadToEnd();
				}
			}
			if (textBox_ReleaseNotes.Text.Length == 0)
			{
				textBox_ReleaseNotes.Text = "Couldn't read release notes from" + Environment.NewLine + filePath;
			}
			textBox_ReleaseNotes.SelectionStart = 0;
			textBox_ReleaseNotes.SelectionLength = 0;
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
