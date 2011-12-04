//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2011 Matthias Kläy.
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

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Abstract base for all HID report containers.
	/// </summary>
	public abstract class HidReportContainer
	{
		private HidDevice device;
		private int reportLength;
		private byte reportId;

		/// <summary>
		/// Creates a report container and stores the reference to the device in use as well as
		/// the length of a report.
		/// </summary>
		protected HidReportContainer(HidDevice device, int reportLength)
		{
			this.device = device;
			this.reportLength = reportLength;
		}

		/// <summary>
		/// The device associated with this report.
		/// </summary>
		protected HidDevice Device
		{
			get { return (this.device); }
		}

		/// <summary>
		/// The length of a report. The length is given by the device capabilities.
		/// </summary>
		public int ReportLength
		{
			get { return (this.reportLength); }
		}

		/// <summary>
		/// The ID of a report. The ID is given by the device capabilities.
		/// </summary>
		public byte ReportId
		{
			get { return (this.reportId); }
		}

		/// <summary>
		/// Sets the ID of a report. The ID is given by the device capabilities.
		/// </summary>
		protected void SetReportId(byte id)
		{
			this.reportId = id;
		}
	}

	/// <summary>
	/// An HID input report container is a simple wrapper around a byte buffer.
	/// </summary>
	public class HidInputReportContainer : HidReportContainer
	{
		private byte[] data;

		/// <summary>
		/// Creates an input report container.
		/// </summary>
		public HidInputReportContainer(HidDevice device)
			: base(device, device.InputReportLength)
		{
		}

		/// <summary>
		/// Returns the data that was received via input reports.
		/// </summary>
		public byte[] Data
		{
			get { return (this.data); }
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report)
		{
			// Ensure that report length matches
			if (report.Length < Device.InputReportLength)
				throw (new ArgumentException("Length of input report doesn't match the device's capabilities", "report"));

			// Get report ID which is located in the first byte
			SetReportId(report[0]);

			// Get report data without the report ID
			List<byte> data = new List<byte>();
			for (int i = 1; i < report.Length; i++)
			{
				if (report[i] != 0x00)
					data.Add(report[i]);
				else
					break;
			}
			this.data = data.ToArray();
		}
	}

	/// <summary>
	/// An HID output report container puts any amount of data into chunks that match the size of
	/// the device's reported output report capabilities.
	/// </summary>
	public class HidOutputReportContainer : HidReportContainer
	{
		private byte[][] reports;

		/// <summary>
		/// Creates an output report container.
		/// </summary>
		public HidOutputReportContainer(HidDevice device)
			: base(device, device.OutputReportLength)
		{
		}

		/// <summary>
		/// Returns the output reports that were created from any amount of data.
		/// </summary>
		public byte[][] Reports
		{
			get { return (this.reports); }
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		public void CreateReportsFromData(byte reportId, byte[] data)
		{
			List<byte[]> reports = new List<byte[]>();
			int usableLength = ReportLength - 1; // 1 byte is used by the report ID.
			
			int offset = 0;
			while (offset < (data.Length))
			{
				// Create the report.
				byte[] report = new byte[ReportLength];

				// Set the report ID.
				report[0] = reportId;

				// Copy as much as possible into remaining usable bytes of the report.
				int lengthToCopy = (data.Length - offset) % usableLength;
				Array.Copy(data, offset, report, 1, lengthToCopy);
				reports.Add(report);

				offset += usableLength;
			}

			this.reports = reports.ToArray();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
