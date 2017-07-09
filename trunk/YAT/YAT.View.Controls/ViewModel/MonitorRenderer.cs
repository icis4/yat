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
// YAT 2.0 Delta Version 1.99.80
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY;

// This code is intentionally placed into the YAT.View.Controls namespace even though the file is
// located in YAT.View.Controls.ViewModel for same location as parent control.
namespace YAT.View.Controls
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many parameters required.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many parameters required.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many parameters required.")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	internal static class MonitorRenderer
	{
		/// <remarks>
		/// For performance reasons, cache font used for drawing.
		/// </remarks>
		private static Font staticLineNumberFont;
		private static Font staticTxDataFont;
		private static Font staticTxControlFont;
		private static Font staticRxDataFont;
		private static Font staticRxControlFont;
		private static Font staticDateFont;
		private static Font staticTimeFont;
		private static Font staticPortFont;
		private static Font staticDirectionFont;
		private static Font staticLengthFont;
		private static Font staticWhiteSpacesFont;
		private static Font staticErrorFont;

		/// <summary>String format used for drawing line numbers.</summary>
		private static TextFormatFlags staticLineNumberFormat;

		/// <summary>String format used for drawing monitor strings.</summary>
		private static TextFormatFlags staticMonitorFormat;

			/// <summary>
		/// Use GenericTypographic format to be able to measure characters individually,
		///   i.e. without a small margin before and after the character.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Hmm... How can the logic below be implemented in the initializer?")]
		static MonitorRenderer()
		{
			staticLineNumberFormat  = TextFormatFlags.Default;
			staticLineNumberFormat |= TextFormatFlags.SingleLine;
			staticLineNumberFormat |= TextFormatFlags.Right;

			staticMonitorFormat  = TextFormatFlags.Default;
			staticMonitorFormat |= TextFormatFlags.SingleLine;
			staticMonitorFormat |= TextFormatFlags.ExpandTabs;
			staticMonitorFormat |= TextFormatFlags.NoPadding;
			staticMonitorFormat |= TextFormatFlags.NoPrefix;

			// Attention, do not use 'EndEllipsis' for the monitor! These ellipses were the root
			// cause of issue #125 "Representation of long texts in the terminal".

			// Also, note that this class earlier used Graphics.DrawString() and .MeasureString()
			// instead of TextRenderer. See SVN revisions #938 and #940 for intermediate state.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLineNumber(string s, Model.Settings.FormatSettings settings,
		                                            Graphics graphics, Rectangle bounds,
		                                            out int requestedWidth)
		{
			Font font;
			SetLineNumberDrawingObjects(settings, out font);

			TextRenderer.DrawText(graphics, s, font, bounds, SystemColors.ControlText, staticLineNumberFormat);

			if (string.IsNullOrEmpty(s) || s.Length <= 1)
				s = "88"; // Always measure at least two digits. Otherwise, the line number bar will already jump at 9 > 10.

			Size requestedSize = TextRenderer.MeasureText(graphics, s, font, bounds.Size, staticLineNumberFormat);
			requestedWidth = requestedSize.Width;
		}

		private static void SetLineNumberDrawingObjects(Model.Settings.FormatSettings settings, out Font font)
		{
			string    fontName  = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle = FontStyle.Regular;
			font = CacheAndAssignIfChanged(ref staticLineNumberFont, fontName, fontSize, fontStyle);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLine(string text, Font font,
		                                      Graphics g, Rectangle bounds, DrawItemState state,
		                                      Color foreColor, Color backColor,
		                                      out int requestedWidth)
		{
			if ((state & DrawItemState.Selected) != 0) // Selected.
				foreColor = SystemColors.HighlightText;

			TextRenderer.DrawText(g, text, font, bounds, foreColor, backColor, staticMonitorFormat);

			Size requestedSize = TextRenderer.MeasureText(g, text, font, bounds.Size, staticMonitorFormat);

			requestedWidth = requestedSize.Width;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLine(Domain.DisplayLine line, Model.Settings.FormatSettings settings,
		                                      IDeviceContext dc, Rectangle bounds, DrawItemState state,
		                                      out int requestedWidth)
		{
			requestedWidth = 0;
			int drawnWidth = 0;

			foreach (var de in line)
			{
				int requestedElementWidth;
				int drawnElementWidth;

				DrawAndMeasureElement(de, settings, dc,
				                      new Rectangle(bounds.X + drawnWidth, bounds.Y, bounds.Width - drawnWidth, bounds.Height),
				                      state, out requestedElementWidth, out drawnElementWidth);

				requestedWidth += requestedElementWidth;
				drawnWidth     += drawnElementWidth;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureElement(Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                                         IDeviceContext dc, Rectangle bounds, DrawItemState state,
		                                         out int requestedWidth, out int drawnWidth)
		{
			if (!string.IsNullOrEmpty(element.Text))
			{
				Font  font;
				Color foreColor;
				Color backColor;
				SetDrawingObjects(element, settings, state, out font, out foreColor, out backColor);

				TextRenderer.DrawText(dc, element.Text, font, bounds, foreColor, backColor, staticMonitorFormat);

				Size requestedSize = TextRenderer.MeasureText(dc, element.Text, font, new Size(int.MaxValue, bounds.Height), staticMonitorFormat);
				Size drawnSize     = TextRenderer.MeasureText(dc, element.Text, font, bounds.Size, staticMonitorFormat);

				requestedWidth = requestedSize.Width;
				drawnWidth     = drawnSize.Width;
			}
			else
			{
				requestedWidth = 0;
				drawnWidth     = 0;
			}
		}

		private static void SetDrawingObjects(Domain.DisplayElement element, Model.Settings.FormatSettings settings, DrawItemState state,
		                                      out Font font, out Color foreColor, out Color backColor)
		{
			string    fontName = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is Domain.DisplayElement.TxData)
			{
				foreColor = settings.TxDataFormat.Color;
				fontStyle = settings.TxDataFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTxDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				foreColor = settings.TxControlFormat.Color;
				fontStyle = settings.TxControlFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTxControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				foreColor = settings.RxDataFormat.Color;
				fontStyle = settings.RxDataFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticRxDataFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				foreColor = settings.RxControlFormat.Color;
				fontStyle = settings.RxControlFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticRxControlFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				foreColor = settings.DateFormat.Color;
				fontStyle = settings.DateFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticDateFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				foreColor = settings.TimeFormat.Color;
				fontStyle = settings.TimeFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTimeFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.PortInfo)
			{
				foreColor = settings.PortFormat.Color;
				fontStyle = settings.PortFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticPortFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.DirectionInfo)
			{
				foreColor = settings.DirectionFormat.Color;
				fontStyle = settings.DirectionFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticDirectionFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.DataLength)
			{
				foreColor = settings.LengthFormat.Color;
				fontStyle = settings.LengthFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticLengthFont, fontName, fontSize, fontStyle);
			}
			else if ((element is Domain.DisplayElement.Nonentity) ||
			         (element is Domain.DisplayElement.DataSpace) ||
			         (element is Domain.DisplayElement.InfoSeparator) ||
			         (element is Domain.DisplayElement.LineStart) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				foreColor = settings.WhiteSpacesFormat.Color;
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticWhiteSpacesFont, fontName, fontSize, fontStyle);
			}
			else if (element is Domain.DisplayElement.ErrorInfo)
			{
				foreColor = settings.ErrorFormat.Color;
				fontStyle = settings.ErrorFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticErrorFont, fontName, fontSize, fontStyle);
			}
			else
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + element.GetType() + "' is an invalid display element!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			// Override if the item is selected:
			if ((state & DrawItemState.Selected) != 0)
			{
				foreColor = SystemColors.HighlightText;
				backColor = SystemColors.Highlight;
			}
			else
			{
				backColor = settings.BackColor;
			}
		}

		private static Font CacheAndAssignIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle)
		{
			// Create and cache the font:
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
