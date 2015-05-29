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
// YAT 2.0 Gamma 1 Version 1.99.32
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum TerminalType

	/// <summary></summary>
	public enum TerminalType
	{
		/// <summary></summary>
		Text,

		/// <summary></summary>
		Binary
	}

	#endregion

	/// <summary>
	/// Extended enum TerminalTypeEx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class TerminalTypeEx : EnumEx
	{
		#region String Definitions

		private const string Text_string      = "Text";
		private const string Text_stringShort = "T";

		private const string Binary_string      = "Binary";
		private const string Binary_stringShort = "B";

		#endregion

		/// <summary>Default is <see cref="TerminalType.Text"/>.</summary>
		public TerminalTypeEx()
			: base(TerminalType.Text)
		{
		}

		/// <summary></summary>
		protected TerminalTypeEx(TerminalType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((TerminalType)UnderlyingEnum)
			{
				case TerminalType.Text:   return (Text_string);
				case TerminalType.Binary: return (Binary_string);
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static TerminalTypeEx[] GetItems()
		{
			List<TerminalTypeEx> a = new List<TerminalTypeEx>();
			a.Add(new TerminalTypeEx(TerminalType.Text));
			a.Add(new TerminalTypeEx(TerminalType.Binary));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static TerminalTypeEx Parse(string s)
		{
			TerminalTypeEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid terminal type string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out TerminalTypeEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Text_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Text_stringShort))
			{
				result = new TerminalTypeEx(TerminalType.Text);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Binary_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Binary_stringShort))
			{
				result = new TerminalTypeEx(TerminalType.Binary);
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
		public static implicit operator TerminalType(TerminalTypeEx type)
		{
			return ((TerminalType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator TerminalTypeEx(TerminalType type)
		{
			return (new TerminalTypeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(TerminalTypeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator TerminalTypeEx(int type)
		{
			return (new TerminalTypeEx((TerminalType)type));
		}

		/// <summary></summary>
		public static implicit operator string(TerminalTypeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator TerminalTypeEx(string type)
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
