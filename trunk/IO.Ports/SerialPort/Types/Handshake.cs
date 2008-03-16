using System;
using System.Collections.Generic;
using System.IO.Ports;

using MKY.Utilities.Types;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Extended enum XHandshake.
	/// </summary>
	/// <remarks>
	/// I think flow control would the better identifier, no clue why .NET uses the
	/// term handshake.
	/// </remarks>
	[Serializable]
	public class XHandshake : XEnum
	{
		#region String Definitions

		private const string None_string = "None";
		private const string None_stringShort = "None";
		private const string RtsCts_string = "Hardware (RTS/CTS)";
		private const string RtsCts_stringShort = "RTS/CTS";
		private const string XOnXOff_string = "Software (XOn/XOff)";
		private const string XOnXOff_stringShort = "XOn/XOff";
		private const string RtsCtsXOnXOff_string = "Combined (RTS/CTS and XOn/XOff)";
		private const string RtsCtsXOnXOff_stringShort = "RTS/CTS + XOn/XOff";

		#endregion

		/// <summary>Default is <see cref="Handshake.None"/></summary>
		public XHandshake()
			: base(Handshake.None)
		{
		}

		/// <summary></summary>
		protected XHandshake(Handshake handshake)
			: base(handshake)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_string);
				case Handshake.RequestToSend:        return (RtsCts_string);
				case Handshake.XOnXOff:              return (XOnXOff_string);
				case Handshake.RequestToSendXOnXOff: return (RtsCtsXOnXOff_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		/// <summary></summary>
		public string ToShortString()
		{
			switch ((Handshake)UnderlyingEnum)
			{
				case Handshake.None:                 return (None_stringShort);
				case Handshake.RequestToSend:        return (RtsCts_stringShort);
				case Handshake.XOnXOff:              return (XOnXOff_stringShort);
				case Handshake.RequestToSendXOnXOff: return (RtsCtsXOnXOff_stringShort);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XHandshake[] GetItems()
		{
			List<XHandshake> a = new List<XHandshake>();
			a.Add(new XHandshake(Handshake.None));
			a.Add(new XHandshake(Handshake.RequestToSend));
			a.Add(new XHandshake(Handshake.XOnXOff));
			a.Add(new XHandshake(Handshake.RequestToSendXOnXOff));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XHandshake Parse(string handshake)
		{
			XHandshake result;

			if (TryParse(handshake, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("handshake", handshake, "Invalid handshake"));
		}

		/// <summary></summary>
		public static bool TryParse(string handshake, out XHandshake result)
		{
			if      ((string.Compare(handshake, None_string, true) == 0) ||
			         (string.Compare(handshake, None_stringShort, true) == 0))
			{
				result = new XHandshake(Handshake.None);
				return (true);
			}
			else if ((string.Compare(handshake, RtsCts_string, true) == 0) ||
			         (string.Compare(handshake, RtsCts_stringShort, true) == 0))
			{
				result = new XHandshake(Handshake.RequestToSend);
				return (true);
			}
			else if ((string.Compare(handshake, XOnXOff_string, true) == 0) ||
			         (string.Compare(handshake, XOnXOff_stringShort, true) == 0))
			{
				result = new XHandshake(Handshake.XOnXOff);
				return (true);
			}
			else if ((string.Compare(handshake, RtsCtsXOnXOff_string, true) == 0) ||
		             (string.Compare(handshake, RtsCtsXOnXOff_stringShort, true) == 0))
			{
				result = new XHandshake(Handshake.RequestToSendXOnXOff);
				return (true);
			}
			else
			{
				result = null;
				return (false);
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

		#endregion
	}
}
