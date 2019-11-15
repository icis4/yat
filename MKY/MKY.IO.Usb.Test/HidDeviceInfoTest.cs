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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
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
	public static class HidDeviceInfoTestData
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
				// Attention:
				// Similar test data exists in DeviceInfoTestData.TestCases{get}.
				// Changes here may have to be applied there too.

				// \remind (2019-11-12 / MKY) there might be a way to create these
				// test cases based on 'DeviceInfoTestData.TestCases', someting like:
				//     foreach (var tcd in DeviceInfoTestData.TestCases)
				//     {
				//         yield return (new TestCaseData(tcd.Arg0, tcd.Arg1, ..., false, 0, 0, tcd.Arg5))
				//     }
				// But haven't found a way right now, accepting duplication for the moment.

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "VID:0ABC PID:1234", "vid:0ABC pid:1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "VID_0ABC PID_1234", "vid_0ABC pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "VID 0ABC PID 1234", "vid 0ABC pid 1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "VID_0ABC&PID_1234", "vid_0ABC&pid_1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "VID 0ABC&PID 1234", "vid 0ABC&pid 1234"} ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, false, 0, 0, new string[] { "Company (VID:0ABC) Product (PID:1234) Generic USB Hub" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "VID:0ABC PID:1234 SNR:XYZ", "vid:0ABC pid:1234 snr:XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "VID_0ABC PID_1234 SNR_XYZ", "vid_0ABC pid_1234 snr_XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "VID 0ABC PID 1234 SNR XYZ", "vid 0ABC pid 1234 snr XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "VID_0ABC&PID_1234&SNR_XYZ", "vid_0ABC&pid_1234&snr_XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR XYZ", "vid 0ABC&pid 1234&snr XYZ" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) XYZ" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä€¢", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR ä€¢", "vid 0ABC&pid 1234&snr ä€¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä€¢", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) ä€¢" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "VID:0ABC PID:1234 SNR:X.Z", "vid:0ABC pid:1234 snr:X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "VID_0ABC PID_1234 SNR_X.Z", "vid_0ABC pid_1234 snr_X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "VID 0ABC PID 1234 SNR X.Z", "vid 0ABC pid 1234 snr X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "VID_0ABC&PID_1234&SNR_X.Z", "vid_0ABC&pid_1234&snr_X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR X.Z", "vid 0ABC&pid 1234&snr X.Z" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X.Z", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) X.Z" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä.¢", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR ä.¢", "vid 0ABC&pid 1234&snr ä.¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä.¢", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) ä.¢" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "VID:0ABC PID:1234 SNR:X 8", "vid:0ABC pid:1234 snr:X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "VID_0ABC PID_1234 SNR_X 8", "vid_0ABC pid_1234 snr_X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "VID 0ABC PID 1234 SNR_X 8", "vid 0ABC pid 1234 snr X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "VID_0ABC&PID_1234&SNR_X 8", "vid_0ABC&pid_1234&snr_X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR_X 8", "vid 0ABC&pid 1234&snr X 8" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "X 8", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) X 8" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä ¢", true, -1, -1, new string[] { "VID 0ABC&PID 1234&SNR_ä ¢", "vid 0ABC&pid 1234&snr ä ¢" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "ä ¢", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) ä ¢" } ));

				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null, true, 0, 0, null)); // Parsing not supported for usage (yet).
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "XYZ", true, 0, 0, null)); // Parsing not supported for usage (yet).

				// Invalid PID/VID:
				yield return (new TestCaseData(false,  0x0000,  0x0000, false, null, true, -1, -1, new string[] { " VID:0000  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0000,  0x0001, false, null, true, -1, -1, new string[] { " VID:0000  PID:0001" } ));
				yield return (new TestCaseData(false,  0x0001,  0x0000, false, null, true, -1, -1, new string[] { " VID:0001  PID:0000" } ));
				yield return (new TestCaseData(false,  0x0001, 0x10000, false, null, true, -1, -1, new string[] { " VID:0001 PID:10000" } ));
				yield return (new TestCaseData(false, 0x10000,  0x0001, false, null, true, -1, -1, new string[] { "VID:10000  PID:0001" } ));
				yield return (new TestCaseData(false, 0x10000, 0x10000, false, null, true, -1, -1, new string[] { "VID:10000 PID:10000" } ));

				// Intentionally strange SNR:
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, false, null,     false, 0,  0, new string[] { "Company (VID:0ABC) Product (PID:1234) VID PID" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "VID",     true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) VID"     } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "VID ABC", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) VID ABC" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "PID",     true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) PID"     } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "PID ABC", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) VID ABC" } ));
				yield return (new TestCaseData( true,  0x0ABC,  0x1234, true, "VID PID", true, -1, -1, new string[] { "Company (VID:0ABC) Product (PID:1234) VID PID" } ));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class HidDeviceInfoTest
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for this test sequence.")]
		[Test, TestCaseSource(typeof(HidDeviceInfoTestData), "TestCases")]
		public virtual void TestConstructorAndParse(bool isValid, int vendorId, int productId, bool matchSerial, string serial, bool matchUsage, int usagePage, int usageId, string[] descriptors)
		{
			// Attention:
			// Similar code exists in DeviceInfoTest.TestConstructorAndParse().
			// Changes here may have to be applied there too.

			if (isValid)
			{
				HidDeviceInfo info;

				if (!matchSerial)
				{
					if (!matchUsage)
					{
						info = new HidDeviceInfo(vendorId, productId);
						Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						Assert.That(info.ProductId, Is.EqualTo(productId));

						foreach (string descriptor in descriptors)
						{
							info = HidDeviceInfo.ParseVidPid(descriptor);
							Assert.That(info.VendorId,  Is.EqualTo(vendorId));
							Assert.That(info.ProductId, Is.EqualTo(productId));
						}
					}
					else // matchUsage
					{
						info = new HidDeviceInfo(vendorId, productId, usagePage, usageId);
						Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						Assert.That(info.ProductId, Is.EqualTo(productId));
						Assert.That(info.UsagePage, Is.EqualTo(usagePage));
						Assert.That(info.UsageId,   Is.EqualTo(usageId));

						foreach (string descriptor in descriptors)
						{
							info = HidDeviceInfo.ParseVidPid(descriptor);
							Assert.That(info.VendorId,  Is.EqualTo(vendorId));
							Assert.That(info.ProductId, Is.EqualTo(productId));
							Assert.That(info.UsagePage, Is.EqualTo(HidDeviceInfo.AnyUsagePage));
							Assert.That(info.UsageId,   Is.EqualTo(HidDeviceInfo.AnyUsageId));

						//// See explanation at HidDeviceInfo.TryParseConsiderately().
						////
						////info = HidDeviceInfo.ParseVidPidUsage(descriptor);
						////Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						////Assert.That(info.ProductId, Is.EqualTo(productId));
						////Assert.That(info.UsagePage, Is.EqualTo(usagePage));
						////Assert.That(info.UsageId,   Is.EqualTo(usageId));
						}
					}
				}
				else // matchSerial
				{
					if (!matchUsage)
					{
						info = new HidDeviceInfo(vendorId, productId, serial);
						Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						Assert.That(info.ProductId, Is.EqualTo(productId));
						Assert.That(info.Serial,    Is.EqualTo(serial));

						foreach (string descriptor in descriptors)
						{
							info = HidDeviceInfo.ParseVidPidSerial(descriptor);
							Assert.That(info.VendorId,  Is.EqualTo(vendorId));
							Assert.That(info.ProductId, Is.EqualTo(productId));
							Assert.That(info.Serial,    Is.EqualTo(serial));
						}
					}
					else // matchUsage
					{
						info = new HidDeviceInfo(vendorId, productId, serial, usagePage, usageId);
						Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						Assert.That(info.ProductId, Is.EqualTo(productId));
						Assert.That(info.Serial,    Is.EqualTo(serial));
						Assert.That(info.UsagePage, Is.EqualTo(usagePage));
						Assert.That(info.UsageId,   Is.EqualTo(usageId));

						foreach (string descriptor in descriptors)
						{
							info = HidDeviceInfo.ParseVidPidSerial(descriptor);
							Assert.That(info.VendorId,  Is.EqualTo(vendorId));
							Assert.That(info.ProductId, Is.EqualTo(productId));
							Assert.That(info.Serial,    Is.EqualTo(serial));
							Assert.That(info.UsagePage, Is.EqualTo(HidDeviceInfo.AnyUsagePage));
							Assert.That(info.UsageId,   Is.EqualTo(HidDeviceInfo.AnyUsageId));

						//// See explanation at HidDeviceInfo.TryParseConsiderately().
						////
						////info = HidDeviceInfo.ParseVidPidSerialUsage(descriptor);
						////Assert.That(info.VendorId,  Is.EqualTo(vendorId));
						////Assert.That(info.ProductId, Is.EqualTo(productId));
						////Assert.That(info.Serial,    Is.EqualTo(serial));
						////Assert.That(info.UsagePage, Is.EqualTo(usagePage));
						////Assert.That(info.UsageId,   Is.EqualTo(usageId));
						}
					}
				}

				// Ensure that ToString() also works if only parts of the info is available
				string s;
				s = info.ToString();
				Assert.That(s, Is.Not.Null.And.Not.Empty, "ToString() resulted in invalid string!");
				s = info.ToShortString();
				Assert.That(s, Is.Not.Null.And.Not.Empty, "ToShortString() resulted in invalid string!");
				s = info.ToLongString();
				Assert.That(s, Is.Not.Null.And.Not.Empty, "ToLongString() resulted in invalid string!");
			}
			else
			{
				try
				{
					HidDeviceInfo dummyInfoToForceException;

					if (!matchSerial)
					{
						if (!matchUsage)
							dummyInfoToForceException = new HidDeviceInfo(vendorId, productId);
						else
							dummyInfoToForceException = new HidDeviceInfo(vendorId, productId, usagePage, usageId);
					}
					else
					{
						if (!matchUsage)
							dummyInfoToForceException = new HidDeviceInfo(vendorId, productId, serial);
						else
							dummyInfoToForceException = new HidDeviceInfo(vendorId, productId, serial, usagePage, usageId);
					}

					UnusedLocal.PreventAnalysisWarning(dummyInfoToForceException);

					if (!matchSerial)
					{
						if (!matchUsage)
							Assert.Fail("Invalid pair " + vendorId + " " + productId + " wasn't properly handled!");
						else
							Assert.Fail("Invalid quadruple " + vendorId + " " + productId + " " + usagePage + " " + usageId + " wasn't properly handled!");
					}
					else
					{
						if (!matchUsage)
							Assert.Fail("Invalid triple " + vendorId + " " + productId + " " + serial + " wasn't properly handled!");
						else
							Assert.Fail("Invalid quintuple " + vendorId + " " + productId + " " + serial + " " + usagePage + " " + usageId + " wasn't properly handled!");
					}
				}
				catch
				{
					// Invalid input must throw an exception before Assert.Fail() above.
				}

				foreach (string descriptor in descriptors)
				{
					try
					{
						HidDeviceInfo dummyInfoToForceException;

						if (!matchSerial)
						{
							dummyInfoToForceException = HidDeviceInfo.ParseVidPid(descriptor);

						//// See explanation at HidDeviceInfo.TryParseConsiderately().
						////
						////if (!matchUsage)
						////	dummyInfoToForceException = HidDeviceInfo.ParseVidPid(descriptor);
						////else
						////	dummyInfoToForceException = HidDeviceInfo.ParseVidPidUsage(descriptor);
						}
						else
						{
							dummyInfoToForceException = HidDeviceInfo.ParseVidPidSerial(descriptor);

						//// See explanation at HidDeviceInfo.TryParseConsiderately().
						////
						////if (!matchUsage)
						////	dummyInfoToForceException = HidDeviceInfo.ParseVidPidSerial(descriptor);
						////else
						////	dummyInfoToForceException = HidDeviceInfo.ParseVidPidSerialUsage(descriptor);
						}

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
		[Test, TestCaseSource(typeof(HidDeviceInfoTestData), "TestCases")]
		public virtual void TestSerialization(bool isValid, int vendorId, int productId, bool matchSerial, string serial, bool matchUsage, int usagePage, int usageId, string[] descriptors)
		{
			// Attention:
			// Similar code exists in DeviceInfoTest.TestSerialization().
			// Changes here may have to be applied there too.

			if (isValid)
			{
				string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
				HidDeviceInfo infoDeserialized = null;
				HidDeviceInfo info;
				if (!matchSerial)
				{
					if (!matchUsage)
						info = new HidDeviceInfo(vendorId, productId);
					else
						info = new HidDeviceInfo(vendorId, productId, usagePage, usageId);
				}
				else // matchSerial
				{
					if (!matchUsage)
						info = new HidDeviceInfo(vendorId, productId, serial);
					else
						info = new HidDeviceInfo(vendorId, productId, serial, usagePage, usageId);
				}

				// Serialize to file:
				XmlSerializerTest.TestSerializeToFile(typeof(HidDeviceInfo), info, filePath);

				// Deserialize from file using different methods and verify the result:
				infoDeserialized = (HidDeviceInfo)XmlSerializerTest.TestDeserializeFromFile(typeof(HidDeviceInfo), filePath);
				Assert.That(infoDeserialized.VendorId,  Is.EqualTo(vendorId));
				Assert.That(infoDeserialized.ProductId, Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(infoDeserialized.Serial, Is.EqualTo(serial));
				}
				if (matchUsage) {
					Assert.That(infoDeserialized.UsagePage, Is.EqualTo(usagePage));
					Assert.That(infoDeserialized.UsageId,   Is.EqualTo(usageId));
				}

				infoDeserialized = (HidDeviceInfo)XmlSerializerTest.TestTolerantDeserializeFromFile(typeof(HidDeviceInfo), filePath);
				Assert.That(infoDeserialized.VendorId,  Is.EqualTo(vendorId));
				Assert.That(infoDeserialized.ProductId, Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(infoDeserialized.Serial, Is.EqualTo(serial));
				}
				if (matchUsage) {
					Assert.That(infoDeserialized.UsagePage, Is.EqualTo(usagePage));
					Assert.That(infoDeserialized.UsageId,   Is.EqualTo(usageId));
				}

				infoDeserialized = (HidDeviceInfo)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(typeof(HidDeviceInfo), filePath);
				Assert.That(infoDeserialized.VendorId,  Is.EqualTo(vendorId));
				Assert.That(infoDeserialized.ProductId, Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(infoDeserialized.Serial, Is.EqualTo(serial));
				}
				if (matchUsage) {
					Assert.That(infoDeserialized.UsagePage, Is.EqualTo(usagePage));
					Assert.That(infoDeserialized.UsageId,   Is.EqualTo(usageId));
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
