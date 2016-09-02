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
// YAT 2.0 Gamma 2 Version 1.99.50
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

using MKY;

namespace YAT.Domain
{
	#region Enum TerminalType

	/// <summary></summary>
	public enum Direction
	{
		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		None = IODirection.None,

		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		Tx = IODirection.Tx,

		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		Rx = IODirection.Rx,

		/// <summary></summary>
		Bidir = 3
	}

	#endregion

	/// <summary>
	/// Extended enum DirectionEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class DirectionEx : EnumEx
	{
		#region String Definitions

		private const string  None_string = "--";
		private const string    Tx_string = "<<"; // Same as C++ stream out operator.
		private const string    Rx_string = ">>"; // Same as C++ stream in operator.
		private const string Bidir_string = "<>";

		#endregion

		/// <summary>Default is <see cref="Direction.None"/>.</summary>
		public const Direction Default = Direction.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public DirectionEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public DirectionEx(Direction direction)
			: base(direction)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Direction)UnderlyingEnum)
			{
				case Direction.None:  return (None_string);
				case Direction.Tx:    return (Tx_string);
				case Direction.Rx:    return (Rx_string);
				case Direction.Bidir: return (Bidir_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static DirectionEx[] GetItems()
		{
			List<DirectionEx> a = new List<DirectionEx>(4); // Preset the required capacity to improve memory management.
			a.Add(new DirectionEx(Direction.None));
			a.Add(new DirectionEx(Direction.Tx));
			a.Add(new DirectionEx(Direction.Rx));
			a.Add(new DirectionEx(Direction.Bidir));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static DirectionEx Parse(string s)
		{
			DirectionEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid serial direction string! String must one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out DirectionEx result)
		{
			Direction enumResult;
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
		public static bool TryParse(string s, out Direction result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = Direction.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = Direction.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Tx_string))
			{
				result = Direction.Tx;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Rx_string))
			{
				result = Direction.Rx;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Bidir_string))
			{
				result = Direction.Bidir;
				return (true);
			}
			else // Invalid string!
			{
				result = new DirectionEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Direction(DirectionEx direction)
		{
			return ((Direction)direction.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator DirectionEx(Direction direction)
		{
			return (new DirectionEx(direction));
		}

		/// <summary></summary>
		public static implicit operator int (DirectionEx direction)
		{
			return (direction.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator DirectionEx(int direction)
		{
			return (new DirectionEx((Direction)direction));
		}

		/// <summary></summary>
		public static implicit operator string (DirectionEx direction)
		{
			return (direction.ToString());
		}

		/// <summary></summary>
		public static implicit operator DirectionEx(string direction)
		{
			return (Parse(direction));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
