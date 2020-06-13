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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Linq;
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
		private Lookup<int, string> payload1Text;
		private Lookup<int, string> fillerBytesText;

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
		////SetControls() is initially called in the 'Paint' event handler.
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
			if (this.isSettingControls)
				return;

			SetControls();
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
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

			var l = new List<KeyValuePair<int, string>>(3); // Preset the required capacity to improve memory management.
			l.Add(new KeyValuePair<int, string>(0,                     "P1'  P2'  ...........................................................  Pn'"));
			l.Add(new KeyValuePair<int, string>(label_Id1.Width,               "P1'  P2'  ...................................................  Pn'"));
			l.Add(new KeyValuePair<int, string>(label_Id1.Width + label_Length1.Width, "P1'  P2'  ...........................................  Pn'"));
			this.payload1Text = (Lookup<int, string>)l.ToLookup(kvp => kvp.Key, kvp => kvp.Value);

			l = new List<KeyValuePair<int, string>>(8); // Preset the required capacity to improve memory management.
			l.Add(new KeyValuePair<int, string>(                                        label_Payload2.Width,                               "0  ..................................  0"));
			l.Add(new KeyValuePair<int, string>(label_Id2.Width +                       label_Payload2.Width,                               "0  ..........................  0"));
			l.Add(new KeyValuePair<int, string>(                  label_Length2.Width + label_Payload2.Width,                               "0  ..........................  0"));
			l.Add(new KeyValuePair<int, string>(                                        label_Payload2.Width + label_TerminatingZero.Width, "0  ..........................  0"));
			l.Add(new KeyValuePair<int, string>(label_Id2.Width + label_Length2.Width + label_Payload2.Width,                               "0  ..................  0"));
			l.Add(new KeyValuePair<int, string>(label_Id2.Width +                       label_Payload2.Width + label_TerminatingZero.Width, "0  ..................  0"));
			l.Add(new KeyValuePair<int, string>(                  label_Length2.Width + label_Payload2.Width + label_TerminatingZero.Width, "0  ..................  0"));
			l.Add(new KeyValuePair<int, string>(label_Id2.Width + label_Length2.Width + label_Payload2.Width + label_TerminatingZero.Width, "0  ..........  0"));
			this.fillerBytesText = (Lookup<int, string>)l.ToLookup(kvp => kvp.Key, kvp => kvp.Value);
		}

		private void SetControls()
		{
			SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
			this.isSettingControls.Enter();
			try
			{
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

				// Windows HID.dll requires that output reports are always filled!
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
				label_Payload1.Text = ExactOrNearest(this.payload1Text, offset);
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
				label_FillerBytes.Text = ExactOrNearest(this.fillerBytesText, offset);
				label_FillerBytesRemarks.Left = offset + this.initialLeftOffset[label_FillerBytesRemarks];
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout(false);
			}
		}

		/// <summary>
		/// Evaluates the exact or nearest match.
		/// </summary>
		/// <remarks>
		/// Required because scaling may lead to slightly off values.
		/// </remarks>
		private static string ExactOrNearest(Lookup<int, string> lookup, int key)
		{
			// Exact?
			if (lookup.Contains(key))
				return (lookup[key].First());

			// Nearest?
			int nearestDiff = int.MaxValue;
			IGrouping<int, string> nearest = null;
			foreach (var group in lookup)
			{
				int currentDiff = Math.Abs(group.Key - key);
				if (nearestDiff > currentDiff)
				{
					nearestDiff = currentDiff;
					nearest = group;
				}
			}

			if (nearest != null)
				return (nearest.First());

			// Nothing!
			return ("");
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnUseIdChanged(EventArgs e)
		{
			EventHelper.RaiseSync(UseIdChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnPrependPayloadByteLengthChanged(EventArgs e)
		{
			EventHelper.RaiseSync(PrependPayloadByteLengthChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAppendTerminatingZeroChanged(EventArgs e)
		{
			EventHelper.RaiseSync(AppendTerminatingZeroChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFillLastReportChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FillLastReportChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FormatChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
