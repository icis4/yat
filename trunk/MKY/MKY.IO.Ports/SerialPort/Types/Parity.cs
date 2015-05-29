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
// MKY Version 1.0.11
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class ParityEx : EnumEx
	{
		#region String Definitions

		private const string Even_string = "Even";
		private const string Even_stringShort = "E";
		private const string Odd_string = "Odd";
		private const string Odd_stringShort = "O";
		private const string None_string = "None";
		private const string None_stringShort = "N";
		private const string Mark_string = "Mark";
		private const string Mark_stringShort = "M";
		private const string Space_string = "Space";
		private const string Space_stringShort = "S";

		#endregion

		/// <summary>Default is <see cref="Parity.None"/>.</summary>
		public ParityEx()
			: base(Parity.None)
		{
		}

		/// <summary></summary>
		protected ParityEx(Parity parity)
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
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
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
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static ParityEx[] GetItems()
		{
			List<ParityEx> a = new List<ParityEx>();
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
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid parity string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out ParityEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Even_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Even_stringShort))
			{
				result = new ParityEx(Parity.Even);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Odd_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Odd_stringShort))
			{
				result = new ParityEx(Parity.Odd);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringShort))
			{
				result = new ParityEx(Parity.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Mark_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Mark_stringShort))
			{
				result = new ParityEx(Parity.Mark);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringShort))
			{
				result = new ParityEx(Parity.Space);
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
