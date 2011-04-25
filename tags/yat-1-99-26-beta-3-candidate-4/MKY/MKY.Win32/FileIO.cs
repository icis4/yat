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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API relating to file I/O.
	/// </summary>
	public static class FileIO
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > Types
		//------------------------------------------------------------------------------------------
		// Native > Types
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		public static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			/// <summary>
			/// Encapsulates Win32 GENERIC_ file access flags into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Type given by Win32.")]
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by Win32.")]
			[Flags]
			[CLSCompliant(false)]
			public enum Access : uint
			{
				GENERIC_READ       = 0x80000000,
				GENERIC_WRITE      = 0x40000000,
				GENERIC_EXECUTE    = 0x20000000,
				GENERIC_ALL        = 0x10000000,

				GENERIC_READ_WRITE = 0xC0000000,

				QUERY_ONLY         = 0x00000000,
			}

			/// <summary>
			/// Encapsulates Win32 FILE_SHARE_ file share mode flags into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Type given by Win32.")]
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by Win32.")]
			[Flags]
			[CLSCompliant(false)]
			public enum ShareMode : uint
			{
				SHARE_NONE       = 0x00000000,
				SHARE_READ       = 0x00000001,
				SHARE_WRITE      = 0x00000002,
				SHARE_DELETE     = 0x00000004,

				SHARE_READ_WRITE = 0x00000003,
				SHARE_ALL        = 0x00000007,
			}

			/// <summary>
			/// Replicates Win32 creation disposition selectors into a C# enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by Win32.")]
			public enum CreationDisposition
			{
				CREATE_NEW        = System.IO.FileMode.CreateNew,
				CREATE_ALWAYS     = System.IO.FileMode.Create,
				OPEN_EXISTING     = System.IO.FileMode.Open,
				OPEN_ALWAYS       = System.IO.FileMode.OpenOrCreate,
				TRUNCATE_EXISTING = System.IO.FileMode.Truncate,
				APPEND            = System.IO.FileMode.Append,
			}

			/// <summary>
			/// Encapsulates Win32 FILE_ATTRIBUTE_ and FILE_FLAG_ values into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Type given by Win32.")]
			[Flags]
			[CLSCompliant(false)]
			public enum AttributesAndFlags : uint
			{
				NONE                          = System.IO.FileOptions.None,

				ATTRIBUTE_READONLY            = System.IO.FileAttributes.ReadOnly,
				ATTRIBUTE_HIDDEN              = System.IO.FileAttributes.Hidden,
				ATTRIBUTE_SYSTEM              = System.IO.FileAttributes.System,
				ATTRIBUTE_DIRECTORY           = System.IO.FileAttributes.Directory,
				ATTRIBUTE_ARCHIVE             = System.IO.FileAttributes.Archive,
				ATTRIBUTE_DEVICE              = System.IO.FileAttributes.Device,
				ATTRIBUTE_NORMAL              = System.IO.FileAttributes.Normal,
				ATTRIBUTE_TEMPORARY           = System.IO.FileAttributes.Temporary,
				ATTRIBUTE_SPARSE_FILE         = System.IO.FileAttributes.SparseFile,
				ATTRIBUTE_REPARSE_POINT       = System.IO.FileAttributes.ReparsePoint,
				ATTRIBUTE_COMPRESSED          = System.IO.FileAttributes.Compressed,
				ATTRIBUTE_OFFLINE             = System.IO.FileAttributes.Offline,
				ATTRIBUTE_NOT_CONTENT_INDEXED = System.IO.FileAttributes.NotContentIndexed,
				ATTRIBUTE_ENCRYPTED           = System.IO.FileAttributes.Encrypted,
				ATTRIBUTE_VIRTUAL             = 0x00010000,

				FLAG_WRITE_THROUGH            = 0x80000000,
				FLAG_OVERLAPPED               = 0x40000000,
				FLAG_NO_BUFFERING             = 0x20000000,
				FLAG_RANDOM_ACCESS            = System.IO.FileOptions.RandomAccess,
				FLAG_SEQUENTIAL_SCAN          = System.IO.FileOptions.SequentialScan,
				FLAG_DELETE_ON_CLOSE          = System.IO.FileOptions.DeleteOnClose,
				FLAG_BACKUP_SEMANTICS         = 0x02000000,
				FLAG_POSIX_SEMANTICS          = 0x01000000,
				FLAG_OPEN_REPARSE_POINT       = 0x00200000,
				FLAG_OPEN_NO_RECALL           = 0x00100000,
				FLAG_FIRST_PIPE_INSTANCE      = 0x00080000,
			}

			/// <summary></summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct SECURITY_ATTRIBUTES
			{
				public Int32 nLength;
				public Int32 lpSecurityDescriptor;
				public Int32 bInheritHandle;
			}

			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct BY_HANDLE_FILE_INFORMATION
			{
				public UInt32 FileAttributes;
				public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
				public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
				public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
				public UInt32 VolumeSerialNumber;
				public UInt32 FileSizeHigh;
				public UInt32 FileSizeLow;
				public UInt32 NumberOfLinks;
				public UInt32 FileIndexHigh;
				public UInt32 FileIndexLow;
			}

			#pragma warning restore 1591
		}
		
		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		public static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			/// <summary>
			/// Cancels a call to ReadFile.
			/// </summary>
			/// <param name="hFile">The device handle.</param>
			/// <returns>True on success, false on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool CancelIo([In] SafeFileHandle hFile);

			/// <summary>
			/// Creates an event object for the overlapped structure used with ReadFile.
			/// </summary>
			/// <param name="SecurityAttributes">A security attributes structure or IntPtr.Zero.</param>
			/// <param name="bManualReset">Manual Reset = False (The system automatically resets the
			/// state to nonsignaled after a waiting thread has been released.).</param>
			/// <param name="bInitialState">Initial state = False (Not signaled.)</param>
			/// <param name="lpName">An event object name (optional).</param>
			/// <returns>A handle to the event object.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr CreateEvent([In] IntPtr SecurityAttributes, [In] bool bManualReset, [In] bool bInitialState, [In] string lpName);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern SafeFileHandle CreateFile([In] string lpFileName, [In] NativeTypes.Access dwDesiredAccess, [In] NativeTypes.ShareMode dwShareMode, [In] IntPtr lpSecurityAttributes, [In] NativeTypes.CreationDisposition dwCreationDisposition, [In] NativeTypes.AttributesAndFlags dwFlagsAndAttributes, [In] IntPtr hTemplateFile);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool GetFileInformationByHandle([In] SafeFileHandle hFile, [Out] out NativeTypes.BY_HANDLE_FILE_INFORMATION lpFileInformation);

			/// <summary>
			/// Gets the result of an overlapped operation.
			/// </summary>
			/// <param name="hFile">A device handle returned by CreateFile.</param>
			/// <param name="lpOverlapped">A pointer to an overlapped structure.</param>
			/// <param name="lpNumberOfBytesTransferred">A pointer to a variable to hold the number of bytes read.</param>
			/// <param name="bWait">False to return immediately.</param>
			/// <returns>Non-zero on success and the number of bytes read.</returns>
			public static bool GetOverlappedResult(SafeFileHandle hFile, IntPtr lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait)
			{
				UInt32 bytesTransferred;
				bool success = GetOverlappedResult(hFile, lpOverlapped, out bytesTransferred, bWait);
				lpNumberOfBytesTransferred = (int)bytesTransferred;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			private static extern bool GetOverlappedResult([In] SafeFileHandle hFile, [In] IntPtr lpOverlapped, [Out] out UInt32 lpNumberOfBytesTransferred, [In] bool bWait);

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
			/// (for overlapped I/O, CreateFile must have been called with FILE_FLAG_OVERLAPPED).</param>
			/// <param name="lpBuffer">A pointer to a buffer for storing the report.</param>
			/// <param name="nNumberOfBytesToRead">The Input report length in bytes returned by HidP_GetCaps.</param>
			/// <param name="lpNumberOfBytesRead">A pointer to a variable that will hold the number of bytes read.</param>
			/// <param name="lpOverlapped">An overlapped structure whose hEvent member is set to an event object.</param>
			/// <returns>The report in ReadBuffer.</returns>
			public static bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, ref NativeOverlapped lpOverlapped)
			{
				UInt32 bytesRead;
				bool success = ReadFile(hFile, lpBuffer, (UInt32)nNumberOfBytesToRead, out bytesRead, ref lpOverlapped);
				lpNumberOfBytesRead = (int)bytesRead;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			private static extern bool ReadFile([In] SafeFileHandle hFile, [Out] IntPtr lpBuffer, [In] UInt32 nNumberOfBytesToRead, [Out] out UInt32 lpNumberOfBytesRead, [In] ref NativeOverlapped lpOverlapped);

			/// <summary>
			/// Waits for at least one report or a timeout.
			/// Used with overlapped ReadFile.
			/// </summary>
			/// <param name="hHandle">An event object created with CreateEvent.</param>
			/// <param name="dwMilliseconds">A timeout value in milliseconds.</param>
			/// <returns>A result code.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern UInt32 WaitForSingleObject([In] IntPtr hHandle, [In] UInt32 dwMilliseconds);

			/// <summary>
			/// Writes an Output report to the device.
			/// </summary>
			/// <param name="hFile">A handle returned by CreateFile.</param>
			/// <param name="lpBuffer">A pointer to a buffer containing the report.</param>
			/// <param name="lpNumberOfBytesWritten">An integer to hold the number of bytes written.</param>
			/// <param name="lpOverlapped">An overlapped structure whose hEvent member is set to an event object.</param>
			/// <returns>True on success, false on failure.</returns>
			public static bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, out int lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped)
			{
				UInt32 bytesWritten;
				bool success = WriteFile(hFile, lpBuffer, (UInt32)lpBuffer.Length, out bytesWritten, ref lpOverlapped);
				lpNumberOfBytesWritten = (int)bytesWritten;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			private static extern bool WriteFile([In] SafeFileHandle hFile, [In] byte[] lpBuffer, [In] UInt32 nNumberOfBytesToWrite, [Out] out UInt32 lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
