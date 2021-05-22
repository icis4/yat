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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
		public abstract string Device { get; }

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

		/// <summary></summary>
		protected string DataAsPrintableString
		{
			get { return (ArrayEx.ValuesToString(Data)); }
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (DataAsPrintableString);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder(indent);

			sb.Append(DataAsPrintableString);
			sb.Append(" | TimeStamp = ");
			sb.Append(TimeStamp.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | Device = ");
			sb.Append(Device);

			return (sb.ToString());
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
		protected DataSentEventArgs(byte[] data, DateTime timeStamp)
			: base(data, timeStamp)
		{
		}
	}

	/// <summary></summary>
	public abstract class IOMessageEventArgs : EventArgs
	{
		/// <summary></summary>
		public Direction Direction { get; }

		/// <summary></summary>
		public string Message { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		protected IOMessageEventArgs(string message)
			: this(Direction.None, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		protected IOMessageEventArgs(string message, DateTime timeStamp)
			: this(Direction.None, message, timeStamp)
		{
		}

		/// <summary></summary>
		protected IOMessageEventArgs(Direction direction, string message)
			: this(direction, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		protected IOMessageEventArgs(Direction direction, string message, DateTime timeStamp)
		{
			Direction = direction;
			Message   = message;
			TimeStamp = timeStamp;
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if (Message != null)
				return (Message);
			else
				return ("");
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder(indent);

			sb.Append(Message ?? "(none)");
			sb.Append(" | Direction = ");
			sb.Append(Direction.ToString());
			sb.Append(" | TimeStamp = ");
			sb.Append(TimeStamp.ToString(CultureInfo.CurrentCulture));

			return (sb.ToString());
		}
	}

	/// <summary></summary>
	public class IOWarningEventArgs : IOMessageEventArgs
	{
		/// <summary></summary>
		public IOWarningEventArgs(string message)
			: this(Direction.None, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOWarningEventArgs(string message, DateTime timeStamp)
			: this(Direction.None, message, timeStamp)
		{
		}

		/// <summary></summary>
		public IOWarningEventArgs(Direction direction, string message)
			: this(direction, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOWarningEventArgs(Direction direction, string message, DateTime timeStamp)
			: base(direction, message, timeStamp)
		{
		}
	}

	/// <summary></summary>
	public class IOErrorEventArgs : IOMessageEventArgs
	{
		/// <summary></summary>
		public ErrorSeverity Severity { get; }

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, string message)
			: this(severity, Direction.None, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, string message, DateTime timeStamp)
			: this(severity, Direction.None, message, timeStamp)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, Direction direction, string message)
			: this(severity, direction, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(ErrorSeverity severity, Direction direction, string message, DateTime timeStamp)
			: base(direction, message, timeStamp)
		{
			Severity  = severity;
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="IOMessageEventArgs.ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="IOMessageEventArgs.ToString()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override string ToDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder(indent);

			sb.Append(Message ?? "(none)");
			sb.Append(" | Severity = ");
			sb.Append(Severity.ToString());
			sb.Append(" | Direction = ");
			sb.Append(Direction.ToString());
			sb.Append(" | TimeStamp = ");
			sb.Append(TimeStamp.ToString(CultureInfo.CurrentCulture));

			return (sb.ToString());
		}
	}

	/// <summary></summary>
	public class SerialPortErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
		public System.IO.Ports.SerialError SerialPortError { get; }

		/// <summary></summary>
		public SerialPortErrorEventArgs(ErrorSeverity severity, Direction direction, string message, System.IO.Ports.SerialError serialPortError)
			: base(severity, direction, message)
		{
			SerialPortError = serialPortError;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
