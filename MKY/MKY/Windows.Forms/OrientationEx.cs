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
// MKY Version 1.0.29
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
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Extended enum OrientationEx which extends <see cref="Orientation"/>.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class OrientationEx : EnumEx
	{
		#region String Definitions

		private const string Horizontal_string = "Horizontal";
		private const string Vertical_string = "Vertical";

		#endregion

		/// <summary>Default is <see cref="Orientation.Horizontal"/>.</summary>
		public const Orientation Default = Orientation.Horizontal;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public OrientationEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public OrientationEx(Orientation orientation)
			: base(orientation)
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
			switch ((Orientation)UnderlyingEnum)
			{
				case Orientation.Horizontal: return (Horizontal_string);
				case Orientation.Vertical:   return (Vertical_string);

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
		public static OrientationEx[] GetItems()
		{
			var a = new List<OrientationEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new OrientationEx(Orientation.Horizontal));
			a.Add(new OrientationEx(Orientation.Vertical));

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
		public static OrientationEx Parse(string s)
		{
			OrientationEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid orientation string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out OrientationEx result)
		{
			Orientation enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new OrientationEx(enumResult);
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
		public static bool TryParse(string s, out Orientation result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Horizontal_string))
			{
				result = Orientation.Horizontal;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Vertical_string))
			{
				result = Orientation.Vertical;
				return (true);
			}
			else // Invalid string!
			{
				result = new OrientationEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region WidthOrHeight
		//==========================================================================================
		// WidthOrHeight
		//==========================================================================================

		/// <summary>
		/// Gets <see cref="Control.Width"/> or <see cref="Control.Height"/> of <paramref name="control"/>,
		/// depending on <see cref="SplitContainer.Orientation"/> of <paramref name="orientation"/>.
		/// </summary>
		public static int SizeToWidthOrHeight(Control control, Orientation orientation)
		{
			return (SizeToWidthOrHeight(control.Size, orientation));
		}

		/// <summary>
		/// Gets <see cref="Size.Width"/> or <see cref="Size.Height"/> of <paramref name="size"/>,
		/// depending on <see cref="SplitContainer.Orientation"/> of <paramref name="orientation"/>.
		/// </summary>
		public static int SizeToWidthOrHeight(Size size, Orientation orientation)
		{
			return ((orientation == Orientation.Vertical) ? (size.Width) : (size.Height));
		}

		/// <summary>
		/// Gets <see cref="SizeF.Width"/> or <see cref="SizeF.Height"/> of <paramref name="size"/>,
		/// depending on <see cref="SplitContainer.Orientation"/> of <paramref name="orientation"/>.
		/// </summary>
		public static float SizeToWidthOrHeight(SizeF size, Orientation orientation)
		{
			return ((orientation == Orientation.Vertical) ? (size.Width) : (size.Height));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
