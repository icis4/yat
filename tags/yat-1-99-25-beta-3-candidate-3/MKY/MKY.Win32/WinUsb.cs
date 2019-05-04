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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the WinUSB API.
	/// </summary>
	/// <remarks>
	/// The WinUSB API is only useful with devices that provide a WinUSB based driver.
	/// See http://download.microsoft.com/download/9/C/5/9C5B2167-8017-4BAE-9FDE-D599BAC8184A/WinUsb_HowTo.docx for details.
	/// </remarks>
	public static class WinUsb
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		private static class NativeMethods
		{
			private const string WINUSB_DLL = "winusb.dll";

			[DllImport(WINUSB_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool WinUsb_Initialize([In] SafeFileHandle DeviceHandle, [Out] out SafeFileHandle InterfaceHandle);

			[DllImport(WINUSB_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool WinUsb_GetDescriptor([In] SafeFileHandle InterfaceHandle, [In] DescriptorType DescriptorType, [In] byte Index, [In] UInt16 LanguageID, [Out] byte[] Buffer, [In] UInt32 BufferLength, [Out] out UInt32 LengthTransferred);

			[DllImport(WINUSB_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool WinUsb_GetDescriptor([In] SafeFileHandle InterfaceHandle, [In] DescriptorType DescriptorType, [In] byte Index, [In] UInt16 LanguageID, [Out] StringBuilder Buffer, [In] UInt32 BufferLength, [Out] out UInt32 LengthTransferred);
		}

		#endregion

		#endregion

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

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Retrieves the device handle of the HID device at the given system path.
		/// </summary>
		public static bool GetUsbHandle(string devicePath, out SafeFileHandle usbHandle)
		{
			SafeFileHandle h = FileIO.NativeMethods.CreateFile
				(
				devicePath,
				FileIO.NativeTypes.Access.GENERIC_READ_WRITE,
				FileIO.NativeTypes.ShareMode.SHARE_READ_WRITE,
				IntPtr.Zero,
				FileIO.NativeTypes.CreationDisposition.OPEN_EXISTING,
				FileIO.NativeTypes.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
				);

			if (!h.IsInvalid)
			{
				usbHandle = h;
				return (true);
			}

			usbHandle = null;
			return (false);
		}

		/// <summary>
		/// Retrieves a handle for the interface that is associated with the indicated device.
		/// </summary>
		/// <param name="deviceHandle">
		/// The handle to the device that CreateFile returned. WinUSB uses overlapped I/O, so
		/// FILE_FLAG_OVERLAPPED must be specified in the dwFlagsAndAttributes parameter of
		/// CreateFile call for DeviceHandle to have the characteristics necessary for
		/// WinUsb_Initialize to function properly.
		/// </param>
		/// <param name="interfaceHandle">
		/// The interface handle that WinUsb_Initialize returns. All other WinUSB routines require
		/// this handle as input. The handle is opaque.
		/// </param>
		/// <returns>
		/// TRUE if the operation succeeds. Otherwise, this routine returns FALSE, and the caller
		/// can retrieve the logged error by calling <see cref="Debug.GetLastErrorCode"/> or
		/// <see cref="Debug.GetLastErrorMessage"/>.
		/// </returns>
		public static bool InitializeInterfaceHandle(SafeFileHandle deviceHandle, out SafeFileHandle interfaceHandle)
		{
			return (NativeMethods.WinUsb_Initialize(deviceHandle, out interfaceHandle));
		}

		/// <summary>
		/// Returns a requested device descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetDeviceDescriptor(SafeFileHandle interfaceHandle, int index, int languageId, byte[] buffer, out int lengthTransferred)
		{
			try
			{
				if (Version.IsWindowsVistaOrLater())
				{
					UInt32 l;
					if (NativeMethods.WinUsb_GetDescriptor(interfaceHandle, DescriptorType.Device, (byte)index, (UInt16)languageId, buffer, (UInt32)buffer.Length, out l))
					{
						lengthTransferred = (int)l;
						return (true);
					}
				}
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(WinUsb), ex);
				throw;
			}
		}

		/// <summary>
		/// Returns a requested configuration descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetConfigurationDescriptor(SafeFileHandle interfaceHandle, int index, int languageId, byte[] buffer, out int lengthTransferred)
		{
			try
			{
				if (Version.IsWindowsVistaOrLater())
				{
					UInt32 l;
					if (NativeMethods.WinUsb_GetDescriptor(interfaceHandle, DescriptorType.Configuration, (byte)index, (UInt16)languageId, buffer, (UInt32)buffer.Length, out l))
					{
						lengthTransferred = (int)l;
						return (true);
					}
				}
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(WinUsb), ex);
				throw;
			}
		}

		/// <summary>
		/// Returns a requested string descriptor.
		/// </summary>
		/// <remarks>
		/// Supported under Windows Vista and later only. Applies to all methods of WinUsb.
		/// </remarks>
		public static bool GetStringDescriptor(SafeFileHandle interfaceHandle, int index, int languageId, out string buffer, out int lengthTransferred)
		{
			try
			{
				if (Version.IsWindowsVistaOrLater())
				{
					StringBuilder s = new StringBuilder(Usb.Descriptors.MaximumStringDescriptorCharLength);
					UInt32 l;
					if (NativeMethods.WinUsb_GetDescriptor(interfaceHandle, DescriptorType.String, (byte)index, (UInt16)languageId, s, (UInt32)s.Capacity, out l))
					{
						buffer = s.ToString();
						lengthTransferred = (int)l;
						return (true);
					}
				}
				buffer = "";
				lengthTransferred = 0;
				return (false); // Not supported before Windows Vista.
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(WinUsb), ex);
				throw;
			}
		}

		/// <summary>
		/// Returns all culture specific strings of the given index string descriptor.
		/// </summary>
		public static Dictionary<CultureInfo, string> GetCultureSpecificStrings(SafeFileHandle interfaceHandle, int index, IEnumerable<CultureInfo> cultureInfo)
		{
			Dictionary<CultureInfo, string> d = new Dictionary<CultureInfo, string>();
			foreach (CultureInfo ci in cultureInfo)
			{
				string s;
				int lengthTransferred;
				if (WinUsb.GetStringDescriptor(interfaceHandle, index, ci.LCID, out s, out lengthTransferred))
					d.Add(ci, s);
			}
			return (d);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================