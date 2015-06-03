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
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2015 Matthias Kläy.
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
using System.Net;

using MKY.IO;
using MKY.Net;
using MKY.Test.Xml.Serialization;

using NUnit.Framework;

#endregion

namespace MKY.Test.Net
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	public static class IPHostTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string LocalhostString     =  "Localhost";
		private const string LocalhostStringNice = "<Localhost>";

		private const string IPv4LocalhostString = "IPv4 localhost";
		private const string IPv6LocalhostString = "IPv6 localhost";

		private static readonly IPAddress SomeIPv4Address = new IPAddress(new byte[] { 1, 2, 3, 4 });
		private const string SomeIPv4AddressString = "1.2.3.4";

		private static readonly IPAddress SomeIPv6Address = new IPAddress(new byte[] { 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6, 0, 7, 0, 8 });
		private const string SomeIPv6AddressString = "1:2:3:4:5:6:7:8";

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
				yield return (new TestCaseData(new IPHost(IPHostType.Localhost),		IPHostType.Localhost,		IPAddress.Loopback,		LocalhostStringNice)	.SetName("Localhost_HostType"));
				yield return (new TestCaseData(new IPHost(IPHostType.IPv4Localhost),	IPHostType.IPv4Localhost,	IPAddress.Loopback,		IPv4LocalhostString)	.SetName("IPv4Localhost_HostType"));
				yield return (new TestCaseData(new IPHost(IPHostType.IPv6Localhost),	IPHostType.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostType"));

				yield return (new TestCaseData(new IPHost(IPAddress.Loopback),			IPHostType.Localhost,		IPAddress.Loopback,		LocalhostStringNice)	.SetName("Localhost_HostAddress"));
				yield return (new TestCaseData(new IPHost(IPAddress.Loopback),			IPHostType.IPv4Localhost,	IPAddress.Loopback,		LocalhostStringNice)	.SetName("IPv4Localhost_HostAddress"));
				yield return (new TestCaseData(new IPHost(IPAddress.IPv6Loopback),		IPHostType.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostAddress"));

				yield return (new TestCaseData(new IPHost(SomeIPv4Address),				IPHostType.Other,			SomeIPv4Address,		SomeIPv4AddressString)	.SetName("SomeIPv4Address"));
				yield return (new TestCaseData(new IPHost(SomeIPv6Address),				IPHostType.Other,			SomeIPv6Address,		SomeIPv6AddressString)	.SetName("SomeIPv6Address"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class IPHostTest
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

		#region Tests > Equal()
		//------------------------------------------------------------------------------------------
		// Tests > Equal()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsHostType(IPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			if ((ipHostType == IPHostType.Localhost) && ((IPHostType)ipHost == IPHostType.IPv4Localhost))
				return; // All fine, 'Localhost' and 'IPv4Localhost' are the same.

			if ((ipHostType == IPHostType.IPv4Localhost) && ((IPHostType)ipHost == IPHostType.Localhost))
				return; // All fine, 'IPv4Localhost' and 'Localhost' are the same.

			Assert.AreEqual(ipHostType, (IPHostType)ipHost);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsAddress(IPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(ipAddress, (IPAddress)ipHost);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostAddressEqualsAddress(IPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(ipAddress, ipHost.IPAddress);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsHostString(IPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.AreEqual(hostString, (string)ipHost);
		}

		#endregion

		#region Tests > Serialization
		//------------------------------------------------------------------------------------------
		// Tests > Serialization
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestSerialization(IPHost ipHost, IPHostType ipHostType, IPAddress ipAddress, string hostString)
		{
			string filePath = Temp.MakeTempFilePath(GetType(), ".xml");
			IPHost ipHostDeserialized = null;

			// Serialize to file:
			XmlSerializerTest.TestSerializeToFile(filePath, typeof(IPHost), ipHost);

			// Serialization for this EnumEx cannot work. Therefore, only default case results in success.
			// Anyway, tests are done to ensure that serialization doesn't throw exceptions.
			if (ipHostType == IPHostType.Localhost)
			{
				// Deserialize from file using different methods and verify the result:

				ipHostDeserialized = (IPHost)XmlSerializerTest.TestDeserializeFromFile(filePath, typeof(IPHost));
				Assert.AreEqual(ipHost, ipHostDeserialized);
				Assert.AreEqual(ipHostType, (IPHostType)ipHostDeserialized);
				Assert.AreEqual(ipAddress, (IPAddress)ipHostDeserialized);
				Assert.AreEqual(ipAddress, ipHostDeserialized.IPAddress);
				Assert.AreEqual(hostString, (string)ipHostDeserialized);

				ipHostDeserialized = (IPHost)XmlSerializerTest.TestTolerantDeserializeFromFile(filePath, typeof(IPHost));
				Assert.AreEqual(ipHost, ipHostDeserialized);
				Assert.AreEqual(ipHostType, (IPHostType)ipHostDeserialized);
				Assert.AreEqual(ipAddress, (IPAddress)ipHostDeserialized);
				Assert.AreEqual(ipAddress, ipHostDeserialized.IPAddress);
				Assert.AreEqual(hostString, (string)ipHostDeserialized);

				ipHostDeserialized = (IPHost)XmlSerializerTest.TestAlternateTolerantDeserializeFromFile(filePath, typeof(IPHost));
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
