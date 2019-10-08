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
// YAT Version 2.3.90 Development
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

namespace YAT.Domain.Utilities
{
	#region Enum LengthSelection

	/// <summary></summary>
	public enum LengthSelection
	{
		/// <summary></summary>
		ByteCount,

		/// <summary></summary>
		CharCount
	}

	#endregion

	/// <summary>
	/// Extended enum LengthSelectionEx which extends <see cref="LengthSelection"/>.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class LengthSelectionEx : EnumEx
	{
		#region String Definitions

		private const string ByteCount_string = "Byte Count";
		private const string CharCount_string = "Character Count";

		#endregion

		/// <summary>Default for text terminals is <see cref="LengthSelection.CharCount"/>.</summary>
		public const LengthSelection TextDefault = LengthSelection.CharCount;

		/// <summary>Default for binary terminals is <see cref="LengthSelection.ByteCount"/>.</summary>
		public const LengthSelection BinaryDefault = LengthSelection.ByteCount;

		/// <summary>
		/// Default is <see cref="BinaryDefault"/> as its value of
		/// <see cref="LengthSelection.ByteCount"/> is supported by all terminal types.
		/// </summary>
		public const LengthSelection Default = BinaryDefault;

		/// <summary>
		/// Default is <see cref="Default"/>.
		/// </summary>
		public LengthSelectionEx()
			: this(Default)
		{
		}

		/// <summary>
		/// Default is <see cref="TextDefault"/> for text
		/// and <see cref="BinaryDefault"/> for binary terminals.
		/// </summary>
		public LengthSelectionEx(TerminalType terminalType)
			: this((terminalType == TerminalType.Text) ? TextDefault : BinaryDefault)
		{
		}

		/// <summary></summary>
		public LengthSelectionEx(LengthSelection selection)
			: base(selection)
		{
		}

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
			switch ((LengthSelection)UnderlyingEnum)
			{
				case LengthSelection.ByteCount: return (ByteCount_string);
				case LengthSelection.CharCount: return (CharCount_string);

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
		public static LengthSelectionEx[] GetItems()
		{
			var a = new List<LengthSelectionEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new LengthSelectionEx(LengthSelection.ByteCount));
			a.Add(new LengthSelectionEx(LengthSelection.CharCount));

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static LengthSelectionEx Parse(string s, TerminalType terminalType = TerminalType.Binary) // 'Default' is 'Binary' as 'BinaryDefault' of 'ByteCount' is supported by all terminal types.
		{
			LengthSelectionEx result;
			if (TryParse(s, terminalType, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid selection string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, TerminalType terminalType, out LengthSelectionEx result)
		{
			LengthSelection enumResult;
			if (TryParse(s, terminalType, out enumResult)) // TryParse() trims whitespace.
			{
				result = new LengthSelectionEx(enumResult);
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
		public static bool TryParse(string s, TerminalType terminalType, out LengthSelection result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, ByteCount_string))
			{
				result = LengthSelection.ByteCount;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CharCount_string))
			{
				result = LengthSelection.CharCount;
				return (true);
			}
			else // Invalid string!
			{
				result = new LengthSelectionEx(terminalType); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator LengthSelection(LengthSelectionEx selection)
		{
			return ((LengthSelection)selection.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator LengthSelectionEx(LengthSelection selection)
		{
			return (new LengthSelectionEx(selection));
		}

		/// <summary></summary>
		public static implicit operator int(LengthSelectionEx selection)
		{
			return (selection.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator LengthSelectionEx(int selection)
		{
			return (new LengthSelectionEx((LengthSelection)selection));
		}

		/// <summary></summary>
		public static implicit operator string(LengthSelectionEx selection)
		{
			return (selection.ToString());
		}

		/// <summary></summary>
		public static implicit operator LengthSelectionEx(string selection)
		{
			return (Parse(selection));
		}

		#endregion
	}
}
