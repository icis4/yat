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
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
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
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API relating to handles.
	/// </summary>
	public static class Handle
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

			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Type given by Win32.")]
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by Win32.")]
			[Flags]
			[CLSCompliant(false)]
			public enum Options : uint
			{
				DUPLICATE_CLOSE_SOURCE = 0x00000001,
				DUPLICATE_SAME_ACCESS  = 0x00000002,
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
			/// Closes an open object handle.
			/// </summary>
			/// <param name="hObject">A valid handle to an open object.</param>
			/// <returns>True on success, false on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool CloseHandle([In] SafeFileHandle hObject);

			/// <summary>
			/// Duplicates an object handle.
			/// </summary>
			/// <param name="hSourceProcessHandle">A handle to the process with the handle to
			/// be duplicated. The handle must have the PROCESS_DUP_HANDLE access right.
			/// For more information, see Process Security and Access Rights.</param>
			/// <param name="hSourceHandle">The handle to be duplicated. This is an open object
			/// handle that is valid in the context of the source process. For a list of objects
			/// whose handles can be duplicated, see the following Remarks section.</param>
			/// <param name="hTargetProcessHandle">A handle to the process that is to receive
			/// the duplicated handle. The handle must have the PROCESS_DUP_HANDLE access right.</param>
			/// <param name="lpTargetHandle">A pointer to a variable that receives the duplicate
			/// handle. This handle value is valid in the context of the target process. 
			/// If hSourceHandle is a pseudo handle returned by GetCurrentProcess or GetCurrentThread,
			/// DuplicateHandle converts it to a real handle to a process or thread, respectively.
			/// If lpTargetHandle is NULL, the function duplicates the handle, but does not return
			/// the duplicate handle value to the caller. This behavior exists only for backward
			/// compatibility with previous versions of this function. You should not use this
			/// feature, as you will lose system resources until the target process terminates.</param>
			/// <param name="dwDesiredAccess">The access requested for the new handle. For the
			/// flags that can be specified for each object type, see the following Remarks section.
			/// This parameter is ignored if the dwOptions parameter specifies the
			/// DUPLICATE_SAME_ACCESS flag. Otherwise, the flags that can be specified depend
			/// on the type of object whose handle is to be duplicated.</param>
			/// <param name="bInheritHandle">A variable that indicates whether the handle is
			/// inheritable. If TRUE, the duplicate handle can be inherited by new processes
			/// created by the target process. If FALSE, the new handle cannot be inherited.</param>
			/// <param name="dwOptions">Optional actions. This parameter can be zero, or any
			/// combination of the values.</param>
			/// <returns> If the function succeeds, the return value is nonzero.
			/// If the function fails, the return value is zero.
			/// To get extended error information, call GetLastError.</returns>
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, NativeTypes.Options dwOptions);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
