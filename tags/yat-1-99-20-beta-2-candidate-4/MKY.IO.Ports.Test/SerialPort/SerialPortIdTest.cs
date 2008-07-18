using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.IO.Ports;

namespace MKY.IO.Ports.Test.SerialPort
{
	[TestFixture]
	public class SerialPortIdTest
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public readonly int PortNumber;
			public readonly string PortName;
			public readonly string[] PortDescriptions;

			public TestSet(int portNumber, string portName, string[] portDescriptions)
			{
				PortNumber = portNumber;
				PortName = portName;
				PortDescriptions = portDescriptions;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] _testSets =
		{
			new TestSet(     1,     "COM1", new string[] { "Serial Communication Port (COM1)", "Serial Port COM1", "serial port com1", "Serial Port Com1" } ),
			new TestSet(     2,     "COM2", new string[] { "Bluetooth Communication Port (COM2)", "COM2 - Bluetooth Communication Port" } ),
			new TestSet(   256,   "COM256", new string[] { "Some Virtual Port (COM256)", "Virtual Port COM256" } ),
			new TestSet( 65536, "COM65536", new string[] { "Super Advanced Port (COM65536)", "Advanced Port COM65536" } ),
		};

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------
		// Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestConstructorAndParse()
		{
			SerialPortId port;

			foreach (TestSet ts in _testSets)
			{
				port = new SerialPortId(ts.PortNumber);
				Assert.AreEqual(ts.PortNumber, port.Number);
				Assert.AreEqual(ts.PortName, port.Name);

				port = new SerialPortId(ts.PortName);
				Assert.AreEqual(ts.PortNumber, port.Number);
				Assert.AreEqual(ts.PortName, port.Name);

				foreach (string description in ts.PortDescriptions)
				{
					port = new SerialPortId(description);
					Assert.AreEqual(ts.PortNumber, port.Number);
					Assert.AreEqual(ts.PortName, port.Name);
				}
			}
		}

		#endregion

		#endregion
	}
}
