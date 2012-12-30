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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2013 Matthias Kläy.
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
using System.Collections.ObjectModel;

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
		private ReadOnlyCollection<byte> data;

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
		public ReadOnlyCollection<byte> Data
		{
			get { return (this.data); }
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report)
		{
			// Ensure that report length matches:
			if (report.Length < Device.InputReportLength)
				throw (new ArgumentException("Length of input report exceeds the device's capabilities", "report"));

			// Get report ID which is located in the first byte:
			SetReportId(report[0]);

			// Get report data without the report ID:
			List<byte> data = new List<byte>();
			for (int i = 1; i < report.Length; i++)
			{
				if (report[i] != 0x00)
					data.Add(report[i]);
				else
					break;
			}

			this.data = data.AsReadOnly();
		}
	}

	/// <summary>
	/// An HID output report container puts any amount of data into chunks that match the size of
	/// the device's reported output report capabilities.
	/// </summary>
	public class HidOutputReportContainer : HidReportContainer
	{
		private ReadOnlyCollection<byte[]> reports;

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
		public ReadOnlyCollection<byte[]> Reports
		{
			get { return (this.reports); }
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		public void CreateReportsFromData(byte reportId, byte[] data)
		{
			int usableLength = ReportLength - 1; // 1 byte is used by the report ID.

			List<byte[]> reports = new List<byte[]>();
			bool reportIsFull = false;

			int accumulatedLength = 0;
			while (accumulatedLength < (data.Length))
			{
				// Evaluate the required report length:
				int remainingLength = data.Length - accumulatedLength;
				int dataLength = ((remainingLength <= usableLength) ? remainingLength : usableLength);
				reportIsFull = (dataLength == usableLength);

				// Create the report, 1 byte is used by the report ID, an additional byte may be needed
				// for the terminating zero:
				int effectiveLength = (reportIsFull ? (dataLength + 1) : (dataLength + 1 + 1));

				// Always create a full report, some devices do not work otherwise:
				byte[] report = new byte[ReportLength];

				// Copy the report ID into the beginning of the report:
				report[0] = reportId;

				// Copy the data into the remaining space of the report:
				Array.Copy(data, accumulatedLength, report, 1, dataLength);

				// Add the terminating zero:
				if (!reportIsFull)
					report[effectiveLength - 1] = 0;

				// Add the report to the list:
				reports.Add(report);

				// Forward to next report:
				accumulatedLength += dataLength;
			}

			// According to the USB specifications, HID must add a terminating report in case the
			// last report was full:
			if ((reports.Count > 0) && reportIsFull)
			{
				byte[] emptyReport = new byte[2];
				emptyReport[0] = reportId;
				emptyReport[1] = 0;
				reports.Add(emptyReport);
			}

			// Return the reports, or <c>null</c> if there are no reports at all:
			this.reports = reports.AsReadOnly();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
