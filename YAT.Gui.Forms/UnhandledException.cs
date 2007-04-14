using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HSR.YAT.Gui.Forms
{
	public partial class UnhandledException : System.Windows.Forms.Form
	{
		Exception _exeption;

		public UnhandledException(Exception exeption)
		{
			InitializeComponent();
			_exeption = exeption;
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Type.Text = _exeption.GetType().ToString();
			textBox_Message.Text = _exeption.Message;
			textBox_Source.Text = _exeption.Source;
			textBox_Stack.Text = _exeption.StackTrace;
		}

		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			StringWriter text = new StringWriter();
			text.WriteLine("Unhandled exception occured in YAT");
			text.WriteLine();
			text.WriteLine("Type:");
			text.WriteLine(_exeption.GetType().ToString());
			text.WriteLine();
			text.WriteLine("Message:");
			text.WriteLine(_exeption.Message);
			text.WriteLine();
			text.WriteLine("Source:");
			text.WriteLine(_exeption.Source);
			text.WriteLine();
			text.WriteLine("Stack:");
			text.WriteLine(_exeption.StackTrace);
			Clipboard.SetDataObject(text.ToString(), true);
		}
	}
}
