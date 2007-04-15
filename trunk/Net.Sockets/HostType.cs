using System;
using System.Collections.Generic;

using HSR.Utilities.Types;

namespace HSR.Net.Sockets
{
	#region Enum HostType

	/// <summary></summary>
	public enum HostType
	{
		/// <summary></summary>
		Unknown,
		/// <summary></summary>
		TcpClient,
		/// <summary></summary>
		TcpServer,
		/// <summary></summary>
		TcpAutoSocket,
		/// <summary></summary>
		Udp,
	}

	#endregion

	/// <summary>
	/// Extended enum XHostType.
	/// </summary>
	[Serializable]
	public class XHostType : XEnum
	{
		#region String Definitions

		private const string Unknown_string       = "Unknown";
		private const string TcpClient_string     = "TCP/IP Client";
		private const string TcpServer_string     = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Udp_string           = "UDP/IP Socket";

		#endregion

		/// <summary>Default is <see cref="HostType.TcpAutoSocket"/></summary>
		public XHostType()
			: base(HostType.TcpAutoSocket)
		{
		}

		/// <summary></summary>
		protected XHostType(HostType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((HostType)UnderlyingEnum)
			{
				case HostType.TcpClient:     return (TcpClient_string);
				case HostType.TcpServer:     return (TcpServer_string);
				case HostType.TcpAutoSocket: return (TcpAutoSocket_string);
				case HostType.Udp:           return (Udp_string);
				default:                     return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XHostType[] GetItems()
		{
			List<XHostType> a = new List<XHostType>();
			a.Add(new XHostType(HostType.TcpClient));
			a.Add(new XHostType(HostType.TcpServer));
			a.Add(new XHostType(HostType.TcpAutoSocket));
			a.Add(new XHostType(HostType.Udp));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XHostType Parse(string type)
		{
			if      (string.Compare(type, TcpClient_string, true) == 0)
			{
				return (new XHostType(HostType.TcpClient));
			}
			else if (string.Compare(type, TcpServer_string, true) == 0)
			{
				return (new XHostType(HostType.TcpServer));
			}
			else if (string.Compare(type, TcpAutoSocket_string, true) == 0)
			{
				return (new XHostType(HostType.TcpAutoSocket));
			}
			else if (string.Compare(type, Udp_string, true) == 0)
			{
				return (new XHostType(HostType.Udp));
			}
			else
			{
				return (new XHostType(HostType.Unknown));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator HostType(XHostType type)
		{
			return ((HostType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XHostType(HostType type)
		{
			return (new XHostType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XHostType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XHostType(int type)
		{
			return (new XHostType((HostType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XHostType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XHostType(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}
