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
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Threading;

using Microsoft.Win32.SafeHandles;

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
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#LastAccessTime")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#FileAttributes")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#VolumeSerialNumber")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#FileSizeLow")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#NumberOfLinks")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#CreationTime")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#LastWriteTime")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#FileSizeHigh")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#FileIndexHigh")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION.#FileIndexLow")]

// Justification = "Type is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES.#bInheritHandle")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES.#lpSecurityDescriptor")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES.#nLength")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#GetOverlappedResult(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr,System.Int32&,System.Boolean)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#GetOverlappedResult(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr,System.Int32&,System.Boolean)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CancelIo(Microsoft.Win32.SafeHandles.SafeFileHandle)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CreateEvent(System.IntPtr,System.Boolean,System.Boolean,System.String)", MessageId = "b")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#WaitForSingleObject(System.IntPtr,System.UInt32)", MessageId = "dw")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#WaitForSingleObject(System.IntPtr,System.UInt32)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#WriteFile(Microsoft.Win32.SafeHandles.SafeFileHandle,System.Byte[],System.Int32&,System.Threading.NativeOverlapped&)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#ReadFile(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr,System.Int32,System.Int32&,System.Threading.NativeOverlapped&)", MessageId = "n")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#ReadFile(Microsoft.Win32.SafeHandles.SafeFileHandle,System.IntPtr,System.Int32,System.Int32&,System.Threading.NativeOverlapped&)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CreateFile(System.String,MKY.Win32.FileIO+NativeTypes+Access,MKY.Win32.FileIO+NativeTypes+ShareMode,System.IntPtr,MKY.Win32.FileIO+NativeTypes+CreationDisposition,MKY.Win32.FileIO+NativeTypes+AttributesAndFlags,System.IntPtr)", MessageId = "dw")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CreateFile(System.String,MKY.Win32.FileIO+NativeTypes+Access,MKY.Win32.FileIO+NativeTypes+ShareMode,System.IntPtr,MKY.Win32.FileIO+NativeTypes+CreationDisposition,MKY.Win32.FileIO+NativeTypes+AttributesAndFlags,System.IntPtr)", MessageId = "h")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#GetFileInformationByHandle(Microsoft.Win32.SafeHandles.SafeFileHandle,MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION&)", MessageId = "h")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_EXECUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#QUERY_ONLY")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ_WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_ALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_ALWAYS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_NEW")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_EXISTING")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_ALWAYS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#TRUNCATE_EXISTING")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_DELETE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_NONE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ_WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_ALL")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_HIDDEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_COMPRESSED")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_RANDOM_ACCESS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_SEQUENTIAL_SCAN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_OFFLINE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_REPARSE_POINT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_NO_RECALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ARCHIVE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_TEMPORARY")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SYSTEM")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NOT_CONTENT_INDEXED")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NORMAL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SPARSE_FILE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_READONLY")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OVERLAPPED")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DEVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_VIRTUAL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_DELETE_ON_CLOSE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_POSIX_SEMANTICS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DIRECTORY")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ENCRYPTED")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_WRITE_THROUGH")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_NO_BUFFERING")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_BACKUP_SEMANTICS")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_EXECUTE", MessageId = "EXECUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_EXECUTE", MessageId = "GENERIC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#QUERY_ONLY", MessageId = "QUERY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#QUERY_ONLY", MessageId = "ONLY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ_WRITE", MessageId = "READ")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ_WRITE", MessageId = "WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ_WRITE", MessageId = "GENERIC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_WRITE", MessageId = "WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_WRITE", MessageId = "GENERIC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_ALL", MessageId = "ALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_ALL", MessageId = "GENERIC")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ", MessageId = "READ")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+Access.#GENERIC_READ", MessageId = "GENERIC")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_HIDDEN", MessageId = "HIDDEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_HIDDEN", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_COMPRESSED", MessageId = "COMPRESSED")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_COMPRESSED", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_RANDOM_ACCESS", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_RANDOM_ACCESS", MessageId = "RANDOM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_RANDOM_ACCESS", MessageId = "ACCESS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_SEQUENTIAL_SCAN", MessageId = "SEQUENTIAL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_SEQUENTIAL_SCAN", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_SEQUENTIAL_SCAN", MessageId = "SCAN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_OFFLINE", MessageId = "OFFLINE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_OFFLINE", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_REPARSE_POINT", MessageId = "REPARSE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_REPARSE_POINT", MessageId = "POINT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_REPARSE_POINT", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_NO_RECALL", MessageId = "RECALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_NO_RECALL", MessageId = "OPEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_NO_RECALL", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#NONE", MessageId = "NONE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ARCHIVE", MessageId = "ARCHIVE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ARCHIVE", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_TEMPORARY", MessageId = "TEMPORARY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_TEMPORARY", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SYSTEM", MessageId = "SYSTEM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SYSTEM", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NOT_CONTENT_INDEXED", MessageId = "INDEXED")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NOT_CONTENT_INDEXED", MessageId = "NOT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NOT_CONTENT_INDEXED", MessageId = "CONTENT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NOT_CONTENT_INDEXED", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NORMAL", MessageId = "NORMAL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_NORMAL", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE", MessageId = "INSTANCE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE", MessageId = "PIPE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE", MessageId = "FIRST")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SPARSE_FILE", MessageId = "SPARSE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SPARSE_FILE", MessageId = "FILE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_SPARSE_FILE", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_READONLY", MessageId = "READONLY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_READONLY", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OVERLAPPED", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OVERLAPPED", MessageId = "OVERLAPPED")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DEVICE", MessageId = "DEVICE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DEVICE", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT", MessageId = "REPARSE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT", MessageId = "POINT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT", MessageId = "OPEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_VIRTUAL", MessageId = "VIRTUAL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_VIRTUAL", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_DELETE_ON_CLOSE", MessageId = "DELETE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_DELETE_ON_CLOSE", MessageId = "CLOSE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_DELETE_ON_CLOSE", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_POSIX_SEMANTICS", MessageId = "POSIX")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_POSIX_SEMANTICS", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_POSIX_SEMANTICS", MessageId = "SEMANTICS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DIRECTORY", MessageId = "DIRECTORY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_DIRECTORY", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ENCRYPTED", MessageId = "ATTRIBUTE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#ATTRIBUTE_ENCRYPTED", MessageId = "ENCRYPTED")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_WRITE_THROUGH", MessageId = "WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_WRITE_THROUGH", MessageId = "THROUGH")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_WRITE_THROUGH", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_NO_BUFFERING", MessageId = "BUFFERING")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_NO_BUFFERING", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_BACKUP_SEMANTICS", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_BACKUP_SEMANTICS", MessageId = "SEMANTICS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_BACKUP_SEMANTICS", MessageId = "BACKUP")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_ALWAYS", MessageId = "CREATE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_ALWAYS", MessageId = "ALWAYS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_NEW", MessageId = "NEW")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#CREATE_NEW", MessageId = "CREATE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_EXISTING", MessageId = "EXISTING")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_EXISTING", MessageId = "OPEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_ALWAYS", MessageId = "ALWAYS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#OPEN_ALWAYS", MessageId = "OPEN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#TRUNCATE_EXISTING", MessageId = "EXISTING")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#TRUNCATE_EXISTING", MessageId = "TRUNCATE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+CreationDisposition.#APPEND", MessageId = "APPEND")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_DELETE", MessageId = "DELETE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_DELETE", MessageId = "SHARE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_NONE", MessageId = "NONE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_NONE", MessageId = "SHARE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ_WRITE", MessageId = "READ")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ_WRITE", MessageId = "SHARE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ_WRITE", MessageId = "WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ", MessageId = "READ")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_READ", MessageId = "SHARE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_WRITE", MessageId = "SHARE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_WRITE", MessageId = "WRITE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_ALL", MessageId = "ALL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+ShareMode.#SHARE_ALL", MessageId = "SHARE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION", MessageId = "INFORMATION")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION", MessageId = "FILE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+BY_HANDLE_FILE_INFORMATION", MessageId = "HANDLE")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES", MessageId = "SECURITY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+SECURITY_ATTRIBUTES", MessageId = "ATTRIBUTES")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CreateEvent(System.IntPtr,System.Boolean,System.Boolean,System.String)", MessageId = "Security")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CancelIo(Microsoft.Win32.SafeHandles.SafeFileHandle)", MessageId = "Io")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeMethods.#CreateFile(System.String,MKY.Win32.FileIO+NativeTypes+Access,MKY.Win32.FileIO+NativeTypes+ShareMode,System.IntPtr,MKY.Win32.FileIO+NativeTypes+CreationDisposition,MKY.Win32.FileIO+NativeTypes+AttributesAndFlags,System.IntPtr)", MessageId = "Flags")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "type", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags", MessageId = "Flags")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_RANDOM_ACCESS", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_SEQUENTIAL_SCAN", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_NO_RECALL", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_FIRST_PIPE_INSTANCE", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OVERLAPPED", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_OPEN_REPARSE_POINT", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_DELETE_ON_CLOSE", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_POSIX_SEMANTICS", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_WRITE_THROUGH", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_NO_BUFFERING", MessageId = "FLAG")]
[module: SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Scope = "member", Target = "MKY.Win32.FileIO+NativeTypes+AttributesAndFlags.#FLAG_BACKUP_SEMANTICS", MessageId = "FLAG")]

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

			/// <summary>
			/// Encapsulates Win32 GENERIC_ file access flags into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			[Flags]
			public enum Access : uint
			{
				GENERIC_READ       = 0x80000000,
				GENERIC_WRITE      = 0x40000000,
				GENERIC_EXECUTE    = 0x20000000,
				GENERIC_ALL        = 0x10000000,

				GENERIC_READ_WRITE = 0xC0000000,

				QUERY_ONLY         = 0x00000000
			}

			/// <summary>
			/// Encapsulates Win32 FILE_SHARE_ file share mode flags into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by the Win32 API.")]
			[CLSCompliant(false)]
			[Flags]
			public enum ShareMode : uint
			{
				SHARE_NONE       = 0x00000000,
				SHARE_READ       = 0x00000001,
				SHARE_WRITE      = 0x00000002,
				SHARE_DELETE     = 0x00000004,

				SHARE_READ_WRITE = 0x00000003,
				SHARE_ALL        = 0x00000007
			}

			/// <summary>
			/// Replicates Win32 creation disposition selectors into a C# enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			public enum CreationDisposition
			{
				CREATE_NEW        = System.IO.FileMode.CreateNew,
				CREATE_ALWAYS     = System.IO.FileMode.Create,
				OPEN_EXISTING     = System.IO.FileMode.Open,
				OPEN_ALWAYS       = System.IO.FileMode.OpenOrCreate,
				TRUNCATE_EXISTING = System.IO.FileMode.Truncate,
				APPEND            = System.IO.FileMode.Append
			}

			/// <summary>
			/// Encapsulates Win32 FILE_ATTRIBUTE_ and FILE_FLAG_ values into a C# flag enum.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[CLSCompliant(false)]
			[Flags]
			public enum AttributesAndFlags : uint
			{
				NONE                          = System.IO.FileOptions.None,

				[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "READONLY", Justification = "Item name is given by the Win32 API.")]
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
				FLAG_FIRST_PIPE_INSTANCE      = 0x00080000
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[StructLayout(LayoutKind.Sequential)]
			public struct SECURITY_ATTRIBUTES
			{
				public Int32 nLength;
				public Int32 lpSecurityDescriptor;
				public Int32 bInheritHandle;
			}

			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			/// <summary>
			/// Cancels a call to ReadFile.
			/// </summary>
			/// <param name="hFile">The device handle.</param>
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean CancelIo([In] SafeFileHandle hFile);

			/// <summary>
			/// Creates an event object for the overlapped structure used with ReadFile.
			/// </summary>
			/// <param name="SecurityAttributes">A security attributes structure or <see cref="IntPtr.Zero"/>.</param>
			/// <param name="bManualReset">Manual Reset = False (The system automatically resets the
			/// state to non-signaled after a waiting thread has been released.).</param>
			/// <param name="bInitialState">Initial state = False (Not signaled.).</param>
			/// <param name="lpName">An event object name (optional).</param>
			/// <returns>A handle to the event object.</returns>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3", Justification = "'CharSet.Auto' will automatically marshal strings appropriately for the target operating system.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr CreateEvent([In] IntPtr SecurityAttributes, [In, MarshalAs(UnmanagedType.Bool)] bool bManualReset, [In, MarshalAs(UnmanagedType.Bool)] bool bInitialState, [In] string lpName);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0", Justification = "'CharSet.Auto' will automatically marshal strings appropriately for the target operating system.")]
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern SafeFileHandle CreateFile([In] string lpFileName, [In] NativeTypes.Access dwDesiredAccess, [In] NativeTypes.ShareMode dwShareMode, [In] IntPtr lpSecurityAttributes, [In] NativeTypes.CreationDisposition dwCreationDisposition, [In] NativeTypes.AttributesAndFlags dwFlagsAndAttributes, [In] IntPtr hTemplateFile);

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetFileInformationByHandle([In] SafeFileHandle hFile, [Out] out NativeTypes.BY_HANDLE_FILE_INFORMATION lpFileInformation);

			/// <summary>
			/// Gets the result of an overlapped operation.
			/// </summary>
			/// <param name="hFile">A device handle returned by CreateFile.</param>
			/// <param name="lpOverlapped">A pointer to an overlapped structure.</param>
			/// <param name="lpNumberOfBytesTransferred">A pointer to a variable to hold the number of bytes read.</param>
			/// <param name="bWait">False to return immediately.</param>
			/// <returns>Non-zero on success and the number of bytes read.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			public static bool GetOverlappedResult(SafeFileHandle hFile, IntPtr lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait)
			{
				UInt32 bytesTransferred;
				bool success = GetOverlappedResult(hFile, lpOverlapped, out bytesTransferred, bWait);
				lpNumberOfBytesTransferred = (int)bytesTransferred;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean GetOverlappedResult([In] SafeFileHandle hFile, [In] IntPtr lpOverlapped, [Out] out UInt32 lpNumberOfBytesTransferred, [In, MarshalAs(UnmanagedType.Bool)] bool bWait);

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
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Function signature is given by the Win32 API.")]
			public static bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, ref NativeOverlapped lpOverlapped)
			{
				UInt32 bytesRead;
				bool success = ReadFile(hFile, lpBuffer, (UInt32)nNumberOfBytesToRead, out bytesRead, ref lpOverlapped);
				lpNumberOfBytesRead = (int)bytesRead;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean ReadFile([In] SafeFileHandle hFile, [Out] IntPtr lpBuffer, [In] UInt32 nNumberOfBytesToRead, [Out] out UInt32 lpNumberOfBytesRead, [In] ref NativeOverlapped lpOverlapped);

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
			/// <returns><c>true</c> on success, <c>false</c> on failure.</returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Function signature is given by the Win32 API.")]
			public static bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, out int lpNumberOfBytesWritten, ref NativeOverlapped lpOverlapped)
			{
				UInt32 bytesWritten;
				bool success = WriteFile(hFile, lpBuffer, (UInt32)lpBuffer.Length, out bytesWritten, ref lpOverlapped);
				lpNumberOfBytesWritten = (int)bytesWritten;
				return (success);
			}

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean WriteFile([In] SafeFileHandle hFile, [In] byte[] lpBuffer, [In] UInt32 nNumberOfBytesToWrite, [Out] out UInt32 lpNumberOfBytesWritten, [In] ref NativeOverlapped lpOverlapped);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
