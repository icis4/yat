using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NUnit.Core;

namespace YAT.Test
{
	public class TestAll
	{
		[Suite]
		public static TestSuite Suite
		{
			get
			{
				TestSuite suite = new TestSuite("TestAll");

				// Utilities.Test
				suite.Add(new MKY.Utilities.Test.IO.XPathTest());
				suite.Add(new MKY.Utilities.Test.Types.XByteTest());

				// YAT.Domain.Test
				//suite.Add(new AssertionTest());

				// YAT.Settings.Test
				//suite.Add(new AssertionTest());

				return (suite);
			}
		}
	}
}
