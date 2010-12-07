//==================================================================================================
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
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class FormatSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private Model.Settings.FormatSettings formatSettings;
		private Model.Settings.FormatSettings formatSettings_Form;

		private Controls.Monitor[] monitors;
		private Controls.TextFormat[] textFormats;

		private List<Domain.DisplayLine> exampleLines;
		private List<Domain.DisplayLine> exampleComplete;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FormatSettings(Model.Settings.FormatSettings formatSettings)
		{
			InitializeComponent();

			this.formatSettings = formatSettings;
			this.formatSettings_Form = new Model.Settings.FormatSettings(formatSettings);

			InitializeExamples();
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Model.Settings.FormatSettings FormatSettingsResult
		{
			get { return (this.formatSettings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void FormatSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textFormat_FormatChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				GetFormatFromControl(int.Parse((string)(((Controls.TextFormat)sender).Tag)));
				SetControls();
			}
		}

		private void button_Font_Click(object sender, EventArgs e)
		{
			ShowFontDialog();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.formatSettings = this.formatSettings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show
				(
				this,
				"Reset all settings to default values?",
				"Defaults?",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button3
				)
				== DialogResult.Yes)
			{
				this.formatSettings_Form.SetDefaults();
				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeExamples()
		{
			this.exampleLines = new List<Domain.DisplayLine>();

			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxData(0x41, "41h")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxControl(0x13, "<CR>")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxData(0x42, "42h")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxControl(0x10, "<LF>")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeStamp(DateTime.Now)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.LineLength(2)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.Error("Message")));

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(24);

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(0x41, "41h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(0x13, "<CR>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.LineLength(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(0x42, "42h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxControl(0x10, "<LF>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.LineLength(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.Error("Message"));

			/*exampleComplete.Enqueue(this.examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(this.examples[0]);
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(this.examples[1]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(this.examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(this.examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(this.examples[2]);
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(this.examples[3]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(this.examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(this.examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(this.examples[6]);*/

			this.exampleComplete = exampleComplete.ToLines();
		}

		private void InitializeControls()
		{
			this.monitors = new Controls.Monitor[]
				{
					monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
					monitor_TimeStamp, monitor_Length, monitor_Error,
				};

			this.textFormats = new Controls.TextFormat[]
				{
					textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
					textFormat_TimeStamp, textFormat_Length, textFormat_Error,
				};

			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].AddLine(this.exampleLines[i]);

			monitor_Example.AddLines(this.exampleComplete);
		}

		private Model.Types.TextFormat GetFormatFromIndex(int index)
		{
			switch (index)
			{
				case 0: return (this.formatSettings_Form.TxDataFormat);
				case 1: return (this.formatSettings_Form.TxControlFormat);
				case 2: return (this.formatSettings_Form.RxDataFormat);
				case 3: return (this.formatSettings_Form.RxControlFormat);
				case 4: return (this.formatSettings_Form.TimeStampFormat);
				case 5: return (this.formatSettings_Form.LengthFormat);
				case 6: return (this.formatSettings_Form.ErrorFormat);
				default: throw (new ArgumentOutOfRangeException("index", index, "There is no format at this index."));
			}
		}

		private void GetFormatFromControl(int index)
		{
			Model.Types.TextFormat tf = GetFormatFromIndex(index);
			tf.FontStyle = this.textFormats[index].FormatFontStyle;
			tf.Color     = this.textFormats[index].FormatColor;
		}

		private void SetControls()
		{
			this.isSettingControls = true;

			for (int i = 0; i < this.monitors.Length; i++)
			{                          // Clone settings before assigning them to control
				this.monitors[i].FormatSettings = new Model.Settings.FormatSettings(this.formatSettings_Form);
			}

			for (int i = 0; i < this.textFormats.Length; i++)
			{
				this.textFormats[i].FormatFontWithoutStyle = this.formatSettings_Form.Font;

				Model.Types.TextFormat tf = GetFormatFromIndex(i);
				this.textFormats[i].FormatFontStyle = tf.FontStyle;
				this.textFormats[i].FormatColor     = tf.Color;
			}

			                           // Clone settings before assigning them to control
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(this.formatSettings_Form);

			this.isSettingControls = false;
		}

		private void ShowFontDialog()
		{
			FontDialog fd;
			Font f = this.formatSettings_Form.Font;
			bool fontOK = false;
			bool cancel = false;
			do
			{
				fd = new FontDialog();
				fd.Font = this.formatSettings_Form.Font;
				fd.ShowEffects = false;
				if (fd.ShowDialog(this) != DialogResult.OK)
				{
					cancel = true;
				}
				else
				{
					Refresh();

					// Check chosen font.
					try
					{
						f = new Font(fd.Font.Name, fd.Font.Size, FontStyle.Regular);
						fontOK = true;
					}
					catch (ArgumentException)
					{
						DialogResult result = MessageBox.Show
							(
								this,
								"Font '" + fd.Font.Name + "' does not support regular style. Choose a different font.",
								"Font Not Supported",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Exclamation
							);

						cancel = (result != DialogResult.OK);
					}
				}
			}
			while (!fontOK && !cancel);

			if (fontOK)
			{
				if ((f.Name != this.formatSettings_Form.Font.Name) || (f.Size != this.formatSettings_Form.Font.Size))
				{
					this.formatSettings_Form.Font = f;
					SetControls();
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
