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
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;

using YAT.Model.Utilities;
using YAT.Settings;

namespace YAT.Log
{
	/// <summary></summary>
	internal class NeatLog : Log
	{
		/// <summary></summary>
		protected enum FileType
		{
			Xml,
			Rtf,
			Text
		}

		private FileType fileType;
		private Encoding encoding;
		private Model.Settings.FormatSettings format;

// TODO		private XmlWriter textWriter;
		private RtfWriter rtfWriter;
		private TextWriter textWriter;

		/// <summary></summary>
		public NeatLog(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode, Model.Settings.FormatSettings format)
			: base(enabled, makeFilePath, writeMode)
		{
			this.encoding = Encoding.UTF8;
			this.format = format;
		}

		/// <summary></summary>
		public NeatLog(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode, Encoding encoding, Model.Settings.FormatSettings format)
			: base(enabled, makeFilePath, writeMode)
		{
			this.encoding = encoding;
			this.format = format;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		protected override void Dispose(bool disposing)
		{
			switch (this.fileType)
			{
				case FileType.Xml:		/* TODO */													break;
				case FileType.Rtf:		if (this.rtfWriter != null)		this.rtfWriter.Close();		break;
				case FileType.Text:
				default:				if (this.textWriter != null)	this.textWriter.Close();	break;
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public void SetSettings(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode, Encoding encoding, Model.Settings.FormatSettings format)
		{
			AssertNotDisposed();

			if (this.IsEnabled && this.IsOn && ((this.encoding != encoding) || (this.format != format)))
			{
				Close();
				this.encoding = encoding;
				this.format = format;
				base.SetSettings(enabled, makeFilePath, writeMode);
				Open();
			}
			else
			{
				this.encoding = encoding;
				this.format = format;
				base.SetSettings(enabled, makeFilePath, writeMode);
			}
		}

		/// <summary></summary>
		protected override void MakeFilePath()
		{
			base.MakeFilePath();
			this.fileType = ToFileType(FilePath);
		}

		/// <summary></summary>
		protected static FileType ToFileType(string filePath)
		{
			if (ExtensionSettings.IsXmlFile(filePath))
				return (FileType.Xml);
			else if (ExtensionSettings.IsRtfFile(filePath))
				return (FileType.Rtf);
			else
				return (FileType.Text);
		}

		/// <summary></summary>
		protected override void OpenWriter(System.IO.FileStream stream)
		{
			AssertNotDisposed();

			switch (this.fileType)
			{
				case FileType.Xml:		/* TODO */																break;
				case FileType.Rtf:		this.rtfWriter = new RtfWriter(stream, this.encoding, this.format);		break;
				case FileType.Text:
				default:				this.textWriter = new TextWriter(stream, this.encoding);				break;
			}
		}

		/// <summary></summary>
		protected override void FlushWriter()
		{
			AssertNotDisposed();

			switch (this.fileType)
			{
				case FileType.Xml:		/* TODO */					break;
				case FileType.Rtf:		this.rtfWriter.Flush();		break;
				case FileType.Text:
				default:				this.textWriter.Flush();	break;
			}
		}

		/// <summary></summary>
		protected override void CloseWriter()
		{
			AssertNotDisposed();

			switch (this.fileType)
			{
				case FileType.Xml:		/* TODO */												break;
				case FileType.Rtf:		this.rtfWriter.Close();		this.rtfWriter = null;		break;
				case FileType.Text:
				default:				this.textWriter.Close();	this.textWriter = null;		break;
			}
		}

		/// <summary></summary>
		public virtual void WriteLine(Domain.DisplayLine line)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn)
			{
				switch (this.fileType)
				{
					case FileType.Xml:		/* TODO */							break;
					case FileType.Rtf:		this.rtfWriter.WriteLine(line);		break;
					case FileType.Text:
					default:				this.textWriter.WriteLine(line);	break;
				}

				TriggerFlushTimer();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
