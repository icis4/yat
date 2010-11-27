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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using MKY;
using MKY.IO.Serial;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum IOType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IOType
	{
		Unknown,
		SerialPort,
		TcpClient,
		TcpServer,
		TcpAutoSocket,
		Udp,
		UsbSerialHid,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IOTypeEx.
	/// </summary>
	public class IOTypeEx : MKY.EnumEx
	{
		#region String Definitions

		private const string Unknown_string = "Unknown";
		private const string SerialPort_string = "Serial Port (COM)";
		private const string TcpClient_string = "TCP/IP Client";
		private const string TcpServer_string = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Udp_string = "UDP/IP Socket";
		private const string UsbSerialHid_string = "USB Ser/HID";

		#endregion

		/// <summary>Default is <see cref="IOType.SerialPort"/>.</summary>
		public IOTypeEx()
			: base(IOType.SerialPort)
		{
		}

		/// <summary></summary>
		protected IOTypeEx(IOType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((IOType)UnderlyingEnum)
			{
				case IOType.SerialPort:    return (SerialPort_string);
				case IOType.TcpClient:     return (TcpClient_string);
				case IOType.TcpServer:     return (TcpServer_string);
				case IOType.TcpAutoSocket: return (TcpAutoSocket_string);
				case IOType.Udp:           return (Udp_string);
				case IOType.UsbSerialHid:        return (UsbSerialHid_string);
				default:                   return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IOTypeEx[] GetItems()
		{
			List<IOTypeEx> a = new List<IOTypeEx>();
			a.Add(new IOTypeEx(IOType.SerialPort));
			a.Add(new IOTypeEx(IOType.TcpClient));
			a.Add(new IOTypeEx(IOType.TcpServer));
			a.Add(new IOTypeEx(IOType.TcpAutoSocket));
			a.Add(new IOTypeEx(IOType.Udp));
			a.Add(new IOTypeEx(IOType.UsbSerialHid));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static IOTypeEx Parse(string type)
		{
			IOTypeEx result;

			if (TryParse(type, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("type", type, "Invalid type."));
		}

		/// <summary></summary>
		public static bool TryParse(string type, out IOTypeEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(type, SerialPort_string))
			{
				result = new IOTypeEx(IOType.SerialPort);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, TcpClient_string))
			{
				result = new IOTypeEx(IOType.TcpClient);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, TcpServer_string))
			{
				result = new IOTypeEx(IOType.TcpServer);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, TcpAutoSocket_string))
			{
				result = new IOTypeEx(IOType.TcpAutoSocket);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, Udp_string))
			{
				result = new IOTypeEx(IOType.Udp);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, UsbSerialHid_string))
			{
				result = new IOTypeEx(IOType.UsbSerialHid);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IOType(IOTypeEx type)
		{
			return ((IOType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IOTypeEx(IOType type)
		{
			return (new IOTypeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(IOTypeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator IOTypeEx(int type)
		{
			return (new IOTypeEx((IOType)type));
		}

		/// <summary></summary>
		public static implicit operator string(IOTypeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator IOTypeEx(string type)
		{
			return (Parse(type));
		}

		/// <summary></summary>
		public static implicit operator SocketHostType(IOTypeEx type)
		{
			switch ((IOType)type)
			{
				case IOType.TcpClient:     return (SocketHostType.TcpClient);
				case IOType.TcpServer:     return (SocketHostType.TcpServer);
				case IOType.TcpAutoSocket: return (SocketHostType.TcpAutoSocket);
				case IOType.Udp:           return (SocketHostType.Udp);
				default:                   return (SocketHostType.Unknown);
			}
		}

		/// <summary></summary>
		public static implicit operator IOTypeEx(SocketHostType type)
		{
			switch (type)
			{
				case SocketHostType.TcpClient:     return (IOType.TcpClient);
				case SocketHostType.TcpServer:     return (IOType.TcpServer);
				case SocketHostType.TcpAutoSocket: return (IOType.TcpAutoSocket);
				case SocketHostType.Udp:           return (IOType.Udp);
				default:                           return (IOType.Unknown);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
