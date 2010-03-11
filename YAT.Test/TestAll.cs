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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections;
using System.Text;

using NUnit.Framework;

namespace YAT.Test
{
	public class TestAll
	{
		#region Suite
		//==========================================================================================
		// Suite
		//==========================================================================================

		[Suite]
		public static IEnumerable Suite
		{
			get
			{
				ArrayList suite = new ArrayList();

				// libusb.NET
				// libusb.NET.Test is a manual test
				// Set libusb.NET.Test as start up project and run it
				//suite.Add(new libusb.NET.Test.<>());

				// MKY.IO.Ports.Test
				suite.Add(new MKY.IO.Ports.Test.SerialPort.SerialPortIdTest());
				suite.Add(new MKY.IO.Ports.Test.SerialPort.SerialPortSettingsTest());

				// MKY.IO.Serial.Test
				suite.Add(new MKY.IO.Serial.Test.SerialPort.SerialPortSettingsTest());
				suite.Add(new MKY.IO.Serial.Test.Socket.SocketSettingsTest());

				// MKY.IO.Usb.Test
				suite.Add(new MKY.IO.Usb.Test.UsbDeviceIdTest());

				// MKY.Utilities.Test
				suite.Add(new MKY.Utilities.Test.IO.XPathTest());
				suite.Add(new MKY.Utilities.Test.Settings.DocumentSettingsHandlerTest());
				suite.Add(new MKY.Utilities.Test.Types.XByteTest());
				suite.Add(new MKY.Utilities.Test.Types.XInt32Test());
				suite.Add(new MKY.Utilities.Test.Types.XUInt64Test());

				// MKY.Windows.Forms.Test
				// MKY.Windows.Forms.Test.FastListBoxTest is a manual test
				// Set MKY.Windows.Forms.Test as start up project and run it
				//suite.Add(new MKY.Windows.Forms.Test.<>());

				// YAT.Controller.Test
				suite.Add(new YAT.Controller.Test.ControllerTest());

				// YAT.Domain.Test
				suite.Add(new YAT.Domain.Test.Parser.ParserTest());
				suite.Add(new YAT.Domain.Test.TextTerminal.SubstitutionParserTest());

				// YAT.Model.Test
				suite.Add(new YAT.Model.Test.ClearTest());
				suite.Add(new YAT.Model.Test.FileHandlingTest());
				suite.Add(new YAT.Model.Test.StressTest());
				suite.Add(new YAT.Model.Test.TransmissionTest());

				// YAT.Settings.Test
				suite.Add(new YAT.Settings.Test.FileVersionsTest());
				suite.Add(new YAT.Settings.Test.XmlTest());

				return (suite);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
