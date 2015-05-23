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
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Extended enum OrientationEx which extends <see cref="Orientation"/>.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[Serializable]
	public class OrientationEx : EnumEx
	{
		#region String Definitions

		private const string Horizontal_string = "Horizontal";
		private const string Vertical_string = "Vertical";

		#endregion

		/// <summary>Default is <see cref="Orientation.Horizontal"/>.</summary>
		public OrientationEx()
			: base(Orientation.Horizontal)
		{
		}

		/// <summary></summary>
		protected OrientationEx(Orientation orientation)
			: base(orientation)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((Orientation)UnderlyingEnum)
			{
				case Orientation.Horizontal: return (Horizontal_string);
				case Orientation.Vertical:   return (Vertical_string);
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static OrientationEx[] GetItems()
		{
			List<OrientationEx> a = new List<OrientationEx>();
			a.Add(new OrientationEx(Orientation.Horizontal));
			a.Add(new OrientationEx(Orientation.Vertical));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static OrientationEx Parse(string s)
		{
			OrientationEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid orientation string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out OrientationEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Horizontal_string))
			{
				result = new OrientationEx(Orientation.Horizontal);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Vertical_string))
			{
				result = new OrientationEx(Orientation.Vertical);
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
		public static implicit operator Orientation(OrientationEx orientation)
		{
			return ((Orientation)orientation.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator OrientationEx(Orientation orientation)
		{
			return (new OrientationEx(orientation));
		}

		/// <summary></summary>
		public static implicit operator int(OrientationEx orientation)
		{
			return (orientation.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator OrientationEx(int orientation)
		{
			return (new OrientationEx((Orientation)orientation));
		}

		/// <summary></summary>
		public static implicit operator string(OrientationEx orientation)
		{
			return (orientation.ToString());
		}

		/// <summary></summary>
		public static implicit operator OrientationEx(string orientation)
		{
			return (Parse(orientation));
		}

		#endregion
	}
}
