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
	public class SerialPortSettingsTest
	{
		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Serialization()
		//------------------------------------------------------------------------------------------
		// Tests > Serialization()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestSerialization()
		{
			// \fixme
			// Improve SerialPortSettings and XBaudRate such that they can handle user defined baud rates.
			// Then, add a test case that verifies that user defined baud rates are properly handled.
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
