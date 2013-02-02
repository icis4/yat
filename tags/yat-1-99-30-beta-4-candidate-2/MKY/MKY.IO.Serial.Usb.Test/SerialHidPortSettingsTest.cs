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
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
				SerialHidDeviceSettings settings = new SerialHidDeviceSettings();
				settings.DeviceInfo = new DeviceInfo(vendorId, productId);
				SerialHidDeviceSettings settingsDeserialized = null;

				// Serialize to file:
				XmlSerializerTest.TestSerializeToFile(filePath, typeof(SerialHidDeviceSettings), settings);

				// Deserialize from file using different methods and verify the result:
				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.AreEqual(vendorId, settingsDeserialized.DeviceInfo.VendorId);
				Assert.AreEqual(productId, settingsDeserialized.DeviceInfo.ProductId);

				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestTolerantDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.AreEqual(vendorId, settingsDeserialized.DeviceInfo.VendorId);
				Assert.AreEqual(productId, settingsDeserialized.DeviceInfo.ProductId);

				settingsDeserialized = (SerialHidDeviceSettings)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(filePath, typeof(SerialHidDeviceSettings));
				Assert.AreEqual(vendorId, settingsDeserialized.DeviceInfo.VendorId);
				Assert.AreEqual(productId, settingsDeserialized.DeviceInfo.ProductId);
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
