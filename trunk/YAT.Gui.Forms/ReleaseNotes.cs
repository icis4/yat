using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	public partial class ReleaseNotes : Form
	{
		//public readonly string ReleaseNotesFilePath = Application.StartupPath + Path.DirectorySeparatorChar + "YAT Release Notes.txt";
		public readonly string ReleaseNotesFilePath = "C:\\Programme\\YAT\\YAT Release Notes.txt";

		public ReleaseNotes()
		{
			InitializeComponent();

			// form title
			string text = Application.ProductName;
			text += VersionInfo.ProductNamePostFix;
			text += " Release Notes";
			Text = text;

			// open and fill release notes
			textBox_ReleaseNotes.Text = "";
			if (File.Exists(ReleaseNotesFilePath))
			{
				using (StreamReader sr = new StreamReader(ReleaseNotesFilePath, Encoding.UTF8, true))
				{
					if (sr != null)
						textBox_ReleaseNotes.Text = sr.ReadToEnd();
				}
			}
			if (textBox_ReleaseNotes.Text == "")
			{
				textBox_ReleaseNotes.Text = "Couldn't read release notes from" + Environment.NewLine + ReleaseNotesFilePath;
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