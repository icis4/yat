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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace YAT.Gui.Utilities
{
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1115:ParameterMustFollowComma", Justification = "Too long for one line.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too long for one line.")]
	[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too long for one line.")]
	public static class Drawing
	{
		private struct DrawingElements
		{
			public Font Font;
			public SolidBrush Brush;
		}

		private static Font italicDefaultFont = new Font(SystemFonts.DefaultFont, FontStyle.Italic);

		/// <remarks>
		/// For performance reasons, cache fonts and brushes used for drawing.
		/// </remarks>
		private static DrawingElements txData      = new DrawingElements();
		private static DrawingElements txControl   = new DrawingElements();
		private static DrawingElements rxData      = new DrawingElements();
		private static DrawingElements rxControl   = new DrawingElements();
		private static DrawingElements timeStamp   = new DrawingElements();
		private static DrawingElements lineLength  = new DrawingElements();
		private static DrawingElements whiteSpaces = new DrawingElements();
		private static DrawingElements error       = new DrawingElements();

		/// <summary>String format used for drawing.</summary>
		private static StringFormat stringFormat = new StringFormat();

		static Drawing()
		{
			// Use GenericTypographic format to be able to measure characters indiviually,
			//   i.e. without a small margin before and after the character
			stringFormat = StringFormat.GenericTypographic;

			// Additionally enable trailing spaces to be able to correctly measure single spaces
			stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
		}

		public static Font ItalicDefaultFont
		{
			get
			{
				// Recreate font if system font has changed
				if ((italicDefaultFont.Name != SystemFonts.DefaultFont.Name) ||
					(italicDefaultFont.Size != SystemFonts.DefaultFont.Size))
				{
					italicDefaultFont = new Font(SystemFonts.DefaultFont, FontStyle.Italic);
				}

				return (italicDefaultFont);
			}
		}

		public static SizeF MeasureItem(Domain.DisplayLine line, Model.Settings.FormatSettings formatSettings,
										Graphics graphics, RectangleF bounds)
		{
			SizeF size = new SizeF(0, 0);
			float width = 0.0f;
			foreach (Domain.DisplayElement de in line)
			{
				size = MeasureItem(de, formatSettings, graphics, bounds);
				width += (float)size.Width;
			}
			return (new SizeF(width, size.Height));
		}

		public static SizeF MeasureItem(Domain.DisplayElement element, Model.Settings.FormatSettings formatSettings,
										Graphics graphics, RectangleF bounds)
		{
			Font font;
			Brush brush;
			SetDrawingItems(element, formatSettings, graphics, out font, out brush);

			return (graphics.MeasureString(element.Text, font, bounds.Size, stringFormat));
		}

		public static SizeF DrawItem(Domain.DisplayLine line, Model.Settings.FormatSettings formatSettings,
									 Graphics graphics, RectangleF bounds, DrawItemState state)
		{
			SizeF size = new SizeF(0, 0);
			float x = bounds.X;
			float width = bounds.Width;
			foreach (Domain.DisplayElement de in line)
			{
				size = DrawItem(de, formatSettings, graphics,
								new RectangleF(x, bounds.Y, width, bounds.Height),
								state);
				x += size.Width;
				width -= size.Width;
			}
			return (new SizeF(x - bounds.X, bounds.Height));
		}

		public static SizeF DrawItem(Domain.DisplayElement element, Model.Settings.FormatSettings formatSettings,
									 Graphics graphics, RectangleF bounds, DrawItemState state)
		{
			Font font;
			Brush brush;
			SetDrawingItems(element, formatSettings, graphics, out font, out brush);

			// select the highlight brush if the item is selected
			if ((state & DrawItemState.Selected) == DrawItemState.Selected)
				brush = SystemBrushes.HighlightText;

			// perform the painting
			graphics.DrawString(element.Text, font, brush, bounds, stringFormat);

			// measure consumed rectangle
			return (graphics.MeasureString(element.Text, font, bounds.Size, stringFormat));
		}

		private static void SetDrawingItems(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                                    Graphics graphics, out Font font, out Brush brush)
		{
			string fontName = settings.Font.Name;
			float fontSize  = settings.Font.Size;
			FontStyle fontStyle;
			Color fontColor;

			if (element is Domain.DisplayElement.TxData)
			{
				fontStyle = settings.TxDataFormat.FontStyle;
				fontColor = settings.TxDataFormat.Color;
				font  = SetFont (ref txData.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref txData.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.FontStyle;
				fontColor = settings.TxControlFormat.Color;
				font  = SetFont (ref txControl.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref txControl.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.FontStyle;
				fontColor = settings.RxDataFormat.Color;
				font  = SetFont (ref rxData.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref rxData.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.FontStyle;
				fontColor = settings.RxControlFormat.Color;
				font  = SetFont (ref rxControl.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref rxControl.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TimeStamp)
			{
				fontStyle = settings.TimeStampFormat.FontStyle;
				fontColor = settings.TimeStampFormat.Color;
				font  = SetFont (ref timeStamp.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref timeStamp.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.LineLength)
			{
				fontStyle = settings.LengthFormat.FontStyle;
				fontColor = settings.LengthFormat.Color;
				font  = SetFont (ref lineLength.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref lineLength.Brush, fontColor);
			}
			else if ((element is Domain.DisplayElement.LeftMargin) ||
					 (element is Domain.DisplayElement.Space) ||
					 (element is Domain.DisplayElement.RightMargin) ||
					 (element is Domain.DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				fontColor = settings.WhiteSpacesFormat.Color;
				font  = SetFont (ref whiteSpaces.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref whiteSpaces.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.Error)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				fontColor = settings.ErrorFormat.Color;
				font  = SetFont (ref error.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref error.Brush, fontColor);
			}
			else
			{
				throw (new NotImplementedException("Unknown DisplayElement"));
			}
		}

		private static Font SetFont(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
		{
			// Create the font
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);
				SetTabStops(cachedFont, graphics);
			}
			else if ((cachedFont.Name  != fontName) ||
					 (cachedFont.Size  != fontSize) ||
					 (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one
				cachedFont.Dispose();
				cachedFont = new Font(fontName, fontSize, fontStyle);

				// Also set tab stops accordingly
				SetTabStops(cachedFont, graphics);
			}
			return (cachedFont);
		}

		private static void SetTabStops(Font font, Graphics graphics)
		{
			// Calculate tabs, currently fixed to 8 characters

			// \remind 2009-08-29 / mky
			// This is a somewhat strange calculation, however, don't know to do it better.

			SizeF size = graphics.MeasureString(" ", font);
			float[] tabStops = new float[256];
			for (int i = 0; i < 256; i++)
				tabStops[i] = size.Width * 14.5f;
			stringFormat.SetTabStops(0, tabStops);
		}

		private static SolidBrush SetBrush(ref SolidBrush cachedBrush, Color color)
		{
			// Create the brush using the font color
			if (cachedBrush == null)
			{
				cachedBrush = new SolidBrush(color);
			}
			else if (cachedBrush.Color.ToArgb() != color.ToArgb())
			{
				// The font color has changed, dispose of the cached brush and create a new one
				cachedBrush.Dispose();
				cachedBrush = new SolidBrush(color);
			}
			return (cachedBrush);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
