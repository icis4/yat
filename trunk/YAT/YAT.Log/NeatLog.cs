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

using System;
using System.Text;

using YAT.Application.Utilities;
using YAT.Model.Utilities;

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

		private XmlWriterNeat xmlWriter;
		private RtfWriter rtfWriter;
		private TextWriter textWriter;
		private object writerSyncObj = new object();

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
				case FileType.Xml:		if (this.xmlWriter != null)		this.xmlWriter.Close();		break;
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
			if      (ExtensionHelper.IsXmlFile(filePath))
				return (FileType.Xml);
			else if (ExtensionHelper.IsRtfFile(filePath))
				return (FileType.Rtf);
			else
				return (FileType.Text);
		}

		/// <summary></summary>
		protected override void OpenWriter(System.IO.FileStream stream)
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
			{
				switch (this.fileType)
				{
					case FileType.Xml:		this.xmlWriter = new XmlWriterNeat(stream, true, FilePath);			break;
					case FileType.Rtf:		this.rtfWriter = new RtfWriter(stream, this.encoding, this.format);	break;
					case FileType.Text:
					default:				this.textWriter = new TextWriter(stream, this.encoding);			break;
				}
			}
		}

		/// <summary></summary>
		protected override void FlushWriter()
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
			{
				switch (this.fileType)
				{
					case FileType.Xml:		this.xmlWriter.Flush();		break;
					case FileType.Rtf:		this.rtfWriter.Flush();		break;
					case FileType.Text:
					default:				this.textWriter.Flush();	break;
				}
			}
		}

		/// <summary></summary>
		protected override void CloseWriter()
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
			{
				switch (this.fileType)
				{
					case FileType.Xml:		this.xmlWriter.Close();		this.xmlWriter = null;		break;
					case FileType.Rtf:		this.rtfWriter.Close();		this.rtfWriter = null;		break;
					case FileType.Text:
					default:				this.textWriter.Close();	this.textWriter = null;		break;
				}
			}
		}

		/// <summary></summary>
		public virtual void WriteLine(Domain.DisplayLine line)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn)
			{
				lock (this.writerSyncObj)
				{
					switch (this.fileType)
					{
						case FileType.Xml:		this.xmlWriter.WriteLine(line);		break;
						case FileType.Rtf:		this.rtfWriter.WriteLine(line);		break;
						case FileType.Text:
						default:				this.textWriter.WriteLine(line);	break;
					}
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
