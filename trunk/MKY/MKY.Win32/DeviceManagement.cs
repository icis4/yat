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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY.Diagnostics;
using MKY.IO;

#endregion

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_size")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_devicetype")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_reserved")]

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_name")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_classguid")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_reserved")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_size")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_devicetype")]

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA.#InterfaceClassGuid")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA.#Reserved")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA.#cbSize")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA.#Flags")]

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA.#DevInst")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA.#ClassGuid")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA.#Reserved")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA.#cbSize")]

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA.#DevicePath")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA.#cbSize")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_size", MessageId = "dbch")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_devicetype", MessageId = "dbch")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_devicetype", MessageId = "devicetype")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR.#dbch_reserved", MessageId = "dbch")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_name", MessageId = "dbcc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_classguid", MessageId = "dbcc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_classguid", MessageId = "classguid")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_reserved", MessageId = "dbcc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_size", MessageId = "dbcc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_devicetype", MessageId = "dbcc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE.#dbcc_devicetype", MessageId = "devicetype")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA.#cbSize", MessageId = "cb")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA.#cbSize", MessageId = "cb")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA.#cbSize", MessageId = "cb")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiCreateDeviceInfoList(System.Guid&,System.IntPtr)", MessageId = "hwnd")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#RegisterDeviceNotification(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE,MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "Devs")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "hwnd")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#ALL_INTERFACE_CLASSES")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#WINDOW_HANDLE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#SERVICE_HANDLE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeConstants.#WM_DEVICECHANGE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT_DEVTYP")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY", MessageId = "DEVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY", MessageId = "NOTIFY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#ALL_INTERFACE_CLASSES", MessageId = "INTERFACE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#ALL_INTERFACE_CLASSES", MessageId = "ALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#ALL_INTERFACE_CLASSES", MessageId = "CLASSES")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#WINDOW_HANDLE", MessageId = "HANDLE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#WINDOW_HANDLE", MessageId = "WINDOW")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#SERVICE_HANDLE", MessageId = "SERVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY.#SERVICE_HANDLE", MessageId = "HANDLE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR", MessageId = "DEV")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR", MessageId = "HDR")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_HDR", MessageId = "BROADCAST")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF", MessageId = "DIGCF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF.#PRESENT", MessageId = "PRESENT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF.#ALLCLASSES", MessageId = "ALLCLASSES")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF.#PROFILE", MessageId = "PROFILE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF.#DEFAULT", MessageId = "DEFAULT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DIGCF.#DEVICEINTERFACE", MessageId = "DEVICEINTERFACE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE", MessageId = "DEVICEINTERFACE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE", MessageId = "DEV")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE", MessageId = "BROADCAST")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeConstants.#WM_DEVICECHANGE", MessageId = "DEVICECHANGE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA", MessageId = "DEVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA", MessageId = "INTERFACE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA", MessageId = "DATA")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT_DEVTYP", MessageId = "DBT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT_DEVTYP", MessageId = "DEVTYP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT_DEVTYP.#HANDLE", MessageId = "HANDLE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT_DEVTYP.#DEVICEINTERFACE", MessageId = "DEVICEINTERFACE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA", MessageId = "DEVINFO")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVINFO_DATA", MessageId = "DATA")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA", MessageId = "DEVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA", MessageId = "INTERFACE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA", MessageId = "DETAIL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DETAIL_DATA", MessageId = "DATA")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiDestroyDeviceInfoList(System.IntPtr)", MessageId = "Device")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiDestroyDeviceInfoList(System.IntPtr)", MessageId = "Di")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiCreateDeviceInfoList(System.Guid&,System.IntPtr)", MessageId = "Class")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiCreateDeviceInfoList(System.Guid&,System.IntPtr)", MessageId = "Di")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#UnregisterDeviceNotification(System.IntPtr)", MessageId = "Handle")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#RegisterDeviceNotification(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE,MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY)", MessageId = "Flags")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#RegisterDeviceNotification(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DEV_BROADCAST_DEVICEINTERFACE,MKY.Win32.DeviceManagement+NativeTypes+DEVICE_NOTIFY)", MessageId = "Notification")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetDeviceInterfaceDetail(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&,System.IntPtr,System.Int32,System.Int32&,System.IntPtr)", MessageId = "Device")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetDeviceInterfaceDetail(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&,System.IntPtr,System.Int32,System.Int32&,System.IntPtr)", MessageId = "Required")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetDeviceInterfaceDetail(System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&,System.IntPtr,System.Int32,System.Int32&,System.IntPtr)", MessageId = "Di")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "Class")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "Flags")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "Di")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiGetClassDevs(System.Guid&,System.IntPtr,System.IntPtr,MKY.Win32.DeviceManagement+NativeTypes+DIGCF)", MessageId = "Enumerator")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiEnumDeviceInterfaces(System.IntPtr,System.IntPtr,System.Guid&,System.Int32,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&)", MessageId = "Device")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiEnumDeviceInterfaces(System.IntPtr,System.IntPtr,System.Guid&,System.Int32,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&)", MessageId = "Interface")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiEnumDeviceInterfaces(System.IntPtr,System.IntPtr,System.Guid&,System.Int32,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&)", MessageId = "Di")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeMethods.#SetupDiEnumDeviceInterfaces(System.IntPtr,System.IntPtr,System.Guid&,System.Int32,MKY.Win32.DeviceManagement+NativeTypes+SP_DEVICE_INTERFACE_DATA&)", MessageId = "Member")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT", MessageId = "DBT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT.#DEVICEREMOVECOMPLETE", MessageId = "DEVICEREMOVECOMPLETE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.DeviceManagement+NativeTypes+DBT.#DEVICEARRIVAL", MessageId = "DEVICEARRIVAL")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API related to device management
	/// (SetupDi___ and RegisterDeviceNotification functions).
	/// </summary>
	/// <remarks>
	/// This class is partly based on GenericHid of Jan Axelson's Lakeview Research.
	/// Visit GenericHid on http://www.lvr.com/hidpage.htm for details.
	/// This class needed to modify the original structure and contents of GenericHid
	/// due to the following reasons:
	/// - Suboptimal structure of the original GenericHid project
	/// - Missing features required for YAT
	/// - Potential reuse of this class for other services directly using the Win32 API
	/// DeviceManagement and Hid types and methods have been split into separate classes.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
	public static class DeviceManagement
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			/// <remarks>dbt.h and saying hello to StyleCop ;-.</remarks>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by the Win32 API.")]
			[CLSCompliant(false)]
			[Flags]
			public enum DIGCF : uint
			{
				/// <remarks>
				/// Only valid with DIGCF_DEVICEINTERFACE.
				/// </remarks>
				DEFAULT         = 0x00000001,
				PRESENT         = 0x00000002,
				ALLCLASSES      = 0x00000004,
				PROFILE         = 0x00000008,
				DEVICEINTERFACE = 0x00000010
			}

			/// <remarks>dbt.h and saying hello to StyleCop ;-.</remarks>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			public enum DBT : uint
			{
				DEVICEARRIVAL        = 0x00008000,
				DEVICEREMOVECOMPLETE = 0x00008004
			}

			/// <remarks>dbt.h and saying hello to StyleCop ;-.</remarks>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			public enum DBT_DEVTYP : uint
			{
				DEVICEINTERFACE = 0x00000005,
				HANDLE          = 0x00000006
			}

			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by the Win32 API.")]
			[CLSCompliant(false)]
			[Flags]
			public enum DEVICE_NOTIFY : uint
			{
				WINDOW_HANDLE         = 0x00000000,
				SERVICE_HANDLE        = 0x00000001,
				ALL_INTERFACE_CLASSES = 0x00000004
			}

			/// <summary>
			/// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
			/// Use this one in the call to RegisterDeviceNotification() and
			/// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure.
			/// </summary>
			/// <remarks>
			/// Must be a class because <see cref="Marshal.PtrToStructure(IntPtr, object)"/> and
			/// <see cref="NativeMethods.RegisterDeviceNotification"/> require a reference type.
			/// </remarks>
			[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field names are given by the Win32 API.")]
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "See remarks above.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public class DEV_BROADCAST_DEVICEINTERFACE
			{
				public UInt32     dbcc_size;
				public DBT_DEVTYP dbcc_devicetype;
				public UInt32     dbcc_reserved;
				public Guid       dbcc_classguid;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
				public Byte[]     dbcc_name;
			}

			/// <remarks>
			/// Must be a class because <see cref="Marshal.PtrToStructure(IntPtr, object)"/> requires a reference type.
			/// </remarks>
			[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field names are given by the Win32 API.")]
			[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "See remarks above.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public class DEV_BROADCAST_HDR
			{
				public UInt32     dbch_size;
				public DBT_DEVTYP dbch_devicetype;
				public UInt32     dbch_reserved;
			}

			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVICE_INTERFACE_DATA
			{
				public UInt32 cbSize;
				public Guid   InterfaceClassGuid;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Naming is given by the Win32 API.")]
				public UInt32 Flags;
				[SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", Justification = "Structure is given by the Win32 API.")]
				public IntPtr Reserved;
			}

			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVICE_INTERFACE_DETAIL_DATA
			{
				public UInt32 cbSize;
				public string DevicePath;
			}

			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SP_DEVINFO_DATA
			{
				public UInt32 cbSize;
				public Guid   ClassGuid;
				public UInt32 DevInst;
				public UInt32 Reserved;
			}

			#pragma warning restore 1591
		}

		#endregion

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Name is given by the Win32 API.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeConstants
		{
			/// <remarks>dbt.h and saying hello to StyleCop ;-.</remarks>
			[CLSCompliant(false)]
			public const UInt32 WM_DEVICECHANGE = 0x00000219;
		}

		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeMethods
		{
			private const string SETUP_DLL = "setupapi.dll";
			private const string USER_DLL = "user32.dll";

			/// <summary>
			/// Request to receive notification messages when a device in an interface class is attached
			/// or removed.
			/// </summary>
			/// <param name="hRecipient">Handle to the window that will receive device events.</param>
			/// <param name="NotificationFilter">Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify
			/// the type of device to send notifications for.</param>
			/// <param name="Flags">DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.</param>
			/// <returns>Device notification handle or NULL on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Naming is given by the Win32 API.")]
			[CLSCompliant(false)]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr RegisterDeviceNotification([In] IntPtr hRecipient, [In] NativeTypes.DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, [In] NativeTypes.DEVICE_NOTIFY Flags);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr SetupDiCreateDeviceInfoList([In] ref Guid ClassGuid, [In] IntPtr hwndParent);

			/// <summary>
			/// Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
			/// </summary>
			/// <returns>True on success.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SetupDiDestroyDeviceInfoList([In] IntPtr DeviceInfoSet);

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
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SetupDiEnumDeviceInterfaces([In] IntPtr DeviceInfoSet, [In] IntPtr DeviceInfoData, [In] ref Guid InterfaceClassGuid, [In] Int32 MemberIndex, [In, Out] ref NativeTypes.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

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
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Naming is given by the Win32 API.")]
			[CLSCompliant(false)]
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr SetupDiGetClassDevs([In] ref Guid ClassGuid, [In] IntPtr Enumerator, [In] IntPtr hwndParent, [In] NativeTypes.DIGCF Flags);

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
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(SETUP_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SetupDiGetDeviceInterfaceDetail([In] IntPtr DeviceInfoSet, [In] ref NativeTypes.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, [Out] IntPtr DeviceInterfaceDetailData, [In] Int32 DeviceInterfaceDetailDataSize, [Out] out Int32 RequiredSize, [Out] IntPtr DeviceInfoData);

			/// <summary>
			/// Stop receiving notification messages.
			/// </summary>
			/// <param name="Handle">Handle returned previously by RegisterDeviceNotification.</param>
			/// <returns>True on success.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean UnregisterDeviceNotification([In] IntPtr Handle);
		}

		#endregion

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
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Emphasize occurrence of an native pointer.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "MKY.Win32.DeviceManagement+NativeMethods.SetupDiDestroyDeviceInfoList(System.IntPtr)", Justification = "Don't care about the result.")]
		public static string[] GetDevicesFromGuid(Guid classGuid)
		{
			int bufferSize = 0;
			IntPtr pDetailDataBuffer = IntPtr.Zero;
			IntPtr pDeviceInfoSet = new IntPtr();
			bool isLastDevice = false;
			int memberIndex = 0;
			NativeTypes.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeTypes.SP_DEVICE_INTERFACE_DATA();
			List<string> devicePaths = new List<string>();

			try
			{
				pDeviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, NativeTypes.DIGCF.PRESENT | NativeTypes.DIGCF.DEVICEINTERFACE);

				// The cbSize element of the deviceInterfaceData structure must be set to the structure's size in bytes:
				deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData); // The size is 28 bytes for 32- and 32 bytes for 64-bit binaries.

				do
				{
					// Begin with 0 and increment through the device information set until no more devices are available:
					if (NativeMethods.SetupDiEnumDeviceInterfaces(pDeviceInfoSet, IntPtr.Zero, ref classGuid, memberIndex, ref deviceInterfaceData))
					{
						// A device is present. Retrieve the size of the data buffer (don't care about the return value, it will be false):
						NativeMethods.SetupDiGetDeviceInterfaceDetail(pDeviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, out bufferSize, IntPtr.Zero);

						// Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size:
						pDetailDataBuffer = Marshal.AllocHGlobal(bufferSize);

						// Store cbSize in the first bytes of the array (the number of bytes varies with 32- and 64-bit systems):
						Marshal.WriteInt32(pDetailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : (8));

						// Call SetupDiGetDeviceInterfaceDetail again. This time, pass a pointer to DetailDataBuffer and the returned required buffer size:
						if (NativeMethods.SetupDiGetDeviceInterfaceDetail(pDeviceInfoSet, ref deviceInterfaceData, pDetailDataBuffer, bufferSize, out bufferSize, IntPtr.Zero))
						{
							// Skip over cbsize (4 bytes) to get the address of the devicePathName:
							IntPtr pDevicePathName = new IntPtr(pDetailDataBuffer.ToInt32() + 4);

							// Get the String containing the devicePathName:
							devicePaths.Add(Marshal.PtrToStringAuto(pDevicePathName));
						}
					}
					else
					{
						isLastDevice = true;
					}
					memberIndex++;
				}
				while (!isLastDevice);
			}
			finally
			{
				if (pDetailDataBuffer != IntPtr.Zero)
				{
					// Free the memory allocated previously by AllocHGlobal:
					Marshal.FreeHGlobal(pDetailDataBuffer);
				}

				if (pDeviceInfoSet != IntPtr.Zero)
				{
					NativeMethods.SetupDiDestroyDeviceInfoList(pDeviceInfoSet);
				}
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "'ClassGuid' is the official term, even WMI uses it.")]
		public static bool RegisterDeviceNotificationHandle(IntPtr windowHandle, Guid classGuid, out IntPtr deviceNotificationHandle)
		{
			deviceNotificationHandle = IntPtr.Zero;

			try
			{
				// A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request:
				NativeTypes.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new NativeTypes.DEV_BROADCAST_DEVICEINTERFACE();

				// Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure and set the size:
				devBroadcastDeviceInterface.dbcc_size = (uint)Marshal.SizeOf(devBroadcastDeviceInterface);

				// Request to receive notifications about a class of devices:
				devBroadcastDeviceInterface.dbcc_devicetype = NativeTypes.DBT_DEVTYP.DEVICEINTERFACE;
				devBroadcastDeviceInterface.dbcc_reserved = 0;

				// Specify the interface class to receive notifications about:
				devBroadcastDeviceInterface.dbcc_classguid = classGuid;

				deviceNotificationHandle = NativeMethods.RegisterDeviceNotification(windowHandle, devBroadcastDeviceInterface, NativeTypes.DEVICE_NOTIFY.WINDOW_HANDLE);

				return (deviceNotificationHandle != IntPtr.Zero);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(DeviceManagement), ex);
				throw; // Rethrow!
			}
		}

		/// <summary>
		/// Requests to stop receiving notification messages when a device in an interface class
		/// is attached or removed.
		/// </summary>
		/// <param name="deviceNotificationHandle">Handle returned previously by RegisterDeviceNotification.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static void UnregisterDeviceNotificationHandle(IntPtr deviceNotificationHandle)
		{
			try
			{
				NativeMethods.UnregisterDeviceNotification(deviceNotificationHandle);
				//// Ignore failures.
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(DeviceManagement), ex, "Exception while unregistering device notification handle!");
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		public static bool DeviceChangeMessageToDevicePath(Message deviceChangeMessage, out string devicePath)
		{
			try
			{
				NativeTypes.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new NativeTypes.DEV_BROADCAST_DEVICEINTERFACE();
				NativeTypes.DEV_BROADCAST_HDR devBroadcastHeader = new NativeTypes.DEV_BROADCAST_HDR();

				// The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.
				Marshal.PtrToStructure(deviceChangeMessage.LParam, devBroadcastHeader);

				if ((devBroadcastHeader.dbch_devicetype == NativeTypes.DBT_DEVTYP.DEVICEINTERFACE))
				{
					// The dbch_devicetype parameter indicates that the event applies to a device
					// interface. So the structure in LParam is actually a DEV_BROADCAST_INTERFACE
					// structure, which begins with a DEV_BROADCAST_HDR.

					// Obtain the number of characters in dbch_name by subtracting the 32 bytes in
					// the strucutre that are not part of dbch_name and dividing by 2 because there
					// are 2 bytes per character.
					int stringSize = Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);
					int byteCount = stringSize * 2;

					// Marshal data from the unmanaged block pointed to by m.LParam to the managed
					// object devBroadcastDeviceInterface.
					Marshal.PtrToStructure(deviceChangeMessage.LParam, devBroadcastDeviceInterface);

					// Decode and store the device name bytes in a string.
					Encoding e = Encoding.Unicode;
					byte[] bytes = devBroadcastDeviceInterface.dbcc_name;
					int charCount = e.GetCharCount(bytes, 0, byteCount);
					char[] chars = new char[charCount];
					e.GetDecoder().GetChars(bytes, 0, byteCount, chars, 0, true);
					devicePath = new string(chars);

					return (true);
				}
			}
			catch (ArgumentException ex)
			{
				DebugEx.WriteException(typeof(DeviceManagement), ex);
			}

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
				return (PathEx.Equals(devicePathFromMessage, devicePath));

			return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
