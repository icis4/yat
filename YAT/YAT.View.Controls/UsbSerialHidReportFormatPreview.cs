//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MKY;
using MKY.IO.Usb;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <remarks>This control is not scalable to simplify implementation.</remarks>
	[DefaultEvent("UseIdChanged")]
	public partial class UsbSerialHidReportFormatPreview : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool UseIdDefault                    = false;
		private const bool PrependPayloadByteLengthDefault = false;
		private const bool AppendTerminatingZeroDefault    = false;
		private const bool FillLastReportDefault           = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;
		private Dictionary<Control, int> initialLeftOffset;
		private Dictionary<int, string> payload1Text;
		private Dictionary<int, string> fillerBytesText;

		private bool useId                    = UseIdDefault;
		private bool prependPayloadByteLength = PrependPayloadByteLengthDefault;
		private bool appendTerminatingZero    = AppendTerminatingZeroDefault;
		private bool fillLastReport           = FillLastReportDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Fired when UseId changed.")]
		public event EventHandler UseIdChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Fired when PrependPayloadByteLength changed.")]
		public event EventHandler PrependPayloadByteLengthChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Fired when AppendTerminatingZero changed.")]
		public event EventHandler AppendTerminatingZeroChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Fired when FillLastReport changed.")]
		public event EventHandler FillLastReportChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Fired when Format changed.")]
		public event EventHandler FormatChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="UsbSerialHidReportFormatPreview"/> class.
		/// </summary>
		public UsbSerialHidReportFormatPreview()
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
		[Category("Data")]
		[Description("Indicates whether a report ID is used.")]
		[DefaultValue(UseIdDefault)]
		public bool UseId
		{
			get { return (this.useId); }
			set
			{
				if (this.useId != value)
				{
					this.useId = value;
					SetControls();
					OnUseIdChanged(EventArgs.Empty);

					// Also notify change of combined property:
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Data")]
		[Description("Indicates whether the payload byte length is prepended.")]
		[DefaultValue(PrependPayloadByteLengthDefault)]
		public bool PrependPayloadByteLength
		{
			get { return (this.prependPayloadByteLength); }
			set
			{
				if (this.prependPayloadByteLength != value)
				{
					this.prependPayloadByteLength = value;
					SetControls();
					OnPrependPayloadByteLengthChanged(EventArgs.Empty);

					// Also notify change of combined property:
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Data")]
		[Description("Indicates whether a terminating zero is appended.")]
		[DefaultValue(AppendTerminatingZeroDefault)]
		public bool AppendTerminatingZero
		{
			get { return (this.appendTerminatingZero); }
			set
			{
				if (this.appendTerminatingZero != value)
				{
					this.appendTerminatingZero = value;
					SetControls();
					OnAppendTerminatingZeroChanged(EventArgs.Empty);

					// Also notify change of combined property:
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Data")]
		[Description("Indicates whether the last report is filled.")]
		[DefaultValue(FillLastReportDefault)]
		public bool FillLastReport
		{
			get { return (this.fillLastReport); }
			set
			{
				if (this.fillLastReport != value)
				{
					this.fillLastReport = value;
					SetControls();
					OnFillLastReportChanged(EventArgs.Empty);

					// Also notify change of combined property:
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Data")]
		[Description("The complete report format.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SerialHidReportFormat Format
		{
			get { return (new SerialHidReportFormat(this.useId, this.prependPayloadByteLength, this.appendTerminatingZero, this.fillLastReport)); }
			set
			{
				if ((this.useId                    != value.UseId) ||
					(this.prependPayloadByteLength != value.PrependPayloadByteLength) ||
					(this.appendTerminatingZero    != value.AppendTerminatingZero) ||
					(this.fillLastReport           != value.FillLastReport))
				{
					this.useId                    = value.UseId;
					this.prependPayloadByteLength = value.PrependPayloadByteLength;
					this.appendTerminatingZero    = value.AppendTerminatingZero;
					this.fillLastReport           = value.FillLastReport;

					SetControls();
					OnFormatChanged(EventArgs.Empty);

					// Also notify change of all other properties:
					OnUseIdChanged                   (EventArgs.Empty);
					OnPrependPayloadByteLengthChanged(EventArgs.Empty);
					OnAppendTerminatingZeroChanged   (EventArgs.Empty);
					OnFillLastReportChanged          (EventArgs.Empty);
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
		private void UsbSerialHidReportFormatPreview_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		private void UsbSerialHidReportFormatPreview_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void InitializeControls()
		{
			this.initialLeftOffset = new Dictionary<Control, int>(6); // Preset the required capacity to improve memory management.
			this.initialLeftOffset.Add(label_LengthLine,             label_LengthLine.Left             - label_Length2.Left);
			this.initialLeftOffset.Add(label_LengthRemarks,          label_LengthRemarks.Left          - label_Length2.Left);
			this.initialLeftOffset.Add(label_PayloadRemarks,         label_PayloadRemarks.Left         - label_Payload2.Left);
			this.initialLeftOffset.Add(label_TerminatingZeroLine,    label_TerminatingZeroLine.Left    - label_TerminatingZero.Left);
			this.initialLeftOffset.Add(label_TerminatingZeroRemarks, label_TerminatingZeroRemarks.Left - label_TerminatingZero.Left);
			this.initialLeftOffset.Add(label_FillerBytesRemarks,     label_FillerBytesRemarks.Left     - label_FillerBytes.Left);

			int widthOffset = label_Id1.Width;
			if (label_Id1.Width != label_Length1.Width)
				throw (new ArgumentException("Labels must have equal width, but are " + label_Id1.Width + " and " + label_Length1.Width + "!"));

			this.payload1Text = new Dictionary<int, string>(3); // Preset the required capacity to improve memory management.
			this.payload1Text.Add((widthOffset * 0), "P1'  P2'  ...........................................................  Pn'");
			this.payload1Text.Add((widthOffset * 1), "P1'  P2'  ...................................................  Pn'");
			this.payload1Text.Add((widthOffset * 2), "P1'  P2'  ...........................................  Pn'");

			widthOffset = label_Id2.Width;
			if (label_Id2.Width != label_Length2.Width)
				throw (new ArgumentException("Labels must have equal width, but are " + label_Id2.Width + " and " + label_Length2.Width + "!"));
			if (label_Id2.Width != label_TerminatingZero.Width)
				throw (new ArgumentException("Labels must have equal width, but are " + label_Id2.Width + " and " + label_TerminatingZero.Width + "!"));

			this.fillerBytesText = new Dictionary<int, string>(4); // Preset the required capacity to improve memory management.
			this.fillerBytesText.Add((widthOffset * 0) + label_Payload2.Width, "0  ..................................  0");
			this.fillerBytesText.Add((widthOffset * 1) + label_Payload2.Width, "0  ..........................  0");
			this.fillerBytesText.Add((widthOffset * 2) + label_Payload2.Width, "0  ..................  0");
			this.fillerBytesText.Add((widthOffset * 3) + label_Payload2.Width, "0  ..........  0");
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			SuspendLayout();

			label_Id1.Enabled = Enabled;
			label_Id2.Enabled = Enabled;
			label_IdLine.Visible = Enabled;
			label_IdRemarks.Visible = Enabled;
			label_Length1.Enabled = Enabled;
			label_Length2.Enabled = Enabled;
			label_LengthLine.Enabled = Enabled;
			label_LengthRemarks.Enabled = Enabled;
			label_Payload1.Enabled = Enabled;
			label_Payload2.Enabled = Enabled;
			label_PayloadRemarks.Enabled = Enabled;
			label_TerminatingZero.Enabled = Enabled;
			label_TerminatingZeroLine.Enabled = Enabled;
			label_TerminatingZeroRemarks.Enabled = Enabled;
			label_FillerBytes.Enabled = Enabled;
			label_FillerBytesRemarks.Enabled = Enabled;

			label_Id1.Visible = this.useId;
			label_Id2.Visible = this.useId;
			label_IdLine.Visible = this.useId;
			label_IdRemarks.Visible = this.useId;
			label_Length1.Visible = this.prependPayloadByteLength;
			label_Length2.Visible = this.prependPayloadByteLength;
			label_LengthLine.Visible = this.prependPayloadByteLength;
			label_LengthRemarks.Visible = this.prependPayloadByteLength;
			label_TerminatingZero.Visible = this.appendTerminatingZero;
			label_TerminatingZeroLine.Visible = this.appendTerminatingZero;
			label_TerminatingZeroRemarks.Visible = this.appendTerminatingZero;
		////label_FillerBytes.Visible = this.fillLastReport;
		////label_FillerBytesRemarks.Visible = this.fillLastReport;

			// Windows HID.dll requires that outgoing reports are always filled!
			label_FillerBytes.Visible = true;
			label_FillerBytesRemarks.Visible = true;

			int offset = 0;
			if (this.useId)
				offset += label_Id1.Width;

			label_Length1.Left = offset;
			label_Length2.Left = offset;
			label_LengthLine.Left = offset + this.initialLeftOffset[label_LengthLine];
			label_LengthRemarks.Left = offset + this.initialLeftOffset[label_LengthRemarks];

			if (this.prependPayloadByteLength)
				offset += label_Length1.Width;

			label_Payload1.Left = offset;
			label_Payload1.Width = Width - offset;
			label_Payload1.Text = this.payload1Text[offset];
			label_Payload2.Left = offset;
			label_PayloadRemarks.Left = offset + this.initialLeftOffset[label_PayloadRemarks];

			offset += label_Payload2.Width;

			label_TerminatingZero.Left = offset;
			label_TerminatingZeroLine.Left = offset + this.initialLeftOffset[label_TerminatingZeroLine];
			label_TerminatingZeroRemarks.Left = offset + this.initialLeftOffset[label_TerminatingZeroRemarks];

			if (this.appendTerminatingZero)
				offset += label_TerminatingZero.Width;

			label_FillerBytes.Left = offset;
			label_FillerBytes.Width = Width - offset;
			label_FillerBytes.Text = this.fillerBytesText[offset];
			label_FillerBytesRemarks.Left = offset + this.initialLeftOffset[label_FillerBytesRemarks];

			ResumeLayout();

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnUseIdChanged(EventArgs e)
		{
			EventHelper.FireSync(UseIdChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnPrependPayloadByteLengthChanged(EventArgs e)
		{
			EventHelper.FireSync(PrependPayloadByteLengthChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAppendTerminatingZeroChanged(EventArgs e)
		{
			EventHelper.FireSync(AppendTerminatingZeroChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFillLastReportChanged(EventArgs e)
		{
			EventHelper.FireSync(FillLastReportChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHelper.FireSync(FormatChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
