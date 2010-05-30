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
	/// <summary>
	/// The YAT test suite.
	/// </summary>
	public class TestAll
	{
		#region Suite
		//==========================================================================================
		// Suite
		//==========================================================================================

		/// <summary>
		/// The NUnit enumerable YAT test suite.
		/// </summary>
		[Suite]
		public static IEnumerable Suite
		{
			get
			{
				ArrayList suite = new ArrayList();

				// MKY.IO.Ports.Test
				suite.Add(typeof(MKY.IO.Ports.Test.SerialPort.SerialPortIdTest));
				suite.Add(typeof(MKY.IO.Ports.Test.SerialPort.SerialPortSettingsTest));

				// MKY.IO.Serial.Test
				suite.Add(typeof(MKY.IO.Serial.Test.SerialPort.SerialPortSettingsTest));
				suite.Add(typeof(MKY.IO.Serial.Test.Socket.SocketSettingsTest));

				// MKY.IO.Usb.Test
				suite.Add(typeof(MKY.IO.Usb.Test.ConnectionTest));
				suite.Add(typeof(MKY.IO.Usb.Test.UsbDeviceIdTest));

				// MKY.Net.Test
				//suite.Add(typeof(MKY.IO.Net.<>));

				// MKY.Utilities.Test
				suite.Add(typeof(MKY.Utilities.Test.IO.XPathTest));
				suite.Add(typeof(MKY.Utilities.Test.Settings.DocumentSettingsHandlerTest));
				suite.Add(typeof(MKY.Utilities.Test.Types.XByteTest));
				suite.Add(typeof(MKY.Utilities.Test.Types.XInt32Test));
				suite.Add(typeof(MKY.Utilities.Test.Types.XUInt64Test));

				// MKY.Windows.Forms.Test
				// MKY.Windows.Forms.Test.FastListBoxTest is a manual test
				// Set MKY.Windows.Forms.Test as start up project and run it
				//suite.Add(typeof(MKY.Windows.Forms.Test.<>));

				// YAT.Controller.Test
				suite.Add(typeof(YAT.Controller.Test.ControllerTest));

				// YAT.Domain.Test
				suite.Add(typeof(YAT.Domain.Test.Parser.ParserTest));
				suite.Add(typeof(YAT.Domain.Test.TextTerminal.SubstitutionParserTest));

				// YAT.Model.Test
				suite.Add(typeof(YAT.Model.Test.ClearTest));
				suite.Add(typeof(YAT.Model.Test.FileHandlingTest));
				suite.Add(typeof(YAT.Model.Test.StressTest));
				suite.Add(typeof(YAT.Model.Test.TransmissionTest));

				// YAT.Settings.Test
				suite.Add(typeof(YAT.Settings.Test.FileVersionsTest));
				suite.Add(typeof(YAT.Settings.Test.XmlTest));

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
