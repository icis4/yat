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
// YAT Version 2.3.90 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum InfoEnclosure

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum InfoEnclosure
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
	public class InfoEnclosureEx : EnumEx, IEquatable<InfoEnclosureEx>
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

		/// <summary>Default is <see cref="InfoEnclosure.None"/>.</summary>
		public const InfoEnclosure Default = InfoEnclosure.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public InfoEnclosureEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="InfoEnclosure.Explicit"/> because that selection requires
		/// an enclosure string. Use <see cref="InfoEnclosureEx(string)"/> instead.
		/// </remarks>
		public InfoEnclosureEx(InfoEnclosure enclosure)
			: base(enclosure)
		{
			Debug.Assert((enclosure != InfoEnclosure.Explicit), "'InfoEnclosure.Explicit' requires an enclosure string, use 'InfoEnclosureEx(string)' instead!");
		}

		/// <summary></summary>
		public InfoEnclosureEx(string enclosure)
			: base(InfoEnclosure.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			this.explicitEnclosure = enclosure;
		}

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToEnclosure()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_enclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_enclosure);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_enclosure);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_enclosure);

				case InfoEnclosure.Explicit:       return (        this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureLeft()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_enclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_enclosureLeft);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_enclosureLeft);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_enclosureLeft);

				case InfoEnclosure.Explicit:       return (StringEx.Left(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureRight()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_enclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_enclosureRight);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_enclosureRight);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_enclosureRight);

				case InfoEnclosure.Explicit:       return (StringEx.Right(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_description);

				case InfoEnclosure.Parentheses:    return (   Parentheses_description);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_description);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_description);

				case InfoEnclosure.Explicit:       return (          this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				if ((InfoEnclosure)UnderlyingEnum == InfoEnclosure.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitEnclosure != null ? this.explicitEnclosure.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as InfoEnclosureEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(InfoEnclosureEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((InfoEnclosure)UnderlyingEnum == InfoEnclosure.Explicit)
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
		public static bool operator ==(InfoEnclosureEx lhs, InfoEnclosureEx rhs)
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
		public static bool operator !=(InfoEnclosureEx lhs, InfoEnclosureEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static InfoEnclosureEx[] GetItems()
		{
			var a = new List<InfoEnclosureEx>(4); // Preset the required capacity to improve memory management.

			a.Add(new InfoEnclosureEx(InfoEnclosure.None));
			a.Add(new InfoEnclosureEx(InfoEnclosure.Parentheses));
			a.Add(new InfoEnclosureEx(InfoEnclosure.SquareBrackets));
			a.Add(new InfoEnclosureEx(InfoEnclosure.CurlyBraces));

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
		public static InfoEnclosureEx Parse(string s)
		{
			InfoEnclosureEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid info element enclosure! String must be an even number of characters (e.g. 2 or 4 characters), or one of the predefined enclosures."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out InfoEnclosureEx result)
		{
			InfoEnclosure enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new InfoEnclosureEx(enumResult);
				return (true);
			}
			else
			{
				if ((s.Length % 2) == 0) // Valid explicit?
				{
					result = new InfoEnclosureEx(s);
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
		public static bool TryParse(string s, out InfoEnclosure result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = InfoEnclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = InfoEnclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_description))
			{
				result = InfoEnclosure.Parentheses;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_description))
			{
				result = InfoEnclosure.SquareBrackets;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_enclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_description))
			{
				result = InfoEnclosure.CurlyBraces;
				return (true);
			}
			else // Invalid string!
			{
				result = new InfoEnclosureEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator InfoEnclosure(InfoEnclosureEx enclosure)
		{
			return ((InfoEnclosure)enclosure.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator InfoEnclosureEx(InfoEnclosure enclosure)
		{
			return (new InfoEnclosureEx(enclosure));
		}

		/// <summary></summary>
		public static implicit operator string(InfoEnclosureEx enclosure)
		{
			return (enclosure.ToString());
		}

		/// <summary></summary>
		public static implicit operator InfoEnclosureEx(string enclosure)
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
