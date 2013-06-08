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
	/// Abstract base for Ser/HID report containers.
	/// </summary>
	public abstract class SerialHidReportContainer : HidReportContainer
	{
		private byte id;

		/// <summary>
		/// Creates a report container and stores the reference to the device in use as well as
		/// the maximum length of a report.
		/// </summary>
		protected SerialHidReportContainer(SerialHidDevice device, int maxLength)
			: base(device, maxLength)
		{
		}

		/// <summary>
		/// The ID of a report. The meaning of the ID is given by the report specification.
		/// </summary>
		public byte Id
		{
			get { return (this.id); }
		}

		/// <summary>
		/// Sets the ID of a report. The meaning of the ID is given by the report specification.
		/// </summary>
		protected void SetId(byte id)
		{
			this.id = id;
		}
	}

	/// <summary>
	/// An HID input report container is a simple wrapper around a byte buffer.
	/// </summary>
	public class SerialHidInputReportContainer : SerialHidReportContainer
	{
		private ReadOnlyCollection<byte> data;

		/// <summary>
		/// Creates an input report container.
		/// </summary>
		public SerialHidInputReportContainer(SerialHidDevice device)
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
		public void CreateDataFromReport(byte[] report, bool idIsPrepended)
		{
			CreateDataFromReport(report, idIsPrepended, false, true, true); // \todo: This is required for MT Ser/HID!
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report, bool idIsPrepended, bool payloadLengthIsPrepended)
		{
			CreateDataFromReport(report, idIsPrepended, payloadLengthIsPrepended, false, false); // \todo: This is required for TI MSP430 Ser/HID!
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report, bool idIsPrepended, bool payloadLengthIsPrepended, bool terminatingZeroIsAppended, bool alwaysSendCompleteReports)
		{
			UnusedArg.PreventAnalysisWarning(alwaysSendCompleteReports); // Might be needed in the future.

			// Ensure that report length fits:
			if (report.Length > MaxLength)
				throw (new ArgumentException("Length of input report exceeds the device's capabilities", "report"));

			// Calculate actual report header length:
			int headerLength = 0;
			if (idIsPrepended)
				headerLength++;
			if (payloadLengthIsPrepended)
				headerLength++;

			// If requested, get the ID which is located in the first byte of the report:
			if (idIsPrepended)
				SetId(report[0]);

			// Get report data:
			List<byte> data = new List<byte>();
			if (payloadLengthIsPrepended)
			{
				// Get the payload by the length which is located in the first or second byte of the report:
				int payloadLength;
				if (!idIsPrepended)
					payloadLength = report[0];
				else
					payloadLength = report[1];

				for (int i = headerLength; i < (headerLength + payloadLength); i++)
				{
					if (i >= report.Length)
						throw (new ArgumentException("The reported payload length exceeds the length of the report"));

					data.Add(report[i]);
				}
			}
			else if (terminatingZeroIsAppended)
			{
				// Simply read until the terminating zero or the end of the report:
				for (int i = headerLength; i < report.Length; i++)
				{
					if (report[i] != 0x00)
						data.Add(report[i]);
					else
						break;
				}
			}
			else
			{
				// In any other case, read until the end of the report:
				for (int i = headerLength; i < report.Length; i++)
					data.Add(report[i]);
			}

			this.data = data.AsReadOnly();
		}
	}

	/// <summary>
	/// An HID output report container puts any amount of data into chunks that match the size of
	/// the device's reported output report capabilities.
	/// </summary>
	public class SerialHidOutputReportContainer : SerialHidReportContainer
	{
		private ReadOnlyCollection<byte[]> reports;

		/// <summary>
		/// Creates an output report container.
		/// </summary>
		public SerialHidOutputReportContainer(SerialHidDevice device)
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
		public void CreateReportsFromData(byte id, byte[] data)
		{
			CreateReportsFromData(true, id, false, data, true, true); // \todo: This is required for MT Ser/HID!
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		public void CreateReportsFromData(byte id, bool prependPayloadLength, byte[] data)
		{
			CreateReportsFromData(true, id, prependPayloadLength, data, false, false); // \todo: This is required for TI MSP430 Ser/HID!
		}

		/// <summary>
		/// Create the report(s) from given data. The data is split into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		private void CreateReportsFromData(bool prependId, byte id, bool prependPayloadLength, byte[] data, bool appendTerminatingZero, bool alwaysSendCompleteReports)
		{
			// Calculate required header length:
			int headerLength = 0;
			if (prependId)
				headerLength++;
			if (prependPayloadLength)
				headerLength++;

			// Calculate usable length:
			int usableLength = MaxLength - headerLength;

			// Create the temporary report container:
			List<byte[]> reports = new List<byte[]>();
			bool reportIsFull = false;

			// Create the reports and accumulate the length:
			int accumulatedLength = 0;
			while (accumulatedLength < (data.Length))
			{
				// Evaluate the required lengths:
				int remainingLength = data.Length - accumulatedLength;
				int payloadLength = ((remainingLength <= usableLength) ? remainingLength : usableLength);
				reportIsFull = (payloadLength == usableLength);

				// Create the report, one or two bytes may be used by the report header,
				// an additional byte may be needed for the terminating zero:
				int effectiveLength;
				if (appendTerminatingZero)
					effectiveLength = (reportIsFull ? (headerLength + payloadLength) : (headerLength + payloadLength + 1));
				else
					effectiveLength = (headerLength + payloadLength);

				// If requested, create a full report, some devices do not work otherwise:
				byte[] report;
				if (alwaysSendCompleteReports)
					report = new byte[MaxLength];       // C# value-type arrays are initialized to 0.
				else
					report = new byte[effectiveLength]; // C# value-type arrays are initialized to 0.

				// If requested, copy the ID into the first byte of the report:
				if (prependId)
				{
					report[0] = id;
				}

				// If requested, copy the payload length into the first or second byte of the report:
				if (prependPayloadLength)
				{
					if (!prependId)
						report[0] = (byte)payloadLength; // Simply cast, a potentially thrown
					else                                 //   'OverflowException' is intended since
						report[1] = (byte)payloadLength; //   that means there is a severe mismatch.
				}

				// Copy the data into the remaining space of the report:
				Array.Copy(data, accumulatedLength, report, headerLength, payloadLength);

				// There is no need to add the terminating zero if requested, the report array has
				// already been initialized with zeros upon creation.

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
