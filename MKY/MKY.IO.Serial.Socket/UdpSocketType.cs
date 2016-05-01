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
	#region Enum UdpSocketType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum UdpSocketType
	{
		Unknown,
		Client,
		Server,
		PairSocket,
	}

#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum UdpSocketTypeEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class UdpSocketTypeEx : EnumEx
	{
		#region String Definitions

		private const string Unknown_string    = "Unknown";
		private const string Client_string     = "UDP/IP Client";
		private const string Server_string     = "UDP/IP Server";
		private const string PairSocket_string = "UDP/IP PairSocket";

		#endregion

		/// <summary>Default is <see cref="UdpSocketType.PairSocket"/>.</summary>
		public UdpSocketTypeEx()
			: this(UdpSocketType.PairSocket)
		{
		}

		/// <summary></summary>
		protected UdpSocketTypeEx(UdpSocketType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((UdpSocketType)UnderlyingEnum)
			{
				case UdpSocketType.Unknown:    return (Unknown_string);
				case UdpSocketType.Client:     return (Client_string);
				case UdpSocketType.Server:     return (Server_string);
				case UdpSocketType.PairSocket: return (PairSocket_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static UdpSocketTypeEx[] GetItems()
		{
			List<UdpSocketTypeEx> a = new List<UdpSocketTypeEx>(3); // Preset the required capactiy to improve memory management.
			a.Add(new UdpSocketTypeEx(UdpSocketType.Client));
			a.Add(new UdpSocketTypeEx(UdpSocketType.Server));
			a.Add(new UdpSocketTypeEx(UdpSocketType.PairSocket));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static UdpSocketTypeEx Parse(string s)
		{
			UdpSocketTypeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid socket host type string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out UdpSocketTypeEx result)
		{
			UdpSocketType enumResult;
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
		public static bool TryParse(string s, out UdpSocketType result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Client_string))
			{
				result = UdpSocketType.Client;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Server_string))
			{
				result = UdpSocketType.Server;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, PairSocket_string))
			{
				result = UdpSocketType.PairSocket;
				return (true);
			}
			else
			{
				result = new UdpSocketTypeEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator UdpSocketType(UdpSocketTypeEx type)
		{
			return ((UdpSocketType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator UdpSocketTypeEx(UdpSocketType type)
		{
			return (new UdpSocketTypeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(UdpSocketTypeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator UdpSocketTypeEx(int type)
		{
			return (new UdpSocketTypeEx((UdpSocketType)type));
		}

		/// <summary></summary>
		public static implicit operator string(UdpSocketTypeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator UdpSocketTypeEx(string type)
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
