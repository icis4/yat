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
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\Socket for better separation of the implementation files.
namespace MKY.IO.Serial
{
	#region Enum SocketHostType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SocketHostType
	{
		Unknown,
		TcpClient,
		TcpServer,
		TcpAutoSocket,
		Udp,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SocketHostTypeEx.
	/// </summary>
	public class SocketHostTypeEx : EnumEx
	{
		#region String Definitions

		private const string Unknown_string       = "Unknown";
		private const string TcpClient_string     = "TCP/IP Client";
		private const string TcpServer_string     = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Udp_string           = "UDP/IP Socket";

		#endregion

		/// <summary>Default is <see cref="SocketHostType.TcpAutoSocket"/>.</summary>
		public SocketHostTypeEx()
			: base(SocketHostType.TcpAutoSocket)
		{
		}

		/// <summary></summary>
		protected SocketHostTypeEx(SocketHostType type)
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
		public static SocketHostTypeEx[] GetItems()
		{
			List<SocketHostTypeEx> a = new List<SocketHostTypeEx>();
			a.Add(new SocketHostTypeEx(SocketHostType.TcpClient));
			a.Add(new SocketHostTypeEx(SocketHostType.TcpServer));
			a.Add(new SocketHostTypeEx(SocketHostType.TcpAutoSocket));
			a.Add(new SocketHostTypeEx(SocketHostType.Udp));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static SocketHostTypeEx Parse(string type)
		{
			SocketHostTypeEx result;

			if (TryParse(type, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("type", type, "Invalid type."));
		}

		/// <summary></summary>
		public static bool TryParse(string type, out SocketHostTypeEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(type, TcpClient_string))
			{
				result = new SocketHostTypeEx(SocketHostType.TcpClient);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, TcpServer_string))
			{
				result = new SocketHostTypeEx(SocketHostType.TcpServer);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, TcpAutoSocket_string))
			{
				result = new SocketHostTypeEx(SocketHostType.TcpAutoSocket);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(type, Udp_string))
			{
				result = new SocketHostTypeEx(SocketHostType.Udp);
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
		public static implicit operator SocketHostType(SocketHostTypeEx type)
		{
			return ((SocketHostType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SocketHostTypeEx(SocketHostType type)
		{
			return (new SocketHostTypeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(SocketHostTypeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SocketHostTypeEx(int type)
		{
			return (new SocketHostTypeEx((SocketHostType)type));
		}

		/// <summary></summary>
		public static implicit operator string(SocketHostTypeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator SocketHostTypeEx(string type)
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
