//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Extended enumeration type which offers more features that a normal enum.
	/// XEnum uses an underlying enum to distinguish objects. Provide an enum,
	/// derive from XEnum, define new name/value methods if necessary
	/// and implement the conversion methods to get a fully functional XEnum.
	/// </summary>
	/// <remarks>
	/// Using a normal enum in client class has the big advantage, that code
	/// designers and features like IntelliSense can use their built-in enum
	/// support.
	/// </remarks>
	/// <example>
	/// 
	/// -----------------------------------------------------------------
	/// Creating a concrete XEnum including implicit conversion operators
	/// -----------------------------------------------------------------
	/// 
	/// public enum Mode
	/// {
	///	    Fast = 1,
	///	    Normal = 2,
	///	    Slow = 3
	/// }
	///
	/// public class XMode : XEnum
	/// {
	///	    // Default is "Mode.Normal"
	///	    public XMode() : base(Mode.Normal)
	///     {
	///     }
	///
	///     protected XMode(Mode mode) : base(mode)
	///     {
	///     }
	///
	///     public override string ToString()
	///     {
	///         return (UnderlyingEnum.GetHashCode().ToString());
	///     }
	///   
	///     public static XMode[] GetItems()
	///     {
	///         List[XMode] a = new List[XMode](); // [] must be replaced be angle brackets
	///         a.Add(new XMode(Mode.Fast));
	///         a.Add(new XMode(Mode.Normal));
	///         a.Add(new XMode(Mode.Slow));
	///         return (a.ToArray());
	///     }
	///   
	///     public static XMode Parse(string mode)
	///     {
	///         return ((XMode)int.Parse(mode));
	///     }
	///   
	///     public static bool TryParse(string mode, out XMode result)
	///     {
	///         int intResult;
	///         
	///         if (int.TryParse(mode, out intResult))
	///         {
	///             result = (XMode)intResult;
	///             return (true);
	///         }
	///         else
	///         {
	///             result = null;
	///             return (false);
	///         }
	///     }
	///   
	///     public static implicit operator Mode(XMode mode)
	///     {
	///         return ((Mode)mode.UnderlyingEnum);
	///     }
	///   
	///     public static implicit operator XMode(Mode mode)
	///     {
	///         return (new XMode(mode));
	///     }
	///   
	///     public static implicit operator int(XMode mode)
	///     {
	///         return (mode.GetHashCode());
	///     }
	///   
	///     public static implicit operator XMode(int mode)
	///     {
	///         if      (mode >= (int)Mode.Slow)   return (new XMode(Mode.Slow));
	///   	    else if (mode >= (int)Mode.Normal) return (new XMode(Mode.Normal));
	///         else                               return (new XMode(Mode.Fast));
	///     }
	///   
	///     public static implicit operator string(XMode mode)
	///     {
	///		    return (mode.ToString());
	///	    }
	///		
	///     public static implicit operator XMode(string mode)
	///     {
	///        return (Parse(mode));
	///     }
	/// }
	///
	/// -------------------------------------------
	/// Adding XEnum design time support to a class
	/// -------------------------------------------
	/// 
	/// public class XEnumClient
	/// {
	///     private Mode _mode = Mode.Fast;         // Mode is a normal enum
	///     ...
	///     public Mode.Mode
	///     {
	///         get { return (_mode); }
	///         set { _mode = value;  }
	///     }
	/// }
	/// 
	/// </example>
	[Serializable]
	public abstract class XEnum : IEquatable<XEnum>, IComparable, ICloneable
	{
		/// <summary>
		/// Underlying enum.
		/// </summary>
		protected readonly Enum UnderlyingEnum;

		/// <summary>
		/// XEnum needs an underlying enum to be constructed.
		/// </summary>
		protected XEnum(Enum underlyingEnum)
		{
			UnderlyingEnum = underlyingEnum;
		}

		#region Object Members
		//------------------------------------------------------------------------------------------
		// Object Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is XEnum)
				return (Equals((XEnum)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(XEnum value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
				return (UnderlyingEnum.Equals(value.UnderlyingEnum));

			return (false);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (UnderlyingEnum.GetHashCode());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string
		/// representation.
		/// </summary>
		public override string ToString()
		{
			return (UnderlyingEnum.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string
		/// representation using the specified format.
		/// </summary>
		public virtual string ToString(string format)
		{
			return (UnderlyingEnum.ToString(format));
		}

		#endregion

		#region IComparable Members
		//------------------------------------------------------------------------------------------
		// IComparable Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return (1);
			if (obj is XEnum)
			{
				XEnum xe = (XEnum)obj;
				return (UnderlyingEnum.CompareTo(xe.UnderlyingEnum));
			}
			throw (new ArgumentException("Object is not a XEnum"));
		}

		#endregion

		#region ICloneable Members
		//------------------------------------------------------------------------------------------
		// ICloneable Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a deep copy of the XEnum and returns it.
		/// </summary>
		public virtual object Clone()
		{
			System.Type[] ta = new System.Type[1];
			ta[0] = UnderlyingEnum.GetType();
			ConstructorInfo ci = GetType().GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ta, null);

			Enum[] ea = new Enum[1];
			ea[0] = UnderlyingEnum;
			return ((XEnum)ci.Invoke(ea));
		}

		#endregion

		#region Static Methods
		//------------------------------------------------------------------------------------------
		// Static Methods
		//------------------------------------------------------------------------------------------

		private static System.Type XEnumTypeToUnderlyingEnumType(System.Type xEnumType)
		{
			System.Type[] ta = xEnumType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

			if (ta.Length < 1)
				throw (new TypeLoadException("No nested types found"));

			foreach (System.Type t in ta)
			{
				if (t.IsEnum)
					return (t);
			}
			throw (new TypeLoadException("No nested enum found"));
		}

		/// <summary>
		/// Retrieves the name of the underlying enum constant in the specified
		/// XEnummeration that has the specified value.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <param name="value">
		/// The value of a particular enumerated constant in terms of its
		/// underlying enum.
		/// </param>
		/// <returns>
		/// A string containing the name of the enumerated constant in xe whose
		/// value is value, or a null reference (Nothing in Visual Basic) if no
		/// such constant is found.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is not of type ex.
		/// </exception>
		public static string GetName(System.Type xEnumType, object value)
		{
			return (Enum.GetName(XEnumTypeToUnderlyingEnumType(xEnumType), value));
		}

		/// <summary>
		/// Retrieves an array of the underlying enum names of the constants in
		/// the specified XEnummeration.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <returns>
		/// A string array of the names of the constants in enumType. The elements
		/// of the array are sorted by the values of the enumerated constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		public static string[] GetNames(System.Type xEnumType)
		{
			return (Enum.GetNames(XEnumTypeToUnderlyingEnumType(xEnumType)));
		}

		/// <summary>
		/// Retrieves an array of the underlying enum values of the constants in
		/// the specified XEnummeration.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <returns>
		/// An Array of the values of the constants in enumType. The elements of
		/// the array are sorted by the values of the enumeration constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		public static Array GetValues(System.Type xEnumType)
		{
			return (Enum.GetValues(XEnumTypeToUnderlyingEnumType(xEnumType)));
		}

		/// <summary>
		/// Retrieves an array of the underlying enums in the specified
		/// XEnummeration.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <returns>
		/// An Array of the values of the constants in enumType. The elements of
		/// the array are sorted by the values of the enumeration constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		public static Enum[] GetItems(System.Type xEnumType)
		{
			System.Type underlyingEnumType = XEnumTypeToUnderlyingEnumType(xEnumType);

			List<Enum> items = new List<Enum>();
			FieldInfo[] fis = underlyingEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (FieldInfo fi in fis)
			{
				Enum e = (Enum)fi.GetValue(null);
				items.Add(e);
			}
			return (items.ToArray());
		}

		/// <summary>
		/// Returns an indication whether an underlying constant with a specified
		/// value exists in the specified enumeration.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <param name="value">The value or name of a constant in ex.</param>
		/// <returns>
		/// true if a constant in enumType has a value equal to value;
		/// otherwise, false.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The type of value is not an XEnum.
		/// 
		/// -or-
		/// 
		/// The type of value is not an underlying type of XEnum.
		/// </exception>
		public static bool IsDefined(System.Type xEnumType, object value)
		{
			return (Enum.IsDefined(XEnumTypeToUnderlyingEnumType(xEnumType), value));
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one
		/// or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <exception cref="ArgumentNullException">
		/// xEnumType or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is either an empty string or only contains white space.
		/// 
		/// -or-
		/// 
		/// value is a name, but not one of the named constants defined for the
		/// enumeration.
		/// </exception>
		public static XEnum Parse(System.Type xEnumType, string value)
		{
			return (Parse(xEnumType, value, true));
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one
		/// or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <param name="ignoreCase">If true, ignore case; otherwise, regard case.</param>
		/// <returns>An XEnum whose value is represented by value.</returns>
		/// <exception cref="ArgumentNullException">
		/// xEnumType or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is either an empty string or only contains white space.
		/// 
		/// -or-
		/// 
		/// value is a name, but not one of the named constants defined for the
		/// enumeration.
		/// </exception>
		public static XEnum Parse(System.Type xEnumType, string value, bool ignoreCase)
		{
			System.Type underlyingEnumType = XEnumTypeToUnderlyingEnumType(xEnumType);
			Enum underlyingEnum = (Enum)Enum.Parse(underlyingEnumType, value, ignoreCase);

			System.Type[] ta = new System.Type[1];
			ta[0] = underlyingEnumType;
			ConstructorInfo ci = xEnumType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ta, null);

			Enum[] ea = new Enum[1];
			ea[0] = underlyingEnum;
			return ((XEnum)ci.Invoke(ea));
		}

		/// <summary>
		/// Converts the specified value of a specified enumerated type to its
		/// equivalent string representation according to the specified format.
		/// </summary>
		/// <param name="xEnumType">The Type of the XEnum.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="format">The output format to use.</param>
		/// <returns>A string representation of value.</returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a null reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The value is from an enumeration that differs in type from enumType.
		/// s
		/// -or-
		/// s
		/// The type of value is not an underlying type of enumType.
		/// </exception>
		/// <exception cref="FormatException">
		/// The format parameter contains an invalid value.
		/// </exception>
		public static string Format(System.Type xEnumType, object value, string format)
		{
			return (Enum.Format(XEnumTypeToUnderlyingEnumType(xEnumType), value, format));
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is XEnum)
			{
				XEnum casted = (XEnum)objA;
				return (casted.CompareTo(objB));
			}
			return (-1);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(XEnum lhs, XEnum rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(XEnum lhs, XEnum rhs)
		{
			return (!(lhs == rhs));
		}

		/// <summary></summary>
		public static bool operator <(XEnum lhs, XEnum rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(XEnum lhs, XEnum rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(XEnum lhs, XEnum rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(XEnum lhs, XEnum rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
