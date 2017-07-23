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
// MKY Version 1.0.20
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
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Properly scales a <see cref="SplitContainer"/> when DPI settings are changed.
	/// </summary>
	/// <remarks>
	/// Based on https://www.codeproject.com/Tips/786170/Proper-Resizing-of-SplitterContainer-Controls-at-a.
	/// </remarks>
	public class SplitContainerScaler
	{
		/// <summary>
		/// Gets or sets the current scale.
		/// </summary>
		public SizeF Scale { get; protected set; } = new SizeF(1.0f, 1.0f);

		/// <summary>
		/// Adjusts the scale by the given factor.
		/// </summary>
		public virtual void AdjustScale(SizeF factor)
		{
			Scale = new SizeF(Scale.Width * factor.Width, Scale.Height * factor.Height);
		}

		/// <summary>
		/// Scales all <see cref="SplitContainer"/> within the specified parent.
		/// </summary>
		public virtual void PerformScaling(Control parent)
		{
			foreach (Control child in parent.Controls)
			{
				var sc = child as SplitContainer;
				if (sc != null)
				{
					PerformScaling(sc);
					PerformScaling(sc.Panel1);
					PerformScaling(sc.Panel2);
				}
				else
				{
					PerformScaling(child);
				}
			}
		}

		/// <summary>
		/// Scales the specified <see cref="SplitContainer"/>.
		/// </summary>
		protected virtual void PerformScaling(SplitContainer sc)
		{
			float scaleOfContainer = ((sc.Orientation == Orientation.Vertical) ? Scale.Width : Scale.Height);

			if (sc.FixedPanel == FixedPanel.Panel1)
			{
				sc.SplitterDistance = (int)Math.Round(sc.SplitterDistance * scaleOfContainer);
			}
			else if (sc.FixedPanel == FixedPanel.Panel2)
			{
				int dimensionOfPanel2 = ((sc.Orientation == Orientation.Vertical) ? sc.Panel2.ClientSize.Width : sc.Panel2.ClientSize.Height);
				int dimensionOffset = (int)(dimensionOfPanel2 * scaleOfContainer);
				sc.SplitterDistance -= (dimensionOffset - dimensionOfPanel2);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
