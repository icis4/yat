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
// MKY Version 1.0.30
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
using System.Reflection;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Extended enumeration type which offers more features that a normal enum.
	/// <see cref="EnumEx"/> uses an underlying enum to distinguish objects. Provide an enum,
	/// derive from <see cref="EnumEx"/>, define new name/value methods if necessary and implement
	/// the conversion methods to get a fully functional <see cref="EnumEx"/>.
	/// </summary>
	/// <remarks>
	/// Using a normal enum in a client class has the big advantage that code designers and
	/// features like IntelliSense can use their built-in enum support.
	/// As a draw-back, <see cref="EnumEx"/> based types are not serializable because it abstract
	/// underlying type <see cref="Enum"/> isn't.
	/// </remarks>
	/// <example>
	///
	/// -------------------------------------------------------------------------------------------
	/// Creating a concrete EnumEx including implicit conversion operators
	/// -------------------------------------------------------------------------------------------
	///
	/// <code>
	/// public enum Mode
	/// {
	///     Fast = 1,
	///     Normal = 2,
	///     Slow = 3
	/// }
	///
	/// public class ModeEx : MKY.EnumEx
	/// {
	///     // Default is "Mode.Normal"
	///     public ModeEx() : this(Mode.Normal)
	///     {
	///     }
	///
	///     protected ModeEx(Mode mode) : base(mode)
	///     {
	///     }
	///
	///     public override string ToString()
	///     {
	///         return (UnderlyingEnum.GetHashCode().ToString());
	///     }
	///
	///     public static ModeEx[] GetItems()
	///     {
	///         var a = new List[ModeEx](3); // [] must be replaced be angle brackets
	///
	///         a.Add(new ModeEx(Mode.Fast));
	///         a.Add(new ModeEx(Mode.Normal));
	///         a.Add(new ModeEx(Mode.Slow));
	///
	///         return (a.ToArray());
	///     }
	///
	///     public static ModeEx Parse(string s)
	///     {
	///         return ((ModeEx)int.Parse(s));
	///     }
	///
	///     public static bool TryParse(string s, out ModeEx result)
	///     {
	///         Mode enumResult;
	///         if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
	///         {
	///             result = new ModeEx(enumResult);
	///             return (true);
	///         }
	///         else
	///         {
	///             result = null;
	///             return (false);
	///         }
	///     }
	///
	///     public static bool TryParse(string s, out Mode result)
	///     {
	///         int intResult;
	///         if (int.TryParse(s.Trim(), out intResult)) // TryParse() trims whitespace.
	///         {
	///             result = (ModeEx)intResult;
	///             return (true);
	///         }
	///         else // Invalid string!
	///         {
	///             result = new ModeEx(); // Default!
	///             return (false);
	///         }
	///     }
	///
	///     public static implicit operator Mode(ModeEx mode)
	///     {
	///         return ((Mode)mode.UnderlyingEnum);
	///     }
	///
	///     public static implicit operator ModeEx(Mode mode)
	///     {
	///         return (new ModeEx(mode));
	///     }
	///
	///     public static implicit operator int(ModeEx mode)
	///     {
	///         return (mode.GetHashCode());
	///     }
	///
	///     public static implicit operator ModeEx(int mode)
	///     {
	///         if      (mode >= (int)Mode.Slow)   return (new ModeEx(Mode.Slow));
	///         else if (mode >= (int)Mode.Normal) return (new ModeEx(Mode.Normal));
	///         else                               return (new ModeEx(Mode.Fast));
	///     }
	///
	///     public static implicit operator string(ModeEx mode)
	///     {
	///         return (mode.ToString());
	///     }
	///
	///     public static implicit operator ModeEx(string mode)
	///     {
	///        return (Parse(mode));
	///     }
	/// }
	/// </code>
	///
	/// -------------------------------------------
	/// Adding ModeEx design time support to a class
	/// -------------------------------------------
	///
	/// <code>
	/// public class EnumExClient
	/// {
	///     public Mode mode { get; set; } = Mode.Fast; // Mode is a normal enum
	///     ...
	/// }
	/// </code>
	/// </example>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "IntelliSense just contains 'Intelli'...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public abstract class EnumEx : IEquatable<EnumEx>, IComparable, ICloneable
	{
		/// <summary>
		/// Underlying enum.
		/// </summary>
		private Enum underlyingEnum;

		/// <summary>
		/// Constructor that allows to specify the underlying enum within the constructor of the deriving class.
		/// </summary>
		protected EnumEx()
		{
		}

		/// <summary>
		/// EnumEx requires an underlying enum to be constructed.
		/// </summary>
		protected EnumEx(Enum underlyingEnum)
		{
			SetUnderlyingEnum(underlyingEnum);
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~EnumEx()
		{
			Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		/// <summary>
		/// Underlying enum.
		/// </summary>
		protected Enum UnderlyingEnum
		{
			get { return (this.underlyingEnum); }
		}

		/// <summary>
		/// Sets the underlying enum.
		/// </summary>
		protected void SetUnderlyingEnum(Enum underlyingEnum)
		{
			this.underlyingEnum = underlyingEnum;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (UnderlyingEnum.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation using the
		/// specified format.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToString(string format)
		{
			return (UnderlyingEnum.ToString(format));
		}

	////Note that Enum.ToString(IFormatProvider) and Enum.ToString(format, IFormatProvider) are obsolete.

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return (UnderlyingEnum.GetHashCode());
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as EnumEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(EnumEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				(UnderlyingEnum != null) && (UnderlyingEnum.Equals(other.UnderlyingEnum)) // "Enum" is a "ValueType" but does
			);                                                                            // not overload the ==/!= operators.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(EnumEx lhs, EnumEx rhs)
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
		public static bool operator !=(EnumEx lhs, EnumEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(object obj)
		{
			var other = (obj as EnumEx);
			if (other != null)
				return (UnderlyingEnum.CompareTo(other.UnderlyingEnum));
			else
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify an 'EnumEx'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as EnumEx);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
		}

		/// <summary></summary>
		public static bool operator <(EnumEx lhs, EnumEx rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(EnumEx lhs, EnumEx rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(EnumEx lhs, EnumEx rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(EnumEx lhs, EnumEx rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion

		#region ICloneable Members
		//==========================================================================================
		// ICloneable Members
		//==========================================================================================

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual object Clone()
		{
			Type[] ta = new Type[1];
			ta[0] = UnderlyingEnum.GetType();
			ConstructorInfo ci = GetType().GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ta, null);

			Enum[] ea = new Enum[1];
			ea[0] = UnderlyingEnum;
			return ((EnumEx)ci.Invoke(ea));
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		private static Type EnumExTypeToUnderlyingEnumType(Type enumExType)
		{
			var potentialEnumTypes = new List<Type>();

			// Search for all constructors with a single enum parameter:
			var cis = enumExType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var ci in cis)
			{
				var p = ci.GetParameters();
				if (p.Length == 1)
				{
					var t = p[0].ParameterType;
					if (t.IsEnum)
						potentialEnumTypes.Add(t);
				}
			}

			// Select the best matching parameter type:
			if (potentialEnumTypes.Count == 1) // Should be the typical case for most EnumEx.
			{
				return (potentialEnumTypes[0]);
			}
			else
			{
				foreach (var t in potentialEnumTypes)
				{
					if (enumExType.Name.StartsWith(t.Name, StringComparison.Ordinal)) // "Ordinal" like a compiler would do.
						return (t);
				}

				throw (new TypeLoadException("No matching enum type found!"));
			}
		}

		/// <summary>
		/// Retrieves the name of the underlying enum constant in the specified
		/// enumeration that has the specified value.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <param name="value">
		/// The value of a particular enumerated constant in terms of its
		/// underlying enum.
		/// </param>
		/// <returns>
		/// A string containing the name of the enumerated constant in enumExType
		/// whose value is value, or a <c>null</c> reference (Nothing in Visual Basic)
		/// if no such constant is found.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is not of type ex.
		/// </exception>
		public static string GetUnderlyingEnumName(Type enumExType, object value)
		{
			return (Enum.GetName(EnumExTypeToUnderlyingEnumType(enumExType), value));
		}

		/// <summary>
		/// Retrieves an array of the underlying enum names of the constants in
		/// the specified enumeration.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <returns>
		/// A string array of the names of the constants in enum type. The values
		/// of the array are sorted by the values of the enumerated constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		public static string[] GetUnderlyingEnumNames(Type enumExType)
		{
			return (Enum.GetNames(EnumExTypeToUnderlyingEnumType(enumExType)));
		}

		/// <summary>
		/// Retrieves an array of the underlying enum values of the constants in
		/// the specified enumeration.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <returns>
		/// An Array of the values of the constants in enum type. The values of
		/// the array are sorted by the values of the enumeration constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		public static Array GetUnderlyingEnumValues(Type enumExType)
		{
			return (Enum.GetValues(EnumExTypeToUnderlyingEnumType(enumExType)));
		}

		/// <summary>
		/// Retrieves an array of the underlying enums in the specified enumeration.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <returns>
		/// An Array of the values of the constants in enum type. The values of
		/// the array are sorted by the values of the enumeration constants.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		public static Enum[] GetUnderlyingEnumItems(Type enumExType)
		{
			var underlyingEnumType = EnumExTypeToUnderlyingEnumType(enumExType);
			var fis = underlyingEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
			var items = new List<Enum>(fis.Length); // Preset the required capacity to improve memory management.

			foreach (var fi in fis)
			{
				var e = (Enum)fi.GetValue(null);
				items.Add(e);
			}

			return (items.ToArray());
		}

		/// <summary>
		/// Returns an indication whether an underlying constant with a specified
		/// value exists in the specified enumeration.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <param name="value">The value or name of a constant in ex.</param>
		/// <returns>
		/// <c>true</c> if a constant in enumType has a value equal to value;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The type of value is not an EnumEx.
		///
		/// -or-
		///
		/// The type of value is not an underlying type of EnumEx.
		/// </exception>
		public static bool IsDefined(Type enumExType, object value)
		{
			return (Enum.IsDefined(EnumExTypeToUnderlyingEnumType(enumExType), value));
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one
		/// or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <exception cref="ArgumentNullException">
		/// enumExType or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is either an empty string or only contains whitespace.
		///
		/// -or-
		///
		/// value is a name, but not one of the named constants defined for the
		/// enumeration.
		/// </exception>
		public static EnumEx Parse(Type enumExType, string value)
		{
			return (Parse(enumExType, value, true));
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one
		/// or more enumerated constants to an equivalent enumerated object.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <param name="ignoreCase">If true, ignore case; otherwise, regard case.</param>
		/// <returns>An EnumEx whose value is represented by value.</returns>
		/// <exception cref="ArgumentNullException">
		/// enumExType or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// value is either an empty string or only contains whitespace.
		///
		/// -or-
		///
		/// value is a name, but not one of the named constants defined for the
		/// enumeration.
		/// </exception>
		public static EnumEx Parse(Type enumExType, string value, bool ignoreCase)
		{
			Type underlyingEnumType = EnumExTypeToUnderlyingEnumType(enumExType);
			Enum underlyingEnum = (Enum)Enum.Parse(underlyingEnumType, value, ignoreCase);

			Type[] ta = new Type[1];
			ta[0] = underlyingEnumType;
			ConstructorInfo ci = enumExType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ta, null);

			Enum[] ea = new Enum[1];
			ea[0] = underlyingEnum;
			return ((EnumEx)ci.Invoke(ea));
		}

		/// <summary>
		/// Converts the specified value of a specified enumerated type to its
		/// equivalent string representation according to the specified format.
		/// </summary>
		/// <param name="enumExType">The Type of the EnumEx.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="format">The output format to use.</param>
		/// <returns>A string representation of value.</returns>
		/// <exception cref="ArgumentNullException">
		/// ex or value is a <c>null</c> reference (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The value is from an enumeration that differs in type from enum type.
		/// s
		/// -or-
		/// s
		/// The type of value is not an underlying type of enum type.
		/// </exception>
		/// <exception cref="FormatException">
		/// The format parameter contains an invalid value.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "2#", Justification = "Just using the same parameter name as .NET Enum.Format()...")]
		public static string Format(Type enumExType, object value, string format)
		{
			return (Enum.Format(EnumExTypeToUnderlyingEnumType(enumExType), value, format));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
