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
// MKY Version 1.0.29
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 error API.
	/// </summary>
	/// <remarks>
	/// Intentionally called "WinError" instead of "Error" to prevent code analysis error due to
	/// "Error" keyword in certain .NET languages (e.g. Visual Basic).
	/// </remarks>
	public static class WinError
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		private static class NativeConstants
		{
			[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
			public const Int32 ERROR_SUCCESS = 0;

			[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Field name is given by the Win32 API.")]
			public const Int32 FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		}

		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		private static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			[SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "4", Justification = "'CharSet.Auto' will automatically marshal strings appropriately for the target operating system.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			public static extern Int32 FormatMessage([In] Int32 dwFlags, [In] ref Int64 lpSource, [In] Int32 dwMessageId, [In] Int32 dwLanguageId, [Out] StringBuilder lpBuffer, [In] Int32 nSize, [In] IntPtr Arguments);
		}

		#endregion

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int Success = NativeConstants.ERROR_SUCCESS;

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Get code that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting code.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming according to naming in the Win32 API.")]
		public static int GetLastErrorCode()
		{
			return (Marshal.GetLastWin32Error());
		}

		/// <summary>
		/// Get message that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting message.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming according to naming in the Win32 API.")]
		public static string GetLastErrorMessage()
		{
			return (GetErrorMessage(GetLastErrorCode()));
		}

		/// <summary>
		/// Get message that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting message.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming according to naming in the Win32 API.")]
		public static string GetErrorMessage(int errorCode)
		{
			// Get the result message that corresponds to the code:
			long temp = 0;
			StringBuilder message = new StringBuilder(256);
			int bytes = NativeMethods.FormatMessage(NativeConstants.FORMAT_MESSAGE_FROM_SYSTEM, ref temp, errorCode, 0, message, message.Capacity, IntPtr.Zero);

			// Subtract two characters from the message to strip EOL:
			int eolLength = Environment.NewLine.Length;
			if (bytes > eolLength)
				message = message.Remove(bytes - eolLength, eolLength);

			return (message.ToString());
		}

		/// <summary>
		/// Get string that describes the error of an API call.
		/// </summary>
		/// <returns>The resulting string.</returns>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming according to naming in the Win32 API.")]
		public static string LastErrorToString()
		{
			var errorCode = GetLastErrorCode();

			var sb = new StringBuilder();
			sb.Append("API call returned code '");
			sb.Append(errorCode);
			sb.Append("' meaning '");
			sb.Append(GetErrorMessage(errorCode));
			sb.Append("'.");
			return (sb.ToString());
		}

		/// <summary>
		/// Get exception object that describes the error of an API call.
		/// </summary>
		public static IOException LastErrorToIOException()
		{
			var errorCode = WinError.GetLastErrorCode();
			var message = WinError.GetErrorMessage(errorCode);
			var hresult = HResultFromErrorCode(errorCode);
			return (new IOException(message, hresult));
		}

		private static int HResultFromErrorCode(int errorCode)
		{
			return ((int)(0x80070000 | (uint)errorCode));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
