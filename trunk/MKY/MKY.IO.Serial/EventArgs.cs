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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary>
	/// Defines event data of an I/O data transfer. In addition to the serial data itself,
	/// it also contains meta information such as a time stamp.
	/// </summary>
	public abstract class DataEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] Data { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public abstract string PortStamp { get; }

		/// <summary></summary>
		protected DataEventArgs(byte[] data)
			: this (data, DateTime.Now)
		{
		}

		/// <summary></summary>
		protected DataEventArgs(byte[] data, DateTime timeStamp)
		{
			Data      = data;
			TimeStamp = timeStamp;
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
	public abstract class DataReceivedEventArgs : DataEventArgs
	{
		/// <summary></summary>
		protected DataReceivedEventArgs(byte[] data)
			: base(data)
		{
		}

		/// <summary></summary>
		protected DataReceivedEventArgs(byte[] data, DateTime timeStamp)
			: base(data, timeStamp)
		{
		}
	}

	/// <summary></summary>
	public abstract class DataSentEventArgs : DataEventArgs
	{
		/// <summary></summary>
		protected DataSentEventArgs(byte[] data)
			: base(data)
		{
		}

		/// <summary></summary>
		protected DataSentEventArgs(byte[] data, DateTime timeStamp)
			: base(data, timeStamp)
		{
		}
	}

	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		private ErrorSeverity severity;
		private Direction direction;
		private string message;

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, string message)
			: this(severity, Direction.None, message)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, Direction direction, string message)
		{
			this.severity = severity;
			this.direction = direction;
			this.message = message;
		}

		/// <summary></summary>
		public ErrorSeverity Severity
		{
			get { return (this.severity); }
		}

		/// <summary></summary>
		public Direction Direction
		{
			get { return (this.direction); }
		}

		/// <summary></summary>
		public string Message
		{
			get { return (this.message); }
		}
	}

	/// <summary></summary>
	public class SerialPortErrorEventArgs : IOErrorEventArgs
	{
		private System.IO.Ports.SerialError serialPortError;

		/// <summary></summary>
		public SerialPortErrorEventArgs(ErrorSeverity severity, Direction direction, string message, System.IO.Ports.SerialError serialPortError)
			: base(severity, direction, message)
		{
			this.serialPortError = serialPortError;
		}

		/// <summary></summary>
		public System.IO.Ports.SerialError SerialPortError
		{
			get { return (this.serialPortError); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
