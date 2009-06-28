//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Types;

namespace YAT.Domain
{
	#region Enum Endianess

	/// <summary></summary>
	public enum Endianess
	{
		/// <summary>e.g. Network, Motorola</summary>
		BigEndian,
		/// <summary>e.g. Intel</summary>
		LittleEndian
	}

	#endregion

	/// <summary>
	/// Extended enum XEndianess.
	/// </summary>
	[Serializable]
	public class XEndianess : XEnum
	{
		#region String Definitions

		private const string BigEndian_string    = "Big-Endian (Network, Motorola)";
		private const string LittleEndian_string = "Little-Endian (Intel)";
		private const string Unknown_string      = "Unknown";

		#endregion

		/// <summary>Default is <see cref="Endianess.BigEndian"/></summary>
		public XEndianess()
			: base(Endianess.BigEndian)
		{
		}

		/// <summary></summary>
		protected XEndianess(Endianess endianess)
			: base(endianess)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Endianess)UnderlyingEnum)
			{
				case Endianess.BigEndian:    return (BigEndian_string);
				case Endianess.LittleEndian: return (LittleEndian_string);
				default:                     return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XEndianess[] GetItems()
		{
			List<XEndianess> a = new List<XEndianess>();
			a.Add(new XEndianess(Endianess.BigEndian));
			a.Add(new XEndianess(Endianess.LittleEndian));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XEndianess Parse(string endianess)
		{
			XEndianess result;

			if (TryParse(endianess, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("endianess", endianess, "Invalid endianess."));
		}

		/// <summary></summary>
		public static bool TryParse(string endianess, out XEndianess result)
		{
			if      (string.Compare(endianess, BigEndian_string, true) == 0)
			{
				result = new XEndianess(Endianess.BigEndian);
				return (true);
			}
			else if (string.Compare(endianess, LittleEndian_string, true) == 0)
			{
				result = new XEndianess(Endianess.LittleEndian);
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
		public static implicit operator Endianess(XEndianess endianess)
		{
			return ((Endianess)endianess.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XEndianess(Endianess endianess)
		{
			return (new XEndianess(endianess));
		}

		/// <summary></summary>
		public static implicit operator int(XEndianess endianess)
		{
			return (endianess.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XEndianess(int endianess)
		{
			return (new XEndianess((Endianess)endianess));
		}

		/// <summary></summary>
		public static implicit operator string(XEndianess endianess)
		{
			return (endianess.ToString());
		}

		/// <summary></summary>
		public static implicit operator XEndianess(string endianess)
		{
			return (Parse(endianess));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
