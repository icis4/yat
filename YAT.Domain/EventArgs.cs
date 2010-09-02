//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class IORequestEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly IORequest Request;

		/// <summary></summary>
		public IORequestEventArgs(IORequest request)
		{
			Request = request;
		}
	}

	/// <summary></summary>
	public class IOErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly IOErrorSeverity Severity;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly string Message;

		/// <summary></summary>
		public IOErrorEventArgs(string message)
			: this(IOErrorSeverity.Severe, message)
		{
		}

		/// <summary></summary>
		public IOErrorEventArgs(IOErrorSeverity severity, string message)
		{
			Severity = severity;
			Message = message;
		}
	}

	/// <summary></summary>
	public class SerialPortErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly System.IO.Ports.SerialError SerialPortError;

		/// <summary></summary>
		public SerialPortErrorEventArgs(string message, System.IO.Ports.SerialError serialPortError)
			: base(message)
		{
			SerialPortError = serialPortError;
		}
	}

	/// <summary></summary>
	public class RawElementEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly RawElement Element;

		/// <summary></summary>
		public RawElementEventArgs(RawElement element)
		{
			Element = element;
		}
	}

	/// <summary></summary>
	public class DisplayElementsEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly DisplayElementCollection Elements;

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
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly List<DisplayLine> Lines;

		/// <summary></summary>
		public DisplayLinesEventArgs(List<DisplayLine> lines)
		{
			Lines = lines;
		}
	}

	/// <summary></summary>
	public class RepositoryEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		public readonly RepositoryType Repository;

		/// <summary></summary>
		public RepositoryEventArgs(RepositoryType repository)
		{
			Repository = repository;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
