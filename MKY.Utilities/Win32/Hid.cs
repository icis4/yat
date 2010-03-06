//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Serial/Enums.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
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
using System.IO;
using System.Text;
using System.Diagnostics;
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
    /// - Potential reuse of this class for other services relying on the Win32 API
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

        #endregion

        #region Constants
        //==========================================================================================
        // Constants
        //==========================================================================================

        private const string HID_DLL = "hid.dll";

        /// <summary>
        /// Maximum of 126 characters in UCS-2 format.
        /// </summary>
        public const int MaximumStringDescriptorCharLength = 126;

        /// <summary>
        /// 2 x 126 characters + 2 x '\0' results in something like 256.
        /// </summary>
        public const int MaximumStringDescriptorByteLength = 256;

        #endregion

        #region External Function Declaration
        //==========================================================================================
        // External Function Declaration
        //==========================================================================================

        /// <summary>
        /// Removes any Input reports waiting in the buffer.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to the device.</param>
        /// <returns>True on success, false on failure.</returns>
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern Boolean HidD_FlushQueue(SafeFileHandle HidDeviceObject);

        /// <summary>
        /// Frees the buffer reserved by HidD_GetPreparsedData.
        /// </summary>
        /// <param name="PreparsedData">A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
        /// <returns>True on success, false on failure.</returns>
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern Boolean HidD_FreePreparsedData(IntPtr PreparsedData);

        /// <summary></summary>
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern Boolean HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        // HidD_GetConfiguration() is reserved for internal system use

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
        /// <summary>
        /// Attempts to read a Feature report from the device.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to an HID.</param>
        /// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <returns>True on success, false on failure.</returns>
        public  static        Boolean HidD_GetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
        {
            return (HidD_GetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
        }

        /// <summary></summary>
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern void HidD_GetHidGuid(ref System.Guid HidGuid);

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetIndexedString(SafeFileHandle HidDeviceObject, UInt32 StringIndex, Byte[] Buffer, UInt32 BufferLength);
        /// <summary></summary>
        [CLSCompliant(false)]
        public  static        Boolean HidD_GetIndexedString(SafeFileHandle HidDeviceObject, UInt32 StringIndex, Byte[] Buffer)
        {
            return (HidD_GetIndexedString(HidDeviceObject, StringIndex, Buffer, (UInt32)Buffer.Length));
        }

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
        /// <summary>
        /// Attempts to read an Input report from the device using a control transfer.
        /// </summary>
        /// <remarks>
        /// Supported under Windows XP and later only.
        /// </remarks>
        /// <param name="HidDeviceObject">A handle to an HID.</param>
        /// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <returns>True on success, false on failure.</returns>
        public  static        Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
        {
            return (HidD_GetInputReport(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
        }

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
        /// <summary></summary>
        public  static        Boolean HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, out string Manufacturer)
        {
            Manufacturer = "";
            Byte[] buffer = new Byte[MaximumStringDescriptorByteLength];
            if (HidD_GetManufacturerString(HidDeviceObject, buffer, (UInt32)buffer.Length))
            {
                if (DecodeStringDescriptor(buffer, out Manufacturer))
                    return (true);
            }
            return (false);
        }

        // HidD_GetMsGenreDescriptor() is reserved for internal system use

        /// <summary>
        /// Retrieves the number of Input reports the host can store.
        /// </summary>
        /// <remarks>
        /// Not supported by Windows 98 Standard Edition.
        /// If the buffer is full and another report arrives, the host drops the oldest report.
        /// </remarks>
        /// <param name="HidDeviceObject">A handle to a device and an integer to hold the number of buffers.</param>
        /// <param name="NumberBuffers">True on success, false on failure.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern Boolean HidD_GetNumInputBuffers(SafeFileHandle HidDeviceObject, ref UInt32 NumberBuffers);

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
        /// <summary></summary>
        public  static        Boolean HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, Byte[] Buffer)
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
        public static extern Boolean HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetProductString(SafeFileHandle HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
        /// <summary></summary>
        public  static        Boolean HidD_GetProductString(SafeFileHandle HidDeviceObject, out string Product)
        {
            Product = "";
            Byte[] buffer = new Byte[MaximumStringDescriptorByteLength];
            if (HidD_GetProductString(HidDeviceObject, buffer, (UInt32)buffer.Length))
            {
                if (DecodeStringDescriptor(buffer, out Product))
                    return (true);
            }
            return (false);
        }

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_GetSerialNumberString(SafeFileHandle HidDeviceObject, Byte[] Buffer, UInt32 BufferLength);
        /// <summary></summary>
        public  static        Boolean HidD_GetSerialNumberString(SafeFileHandle HidDeviceObject, out string SerialNumber)
        {
            SerialNumber = "";
            Byte[] buffer = new Byte[MaximumStringDescriptorByteLength];
            if (HidD_GetSerialNumberString(HidDeviceObject, buffer, (UInt32)buffer.Length))
            {
                if (DecodeStringDescriptor(buffer, out SerialNumber))
                    return (true);
            }
            return (false);
        }

        // HidD_SetConfiguration() is reserved for internal system use

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
        /// <summary>
        /// Attempts to send a Feature report to the device.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to a HID.</param>
        /// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <returns>True on success, false on failure.</returns>
        public  static        Boolean HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
        {
            return (HidD_SetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
        }

        /// <summary>
        /// Sets the number of Input reports the host can store.
        /// </summary>
        /// <remarks>
        /// If the buffer is full and another report arrives, the host drops the oldest report.
        /// </remarks>
        /// <param name="HidDeviceObject">A handle to an HID.</param>
        /// <param name="NumberBuffers">An integer to hold the number of buffers.</param>
        /// <returns>True on success, false on failure.</returns>
        [CLSCompliant(false)]
        [DllImport(HID_DLL, SetLastError = true)]
        public static extern Boolean HidD_SetNumInputBuffers(SafeFileHandle HidDeviceObject, UInt32 NumberBuffers);

        [DllImport(HID_DLL, SetLastError = true)]
        private static extern Boolean HidD_SetOutputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer, UInt32 ReportBufferLength);
        /// <summary>
        /// Attempts to send an Output report to the device using a control transfer.
        /// </summary>
        /// <remarks>
        /// Requires Windows XP or later.
        /// </remarks>
        /// <param name="HidDeviceObject">A handle to an HID.</param>
        /// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <returns>True on success, false on failure.</returns>
        public  static        Boolean HidD_SetOutputReport(SafeFileHandle HidDeviceObject, Byte[] ReportBuffer)
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

        #region Methods
        //==========================================================================================
        // Methods
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
        /// Remove any input reports waiting in the buffer.
        /// </summary>
        /// <param name="hidHandle">A handle to a device.</param>
        /// <returns> True on success, false on failure.</returns>
        public static bool FlushQueue(SafeFileHandle hidHandle)
        {
            try
            {
                bool success = HidD_FlushQueue(hidHandle);
                return (success);
            }
            catch (Exception ex)
            {
                XDebug.WriteException(typeof(Hid), ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a structure with information about a device's capabilities. 
        /// </summary>
        /// <param name="hidHandle">A handle to a device.</param>
        /// <returns>An HIDP_CAPS structure.</returns>
        public static HIDP_CAPS GetDeviceCapabilities(SafeFileHandle hidHandle)
        {
            HIDP_CAPS capabilities = new HIDP_CAPS();
            IntPtr preparsedData = new IntPtr();
            bool success = false;

            try
            {
                success = HidD_GetPreparsedData(hidHandle, ref preparsedData);
                Int32 result = HidP_GetCaps(preparsedData, ref capabilities);
                if ((result != 0))
                {
                    Debug.WriteLine("");
                    Debug.WriteLine("  Usage: " + Convert.ToString(capabilities.Usage, 16));
                    Debug.WriteLine("  Usage Page: " + Convert.ToString(capabilities.UsagePage, 16));
                    Debug.WriteLine("  Input Report Byte Length: " + capabilities.InputReportByteLength);
                    Debug.WriteLine("  Output Report Byte Length: " + capabilities.OutputReportByteLength);
                    Debug.WriteLine("  Feature Report Byte Length: " + capabilities.FeatureReportByteLength);
                    Debug.WriteLine("  Number of Link Collection Nodes: " + capabilities.NumberLinkCollectionNodes);
                    Debug.WriteLine("  Number of Input Button Caps: " + capabilities.NumberInputButtonCaps);
                    Debug.WriteLine("  Number of Input Value Caps: " + capabilities.NumberInputValueCaps);
                    Debug.WriteLine("  Number of Input Data Indices: " + capabilities.NumberInputDataIndices);
                    Debug.WriteLine("  Number of Output Button Caps: " + capabilities.NumberOutputButtonCaps);
                    Debug.WriteLine("  Number of Output Value Caps: " + capabilities.NumberOutputValueCaps);
                    Debug.WriteLine("  Number of Output Data Indices: " + capabilities.NumberOutputDataIndices);
                    Debug.WriteLine("  Number of Feature Button Caps: " + capabilities.NumberFeatureButtonCaps);
                    Debug.WriteLine("  Number of Feature Value Caps: " + capabilities.NumberFeatureValueCaps);
                    Debug.WriteLine("  Number of Feature Data Indices: " + capabilities.NumberFeatureDataIndices);

                    HIDP_VALUE_CAPS valueCaps = new HIDP_VALUE_CAPS();
                    HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, ref valueCaps, preparsedData);
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
                    success = HidD_FreePreparsedData(preparsedData);
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
        public static string GetHidUsage(HIDP_CAPS capabilities)
        {
            try
            {
                string usageDescription = "";

                //  Create32-bit Usage from Usage Page and Usage ID.
                Int32 usage = capabilities.UsagePage * 256 + capabilities.Usage;

                if (usage == Convert.ToInt32(0x102))
                    usageDescription = "Mouse";

                if (usage == Convert.ToInt32(0x106))
                    usageDescription = "Keyboard";

                return (usageDescription);
            }
            catch (Exception ex)
            {
                XDebug.WriteException(typeof(Hid), ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the number of Input reports the host can store.
        /// </summary>
        /// <param name="hidDeviceObject">A handle to a device.</param>
        /// <param name="numberOfInputBuffers">An integer to hold the returned value.</param>
        /// <returns>True on success, false on failure.</returns>
        [CLSCompliant(false)]
        public static bool GetNumberOfInputBuffers(SafeFileHandle hidDeviceObject, ref UInt32 numberOfInputBuffers)
        {

            try
            {
                bool success = false;
                if (!((IsWindows98SE())))
                {
                    success = HidD_GetNumInputBuffers(hidDeviceObject, ref numberOfInputBuffers);
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
        /// Requires Windows XP or later.
        /// </remarks>
        /// <param name="hidDeviceObject">A handle to the device.</param>
        /// <param name="numberBuffers">The requested number of input reports.</param>
        /// <returns>True on success. False on failure.</returns>
        [CLSCompliant(false)]
        public static bool SetNumberOfInputBuffers(SafeFileHandle hidDeviceObject, UInt32 numberBuffers)
        {
            try
            {
                if (!IsWindows98SE())
                {
                    HidD_SetNumInputBuffers(hidDeviceObject, numberBuffers);
                    return (true);
                }
                else
                {
                    //  Not supported under Windows 98 Standard Edition.
                    return (false);
                }
            }
            catch (Exception ex)
            {
                XDebug.WriteException(typeof(Hid), ex);
                throw;
            }
        }

        /// <summary>
        /// Find out if the current operating system is Windows XP or later.
        /// </summary>
        /// <remarks>
        /// Windows XP or later is required for HidD_GetInputReport and HidD_SetInputReport.
        /// </remarks>
        private static bool IsWindowsXpOrLater()
        {
            try
            {
                OperatingSystem environment = Environment.OSVersion;

                //  Windows XP is version 5.1.
                System.Version versionXP = new System.Version(5, 1);

                if (environment.Version >= versionXP)
                {
                    Debug.Write("The OS is Windows XP or later.");
                    return (true);
                }
                else
                {
                    Debug.Write("The OS is earlier than Windows XP.");
                    return (false);
                }
            }
            catch (Exception ex)
            {
                XDebug.WriteException(typeof(Hid), ex);
                throw;
            }
        }

        /// <summary>
        /// Find out if the current operating system is Windows 98 Standard Edition.
        /// </summary>
        /// <remarks>
        /// Windows 98 Standard Edition does not support the following:
        /// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
        /// - HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
        /// (Not yet tested on a Windows 98 Standard Edition system.)
        /// </remarks>
        private static bool IsWindows98SE()
        {
            try
            {
                OperatingSystem environment = Environment.OSVersion;

                //  Windows 98 Standard Edition is version 4.10 with a build number less than 2183.
                System.Version version98SE = new System.Version(4, 10, 2183);

                if (environment.Version < version98SE)
                {
                    Debug.Write("The OS is Windows 98 Standard Edition.");
                    return (true);
                }
                else
                {
                    Debug.Write("The OS is more recent than Windows 98 Standard Edition.");
                    return (false);
                }
            }
            catch (Exception ex)
            {
                XDebug.WriteException(typeof(Hid), ex);
                throw;
            }
        }

        private static bool DecodeStringDescriptor(byte[] buffer, out string decoded)
        {
            decoded = "";
            Decoder d = UnicodeEncoding.Unicode.GetDecoder();
            int charCount = d.GetCharCount(buffer, 0, buffer.Length, true);
            char[] chars = new char[charCount];
            if (d.GetChars(buffer, 0, buffer.Length, chars, 0, true) > 0)
            {
                int code = (int)chars[0];
                if (code != 0x0409) // 0x0409 = 1033 represents 'unknown'
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in chars)
                    {
                        if (c == 0)
                            break;

                        sb.Append(c);
                    }
                    decoded = sb.ToString();
                }
            }
            return (decoded != "");
        }

        #endregion
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Usb/UsbDeviceCollection.cs $
//==================================================================================================
