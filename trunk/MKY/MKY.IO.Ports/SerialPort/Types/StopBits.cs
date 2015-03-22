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
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class StopBitsEx : EnumEx
	{
		#region Double Definitions

		private const double None_double = 0.0;
		private const double One_double = 1.0;
		private const double OnePointFive_double = 1.5;
		private const double Two_double = 2.0;

		#endregion

		/// <summary>Default is <see cref="StopBits.One"/>.</summary>
		public StopBitsEx()
			: base(StopBits.One)
		{
		}

		/// <summary></summary>
		protected StopBitsEx(StopBits bits)
			: base(bits)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((StopBits)UnderlyingEnum)
			{
				case StopBits.None:         return (None_double        .ToString(CultureInfo.InvariantCulture));
				case StopBits.One:          return (One_double         .ToString(CultureInfo.InvariantCulture));
				case StopBits.OnePointFive: return (OnePointFive_double.ToString(CultureInfo.InvariantCulture));
				case StopBits.Two:          return (Two_double         .ToString(CultureInfo.InvariantCulture));
			}
			throw (new InvalidOperationException("Code execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static StopBitsEx[] GetItems()
		{
			List<StopBitsEx> a = new List<StopBitsEx>();
			a.Add(new StopBitsEx(StopBits.None));
			a.Add(new StopBitsEx(StopBits.One));
			a.Add(new StopBitsEx(StopBits.OnePointFive));
			a.Add(new StopBitsEx(StopBits.Two));
			return (a.ToArray());
		}

		#endregion

		#region Parse/From

		/// <remarks>
		/// Following the convention of the .NET framework,
		/// whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static StopBitsEx Parse(string s)
		{
			StopBitsEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException("'" + s + "' is no valid stop bits string"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework,
		/// whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out StopBitsEx result)
		{
			double doubleResult;
			if (double.TryParse(s, out doubleResult)) // TryParse() trims whitespace.
				return (TryFrom(doubleResult, out result));

			result = null;
			return (false);
		}

		/// <summary>
		/// Tries to create a <see cref="StopBitsEx"/> object from the given port number.
		/// </summary>
		public static bool TryFrom(double stopBits, out StopBitsEx result)
		{
			if (IsValidStopBits(stopBits))
			{
				result = (StopBitsEx)stopBits;
				return (true);
			}

			result = null;
			return (false);
		}

		/// <summary></summary>
		public static bool IsValidStopBits(double stopBits)
		{
			if (stopBits == None_double)         return (true);
			if (stopBits == One_double)          return (true);
			if (stopBits == OnePointFive_double) return (true);
			if (stopBits == Two_double)          return (true);

			return (false);
		}

		#endregion

		#region Conversion Operators

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
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The value is intended to be limited ")]
		public static implicit operator double(StopBitsEx bits)
		{
			switch ((StopBits)bits.UnderlyingEnum)
			{
				case StopBits.None:         return (None_double);
				case StopBits.One:          return (One_double);
				case StopBits.OnePointFive: return (OnePointFive_double);
				case StopBits.Two:          return (Two_double);
			}
			throw (new InvalidOperationException("Code execution should never get here, please report this bug"));
		}

		/// <summary></summary>
		public static implicit operator StopBitsEx(double bits)
		{
			if      (bits >= Two_double)          return (new StopBitsEx(StopBits.Two));
			else if (bits >= OnePointFive_double) return (new StopBitsEx(StopBits.OnePointFive));
			else if (bits >= One_double)          return (new StopBitsEx(StopBits.One));
			else                                  return (new StopBitsEx(StopBits.None));
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
