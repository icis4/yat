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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

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
			: this(severity, IODirection.None, message)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, IODirection direction, string message)
		{
			this.severity  = severity;
			this.direction = direction;
			this.message   = message;
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
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
