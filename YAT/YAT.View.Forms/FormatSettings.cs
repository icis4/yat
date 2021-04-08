//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Scope = "member", Target = "YAT.View.Forms.FormatSettings.#InitializeComponent()", MessageId = "System.TimeSpan.Parse(System.String)", Justification = "Designer generated!")]

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class FormatSettings : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Format.Settings.FormatSettings formatSettings;
		private Format.Settings.FormatSettings formatSettingsInEdit;

		private int[] customColors;

		private DateTime initialTimeStamp;

		private bool   timeStampUseUtc;
		private string timeStampFormat;
		private string timeSpanFormat;
		private string timeDeltaFormat;
		private string timeDurationFormat;

		private Domain.SeparatorEx contentSeparator;
		private Domain.SeparatorEx infoSeparator;
		private Domain.EnclosureEx infoEnclosure;

		private Controls.Monitor[] monitors;
		private Controls.TextFormat[] textFormats;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		public FormatSettings(Format.Settings.FormatSettings formatSettings, int[] customColors,
		                      Domain.SeparatorEx contentSeparator, Domain.SeparatorEx infoSeparator, Domain.EnclosureEx infoEnclosure,
		                      bool timeStampUseUtc, string timeStampFormat, string timeSpanFormat, string timeDeltaFormat, string timeDurationFormat)
		{
			InitializeComponent();

			this.formatSettings = formatSettings;
			this.formatSettingsInEdit = new Format.Settings.FormatSettings(formatSettings);

			this.customColors = customColors;

			this.initialTimeStamp = DateTime.Now;

			this.contentSeparator = contentSeparator;
			this.infoSeparator    = infoSeparator;
			this.infoEnclosure    = infoEnclosure;

			this.timeStampUseUtc    = timeStampUseUtc;
			this.timeStampFormat    = timeStampFormat;
			this.timeSpanFormat     = timeSpanFormat;
			this.timeDeltaFormat    = timeDeltaFormat;
			this.timeDurationFormat = timeDurationFormat;

			InitializeControls();
		////SetControls() is initially called in the 'Shown' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Same type as ColorDialog.CustomColors.")]
		public int[] CustomColors
		{
			get { return (this.customColors); }
			set { this.customColors = value;  }
		}

		/// <summary></summary>
		public Format.Settings.FormatSettings FormatSettingsResult
		{
			get { return (this.formatSettings); }
		}

		/// <summary></summary>
		public Domain.SeparatorEx ContentSeparatorResult
		{
			get { return (this.contentSeparator); }
		}

		/// <summary></summary>
		public Domain.SeparatorEx InfoSeparatorResult
		{
			get { return (this.infoSeparator); }
		}

		/// <summary></summary>
		public Domain.EnclosureEx InfoEnclosureResult
		{
			get { return (this.infoEnclosure); }
		}

		/// <summary></summary>
		public bool TimeStampUseUtcResult
		{
			get { return (this.timeStampUseUtc); }
		}

		/// <summary></summary>
		public string TimeStampFormatResult
		{
			get { return (this.timeStampFormat); }
		}

		/// <summary></summary>
		public string TimeSpanFormatResult
		{
			get { return (this.timeSpanFormat); }
		}

		/// <summary></summary>
		public string TimeDeltaFormatResult
		{
			get { return (this.timeDeltaFormat); }
		}

		/// <summary></summary>
		public string TimeDurationFormatResult
		{
			get { return (this.timeDurationFormat); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void FormatSettings_Shown(object sender, EventArgs e)
		{
			SetControls();
		}

		private void FormatSettings_Deactivate(object sender, EventArgs e)
		{
			comboBox_ContentSeparator.OnFormDeactivateWorkaround();
			comboBox_InfoSeparator   .OnFormDeactivateWorkaround();
			comboBox_InfoEnclosure   .OnFormDeactivateWorkaround();
		////comboBox_TimeStampFormatPreset    is a standard ComboBox.
		////comboBox_TimeSpanFormatPreset     is a standard ComboBox.
		////comboBox_TimeDeltaFormatPreset    is a standard ComboBox.
		////comboBox_TimeDurationFormatPreset is a standard ComboBox.
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_EnableFormatting_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			formatSettingsInEdit.FormattingEnabled = checkBox_EnableFormatting.Checked;
			SetControls();
		}

		private void textFormat_FormatChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			GetFormatFromControl(ControlEx.TagToInt32(sender));
			SetControls();
		}

		private void textFormat_CustomColorsChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			GetCustomColorsFromControl(ControlEx.TagToInt32(sender));
			SetControls();
		}

		private void button_Font_Click(object sender, EventArgs e)
		{
			ShowFontDialog();
		}

		private void button_Background_Click(object sender, EventArgs e)
		{
			ShowBackgroundColorDialog();
		}

	////private void comboBox_ContentSeparator_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since        "        _Validating() below gets called anyway.

		private void comboBox_ContentSeparator_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ValidateAndUpdateContentSeparator(comboBox_ContentSeparator.Text);
		}

		private void comboBox_ContentSeparator_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!ValidateAndUpdateContentSeparator(comboBox_ContentSeparator.Text))
				e.Cancel = true;
		}

		private bool ValidateAndUpdateContentSeparator(string separatorText)
		{
			Domain.SeparatorEx separator;
			if (Domain.SeparatorEx.TryParse(separatorText, out separator))
			{
				this.contentSeparator = separator;
				SetControls();
				return (true);
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"This separator is not supported!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

	////private void comboBox_InfoSeparator_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since       "      _Validating() below gets called anyway.

		private void comboBox_InfoSeparator_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ValidateAndUpdateInfoSeparator(comboBox_InfoSeparator.Text);
		}

		private void comboBox_InfoSeparator_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!ValidateAndUpdateInfoSeparator(comboBox_InfoSeparator.Text))
				e.Cancel = true;
		}

		private bool ValidateAndUpdateInfoSeparator(string separatorText)
		{
			Domain.SeparatorEx separator;
			if (Domain.SeparatorEx.TryParse(separatorText, out separator))
			{
				this.infoSeparator = separator;
				SetControls();
				return (true);
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"This separator is not supported!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

	////private void comboBox_InfoEnclosure_SelectedIndexChanged(object sender, EventArgs e)
	////is not required since       "      _Validating() below gets called anyway.

		private void comboBox_InfoEnclosure_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ValidateAndUpdateInfoEnclosure(comboBox_InfoEnclosure.Text);
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_InfoEnclosure_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (!ValidateAndUpdateInfoEnclosure(comboBox_InfoEnclosure.Text))
				e.Cancel = true;
		}

		private bool ValidateAndUpdateInfoEnclosure(string enclosureText)
		{
			Domain.EnclosureEx enclosure;
			if (Domain.EnclosureEx.TryParse(enclosureText, out enclosure))
			{
				this.infoEnclosure = enclosure;
				SetControls();
				return (true);
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Enclosure string must be an even number of characters (e.g. 2 or 4 characters).",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning // Just a warning since _TextChanged() on editing likely ends here.
				);

				return (false);
			}
		}

		private void checkBox_TimeStampUseUtc_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.timeStampUseUtc = checkBox_TimeStampUseUtc.Checked;
			SetControls();
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeStampFormatPresetEx.TryParse(System.String,YAT.Domain.TimeStampFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		private void textBox_TimeStampFormat_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Domain.TimeStampFormatPresetEx preset;
			Domain.TimeStampFormatPresetEx.TryParse(textBox_TimeStampFormat.Text, out preset);

			this.isSettingControls.Enter();
			try
			{
				ComboBoxHelper.Select(comboBox_TimeStampFormatPreset, preset);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void textBox_TimeStampFormat_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			var format = textBox_TimeStampFormat.Text;
			if (!ValidateAndUpdateTimeStampFormat(format))
				e.Cancel = true;

			this.timeStampFormat = format;
			SetControls();
		}

		private bool ValidateAndUpdateTimeStampFormat(string format)
		{
			try
			{
				var item = DateTime.Now;
				item.ToString(format, CultureInfo.CurrentCulture);

				return (true);
			}
			catch (FormatException ex)
			{
				string message = "Invalid date/time format!" + Environment.NewLine + Environment.NewLine +
				                 "Framework error message:" + Environment.NewLine + ex.Message;

				MessageBoxEx.Show
				(
					this,
					message,
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		private void comboBox_TimeStampFormatPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var preset = (comboBox_TimeStampFormatPreset.SelectedItem as Domain.TimeStampFormatPresetEx);
			if (preset != null)
			{
				if (preset != Domain.TimeStampFormatPreset.None)
				{
					this.timeStampFormat = preset.ToFormat();
					SetControls();
				}
				else
				{
					SetControls();
				}
			}
			else
			{
				SetControls();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeSpanFormatPresetEx.TryParse(System.String,YAT.Domain.TimeSpanFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		private void textBox_TimeSpanFormat_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Domain.TimeSpanFormatPresetEx preset;
			Domain.TimeSpanFormatPresetEx.TryParse(textBox_TimeSpanFormat.Text, out preset);

			this.isSettingControls.Enter();
			try
			{
				ComboBoxHelper.Select(comboBox_TimeSpanFormatPreset, preset);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void textBox_TimeSpanFormat_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			var format = textBox_TimeSpanFormat.Text;
			if (!ValidateAndUpdateTimeSpanFormat(format))
				e.Cancel = true;

			this.timeSpanFormat = format;
			SetControls();
		}

		private bool ValidateAndUpdateTimeSpanFormat(string format)
		{
			try
			{
				var item = TimeSpan.Zero;
				item.ToString(format);

				return (true);
			}
			catch (FormatException ex)
			{
				string message = "Invalid time span format!" + Environment.NewLine + Environment.NewLine +
				                 "Framework error message:" + Environment.NewLine + ex.Message;

				MessageBoxEx.Show
				(
					this,
					message,
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		private void comboBox_TimeSpanFormatPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var preset = (comboBox_TimeSpanFormatPreset.SelectedItem as Domain.TimeSpanFormatPresetEx);
			if (preset != null)
			{
				if (preset != Domain.TimeSpanFormatPreset.None)
				{
					this.timeSpanFormat = preset.ToFormat();
					SetControls();
				}
				else
				{
					SetControls();
				}
			}
			else
			{
				SetControls();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeDeltaFormatPresetEx.TryParse(System.String,YAT.Domain.TimeDeltaFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		private void textBox_TimeDeltaFormat_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Domain.TimeDeltaFormatPresetEx preset;
			Domain.TimeDeltaFormatPresetEx.TryParse(textBox_TimeDeltaFormat.Text, out preset);

			this.isSettingControls.Enter();
			try
			{
				ComboBoxHelper.Select(comboBox_TimeDeltaFormatPreset, preset);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void textBox_TimeDeltaFormat_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			var format = textBox_TimeDeltaFormat.Text;
			if (!ValidateAndUpdateTimeSpanFormat(format))
				e.Cancel = true;

			this.timeDeltaFormat = format;
			SetControls();
		}

		private void comboBox_TimeDeltaFormatPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var preset = (comboBox_TimeDeltaFormatPreset.SelectedItem as Domain.TimeDeltaFormatPresetEx);
			if (preset != null)
			{
				if (preset != Domain.TimeDeltaFormatPreset.None)
				{
					this.timeDeltaFormat = preset.ToFormat();
					SetControls();
				}
				else
				{
					SetControls();
				}
			}
			else
			{
				SetControls();
			}
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeDeltaFormatPresetEx.TryParse(System.String,YAT.Domain.TimeDeltaFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		private void textBox_TimeDurationFormat_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Domain.TimeDeltaFormatPresetEx preset; // Using 'TimeDeltaFormatPreset' too!
			Domain.TimeDeltaFormatPresetEx.TryParse(textBox_TimeDurationFormat.Text, out preset);

			this.isSettingControls.Enter();
			try
			{
				ComboBoxHelper.Select(comboBox_TimeDurationFormatPreset, preset);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void textBox_TimeDurationFormat_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			var format = textBox_TimeDurationFormat.Text;
			if (!ValidateAndUpdateTimeSpanFormat(format))
				e.Cancel = true;

			this.timeDurationFormat = format;
			SetControls();
		}

		private void comboBox_TimeDurationFormatPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var preset = (comboBox_TimeDurationFormatPreset.SelectedItem as Domain.TimeDeltaFormatPresetEx); // Using 'TimeDeltaFormatPreset' too!
			if (preset != null)
			{
				if (preset != Domain.TimeDeltaFormatPreset.None) // Using 'TimeDeltaFormatPreset' too!
				{
					this.timeDurationFormat = preset.ToFormat();
					SetControls();
				}
				else
				{
					SetControls();
				}
			}
			else
			{
				SetControls();
			}
		}

		private void linkLabel_DateTimeFormat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
		}

		private void linkLabel_TimeSpanFormat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.formatSettings = this.formatSettingsInEdit;

			// Options (separators/enclosures/time) are handled separately.
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Defaults_Click(object sender, EventArgs e)
		{
			if (MessageBoxEx.Show
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
				this.formatSettingsInEdit.SetDefaults();

				this.contentSeparator = Domain.Settings.DisplaySettings.ContentSeparatorDefault;
				this.infoSeparator    = Domain.Settings.DisplaySettings.InfoSeparatorDefault;
				this.infoEnclosure    = Domain.Settings.DisplaySettings.InfoEnclosureDefault;

				this.timeStampUseUtc    = Domain.Settings.DisplaySettings.TimeStampUseUtcDefault;
				this.timeStampFormat    = Domain.Settings.DisplaySettings.TimeStampFormatDefault;
				this.timeSpanFormat     = Domain.Settings.DisplaySettings.TimeSpanFormatDefault;
				this.timeDeltaFormat    = Domain.Settings.DisplaySettings.TimeDeltaFormatDefault;
				this.timeDurationFormat = Domain.Settings.DisplaySettings.TimeDurationFormatDefault;

				SetControls();
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.isSettingControls.Enter();
			try
			{
				this.monitors = new Controls.Monitor[]
				{
					monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
					monitor_TimeStamp, monitor_TimeSpan, monitor_TimeDelta, monitor_TimeDuration,
					monitor_Device, monitor_Direction, monitor_Length,
					monitor_IOControl,
					monitor_Warning, monitor_Error,
					monitor_WhiteSpace // See "YAT.Format.Settings.WhiteSpaceFormat" for explanation on "WhiteSpace" vs. "Separators".
				};

				this.textFormats = new Controls.TextFormat[]
				{
					textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
					textFormat_TimeStamp, textFormat_TimeSpan, textFormat_TimeDelta, textFormat_TimeDuration,
					textFormat_Device, textFormat_Direction, textFormat_Length,
					textFormat_IOControl,
					textFormat_Warning, textFormat_Error,
					textFormat_WhiteSpace // See "YAT.Format.Settings.WhiteSpaceFormat" for explanation on "WhiteSpace" vs. "Separators".
				};

				comboBox_ContentSeparator.Items.Clear();
				comboBox_ContentSeparator.Items.AddRange(Domain.SeparatorEx.GetItems());

 				comboBox_InfoSeparator.Items.Clear();
				comboBox_InfoSeparator.Items.AddRange(Domain.SeparatorEx.GetItems());

				comboBox_InfoEnclosure.Items.Clear();
				comboBox_InfoEnclosure.Items.AddRange(Domain.EnclosureEx.GetItems());

				comboBox_TimeStampFormatPreset.Items.Clear();
				comboBox_TimeStampFormatPreset.Items.AddRange(Domain.TimeStampFormatPresetEx.GetItems());

				comboBox_TimeSpanFormatPreset.Items.Clear();
				comboBox_TimeSpanFormatPreset.Items.AddRange(Domain.TimeSpanFormatPresetEx.GetItems());

				comboBox_TimeDeltaFormatPreset.Items.Clear();
				comboBox_TimeDeltaFormatPreset.Items.AddRange(Domain.TimeDeltaFormatPresetEx.GetItems());

				comboBox_TimeDurationFormatPreset.Items.Clear();
				comboBox_TimeDurationFormatPreset.Items.AddRange(Domain.TimeDeltaFormatPresetEx.GetItems()); // Using 'TimeDeltaFormatPreset' too!

				var linkText = ".NET" + Environment.NewLine + "Date/Time Format";
				var linkUri = @"https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings";
				linkLabel_DateTimeFormat.Links.Clear();
				linkLabel_DateTimeFormat.Text = linkText;
				linkLabel_DateTimeFormat.Links.Add(0, linkText.Length, linkUri);

				linkText = ".NET" + Environment.NewLine + "Time Span Format";
				linkUri = @"https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings";
				linkLabel_TimeSpanFormat.Links.Clear();
				linkLabel_TimeSpanFormat.Text = linkText;
				linkLabel_TimeSpanFormat.Links.Add(0, linkText.Length, linkUri);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private Format.Types.TextFormat GetFormatFromIndex(int index)
		{
			switch (index)
			{
				case  0: return (this.formatSettingsInEdit.TxDataFormat);
				case  1: return (this.formatSettingsInEdit.TxControlFormat);
				case  2: return (this.formatSettingsInEdit.RxDataFormat);
				case  3: return (this.formatSettingsInEdit.RxControlFormat);
				case  4: return (this.formatSettingsInEdit.TimeStampFormat);
				case  5: return (this.formatSettingsInEdit.TimeSpanFormat);
				case  6: return (this.formatSettingsInEdit.TimeDeltaFormat);
				case  7: return (this.formatSettingsInEdit.TimeDurationFormat);
				case  8: return (this.formatSettingsInEdit.DeviceFormat);
				case  9: return (this.formatSettingsInEdit.DirectionFormat);
				case 10: return (this.formatSettingsInEdit.LengthFormat);
				case 11: return (this.formatSettingsInEdit.IOControlFormat);
				case 12: return (this.formatSettingsInEdit.WarningFormat);
				case 13: return (this.formatSettingsInEdit.ErrorFormat);
				case 14: return (this.formatSettingsInEdit.WhiteSpaceFormat); // See "WhiteSpaceFormat" for explanation on "WhiteSpace" vs. "Separators".

				default:  throw (new ArgumentOutOfRangeException("index", index, MessageHelper.InvalidExecutionPreamble + "There is no format at index '" + index + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void GetFormatFromControl(int index)
		{
			var tf = GetFormatFromIndex(index);
			tf.FontStyle = this.textFormats[index].FormatFontStyle;
			tf.Color     = this.textFormats[index].FormatColor;
		}

		private void GetCustomColorsFromControl(int index)
		{
			this.customColors = this.textFormats[index].CustomColors;
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeStampFormatPresetEx.TryParse(System.String,YAT.Domain.TimeStampFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeSpanFormatPresetEx.TryParse(System.String,YAT.Domain.TimeSpanFormatPresetEx@)",   Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "YAT.Domain.TimeDeltaFormatPresetEx.TryParse(System.String,YAT.Domain.TimeDeltaFormatPresetEx@)", Justification = "Result doesn't matter, it's OK to pass 'null' to 'ComboBoxHelper'.")]
		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				ClearExamples();

				for (int i = 0; i < this.monitors.Length; i++)
				{                                      // Clone settings to ensure decoupling:
					this.monitors[i].FormatSettings = new Format.Settings.FormatSettings(this.formatSettingsInEdit);
				}

				checkBox_EnableFormatting.Checked = this.formatSettingsInEdit.FormattingEnabled;

				for (int i = 0; i < this.textFormats.Length; i++)
				{
					this.textFormats[i].FormatFontWithoutStyle = this.formatSettingsInEdit.Font;

					var tf = GetFormatFromIndex(i);
					this.textFormats[i].FormatFontStyle = tf.FontStyle;
					this.textFormats[i].FormatColor     = tf.Color;

					this.textFormats[i].CustomColors = this.customColors;
				}

				ComboBoxHelper.Select(comboBox_ContentSeparator, this.contentSeparator, this.contentSeparator);
				ComboBoxHelper.Select(comboBox_InfoSeparator,    this.infoSeparator,    this.infoSeparator);
				ComboBoxHelper.Select(comboBox_InfoEnclosure,    this.infoEnclosure,    this.infoEnclosure);

				checkBox_TimeStampUseUtc.Checked = this.timeStampUseUtc;
				textBox_TimeStampFormat.Text     = this.timeStampFormat;
				textBox_TimeSpanFormat.Text      = this.timeSpanFormat;
				textBox_TimeDeltaFormat.Text     = this.timeDeltaFormat;
				textBox_TimeDurationFormat.Text  = this.timeDurationFormat;

				// Try to automatically select the according presets:
				Domain.TimeStampFormatPresetEx timeStampFormatPreset;
				Domain.TimeSpanFormatPresetEx  timeSpanFormatPreset;
				Domain.TimeDeltaFormatPresetEx timeDeltaFormatPreset;
				Domain.TimeDeltaFormatPresetEx timeDurationFormatPreset; // Using 'TimeDeltaFormatPreset' too!

				Domain.TimeStampFormatPresetEx.TryParse(this.timeStampFormat,    out timeStampFormatPreset);
				Domain.TimeSpanFormatPresetEx. TryParse(this.timeSpanFormat,     out timeSpanFormatPreset);
				Domain.TimeDeltaFormatPresetEx.TryParse(this.timeDeltaFormat,    out timeDeltaFormatPreset);
				Domain.TimeDeltaFormatPresetEx.TryParse(this.timeDurationFormat, out timeDurationFormatPreset); // Using 'TimeDeltaFormatPreset' too!

				ComboBoxHelper.Select(comboBox_TimeStampFormatPreset,    timeStampFormatPreset);
				ComboBoxHelper.Select(comboBox_TimeSpanFormatPreset,     timeSpanFormatPreset );
				ComboBoxHelper.Select(comboBox_TimeDeltaFormatPreset,    timeDeltaFormatPreset);
				ComboBoxHelper.Select(comboBox_TimeDurationFormatPreset, timeDurationFormatPreset);
				                                //// Clone settings to ensure decoupling:
				monitor_Example.FormatSettings = new Format.Settings.FormatSettings(this.formatSettingsInEdit);

				SetExamples();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ClearExamples()
		{
			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].Clear();

			monitor_Example.Clear();
		}

		private void SetExamples()
		{
			var now = DateTime.Now;
			var diff = (now - this.initialTimeStamp);
			var delta      = new TimeSpan(0, 0, 0, 0, 111); // 111 ms
			var durationTx = new TimeSpan(0, 0, 0, 0,  22); //  22 ms
			var durationRx = new TimeSpan(0, 0, 0, 0,   3); //   3 ms

			var contentSeparator   = this.contentSeparator.ToSeparator();
			var infoSeparator      = this.infoSeparator.ToSeparator();
			var infoEnclosureLeft  = this.infoEnclosure.ToEnclosureLeft();
			var infoEnclosureRight = this.infoEnclosure.ToEnclosureRight();

			var exampleLines = new Domain.DisplayLineCollection(15); // Preset the required capacity to improve memory management.

			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxData(now, 0x41, "A")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxControl(now, 0x13, "<CR>")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxData(now, 0x58, "58h")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxControl(now, 0x10, "<LF>")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeStampInfo(   now,        this.timeStampFormat, this.timeStampUseUtc, infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeSpanInfo(    diff,       this.timeSpanFormat,                        infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeDeltaInfo(   delta,      this.timeDeltaFormat,                       infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeDurationInfo(durationTx, this.timeDurationFormat,                    infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DeviceInfo("COM1",                                                       infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Tx,                                       infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.ContentLength(0,                                                         infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.IOControlInfo("RTS=on")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.WarningInfo("Message")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.ErrorInfo("Message")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.InfoSeparator("_-°,;")));

			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].AddLine(exampleLines[i]);

			var exampleComplete = new Domain.DisplayRepository(21 + 23 + 9); // Preset the required capacity to improve memory management.

			exampleComplete.Enqueue(new Domain.DisplayElement.LineStart());
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStampInfo(now, this.timeStampFormat, this.timeStampUseUtc, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeSpanInfo(diff, this.timeSpanFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeDeltaInfo(diff, this.timeDeltaFormat, infoEnclosureLeft, infoEnclosureRight)); // Also diff since opening!
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DeviceInfo("COM1", infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Tx, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(now, 0x41, "A"));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(now, 0x42, "B"));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(now, 0x43, "C"));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(now, 0x13, "<CR>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(now, 0x10, "<LF>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.ContentLength(5, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeDurationInfo(durationTx, this.timeDurationFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.LineStart());
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStampInfo(now, this.timeStampFormat, this.timeStampUseUtc, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeSpanInfo(diff, this.timeSpanFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeDeltaInfo(delta, this.timeDeltaFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DeviceInfo("COM1", infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Rx, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(now, 0x58, "58h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.ContentSeparator(Domain.Direction.Rx, contentSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(now, 0x59, "59h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.ContentSeparator(Domain.Direction.Rx, contentSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(now, 0x5A, "5Ah"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.IOControlInfo(now, Domain.Direction.Tx, "RTS=off"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.ContentLength(3, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeDurationInfo(durationRx, this.timeDurationFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.LineStart());
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeStampInfo(now, this.timeStampFormat, this.timeStampUseUtc, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeSpanInfo(diff, this.timeSpanFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeDeltaInfo(delta, this.timeDeltaFormat, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.WarningInfo("Message"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSeparator(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.ErrorInfo("Message"));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			monitor_Example.AddLines(exampleComplete.ToLines());
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowFontDialog()
		{
			FontDialog fd;
			Font f = this.formatSettingsInEdit.Font;
			bool fontOK = false;
			bool cancel = false;
			do
			{
				fd = new FontDialog();
				fd.Font = this.formatSettingsInEdit.Font;
				fd.ShowColor = false; // Color is selected by display element format.
				fd.ShowEffects = false; // Effects make no sense.
				fd.AllowVerticalFonts = false; // Not supported by YAT
				if (fd.ShowDialog(this) != DialogResult.OK)
				{
					cancel = true;
				}
				else
				{
					// Check chosen font:
					try
					{
						f = new Font(fd.Font.Name, fd.Font.Size, FontStyle.Regular);
						fontOK = true;
					}
					catch (ArgumentException)
					{
						var dr = MessageBoxEx.Show
						(
							this,
							"Font '" + fd.Font.Name + "' does not support regular style. Select a different font.",
							"Font Not Supported",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Exclamation
						);

						cancel = (dr != DialogResult.OK);
					}
				}
			}
			while (!fontOK && !cancel);

			if (fontOK)
			{
				if ((f.Name != this.formatSettingsInEdit.Font.Name) || (f.Size != this.formatSettingsInEdit.Font.Size))
				{
					this.formatSettingsInEdit.Font = f;
					SetControls();
				}
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowBackgroundColorDialog()
		{
			colorDialog.CustomColors = this.customColors;
			colorDialog.Color        = this.formatSettingsInEdit.BackColor;
			if (colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				this.formatSettingsInEdit.BackColor = colorDialog.Color;
				this.customColors                   = colorDialog.CustomColors;

				SetControls();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
