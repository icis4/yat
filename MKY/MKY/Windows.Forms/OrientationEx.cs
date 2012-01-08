//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
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
		public override string ToString()
		{
			switch ((Orientation)UnderlyingEnum)
			{
				case Orientation.Horizontal: return (Horizontal_string);
				case Orientation.Vertical:   return (Vertical_string);
				default: throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
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

		/// <summary></summary>
		public static OrientationEx Parse(string orientation)
		{
			OrientationEx result;

			if (TryParse(orientation, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("orientation", orientation, "Invalid orientation."));
		}

		/// <summary></summary>
		public static bool TryParse(string orientation, out OrientationEx result)
		{
			if      (StringEx.EqualsOrdinalIgnoreCase(orientation, Horizontal_string))
			{
				result = new OrientationEx(Orientation.Horizontal);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(orientation, Vertical_string))
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
