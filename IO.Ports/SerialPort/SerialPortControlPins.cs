using System;

namespace HSR.IO.Ports
{
	/// <summary>
	/// Serial port control pins.
	/// </summary>
	public struct SerialPortControlPins
	{
		/// <summary>
		/// Request To Send.
		/// </summary>
		public bool Rts;

		/// <summary>
		/// Clear To Send.
		/// </summary>
		public bool Cts;

		/// <summary>
		/// Data Terminal Ready.
		/// </summary>
		public bool Dtr;

		/// <summary>
		/// Data Set Ready.
		/// </summary>
		public bool Dsr;

		/// <summary>
		/// Carrier Detect.
		/// </summary>
		public bool Cd;
	}
}
