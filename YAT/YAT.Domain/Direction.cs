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
// Copyright © 2003-2021 Matthias Kläy.
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
		/// <remarks>Also usable with meaning 'Unknown' (yet).</remarks>
		None  = IODirection.None,

		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		Tx    = IODirection.Tx,

		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		Rx    = IODirection.Rx,

		/// <remarks>Same as <see cref="IODirection"/> to allow casting for optimized speed.</remarks>
		/// <remarks>Usable for I/O operations not tied to a direction.</remarks>
		Bidir = IODirection.Bidir
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
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((Direction)UnderlyingEnum)
			{
				case Direction.None:  return (None_string);
				case Direction.Tx:    return (Tx_string);
				case Direction.Rx:    return (Rx_string);
				case Direction.Bidir: return (Bidir_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static DirectionEx[] GetItems()
		{
			var a = new List<DirectionEx>(4); // Preset the required capacity to improve memory management.

			a.Add(new DirectionEx(Direction.None));
			a.Add(new DirectionEx(Direction.Tx));
			a.Add(new DirectionEx(Direction.Rx));
			a.Add(new DirectionEx(Direction.Bidir));

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
		public static DirectionEx Parse(string s)
		{
			DirectionEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid serial direction string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out DirectionEx result)
		{
			Direction enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new DirectionEx(enumResult);
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
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
