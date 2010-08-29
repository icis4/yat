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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing RTF reader functionality for YAT.
	/// </summary>
	public static class RtfReader
	{
		/// <summary></summary>
		public static string[] LinesFromRtfFile(string rtfFilePath)
		{
			RichTextBox rtb = new RichTextBox();
			using (FileStream fs = File.OpenRead(rtfFilePath))
			{
				rtb.LoadFile(fs, RichTextBoxStreamType.RichText);
			}
			return (rtb.Lines);
		}
	}

	/// <summary>
	/// Static utility class providing RTF writer functionality for YAT.
	/// </summary>
	public static class RtfWriter
	{
		/// <summary></summary>
		public static void LinesToRtfFile(List<Domain.DisplayLine> lines, string rtfFilePath, Settings.FormatSettings formatSettings, RichTextBoxStreamType rtfType)
		{
			RichTextBox rtb = LinesToRichTextBox(lines, formatSettings);
			rtb.SaveFile(rtfFilePath, rtfType);
		}

		/// <summary></summary>
		public static void LinesToClipboard(List<Domain.DisplayLine> lines, Settings.FormatSettings formatSettings)
		{
			RichTextBox rtb = LinesToRichTextBox(lines, formatSettings);
			rtb.SelectAll();
			rtb.Copy();
		}

		/// <summary></summary>
		public static RichTextBox LinesToRichTextBox(List<Domain.DisplayLine> lines, Settings.FormatSettings formatSettings)
		{
			RichTextBox rtb = new RichTextBox();
			foreach (Domain.DisplayLine line in lines)
				AppendDisplayLine(rtb, line, formatSettings);

			return (rtb);
		}

		private static void AppendDisplayLine(RichTextBox rtb, Domain.DisplayLine line, Settings.FormatSettings formatSettings)
		{
			AppendDisplayElements(rtb, line, formatSettings);
		}

		private static void AppendDisplayElements(RichTextBox rtb, List<Domain.DisplayElement> elements, Settings.FormatSettings formatSettings)
		{
			foreach (Domain.DisplayElement de in elements)
				AppendDisplayElement(rtb, de, formatSettings);
		}

		private static void AppendDisplayElement(RichTextBox rtb, Domain.DisplayElement element, Settings.FormatSettings formatSettings)
		{
			string fontName = formatSettings.Font.Name;
			float fontSize = formatSettings.Font.Size;

			if (element is Domain.DisplayElement.TxData)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.TxDataFormat.FontStyle);
				rtb.SelectionColor = formatSettings.TxDataFormat.Color;
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.TxControlFormat.FontStyle);
				rtb.SelectionColor = formatSettings.TxControlFormat.Color;
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.RxDataFormat.FontStyle);
				rtb.SelectionColor = formatSettings.RxDataFormat.Color;
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.RxControlFormat.FontStyle);
				rtb.SelectionColor = formatSettings.RxControlFormat.Color;
			}
			else if (element is Domain.DisplayElement.TimeStamp)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.TimeStampFormat.FontStyle);
				rtb.SelectionColor = formatSettings.TimeStampFormat.Color;
			}
			else if (element is Domain.DisplayElement.LineLength)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.LengthFormat.FontStyle);
				rtb.SelectionColor = formatSettings.LengthFormat.Color;
			}
			else if ((element is Domain.DisplayElement.LeftMargin) ||
					 (element is Domain.DisplayElement.Space) ||
					 (element is Domain.DisplayElement.RightMargin) ||
					 (element is Domain.DisplayElement.LineBreak))
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.WhiteSpacesFormat.FontStyle);
				rtb.SelectionColor = formatSettings.WhiteSpacesFormat.Color;
			}
			else if (element is Domain.DisplayElement.Error)
			{
				rtb.SelectionFont = new Font(fontName, fontSize, formatSettings.ErrorFormat.FontStyle);
				rtb.SelectionColor = formatSettings.ErrorFormat.Color;
			}
			else
			{
				throw (new NotImplementedException("Unknown DisplayElement"));
			}

			// Handle line break according to current system.
			if (element is Domain.DisplayElement.LineBreak)
				rtb.AppendText(Environment.NewLine);
			else
				rtb.AppendText(element.Text);
		}
	}

	/// <summary>
	/// Static utility class providing RTF printer functionality for YAT.
	/// </summary>
	public class RtfPrinter
	{
		private PrintDocument pd;
		private RichTextBox rtb;
		private StringReader reader;

		/// <summary></summary>
		public RtfPrinter(PrinterSettings settings)
		{
			this.pd = new PrintDocument();
			this.pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
			this.pd.PrinterSettings = settings;
		}

		/// <summary></summary>
		public virtual void Print(RichTextBox rtb)
		{
			this.rtb = rtb;
			this.reader = new StringReader(this.rtb.Text);
			try
			{
				this.pd.Print();
			}
			finally
			{
				this.reader.Close();
			}
		}

		private void pd_PrintPage(object sender, PrintPageEventArgs e)
		{
			// Calculate the number of lines per page.
			int linesPerPage = (int)(e.MarginBounds.Height / this.rtb.Font.GetHeight(e.Graphics));
			int lineCount = 0;

			// Print each line of the file.
			string line = null;
			while (lineCount < linesPerPage && ((line = this.reader.ReadLine()) != null))
			{
				float yPos = 0;
				yPos = e.MarginBounds.Top + (lineCount * this.rtb.Font.GetHeight(e.Graphics));
				e.Graphics.DrawString(line, this.rtb.Font, Brushes.Black, e.MarginBounds.Left, yPos, new StringFormat());
				lineCount++;
			}

			// If more lines exist, print another page.
			if (line != null)
				e.HasMorePages = true;
			else
				e.HasMorePages = false;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
