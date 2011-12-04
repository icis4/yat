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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
	#region Enum Endianess

	/// <summary></summary>
	public enum Endianess
	{
		/// <summary>e.g. Network, Motorola.</summary>
		BigEndian,

		/// <summary>e.g. Intel.</summary>
		LittleEndian
	}

	#endregion

	/// <summary>
	/// Extended enum EndianessEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class EndianessEx : MKY.EnumEx
	{
		#region String Definitions

		private const string BigEndian_string    = "Big-Endian (Network, Motorola)";
		private const string LittleEndian_string = "Little-Endian (Intel)";
		private const string Unknown_string      = "Unknown";

		#endregion

		/// <summary>Default is <see cref="Endianess.BigEndian"/>.</summary>
		public EndianessEx()
			: base(Endianess.BigEndian)
		{
		}

		/// <summary></summary>
		protected EndianessEx(Endianess endianess)
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
		public static EndianessEx[] GetItems()
		{
			List<EndianessEx> a = new List<EndianessEx>();
			a.Add(new EndianessEx(Endianess.BigEndian));
			a.Add(new EndianessEx(Endianess.LittleEndian));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static EndianessEx Parse(string endianess)
		{
			EndianessEx result;

			if (TryParse(endianess, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("endianess", endianess, "Invalid endianess."));
		}

		/// <summary></summary>
		public static bool TryParse(string endianess, out EndianessEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(endianess, BigEndian_string))
			{
				result = new EndianessEx(Endianess.BigEndian);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(endianess, LittleEndian_string))
			{
				result = new EndianessEx(Endianess.LittleEndian);
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
		public static implicit operator Endianess(EndianessEx endianess)
		{
			return ((Endianess)endianess.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator EndianessEx(Endianess endianess)
		{
			return (new EndianessEx(endianess));
		}

		/// <summary></summary>
		public static implicit operator int(EndianessEx endianess)
		{
			return (endianess.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator EndianessEx(int endianess)
		{
			return (new EndianessEx((Endianess)endianess));
		}

		/// <summary></summary>
		public static implicit operator string(EndianessEx endianess)
		{
			return (endianess.ToString());
		}

		/// <summary></summary>
		public static implicit operator EndianessEx(string endianess)
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
