﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.22
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using MKY.IO.Usb;
using MKY.IO.Usb.Test;
using MKY.Test.Xml.Serialization;

using NUnit.Framework;

#endregion

namespace MKY.IO.Serial.Usb.Test
{
	/// <summary></summary>
	[TestFixture]
	public class SerialHidPortSettingsTest
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

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(DeviceInfoTestData), "TestCases")]
		public virtual void TestSerialization(bool isValid, int vendorId, int productId, bool matchSerial, string serial, string[] descriptors)
		{
			if (isValid)
			{
				string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
				SerialHidDeviceSettings settingsDeserialized = null;
				SerialHidDeviceSettings settings = new SerialHidDeviceSettings();
				if (!matchSerial)
					settings.DeviceInfo = new DeviceInfo(vendorId, productId);
				else
					settings.DeviceInfo = new DeviceInfo(vendorId, productId, serial);

				// Serialize to file:
				XmlSerializerTest.TestSerializeToFile(filePath, typeof(SerialHidDeviceSettings), settings);

				// Deserialize from file using different methods and verify the result:
				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.That(settingsDeserialized.DeviceInfo.VendorId,   Is.EqualTo(vendorId));
				Assert.That(settingsDeserialized.DeviceInfo.ProductId,  Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(settingsDeserialized.DeviceInfo.Serial, Is.EqualTo(serial));
				}
				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestTolerantDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.That(settingsDeserialized.DeviceInfo.VendorId,   Is.EqualTo(vendorId));
				Assert.That(settingsDeserialized.DeviceInfo.ProductId,  Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(settingsDeserialized.DeviceInfo.Serial, Is.EqualTo(serial));
				}
				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.That(settingsDeserialized.DeviceInfo.VendorId,   Is.EqualTo(vendorId));
				Assert.That(settingsDeserialized.DeviceInfo.ProductId,  Is.EqualTo(productId));
				if (matchSerial) {
					Assert.That(settingsDeserialized.DeviceInfo.Serial, Is.EqualTo(serial));
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
