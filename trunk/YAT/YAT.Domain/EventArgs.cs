﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MKY;

namespace YAT.Domain
{
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
		public bool Highlight { get; set; } // = false;

		/// <remarks>
		/// <see cref="Highlight"/> is intended to be set by the event sink.
		/// </remarks>
		public RawChunkEventArgs(RawChunk value)
			: base(value)
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
	public class DisplayLinesEventArgs : EventArgs
	{
		/// <summary></summary>
		public List<DisplayLine> Lines { get; }

		/// <summary></summary>
		public DisplayLinesEventArgs(List<DisplayLine> lines)
		{
			Lines = lines;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
