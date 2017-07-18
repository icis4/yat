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

namespace MKY.IO.Ports.Test.WorkaroundAnalysis.OnTwoWorkerThreads
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

			Console.WriteLine("This application analyzes/demonstrates issues with the .NET " + Environment.Version);
			Console.WriteLine("System.IO.Ports.SerialPort class and known workarounds, using " + portName);
			Console.WriteLine("on Win " + Environment.OSVersion);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			try
			{
				for (;;) // forever until [Ctrl+C] or [Ctrl+Break]:
				{
					using (var connection = new Connection(portName))
					{
						var wrapper = new ConnectionWrapper(connection);
						{
							var t = new Thread(new ThreadStart(wrapper.RepeatUntilOpen));
							t.Name = typeof(AnalysisMain).FullName + "-RepeatUntilOpen";
							t.Start();
							t.Join();
						}
						{
							var t = new Thread(new ThreadStart(wrapper.ProbeUntilDisconnected));
							t.Name = typeof(AnalysisMain).FullName + "-ProbeUntilDisconnected";
							t.Start();
							t.Join();
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

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine("An unhandled asynchronous non-synchronized exception occurred!");

			var ex = (e.ExceptionObject as Exception);
			if (ex != null)
				Diagnostics.WriteErrorDetailsToConsole(ex);
		}
	}

	public class ConnectionWrapper
	{
		protected Connection connection;

		public ConnectionWrapper(Connection connection)
		{
			this.connection = connection;
		}

		public virtual void RepeatUntilOpen()
		{
			while (!connection.TryOpenPort())
			{
			////Console.WriteLine("Trying to open " + connection.PortName + "... Connect device or [Ctrl+C/Break] to terminate");

				Thread.Sleep(1000);
			}
		}

		public virtual void ProbeUntilDisconnected()
		{
			while (connection.PortIsAvailable())
			{
			////Console.WriteLine(connection.PortName + " is available, probing... Disconnect device or [Ctrl+C/Break] to terminate");

				if (!connection.TryProbePort())
					break;

				Thread.Sleep(1000);
			}

		////Console.WriteLine(connection.PortName + " is no longer available!");

			connection.TryClosePort();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
