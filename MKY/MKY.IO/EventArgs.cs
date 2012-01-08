//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/EventArgs.cs $
// $Author: klaey-1 $
// $Date: 2011/08/24 13:38:45MESZ $
// $Revision: 1.1 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.IO
{
	/// <summary></summary>
	public class IORequestEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
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
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly IOErrorSeverity Severity;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly IODirection Direction;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly string Message;

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
			Severity = severity;
			Direction = direction;
			Message = message;
		}
	}

	/// <summary></summary>
	public class SerialPortIOErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
		public readonly System.IO.Ports.SerialError SerialPortError;

		/// <summary></summary>
		public SerialPortIOErrorEventArgs(string message, System.IO.Ports.SerialError serialPortError)
			: base(message)
		{
			SerialPortError = serialPortError;
		}
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/EventArgs.cs $
//==================================================================================================
