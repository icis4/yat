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
using System.Windows.Forms;

using MKY;

#endregion

namespace YAT.View.Controls.ViewModel
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too long for one line.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too long for one line.")]
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too long for one line.")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class MonitorRenderer
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
			SetLineNumberDrawingObjects(settings, graphics, out font);

			TextRenderer.DrawText(graphics, s, font, bounds, SystemColors.ControlText, staticLineNumberFormat);

			Size requestedSize = TextRenderer.MeasureText(graphics, s, font, bounds.Size, staticLineNumberFormat);
			requestedWidth = requestedSize.Width;
		}

		private static void SetLineNumberDrawingObjects(Model.Settings.FormatSettings settings,
		                                                Graphics graphics, out Font font)
		{
			string    fontName  = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle = FontStyle.Regular;
			font = CacheAndAssignIfChanged(ref staticLineNumberFont, fontName, fontSize, fontStyle, graphics);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static void DrawAndMeasureLine(Domain.DisplayLine line, Model.Settings.FormatSettings settings,
		                                      Graphics graphics, Rectangle bounds, DrawItemState state,
		                                      out int requestedWidth, out int drawnWidth)
		{
			requestedWidth = 0;
			drawnWidth     = 0;

			foreach (Domain.DisplayElement de in line)
			{
				int requestedElementWidth;
				int drawnElementWidth;

				DrawAndMeasureElement(de, settings, graphics,
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
		                                         Graphics graphics, Rectangle bounds, DrawItemState state,
		                                         out int requestedWidth, out int drawnWidth)
		{
			if (!string.IsNullOrEmpty(element.Text))
			{
				Font font;
				Color foreColor;
				Color backColor;
				SetDrawingObjects(element, settings, graphics, state, out font, out foreColor, out backColor);

				TextRenderer.DrawText(graphics, element.Text, font, bounds, foreColor, backColor, staticMonitorFormat);

				Size requestedSize = TextRenderer.MeasureText(graphics, element.Text, font, new Size(int.MaxValue, bounds.Height), staticMonitorFormat);
				Size drawnSize     = TextRenderer.MeasureText(graphics, element.Text, font, bounds.Size, staticMonitorFormat);

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
			string    fontName = settings.Font.Name;
			float     fontSize  = settings.Font.Size;
			FontStyle fontStyle;

			if      (element is Domain.DisplayElement.TxData)
			{
				foreColor = settings.TxDataFormat.Color;
				fontStyle = settings.TxDataFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTxDataFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				foreColor = settings.TxControlFormat.Color;
				fontStyle = settings.TxControlFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTxControlFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				foreColor = settings.RxDataFormat.Color;
				fontStyle = settings.RxDataFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticRxDataFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				foreColor = settings.RxControlFormat.Color;
				fontStyle = settings.RxControlFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticRxControlFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.DateInfo)
			{
				foreColor = settings.DateFormat.Color;
				fontStyle = settings.DateFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticDateFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.TimeInfo)
			{
				foreColor = settings.TimeFormat.Color;
				fontStyle = settings.TimeFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticTimeFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.PortInfo)
			{
				foreColor = settings.PortFormat.Color;
				fontStyle = settings.PortFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticPortFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.DirectionInfo)
			{
				foreColor = settings.DirectionFormat.Color;
				fontStyle = settings.DirectionFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticDirectionFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.Length)
			{
				foreColor = settings.LengthFormat.Color;
				fontStyle = settings.LengthFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticLengthFont, fontName, fontSize, fontStyle, graphics);
			}
			else if ((element is Domain.DisplayElement.NoData) ||
			         (element is Domain.DisplayElement.LeftMargin) ||
			         (element is Domain.DisplayElement.Space) ||
			         (element is Domain.DisplayElement.RightMargin) ||
			         (element is Domain.DisplayElement.LineBreak))
			{
				foreColor = settings.WhiteSpacesFormat.Color;
				fontStyle = settings.WhiteSpacesFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticWhiteSpacesFont, fontName, fontSize, fontStyle, graphics);
			}
			else if (element is Domain.DisplayElement.ErrorInfo)
			{
				foreColor = settings.ErrorFormat.Color;
				fontStyle = settings.ErrorFormat.FontStyle;
				font      = CacheAndAssignIfChanged(ref staticErrorFont, fontName, fontSize, fontStyle, graphics);
			}
			else
			{
				throw (new NotSupportedException("Program execution should never get here, '" + element.GetType() + "' is an invalid display element!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		private static Font CacheAndAssignIfChanged(ref Font cachedFont, string fontName, float fontSize, FontStyle fontStyle, Graphics graphics)
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
