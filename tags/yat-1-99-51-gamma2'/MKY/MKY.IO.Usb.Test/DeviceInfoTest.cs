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
// MKY Version 1.0.15
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
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
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "VID:0ABC PID:1234", "vid:0ABC pid:1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "VID_0ABC PID_1234", "vid_0ABC pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "VID 0ABC PID 1234", "vid 0ABC pid 1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "VID_0ABC&PID_1234", "vid_0ABC&pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "VID 0ABC&PID 1234", "vid 0ABC&pid 1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, new string[] { "Company (VID:0ABC) Product (PID:1234) Generic USB Hub" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "VID:0ABC PID:1234 SNR:XYZ", "vid:0ABC pid:1234 snr:XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "VID_0ABC PID_1234 SNR_XYZ", "vid_0ABC pid_1234 snr_XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "VID 0ABC PID 1234 SNR XYZ", "vid 0ABC pid 1234 snr XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "VID_0ABC&PID_1234&SNR_XYZ", "vid_0ABC&pid_1234&snr_XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "VID 0ABC&PID 1234&SNR XYZ", "vid 0ABC&pid 1234&snr XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", new string[] { "Company (VID:0ABC) Product (PID:1234) XYZ" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä€¢", new string[] { "VID 0ABC&PID 1234&SNR ä€¢", "vid 0ABC&pid 1234&snr ä€¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä€¢", new string[] { "Company (VID:0ABC) Product (PID:1234) ä€¢" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "VID:0ABC PID:1234 SNR:X.Z", "vid:0ABC pid:1234 snr:X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "VID_0ABC PID_1234 SNR_X.Z", "vid_0ABC pid_1234 snr_X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "VID 0ABC PID 1234 SNR X.Z", "vid 0ABC pid 1234 snr X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "VID_0ABC&PID_1234&SNR_X.Z", "vid_0ABC&pid_1234&snr_X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "VID 0ABC&PID 1234&SNR X.Z", "vid 0ABC&pid 1234&snr X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", new string[] { "Company (VID:0ABC) Product (PID:1234) X.Z" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä.¢", new string[] { "VID 0ABC&PID 1234&SNR ä.¢", "vid 0ABC&pid 1234&snr ä.¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä.¢", new string[] { "Company (VID:0ABC) Product (PID:1234) ä.¢" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "VID:0ABC PID:1234 SNR:X 8", "vid:0ABC pid:1234 snr:X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "VID_0ABC PID_1234 SNR_X 8", "vid_0ABC pid_1234 snr_X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "VID 0ABC PID 1234 SNR_X 8", "vid 0ABC pid 1234 snr X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "VID_0ABC&PID_1234&SNR_X 8", "vid_0ABC&pid_1234&snr_X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "VID 0ABC&PID 1234&SNR_X 8", "vid 0ABC&pid 1234&snr X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", new string[] { "Company (VID:0ABC) Product (PID:1234) X 8" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä ¢", new string[] { "VID 0ABC&PID 1234&SNR_ä ¢", "vid 0ABC&pid 1234&snr ä ¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä ¢", new string[] { "Company (VID:0ABC) Product (PID:1234) ä ¢" } ));

				yield return (new TestCaseData(false,  0x0000,  0x0000, false, null, new string[] { " VID:0000  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0000,  0x0001, false, null, new string[] { " VID:0000  PID:0001" } ));
				yield return (new TestCaseData(false,  0x0001,  0x0000, false, null, new string[] { " VID:0001  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0001, 0x10000, false, null, new string[] { " VID:0001 PID:10000" } ));
				yield return (new TestCaseData(false, 0x10000,  0x0001, false, null, new string[] { "VID:10000  PID:0001" } ));
				yield return (new TestCaseData(false, 0x10000, 0x10000, false, null, new string[] { "VID:10000 PID:10000" } ));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class DeviceInfoTest
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[Test, TestCaseSource(typeof(DeviceInfoTestData), "TestCases")]
		public virtual void TestConstructorAndParse(bool isValid, int vendorId, int productId, bool matchSerial, string serial, string[] descriptors)
		{
			if (isValid)
			{
				DeviceInfo info;

				if (!matchSerial)
				{
					info = new DeviceInfo(vendorId, productId);
					Assert.AreEqual(vendorId,  info.VendorId);
					Assert.AreEqual(productId, info.ProductId);

					foreach (string descriptor in descriptors)
					{
						info = DeviceInfo.ParseFromVidAndPid(descriptor);
						Assert.AreEqual(vendorId,  info.VendorId);
						Assert.AreEqual(productId, info.ProductId);
					}
				}
				else
				{
					info = new DeviceInfo(vendorId, productId, serial);
					Assert.AreEqual(vendorId,  info.VendorId);
					Assert.AreEqual(productId, info.ProductId);
					Assert.AreEqual(serial,    info.Serial);

					foreach (string descriptor in descriptors)
					{
						info = DeviceInfo.ParseFromVidAndPidAndSerial(descriptor);
						Assert.AreEqual(vendorId,  info.VendorId);
						Assert.AreEqual(productId, info.ProductId);
						Assert.AreEqual(serial,    info.Serial);
					}
				}

				// Ensure that ToString() also works if only parts of the info is available
				string s;
				s = info.ToString();
				Assert.IsNotNullOrEmpty(s, "ToString() resulted in invalid string!");
				s = info.ToShortString();
				Assert.IsNotNullOrEmpty(s, "ToShortString() resulted in invalid string!");
				s = info.ToLongString();
				Assert.IsNotNullOrEmpty(s, "ToLongString() resulted in invalid string!");
			}
			else
			{
				try
				{
					DeviceInfo dummyInfoToForceException;

					if (!matchSerial)
						dummyInfoToForceException = new DeviceInfo(vendorId, productId);
					else
						dummyInfoToForceException = new DeviceInfo(vendorId, productId, serial);

					UnusedLocal.PreventAnalysisWarning(dummyInfoToForceException);

					if (!matchSerial)
						Assert.Fail("Invalid pair " + vendorId + "/" + productId + " wasn't properly handled!");
					else
						Assert.Fail("Invalid triple " + vendorId + "/" + productId + "/" + serial + " wasn't properly handled!");
				}
				catch
				{
					// Invalid input must throw an exception before Assert.Fail() above.
				}

				foreach (string descriptor in descriptors)
				{
					try
					{
						DeviceInfo dummyInfoToForceException;

						if (!matchSerial)
							dummyInfoToForceException = DeviceInfo.ParseFromVidAndPid(descriptor);
						else
							dummyInfoToForceException = DeviceInfo.ParseFromVidAndPidAndSerial(descriptor);

						UnusedLocal.PreventAnalysisWarning(dummyInfoToForceException);

						Assert.Fail("Invalid descripton " + descriptor + " wasn't properly handled!");
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
		public virtual void TestSerialization(bool isValid, int vendorId, int productId, bool matchSerial, string serial, string[] descriptors)
		{
			if (isValid)
			{
				string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
				DeviceInfo infoDeserialized = null;
				DeviceInfo info;
				if (!matchSerial)
					info = new DeviceInfo(vendorId, productId);
				else
					info = new DeviceInfo(vendorId, productId, serial);

				// Serialize to file:
				XmlSerializerTest.TestSerializeToFile(filePath, typeof(DeviceInfo), info);

				// Deserialize from file using different methods and verify the result:
				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId,  infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);
				if (matchSerial)
					Assert.AreEqual(serial, infoDeserialized.Serial);

				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestTolerantDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId,  infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);
				if (matchSerial)
					Assert.AreEqual(serial, infoDeserialized.Serial);

				infoDeserialized = (DeviceInfo)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(filePath, typeof(DeviceInfo));
				Assert.AreEqual(vendorId,  infoDeserialized.VendorId);
				Assert.AreEqual(productId, infoDeserialized.ProductId);
				if (matchSerial)
					Assert.AreEqual(serial, infoDeserialized.Serial);
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
