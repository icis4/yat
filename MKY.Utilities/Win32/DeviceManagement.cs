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
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace MKY.Utilities.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API relating relating to device management
	/// (SetupDixxx and RegisterDeviceNotification functions).
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
	public static class DeviceManagement
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary>
		/// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
		/// Use this one in the call to RegisterDeviceNotification() and
		/// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public Int32 dbcc_size;
			public Int32 dbcc_devicetype;
			public Int32 dbcc_reserved;
			public System.Guid dbcc_classguid;
			public Int16 dbcc_name;
		}

		/// <summary>
		/// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
		/// Use this to read the dbcc_name String and classguid:
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct DEV_BROADCAST_DEVICEINTERFACE_1
		{
			public Int32 dbcc_size;
			public Int32 dbcc_devicetype;
			public Int32 dbcc_reserved;
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
			public Byte[] dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			public Char[] dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct DEV_BROADCAST_HDR
		{
			public Int32 dbch_size;
			public Int32 dbch_devicetype;
			public Int32 dbch_reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SP_DEVICE_INTERFACE_DATA
		{
			public Int32 cbSize;
			public System.Guid InterfaceClassGuid;
			public Int32 Flags;
			public IntPtr Reserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public Int32 cbSize;
			public String DevicePath;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SP_DEVINFO_DATA
		{
			public Int32 cbSize;
			public System.Guid ClassGuid;
			public Int32 DevInst;
			public Int32 Reserved;
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string SETUP_DLL = "setupapi.dll";
		private const string USER_DLL = "user32.dll";

		// dbt.h
		private const Int32 DBT_DEVICEARRIVAL = 0x8000;
		private const Int32 DBT_DEVICEREMOVECOMPLETE = 0x8004;
		private const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
		private const Int32 DBT_DEVTYP_HANDLE = 6;
		private const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		private const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		private const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		private const Int32 WM_DEVICECHANGE = 0x219;

		// setupapi.h
		private const Int32 DIGCF_PRESENT = 2;
		private const Int32 DIGCF_DEVICEINTERFACE = 0x10;

		#endregion

		#region External Functions
		//==========================================================================================
		// External Functions
		//==========================================================================================

		/// <summary>
		/// Request to receive notification messages when a device in an interface class is attached
		/// or removed.
		/// </summary>
		/// <param name="hRecipient">Handle to the window that will receive device events.</param>
		/// <param name="NotificationFilter">Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify
		/// the type of device to send notifications for.</param>
		/// <param name="Flags">DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.</param>
		/// <returns>Device notification handle or NULL on failure.</returns>
		[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

		[DllImport(SETUP_DLL, SetLastError = true)]
		private static extern Int32 SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, Int32 hwndParent);

		/// <summary>
		/// Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
		/// </summary>
		/// <param name="DeviceInfoSet"></param>
		/// <returns>True on success.</returns>
		[DllImport(SETUP_DLL, SetLastError = true)]
		private static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		/// <summary>
		/// Retrieves a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.
		/// On return, DeviceInterfaceData contains the handle to a SP_DEVICE_INTERFACE_DATA structure for a detected device.
		/// </summary>
		/// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs.</param>
		/// <param name="DeviceInfoData">Optional SP_DEVINFO_DATA structure that defines a device
		/// instance that is a member of a device information set.</param>
		/// <param name="InterfaceClassGuid">Device interface GUID.</param>
		/// <param name="MemberIndex">Index to specify a device in a device information set.</param>
		/// <param name="DeviceInterfaceData">Pointer to a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.</param>
		/// <returns>True on success.</returns>
		[DllImport(SETUP_DLL, SetLastError = true)]
		private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		/// <summary>
		/// Retrieves a device information set for a specified group of devices.
		/// SetupDiEnumDeviceInterfaces uses the device information set.
		/// </summary>
		/// <param name="ClassGuid">Interface class GUID.</param>
		/// <param name="Enumerator">Null to retrieve information for all device instances.</param>
		/// <param name="hwndParent">Optional handle to a top-level window (unused here).</param>
		/// <param name="Flags">Flags to limit the returned information to currently present devices
		/// and devices that expose interfaces in the class specified by the GUID.</param>
		/// <returns>Handle to a device information set for the devices.</returns>
		[DllImport(SETUP_DLL, SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr SetupDiGetClassDevs(ref System.Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

		/// <summary>
		/// Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure containing information about a device.
		/// To retrieve the information, call this function twice. The first time returns the size of the structure.
		/// The second time returns a pointer to the data.
		/// </summary>
		/// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs</param>
		/// <param name="DeviceInterfaceData">SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces</param>
		/// <param name="DeviceInterfaceDetailData">A returned pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA.
		/// Structure to receive information about the specified interface.</param>
		/// <param name="DeviceInterfaceDetailDataSize">The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
		/// <param name="RequiredSize">Pointer to a variable that will receive the returned required size of the
		/// SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
		/// <param name="DeviceInfoData">Returned pointer to an SP_DEVINFO_DATA structure to receive information about the device.</param>
		/// <returns>True on success.</returns>
		[DllImport(SETUP_DLL, SetLastError = true, CharSet = CharSet.Auto)]
		private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);

		/// <summary>
		/// Stop receiving notification messages.
		/// </summary>
		/// <param name="Handle">Handle returned previously by RegisterDeviceNotification.</param>
		/// <returns>True on success.</returns>
		[DllImport(USER_DLL, SetLastError = true)]
		private static extern Boolean UnregisterDeviceNotification(IntPtr Handle);

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Compares two device path names. Used to find out if the device name of a recently
		/// attached or removed device matches the name of a device the application is
		/// communicating with.
		/// </summary>
		/// <param name="message">
		/// A WM_DEVICECHANGE message. A call to RegisterDeviceNotification causes
		/// WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine..
		/// </param>
		/// <param name="devicePathName">
		/// A device pathname returned by SetupDiGetDeviceInterfaceDetail in an
		/// SP_DEVICE_INTERFACE_DETAIL_DATA structure.
		/// </param>
		/// <returns>True if the names match, False if not.</returns>
		public static Boolean DeviceNameMatch(Message message, string devicePathName)
		{
			try
			{
				DEV_BROADCAST_DEVICEINTERFACE_1 devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE_1();
				DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();

				// The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.
				Marshal.PtrToStructure(message.LParam, devBroadcastHeader);

				if ((devBroadcastHeader.dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE))
				{
					// The dbch_devicetype parameter indicates that the event applies to a device interface.
					// So the structure in LParam is actually a DEV_BROADCAST_INTERFACE structure, 
					// which begins with a DEV_BROADCAST_HDR.

					// Obtain the number of characters in dbch_name by subtracting the 32 bytes
					// in the strucutre that are not part of dbch_name and dividing by 2 because there are 
					// 2 bytes per character.
					int stringSize = System.Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);

					// The dbcc_name parameter of devBroadcastDeviceInterface contains the device name. 
					// Trim dbcc_name to match the size of the String.
					devBroadcastDeviceInterface.dbcc_name = new Char[stringSize + 1];

					// Marshal data from the unmanaged block pointed to by m.LParam 
					// to the managed object devBroadcastDeviceInterface.
					Marshal.PtrToStructure(message.LParam, devBroadcastDeviceInterface);

					// Store the device name in a String.
					String DeviceNameString = new String(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);

					// Compare the name of the newly attached device with the name of the device 
					// the application is accessing (devicePathName).
					// Set ignorecase True.
					if ((String.Compare(DeviceNameString, devicePathName, true) == 0))
						return (true);
					else
						return (false);
				}

				return (false);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Use SetupDi API functions to retrieve the device path names of attached devices that
		/// belong to a device interface class.
		/// </summary>
		/// <param name="classGuid">An interface class GUID.</param>
		/// <returns>An array containing the path names of the devices currently available on the system.</returns>
		public static string[] GetDevicesFromGuid(System.Guid classGuid)
		{
			int bufferSize = 0;
			IntPtr detailDataBuffer = IntPtr.Zero;
			IntPtr deviceInfoSet = new IntPtr();
			Boolean lastDevice = false;
			int memberIndex = 0;
			SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
			List<string> devicePaths = new List<string>();

			try
			{
				deviceInfoSet = SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

				// The cbSize element of the deviceInterfaceData structure must be set to the structure's size in bytes. 
				// The size is 28 bytes for 32-bit code and 32 bits for 64-bit code.
				deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);

				do
				{
					// Begin with 0 and increment through the device information set until no more devices are available.
					if (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref classGuid, memberIndex, ref deviceInterfaceData))
					{
						// A device is present. Retrieve the size of the data buffer. Don't care about the return value, it will be false.
						SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

						// Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.
						detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

						// Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.
						Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

						// Call SetupDiGetDeviceInterfaceDetail again.
						// This time, pass a pointer to DetailDataBuffer and the returned required buffer size.
						if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, IntPtr.Zero))
						{
							// Skip over cbsize (4 bytes) to get the address of the devicePathName.
							IntPtr pDevicePathName = new IntPtr(detailDataBuffer.ToInt32() + 4);

							// Get the String containing the devicePathName.
							devicePaths.Add(Marshal.PtrToStringAuto(pDevicePathName));
						}
					}
					else
					{
						lastDevice = true;
					}
					memberIndex++;
				}
				while (!((lastDevice == true)));
			}
			catch
			{
				throw;
			}
			finally
			{
				if (detailDataBuffer != IntPtr.Zero)
				{
					// Free the memory allocated previously by AllocHGlobal.
					Marshal.FreeHGlobal(detailDataBuffer);
				}

				if (deviceInfoSet != IntPtr.Zero)
					SetupDiDestroyDeviceInfoList(deviceInfoSet);
			}

			return (devicePaths.ToArray());
		}

		/// <summary>
		/// Requests to receive a notification when a device is attached or removed.
		/// </summary>
		/// <param name="devicePathName">Handle to a device.</param>
		/// <param name="formHandle">Handle to the window that will receive device events.</param>
		/// <param name="classGuid">Device interface GUID.</param>
		/// <param name="deviceNotificationHandle">Returned device notification handle.</param>
		/// <returns>True on success.</returns>
		public static Boolean RegisterForDeviceNotifications(string devicePathName, IntPtr formHandle, System.Guid classGuid, ref IntPtr deviceNotificationHandle)
		{
			// A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.
			DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
			IntPtr devBroadcastDeviceInterfaceBuffer = IntPtr.Zero;
			int size = 0;

			try
			{
				// Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.
				// Set the size.
				size = Marshal.SizeOf(devBroadcastDeviceInterface);
				devBroadcastDeviceInterface.dbcc_size = size;

				// Request to receive notifications about a class of devices.
				devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
				devBroadcastDeviceInterface.dbcc_reserved = 0;

				// Specify the interface class to receive notifications about.
				devBroadcastDeviceInterface.dbcc_classguid = classGuid;

				// Allocate memory for the buffer that holds the DEV_BROADCAST_DEVICEINTERFACE structure.
				devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);

				// Copy the DEV_BROADCAST_DEVICEINTERFACE structure to the buffer.
				// Set fDeleteOld True to prevent memory leaks.
				Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);

				deviceNotificationHandle = RegisterDeviceNotification(formHandle, devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);

				// Marshal data from the unmanaged block devBroadcastDeviceInterfaceBuffer to
				// the managed object devBroadcastDeviceInterface
				Marshal.PtrToStructure(devBroadcastDeviceInterfaceBuffer, devBroadcastDeviceInterface);

				if ((deviceNotificationHandle.ToInt32() == IntPtr.Zero.ToInt32()))
					return (false);
				else
					return (true);
			}
			catch
			{
				throw;
			}
			finally
			{
				if (devBroadcastDeviceInterfaceBuffer != IntPtr.Zero)
				{
					// Free the memory allocated previously by AllocHGlobal.
					Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer);
				}
			}
		}

		/// <summary>
		/// Requests to stop receiving notification messages when a device in an interface class
		/// is attached or removed.
		/// </summary>
		/// <param name="deviceNotificationHandle">Handle returned previously by RegisterDeviceNotification.</param>
		public static void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
		{
			try
			{
				DeviceManagement.UnregisterDeviceNotification(deviceNotificationHandle);
				// Ignore failures.
			}
			catch
			{
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
