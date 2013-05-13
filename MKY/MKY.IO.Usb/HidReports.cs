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
// MKY Development Version 1.0.10
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
		public void CreateDataFromReport(byte[] report, bool reportIdIsPrepended)
		{
			CreateDataFromReport(report, reportIdIsPrepended, false);
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report, bool reportIdIsPrepended, bool reportSizeIsPrepended)
		{
			// Ensure that report length matches:
			if (report.Length < Device.InputReportLength)
				throw (new ArgumentException("Length of input report exceeds the device's capabilities", "report"));

			// Calculate actual report header length:
			int headerLength = 0;
			if (reportIdIsPrepended)
				headerLength++;
			if (reportSizeIsPrepended)
				headerLength++;

			// If requested, get report ID which is located in the first byte:
			if (reportIdIsPrepended)
				SetReportId(report[0]);

			// Get report data, starting at the end of the report header:
			List<byte> data = new List<byte>();
			for (int i = headerLength; i < report.Length; i++)
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
			CreateReportsFromData(true, reportId, false, data);
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		public void CreateReportsFromData(byte reportId, bool prependReportSize, byte[] data)
		{
			CreateReportsFromData(true, reportId, prependReportSize, data);
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		private void CreateReportsFromData(bool prependReportId, byte reportId, bool prependReportSize, byte[] data)
		{
			// Calculate required header length:
			int headerLength = 0;
			if (prependReportId)
				headerLength++;
			if (prependReportSize)
				headerLength++;

			// Calculate usable length:
			int usableLength = ReportLength - headerLength;

			// Create the temporary report container:
			List<byte[]> reports = new List<byte[]>();
			bool reportIsFull = false;

			// Create the reports and accumulate the length:
			int accumulatedLength = 0;
			while (accumulatedLength < (data.Length))
			{
				// Evaluate the required report length:
				int remainingLength = data.Length - accumulatedLength;
				int payloadLength = ((remainingLength <= usableLength) ? remainingLength : usableLength);
				reportIsFull = (payloadLength == usableLength);

				// Create the report, one or two bytes may be used by the report header,
				// an additional byte may be needed for the terminating zero:
				int effectiveLength = (reportIsFull ? (headerLength + payloadLength) : (headerLength + payloadLength + 1));

				// Always create a full report, some devices do not work otherwise:
				byte[] report = new byte[ReportLength];

				// If requested, copy the report ID into the first byte of the report:
				if (prependReportId)
				{
					report[0] = reportId;
				}

				// If requested, copy the report size into the first or second byte of the report:
				if (prependReportSize)
				{
					if (!prependReportId)
						report[0] = (byte)ReportLength; // Simply cast, a potentially thrown
					else                                //   'OverflowException' is intended since
						report[1] = (byte)ReportLength; //   that means there is a severe mismatch.
				}

				// Copy the data into the remaining space of the report:
				Array.Copy(data, accumulatedLength, report, headerLength, payloadLength);

				// Add the terminating zero:
				if (!reportIsFull)
					report[effectiveLength - 1] = 0;

				// Add the report to the list:
				reports.Add(report);

				// Forward to next report:
				accumulatedLength += payloadLength;
			}

			// According to the USB specifications, a terminating report must be added in case the
			// last report was full. However, this only applies to data that is sent from device to
			// host. In case of data that is sent from host to device this doesn't apply, due to the
			// asymmetrical nature of USB.

			// Return the reports, or <c>null</c> if there are no reports at all:
			this.reports = reports.AsReadOnly();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
