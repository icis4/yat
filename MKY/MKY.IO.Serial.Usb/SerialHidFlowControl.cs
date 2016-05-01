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

namespace MKY.IO.Serial.Usb
{
	/// <summary></summary>
	public enum SerialHidFlowControl
	{
		/// <summary>
		/// No flow control, fully relying on underlying USB flow control.
		/// </summary>
		None,

		/// <summary>
		/// Automatically managing XOn/XOff flow control, useful with devices that provide XOn/XOff
		/// for USB Ser/HID, e.g. MT Ser/HID devices.
		/// </summary>
		/// <remarks>
		/// This USB Ser/HID host implementation so far never sends an XOff, since the buffers are
		/// dynamically managed and increased if necessary. Received XOff will be taken into account
		/// though.
		/// </remarks>
		Software,

		/// <summary>
		/// Manually managing XOn/XOff flow control, mainly useful for development of devices that
		/// provide XOn/XOff for USB Ser/HID, e.g. MT Ser/HID devices.
		/// </summary>
		ManualSoftware,
	}

	/// <summary>
	/// Extended enum SerialHidFlowControlEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class SerialHidFlowControlEx : EnumEx
	{
		#region String Definitions

		private const string             None_string = "None";
		private static readonly string[] None_stringAlternatives = new string[] { "N" };

		private const string             Software_string = "Software (XOn/XOff)";
		private const string             Software_stringShort = "Software";
		private static readonly string[] Software_stringAlternatives = new string[] { "Software", "Soft", "SW", "S" };

		private const string             ManualSoftware_string = "Manual Software (XOn/XOff)";
		private const string             ManualSoftware_stringShort = "Manual Software";
		private static readonly string[] ManualSoftware_stringAlternatives = new string[] { "Manual Software", "ManualSoftware", "Manual Soft", "ManualSoft", "Manual SW", "ManualSW", "MSW", "MS" };

		#endregion

		/// <summary>Default is <see cref="SerialHidFlowControl.None"/>.</summary>
		public SerialHidFlowControlEx()
			: this(SerialHidFlowControl.None)
		{
		}

		/// <summary></summary>
		public SerialHidFlowControlEx(SerialHidFlowControl flowControl)
			: base(flowControl)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((SerialHidFlowControl)UnderlyingEnum)
			{
				case SerialHidFlowControl.None:           return (None_string);
				case SerialHidFlowControl.Software:       return (Software_string);
				case SerialHidFlowControl.ManualSoftware: return (ManualSoftware_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public string ToShortString()
		{
			switch ((SerialHidFlowControl)UnderlyingEnum)
			{
				case SerialHidFlowControl.None:           return (None_string);
				case SerialHidFlowControl.Software:       return (Software_stringShort);
				case SerialHidFlowControl.ManualSoftware: return (ManualSoftware_stringShort);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static SerialHidFlowControlEx[] GetItems()
		{
			List<SerialHidFlowControlEx> a = new List<SerialHidFlowControlEx>(3); // Preset the required capactiy to improve memory management.
			a.Add(new SerialHidFlowControlEx(SerialHidFlowControl.None));
			a.Add(new SerialHidFlowControlEx(SerialHidFlowControl.Software));
			a.Add(new SerialHidFlowControlEx(SerialHidFlowControl.ManualSoftware));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialHidFlowControlEx Parse(string s)
		{
			SerialHidFlowControlEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid serial flow control string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialHidFlowControlEx result)
		{
			SerialHidFlowControl enumResult;
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
		public static bool TryParse(string s, out SerialHidFlowControl result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = SerialHidFlowControl.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Software_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, Software_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Software_stringAlternatives))
			{
				result = SerialHidFlowControl.Software;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualSoftware_stringAlternatives))
			{
				result = SerialHidFlowControl.ManualSoftware;
				return (true);
			}
			else if (string.IsNullOrEmpty(s))
			{
				result = new SerialHidFlowControlEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else // Invalid string!
			{
				result = new SerialHidFlowControlEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SerialHidFlowControl(SerialHidFlowControlEx flowControl)
		{
			return ((SerialHidFlowControl)flowControl.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlEx(SerialHidFlowControl flowControl)
		{
			return (new SerialHidFlowControlEx(flowControl));
		}

		/// <summary></summary>
		public static implicit operator int(SerialHidFlowControlEx flowControl)
		{
			return (flowControl.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlEx(int flowControl)
		{
			return (new SerialHidFlowControlEx((SerialHidFlowControl)flowControl));
		}

		/// <summary></summary>
		public static implicit operator string(SerialHidFlowControlEx flowControl)
		{
			return (flowControl.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlEx(string flowControl)
		{
			return (Parse(flowControl));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
