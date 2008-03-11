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

		private Model.Settings.FormatSettings _settings;
		private Model.Settings.FormatSettings _settings_Form;

		private Controls.Monitor[] _monitors;
		private Controls.TextFormat[] _textFormats;
		private Model.Types.TextFormat[] _formatSettings;

		private List<Domain.DisplayElement> _examples;
		private List<Domain.DisplayElement> _exampleComplete;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public FormatSettings(Model.Settings.FormatSettings settings)
		{
			InitializeComponent();

			_settings = settings;
			_settings_Form = new Model.Settings.FormatSettings(settings);
			InitializeExamples();
			InitializeControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public Model.Settings.FormatSettings SettingsResult
		{
			get { return (_settings); }
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
			_settings = _settings_Form;
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		private void button_Defaults_Click(object sender, EventArgs e)
		{
			_settings_Form.SetDefaults();
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
			_examples.Add(new Domain.DisplayElement.Space());
			_examples.Add(new Domain.DisplayElement.Error("Message"));

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(24);

			exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[0]);
			exampleComplete.Enqueue(_examples[6]);
			exampleComplete.Enqueue(_examples[1]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(_examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[2]);
			exampleComplete.Enqueue(_examples[6]);
			exampleComplete.Enqueue(_examples[3]);
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(_examples[5]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(_examples[4]);
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(_examples[7]);

			_exampleComplete = exampleComplete.ToElements();
		}

		private void InitializeControls()
		{
			_monitors = new Controls.Monitor[]
				{
					monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
					monitor_TimeStamp, monitor_Length, monitor_WhiteSpace, monitor_Error,
				};

			_textFormats = new Controls.TextFormat[]
				{
					textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
					textFormat_TimeStamp, textFormat_Length, textFormat_WhiteSpace, textFormat_Error,
				};

			_formatSettings = new Model.Types.TextFormat[]
				{
					_settings_Form.TxDataFormat, _settings_Form.TxControlFormat, _settings_Form.RxDataFormat, _settings_Form.RxControlFormat,
					_settings_Form.TimeStampFormat, _settings_Form.LengthFormat, _settings_Form.WhiteSpacesFormat, _settings_Form.ErrorFormat,
				};

			for (int i = 0; i < _monitors.Length; i++)
				_monitors[i].AddElement(_examples [i]);

			monitor_Example.AddElements(_exampleComplete);
		}

		private void GetFormat(int index)
		{
			_formatSettings[index].Style = _textFormats[index].FormatFontStyle;
			_formatSettings[index].Color = _textFormats[index].FormatColor;
		}

		private void SetControls()
		{
			_isSettingControls = true;

			for (int i = 0; i < _monitors.Length; i++)
			{                          // clone settings before assigning them to control
				_monitors[i].FormatSettings = new Model.Settings.FormatSettings(_settings_Form);
			}

			for (int i = 0; i < _textFormats.Length; i++)
			{
				_textFormats[i].FormatFont = _settings_Form.Font;
				_textFormats[i].FormatFontStyle = _formatSettings[i].Style;
				_textFormats[i].FormatColor = _formatSettings[i].Color;
			}
			                           // clone settings before assigning them to control
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(_settings_Form);

			_isSettingControls = false;
		}

		private void ShowFontDialog()
		{
			FontDialog fd;
			Font f = _settings_Form.Font;
			DialogResult result;
			do
			{
				fd = new FontDialog();
				fd.Font = _settings_Form.Font;
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
				if ((f.Name != _settings_Form.Font.Name) || (f.Size != _settings_Form.Font.Size))
				{
					_settings_Form.Font = f;
					foreach (Controls.TextFormat tf in _textFormats)
					{
						tf.FormatFontWithoutStyle = _settings_Form.Font;
					}
					SetControls();
				}
			}
		}

		#endregion
	}
}
