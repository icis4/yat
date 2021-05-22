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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	#region Enum SerialFlowControl

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <remarks>
	/// I think flow control is the better identifier, no clue why .NET uses the term handshake.
	/// </remarks>
	public enum SerialFlowControl
	{
		None     = System.IO.Ports.Handshake.None,

		Hardware = System.IO.Ports.Handshake.RequestToSend,
		Software = System.IO.Ports.Handshake.XOnXOff,
		Combined = System.IO.Ports.Handshake.RequestToSendXOnXOff,

		ManualHardware,
		ManualSoftware,
		ManualCombined,

		RS485
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SerialFlowControlEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class SerialFlowControlEx : MKY.IO.Ports.HandshakeEx
	{
		#region String Definitions

		private const string             ManualHardware_string = "Manual Hardware (RTS/CTS + DTR/DSR)";
		private const string             ManualHardware_stringShort = "Manual Hardware";
		private static readonly string[] ManualHardware_stringAlternatives = new string[] { "Manual Hardware", "ManualHardware", "Manual Hard", "ManualHard", "Manual HW", "ManualHW", "MHW", "MH" };

		private const string             ManualSoftware_string = "Manual Software (XOn/XOff)";
		private const string             ManualSoftware_stringShort = "Manual Software";
		private static readonly string[] ManualSoftware_stringAlternatives = new string[] { "Manual Software", "ManualSoftware", "Manual Soft", "ManualSoft", "Manual SW", "ManualSW", "MSW", "MS" };

		private const string             ManualCombined_string = "Manual Combined (RTS/CTS + DTR/DSR + XOn/XOff)";
		private const string             ManualCombined_stringShort = "Manual Combined";
		private static readonly string[] ManualCombined_stringAlternatives = new string[] { "Manual Combined", "ManualCombined", "Manual Combi", "ManualCombi", "Manual C", "ManualC", "MHW", "MH" };

		private const string             RS485_string = "RS-485 Transceiver Control";
		private const string             RS485_stringShort = "RS-485";
		private static readonly string[] RS485_stringAlternatives = new string[] { "RS485", "485" };

		#endregion

		/// <summary>Default is <see cref="SerialFlowControl.None"/>.</summary>
		public new const SerialFlowControl Default = SerialFlowControl.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SerialFlowControlEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SerialFlowControlEx(SerialFlowControl flowControl)
			: base((System.IO.Ports.Handshake)flowControl)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			switch ((SerialFlowControl)UnderlyingEnum)
			{
				case SerialFlowControl.ManualHardware: return (ManualHardware_string);
				case SerialFlowControl.ManualSoftware: return (ManualSoftware_string);
				case SerialFlowControl.ManualCombined: return (ManualCombined_string);
				case SerialFlowControl.RS485:          return (RS485_string);
				default:                               return (base.ToString());
			}
		}

		/// <summary></summary>
		public override string ToShortString()
		{
			switch ((SerialFlowControl)UnderlyingEnum)
			{
				case SerialFlowControl.ManualHardware: return (ManualHardware_stringShort);
				case SerialFlowControl.ManualSoftware: return (ManualSoftware_stringShort);
				case SerialFlowControl.ManualCombined: return (ManualCombined_stringShort);
				case SerialFlowControl.RS485:          return (RS485_stringShort);
				default:                               return (base.ToShortString());
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
		public static new SerialFlowControlEx[] GetItems()
		{
			var a = new List<SerialFlowControlEx>(8) // Preset the required capacity to improve memory management.
			{
				new SerialFlowControlEx(SerialFlowControl.None),
				new SerialFlowControlEx(SerialFlowControl.Hardware),
				new SerialFlowControlEx(SerialFlowControl.Software),
				new SerialFlowControlEx(SerialFlowControl.Combined),
				new SerialFlowControlEx(SerialFlowControl.ManualHardware),
				new SerialFlowControlEx(SerialFlowControl.ManualSoftware),
				new SerialFlowControlEx(SerialFlowControl.ManualCombined),
				new SerialFlowControlEx(SerialFlowControl.RS485)
			};
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
		public static new SerialFlowControlEx Parse(string s)
		{
			SerialFlowControlEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid serial flow control string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialFlowControlEx result)
		{
			SerialFlowControl enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new SerialFlowControlEx(enumResult);
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
		public static bool TryParse(string s, out SerialFlowControl result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, ManualHardware_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualHardware_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualHardware_stringAlternatives))
			{
				result = SerialFlowControl.ManualHardware;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualSoftware_stringAlternatives))
			{
				result = SerialFlowControl.ManualSoftware;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ManualCombined_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualCombined_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualCombined_stringAlternatives))
			{
				result = SerialFlowControl.ManualCombined;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, RS485_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, RS485_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, RS485_stringAlternatives))
			{
				result = SerialFlowControl.RS485;
				return (true);
			}
			else
			{
				System.IO.Ports.Handshake handshake;
				if (TryParse(s, out handshake))
				{
					result = new SerialFlowControlEx((SerialFlowControlEx)handshake);
					return (true);
				}
				else // Invalid string!
				{
					result = new SerialFlowControlEx(); // Default!
					return (false);
				}
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int flowControl, out SerialFlowControlEx result)
		{
			if (IsDefined(flowControl))
			{
				result = flowControl;
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
		public static bool TryFrom(int flowControl, out SerialFlowControl result)
		{
			if (IsDefined(flowControl))
			{
				result = (SerialFlowControlEx)flowControl;
				return (true);
			}
			else
			{
				result = new SerialFlowControlEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static bool IsDefined(int flowControl)
		{
			return (IsDefined(typeof(SerialFlowControlEx), flowControl));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator SerialFlowControl(SerialFlowControlEx flowControl)
		{
			return ((SerialFlowControl)flowControl.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialFlowControlEx(SerialFlowControl flowControl)
		{
			return (new SerialFlowControlEx(flowControl));
		}

		/// <summary></summary>
		public static implicit operator int(SerialFlowControlEx flowControl)
		{
			return (flowControl.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialFlowControlEx(int flowControl)
		{
			return (new SerialFlowControlEx((SerialFlowControl)flowControl));
		}

		/// <summary></summary>
		public static implicit operator string(SerialFlowControlEx flowControl)
		{
			return (flowControl.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialFlowControlEx(string flowControl)
		{
			return (Parse(flowControl));
		}

		/// <summary></summary>
		public static implicit operator System.IO.Ports.Handshake(SerialFlowControlEx flowControl)
		{
			switch ((SerialFlowControl)flowControl.UnderlyingEnum)
			{
				case SerialFlowControl.None:
				case SerialFlowControl.Hardware:
				case SerialFlowControl.Software:
				case SerialFlowControl.Combined:
					return ((System.IO.Ports.Handshake)(SerialFlowControl)flowControl);

				case SerialFlowControl.ManualHardware:
				case SerialFlowControl.ManualSoftware:
				case SerialFlowControl.ManualCombined:
				case SerialFlowControl.RS485:
				default:
					return (System.IO.Ports.Handshake.None);
			}
		}

		/// <summary></summary>
		public static implicit operator SerialFlowControlEx(System.IO.Ports.Handshake flowControl)
		{
			return (new SerialFlowControlEx((SerialFlowControl)flowControl));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
