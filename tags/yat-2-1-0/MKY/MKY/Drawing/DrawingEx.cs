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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Drawing
{
	/// <summary>
	/// Drawing utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DrawingEx
	{
		private static Font staticPreviousDefaultFont = SystemFonts.DefaultFont;
		private static Font staticDefaultFontItalicCache = new Font(SystemFonts.DefaultFont, FontStyle.Italic);

		/// <summary>
		/// Gets the <see cref="SystemFonts.DefaultFont"/> with <see cref="FontStyle.Italic"/>.
		/// </summary>
		/// <remarks>
		/// Same as retrieving <see cref="SystemFonts.DefaultFont"/>, retrieving this property can
		/// be a rather time-consuming operation, this property shall not be retrieved more needed.
		///
		/// Measurements 2019-09-02..05 in PredefinedCommandButtonSet around lines #490..505 at
		/// ~2062500 ticks per second, i.e. each tick ~0.5 us:
		/// <code>
		/// System.Diagnostics.Trace.WriteLine("FR = " + System.Diagnostics.Stopwatch.Frequency);
		/// </code>
		///
		/// Retrieving <see cref="SystemFonts.DefaultFont"/> for evaluation has been measured as
		/// 220 us to 1.1 ms to even 22 ms !?!
		/// <code>
		/// System.Diagnostics.Trace.WriteLine("DF = " + System.Diagnostics.Stopwatch.GetTimestamp());
		///
		/// if (someControl.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
		///     someControl.Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.
		///
		/// System.Diagnostics.Trace.WriteLine("DF = " + System.Diagnostics.Stopwatch.GetTimestamp());
		/// </code>
		///
		/// The same applies to <see cref="DefaultFontItalic"/>. Attempts to fix this by caching did not succeed:
		/// <code>
		/// System.Diagnostics.Trace.WriteLine("IT = " + System.Diagnostics.Stopwatch.GetTimestamp());
		///
		/// var defaultFontItalic = DrawingEx.UpdateCacheIfNameOrSizeHasChanged(ref staticDefaultFontItalicCache, SystemFonts.DefaultFont, FontStyle.Italic);
		///
		/// System.Diagnostics.Trace.WriteLine("IT = " + System.Diagnostics.Stopwatch.GetTimestamp());
		///
		/// if (someControl.Font != defaultFontItalic) // Improve performance by only assigning if different.
		///     someControl.Font = defaultFontItalic;  // Improves because 'Font' is managed by a 'PropertyStore'.
		///
		/// System.Diagnostics.Trace.WriteLine("IT = " + System.Diagnostics.Stopwatch.GetTimestamp());
		/// </code>
		///
		/// Solving issue by reducing the number of accesses to <see cref="SystemFonts.DefaultFont"/>
		/// and <see cref="DefaultFontItalic"/>, which is the better approach anyway.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		public static Font DefaultFontItalic
		{
			get
			{
				// Recreate font if system font has changed:
				if (staticPreviousDefaultFont != SystemFonts.DefaultFont)
				{
					staticPreviousDefaultFont = SystemFonts.DefaultFont;
					staticDefaultFontItalicCache = new Font(SystemFonts.DefaultFont, FontStyle.Italic);
				}

				return (staticDefaultFontItalicCache);
			}
		}

		/// <summary>
		/// Converts a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment">ContentAlignment to be converted.</param>
		/// <returns>Converted TextFormatFlags.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Emphasize that the text format is stored as flags.")]
		public static TextFormatFlags ConvertContentAlignmentToTextFormatFlags(ContentAlignment contentAlignment)
		{
			return (ApplyContentAlignmentToTextFormatFlags(contentAlignment));
		}

		/// <summary>
		/// Applies a <see cref="ContentAlignment"/> enum to a <see cref="TextFormatFlags"/> enum.
		/// Can be used for user drawn controls.
		/// </summary>
		/// <param name="contentAlignment">ContentAlignment to be converted.</param>
		/// <param name="textFormatFlags">TextFormatFlags to be changed.</param>
		/// <returns>Changed TextFormatFlags.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Emphasize that the text format is stored as flags.")]
		public static TextFormatFlags ApplyContentAlignmentToTextFormatFlags(ContentAlignment contentAlignment, TextFormatFlags textFormatFlags = TextFormatFlags.Default)
		{
			// ATTENTION
			// Do not care about TextFormatFlags.Top and TextFormatFlags.Left; their values are 0.

			// Clear alignment related flags.
		////textFormatFlags &= ~TextFormatFlags.Top;
			textFormatFlags &= ~TextFormatFlags.VerticalCenter;
			textFormatFlags &= ~TextFormatFlags.Bottom;

		////textFormatFlags &= ~TextFormatFlags.Left;
			textFormatFlags &= ~TextFormatFlags.HorizontalCenter;
			textFormatFlags &= ~TextFormatFlags.Right;

			// Set alignment related flags.
			switch (contentAlignment)
			{
				case ContentAlignment.TopLeft:
				////textFormatFlags |= TextFormatFlags.Top;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.MiddleLeft:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.BottomLeft:
					textFormatFlags |= TextFormatFlags.Bottom;
				////textFormatFlags |= TextFormatFlags.Left;
					break;

				case ContentAlignment.TopCenter:
				////textFormatFlags |= TextFormatFlags.Top;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.MiddleCenter:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.BottomCenter:
					textFormatFlags |= TextFormatFlags.Bottom;
					textFormatFlags |= TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.TopRight:
				////textFormatFlags |= TextFormatFlags.Top;
					textFormatFlags |= TextFormatFlags.Right;
					break;

				case ContentAlignment.MiddleRight:
					textFormatFlags |= TextFormatFlags.VerticalCenter;
					textFormatFlags |= TextFormatFlags.Right;
					break;

				case ContentAlignment.BottomRight:
					textFormatFlags |= TextFormatFlags.Bottom;
					textFormatFlags |= TextFormatFlags.Right;
					break;
			}
			return (textFormatFlags);
		}

		/// <summary>
		/// If the cached font doesn't exist, it is created.
		/// If the font properties have changed, the cached font is updated.
		/// </summary>
		/// <param name="cachedFont">The cached font.</param>
		/// <param name="fontName">Name of the font.</param>
		/// <param name="fontSize">Size of the font.</param>
		/// <param name="fontStyle">The font style.</param>
		/// <returns>The reference to the cached font.</returns>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Font is required to be received, modified and returned.")]
		public static Font UpdateCacheIfAnyHasChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle)
		{
			if (cachedFont == null)
			{
				// The font has not existed, create it:
				cachedFont = new Font(fontName, fontSize, fontStyle);
			}
			else if ((cachedFont.Name  != fontName) ||
			         (cachedFont.Size  != fontSize) ||
			         (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one:
				cachedFont.Dispose();
				cachedFont = new Font(fontName, fontSize, fontStyle);
			}

			return (cachedFont);
		}

		/// <summary>
		/// If the cached font doesn't exist, it is created.
		/// If the font properties have changed, the cached font is updated.
		/// </summary>
		/// <param name="cachedFont">The cached font.</param>
		/// <param name="fontNameAndSizeToEvaluate">The font to retrieve name and size from for evaluation of change.</param>
		/// <param name="fontStyleToApply">The font style to apply to the cached font.</param>
		/// <returns>The reference to the cached font.</returns>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Font is required to be received, modified and returned.")]
		public static Font UpdateCacheIfNameOrSizeHasChanged(ref Font cachedFont, Font fontNameAndSizeToEvaluate, FontStyle fontStyleToApply)
		{
			return (UpdateCacheIfNameOrSizeHasChanged(ref cachedFont, fontNameAndSizeToEvaluate.Name, fontNameAndSizeToEvaluate.Size, fontStyleToApply));
		}

		/// <summary>
		/// If the cached font doesn't exist, it is created.
		/// If the font properties have changed, the cached font is updated.
		/// </summary>
		/// <param name="cachedFont">The cached font.</param>
		/// <param name="fontNameToEvaluate">Name of the font for evaluation of change.</param>
		/// <param name="fontSizeToEvaluate">Size of the font for evaluation of change.</param>
		/// <param name="fontStyleToApply">The font style to apply to the cached font.</param>
		/// <returns>The reference to the cached font.</returns>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Font is required to be received, modified and returned.")]
		public static Font UpdateCacheIfNameOrSizeHasChanged(ref Font cachedFont, string fontNameToEvaluate, float fontSizeToEvaluate, FontStyle fontStyleToApply)
		{
			if (cachedFont == null)
			{
				// The font has not existed, create it:
				cachedFont = new Font(fontNameToEvaluate, fontSizeToEvaluate, fontStyleToApply);
			}
			else if ((cachedFont.Name != fontNameToEvaluate) ||
			         (cachedFont.Size != fontSizeToEvaluate))
			{
				// The font has changed, dispose of the cached font and create a new one:
				cachedFont.Dispose();
				cachedFont = new Font(fontNameToEvaluate, fontSizeToEvaluate, fontStyleToApply);
			}

			return (cachedFont);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
