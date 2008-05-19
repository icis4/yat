using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Types;

namespace MKY.IO.Serial
{
	#region Enum FlowControl

	/// <summary></summary>
	/// <remarks>
	/// I think flow control is the better identifier, no clue why .NET uses the
	/// term flowControl.
	/// </remarks>
	public enum FlowControl
	{
		/// <summary></summary>
		None = System.IO.Ports.Handshake.None,
		/// <summary></summary>
		RequestToSend = System.IO.Ports.Handshake.RequestToSend,
		/// <summary></summary>
		XOnXOff = System.IO.Ports.Handshake.XOnXOff,
		/// <summary></summary>
		RequestToSendXOnXOff = System.IO.Ports.Handshake.RequestToSendXOnXOff,
		/// <summary></summary>
		Manual,
		/// <summary></summary>
		RS485,
	}

	#endregion

	/// <summary></summary>
	[Serializable]
	public class XFlowControl : MKY.IO.Ports.XHandshake
	{
		#region String Definitions

		private const string Manual_string = "Manual";
		private const string Manual_stringShort = "Manual";
		private const string RS485_string = "RS-485 Transceiver Control";
		private const string RS485_stringShort = "RS-485";

		#endregion

		/// <summary>Default is <see cref="FlowControl.None"/></summary>
		public XFlowControl()
			: base((System.IO.Ports.Handshake)FlowControl.None)
		{
		}

		/// <summary></summary>
		protected XFlowControl(FlowControl flowControl)
			: base((System.IO.Ports.Handshake)flowControl)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((FlowControl)UnderlyingEnum)
			{
				case FlowControl.Manual: return (Manual_string);
				case FlowControl.RS485:  return (RS485_string);
				default:                 return (base.ToString());
			}
		}

		/// <summary></summary>
		public new string ToShortString()
		{
			switch ((FlowControl)UnderlyingEnum)
			{
				case FlowControl.Manual: return (Manual_stringShort);
				case FlowControl.RS485:  return (RS485_stringShort);
				default:                 return (base.ToShortString());
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public new static XFlowControl[] GetItems()
		{
			List<XFlowControl> a = new List<XFlowControl>();
			a.Add(new XFlowControl(FlowControl.None));
			a.Add(new XFlowControl(FlowControl.RequestToSend));
			a.Add(new XFlowControl(FlowControl.XOnXOff));
			a.Add(new XFlowControl(FlowControl.RequestToSendXOnXOff));
			a.Add(new XFlowControl(FlowControl.Manual));
			a.Add(new XFlowControl(FlowControl.RS485));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public new static XFlowControl Parse(string flowControl)
		{
			if      ((string.Compare(flowControl, Manual_string, true) == 0) ||
					 (string.Compare(flowControl, Manual_stringShort, true) == 0))
			{
				return (new XFlowControl(FlowControl.Manual));
			}
			else if ((string.Compare(flowControl, RS485_string, true) == 0) ||
					 (string.Compare(flowControl, RS485_stringShort, true) == 0))
			{
				return (new XFlowControl(FlowControl.RS485));
			}
			else
			{
				return ((XFlowControl)MKY.IO.Ports.XHandshake.Parse(flowControl));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator FlowControl(XFlowControl flowControl)
		{
			return ((FlowControl)flowControl.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XFlowControl(FlowControl flowControl)
		{
			return (new XFlowControl(flowControl));
		}

		/// <summary></summary>
		public static implicit operator int(XFlowControl flowControl)
		{
			return (flowControl.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XFlowControl(int flowControl)
		{
			return (new XFlowControl((FlowControl)flowControl));
		}

		/// <summary></summary>
		public static implicit operator string(XFlowControl flowControl)
		{
			return (flowControl.ToString());
		}

		/// <summary></summary>
		public static implicit operator XFlowControl(string flowControl)
		{
			return (Parse(flowControl));
		}

		/// <summary></summary>
		public static implicit operator System.IO.Ports.Handshake(XFlowControl flowControl)
		{
			switch ((FlowControl)flowControl.UnderlyingEnum)
			{
				case FlowControl.None:
				case FlowControl.RequestToSend:
				case FlowControl.XOnXOff:
				case FlowControl.RequestToSendXOnXOff:
					return ((System.IO.Ports.Handshake)(FlowControl)flowControl);

				case FlowControl.RS485:
				case FlowControl.Manual:
				default:
					return (System.IO.Ports.Handshake.None);
			}
		}

		/// <summary></summary>
		public static implicit operator XFlowControl(System.IO.Ports.Handshake flowControl)
		{
			return (new XFlowControl((FlowControl)flowControl));
		}

		#endregion
	}
}
