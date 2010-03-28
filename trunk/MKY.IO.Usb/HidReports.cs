//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
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
//using System.Diagnostics;
//using System.Runtime.InteropServices;
//using System.Threading;
//using Microsoft.Win32.SafeHandles;

//using MKY.Utilities.Diagnostics;
//using MKY.Utilities.Win32;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Abstract base for all HID report containers.
	/// </summary>
	public abstract class HidReportContainer
	{
		private HidDevice _device;
		private int _reportLength;

		/// <summary>
		/// Creates a report container and stores the reference to the device in use as well as
		/// the length of a report.
		/// </summary>
		public HidReportContainer(HidDevice device, int reportLength)
		{
			_device = device;
			_reportLength = reportLength;
		}

		/// <summary>
		/// The length of a report. The length is given by the device capabilities.
		/// </summary>
		public int ReportLength
		{
			get { return (_reportLength); }
		}
	}

	/// <summary>
	/// An HID input report container is a simple wrapper around a byte buffer.
	/// </summary>
	public class HidInputReportContainer : HidReportContainer
	{
		private byte[] _data;

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
			get { return (_data); }
		}

		/// <summary>
		/// Create the data from a given input report.
		/// </summary>
		public void CreateDataFromReport(byte[] report)
		{
			List<byte> data = new List<byte>();

			// Skip first byte, it always contains 0
			for (int i = 1; i < report.Length; i++)
			{
				if (report[i] != 0x00)
					data.Add(report[i]);
				else
					break;
			}

			_data = data.ToArray();
		}
	}

	/// <summary>
	/// An HID output report container puts any amount of data into chunks that match the size of
	/// the device's reported output report capabilities.
	/// </summary>
	public class HidOutputReportContainer : HidReportContainer
	{
		private byte[][] _reports;

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
			get { return (_reports); }
		}

		/// <summary>
		/// Create the report(s) from given data. The data is splitted into chunks that match the
		/// size of the device's reported output report capabilities.
		/// </summary>
		public void CreateReportsFromData(byte[] data)
		{
			List<byte[]> reports = new List<byte[]>();

			int usableLength = ReportLength - 1;
			int offset = 0;
			while (offset < (data.Length))
			{
				// Create report and set first byte to 0.
				byte[] report = new byte[ReportLength];
				report[0] = 0x00;

				// Copy as much as possible into remaining usable bytes of the report.
				int lengthToCopy = (data.Length - offset) % usableLength;
				Array.Copy(data, offset, report, 1, lengthToCopy);
				reports.Add(report);

				offset += usableLength;
			}

			_reports = reports.ToArray();
		}
	}

	/*
	#region Input Reports
	//==============================================================================================
	// Input Reports
	//==============================================================================================

	#region Input Reports > IInputReport
	//----------------------------------------------------------------------------------------------
	// Input Reports > IInputReport
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For reports the device sends to the host.
	/// </summary>
	interface IInputReport
	{
		/// <summary>
		/// Each class that handles reading reports defines a read method for reading a type
		/// of report.
		/// </summary>
		bool Read(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref bool deviceDetected, ref Byte[] readBuffer);
	}

	#endregion

	#region Input Reports > InputFeatureReport
	//----------------------------------------------------------------------------------------------
	// Input Reports > InputFeatureReport
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For reading feature reports.
	/// </summary>
	public class InputFeatureReport : IInputReport
	{
		/// <summary>
		/// Reads a feature report from the device.
		/// </summary>
		/// <param name="hidHandle">The handle for learning about the device and exchanging feature reports.</param>
		/// <param name="readHandle">The handle for reading input reports from the device.</param>
		/// <param name="writeHandle">The handle for writing output reports to the device.</param>
		/// <param name="deviceDetected">Tells whether the device is currently attached.</param>
		/// <param name="inFeatureReportBuffer">contains the requested report.</param>
		public virtual bool Read(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref bool deviceDetected, ref Byte[] inFeatureReportBuffer)
		{
			try
			{
				bool success = Hid.HidD_GetFeature(hidHandle, inFeatureReportBuffer);
				System.Diagnostics.Debug.Print("HidD_GetFeature success = " + success);
				return (success);
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("Hid.HidD_GetFeature", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}
	}

	#endregion

	#region Input Reports > InputReportViaControlTransfer
	//----------------------------------------------------------------------------------------------
	// Input Reports > InputReportViaControlTransfer
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For reading input reports via control transfers.
	/// </summary>
	public class InputReportViaControlTransfer : IInputReport
	{
		/// <summary>
		/// Reads an Input report from the device using a control transfer.
		/// </summary>
		/// <param name="hidHandle">The handle for learning about the device and exchanging feature reports.</param>
		/// <param name="readHandle">The handle for reading input reports from the device.</param>
		/// <param name="writeHandle">The handle for writing output reports to the device.</param>
		/// <param name="deviceDetected">Tells whether the device is currently attached.</param>
		/// <param name="inputReportBuffer">contains the requested report.</param>
		public virtual bool Read(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref bool deviceDetected, ref Byte[] inputReportBuffer)
		{
			bool success = Hid.GetInputReport(hidHandle, inputReportBuffer);
			System.Diagnostics.Debug.Print("HidD_GetInputReport success = " + success);
			return (success);
		}
	}

	#endregion

	#region Input Reports > InputReportViaInterruptTransfer
	//----------------------------------------------------------------------------------------------
	// Input Reports > InputReportViaInterruptTransfer
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For reading input reports.
	/// </summary>
	public class InputReportViaInterruptTransfer : IInputReport
	{
		/// <summary>
		/// Closes open handles to a device.
		/// </summary>
		/// <param name="hidHandle">The handle for learning about the device and exchanging feature reports.</param>
		/// <param name="readHandle">The handle for reading input reports from the device.</param>
		/// <param name="writeHandle">The handle for writing output reports to the device.</param>
		/// <param name="eventObject"></param>
		public virtual void CancelTransfer(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, IntPtr eventObject)
		{
			try
			{
				FileIO.CancelIo(readHandle);

				System.Diagnostics.Debug.WriteLine("Transfer cancelled:");
				System.Diagnostics.Debug.Indent();
				System.Diagnostics.Debug.WriteLine(Utilities.Win32.Debug.GetLastErrorMessage());
				System.Diagnostics.Debug.Unindent();

				// The failure may have been because the device was removed, so close any open
				// handles and set deviceDetected=False to cause the application to look for the
				// device on the next attempt.

				if ((!(hidHandle.IsInvalid)))
					hidHandle.Close();

				if ((!(readHandle.IsInvalid)))
					readHandle.Close();

				if ((!(writeHandle.IsInvalid)))
					writeHandle.Close();
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("FileIO.CancelIo", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}

		/// <summary>
		/// Creates an event object for the overlapped structure used with ReadFile. 
		/// </summary>
		/// <param name="hidOverlapped">The overlapped structure.</param>
		/// <param name="eventObject">The event object.</param>
		public virtual void PrepareForOverlappedTransfer(ref NativeOverlapped hidOverlapped, ref IntPtr eventObject)
		{
			try
			{
				eventObject = FileIO.CreateEvent(IntPtr.Zero, false, false, "");

				// Set the members of the overlapped structure.
				hidOverlapped.OffsetLow = 0;
				hidOverlapped.OffsetHigh = 0;
				hidOverlapped.EventHandle = eventObject;
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("FileIO.CreateEvent", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}

		/// <summary>
		/// Reads an input report from the device using interrupt transfers.
		/// </summary>
		/// <param name="hidHandle">The handle for learning about the device and exchanging feature reports.</param>
		/// <param name="readHandle">The handle for reading input reports from the device.</param>
		/// <param name="writeHandle">The handle for writing output reports to the device.</param>
		/// <param name="deviceDetected">Tells whether the device is currently attached.</param>
		/// <param name="inputReportBuffer">contains the requested report.</param>
		public virtual bool Read(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref bool deviceDetected, ref Byte[] inputReportBuffer)
		{
			IntPtr eventObject = IntPtr.Zero;
			NativeOverlapped HidOverlapped = new NativeOverlapped();
			IntPtr nonManagedBuffer = IntPtr.Zero;
			IntPtr nonManagedOverlapped = IntPtr.Zero;
			int numberOfBytesRead = 0;
			UInt32 result = 0;

			try
			{
				// Set up the overlapped structure for ReadFile.
				PrepareForOverlappedTransfer(ref HidOverlapped, ref eventObject);

				// Allocate memory for the input buffer and overlapped structure. 
				nonManagedBuffer = Marshal.AllocHGlobal(inputReportBuffer.Length);
				nonManagedOverlapped = Marshal.AllocHGlobal(Marshal.SizeOf(HidOverlapped));
				Marshal.StructureToPtr(HidOverlapped, nonManagedOverlapped, false);

				bool success = FileIO.ReadFile(readHandle, nonManagedBuffer, inputReportBuffer.Length, out numberOfBytesRead, nonManagedOverlapped);
				if (!success)
				{
					System.Diagnostics.Debug.WriteLine("Waiting for ReadFile.");
					result = FileIO.WaitForSingleObject(eventObject, 3000);

					// Find out if ReadFile completed or timeout.
					switch (result)
					{
						case FileIO.WAIT_OBJECT_0:
						{
							// ReadFile has completed
							success = true;
							System.Diagnostics.Debug.WriteLine("ReadFile completed successfully.");

							// Get the number of bytes read.
							FileIO.GetOverlappedResult(readHandle, nonManagedOverlapped, out numberOfBytesRead, false);

							break;
						}
						case FileIO.WAIT_TIMEOUT:
						{
							// Cancel the operation on timeout
							CancelTransfer(hidHandle, readHandle, writeHandle, eventObject);
							System.Diagnostics.Debug.WriteLine("ReadFile timeout.");
							success = false;
							deviceDetected = false;
							break;
						}
						default:
						{
							// Cancel the operation on other error.
							CancelTransfer(hidHandle, readHandle, writeHandle, eventObject);
							System.Diagnostics.Debug.WriteLine("ReadFile undefined error.");
							success = false;
							deviceDetected = false;
							break;
						}
					}

				}

				if (success)
				{
					// A report was received.
					// Copy the received data to inputReportBuffer for the application to use.
					Marshal.Copy(nonManagedBuffer, inputReportBuffer, 0, (int)numberOfBytesRead);
				}

				return (success);
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("FileIO.ReadFile", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}
	}

	#endregion

	#endregion

	#region Output Reports
	//==============================================================================================
	// Output Reports
	//==============================================================================================

	#region Output Reports > IOutputReport
	//----------------------------------------------------------------------------------------------
	// Output Reports > IOutputReport
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For reports the host sends to the device.
	/// </summary>
	interface IOutputReport
	{
		/// <summary>
		/// Each class that handles writing reports defines a write method for writing a type of report.
		/// </summary>
		/// <param name="reportBuffer">Contains the report ID and report data.</param>
		/// <param name="deviceHandle">Handle to the device.</param>
		/// <returns>True on success. False on failure.</returns>
		bool Write(Byte[] reportBuffer, SafeFileHandle deviceHandle);
	}

	#endregion

	#region Output Reports > OutputFeatureReport
	//----------------------------------------------------------------------------------------------
	// Output Reports > OutputFeatureReport
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For feature reports the host sends to the device.
	/// </summary>
	public class OutputFeatureReport : IOutputReport
	{
		/// <summary>
		/// Writes a feature report to the device.
		/// </summary>
		/// <param name="outFeatureReportBuffer">Contains the report ID and report data.</param>
		/// <param name="hidHandle">Handle to the device.</param>
		/// <returns>True on success. False on failure.</returns>
		public virtual bool Write(Byte[] outFeatureReportBuffer, SafeFileHandle hidHandle)
		{
			try
			{
				bool success = Hid.HidD_SetFeature(hidHandle, outFeatureReportBuffer);
				System.Diagnostics.Debug.Print("HidD_SetFeature success = " + success);
				return (success);
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("Hid.HidD_SetFeature", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}
	}

	#endregion

	#region Output Reports > OutputReportViaControlTransfer
	//----------------------------------------------------------------------------------------------
	// Output Reports > OutputReportViaControlTransfer
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For writing output reports via control transfers.
	/// </summary>
	public class OutputReportViaControlTransfer : IOutputReport
	{
		/// <summary>
		/// Writes an output report to the device using a control transfer.
		/// </summary>
		/// <param name="outputReportBuffer">Contains the report ID and report data.</param>
		/// <param name="hidHandle">Handle to the device.</param>
		/// <returns>True on success. False on failure.</returns>
		public virtual bool Write(Byte[] outputReportBuffer, SafeFileHandle hidHandle)
		{
			bool success = Hid.SetOutputReport(hidHandle, outputReportBuffer);
			System.Diagnostics.Debug.Print("HidD_SetOutputReport success = " + success);
			return (success);
		}
	}

	#endregion

	#region Output Reports > OutputReportViaInterruptTransfer
	//----------------------------------------------------------------------------------------------
	// Output Reports > OutputReportViaInterruptTransfer
	//----------------------------------------------------------------------------------------------

	/// <summary>
	/// For Output reports the host sends to the device.
	/// </summary>
	/// <remarks>
	/// Uses interrupt or control transfers depending on the device and OS.
	/// </remarks>
	public class OutputReportViaInterruptTransfer : IOutputReport
	{
		/// <summary>
		/// Writes an output report to the device.
		/// </summary>
		/// <remarks>
		/// The host will use an interrupt transfer if the HID has an interrupt OUT endpoint
		/// (requires USB 1.1 or later) AND the OS is NOT Windows 98 Standard Edition. Otherwise
		/// the host will use a control transfer. The application doesn't have to know or care
		/// which type of transfer is used.
		/// </remarks>
		/// <param name="outputReportBuffer">Contains the report ID and report data.</param>
		/// <param name="writeHandle">Handle to the device.</param>
		/// <returns>True on success. False on failure.</returns>
		public virtual bool Write(Byte[] outputReportBuffer, SafeFileHandle writeHandle)
		{
			try
			{
				int numberOfBytesWritten = 0;
				bool success = FileIO.WriteFile(writeHandle, outputReportBuffer, out numberOfBytesWritten, IntPtr.Zero);

				System.Diagnostics.Debug.Print("WriteFile success = " + success);

				if (!((success)))
				{
					if ((!(writeHandle.IsInvalid)))
						writeHandle.Close();
				}
				return (success);
			}
			catch (Exception nativeEx)
			{
				NativeMethodCallUsbException ex = new NativeMethodCallUsbException("FileIO.WriteFile", nativeEx.Message);
				XDebug.WriteException(this, ex);
				throw (ex);
			}
		}
	}

	#endregion

	#endregion
	*/
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
