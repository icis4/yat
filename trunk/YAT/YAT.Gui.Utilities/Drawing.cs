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
		private static TextFormatFlags staticLineNumberFormat;

		/// <summary>String format used for drawing monitor strings.</summary>
		private static TextFormatFlags staticMonitorDrawingFormat;

		/// <summary>String format used for measuring monitor strings.</summary>
		private static TextFormatFlags staticMonitorMeasuringFormat;

		/// <summary>
		/// Use GenericTypographic format to be able to measure characters individually,
		///   i.e. without a small margin before and after the character.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Hmm... How can the logic below be implemented in the initializer?")]
		static Drawing()
		{
			staticLineNumberFormat  = TextFormatFlags.Default;
			staticLineNumberFormat |= TextFormatFlags.SingleLine;

			// Enable drawing ellipses of text that exceeds the layout rectangle:
			staticMonitorDrawingFormat  = TextFormatFlags.Default;
			staticMonitorDrawingFormat |= TextFormatFlags.SingleLine;
			staticMonitorDrawingFormat |= TextFormatFlags.EndEllipsis;

			// For measuring, do not use ellipses:
			staticMonitorMeasuringFormat  = TextFormatFlags.Default;
			staticMonitorMeasuringFormat |= TextFormatFlags.SingleLine;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLineNumber(string s, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                            Graphics graphics, Rectangle bounds,
		                                            out int requestedWidth)
		{
			Font font;
			SetLineNumberDrawingObjects(settings, graphics, out font);

			TextFormatFlags flags = staticLineNumberFormat;
			if (rightToLeft == RightToLeft.Yes)
				flags |= TextFormatFlags.RightToLeft;

			TextRenderer.DrawText(graphics, s, font, bounds, SystemColors.ControlText, flags);

			Size requestedSize = TextRenderer.MeasureText(graphics, s, font, bounds.Size, flags);
			requestedWidth = requestedSize.Width;
		}

		private static void SetLineNumberDrawingObjects(Model.Settings.FormatSettings settings,
		                                                Graphics graphics, out Font font)
		{
			string    fontName  = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle = FontStyle.Regular;
			font = AssignIfChanged(ref staticLineNumberObjects.Font, fontName, fontSize, fontStyle, graphics);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLine(Domain.DisplayLine line, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                      Graphics graphics, Rectangle bounds, DrawItemState state,
		                                      out int requestedWidth, out int drawnWidth)
		{
			requestedWidth = 0;
			drawnWidth = 0;

			foreach (Domain.DisplayElement de in line)
			{
				int requestedElementWidth;
				int drawnElementWidth;

				DrawAndMeasureElement(de, settings, rightToLeft, graphics,
				                      new Rectangle(bounds.X + drawnWidth, bounds.Y, bounds.Width - drawnWidth, bounds.Height),
				                      state, out requestedElementWidth, out drawnElementWidth);

				requestedWidth += requestedElementWidth;
				drawnWidth     += drawnElementWidth;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureElement(Domain.DisplayElement element, Model.Settings.FormatSettings settings, RightToLeft rightToLeft,
		                                         Graphics graphics, Rectangle bounds, DrawItemState state,
		                                         out int requestedWidth, out int drawnWidth)
		{
			if (!string.IsNullOrEmpty(element.Text))
			{
				Font font;
				Color foreColor;
				Color backColor;
				SetDrawingObjects(element, settings, graphics, state, out font, out foreColor, out backColor);

				TextFormatFlags flags = staticMonitorDrawingFormat;
				if (rightToLeft == RightToLeft.Yes)
					flags |= TextFormatFlags.RightToLeft;

				TextRenderer.DrawText(graphics, element.Text, font, bounds, foreColor, backColor, flags);

				flags = staticMonitorMeasuringFormat;
				if (rightToLeft == RightToLeft.Yes)
					flags |= TextFormatFlags.RightToLeft;

				Size requestedSize = TextRenderer.MeasureText(graphics, element.Text, font, bounds.Size, flags);
				Size drawnSize     = TextRenderer.MeasureText(graphics, element.Text, font, bounds.Size, flags);

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
		                                      Graphics graphics, DrawItemState state,
		                                      out Font font, out Color foreColor, out Color backColor)
		{
			string fontName = settings.Font.Name;
			float fontSize  = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is Domain.DisplayElement.TxData)
			{
				foreColor = settings.TxDataFormat.Color;
				fontStyle = settings.TxDataFormat.FontStyle;
				font      = AssignIfChanged(ref staticTxDataObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				foreColor = settings.TxControlFormat.Color;
				fontStyle = settings.TxControlFormat.FontStyle;
				font      = AssignIfChanged(ref staticTxControlObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				foreColor = settings.RxDataFormat.Color;
				fontStyle = settings.RxDataFormat.FontStyle;
				font      = AssignIfChanged(ref staticRxDataObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				foreColor = settings.RxControlFormat.Color;
				fontStyle = settings.RxControlFormat.FontStyle;
				font      = AssignIfChanged(ref staticRxControlObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				foreColor = settings.DateFormat.Color;
				fontStyle = settings.DateFormat.FontStyle;
				font      = AssignIfChanged(ref staticDateObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				foreColor = settings.TimeFormat.Color;
				fontStyle = settings.TimeFormat.FontStyle;
				font      = AssignIfChanged(ref staticTimeObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.PortInfo)
			{
				foreColor = settings.PortFormat.Color;
				fontStyle = settings.PortFormat.FontStyle;
				font      = AssignIfChanged(ref staticPortObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.DirectionInfo)
			{
				foreColor = settings.DirectionFormat.Color;
				fontStyle = settings.DirectionFormat.FontStyle;
				font      = AssignIfChanged(ref staticDirectionObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.Length)
			{
				foreColor = settings.LengthFormat.Color;
				fontStyle = settings.LengthFormat.FontStyle;
				font      = AssignIfChanged(ref staticLengthObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if ((element is Domain.DisplayElement.NoData) ||
			         (element is Domain.DisplayElement.LeftMargin) ||
			         (element is Domain.DisplayElement.Space) ||
			         (element is Domain.DisplayElement.RightMargin) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				foreColor = settings.WhiteSpacesFormat.Color;
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				font      = AssignIfChanged(ref staticWhiteSpacesObjects.Font, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.ErrorInfo)
			{
				foreColor = settings.ErrorFormat.Color;
				fontStyle = settings.ErrorFormat.FontStyle;
				font      = AssignIfChanged(ref staticErrorObjects.Font,  fontName, fontSize, fontStyle, graphics);
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Unknown DisplayElement:");
				sb.Append(element.ToString());
				throw (new NotImplementedException(sb.ToString()));
			}

			// Override if the item is selected:
			if ((state & DrawItemState.Selected) == DrawItemState.Selected)
			{
				foreColor = SystemColors.HighlightText;
				backColor = SystemColors.Highlight;
			}
			else
			{
				backColor = settings.BackColor;
			}
		}

		private static Font AssignIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
		{
			// Create the font:
			if (cachedFont == null)
			{
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
