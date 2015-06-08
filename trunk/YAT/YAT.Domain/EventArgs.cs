﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1'' Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		private IOErrorSeverity severity;
		private IODirection direction;
		private string message;

		/// <summary></summary>
		public IOErrorEventArgs(string message)
			: this(IOErrorSeverity.Severe, message)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, string message)
			: this(severity, IODirection.Any, message)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, IODirection direction, string message)
		{
			this.severity = severity;
			this.direction = direction;
			this.message = message;
		}

		/// <summary></summary>
		public IOErrorSeverity Severity
		{
			get { return (this.severity); }
		}

		/// <summary></summary>
		public IODirection Direction
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
		public SerialPortErrorEventArgs(string message, System.IO.Ports.SerialError serialPortError)
			: base(message)
		{
			this.serialPortError = serialPortError;
		}

		/// <summary></summary>
		public System.IO.Ports.SerialError SerialPortError
		{
			get { return (this.serialPortError); }
		}
	}

	/// <summary></summary>
	public class RawElementEventArgs : EventArgs
	{
		private RawElement element;

		/// <summary></summary>
		public RawElementEventArgs(RawElement element)
		{
			this.element = element;
		}

		/// <summary></summary>
		public RawElement Element
		{
			get { return (this.element); }
		}
	}

	/// <summary></summary>
	public class DisplayElementsEventArgs : EventArgs
	{
		private DisplayElementCollection elements;

		/// <summary></summary>
		public DisplayElementsEventArgs(DisplayElementCollection elements)
		{
			this.elements = elements;
		}

		/// <summary></summary>
		public DisplayElementCollection Elements
		{
			get { return (this.elements); }
		}
	}

	/// <summary></summary>
	public class DisplayLinesEventArgs : EventArgs
	{
		private List<DisplayLine> lines;

		/// <summary></summary>
		public DisplayLinesEventArgs(List<DisplayLine> lines)
		{
			this.lines = lines;
		}

		/// <summary></summary>
		public List<DisplayLine> Lines
		{
			get { return (this.lines); }
		}
	}

	/// <summary></summary>
	public class RepositoryEventArgs : EventArgs
	{
		private RepositoryType repository;

		/// <summary></summary>
		public RepositoryEventArgs(RepositoryType repository)
		{
			this.repository = repository;
		}

		/// <summary></summary>
		public RepositoryType Repository
		{
			get { return (this.repository); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
