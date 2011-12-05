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
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;
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

		/// <summary></summary>
		public UsbException(string message, Exception innerException)
			: base(message, innerException)
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
			ErrorCode     = Win32.Debug.GetLastErrorCode();
			NativeMessage = Win32.Debug.GetLastErrorMessage();
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
			sb.AppendLine(ErrorCode.ToString(NumberFormatInfo.InvariantInfo));

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
