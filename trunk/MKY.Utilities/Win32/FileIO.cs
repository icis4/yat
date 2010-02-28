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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#endregion

namespace MKY.Utilities.Win32
{
    /// <summary>
    /// Encapsulates parts of the Win32 API relating to file I/O.
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
    public static class FileIO  
    {
        #region Types
        //==========================================================================================
        // Types
        //==========================================================================================

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public Int32 nLength;
            public Int32 lpSecurityDescriptor;
            public Int32 bInheritHandle;
        }

        #endregion

        #region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

        private const Int32 FILE_FLAG_OVERLAPPED = 0x40000000;
        private const Int32 FILE_SHARE_READ = 1;
        private const Int32 FILE_SHARE_WRITE = 2;
        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const Int32 INVALID_HANDLE_VALUE = -1;
        private const Int32 OPEN_EXISTING = 3;
        internal const Int32 WAIT_TIMEOUT = 0x102;
        internal const Int32 WAIT_OBJECT_0 = 0;         
    
        #endregion

        #region External Function Declaration
        //==========================================================================================
        // External Function Declaration
        //==========================================================================================

        /// <summary>
        /// Cancels a call to ReadFile.
        /// </summary>
        /// <param name="hFile">The device handle.</param>
        /// <returns>True on success, false on failure.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
		internal static extern Int32 CancelIo(SafeFileHandle hFile);

        /// <summary>
        /// Creates an event object for the overlapped structure used with ReadFile.
        /// </summary>
        /// <param name="SecurityAttributes">A security attributes structure or IntPtr.Zero.</param>
        /// <param name="bManualReset">Manual Reset = False (The system automatically resets the
        /// state to nonsignaled after a waiting thread has been released.)</param>
        /// <param name="bInitialState">Initial state = False (Not signaled.)</param>
        /// <param name="lpName">An event object name (optional).</param>
        /// <returns>A handle to the event object.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CreateEvent(IntPtr SecurityAttributes, Boolean bManualReset, Boolean bInitialState, String lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);

        /// <summary>
        /// Gets the result of an overlapped operation.
        /// </summary>
        /// <param name="hFile">A device handle returned by CreateFile.</param>
        /// <param name="lpOverlapped">A pointer to an overlapped structure.</param>
        /// <param name="lpNumberOfBytesTransferred">A pointer to a variable to hold the number of bytes read.</param>
        /// <param name="bWait">False to return immediately.</param>
        /// <returns>Non-zero on success and the number of bytes read.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern Boolean GetOverlappedResult(SafeFileHandle hFile, IntPtr lpOverlapped, ref Int32 lpNumberOfBytesTransferred, Boolean bWait);

        /// <summary>
        /// Attempts to read an Input report from the device.
        /// </summary>
        /// <remarks>
        /// The overlapped call returns immediately, even if the data hasn't been received yet.
        /// To read multiple reports with one ReadFile, increase the size of ReadBuffer and use
        /// NumberOfBytesRead to determine how many reports were returned. Use a larger buffer
        /// if the application can't keep up with reading each report individually. 
        /// </remarks>
        /// <param name="hFile">A device handle returned by CreateFile
        /// (for overlapped I/O, CreateFile must have been called with FILE_FLAG_OVERLAPPED)</param>
        /// <param name="lpBuffer">A pointer to a buffer for storing the report.</param>
        /// <param name="nNumberOfBytesToRead">The Input report length in bytes returned by HidP_GetCaps.</param>
        /// <param name="lpNumberOfBytesRead">A pointer to a variable that will hold the number of bytes read.</param>
        /// <param name="lpOverlapped">An overlapped structure whose hEvent member is set to an event object.</param>
        /// <returns>The report in ReadBuffer.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, Int32 nNumberOfBytesToRead, ref Int32 lpNumberOfBytesRead, IntPtr lpOverlapped);

        /// <summary>
        /// Waits for at least one report or a timeout.
        /// Used with overlapped ReadFile.
        /// </summary>
        /// <param name="hHandle">An event object created with CreateEvent.</param>
        /// <param name="dwMilliseconds">A timeout value in milliseconds.</param>
        /// <returns>A result code.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Int32 WaitForSingleObject(IntPtr hHandle, Int32 dwMilliseconds);

        /// <summary>
        /// Writes an Output report to the device.
        /// </summary>
        /// <param name="hFile">A handle returned by CreateFile.</param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten">An integer to hold the number of bytes written.</param>
        /// <param name="lpOverlapped"></param>
        /// <returns>True on success, false on failure.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean WriteFile(SafeFileHandle hFile, Byte[] lpBuffer, Int32 nNumberOfBytesToWrite, ref Int32 lpNumberOfBytesWritten, IntPtr lpOverlapped);        

        #endregion
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Usb/UsbDeviceCollection.cs $
//==================================================================================================
