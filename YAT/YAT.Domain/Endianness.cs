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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Domain
{
	#region Enum Endianness

	/// <summary>
	/// Enumeration of the the supported byte endiannesses.
	/// </summary>
	/// <remarks>
	/// Implemented as an enumeration (instead of a simple 'IsBigEndian' flag) to potentially
	/// support exotic endiannesses (e.g. PDP-11 'MiddleEndian').
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Endianness' is a correct English term and 'Endiannesses' seems the obvious plural.")]
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
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Endianness' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class EndiannessEx : EnumEx
	{
		#region String Definitions

		private const string BigEndian_string    = "Big-Endian (Network, Motorola)";
		private const string LittleEndian_string = "Little-Endian (Intel)";

		#endregion

		/// <summary>Default is <see cref="Endianness.BigEndian"/>.</summary>
		public const Endianness Default = Endianness.BigEndian;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public EndiannessEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public EndiannessEx(Endianness endianness)
			: base(endianness)
		{
		}

		/// <summary>
		/// Gets a value indicating whether the endianness of this instance is the same as the
		/// endianness of the machine, i.e. the computer architecture.
		/// </summary>
		public bool IsSameAsMachine
		{
			get
			{
				switch ((Endianness)UnderlyingEnum)
				{
					case Endianness.BigEndian:    return (!BitConverter.IsLittleEndian);
					case Endianness.LittleEndian: return ( BitConverter.IsLittleEndian);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((Endianness)UnderlyingEnum)
			{
				case Endianness.BigEndian:    return (BigEndian_string);
				case Endianness.LittleEndian: return (LittleEndian_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static EndiannessEx[] GetItems()
		{
			var a = new List<EndiannessEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new EndiannessEx(Endianness.BigEndian));
			a.Add(new EndiannessEx(Endianness.LittleEndian));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static EndiannessEx Parse(string s)
		{
			EndiannessEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid endianness string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out EndiannessEx result)
		{
			Endianness enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new EndiannessEx(enumResult);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public static bool TryParse(string s, out Endianness result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, BigEndian_string))
			{
				result = Endianness.BigEndian;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LittleEndian_string))
			{
				result = Endianness.LittleEndian;
				return (true);
			}
			else // Invalid string!
			{
				result = new EndiannessEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
