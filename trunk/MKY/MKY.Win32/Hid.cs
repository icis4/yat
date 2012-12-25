//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "The structure is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberInputDataIndices")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberFeatureDataIndices")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#Reserved")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#FeatureReportByteLength")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#Usage")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberOutputValueCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberInputValueCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#InputReportByteLength")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberFeatureValueCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberFeatureButtonCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberInputButtonCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberOutputDataIndices")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#OutputReportByteLength")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberOutputButtonCaps")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#NumberLinkCollectionNodes")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS.#UsagePage")]

// Justification = "The structure is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#ReportID")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved5")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved4")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#StringMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#DataIndexMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#LinkCollection")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#UsageMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#HasNull")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#LogicalMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#DesignatorMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved2")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#LinkUsagePage")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#IsAlias")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#IsAbsolute")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#PhysicalMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#UsagePage")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#IsDesignatorRange")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#DataIndexMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#BitSize")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#BitField")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#IsStringRange")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#StringMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved3")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#ReportCount")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#LinkUsage")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#Reserved6")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#DesignatorMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#PhysicalMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#IsRange")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#UsageMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS.#LogicalMin")]

// Justification = "The structure is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES.#VendorID")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES.#ProductID")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES.#VersionNumber")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES.#Size")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32&)", MessageId = "Num")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetCaps(System.IntPtr,MKY.Win32.Hid+NativeTypes+HIDP_CAPS&)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPreparsedData(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr&)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_FreePreparsedData(System.IntPtr)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32)", MessageId = "Num")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetValueCaps(MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE,MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS&,System.IntPtr)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS.#HidP_InvalidPreparsedData", MessageId = "Preparsed")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE.#HidP_Input")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE.#HidP_Feature")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE.#HidP_Output")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetAttributes(Microsoft.Win32.SafeHandles.SafeFileHandle,MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetCaps(System.IntPtr,MKY.Win32.Hid+NativeTypes+HIDP_CAPS&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetManufacturerString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetOutputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPreparsedData(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPhysicalDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetProductString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_FreePreparsedData(System.IntPtr)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetHidGuid(System.Guid&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_FlushQueue(Microsoft.Win32.SafeHandles.SafeFileHandle)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetIndexedString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Int32,System.String&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialNumberString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetInputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetValueCaps(MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE,MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS&,System.IntPtr)")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS.#HidP_Success")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS.#HidP_InvalidPreparsedData")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES", MessageId = "ATTRIBUTES")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES", MessageId = "HIDD")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS", MessageId = "CAPS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_CAPS", MessageId = "HIDP")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE", MessageId = "HIDP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE", MessageId = "TYPE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE", MessageId = "REPORT")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32&)", MessageId = "Number")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetAttributes(Microsoft.Win32.SafeHandles.SafeFileHandle,MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES&)", MessageId = "Attributes")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetAttributes(Microsoft.Win32.SafeHandles.SafeFileHandle,MKY.Win32.Hid+NativeTypes+HIDD_ATTRIBUTES&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetCaps(System.IntPtr,MKY.Win32.Hid+NativeTypes+HIDP_CAPS&)", MessageId = "Capabilities")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetCaps(System.IntPtr,MKY.Win32.Hid+NativeTypes+HIDP_CAPS&)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetManufacturerString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Manufacturer")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetManufacturerString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetOutputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetOutputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Report")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPreparsedData(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPreparsedData(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr&)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Report")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPhysicalDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Buffer")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetPhysicalDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetProductString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Product")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetProductString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_FreePreparsedData(System.IntPtr)", MessageId = "Preparsed")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetHidGuid(System.Guid&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_FlushQueue(Microsoft.Win32.SafeHandles.SafeFileHandle)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetIndexedString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Int32,System.String&)", MessageId = "String")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetIndexedString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Int32,System.String&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetIndexedString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Int32,System.String&)", MessageId = "Indexed")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_SetNumInputBuffers(Microsoft.Win32.SafeHandles.SafeFileHandle,System.UInt32)", MessageId = "Number")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetFeature(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Report")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialNumberString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialNumberString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Serial")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetInputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetInputReport(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[])", MessageId = "Report")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetValueCaps(MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE,MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS&,System.IntPtr)", MessageId = "Value")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetValueCaps(MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE,MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS&,System.IntPtr)", MessageId = "Report")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidP_GetValueCaps(MKY.Win32.Hid+NativeTypes+HIDP_REPORT_TYPE,MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS&,System.IntPtr)", MessageId = "Preparsed")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS", MessageId = "HIDP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_STATUS", MessageId = "STATUS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS", MessageId = "CAPS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS", MessageId = "HIDP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Hid+NativeTypes+HIDP_VALUE_CAPS", MessageId = "VALUE")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API for HID communications.
	/// </summary>
	/// <remarks>
	/// This class is partly based on GenericHid of Jan Axelson's Lakeview Research.
	/// Visit GenericHid on http://www.lvr.com/hidpage.htm for details.
	/// <see cref="MKY.Win32.Hid"/> needs to modify the structure and contents of
	/// GenericHid due to the following reasons:
	/// - Suboptimal structure of the original GenericHid project
	/// - Missing features required for YAT
	/// - Potential reuse of this class for other services directly using the Win32 API
	/// </remarks>
	public static class Hid
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
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
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Not performance critical.")]
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
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Indices is a correct English term and used throughout the .NET framework.")]
				public Int16 NumberInputDataIndices;
				public Int16 NumberOutputButtonCaps;
				public Int16 NumberOutputValueCaps;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Indices is a correct English term and used throughout the .NET framework.")]
				public Int16 NumberOutputDataIndices;
				public Int16 NumberFeatureButtonCaps;
				public Int16 NumberFeatureValueCaps;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Indices is a correct English term and used throughout the .NET framework.")]
				public Int16 NumberFeatureDataIndices;
			}

			/// <summary>
			/// The HIDP_REPORT_TYPE enumeration type is used to specify a HID report type.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			public enum HIDP_REPORT_TYPE
			{
				HidP_Input   = 0,
				HidP_Output  = 1,
				HidP_Feature = 2,
			}

			/// <summary>
			/// The HIDP_REPORT_TYPE enumeration type is used to specify a HID report type.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
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
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[StructLayout(LayoutKind.Sequential)]
			public struct HIDP_VALUE_CAPS
			{
				public Int16 UsagePage;
				public byte  ReportID;
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
				public byte  Reserved;
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeMethods
		{
			private const string HID_DLL = "hid.dll";

			/// <summary>
			/// Removes any Input reports waiting in the buffer.
			/// </summary>
			/// <remarks>
			/// Public via <see cref="FlushQueue"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to the device.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_FlushQueue([In] SafeFileHandle HidDeviceObject);

			/// <summary>
			/// Frees the buffer reserved by HidD_GetPreparsedData.
			/// </summary>
			/// <param name="PreparsedData">A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_FreePreparsedData([In] IntPtr PreparsedData);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_GetAttributes([In] SafeFileHandle HidDeviceObject, [In, Out] ref NativeTypes.HIDD_ATTRIBUTES Attributes);

			// HidD_GetConfiguration() is reserved for internal system use

			/// <summary>
			/// Attempts to read a Feature report from the device.
			/// </summary>
			/// <param name="HidDeviceObject">A handle to an HID.</param>
			/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			public static bool HidD_GetFeature(SafeFileHandle HidDeviceObject, byte[] ReportBuffer)
			{
				return (HidD_GetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetFeature([In] SafeFileHandle HidDeviceObject, [Out] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <remarks>
			/// Public via <see cref="GetHidGuid()"/>.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern void HidD_GetHidGuid([In, Out] ref Guid HidGuid);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			public static bool HidD_GetIndexedString(SafeFileHandle HidDeviceObject, int StringIndex, out string IndexedString)
			{
				StringBuilder s = new StringBuilder(Usb.Descriptors.MaximumStringDescriptorCharLength);
				if (HidD_GetIndexedString(HidDeviceObject, (UInt32)StringIndex, s, (UInt32)s.Capacity))
				{
					IndexedString = s.ToString();
					return (true);
				}
				IndexedString = "";
				return (false);
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetIndexedString([In] SafeFileHandle HidDeviceObject, [In] UInt32 StringIndex, [Out] StringBuilder Buffer, [In] UInt32 BufferLength);

			/// <summary>
			/// Attempts to read an Input report from the device using a control transfer.
			/// </summary>
			/// <remarks>
			/// Supported under Windows XP and later only. Also applies to <see cref="HidD_SetOutputReport(SafeFileHandle, byte[])"/>.
			/// Public via <see cref="GetInputReport"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to an HID.</param>
			/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			public static bool HidD_GetInputReport(SafeFileHandle HidDeviceObject, byte[] ReportBuffer)
			{
				return (HidD_GetInputReport(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetInputReport([In] SafeFileHandle HidDeviceObject, [In, Out] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			public static bool HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, out string Manufacturer)
			{
				StringBuilder s = new StringBuilder(Usb.Descriptors.MaximumStringDescriptorCharLength);
				if (HidD_GetManufacturerString(HidDeviceObject, s, (UInt32)s.Capacity))
				{
					Manufacturer = s.ToString();
					return (true);
				}
				Manufacturer = "";
				return (false);
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetManufacturerString([In] SafeFileHandle HidDeviceObject, [Out] StringBuilder Buffer, [In] UInt32 BufferLength);

			// HidD_GetMsGenreDescriptor() is reserved for internal system use

			/// <summary>
			/// Retrieves the number of Input reports the host can store.
			/// </summary>
			/// <remarks>
			/// Not supported by Windows 98 Standard Edition.
			/// If the buffer is full and another report arrives, the host drops the oldest report.
			/// Public via <see cref="GetNumberOfInputBuffers"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to a device and an integer to hold the number of buffers.</param>
			/// <param name="NumberBuffers"><c>true</c> on success, <c>false</c> on failure.</param>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_GetNumInputBuffers([In] SafeFileHandle HidDeviceObject, [Out] out UInt32 NumberBuffers);

			/// <summary></summary>
			public static bool HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, byte[] Buffer)
			{
				return (HidD_GetPhysicalDescriptor(HidDeviceObject, Buffer, (UInt32)Buffer.Length));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetPhysicalDescriptor([In] SafeFileHandle HidDeviceObject, [Out] byte[] Buffer, [In] UInt32 BufferLength);

			/// <summary>
			/// Retrieves a pointer to a buffer containing information about the device's capabilities.
			/// HidP_GetCaps and other API functions require a pointer to the buffer.
			/// </summary>
			/// <param name="HidDeviceObject">A handle returned by CreateFile.</param>
			/// <param name="PreparsedData">A pointer to a buffer.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_GetPreparsedData([In] SafeFileHandle HidDeviceObject, [Out] out IntPtr PreparsedData);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			public static bool HidD_GetProductString(SafeFileHandle HidDeviceObject, out string Product)
			{
				StringBuilder s = new StringBuilder(Usb.Descriptors.MaximumStringDescriptorCharLength);
				if (HidD_GetProductString(HidDeviceObject, s, (UInt32)s.Capacity))
				{
					Product = s.ToString();
					return (true);
				}
				Product = "";
				return (false);
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetProductString([In] SafeFileHandle HidDeviceObject, [Out] StringBuilder Buffer, [In] UInt32 BufferLength);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
			public static bool HidD_GetSerialNumberString(SafeFileHandle HidDeviceObject, out string SerialNumber)
			{
				StringBuilder s = new StringBuilder(Usb.Descriptors.MaximumStringDescriptorCharLength);
				if (HidD_GetSerialNumberString(HidDeviceObject, s, (UInt32)s.Capacity))
				{
					SerialNumber = s.ToString();
					return (true);
				}
				SerialNumber = "";
				return (false);
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_GetSerialNumberString([In] SafeFileHandle HidDeviceObject, [Out] StringBuilder Buffer, [In] UInt32 BufferLength);

			// HidD_SetConfiguration() is reserved for internal system use

			/// <summary>
			/// Attempts to send a Feature report to the device.
			/// </summary>
			/// <param name="HidDeviceObject">A handle to a HID.</param>
			/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			public static bool HidD_SetFeature(SafeFileHandle HidDeviceObject, byte[] ReportBuffer)
			{
				return (HidD_SetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_SetFeature([In] SafeFileHandle HidDeviceObject, [In] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary>
			/// Sets the number of Input reports the host can store.
			/// </summary>
			/// <remarks>
			/// If the buffer is full and another report arrives, the host drops the oldest report.
			/// Public via <see cref="SetNumberOfInputBuffers"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to an HID.</param>
			/// <param name="NumberBuffers">An integer to hold the number of buffers.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool HidD_SetNumInputBuffers([In] SafeFileHandle HidDeviceObject, [In] UInt32 NumberBuffers);

			/// <summary>
			/// Attempts to send an Output report to the device using a control transfer.
			/// </summary>
			/// <remarks>
			/// Requires Windows XP or later. Also applies to <see cref="HidD_GetInputReport(SafeFileHandle, byte[])"/>.
			/// Public via <see cref="SetOutputReport"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to an HID.</param>
			/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			public static bool HidD_SetOutputReport(SafeFileHandle HidDeviceObject, byte[] ReportBuffer)
			{
				return (HidD_SetOutputReport(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool HidD_SetOutputReport([In] SafeFileHandle HidDeviceObject, [In] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary>
			/// Find out a device's capabilities. For standard devices such as joysticks, you can find
			/// out the specific capabilities of the device. For a custom device where the software
			/// knows what the device is capable of, this call may be unneeded.
			/// </summary>
			/// <param name="PreparsedData">A pointer returned by HidD_GetPreparsedData.</param>
			/// <param name="Capabilities">A pointer to a HIDP_CAPS structure.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 HidP_GetCaps([In] IntPtr PreparsedData, [In, Out] ref NativeTypes.HIDP_CAPS Capabilities);

			/// <summary>
			/// Retrieves a buffer containing an array of HidP_ValueCaps structures. Each structure
			/// defines the capabilities of one value. This application doesn't use this data.
			/// </summary>
			/// <param name="ReportType">A report type enumerator from hidpi.h.</param>
			/// <param name="ValueCaps">A pointer to a buffer for the returned array.</param>
			/// <param name="PreparsedData"> A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[CLSCompliant(false)]
			public static NativeTypes.HIDP_STATUS HidP_GetValueCaps(NativeTypes.HIDP_REPORT_TYPE ReportType, ref NativeTypes.HIDP_VALUE_CAPS ValueCaps, IntPtr PreparsedData)
			{
				UInt32 ValueCapsLength = (UInt32)Marshal.SizeOf(typeof(NativeTypes.HIDP_VALUE_CAPS));
				return (HidP_GetValueCaps(ReportType, ref ValueCaps, ref ValueCapsLength, PreparsedData));
			}

			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			private static extern NativeTypes.HIDP_STATUS HidP_GetValueCaps([In] NativeTypes.HIDP_REPORT_TYPE ReportType, [In, Out] ref NativeTypes.HIDP_VALUE_CAPS ValueCaps, [In] ref UInt32 ValueCapsLength, [In] IntPtr PreparsedData);
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

		/// <summary>
		/// String descriptor types.
		/// </summary>
		private enum StringDescriptorIndex
		{
			LanguageIds  = 0,
			Manufacturer = 1,
			Product      = 2,
			SerialNumber = 3,
		}

		#pragma warning restore 1591

		private delegate bool GetHidStringDelegate(SafeFileHandle deviceHandle, out string hidString);

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Returns the GUID associated with USB HID.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Emphasizes that this is a call to underlying system functions.")]
		public static Guid GetHidGuid()
		{
			Guid hidGuid = new Guid();
			NativeMethods.HidD_GetHidGuid(ref hidGuid);
			return (hidGuid);
		}

		/// <summary>
		/// Creates a device handle of the HID device at the given device path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool CreateSharedQueryOnlyDeviceHandle(string devicePath, out SafeFileHandle deviceHandle)
		{
			SafeFileHandle h = FileIO.NativeMethods.CreateFile
				(
				devicePath,
				FileIO.NativeTypes.Access.QUERY_ONLY,
				FileIO.NativeTypes.ShareMode.SHARE_READ_WRITE,
				IntPtr.Zero,
				FileIO.NativeTypes.CreationDisposition.OPEN_EXISTING,
				FileIO.NativeTypes.AttributesAndFlags.NONE,
				IntPtr.Zero
				);

			if (!h.IsInvalid)
			{
				deviceHandle = h;
				return (true);
			}

			System.Diagnostics.Debug.WriteLine("USB HID couldn't create shared device query handle:");
			System.Diagnostics.Debug.Indent();
			System.Diagnostics.Debug.WriteLine("Path = " + devicePath);
			System.Diagnostics.Debug.WriteLine("System error message = " + Debug.GetLastError());
			System.Diagnostics.Debug.Unindent();

			deviceHandle = null;
			return (false);
		}

		/// <summary>
		/// Creates a device handle of the HID device at the given system path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool CreateSharedReadWriteHandle(string devicePath, out SafeFileHandle readHandle)
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
				readHandle = h;
				return (true);
			}

			System.Diagnostics.Debug.WriteLine("USB HID couldn't create shared device read/write handle.");
			System.Diagnostics.Debug.Indent();
			System.Diagnostics.Debug.WriteLine("Path = " + devicePath);
			System.Diagnostics.Debug.WriteLine("System error message = " + Debug.GetLastError());
			System.Diagnostics.Debug.Unindent();

			readHandle = null;
			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetManufacturerString(SafeFileHandle deviceHandle, out string manufacturer)
		{
			return (GetString(deviceHandle, NativeMethods.HidD_GetManufacturerString, out manufacturer));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetProductString(SafeFileHandle deviceHandle, out string product)
		{
			return (GetString(deviceHandle, NativeMethods.HidD_GetProductString, out product));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetSerialNumberString(SafeFileHandle deviceHandle, out string serialNumber)
		{
			return (GetString(deviceHandle, NativeMethods.HidD_GetSerialNumberString, out serialNumber));
		}

		/// <summary>
		/// Gets one of the standard strings (manufacturer, product, serial number).
		/// </summary>
		/// <remarks>
		/// \fixme (2010-03-14 / mky):
		/// Don't know how to retrieve culture specific strings based on language ID. Simply return "".
		/// Seems like HID.dll doesn't support to retrieve culture specific strings. WinUSB.dll does,
		/// however, WinUSB.dll can only be used in combination with devices that provide a WinUSB.dll
		/// based driver.
		/// Considerations
		/// - How many languages are available? Retrieve language IDs at index 0.
		/// - How are the indices mapped to the languages? Device descriptor returns indices for the strings.
		/// - How can culture specific strings be accessed? There must be something like SetDescriptor()/GetDescriptor()
		///   that takes an index and a text ID as argument.
		/// </remarks>
		private static bool GetString(SafeFileHandle deviceHandle, GetHidStringDelegate method, out string hidString)
		{
			if (!deviceHandle.IsInvalid)
			{
				// Retrieve language IDs at index 0.
				string languageString;
				if (NativeMethods.HidD_GetIndexedString(deviceHandle, (int)StringDescriptorIndex.LanguageIds, out languageString))
				{
					// Retrieve content string.
					string contentString;
					if (method(deviceHandle, out contentString)) // GetManufacturerString() or GetProductString() or GetSerialNumberString().
					{
						if (contentString != languageString) // Looks like a proper invariant string.
						{
							hidString = contentString;
							return (true);
						}
						else // contentString == languageString means that content isn't available and index 0 has be retrieved.
						{
							hidString = "";
							return (true);
						}
					}
				}
			}
			hidString = "";
			return (false);
		}

		/// <summary>
		/// Retrieves the number of Input reports the host can store.
		/// </summary>
		/// <remarks>
		/// Windows 98 Standard Edition does not support the following:
		/// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
		/// - HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		/// </remarks>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <param name="numberOfInputBuffers">An integer to hold the returned value.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetNumberOfInputBuffers(SafeFileHandle deviceHandle, out int numberOfInputBuffers)
		{
			bool success = false;
			if (!Version.IsWindows98Standard)
			{
				UInt32 numberBuffers;
				success = NativeMethods.HidD_GetNumInputBuffers(deviceHandle, out numberBuffers);
				numberOfInputBuffers = (int)numberBuffers;
			}
			else
			{
				// Under Windows 98 Standard Edition, the number of buffers is fixed at 2.
				numberOfInputBuffers = 2;
				success = true;
			}
			return (success);
		}

		/// <summary>
		/// Sets the number of input reports the host will store.
		/// </summary>
		/// <remarks>
		/// Windows 98 Standard Edition does not support the following:
		/// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
		/// - HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		/// </remarks>
		/// <param name="deviceHandle">A handle to the device.</param>
		/// <param name="numberOfInputBuffers">The requested number of input reports.</param>
		/// <returns>True on success. False on failure.</returns>
		public static bool SetNumberOfInputBuffers(SafeFileHandle deviceHandle, int numberOfInputBuffers)
		{
			if (!Version.IsWindows98Standard)
				return (NativeMethods.HidD_SetNumInputBuffers(deviceHandle, (UInt32)numberOfInputBuffers));
			else
				return (false); // Not supported under Windows 98 Standard Edition.
		}

		/// <summary>
		/// Attempts to read an Input report from the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Supported under Windows XP and later only. Also applies to <see cref="SetOutputReport"/>.
		/// Public via <see cref="GetInputReport"/>.
		/// </remarks>
		/// <param name="deviceHandle">A handle to an HID.</param>
		/// <param name="reportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		public static bool GetInputReport(SafeFileHandle deviceHandle, byte[] reportBuffer)
		{
			if (!Version.IsWindowsXpOrLater)
				return (NativeMethods.HidD_GetInputReport(deviceHandle, reportBuffer));
			else
				return (false); // Not supported before Windows XP.
		}

		/// <summary>
		/// Attempts to send an Output report to the device using a control transfer.
		/// </summary>
		/// <remarks>
		/// Requires Windows XP or later. Also applies to <see cref="GetInputReport"/>.
		/// Public via <see cref="SetOutputReport"/>.
		/// </remarks>
		/// <param name="deviceHandle">A handle to an HID.</param>
		/// <param name="reportBuffer">A pointer to a buffer containing the report ID and report.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		public static bool SetOutputReport(SafeFileHandle deviceHandle, byte[] reportBuffer)
		{
			if (!Version.IsWindowsXpOrLater)
				return (NativeMethods.HidD_SetOutputReport(deviceHandle, reportBuffer));
			else
				return (false); // Not supported before Windows XP.
		}

		/// <summary>
		/// Remove any input reports waiting in the buffer.
		/// </summary>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		public static bool FlushQueue(SafeFileHandle deviceHandle)
		{
			bool success = NativeMethods.HidD_FlushQueue(deviceHandle);
			return (success);
		}

		/// <summary>
		/// Retrieves a structure with information about a device's capabilities. 
		/// </summary>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <param name="capabilities">An HIDP_CAPS structure.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		public static bool GetDeviceCapabilities(SafeFileHandle deviceHandle, ref NativeTypes.HIDP_CAPS capabilities)
		{
			IntPtr pPreparsedData = new IntPtr();

			if (!NativeMethods.HidD_GetPreparsedData(deviceHandle, out pPreparsedData))
				return (false);

			if ((NativeMethods.HidP_GetCaps(pPreparsedData, ref capabilities) == 0))
				return (false);

			System.Diagnostics.Debug.WriteLine("USB device capabilities:");
			System.Diagnostics.Debug.Indent();
			System.Diagnostics.Debug.WriteLine("Usage (hex):                     " + capabilities.Usage    .ToString("X2", NumberFormatInfo.InvariantInfo));
			System.Diagnostics.Debug.WriteLine("Usage Page (hex):                " + capabilities.UsagePage.ToString("X2", NumberFormatInfo.InvariantInfo));
			System.Diagnostics.Debug.WriteLine("Input Report byte Length:        " + capabilities.InputReportByteLength);
			System.Diagnostics.Debug.WriteLine("Output Report byte Length:       " + capabilities.OutputReportByteLength);
			System.Diagnostics.Debug.WriteLine("Feature Report byte Length:      " + capabilities.FeatureReportByteLength);
			System.Diagnostics.Debug.WriteLine("Number of Link Collection Nodes: " + capabilities.NumberLinkCollectionNodes);
			System.Diagnostics.Debug.WriteLine("Number of Input Button Caps:     " + capabilities.NumberInputButtonCaps);
			System.Diagnostics.Debug.WriteLine("Number of Input Value Caps:      " + capabilities.NumberInputValueCaps);
			System.Diagnostics.Debug.WriteLine("Number of Input Data Indices:    " + capabilities.NumberInputDataIndices);
			System.Diagnostics.Debug.WriteLine("Number of Output Button Caps:    " + capabilities.NumberOutputButtonCaps);
			System.Diagnostics.Debug.WriteLine("Number of Output Value Caps:     " + capabilities.NumberOutputValueCaps);
			System.Diagnostics.Debug.WriteLine("Number of Output Data Indices:   " + capabilities.NumberOutputDataIndices);
			System.Diagnostics.Debug.WriteLine("Number of Feature Button Caps:   " + capabilities.NumberFeatureButtonCaps);
			System.Diagnostics.Debug.WriteLine("Number of Feature Value Caps:    " + capabilities.NumberFeatureValueCaps);
			System.Diagnostics.Debug.WriteLine("Number of Feature Data Indices:  " + capabilities.NumberFeatureDataIndices);
			System.Diagnostics.Debug.Unindent();

			// \remind (2010-03-21 / mky):
			// The following two lines demonstrate how the devices value capabilities can be retrieved.
			// However, due to some reaseon HidP_GetValueCaps() overwrites 'deviceHandle'. Before
			// making use of the following lines, ensure that 'deviceHandle' isn't overwritten anymore.
			//
			//HIDP_VALUE_CAPS valueCaps = new HIDP_VALUE_CAPS();
			//HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, ref valueCaps, preparsedData);

			return (true);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
