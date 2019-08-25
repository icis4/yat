using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Types;

namespace HSR.YAT.Domain
{
	#region Enum IOType

	/// <summary></summary>
	public enum IOType
	{
		/// <summary></summary>
		Unknown,
		/// <summary></summary>
		SerialPort,
		/// <summary></summary>
		TcpClient,
		/// <summary></summary>
		TcpServer,
		/// <summary></summary>
		TcpAutoSocket,
		/// <summary></summary>
		Udp
	}

	#endregion

	/// <summary>
	/// Extended enum XIOType.
	/// </summary>
	[Serializable]
	public class XIOType : XEnum
	{
		#region String Definitions

		private const string Unknown_string = "Unknown";
		private const string SerialPort_string = "Serial port (COM)";
		private const string TcpClient_string = "TCP/IP Client";
		private const string TcpServer_string = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Udp_string = "UDP/IP Socket";

		#endregion

		/// <summary>Default is <see cref="IOType.SerialPort"/></summary>
		public XIOType()
			: base(IOType.SerialPort)
		{
		}

		/// <summary></summary>
		protected XIOType(IOType type)
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
				default:                   return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XIOType[] GetItems()
		{
			List<XIOType> a = new List<XIOType>();
			a.Add(new XIOType(IOType.SerialPort));
			a.Add(new XIOType(IOType.TcpClient));
			a.Add(new XIOType(IOType.TcpServer));
			a.Add(new XIOType(IOType.TcpAutoSocket));
			a.Add(new XIOType(IOType.Udp));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XIOType Parse(string type)
		{
			if      (string.Compare(type, SerialPort_string, true) == 0)
			{
				return (new XIOType(IOType.SerialPort));
			}
			else if (string.Compare(type, TcpClient_string, true) == 0)
			{
				return (new XIOType(IOType.TcpClient));
			}
			else if (string.Compare(type, TcpServer_string, true) == 0)
			{
				return (new XIOType(IOType.TcpServer));
			}
			else if (string.Compare(type, TcpAutoSocket_string, true) == 0)
			{
				return (new XIOType(IOType.TcpAutoSocket));
			}
			else if (string.Compare(type, Udp_string, true) == 0)
			{
				return (new XIOType(IOType.Udp));
			}
			else
			{
				return (new XIOType(IOType.Unknown));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IOType(XIOType type)
		{
			return ((IOType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XIOType(IOType type)
		{
			return (new XIOType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XIOType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XIOType(int type)
		{
			return (new XIOType((IOType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XIOType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XIOType(string type)
		{
			return (Parse(type));
		}

		/// <summary></summary>
		public static implicit operator Net.Sockets.HostType(XIOType type)
		{
			switch ((IOType)type)
			{
				case IOType.TcpClient:     return (Net.Sockets.HostType.TcpClient);
				case IOType.TcpServer:     return (Net.Sockets.HostType.TcpServer);
				case IOType.TcpAutoSocket: return (Net.Sockets.HostType.TcpAutoSocket);
				case IOType.Udp:           return (Net.Sockets.HostType.Udp);
				default:                   return (Net.Sockets.HostType.Unknown);
			}
		}

		/// <summary></summary>
		public static implicit operator XIOType(Net.Sockets.HostType type)
		{
			switch (type)
			{
				case Net.Sockets.HostType.TcpClient:     return (IOType.TcpClient);
				case Net.Sockets.HostType.TcpServer:     return (IOType.TcpServer);
				case Net.Sockets.HostType.TcpAutoSocket: return (IOType.TcpAutoSocket);
				case Net.Sockets.HostType.Udp:           return (IOType.Udp);
				default:                                 return (IOType.Unknown);
			}
		}

		#endregion
	}
}