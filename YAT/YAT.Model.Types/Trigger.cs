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
	public enum Trigger
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

		DedicatedCommand
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum TriggerEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class TriggerEx : EnumEx
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

		private Command dedicatedCommand;

		/// <summary>Default is <see cref="Trigger.None"/>.</summary>
		public TriggerEx()
			: base(Trigger.None)
		{
		}

		/// <summary></summary>
		protected TriggerEx(Trigger trigger)
			: base(trigger)
		{
		}

		/// <summary></summary>
		protected TriggerEx(string dedicatedCommand)
			: base(Trigger.DedicatedCommand)
		{
			this.dedicatedCommand = new Command(dedicatedCommand);
		}

		/// <summary></summary>
		public Command DedicatedCommand
		{
			get
			{
				if ((Trigger)UnderlyingEnum == Trigger.DedicatedCommand)
					return (this.dedicatedCommand);
				else
					return (new Command());
			}
		}

		/// <summary></summary>
		public bool CommandIsRequired
		{
			get
			{
				switch ((Trigger)UnderlyingEnum)
				{
					case Trigger.None:
					case Trigger.AnyLine:
						return (false);

					default:
						return (true);
				}
			}
		}

		/// <summary></summary>
		public int ToPredefinedCommandId()
		{
			int triggerInt = (int)(Trigger)UnderlyingEnum;
			if ((triggerInt >= (int)Trigger.PredefinedCommand1) &&
				(triggerInt <= (int)Trigger.PredefinedCommand12))
			{
				return (triggerInt);
			}
			else
			{
				return (InvalidPredefinedCommandId);
			}
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Trigger)UnderlyingEnum)
			{
				case Trigger.None:                return (None_string);
				case Trigger.PredefinedCommand1:  return (PredefinedCommand_string + " 1]");
				case Trigger.PredefinedCommand2:  return (PredefinedCommand_string + " 2]");
				case Trigger.PredefinedCommand3:  return (PredefinedCommand_string + " 3]");
				case Trigger.PredefinedCommand4:  return (PredefinedCommand_string + " 4]");
				case Trigger.PredefinedCommand5:  return (PredefinedCommand_string + " 5]");
				case Trigger.PredefinedCommand6:  return (PredefinedCommand_string + " 6]");
				case Trigger.PredefinedCommand7:  return (PredefinedCommand_string + " 7]");
				case Trigger.PredefinedCommand8:  return (PredefinedCommand_string + " 8]");
				case Trigger.PredefinedCommand9:  return (PredefinedCommand_string + " 9]");
				case Trigger.PredefinedCommand10: return (PredefinedCommand_string + " 10]");
				case Trigger.PredefinedCommand11: return (PredefinedCommand_string + " 11]");
				case Trigger.PredefinedCommand12: return (PredefinedCommand_string + " 12]");
				case Trigger.AnyLine:             return (AnyLine_string);
				case Trigger.DedicatedCommand:    return (this.dedicatedCommand.SingleLineText);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static TriggerEx[] GetItems()
		{
			return (GetItems(true, true));
		}

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static TriggerEx[] GetItems(bool addFixed, bool addVariable)
		{
			List<TriggerEx> a = new List<TriggerEx>();

			if (addFixed)		a.Add(new TriggerEx(Trigger.None));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand1));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand2));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand3));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand4));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand5));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand6));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand7));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand8));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand9));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand10));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand11));
			if (addVariable)	a.Add(new TriggerEx(Trigger.PredefinedCommand12));
			if (addFixed)		a.Add(new TriggerEx(Trigger.AnyLine));

			// Do not add Trigger.DedicatedCommand as this needs to be filled-in as string.

			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static TriggerEx Parse(string s)
		{
			TriggerEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid Trigger string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out TriggerEx result)
		{
			Trigger enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				if (enumResult == Trigger.DedicatedCommand)
					result = new TriggerEx(s);
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
		public static bool TryParse(string s, out Trigger result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = Trigger.None;
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
							int enumValue = ((int)Trigger.PredefinedCommand1 - 1) + intValue;
							result = (Trigger)enumValue;
							return (true);
						}
					}
				}

				// Fallback:
				result = Trigger.PredefinedCommand1;
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, AnyLine_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, AnyLine_stringAlternatives))
			{
				result = Trigger.AnyLine;
				return (true);
			}
			else
			{
				result = Trigger.DedicatedCommand;
				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Trigger(TriggerEx trigger)
		{
			return ((Trigger)trigger.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator TriggerEx(Trigger trigger)
		{
			return (new TriggerEx(trigger));
		}

		/// <summary></summary>
		public static implicit operator string(TriggerEx trigger)
		{
			return (trigger.ToString());
		}

		/// <summary></summary>
		public static implicit operator TriggerEx(string trigger)
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
