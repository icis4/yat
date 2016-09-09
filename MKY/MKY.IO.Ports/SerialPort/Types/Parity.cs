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
// MKY Version 1.0.15
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
	/// Extended enum ParityEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class ParityEx : EnumEx
	{
		#region String Definitions

		private const string Even_string       = "Even";
		private const string Even_stringShort  = "E";
		private const string Odd_string        = "Odd";
		private const string Odd_stringShort   = "O";
		private const string None_string       = "None";
		private const string None_stringShort  = "N";
		private const string Mark_string       = "Mark";
		private const string Mark_stringShort  = "M";
		private const string Space_string      = "Space";
		private const string Space_stringShort = "S";

		#endregion

		/// <summary>Default is <see cref="Parity.None"/>.</summary>
		public const Parity Default = Parity.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public ParityEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public ParityEx(Parity parity)
			: base(parity)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Parity)UnderlyingEnum)
			{
				case Parity.Even:  return (Even_string);
				case Parity.Odd:   return (Odd_string);
				case Parity.None:  return (None_string);
				case Parity.Mark:  return (Mark_string);
				case Parity.Space: return (Space_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			switch ((Parity)UnderlyingEnum)
			{
				case Parity.Even: return (Even_stringShort);
				case Parity.Odd: return (Odd_stringShort);
				case Parity.None: return (None_stringShort);
				case Parity.Mark: return (Mark_stringShort);
				case Parity.Space: return (Space_stringShort);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static ParityEx[] GetItems()
		{
			List<ParityEx> a = new List<ParityEx>(5); // Preset the required capacity to improve memory management.
			a.Add(new ParityEx(Parity.Even));
			a.Add(new ParityEx(Parity.Odd));
			a.Add(new ParityEx(Parity.None));
			a.Add(new ParityEx(Parity.Mark));
			a.Add(new ParityEx(Parity.Space));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static ParityEx Parse(string s)
		{
			ParityEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid parity string! String must one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out ParityEx result)
		{
			Parity enumResult;
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
		public static bool TryParse(string s, out Parity result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = new ParityEx(); // Default!
				return (true); // Default silently, can e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Even_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Even_stringShort))
			{
				result = Parity.Even;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Odd_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Odd_stringShort))
			{
				result = Parity.Odd;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringShort))
			{
				result = Parity.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Mark_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Mark_stringShort))
			{
				result = Parity.Mark;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringShort))
			{
				result = Parity.Space;
				return (true);
			}
			else // Invalid string!
			{
				result = new ParityEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Parity(ParityEx parity)
		{
			return ((Parity)parity.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator ParityEx(Parity parity)
		{
			return (new ParityEx(parity));
		}

		/// <summary></summary>
		public static implicit operator int(ParityEx parity)
		{
			return (parity.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator ParityEx(int parity)
		{
			return (new ParityEx((Parity)parity));
		}

		/// <summary></summary>
		public static implicit operator string(ParityEx parity)
		{
			return (parity.ToString());
		}

		/// <summary></summary>
		public static implicit operator ParityEx(string parity)
		{
			return (Parse(parity));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
