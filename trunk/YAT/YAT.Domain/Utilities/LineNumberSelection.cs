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

namespace YAT.Domain.Utilities
{
	#region Enum LineNumberSelection

	/// <summary></summary>
	public enum LineNumberSelection
	{
		/// <summary></summary>
		Total,

		/// <summary></summary>
		Buffer
	}

	#endregion

	/// <summary>
	/// Extended enum LineNumberSelectionEx which extends <see cref="LineNumberSelection"/>.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class LineNumberSelectionEx : EnumEx
	{
		#region String Definitions

		private const string Total_string  = "Total Absolute";
		private const string Buffer_string = "Buffer Relative";

		#endregion

		/// <summary>Default is <see cref="LineNumberSelection.Total"/>.</summary>
		public const LineNumberSelection Default = LineNumberSelection.Total;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public LineNumberSelectionEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public LineNumberSelectionEx(LineNumberSelection selection)
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
			switch ((LineNumberSelection)UnderlyingEnum)
			{
				case LineNumberSelection.Total:  return (Total_string);
				case LineNumberSelection.Buffer: return (Buffer_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static LineNumberSelectionEx[] GetItems()
		{
			var a = new List<LineNumberSelectionEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new LineNumberSelectionEx(LineNumberSelection.Total));
			a.Add(new LineNumberSelectionEx(LineNumberSelection.Buffer));

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
		public static LineNumberSelectionEx Parse(string s)
		{
			LineNumberSelectionEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid selection string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out LineNumberSelectionEx result)
		{
			LineNumberSelection enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new LineNumberSelectionEx(enumResult);
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
		public static bool TryParse(string s, out LineNumberSelection result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Total_string))
			{
				result = LineNumberSelection.Total;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Buffer_string))
			{
				result = LineNumberSelection.Buffer;
				return (true);
			}
			else // Invalid string!
			{
				result = new LineNumberSelectionEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator LineNumberSelection(LineNumberSelectionEx selection)
		{
			return ((LineNumberSelection)selection.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator LineNumberSelectionEx(LineNumberSelection selection)
		{
			return (new LineNumberSelectionEx(selection));
		}

		/// <summary></summary>
		public static implicit operator int(LineNumberSelectionEx selection)
		{
			return (selection.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator LineNumberSelectionEx(int selection)
		{
			return (new LineNumberSelectionEx((LineNumberSelection)selection));
		}

		/// <summary></summary>
		public static implicit operator string(LineNumberSelectionEx selection)
		{
			return (selection.ToString());
		}

		/// <summary></summary>
		public static implicit operator LineNumberSelectionEx(string selection)
		{
			return (Parse(selection));
		}

		#endregion
	}
}
