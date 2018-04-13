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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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
using System.IO;
using System.Text;
using System.Windows.Forms;

using MKY.IO;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class ReleaseNotes : Form
	{
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private static readonly string ReleaseNotesFileName = ApplicationEx.ProductName + " Release Notes.txt";

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private static readonly string ReleaseNotesFilePath = System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + ReleaseNotesFileName;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private static readonly string ReleaseNotesDevelopmentRelativeFilePath =
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
			switch (Path.GetFileName(System.Windows.Forms.Application.StartupPath))
			{
				case "Debug":
				case "Release":
					filePath = PathEx.CombineDirectoryAndFilePaths(System.Windows.Forms.Application.StartupPath, ReleaseNotesDevelopmentRelativeFilePath);
					break;

				default:
					filePath = ReleaseNotesFilePath;
					break;
			}

			// Set form title:
			Text = ApplicationEx.ProductName + " Release Notes";

			// Open and fill release notes:
			textBox_ReleaseNotes.Text = "";
			if (File.Exists(filePath))
			{
				using (var sr = new StreamReader(filePath, Encoding.UTF8, true))
				{
					if (sr != null)
						textBox_ReleaseNotes.Text = sr.ReadToEnd();
				}
			}

			if (string.IsNullOrEmpty(textBox_ReleaseNotes.Text))
			{
				textBox_ReleaseNotes.Text = "Couldn't read release notes from" + Environment.NewLine + filePath;
			}
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
