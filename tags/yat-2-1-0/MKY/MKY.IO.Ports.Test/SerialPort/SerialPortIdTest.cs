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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using MKY.Test.Xml.Serialization;

using NUnit.Framework;

#endregion

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
			SerialPortId id;

			if (standardPortNumber != 0)
			{
				id = new SerialPortId(standardPortNumber);
				Assert.That(id.StandardPortNumber, Is.EqualTo(standardPortNumber));
				Assert.That(id.Name,               Is.EqualTo(portName));
			}

			id = new SerialPortId(portName);
			Assert.That(id.StandardPortNumber, Is.EqualTo(standardPortNumber));
			Assert.That(id.Name,               Is.EqualTo(portName));

			Assert.That(SerialPortId.TryParse(portName, out id), Is.True);
			Assert.That(id.StandardPortNumber, Is.EqualTo(standardPortNumber));
			Assert.That(id.Name,               Is.EqualTo(portName));

			foreach (string description in portDescriptions)
			{
				id = new SerialPortId(description);
				Assert.That(id.StandardPortNumber, Is.EqualTo(standardPortNumber));
				Assert.That(id.Name,               Is.EqualTo(portName));
			}
		}

		#endregion

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(SerialPortIdTestData), "TestCases")]
		public virtual void TestSerialization(int standardPortNumber, string portName, string[] portDescriptions)
		{
			string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
			SerialPortId id = new SerialPortId(portName);
			SerialPortId idDeserialized = null;

			// Serialize to file:
			XmlSerializerTest.TestSerializeToFile(typeof(SerialPortId), id, filePath);

			// Deserialize from file using different methods and verify the result:
			idDeserialized = (SerialPortId)XmlSerializerTest.TestDeserializeFromFile(typeof(SerialPortId), filePath);
			Assert.That(   (int)idDeserialized, Is.EqualTo(standardPortNumber));
			Assert.That((string)idDeserialized, Is.EqualTo(portName));

			if (portName != "1") // \remind "1" will be treated as byte value by the tolerant deserializer...
			{
				idDeserialized = (SerialPortId)XmlSerializerTest.TestTolerantDeserializeFromFile(typeof(SerialPortId), filePath);
				Assert.That(   (int)idDeserialized, Is.EqualTo(standardPortNumber));
				Assert.That((string)idDeserialized, Is.EqualTo(portName));

				idDeserialized = (SerialPortId)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(typeof(SerialPortId), filePath);
				Assert.That(   (int)idDeserialized, Is.EqualTo(standardPortNumber));
				Assert.That((string)idDeserialized, Is.EqualTo(portName));
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
