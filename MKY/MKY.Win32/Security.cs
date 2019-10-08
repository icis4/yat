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

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "type", Target = "MKY.Win32.Security+NativeTypes+SECURITY_IMPERSONATION_LEVEL")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Security+NativeTypes+SECURITY_IMPERSONATION_LEVEL", MessageId = "SECURITY")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Security+NativeTypes+SECURITY_IMPERSONATION_LEVEL", MessageId = "IMPERSONATION")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "MKY.Win32.Security+NativeTypes+SECURITY_IMPERSONATION_LEVEL", MessageId = "LEVEL")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 security API.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Type name is given by the Win32 API.")]
	public static class Security
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeTypes
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			public enum SECURITY_IMPERSONATION_LEVEL
			{
				/// <summary>
				/// The server process cannot obtain identification information about the client,
				/// and it cannot impersonate the client.
				/// </summary>
				/// <remarks>
				/// It is defined with no value given, and thus, by ANSI C rules, defaults to a value of zero.
				/// </remarks>
				SecurityAnonymous = 0,

				/// <summary>
				/// The server process can obtain information about the client, such as security
				/// identifiers and privileges, but it cannot impersonate the client. This is useful
				/// for servers that export their own objects, for example, database products that
				/// export tables and views. Using the retrieved client-security information, the
				/// server can make access-validation decisions without being able to use other
				/// services that are using the client's security context.
				/// </summary>
				SecurityIdentification = 1,

				/// <summary>
				/// The server process can impersonate the client's security context on its local
				/// system. The server cannot impersonate the client on remote systems.
				/// </summary>
				SecurityImpersonation = 2,

				/// <summary>
				/// The server process can impersonate the client's security context on remote systems.
				/// </summary>
				/// <remarks>
				/// Windows NT:  This impersonation level is not supported.
				/// </remarks>
				SecurityDelegation = 3,
			}
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
			private const string ADVANCED_DLL = "advapi32.dll";

			/// <summary></summary>
			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method implemented but then not needed, kept for potential future use.")]
			[DllImport(ADVANCED_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern Boolean ImpersonateSelf([In] NativeTypes.SECURITY_IMPERSONATION_LEVEL ImpersonationLevel);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
