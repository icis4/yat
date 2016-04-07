//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2007-2016 Matthias Kläy.
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
using System.Diagnostics;
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
		private static IEnumerable<int> Lines
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
					foreach (int lines in Lines)
					{
						string name = nameI + lines.ToString(CultureInfo.InvariantCulture);
						yield return (new TestCaseData(i.Value2, lines).SetName(name));
					}
				}
			}
		}

		#endregion
	}

	/// <remarks>
	/// Currently, neither device/driver loses any data. However, measurements of
	/// 'TestContinuousReceivingOfSIR' (now 'PerformContinuousReceivingOfECHO')
	/// on 2008-06-15..18 resulted in the following figures:
	/// 
	/// Laptop @ Home (WinXP) with MCT U232:
	/// - 2008-06-15 @ 1613 :  ~20 missing chars out of   ~600'000 chars total
	/// - 2008-06-15 @ 1649 :  ~10 missing chars out of   ~360'000 chars total
	/// - 2008-06-15 @ 1813 :  ~10 missing chars out of   ~360'000 chars total
	/// - 2008-06-16 @ 1032 : ~130 missing chars out of ~9'000'000 chars total
	/// - 2008-06-16 @ 2245 :   ~6 missing chars out of   ~360'000 chars total
	/// 
	/// Desktop @ Work (WinXP) with Prolific PL-2303:
	/// - 2008-06-18 @ 1111 :    0 missing chars out of   ~360'000 chars total :-)
	/// 
	/// => MCT reproducibly lost 1 char/byte per approx. 70'000 chars/bytes !!!
	/// </remarks>
	[TestFixture, Explicit("This test fixture assesses the reliability of serial port drivers. It does not perform any tests. It is only useful for measurments and analysis.")]
	public class ReliabilityAnalysis
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int WaitForOperation = 200;
		private const int AdditionalThreads = 10;

		private const string CommandToEcho = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwyxyz0123456789";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private System.IO.Ports.SerialPort port;
		private StreamWriter file;

		private int receivedBytes = 0;
		private int receivedBytesOfCurrentLine = 0;
		private int receivedLines = 0;
		private ReaderWriterLockSlim receivedDataLock = new ReaderWriterLockSlim();

		private int receivedErrors = 0;
		private ReaderWriterLockSlim receivedErrorLock = new ReaderWriterLockSlim();

		Thread[] threads = null;
		private bool isOngoing = false;

		#endregion

		#region TestFixture
		//==========================================================================================
		// TestFixture
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown()
		{
			Temp.CleanTempPath(GetType());
		}

		#endregion

		#region TestInstance
		//==========================================================================================
		// TestInstance
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp", Justification = "Naming according to NUnit.")]
		[SetUp]
		public virtual void SetUp()
		{
			this.isOngoing = false;

			// Create some additional threads to create some workload on the process:
			this.threads = new Thread[AdditionalThreads];
			for (int i = 0; i < this.threads.Length; i++)
			{
				this.threads[i] = new Thread(new ThreadStart(thread_DoSomething));
				this.threads[i].Start();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TearDown]
		public virtual void TearDown()
		{
			this.isOngoing = false;

			// Join the additional threads:
			foreach (Thread t in this.threads)
			{
				Debug.Assert(t.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");
				t.Join();
			}

			// Ensure that echoing has stopped (e.g. in case of an exception):
			if ((this.port != null) && (this.port.IsOpen))
			{
				this.port.Write(new byte[]{ 0x1B }, 0, 1); // <ESC> to quit ECHO mode.
				Thread.Sleep(WaitForOperation);

				this.port.Close();
				this.port.Dispose();
				this.port = null;
			}
		}

		#endregion

		#region Analysis
		//==========================================================================================
		// Analysis
		//==========================================================================================

		#region Analysis > PerformSubsequentTransmissionOfECHO
		//------------------------------------------------------------------------------------------
		// Analysis > PerformSubsequentTransmissionOfECHO
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(AnalysisTestData), "TestCases")]
		public virtual void PerformSubsequentTransmissionOfECHO(string portName, int linesToTransmit)
		{
			// Create file for logging:
			string filePath = Temp.MakeTempFilePath(GetType(), "SubsequentECHO-" + portName + "-" + linesToTransmit.ToString(CultureInfo.InvariantCulture), ".txt");
			this.file = new StreamWriter(filePath);

			// Open port:
			this.port = new System.IO.Ports.SerialPort();
			this.port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
			this.port.NewLine = "\r\n"; // <CR><LF>
			this.port.PortName = portName;
			this.port.Open();
			Assert.IsTrue(port.IsOpen);

			// Prepare transmission:
			Thread.Sleep(WaitForOperation);
			this.port.WriteLine(""); // Sync before requesting ECHO.
			Thread.Sleep(WaitForOperation);
			this.port.ReadExisting(); // Clear sync data.
			Trace.WriteLine(">> ECHO 1");
			this.port.WriteLine("ECHO 1"); // Activate single echo mode.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ECHO C", this.port.ReadLine(), "Failed to initiate ECHO mode 1!");

			this.port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);

			// Perform ECHO:
			for (int i = 0; i < linesToTransmit; i++)
			{
				Trace.WriteLine(">> line #" + i);
				this.port.WriteLine(CommandToEcho); // Request single echo.
			}
			Thread.Sleep(WaitForOperation);

			// Terminate transmission:
			this.port.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);

			this.port.Write(new byte[]{ 0x1B }, 0, 1); // <ESC> to quit ECHO mode.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("",       this.port.ReadLine(), "Failed to quit ECHO mode!");
			Assert.AreEqual("ECHO A", this.port.ReadLine(), "Failed to quit ECHO mode!");

			this.isOngoing = false;

			this.port.Close();
			this.port.Dispose();
			this.port = null;

			// Close file:
			this.file.Close();

			// Process results:
			this.receivedDataLock.EnterReadLock();
			int receivedBytes = this.receivedBytes;
			int receivedLines = this.receivedLines;
			this.receivedDataLock.ExitReadLock();

			this.receivedErrorLock.EnterReadLock();
			int receivedErrors = this.receivedErrors;
			this.receivedErrorLock.ExitReadLock();

			int sentBytes = (linesToTransmit * (CommandToEcho.Length + 2)); // + 2 = <CR><LF>
			bool exact = (receivedBytes == sentBytes);

			// Output summary:
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine(@"Summary for """ + portName + @""":");
			sb.AppendLine("Sent...");
			sb.AppendLine("..." + sentBytes.ToString(CultureInfo.InvariantCulture) + " bytes.");
			sb.AppendLine("Received...");
			sb.AppendLine("..." + receivedBytes.ToString(CultureInfo.InvariantCulture) + " bytes, while...");
			sb.AppendLine("..." + receivedErrors.ToString(CultureInfo.InvariantCulture) + " errors occured.");
			sb.AppendLine();

			if (exact)
				Console.WriteLine(sb.ToString());
			else
				Console.Error.WriteLine(sb.ToString());

			// Let the test fail in case of non-exact result:
			if (!exact)
				Assert.Fail("Inexact result!" + Environment.NewLine + @"See ""Output"" for details.");
		}

		#endregion

		#region Analysis > PerformContinuousReceivingOfSIR
		//------------------------------------------------------------------------------------------
		// Analysis > PerformContinuousReceivingOfSIR
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(AnalysisTestData), "TestCases")]
		public virtual void PerformContinuousReceivingOfECHO(string portName, int linesToReceive)
		{
			// Create file for logging:
			string filePath = Temp.MakeTempFilePath(GetType(), "ContinuousECHO-" + portName + "-" + linesToReceive.ToString(CultureInfo.InvariantCulture), ".txt");
			this.file = new StreamWriter(filePath);

			// Open port:
			this.port = new System.IO.Ports.SerialPort();
			this.port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(port_ErrorReceived);
			this.port.NewLine = "\r\n"; // <CR><LF>
			this.port.PortName = portName;
			this.port.Open();
			Assert.IsTrue(port.IsOpen);

			// Prepare transmission:
			Thread.Sleep(WaitForOperation);
			this.port.WriteLine(""); // Sync before requesting continuous ECHO.
			Thread.Sleep(WaitForOperation);
			this.port.ReadExisting(); // Clear sync data.
			Trace.WriteLine(">> ECHO 2");
			this.port.WriteLine("ECHO 2"); // Activate continuous echo mode.
			Thread.Sleep(WaitForOperation);
			Assert.AreEqual("ECHO C", this.port.ReadLine(), "Failed to initiate ECHO mode 2!");

			this.port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);

			// Perform ECHO:
			Trace.WriteLine(">> " + CommandToEcho);
			this.port.WriteLine(CommandToEcho); // Request continuous echo.
			int receivedLines = 0;
			do
			{
				Thread.Sleep(1); // Wait just a little => improves accuracy in terms of number of received lines.

				this.receivedDataLock.EnterReadLock();
				receivedLines = this.receivedLines;
				this.receivedDataLock.ExitReadLock();
			}
			while (receivedLines < linesToReceive);
			Thread.Sleep(WaitForOperation);

			// Terminate transmission:
			this.port.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);

			this.port.Write(new byte[]{ 0x1B }, 0, 1); // <ESC> to quit ECHO mode.
			Thread.Sleep(WaitForOperation);

			this.isOngoing = false;

			this.port.Close();
			this.port.Dispose();
			this.port = null;

			// Close file:
			this.file.Close();

			// Process results:
			this.receivedDataLock.EnterReadLock();
			int receivedBytes = this.receivedBytes;
			receivedBytes    -= this.receivedBytesOfCurrentLine; // Account for incomplete lines.
			receivedLines     = this.receivedLines;
			this.receivedDataLock.ExitReadLock();

			this.receivedErrorLock.EnterReadLock();
			int receivedErrors = this.receivedErrors;
			this.receivedErrorLock.ExitReadLock();

			float bytesPerLine = (float)receivedBytes / receivedLines;
			int bplA = (int)Math.Round(bytesPerLine);
			int bplB = (int)Math.Round(bytesPerLine * 1000) / 1000;
			bool exact = (bplA == bplB);

			// Output summary:
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine(@"Summary for """ + portName + @""":");
			sb.AppendLine("Received...");
			sb.Append    ("..." + receivedBytes.ToString(CultureInfo.InvariantCulture) + " bytes in ");
			sb.AppendLine(        receivedLines.ToString(CultureInfo.InvariantCulture) + " lines, resulting in...");
			sb.AppendLine("..." + (exact ? "exactly " : "an average of ") + bytesPerLine.ToString((exact ? "F1" : "F3"), CultureInfo.InvariantCulture) + " bytes per line, while...");
			sb.AppendLine("..." + receivedErrors.ToString(CultureInfo.InvariantCulture) + " errors occured.");
			sb.AppendLine();

			if (exact)
				Console.WriteLine(sb.ToString());
			else
				Console.Error.WriteLine(sb.ToString());

			// Let the test fail in case of non-exact result:
			if (!exact)
				Assert.Fail("Inexact result!" + Environment.NewLine + @"See ""Output"" for details.");
		}

		#endregion

		#endregion

		#region Port
		//==========================================================================================
		// Port
		//==========================================================================================

		private void port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			int bytesToRead = this.port.BytesToRead;
			byte[] buffer = new byte[bytesToRead];
			this.port.Read(buffer, 0, bytesToRead);

			this.receivedDataLock.EnterWriteLock();
			this.receivedBytes += buffer.Length;
			this.receivedDataLock.ExitWriteLock();

			foreach (byte b in buffer)
			{
				this.file.Write(Convert.ToChar(b));
				if (b == 0x0A) // <LF>
				{
					this.receivedDataLock.EnterWriteLock();
					this.receivedBytesOfCurrentLine = 0; // Line is complete.
					int receivedLines = this.receivedLines++;
					this.receivedDataLock.ExitWriteLock();

					Trace.WriteLine("<< line #" + receivedLines);
				}
				else
				{
					this.receivedDataLock.EnterWriteLock();
					this.receivedBytesOfCurrentLine++; // Line is incomplete.
					this.receivedDataLock.ExitWriteLock();
				}
			}
		}

		private void port_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
		{
			this.receivedErrorLock.EnterWriteLock();
			this.receivedErrors++;
			this.receivedErrorLock.ExitWriteLock();
		}

		#endregion

		#region Threads
		//==========================================================================================
		// Threads
		//==========================================================================================

		private void thread_DoSomething()
		{
			Random r = new Random(RandomEx.NextPseudoRandomSeed());

			bool isOngoing = false;
			do
			{
				for (int i = 0, j = 0; i < 2000; i++, j += 2)
					j = j - i;

				Thread.Sleep(r.Next(10, 100));

				this.receivedDataLock.EnterReadLock();
				isOngoing = this.isOngoing;
				this.receivedDataLock.ExitReadLock();
			}
			while (isOngoing);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
