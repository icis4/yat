//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\SerialPort for better separation of the implementation files.
namespace MKY.IO.Serial
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
		None                 = System.IO.Ports.Handshake.None,
		RequestToSend        = System.IO.Ports.Handshake.RequestToSend,
		XOnXOff              = System.IO.Ports.Handshake.XOnXOff,
		RequestToSendXOnXOff = System.IO.Ports.Handshake.RequestToSendXOnXOff,
		Manual,
		RS485,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary></summary>
	[Serializable]
	public class SerialFlowControlEx : MKY.IO.Ports.HandshakeEx
	{
		#region String Definitions

		private const string Manual_string = "Manual";
		private const string Manual_stringShort = "Manual";
		private const string RS485_string = "RS-485 Transceiver Control";
		private const string RS485_stringShort = "RS-485";

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
				case SerialFlowControl.Manual: return (Manual_string);
				case SerialFlowControl.RS485:  return (RS485_string);
				default:                       return (base.ToString());
			}
		}

		/// <summary></summary>
		public override string ToShortString()
		{
			switch ((SerialFlowControl)UnderlyingEnum)
			{
				case SerialFlowControl.Manual: return (Manual_stringShort);
				case SerialFlowControl.RS485:  return (RS485_stringShort);
				default:                 return (base.ToShortString());
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static new SerialFlowControlEx[] GetItems()
		{
			List<SerialFlowControlEx> a = new List<SerialFlowControlEx>();
			a.Add(new SerialFlowControlEx(SerialFlowControl.None));
			a.Add(new SerialFlowControlEx(SerialFlowControl.RequestToSend));
			a.Add(new SerialFlowControlEx(SerialFlowControl.XOnXOff));
			a.Add(new SerialFlowControlEx(SerialFlowControl.RequestToSendXOnXOff));
			a.Add(new SerialFlowControlEx(SerialFlowControl.Manual));
			a.Add(new SerialFlowControlEx(SerialFlowControl.RS485));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static new SerialFlowControlEx Parse(string flowControl)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(flowControl, Manual_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(flowControl, Manual_stringShort))
			{
				return (new SerialFlowControlEx(SerialFlowControl.Manual));
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(flowControl, RS485_string) ||
			         StringEx.EqualsOrdinalIgnoreCase(flowControl, RS485_stringShort))
			{
				return (new SerialFlowControlEx(SerialFlowControl.RS485));
			}
			else
			{
				return ((SerialFlowControlEx)MKY.IO.Ports.HandshakeEx.Parse(flowControl));
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
				case SerialFlowControl.RequestToSend:
				case SerialFlowControl.XOnXOff:
				case SerialFlowControl.RequestToSendXOnXOff:
					return ((System.IO.Ports.Handshake)(SerialFlowControl)flowControl);

				case SerialFlowControl.RS485:
				case SerialFlowControl.Manual:
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
