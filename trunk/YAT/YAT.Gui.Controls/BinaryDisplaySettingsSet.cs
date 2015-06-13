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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Domain.Settings;
using YAT.Gui.Utilities;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	public partial class BinaryDisplaySettingsSet : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;
		private BinaryDisplaySettings settings = new BinaryDisplaySettings();

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
		public BinaryDisplaySettingsSet()
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
		public BinaryDisplaySettings Settings
		{
			get { return (this.settings); }
			set
			{
				if (value != null)
					this.settings = value;
				else
					this.settings = new BinaryDisplaySettings();

				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_LengthLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.BinaryLengthLineBreak llb = this.settings.LengthLineBreak;
				llb.Enabled = checkBox_LengthLineBreak.Checked;
				this.settings.LengthLineBreak = llb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
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

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_LengthLineBreak_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int length;
				if (int.TryParse(textBox_LengthLineBreak.Text, out length) && (length >= 1))
				{
					Domain.BinaryLengthLineBreak llb = this.settings.LengthLineBreak;
					llb.LineLength = length;
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
		}

		private void checkBox_SequenceLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.BinarySequenceLineBreak slb = this.settings.SequenceLineBreak;
				slb.Enabled = checkBox_SequenceLineBreak.Checked;
				this.settings.SequenceLineBreak = slb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
			}
		}

		private void textBox_SequenceLineBreakSequence_Validating(object sender, CancelEventArgs e)
		{
			int invalidTextStart;
			int invalidTextLength;
			if (Validation.ValidateSequence(this, "Sequence", textBox_SequenceLineBreakSequence.Text, Domain.Parser.Modes.AllByteArrayResults, out invalidTextStart, out invalidTextLength))
			{
				Domain.BinarySequenceLineBreak slb = this.settings.SequenceLineBreak;
				slb.Sequence = textBox_SequenceLineBreakSequence.Text;
				this.settings.SequenceLineBreak = slb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
			}
			else
			{
				textBox_SequenceLineBreakSequence.Select(invalidTextStart, invalidTextLength);
				e.Cancel = true;
			}
		}

		private void checkBox_TimedLineBreak_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.BinaryTimedLineBreak tlb = this.settings.TimedLineBreak;
				tlb.Enabled = checkBox_TimedLineBreak.Checked;
				this.settings.TimedLineBreak = tlb;
				SetControls();
				OnSettingsChanged(EventArgs.Empty);
			}
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an invalid user input.")]
		private void textBox_TimedLineBreakTimeout_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int timeout;
				if (int.TryParse(textBox_TimedLineBreakTimeout.Text, out timeout) && (timeout >= 0))
				{
					Domain.BinaryTimedLineBreak tlb = this.settings.TimedLineBreak;
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
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			bool enabled;

			this.isSettingControls.Enter();

			enabled = this.settings.LengthLineBreak.Enabled;
			checkBox_LengthLineBreak.Checked = enabled;
			textBox_LengthLineBreak.Enabled = enabled;
			textBox_LengthLineBreak.Text = this.settings.LengthLineBreak.LineLength.ToString(CultureInfo.CurrentCulture);

			enabled = this.settings.SequenceLineBreak.Enabled;
			checkBox_SequenceLineBreak.Checked = enabled;
			textBox_SequenceLineBreakSequence.Enabled = enabled;
			textBox_SequenceLineBreakSequence.Text = this.settings.SequenceLineBreak.Sequence;

			enabled = this.settings.TimedLineBreak.Enabled;
			checkBox_TimedLineBreak.Checked = enabled;
			textBox_TimedLineBreakTimeout.Enabled = enabled;
			textBox_TimedLineBreakTimeout.Text = this.settings.TimedLineBreak.Timeout.ToString(CultureInfo.CurrentCulture);

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnSettingsChanged(EventArgs e)
		{
			EventHelper.FireSync(SettingsChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
