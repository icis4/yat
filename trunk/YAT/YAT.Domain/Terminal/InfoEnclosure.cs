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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
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

		private const string None_stringEnclosure                = "";
		private const string None_stringDescription              = "[None]";

		private const string Parentheses_stringEnclosure         = "()";
		private const string Parentheses_stringEnclosureLeft     = "(";
		private const string Parentheses_stringEnclosureRight    = ")";
		private const string Parentheses_stringDescription       = "Parentheses ()";

		private const string SquareBrackets_stringEnclosure      = "[]";
		private const string SquareBrackets_stringEnclosureLeft  = "[";
		private const string SquareBrackets_stringEnclosureRight = "]";
		private const string SquareBrackets_stringDescription    = "Brackets []";

		private const string CurlyBraces_stringEnclosure         = "{}";
		private const string CurlyBraces_stringEnclosureLeft     = "{";
		private const string CurlyBraces_stringEnclosureRight    = "}";
		private const string CurlyBraces_stringDescription       = "Braces {}";

		#endregion

		private string explicitEnclosure; // = null;

		/// <summary>Default is <see cref="InfoEnclosure.None"/>.</summary>
		public const InfoEnclosure Default = InfoEnclosure.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public InfoEnclosureEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public InfoEnclosureEx(InfoEnclosure enclosure)
			: base(enclosure)
		{
			if (enclosure == InfoEnclosure.Explicit)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'InfoEnclosure.Explicit' requires an enclosure string, use InfoElementEnclosureEx(string) instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public InfoEnclosureEx(string enclosure)
			: this(InfoEnclosure.Explicit)
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
				case InfoEnclosure.None:           return (          None_stringEnclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_stringEnclosure);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosure);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_stringEnclosure);

				case InfoEnclosure.Explicit:       return (        this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureLeft()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_stringEnclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_stringEnclosureLeft);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosureLeft);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_stringEnclosureLeft);

				case InfoEnclosure.Explicit:       return (StringEx.Left(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToEnclosureRight()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_stringEnclosure);

				case InfoEnclosure.Parentheses:    return (   Parentheses_stringEnclosureRight);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_stringEnclosureRight);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_stringEnclosureRight);

				case InfoEnclosure.Explicit:       return (StringEx.Right(this.explicitEnclosure, (this.explicitEnclosure.Length / 2)));

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoEnclosure)UnderlyingEnum)
			{
				case InfoEnclosure.None:           return (          None_stringDescription);

				case InfoEnclosure.Parentheses:    return (   Parentheses_stringDescription);
				case InfoEnclosure.SquareBrackets: return (SquareBrackets_stringDescription);
				case InfoEnclosure.CurlyBraces:    return (   CurlyBraces_stringDescription);

				case InfoEnclosure.Explicit:       return (          this.explicitEnclosure);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
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
			List<InfoEnclosureEx> a = new List<InfoEnclosureEx>(4); // Preset the required capacity to improve memory management.
			a.Add(new InfoEnclosureEx(InfoEnclosure.None));
			a.Add(new InfoEnclosureEx(InfoEnclosure.Parentheses));
			a.Add(new InfoEnclosureEx(InfoEnclosure.SquareBrackets));
			a.Add(new InfoEnclosureEx(InfoEnclosure.CurlyBraces));
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
				result = enumResult;
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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription))
			{
				result = InfoEnclosure.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Parentheses_stringDescription))
			{
				result = InfoEnclosure.Parentheses;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SquareBrackets_stringDescription))
			{
				result = InfoEnclosure.SquareBrackets;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringEnclosure) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CurlyBraces_stringDescription))
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
