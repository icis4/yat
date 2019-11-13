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
// YAT Version 2.1.1 Development
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
using System.Collections.ObjectModel;
#if (WITH_SCRIPTING)
using System.Diagnostics.CodeAnalysis;
#endif

using MKY;

namespace YAT.Domain
{
#if (WITH_SCRIPTING)
	/// <summary></summary>
	public class PacketEventArgs : EventArgs
	{
		/// <summary></summary>
		public byte[] Data { get; protected set; }

		/// <summary></summary>
		public PacketEventArgs(byte[] data)
		{
			Data = data;
		}
	}

	/// <remarks>
	/// Not inheriting from <see cref="PacketEventArgs"/> since <see cref="Data"/> must be modifiable.
	/// </remarks>
	public class ModifiablePacketEventArgs : EventArgs
	{
		/// <summary></summary>
		public byte[] Data { get; set; }

		/// <summary></summary>
		public bool Cancel { get; set; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public ModifiablePacketEventArgs(byte[] data, bool cancel = false)
		{
			Data = data;
			Cancel = cancel;
		}
	}

	/// <summary></summary>
	public class MessageEventArgs : EventArgs
	{
		/// <summary></summary>
		public string Message { get; protected set; }

		/// <summary></summary>
		public MessageEventArgs(string message)
		{
			Message = message;
		}
	}
#endif // WITH_SCRIPTING

	/// <summary></summary>
	public class IOControlEventArgs : EventArgs
	{
		/// <summary></summary>
		public IODirection Direction { get; }

		/// <summary></summary>
		public ReadOnlyCollection<string> Texts { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public IOControlEventArgs()
			: this(IODirection.None)
		{
		}

		/// <summary></summary>
		public IOControlEventArgs(IODirection direction)
			: this(direction, null)
		{
		}

		/// <summary></summary>
		public IOControlEventArgs(IODirection direction, ReadOnlyCollection<string> texts)
			: this(direction, texts, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOControlEventArgs(IODirection direction, ReadOnlyCollection<string> texts, DateTime timeStamp)
		{
			Direction = direction;
			Texts     = texts;
			TimeStamp = timeStamp;
		}
	}

	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public IOErrorSeverity Severity { get; }

		/// <summary></summary>
		public IODirection Direction { get; }

		/// <summary></summary>
		public string Message { get; }

		/// <summary></summary>
		public DateTime TimeStamp { get; }

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, IODirection direction, string message)
			: this(severity, direction, message, DateTime.Now)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, IODirection direction, string message, DateTime timeStamp)
		{
			Severity  = severity;
			Direction = direction;
			Message   = message;
			TimeStamp = timeStamp;
		}
	}

	/// <summary></summary>
	public class SerialPortErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
		public System.IO.Ports.SerialError SerialPortError { get; }

		/// <summary></summary>
		public SerialPortErrorEventArgs(IOErrorSeverity severity, IODirection direction, string message, System.IO.Ports.SerialError serialPortError, DateTime timeStamp)
			: base(severity, direction, message, timeStamp)
		{
			SerialPortError = serialPortError;
		}
	}

	/// <summary></summary>
	public class RawChunkEventArgs : EventArgs<RawChunk>
	{
		/// <summary></summary>
		public LineChunkAttribute Attribute { get; set; } // = LineChunkAttributeNone;

		/// <remarks>
		/// The <see cref="Attribute"/> is intended to be set by the event sink,
		/// therefore not part of the constructor parameters.
		/// </remarks>
		public RawChunkEventArgs(RawChunk chunk)
			: base(chunk)
		{
		}
	}

	/// <summary></summary>
	public class DisplayElementsEventArgs : EventArgs
	{
		/// <summary></summary>
		public DisplayElementCollection Elements { get; }

		/// <summary></summary>
		public DisplayElementsEventArgs(DisplayElementCollection elements)
		{
			Elements = elements;
		}
	}

	/// <summary></summary>
	public class DisplayLineEventArgs : EventArgs
	{
		/// <summary></summary>
		public DisplayLine Line { get; }

		/// <summary></summary>
		public DisplayLineEventArgs(DisplayLine line)
		{
			Line = line;
		}
	}

	/// <summary></summary>
	public class DisplayLinesEventArgs : EventArgs
	{
		/// <summary></summary>
		public DisplayLineCollection Lines { get; }

		/// <summary></summary>
		public DisplayLinesEventArgs(DisplayLineCollection lines)
		{
			Lines = lines;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
