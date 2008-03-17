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

		private Domain.Settings.CharReplaceSettings _charReplaceSettings;
		private Domain.Settings.CharReplaceSettings _charReplaceSettings_Form;

		private Controls.Monitor[] _monitors;
		private Controls.TextFormat[] _textFormats;
		private Model.Types.TextFormat[] _textFormatSettings;

		private List<Domain.DisplayElement> _examples;
		private List<Domain.DisplayElement> _exampleComplete;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public FormatSettings(Model.Settings.FormatSettings formatSettings, Domain.Settings.CharReplaceSettings charReplaceSettings)
		{
			InitializeComponent();

			_formatSettings = formatSettings;
			_formatSettings_Form = new Model.Settings.FormatSettings(formatSettings);

			_charReplaceSettings = charReplaceSettings;
			_charReplaceSettings_Form = new Domain.Settings.CharReplaceSettings(charReplaceSettings);

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

		public Domain.Settings.CharReplaceSettings CharReplaceSettingsResult
		{
			get { return (_charReplaceSettings); }
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
				GetFormat(int.Parse((string)(((Controls.TextFormat)sender).Tag)));
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
			_charReplaceSettings = _charReplaceSettings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			_formatSettings_Form.SetDefaults();
			_charReplaceSettings_Form.SetDefaults();

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

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(24);

			exampleComplete.Enqueue(_examples[4]);
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
			exampleComplete.Enqueue(_examples[6]);

			_exampleComplete = exampleComplete.ToElements();
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

			_textFormatSettings = new Model.Types.TextFormat[]
				{
					_formatSettings_Form.TxDataFormat, _formatSettings_Form.TxControlFormat, _formatSettings_Form.RxDataFormat, _formatSettings_Form.RxControlFormat,
					_formatSettings_Form.TimeStampFormat, _formatSettings_Form.LengthFormat, _formatSettings_Form.ErrorFormat,
				};

			for (int i = 0; i < _monitors.Length; i++)
				_monitors[i].AddElement(_examples [i]);

			monitor_Example.AddElements(_exampleComplete);
		}

		private void GetFormat(int index)
		{
			_textFormatSettings[index].Style = _textFormats[index].FormatFontStyle;
			_textFormatSettings[index].Color = _textFormats[index].FormatColor;
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
				_textFormats[i].FormatFont = _formatSettings_Form.Font;
				_textFormats[i].FormatFontStyle = _textFormatSettings[i].Style;
				_textFormats[i].FormatColor = _textFormatSettings[i].Color;
			}
			                           // clone settings before assigning them to control
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(_formatSettings_Form);

			_isSettingControls = false;
		}

		private void ShowFontDialog()
		{
			FontDialog fd;
			Font f = _formatSettings_Form.Font;
			DialogResult result;
			do
			{
				fd = new FontDialog();
				fd.Font = _formatSettings_Form.Font;
				fd.ShowEffects = false;
				if ((result = fd.ShowDialog(this)) != DialogResult.OK)
					continue;

				Refresh();

				try
				{
					f = new Font(fd.Font.Name, fd.Font.Size, FontStyle.Regular);
				}
				catch (Exception)
				{
					result = MessageBox.Show(this,
					  "Font \"" + fd.Font.Name + "\" does not support regular style. Choose a different font.",
					  "Bad font", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
					continue;
				}

				try
				{
					RichTextBox rtb = new RichTextBox();
					rtb.SelectionFont = f;
					rtb.AppendText("Test");
					break;
				}
				catch (Exception)
				{
					result = MessageBox.Show(this,
					  "Font \"" + fd.Font.Name + "\" can not be applied to rich text boxes. Choose a different font.",
					  "Bad font", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
					continue;
				}
			}
			while (result == DialogResult.OK);

			if (result == DialogResult.OK)
			{
				if ((f.Name != _formatSettings_Form.Font.Name) || (f.Size != _formatSettings_Form.Font.Size))
				{
					_formatSettings_Form.Font = f;
					foreach (Controls.TextFormat tf in _textFormats)
					{
						tf.FormatFontWithoutStyle = _formatSettings_Form.Font;
					}
					SetControls();
				}
			}
		}

		#endregion
	}
}
