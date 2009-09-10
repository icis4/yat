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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using MKY.Utilities.Types;

// The MKY.IO.Serial namespace combines serial port and socket infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	#region Enum SocketHostType

	/// <summary></summary>
	public enum SocketHostType
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
	/// Extended enum XSocketHostType.
	/// </summary>
	public class XSocketHostType : XEnum
	{
		#region String Definitions

		private const string Unknown_string       = "Unknown";
		private const string TcpClient_string     = "TCP/IP Client";
		private const string TcpServer_string     = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Udp_string           = "UDP/IP Socket";

		#endregion

		/// <summary>Default is <see cref="SocketHostType.TcpAutoSocket"/></summary>
		public XSocketHostType()
			: base(SocketHostType.TcpAutoSocket)
		{
		}

		/// <summary></summary>
		protected XSocketHostType(SocketHostType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((SocketHostType)UnderlyingEnum)
			{
				case SocketHostType.TcpClient:     return (TcpClient_string);
				case SocketHostType.TcpServer:     return (TcpServer_string);
				case SocketHostType.TcpAutoSocket: return (TcpAutoSocket_string);
				case SocketHostType.Udp:           return (Udp_string);
				default:                     return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XSocketHostType[] GetItems()
		{
			List<XSocketHostType> a = new List<XSocketHostType>();
			a.Add(new XSocketHostType(SocketHostType.TcpClient));
			a.Add(new XSocketHostType(SocketHostType.TcpServer));
			a.Add(new XSocketHostType(SocketHostType.TcpAutoSocket));
			a.Add(new XSocketHostType(SocketHostType.Udp));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XSocketHostType Parse(string type)
		{
			XSocketHostType result;

			if (TryParse(type, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("type", type, "Invalid type."));
		}

		/// <summary></summary>
		public static bool TryParse(string type, out XSocketHostType result)
		{
			if (string.Compare(type, TcpClient_string, true) == 0)
			{
				result = new XSocketHostType(SocketHostType.TcpClient);
				return (true);
			}
			else if (string.Compare(type, TcpServer_string, true) == 0)
			{
				result = new XSocketHostType(SocketHostType.TcpServer);
				return (true);
			}
			else if (string.Compare(type, TcpAutoSocket_string, true) == 0)
			{
				result = new XSocketHostType(SocketHostType.TcpAutoSocket);
				return (true);
			}
			else if (string.Compare(type, Udp_string, true) == 0)
			{
				result = new XSocketHostType(SocketHostType.Udp);
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
		public static implicit operator SocketHostType(XSocketHostType type)
		{
			return ((SocketHostType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XSocketHostType(SocketHostType type)
		{
			return (new XSocketHostType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XSocketHostType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XSocketHostType(int type)
		{
			return (new XSocketHostType((SocketHostType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XSocketHostType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XSocketHostType(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
