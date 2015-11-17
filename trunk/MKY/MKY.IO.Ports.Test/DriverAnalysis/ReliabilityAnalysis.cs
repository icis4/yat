//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2007-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using MKY.Collections.Generic;

using NUnit.Framework;

#endregion

namespace MKY.IO.Ports.Test.DriverAnalysis
{
	/// <summary></summary>
	public static class AnalysisTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		private static IEnumerable<Pair<string, string>> Interfaces
		{
			get
			{
				// Modify the test board and un-comment the according tests as desired:
				yield return (new Pair<string, string>("MCT",      "COM14"));
				yield return (new Pair<string, string>("FTDI",     "COM24"));
				yield return (new Pair<string, string>("Prolific", "COM31"));
				yield return (new Pair<string, string>("MT",       "COM43"));
				
			}
		}

		/// <summary></summary>
		private static IEnumerable<int> LinesToReceive
		{
			get
			{
				yield return (   100); // Approx. 10 seconds
				yield return (  6000); // Approx. 10 minutes
				yield return (360000); // Approx. 10 hours
			}
		}

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				foreach (Pair<string, string> i in Interfaces)
				{
					string nameI = i.Value1 + "_" + i.Value2 + "_";
					foreach (int linesToReceive in LinesToReceive)
					{
						string name = nameI + linesToReceive.ToString(CultureInfo.InvariantCulture);
						yield return (new TestCaseData(i.Value2, linesToReceive).SetName(name));
					}
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// Measurements 'TestContinuousReceivingOfSIR' 2008-06-15..18
	/// 
	/// Laptop @ Home with MCT U232:
	/// - 2008-06-15 @ 1613 :  ~20 missing chars out of   ~600'000 chars total
	/// - 2008-06-15 @ 1649 :  ~10 missing chars out of   ~360'000 chars total
	/// - 2008-06-15 @ 1813 :  ~10 missing chars out of   ~360'000 chars total
	/// - 2008-06-16 @ 1032 : ~130 missing chars out of ~9'000'000 chars total
	/// - 2008-06-16 @ 2245 :   ~6 missing chars out of   ~360'000 chars total
	/// 
	/// Desktop @ Work with Prolific PL-2303:
	/// - 2008-06-18 @ 1111 :    0 missing chars out of   ~360'000 chars total :-)
	/// </remarks>
	[TestFixture, Explicit("This test fixture assesses the reliability of serial port drivers. It does not perform any tests. It is only useful for measurments and analysis.")]
	public class ReliabilityAnalysis
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private const int AdditionalThreads = 10;

		private System.IO.Ports.SerialPort port;

		private bool receiveIsOngoing = false;

		private int receivedBytes = 0;
		private int receivedLines = 0;
		private ReaderWriterLock receivedDataLock = new ReaderWriterLock();

		private int receivedErrors = 0;
		private ReaderWriterLock receivedErrorLock = new ReaderWriterLock();

		private StreamWriter file;

		#endregion

		#region Tear Down Fixture
		//==========================================================================================
		// Tear Down Fixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region ContinuousReceivingOfSIR
		//==========================================================================================
		// ContinuousReceivingOfSIR
		//==========================================================================================

		/// <summary></summary>
		[Test, TestCaseSource(typeof(AnalysisTestData), "TestCases")]
		public virtual void PerformContinuousReceivingOfSIR(string portName, int linesToReceive)
		{
			// Create some additional threads that create some workload on the process:
			Thread[] threads = new Thread[AdditionalThreads];
			for (int i = 0; i < AdditionalThreads; i++)
			{
				threads[i] = new Thread(new ThreadStart(thread_DoSomething));
				threads[i].Start();
			}

			// Create file for logging:
			string filePath = Temp.MakeTempFilePath(GetType(), portName + "-" + linesToReceive.ToString(CultureInfo.InvariantCulture), ".txt");
			this.file = new StreamWriter(filePath);

			// Open port and start communication:
			this.port = new System.IO.Ports.SerialPort();
			this.port.PortName = portName;
			this.port.Open();

			Thread.Sleep(500);
			this.port.WriteLine("SI"); // Sync before requesting continuous values.
			Thread.Sleep(500);

			this.port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
			this.port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);

			this.port.WriteLine("SIR"); // Request continuous values.

			this.receivedDataLock.AcquireWriterLock(Timeout.Infinite);
			this.receiveIsOngoing = true;
			this.receivedDataLock.ReleaseWriterLock();

			int receivedLines = 0;
			do
			{
				Thread.Sleep(1); // Wait just a little => improves accuracy in terms of number of received lines.

				this.receivedDataLock.AcquireReaderLock(Timeout.Infinite);
				receivedLines = this.receivedLines;
				this.receivedDataLock.ReleaseReaderLock();
			}
			while (receivedLines < linesToReceive);

			this.port.WriteLine("SI"); // Stop continuous values.
			Thread.Sleep(500);

			this.receivedDataLock.AcquireWriterLock(Timeout.Infinite);
			this.receiveIsOngoing = false;
			this.receivedDataLock.ReleaseWriterLock();

			this.receivedDataLock.AcquireReaderLock(Timeout.Infinite);
			receivedLines  = this.receivedLines;
			this.receivedDataLock.ReleaseReaderLock();

			this.receivedErrorLock.AcquireReaderLock(Timeout.Infinite);
			int receivedErrors = this.receivedErrors;
			this.receivedErrorLock.ReleaseReaderLock();

			this.port.Close();
			this.port.Dispose();
			this.port = null;

			// Close file:
			this.file.Close();

			// Process results:
			float bytesPerLine = (float)receivedBytes / receivedLines;
			int bplA = (int)Math.Round(bytesPerLine);
			int bplB = (int)Math.Round(bytesPerLine * 1000) / 1000;
			bool exact = (bplA == bplB);

			// Output summary:
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine(@"Summary for """ + portName + @""":");
			sb.AppendLine("Received...");
			sb.AppendLine("..." + receivedBytes.ToString(CultureInfo.InvariantCulture) + " bytes, resulting in...");
			sb.AppendLine("..." + receivedLines.ToString(CultureInfo.InvariantCulture) + " lines, resulting in...");
			sb.AppendLine("..." + (exact ? "exactly" : "roughly") + " " + bytesPerLine.ToString("F4", CultureInfo.InvariantCulture) + " bytes per line, while...");
			sb.AppendLine("..." + receivedErrors.ToString(CultureInfo.InvariantCulture) + " errors occured.");
			sb.AppendLine();

			if (exact)
				Console.WriteLine(sb.ToString());
			else
				Console.Error.WriteLine(sb.ToString());

			// Join the additional threads:
			for (int i = 0; i < AdditionalThreads; i++)
				threads[i].Join();

			// Let the test fail in case of non-exact result:
			if (!exact)
				Assert.Fail("Inexact result!" + Environment.NewLine + @"See ""Output"" for details.");
		}

		private void port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			int bytesToRead = this.port.BytesToRead;
			byte[] buffer = new byte[bytesToRead];
			this.port.Read(buffer, 0, bytesToRead);

			this.receivedDataLock.AcquireWriterLock(Timeout.Infinite);
			this.receivedBytes += buffer.Length;
			this.receivedDataLock.ReleaseWriterLock();

			foreach (byte b in buffer)
			{
				this.file.Write(Convert.ToChar(b));
				if (b == 0x0A) // <LF>
				{
					this.receivedDataLock.AcquireWriterLock(Timeout.Infinite);
					this.receivedLines++;
					this.receivedDataLock.ReleaseWriterLock();
				}
			}
		}

		private void port_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			this.receivedErrorLock.AcquireWriterLock(Timeout.Infinite);
			this.receivedErrors++;
			this.receivedErrorLock.ReleaseWriterLock();
		}

		private void thread_DoSomething()
		{
			Random r = new Random(RandomEx.NextPseudoRandomSeed());

			bool receiveIsStillOngoing = false;
			do
			{
				for (int i = 0, j = 0; i < 2000; i++, j += 2)
					j = j - i;

				Thread.Sleep(r.Next(10, 100));

				this.receivedDataLock.AcquireReaderLock(Timeout.Infinite);
				receiveIsStillOngoing = this.receiveIsOngoing;
				this.receivedDataLock.ReleaseReaderLock();
			}
			while (receiveIsStillOngoing);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
