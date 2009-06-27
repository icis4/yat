//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Ports
{
	/// <summary></summary>
	[Serializable]
	public class SerialDataReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly System.IO.Ports.SerialData EventType = System.IO.Ports.SerialData.Chars;

		/// <summary></summary>
		public SerialDataReceivedEventArgs()
		{
			// do nothing
		}

		/// <summary></summary>
		public SerialDataReceivedEventArgs(System.IO.Ports.SerialData eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly System.IO.Ports.SerialError EventType = System.IO.Ports.SerialError.Frame;

		/// <summary></summary>
		public SerialErrorReceivedEventArgs()
		{
			// do nothing
		}

		/// <summary></summary>
		public SerialErrorReceivedEventArgs(System.IO.Ports.SerialError eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialPinChangedEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly MKY.IO.Ports.SerialPinChange EventType = MKY.IO.Ports.SerialPinChange.Unknown;

		/// <summary></summary>
		public SerialPinChangedEventArgs()
		{
			// do nothing
		}

		/// <summary></summary>
		public SerialPinChangedEventArgs(MKY.IO.Ports.SerialPinChange eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);

	/// <summary></summary>
	public delegate void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e);

	/// <summary></summary>
	public delegate void SerialPinChangedEventHandler(object sender, SerialPinChangedEventArgs e);
}

//==================================================================================================
// End of $URL$
//==================================================================================================
