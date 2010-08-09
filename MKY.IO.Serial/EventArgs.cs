//==================================================================================================
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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class IORequestEventArgs : EventArgs
	{
		/// <summary></summary>
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
	public class SerialPortIOErrorEventArgs : IOErrorEventArgs
	{
		/// <summary></summary>
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
// $URL$
//==================================================================================================
