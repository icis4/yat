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
using System.Text;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 debugging API.
	/// </summary>
	public static class Debug
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		private static class NativeConstants
		{
			public const Int32 FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		}

		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		private static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 FormatMessage([In] Int32 dwFlags, [In] ref Int64 lpSource, [In] Int32 dwMessageId, [In] Int32 dwLanguageId, [Out] StringBuilder lpBuffer, [In] Int32 nSize, [In] IntPtr Arguments);
		}

		#endregion

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
			int bytes = NativeMethods.FormatMessage(NativeConstants.FORMAT_MESSAGE_FROM_SYSTEM, ref temp, errorCode, 0, message, message.Capacity, IntPtr.Zero);

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
