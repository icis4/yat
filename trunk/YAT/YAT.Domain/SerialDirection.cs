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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
	public enum SerialDirection
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		Tx,

		/// <summary></summary>
		Rx,

		/// <summary></summary>
		Bidir
	}

	#endregion

	/// <summary>
	/// Extended enum SerialDirectionEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class SerialDirectionEx : EnumEx
	{
		#region String Definitions

		private const string None_string  = "--";
		private const string Tx_string    = "<<"; // Same as C++ stream out operator.
		private const string Rx_string    = ">>"; // Same as C++ stream in operator.
		private const string Bidir_string = "<>";

		#endregion

		/// <summary>Default is <see cref="SerialDirection.None"/>.</summary>
		public SerialDirectionEx()
			: base(SerialDirection.None)
		{
		}

		/// <summary></summary>
		protected SerialDirectionEx(SerialDirection direction)
			: base(direction)
		{
		}
		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((SerialDirection)UnderlyingEnum)
			{
				case SerialDirection.None:  return (None_string);
				case SerialDirection.Tx:    return (Tx_string);
				case SerialDirection.Rx:    return (Rx_string);
				case SerialDirection.Bidir: return (Bidir_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static SerialDirectionEx[] GetItems()
		{
			List<SerialDirectionEx> a = new List<SerialDirectionEx>();
			a.Add(new SerialDirectionEx(SerialDirection.None));
			a.Add(new SerialDirectionEx(SerialDirection.Tx));
			a.Add(new SerialDirectionEx(SerialDirection.Rx));
			a.Add(new SerialDirectionEx(SerialDirection.Bidir));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialDirectionEx Parse(string s)
		{
			SerialDirectionEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid terminal direction string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialDirectionEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = new SerialDirectionEx(SerialDirection.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Tx_string))
			{
				result = new SerialDirectionEx(SerialDirection.Tx);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Rx_string))
			{
				result = new SerialDirectionEx(SerialDirection.Rx);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Bidir_string))
			{
				result = new SerialDirectionEx(SerialDirection.Bidir);
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
		public static implicit operator SerialDirection(SerialDirectionEx direction)
		{
			return ((SerialDirection)direction.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialDirectionEx(SerialDirection direction)
		{
			return (new SerialDirectionEx(direction));
		}

		/// <summary></summary>
		public static implicit operator int (SerialDirectionEx direction)
		{
			return (direction.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialDirectionEx(int direction)
		{
			return (new SerialDirectionEx((SerialDirection)direction));
		}

		/// <summary></summary>
		public static implicit operator string (SerialDirectionEx direction)
		{
			return (direction.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialDirectionEx(string direction)
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
