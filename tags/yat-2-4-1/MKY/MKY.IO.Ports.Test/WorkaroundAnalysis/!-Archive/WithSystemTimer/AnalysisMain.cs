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
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Threading;

namespace MKY.IO.Ports.Test.WorkaroundAnalysis.WithSystemTimer
{
	public static class AnalysisMain
	{
		private static string portName;

		[STAThread]
		private static void Main(string[] args)
		{
			portName = "COM1";
			if ((args.Length > 0) && (args[0].StartsWith("COM")))
				portName = args[0];

			Console.WriteLine("This is " + typeof(AnalysisMain).FullName);
			Console.WriteLine("that analyzes/demonstrates issues with the .NET " + Environment.Version);
			Console.WriteLine("System.IO.Ports.SerialPort class and known workarounds, using");
			Console.WriteLine(portName + " on Win " + Environment.OSVersion);

			try
			{
				for (;;) // forever until [Ctrl+C] or [Ctrl+Break]:
				{
					using (var connection = new Connection(portName))
					{
						var wrapper = new ConnectionWrapper(connection);
						{
							wrapper.RepeatUntilOpen();
							wrapper.ProbeUntilDisconnected();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Diagnostics.WriteErrorDetailsToConsole(ex, "An unhandled synchronous exception occurred!");
			}
			finally
			{
				Console.WriteLine("Press [Enter] to exit");
				Console.ReadLine();
			}
		}
	}

	public class ConnectionWrapper
	{
		protected Connection connection;
		protected System.Timers.Timer aliveMonitor;

		public ConnectionWrapper(Connection connection)
		{
			this.connection = connection;
		}

		public virtual void RepeatUntilOpen()
		{
			while (!connection.TryOpenPort())
			{
				Console.WriteLine("Trying to open " + connection.PortName + "... Connect device or [Ctrl+C/Break] to terminate");

				Thread.Sleep(1000);
			}
		}

		public virtual void ProbeUntilDisconnected()
		{
			this.aliveMonitor = new System.Timers.Timer(1000);
			this.aliveMonitor.AutoReset = true;
			this.aliveMonitor.Elapsed += aliveMonitor_Elapsed;
			this.aliveMonitor.Start();

			while (this.aliveMonitor.Enabled)
			{
				Thread.Sleep(1000);
			}
		}

		private object aliveMonitor_Elapsed_SyncObj = new object();

		private void aliveMonitor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (Monitor.TryEnter(aliveMonitor_Elapsed_SyncObj))
			{
				try
				{
					if (this.connection.PortIsAvailable())
					{
						Console.WriteLine(this.connection.PortName + " is available, probing... Disconnect device or [Ctrl+C/Break] to terminate");

						if (!this.connection.TryProbePort())
						{
							this.aliveMonitor.Stop();
							this.connection.TryClosePort();
						}
					}
					else
					{
						Console.WriteLine(this.connection.PortName + " is no longer available!");

						this.aliveMonitor.Stop();
						this.connection.TryClosePort();
					}
				}
				finally
				{
					Monitor.Exit(aliveMonitor_Elapsed_SyncObj);
				}
			} // Monitor.TryEnter()
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
