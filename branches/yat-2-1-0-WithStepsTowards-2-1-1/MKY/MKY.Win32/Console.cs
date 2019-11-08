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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32.SafeHandles;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 debugging API.
	/// </summary>
	public static class Console
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
		private static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			public enum STD_HANDLE : uint
			{
				STD_INPUT_HANDLE  = 0xFFFFFFF6,
				STD_OUTPUT_HANDLE = 0xFFFFFFF5,
				STD_ERROR_HANDLE  = 0xFFFFFFF4
			}

			#pragma warning restore 1591
		}

		#endregion

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
		private static class NativeConstants
		{
			public const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
		}

		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
		private static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			/// <summary></summary>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
			public static bool AttachConsoleToProcess(int dwProcessId)
			{
				return (AttachConsole((UInt32)dwProcessId));
			}

			/// <summary></summary>
			public static bool AttachConsoleToParentProcess()
			{
				return (AttachConsole(NativeConstants.ATTACH_PARENT_PROCESS));
			}

			/// <summary></summary>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean AttachConsole([In] UInt32 dwProcessId);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
			public static bool FreeConsoleFromProcess(int dwProcessId)
			{
				return (FreeConsole((UInt32)dwProcessId));
			}

			/// <summary></summary>
			public static bool FreeConsoleFromParentProcess()
			{
				return (FreeConsole(NativeConstants.ATTACH_PARENT_PROCESS));
			}

			/// <summary></summary>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean FreeConsole([In] UInt32 dwProcessId);

			/// <summary></summary>
			public static SafeFileHandle GetStandardInputHandle()
			{
				return (GetStdHandle(NativeTypes.STD_HANDLE.STD_INPUT_HANDLE));
			}

			/// <summary></summary>
			public static SafeFileHandle GetStandardOutputHandle()
			{
				return (GetStdHandle(NativeTypes.STD_HANDLE.STD_OUTPUT_HANDLE));
			}

			/// <summary></summary>
			public static SafeFileHandle GetStandardErrorHandle()
			{
				return (GetStdHandle(NativeTypes.STD_HANDLE.STD_ERROR_HANDLE));
			}

			/// <summary>
			/// Retrieves a handle to the specified standard device (standard input, standard output, or standard error).
			/// </summary>
			/// <param name="nStdHandle">The standard device.</param>
			/// <returns>
			/// If the function succeeds, the return value is a handle to the specified device,
			/// or a redirected handle set by a previous call to SetStdHandle. The handle has
			/// GENERIC_READ and GENERIC_WRITE access rights, unless the application has used
			/// SetStdHandle to set a standard handle with lesser access.
			/// If the function fails, the return value is INVALID_HANDLE_VALUE. To get extended
			/// error information, call <see cref="WinError.LastErrorToString"/>.
			/// If an application does not have associated standard handles, such as a service
			/// running on an interactive desktop, and has not redirected them, the return value
			/// is NULL.
			/// </returns>
			/// <remarks>
			/// Handles returned by GetStdHandle can be used by applications that need to read
			/// from or write to the console. When a console is created, the standard input handle
			/// is a handle to the console's input buffer, and the standard output and standard
			/// error handles are handles of the console's active screen buffer. These handles
			/// can be used by the ReadFile and WriteFile functions, or by any of the console
			/// functions that access the console input buffer or a screen buffer (for example,
			/// the ReadConsoleInput, WriteConsole, or GetConsoleScreenBufferInfo functions).
			/// The standard handles of a process may be redirected by a call to SetStdHandle,
			/// in which case GetStdHandle returns the redirected handle. If the standard handles
			/// have been redirected, you can specify the CONIN$ value in a call to the CreateFile
			/// function to get a handle to a console's input buffer. Similarly, you can specify
			/// the CONOUT$ value to get a handle to a console's active screen buffer.
			/// </remarks>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			private static extern SafeFileHandle GetStdHandle([In] NativeTypes.STD_HANDLE nStdHandle);

			/// <summary></summary>
			public static bool SetStandardInputHandle(SafeFileHandle fileHandle)
			{
				return (SetStdHandle(NativeTypes.STD_HANDLE.STD_INPUT_HANDLE, fileHandle));
			}

			/// <summary></summary>
			public static bool SetStandardOutputHandle(SafeFileHandle fileHandle)
			{
				return (SetStdHandle(NativeTypes.STD_HANDLE.STD_OUTPUT_HANDLE, fileHandle));
			}

			/// <summary></summary>
			public static bool SetStandardErrorHandle(SafeFileHandle fileHandle)
			{
				return (SetStdHandle(NativeTypes.STD_HANDLE.STD_ERROR_HANDLE, fileHandle));
			}

			/// <summary>
			/// Sets the handle for the specified standard device (standard input, standard output, or standard error).
			/// </summary>
			/// <param name="nStdHandle">The standard device for which the handle is to be set.</param>
			/// <param name="hHandle">The handle for the standard device.</param>
			/// <returns>
			/// If the function succeeds, the return value is nonzero.
			/// If the function fails, the return value is zero.
			/// To get extended error information, call <see cref="WinError.LastErrorToString"/>.
			/// </returns>
			/// <remarks>
			/// The standard handles of a process may have been redirected by a call to SetStdHandle,
			/// in which case GetStdHandle will return the redirected handle. If the standard handles
			/// have been redirected, you can specify the CONIN$ value in a call to the CreateFile
			/// function to get a handle to a console's input buffer. Similarly, you can specify the
			/// CONOUT$ value to get a handle to the console's active screen buffer.
			/// </remarks>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean SetStdHandle([In] NativeTypes.STD_HANDLE nStdHandle, [In] SafeFileHandle hHandle);
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Attaches the input/output/error console handles to the current process.
		/// Useful for windows applications since they do not support the console by default.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Emphasize occurrence of an native handle.")]
		public static bool Attach()
		{
			bool success = true;

			SafeFileHandle hStdIn, hStdOut, hStdErr;
			SafeFileHandle hStdInDup, hStdOutDup, hStdErrDup;
			FileIO.NativeTypes.BY_HANDLE_FILE_INFORMATION bhfi;

			hStdIn  = NativeMethods.GetStandardInputHandle();
			hStdOut = NativeMethods.GetStandardOutputHandle();
			hStdErr = NativeMethods.GetStandardErrorHandle();

			// Get current process handle.
			IntPtr hProcess = Process.GetCurrentProcess().Handle;

			// Duplicate handles to save initial value.
			Handle.NativeMethods.DuplicateHandle(hProcess, hStdIn,  hProcess, out hStdInDup,  0, true, Handle.NativeTypes.Options.DUPLICATE_SAME_ACCESS);
			Handle.NativeMethods.DuplicateHandle(hProcess, hStdOut, hProcess, out hStdOutDup, 0, true, Handle.NativeTypes.Options.DUPLICATE_SAME_ACCESS);
			Handle.NativeMethods.DuplicateHandle(hProcess, hStdErr, hProcess, out hStdErrDup, 0, true, Handle.NativeTypes.Options.DUPLICATE_SAME_ACCESS);

			// Attach to console window – this may modify the standard handles.
			NativeMethods.AttachConsoleToParentProcess();

			// Adjust the standard handles.
			if (FileIO.NativeMethods.GetFileInformationByHandle(NativeMethods.GetStandardInputHandle(), out bhfi))
			{
				if (!NativeMethods.SetStandardInputHandle(hStdInDup))
					success = false;
			}
			else
			{
				if (!NativeMethods.SetStandardInputHandle(hStdIn))
					success = false;
			}

			if (FileIO.NativeMethods.GetFileInformationByHandle(NativeMethods.GetStandardOutputHandle(), out bhfi))
			{
				if (!NativeMethods.SetStandardOutputHandle(hStdOutDup))
					success = false;
			}
			else
			{
				if (!NativeMethods.SetStandardOutputHandle(hStdOut))
					success = false;
			}

			if (FileIO.NativeMethods.GetFileInformationByHandle(NativeMethods.GetStandardErrorHandle(), out bhfi))
			{
				if (!NativeMethods.SetStandardErrorHandle(hStdErrDup))
					success = false;
			}
			else
			{
				if (!NativeMethods.SetStandardErrorHandle(hStdErr))
					success = false;
			}

			return (success);
		}

		/// <summary>
		/// Detaches the input/output/error console handles from the current process.
		/// Useful for windows applications since they do not support the console by default.
		/// </summary>
		public static bool Detach()
		{
			bool success = NativeMethods.FreeConsoleFromParentProcess();
			SendKeys.SendWait("{ENTER}");
			return (success);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
