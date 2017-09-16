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

using System.Diagnostics.CodeAnalysis;
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
	public class SplitContainerHelper
	{
		/// <summary>
		/// Gets or sets the current scale.
		/// </summary>
		protected SizeF Scale { get; set; } = new SizeF(1.0f, 1.0f);

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
					// Adjust splitter distance:
					int scaledDistance = CalculateScaledDistanceFromUnscaled(sc, sc.SplitterDistance);
					if (sc.SplitterDistance != scaledDistance)
					{
						int widthOrHeight = OrientationEx.SizeToWidthOrHeight(sc, sc.Orientation);
						sc.SplitterDistance = Int32Ex.Limit(scaledDistance, 0, (widthOrHeight - 1));
					}

					// Continue with the panels:
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
		/// Calculates the scaled splitter distance from the unscaled splitter distance.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'unscaled' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unscaled", Justification = "'unscaled' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "unscaled", Justification = "'unscaled' is a correct English term.")]
		public virtual int CalculateScaledDistanceFromUnscaled(SplitContainer sc, int unscaledDistance)
		{
			float scaleOfContainer = OrientationEx.SizeToWidthOrHeight(Scale, sc.Orientation);

			if (sc.FixedPanel == FixedPanel.Panel1)
			{
				return ((int)((unscaledDistance * scaleOfContainer) + 0.5f)); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
			}
			else if (sc.FixedPanel == FixedPanel.Panel2)
			{
				int widthOrHeightOfPanel2 = OrientationEx.SizeToWidthOrHeight(sc.Panel2.ClientSize, sc.Orientation);
				int widthOrHeightOffset = (int)((widthOrHeightOfPanel2 * scaleOfContainer) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
				return (unscaledDistance - (widthOrHeightOffset - widthOrHeightOfPanel2));
			}
			else // No fixed panel:
			{
				return (unscaledDistance);
			}
		}

		/// <summary>
		/// Calculates the scaled splitter distance from the unscaled splitter distance.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'unscaled' is a correct English term.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unscaled", Justification = "'unscaled' is a correct English term.")]
		public virtual int CalculateUnscaledDistanceFromScaled(SplitContainer sc, int scaledDistance)
		{
			float scaleOfContainer = OrientationEx.SizeToWidthOrHeight(Scale, sc.Orientation);

			if (sc.FixedPanel == FixedPanel.Panel1)
			{
				return ((int)((scaledDistance / scaleOfContainer) + 0.5f)); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
			}
			else if (sc.FixedPanel == FixedPanel.Panel2)
			{
				int widthOrHeightOfPanel2 = OrientationEx.SizeToWidthOrHeight(sc.Panel2.ClientSize, sc.Orientation);
				int widthOrHeightOffset = (int)((widthOrHeightOfPanel2 / scaleOfContainer) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
				return (scaledDistance - (widthOrHeightOffset - widthOrHeightOfPanel2));
			}
			else // No fixed panel:
			{
				return (scaledDistance);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
