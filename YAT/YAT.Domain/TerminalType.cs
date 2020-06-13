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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class TerminalTypeEx : EnumEx
	{
		#region String Definitions

		private const string Text_string      = "Text";
		private const string Text_stringShort = "T";
		private const string Text_stringStart = "Tex";

		private const string Binary_string      = "Binary";
		private const string Binary_stringShort = "B";
		private const string Binary_stringStart = "Bin";

		#endregion

		/// <summary>Default is <see cref="TerminalType.Text"/>.</summary>
		public const TerminalType Default = TerminalType.Text;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public TerminalTypeEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public TerminalTypeEx(TerminalType type)
			: base(type)
		{
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual bool IsText
		{
			get { return ((TerminalType)UnderlyingEnum == TerminalType.Text); }
		}

		/// <summary></summary>
		public virtual bool IsBinary
		{
			get { return ((TerminalType)UnderlyingEnum == TerminalType.Binary); }
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
			switch ((TerminalType)UnderlyingEnum)
			{
				case TerminalType.Text:   return (Text_string);
				case TerminalType.Binary: return (Binary_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is a terminal type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static TerminalTypeEx[] GetItems()
		{
			var a = new List<TerminalTypeEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new TerminalTypeEx(TerminalType.Text));
			a.Add(new TerminalTypeEx(TerminalType.Binary));

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
		public static TerminalTypeEx Parse(string s)
		{
			TerminalTypeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid terminal type string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out TerminalTypeEx result)
		{
			TerminalType enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new TerminalTypeEx(enumResult);
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
		public static bool TryParse(string s, out TerminalType result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase    (s, Text_string) ||
			         StringEx.EqualsOrdinalIgnoreCase    (s, Text_stringShort) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Text_stringStart))
			{
				result = TerminalType.Text;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase    (s, Binary_string) ||
			         StringEx.EqualsOrdinalIgnoreCase    (s, Binary_stringShort) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, Binary_stringStart))
			{
				result = TerminalType.Binary;
				return (true);
			}
			else // Invalid string!
			{
				result = new TerminalTypeEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
