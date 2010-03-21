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

using System;
using System.Text;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	[Serializable]
	public class UsbException : Exception
	{
		/// <summary></summary>
		public UsbException()
		{
		}

		/// <summary></summary>
		public UsbException(string message)
			: base(message)
		{
		}
	}

	/// <summary></summary>
	[Serializable]
	public class NativeMethodCallUsbException : UsbException
	{
		/// <summary></summary>
		public readonly string Method;
		/// <summary></summary>
		public readonly int ErrorCode;
		/// <summary></summary>
		public readonly string NativeMessage;

		/// <summary></summary>
		public NativeMethodCallUsbException(string method, string message)
			: base(message)
		{
			Method        = method;
			ErrorCode     = Utilities.Win32.Debug.GetLastErrorCode();
			NativeMessage = Utilities.Win32.Debug.GetLastErrorMessage();
		}

		/// <summary></summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("USB exception while calling ");
			sb.AppendLine(Method);

			sb.Append("  Message: ");
			sb.AppendLine(Message);

			sb.Append("  Error code: ");
			sb.AppendLine(ErrorCode.ToString());

			sb.Append("  Native message: ");
			sb.AppendLine(NativeMessage);

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
