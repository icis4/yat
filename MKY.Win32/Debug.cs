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
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 debugging API.
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
	public static class Debug
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		/// <summary>
		/// Class encapsulating native Win32 types, constants and functions.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		private static class Native
		{
			#region Constants
			//==========================================================================================
			// Constants
			//==========================================================================================

			private const string KERNEL_DLL = "kernel32.dll";

			public const Int32 FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;

			#endregion

			#region External Functions
			//==========================================================================================
			// External Functions
			//==========================================================================================

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 FormatMessage([In] Int32 dwFlags, [In] ref Int64 lpSource, [In] Int32 dwMessageId, [In] Int32 dwLanguageId, [Out] StringBuilder lpBuffer, [In] Int32 nSize, [In] IntPtr Arguments);

			#endregion
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Get code that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting code.</returns>
		public static int GetLastErrorCode()
		{
			// Get the error code for the last API call.
			return (System.Runtime.InteropServices.Marshal.GetLastWin32Error());
		}

		/// <summary>
		/// Get message that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting message.</returns>
		public static string GetLastErrorMessage()
		{
			// Get the error code for the last API call.
			int errorCode = GetLastErrorCode();

			// Get the result message that corresponds to the code.
			long temp = 0;
			StringBuilder message = new StringBuilder(256);
			int bytes = Native.FormatMessage(Native.FORMAT_MESSAGE_FROM_SYSTEM, ref temp, errorCode, 0, message, message.Capacity, IntPtr.Zero);

			// Subtract two characters from the message to strip EOL.
			int eolLength = Environment.NewLine.Length;
			if (bytes > eolLength)
				message = message.Remove(bytes - eolLength, eolLength);

			return (message.ToString());
		}

		/// <summary>
		/// Get string that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting string.</returns>
		public static string GetLastError()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("API call returned code '");
			sb.Append(GetLastErrorCode());
			sb.Append("' meaning '");
			sb.Append(GetLastErrorMessage());
			sb.Append("'.");
			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
