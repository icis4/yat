using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Types;

namespace MKY.YAT.Domain.IO
{
	#region Enum Handshake

	/// <summary></summary>
	public enum Handshake
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
	public class XHandshake : MKY.IO.Ports.XHandshake
	{
		#region String Definitions

		private const string Manual_string = "Manual";
		private const string Manual_stringShort = "Manual";
		private const string RS485_string = "RS-485 Transceiver Control";
		private const string RS485_stringShort = "RS-485";

		#endregion

		/// <summary>Default is <see cref="Handshake.None"/></summary>
		public XHandshake()
			: base((System.IO.Ports.Handshake)Handshake.None)
		{
		}

		/// <summary></summary>
		protected XHandshake(Handshake handshake)
			: base((System.IO.Ports.Handshake)handshake)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.Manual: return (Manual_string);
				case Handshake.RS485:  return (RS485_string);
				default:               return (base.ToString());
			}
		}

		/// <summary></summary>
		public new string ToShortString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.Manual: return (Manual_stringShort);
				case Handshake.RS485:  return (RS485_stringShort);
				default:               return (base.ToShortString());
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public new static XHandshake[] GetItems()
		{
			List<XHandshake> a = new List<XHandshake>();
			a.Add(new XHandshake(Handshake.None));
			a.Add(new XHandshake(Handshake.RequestToSend));
			a.Add(new XHandshake(Handshake.XOnXOff));
			a.Add(new XHandshake(Handshake.RequestToSendXOnXOff));
			a.Add(new XHandshake(Handshake.Manual));
			a.Add(new XHandshake(Handshake.RS485));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public new static XHandshake Parse(string handshake)
		{
			if      ((string.Compare(handshake, Manual_string, true) == 0) ||
					 (string.Compare(handshake, Manual_stringShort, true) == 0))
			{
				return (new XHandshake(Handshake.Manual));
			}
			else if ((string.Compare(handshake, RS485_string, true) == 0) ||
					 (string.Compare(handshake, RS485_stringShort, true) == 0))
			{
				return (new XHandshake(Handshake.RS485));
			}
			else
			{
				return ((XHandshake)MKY.IO.Ports.XHandshake.Parse(handshake));
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Handshake(XHandshake handshake)
		{
			return ((Handshake)handshake.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XHandshake(Handshake handshake)
		{
			return (new XHandshake(handshake));
		}

		/// <summary></summary>
		public static implicit operator int(XHandshake handshake)
		{
			return (handshake.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XHandshake(int handshake)
		{
			return (new XHandshake((Handshake)handshake));
		}

		/// <summary></summary>
		public static implicit operator string(XHandshake handshake)
		{
			return (handshake.ToString());
		}

		/// <summary></summary>
		public static implicit operator XHandshake(string handshake)
		{
			return (Parse(handshake));
		}

		/// <summary></summary>
		public static implicit operator System.IO.Ports.Handshake(XHandshake handshake)
		{
			switch ((Handshake)handshake.UnderlyingEnum)
			{
				case Handshake.None:
				case Handshake.RequestToSend:
				case Handshake.XOnXOff:
				case Handshake.RequestToSendXOnXOff:
					return ((System.IO.Ports.Handshake)(Handshake)handshake);

				case Handshake.RS485:
				case Handshake.Manual:
				default:
					return (System.IO.Ports.Handshake.None);
			}
		}

		/// <summary></summary>
		public static implicit operator XHandshake(System.IO.Ports.Handshake handshake)
		{
			return (new XHandshake((Handshake)handshake));
		}

		#endregion
	}
}
