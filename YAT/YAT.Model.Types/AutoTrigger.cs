//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

namespace YAT.Model.Types
{
	#region Enum Trigger

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum AutoTrigger
	{
		None = 0,

		PredefinedCommand1 = 1,
		PredefinedCommand2 = 2,
		PredefinedCommand3 = 3,
		PredefinedCommand4 = 4,
		PredefinedCommand5 = 5,
		PredefinedCommand6 = 6,
		PredefinedCommand7 = 7,
		PredefinedCommand8 = 8,
		PredefinedCommand9 = 9,
		PredefinedCommand10 = 10,
		PredefinedCommand11 = 11,
		PredefinedCommand12 = 12,

		AnyLine,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum TriggerEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoTriggerEx : EnumEx
	{
		/// <summary>
		/// The invalid predefined command ID (1..12).
		/// </summary>
		public const int InvalidPredefinedCommandId = 0;

		#region String Definitions

		private const string             None_string = "[None]";
		private static readonly string[] None_stringAlternatives = new string[] { "[N]" };

		private const string             PredefinedCommand_string = "[Predefined Command"; // 'StartsWith', see below.
		private static readonly string[] PredefinedCommand_stringAlternatives = new string[] { "[Predefined", "[PC", "[P" };

		private const string             AnyLine_string = "[Any Line]";
		private static readonly string[] AnyLine_stringAlternatives = new string[] { "[A]", "[AL]", "[*L]" };

		#endregion

		private string explicitCommandString;

		/// <summary>Default is <see cref="AutoTrigger.None"/>.</summary>
		public const AutoTrigger Default = AutoTrigger.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public AutoTriggerEx()
			: base(Default)
		{
		}

		/// <summary></summary>
		public AutoTriggerEx(AutoTrigger trigger)
			: base(trigger)
		{
		}

		/// <summary></summary>
		public AutoTriggerEx(string explicitCommandString)
			: base(AutoTrigger.Explicit)
		{
			this.explicitCommandString = explicitCommandString;
		}

		/// <summary></summary>
		public bool CommandIsRequired
		{
			get
			{
				switch ((AutoTrigger)UnderlyingEnum)
				{
					case AutoTrigger.None:
					case AutoTrigger.AnyLine:
						return (false);

					default:
						return (true);
				}
			}
		}

		/// <summary></summary>
		public int ToPredefinedCommandId()
		{
			int triggerInt = (int)(AutoTrigger)UnderlyingEnum;
			if ((triggerInt >= (int)AutoTrigger.PredefinedCommand1) &&
				(triggerInt <= (int)AutoTrigger.PredefinedCommand12))
			{
				return (triggerInt);
			}
			else
			{
				return (InvalidPredefinedCommandId);
			}
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			AutoTriggerEx other = (AutoTriggerEx)obj;
			if ((AutoTrigger)UnderlyingEnum == AutoTrigger.Explicit)
			{
				return
				(
					base.Equals(other) &&
					(this.explicitCommandString == other.explicitCommandString)
				);
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((AutoTrigger)UnderlyingEnum == AutoTrigger.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitCommandString != null ? this.explicitCommandString.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			switch ((AutoTrigger)UnderlyingEnum)
			{
				case AutoTrigger.None:                return (None_string);
				case AutoTrigger.PredefinedCommand1:  return (PredefinedCommand_string + " 1]");
				case AutoTrigger.PredefinedCommand2:  return (PredefinedCommand_string + " 2]");
				case AutoTrigger.PredefinedCommand3:  return (PredefinedCommand_string + " 3]");
				case AutoTrigger.PredefinedCommand4:  return (PredefinedCommand_string + " 4]");
				case AutoTrigger.PredefinedCommand5:  return (PredefinedCommand_string + " 5]");
				case AutoTrigger.PredefinedCommand6:  return (PredefinedCommand_string + " 6]");
				case AutoTrigger.PredefinedCommand7:  return (PredefinedCommand_string + " 7]");
				case AutoTrigger.PredefinedCommand8:  return (PredefinedCommand_string + " 8]");
				case AutoTrigger.PredefinedCommand9:  return (PredefinedCommand_string + " 9]");
				case AutoTrigger.PredefinedCommand10: return (PredefinedCommand_string + " 10]");
				case AutoTrigger.PredefinedCommand11: return (PredefinedCommand_string + " 11]");
				case AutoTrigger.PredefinedCommand12: return (PredefinedCommand_string + " 12]");
				case AutoTrigger.AnyLine:             return (AnyLine_string);
				case AutoTrigger.Explicit:            return (this.explicitCommandString);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoTriggerEx[] GetAllItems()
		{
			return (GetItems(true, true));
		}

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoTriggerEx[] GetFixedItems()
		{
			return (GetItems(true, false));
		}

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		private static AutoTriggerEx[] GetItems(bool addFixed, bool addVariable)
		{
			List<AutoTriggerEx> a = new List<AutoTriggerEx>(16); // Preset the initial capactiy to improve memory management, 16 is a large enough value.
			if (addFixed)		a.Add(new AutoTriggerEx(AutoTrigger.None));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand1));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand2));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand3));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand4));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand5));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand6));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand7));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand8));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand9));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand10));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand11));
			if (addVariable)	a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand12));
			if (addFixed)		a.Add(new AutoTriggerEx(AutoTrigger.AnyLine));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static AutoTriggerEx Parse(string s)
		{
			AutoTriggerEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid trigger string! String must be a valid trigger string, or one of the predefined triggers."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoTriggerEx result)
		{
			AutoTrigger enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				if (enumResult == AutoTrigger.Explicit)
					result = new AutoTriggerEx(s);
				else
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
		public static bool TryParse(string s, out AutoTrigger result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = AutoTrigger.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = AutoTrigger.None;
				return (true);
			}
			else if (StringEx.StartsWithOrdinalIgnoreCase   (s, PredefinedCommand_string) ||
			         StringEx.StartsWithAnyOrdinalIgnoreCase(s, PredefinedCommand_stringAlternatives))
			{
				string[] values = Regex.Split(s, @"\d+");
				if (values.Length > 0)
				{
					int intValue;
					if (int.TryParse(values[0], out intValue))
					{
						if ((intValue >= 1) && (intValue <= 12))
						{
							int enumValue = ((int)AutoTrigger.PredefinedCommand1 - 1) + intValue;
							result = (AutoTrigger)enumValue;
							return (true);
						}
					}
				}

				// Fallback:
				result = AutoTrigger.PredefinedCommand1;
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, AnyLine_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, AnyLine_stringAlternatives))
			{
				result = AutoTrigger.AnyLine;
				return (true);
			}
			else // Explicit!
			{
				result = AutoTrigger.Explicit;
				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator AutoTrigger(AutoTriggerEx trigger)
		{
			return ((AutoTrigger)trigger.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator AutoTriggerEx(AutoTrigger trigger)
		{
			return (new AutoTriggerEx(trigger));
		}

		/// <summary></summary>
		public static implicit operator string(AutoTriggerEx trigger)
		{
			return (trigger.ToString());
		}

		/// <summary></summary>
		public static implicit operator AutoTriggerEx(string trigger)
		{
			return (Parse(trigger));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
