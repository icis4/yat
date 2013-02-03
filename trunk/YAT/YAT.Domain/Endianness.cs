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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Endianness

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
	public enum Endianness
	{
		/// <summary>e.g. Network, Motorola.</summary>
		BigEndian,

		/// <summary>e.g. Intel.</summary>
		LittleEndian
	}

	#endregion

	/// <summary>
	/// Extended enum EndiannessEx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class EndiannessEx : EnumEx
	{
		#region String Definitions

		private const string BigEndian_string    = "Big-Endian (Network, Motorola)";
		private const string LittleEndian_string = "Little-Endian (Intel)";
		private const string Unknown_string      = "Unknown";

		#endregion

		/// <summary>Default is <see cref="Endianness.BigEndian"/>.</summary>
		public EndiannessEx()
			: base(Endianness.BigEndian)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		protected EndiannessEx(Endianness endianness)
			: base(endianness)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Endianness)UnderlyingEnum)
			{
				case Endianness.BigEndian:    return (BigEndian_string);
				case Endianness.LittleEndian: return (LittleEndian_string);
				default:                     return (Unknown_string);
			}
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static EndiannessEx[] GetItems()
		{
			List<EndiannessEx> a = new List<EndiannessEx>();
			a.Add(new EndiannessEx(Endianness.BigEndian));
			a.Add(new EndiannessEx(Endianness.LittleEndian));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static EndiannessEx Parse(string endianness)
		{
			EndiannessEx result;

			if (TryParse(endianness, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("endianness", endianness, "Invalid endianness."));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static bool TryParse(string endianness, out EndiannessEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(endianness, BigEndian_string))
			{
				result = new EndiannessEx(Endianness.BigEndian);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(endianness, LittleEndian_string))
			{
				result = new EndiannessEx(Endianness.LittleEndian);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator Endianness(EndiannessEx endianness)
		{
			return ((Endianness)endianness.UnderlyingEnum);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator EndiannessEx(Endianness endianness)
		{
			return (new EndiannessEx(endianness));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator int(EndiannessEx endianness)
		{
			return (endianness.GetHashCode());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator EndiannessEx(int endianness)
		{
			return (new EndiannessEx((Endianness)endianness));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator string(EndiannessEx endianness)
		{
			return (endianness.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static implicit operator EndiannessEx(string endianness)
		{
			return (Parse(endianness));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
