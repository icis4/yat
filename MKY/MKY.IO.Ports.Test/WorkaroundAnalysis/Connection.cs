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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;

namespace MKY.IO.Ports.Test.WorkaroundAnalysis
{
	public class Connection : IDisposable
	{
		public string PortName { get; protected set; }

		private SerialPort port;
		private object portSynObj = new object();

		public Connection(string portName)
		{
			PortName = portName;
		}

		#region Disposal

		public bool IsDisposed { get; protected set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (this.port != null)
					{
						try
						{
							this.port.Dispose();
						}
						catch (IOException ex)
						{
							Diagnostics.WriteErrorDetailsToConsole(ex, "IOException-on-Dispose()-workaround successful");
							Console.WriteLine();
						}
					}
				}

				this.port = null;
				IsDisposed = true;
			}
		}

		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		public virtual bool TryOpenPort()
		{
			AssertNotDisposed();

			try
			{
				lock (this.portSynObj)
				{
					this.port = new SerialPort(PortName);
					this.port.Open();
				}
				Console.WriteLine();
				Console.WriteLine("Successfully created and opened " + PortName);
				Console.WriteLine();
				return (true);
			}
			catch (IOException) // ex)
			{
			////Diagnostics.WriteErrorDetailsToConsole(ex, false);
				return (false);
			}
			catch (Exception ex)
			{
				Diagnostics.WriteErrorDetailsToConsole(ex, "Failed to create and open " + PortName + "!");
				return (false);
			}
		}

		public virtual bool TryClosePort()
		{
			AssertNotDisposed();

			try
			{
				lock (this.portSynObj)
				{
					this.port.Close();
					this.port = null;
				}
				Console.WriteLine();
				Console.WriteLine("Successfully closed " + PortName);
				Console.WriteLine();
				return (true);
			}
			catch (IOException) // ex)
			{
			////Diagnostics.WriteErrorDetailsToConsole(ex, false);
				return (false);
			}
			catch (Exception ex)
			{
				Diagnostics.WriteErrorDetailsToConsole(ex, "Failed to close " + PortName + "!");
				return (false);
			}
		}

		public virtual bool PortIsAvailable()
		{
			return (SerialPort.GetPortNames().Contains(PortName));
		}

		public virtual bool TryProbePort()
		{
			AssertNotDisposed();

			lock (this.portSynObj)
			{
				Trace.Assert((this.port.IsOpen == true), "SerialPort.IsOpen is no longer true!");
				Trace.Assert((this.port.BaseStream != null), "SerialPort.BaseStream is no longer valid!");
				Trace.Assert((this.port.BytesToWrite >= 0), "SerialPort.BytesToWrite is invalid!");
				Trace.Assert((this.port.BytesToRead >= 0), "SerialPort.BytesToRead is invalid!");
			}

			return (true);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
