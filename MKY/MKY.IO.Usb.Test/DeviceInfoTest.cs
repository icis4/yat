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
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2013 Matthias Kläy.
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

namespace MKY.IO.Usb.Test
{
	/// <summary></summary>
	public static class DeviceInfoTestData
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
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, new string[] { "VID:0ABC PID:1234", "vid:0ABC pid:1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, new string[] { "VID_0ABC PID_1234", "vid_0ABC pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, new string[] { "VID_0ABC&PID_1234", "vid_0ABC&pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, new string[] { "Company (VID:0ABC) Product (PID:1234) Generic USB Hub" } ));
				yield return (new TestCaseData(false,  0x0000,  0x0000, new string[] { " VID:0000  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0000,  0x0001, new string[] { " VID:0000  PID:0001" } ));
				yield return (new TestCaseData(false,  0x0001,  0x0000, new string[] { " VID:0001  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0001, 0x10000, new string[] { " VID:0001 PID:10000" } ));
				yield return (new TestCaseData(false, 0x10000,  0x0001, new string[] { "VID:10000  PID:0001" } ));
				yield return (new TestCaseData(false, 0x10000, 0x10000, new string[] { "VID:10000 PID:10000" } ));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DeviceInfoTest
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[Test, TestCaseSource(typeof(DeviceInfoTestData), "TestCases")]
		public virtual void TestConstructorAndParse(bool isValid, int vendorId, int productId, string[] descriptions)
		{
			if (isValid)
			{
				DeviceInfo di = new DeviceInfo(vendorId, productId);
				Assert.AreEqual(vendorId,  di.VendorId);
				Assert.AreEqual(productId, di.ProductId);

				foreach (string description in descriptions)
				{
					di = DeviceInfo.Parse(description);
					Assert.AreEqual(vendorId,  di.VendorId);
					Assert.AreEqual(productId, di.ProductId);
				}

				// Ensure that ToString() also works if only parts of the info is available
				string s;
				s = di.ToString();
				Assert.IsNotNullOrEmpty(s, "ToString() resulted in invalid string!");
				s = di.ToShortString();
				Assert.IsNotNullOrEmpty(s, "ToShortString() resulted in invalid string!");
				s = di.ToLongString();
				Assert.IsNotNullOrEmpty(s, "ToLongString() resulted in invalid string!");
			}
			else
			{
				try
				{
					DeviceInfo di = new DeviceInfo(vendorId, productId);
					Assert.Fail("Invalid ID pair " + vendorId + "/" + productId + " wasn't properly handled!");
				}
				catch
				{
					// Invalid input must throw an exception before Assert.Fail() above.
				}

				foreach (string description in descriptions)
				{
					try
					{
						DeviceInfo di = DeviceInfo.Parse(description);
						Assert.Fail("Invalid descripton " + description + " wasn't properly handled!");
					}
					catch
					{
						// Invalid input must throw an exception before Assert.Fail() above.
					}
				}
			}
		}

		#endregion

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[Test, TestCaseSource(typeof(DeviceInfoTestData), "TestCases")]
		public virtual void TestSerialization(bool isValid, int vendorId, int productId, string[] descriptions)
		{
			if (isValid)
			{
				string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
				DeviceInfo info = new DeviceInfo(vendorId, productId);
				DeviceInfo infoDeserialized = null;

				// Serialize to file:
				XmlSerializerTest.TestSerializeToFile(filePath, typeof(DeviceInfo), info);

				// Deserialize from file using different methods and verify the result:
				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId, infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);

				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestTolerantDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId, infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);

				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId, infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);
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
