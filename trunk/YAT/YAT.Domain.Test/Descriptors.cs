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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;

namespace YAT.Domain.Test
{
	/// <summary></summary>
	public class Descriptor
	{
		/// <summary></summary>
		public string Name { get; }

		/// <summary></summary>
		public string[] Categories { get; }

		/// <summary></summary>
		public Descriptor(string name, string[] categories)
		{
			Name = name;
			Categories = categories;
		}
	}

	/// <summary></summary>
	public class SerialPortLoopbackPairDescriptor : Descriptor
	{
		/// <summary></summary>
		public string PortA { get; set; }

		/// <summary></summary>
		public string PortB { get; set; }

		/// <summary></summary>
		public SerialPortLoopbackPairDescriptor(string portA, string portB, string name, string[] categories)
			: base(name, categories)
		{
			PortA = portA;
			PortB = portB;
		}
	}

	/// <summary></summary>
	public class SerialPortLoopbackSelfDescriptor : Descriptor
	{
		/// <summary></summary>
		public string Port { get; set; }

		/// <summary></summary>
		public SerialPortLoopbackSelfDescriptor(string port, string name, string[] categories)
			: base(name, categories)
		{
			Port = port;
		}
	}

	/// <summary></summary>
	public static class Descriptors
	{
		/// <summary>
		/// Returns test case descriptors for serial COM port loopback pairs.
		/// </summary>
		public static IEnumerable<SerialPortLoopbackPairDescriptor> SerialPortLoopbackPairs
		{
			get
			{
				foreach (MKY.IO.Ports.Test.SerialPortPairConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairs)
				{
					string name = "SerialPortLoopbackPair_" + ce.PortA + "_" + ce.PortB;
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
					yield return (new SerialPortLoopbackPairDescriptor(ce.PortA, ce.PortB, name, cats));
				}
			}
		}

		/// <summary>
		/// Returns test case descriptors for serial COM port loopback selfs.
		/// </summary>
		public static IEnumerable<SerialPortLoopbackSelfDescriptor> SerialPortLoopbackSelfs
		{
			get
			{
				foreach (MKY.IO.Ports.Test.SerialPortConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfs)
				{
					string name = "SerialPortLoopbackSelf_" + ce.Port;
					string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
					yield return (new SerialPortLoopbackSelfDescriptor(ce.Port, name, cats));
				}
			}
		}

		/// <summary>
		/// Returns test case descriptors for TCP/IP and UDP/IP Client/Server as well as AutoSocket.
		/// </summary>
		/// <remarks>
		/// TCP/IP combinations Server/AutoSocket and AutoSocket/Client are skipped as they don't really offer additional test coverage.
		/// UPD/IP PairSocket is also skipped as that would require additional settings with different ports, and they are tested further below anyway.
		/// </remarks>
		public static IEnumerable<Descriptor> IPLoopbackPairs
		{
			get
			{
				string name;
				string[] cats;

				// TCP/IP Client/Server

				name = "TcpClientServer_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpClientServer_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpClientServer_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpClientServer_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				// TCP/IP AutoSocket

				name = "TcpAutoSocket_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpAutoSocket_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpAutoSocket_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "TcpAutoSocket_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				// UDP/IP Client/Server

				name = "UdpClientServer_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpClientServer_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpClientServer_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpClientServer_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));
			}
		}

		/// <summary>
		/// Returns test case descriptors for UDP/IP PairSocket.
		/// </summary>
		public static IEnumerable<Descriptor> IPLoopbackSelfs
		{
			get
			{
				string name;
				string[] cats;

				// UDP/IP PairSocket

				name = "UdpPairSocket_IPv4Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpPairSocket_IPv6Loopback";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpPairSocket_IPv4SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));

				name = "UdpPairSocket_IPv6SpecificInterface";
				cats = new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable };
				yield return (new Descriptor(name, cats));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
