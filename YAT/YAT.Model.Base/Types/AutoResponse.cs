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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		Trigger, // Located after predefined commands to allow numbering them 1..12 accordingly.

		SendText,
		SendFile,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum AutoResponseEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoResponseEx : EnumEx, IEquatable<AutoResponseEx>
	{
		/// <summary>
		/// The invalid predefined command ID (1..12).
		/// </summary>
		public const int InvalidPredefinedCommandId = 0;

		#region String Definitions

		private const string             None_string = "[None]";
		private static readonly string[] None_stringAlternatives = new string[] { "[N]" };

		private const string             Trigger_string = "[Trigger]";
		private static readonly string[] Trigger_stringAlternatives = new string[] { "[T]" };

		private const string             SendText_string = "[Send Text]";
		private static readonly string[] SendText_stringAlternatives = new string[] { "[Text]", "[ST]" };

		private const string             SendFile_string = "[Send File]";
		private static readonly string[] SendFile_stringAlternatives = new string[] { "[File]", "[SF]" };

		private const string             PredefinedCommand_string = "[Predefined Command"; // 'StartsWith', see below.
		private static readonly string[] PredefinedCommand_stringAlternatives = new string[] { "[Predefined", "[PC", "[P" };

		#endregion

		private string explicitCommandString;

		/// <summary>Default is <see cref="AutoResponse.None"/>.</summary>
		public const AutoResponse Default = AutoResponse.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public AutoResponseEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="AutoResponse.Explicit"/> because that selection requires
		/// a response command string. Use <see cref="AutoResponseEx(string)"/> instead.
		/// </remarks>
		public AutoResponseEx(AutoResponse response)
			: base(response)
		{
			Debug.Assert((response != AutoResponse.Explicit), "'AutoResponse.Explicit' requires a response command string, use 'AutoResponseEx(string)' instead!");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "The naming emphasizes the difference between string and enum parameters.")]
		public AutoResponseEx(string explicitCommandString)
			: base(AutoResponse.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			this.explicitCommandString = explicitCommandString;
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
			switch ((AutoResponse)UnderlyingEnum)
			{
				case AutoResponse.None:                return (None_string);
				case AutoResponse.Trigger:             return (Trigger_string);
				case AutoResponse.SendText:            return (SendText_string);
				case AutoResponse.SendFile:            return (SendFile_string);
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
				case AutoResponse.Explicit:            return (this.explicitCommandString);

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

				if ((AutoResponse)UnderlyingEnum == AutoResponse.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitCommandString != null ? this.explicitCommandString.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as AutoResponseEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(AutoResponseEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((AutoResponse)UnderlyingEnum == AutoResponse.Explicit)
			{
				return
				(
					base.Equals(other) &&
					StringEx.EqualsOrdinal(this.explicitCommandString, other.explicitCommandString)
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
		public static bool operator ==(AutoResponseEx lhs, AutoResponseEx rhs)
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
		public static bool operator !=(AutoResponseEx lhs, AutoResponseEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoResponseEx[] GetAllItems()
		{
			return (GetItems(true, true));
		}

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoResponseEx[] GetFixedItems()
		{
			return (GetItems(true, false));
		}

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		private static AutoResponseEx[] GetItems(bool addFixed, bool addVariable)
		{
			var a = new List<AutoResponseEx>(16); // Preset the required capacity to improve memory management; 16 is the number of items.

			if (addFixed)    a.Add(new AutoResponseEx(AutoResponse.None));
			if (addFixed)    a.Add(new AutoResponseEx(AutoResponse.Trigger));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.SendText));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.SendFile));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand1));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand2));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand3));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand4));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand5));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand6));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand7));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand8));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand9));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand10));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand11));
			if (addVariable) a.Add(new AutoResponseEx(AutoResponse.PredefinedCommand12));

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
		public static AutoResponseEx Parse(string s)
		{
			AutoResponseEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid response string! String must be a valid response string, or one of the predefined responses."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoResponseEx result)
		{
			AutoResponse enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				if (enumResult != AutoResponse.Explicit)
					result = new AutoResponseEx(enumResult);
				else
					result = new AutoResponseEx(s);

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
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = AutoResponse.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = AutoResponse.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Trigger_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Trigger_stringAlternatives))
			{
				result = AutoResponse.Trigger;
				return (true);
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
			else if (StringEx.StartsWithOrdinalIgnoreCase   (s, PredefinedCommand_string) ||
			         StringEx.StartsWithAnyOrdinalIgnoreCase(s, PredefinedCommand_stringAlternatives)) // StartWith()! Not Equals()!
			{
				var match = Regex.Match(s, @"\d+");
				if (match.Success)
				{
					int intValue;
					if (int.TryParse(match.Groups[0].Value, out intValue))
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
			else // Explicit!
			{
				result = AutoResponse.Explicit;
				return (true);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
