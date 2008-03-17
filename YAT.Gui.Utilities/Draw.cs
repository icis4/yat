using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.IO;

namespace YAT.Gui.Utilities
{
	public static class Draw
	{
		// for efficiency, cache the font and brushes to use for drawing
		private static Font _font;
		private static SolidBrush _txDataBrush      = new SolidBrush(Color.Black);
		private static SolidBrush _txControlBrush   = new SolidBrush(Color.Black);
		private static SolidBrush _rxDataBrush      = new SolidBrush(Color.Black);
		private static SolidBrush _rxControlBrush   = new SolidBrush(Color.Black);
		private static SolidBrush _timestampBrush   = new SolidBrush(Color.Black);
		private static SolidBrush _lineLengthBrush  = new SolidBrush(Color.Black);
		private static SolidBrush _whiteSpacesBrush = new SolidBrush(Color.Black);
		private static SolidBrush _errorBrush       = new SolidBrush(Color.Black);
		private static StringFormat _stringFormat   = new StringFormat();

		static Draw()
		{
			// use GenericTypographic format to be able to measure characters indiviually,
			// i.e. without a small margin before and after the character
			_stringFormat = StringFormat.GenericTypographic;

			// additionally enable trailing spaces to be able to correctly measure
			// single spaces
			_stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
		}

		public static SizeF MeasureItem(List<Domain.DisplayElement> line, Model.Settings.FormatSettings formatSettings,
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
			string fontName = formatSettings.Font.Name;
			float fontSize = formatSettings.Font.Size;
			FontStyle fontStyle;
			Color fontColor;
			SetStyleAndColorAndBrush(element, formatSettings, out fontStyle, out fontColor);

			Font font = SetFont(fontName, fontSize, fontStyle);

			return (graphics.MeasureString(element.Text, font, bounds.Size, _stringFormat));
		}

		public static SizeF DrawItem(List<Domain.DisplayElement> line, Model.Settings.FormatSettings formatSettings,
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
			FontStyle fontStyle;
			Color fontColor;
			Brush brush = SetStyleAndColorAndBrush(element, formatSettings, out fontStyle, out fontColor);

			string fontName = formatSettings.Font.Name;
			float fontSize = formatSettings.Font.Size;
			Font font = SetFont(fontName, fontSize, fontStyle);

			// select the highlight brush if the item is selected
			if ((state & DrawItemState.Selected) == DrawItemState.Selected)
				brush = SystemBrushes.HighlightText;

			// perform the painting
			graphics.DrawString(element.Text, font, brush, bounds, _stringFormat);

			// measure consumed rectangle
			return (graphics.MeasureString(element.Text, font, bounds.Size, _stringFormat));
		}

		private static Font SetFont(string fontName, float fontSize, FontStyle fontStyle)
		{
			if (_font == null)
			{
				_font = new Font(fontName, fontSize, fontStyle);
			}
			else if ((_font.Name != fontName) ||
					 (_font.Size != fontSize) ||
					 (_font.Style != fontStyle))
			{
				// the font has changed, so dispose of the cached font and create a new one
				_font.Dispose();
				_font = new Font(fontName, fontSize, fontStyle);
			}
			return (_font);
		}

		private static SolidBrush SetStyleAndColorAndBrush
								 (Domain.DisplayElement element, Model.Settings.FormatSettings settings,
		                          out FontStyle fontStyle, out Color fontColor)
		{
			SolidBrush brush;
			if (element is Domain.DisplayElement.TxData)
			{
				fontStyle = settings.TxDataFormat.Style;
				fontColor = settings.TxDataFormat.Color;
				brush = SetBrush(_txDataBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				fontStyle = settings.TxControlFormat.Style;
				fontColor = settings.TxControlFormat.Color;
				brush = SetBrush(_txControlBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				fontStyle = settings.RxDataFormat.Style;
				fontColor = settings.RxDataFormat.Color;
				brush = SetBrush(_rxDataBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				fontStyle = settings.RxControlFormat.Style;
				fontColor = settings.RxControlFormat.Color;
				brush = SetBrush(_rxControlBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.TimeStamp)
			{
				fontStyle = settings.TimeStampFormat.Style;
				fontColor = settings.TimeStampFormat.Color;
				brush = SetBrush(_timestampBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.LineLength)
			{
				fontStyle = settings.LengthFormat.Style;
				fontColor = settings.LengthFormat.Color;
				brush = SetBrush(_lineLengthBrush, fontColor);
			}
			else if ((element is Domain.DisplayElement.LeftMargin) ||
					 (element is Domain.DisplayElement.Space) ||
					 (element is Domain.DisplayElement.RightMargin) ||
					 (element is Domain.DisplayElement.LineBreak))
			{
				fontStyle = settings.WhiteSpacesFormat.Style;
				fontColor = settings.WhiteSpacesFormat.Color;
				brush = SetBrush(_whiteSpacesBrush, fontColor);
			}
			else if (element is Domain.DisplayElement.Error)
			{
				fontStyle = settings.ErrorFormat.Style;
				fontColor = settings.ErrorFormat.Color;
				brush = SetBrush(_errorBrush, fontColor);
			}
			else
			{
				throw (new NotImplementedException("Unknown DisplayElement"));
			}
			return (brush);
		}

		private static SolidBrush SetBrush(SolidBrush brush, Color color)
		{
			// create the brush using the font color
			if (brush == null)
			{
				brush = new SolidBrush(color);
			}
			else if (brush.Color != color)
			{
				// the font color has changed, so dispose of the cached brush and create a new one
				brush.Dispose();
				brush = new SolidBrush(color);
			}
			return (brush);
		}

		private static string FinalizeText(Domain.DisplayElement element, Domain.Settings.CharReplaceSettings settings)
		{
			if (element is Domain.DisplayElement.Space)
			{
				if (settings.ReplaceSpace)
					return (Domain.Settings.CharReplaceSettings.ReplaceSpaceString);
			}
			return (element.Text);
		}
	}
}
