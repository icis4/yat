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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

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
		UdpClient,
		UdpServer,
		UdpPairSocket,
		UsbSerialHid
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IOTypeEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class IOTypeEx : EnumEx
	{
		#region String Definitions

		private const string Unknown_string            = "Unknown";

		private const string SerialPort_string         = "Serial COM Port";
		private const string SerialPort_stringShort    = "COM";

		private const string TcpClient_string          = "TCP/IP Client";
		private const string TcpClient_stringShort     = "TCPClient";

		private const string TcpServer_string          = "TCP/IP Server";
		private const string TcpServer_stringShort     = "TCPServer";

		private const string TcpAutoSocket_string      = "TCP/IP AutoSocket";
		private const string TcpAutoSocket_stringShort = "TCPAutoSocket";
		private const string Tcp_stringDefault         = "TCP";

		private const string UdpClient_string          = "UDP/IP Client";
		private const string UdpClient_stringShort     = "UDPClient";

		private const string UdpServer_string          = "UDP/IP Server";
		private const string UdpServer_stringShort     = "UDPServer";

		private const string UdpPairSocket_string      = "UDP/IP PairSocket";
		private const string UdpPairSocket_stringShort = "UDPPairSocket";
		private const string Udp_stringDefault         = "UDP";

		private const string UsbSerialHid_string       = "USB Ser/HID";
		private const string UsbSerialHid_stringShort  = "USBSerHID";
		private const string Usb_stringDefault         = "USB";

		#endregion

		/// <summary>Default is <see cref="IOType.SerialPort"/>.</summary>
		public const IOType Default = IOType.SerialPort;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public IOTypeEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public IOTypeEx(IOType type)
			: base(type)
		{
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual bool IsSerialPort
		{
			get { return ((IOType)UnderlyingEnum == IOType.SerialPort); }
		}

		/// <summary></summary>
		public virtual bool IsSocket
		{
			get { return (IsTcpSocket || IsUdpSocket); }
		}

		/// <summary></summary>
		public virtual bool IsTcpSocket
		{
			get
			{
				switch ((IOType)UnderlyingEnum)
				{
					case IOType.TcpClient:     return (true);
					case IOType.TcpServer:     return (true);
					case IOType.TcpAutoSocket: return (true);
					default:                   return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsUdpSocket
		{
			get
			{
				switch ((IOType)UnderlyingEnum)
				{
					case IOType.UdpClient:     return (true);
					case IOType.UdpServer:     return (true);
					case IOType.UdpPairSocket: return (true);
					default:                   return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsUsbSerialHid
		{
			get { return ((IOType)UnderlyingEnum == IOType.UsbSerialHid); }
		}

		#endregion

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((IOType)UnderlyingEnum)
			{
				case IOType.Unknown:       return (Unknown_string);
				case IOType.SerialPort:    return (SerialPort_string);
				case IOType.TcpClient:     return (TcpClient_string);
				case IOType.TcpServer:     return (TcpServer_string);
				case IOType.TcpAutoSocket: return (TcpAutoSocket_string);
				case IOType.UdpClient:     return (UdpClient_string);
				case IOType.UdpServer:     return (UdpServer_string);
				case IOType.UdpPairSocket: return (UdpPairSocket_string);
				case IOType.UsbSerialHid:  return (UsbSerialHid_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static IOTypeEx[] GetItems()
		{
			var a = new List<IOTypeEx>(8); // Preset the required capacity to improve memory management.

			a.Add(new IOTypeEx(IOType.SerialPort));
			a.Add(new IOTypeEx(IOType.TcpClient));
			a.Add(new IOTypeEx(IOType.TcpServer));
			a.Add(new IOTypeEx(IOType.TcpAutoSocket));
			a.Add(new IOTypeEx(IOType.UdpClient));
			a.Add(new IOTypeEx(IOType.UdpServer));
			a.Add(new IOTypeEx(IOType.UdpPairSocket));

			if (EnvironmentEx.IsStandardWindows)
				a.Add(new IOTypeEx(IOType.UsbSerialHid)); // Requires Windows "HID.dll".

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static IOTypeEx Parse(string s)
		{
			IOTypeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid I/O type string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out IOTypeEx result)
		{
			IOType enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new IOTypeEx(enumResult);
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
		public static bool TryParse(string s, out IOType result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, SerialPort_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SerialPort_stringShort))
			{
				result = IOType.SerialPort;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TcpClient_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, TcpClient_stringShort))
			{
				result = IOType.TcpClient;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TcpServer_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, TcpServer_stringShort))
			{
				result = IOType.TcpServer;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, TcpAutoSocket_string) ||
			         StringEx.EqualsOrdinalIgnoreCase    (s, TcpAutoSocket_stringShort) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Tcp_stringDefault))
			{
				result = IOType.TcpAutoSocket;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UdpClient_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UdpClient_stringShort))
			{
				result = IOType.UdpClient;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UdpServer_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UdpServer_stringShort))
			{
				result = IOType.UdpServer;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, UdpPairSocket_string) ||
			         StringEx.EqualsOrdinalIgnoreCase    (s, UdpPairSocket_stringShort) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Udp_stringDefault))
			{
				result = IOType.UdpPairSocket;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, UsbSerialHid_string) ||
			         StringEx.EqualsOrdinalIgnoreCase    (s, UsbSerialHid_stringShort) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Usb_stringDefault))
			{
				result = IOType.UsbSerialHid;
				return (true);
			}
			else // Invalid string!
			{
				result = new IOTypeEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
		public static implicit operator MKY.IO.Serial.Socket.SocketType(IOTypeEx type)
		{
			switch ((IOType)type)
			{
				case IOType.TcpClient:     return (MKY.IO.Serial.Socket.SocketType.TcpClient);
				case IOType.TcpServer:     return (MKY.IO.Serial.Socket.SocketType.TcpServer);
				case IOType.TcpAutoSocket: return (MKY.IO.Serial.Socket.SocketType.TcpAutoSocket);
				case IOType.UdpClient:     return (MKY.IO.Serial.Socket.SocketType.UdpClient);
				case IOType.UdpServer:     return (MKY.IO.Serial.Socket.SocketType.UdpServer);
				case IOType.UdpPairSocket: return (MKY.IO.Serial.Socket.SocketType.UdpPairSocket);
				default:                   return (MKY.IO.Serial.Socket.SocketType.Unknown);
			}
		}

		/// <summary></summary>
		public static implicit operator IOTypeEx(MKY.IO.Serial.Socket.SocketType type)
		{
			switch (type)
			{
				case MKY.IO.Serial.Socket.SocketType.TcpClient:     return (IOType.TcpClient);
				case MKY.IO.Serial.Socket.SocketType.TcpServer:     return (IOType.TcpServer);
				case MKY.IO.Serial.Socket.SocketType.TcpAutoSocket: return (IOType.TcpAutoSocket);
				case MKY.IO.Serial.Socket.SocketType.UdpClient:     return (IOType.UdpClient);
				case MKY.IO.Serial.Socket.SocketType.UdpServer:     return (IOType.UdpServer);
				case MKY.IO.Serial.Socket.SocketType.UdpPairSocket: return (IOType.UdpPairSocket);
				default:                                            return (IOType.Unknown);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
