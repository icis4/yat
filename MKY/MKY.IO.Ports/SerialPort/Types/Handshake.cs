//==================================================================================================
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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	/// 
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HandshakeEx : EnumEx
	{
		#region String Definitions

		private const string             None_string = "None";
		private static readonly string[] None_stringAlternatives = new string[] { "N" };

		private const string             RfrCts_string = "Hardware (RFR/CTS)";
		private const string             RfrCts_stringShort = "Hardware";
		private static readonly string[] RfrCts_stringAlternatives = new string[] { "Hardware", "Hard", "HW", "H" };

		private const string             XOnXOff_string = "Software (XOn/XOff)";
		private const string             XOnXOff_stringShort = "Software";
		private static readonly string[] XOnXOff_stringAlternatives = new string[] { "Software", "Soft", "SW", "S" };

		private const string             RfrCtsXOnXOff_string = "Combined (RFR/CTS + XOn/XOff)";
		private const string             RfrCtsXOnXOff_stringShort = "Combined";
		private static readonly string[] RfrCtsXOnXOff_stringAlternatives = new string[] { "Combined", "Combi", "C" };

		#endregion

		/// <summary>Default is <see cref="Handshake.None"/>.</summary>
		public const Handshake Default = Handshake.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public HandshakeEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public HandshakeEx(Handshake handshake)
			: base(handshake)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_string);
				case Handshake.RequestToSend:        return (RfrCts_string);
				case Handshake.XOnXOff:              return (XOnXOff_string);
				case Handshake.RequestToSendXOnXOff: return (RfrCtsXOnXOff_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public virtual string ToShortString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_string);
				case Handshake.RequestToSend:        return (RfrCts_stringShort);
				case Handshake.XOnXOff:              return (XOnXOff_stringShort);
				case Handshake.RequestToSendXOnXOff: return (RfrCtsXOnXOff_stringShort);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static HandshakeEx[] GetItems()
		{
			List<HandshakeEx> a = new List<HandshakeEx>(4); // Preset the required capacity to improve memory management.
			a.Add(new HandshakeEx(Handshake.None));
			a.Add(new HandshakeEx(Handshake.RequestToSend));
			a.Add(new HandshakeEx(Handshake.XOnXOff));
			a.Add(new HandshakeEx(Handshake.RequestToSendXOnXOff));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static HandshakeEx Parse(string s)
		{
			HandshakeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid handshake string! String must one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out HandshakeEx result)
		{
			Handshake enumResult;
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
		public static bool TryParse(string s, out Handshake result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new HandshakeEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = Handshake.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, RfrCts_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, RfrCts_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, RfrCts_stringAlternatives))
			{
				result = Handshake.RequestToSend;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, XOnXOff_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, XOnXOff_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, XOnXOff_stringAlternatives))
			{
				result = Handshake.XOnXOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, RfrCtsXOnXOff_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, RfrCtsXOnXOff_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, RfrCtsXOnXOff_stringAlternatives))
			{
				result = Handshake.RequestToSendXOnXOff;
				return (true);
			}
			else // Invalid string!
			{
				result = new HandshakeEx(); // Default!
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
