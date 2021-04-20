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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum DecodingMismatchBehavior

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum DecodingMismatchBehavior
	{
		ComprehensiveWarning,
		UnicodeReplacementCharacterAndCompactWarning,
		UnicodeReplacementCharacter,
		QuestionMarkAndCompactWarning,
		QuestionMark,
		Discard
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum DecodingMismatchBehaviorEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class DecodingMismatchBehaviorEx : EnumEx
	{
		#region String Definitions

		private const string ComprehensiveWarning_string                         = "Comprehensive warning";
		private const string UnicodeReplacementCharacterAndCompactWarning_string = "� and compact warning";
		private const string UnicodeReplacementCharacter_string                  = "� only";
		private const string QuestionMarkAndCompactWarning_string                = "? and compact warning";
		private const string QuestionMark_string                                 = "? only";
		private const string Discard_string                                      = "Discard invalid bytes";

		#endregion

		/// <summary>Default is <see cref="DecodingMismatchBehavior.ComprehensiveWarning"/>.</summary>
		public const DecodingMismatchBehavior Default = DecodingMismatchBehavior.ComprehensiveWarning;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public DecodingMismatchBehaviorEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public DecodingMismatchBehaviorEx(DecodingMismatchBehavior behavior)
			: base(behavior)
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
			switch ((DecodingMismatchBehavior)UnderlyingEnum)
			{
				case DecodingMismatchBehavior.ComprehensiveWarning:                         return (ComprehensiveWarning_string);
				case DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning: return (UnicodeReplacementCharacterAndCompactWarning_string);
				case DecodingMismatchBehavior.UnicodeReplacementCharacter:                  return (UnicodeReplacementCharacter_string);
				case DecodingMismatchBehavior.QuestionMarkAndCompactWarning:                return (QuestionMarkAndCompactWarning_string);
				case DecodingMismatchBehavior.QuestionMark:                                 return (QuestionMark_string);
				case DecodingMismatchBehavior.Discard:                                      return (Discard_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static DecodingMismatchBehaviorEx[] GetItems()
		{
			var a = new List<DecodingMismatchBehaviorEx>(7); // Preset the required capacity to improve memory management.

			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.ComprehensiveWarning));
			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.UnicodeReplacementCharacterAndCompactWarning));
			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.UnicodeReplacementCharacter));
			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.QuestionMarkAndCompactWarning));
			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.QuestionMark));
			a.Add(new DecodingMismatchBehaviorEx(DecodingMismatchBehavior.Discard));

			return (a.ToArray());
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator DecodingMismatchBehavior(DecodingMismatchBehaviorEx behavior)
		{
			return ((DecodingMismatchBehavior)behavior.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator DecodingMismatchBehaviorEx(DecodingMismatchBehavior behavior)
		{
			return (new DecodingMismatchBehaviorEx(behavior));
		}

		/// <summary></summary>
		public static implicit operator int(DecodingMismatchBehaviorEx behavior)
		{
			return (behavior.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator DecodingMismatchBehaviorEx(int behavior)
		{
			return (new DecodingMismatchBehaviorEx((DecodingMismatchBehavior)behavior));
		}

		/// <summary></summary>
		public static implicit operator string(DecodingMismatchBehaviorEx behavior)
		{
			return (behavior.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
