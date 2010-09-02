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
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MKY.Utilities.Diagnostics;

#endregion

namespace MKY.Win32
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
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		/// <summary>
		/// Class encapsulating native Win32 types, constants and functions.
		/// </summary>
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		public static class Native
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

			/// <remarks>dbt.h</remarks>
			[Flags]
			[CLSCompliant(false)]
			public enum DIGCF : uint
			{
				/// <remarks>
				/// Only valid with DIGCF_DEVICEINTERFACE
				/// </remarks>
				DEFAULT         = 0x00000001,
				PRESENT         = 0x00000002,
				ALLCLASSES      = 0x00000004,
				PROFILE         = 0x00000008,
				DEVICEINTERFACE = 0x00000010,
			}

			/// <remarks>dbt.h</remarks>
			[CLSCompliant(false)]
			public enum DBT : uint
			{
				DEVICEARRIVAL        = 0x00008000,
				DEVICEREMOVECOMPLETE = 0x00008004,
			}

			/// <remarks>dbt.h</remarks>
			[CLSCompliant(false)]
			public enum DBT_DEVTYP : uint
			{
				DEVICEINTERFACE = 0x00000005,
				HANDLE          = 0x00000006,
			}

			[Flags]
			[CLSCompliant(false)]
			public enum DEVICE_NOTIFY : uint
			{
				WINDOW_HANDLE         = 0x00000000,
				SERVICE_HANDLE        = 0x00000001,
				ALL_INTERFACE_CLASSES = 0x00000004,
			}

			/// <summary>
			/// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
			/// Use this one in the call to RegisterDeviceNotification() and
			/// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure.
			/// </summary>
			/// <remarks>
			/// Must be a class because <see cref="Marshal.PtrToStructure(IntPtr, object)"/> and
			/// <see cref="RegisterDeviceNotification"/> require a reference type.
			/// </remarks>
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "See remarks above.")]
			public class DEV_BROADCAST_DEVICEINTERFACE
			{
				public UInt32      dbcc_size;
				public DBT_DEVTYP  dbcc_devicetype;
				public UInt32      dbcc_reserved;
				public System.Guid dbcc_classguid;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
				public char[]      dbcc_name;
			}

			/// <remarks>
			/// Must be a class because <see cref="Marshal.PtrToStructure(IntPtr, object)"/> requires a reference type.
			/// </remarks>
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1401:FieldsMustBePrivate", Justification = "See remarks above.")]
			public class DEV_BROADCAST_HDR
			{
				public UInt32     dbch_size;
				public DBT_DEVTYP dbch_devicetype;
				public UInt32     dbch_reserved;
			}

			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVICE_INTERFACE_DATA
			{
				public UInt32 cbSize;
				public System.Guid InterfaceClassGuid;
				public UInt32 Flags;
				public IntPtr Reserved;
			}

			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVICE_INTERFACE_DETAIL_DATA
			{
				public UInt32 cbSize;
				public string DevicePath;
			}

			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVINFO_DATA
			{
				public UInt32 cbSize;
				public System.Guid ClassGuid;
				public UInt32 DevInst;
				public UInt32 Reserved;
			}

			#pragma warning restore 1591

			#endregion

			#region Constants
			//==========================================================================================
			// Constants
			//==========================================================================================

			private const string SETUP_DLL = "setupapi.dll";
			private const string USER_DLL = "user32.dll";

			/// <remarks>dbt.h</remarks>
			[CLSCompliant(false)]
			public const UInt32 WM_DEVICECHANGE = 0x00000219;

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
			[CLSCompliant(false)]
			public static extern IntPtr RegisterDeviceNotification([In] IntPtr hRecipient, [In] DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, [In] DEVICE_NOTIFY Flags);

			/// <summary></summary>
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[CLSCompliant(false)]
			public static extern Int32 SetupDiCreateDeviceInfoList([In] ref System.Guid ClassGuid, [In] Int32 hwndParent);

			/// <summary>
			/// Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
			/// </summary>
			/// <returns>True on success.</returns>
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 SetupDiDestroyDeviceInfoList([In] IntPtr DeviceInfoSet);

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
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[CLSCompliant(false)]
			public static extern bool SetupDiEnumDeviceInterfaces([In] IntPtr DeviceInfoSet, [In] IntPtr DeviceInfoData, [In] ref System.Guid InterfaceClassGuid, [In] Int32 MemberIndex, [In, Out] ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

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
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[CLSCompliant(false)]
			public static extern IntPtr SetupDiGetClassDevs([In] ref System.Guid ClassGuid, [In] IntPtr Enumerator, [In] IntPtr hwndParent, [In] DIGCF Flags);

			/// <summary>
			/// Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure containing information about a device.
			/// To retrieve the information, call this function twice. The first time returns the size of the structure.
			/// The second time returns a pointer to the data.
			/// </summary>
			/// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs.</param>
			/// <param name="DeviceInterfaceData">SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces.</param>
			/// <param name="DeviceInterfaceDetailData">A returned pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA.
			/// Structure to receive information about the specified interface.</param>
			/// <param name="DeviceInterfaceDetailDataSize">The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
			/// <param name="RequiredSize">Pointer to a variable that will receive the returned required size of the
			/// SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
			/// <param name="DeviceInfoData">Returned pointer to an SP_DEVINFO_DATA structure to receive information about the device.</param>
			/// <returns>True on success.</returns>
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[CLSCompliant(false)]
			public static extern bool SetupDiGetDeviceInterfaceDetail([In] IntPtr DeviceInfoSet, [In] ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, [Out] IntPtr DeviceInterfaceDetailData, [In] Int32 DeviceInterfaceDetailDataSize, [Out] out Int32 RequiredSize, [Out] IntPtr DeviceInfoData);

			/// <summary>
			/// Stop receiving notification messages.
			/// </summary>
			/// <param name="Handle">Handle returned previously by RegisterDeviceNotification.</param>
			/// <returns>True on success.</returns>
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool UnregisterDeviceNotification([In] IntPtr Handle);

			#endregion
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Use SetupDi API functions to retrieve the device path names of attached devices that
		/// belong to a device interface class.
		/// </summary>
		/// <param name="classGuid">An interface class GUID.</param>
		/// <returns>An array containing the path names of the devices currently available on the system.</returns>
		public static string[] GetDevicesFromGuid(System.Guid classGuid)
		{
			int bufferSize = 0;
			IntPtr pDetailDataBuffer = IntPtr.Zero;
			IntPtr pDeviceInfoSet = new IntPtr();
			bool lastDevice = false;
			int memberIndex = 0;
			Native.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new Native.SP_DEVICE_INTERFACE_DATA();
			List<string> devicePaths = new List<string>();

			try
			{
				pDeviceInfoSet = Native.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, Native.DIGCF.PRESENT | Native.DIGCF.DEVICEINTERFACE);

				// The cbSize element of the deviceInterfaceData structure must be set to the structure's size in bytes. 
				// The size is 28 bytes for 32-bit code and 32 bits for 64-bit code.
				deviceInterfaceData.cbSize = (UInt32)Marshal.SizeOf(deviceInterfaceData);

				do
				{
					// Begin with 0 and increment through the device information set until no more devices are available.
					if (Native.SetupDiEnumDeviceInterfaces(pDeviceInfoSet, IntPtr.Zero, ref classGuid, memberIndex, ref deviceInterfaceData))
					{
						// A device is present. Retrieve the size of the data buffer. Don't care about the return value, it will be false.
						Native.SetupDiGetDeviceInterfaceDetail(pDeviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, out bufferSize, IntPtr.Zero);

						// Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.
						pDetailDataBuffer = Marshal.AllocHGlobal(bufferSize);

						// Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.
						Marshal.WriteInt32(pDetailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

						// Call SetupDiGetDeviceInterfaceDetail again.
						// This time, pass a pointer to DetailDataBuffer and the returned required buffer size.
						if (Native.SetupDiGetDeviceInterfaceDetail(pDeviceInfoSet, ref deviceInterfaceData, pDetailDataBuffer, bufferSize, out bufferSize, IntPtr.Zero))
						{
							// Skip over cbsize (4 bytes) to get the address of the devicePathName.
							IntPtr pDevicePathName = new IntPtr(pDetailDataBuffer.ToInt32() + 4);

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
				if (pDetailDataBuffer != IntPtr.Zero)
				{
					// Free the memory allocated previously by AllocHGlobal.
					Marshal.FreeHGlobal(pDetailDataBuffer);
				}

				if (pDeviceInfoSet != IntPtr.Zero)
					Native.SetupDiDestroyDeviceInfoList(pDeviceInfoSet);
			}

			return (devicePaths.ToArray());
		}

		/// <summary>
		/// Requests to receive a notification when a device is attached or removed.
		/// </summary>
		/// <param name="windowHandle">Handle to the window that will receive device events.</param>
		/// <param name="classGuid">Device interface GUID.</param>
		/// <param name="deviceNotificationHandle">Returned device notification handle.</param>
		/// <returns>True on success.</returns>
		public static bool RegisterDeviceNotificationHandle(IntPtr windowHandle, System.Guid classGuid, out IntPtr deviceNotificationHandle)
		{
			deviceNotificationHandle = IntPtr.Zero;

			try
			{
				// A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.
				Native.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new Native.DEV_BROADCAST_DEVICEINTERFACE();

				// Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure. Set the size.
				devBroadcastDeviceInterface.dbcc_size = (UInt32)Marshal.SizeOf(devBroadcastDeviceInterface);

				// Request to receive notifications about a class of devices.
				devBroadcastDeviceInterface.dbcc_devicetype = Native.DBT_DEVTYP.DEVICEINTERFACE;
				devBroadcastDeviceInterface.dbcc_reserved = 0;

				// Specify the interface class to receive notifications about.
				devBroadcastDeviceInterface.dbcc_classguid = classGuid;

				deviceNotificationHandle = Native.RegisterDeviceNotification(windowHandle, devBroadcastDeviceInterface, Native.DEVICE_NOTIFY.WINDOW_HANDLE);

				return (deviceNotificationHandle != IntPtr.Zero);
			}
			catch (Exception ex)
			{
				XDebug.WriteException(typeof(DeviceManagement), ex);
				throw (ex);
			}
		}

		/// <summary>
		/// Requests to stop receiving notification messages when a device in an interface class
		/// is attached or removed.
		/// </summary>
		/// <param name="deviceNotificationHandle">Handle returned previously by RegisterDeviceNotification.</param>
		public static void UnregisterDeviceNotificationHandle(IntPtr deviceNotificationHandle)
		{
			try
			{
				Native.UnregisterDeviceNotification(deviceNotificationHandle);

				// Ignore failures.
			}
			catch
			{
			}
		}

		/// <summary>
		/// Converts a device change message the a device path. Used to find out if
		/// the device name of a recently attached or removed device matches the name
		/// of a device the application is communicating with.
		/// </summary>
		/// <param name="deviceChangeMessage">
		/// A WM_DEVICECHANGE message. A call to RegisterDeviceNotification causes
		/// WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine.
		/// </param>
		/// <param name="devicePath">
		/// A device pathname returned by SetupDiGetDeviceInterfaceDetail in an
		/// SP_DEVICE_INTERFACE_DETAIL_DATA structure.
		/// </param>
		/// <returns>True if the conversion succeeded, False if not.</returns>
		public static bool DeviceChangeMessageToDevicePath(Message deviceChangeMessage, out string devicePath)
		{
			try
			{
				Native.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new Native.DEV_BROADCAST_DEVICEINTERFACE();
				Native.DEV_BROADCAST_HDR devBroadcastHeader = new Native.DEV_BROADCAST_HDR();

				// The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.
				Marshal.PtrToStructure(deviceChangeMessage.LParam, devBroadcastHeader);

				if ((devBroadcastHeader.dbch_devicetype == Native.DBT_DEVTYP.DEVICEINTERFACE))
				{
					// The dbch_devicetype parameter indicates that the event applies to a device
					// interface. So the structure in LParam is actually a DEV_BROADCAST_INTERFACE
					// structure, which begins with a DEV_BROADCAST_HDR.

					// Obtain the number of characters in dbch_name by subtracting the 32 bytes in
					// the strucutre that are not part of dbch_name and dividing by 2 because there
					// are 2 bytes per character.
					int stringSize = System.Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);

					// The dbcc_name parameter of devBroadcastDeviceInterface contains the device
					// name. Trim dbcc_name to match the size of the String.
					devBroadcastDeviceInterface.dbcc_name = new char[stringSize + 1];

					// Marshal data from the unmanaged block pointed to by m.LParam to the managed
					// object devBroadcastDeviceInterface.
					Marshal.PtrToStructure(deviceChangeMessage.LParam, devBroadcastDeviceInterface);

					// Store the device name in a String.
					devicePath = new string(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);
					return (true);
				}
			}
			catch { }

			devicePath = "";
			return (false);
		}

		/// <summary>
		/// Compares two device path names. Used to find out if the device name of a recently
		/// attached or removed device matches the name of a device the application is
		/// communicating with.
		/// </summary>
		/// <param name="deviceChangeMessage">
		/// A WM_DEVICECHANGE message. A call to RegisterDeviceNotification causes
		/// WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine.
		/// </param>
		/// <param name="devicePath">
		/// A device pathname returned by SetupDiGetDeviceInterfaceDetail in an
		/// SP_DEVICE_INTERFACE_DETAIL_DATA structure.
		/// </param>
		/// <returns>True if the names match, False if not.</returns>
		public static bool CompareDeviceChangeMessageToDevicePath(Message deviceChangeMessage, string devicePath)
		{
			string devicePathFromMessage;

			if (DeviceChangeMessageToDevicePath(deviceChangeMessage, out devicePathFromMessage))
				return (string.Compare(devicePathFromMessage, devicePath, true) == 0);

			return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
