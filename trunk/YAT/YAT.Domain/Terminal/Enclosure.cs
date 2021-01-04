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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Enclosure

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Enclosure
	{
		None,

		Parentheses,
		SquareBrackets,
		CurlyBraces,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum InfoEnclosureEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class EnclosureEx : EnumEx, IEquatable<EnclosureEx>
	{
		#region String Definitions

		private const string None_enclosure                = "";
		private const string None_description              = "[None]";

		private const string Parentheses_enclosure         = "()";
		private const string Parentheses_enclosureLeft     = "(";
		private const string Parentheses_enclosureRight    = ")";
		private const string Parentheses_description       = "Parentheses ()";

		private const string SquareBrackets_enclosure      = "[]";
		private const string SquareBrackets_enclosureLeft  = "[";
		private const string SquareBrackets_enclosureRight = "]";
		private const string SquareBrackets_description    = "Brackets []";

		private const string CurlyBraces_enclosure         = "{}";
		private const string CurlyBraces_enclosureLeft     = "{";
		private const string CurlyBraces_enclosureRight    = "}";
		private const string CurlyBraces_description       = "Braces {}";

		#endregion

		private string explicitEnclosure; // = null;

		/// <summary>Default is <see cref="Enclosure.None"/>.</summary>
		public const Enclosure Default = Enclosure.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public EnclosureEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="Enclosure.Explicit"/> because that selection requires
		/// an enclosure string. Use <see cref="EnclosureEx(string)"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="enclosure"/> is <see cref="Enclosure.Explicit"/>. Use <see cref="EnclosureEx(string)"/> instead.
		/// </exception>
		public EnclosureEx(Enclosure enclosure)
			: base(enclosure)
		{
			if (enclosure == Enclosure.Explicit)
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'Enclosure.Explicit' requires an enclosure string, use 'EnclosureEx(string)' instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public EnclosureEx(string enclosure)
			: base(Enclosure.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			this.explicitEnclosure = enclosure;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsExplicit
		{
			get { return ((Enclosure)UnderlyingEnum == Enclosure.Explicit); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToEnclosure()
		{
			switch ((Enclosure)UnderlyingEnum)
			{
				case Enclosure.None:           return (          None_enclosure);

				case Enclosure.Parentheses:    return (   Parentheses_enclosure);
				case Enclosure.SquareBrackets: return (SquareBrackets_enclosure);
				case Enclosure.CurlyBraces:    return (   CurlyBraces_enclosure);

				case Enclosure.Explicit:       return (        this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureLeft()
		{
			switch ((Enclosure)UnderlyingEnum)
			{
				case Enclosure.None:           return (          None_enclosure);

				case Enclosure.Parentheses:    return (   Parentheses_enclosureLeft);
				case Enclosure.SquareBrackets: return (SquareBrackets_enclosureLeft);
				case Enclosure.CurlyBraces:    return (   CurlyBraces_enclosureLeft);

				case Enclosure.Explicit:       return (StringEx.Left(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureRight()
		{
			switch ((Enclosure)UnderlyingEnum)
			{
				case Enclosure.None:           return (          None_enclosure);

				case Enclosure.Parentheses:    return (   Parentheses_enclosureRight);
				case Enclosure.SquareBrackets: return (SquareBrackets_enclosureRight);
				case Enclosure.CurlyBraces:    return (   CurlyBraces_enclosureRight);

				case Enclosure.Explicit:       return (StringEx.Right(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((Enclosure)UnderlyingEnum)
			{
				case Enclosure.None:           return (          None_description);

				case Enclosure.Parentheses:    return (   Parentheses_description);
				case Enclosure.SquareBrackets: return (SquareBrackets_description);
				case Enclosure.CurlyBraces:    return (   CurlyBraces_description);

				case Enclosure.Explicit:       return (          this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Removes the enclosure strings from the given <see cref="string"/> object.
		/// </summary>
		/// <param name="str">The string to trim.</param>
		/// <returns>
		/// The string that remains after the enclosure has been removed from the given
		/// <see cref="string"/> object.
		/// </returns>
		public virtual string Trim(string str)
		{
			string enclosure;

			enclosure = ToEnclosureLeft();
			if (str.StartsWith(enclosure, StringComparison.CurrentCulture))
				str = str.Substring(enclosure.Length);

			enclosure = ToEnclosureRight();
			if (str.EndsWith(enclosure, StringComparison.CurrentCulture))
				str = str.Substring(0, (str.Length - enclosure.Length));

			return (str);
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
			return (ToDescription());
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((Enclosure)UnderlyingEnum == Enclosure.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitEnclosure != null ? this.explicitEnclosure.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as EnclosureEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(EnclosureEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((Enclosure)UnderlyingEnum == Enclosure.Explicit)
			{
				return
				(
					base.Equals(other) &&
					StringEx.EqualsOrdinal(this.explicitEnclosure, other.explicitEnclosure)
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
		public static bool operator ==(EnclosureEx lhs, EnclosureEx rhs)
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
		public static bool operator !=(EnclosureEx lhs, EnclosureEx rhs)
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
		public static EnclosureEx[] GetItems()
		{
			var a = new List<EnclosureEx>(4); // Preset the required capacity to improve memory management.

			a.Add(new EnclosureEx(Enclosure.None));
			a.Add(new EnclosureEx(Enclosure.Parentheses));
			a.Add(new EnclosureEx(Enclosure.SquareBrackets));
			a.Add(new EnclosureEx(Enclosure.CurlyBraces));

			// This method shall only return the fixed items, 'Explicit' is not added therefore.

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
		public static EnclosureEx Parse(string s)
		{
			EnclosureEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid info element enclosure! String must be an even number of characters (e.g. 2 or 4 characters), or one of the predefined enclosures."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out EnclosureEx result)
		{
			Enclosure enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new EnclosureEx(enumResult);
				return (true);
			}
			else
			{
				if ((s.Length % 2) == 0) // Valid explicit?
				{
					result = new EnclosureEx(s);
					return (true);
				}
				else // Invalid string!
				{
					result = null;
					return (false);
				}
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out Enclosure result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = Enclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = Enclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_description))
			{
				result = Enclosure.Parentheses;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_description))
			{
				result = Enclosure.SquareBrackets;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_description))
			{
				result = Enclosure.CurlyBraces;
				return (true);
			}
			else // Invalid string!
			{
				result = new EnclosureEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator Enclosure(EnclosureEx enclosure)
		{
			return ((Enclosure)enclosure.UnderlyingEnum);
		}

		/// <remarks>
		/// Explicit because cast doesn't work for <see cref="Enclosure.Explicit"/>.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="enclosure"/> is <see cref="Enclosure.Explicit"/>.
		/// </exception>
		public static explicit operator EnclosureEx(Enclosure enclosure)
		{
			return (new EnclosureEx(enclosure));
		}

		/// <summary></summary>
		public static implicit operator string(EnclosureEx enclosure)
		{
			return (enclosure.ToString());
		}

		/// <summary></summary>
		public static implicit operator EnclosureEx(string enclosure)
		{
			return (Parse(enclosure));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
