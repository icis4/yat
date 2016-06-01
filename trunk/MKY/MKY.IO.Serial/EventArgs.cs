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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.ObjectModel;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary>
	/// Defines event data of an I/O data transfer. In addition to the serial data itself,
	/// it also contains meta information such as a time stamp.
	/// </summary>
	public abstract class DataEventArgs : EventArgs
	{
		/// <remarks>
		/// "Guidelines for Collections": "Do use byte arrays instead of collections of bytes."
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private byte[] data;

		private DateTime timeStamp;

		/// <summary></summary>
		protected DataEventArgs(byte[] data)
		{
			this.data = data;
			this.timeStamp = DateTime.Now;
		}

		/// <summary></summary>
		public virtual byte[] Data
		{
			get { return (this.data); }
		}

		/// <summary></summary>
		public virtual DateTime TimeStamp
		{
			get { return (this.timeStamp); }
		}

		/// <summary></summary>
		public abstract string PortStamp
		{
			get;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in this.data)
				sb.Append(Convert.ToChar(b));

			return (indent + sb.ToString());
		}
	}

	/// <summary></summary>
	public abstract class DataReceivedEventArgs : DataEventArgs
	{
		/// <summary></summary>
		public DataReceivedEventArgs(byte[] data)
			: base(data)
		{
		}
	}

	/// <summary></summary>
	public abstract class DataSentEventArgs : DataEventArgs
	{
		/// <summary></summary>
		public DataSentEventArgs(byte[] data)
			: base(data)
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
