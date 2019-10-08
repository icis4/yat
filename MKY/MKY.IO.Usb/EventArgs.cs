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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	public class DeviceEventArgs : EventArgs
	{
		/// <summary></summary>
		public DeviceClass DeviceClass { get; }

		/// <summary></summary>
		public DeviceInfo DeviceInfo { get; }

		/// <summary></summary>
		public DeviceEventArgs(DeviceClass deviceClass, DeviceInfo deviceInfo)
		{
			DeviceClass = deviceClass;
			DeviceInfo  = deviceInfo;
		}
	}

	/// <summary>
	/// Defines event data of an I/O data transfer. In addition to the serial data itself,
	/// it also contains meta information such as a time stamp.
	/// </summary>
	public class DataEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] Data { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public DataEventArgs(byte[] data)
		{
			Data      = data;
			TimeStamp = DateTime.Now;
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public virtual string ToString(string indent)
		{
			var sb = new StringBuilder();
			foreach (byte b in Data)
				sb.Append(Convert.ToChar(b));

			return (indent + sb.ToString());
		}
	}

	/// <summary></summary>
	public class ErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public string Message { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public ErrorEventArgs(string message)
		{
			Message = message;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
