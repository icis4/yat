﻿//==================================================================================================
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
	#region Enum AutoResponse

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum AutoResponse
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

		SendText,
		SendFile,

		DedicatedCommand
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum AutoResponseEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoResponseEx : EnumEx
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

		private const string             SendText_string = "[Send Text]";
		private static readonly string[] SendText_stringAlternatives = new string[] { "[Text]", "[ST]", "[T]" };

		private const string             SendFile_string = "[Send File]";
		private static readonly string[] SendFile_stringAlternatives = new string[] { "[File]", "[SF]", "[F]" };

		#endregion

		private Command dedicatedCommand;

		/// <summary>Default is <see cref="AutoResponse.None"/>.</summary>
		public AutoResponseEx()
			: this(AutoResponse.None)
		{
		}

		/// <summary></summary>
		public AutoResponseEx(AutoResponse autoResponse)
			: base(autoResponse)
		{
		}

		/// <summary></summary>
		public AutoResponseEx(string dedicatedCommand)
			: base(AutoResponse.DedicatedCommand)
		{
			this.dedicatedCommand = new Command(dedicatedCommand);
		}

		/// <summary></summary>
		public Command DedicatedCommand
		{
			get
			{
				if ((AutoResponse)UnderlyingEnum == AutoResponse.DedicatedCommand)
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
				switch ((AutoResponse)UnderlyingEnum)
				{
					case AutoResponse.None:
						return (false);

					default:
						return (true);
				}
			}
		}

		/// <summary></summary>
		public int ToPredefinedCommandId()
		{
			int responseInt = (int)(AutoResponse)UnderlyingEnum;
			if ((responseInt >= (int)AutoResponse.PredefinedCommand1) &&
				(responseInt <= (int)AutoResponse.PredefinedCommand12))
			{
				return (responseInt);
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
			switch ((AutoResponse)UnderlyingEnum)
			{
				case AutoResponse.None:                return (None_string);
				case AutoResponse.PredefinedCommand1:  return (PredefinedCommand_string + " 1]");
				case AutoResponse.PredefinedCommand2:  return (PredefinedCommand_string + " 2]");
				case AutoResponse.PredefinedCommand3:  return (PredefinedCommand_string + " 3]");
				case AutoResponse.PredefinedCommand4:  return (PredefinedCommand_string + " 4]");
				case AutoResponse.PredefinedCommand5:  return (PredefinedCommand_string + " 5]");
				case AutoResponse.PredefinedCommand6:  return (PredefinedCommand_string + " 6]");
				case AutoResponse.PredefinedCommand7:  return (PredefinedCommand_string + " 7]");
				case AutoResponse.PredefinedCommand8:  return (PredefinedCommand_string + " 8]");
				case AutoResponse.PredefinedCommand9:  return (PredefinedCommand_string + " 9]");
				case AutoResponse.PredefinedCommand10: return (PredefinedCommand_string + " 10]");
				case AutoResponse.PredefinedCommand11: return (PredefinedCommand_string + " 11]");
				case AutoResponse.PredefinedCommand12: return (PredefinedCommand_string + " 12]");
				case AutoResponse.SendText:            return (SendText_string);
				case AutoResponse.SendFile:            return (SendFile_string);
				case AutoResponse.DedicatedCommand:    return (this.dedicatedCommand.SingleLineText);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoResponseEx[] GetAllItems()
		{
			return (GetItems(true, true));
		}

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoResponseEx[] GetFixedItems()
		{
			return (GetItems(true, false));
		}

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		private static AutoResponseEx[] GetItems(bool addFixed, bool addVariable)
		{
			List<AutoResponseEx> a = new List<AutoResponseEx>(16); // Preset the initial capactiy to improve memory management, 16 is a large enough value.

			if (addFixed)		a.Add(new AutoResponseEx(AutoResponse.None));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand1));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand2));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand3));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand4));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand5));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand6));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand7));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand8));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand9));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand10));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand11));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand12));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.SendText));
			if (addVariable)	a.Add(new AutoResponseEx(AutoResponse.SendFile));

			// Do not add AutoResponse.DedicatedCommand as this needs to be filled-in as string.

			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static AutoResponseEx Parse(string s)
		{
			AutoResponseEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid AutoResponse string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoResponseEx result)
		{
			AutoResponse enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				if (enumResult == AutoResponse.DedicatedCommand)
					result = new AutoResponseEx(s);
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
		public static bool TryParse(string s, out AutoResponse result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = AutoResponse.None;
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
							int enumValue = ((int)AutoResponse.PredefinedCommand1 - 1) + intValue;
							result = (AutoResponse)enumValue;
							return (true);
						}
					}
				}

				// Fallback:
				result = AutoResponse.PredefinedCommand1;
				return (false);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, SendText_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, SendText_stringAlternatives))
			{
				result = AutoResponse.SendText;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, SendFile_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, SendFile_stringAlternatives))
			{
				result = AutoResponse.SendFile;
				return (true);
			}
			else
			{
				result = AutoResponse.DedicatedCommand;
				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator AutoResponse(AutoResponseEx autoResponse)
		{
			return ((AutoResponse)autoResponse.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator AutoResponseEx(AutoResponse autoResponse)
		{
			return (new AutoResponseEx(autoResponse));
		}

		/// <summary></summary>
		public static implicit operator string(AutoResponseEx autoResponse)
		{
			return (autoResponse.ToString());
		}

		/// <summary></summary>
		public static implicit operator AutoResponseEx(string autoResponse)
		{
			return (Parse(autoResponse));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
