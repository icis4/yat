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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.IO;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	public partial class ReleaseNotes : Form
	{
		private const string _ReleaseNotesFileName = @"YAT Release Notes.txt";

		private readonly string _ReleaseNotesFilePath = Application.StartupPath + Path.DirectorySeparatorChar + _ReleaseNotesFileName;
		private readonly string _ReleaseNotesDevelopmentRelativeFilePath =
			@".." + Path.DirectorySeparatorChar +
			@".." + Path.DirectorySeparatorChar +
			@".." + Path.DirectorySeparatorChar +
			@"_Doc.User" + Path.DirectorySeparatorChar + _ReleaseNotesFileName;
		//private readonly string _ReleaseNotesInstallationAbsoluteFilePath = @"C:\Programme\YAT\" + _ReleaseNotesFileName;

		public ReleaseNotes()
		{
			InitializeComponent();

			// get file path
			string _filePath;
			switch (Path.GetFileName(Application.StartupPath))
			{
				case "Debug":
				case "Release":
					_filePath = XPath.CombineDirectoryAndFilePaths(Application.StartupPath, _ReleaseNotesDevelopmentRelativeFilePath);
					break;

				default:
					_filePath = _ReleaseNotesFilePath;
					break;
			}

			// form title
			string text = ApplicationInfo.ProductName;
			text += " Release Notes";
			Text = text;

			// open and fill release notes
			textBox_ReleaseNotes.Text = "";
			if (File.Exists(_filePath))
			{
				using (StreamReader sr = new StreamReader(_filePath, Encoding.UTF8, true))
				{
					if (sr != null)
						textBox_ReleaseNotes.Text = sr.ReadToEnd();
				}
			}
			if (textBox_ReleaseNotes.Text == "")
			{
				textBox_ReleaseNotes.Text = "Couldn't read release notes from" + Environment.NewLine + _filePath;
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
// End of $URL$
//==================================================================================================
