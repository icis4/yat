﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MKY.Diagnostics;
using MKY.Windows.Forms;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Form indeed deals with exceptions.")]
	public partial class UnhandledException : System.Windows.Forms.Form
	{
		private Exception exeption;
		private bool isAsynchronous;
		private string originMessage;

		/// <summary></summary>
		public UnhandledException(Exception exeption, string originMessage, bool isAsynchronous)
		{
			InitializeComponent();

			// Set form title
			StringBuilder sb = new StringBuilder(Application.ProductName);
			sb.Append(" Unhandled");

			if (isAsynchronous)
				sb.Append(" Asynchronous");
			else
				sb.Append(" Synchronous");

			sb.Append(" Exception");
			Text = sb.ToString();

			// Set exception information.
			this.exeption = exeption;
			this.isAsynchronous = isAsynchronous;
			this.originMessage = originMessage;
		}

		private void UnhandledException_Load(object sender, EventArgs e)
		{
			textBox_Type.Text    = this.exeption.GetType().ToString();
			textBox_Message.Text = this.exeption.Message;
			textBox_Source.Text  = this.exeption.Source;
			textBox_Stack.Text   = this.exeption.StackTrace;
		}

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation always succeeds.")]
		private void button_CopyToClipboard_Click(object sender, EventArgs e)
		{
			StringWriter text = new StringWriter();
			try
			{
				text.WriteLine(this.originMessage);
				text.WriteLine();
				text.WriteLine(ApplicationInfo.ProductNameAndBuildNameAndVersion);
				text.WriteLine();

				AnyWriter.WriteException(text, null, this.exeption);
			}
			catch
			{
				text.Write("Error, unhandled exception data could not be retrieved.");
			}

			bool retry = false;
			do
			{
				try
				{
					Clipboard.SetDataObject(text.ToString(), true);
				}
				catch (ExternalException)
				{
					DialogResult userInput = MessageBox.Show
						(
						this,
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine +
						"Ensure that clipboard is not in use and try again.",
						"Clipboard Error",
						MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Error
						);
					retry = (userInput == DialogResult.Retry);
				}
				catch (Exception ex)
				{
					MessageBox.Show
						(
						this,
						"Unhandled exception data could not be copied onto clipboard." + Environment.NewLine +
						"Copy the information above manually to the bug description and add the system error message below." + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine +
						ex.Message,
						"Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					retry = false;
				}
			}
			while (retry);
		}

		private void button_Instructions_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================