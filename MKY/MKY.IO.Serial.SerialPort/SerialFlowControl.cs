//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2015 Matthias Kläy.
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

		RS485,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class SerialFlowControlEx : MKY.IO.Ports.HandshakeEx
	{
		#region String Definitions

		private const string             ManualHardware_string = "Manual Hardware (RFR/CTS)";
		private const string             ManualHardware_stringShort = "Manual RFR/CTS";
		private static readonly string[] ManualHardware_stringAlternatives = new string[] { "Manual Hardware", "ManualHardware", "Manual Hard", "ManualHard", "Manual HW", "ManualHW", "MHW", "MH" };

		private const string             ManualSoftware_string = "Manual Software (XOn/XOff)";
		private const string             ManualSoftware_stringShort = "Manual XOn/XOff";
		private static readonly string[] ManualSoftware_stringAlternatives = new string[] { "Manual Software", "ManualSoftware", "Manual Soft", "ManualSoft", "Manual SW", "ManualSW", "MSW", "MS" };

		private const string             ManualCombined_string = "Manual Combined (RFR/CTS + XOn/XOff)";
		private const string             ManualCombined_stringShort = "Manual RFR/CTS + XOn/XOff";
		private static readonly string[] ManualCombined_stringAlternatives = new string[] { "Manual Combined", "ManualCombined", "Manual Combi", "ManualCombi", "Manual C", "ManualC", "MHW", "MH" };

		private const string             RS485_string = "RS-485 Transceiver Control";
		private const string             RS485_stringShort = "RS-485";
		private static readonly string[] RS485_stringAlternatives = new string[] { "RS485", "485" };

		#endregion

		/// <summary>Default is <see cref="SerialFlowControl.None"/>.</summary>
		public SerialFlowControlEx()
			: base((System.IO.Ports.Handshake)SerialFlowControl.None)
		{
		}

		/// <summary></summary>
		protected SerialFlowControlEx(SerialFlowControl flowControl)
			: base((System.IO.Ports.Handshake)flowControl)
		{
		}

		#region ToString

		/// <summary></summary>
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

		/// <summary></summary>
		public static new SerialFlowControlEx[] GetItems()
		{
			List<SerialFlowControlEx> a = new List<SerialFlowControlEx>();
			a.Add(new SerialFlowControlEx(SerialFlowControl.None));
			a.Add(new SerialFlowControlEx(SerialFlowControl.Hardware));
			a.Add(new SerialFlowControlEx(SerialFlowControl.Software));
			a.Add(new SerialFlowControlEx(SerialFlowControl.Combined));
			a.Add(new SerialFlowControlEx(SerialFlowControl.ManualHardware));
			a.Add(new SerialFlowControlEx(SerialFlowControl.ManualSoftware));
			a.Add(new SerialFlowControlEx(SerialFlowControl.ManualCombined));
			a.Add(new SerialFlowControlEx(SerialFlowControl.RS485));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static new SerialFlowControlEx Parse(string s)
		{
			SerialFlowControlEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid serial flow control string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialFlowControlEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase   (s, ManualHardware_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualHardware_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualHardware_stringAlternatives))
			{
				result = new SerialFlowControlEx(SerialFlowControl.ManualHardware);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualSoftware_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualSoftware_stringAlternatives))
			{
				result = new SerialFlowControlEx(SerialFlowControl.ManualSoftware);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ManualCombined_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, ManualCombined_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ManualCombined_stringAlternatives))
			{
				result = new SerialFlowControlEx(SerialFlowControl.ManualCombined);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, RS485_string) ||
			         StringEx.EqualsOrdinalIgnoreCase   (s, RS485_stringShort) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, RS485_stringAlternatives))
			{
				result = new SerialFlowControlEx(SerialFlowControl.RS485);
				return (true);
			}
			else
			{
				Ports.HandshakeEx handshake;
				if (Ports.HandshakeEx.TryParse(s, out handshake))
				{
					result = new SerialFlowControlEx((SerialFlowControl)(System.IO.Ports.Handshake)handshake);
					return (true);
				}
				else
				{
					result = null;
					return (false);
				}
			}
		}

		#endregion

		#region Conversion Operators

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
