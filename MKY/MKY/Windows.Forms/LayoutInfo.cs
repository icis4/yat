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
// MKY Version 1.0.27
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

namespace MKY.Windows.Forms
{
	/// <remarks>
	/// Must be a reference type to ease handling within the layout dictionary.
	/// </remarks>
	public class LayoutInfo
	{
		/// <summary></summary>
		public float Left { get; set; }

		/// <summary></summary>
		public float Top { get; set; }

		/// <summary></summary>
		public float Width { get; set; }

		/// <summary></summary>
		public float Height { get; set; }

		/// <summary></summary>
		public float FontSize { get; set; }

		/// <summary></summary>
		public LayoutInfo()
		{
		}

		/// <summary></summary>
		public LayoutInfo(float left, float top, float width, float height, float fontSize)
		{
			Left   = left;
			Top    = top;
			Width  = width;
			Height = height;

			FontSize = fontSize;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
