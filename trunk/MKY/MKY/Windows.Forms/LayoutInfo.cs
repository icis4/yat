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

namespace MKY.Windows.Forms
{
	/// <remarks>
	/// Must be a reference type to ease handling within the layout dictionary.
	/// </remarks>
	public class LayoutInfo
	{
		/// <summary></summary>
		public float Left;

		/// <summary></summary>
		public float Top;

		/// <summary></summary>
		public float Width;

		/// <summary></summary>
		public float Height;

		/// <summary></summary>
		public float FontSize;

		/// <summary></summary>
		public LayoutInfo()
		{
		}

		/// <summary></summary>
		public LayoutInfo(float left, float top, float width, float height, float fontSize)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;

			FontSize = fontSize;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
