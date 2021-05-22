//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
		Eight = 8
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum DataBitsEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class DataBitsEx : EnumEx
	{
		/// <summary>Default is <see cref="DataBits.Eight"/>.</summary>
		public const DataBits Default = DataBits.Eight;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public DataBitsEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public DataBitsEx(DataBits bits)
			: base(bits)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static DataBitsEx[] GetItems()
		{
			var a = new List<DataBitsEx>(4) // Preset the required capacity to improve memory management.
			{
				new DataBitsEx(DataBits.Five),
				new DataBitsEx(DataBits.Six),
				new DataBitsEx(DataBits.Seven),
				new DataBitsEx(DataBits.Eight)
			};
			return (a.ToArray());
		}

		#endregion

		#region Parse/Form
		//==========================================================================================
		// Parse/From
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static DataBitsEx Parse(string s)
		{
			DataBitsEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid data bits string! String must a valid integer value."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out DataBitsEx result)
		{
			DataBits enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new DataBitsEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out DataBits result)
		{
			int intResult;
			if (int.TryParse(s, out intResult)) // TryParse() trims whitespace.
			{
				return (TryFrom(intResult, out result));
			}
			else // Invalid string!
			{
				result = new DataBitsEx(); // Default!
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int dataBits, out DataBitsEx result)
		{
			if (IsDefined(dataBits))
			{
				result = dataBits;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int dataBits, out DataBits result)
		{
			if (IsDefined(dataBits))
			{
				result = (DataBitsEx)dataBits;
				return (true);
			}
			else
			{
				result = new DataBitsEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static bool IsDefined(int dataBits)
		{
			return (IsDefined(typeof(DataBitsEx), dataBits));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
