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

namespace MKY.IO.Ports.Test.WorkaroundAnalysis.OnOneWorkerThread
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

			var t = new Thread(new ThreadStart(AnalysisWorkerThread));
			t.Name = typeof(AnalysisMain).FullName;
			t.Start();
			t.Join();
		}

		private static void AnalysisWorkerThread()
		{
			try
			{
				for (;;) // forever until [Ctrl+C] or [Ctrl+Break]:
				{
					using (var connection = new Connection(portName))
					{
						while (!connection.TryOpenPort())
						{
							Console.WriteLine("Trying to open " + portName + "... Connect device or [Ctrl+C/Break] to terminate");

							Thread.Sleep(1000);
						}

						while (connection.PortIsAvailable())
						{
							Console.WriteLine(portName + " is available, probing... Disconnect device or [Ctrl+C/Break] to terminate");

							if (!connection.TryProbePort())
								break;

							Thread.Sleep(1000);
						}

						Console.WriteLine(portName + " is no longer available!");

						connection.TryClosePort();
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
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
