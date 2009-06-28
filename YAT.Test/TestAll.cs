//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NUnit.Core;

namespace YAT.Test
{
	public class TestAll
	{
		#region Suite
		//==========================================================================================
		// Suite
		//==========================================================================================

		[Suite]
		public static TestSuite Suite
		{
			get
			{
				TestSuite suite = new TestSuite("TestAll");

				// MKY.Utilities.Test
				suite.Add(new MKY.Utilities.Test.IO.XPathTest());
				suite.Add(new MKY.Utilities.Test.Settings.DocumentSettingsHandlerTest());
				suite.Add(new MKY.Utilities.Test.Types.XByteTest());
				suite.Add(new MKY.Utilities.Test.Types.XInt32Test());
				suite.Add(new MKY.Utilities.Test.Types.XUInt64Test());

				// YAT.Domain.Test
				suite.Add(new YAT.Domain.Test.Parser.ParserTest());
				suite.Add(new YAT.Domain.Test.TextTerminal.SubstitutionParserTest());

				// YAT.Model.Test
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
