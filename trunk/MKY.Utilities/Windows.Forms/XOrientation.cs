using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Types;

namespace MKY.Utilities.Windows.Forms
{
	/// <summary>
	/// Extended enum XOrientation which extends <see cref="Orientation"/>.
	/// </summary>
	[Serializable]
	public class XOrientation : XEnum
	{
		#region String Definitions

		private const string Horizontal_string = "Horizontal";
		private const string Vertical_string = "Vertical";

		#endregion

		/// <summary>Default is <see cref="Orientation.Horizontal"/></summary>
		public XOrientation()
			: base(Orientation.Horizontal)
		{
		}

		/// <summary></summary>
		protected XOrientation(Orientation orientation)
			: base(orientation)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Orientation)UnderlyingEnum)
			{
				case Orientation.Horizontal: return (Horizontal_string);
				case Orientation.Vertical:   return (Vertical_string);
				default: throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XOrientation[] GetItems()
		{
			List<XOrientation> a = new List<XOrientation>();
			a.Add(new XOrientation(Orientation.Horizontal));
			a.Add(new XOrientation(Orientation.Vertical));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XOrientation Parse(string orientation)
		{
			XOrientation result;

			if (TryParse(orientation, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("orientation", orientation, "Invalid orientation."));
		}

		/// <summary></summary>
		public static bool TryParse(string orientation, out XOrientation result)
		{
			if (string.Compare(orientation, Horizontal_string, true) == 0)
			{
				result = new XOrientation(Orientation.Horizontal);
				return (true);
			}
			else if (string.Compare(orientation, Vertical_string, true) == 0)
			{
				result = new XOrientation(Orientation.Vertical);
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
		public static implicit operator Orientation(XOrientation orientation)
		{
			return ((Orientation)orientation.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XOrientation(Orientation orientation)
		{
			return (new XOrientation(orientation));
		}

		/// <summary></summary>
		public static implicit operator int(XOrientation orientation)
		{
			return (orientation.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XOrientation(int orientation)
		{
			return (new XOrientation((Orientation)orientation));
		}

		/// <summary></summary>
		public static implicit operator string(XOrientation orientation)
		{
			return (orientation.ToString());
		}

		/// <summary></summary>
		public static implicit operator XOrientation(string orientation)
		{
			return (Parse(orientation));
		}

		#endregion
	}
}
