//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace YAT.Gui.Forms
{
	public partial class FormatSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		private Model.Settings.FormatSettings _formatSettings;
		private Model.Settings.FormatSettings _formatSettings_Form;

		private Controls.Monitor[] _monitors;
		private Controls.TextFormat[] _textFormats;

		private List<Domain.DisplayElement> _examples;
		private List<List<Domain.DisplayElement>> _exampleComplete;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public FormatSettings(Model.Settings.FormatSettings formatSettings)
		{
			InitializeComponent();

			_formatSettings = formatSettings;
			_formatSettings_Form = new Model.Settings.FormatSettings(formatSettings);

			InitializeExamples();
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Model.Settings.FormatSettings FormatSettingsResult
		{
			get { return (_formatSettings); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void FormatSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// initially set controls and validate its contents where needed
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
			if (!_isSettingControls)
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
			_formatSettings = _formatSettings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			_formatSettings_Form.SetDefaults();
			SetControls();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeExamples()
		{
			_examples = new List<Domain.DisplayElement>();

			_examples.Add(new Domain.DisplayElement.TxData(null, "12h"));
			_examples.Add(new Domain.DisplayElement.TxControl(null, "<CR>"));
			_examples.Add(new Domain.DisplayElement.RxData(null, "34h"));
			_examples.Add(new Domain.DisplayElement.RxControl(null, "<LF>"));
			_examples.Add(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			_examples.Add(new Domain.DisplayElement.LineLength(2));
			_examples.Add(new Domain.DisplayElement.Error("Message"));

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(24, 32);

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(null, "12h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(null, "<CR>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.LineLength(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(null, "34h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxControl(null, "<LF>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.LineLength(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStamp(DateTime.Now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.Error("Message"));

			/*exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[0]);
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(_examples[1]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(_examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[2]);
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(_examples[3]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(_examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[6]);*/

			_exampleComplete = exampleComplete.ToLines();
		}

		private void InitializeControls()
		{
			_monitors = new Controls.Monitor[]
				{
					monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
					monitor_TimeStamp, monitor_Length, monitor_Error,
				};

			_textFormats = new Controls.TextFormat[]
				{
					textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
					textFormat_TimeStamp, textFormat_Length, textFormat_Error,
				};

			for (int i = 0; i < _monitors.Length; i++)
				_monitors[i].AddElement(_examples[i]);

			monitor_Example.AddLines(_exampleComplete);
		}

		private Model.Types.TextFormat GetFormatFromIndex(int index)
		{
			switch (index)
			{
				case 0: return (_formatSettings_Form.TxDataFormat);
				case 1: return (_formatSettings_Form.TxControlFormat);
				case 2: return (_formatSettings_Form.RxDataFormat);
				case 3: return (_formatSettings_Form.RxControlFormat);
				case 4: return (_formatSettings_Form.TimeStampFormat);
				case 5: return (_formatSettings_Form.LengthFormat);
				case 6: return (_formatSettings_Form.ErrorFormat);
				default: throw (new ArgumentOutOfRangeException("index", index, "There is no format at this index."));
			}
		}

		private void GetFormatFromControl(int index)
		{
			Model.Types.TextFormat tf = GetFormatFromIndex(index);
			tf.FontStyle = _textFormats[index].FormatFontStyle;
			tf.Color     = _textFormats[index].FormatColor;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			for (int i = 0; i < _monitors.Length; i++)
			{                          // clone settings before assigning them to control
				_monitors[i].FormatSettings = new Model.Settings.FormatSettings(_formatSettings_Form);
			}

			for (int i = 0; i < _textFormats.Length; i++)
			{
				_textFormats[i].FormatFontWithoutStyle = _formatSettings_Form.Font;

				Model.Types.TextFormat tf = GetFormatFromIndex(i);
				_textFormats[i].FormatFontStyle = tf.FontStyle;
				_textFormats[i].FormatColor     = tf.Color;
			}
			                           // clone settings before assigning them to control
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(_formatSettings_Form);

			_isSettingControls = false;
		}

		private void ShowFontDialog()
		{
			FontDialog fd;
			Font f = _formatSettings_Form.Font;
			bool fontOK = false;
			bool cancel = false;
			do
			{
				fd = new FontDialog();
				fd.Font = _formatSettings_Form.Font;
				fd.ShowEffects = false;
				if (fd.ShowDialog(this) != DialogResult.OK)
				{
					cancel = true;
				}
				else
				{
					// check chosen font

					Refresh();

					try
					{
						f = new Font(fd.Font.Name, fd.Font.Size, FontStyle.Regular);
						fontOK = true;
					}
					catch (Exception)
					{
						cancel =
							MessageBox.Show
							(
								this,
								@"Font """ + fd.Font.Name + @""" does not support regular style. Choose a different font.",
								@"Font not supported",
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Exclamation
							)
							!= DialogResult.OK;
					}
				}
			}
			while (!fontOK && !cancel);

			if (fontOK)
			{
				if ((f.Name != _formatSettings_Form.Font.Name) || (f.Size != _formatSettings_Form.Font.Size))
				{
					_formatSettings_Form.Font = f;
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
