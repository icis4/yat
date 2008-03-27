using System;
using System.Collections.Generic;

namespace MKY.IO.Ports
{
	/// <summary>
	/// List containing serial port IDs.
	/// </summary>
	[Serializable]
	public class SerialPortList : List<SerialPortId>
	{
		/// <summary></summary>
		public class PortChangedAndCancelEventArgs : EventArgs
		{
			/// <summary></summary>
			public readonly SerialPortId Port;

			/// <summary></summary>
			public bool Cancel = false;

			/// <summary></summary>
			public PortChangedAndCancelEventArgs(SerialPortId port)
			{
				Port = port;
			}
		}

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
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to description
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public void GetDescriptionsFromSystem()
		{
			Dictionary<int, string> descriptions = SerialPortSearcher.GetDescriptionsFromSystem();

			foreach (SerialPortId portId in this)
			{
				if (descriptions.ContainsKey(portId.Number))
					portId.Description = descriptions[portId.Number];
			}
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
			MarkPortsInUse(null);
		}

		/// <summary>
		/// Checks all ports whether they are currently in use and marks them.
		/// </summary>
		/// <remarks>
		/// In .NET 2.0, no class provides a method to retrieve whether a port is currently
		/// in use or not. Therefore, this method actively tries to open every port. This
		/// takes some time.
		/// </remarks>
		/// <param name="portChangedCallback">
		/// Callback delegate, can be used to get an event each time a new port is being
		/// tried to be opened. Set the <see cref="PortChangedAndCancelEventArgs.Cancel"/>
		/// property the true to cancel port scanning.
		/// </param>
		public void MarkPortsInUse(EventHandler<PortChangedAndCancelEventArgs> portChangedCallback)
		{
			foreach (SerialPortId portId in this)
			{
				if (portChangedCallback != null)
				{
					PortChangedAndCancelEventArgs args = new PortChangedAndCancelEventArgs(portId);
					portChangedCallback.Invoke(this, args);
					if (args.Cancel)
						break;
				}

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
