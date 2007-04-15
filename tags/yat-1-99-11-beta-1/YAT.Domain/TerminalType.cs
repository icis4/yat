using System;
using System.Collections.Generic;
using System.Text;

using HSR.Utilities.Types;

namespace HSR.YAT.Domain
{
	#region Enum TerminalType

	/// <summary></summary>
	public enum TerminalType
	{
		/// <summary></summary>
		Text,
		/// <summary></summary>
		Binary
	}

	#endregion

	/// <summary>
	/// Extended enum XTerminalType.
	/// </summary>
	[Serializable]
	public class XTerminalType : XEnum
	{
		#region String Definitions

		private const string Text_string = "Text";
		private const string Binary_string = "Binary";

		#endregion

		/// <summary>Default is <see cref="TerminalType.Text"/></summary>
		public XTerminalType()
			: base(TerminalType.Text)
		{
		}

		/// <summary></summary>
		protected XTerminalType(TerminalType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((TerminalType)UnderlyingEnum)
			{
				case TerminalType.Text:   return (Text_string);
				case TerminalType.Binary: return (Binary_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XTerminalType[] GetItems()
		{
			List<XTerminalType> a = new List<XTerminalType>();
			a.Add(new XTerminalType(TerminalType.Text));
			a.Add(new XTerminalType(TerminalType.Binary));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XTerminalType Parse(string type)
		{
			if (string.Compare(type, Text_string, true) == 0)
			{
				return (new XTerminalType(TerminalType.Text));
			}
			else if (string.Compare(type, Binary_string, true) == 0)
			{
				return (new XTerminalType(TerminalType.Binary));
			}

			throw (new ArgumentOutOfRangeException(type));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator TerminalType(XTerminalType type)
		{
			return ((TerminalType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XTerminalType(TerminalType type)
		{
			return (new XTerminalType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XTerminalType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XTerminalType(int type)
		{
			return (new XTerminalType((TerminalType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XTerminalType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XTerminalType(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}
