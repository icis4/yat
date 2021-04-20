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
// MKY Version 1.0.30
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
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.User+NativeTypes+LASTINPUTINFO.#cbSize")]
[module: SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Scope = "member", Target = "MKY.Win32.User+NativeTypes+LASTINPUTINFO.#dwTime")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.User+NativeTypes+LASTINPUTINFO.#cbSize", MessageId = "cb")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.User+NativeTypes+LASTINPUTINFO.#dwTime", MessageId = "dw")]
[module: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Scope = "member", Target = "MKY.Win32.User+NativeMethods.#GetLastInputInfo(MKY.Win32.User+NativeTypes+LASTINPUTINFO&)", MessageId = "plii")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.User+NativeTypes+LASTINPUTINFO", MessageId = "LASTINPUTINFO")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API related to window management.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Don't care about suboptimal documentation of Win32 API items.")]
	public static class User
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
			public struct LASTINPUTINFO
			{
				public UInt32 cbSize;
				public UInt32 dwTime;
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
			private const string USER_DLL = "user32.dll";

			/// <summary>
			/// Retrieves the time of the last input event..
			/// </summary>
			/// <param name="plii">
			/// A pointer to a LASTINPUTINFO structure that receives the time of the last input event.
			/// </param>
			/// <returns>
			/// If the function succeeds, the return value is nonzero.
			/// If the function fails, the return value is zero.
			/// </returns>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Function signature is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Interoperability", "CA1401:PInvokesShouldNotBeVisible", Justification = "Method is encapsulated in Win32 specific assembly.")]
			[CLSCompliant(false)]
			[DllImport(USER_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetLastInputInfo([Out] out NativeTypes.LASTINPUTINFO plii);
		}

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary>
		/// Uses the <see cref="NativeMethods.GetLastInputInfo"/> API function to retrieve the last
		/// user input and calculates the corresponding idle time.
		/// </summary>
		/// <returns>The idle time.</returns>
		public static TimeSpan IdleTime
		{
			get
			{
				NativeTypes.LASTINPUTINFO lastInputInfo = new NativeTypes.LASTINPUTINFO();
				lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
				lastInputInfo.dwTime = 0;

				if (NativeMethods.GetLastInputInfo(out lastInputInfo))
				{
					uint lastInputTime = lastInputInfo.dwTime;
					long idleTime = Environment.TickCount - lastInputTime;

					if (idleTime > 0)
						return (TimeSpan.FromMilliseconds(idleTime));
				}

				return (TimeSpan.Zero);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
