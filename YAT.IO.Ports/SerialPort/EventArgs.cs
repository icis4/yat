using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.IO.Ports
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
		public readonly HSR.IO.Ports.SerialPinChange EventType = HSR.IO.Ports.SerialPinChange.Unknown;

		/// <summary></summary>
		public SerialPinChangedEventArgs()
		{
			// do nothing
		}

		/// <summary></summary>
		public SerialPinChangedEventArgs(HSR.IO.Ports.SerialPinChange eventType)
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
