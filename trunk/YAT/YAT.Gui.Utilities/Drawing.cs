//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		private struct DrawingObjects
		{
			public Font Font;
			public SolidBrush Brush;
		}

		/// <remarks>
		/// For performance reasons, cache elements such as fonts and brushes used for drawing.
		/// </remarks>
		private static DrawingObjects lineNumberObjects;
		private static DrawingObjects txDataObjects;
		private static DrawingObjects txControlObjects;
		private static DrawingObjects rxDataObjects;
		private static DrawingObjects rxControlObjects;
		private static DrawingObjects dateObjects;
		private static DrawingObjects timeObjects;
		private static DrawingObjects portObjects;
		private static DrawingObjects directionObjects;
		private static DrawingObjects lengthObjects;
		private static DrawingObjects whiteSpacesObjects;
		private static DrawingObjects errorObjects;

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
			// Line numbers shall be aligned right:
			lineNumberStringFormat = new StringFormat(StringFormat.GenericDefault);
			lineNumberStringFormat.Alignment = StringAlignment.Far;

			// Enable trailing spaces to be able to correctly measure single spaces.
			// Also enable drawing of text that exceeds the layout rectangle:
			monitorDrawingStringFormat = new StringFormat(StringFormat.GenericTypographic);
			monitorDrawingStringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			monitorDrawingStringFormat.Trimming = StringTrimming.EllipsisCharacter;

			// Do not trim to measure the size actually requested:
			monitorVirtualStringFormat = new StringFormat(StringFormat.GenericTypographic);
			monitorVirtualStringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			monitorVirtualStringFormat.Trimming = StringTrimming.None;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLineNumberString(string s, Model.Settings.FormatSettings settings,
		                                                  Graphics graphics, RectangleF bounds,
		                                                  out SizeF requestedSize)
		{
			Font font;
			Brush brush;
			SetLineNumberDrawingObjects(settings, graphics, out font, out brush);

			graphics.DrawString(s, font, brush, bounds, lineNumberStringFormat);

			requestedSize = graphics.MeasureString(s, font, int.MaxValue, lineNumberStringFormat);
		}

		/// <remarks>
		/// Line numbers shall be formatted the same as 'normal' Windows.Forms control text. This format
		/// is only used here, and it is not contained in the <see cref="Model.Settings.FormatSettings"/>.
		/// </remarks>
		private static void SetLineNumberDrawingObjects(Model.Settings.FormatSettings settings,
		                                                Graphics graphics, out Font font, out Brush brush)
		{
			string    fontName  = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle = FontStyle.Regular;
			Color     fontColor = SystemColors.ControlText;

			font  = AssignIfChanged (ref lineNumberObjects.Font, fontName, fontSize, fontStyle, graphics);
			brush = AssignIfChanged(ref lineNumberObjects.Brush, fontColor);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureItem(Domain.DisplayLine line, Model.Settings.FormatSettings settings,
		                                      Graphics graphics, RectangleF bounds, DrawItemState state,
		                                      out SizeF requestedSize, out SizeF drawnSize)
		{
			float requestedWidth = 0;
			float drawnWidth = 0;
			foreach (Domain.DisplayElement de in line)
			{
				SizeF requestedElementSize;
				SizeF drawnElementSize;
				DrawAndMeasureItem(de, settings, graphics,
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
		public static void DrawAndMeasureItem(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                                      Graphics graphics, RectangleF bounds, DrawItemState state,
		                                      out SizeF requestedSize, out SizeF drawnSize)
		{
			if (!string.IsNullOrEmpty(element.Text))
			{
				Font font;
				Brush brush;
				SetDrawingObjects(element, settings, graphics, out font, out brush);

				// Select the highlight brush if the item is selected:
				if ((state & DrawItemState.Selected) == DrawItemState.Selected)
					brush = SystemBrushes.HighlightText;

				// Perform drawing of text:
				graphics.DrawString(element.Text, font, brush, bounds, monitorDrawingStringFormat);

				// Measure consumed rectangle: Requested virtual and effectively drawn:
				requestedSize = graphics.MeasureString(element.Text, font, int.MaxValue, monitorVirtualStringFormat);
				drawnSize     = graphics.MeasureString(element.Text, font, bounds.Size, monitorDrawingStringFormat);
			}
			else
			{
				requestedSize = new SizeF();
				drawnSize     = new SizeF();
			}
		}

		private static void SetDrawingObjects(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
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
				font  = AssignIfChanged(ref txDataObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref txDataObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.FontStyle;
				fontColor = settings.TxControlFormat.Color;
				font  = AssignIfChanged(ref txControlObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref txControlObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.FontStyle;
				fontColor = settings.RxDataFormat.Color;
				font  = AssignIfChanged(ref rxDataObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref rxDataObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.FontStyle;
				fontColor = settings.RxControlFormat.Color;
				font  = AssignIfChanged(ref rxControlObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref rxControlObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				fontStyle = settings.DateFormat.FontStyle;
				fontColor = settings.DateFormat.Color;
				font  = AssignIfChanged(ref dateObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref dateObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				fontStyle = settings.TimeFormat.FontStyle;
				fontColor = settings.TimeFormat.Color;
				font  = AssignIfChanged (ref timeObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref timeObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.PortInfo)
			{
				fontStyle = settings.PortFormat.FontStyle;
				fontColor = settings.PortFormat.Color;
				font  = AssignIfChanged(ref portObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref portObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.DirectionInfo)
			{
				fontStyle = settings.DirectionFormat.FontStyle;
				fontColor = settings.DirectionFormat.Color;
				font  = AssignIfChanged(ref directionObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref directionObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.Length)
			{
				fontStyle = settings.LengthFormat.FontStyle;
				fontColor = settings.LengthFormat.Color;
				font  = AssignIfChanged(ref lengthObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref lengthObjects.Brush, fontColor);
			}
			else if ((element is Domain.DisplayElement.NoData) ||
			         (element is Domain.DisplayElement.LeftMargin) ||
			         (element is Domain.DisplayElement.Space) ||
			         (element is Domain.DisplayElement.RightMargin) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				fontColor = settings.WhiteSpacesFormat.Color;
				font  = AssignIfChanged(ref whiteSpacesObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref whiteSpacesObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.ErrorInfo)
			{
				fontStyle = settings.ErrorFormat.FontStyle;
				fontColor = settings.ErrorFormat.Color;
				font  = AssignIfChanged(ref errorObjects.Font,  fontName, fontSize, fontStyle, graphics);
				brush = AssignIfChanged(ref errorObjects.Brush, fontColor);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Unknown DisplayElement:");
				sb.Append(element.ToString());
				throw (new NotImplementedException(sb.ToString()));
			}
		}

		private static Font AssignIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
		{
			// Create the font.
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);

				// Also set tab stops accordingly.
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

		private static SolidBrush AssignIfChanged(ref SolidBrush cachedBrush, Color color)
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
