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

namespace MKY.IO.Ports
{
	/// <summary>
	/// List containing serial port IDs.
	/// </summary>
	[Serializable]
	public class SerialPortCollection : List<SerialPortId>
	{
		/// <summary></summary>
		public class PortChangedAndCancelEventArgs : EventArgs
		{
			/// <summary></summary>
			public readonly SerialPortId Port;

			/// <summary></summary>
			private bool cancel = false;

			/// <summary></summary>
			public PortChangedAndCancelEventArgs(SerialPortId port)
			{
				Port = port;
			}

			/// <summary></summary>
			public bool Cancel
			{
				get { return (this.cancel); }
				set { this.cancel = value;  }
			}
		}

		/// <summary></summary>
		public SerialPortCollection()
			: base(SerialPortId.LastStandardPortNumber - SerialPortId.FirstStandardPortNumber + 1)
		{
		}

		/// <summary></summary>
		public SerialPortCollection(IEnumerable<SerialPortId> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all ports from <see cref="SerialPortId.FirstStandardPortNumber"/> to
		/// <see cref="SerialPortId.LastStandardPortNumber"/>.
		/// </summary>
		public virtual void FillWithStandardPorts()
		{
			Clear();
			for (int i = SerialPortId.FirstStandardPortNumber; i <= SerialPortId.LastStandardPortNumber; i++)
			{
				Add(new SerialPortId(i));
			}
			Sort();
		}

		/// <summary>
		/// Fills list with all ports from <see cref="System.IO.Ports.SerialPort.GetPortNames()"/>.
		/// </summary>
		/// <param name="getPortDescriptionsFromSystem">
		/// On request, this method queries the port descriptions from the system.
		/// </param>
		public virtual void FillWithAvailablePorts(bool getPortDescriptionsFromSystem)
		{
			Clear();
			foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
			{
				Add(new SerialPortId(portName));
			}
			Sort();

			if (getPortDescriptionsFromSystem)
				GetPortDescriptionsFromSystem();
		}

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to description
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual void GetPortDescriptionsFromSystem()
		{
			Dictionary<string, string> descriptions = SerialPortSearcher.GetDescriptionsFromSystem();

			foreach (SerialPortId portId in this)
			{
				if (descriptions.ContainsKey(portId.Name))
					portId.Description = descriptions[portId.Name];
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
		public virtual void MarkPortsInUse()
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
		/// property to <c>true</c> to cancel port scanning.
		/// </param>
		public virtual void MarkPortsInUse(EventHandler<PortChangedAndCancelEventArgs> portChangedCallback)
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
