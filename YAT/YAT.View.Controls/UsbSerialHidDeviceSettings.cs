﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <remarks>The preview control is not scalable to simplify implementation.</remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[DefaultEvent("ReportFormatChanged")]
	public partial class UsbSerialHidDeviceSettings : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const           MKY.IO.Usb.SerialHidDeviceSettingsPreset PresetDefault = MKY.IO.Serial.Usb.SerialHidDeviceSettings.PresetDefault;
		private static readonly MKY.IO.Usb.SerialHidReportFormat   ReportFormatDefault = MKY.IO.Serial.Usb.SerialHidDeviceSettings.ReportFormatDefault;
		private static readonly MKY.IO.Usb.SerialHidRxFilterUsage RxFilterUsageDefault = MKY.IO.Serial.Usb.SerialHidDeviceSettings.RxFilterUsageDefault;

		private const MKY.IO.Serial.Usb.SerialHidFlowControl FlowControlDefault = MKY.IO.Serial.Usb.SerialHidFlowControl.None;
		private const bool AutoOpenDefault                                      = MKY.IO.Serial.Usb.SerialHidDeviceSettings.AutoOpenDefault;

		private const string AnyIdIndication = "*";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private MKY.IO.Usb.SerialHidDeviceSettingsPreset preset = PresetDefault;
		private MKY.IO.Usb.SerialHidReportFormat reportFormat   = ReportFormatDefault;
		private MKY.IO.Usb.SerialHidRxFilterUsage rxFilterUsage = RxFilterUsageDefault;

		private MKY.IO.Serial.Usb.SerialHidFlowControl flowControl = FlowControlDefault;
		private bool autoOpen                                      = AutoOpenDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Preset property is changed.")]
		public event EventHandler PresetChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the ReportFormat property is changed.")]
		public event EventHandler ReportFormatChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the RxFilterUsage property is changed.")]
		public event EventHandler RxFilterUsageChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the FlowControl property is changed.")]
		public event EventHandler FlowControlChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the AutoOpen property is changed.")]
		public event EventHandler AutoOpenChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public UsbSerialHidDeviceSettings()
		{
			InitializeComponent();

			InitializeControls();
			//// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("USB Ser/HID")]
		[Description("One of the available presets.")]
		[DefaultValue(PresetDefault)]
		public MKY.IO.Usb.SerialHidDeviceSettingsPreset Preset
		{
			get { return (this.preset); }
			set
			{
				if (this.preset != value)
				{
					if (value != MKY.IO.Usb.SerialHidDeviceSettingsPreset.None) // Only true presets shall change the other properties.
					{
						// Keep preset to be able to restore ambiguous presets:
						if (value == MKY.IO.Usb.SerialHidDeviceSettingsPreset.YAT)         // But do not keep [YAT] preset as
							this.preset = MKY.IO.Usb.SerialHidDeviceSettingsPreset.Common; // that shall be redirected to [Common].
						else
							this.preset = value;

						this.reportFormat  = ((MKY.IO.Usb.SerialHidDeviceSettingsPresetEx)value).ToReportFormat();
						this.rxFilterUsage = ((MKY.IO.Usb.SerialHidDeviceSettingsPresetEx)value).ToRxFilterUsage();
						SetControls(); // Assign field instead of properties to reduce the number of SetControls() invocations.
						OnPresetChanged(EventArgs.Empty);
						OnReportFormatChanged(EventArgs.Empty);
						OnRxFilterUsageChanged(EventArgs.Empty);
					}
					else
					{
						this.preset = value;
						SetControls();
						OnPresetChanged(EventArgs.Empty);
					}
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MKY.IO.Usb.SerialHidReportFormat ReportFormat
		{
			get { return (this.reportFormat); }
			set
			{
				if (this.reportFormat != value)
				{
					this.reportFormat = value;
					SetControls();
					OnReportFormatChanged(EventArgs.Empty);
				}
				else
				{
					// Set controls even if format did not change. This ensures that the preset
					// selection is updated with the current format, i.e. "<No preset selected>"
					// is changed to the preset in use (if applicable).
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MKY.IO.Usb.SerialHidRxFilterUsage RxFilterUsage
		{
			get { return (this.rxFilterUsage); }
			set
			{
				if (this.rxFilterUsage != value)
				{
					this.rxFilterUsage = value;
					SetControls();
					OnRxFilterUsageChanged(EventArgs.Empty);
				}
				else
				{
					// Set controls even if format did not change. This ensures that the preset
					// selection is updated with the current format, i.e. "<No preset selected>"
					// is changed to the preset in use (if applicable).
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("USB Ser/HID")]
		[Description("The flow control type.")]
		[DefaultValue(FlowControlDefault)]
		public virtual MKY.IO.Serial.Usb.SerialHidFlowControl FlowControl
		{
			get { return (this.flowControl); }
			set
			{
				if (this.flowControl != value)
				{
					this.flowControl = value;
					SetControls();
					OnFlowControlChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("USB Ser/HID")]
		[Description("The auto open setting.")]
		[DefaultValue(AutoOpenDefault)]
		public bool AutoOpen
		{
			get { return (this.autoOpen); }
			set
			{
				if (this.autoOpen != value)
				{
					this.autoOpen = value;
					SetControls();
					OnAutoOpenChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void UsbSerialHidPortSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void UsbSerialHidDeviceSettings_EnabledChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_UseId_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.reportFormat.UseId = checkBox_UseId.Checked;
			SetControls();
			OnReportFormatChanged(EventArgs.Empty);
		}

		private void textBox_Id_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			byte id;
			if (byte.TryParse(textBox_Id.Text, out id)) // Attention, a similar validation exists in 'Domain.Parser.KeywordEx'. Changes here may have to be applied there too.
			{
				this.reportFormat.Id = id;
				SetControls();
				OnReportFormatChanged(EventArgs.Empty);
			}
			else
			{
				MessageBoxEx.Show
				(
					this,
					"ID must be a numeric value within 0..255!",
					"Invalid Input",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				e.Cancel = true;
			}
		}

		private void checkBox_SeparateRxId_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.rxFilterUsage.SeparateRxId = checkBox_SeparateRxId.Checked;
			SetControls();
			OnRxFilterUsageChanged(EventArgs.Empty);
		}

		private void textBox_RxId_Validating(object sender, CancelEventArgs e)
		{
			if (this.isSettingControls)
				return;

			string idText = textBox_RxId.Text;
			if (idText == AnyIdIndication)
			{
				this.rxFilterUsage.AnyRxId = true;
				this.rxFilterUsage.RxId = this.reportFormat.Id;
				SetControls();
				OnRxFilterUsageChanged(EventArgs.Empty);
			}
			else
			{
				byte id;
				if (byte.TryParse(idText, out id))
				{
					this.rxFilterUsage.AnyRxId = false;
					this.rxFilterUsage.RxId = id;
					SetControls();
					OnRxFilterUsageChanged(EventArgs.Empty);
				}
				else
				{
					MessageBoxEx.Show
					(
						this,
						"ID must be a numeric value within 0..255 or enter '" + AnyIdIndication + "' to accept any ID.",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					e.Cancel = true;
				}
			}
		}

		private void checkBox_PrependPayloadByteLength_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.reportFormat.PrependPayloadByteLength = checkBox_PrependPayloadByteLength.Checked;
			SetControls();
			OnReportFormatChanged(EventArgs.Empty);
		}

		private void checkBox_AppendTerminatingZero_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			this.reportFormat.AppendTerminatingZero = checkBox_AppendTerminatingZero.Checked;
			SetControls();
			OnReportFormatChanged(EventArgs.Empty);
		}

		private void checkBox_FillLastReport_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			string message =
				"USB HID is designed to use complete reports; " +
				"the Windows HID infrastructure even enforces this constraint. " +
				"As a consequence, this option must always be kept enabled.";

			// Windows HID.dll requires that input as well as output reports are always filled!
			// Still, enable the check box in order to make tool tip visible to the user.
			MessageBoxEx.Show
			(
				this,
				message,
				"USB HID Constraint",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);

		////this.reportFormat.FillLastReport = checkBox_FillLastReport.Checked;
			SetControls();
		////OnReportFormatChanged(EventArgs.Empty);
		}

		private void comboBox_Preset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var preset = (comboBox_Preset.SelectedItem as MKY.IO.Usb.SerialHidDeviceSettingsPresetEx);
			if (preset != null)
			{
				Preset = preset;
			////SetControls() will be called by the Preset property method.
			}
			else
			{
				SetControls();
			}
		}

		private void linkLabel_Info_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(Parent, e);
		}

		private void comboBox_FlowControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			FlowControl = (MKY.IO.Serial.Usb.SerialHidFlowControlEx)comboBox_FlowControl.SelectedItem;
		}

		private void checkBox_AutoOpen_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			AutoOpen = checkBox_AutoOpen.Checked;
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
				comboBox_Preset     .Items.AddRange(MKY.IO.Usb.SerialHidDeviceSettingsPresetEx.GetItems());
				comboBox_FlowControl.Items.AddRange(MKY.IO.Serial.Usb.SerialHidFlowControlEx.GetItems());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				label_ReportFormat.Enabled = Enabled;

				if (this.reportFormat.UseId)
				{
					checkBox_UseId.Text        = "Tx &ID:";
					checkBox_SeparateRxId.Text = "Rx I&D:";

					checkBox_UseId.Checked        =  Enabled;
					checkBox_SeparateRxId.Enabled =  Enabled;
					checkBox_SeparateRxId.Checked = (Enabled ? this.rxFilterUsage.SeparateRxId : false);
					textBox_Id.Enabled            =  Enabled;
					textBox_RxId.Enabled          = (Enabled ? this.rxFilterUsage.SeparateRxId : false);

					textBox_Id.Text = this.reportFormat.Id.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for report ID!
					if (!this.rxFilterUsage.SeparateRxId) // Typical case = same ID for Tx and Rx.
					{
						textBox_RxId.Text = this.reportFormat.Id.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for report ID!
					}
					else // Special case = separate ID for Rx.
					{
						if (this.rxFilterUsage.AnyRxId)
							textBox_RxId.Text = AnyIdIndication;
						else
							textBox_RxId.Text = this.rxFilterUsage.RxId.ToString(CultureInfo.InvariantCulture); // 'InvariantCulture' for report ID!
					}
				}
				else
				{
					checkBox_UseId.Text        = "Use &ID:";
					checkBox_SeparateRxId.Text = "";

					checkBox_UseId.Checked        = false;
					checkBox_SeparateRxId.Enabled = false;
					checkBox_SeparateRxId.Checked = false;
					textBox_Id.Enabled            = false;
					textBox_RxId.Enabled          = false;

					textBox_Id.Text   = "";
					textBox_RxId.Text = "";
				}

				checkBox_PrependPayloadByteLength.Checked = (Enabled ? this.reportFormat.PrependPayloadByteLength : false);
				checkBox_AppendTerminatingZero.Checked    = (Enabled ? this.reportFormat.AppendTerminatingZero : false);
			////checkBox_FillLastReport.Checked           = (Enabled ? this.reportFormat.FillLastReport : false);
				checkBox_FillLastReport.Checked           = true; // Windows HID.dll requires that output reports are always filled!

				reportFormatPreview.Format = this.reportFormat;

				if (Enabled)
				{
					comboBox_Preset.SelectedItem = (MKY.IO.Usb.SerialHidDeviceSettingsPresetEx)this.preset;

					// Note that 'DropDownList' requires that an item like "[No preset selected]" is
					// listed. It is not possible to set the 'SelectedIndex' to 'ControlEx.InvalidIndex'
					// and then set an arbitrary 'Text';
				}
				else // Note that 'SelectionHelper' is not used for this 'DropDownList'-style ComboBox.
				{
					comboBox_Preset.SelectedIndex = ControlEx.InvalidIndex;
				}

				string linkText;
				string linkUri;
				if (((MKY.IO.Usb.SerialHidDeviceSettingsPresetEx)preset).HasInfoLink(out linkText, out linkUri))
				{
					linkLabel_Info.Links.Clear();
					linkLabel_Info.Text = linkText;
					linkLabel_Info.Links.Add(0, linkText.Length, linkUri);
					linkLabel_Info.Visible = true;
				}
				else
				{
					linkLabel_Info.Visible = false;
				}

				if (Enabled)
				{
					comboBox_FlowControl.SelectedItem = (MKY.IO.Serial.Usb.SerialHidFlowControlEx)this.flowControl;
				}
				else
				{
					comboBox_FlowControl.SelectedIndex = ControlEx.InvalidIndex;
				}

				checkBox_AutoOpen.Checked = (Enabled ? this.autoOpen : false);
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
		protected virtual void OnPresetChanged(EventArgs e)
		{
			EventHelper.RaiseSync(PresetChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnReportFormatChanged(EventArgs e)
		{
			EventHelper.RaiseSync(ReportFormatChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRxFilterUsageChanged(EventArgs e)
		{
			EventHelper.RaiseSync(RxFilterUsageChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFlowControlChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FlowControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoOpenChanged(EventArgs e)
		{
			EventHelper.RaiseSync(AutoOpenChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
