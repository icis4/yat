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
	/// Encapsulates parts of the Win32 API for USB communications.
	/// </summary>
	public static class Usb
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
		private enum DescriptorType
		{
			Device        = 1,
			Configuration = 2,
			String        = 3,
		}

		#pragma warning restore 1591

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string USB_DLL = "winusb.dll";

		/// <summary>
		/// Maximum of 126 characters in UCS-2 format.
		/// </summary>
		public const int MaximumStringDescriptorCharLength = 126;

		/// <summary>
		/// 2 x 126 characters + 2 x '\0' results in 254.
		/// </summary>
		public const int MaximumStringDescriptorByteLength = 254;

		#endregion

		#region External Functions
		//==========================================================================================
		// External Functions
		//==========================================================================================

		[DllImport(USB_DLL, SetLastError = true)]
		private static extern Boolean WinUsb_Initialize(SafeFileHandle DeviceHandle, out IntPtr InterfaceHandle);

		[DllImport(USB_DLL, SetLastError = true)]
		private static extern Boolean WinUsb_GetDescriptor(IntPtr InterfaceHandle, DescriptorType DescriptorType, Byte Index, UInt16 LanguageID, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred);

		[DllImport(USB_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern Boolean WinUsb_GetDescriptor(IntPtr InterfaceHandle, DescriptorType DescriptorType, Byte Index, UInt16 LanguageID, StringBuilder Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred);

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Retrieves a handle for the interface that is associated with the indicated device.
		/// </summary>
		public static bool InitializeHandle(SafeFileHandle deviceHandle, out IntPtr interfaceHandle)
		{
			return (WinUsb_Initialize(deviceHandle, out interfaceHandle));
		}

		/// <summary>
		/// Returns a requested device descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetDeviceDescriptor(IntPtr interfaceHandle, byte index, int languageId, byte[] buffer, out int lengthTransferred)
		{
			try
			{
				if (!Version.IsWindowsVistaOrLater())
				{
					UInt32 transferred = 0;
					if (WinUsb_GetDescriptor(interfaceHandle, DescriptorType.Device, index, (UInt16)languageId, buffer, (UInt32)buffer.Length, ref transferred))
					{
						lengthTransferred = (int)transferred;
						return (true);
					}
				}
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Usb), ex);
				throw;
			}
		}

		/// <summary>
		/// Returns a requested configuration descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetConfigurationDescriptor(IntPtr interfaceHandle, byte index, int languageId, byte[] buffer, out int lengthTransferred)
		{
			try
			{
				if (!Version.IsWindowsVistaOrLater())
				{
					UInt32 transferred = 0;
					if (WinUsb_GetDescriptor(interfaceHandle, DescriptorType.Configuration, index, (UInt16)languageId, buffer, (UInt32)buffer.Length, ref transferred))
					{
						lengthTransferred = (int)transferred;
						return (true);
					}
				}
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Usb), ex);
				throw;
			}
		}

		/// <summary>
		/// Returns a requested string descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetStringDescriptor(IntPtr interfaceHandle, int index, int languageId, out string buffer, out int lengthTransferred)
		{
			try
			{
				if (!Version.IsWindowsVistaOrLater())
				{
					StringBuilder s = new StringBuilder(Usb.MaximumStringDescriptorCharLength);
					UInt32 transferred = 0;
					if (WinUsb_GetDescriptor(interfaceHandle, DescriptorType.String, (Byte)index, (UInt16)languageId, s, (UInt32)s.Capacity, ref transferred))
					{
						buffer = s.ToString();
						lengthTransferred = (int)transferred;
						return (true);
					}
				}
				buffer = "";
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(Usb), ex);
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
