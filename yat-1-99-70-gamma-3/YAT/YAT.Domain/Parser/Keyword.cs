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
// YAT 2.0 Gamma 3 Version 1.99.70
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Domain.Parser
{
	#region Enum Keyword

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Keyword
	{
		None,
		Clear,
		Delay,
		LineDelay,
		LineInterval,
		LineRepeat,
		Eol,
		NoEol,
		OutputBreakOn,
		OutputBreakOff,
		OutputBreakToggle,
		ReportId,

		/// <summary>
		/// A special keyword for internal testing (= FIT).
		/// </summary>
		/// <remarks>
		/// Prepended "ZZZ_" to get it at the bottom of a selection list.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "See remarks above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ZZZ", Justification = "See remarks above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FIT", Justification = "See remarks above.")]
		ZZZ_FIT
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum KeywordEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class KeywordEx : EnumEx
	{
		#region String Definitions

		private const string Clear_string             = "Clear";
		private const string Delay_string             = "Delay";
		private const string LineDelay_string         = "LineDelay";
		private const string LineInterval_string      = "LineInterval";
		private const string LineRepeat_string        = "LineRepeat";
		private const string Eol_string               = "EOL";
		private const string NoEol_string             = "NoEOL";
		private const string OutputBreakOn_string     = "OutputBreakOn";
		private const string OutputBreakOff_string    = "OutputBreakOff";
		private const string OutputBreakToggle_string = "OutputBreakToggle";
		private const string ReportId_string          = "ReportID"; // "ID" instead of "Id" for better readability.

		private const string ZZZ_FIT_string = "ZZZ_FIT"; // = for internal testing.

		#endregion

		/// <summary>Default is <see cref="Keyword.None"/>.</summary>
		public const Keyword Default = Keyword.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public KeywordEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public KeywordEx(Keyword keyword)
			: base(keyword)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:             return (Clear_string);
				case Keyword.Delay:             return (Delay_string);
				case Keyword.LineDelay:         return (LineDelay_string);
				case Keyword.LineInterval:      return (LineInterval_string);
				case Keyword.LineRepeat:        return (LineRepeat_string);
				case Keyword.Eol:               return (Eol_string);
				case Keyword.NoEol:             return (NoEol_string);
				case Keyword.OutputBreakOn:     return (OutputBreakOn_string);
				case Keyword.OutputBreakOff:    return (OutputBreakOff_string);
				case Keyword.OutputBreakToggle: return (OutputBreakToggle_string);
				case Keyword.ReportId:          return (ReportId_string);

				case Keyword.ZZZ_FIT: return (ZZZ_FIT_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems/Args/Validation
		//==========================================================================================
		// GetItems/Args/Validation
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static KeywordEx[] GetItems()
		{
			List<KeywordEx> a = new List<KeywordEx>(11); // Preset the required capacity to improve memory management.

			// Do not add 'None'.

			a.Add(new KeywordEx(Keyword.Clear));
			a.Add(new KeywordEx(Keyword.Delay));
			a.Add(new KeywordEx(Keyword.LineDelay));
			a.Add(new KeywordEx(Keyword.LineInterval));
			a.Add(new KeywordEx(Keyword.LineRepeat));
			a.Add(new KeywordEx(Keyword.Eol));
			a.Add(new KeywordEx(Keyword.NoEol));
			a.Add(new KeywordEx(Keyword.OutputBreakOn));
			a.Add(new KeywordEx(Keyword.OutputBreakOff));
			a.Add(new KeywordEx(Keyword.OutputBreakToggle));
			a.Add(new KeywordEx(Keyword.ReportId));

			// Do not add 'ZZZ_FIT' (= for internal testing).

			return (a.ToArray());
		}

		/// <summary>
		/// Gets the maximum arguments count for this keyword.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Naming same as 'GetItems()'.")]
		public virtual int GetMaxArgsCount()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay:        return (1);
				case Keyword.LineDelay:    return (1);
				case Keyword.LineInterval: return (1);
				case Keyword.LineRepeat:   return (1);
				case Keyword.ReportId:     return (1);

				case Keyword.ZZZ_FIT:      return (3); // = for internal testing.

				default: return (0);
			}
		}

		/// <summary>
		/// Validates the given value for the given argument index of this keyword.
		/// </summary>
		public virtual bool Validate(int argIndex, int argValue)
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay:        return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:    return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval: return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				                                                  //// Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineRepeat:   return ((argValue >= 1) || (argValue == Settings.SendSettings.LineRepeatInfinite));
				                                                  //// Attention, a similar validation exists in 'View.Controls.UsbSerialHidDeviceSettings'. Changes here may have to be applied there too.
				case Keyword.ReportId:     return ((argValue >= 0) && (argValue <= 255));

				case Keyword.ZZZ_FIT: // = for internal testing.
				{
					switch (argIndex)
					{
						case 0: return (true);
						case 1: return (true);
						case 2: return (true);

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support more than 3 arguments!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Gets the validation fragment for the given argument index of this keyword.
		/// </summary>
		public virtual string GetValidationFragment(int argIndex)
		{
			string noArgSupportedMessage = "[" + MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support arguments! " + MessageHelper.SubmitBug + "]";

			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:             return (noArgSupportedMessage);
				case Keyword.Delay:             return ("an integer value of 1 or more indicating the delay in milliseconds");    // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:         return ("an integer value of 1 or more indicating the delay in milliseconds");    // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval:      return ("an integer value of 1 or more indicating the interval in milliseconds"); // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineRepeat:        return ("an integer value of 1 or more indicating the number of repetitions, or " + Settings.SendSettings.LineRepeatInfinite + " for infinite repetitions");
				case Keyword.Eol:               return (noArgSupportedMessage);                                                   // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.NoEol:             return (noArgSupportedMessage);
				case Keyword.OutputBreakOn:     return (noArgSupportedMessage);
				case Keyword.OutputBreakOff:    return (noArgSupportedMessage);
				case Keyword.OutputBreakToggle: return (noArgSupportedMessage);
				case Keyword.ReportId:          return ("ID must be a numeric value within 0..255"); // Attention, a similar message exists in 'View.Controls.UsbSerialHidDeviceSettings'. Changes here may have to be applied there too.

				case Keyword.ZZZ_FIT: // = for internal testing.
				{
					switch (argIndex)
					{
						case 0: return ("an integer value");
						case 1: return ("an integer value");
						case 2: return ("an integer value");

						default: return ("[" + MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support more than 3 arguments! " + MessageHelper.SubmitBug + "]");
					}
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static KeywordEx Parse(string s)
		{
			KeywordEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid keyword! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out KeywordEx result)
		{
			Keyword enumResult;
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
		public static bool TryParse(string s, out Keyword result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Clear_string))
			{
				result = Keyword.Clear;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Delay_string))
			{
				result = Keyword.Delay;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineDelay_string))
			{
				result = Keyword.LineDelay;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineInterval_string))
			{
				result = Keyword.LineInterval;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineRepeat_string))
			{
				result = Keyword.LineRepeat;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Eol_string))
			{
				result = Keyword.Eol;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, NoEol_string))
			{
				result = Keyword.NoEol;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOn_string))
			{
				result = Keyword.OutputBreakOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOff_string))
			{
				result = Keyword.OutputBreakOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakToggle_string))
			{
				result = Keyword.OutputBreakToggle;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ReportId_string))
			{
				result = Keyword.ReportId;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ZZZ_FIT_string))
			{
				result = Keyword.ZZZ_FIT;
				return (true);
			}
			else // Invalid string!
			{
				result = new KeywordEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator Keyword(KeywordEx keyword)
		{
			return ((Keyword)keyword.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(Keyword keyword)
		{
			return (new KeywordEx(keyword));
		}

		/// <summary></summary>
		public static implicit operator int(KeywordEx keyword)
		{
			return (keyword.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(int keyword)
		{
			return (new KeywordEx((Keyword)keyword));
		}

		/// <summary></summary>
		public static implicit operator string(KeywordEx keyword)
		{
			return (keyword.ToString());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(string keyword)
		{
			return (Parse(keyword));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
