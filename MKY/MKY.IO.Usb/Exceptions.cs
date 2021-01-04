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
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
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

		/// <summary></summary>
		protected UsbException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary></summary>
	[Serializable]
	public class NativeMethodCallUsbException : UsbException
	{
		/// <summary></summary>
		public string Method { get; }

		/// <summary></summary>
		public int ErrorCode { get; }

		/// <summary></summary>
		public string NativeMessage { get; }

		/// <summary></summary>
		public NativeMethodCallUsbException()
			: this(null, null, null)
		{
		}

		/// <summary></summary>
		public NativeMethodCallUsbException(string message)
			: this(message, null, null)
		{
		}

		/// <summary></summary>
		public NativeMethodCallUsbException(string message, string method)
			: this(message, null, method)
		{
		}

		/// <summary></summary>
		public NativeMethodCallUsbException(string message, Exception innerException)
			: this(message, innerException, null)
		{
		}

		/// <summary></summary>
		public NativeMethodCallUsbException(string message, Exception innerException, string method)
			: base(message, innerException)
		{
			Method        = method;
			ErrorCode     = Win32.WinError.GetLastErrorCode();
			NativeMessage = Win32.WinError.GetLastErrorMessage();
		}

		#region ISerializable Members

		/// <summary></summary>
		protected NativeMethodCallUsbException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary></summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Method", Method);
			info.AddValue("ErrorCode", ErrorCode);
			info.AddValue("NativeMessage", NativeMessage);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append("USB exception while calling ");
			sb.AppendLine(Method);

			sb.Append("  Message: ");
			sb.AppendLine(Message);

			sb.Append("  Error code: ");
			sb.AppendLine(ErrorCode.ToString(CultureInfo.InvariantCulture));

			sb.Append("  Native message: ");
			sb.AppendLine(NativeMessage);

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
