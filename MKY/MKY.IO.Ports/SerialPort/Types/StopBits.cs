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
// MKY Version 1.0.28 Development
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
using System.IO.Ports;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Extended enum StopBitsEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class StopBitsEx : EnumEx
	{
		#region Double Definitions

		private const double LowerLimit_double   = -0.25;

		private const double None_double         =  0.0;
		private const double One_double          =  1.0;
		private const double OnePointFive_double =  1.5;
		private const double Two_double          =  2.0;

		private const double UpperLimit_double   =  2.25;

		#endregion

		/// <summary>Default is <see cref="StopBits.One"/>.</summary>
		public const StopBits Default = StopBits.One;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public StopBitsEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public StopBitsEx(StopBits bits)
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
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((StopBits)UnderlyingEnum)
			{
				case StopBits.None:         return (None_double        .ToString(CultureInfo.InvariantCulture));
				case StopBits.One:          return (One_double         .ToString(CultureInfo.InvariantCulture));
				case StopBits.OnePointFive: return (OnePointFive_double.ToString(CultureInfo.InvariantCulture));
				case StopBits.Two:          return (Two_double         .ToString(CultureInfo.InvariantCulture));

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
		public static StopBitsEx[] GetItems()
		{
			var a = new List<StopBitsEx>(4); // Preset the required capacity to improve memory management.

			a.Add(new StopBitsEx(StopBits.None));
			a.Add(new StopBitsEx(StopBits.One));
			a.Add(new StopBitsEx(StopBits.OnePointFive));
			a.Add(new StopBitsEx(StopBits.Two));

			return (a.ToArray());
		}

		#endregion

		#region Parse/From
		//==========================================================================================
		// Parse/From
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static StopBitsEx Parse(string s)
		{
			StopBitsEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid stop bits string! String must a valid decimal value."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out StopBitsEx result)
		{
			StopBits enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new StopBitsEx(enumResult);
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
		public static bool TryParse(string s, out StopBits result)
		{
			double doubleResult;
			if (double.TryParse(s, out doubleResult)) // TryParse() trims whitespace.
			{
				return (TryFrom(doubleResult, out result));
			}
			else // Invalid string!
			{
				result = new StopBitsEx(); // Default!
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(double bits, out StopBitsEx result)
		{
			if (IsDefined(bits))
			{
				result = bits;
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
		public static bool TryFrom(double bits, out StopBits result)
		{
			if (IsDefined(bits))
			{
				result = (StopBitsEx)bits;
				return (true);
			}
			else
			{
				result = new StopBitsEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static bool IsDefined(double bits)
		{
			return ((bits >= LowerLimit_double) && (bits <= UpperLimit_double));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator StopBits(StopBitsEx bits)
		{
			return ((StopBits)bits.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(StopBits bits)
		{
			return (new StopBitsEx(bits));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public static implicit operator double(StopBitsEx bits)
		{
			switch ((StopBits)bits.UnderlyingEnum)
			{
				case StopBits.None:         return (None_double);
				case StopBits.One:          return (One_double);
				case StopBits.OnePointFive: return (OnePointFive_double);
				case StopBits.Two:          return (Two_double);

				default: throw (new ArgumentOutOfRangeException("bits", bits, MessageHelper.InvalidExecutionPreamble + "'" + bits.UnderlyingEnum.ToString() + "' is a stop bit value that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public static implicit operator StopBitsEx(double bits)
		{
			if (IsDefined(bits))
			{
				if      (bits >= Two_double)          return (new StopBitsEx(StopBits.Two));
				else if (bits >= OnePointFive_double) return (new StopBitsEx(StopBits.OnePointFive));
				else if (bits >= One_double)          return (new StopBitsEx(StopBits.One));
				else                                  return (new StopBitsEx(StopBits.None));
			}
			else
			{
				throw (new ArgumentOutOfRangeException("bits", bits, MessageHelper.InvalidExecutionPreamble + "'" + bits.ToString(CultureInfo.InvariantCulture) + "' is a stop bit value that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public static implicit operator string(StopBitsEx bits)
		{
			return (bits.ToString());
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(string bits)
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
