﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using MKY.Windows.Forms;

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

		private Model.Settings.FormatSettings formatSettings;
		private Model.Settings.FormatSettings formatSettingsInEdit;

		private Domain.InfoElementSeparatorEx infoSeparator;
		private Domain.InfoElementEnclosureEx infoEnclosure;

		private Controls.Monitor[] monitors;
		private Controls.TextFormat[] textFormats;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public FormatSettings(Model.Settings.FormatSettings formatSettings, Domain.InfoElementSeparatorEx infoSeparator, Domain.InfoElementEnclosureEx infoEnclosure)
		{
			InitializeComponent();

			this.formatSettings = formatSettings;
			this.formatSettingsInEdit = new Model.Settings.FormatSettings(formatSettings);

			this.infoSeparator = infoSeparator;
			this.infoEnclosure = infoEnclosure;

			InitializeControls();

			// SetControls() is initially called in the 'Shown' event handler.
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

		/// <summary></summary>
		public Domain.InfoElementSeparatorEx InfoSeparatorResult
		{
			get { return (this.infoSeparator); }
		}

		/// <summary></summary>
		public Domain.InfoElementEnclosureEx InfoEnclosureResult
		{
			get { return (this.infoEnclosure); }
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

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void textFormat_FormatChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				GetFormatFromControl(int.Parse((string)(((Controls.TextFormat)sender).Tag), CultureInfo.InvariantCulture));
				SetControls();
			}
		}

		private void button_Font_Click(object sender, EventArgs e)
		{
			ShowFontDialog();
		}

		private void button_Background_Click(object sender, EventArgs e)
		{
			ShowBackgroundColorDialog();
		}

		private void comboBox_InfoSeparator_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var enclosure = (comboBox_InfoSeparator.SelectedItem as Domain.InfoElementSeparatorEx);
				if (enclosure != null)
					this.infoSeparator = enclosure.ToSeparator();

				SetControls();
			}
		}

		private void comboBox_InfoSeparator_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.InfoElementSeparatorEx separator;
				if (Domain.InfoElementSeparatorEx.TryParse(comboBox_InfoSeparator.Text, out separator))
				{
					this.infoSeparator = separator.ToSeparator();
					SetControls();
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
					e.Cancel = true;
				}
			}
		}

		private void comboBox_InfoEnclosure_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var enclosure = (comboBox_InfoEnclosure.SelectedItem as Domain.InfoElementEnclosureEx);
				if (enclosure != null)
					this.infoEnclosure = enclosure.ToEnclosure();

				SetControls();
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void comboBox_InfoEnclosure_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.InfoElementEnclosureEx enclosure;
				if (Domain.InfoElementEnclosureEx.TryParse(comboBox_InfoEnclosure.Text, out enclosure))
				{
					this.infoEnclosure = enclosure.ToEnclosure();
					SetControls();
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"Enclosure string must be an even number of characters! E.g. enter 2 or 4 characters.",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					e.Cancel = true;
				}
			}
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.formatSettings = this.formatSettingsInEdit;

			// InfoSeparator and InfoEnclosure are handled as separate elements.
		}

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// Do nothing.
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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

				this.infoSeparator = Domain.Settings.DisplaySettings.InfoSeparatorDefault;
				this.infoEnclosure = Domain.Settings.DisplaySettings.InfoEnclosureDefault;

				SetControls();
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.monitors = new Controls.Monitor[]
			{
				monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
				monitor_Date, monitor_Time, monitor_Port, monitor_Direction, monitor_Length,
				monitor_Error,
			};

			this.textFormats = new Controls.TextFormat[]
			{
				textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
				textFormat_Date, textFormat_Time, textFormat_Port, textFormat_Direction, textFormat_Length,
				textFormat_Error,
			};

			comboBox_InfoSeparator.Items.Clear();
			comboBox_InfoSeparator.Items.AddRange(Domain.InfoElementSeparatorEx.GetItems());

			comboBox_InfoEnclosure.Items.Clear();
			comboBox_InfoEnclosure.Items.AddRange(Domain.InfoElementEnclosureEx.GetItems());
		}

		private Model.Types.TextFormat GetFormatFromIndex(int index)
		{
			switch (index)
			{
				case 0: return (this.formatSettingsInEdit.TxDataFormat);
				case 1: return (this.formatSettingsInEdit.TxControlFormat);
				case 2: return (this.formatSettingsInEdit.RxDataFormat);
				case 3: return (this.formatSettingsInEdit.RxControlFormat);
				case 4: return (this.formatSettingsInEdit.DateFormat);
				case 5: return (this.formatSettingsInEdit.TimeFormat);
				case 6: return (this.formatSettingsInEdit.PortFormat);
				case 7: return (this.formatSettingsInEdit.DirectionFormat);
				case 8: return (this.formatSettingsInEdit.LengthFormat);
				case 9: return (this.formatSettingsInEdit.ErrorFormat);
			}
			throw (new ArgumentOutOfRangeException("index", index, "There is no format at this index!"));
		}

		private void GetFormatFromControl(int index)
		{
			Model.Types.TextFormat tf = GetFormatFromIndex(index);
			tf.FontStyle = this.textFormats[index].FormatFontStyle;
			tf.Color     = this.textFormats[index].FormatColor;
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			ClearExamples();

			for (int i = 0; i < this.monitors.Length; i++)
			{                                   // Clone settings before assigning them to control:
				this.monitors[i].FormatSettings = new Model.Settings.FormatSettings(this.formatSettingsInEdit);
			}

			for (int i = 0; i < this.textFormats.Length; i++)
			{
				this.textFormats[i].FormatFontWithoutStyle = this.formatSettingsInEdit.Font;

				Model.Types.TextFormat tf = GetFormatFromIndex(i);
				this.textFormats[i].FormatFontStyle = tf.FontStyle;
				this.textFormats[i].FormatColor     = tf.Color;
			}

			Domain.InfoElementSeparatorEx separator;
			if (Domain.InfoElementSeparatorEx.TryParse(this.infoSeparator, out separator))
				comboBox_InfoSeparator.SelectedItem = separator;
			else
				comboBox_InfoSeparator.Text = this.infoSeparator;

			Domain.InfoElementEnclosureEx enclosure;
			if (Domain.InfoElementEnclosureEx.TryParse(this.infoEnclosure, out enclosure))
				comboBox_InfoEnclosure.SelectedItem = enclosure;
			else
				comboBox_InfoEnclosure.Text = this.infoEnclosure;

			                               // Clone settings before assigning them to control:
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(this.formatSettingsInEdit);

			SetExamples();

			this.isSettingControls.Leave();
		}

		private void ClearExamples()
		{
			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].Clear();

			monitor_Example.Clear();
		}

		private void SetExamples()
		{
			DateTime now = DateTime.Now;

			string infoSeparator      = this.infoSeparator.ToSeparator();
			string infoEnclosureLeft  = this.infoEnclosure.ToEnclosureLeft();
			string infoEnclosureRight = this.infoEnclosure.ToEnclosureRight();

			List<Domain.DisplayLine> exampleLines = new List<Domain.DisplayLine>(10); // Preset the required capactiy to improve memory management.

			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxData(0x41, "41h")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxControl(0x13, "<CR>")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxData(0x42, "42h")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxControl(0x10, "<LF>")));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DateInfo(now, infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeInfo(now, infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.PortInfo(Domain.Direction.Tx, "COM1", infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Tx, infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DataLength(2, infoEnclosureLeft, infoEnclosureRight)));
			exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.ErrorInfo("Message")));

			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].AddLine(exampleLines[i]);

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(26);

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.PortInfo(Domain.Direction.Tx, "COM1", infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Tx, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(0x41, "41h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.DataSpace());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(0x13, "<CR>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DataLength(2, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.PortInfo(Domain.Direction.Rx, "COM1", infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionInfo(Domain.Direction.Rx, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(0x42, "42h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.DataSpace());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxControl(0x10, "<LF>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.DataLength(2, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now, infoEnclosureLeft, infoEnclosureRight));
			exampleComplete.Enqueue(new Domain.DisplayElement.InfoSpace(infoSeparator));
			exampleComplete.Enqueue(new Domain.DisplayElement.ErrorInfo("Message"));

			monitor_Example.AddLines(exampleComplete.ToLines());
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
					Refresh();

					// Check chosen font:
					try
					{
						f = new Font(fd.Font.Name, fd.Font.Size, FontStyle.Regular);
						fontOK = true;
					}
					catch (ArgumentException)
					{
						DialogResult dr = MessageBoxEx.Show
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowBackgroundColorDialog()
		{
			ColorDialog cd = new ColorDialog();
			cd.Color = this.formatSettingsInEdit.BackFormat.Color;
			if (cd.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.formatSettingsInEdit.BackFormat.Color = cd.Color;
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
