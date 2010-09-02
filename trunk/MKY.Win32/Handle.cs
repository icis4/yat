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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 API relating to handles.
	/// </summary>
	public static class Handle
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
			#region Constants
			//==========================================================================================
			// Constants
			//==========================================================================================

			private const string KERNEL_DLL = "kernel32.dll";

			#endregion

			#region External Functions
			//==========================================================================================
			// External Functions
			//==========================================================================================

			/// <summary>
			/// Closes an open object handle.
			/// </summary>
			/// <param name="hObject">A valid handle to an open object.</param>
			/// <returns>True on success, false on failure.</returns>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern bool CloseHandle([In] SafeFileHandle hObject);

			#endregion
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
