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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Serial.SerialPort
{
	#region Enum SerialControlPinState

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SerialControlPinState
	{
		Automatic = -1,
		Disabled = 0,
		Enabled = 1
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class SerialControlPinStateEx : EnumEx
	{
		#region String Definitions

		private const string             Automatic_string = "Automatic";
		private static readonly string[] Automatic_stringAlternatives = new string[] { "Auto" };

		private const string             Disabled_string = "Disabled";
		private static readonly string[] Disabled_stringAlternatives = new string[] { "Off", "False" };

		private const string             Enabled_string = "Enabled";
		private static readonly string[] Enabled_stringAlternatives = new string[] { "On", "True" };

		#endregion

		/// <summary>Default is <see cref="SerialControlPinState.Automatic"/>.</summary>
		public const SerialControlPinState Default = SerialControlPinState.Automatic;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SerialControlPinStateEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SerialControlPinStateEx(SerialControlPinState pinState)
			: base(pinState)
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
			switch ((SerialControlPinState)UnderlyingEnum)
			{
				case SerialControlPinState.Automatic: return (Automatic_string);
				case SerialControlPinState.Disabled:  return (Disabled_string);
				case SerialControlPinState.Enabled:   return (Enabled_string);

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
		public static SerialControlPinStateEx[] GetItems()
		{
			var a = new List<SerialControlPinStateEx>(3); // Preset the required capacity to improve memory management.

			a.Add(new SerialControlPinStateEx(SerialControlPinState.Automatic));
			a.Add(new SerialControlPinStateEx(SerialControlPinState.Disabled));
			a.Add(new SerialControlPinStateEx(SerialControlPinState.Enabled));

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
		public static SerialControlPinStateEx Parse(string s)
		{
			SerialControlPinStateEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid serial control pin state string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialControlPinStateEx result)
		{
			SerialControlPinState enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new SerialControlPinStateEx(enumResult);
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
		public static bool TryParse(string s, out SerialControlPinState result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, Automatic_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Automatic_stringAlternatives))
			{
				result = SerialControlPinState.Automatic;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Disabled_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Disabled_stringAlternatives))
			{
				result = SerialControlPinState.Disabled;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Enabled_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Enabled_stringAlternatives))
			{
				result = SerialControlPinState.Enabled;
				return (true);
			}
			else // Invalid string!
			{
				result = new SerialControlPinStateEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator SerialControlPinState(SerialControlPinStateEx pinState)
		{
			return ((SerialControlPinState)pinState.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialControlPinStateEx(SerialControlPinState pinState)
		{
			return (new SerialControlPinStateEx(pinState));
		}

		/// <summary></summary>
		public static implicit operator int(SerialControlPinStateEx pinState)
		{
			return (pinState.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialControlPinStateEx(int pinState)
		{
			return (new SerialControlPinStateEx((SerialControlPinState)pinState));
		}

		/// <summary></summary>
		public static implicit operator string(SerialControlPinStateEx pinState)
		{
			return (pinState.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialControlPinStateEx(string pinState)
		{
			return (Parse(pinState));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
