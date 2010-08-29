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

using MKY.Utilities.Types;

namespace MKY.IO.Ports
{
	#region Enum DataBits

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum DataBits
	{
		Four  = 4,
		Five  = 5,
		Six   = 6,
		Seven = 7,
		Eight = 8,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum XDataBits.
	/// </summary>
	public class XDataBits : XEnum
	{
		/// <summary>Default is <see cref="DataBits.Eight"/>.</summary>
		public XDataBits()
			: base(DataBits.Eight)
		{
		}

		/// <summary></summary>
		protected XDataBits(DataBits bits)
			: base(bits)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString());
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XDataBits[] GetItems()
		{
			List<XDataBits> a = new List<XDataBits>();
			a.Add(new XDataBits(DataBits.Four));
			a.Add(new XDataBits(DataBits.Five));
			a.Add(new XDataBits(DataBits.Six));
			a.Add(new XDataBits(DataBits.Seven));
			a.Add(new XDataBits(DataBits.Eight));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XDataBits Parse(string bits)
		{
			return ((XDataBits)int.Parse(bits));
		}

		/// <summary></summary>
		public static bool TryParse(string bits, out XDataBits result)
		{
			int intResult;

			if (int.TryParse(bits, out intResult))
			{
				result = (XDataBits)intResult;
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
		public static implicit operator DataBits(XDataBits bits)
		{
			return ((DataBits)bits.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XDataBits(DataBits bits)
		{
			return (new XDataBits(bits));
		}

		/// <summary></summary>
		public static implicit operator int(XDataBits bits)
		{
			return (bits.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XDataBits(int bits)
		{
			if      (bits >= (int)DataBits.Eight) return (new XDataBits(DataBits.Eight));
			else if (bits >= (int)DataBits.Seven) return (new XDataBits(DataBits.Seven));
			else if (bits >= (int)DataBits.Six)   return (new XDataBits(DataBits.Six));
			else if (bits >= (int)DataBits.Five)  return (new XDataBits(DataBits.Five));
			else                                  return (new XDataBits(DataBits.Four));
		}

		/// <summary></summary>
		public static implicit operator string(XDataBits bits)
		{
			return (bits.ToString());
		}

		/// <summary></summary>
		public static implicit operator XDataBits(string bits)
		{
			return (Parse(bits));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
