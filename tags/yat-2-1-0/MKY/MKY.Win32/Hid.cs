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
// MKY Version 1.0.27
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of string access:
////#define DEBUG_STRING_ACCESS

	// Enable debugging of string access:
////#define DEBUG_STRING_ACCESS_MESSAGEBOXES

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

using MKY.Windows.Forms;

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

// Justification = "Type is defined by the Win32 API."
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

// Justification = "Type is defined by the Win32 API."
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
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)")]
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
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Hid")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Hid+NativeMethods.#HidD_GetSerialString(Microsoft.Win32.SafeHandles.SafeFileHandle,System.String&)", MessageId = "Serial")]
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
	/// This class needed to modify the original structure and contents of GenericHid
	/// due to the following reasons:
	/// - Suboptimal structure of the original GenericHid project
	/// - Missing features required for YAT
	/// - Potential reuse of this class for other services directly using the Win32 API
	/// DeviceManagement and Hid types and methods have been split into separate classes.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct HIDD_ATTRIBUTES
			{
				public UInt32 Size; // ULONG = 32 bits
				public UInt16 VendorID; // USHORT = 16 bits
				public UInt16 ProductID; // USHORT
				public UInt16 VersionNumber; // USHORT
			}

			// HIDD_CONFIGURATION is reserved for internal system use

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct HIDP_CAPS
			{       // USAGE = USHORT = 16 bits
				public UInt16 Usage;     // Attention: The Win32 HIDP_CAPS structure contains 'Usage' before 'UsagePage', even though it should be vice-versa from a USB logic point of view!
				public UInt16 UsagePage; // Attention: The Win32 HIDP_CAPS structure contains 'UsagePage' after 'Usage', even though it should be vice-versa from a USB logic point of view!
				public UInt16 InputReportByteLength; // All USHORT = 16 bits
				public UInt16 OutputReportByteLength;
				public UInt16 FeatureReportByteLength;
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
				public UInt16[] Reserved;
				public UInt16 NumberLinkCollectionNodes;
				public UInt16 NumberInputButtonCaps;
				public UInt16 NumberInputValueCaps;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
				public UInt16 NumberInputDataIndices;
				public UInt16 NumberOutputButtonCaps;
				public UInt16 NumberOutputValueCaps;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
				public UInt16 NumberOutputDataIndices;
				public UInt16 NumberFeatureButtonCaps;
				public UInt16 NumberFeatureValueCaps;
				[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
				public UInt16 NumberFeatureDataIndices;
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
			/// The HIDP_STATUS enumeration type is used to specify a HID report type.
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
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct HIDP_VALUE_CAPS
			{
				public UInt16 UsagePage;      // USAGE = USHORT = 16 bits
				public byte   ReportID;       // UCHAR
				public int    IsAlias;        // BOOLEAN
				public UInt16 BitField;       // USHORT = 16 bits
				public UInt16 LinkCollection; //   "
				public UInt16 LinkUsage;      // USAGE = USHORT = 16 bits
				public UInt16 LinkUsagePage;  //   "
				public int    IsRange;           // BOOLEAN
				public int    IsStringRange;     //    "
				public int    IsDesignatorRange; //    "
				public int    IsAbsolute;        //    "
				public int    HasNull;           //    "
				public byte   Reserved;    // UCHAR
				public UInt16 BitSize;     // USHORT = 16 bits
				public UInt16 ReportCount; //   "
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
				public UInt16 Reserved2;   // USHORT
				public Int32 LogicalMin;   // LONG
				public Int32 LogicalMax;   //  "
				public Int32 PhysicalMin;  //  "
				public Int32 PhysicalMax;  //  "
				public UInt16 UsageMin;    // USAGE = USHORT = 16 bits
				public UInt16 UsageMax;    //   "
				public UInt16 StringMin;   // USHORT = 16 bits
				public UInt16 StringMax;   //   "
				public UInt16 DesignatorMin; // USHORT = 16 bits
				public UInt16 DesignatorMax; //   "
				public UInt16 DataIndexMin;  //   "
				public UInt16 DataIndexMax;  //   "
			}

			#pragma warning restore 1591
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
			private const string HID_DLL = "hid.dll";

			/// <summary>
			/// Safe buffer length for retrieving descriptor string via native Win32 API.
			/// </summary>
			/// <remarks>
			/// 2 x <see cref="Usb.Descriptors.MaxStringDescriptorCharLength"/> + 2 x '\0' would
			/// result in 254;
			/// 2 x <see cref="Usb.Descriptors.MaxStringDescriptorCharLength"/> + 1 x '\0' would
			/// result in 253.
			/// However, a buffer length of 254 or 253 may lead to weird results like:
			///  > HID language IDs string is "WindowsForms10.STATIC.app.0.3ee13a2"
			///  > HID content string is "SysTabControl32"
			///  > HID content string is "file"
			/// And in case of wireless Microsoft keyboards, such buffer lengths will result in "".
			/// https://www.pinvoke.net/default.aspx/hid.HidD_GetManufacturerString as well as
			/// https://www.pinvoke.net/default.aspx/hid.HidD_GetSerialNumberString propose a buffer
			/// of 128 bytes. While this conflicts with the USB specification (the maximum string
			/// length is 126 wide characters (not including the terminating NULL character)) only
			/// that value properly works!
			///
			/// Saying hello to StyleCop ;-.
			/// </remarks>
			private const int SafeStringDescriptorBufferLength = 128;

			/// <summary>
			/// Removes any Input reports waiting in the buffer.
			/// </summary>
			/// <remarks>
			/// Public via <see cref="FlushQueue"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to the device.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_FlushQueue([In] SafeFileHandle HidDeviceObject);

			/// <summary>
			/// Frees the buffer reserved by <see cref="HidD_GetPreparsedData"/>.
			/// </summary>
			/// <param name="PreparsedData">A pointer to the pre-parsed data structure returned by <see cref="HidD_GetPreparsedData"/>.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_FreePreparsedData([In] IntPtr PreparsedData);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_GetAttributes([In] SafeFileHandle HidDeviceObject, [In, Out] ref NativeTypes.HIDD_ATTRIBUTES Attributes);

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

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetFeature([In] SafeFileHandle HidDeviceObject, [Out] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <remarks>
			/// Public via <see cref="GetHidGuid()"/>.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern void HidD_GetHidGuid([In, Out] ref Guid HidGuid);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			public static bool HidD_GetIndexedString(SafeFileHandle HidDeviceObject, int StringIndex, out string IndexedString)
			{
				byte[] buffer = new byte[SafeStringDescriptorBufferLength]; // See remarks at constant!];
				if (HidD_GetIndexedString(HidDeviceObject, (UInt32)StringIndex, buffer, (UInt32)buffer.Length))
				{
					var s = Encoding.Default.GetString(Encoding.Convert(Encoding.Unicode, Encoding.Default, buffer));
					IndexedString = s.Remove(s.IndexOf('\0'));
					return (true);
				}
				IndexedString = "";
				return (false);
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetIndexedString([In] SafeFileHandle HidDeviceObject, [In] UInt32 StringIndex, [Out] byte[] Buffer, [In] UInt32 BufferLength);

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

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetInputReport([In] SafeFileHandle HidDeviceObject, [In, Out] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			public static bool HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, out string Manufacturer)
			{
				byte[] buffer = new byte[SafeStringDescriptorBufferLength]; // See remarks at constant!];
				if (HidD_GetManufacturerString(HidDeviceObject, buffer, (UInt32)buffer.Length))
				{
					var s = Encoding.Default.GetString(Encoding.Convert(Encoding.Unicode, Encoding.Default, buffer));
					Manufacturer = s.Remove(s.IndexOf('\0'));
					return (true);
				}
				Manufacturer = "";
				return (false);
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetManufacturerString([In] SafeFileHandle HidDeviceObject, [Out] byte[] Buffer, [In] UInt32 BufferLength);

			// HidD_GetMsGenreDescriptor() is reserved for internal system use

			/// <summary>
			/// Retrieves the number of input reports the host can store.
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
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_GetNumInputBuffers([In] SafeFileHandle HidDeviceObject, [Out] out UInt32 NumberBuffers);

			/// <summary></summary>
			public static bool HidD_GetPhysicalDescriptor(SafeFileHandle HidDeviceObject, byte[] Buffer)
			{
				return (HidD_GetPhysicalDescriptor(HidDeviceObject, Buffer, (UInt32)Buffer.Length));
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetPhysicalDescriptor([In] SafeFileHandle HidDeviceObject, [Out] byte[] Buffer, [In] UInt32 BufferLength);

			/// <summary>
			/// Retrieves a pointer to a buffer containing information about the device's capabilities.
			/// HidP_GetCaps and other API functions require a pointer to the buffer.
			/// </summary>
			/// <param name="HidDeviceObject">A handle returned by CreateFile.</param>
			/// <param name="PreparsedData">A pointer to a buffer.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_GetPreparsedData([In] SafeFileHandle HidDeviceObject, [Out] out IntPtr PreparsedData);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			public static bool HidD_GetProductString(SafeFileHandle HidDeviceObject, out string Product)
			{
				byte[] buffer = new byte[SafeStringDescriptorBufferLength]; // See remarks at constant!];
				if (HidD_GetProductString(HidDeviceObject, buffer, (UInt32)buffer.Length))
				{
					var s = Encoding.Default.GetString(Encoding.Convert(Encoding.Unicode, Encoding.Default, buffer));
					Product = s.Remove(s.IndexOf('\0'));
					return (true);
				}
				Product = "";
				return (false);
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetProductString([In] SafeFileHandle HidDeviceObject, [Out] byte[] Buffer, [In] UInt32 BufferLength);

			/// <remarks>
			/// USB specifies that serial is a string, not limited to a number!
			///
			/// Saying hello to StyleCop ;-.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			public static bool HidD_GetSerialString(SafeFileHandle HidDeviceObject, out string Serial)
			{
				byte[] buffer = new byte[SafeStringDescriptorBufferLength]; // See remarks at constant!];
				if (HidD_GetSerialNumberString(HidDeviceObject, buffer, (UInt32)buffer.Length))
				{
					var s = Encoding.Default.GetString(Encoding.Convert(Encoding.Unicode, Encoding.Default, buffer));
					Serial = s.Remove(s.IndexOf('\0'));
					return (true);
				}
				Serial = "";
				return (false);
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_GetSerialNumberString([In] SafeFileHandle HidDeviceObject, [Out] byte[] Buffer, [In] UInt32 BufferLength);

			// HidD_SetConfiguration() is reserved for internal system use

			/// <summary>
			/// Attempts to send a feature report to the device.
			/// </summary>
			/// <param name="HidDeviceObject">A handle to a HID.</param>
			/// <param name="ReportBuffer">A pointer to a buffer containing the report ID and report.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			public static bool HidD_SetFeature(SafeFileHandle HidDeviceObject, byte[] ReportBuffer)
			{
				return (HidD_SetFeature(HidDeviceObject, ReportBuffer, (UInt32)ReportBuffer.Length));
			}

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_SetFeature([In] SafeFileHandle HidDeviceObject, [In] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary>
			/// Sets the number of input reports the host can store.
			/// </summary>
			/// <remarks>
			/// If the buffer is full and another report arrives, the host drops the oldest report.
			/// Public via <see cref="SetNumberOfInputBuffers"/>.
			/// </remarks>
			/// <param name="HidDeviceObject">A handle to an HID.</param>
			/// <param name="NumberBuffers">An integer to hold the number of buffers.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean HidD_SetNumInputBuffers([In] SafeFileHandle HidDeviceObject, [In] UInt32 NumberBuffers);

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

			[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return", Justification = "Return is actually marshalled as 'UnmanagedType.Bool' which should be good enough.")]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean HidD_SetOutputReport([In] SafeFileHandle HidDeviceObject, [In] byte[] ReportBuffer, [In] UInt32 ReportBufferLength);

			/// <summary>
			/// Find out a device's capabilities. For standard devices such as joysticks, you can find
			/// out the specific capabilities of the device. For a custom device where the software
			/// knows what the device is capable of, this call may be unneeded.
			/// </summary>
			/// <param name="PreparsedData">A pointer returned by <see cref="HidD_GetPreparsedData"/>.</param>
			/// <param name="Capabilities">A pointer to a HIDP_CAPS structure.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(HID_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 HidP_GetCaps([In] IntPtr PreparsedData, [In, Out] ref NativeTypes.HIDP_CAPS Capabilities);

			/// <summary>
			/// Retrieves a buffer containing an array of HidP_ValueCaps structures. Each structure
			/// defines the capabilities of one value. This application doesn't use this data.
			/// </summary>
			/// <param name="ReportType">A report type enumerator from hidpi.h.</param>
			/// <param name="ValueCaps">A pointer to a buffer for the returned array.</param>
			/// <param name="PreparsedData"> A pointer to the pre-parsed data structure returned by <see cref="HidD_GetPreparsedData"/>.</param>
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
		/// <remarks>
		/// Replicated in 'IO.Usb.StringDescriptorIndex' for less coupling to this assembly.
		/// </remarks>
		private enum StringDescriptorIndex
		{
			LanguageIds  = 0,
			Manufacturer = 1,
			Product      = 2,
			Serial       = 3,
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

			Debug.WriteLine("USB HID couldn't create shared device query handle:");
			Debug.Indent();
			Debug.WriteLine("Path = " + devicePath);
			Debug.WriteLine(WinError.LastErrorToString());
			Debug.Unindent();

			deviceHandle = null;
			return (false);
		}

		/// <summary>
		/// Creates a device handle of the HID device at the given system path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool CreateSharedReadHandle(string devicePath, out SafeFileHandle handle)
		{
			SafeFileHandle h = FileIO.NativeMethods.CreateFile
			(
				devicePath,
				FileIO.NativeTypes.Access.GENERIC_READ,
				FileIO.NativeTypes.ShareMode.SHARE_READ,
				IntPtr.Zero,
				FileIO.NativeTypes.CreationDisposition.OPEN_EXISTING,
				FileIO.NativeTypes.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
			);

			if (!h.IsInvalid)
			{
				handle = h;
				return (true);
			}

			Debug.WriteLine("USB HID couldn't create shared device read handle.");
			Debug.Indent();
			Debug.WriteLine("Path = " + devicePath);
			Debug.WriteLine(WinError.LastErrorToString());
			Debug.Unindent();

			handle = null;
			return (false);
		}

		/// <summary>
		/// Creates a device handle of the HID device at the given system path.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool CreateSharedReadWriteHandle(string devicePath, out SafeFileHandle handle)
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
				handle = h;
				return (true);
			}

			Debug.WriteLine("USB HID couldn't create shared device read/write handle.");
			Debug.Indent();
			Debug.WriteLine("Path = " + devicePath);
			Debug.WriteLine(WinError.LastErrorToString());
			Debug.Unindent();

			handle = null;
			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		public static bool GetManufacturerString(SafeFileHandle deviceHandle, out string manufacturer)
		{
			DebugStringAccessMessageBoxes("Retrieving manufacturer string via hid.dll::HidD_GetManufacturerString...");

			bool success = GetString(deviceHandle, NativeMethods.HidD_GetManufacturerString, out manufacturer);

			DebugStringAccessMessageBoxes(success, @"...successfully retrieved """ + manufacturer + @""".", "...failed to retrieve manufacturer string.");

			return (success);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		public static bool GetProductString(SafeFileHandle deviceHandle, out string product)
		{
			DebugStringAccessMessageBoxes("Retrieving product string via hid.dll::HidD_GetProductString...");

			bool success = GetString(deviceHandle, NativeMethods.HidD_GetProductString, out product);

			DebugStringAccessMessageBoxes(success, @"...successfully retrieved """ + product + @""".", "...failed to retrieve product string.");

			return (success);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		public static bool GetSerialString(SafeFileHandle deviceHandle, out string serial)
		{
			DebugStringAccessMessageBoxes("Retrieving serial string via hid.dll::HidD_GetSerialString...");

			bool success = GetString(deviceHandle, NativeMethods.HidD_GetSerialString, out serial);

			DebugStringAccessMessageBoxes(success, @"...successfully retrieved """ + serial + @""".", "...failed to retrieve serial string.");

			return (success);
		}

		/// <summary>
		/// Gets one of the standard strings (manufacturer, product, serial).
		/// </summary>
		/// <remarks>
		/// \fixme (2010-03-14 / MKY):
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with .dll?")]
		private static bool GetString(SafeFileHandle deviceHandle, GetHidStringDelegate method, out string hidString)
		{
			if (!deviceHandle.IsInvalid)
			{
				// Retrieve language IDs at index 0:
				string languageString;
				if (NativeMethods.HidD_GetIndexedString(deviceHandle, (int)StringDescriptorIndex.LanguageIds, out languageString))
				{
					DebugStringAccess(@"HID language IDs string is """ + languageString + @"""."); // Typically "Љ" or "".

					// \remind (2019-08-22 / MKY) bug #465
					// Retrieving USB HID string only properly works on "Љ".
					// Don't know yet how to retrieve other language strings.

					// Retrieve content string:
					string contentString;
					if (method(deviceHandle, out contentString)) // GetManufacturerString() or GetProductString() or GetSerialString().
					{
						DebugStringAccess(@"HID content string is """ + contentString + @""".");

						if (!string.IsNullOrEmpty(contentString) && (contentString != languageString)) // Looks like a proper invariant string.
						{
							DebugStringAccess(@"HID content string successfully retrieved.");

							hidString = contentString;
							return (true);
						}
						else // contentString == languageString means that content isn't available and index 0 has been retrieved.
						{
							DebugStringAccess(@"HID content string could be retrieved but is invalid.");

							hidString = "";
							return (true);
						}
					}
				}
			}

			DebugStringAccess(@"HID content string could not be retrieved!");

			hidString = "";
			return (false);
		}

		/// <summary>
		/// Retrieves the number of input reports the host can store.
		/// </summary>
		/// <remarks>
		/// Windows 98 Standard Edition does not support the following:
		/// - Interrupt OUT transfers (WriteFile uses control transfers and Set_Report)
		/// - <see cref="NativeMethods.HidD_GetNumInputBuffers"/> and <see cref="NativeMethods.HidD_SetNumInputBuffers"/>
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <param name="deviceHandle">A handle to a device.</param>
		/// <param name="numberOfInputBuffers">An integer to hold the returned value.</param>
		/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static bool GetNumberOfInputBuffers(SafeFileHandle deviceHandle, out int numberOfInputBuffers)
		{
			bool success = false;
			if (!EnvironmentEx.IsWindows98Standard)
			{
				uint numberBuffers;
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
		/// - <see cref="NativeMethods.HidD_GetNumInputBuffers"/> and <see cref="NativeMethods.HidD_SetNumInputBuffers"/>
		/// (Not yet tested on a Windows 98 Standard Edition system.)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		/// <param name="deviceHandle">A handle to the device.</param>
		/// <param name="numberOfInputBuffers">The requested number of input reports.</param>
		/// <returns>True on success. False on failure.</returns>
		public static bool SetNumberOfInputBuffers(SafeFileHandle deviceHandle, int numberOfInputBuffers)
		{
			if (!EnvironmentEx.IsWindows98Standard)
				return (NativeMethods.HidD_SetNumInputBuffers(deviceHandle, (uint)numberOfInputBuffers));
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
			if (!EnvironmentEx.IsWindowsXpOrLater)
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
			if (!EnvironmentEx.IsWindowsXpOrLater)
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
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Emphasize occurrence of an native pointer.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
		[CLSCompliant(false)]
		public static bool GetDeviceCapabilities(SafeFileHandle deviceHandle, ref NativeTypes.HIDP_CAPS capabilities)
		{
			IntPtr pPreparsedData = new IntPtr();

			if (!NativeMethods.HidD_GetPreparsedData(deviceHandle, out pPreparsedData))
				return (false);

			if ((NativeMethods.HidP_GetCaps(pPreparsedData, ref capabilities) == 0))
				return (false);

			Debug.WriteLine("USB HID device capabilities:");
			Debug.Indent();
			Debug.WriteLine("Usage page:                    0x" + capabilities.UsagePage.ToString("X4", CultureInfo.InvariantCulture));
			Debug.WriteLine("Usage ID:                      0x" + capabilities.Usage    .ToString("X4", CultureInfo.InvariantCulture));
			Debug.WriteLine("Input report byte length:        " + capabilities.InputReportByteLength);
			Debug.WriteLine("Output report byte length:       " + capabilities.OutputReportByteLength);
			Debug.WriteLine("Feature report byte length:      " + capabilities.FeatureReportByteLength);
			Debug.WriteLine("Number of link collection nodes: " + capabilities.NumberLinkCollectionNodes);
			Debug.WriteLine("Number of input button caps:     " + capabilities.NumberInputButtonCaps);
			Debug.WriteLine("Number of input value caps:      " + capabilities.NumberInputValueCaps);
			Debug.WriteLine("Number of input data indices:    " + capabilities.NumberInputDataIndices);
			Debug.WriteLine("Number of output button caps:    " + capabilities.NumberOutputButtonCaps);
			Debug.WriteLine("Number of output value caps:     " + capabilities.NumberOutputValueCaps);
			Debug.WriteLine("Number of output data indices:   " + capabilities.NumberOutputDataIndices);
			Debug.WriteLine("Number of feature button caps:   " + capabilities.NumberFeatureButtonCaps);
			Debug.WriteLine("Number of feature value caps:    " + capabilities.NumberFeatureValueCaps);
			Debug.WriteLine("Number of feature data indices:  " + capabilities.NumberFeatureDataIndices);
			Debug.Unindent();

			// \remind (2010-03-21 / MKY)
			// The following two lines demonstrate how the device's value capabilities can be retrieved.
			// However, due to some reason HidP_GetValueCaps() overwrites 'deviceHandle'.
			// Before making use of the following lines, ensure that 'deviceHandle' isn't overwritten anymore.
			//
		////HIDP_VALUE_CAPS valueCaps = new HIDP_VALUE_CAPS();
		////HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, ref valueCaps, preparsedData);

			return (true);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_STRING_ACCESS")]
		private static void DebugStringAccess(string message)
		{
			Debug.WriteLine(message);
		}

		[Conditional("DEBUG_STRING_ACCESS_MESSAGEBOXES")]
		private static void DebugStringAccessMessageBoxes(string message)
		{
			MessageBoxEx.Show(message, "Debug Message");
		}

		[Conditional("DEBUG_STRING_ACCESS_MESSAGEBOXES")]
		private static void DebugStringAccessMessageBoxes(bool condition, string messageIf, string messageElse)
		{
			if (condition)
				DebugStringAccessMessageBoxes(messageIf);
			else
				DebugStringAccessMessageBoxes(messageElse);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
