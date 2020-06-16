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
// Copyright © 2003-2020 Matthias Kläy.
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
	#region Enum BaudRate

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <remarks>Using "Baud" prefix in order to be CLS-compliant.</remarks>
	public enum BaudRate
	{
		Baud75      = 75,
		Baud110     = 110,
		Baud134     = 134,
		Baud150     = 150,
		Baud300     = 300,
		Baud600     = 600,
		Baud1200    = 1200,
		Baud1800    = 1800,
		Baud2400    = 2400,
		Baud4800    = 4800,
		Baud7200    = 7200,
		Baud9600    = 9600,
		Baud14400   = 14400, // TI standard baud rate.
		Baud19200   = 19200,
		Baud28800   = 28800, // TI standard baud rate.
		Baud33600   = 33600,
		Baud38400   = 38400,
		Baud57600   = 57600,
		Baud115200  = 115200,
		Baud128000  = 128000,
		Baud230400  = 230400,
		Baud256000  = 256000,
		Baud460800  = 460800,
		Baud921600  = 921600,
		Baud960000  = 960000,  // FTDI standard baud rate (https://www.ftdichip.com/Support/Knowledgebase/index.html?whatbaudratesareachieveabl.htm).
		Baud1000000 = 1000000, //  ""
		Baud1200000 = 1200000, //  ""
		Baud1500000 = 1500000, //  ""
		Baud2000000 = 2000000, //  ""
		Baud3000000 = 3000000, //  ""

		Explicit = 0,

		/// <summary>Theoretical minimum is 1.</summary>
		Minimum = 1,

		/// <summary>Theoretical maximum is <see cref="int.MaxValue"/>.</summary>
		Maximum = int.MaxValue
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum BaudRateEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class BaudRateEx : EnumEx, IEquatable<BaudRateEx>
	{
		private int explicitBaudRate;

		/// <summary>Default is <see cref="BaudRate.Baud9600"/>.</summary>
		public const BaudRate Default = BaudRate.Baud9600;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public BaudRateEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="BaudRate.Explicit"/> because that selection requires
		/// a baud rate value. Use <see cref="BaudRateEx(int)"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="baudRate"/> is <see cref="BaudRate.Explicit"/>. Use <see cref="BaudRateEx(int)"/> instead.
		/// </exception>
		public BaudRateEx(BaudRate baudRate)
			: base(baudRate)
		{
			if (baudRate == BaudRate.Explicit)
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'BaudRate.Explicit' requires a baud rate value, use 'BaudRateEx(int)' instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="baudRate"/> is no potentially valid baud rate value.
		/// </exception>
		public BaudRateEx(int baudRate)
			: base(BaudRate.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			if (IsPotentiallyValid(baudRate))
			{
				this.explicitBaudRate = baudRate;
			}
			else
			{
				throw (new ArgumentOutOfRangeException
				(
					"baudRate",
					baudRate,
					"Baud rate must be a potentially valid baud rate value!"
				)); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.
			}
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsExplicit
		{
			get { return ((BaudRate)UnderlyingEnum == BaudRate.Explicit); }
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if ((BaudRate)UnderlyingEnum == BaudRate.Explicit)
				return (this.explicitBaudRate.ToString(CultureInfo.InvariantCulture));
			else
				return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((BaudRate)UnderlyingEnum == BaudRate.Explicit)
					hashCode = (hashCode * 397) ^ this.explicitBaudRate;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as BaudRateEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(BaudRateEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((BaudRate)UnderlyingEnum == BaudRate.Explicit)
			{
				return
				(
					base.Equals(other) &&
					this.explicitBaudRate.Equals(other.explicitBaudRate)
				);
			}
			else
			{
				return
				(
					base.Equals(other)
				);
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BaudRateEx lhs, BaudRateEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BaudRateEx lhs, BaudRateEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static BaudRateEx[] GetItems()
		{
			var a = new List<BaudRateEx>(32); // Preset the required capacity to improve memory management; 32 is a large enough value.

			a.Add(new BaudRateEx(BaudRate.Baud75));
			a.Add(new BaudRateEx(BaudRate.Baud110));
			a.Add(new BaudRateEx(BaudRate.Baud134));
			a.Add(new BaudRateEx(BaudRate.Baud150));
			a.Add(new BaudRateEx(BaudRate.Baud300));
			a.Add(new BaudRateEx(BaudRate.Baud600));
			a.Add(new BaudRateEx(BaudRate.Baud1200));
			a.Add(new BaudRateEx(BaudRate.Baud1800));
			a.Add(new BaudRateEx(BaudRate.Baud2400));
			a.Add(new BaudRateEx(BaudRate.Baud4800));
			a.Add(new BaudRateEx(BaudRate.Baud7200));
			a.Add(new BaudRateEx(BaudRate.Baud9600));
			a.Add(new BaudRateEx(BaudRate.Baud14400));
			a.Add(new BaudRateEx(BaudRate.Baud19200));
			a.Add(new BaudRateEx(BaudRate.Baud28800));
			a.Add(new BaudRateEx(BaudRate.Baud33600));
			a.Add(new BaudRateEx(BaudRate.Baud38400));
			a.Add(new BaudRateEx(BaudRate.Baud57600));
			a.Add(new BaudRateEx(BaudRate.Baud115200));
			a.Add(new BaudRateEx(BaudRate.Baud128000));
			a.Add(new BaudRateEx(BaudRate.Baud230400));
			a.Add(new BaudRateEx(BaudRate.Baud256000));
			a.Add(new BaudRateEx(BaudRate.Baud460800));
			a.Add(new BaudRateEx(BaudRate.Baud921600));
			a.Add(new BaudRateEx(BaudRate.Baud960000));
			a.Add(new BaudRateEx(BaudRate.Baud1000000));
			a.Add(new BaudRateEx(BaudRate.Baud1200000));
			a.Add(new BaudRateEx(BaudRate.Baud1500000));
			a.Add(new BaudRateEx(BaudRate.Baud2000000));
			a.Add(new BaudRateEx(BaudRate.Baud3000000));

			// This method shall only return the fixed items, 'Explicit' is not added therefore.

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
		public static BaudRateEx Parse(string s)
		{
			BaudRateEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid baud rate string! String must a valid integer value."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out BaudRateEx result)
		{
			BaudRate enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new BaudRateEx(enumResult);
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
		public static bool TryParse(string s, out BaudRate result)
		{
			int intResult;
			if (int.TryParse(s, out intResult)) // TryParse() trims whitespace.
			{
				return (TryFrom(intResult, out result));
			}
			else // Invalid string!
			{
				result = new BaudRateEx(); // Default!
				return (false);
			}
		}

		/// <summary>
		/// Tries to create an item from the given value.
		/// </summary>
		public static bool TryFrom(int baudRate, out BaudRateEx result)
		{
			if (IsPotentiallyValid(baudRate))
			{
				result = baudRate;
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
		public static bool TryFrom(int baudRate, out BaudRate result)
		{
			if (IsPotentiallyValid(baudRate))
			{
				result = (BaudRateEx)baudRate;
				return (true);
			}
			else
			{
				result = new BaudRateEx(); // Default!
				return (false);
			}
		}

		/// <summary></summary>
		public static bool IsPotentiallyValid(int baudRate)
		{
			return ((baudRate > (int)BaudRate.Minimum) && (baudRate < (int)BaudRate.Maximum));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator BaudRate(BaudRateEx baudRate)
		{
			return ((BaudRate)baudRate.UnderlyingEnum);
		}

		/// <remarks>
		/// Explicit because cast doesn't work for <see cref="BaudRate.Explicit"/>.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="baudRate"/> is <see cref="BaudRate.Explicit"/>.
		/// </exception>
		public static explicit operator BaudRateEx(BaudRate baudRate)
		{
			return (new BaudRateEx(baudRate));
		}

		/// <summary></summary>
		public static implicit operator int(BaudRateEx baudRate)
		{
			if ((BaudRate)baudRate.UnderlyingEnum == BaudRate.Explicit)
				return (baudRate.explicitBaudRate);
			else
				return (baudRate.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator BaudRateEx(int baudRate)
		{
			// Sorted big to small for faster lookup of 'modern' baud rates.

			if      (baudRate == (int)BaudRate.Baud3000000) return (new BaudRateEx(BaudRate.Baud3000000));
			else if (baudRate == (int)BaudRate.Baud2000000) return (new BaudRateEx(BaudRate.Baud2000000));
			else if (baudRate == (int)BaudRate.Baud1500000) return (new BaudRateEx(BaudRate.Baud1500000));
			else if (baudRate == (int)BaudRate.Baud1200000) return (new BaudRateEx(BaudRate.Baud1200000));
			else if (baudRate == (int)BaudRate.Baud1000000) return (new BaudRateEx(BaudRate.Baud1000000));
			else if (baudRate == (int)BaudRate.Baud960000)  return (new BaudRateEx(BaudRate.Baud960000));
			else if (baudRate == (int)BaudRate.Baud921600)  return (new BaudRateEx(BaudRate.Baud921600));
			else if (baudRate == (int)BaudRate.Baud460800)  return (new BaudRateEx(BaudRate.Baud460800));
			else if (baudRate == (int)BaudRate.Baud256000)  return (new BaudRateEx(BaudRate.Baud256000));
			else if (baudRate == (int)BaudRate.Baud230400)  return (new BaudRateEx(BaudRate.Baud230400));
			else if (baudRate == (int)BaudRate.Baud128000)  return (new BaudRateEx(BaudRate.Baud128000));
			else if (baudRate == (int)BaudRate.Baud115200)  return (new BaudRateEx(BaudRate.Baud115200));
			else if (baudRate == (int)BaudRate.Baud57600)   return (new BaudRateEx(BaudRate.Baud57600));
			else if (baudRate == (int)BaudRate.Baud38400)   return (new BaudRateEx(BaudRate.Baud38400));
			else if (baudRate == (int)BaudRate.Baud33600)   return (new BaudRateEx(BaudRate.Baud33600));
			else if (baudRate == (int)BaudRate.Baud28800)   return (new BaudRateEx(BaudRate.Baud28800));
			else if (baudRate == (int)BaudRate.Baud19200)   return (new BaudRateEx(BaudRate.Baud19200));
			else if (baudRate == (int)BaudRate.Baud14400)   return (new BaudRateEx(BaudRate.Baud14400));
			else if (baudRate == (int)BaudRate.Baud9600)    return (new BaudRateEx(BaudRate.Baud9600));
			else if (baudRate == (int)BaudRate.Baud7200)    return (new BaudRateEx(BaudRate.Baud7200));
			else if (baudRate == (int)BaudRate.Baud4800)    return (new BaudRateEx(BaudRate.Baud4800));
			else if (baudRate == (int)BaudRate.Baud2400)    return (new BaudRateEx(BaudRate.Baud2400));
			else if (baudRate == (int)BaudRate.Baud1200)    return (new BaudRateEx(BaudRate.Baud1200));
			else if (baudRate == (int)BaudRate.Baud600)     return (new BaudRateEx(BaudRate.Baud600));
			else if (baudRate == (int)BaudRate.Baud300)     return (new BaudRateEx(BaudRate.Baud300));
			else if (baudRate == (int)BaudRate.Baud150)     return (new BaudRateEx(BaudRate.Baud150));
			else if (baudRate == (int)BaudRate.Baud134)     return (new BaudRateEx(BaudRate.Baud134));
			else if (baudRate == (int)BaudRate.Baud110)     return (new BaudRateEx(BaudRate.Baud110));
			else if (baudRate == (int)BaudRate.Baud75)      return (new BaudRateEx(BaudRate.Baud75));
			else                                            return (new BaudRateEx(baudRate));
		}

		/// <summary></summary>
		public static implicit operator string(BaudRateEx baudRate)
		{
			return (baudRate.ToString());
		}

		/// <summary></summary>
		public static implicit operator BaudRateEx(string baudRate)
		{
			return (Parse(baudRate));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
