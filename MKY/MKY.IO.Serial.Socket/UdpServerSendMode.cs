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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Serial.Socket
{
	#region Enum UdpServerSendMode

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum UdpServerSendMode
	{
		None  = 0,
		First = 1,
		MostRecent
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum UdpServerSendModeEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A type shall spell 'Udp' like this...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class UdpServerSendModeEx : EnumEx
	{
		#region String Definitions

		private const string None_string       = "None";
		private const string First_string      = "First";
		private const string MostRecent_string = "Most Recent";

		#endregion

		/// <summary>Default is <see cref="UdpServerSendMode.MostRecent"/>.</summary>
		public const UdpServerSendMode Default = UdpServerSendMode.MostRecent;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public UdpServerSendModeEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public UdpServerSendModeEx(UdpServerSendMode type)
			: base(type)
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
			switch ((UdpServerSendMode)UnderlyingEnum)
			{
				case UdpServerSendMode.None:       return (None_string);
				case UdpServerSendMode.First:      return (First_string);
				case UdpServerSendMode.MostRecent: return (MostRecent_string);

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
		public static UdpServerSendModeEx[] GetItems()
		{
			var a = new List<UdpServerSendModeEx>(3); // Preset the required capacity to improve memory management.

			a.Add(new UdpServerSendModeEx(UdpServerSendMode.None));
			a.Add(new UdpServerSendModeEx(UdpServerSendMode.First));
			a.Add(new UdpServerSendModeEx(UdpServerSendMode.MostRecent));

			return (a.ToArray());
		}

		#endregion

		#region Parse/From
		//==========================================================================================
		// Parse/From
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static UdpServerSendModeEx Parse(string s)
		{
			UdpServerSendModeEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid socket host type string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out UdpServerSendModeEx result)
		{
			UdpServerSendMode enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new UdpServerSendModeEx(enumResult);
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
		public static bool TryParse(string s, out UdpServerSendMode result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = UdpServerSendMode.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = UdpServerSendMode.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, First_string))
			{
				result = UdpServerSendMode.First;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, MostRecent_string))
			{
				result = UdpServerSendMode.MostRecent;
				return (true);
			}
			else // Invalid string!
			{
				result = new UdpServerSendModeEx(); // Default!
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int sendMode, out UdpServerSendModeEx result)
		{
			if (IsValidSendMode(sendMode))
			{
				result = sendMode;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int sendMode, out UdpServerSendMode result)
		{
			if (IsValidSendMode(sendMode))
			{
				result = (UdpServerSendModeEx)sendMode;
				return (true);
			}
			else
			{
				result = new UdpServerSendModeEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static bool IsValidSendMode(int sendMode)
		{
			if (sendMode == (int)UdpServerSendMode.None)       return (true);
			if (sendMode == (int)UdpServerSendMode.First)      return (true);
			if (sendMode == (int)UdpServerSendMode.MostRecent) return (true);

			return (false);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator UdpServerSendMode(UdpServerSendModeEx type)
		{
			return ((UdpServerSendMode)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator UdpServerSendModeEx(UdpServerSendMode type)
		{
			return (new UdpServerSendModeEx(type));
		}

		/// <summary></summary>
		public static implicit operator int(UdpServerSendModeEx type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator UdpServerSendModeEx(int type)
		{
			return (new UdpServerSendModeEx((UdpServerSendMode)type));
		}

		/// <summary></summary>
		public static implicit operator string(UdpServerSendModeEx type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator UdpServerSendModeEx(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
