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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Properly scales a <see cref="SplitContainer"/> when DPI settings are changed.
	/// </summary>
	/// <remarks>
	/// Based on https://www.codeproject.com/Tips/786170/Proper-Resizing-of-SplitterContainer-Controls-at-a.
	/// </remarks>
	public class SplitContainerHelper : SizeHelper
	{
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
					int limitedDistance;
					if (TryLimitSplitterDistance(sc, scaledDistance, out limitedDistance))
					{
						if (sc.SplitterDistance != limitedDistance)
							sc.SplitterDistance = limitedDistance;
					}
				#if DEBUG
					else
					{
						Debugger.Break(); // See debug output for issue and instructions!
					}
				#endif

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

		/// <summary>
		/// Limits <paramref name="distance"/> according to the preconditions of the
		/// <see cref="SplitContainer.SplitterDistance"/> property.
		/// </summary>
		public static bool TryLimitSplitterDistance(SplitContainer sc, int distance, out int limited)
		{
			int widthOrHeight = OrientationEx.SizeToWidthOrHeight(sc, sc.Orientation);

			if (widthOrHeight <= 0) // Case e.g. if the form/control is hidden.
			{                       // Needed to prevent 'ArgumentException' at Int32Ex.Limit() below.
				Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Size should be larger than 0, but is {0}. This may happen if '{1}' is hidden.", widthOrHeight, sc.Name));
				limited = 0;
				return (false);
			}

			int min =                       (sc.Panel1Collapsed ? 0 : sc.Panel1MinSize);
			int max = (widthOrHeight - 1) - (sc.Panel2Collapsed ? 0 : sc.Panel2MinSize);

			min = Int32Ex.Limit(min, 0, (widthOrHeight - 1)); // Both values must be 0 or above but
			max = Int32Ex.Limit(max, 0, (widthOrHeight - 1)); // not above the size of the control.

			if (min <= max) // Normal case if everything is OK:
			{
				limited = Int32Ex.Limit(distance, min, max);
				return (true);
			}
			else // Case e.g. if there are mistakes in the layout of a form.
			{    // Needed to prevent 'ArgumentException' at Int32Ex.Limit().
				Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Result should be 'min' <= 'max', but 'min' is {0} and 'max' is {1}! Check the layout and resulting size of '{2}'!", min, max, sc.Name));
				limited = 0;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
