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
// YAT Version 2.4.1
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Domain.Settings;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	public partial class TextDisplaySettingsSet : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;
		private TextDisplaySettings settings = new TextDisplaySettings();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any of the settings properties is changed.")]
		public event EventHandler SettingsChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextDisplaySettingsSet()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TextDisplaySettings Settings
		{
			get { return (this.settings); }
			set
			{
				if (value != null)
					this.settings = value;
				else
					this.settings = new TextDisplaySettings();

				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_ChunkLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.settings.ChunkLineBreakEnabled = checkBox_ChunkLineBreak.Checked;
			SetControls();
			OnSettingsChanged(EventArgs.Empty);
		}

		private void checkBox_LengthLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var llb = this.settings.LengthLineBreak;
			llb.Enabled = checkBox_LengthLineBreak.Checked;
			this.settings.LengthLineBreak = llb;
			SetControls();
			OnSettingsChanged(EventArgs.Empty);
		}

		private void textBox_LengthLineBreak_TextChanged(object sender, EventArgs e)
		{
			int length;
			if (int.TryParse(textBox_LengthLineBreak.Text, out length) && (Math.Abs(length) == 1))
				label_LengthLineBreakUnit.Text = "char";
			else
				label_LengthLineBreakUnit.Text = "chars";
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_LengthLineBreak_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int length;
			if (int.TryParse(textBox_LengthLineBreak.Text, out length) && (length >= 1))
			{
				LengthSettingTuple llb = this.settings.LengthLineBreak;
				llb.Length = length;
				this.settings.LengthLineBreak = llb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
			}
			else
			{
				MessageBoxEx.Show
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

		private void checkBox_TimedLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var tlb = this.settings.TimedLineBreak;
			tlb.Enabled = checkBox_TimedLineBreak.Checked;
			this.settings.TimedLineBreak = tlb;
			SetControls();
			OnSettingsChanged(EventArgs.Empty);
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_TimedLineBreakTimeout_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			int timeout;
			if (int.TryParse(textBox_TimedLineBreakTimeout.Text, out timeout) && (timeout >= 0))
			{
				var tlb = this.settings.TimedLineBreak;
				tlb.Timeout = timeout;
				this.settings.TimedLineBreak = tlb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"Line break time-out must be a positive number!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				e.Cancel = true;
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void SetControls()
		{
			bool enabled;

			this.isSettingControls.Enter();
			try
			{
				checkBox_ChunkLineBreak.Checked = this.settings.ChunkLineBreakEnabled;

				enabled = this.settings.LengthLineBreak.Enabled;
				checkBox_LengthLineBreak.Checked = enabled;
				textBox_LengthLineBreak.Enabled = enabled;
				textBox_LengthLineBreak.Text = this.settings.LengthLineBreak.Length.ToString(CultureInfo.CurrentCulture);

				enabled = this.settings.TimedLineBreak.Enabled;
				checkBox_TimedLineBreak.Checked = enabled;
				textBox_TimedLineBreakTimeout.Enabled = enabled;
				textBox_TimedLineBreakTimeout.Text = this.settings.TimedLineBreak.Timeout.ToString(CultureInfo.CurrentCulture);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnSettingsChanged(EventArgs e)
		{
			EventHelper.RaiseSync(SettingsChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
