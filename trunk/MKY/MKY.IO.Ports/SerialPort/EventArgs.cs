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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Ports
{
	/// <summary></summary>
	[Serializable]
	public class SerialDataReceivedEventArgs : EventArgs
	{
		private System.IO.Ports.SerialData eventType;

		/// <summary></summary>
		public SerialDataReceivedEventArgs(System.IO.Ports.SerialData eventType)
		{
			this.eventType = eventType;
		}

		/// <summary></summary>
		public System.IO.Ports.SerialData EventType
		{
			get { return (this.eventType); }
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		private System.IO.Ports.SerialError eventType;

		/// <summary></summary>
		public SerialErrorReceivedEventArgs(System.IO.Ports.SerialError eventType)
		{
			this.eventType = eventType;
		}

		/// <summary></summary>
		public System.IO.Ports.SerialError EventType
		{
			get { return (this.eventType); }
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialPinChangedEventArgs : EventArgs
	{
		private MKY.IO.Ports.SerialPinChange eventType;

		/// <summary></summary>
		public SerialPinChangedEventArgs(MKY.IO.Ports.SerialPinChange eventType)
		{
			this.eventType = eventType;
		}

		/// <summary></summary>
		public MKY.IO.Ports.SerialPinChange EventType
		{
			get { return (this.eventType); }
		}
	}

	/// <summary></summary>
	[Serializable]
	public class SerialPortChangedAndCancelEventArgs : EventArgs
	{
		private SerialPortId port;
		private bool cancel;

		/// <summary></summary>
		public SerialPortChangedAndCancelEventArgs(SerialPortId port)
		{
			this.port = port;
		}

		/// <summary></summary>
		public SerialPortId Port
		{
			get { return (this.port); }
		}

		/// <summary></summary>
		public bool Cancel
		{
			get { return (this.cancel); }
			set { this.cancel = value; }
		}
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
