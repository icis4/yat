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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// An HID input report container is a simple wrapper around a byte buffer.
	/// </summary>
	public class SerialHidInputReportContainer : HidReportContainer
	{
		private byte id;

		/// <remarks>
		/// "Guidelines for Collections": "Do use byte arrays instead of collections of bytes."
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private byte[] payload;

		/// <summary>
		/// Creates an input report container.
		/// </summary>
		public SerialHidInputReportContainer(SerialHidDevice device)
			: base(device, device.InputReportByteLength)
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
		/// Returns the payload that was received via the input report.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] Payload
		{
			get { return (this.payload); }
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		/// <exception cref="ArgumentException"> if either input report or payload in invalid.</exception>
		public void ProcessReport(SerialHidReportFormat format, byte[] report)
		{
			// Ensure that report length fits:
			if (report.Length > MaxByteLength)
				throw (new ArgumentException("Length of input report exceeds the device's capabilities!", "report")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			// If requested, get the ID which is located in the first byte of the report:
			if (format.UseId)
				this.id = report[0];

			// Get report data:
			List<byte> payload = new List<byte>(report.Length); // Preset the required capacity to improve memory management.
			if (format.PrependPayloadByteLength)
			{
				// Get the payload by the length which is located in the first or second byte of the report:
				byte payloadByteLength;
				if (!format.UseId)
					payloadByteLength = report[0];
				else
					payloadByteLength = report[1];

				for (int i = format.HeaderByteLength; i < (format.HeaderByteLength + payloadByteLength); i++)
				{
					if (i >= report.Length)
						throw (new ArgumentException("The reported payload length exceeds the length of the report!")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

					payload.Add(report[i]);
				}
			}
			else if (format.AppendTerminatingZero)
			{
				// Read until the terminating zero:
				for (int i = format.HeaderByteLength; i < report.Length; i++)
				{
					if (report[i] != 0x00)
						payload.Add(report[i]);
					else
						break;
				}
			}
			else
			{
				// In any other case, read until the end of the report:
				for (int i = format.HeaderByteLength; i < report.Length; i++)
					payload.Add(report[i]);

				// Note that this is the case if neither length is prepended nor terminating zero
				// is appended. In such case, there is no way to distinguish 0x00 payload bytes from
				// 0x00 filler bytes, thus consider the whole report as payload.
			}

			this.payload = payload.ToArray();
		}
	}

	/// <summary>
	/// An HID output report container puts any amount of data into chunks that match the size of
	/// the device's reported output report capabilities.
	/// </summary>
	public class SerialHidOutputReportContainer : HidReportContainer
	{
		private ReadOnlyCollection<byte[]> reports;

		/// <summary>
		/// Creates an output report container.
		/// </summary>
		public SerialHidOutputReportContainer(SerialHidDevice device)
			: base(device, device.OutputReportByteLength)
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
		public void CreateReports(SerialHidReportFormat format, byte[] payload)
		{
			// Calculate usable length:
			int usableLength = (MaxByteLength - format.HeaderByteLength);

			// Create the temporary report container:
			List<byte[]> reports = new List<byte[]>();

			// Create the reports and accumulate the length:
			int accumulatedPayloadLength = 0;
			while (accumulatedPayloadLength < (payload.Length))
			{
				// Evaluate the required lengths:
				int remainingPayloadLength = (payload.Length - accumulatedPayloadLength);
				int payloadLength = ((remainingPayloadLength <= usableLength) ? (remainingPayloadLength) : (usableLength));

				// Create the report, one or two bytes may be used by the report header,
				// an additional byte may be needed for the terminating zero:
				int effectiveLength = (format.HeaderByteLength + payloadLength);
				if (format.AppendTerminatingZero)
					effectiveLength += 1;

			////// Code if Windows HID.dll didn't require that output reports are always filled:
			////byte[] report;
			////// If requested, create a full report, many systems don't work otherwise:
			////if (format.FillLastReport)
			////	report = new byte[MaxByteLength];    // C# value-type arrays are initialized to 0.
			////else
			////	report = new byte[effectiveLength];  // C# value-type arrays are initialized to 0.
			////// Also see "\!-Doc\SerHID Profile Description.txt" for USB HID constraints.

				// Windows HID.dll requires that output reports are always filled!
				byte[] report = new byte[MaxByteLength]; // C# value-type arrays are initialized to 0.

				// If requested, copy the ID into the first byte of the report:
				if (format.UseId)
					report[0] = format.Id;

				// If requested, copy the payload length into the first or second byte of the report:
				if (format.PrependPayloadByteLength)
				{
					if (!format.UseId)
						report[0] = (byte)payloadLength; // Simply cast, a potentially thrown
					else                                 //   'OverflowException' is intended since
						report[1] = (byte)payloadLength; //   that means there is a severe mismatch.
				}

				// Copy the payload data into the remaining space of the report:
				Array.Copy(payload, accumulatedPayloadLength, report, format.HeaderByteLength, payloadLength);

				// There is no need to add the terminating zero if requested, the report array has
				// already been initialized with zeros upon creation.

				// Add the report to the list:
				reports.Add(report);

				// Forward to next report:
				accumulatedPayloadLength += payloadLength;
			}

			// According to the USB specifications, a terminating report must be added in case the
			// last report was full. However, this only applies to data that is sent from device to
			// host. In case of data that is sent from host to device this doesn't apply, due to the
			// asymmetrical nature of USB.

			// Return the reports; or <c>null</c> if there are no reports at all:
			this.reports = reports.AsReadOnly();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
