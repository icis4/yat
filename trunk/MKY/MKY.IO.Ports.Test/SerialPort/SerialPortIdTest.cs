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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.Diagnostics;
using MKY.IO;

namespace MKY.IO.Ports.Test.SerialPort
{
	/// <summary></summary>
	public static class SerialPortIdTestData
	{
		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(     1,     "COM1", new string[] { "Serial Communication Port (COM1)", "Serial Port COM1", "serial port com1", "Serial Port Com1" } ).SetName("Standard COM1"));
				yield return (new TestCaseData(     2,     "COM2", new string[] { "Bluetooth Communication Port (COM2)", "COM2 - Bluetooth Communication Port" } )                  .SetName("Standard COM2"));
				yield return (new TestCaseData(     3,     "COM3", new string[] { "Some Port (COM3)", "Some (COM3) Port", "(COM3) Some Port" } )                                    .SetName("Standard COM3"));
				yield return (new TestCaseData(     4,     "COM4", new string[] { "Some Port COM4", "Some COM4 Port", "COM4 Some Port" } )                                          .SetName("Standard COM4"));
				yield return (new TestCaseData(   255,   "COM255", new string[] { "Virtual Port (COM255)", "Virtual Port COM255" } )      .SetName("Standard COM255"));
				yield return (new TestCaseData(   256,   "COM256", new string[] { "Virtual Port (COM256)", "Virtual Port COM256" } )      .SetName("Standard COM526"));
				yield return (new TestCaseData(   257,   "COM257", new string[] { "Virtual Port (COM257)", "Virtual Port COM257" } )      .SetName("Standard COM257"));
				yield return (new TestCaseData( 65535, "COM65535", new string[] { "Advanced Port (COM65535)", "Advanced Port COM65535" } ).SetName("Standard COM65535"));
				yield return (new TestCaseData( 65536, "COM65536", new string[] { "Advanced Port (COM65536)", "Advanced Port COM65536" } ).SetName("Standard COM65536"));
				yield return (new TestCaseData(     0,        "Y", new string[] { } ).SetName("Non-Standard Y"));
				yield return (new TestCaseData(     0,       "1Y", new string[] { } ).SetName("Non-Standard 1Y"));
				yield return (new TestCaseData(     0,        "1", new string[] { } ).SetName("Non-Standard 1"));
				yield return (new TestCaseData(     0,    "CNCA0", new string[] { } ).SetName("Non-Standard CNCA0"));
				yield return (new TestCaseData(     0,    "CNCB0", new string[] { } ).SetName("Non-Standard CNCB0"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class SerialPortIdTest
	{
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

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------
		// Tests > Constructor/Parse()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SerialPortIdTestData), "TestCases")]
		public virtual void TestConstructorAndParse(int standardPortNumber, string portName, string[] portDescriptions)
		{
			SerialPortId port;

			if (standardPortNumber != 0)
			{
				port = new SerialPortId(standardPortNumber);
				Assert.AreEqual(standardPortNumber, port.StandardPortNumber);
				Assert.AreEqual(portName, port.Name);
			}

			port = new SerialPortId(portName);
			Assert.AreEqual(standardPortNumber, port.StandardPortNumber);
			Assert.AreEqual(portName, port.Name);

			foreach (string description in portDescriptions)
			{
				port = new SerialPortId(description);
				Assert.AreEqual(standardPortNumber, port.StandardPortNumber);
				Assert.AreEqual(portName, port.Name);
			}
		}

		#endregion

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		[Test, TestCaseSource(typeof(SerialPortIdTestData), "TestCases")]
		public virtual void TestSerialization(int standardPortNumber, string portName, string[] portDescriptions)
		{
			string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
			SerialPortId id = new SerialPortId(portName);
			SerialPortId idDeserialized = null;

			// Serialize to file.
			try
			{
				using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SerialPortId));
					serializer.Serialize(sw, id);
				}
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SerialPortIdTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Deserialize from file.
			try
			{
				using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, true))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(SerialPortId));
					idDeserialized = (SerialPortId)serializer.Deserialize(sr);
				}
			}
			catch (Exception ex)
			{
				TraceEx.WriteException(typeof(SerialPortIdTest), ex);

				// Attention: The following call throws an exception, code below that call won't be executed.
				Assert.Fail("XML deserialize error: " + ex.Message);
			}

			// Verify deserialized object.
			Assert.AreEqual(standardPortNumber, (int)idDeserialized);
			Assert.AreEqual(portName, (string)idDeserialized);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
