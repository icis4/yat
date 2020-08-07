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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using NUnitEx;

using MKY.IO.Serial.Socket;
using MKY.Net;

#endregion

namespace YAT.Domain.Test
{
	#region SerialPort
	//----------------------------------------------------------------------------------------------
	// SerialPort
	//----------------------------------------------------------------------------------------------

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class SerialPortDescriptor : TestCaseDescriptor
	{
		/// <summary></summary>
		public string Port { get; }

		/// <summary></summary>
		public SerialPortDescriptor(string port, string name, string[] categories)
			: base(name, categories)
		{
			Port = port;
		}
	}

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class SerialPortPairDescriptor : TestCaseDescriptor
	{
		/// <summary></summary>
		public string PortA { get; }

		/// <summary></summary>
		public string PortB { get; }

		/// <summary></summary>
		public SerialPortPairDescriptor(string portA, string portB, string name, string[] categories)
			: base(name, categories)
		{
			PortA = portA;
			PortB = portB;
		}
	}

	#endregion

	#region Socket
	//----------------------------------------------------------------------------------------------
	// Socket
	//----------------------------------------------------------------------------------------------

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public abstract class IPSocketDescriptorBase : TestCaseDescriptor
	{
		/// <summary></summary>
		public IPNetworkInterfaceEx LocalInterface { get; }

		/// <summary></summary>
		public IPSocketDescriptorBase(IPNetworkInterfaceEx localInterface, string name, string[] categories)
			: base(name, categories)
		{
			LocalInterface = localInterface;
		}
	}

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class IPSocketTypeDescriptor : IPSocketDescriptorBase
	{
		/// <summary></summary>
		public SocketType SocketType { get; }

		/// <summary></summary>
		public IPSocketTypeDescriptor(SocketType socketType, IPNetworkInterfaceEx localInterface, string name, string[] categories)
			: base(localInterface, name, categories)
		{
			SocketType = socketType;
		}
	}

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class IPSocketTypePairDescriptor : IPSocketDescriptorBase
	{
		/// <summary></summary>
		public SocketType SocketTypeA { get; }

		/// <summary></summary>
		public SocketType SocketTypeB { get; }

		/// <summary></summary>
		public IPSocketTypePairDescriptor(SocketType socketTypeA, SocketType socketTypeB, IPNetworkInterfaceEx localInterface, string name, string[] categories)
			: base(localInterface, name, categories)
		{
			SocketTypeA = socketTypeA;
			SocketTypeB = socketTypeB;
		}
	}

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class IPSocketDescriptor : IPSocketTypeDescriptor
	{
		/// <summary></summary>
		public int Port { get; }

		/// <summary></summary>
		public IPSocketDescriptor(SocketType socketType, IPNetworkInterfaceEx localInterface, int port, string name, string[] categories)
			: base(socketType, localInterface, name, categories)
		{
			Port = port;
		}
	}

	#endregion

	#region USB Ser/HID
	//----------------------------------------------------------------------------------------------
	// USB Ser/HID
	//----------------------------------------------------------------------------------------------

	/// <remarks>Just named "Descriptor" rather than "TestCaseDescriptor" for compactness.</remarks>
	public class UsbSerialHidDescriptor : TestCaseDescriptor
	{
		/// <summary></summary>
		public string DeviceInfo { get; }

		/// <summary></summary>
		public UsbSerialHidDescriptor(string deviceInfo, string name, string[] categories)
			: base(name, categories)
		{
			DeviceInfo = deviceInfo;
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
