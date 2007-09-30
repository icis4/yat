using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using YAT.Domain.Settings;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	public partial class BinaryDisplaySettingsSet : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool _isSettingControls = false;
		private BinaryDisplaySettings _settings = new BinaryDisplaySettings();

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when any of the settings properties is changed.")]
		public event EventHandler SettingsChanged;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public BinaryDisplaySettingsSet()
		{
			InitializeComponent();
			SetControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public BinaryDisplaySettings Settings
		{
			get { return (_settings); }
			set
			{
				if (value != null)
					_settings = value;
				else
					_settings = new BinaryDisplaySettings();

				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void checkBox_LengthLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.BinaryLengthLineBreak llb = _settings.LengthLineBreak;
				llb.Enabled = checkBox_LengthLineBreak.Checked;
				_settings.LengthLineBreak = llb;
				SetControls();
				OnSettingsChanged(new EventArgs());
			}
		}

		private void textBox_LengthLineBreak_TextChanged(object sender, EventArgs e)
		{
			int length;
			if (int.TryParse(textBox_LengthLineBreak.Text, out length) && (Math.Abs(length) == 1))
				label_LengthLineBreakUnit.Text = "byte";
			else
				label_LengthLineBreakUnit.Text = "bytes";
		}

		private void textBox_LengthLineBreak_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int length;
				if (int.TryParse(textBox_LengthLineBreak.Text, out length) && (length >= 1))
				{
					Domain.BinaryLengthLineBreak llb = _settings.LengthLineBreak;
					llb.LineLength = length;
					_settings.LengthLineBreak = llb;
					SetControls();
					OnSettingsChanged(new EventArgs());
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Length must be at least 1 byte!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		private void checkBox_SequenceLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.BinarySequenceLineBreak slb = _settings.SequenceLineBreak;
				slb.Enabled = checkBox_SequenceLineBreak.Checked;
				_settings.SequenceLineBreak = slb;
				SetControls();
				OnSettingsChanged(new EventArgs());
			}
		}

		private void textBox_SequenceLineBreakSequence_Validating(object sender, CancelEventArgs e)
		{
			if (Validation.ValidateSequence(this, "Sequence", textBox_SequenceLineBreakSequence.Text))
			{
				Domain.BinarySequenceLineBreak slb = _settings.SequenceLineBreak;
				slb.Sequence = textBox_SequenceLineBreakSequence.Text;
				_settings.SequenceLineBreak = slb;
				SetControls();
				OnSettingsChanged(new EventArgs());
			}
			else
			{
				e.Cancel = true;
			}
		}

		private void checkBox_TimedLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				Domain.BinaryTimedLineBreak tlb = _settings.TimedLineBreak;
				tlb.Enabled = checkBox_TimedLineBreak.Checked;
				_settings.TimedLineBreak = tlb;
				SetControls();
				OnSettingsChanged(new EventArgs());
			}
		}

		private void textBox_TimedLineBreakTimeout_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int timeout;
				if (int.TryParse(textBox_TimedLineBreakTimeout.Text, out timeout) && (timeout >= 0))
				{
					Domain.BinaryTimedLineBreak tlb = _settings.TimedLineBreak;
					tlb.Timeout = timeout;
					_settings.TimedLineBreak = tlb;
					SetControls();
					OnSettingsChanged(new EventArgs());
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Line break timeout must be a positive number!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			bool enabled;

			_isSettingControls = true;

			enabled = _settings.LengthLineBreak.Enabled;
			checkBox_LengthLineBreak.Checked = enabled;
			textBox_LengthLineBreak.Enabled = enabled;
			textBox_LengthLineBreak.Text = _settings.LengthLineBreak.LineLength.ToString();

			enabled = _settings.SequenceLineBreak.Enabled;
			checkBox_SequenceLineBreak.Checked = enabled;
			textBox_SequenceLineBreakSequence.Enabled = enabled;
			textBox_SequenceLineBreakSequence.Text = _settings.SequenceLineBreak.Sequence;

			enabled = _settings.TimedLineBreak.Enabled;
			checkBox_TimedLineBreak.Checked = enabled;
			textBox_TimedLineBreakTimeout.Enabled = enabled;
			textBox_TimedLineBreakTimeout.Text = _settings.TimedLineBreak.Timeout.ToString();

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnSettingsChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(SettingsChanged, this, e);
		}

		#endregion
	}
}
