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
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using MKY.IO.Ports;

namespace MKY.IO.Ports.Test.SerialPort
{
	/// <summary></summary>
	[TestFixture]
	public class SerialPortIdTest
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public readonly int StandardPortNumber;
			public readonly string PortName;
			public readonly string[] PortDescriptions;

			public TestSet(int standardPortNumber, string portName, string[] portDescriptions)
			{
				StandardPortNumber = standardPortNumber;
				PortName           = portName;
				PortDescriptions   = portDescriptions;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] testSets =
		{
			new TestSet(     1,     "COM1", new string[] { "Serial Communication Port (COM1)", "Serial Port COM1", "serial port com1", "Serial Port Com1" } ),
			new TestSet(     2,     "COM2", new string[] { "Bluetooth Communication Port (COM2)", "COM2 - Bluetooth Communication Port" } ),
			new TestSet(     3,     "COM3", new string[] { "Some Port (COM3)", "Some (COM3) Port", "(COM3) Some Port" } ),
			new TestSet(     4,     "COM4", new string[] { "Some Port COM4", "Some COM4 Port", "COM4 Some Port" } ),
			new TestSet(   255,   "COM255", new string[] { "Virtual Port (COM255)", "Virtual Port COM255" } ),
			new TestSet(   256,   "COM256", new string[] { "Virtual Port (COM256)", "Virtual Port COM256" } ),
			new TestSet(   257,   "COM257", new string[] { "Virtual Port (COM257)", "Virtual Port COM257" } ),
			new TestSet( 65535, "COM65535", new string[] { "Advanced Port (COM65535)", "Advanced Port COM65535" } ),
			new TestSet( 65536, "COM65536", new string[] { "Advanced Port (COM65536)", "Advanced Port COM65536" } ),
			new TestSet(     0,        "Y", new string[] { } ),
			new TestSet(     0,       "1Y", new string[] { } ),
			new TestSet(     0,        "1", new string[] { } ),
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

		/// <summary></summary>
		[Test]
		public virtual void TestConstructorAndParse()
		{
			SerialPortId port;

			foreach (TestSet ts in this.testSets)
			{
				if (ts.StandardPortNumber != 0)
				{
					port = new SerialPortId(ts.StandardPortNumber);
					Assert.AreEqual(ts.StandardPortNumber, port.StandardPortNumber);
					Assert.AreEqual(ts.PortName, port.Name);
				}

				port = new SerialPortId(ts.PortName);
				Assert.AreEqual(ts.StandardPortNumber, port.StandardPortNumber);
				Assert.AreEqual(ts.PortName, port.Name);

				foreach (string description in ts.PortDescriptions)
				{
					port = new SerialPortId(description);
					Assert.AreEqual(ts.StandardPortNumber, port.StandardPortNumber);
					Assert.AreEqual(ts.PortName, port.Name);
				}
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
