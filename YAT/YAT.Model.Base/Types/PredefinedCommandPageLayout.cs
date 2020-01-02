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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using MKY;

#endregion

namespace YAT.Model.Types
{
	#region Enum PredefinedCommandPageLayout

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary>
	/// Specifies the layout of a predefined command page.
	/// </summary>
	public enum PredefinedCommandPageLayout
	{
		OneByOne,
		TwoByOne,
		ThreeByOne,
		OneByTwo,
		TwoByTwo,
		ThreeByTwo,
		OneByThree,
		TwoByThree,
		ThreeByThree
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum PredefinedCommandPageLayoutEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class PredefinedCommandPageLayoutEx : EnumEx
	{
		#region String Definitions

		private const string             OneByOne_string                 =                 "12 (1x12)";
		private static readonly string[] OneByOne_stringAlternatives     = new string[] {  "12", "1x12" };
		private const string             TwoByOne_string                 =                 "24 (2x12)";
		private static readonly string[] TwoByOne_stringAlternatives     = new string[] {  "24", "2x12" };
		private const string             ThreeByOne_string               =                 "36 (3x12)";
		private static readonly string[] ThreeByOne_stringAlternatives   = new string[] {  "36", "3x12)" };
		private const string             OneByTwo_string                 =                 "24 (1x2x12)";
		private static readonly string[] OneByTwo_stringAlternatives     = new string[] {  "24", "1x2x12)" };
		private const string             TwoByTwo_string                 =                 "48 (2x2x12)";
		private static readonly string[] TwoByTwo_stringAlternatives     = new string[] {  "48", "2x2x12)" };
		private const string             ThreeByTwo_string               =                 "72 (3x2x12)";
		private static readonly string[] ThreeByTwo_stringAlternatives   = new string[] {  "72", "3x2x12)" };
		private const string             OneByThree_string               =                 "36 (1x3x12)";
		private static readonly string[] OneByThree_stringAlternatives   = new string[] {  "36", "1x3x12)" };
		private const string             TwoByThree_string               =                 "72 (2x3x12)";
		private static readonly string[] TwoByThree_stringAlternatives   = new string[] {  "72", "2x3x12)" };
		private const string             ThreeByThree_string             =                "108 (3x3x12)";
		private static readonly string[] ThreeByThree_stringAlternatives = new string[] { "108", "3x3x12)" };

		#endregion

		/// <summary>Default is <see cref="PredefinedCommandPageLayout.OneByOne"/>.</summary>
		public const PredefinedCommandPageLayout Default = PredefinedCommandPageLayout.OneByOne;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public PredefinedCommandPageLayoutEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout layout)
			: base(layout)
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
			switch ((PredefinedCommandPageLayout)UnderlyingEnum)
			{
				case PredefinedCommandPageLayout.OneByOne:     return (OneByOne_string);
				case PredefinedCommandPageLayout.TwoByOne:     return (TwoByOne_string);
				case PredefinedCommandPageLayout.ThreeByOne:   return (ThreeByOne_string);
				case PredefinedCommandPageLayout.OneByTwo:     return (OneByTwo_string);
				case PredefinedCommandPageLayout.TwoByTwo:     return (TwoByTwo_string);
				case PredefinedCommandPageLayout.ThreeByTwo:   return (ThreeByTwo_string);
				case PredefinedCommandPageLayout.OneByThree:   return (OneByThree_string);
				case PredefinedCommandPageLayout.TwoByThree:   return (TwoByThree_string);
				case PredefinedCommandPageLayout.ThreeByThree: return (ThreeByThree_string);

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
		public static PredefinedCommandPageLayoutEx[] GetItems()
		{
			var a = new List<PredefinedCommandPageLayoutEx>(9); // Preset the required capacity to improve memory management.

			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.OneByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.TwoByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.ThreeByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.OneByTwo));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.TwoByTwo));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.ThreeByTwo));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.OneByThree));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.TwoByThree));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.ThreeByThree));

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
		public static PredefinedCommandPageLayoutEx Parse(string s)
		{
			PredefinedCommandPageLayoutEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid predefined command page layout string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out PredefinedCommandPageLayoutEx result)
		{
			PredefinedCommandPageLayout enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new PredefinedCommandPageLayoutEx(enumResult);
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
		public static bool TryParse(string s, out PredefinedCommandPageLayout result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, OneByOne_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, OneByOne_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.OneByOne;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, TwoByOne_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, TwoByOne_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.TwoByOne;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ThreeByOne_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ThreeByOne_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.ThreeByOne;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, OneByTwo_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, OneByTwo_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.OneByTwo;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, TwoByTwo_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, TwoByTwo_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.TwoByTwo;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ThreeByTwo_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ThreeByTwo_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.ThreeByTwo;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, OneByThree_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, OneByThree_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.OneByThree;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, TwoByThree_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, TwoByThree_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.TwoByThree;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ThreeByThree_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ThreeByThree_stringAlternatives))
			{
				result = PredefinedCommandPageLayout.ThreeByThree;
				return (true);
			}
			else // Invalid string!
			{
				result = new PredefinedCommandPageLayoutEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region ...PerPage
		//==========================================================================================
		// ...PerPage
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public int ColumnsPerPage
		{
			get
			{
				switch ((PredefinedCommandPageLayout)UnderlyingEnum)
				{
					case PredefinedCommandPageLayout.OneByOne:
					case PredefinedCommandPageLayout.TwoByOne:
					case PredefinedCommandPageLayout.ThreeByOne:   return (1);
					case PredefinedCommandPageLayout.OneByTwo:
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.ThreeByTwo:   return (2);
					case PredefinedCommandPageLayout.OneByThree:
					case PredefinedCommandPageLayout.TwoByThree:
					case PredefinedCommandPageLayout.ThreeByThree: return (3);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public int RowsPerPage
		{
			get
			{
				switch ((PredefinedCommandPageLayout)UnderlyingEnum)
				{
					case PredefinedCommandPageLayout.OneByOne:
					case PredefinedCommandPageLayout.OneByTwo:
					case PredefinedCommandPageLayout.OneByThree:   return (1);
					case PredefinedCommandPageLayout.TwoByOne:
					case PredefinedCommandPageLayout.TwoByTwo:
					case PredefinedCommandPageLayout.TwoByThree:   return (2);
					case PredefinedCommandPageLayout.ThreeByOne:
					case PredefinedCommandPageLayout.ThreeByTwo:
					case PredefinedCommandPageLayout.ThreeByThree: return (3);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public int CommandCapacityPerPage
		{
			get
			{
				switch ((PredefinedCommandPageLayout)UnderlyingEnum)
				{
					case PredefinedCommandPageLayout.OneByOne:     return ( 12);
					case PredefinedCommandPageLayout.TwoByOne:     return ( 24);
					case PredefinedCommandPageLayout.ThreeByOne:   return ( 36);
					case PredefinedCommandPageLayout.OneByTwo:     return ( 24);
					case PredefinedCommandPageLayout.TwoByTwo:     return ( 48);
					case PredefinedCommandPageLayout.ThreeByTwo:   return ( 72);
					case PredefinedCommandPageLayout.OneByThree:   return ( 36);
					case PredefinedCommandPageLayout.TwoByThree:   return ( 72);
					case PredefinedCommandPageLayout.ThreeByThree: return (108);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public static int GetMatchingCommandCapacityPerPage(int commandCount)
		{
			return (GetMatchingItem(commandCount).CommandCapacityPerPage);
		}

		/// <summary>
		/// Gets the item for best matches the given count.
		/// </summary>
		public static PredefinedCommandPageLayoutEx GetMatchingItem(int commandCount)
		{
			if      (commandCount <=  12) return (PredefinedCommandPageLayout.OneByOne);
			else if (commandCount <=  24) return (PredefinedCommandPageLayout.TwoByOne); // rather than OneByTwo since 24 buttons likely fit on screen vertically.
			else if (commandCount <=  36) return (PredefinedCommandPageLayout.OneByThree);
			else if (commandCount <=  48) return (PredefinedCommandPageLayout.TwoByTwo);
			else if (commandCount <=  72) return (PredefinedCommandPageLayout.TwoByThree);
			else if (commandCount <= 108) return (PredefinedCommandPageLayout.ThreeByThree);
			else throw (new ArgumentOutOfRangeException("count", commandCount, MessageHelper.InvalidExecutionPreamble + "'" + commandCount.ToString(CultureInfo.InvariantCulture) + "' is a count that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator PredefinedCommandPageLayout(PredefinedCommandPageLayoutEx layout)
		{
			return ((PredefinedCommandPageLayout)layout.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout layout)
		{
			return (new PredefinedCommandPageLayoutEx(layout));
		}

	/////// <summary></summary>
	////public static implicit operator int(PredefinedCommandPageLayoutEx layout)
	////{
	////	return (layout.GetHashCode());
	////}
	//// Excluded to prevent accidental misuse with the number of rows/colmuns/commands per page.

	/////// <summary></summary>
	////public static implicit operator PredefinedCommandPageLayoutEx(int layout)
	////{
	////	return (new PredefinedCommandPageLayoutEx((PredefinedCommandPageLayout)layout));
	////}
	//// Excluded to prevent accidental misuse with the number of rows/colmuns/commands per page.

		/// <summary></summary>
		public static implicit operator string(PredefinedCommandPageLayoutEx layout)
		{
			return (layout.ToString());
		}

		/// <summary></summary>
		public static implicit operator PredefinedCommandPageLayoutEx(string layout)
		{
			return (Parse(layout));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
