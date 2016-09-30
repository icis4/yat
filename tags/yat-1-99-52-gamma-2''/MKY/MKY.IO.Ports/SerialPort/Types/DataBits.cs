//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static DataBitsEx[] GetItems()
		{
			List<DataBitsEx> a = new List<DataBitsEx>(4); // Preset the required capacity to improve memory management.
			a.Add(new DataBitsEx(DataBits.Five));
			a.Add(new DataBitsEx(DataBits.Six));
			a.Add(new DataBitsEx(DataBits.Seven));
			a.Add(new DataBitsEx(DataBits.Eight));
			return (a.ToArray());
		}

		#endregion

		#region Parse/Form

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
				result = enumResult;
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
			else
			{
				if (s != null)
					s = s.Trim();

				if (string.IsNullOrEmpty(s))
				{
					result = new DataBitsEx(); // Default!
					return (true); // Default silently, could e.g. happen when deserializing an XML.
				}
				else // Invalid string!
				{
					result = new DataBitsEx(); // Default!
					return (false);
				}
			}
		}

		/// <summary>
		/// Tries to create an item from the given port number.
		/// </summary>
		public static bool TryFrom(int dataBits, out DataBitsEx result)
		{
			if (IsValidDataBits(dataBits))
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
		/// Tries to create an item from the given port number.
		/// </summary>
		public static bool TryFrom(int dataBits, out DataBits result)
		{
			if (IsValidDataBits(dataBits))
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
