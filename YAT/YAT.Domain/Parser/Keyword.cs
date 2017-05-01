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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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
using System.Text.RegularExpressions;

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

		ZzForInternalTesting
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

		private const string ZzForInternalTesting_string = "ZzForInternalTesting";

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

				case Keyword.ZzForInternalTesting: return (ZzForInternalTesting_string);

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
			List<KeywordEx> a = new List<KeywordEx>(10); // Preset the required capacity to improve memory management.

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

			// Do not add 'ZzForInternalTesting'.

			return (a.ToArray());
		}

		/// <summary>
		/// Gets the maximum arguments count for this keyword.
		/// </summary>
		public virtual int GetMaxArgsCount()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay:        return (1);
				case Keyword.LineDelay:    return (1);
				case Keyword.LineInterval: return (1);
				case Keyword.LineRepeat:   return (1);

				case Keyword.ZzForInternalTesting: return (3);

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
				case Keyword.Delay:        return (argValue >= 1); // Attention, a similar validation exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:    return (argValue >= 1); // Attention, a similar validation exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval: return (argValue >= 1); // Attention, a similar validation exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineRepeat:   return ((argValue >= 1) || (argValue == Settings.SendSettings.LineRepeatInfinite));
				                                                   // Attention, a similar validation exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.ZzForInternalTesting:
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
			string NoArgSupportedMessage = "[" + MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support arguments! " + MessageHelper.SubmitBug + "]";

			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:             return (NoArgSupportedMessage);
				case Keyword.Delay:             return ("an integer value of 1 or more indicating the delay in milliseconds");    // Attention, a similar message exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:         return ("an integer value of 1 or more indicating the delay in milliseconds");    // Attention, a similar message exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval:      return ("an integer value of 1 or more indicating the interval in milliseconds"); // Attention, a similar message exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineRepeat:        return ("an integer value of 1 or more indicating the number of repetitions, or " + Settings.SendSettings.LineRepeatInfinite + " for infinite repetitions");
				case Keyword.Eol:               return (NoArgSupportedMessage);                                                   // Attention, a similar message exists in 'View.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.NoEol:             return (NoArgSupportedMessage);
				case Keyword.OutputBreakOn:     return (NoArgSupportedMessage);
				case Keyword.OutputBreakOff:    return (NoArgSupportedMessage);
				case Keyword.OutputBreakToggle: return (NoArgSupportedMessage);

				case Keyword.ZzForInternalTesting:
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
				throw (new FormatException(@"""" + s + @""" is an invalid keyword! String must one of the underlying enumeration designations."));
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

			if (string.IsNullOrEmpty(s))
			{
				result = new KeywordEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Clear_string))
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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ZzForInternalTesting_string))
			{
				result = Keyword.ZzForInternalTesting;
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
