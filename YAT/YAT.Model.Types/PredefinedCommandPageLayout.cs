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
// YAT Version 2.0.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
		OneByOne   = 12,
		TwoByOne   = 24,
		ThreeByOne = 36,
		TwoByTwo   = 48,
		ThreeByTwo = 72
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
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class PredefinedCommandPageLayoutEx : EnumEx
	{
		#region String Definitions

		private const string             OneByOne_string               =                "12 (1x12)";
		private static readonly string[] OneByOne_stringAlternatives   = new string[] { "12", "1x12" };
		private const string             TwoByOne_string               =                "24 (2x12)";
		private static readonly string[] TwoByOne_stringAlternatives   = new string[] { "24", "2x12" };
		private const string             ThreeByOne_string             =                "36 (3x12)";
		private static readonly string[] ThreeByOne_stringAlternatives = new string[] { "36", "3x12)" };
		private const string             TwoByTwo_string               =                "48 (2x2x12)";
		private static readonly string[] TwoByTwo_stringAlternatives   = new string[] { "48", "2x2x12)" };
		private const string             ThreeByTwo_string             =                "72 (3x2x12)";
		private static readonly string[] ThreeByTwo_stringAlternatives = new string[] { "72", "3x2x12)" };

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
				case PredefinedCommandPageLayout.OneByOne:   return (OneByOne_string);
				case PredefinedCommandPageLayout.TwoByOne:   return (TwoByOne_string);
				case PredefinedCommandPageLayout.ThreeByOne: return (ThreeByOne_string);
				case PredefinedCommandPageLayout.TwoByTwo:   return (TwoByTwo_string);
				case PredefinedCommandPageLayout.ThreeByTwo: return (ThreeByTwo_string);

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
			var a = new List<PredefinedCommandPageLayoutEx>(7); // Preset the required capacity to improve memory management.

			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.OneByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.TwoByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.ThreeByOne));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.TwoByTwo));
			a.Add(new PredefinedCommandPageLayoutEx(PredefinedCommandPageLayout.ThreeByTwo));

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
			else // Invalid string!
			{
				result = new PredefinedCommandPageLayoutEx(); // Default!
				return (false);
			}
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

		/// <summary></summary>
		public static implicit operator int(PredefinedCommandPageLayoutEx layout)
		{
			return (layout.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator PredefinedCommandPageLayoutEx(int layout)
		{
			return (new PredefinedCommandPageLayoutEx((PredefinedCommandPageLayout)layout));
		}

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
