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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace YAT.Gui.Utilities
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too long for one line.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too long for one line.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too long for one line.")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
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
		private static DrawingElements neutral     = new DrawingElements();
		private static DrawingElements txData      = new DrawingElements();
		private static DrawingElements txControl   = new DrawingElements();
		private static DrawingElements rxData      = new DrawingElements();
		private static DrawingElements rxControl   = new DrawingElements();
		private static DrawingElements timeStamp   = new DrawingElements();
		private static DrawingElements lineLength  = new DrawingElements();
		private static DrawingElements whiteSpaces = new DrawingElements();
		private static DrawingElements error       = new DrawingElements();

		/// <summary>String format used for drawing line numbers.</summary>
		private static StringFormat lineNumberStringFormat;

		/// <summary>String format used for drawing monitor strings.</summary>
		private static StringFormat monitorDrawingStringFormat;

		/// <summary>String format used for measuring monitor strings.</summary>
		private static StringFormat monitorVirtualStringFormat;

		/// <summary>
		/// Use GenericTypographic format to be able to measure characters individually,
		///   i.e. without a small margin before and after the character.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Hmm... How can the logic below be implemented in the initializer?")]
		static Drawing()
		{
			// Line numbers shall be aligned right.
			lineNumberStringFormat = new StringFormat(StringFormat.GenericDefault);
			lineNumberStringFormat.Alignment = StringAlignment.Far;

			// Enable trailing spaces to be able to correctly measure single spaces.
			// Also enable drawing of text that exceeds the layout rectangle.
			monitorDrawingStringFormat = new StringFormat(StringFormat.GenericTypographic);
			monitorDrawingStringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			monitorDrawingStringFormat.Trimming = StringTrimming.EllipsisCharacter;

			// Do not trim to measure the size actually requested.
			monitorVirtualStringFormat = new StringFormat(StringFormat.GenericTypographic);
			monitorVirtualStringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			monitorVirtualStringFormat.Trimming = StringTrimming.None;
		}

		/// <summary></summary>
		public static Font ItalicDefaultFont
		{
			get
			{
				// Recreate font if system font has changed.
				if ((italicDefaultFont.Name != SystemFonts.DefaultFont.Name) ||
					(italicDefaultFont.Size != SystemFonts.DefaultFont.Size))
				{
					italicDefaultFont = new Font(SystemFonts.DefaultFont, FontStyle.Italic);
				}

				return (italicDefaultFont);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLineNumberString(string s, Model.Settings.FormatSettings formatSettings,
		                                                  Graphics graphics, RectangleF bounds,
		                                                  out SizeF requestedSize)
		{
			Font font;
			Brush brush;
			SetNeutralDrawingItems(formatSettings, graphics, out font, out brush);

			graphics.DrawString(s, font, brush, bounds, lineNumberStringFormat);

			requestedSize = graphics.MeasureString(s, font, int.MaxValue, lineNumberStringFormat);
		}

		private static void SetNeutralDrawingItems(Model.Settings.FormatSettings settings,
		                                           Graphics graphics, out Font font, out Brush brush)
		{
			string    fontName  = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle = FontStyle.Regular;
			Color     fontColor = SystemColors.ControlText;

			font  = SetFont (ref neutral.Font, fontName, fontSize, fontStyle, graphics);
			brush = SetBrush(ref neutral.Brush, fontColor);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureItem(Domain.DisplayLine line, Model.Settings.FormatSettings formatSettings,
		                                      Graphics graphics, RectangleF bounds, DrawItemState state,
		                                      out SizeF requestedSize, out SizeF drawnSize)
		{
			float requestedWidth = 0;
			float drawnWidth = 0;
			foreach (Domain.DisplayElement de in line)
			{
				SizeF requestedElementSize;
				SizeF drawnElementSize;
				DrawAndMeasureItem(de, formatSettings, graphics,
				                   new RectangleF(bounds.X + drawnWidth, bounds.Y, bounds.Width - drawnWidth, bounds.Height),
				                   state, out requestedElementSize, out drawnElementSize);
				requestedWidth += requestedElementSize.Width;
				drawnWidth     += drawnElementSize.Width;
			}
			requestedSize = new SizeF(requestedWidth, bounds.Height);
			drawnSize     = new SizeF(drawnWidth, bounds.Height);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureItem(Domain.DisplayElement element, Model.Settings.FormatSettings formatSettings,
		                                      Graphics graphics, RectangleF bounds, DrawItemState state,
		                                      out SizeF requestedSize, out SizeF drawnSize)
		{
			Font font;
			Brush brush;
			SetDrawingItems(element, formatSettings, graphics, out font, out brush);

			// Select the highlight brush if the item is selected.
			if ((state & DrawItemState.Selected) == DrawItemState.Selected)
				brush = SystemBrushes.HighlightText;

			// Perform drawing.
			graphics.DrawString(element.Text, font, brush, bounds, monitorDrawingStringFormat);

			// Measure consumed rectangle: Requested virtual and effectively drawn.
			requestedSize = graphics.MeasureString(element.Text, font, int.MaxValue, monitorVirtualStringFormat);
			drawnSize     = graphics.MeasureString(element.Text, font, bounds.Size, monitorDrawingStringFormat);
		}

		private static void SetDrawingItems(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                                    Graphics graphics, out Font font, out Brush brush)
		{
			string fontName = settings.Font.Name;
			float fontSize  = settings.Font.Size;
			FontStyle fontStyle;
			Color fontColor;

			if      (element is Domain.DisplayElement.TxData)
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
			else if (element is Domain.DisplayElement.IOError)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				fontColor = settings.ErrorFormat.Color;
				font  = SetFont (ref error.Font, fontName, fontSize, fontStyle, graphics);
				brush = SetBrush(ref error.Brush, fontColor);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Unknown DisplayElement:");
				sb.Append(element.ToString());
				throw (new NotImplementedException(sb.ToString()));
			}
		}

		private static Font SetFont(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
		{
			// Create the font.
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);
				SetTabStops(cachedFont, graphics);
			}
			else if ((cachedFont.Name  != fontName) ||
			         (cachedFont.Size  != fontSize) ||
			         (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one.
				cachedFont.Dispose();
				cachedFont = new Font(fontName, fontSize, fontStyle);

				// Also set tab stops accordingly.
				SetTabStops(cachedFont, graphics);
			}
			return (cachedFont);
		}

		private static void SetTabStops(Font font, Graphics graphics)
		{
			// Calculate tabs, currently fixed to 8 characters.

			// \remind (2009-08-29 / mky):
			// This is a somewhat strange calculation, however, don't know to do it better.

			SizeF size = graphics.MeasureString(" ", font);
			float[] tabStops = new float[256];
			
			for (int i = 0; i < 256; i++)
				tabStops[i] = size.Width * 14.5f;

			monitorDrawingStringFormat.SetTabStops(0, tabStops);
			monitorVirtualStringFormat.SetTabStops(0, tabStops);
		}

		private static SolidBrush SetBrush(ref SolidBrush cachedBrush, Color color)
		{
			// Create the brush using the font color.
			if (cachedBrush == null)
			{
				cachedBrush = new SolidBrush(color);
			}
			else if (cachedBrush.Color.ToArgb() != color.ToArgb())
			{
				// The font color has changed, dispose of the cached brush and create a new one.
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
