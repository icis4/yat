//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Xml.Serialization;

using NUnit.Framework;

using MKY.System.Diagnostics;
using MKY.System.Net;

namespace MKY.System.Test.Net
{
	/// <summary></summary>
	public static class IPHostTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private static readonly string LocalhostString = "localhost";
		private static readonly string IPv4LocalhostString = "IPv4 localhost";
		private static readonly string IPv6LocalhostString = "IPv6 localhost";

		private static readonly IPAddress SomeIPv4Address = new IPAddress(new byte[] { 1, 2, 3, 4 });
		private static readonly string SomeIPv4AddressString = "1.2.3.4";

		private static readonly IPAddress SomeIPv6Address = new IPAddress(new byte[] { 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 });
		private static readonly string SomeIPv6AddressString = "1:2:3:4:5:6:7:8";

		#endregion

		#region Test Cases
		//==========================================================================================
		// Test Cases
		//==========================================================================================

		/// <summary></summary>
		public static IEnumerable TestCases
		{
			get
			{
				yield return (new TestCaseData(new XIPHost(IPHostType.Localhost),		IPHostType.Localhost,		IPAddress.Loopback,		LocalhostString)		.SetName("Localhost_HostType"));
				yield return (new TestCaseData(new XIPHost(IPHostType.IPv4Localhost),	IPHostType.IPv4Localhost,	IPAddress.Loopback,		IPv4LocalhostString)	.SetName("IPv4Localhost_HostType"));
				yield return (new TestCaseData(new XIPHost(IPHostType.IPv6Localhost),	IPHostType.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostType"));
				yield return (new TestCaseData(new XIPHost(SomeIPv4Address),			IPHostType.Other,			SomeIPv4Address,		SomeIPv4AddressString)	.SetName("SomeIPv4Address_HostType"));
				yield return (new TestCaseData(new XIPHost(SomeIPv6Address),			IPHostType.Other,			SomeIPv6Address,		SomeIPv6AddressString)	.SetName("SomeIPv6Address_HostType"));

				yield return (new TestCaseData(new XIPHost(IPAddress.Loopback),			IPHostType.Localhost,		IPAddress.Loopback,		LocalhostString)		.SetName("Localhost_HostAddress"));
				yield return (new TestCaseData(new XIPHost(IPAddress.Loopback),			IPHostType.Localhost,		IPAddress.Loopback,		LocalhostString)		.SetName("IPv4Localhost_HostAddress"));
				yield return (new TestCaseData(new XIPHost(IPAddress.IPv6Loopback),		IPHostType.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostAddress"));
				yield return (new TestCaseData(new XIPHost(SomeIPv4Address),			IPHostType.Other,			SomeIPv4Address,		SomeIPv4AddressString)	.SetName("SomeIPv4Address_HostAddress"));
				yield return (new TestCaseData(new XIPHost(SomeIPv6Address),			IPHostType.Other,			SomeIPv6Address,		SomeIPv6AddressString)	.SetName("SomeIPv6Address_HostAddress"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class IPHostTest
	{
		#region Tear Down
		//==========================================================================================
		// Tear Down
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TearDown", Justification = "Naming according to NUnit.")]
		[TearDown]
		public virtual void TearDown()
		{
			XPath.CleanTempPath(this);
		}

		#endregion

		#region Tests
		//==========================================================================================
		// Tests
		//==========================================================================================

		#region Tests > Equal()
		//------------------------------------------------------------------------------------------
		// Tests > Equal()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsHostType(XIPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(ipHostType, (IPHostType)ipHost);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsAddress(XIPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(ipAddress, (IPAddress)ipHost);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostAddressEqualsAddress(XIPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(ipAddress, ipHost.IPAddress);
		}

		/// <summary></summary>
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsHostString(XIPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(hostString, (string)ipHost);
		}

		#endregion

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestSerialization(XIPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			string filePath = XPath.MakeTempFilePath(this, ".xml");
			XIPHost ipHostDeserialized = null;

			// Serialize to file.
			try
			{
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(XIPHost));
					serializer.Serialize(sw, ipHost);
				}
			}
			catch (Exception ex)
			{
				XConsole.WriteException(typeof(IPHostTest), ex);

				// Attention: The following call throws an exception, code below it won't be executed.
				Assert.Fail("XML serialize error: " + ex.Message);
			}

			// Serialization for this XEnum cannot work. Therefore, only default case results in success.
			// Anyway, tests are done to ensure that serialization doesn't throw exceptions.
			if (ipHostType == IPHostType.Localhost)
			{
				// Deserialize from file.
				try
				{
					using (StreamReader sr = new StreamReader(filePath))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(XIPHost));
						ipHostDeserialized = (XIPHost)serializer.Deserialize(sr);
					}
				}
				catch (Exception ex)
				{
					XConsole.WriteException(typeof(IPHostTest), ex);

					// Attention: The following call throws an exception, code below it won't be executed.
					Assert.Fail("XML deserialize error: " + ex.Message);
				}

				// Verify deserialized object.
				Assert.AreEqual(ipHost, ipHostDeserialized);
				Assert.AreEqual(ipHostType, (IPHostType)ipHostDeserialized);
				Assert.AreEqual(ipAddress, (IPAddress)ipHostDeserialized);
				Assert.AreEqual(ipAddress, ipHostDeserialized.IPAddress);
				Assert.AreEqual(hostString, (string)ipHostDeserialized);
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
