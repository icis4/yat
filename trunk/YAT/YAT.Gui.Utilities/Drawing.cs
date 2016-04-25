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

using MKY.Diagnostics;

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
		private static DrawingObjects staticLineNumberObjects;
		private static DrawingObjects staticTxDataObjects;
		private static DrawingObjects staticTxControlObjects;
		private static DrawingObjects staticRxDataObjects;
		private static DrawingObjects staticRxControlObjects;
		private static DrawingObjects staticDateObjects;
		private static DrawingObjects staticTimeObjects;
		private static DrawingObjects staticPortObjects;
		private static DrawingObjects staticDirectionObjects;
		private static DrawingObjects staticLengthObjects;
		private static DrawingObjects staticWhiteSpacesObjects;
		private static DrawingObjects staticErrorObjects;

		/// <summary>String format used for drawing line numbers.</summary>
		private static StringFormat staticLineNumberFormat;

		/// <summary>String format used for drawing monitor strings.</summary>
		private static StringFormat staticMonitorDrawingFormat;

		/// <summary>String format used for measuring monitor strings.</summary>
		private static StringFormat staticMonitorRequestingFormat;

		/// <summary>
		/// Use GenericTypographic format to be able to measure characters individually,
		///   i.e. without a small margin before and after the character.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Hmm... How can the logic below be implemented in the initializer?")]
		static Drawing()
		{
			// Line numbers shall be aligned right:
			staticLineNumberFormat = new StringFormat(StringFormat.GenericDefault);
			staticLineNumberFormat.Alignment = StringAlignment.Far;

			// Enable trailing spaces to be able to correctly measure single spaces.
			// Also enable drawing ellipsis if the text exceeds the layout rectangle:
			staticMonitorDrawingFormat = new StringFormat(StringFormat.GenericTypographic);
			staticMonitorDrawingFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			staticMonitorDrawingFormat.Trimming = StringTrimming.EllipsisCharacter;

			// Do not trim to measure the size actually requested:
			staticMonitorRequestingFormat = new StringFormat(StringFormat.GenericTypographic);
			staticMonitorRequestingFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			staticMonitorRequestingFormat.Trimming = StringTrimming.None;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLineNumber(string s, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                            Graphics graphics, RectangleF bounds,
		                                            out float requestedWidth)
		{
			Font font;
			Brush brush;
			SetLineNumberDrawingObjects(settings, graphics, out font, out brush);

			if (rightToLeft == RightToLeft.Yes)
				staticLineNumberFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			else
				staticLineNumberFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;

			graphics.DrawString(s, font, brush, bounds, staticLineNumberFormat);

			SizeF requestedSize = graphics.MeasureString(s, font, int.MaxValue, staticLineNumberFormat);
			requestedWidth = requestedSize.Width;
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

			font  = SetIfChanged(ref staticLineNumberObjects.Font, fontName, fontSize, fontStyle, graphics);
			brush = SetIfChanged(ref staticLineNumberObjects.Brush, fontColor);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLine(Domain.DisplayLine line, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                      Graphics graphics, RectangleF bounds, DrawItemState state,
		                                      out float requestedWidth, out float drawnWidth)
		{
			requestedWidth = 0.0f;
			drawnWidth     = 0.0f;

			foreach (Domain.DisplayElement de in line)
			{
				float requestedElementWidth;
				float drawnElementWidth;

				DrawAndMeasureElement(de, settings, rightToLeft, graphics,
				                      new RectangleF(bounds.X + drawnWidth, bounds.Y, bounds.Width - drawnWidth, bounds.Height),
				                      state, out requestedElementWidth, out drawnElementWidth);

				requestedWidth += requestedElementWidth;
				drawnWidth     += drawnElementWidth;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureElement(Domain.DisplayElement element, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                         Graphics graphics, RectangleF bounds, DrawItemState state,
		                                         out float requestedWidth, out float drawnWidth)
		{
			if (!string.IsNullOrEmpty(element.Text))
			{
				Font font;
				Brush brush;
				SetDrawingObjects(element, settings, graphics, out font, out brush);

				// Override to highlight brush if the item is selected:
				if ((state & DrawItemState.Selected) == DrawItemState.Selected)
					brush = SystemBrushes.HighlightText;

				if (rightToLeft == RightToLeft.Yes)
				{
					staticMonitorRequestingFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					staticMonitorDrawingFormat   .FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				}
				else
				{
					staticMonitorRequestingFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
					staticMonitorDrawingFormat   .FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
				}

				// Perform drawing of text:
				try
				{
					graphics.DrawString(element.Text, font, brush, bounds, staticMonitorDrawingFormat);
				}
				catch (System.Runtime.InteropServices.ExternalException ex)
				{
					DebugEx.WriteException(typeof(Drawing), ex);

					// Note that this exception occasionally happens and also has been reported:
					//  > #191 "General error in GDI+ in Drawing.DrawAndMeasureItem()"
					//  > #266 "Hitting unhandled synchronous in GDI+ in Drawing.DrawAndMeasureItem()"
					//  > #284 "Retrieving a large block of data causes exception"
					//  > #286 "Exception if terminal receives data with wrong baudrate"
					//  > #325 "Extremely long line crash"
					//
					// The 'ExternalException' states "A generic error occurred in GDI+" and happens
					// at Graphics.DrawString() at Graphics.CheckErrorStatus(). This error doesn't
					// seem to make any sense and seems to happen mainly or only when sending long
					// lines. Spent several hours trying to find the root cause, without succeess.
					// Thus, handling the exception here. In addition, manualy stress test case
					// "Stress-4-EnormousLine.txt" added.
				}

				// Measure the consumed rectangle:
				SizeF requestedSize = graphics.MeasureString(element.Text, font, int.MaxValue, staticMonitorRequestingFormat);
				SizeF drawnSize     = graphics.MeasureString(element.Text, font, bounds.Size,  staticMonitorDrawingFormat);

				requestedWidth = requestedSize.Width;
				drawnWidth     = drawnSize.Width;
			}
			else
			{
				requestedWidth = 0;
				drawnWidth     = 0;
			}
		}

		private static void SetDrawingObjects(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                                      Graphics graphics, out Font font, out Brush brush)
		{
			string    fontName = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle;
			Color     fontColor;

			if      (element is Domain.DisplayElement.TxData)
			{
				fontColor = settings.TxDataFormat.Color;
				fontStyle = settings.TxDataFormat.FontStyle;
				font      = SetIfChanged(ref staticTxDataObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticTxDataObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				fontColor = settings.TxControlFormat.Color;
				fontStyle = settings.TxControlFormat.FontStyle;
				font      = SetIfChanged(ref staticTxControlObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticTxControlObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				fontColor = settings.RxDataFormat.Color;
				fontStyle = settings.RxDataFormat.FontStyle;
				font      = SetIfChanged(ref staticRxDataObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticRxDataObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				fontColor = settings.RxControlFormat.Color;
				fontStyle = settings.RxControlFormat.FontStyle;
				font      = SetIfChanged(ref staticRxControlObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticRxControlObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				fontColor = settings.DateFormat.Color;
				fontStyle = settings.DateFormat.FontStyle;
				font      = SetIfChanged(ref staticDateObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticDateObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				fontColor = settings.TimeFormat.Color;
				fontStyle = settings.TimeFormat.FontStyle;
				font      = SetIfChanged(ref staticTimeObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticTimeObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.PortInfo)
			{
				fontColor = settings.PortFormat.Color;
				fontStyle = settings.PortFormat.FontStyle;
				font      = SetIfChanged(ref staticPortObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticPortObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.DirectionInfo)
			{
				fontColor = settings.DirectionFormat.Color;
				fontStyle = settings.DirectionFormat.FontStyle;
				font      = SetIfChanged(ref staticDirectionObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticDirectionObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.Length)
			{
				fontColor = settings.LengthFormat.Color;
				fontStyle = settings.LengthFormat.FontStyle;
				font      = SetIfChanged(ref staticLengthObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticLengthObjects.Brush, fontColor);
			}
			else if ((element is Domain.DisplayElement.NoData) ||
			         (element is Domain.DisplayElement.LeftMargin) ||
			         (element is Domain.DisplayElement.Space) ||
			         (element is Domain.DisplayElement.RightMargin) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				fontColor = settings.WhiteSpacesFormat.Color;
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				font      = SetIfChanged(ref staticWhiteSpacesObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticWhiteSpacesObjects.Brush, fontColor);
			}
			else if (element is Domain.DisplayElement.ErrorInfo)
			{
				fontColor = settings.ErrorFormat.Color;
				fontStyle = settings.ErrorFormat.FontStyle;
				font      = SetIfChanged(ref staticErrorObjects.Font, fontName, fontSize, fontStyle, graphics);
				brush     = SetIfChanged(ref staticErrorObjects.Brush, fontColor);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Unknown DisplayElement:");
				sb.Append(element.ToString());
				throw (new NotImplementedException(sb.ToString()));
			}
		}

		private static Font SetIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
		{
			// Create the font:
			if (cachedFont == null)
			{
				cachedFont = new Font(fontName, fontSize, fontStyle);

				// Also set tab stops accordingly:
				SetTabStops(cachedFont, graphics);
			}
			else if ((cachedFont.Name  != fontName) ||
			         (cachedFont.Size  != fontSize) ||
			         (cachedFont.Style != fontStyle))
			{
				// The font has changed, dispose of the cached font and create a new one:
				cachedFont.Dispose();
				cachedFont = new Font(fontName, fontSize, fontStyle);

				// Also set tab stops accordingly:
				SetTabStops(cachedFont, graphics);
			}

			return (cachedFont);
		}

		private static void SetTabStops(Font font, Graphics graphics)
		{
			// Calculate tabs, currently fixed to 8 characters:

			// \fixme (2009-08-29 / mky):
			// This is a somewhat strange calculation, however, don't know to do it better.

			SizeF size = graphics.MeasureString(" ", font);
			float[] tabStops = new float[256];
			
			for (int i = 0; i < 256; i++)
				tabStops[i] = size.Width * 14.5f;

			staticMonitorRequestingFormat.SetTabStops(0, tabStops);
			staticMonitorDrawingFormat   .SetTabStops(0, tabStops);
		}

		private static SolidBrush SetIfChanged(ref SolidBrush cachedBrush, Color color)
		{
			// Create the brush using the font color:
			if (cachedBrush == null)
			{
				cachedBrush = new SolidBrush(color);
			}
			else if (cachedBrush.Color.ToArgb() != color.ToArgb())
			{
				// The font color has changed, dispose of the cached brush and create a new one:
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
