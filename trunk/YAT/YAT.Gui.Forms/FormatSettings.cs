//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2015 Matthias Kläy.
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

namespace YAT.Gui.Forms
{
	/// <summary></summary>
	public partial class FormatSettings : System.Windows.Forms.Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Model.Settings.FormatSettings formatSettings;
		private Model.Settings.FormatSettings formatSettingsInEdit;

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
			this.formatSettingsInEdit = new Model.Settings.FormatSettings(formatSettings);

			InitializeExamples();
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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
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

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.formatSettings = this.formatSettingsInEdit;
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
			DateTime now = DateTime.Now;

			this.exampleLines = new List<Domain.DisplayLine>();

			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxData(0x41, "41h")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TxControl(0x13, "<CR>")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxData(0x42, "42h")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.RxControl(0x10, "<LF>")));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DateInfo(now)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.TimeInfo(now)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.DirectionStamp(Domain.Direction.Tx)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.Length(2)));
			this.exampleLines.Add(new Domain.DisplayLine(new Domain.DisplayElement.IOError("Message")));

			Domain.DisplayRepository exampleComplete = new Domain.DisplayRepository(24);

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionStamp(Domain.Direction.Tx));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxData(0x41, "41h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.TxControl(0x13, "<CR>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.Length(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.DirectionStamp(Domain.Direction.Rx));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxData(0x42, "42h"));
			exampleComplete.Enqueue(new Domain.DisplayElement.Space());
			exampleComplete.Enqueue(new Domain.DisplayElement.RxControl(0x10, "<LF>"));
			exampleComplete.Enqueue(new Domain.DisplayElement.RightMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.Length(2));
			exampleComplete.Enqueue(new Domain.DisplayElement.LineBreak());

			exampleComplete.Enqueue(new Domain.DisplayElement.DateInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.TimeInfo(now));
			exampleComplete.Enqueue(new Domain.DisplayElement.LeftMargin());
			exampleComplete.Enqueue(new Domain.DisplayElement.IOError("Message"));

			this.exampleComplete = exampleComplete.ToLines();
		}

		private void InitializeControls()
		{
			this.monitors = new Controls.Monitor[]
			{
				monitor_TxData, monitor_TxControl, monitor_RxData, monitor_RxControl,
				monitor_Date, monitor_Time, monitor_Direction, monitor_Length, monitor_Error,
			};

			this.textFormats = new Controls.TextFormat[]
			{
				textFormat_TxData, textFormat_TxControl, textFormat_RxData, textFormat_RxControl,
				textFormat_Date, textFormat_Time, textFormat_Direction, textFormat_Length, textFormat_Error,
			};

			for (int i = 0; i < this.monitors.Length; i++)
				this.monitors[i].AddLine(this.exampleLines[i]);

			monitor_Example.AddLines(this.exampleComplete);
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
				case 6: return (this.formatSettingsInEdit.DirectionFormat);
				case 7: return (this.formatSettingsInEdit.LengthFormat);
				case 8: return (this.formatSettingsInEdit.ErrorFormat);
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

			for (int i = 0; i < this.monitors.Length; i++)
			{                          // Clone settings before assigning them to control
				this.monitors[i].FormatSettings = new Model.Settings.FormatSettings(this.formatSettingsInEdit);
			}

			for (int i = 0; i < this.textFormats.Length; i++)
			{
				this.textFormats[i].FormatFontWithoutStyle = this.formatSettingsInEdit.Font;

				Model.Types.TextFormat tf = GetFormatFromIndex(i);
				this.textFormats[i].FormatFontStyle = tf.FontStyle;
				this.textFormats[i].FormatColor     = tf.Color;
			}

			                           // Clone settings before assigning them to control
			monitor_Example.FormatSettings = new Model.Settings.FormatSettings(this.formatSettingsInEdit);

			this.isSettingControls.Leave();
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
						DialogResult dr = MessageBoxEx.Show
						(
							this,
							"Font '" + fd.Font.Name + "' does not support regular style. Choose a different font.",
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
