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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Ports
{
	/// <summary></summary>
	public class SerialDataReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		public System.IO.Ports.SerialData EventType { get; }

		/// <summary></summary>
		public SerialDataReceivedEventArgs(System.IO.Ports.SerialData eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		/// <summary></summary>
		public System.IO.Ports.SerialError EventType { get; }

		/// <summary></summary>
		public SerialErrorReceivedEventArgs(System.IO.Ports.SerialError eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	public class SerialPinChangedEventArgs : EventArgs
	{
		/// <summary></summary>
		public MKY.IO.Ports.SerialPinChange EventType { get; }

		/// <summary></summary>
		public SerialPinChangedEventArgs(MKY.IO.Ports.SerialPinChange eventType)
		{
			EventType = eventType;
		}
	}

	/// <summary></summary>
	public class SerialPortChangedAndCancelEventArgs : EventArgs
	{
		/// <summary></summary>
		public SerialPortId Port { get; }

		/// <summary></summary>
		public bool Cancel { get; set; }

		/// <summary></summary>
		public SerialPortChangedAndCancelEventArgs(SerialPortId port)
		{
			Port = port;
		}
	}

	/// <summary></summary>
	public class SerialPortInUseLookupEventArgs : EventArgs
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Event requires to retrieve a collection.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Event requires to retrieve a collection.")]
		public List<InUseInfo> InUseLookup { get; set; }
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "Type is given by System.IO.Ports.SerialPort which unfortunately doesn't use proper event types.")]
	public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "Type is given by System.IO.Ports.SerialPort which unfortunately doesn't use proper event types.")]
	public delegate void SerialErrorReceivedEventHandler(object sender, SerialErrorReceivedEventArgs e);

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "Type is given by System.IO.Ports.SerialPort which unfortunately doesn't use proper event types.")]
	public delegate void SerialPinChangedEventHandler(object sender, SerialPinChangedEventArgs e);
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
