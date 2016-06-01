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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
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
		/// Sets the ID of a report. The meaning of the ID is given by the report specification.
		/// </summary>
		protected void SetId(byte id)
		{
			this.id = id;
		}

		/// <summary>
		/// Returns the payload that was received via input reports.
		/// </summary>
		public byte[] Payload
		{
			get { return (this.payload); }
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void ProcessReport(SerialHidReportFormat format, byte[] report)
		{
			// Ensure that report length fits:
			if (report.Length > MaxByteLength)
				throw (new ArgumentException("Length of input report exceeds the device's capabilities.", "report"));

			// If requested, get the ID which is located in the first byte of the report:
			if (format.UseId)
				SetId(report[0]);

			// Get report data:
			List<byte> data = new List<byte>(report.Length); // Preset the initial capactiy to improve memory management.
			if (format.PrependPayloadByteLength)
			{
				// Get the payload by the length which is located in the first or second byte of the report:
				int payloadLength;
				if (!format.UseId)
					payloadLength = report[0];
				else
					payloadLength = report[1];

				for (int i = format.HeaderByteLength; i < (format.HeaderByteLength + payloadLength); i++)
				{
					if (i >= report.Length)
						throw (new ArgumentException("The reported payload length exceeds the length of the report."));

					data.Add(report[i]);
				}
			}
			else if (format.AppendTerminatingZero || format.FillLastReport)
			{
				// Simply read until the terminating zero or the end of the report:
				for (int i = format.HeaderByteLength; i < report.Length; i++)
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
				for (int i = format.HeaderByteLength; i < report.Length; i++)
					data.Add(report[i]);
			}

			this.payload = data.ToArray();
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
			int usableLength = MaxByteLength - format.HeaderByteLength;

			// Create the temporary report container:
			List<byte[]> reports = new List<byte[]>();

			// Create the reports and accumulate the length:
			int accumulatedPayloadLength = 0;
			while (accumulatedPayloadLength < (payload.Length))
			{
				// Evaluate the required lengths:
				int remainingPayloadLength = payload.Length - accumulatedPayloadLength;
				int payloadLength = ((remainingPayloadLength <= usableLength) ? remainingPayloadLength : usableLength);

				// Create the report, one or two bytes may be used by the report header,
				// an additional byte may be needed for the terminating zero:
				int effectiveLength = format.HeaderByteLength + payloadLength;
				if (format.AppendTerminatingZero)
					effectiveLength += 1;

			////// Code if Windows HID.dll didn't require that outgoing reports are always filled:
			////byte[] report;
			////// If requested, create a full report, many systems don't work otherwise:
			////if (format.FillLastReport)
			////	report = new byte[MaxByteLength];    // C# value-type arrays are initialized to 0.
			////else
			////	report = new byte[effectiveLength];  // C# value-type arrays are initialized to 0.

				// Windows HID.dll requires that outgoing reports are always filled!
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

			// Return the reports, or <c>null</c> if there are no reports at all:
			this.reports = reports.AsReadOnly();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
