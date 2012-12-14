//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Extended enum HandshakeEx.
	/// </summary>
	/// <remarks>
	/// I think flow control would be the better term, no clue why .NET uses handshake.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HandshakeEx : EnumEx
	{
		#region String Definitions

		private const string             None_string = "None";
		private static readonly string[] None_stringAlternatives = new string[] { "N" };

		private const string             RtsCts_string = "Hardware (RTS/CTS)";
		private const string             RtsCts_stringShort = "RTS/CTS";
		private static readonly string[] RtsCts_stringAlternatives = new string[] { "Hardware", "Hard", "HW", "H" };

		private const string             XOnXOff_string = "Software (XOn/XOff)";
		private const string             XOnXOff_stringShort = "XOn/XOff";
		private static readonly string[] XOnXOff_stringAlternatives = new string[] { "Software", "Soft", "SW", "S" };

		private const string             RtsCtsXOnXOff_string = "Combined (RTS/CTS + XOn/XOff)";
		private const string             RtsCtsXOnXOff_stringShort = "RTS/CTS + XOn/XOff";
		private static readonly string[] RtsCtsXOnXOff_stringAlternatives = new string[] { "Combined", "Combi", "C" };

		#endregion

		/// <summary>Default is <see cref="Handshake.None"/>.</summary>
		public HandshakeEx()
			: base(Handshake.None)
		{
		}

		/// <summary></summary>
		protected HandshakeEx(Handshake handshake)
			: base(handshake)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_string);
				case Handshake.RequestToSend:        return (RtsCts_string);
				case Handshake.XOnXOff:              return (XOnXOff_string);
				case Handshake.RequestToSendXOnXOff: return (RtsCtsXOnXOff_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_string);
				case Handshake.RequestToSend:        return (RtsCts_stringShort);
				case Handshake.XOnXOff:              return (XOnXOff_stringShort);
				case Handshake.RequestToSendXOnXOff: return (RtsCtsXOnXOff_stringShort);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static HandshakeEx[] GetItems()
		{
			List<HandshakeEx> a = new List<HandshakeEx>();
			a.Add(new HandshakeEx(Handshake.None));
			a.Add(new HandshakeEx(Handshake.RequestToSend));
			a.Add(new HandshakeEx(Handshake.XOnXOff));
			a.Add(new HandshakeEx(Handshake.RequestToSendXOnXOff));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static HandshakeEx Parse(string handshake)
		{
			HandshakeEx result;

			if (TryParse(handshake, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("handshake", handshake, "Invalid handshake."));
		}

		/// <summary></summary>
		public static bool TryParse(string handshake, out HandshakeEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase   (handshake, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(handshake, None_stringAlternatives))
			{
				result = new HandshakeEx(Handshake.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (handshake, RtsCts_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (handshake, RtsCts_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(handshake, RtsCts_stringAlternatives))
			{
				result = new HandshakeEx(Handshake.RequestToSend);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (handshake, XOnXOff_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (handshake, XOnXOff_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(handshake, XOnXOff_stringAlternatives))
			{
				result = new HandshakeEx(Handshake.XOnXOff);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (handshake, RtsCtsXOnXOff_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (handshake, RtsCtsXOnXOff_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(handshake, RtsCtsXOnXOff_stringAlternatives))
			{
				result = new HandshakeEx(Handshake.RequestToSendXOnXOff);
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
		public static implicit operator Handshake(HandshakeEx handshake)
		{
			return ((Handshake)handshake.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator HandshakeEx(Handshake handshake)
		{
			return (new HandshakeEx(handshake));
		}

		/// <summary></summary>
		public static implicit operator int(HandshakeEx handshake)
		{
			return (handshake.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator HandshakeEx(int handshake)
		{
			return (new HandshakeEx((Handshake)handshake));
		}

		/// <summary></summary>
		public static implicit operator string(HandshakeEx handshake)
		{
			return (handshake.ToString());
		}

		/// <summary></summary>
		public static implicit operator HandshakeEx(string handshake)
		{
			return (Parse(handshake));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
