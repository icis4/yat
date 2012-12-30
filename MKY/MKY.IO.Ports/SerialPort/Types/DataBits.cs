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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.IO.Ports
{
	#region Enum DataBits

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "There is no setting with 0 data bits.")]
	[SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Well, five/six/seven/eight are plural...")]
	public enum DataBits
	{
		Five  = 5,
		Six   = 6,
		Seven = 7,
		Eight = 8,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum DataBitsEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class DataBitsEx : EnumEx
	{
		/// <summary>Default is <see cref="DataBits.Eight"/>.</summary>
		public DataBitsEx()
			: base(DataBits.Eight)
		{
		}

		/// <summary></summary>
		protected DataBitsEx(DataBits bits)
			: base(bits)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static DataBitsEx[] GetItems()
		{
			List<DataBitsEx> a = new List<DataBitsEx>();
			a.Add(new DataBitsEx(DataBits.Five));
			a.Add(new DataBitsEx(DataBits.Six));
			a.Add(new DataBitsEx(DataBits.Seven));
			a.Add(new DataBitsEx(DataBits.Eight));
			return (a.ToArray());
		}

		#endregion

		#region Parse/Form

		/// <summary></summary>
		public static DataBitsEx Parse(string bits)
		{
			return ((DataBitsEx)int.Parse(bits, NumberFormatInfo.InvariantInfo));
		}

		/// <summary></summary>
		public static bool TryParse(string bits, out DataBitsEx result)
		{
			int intResult;
			if (int.TryParse(bits, out intResult))
				return (TryFrom(intResult, out result));

			result = null;
			return (false);
		}

		/// <summary>
		/// Tries to create a <see cref="DataBitsEx"/> object from the given port number.
		/// </summary>
		public static bool TryFrom(int dataBits, out DataBitsEx result)
		{
			if (IsValidDataBits(dataBits))
			{
				result = (DataBitsEx)dataBits;
				return (true);
			}

			result = null;
			return (false);
		}

		/// <summary></summary>
		public static bool IsValidDataBits(int dataBits)
		{
			return ((dataBits >= 4) && (dataBits <= 8));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator DataBits(DataBitsEx bits)
		{
			return ((DataBits)bits.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator DataBitsEx(DataBits bits)
		{
			return (new DataBitsEx(bits));
		}

		/// <summary></summary>
		public static implicit operator int(DataBitsEx bits)
		{
			return (bits.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator DataBitsEx(int bits)
		{
			if      (bits >= (int)DataBits.Eight) return (new DataBitsEx(DataBits.Eight));
			else if (bits >= (int)DataBits.Seven) return (new DataBitsEx(DataBits.Seven));
			else if (bits >= (int)DataBits.Six)   return (new DataBitsEx(DataBits.Six));
			else                                  return (new DataBitsEx(DataBits.Five));
		}

		/// <summary></summary>
		public static implicit operator string(DataBitsEx bits)
		{
			return (bits.ToString());
		}

		/// <summary></summary>
		public static implicit operator DataBitsEx(string bits)
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
