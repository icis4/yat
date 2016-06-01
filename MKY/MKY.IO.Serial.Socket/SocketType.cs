﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Serial.Socket
{
	#region Enum SocketType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SocketType
	{
		Unknown,
		TcpClient,
		TcpServer,
		TcpAutoSocket,
		UdpClient,
		UdpServer,
		UdpPairSocket,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SocketTypeEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class SocketTypeEx : EnumEx
	{
		#region String Definitions

		private const string Unknown_string       = "Unknown";

		private const string TcpClient_string     = "TCP/IP Client";
		private const string TcpServer_string     = "TCP/IP Server";
		private const string TcpAutoSocket_string = "TCP/IP AutoSocket";
		private const string Tcp_stringDefault    = "TCP";

		private const string UdpClient_string     = "UDP/IP Client";
		private const string UdpServer_string     = "UDP/IP Server";
		private const string UdpPairSocket_string = "UDP/IP PairSocket";
		private const string Udp_stringDefault    = "UDP";

		#endregion

		/// <summary>Default is <see cref="SocketType.TcpAutoSocket"/>.</summary>
		public const SocketType Default = SocketType.TcpAutoSocket;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SocketTypeEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SocketTypeEx(SocketType type)
			: base(type)
		{
		}

		#region Properties

		/// <summary></summary>
		public virtual bool IsTcp
		{
			get
			{
				switch ((SocketType)UnderlyingEnum)
				{
					case SocketType.TcpClient:     return (true);
					case SocketType.TcpServer:     return (true);
					case SocketType.TcpAutoSocket: return (true);
					case SocketType.UdpClient:     return (false);
					case SocketType.UdpServer:     return (false);
					case SocketType.UdpPairSocket:     return (false);
					default:                       return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsUdp
		{
			get
			{
				switch ((SocketType)UnderlyingEnum)
				{
					case SocketType.TcpClient:     return (false);
					case SocketType.TcpServer:     return (false);
					case SocketType.TcpAutoSocket: return (false);
					case SocketType.UdpClient:     return (true);
					case SocketType.UdpServer:     return (true);
					case SocketType.UdpPairSocket:     return (true);
					default:                       return (false);
				}
			}
		}

		#endregion

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((SocketType)UnderlyingEnum)
			{
				case SocketType.Unknown:       return (Unknown_string);
				case SocketType.TcpClient:     return (TcpClient_string);
				case SocketType.TcpServer:     return (TcpServer_string);
				case SocketType.TcpAutoSocket: return (TcpAutoSocket_string);
				case SocketType.UdpClient:     return (UdpClient_string);
				case SocketType.UdpServer:     return (UdpServer_string);
				case SocketType.UdpPairSocket: return (UdpPairSocket_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static SocketTypeEx[] GetItems()
		{
			List<SocketTypeEx> a = new List<SocketTypeEx>(6); // Preset the required capactiy to improve memory management.
			a.Add(new SocketTypeEx(SocketType.TcpClient));
			a.Add(new SocketTypeEx(SocketType.TcpServer));
			a.Add(new SocketTypeEx(SocketType.TcpAutoSocket));
			a.Add(new SocketTypeEx(SocketType.UdpClient));
			a.Add(new SocketTypeEx(SocketType.UdpServer));
			a.Add(new SocketTypeEx(SocketType.UdpPairSocket));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SocketTypeEx Parse(string s)
		{
			SocketTypeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid socket host type string! String must one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SocketTypeEx result)
		{
			SocketType enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SocketType result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new SocketTypeEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TcpClient_string))
			{
				result = SocketType.TcpClient;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TcpServer_string))
			{
				result = SocketType.TcpServer;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, TcpAutoSocket_string) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Tcp_stringDefault))
			{
				result = SocketType.TcpAutoSocket;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UdpClient_string))
			{
				result = SocketType.UdpClient;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UdpServer_string))
			{
				result = SocketType.UdpServer;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, UdpPairSocket_string) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Udp_stringDefault))
			{
				result = SocketType.UdpPairSocket;
				return (true);
			}
			else // Invalid string!
			{
				result = new SocketTypeEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SocketType(SocketTypeEx type)
		{
			return ((SocketType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SocketTypeEx(SocketType type)
		{
			return (new SocketTypeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(SocketTypeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SocketTypeEx(int type)
		{
			return (new SocketTypeEx((SocketType)type));
		}

		/// <summary></summary>
		public static implicit operator string(SocketTypeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator SocketTypeEx(string type)
		{
			return (Parse(type));
		}

		/// <summary></summary>
		public static implicit operator UdpSocketType(SocketTypeEx type)
		{
			switch ((SocketType)type)
			{
				case SocketType.UdpClient:     return (UdpSocketType.Client);
				case SocketType.UdpServer:     return (UdpSocketType.Server);
				case SocketType.UdpPairSocket: return (UdpSocketType.PairSocket);
				default:                       return (UdpSocketType.Unknown);
			}
		}

		/// <summary></summary>
		public static implicit operator SocketTypeEx(UdpSocketType type)
		{
			switch (type)
			{
				case UdpSocketType.Client:     return (SocketType.UdpClient);
				case UdpSocketType.Server:     return (SocketType.UdpServer);
				case UdpSocketType.PairSocket: return (SocketType.UdpPairSocket);
				default:                       return (SocketType.Unknown);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
