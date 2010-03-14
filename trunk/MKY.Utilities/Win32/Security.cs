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
using System.Runtime.InteropServices;

#endregion

namespace MKY.Utilities.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 security API.
	/// </summary>
	public static class Security
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
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

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string ADVANCED_DLL = "advapi32.dll";

		#endregion

		#region External Functions
		//==========================================================================================
		// External Functions
		//==========================================================================================

		/// <summary></summary>
		[DllImport(ADVANCED_DLL, SetLastError = true)]
		public static extern Boolean ImpersonateSelf(SECURITY_IMPERSONATION_LEVEL ImpersonationLevel);

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
