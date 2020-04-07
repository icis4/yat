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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using MKY;
using MKY.Text.RegularExpressions;

#endregion

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

		PredefinedCommand1 = 1, // 1:1 mapping used by ToPredefinedCommandId().
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

		AnyLine, // Logically before, but located after the predefined commands to allow numbering them 1..12 accordingly.

		SendText, // Located after predefined commands same as on view.

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum AutoTriggerEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoTriggerEx : EnumEx, IEquatable<AutoTriggerEx>
	{
		/// <summary>
		/// The invalid predefined command ID (1..12).
		/// </summary>
		public const int InvalidPredefinedCommandId = 0;

		#region String Definitions
		                     // Attention:
		                     // These strings are used for XML serialization!
		                     // Not ideal, but required for 'Explicit'!
		private const string None_string                   = "[None]";
		private const string AnyLine_string                = "[Any Line]";       // Incl. space!
		private const string PredefinedCommand_stringStart = "[Predefined Command "; // Any of the commands, ID is parsed separately.
		private const string SendText_string               = "[Send Text]";

		#endregion

		/// <summary>Common regular expression capture patterns.</summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "A 'ReadOnlyCollection' indeed is a read-only wrapper.")]
		public static readonly ReadOnlyCollection<string> CommonRegexCapturePatterns;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Future patterns may have to implement more logic.")]
		static AutoTriggerEx()
		{
			var l = new List<string>(8); // Preset the required capacity to improve memory management.

			l.Add("(" + CommonPatterns.IntegralNumber            + ")");
			l.Add("(" + CommonPatterns.FloatingPointNormal       + ")");
			l.Add("(" + CommonPatterns.FloatingPointScientific   + ")");
			l.Add("(" + CommonPatterns.FloatingPointAny          + ")");
			l.Add("(" + CommonPatterns.TimeWithColons            + ")");
			l.Add("(" + CommonPatterns.DateWithHyphensDescending + ")");
			l.Add(@"\b([A-Z]{1})\b");
			l.Add(@"\b([A-Za-z]{2,4})\b");

			CommonRegexCapturePatterns = l.AsReadOnly();
		}

		private string explicitTriggerString;

		/// <summary>Default is <see cref="AutoTrigger.None"/>.</summary>
		public const AutoTrigger Default = AutoTrigger.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public AutoTriggerEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="AutoTrigger.Explicit"/> because that selection requires
		/// a trigger command string. Use <see cref="AutoTriggerEx(string)"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="trigger"/> is <see cref="AutoTrigger.Explicit"/>. Use <see cref="AutoTriggerEx(string)"/> instead.
		/// </exception>
		public AutoTriggerEx(AutoTrigger trigger)
			: base(trigger)
		{
			if (trigger == AutoTrigger.Explicit)
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'AutoTrigger.Explicit' requires a trigger command string, use 'AutoTriggerEx(string)' instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and enum parameters.")]
		public AutoTriggerEx(string explicitTriggerString)
			: base(AutoTrigger.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			this.explicitTriggerString = explicitTriggerString;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsExplicit
		{
			get { return ((AutoTrigger)UnderlyingEnum == AutoTrigger.Explicit); }
		}

		/// <summary></summary>
		public bool IsActive
		{
			get { return ((AutoTrigger)UnderlyingEnum != AutoTrigger.None); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Technically, 'Predefined' could also support text/regex, but doesn't make much sense:
		/// <list type="bullet">
		/// <item><description>Command defines a byte sequence anyway.</description></item>
		/// <item><description>YAT style escapes are useless for text matching.</description></item>
		/// <item><description>Regex style escapes are not supported for commands.</description></item>
		/// </list>
		/// </remarks>
		public bool TextIsSupported
		{
			get { return (((AutoTrigger)UnderlyingEnum) == AutoTrigger.Explicit); }
		}

		/// <remarks>
		/// Technically, 'Predefined' could also support text/regex, but doesn't make much sense:
		/// <list type="bullet">
		/// <item><description>Command defines a byte sequence anyway.</description></item>
		/// <item><description>YAT style escapes are useless for text matching.</description></item>
		/// <item><description>Regex style escapes are not supported for commands.</description></item>
		/// </list>
		/// </remarks>
		public bool RegexIsSupported
		{
			get { return (((AutoTrigger)UnderlyingEnum) == AutoTrigger.Explicit); }
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

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((AutoTrigger)UnderlyingEnum)
			{
				case AutoTrigger.None:                return (None_string);
				case AutoTrigger.AnyLine:             return (AnyLine_string);
				case AutoTrigger.PredefinedCommand1:  return (PredefinedCommand_stringStart + "1]");
				case AutoTrigger.PredefinedCommand2:  return (PredefinedCommand_stringStart + "2]");
				case AutoTrigger.PredefinedCommand3:  return (PredefinedCommand_stringStart + "3]");
				case AutoTrigger.PredefinedCommand4:  return (PredefinedCommand_stringStart + "4]");
				case AutoTrigger.PredefinedCommand5:  return (PredefinedCommand_stringStart + "5]");
				case AutoTrigger.PredefinedCommand6:  return (PredefinedCommand_stringStart + "6]");
				case AutoTrigger.PredefinedCommand7:  return (PredefinedCommand_stringStart + "7]");
				case AutoTrigger.PredefinedCommand8:  return (PredefinedCommand_stringStart + "8]");
				case AutoTrigger.PredefinedCommand9:  return (PredefinedCommand_stringStart + "9]");
				case AutoTrigger.PredefinedCommand10: return (PredefinedCommand_stringStart + "10]");
				case AutoTrigger.PredefinedCommand11: return (PredefinedCommand_stringStart + "11]");
				case AutoTrigger.PredefinedCommand12: return (PredefinedCommand_stringStart + "12]");
				case AutoTrigger.SendText:            return (SendText_string);
				case AutoTrigger.Explicit:            return (this.explicitTriggerString);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					hashCode = (hashCode * 397) ^ (this.explicitTriggerString != null ? this.explicitTriggerString.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as AutoTriggerEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(AutoTriggerEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((AutoTrigger)UnderlyingEnum == AutoTrigger.Explicit)
			{
				return
				(
					base.Equals(other) &&
					StringEx.EqualsOrdinal(this.explicitTriggerString, other.explicitTriggerString)
				);
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoTriggerEx lhs, AutoTriggerEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(AutoTriggerEx lhs, AutoTriggerEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static AutoTriggerEx[] GetAllItems()
		{
			return (GetItems(true, true));
		}

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static AutoTriggerEx[] GetFixedItems()
		{
			return (GetItems(true, false));
		}

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		private static AutoTriggerEx[] GetItems(bool addFixed, bool addVariable)
		{
			var a = new List<AutoTriggerEx>(16); // Preset the required capacity to improve memory management; 16 is a large enough value.

			if (addFixed)    a.Add(new AutoTriggerEx(AutoTrigger.None));
			if (addFixed)    a.Add(new AutoTriggerEx(AutoTrigger.AnyLine));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand1));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand2));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand3));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand4));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand5));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand6));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand7));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand8));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand9));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand10));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand11));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.PredefinedCommand12));
			if (addVariable) a.Add(new AutoTriggerEx(AutoTrigger.SendText));

			// This method shall only return the defined items, 'Explicit' is not added therefore.

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
				if (enumResult != AutoTrigger.Explicit)
					result = new AutoTriggerEx(enumResult);
				else
					result = new AutoTriggerEx(s);

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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = AutoTrigger.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, AnyLine_string))
			{
				result = AutoTrigger.AnyLine;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, PredefinedCommand_stringStart)) // StartWith()! Not Equals()!
			{
				var match = Regex.Match(s.Substring(PredefinedCommand_stringStart.Length - 1), @"\d+");
				if (match.Success)
				{
					int intValue;
					if (int.TryParse(match.Groups[0].Value, out intValue))
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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SendText_string))
			{
				result = AutoTrigger.SendText;
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
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator AutoTrigger(AutoTriggerEx trigger)
		{
			return ((AutoTrigger)trigger.UnderlyingEnum);
		}

		/// <remarks>
		/// Explicit because cast doesn't work for <see cref="AutoTrigger.Explicit"/>.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="trigger"/> is <see cref="AutoTrigger.Explicit"/>.
		/// </exception>
		public static explicit operator AutoTriggerEx(AutoTrigger trigger)
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
