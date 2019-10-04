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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+RECT.#bottom")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+RECT.#right")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+RECT.#left")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+RECT.#top")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#nMin")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#nPage")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#nPos")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#nTrackPos")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#nMax")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#fMask")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#cbSize")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO.#cbSize", MessageId = "cb")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#SetScrollInfo(System.IntPtr,System.Int32,MKY.Win32.Window+NativeTypes+SCROLLINFO&,System.Boolean)", MessageId = "lpsi")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#SetScrollInfo(System.IntPtr,System.Int32,MKY.Win32.Window+NativeTypes+SCROLLINFO&,System.Boolean)", MessageId = "hwnd")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#SetScrollInfo(System.IntPtr,System.Int32,MKY.Win32.Window+NativeTypes+SCROLLINFO&,System.Boolean)", MessageId = "f")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#ScrollWindowEx(System.IntPtr,System.Int32,System.Int32,MKY.Win32.Window+NativeTypes+RECT&,MKY.Win32.Window+NativeTypes+RECT&,System.IntPtr,MKY.Win32.Window+NativeTypes+RECT&,System.UInt32)", MessageId = "dy")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#ScrollWindowEx(System.IntPtr,System.Int32,System.Int32,MKY.Win32.Window+NativeTypes+RECT&,MKY.Win32.Window+NativeTypes+RECT&,System.IntPtr,MKY.Win32.Window+NativeTypes+RECT&,System.UInt32)", MessageId = "hrgn")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#ScrollWindowEx(System.IntPtr,System.Int32,System.Int32,MKY.Win32.Window+NativeTypes+RECT&,MKY.Win32.Window+NativeTypes+RECT&,System.IntPtr,MKY.Win32.Window+NativeTypes+RECT&,System.UInt32)", MessageId = "hwnd")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#ScrollWindowEx(System.IntPtr,System.Int32,System.Int32,MKY.Win32.Window+NativeTypes+RECT&,MKY.Win32.Window+NativeTypes+RECT&,System.IntPtr,MKY.Win32.Window+NativeTypes+RECT&,System.UInt32)", MessageId = "prc")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#GetScrollInfo(System.IntPtr,System.Int32,MKY.Win32.Window+NativeTypes+SCROLLINFO&)", MessageId = "lpsi")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeMethods.#GetScrollInfo(System.IntPtr,System.Int32,MKY.Win32.Window+NativeTypes+SCROLLINFO&)", MessageId = "hwnd")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#WM_HSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#WM_VSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_SCROLLCHILDREN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_INVALIDATE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_ERASE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_HORZ")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_VERT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_CTL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_BOTH")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINEUP")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINELEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINEDOWN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINERIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGEUP")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGELEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGEDOWN")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGERIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_THUMBPOSITION")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_THUMBTRACK")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_TOP")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_BOTTOM")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_RIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_ENDSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_RANGE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_PAGE")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_POS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_DISABLENOSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_TRACKPOS")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_ALL")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Window+NativeTypes+RECT", MessageId = "RECT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Window+NativeTypes+SCROLLINFO", MessageId = "SCROLLINFO")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#WM_HSCROLL", MessageId = "HSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#WM_VSCROLL", MessageId = "VSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_SCROLLCHILDREN", MessageId = "SCROLLCHILDREN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_INVALIDATE", MessageId = "INVALIDATE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SW_ERASE", MessageId = "ERASE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_HORZ", MessageId = "HORZ")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_VERT", MessageId = "VERT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_CTL", MessageId = "CTL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_BOTH", MessageId = "BOTH")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINEUP", MessageId = "LINEUP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINELEFT", MessageId = "LINELEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINEDOWN", MessageId = "LINEDOWN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LINERIGHT", MessageId = "LINERIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGEUP", MessageId = "PAGEUP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGELEFT", MessageId = "PAGELEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGEDOWN", MessageId = "PAGEDOWN")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_PAGERIGHT", MessageId = "PAGERIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_THUMBPOSITION", MessageId = "THUMBPOSITION")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_THUMBTRACK", MessageId = "THUMBTRACK")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_TOP", MessageId = "TOP")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_LEFT", MessageId = "LEFT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_BOTTOM", MessageId = "BOTTOM")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_RIGHT", MessageId = "RIGHT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SB_ENDSCROLL", MessageId = "ENDSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_RANGE", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_RANGE", MessageId = "RANGE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_PAGE", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_PAGE", MessageId = "PAGE")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_POS", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_POS", MessageId = "POS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_DISABLENOSCROLL", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_DISABLENOSCROLL", MessageId = "DISABLENOSCROLL")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_TRACKPOS", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_TRACKPOS", MessageId = "TRACKPOS")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_ALL", MessageId = "SIF")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Window+NativeConstants.#SIF_ALL", MessageId = "ALL")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API related to window management.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
	public static class Window
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

			/// <remarks>winuser.h and saying hello to StyleCop ;-.</remarks>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct SCROLLINFO
			{
				public UInt32 cbSize;
				public UInt32 fMask;
				public Int32  nMin;
				public Int32  nMax;
				public UInt32 nPage;
				public Int32  nPos;
				public Int32  nTrackPos;
			}

			/// <remarks>windef.h and saying hello to StyleCop ;-.</remarks>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct RECT
			{
				public Int64 left;
				public Int64 top;
				public Int64 right;
				public Int64 bottom;
			}

			#pragma warning restore 1591
		}

		#endregion

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		/// <remarks>winuser.h and saying hello to StyleCop ;-.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Name is given by the Win32 API.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeConstants
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			public const Int32 WM_HSCROLL = 0x114;
			public const Int32 WM_VSCROLL = 0x115;

			// Scroll flags:
			public const Int32 SW_SCROLLCHILDREN = 0x0001;
			public const Int32 SW_INVALIDATE     = 0x0002;
			public const Int32 SW_ERASE          = 0x0004;

			// Scroll bar constants:
			public const Int32 SB_HORZ = 0;
			public const Int32 SB_VERT = 1;
			public const Int32 SB_CTL  = 2;
			public const Int32 SB_BOTH = 3;

			// Scroll bar commands:
			public const Int32 SB_LINEUP        = 0;
			public const Int32 SB_LINELEFT      = 0;
			public const Int32 SB_LINEDOWN      = 1;
			public const Int32 SB_LINERIGHT     = 1;
			public const Int32 SB_PAGEUP        = 2;
			public const Int32 SB_PAGELEFT      = 2;
			public const Int32 SB_PAGEDOWN      = 3;
			public const Int32 SB_PAGERIGHT     = 3;
			public const Int32 SB_THUMBPOSITION = 4;
			public const Int32 SB_THUMBTRACK    = 5;
			public const Int32 SB_TOP           = 6;
			public const Int32 SB_LEFT          = 6;
			public const Int32 SB_BOTTOM        = 7;
			public const Int32 SB_RIGHT         = 7;
			public const Int32 SB_ENDSCROLL     = 8;

			// Scroll bar messages:
			public const Int32 SIF_RANGE           = 0x0001;
			public const Int32 SIF_PAGE            = 0x0002;
			public const Int32 SIF_POS             = 0x0004;
			public const Int32 SIF_DISABLENOSCROLL = 0x0008;
			public const Int32 SIF_TRACKPOS        = 0x0010;
			public const Int32 SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

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
			private const string USER_DLL = "user32.dll";

			/// <summary>
			/// The GetScrollInfo function retrieves the parameters of a scroll bar, including the minimum
			/// and maximum scrolling positions, the page size, and the position of the scroll box (thumb).
			/// </summary>
			/// <param name="hwnd">
			/// Handle to a scroll bar control or a window with a standard scroll bar, depending on the
			/// value of the <paramref name="fnBar"/> parameter.
			/// </param>
			/// <param name="fnBar">
			/// Specifies the type of scroll bar for which to retrieve parameters. This parameter can be one of the following values:
			/// - <see cref="NativeConstants.SB_CTL"/>  Retrieves the parameters for a scroll bar control. The <paramref name="hwnd"/> parameter must be the handle to the scroll bar control.
			/// - <see cref="NativeConstants.SB_HORZ"/> Retrieves the parameters for the window's standard horizontal scroll bar.
			/// - <see cref="NativeConstants.SB_VERT"/> Retrieves the parameters for the window's standard vertical scroll bar.
			/// </param>
			/// <param name="lpsi">
			/// Pointer to a <see cref="NativeTypes.SCROLLINFO"/> structure. Before calling GetScrollInfo,
			/// set the cbSize member to sizeof(SCROLLINFO), and set the fMask member to specify the scroll
			/// bar parameters to retrieve. Before returning, the function copies the specified parameters
			/// to the appropriate members of the structure.
			/// The fMask member can be one or more of the following values:
			/// - <see cref="NativeConstants.SIF_PAGE"/>     Copies the scroll page to the nPage member of the SCROLLINFO structure pointed to by lpsi.
			/// - <see cref="NativeConstants.SIF_POS"/>      Copies the scroll position to the nPos member of the SCROLLINFO structure pointed to by lpsi.
			/// - <see cref="NativeConstants.SIF_RANGE"/>    Copies the scroll range to the nMin and nMax members of the SCROLLINFO structure pointed to by lpsi.
			/// - <see cref="NativeConstants.SIF_TRACKPOS"/> Copies the current scroll box tracking position to the nTrackPos member of the SCROLLINFO structure pointed to by lpsi.
			/// </param>
			/// <returns>
			/// If the function retrieved any values, the return value is nonzero.
			/// If the function does not retrieve any values, the return value is zero.
			/// To get extended error information, call <see cref="WinError.GetLastErrorCode"/>.
			/// </returns>
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetScrollInfo([In] IntPtr hwnd, [In] Int32 fnBar, [In] ref NativeTypes.SCROLLINFO lpsi);

			/// <summary>
			/// The SetScrollInfo function sets the parameters of a scroll bar, including the minimum
			/// and maximum scrolling positions, the page size, and the position of the scroll box (thumb).
			/// The function also redraws the scroll bar, if requested.
			/// </summary>
			/// <param name="hwnd">
			/// Handle to a scroll bar control or a window with a standard scroll bar, depending on the
			/// value of the <paramref name="fnBar"/> parameter.
			/// </param>
			/// <param name="fnBar">
			/// Specifies the type of scroll bar for which to retrieve parameters. This parameter can be one of the following values:
			/// - <see cref="NativeConstants.SB_CTL"/>  Retrieves the parameters for a scroll bar control. The <paramref name="hwnd"/> parameter must be the handle to the scroll bar control.
			/// - <see cref="NativeConstants.SB_HORZ"/> Retrieves the parameters for the window's standard horizontal scroll bar.
			/// - <see cref="NativeConstants.SB_VERT"/> Retrieves the parameters for the window's standard vertical scroll bar.
			/// </param>
			/// <param name="lpsi">
			/// Pointer to a <see cref="NativeTypes.SCROLLINFO"/> structure. Before calling GetScrollInfo,
			/// set the cbSize member to sizeof(SCROLLINFO), and set the fMask member to specify the scroll
			/// bar parameters to retrieve. Before returning, the function copies the specified parameters
			/// to the appropriate members of the structure.
			/// The fMask member can be one or more of the following values:
			/// - <see cref="NativeConstants.SIF_DISABLENOSCROLL"/> Disables the scroll bar instead of removing it, if the scroll bar's new parameters make the scroll bar unnecessary.
			/// - <see cref="NativeConstants.SIF_PAGE"/>  Sets the scroll page to the value specified in the nPage member of the SCROLLINFO structure pointed to by lpsi.
			/// - <see cref="NativeConstants.SIF_POS"/>   Sets the scroll position to the value specified in the nPos member of the SCROLLINFO structure pointed to by lpsi.
			/// - <see cref="NativeConstants.SIF_RANGE"/> Sets the scroll range to the value specified in the nMin and nMax members of the SCROLLINFO structure pointed to by lpsi.
			/// </param>
			/// <param name="fRedraw">
			/// Specifies whether the scroll bar is redrawn to reflect the changes to the scroll bar. If this parameter is <c>true</c>, the scroll bar is redrawn, otherwise, it is not redrawn.
			/// </param>
			/// <returns>
			/// The return value is the current position of the scroll box.
			/// </returns>
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 SetScrollInfo([In] IntPtr hwnd, [In] Int32 fnBar, [In] ref NativeTypes.SCROLLINFO lpsi, [In, MarshalAs(UnmanagedType.Bool)] bool fRedraw);

			/// <summary>
			/// The ScrollWindowEx function scrolls the contents of the specified window's client area.
			/// </summary>
			/// <param name="hwnd">Handle to the window where the client area is to be scrolled.</param>
			/// <param name="dx">
			/// Specifies the amount, in device units, of horizontal scrolling.
			/// This parameter must be a negative value to scroll to the left.
			/// </param>
			/// <param name="dy">
			/// Specifies the amount, in device units, of vertical scrolling.
			/// This parameter must be a negative value to scroll up.
			/// </param>
			/// <param name="prcScroll">
			/// Pointer to a RECT structure that specifies the portion of the client area to be scrolled.
			/// If this parameter is <c>null</c>, the entire client area is scrolled.
			/// </param>
			/// <param name="prcClip">
			/// Pointer to a RECT structure that contains the coordinates of the clipping rectangle.
			/// Only device bits within the clipping rectangle are affected. Bits scrolled from the
			/// outside of the rectangle to the inside are painted; bits scrolled from the inside of
			/// the rectangle to the outside are not painted. This parameter may be <c>null</c>.
			/// </param>
			/// <param name="hrgnUpdate">
			/// Handle to the region that is modified to hold the region invalidated by scrolling.
			/// This parameter may be <c>null</c>.
			/// </param>
			/// <param name="prcUpdate">
			/// Pointer to a RECT structure that receives the boundaries of the rectangle invalidated
			/// by scrolling. This parameter may be <c>null</c>.
			/// </param>
			/// <param name="flags">
			/// Specifies flags that control scrolling. This parameter can be a combination of the
			/// following values:
			/// - <see cref="NativeConstants.SW_ERASE"/>          Erases the newly invalidated region by sending a WM_ERASEBKGND message to the window when specified with the SW_INVALIDATE flag.
			/// - <see cref="NativeConstants.SW_INVALIDATE"/>     Invalidates the region identified by the hrgnUpdate parameter after scrolling.
			/// - <see cref="NativeConstants.SW_SCROLLCHILDREN"/> Scrolls all child windows that intersect the rectangle pointed to by the prcScroll parameter. The child windows are scrolled by the number of pixels specified by the dx and dy parameters. The system sends a WM_MOVE message to all child windows that intersect the prcScroll rectangle, even if they do not move.
			/// </param>
			/// <returns>
			/// If the function succeeds, the return value is SIMPLEREGION (rectangular invalidated
			/// region), COMPLEXREGION (nonrectangular invalidated region; overlapping rectangles),
			/// or NULLREGION (no invalidated region).
			/// If the function fails, the return value is ERROR. To get extended error information,
			/// call <see cref="WinError.LastErrorToString"/>.
			/// </returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' is used by the underlying Win32 'user32.dll'.")]
			[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 ScrollWindowEx([In] IntPtr hwnd, [In] Int32 dx, [In] Int32 dy, [In] ref NativeTypes.RECT prcScroll, [In] ref NativeTypes.RECT prcClip, [In] IntPtr hrgnUpdate, [Out] out NativeTypes.RECT prcUpdate, [In] UInt32 flags);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
