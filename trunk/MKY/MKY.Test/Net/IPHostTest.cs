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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	public static class IPHostTestData
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string LocalhostString     = "[localhost]";
		private const string IPv4LocalhostString = "IPv4 localhost (127.0.0.1)";
		private const string BroadcastString     = "[broadcast]";
		private const string IPv4BroadcastString = "IPv4 broadcast (255.255.255.255)";
		private const string IPv6LocalhostString = "IPv6 localhost (::1)";

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
				yield return (new TestCaseData(new IPHostEx(IPHost.Localhost),			IPHost.Localhost,		IPAddress.Loopback,		LocalhostString)		.SetName("Localhost_HostType"));
				yield return (new TestCaseData(new IPHostEx(IPHost.IPv4Localhost),		IPHost.IPv4Localhost,	IPAddress.Loopback,		IPv4LocalhostString)	.SetName("IPv4Localhost_HostType"));
				yield return (new TestCaseData(new IPHostEx(IPHost.Broadcast),			IPHost.Broadcast,		IPAddress.Broadcast,	BroadcastString)		.SetName("Broadcast_HostType"));
				yield return (new TestCaseData(new IPHostEx(IPHost.IPv4Broadcast),		IPHost.IPv4Broadcast,	IPAddress.Broadcast,	IPv4BroadcastString)	.SetName("IPv4Broadcast_HostType"));
				yield return (new TestCaseData(new IPHostEx(IPHost.IPv6Localhost),		IPHost.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostType"));

				yield return (new TestCaseData(new IPHostEx(IPAddress.Loopback),		IPHost.Localhost,		IPAddress.Loopback,		LocalhostString)		.SetName("Localhost_HostAddress"));
				yield return (new TestCaseData(new IPHostEx(IPAddress.Loopback),		IPHost.IPv4Localhost,	IPAddress.Loopback,		LocalhostString)		.SetName("IPv4Localhost_HostAddress"));
				yield return (new TestCaseData(new IPHostEx(IPAddress.Broadcast),		IPHost.Broadcast,		IPAddress.Broadcast,	BroadcastString)		.SetName("Broadcast_HostAddress"));
				yield return (new TestCaseData(new IPHostEx(IPAddress.Broadcast),		IPHost.IPv4Broadcast,	IPAddress.Broadcast,	BroadcastString)		.SetName("IPv4Broadcast_HostAddress"));
				yield return (new TestCaseData(new IPHostEx(IPAddress.IPv6Loopback),	IPHost.IPv6Localhost,	IPAddress.IPv6Loopback,	IPv6LocalhostString)	.SetName("IPv6Localhost_HostAddress"));

				yield return (new TestCaseData(new IPHostEx(SomeIPv4Address),			IPHost.Explicit,		SomeIPv4Address,		SomeIPv4AddressString)	.SetName("SomeIPv4Address"));
				yield return (new TestCaseData(new IPHostEx(SomeIPv6Address),			IPHost.Explicit,		SomeIPv6Address,		SomeIPv6AddressString)	.SetName("SomeIPv6Address"));
			}
		}

		#endregion
	}

	/// <summary></summary>
	[TestFixture]
	public class IPHostTest
	{
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
		public virtual void TestHostEqualsHostType(IPHostEx ipHost, IPHost ipHostType, IPAddress ipAddress, string hostString)
		{
			if ((ipHostType == IPHost.Localhost) && (ipHost == IPHost.IPv4Localhost))
				return; // All fine, 'Localhost' and 'IPv4Localhost' are the same.

			if ((ipHostType == IPHost.IPv4Localhost) && (ipHost == IPHost.Localhost))
				return; // All fine, 'IPv4Localhost' and 'Localhost' are the same.

			if ((ipHostType == IPHost.Broadcast) && (ipHost == IPHost.IPv4Broadcast))
				return; // All fine, 'Broadcast' and 'IPv4Broadcast' are the same.

			if ((ipHostType == IPHost.IPv4Broadcast) && (ipHost == IPHost.Broadcast))
				return; // All fine, 'IPv4Broadcast' and 'Broadcast' are the same.

			Assert.That((IPHost)ipHost, Is.EqualTo(ipHostType));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsAddress(IPHostEx ipHost, IPHost ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.That((IPAddress)ipHost, Is.EqualTo(ipAddress));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostAddressEqualsAddress(IPHostEx ipHost, IPHost ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.That(ipHost.Address, Is.EqualTo(ipAddress));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and struct parameters.")]
		[Test, TestCaseSource(typeof(IPHostTestData), "TestCases")]
		public virtual void TestHostEqualsHostString(IPHostEx ipHost, IPHost ipHostType, IPAddress ipAddress, string hostString)
		{
			Assert.That((string)ipHost, Is.EqualTo(hostString));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
