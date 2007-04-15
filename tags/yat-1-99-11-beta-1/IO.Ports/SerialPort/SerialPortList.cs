using System;
using System.Collections.Generic;

namespace HSR.IO.Ports
{
	/// <summary>
	/// List containing serial port IDs.
	/// </summary>
	[Serializable]
	public class SerialPortList : List<SerialPortId>
	{
		/// <summary></summary>
		public SerialPortList()
			: base(SerialPortId.StandardLastPort - SerialPortId.StandardFirstPort + 1)
		{
		}

		/// <summary></summary>
		public SerialPortList(IEnumerable<SerialPortId> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all ports from <see cref="SerialPortId.StandardFirstPort"/> to
		/// <see cref="SerialPortId.StandardLastPort"/>.
		/// </summary>
		public void FillWithStandardPorts()
		{
			Clear();
			for (int i = SerialPortId.StandardFirstPort; i <= SerialPortId.StandardLastPort; i++)
			{
				base.Add(new SerialPortId(i));
			}
			Sort();
		}

		/// <summary>
		/// Fills list with all ports from <see cref="System.IO.Ports.SerialPort.GetPortNames()"/>.
		/// </summary>
		public void FillWithAvailablePorts()
		{
			Clear();
			foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
			{
				base.Add(new SerialPortId(portName));
			}
			Sort();
		}

		/// <summary>
		/// Checks all ports whether they are currently in use and marks them.
		/// </summary>
		/// <remarks>
		/// In .NET 2.0, no class provides a method to retrieve whether a port is currently
		/// in use or not. Therefore, this method actively tries to open every port. This
		/// takes some time.
		/// </remarks>
		public void MarkPortsInUse()
		{
			foreach (SerialPortId portId in this)
			{
				System.IO.Ports.SerialPort p = new System.IO.Ports.SerialPort(portId);
				try
				{
					p.Open();
					p.Close();
					portId.IsInUse = false;
				}
				catch
				{
					portId.IsInUse = true;
				}
				finally
				{
					p.Dispose();
				}
			}
		}
	}
}
