//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

		/// <summary></summary>
		protected UsbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary></summary>
	[Serializable]
	public class NativeMethodCallUsbException : UsbException
	{
		private string method;
		private int errorCode;
		private string nativeMessage;

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
			this.method        = method;
			this.errorCode     = Win32.WinError.GetLastErrorCode();
			this.nativeMessage = Win32.WinError.GetLastErrorMessage();
		}

		#region ISerializable Members

		/// <summary></summary>
		protected NativeMethodCallUsbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary></summary>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Method", this.method);
			info.AddValue("ErrorCode", this.errorCode);
			info.AddValue("NativeMessage", this.nativeMessage);
		}

		#endregion

		/// <summary></summary>
		public string Method
		{
			get { return (this.method); }
		}

		/// <summary></summary>
		public int ErrorCode
		{
			get { return (this.errorCode); }
		}

		/// <summary></summary>
		public string NativeMessage
		{
			get { return (this.nativeMessage); }
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
			sb.AppendLine(ErrorCode.ToString(CultureInfo.InvariantCulture));

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
