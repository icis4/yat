//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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
using System.Text;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

using MKY.Utilities.Diagnostics;

#endregion

namespace MKY.Utilities.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API for HID communications.
	/// </summary>
	/// <remarks>
	/// This class is partly based on GenericHid of Jan Axelson's Lakeview Research. Visit GenericHid
	/// on http://www.lvr.com/hidpage.htm for details.
	/// MKY.Utilities.Win32 needs to modify the structure and contents of GenericHid due to the
	/// following reasons:
	/// - Suboptimal structure of the original GenericHid project
	/// - Missing features required for YAT
	/// - Potential reuse of this class for other services directly using the Win32 API
	/// </remarks>
	public static class Hid
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
		// warnings for each undocumented member below. Documenting each member makes little sense
		// since they pretty much tell their purpose and documentation tags between the members
		// makes the code less readable.
		#pragma warning disable 1591

		/// <summary>
		/// String descriptor types.
		/// </summary>
		private enum StringDescriptorIndex
		{
			LanguageIds  = 0,
			Manufacturer = 1,
			Product      = 2,
			SerialNumber = 3,
		}

		/// <summary></summary>
		public enum Usage
		{
			Unknown  = 0,
			Keyboard = 0x0106,
			Mouse    = 0x0102,
		}

		/// <summary></summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HIDD_ATTRIBUTES
		{
			public Int32 Size;
			[CLSCompliant(false)]
			public UInt16 VendorID;
			[CLSCompliant(false)]
			public UInt16 ProductID;
			[CLSCompliant(false)]
			public UInt16 VersionNumber;
		}

		// HIDD_CONFIGURATION is reserved for internal system use

		/// <summary></summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HIDP_CAPS
		{
			public Int16 Usage;
			public Int16 UsagePage;
			public Int16 InputReportByteLength;
			public Int16 OutputReportByteLength;
			public Int16 FeatureReportByteLength;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
			public Int16[] Reserved;
			public Int16 NumberLinkCollectionNodes;
			public Int16 NumberInputButtonCaps;
			public Int16 NumberInputValueCaps;
			public Int16 NumberInputDataIndices;
			public Int16 NumberOutputButtonCaps;
			public Int16 NumberOutputValueCaps;
			public Int16 NumberOutputDataIndices;
			public Int16 NumberFeatureButtonCaps;
			public Int16 NumberFeatureValueCaps;
			public Int16 NumberFeatureDataIndices;
		}

		/// <summary>
		/// The HIDP_REPORT_TYPE enumeration type is used to specify a HID report type.
		/// </summary>
		public enum HIDP_REPORT_TYPE
		{
			HidP_Input   = 0,
			HidP_Output  = 1,
			HidP_Feature = 2,
		}

		/// <summary>
		/// The HIDP_REPORT_TYPE enumeration type is used to specify a HID report type.
		/// </summary>
		[CLSCompliant(false)]
		public enum HIDP_STATUS : uint
		{
			HidP_Success              = 0x00110000,
			HidP_InvalidPreparsedData = 0xC0110001,
		}

		/// <summary>
		/// If IsRange is false, UsageMin is the Usage and UsageMax is unused.
		/// If IsStringRange is false, StringMin is the String index and StringMax is unused.
		/// If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HIDP_VALUE_CAPS
		{
			public Int16 UsagePage;
			public Byte  ReportID;
			public Int32 IsAlias;
			public Int16 BitField;
			public Int16 LinkCollection;
			public Int16 LinkUsage;
			public Int16 LinkUsagePage;
			public Int32 IsRange;
			public Int32 IsStringRange;
			public Int32 IsDesignatorRange;
			public Int32 IsAbsolute;
			public Int32 HasNull;
			public Byte  Reserved;
			public Int16 BitSize;
			public Int16 ReportCount;
			public Int16 Reserved2;
			public Int16 Reserved3;
			public Int16 Reserved4;
			public Int16 Reserved5;
			public Int16 Reserved6;
			public Int32 LogicalMin;
			public Int32 LogicalMax;
			public Int32 PhysicalMin;
			public Int32 PhysicalMax;
			public Int16 UsageMin;
			public Int16 UsageMax;
			public Int16 StringMin;
			public Int16 StringMax;
			public Int16 DesignatorMin;
			public Int16 DesignatorMax;
			public Int16 DataIndexMin;
			public Int16 DataIndexMax;
		}

		#pragma warning restore 1591

		private delegate bool GetHidStringDelegate(IntPtr deviceHandle, out string hidString);
//		private delegate bool GetHidStringDelegate(SafeFileHandle deviceHandle, out string hidString);

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string HID_DLL = "hid.dll";

		#endregion

		#region External Functions
		//==========================================================================================
		// External Functions
		//==========================================================================================

		/// <summary>
		/// Removes any Input reports waiting in the buffer.
		/// </summary>
		/// <remarks>
		/// Public via <see cref="FlushQueue"/>.
		/// </remarks>
		/// <param name="HidDeviceObject">A handle to the device.</param>
		/// <returns>True on success, false on failure.</returns>
		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_FlushQueue(IntPtr HidDeviceObject);
//		private static extern Boolean HidD_FlushQueue(SafeFileHandle HidDeviceObject);

		/// <summary>
		/// Frees the buffer reserved by HidD_GetPreparsedData.
		/// </summary>
		/// <param name="PreparsedData">A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
		/// <returns>True on success, false on failure.</returns>
		[DllImport(HID_DLL, SetLastError = true)]
		public static extern Boolean HidD_FreePreparsedData(IntPtr PreparsedData);

		/// <summary></summary>
		[DllImport(HID_DLL, SetLastError = true)]
		public static extern Boolean HidD_GetAttributes(IntPtr HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);
//		public static extern Boolean HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

		// HidD_GetConfiguration() is reserved for internal system use

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_GetFeature(IntPtr HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
//		private static extern Boolean HidD_GetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
		/// <summary>
		/// Attempts to read a Feature report from the device.
		/// </summary>
		/// <param name="HidDeviceObject">A handle to an HID.</param>
		/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		public  static        Boolean HidD_GetFeature(IntPtr HidDeviceObject, Byte[] ReportBuffer)
//		public  static        Boolean HidD_GetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
		{
			return (HidD_GetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
		}

		/// <remarks>
		/// Public via <see cref="GetHidGuid()"/>.
		/// </remarks>
		[DllImport(HID_DLL, SetLastError = true)]
		private static extern void HidD_GetHidGuid(ref System.Guid HidGuid);

		[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean HidD_GetIndexedString(IntPtr HidDeviceObject, UInt32 StringIndex, StringBuilder Buffer, UInt32 BufferLength);
//		private static extern Boolean HidD_GetIndexedString(SafeFileHandle HidDeviceObject, UInt32 StringIndex, StringBuilder Buffer, UInt32 BufferLength);
		/// <summary></summary>
		public  static        Boolean HidD_GetIndexedString(IntPtr HidDeviceObject, int StringIndex, out string IndexedString)
//		public  static        Boolean HidD_GetIndexedString(SafeFileHandle HidDeviceObject, int StringIndex, out string IndexedString)
		{
			StringBuilder s = new StringBuilder(Utilities.Usb.Descriptors.MaximumStringDescriptorCharLength);
			if (HidD_GetIndexedString(HidDeviceObject, (UInt32)StringIndex, s, (UInt32)s.Capacity))
			{
				IndexedString = s.ToString();
				return (true);
			}
			IndexedString = "";
			return (false);
		}

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_GetInputReport(IntPtr HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
//		private static extern Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
		/// <summary>
		/// Attempts to read an Input report from the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Supported under Windows XP and later only. Also applies to <see cref="HidD_SetOutputReport(SafeFileHandle, Byte[])"/>.
		/// Public via <see cref="GetInputReport"/>.
		/// </remarks>
		/// <param name="HidDeviceObject">A handle to an HID.</param>
		/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		private static        Boolean HidD_GetInputReport(IntPtr HidDeviceObject, Byte[] ReportBuffer)
//		private static        Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
		{
			return (HidD_GetInputReport(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
		}

		[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean HidD_GetManufacturerString(IntPtr HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
//		private static extern Boolean HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
		/// <summary></summary>
		public  static        Boolean HidD_GetManufacturerString(IntPtr HidDeviceObject, out string Manufacturer)
//		public  static        Boolean HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, out string Manufacturer)
		{
			StringBuilder s = new StringBuilder(Utilities.Usb.Descriptors.MaximumStringDescriptorCharLength);
			if (HidD_GetManufacturerString(HidDeviceObject, s, (UInt32)s.Capacity))
			{
				Manufacturer = s.ToString();
				return (true);
			}
			Manufacturer = "";
			return (false);
		}

		// HidD_GetMsGenreDescriptor() is reserved for internal system use

		/// <summary>
		/// Retrieves the number of Input reports the host can store.
		/// </summary>
		/// <remarks>
		/// Not supported by Windows 98 Standard Edition.
		/// If the buffer is full and another report arrives, the host drops the oldest report.
		/// Public via <see cref="GetNumberOfInputBuffers"/>.
		/// </remarks>
		/// <param name="HidDeviceObject">A handle to a device and an integer to hold the number of buffers.</param>
		/// <param name="NumberBuffers">True on success, false on failure.</param>
		/// <returns></returns>
		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_GetNumInputBuffers(IntPtr HidDeviceObject, ref UInt32 NumberBuffers);
//		private static extern Boolean HidD_GetNumInputBuffers(SafeFileHandle HidDeviceObject, ref UInt32 NumberBuffers);

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_GetPhysicalDescriptor(IntPtr HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
//		private static extern Boolean HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
		/// <summary></summary>
		public  static        Boolean HidD_GetPhysicalDescriptor(IntPtr HidDeviceObject, Byte[] Buffer)
//		public  static        Boolean HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, Byte[] Buffer)
		{
			return (HidD_GetPhysicalDescriptor(HidDeviceObject, Buffer, (UInt32)Buffer.Length));
		}

		/// <summary>
		/// Retrieves a pointer to a buffer containing information about the device's capabilities.
		/// HidP_GetCaps and other API functions require a pointer to the buffer.
		/// </summary>
		/// <param name="HidDeviceObject">A handle returned by CreateFile.</param>
		/// <param name="PreparsedData">A pointer to a buffer.</param>
		/// <returns>True on success, false on failure.</returns>
		[DllImport(HID_DLL, SetLastError = true)]
		public static extern Boolean HidD_GetPreparsedData(IntPtr HidDeviceObject, ref IntPtr PreparsedData);
//		public static extern Boolean HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

		[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean HidD_GetProductString(IntPtr HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
//		private static extern Boolean HidD_GetProductString(SafeFileHandle HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
		/// <summary></summary>
		public  static        Boolean HidD_GetProductString(IntPtr HidDeviceObject, out string Product)
//		public  static        Boolean HidD_GetProductString(SafeFileHandle HidDeviceObject, out string Product)
		{
			StringBuilder s = new StringBuilder(Utilities.Usb.Descriptors.MaximumStringDescriptorCharLength);
			if (HidD_GetProductString(HidDeviceObject, s, (UInt32)s.Capacity))
			{
				Product = s.ToString();
				return (true);
			}
			Product = "";
			return (false);
		}

		[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean HidD_GetSerialNumberString(IntPtr HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
//		private static extern Boolean HidD_GetSerialNumberString(SafeFileHandle HidDeviceObject, StringBuilder Buffer, UInt32 BufferLength);
		/// <summary></summary>
		public  static        Boolean HidD_GetSerialNumberString(IntPtr HidDeviceObject, out string SerialNumber)
//		public  static        Boolean HidD_GetSerialNumberString(SafeFileHandle HidDeviceObject, out string SerialNumber)
		{
			StringBuilder s = new StringBuilder(Utilities.Usb.Descriptors.MaximumStringDescriptorCharLength);
			if (HidD_GetSerialNumberString(HidDeviceObject, s, (UInt32)s.Capacity))
			{
				SerialNumber = s.ToString();
				return (true);
			}
			SerialNumber = "";
			return (false);
		}

		// HidD_SetConfiguration() is reserved for internal system use

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_SetFeature(IntPtr HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
//		private static extern Boolean HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
		/// <summary>
		/// Attempts to send a Feature report to the device.
		/// </summary>
		/// <param name="HidDeviceObject">A handle to a HID.</param>
		/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		public  static        Boolean HidD_SetFeature(IntPtr HidDeviceObject, Byte[] ReportBuffer)
//		public  static        Boolean HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
		{
			return (HidD_SetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
		}

		/// <summary>
		/// Sets the number of Input reports the host can store.
		/// </summary>
		/// <remarks>
		/// If the buffer is full and another report arrives, the host drops the oldest report.
		/// Public via <see cref="SetNumberOfInputBuffers"/>.
		/// </remarks>
		/// <param name="HidDeviceObject">A handle to an HID.</param>
		/// <param name="NumberBuffers">An integer to hold the number of buffers.</param>
		/// <returns>True on success, false on failure.</returns>
		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_SetNumInputBuffers(IntPtr HidDeviceObject, UInt32 NumberBuffers);
//		private static extern Boolean HidD_SetNumInputBuffers(SafeFileHandle HidDeviceObject, UInt32 NumberBuffers);

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern Boolean HidD_SetOutputReport(IntPtr HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
//		private static extern Boolean HidD_SetOutputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
		/// <summary>
		/// Attempts to send an Output report to the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Requires Windows XP or later. Also applies to <see cref="HidD_GetInputReport(SafeFileHandle, Byte[])"/>.
		/// Public via <see cref="SetOutputReport"/>.
		/// </remarks>
		/// <param name="HidDeviceObject">A handle to an HID.</param>
		/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		private static        Boolean HidD_SetOutputReport(IntPtr HidDeviceObject, Byte[] ReportBuffer)
//		private static        Boolean HidD_SetOutputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
		{
			return (HidD_SetOutputReport(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
		}

		/// <summary>
		/// Find out a device's capabilities. For standard devices such as joysticks, you can find
		/// out the specific capabilities of the device. For a custom device where the software
		/// knows what the device is capable of, this call may be unneeded.
		/// </summary>
		/// <param name="PreparsedData">A pointer returned by HidD_GetPreparsedData.</param>
		/// <param name="Capabilities">A pointer to a HIDP_CAPS structure.</param>
		/// <returns>True on success, false on failure.</returns>
		[DllImport(HID_DLL, SetLastError = true)]
		public static extern Int32 HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

		[DllImport(HID_DLL, SetLastError = true)]
		private static extern HIDP_STATUS HidP_GetValueCaps(HIDP_REPORT_TYPE ReportType, ref HIDP_VALUE_CAPS ValueCaps, ref UInt32 ValueCapsLength, IntPtr PreparsedData);
		/// <summary>
		/// Retrieves a buffer containing an array of HidP_ValueCaps structures. Each structure
		/// defines the capabilities of one value. This application doesn't use this data.
		/// </summary>
		/// <param name="ReportType">A report type enumerator from hidpi.h.</param>
		/// <param name="ValueCaps">A pointer to a buffer for the returned array.</param>
		/// <param name="PreparsedData"> A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
		/// <returns>True on success, false on failure.</returns>
		[CLSCompliant(false)]
		public  static        HIDP_STATUS HidP_GetValueCaps(HIDP_REPORT_TYPE ReportType, ref HIDP_VALUE_CAPS ValueCaps, IntPtr PreparsedData)
		{
			UInt32 ValueCapsLength = (UInt32)Marshal.SizeOf(typeof(HIDP_VALUE_CAPS));
			return (HidP_GetValueCaps(ReportType, ref ValueCaps, ref ValueCapsLength, PreparsedData));
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Returns the GUID associated with USB HID.
		/// </summary>
		public static System.Guid GetHidGuid()
		{
			System.Guid hidGuid = new System.Guid();
			HidD_GetHidGuid(ref hidGuid);
			return (hidGuid);
		}

		/// <summary>
		/// Creates a device handle of the HID device at the given systemPath.
		/// </summary>
		public static bool CreateDeviceHandle(string systemPath, out IntPtr deviceHandle)
//		public static bool CreateDeviceHandle(string systemPath, out SafeFileHandle deviceHandle)
		{
			/*SafeFileHandle h = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				FileIO.Access.QUERY_ONLY,
				FileIO.ShareMode.SHARE_READ_WRITE,
				IntPtr.Zero,
				FileIO.CreationDisposition.OPEN_EXISTING,
				FileIO.AttributesAndFlags.NONE,
				IntPtr.Zero
				);*/

			IntPtr h = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				FileIO.Access.GENERIC_READ_WRITE,
				FileIO.ShareMode.SHARE_NONE,
				IntPtr.Zero,
				FileIO.CreationDisposition.OPEN_EXISTING,
				FileIO.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
				);

			if (h == Constants.InvalidHandle)
			{
				System.Diagnostics.Debug.WriteLine(Debug.GetLastError());
				deviceHandle = IntPtr.Zero;
				return (false);
			}

			if (h != IntPtr.Zero)
			{
				deviceHandle = h;
				return (true);
			}

			deviceHandle = IntPtr.Zero;
			return (false);

			/*if (!h.IsInvalid)
			{
				deviceHandle = h;
				return (true);
			}

			deviceHandle = null;
			return (false);*/
		}

		/*/// <summary>
		/// Creates a handle to read from the HID device at the given systemPath.
		/// </summary>
		public static bool CreateReadHandle(string systemPath, out SafeFileHandle readHandle)
		{
			SafeFileHandle h = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				FileIO.Access.GENERIC_READ,
				FileIO.ShareMode.SHARE_READ_WRITE,
				IntPtr.Zero,
				FileIO.CreationDisposition.OPEN_EXISTING,
				FileIO.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
				);

			if (!h.IsInvalid)
			{
				readHandle = h;
				return (true);
			}

			readHandle = null;
			return (false);
		}

		/// <summary>
		/// Creates a handle to write to the HID device at the given systemPath.
		/// </summary>
		public static bool CreateWriteHandle(string systemPath, out SafeFileHandle writeHandle)
		{
			SafeFileHandle h = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				FileIO.Access.GENERIC_WRITE,
				FileIO.ShareMode.SHARE_READ_WRITE,
				IntPtr.Zero,
				FileIO.CreationDisposition.OPEN_EXISTING,
				FileIO.AttributesAndFlags.NONE,
				IntPtr.Zero
				);

			if (!h.IsInvalid)
			{
				writeHandle = h;
				return (true);
			}

			writeHandle = null;
			return (false);
		}

		/// <summary>
		/// Creates a handle to read and write to the HID device at the given systemPath.
		/// </summary>
		public static bool CreateReadWriteHandle(string systemPath, out SafeFileHandle writeHandle)
		{
			SafeFileHandle h = Utilities.Win32.FileIO.CreateFile
				(
				systemPath,
				FileIO.Access.GENERIC_READ_WRITE,
				FileIO.ShareMode.SHARE_NONE,
				IntPtr.Zero,
				FileIO.CreationDisposition.OPEN_EXISTING,
				FileIO.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
				);

			if (!h.IsInvalid)
			{
				writeHandle = h;
				return (true);
			}

			writeHandle = null;
			return (false);
		}*/

		/// <summary>
		/// Retrieves a structure with information about a device's capabilities. 
		/// </summary>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <returns>An HIDP_CAPS structure.</returns>
		public static HIDP_CAPS GetDeviceCapabilities(IntPtr deviceHandle)
//		public static HIDP_CAPS GetDeviceCapabilities(SafeFileHandle deviceHandle)
		{
			HIDP_CAPS capabilities = new HIDP_CAPS();
			IntPtr preparsedData = new IntPtr();
			bool success = false;

			try
			{
				success = HidD_GetPreparsedData(deviceHandle, ref preparsedData);
				Int32 result = HidP_GetCaps(preparsedData, ref capabilities);
				if ((result != 0))
				{
					System.Diagnostics.Debug.WriteLine("USB device capabilities:");
					System.Diagnostics.Debug.Indent();
					System.Diagnostics.Debug.WriteLine("Usage (hex):                     " + capabilities.Usage.ToString("X2"));
					System.Diagnostics.Debug.WriteLine("Usage Page (hex):                " + capabilities.UsagePage.ToString("X2"));
					System.Diagnostics.Debug.WriteLine("Input Report Byte Length:        " + capabilities.InputReportByteLength);
					System.Diagnostics.Debug.WriteLine("Output Report Byte Length:       " + capabilities.OutputReportByteLength);
					System.Diagnostics.Debug.WriteLine("Feature Report Byte Length:      " + capabilities.FeatureReportByteLength);
					System.Diagnostics.Debug.WriteLine("Number of Link Collection Nodes: " + capabilities.NumberLinkCollectionNodes);
					System.Diagnostics.Debug.WriteLine("Number of Input Button Caps:     " + capabilities.NumberInputButtonCaps);
					System.Diagnostics.Debug.WriteLine("Number of Input Value Caps:      " + capabilities.NumberInputValueCaps);
					System.Diagnostics.Debug.WriteLine("Number of Input Data Indices:    " + capabilities.NumberInputDataIndices);
					System.Diagnostics.Debug.WriteLine("Number of Output Button Caps:    " + capabilities.NumberOutputButtonCaps);
					System.Diagnostics.Debug.WriteLine("Number of Output Value Caps:     " + capabilities.NumberOutputValueCaps);
					System.Diagnostics.Debug.WriteLine("Number of Output Data Indices:   " + capabilities.NumberOutputDataIndices);
					System.Diagnostics.Debug.WriteLine("Number of Feature Button Caps:   " + capabilities.NumberFeatureButtonCaps);
					System.Diagnostics.Debug.WriteLine("Number of Feature Value Caps:    " + capabilities.NumberFeatureValueCaps);
					System.Diagnostics.Debug.WriteLine("Number of Feature Data Indices:  " + capabilities.NumberFeatureDataIndices);
					System.Diagnostics.Debug.Unindent();

					// \remind 2010-03-21 / mky
					// The following two lines demonstrate how the devices value capabilities can be retrieved.
					// However, due to some reaseon HidP_GetValueCaps() overwrites 'deviceHandle'. Before
					// making use of the following lines, ensure that 'deviceHandle' isn't overwritten anymore.
					//
					//HIDP_VALUE_CAPS valueCaps = new HIDP_VALUE_CAPS();
					//HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, ref valueCaps, preparsedData);
				}
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
			finally
			{
				if (preparsedData != IntPtr.Zero)
				{
					success = HidD_FreePreparsedData(preparsedData);
					preparsedData = IntPtr.Zero;
				}
			}

			return (capabilities);
		}

		/// <summary>
		/// Creates a 32-bit Usage from the Usage Page and Usage ID. 
		/// Determines whether the Usage is a system mouse or keyboard.
		/// Can be modified to detect other Usages.
		/// </summary>
		/// <param name="capabilities">A HIDP_CAPS structure retrieved with HidP_GetCaps.</param>
		/// <returns>A String describing the usage.</returns>
		public static Usage GetHidUsage(HIDP_CAPS capabilities)
		{
			try
			{
				// For a complete list,.see http://www.usb.org/developers/devclass_docs/Hut1_12.pdf.
				switch (capabilities.UsagePage)
				{
					case 0x01: // Generic Desktop Page
					{
						switch (capabilities.Usage)
						{
							case 0x02: // Mouse
								return (Usage.Mouse);

							case 0x06: // Keyboard
								return (Usage.Keyboard);
						}
						break;
					}
				}
				return (Usage.Unknown);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		/// <summary></summary>
		public static bool GetManufacturerString(IntPtr deviceHandle, out string manufacturer)
//		public static bool GetManufacturerString(SafeFileHandle deviceHandle, out string manufacturer)
		{
			return (GetString(deviceHandle, HidD_GetManufacturerString, out manufacturer));
		}

		/// <summary></summary>
		public static bool GetProductString(IntPtr deviceHandle, out string product)
//		public static bool GetProductString(SafeFileHandle deviceHandle, out string product)
		{
			return (GetString(deviceHandle, HidD_GetProductString, out product));
		}

		/// <summary></summary>
		public static bool GetSerialNumberString(IntPtr deviceHandle, out string serialNumber)
//		public static bool GetSerialNumberString(SafeFileHandle deviceHandle, out string serialNumber)
		{
			return (GetString(deviceHandle, HidD_GetSerialNumberString, out serialNumber));
		}

		/// <summary>
		/// Gets one of the standard strings (manufacturer, product, serial number).
		/// </summary>
		/// <remarks>
		/// \fixme MKY 2010-03-14
		/// Don't know how to retrieve culture specific strings based on language ID. Simply return "".
		/// Seems like HID.dll doesn't support to retrieve culture specific strings. WinUSB.dll does,
		/// however, WinUSB.dll can only be used in combination with devices that provide a WinUSB.dll
		/// based driver.
		/// Considerations
		/// - How many languages are available? Retrieve language IDs at index 0.
		/// - How are the indecies mapped to the languages? Device descriptor returns indecies for the strings.
		/// - How can culture specific strings be accessed? There must be something like SetDescriptor()/GetDescriptor()
		///   that takes an index and a text ID as argument.
		/// </remarks>
		private static bool GetString(IntPtr deviceHandle, GetHidStringDelegate method, out string hidString)
//		private static bool GetString(SafeFileHandle deviceHandle, GetHidStringDelegate method, out string hidString)
		{
//			if (!deviceHandle.IsInvalid)
			if (deviceHandle != IntPtr.Zero)
			{
				try
				{
					// Retrieve language IDs at index 0
					string languageString;
					if (HidD_GetIndexedString(deviceHandle, (int)StringDescriptorIndex.LanguageIds, out languageString))
					{
						// Retrieve invariant string
						string invariantString;
						if (method(deviceHandle, out invariantString)) // GetManufacturerString() or GetProductString() or GetSerialNumberString()
						{
							if (invariantString != languageString) // Looks like a proper invariant string
							{
								hidString = invariantString;
								return (true);
							}
							else // invariantString == languageString means that invariant string not really contains useful data
							{
								// Retrieve culture specific strings

								// 
								//
								// Q & A
								//  Index 0 

								/*
								CultureInfo[] l = Usb.Descriptors.GetCultureInfoFromLanguageString(languageString);
								Dictionary<CultureInfo, string> d = GetCultureSpecificStrings(deviceHandle, index, l);
								CultureInfo ci = Globalization.XCultureInfo.GetMostAppropriateCultureInfo(d.Keys);
								if ((ci != null) && (d.ContainsKey(ci)))
								{
									hidString = d[ci];
									return (true);
								}
								*/

								string cultureSpecificString = "";
								hidString = cultureSpecificString;
								return (true);
							}
						}
					}
				}
				catch (Exception ex)
				{
					XDebug.WriteException(typeof(Hid), ex);
				}
			}
			hidString = "";
			return (false);
		}

		/// <summary>
		/// Retrieves the number of Input reports the host can store.
		/// </summary>
		/// <remarks>
		/// Windows 98 Standard Edition does not support the following:
		/// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
		/// - HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		/// </remarks>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <param name="numberOfInputBuffers">An integer to hold the returned value.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool GetNumberOfInputBuffers(IntPtr deviceHandle, out int numberOfInputBuffers)
//		public static bool GetNumberOfInputBuffers(SafeFileHandle deviceHandle, out int numberOfInputBuffers)
		{

			try
			{
				bool success = false;
				if (!Version.IsWindows98Standard())
				{
					UInt32 numberBuffers = 0;
					success = HidD_GetNumInputBuffers(deviceHandle, ref numberBuffers);
					numberOfInputBuffers = (int)numberBuffers;
				}
				else
				{
					// Under Windows 98 Standard Edition, the number of buffers is fixed at 2.
					numberOfInputBuffers = 2;
					success = true;
				}
				return (success);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		/// <summary>
		/// Sets the number of input reports the host will store.
		/// </summary>
		/// <remarks>
		/// Windows 98 Standard Edition does not support the following:
		/// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
		/// - HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		/// </remarks>
		/// <param name="deviceHandle">A handle to the device.</param>
		/// <param name="numberOfInputBuffers">The requested number of input reports.</param>
		/// <returns>True on success. False on failure.</returns>
		public static bool SetNumberOfInputBuffers(IntPtr deviceHandle, int numberOfInputBuffers)
//		public static bool SetNumberOfInputBuffers(SafeFileHandle deviceHandle, int numberOfInputBuffers)
		{
			try
			{
				if (!Version.IsWindows98Standard())
					return (HidD_SetNumInputBuffers(deviceHandle, (UInt32)numberOfInputBuffers));
				else
					return (false); // Not supported under Windows 98 Standard Edition.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		/// <summary>
		/// Attempts to read an Input report from the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Supported under Windows XP and later only. Also applies to <see cref="SetOutputReport"/>.
		/// Public via <see cref="GetInputReport"/>.
		/// </remarks>
		/// <param name="deviceHandle">A handle to an HID.</param>
		/// <param name="reportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool GetInputReport(IntPtr deviceHandle, byte[] reportBuffer)
//		public static bool GetInputReport(SafeFileHandle deviceHandle, byte[] reportBuffer)
		{
			try
			{
				if (!Version.IsWindowsXpOrLater())
					return (HidD_GetInputReport(deviceHandle, reportBuffer));
				else
					return (false); // Not supported before Windows XP.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		/// <summary>
		/// Attempts to send an Output report to the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Requires Windows XP or later. Also applies to <see cref="GetInputReport"/>.
		/// Public via <see cref="SetOutputReport"/>.
		/// </remarks>
		/// <param name="deviceHandle">A handle to an HID.</param>
		/// <param name="reportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool SetOutputReport(IntPtr deviceHandle, byte[] reportBuffer)
//		public static bool SetOutputReport(SafeFileHandle deviceHandle, byte[] reportBuffer)
		{
			try
			{
				if (!Version.IsWindowsXpOrLater())
					return (HidD_SetOutputReport(deviceHandle, reportBuffer));
				else
					return (false); // Not supported before Windows XP.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		/// <summary>
		/// Remove any input reports waiting in the buffer.
		/// </summary>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <returns> True on success, false on failure.</returns>
		public static bool FlushQueue(IntPtr deviceHandle)
//		public static bool FlushQueue(SafeFileHandle deviceHandle)
		{
			try
			{
				bool success = HidD_FlushQueue(deviceHandle);
				return (success);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Hid), ex);
				throw;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
