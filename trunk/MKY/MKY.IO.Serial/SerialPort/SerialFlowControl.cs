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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.Types;

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
	public class XSerialFlowControl : MKY.IO.Ports.HandshakeEx
	{
		#region String Definitions

		private const string Manual_string = "Manual";
		private const string Manual_stringShort = "Manual";
		private const string RS485_string = "RS-485 Transceiver Control";
		private const string RS485_stringShort = "RS-485";

		#endregion

		/// <summary>Default is <see cref="SerialFlowControl.None"/>.</summary>
		public XSerialFlowControl()
			: base((System.IO.Ports.Handshake)SerialFlowControl.None)
		{
		}

		/// <summary></summary>
		protected XSerialFlowControl(SerialFlowControl flowControl)
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
				default:                 return (base.ToString());
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
		public static new XSerialFlowControl[] GetItems()
		{
			List<XSerialFlowControl> a = new List<XSerialFlowControl>();
			a.Add(new XSerialFlowControl(SerialFlowControl.None));
			a.Add(new XSerialFlowControl(SerialFlowControl.RequestToSend));
			a.Add(new XSerialFlowControl(SerialFlowControl.XOnXOff));
			a.Add(new XSerialFlowControl(SerialFlowControl.RequestToSendXOnXOff));
			a.Add(new XSerialFlowControl(SerialFlowControl.Manual));
			a.Add(new XSerialFlowControl(SerialFlowControl.RS485));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static new XSerialFlowControl Parse(string flowControl)
		{
			if      ((string.Compare(flowControl, Manual_string, StringComparison.OrdinalIgnoreCase) == 0) ||
			         (string.Compare(flowControl, Manual_stringShort, StringComparison.OrdinalIgnoreCase) == 0))
			{
				return (new XSerialFlowControl(SerialFlowControl.Manual));
			}
			else if ((string.Compare(flowControl, RS485_string, StringComparison.OrdinalIgnoreCase) == 0) ||
			         (string.Compare(flowControl, RS485_stringShort, StringComparison.OrdinalIgnoreCase) == 0))
			{
				return (new XSerialFlowControl(SerialFlowControl.RS485));
			}
			else
			{
				return ((XSerialFlowControl)MKY.IO.Ports.HandshakeEx.Parse(flowControl));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SerialFlowControl(XSerialFlowControl flowControl)
		{
			return ((SerialFlowControl)flowControl.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XSerialFlowControl(SerialFlowControl flowControl)
		{
			return (new XSerialFlowControl(flowControl));
		}

		/// <summary></summary>
		public static implicit operator int(XSerialFlowControl flowControl)
		{
			return (flowControl.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XSerialFlowControl(int flowControl)
		{
			return (new XSerialFlowControl((SerialFlowControl)flowControl));
		}

		/// <summary></summary>
		public static implicit operator string(XSerialFlowControl flowControl)
		{
			return (flowControl.ToString());
		}

		/// <summary></summary>
		public static implicit operator XSerialFlowControl(string flowControl)
		{
			return (Parse(flowControl));
		}

		/// <summary></summary>
		public static implicit operator System.IO.Ports.Handshake(XSerialFlowControl flowControl)
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
		public static implicit operator XSerialFlowControl(System.IO.Ports.Handshake flowControl)
		{
			return (new XSerialFlowControl((SerialFlowControl)flowControl));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
