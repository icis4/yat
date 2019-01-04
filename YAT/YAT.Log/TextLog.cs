﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Text;

using YAT.Application.Utilities;
using YAT.Format.Settings;
using YAT.Log.Utilities;

#endregion

namespace YAT.Log
{
	/// <summary></summary>
	internal class TextLog : Log
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
		private FormatSettings format;

		private XmlWriterText xmlWriter;
		private RtfWriter rtfWriter;
		private TextWriter textWriter;
		private object writerSyncObj = new object();

		/// <summary></summary>
		public TextLog(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode, Encoding encoding, FormatSettings format)
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
			CloseAndDisposeWriterNotSynchronized();

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public void ApplySettings(bool enabled, bool isOn, Func<string> makeFilePath, LogFileWriteMode writeMode, Encoding encoding, FormatSettings format)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn && ((this.encoding != encoding) || (this.format != format)))
				Close();

			this.encoding = encoding;
			this.format = format;
			base.ApplySettings(enabled, isOn, makeFilePath, writeMode);
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
					case FileType.Xml:
					{
						this.xmlWriter = new XmlWriterText(stream, true, FilePath);
						break;
					}

					case FileType.Rtf:
					{
						this.rtfWriter = new RtfWriter(stream, this.format);
						break;
					}

					case FileType.Text:
					default:
					{
						this.textWriter = new TextWriter(stream, this.encoding);
						break;
					}
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
					case FileType.Xml:
					{
						if (this.xmlWriter != null)
							this.xmlWriter.Flush();

						break;
					}

					case FileType.Rtf:
					{
						if (this.rtfWriter != null)
							this.rtfWriter.Flush();

						break;
					}

					case FileType.Text:
					default:
					{
						if (this.textWriter != null)
							this.textWriter.Flush();

						break;
					}
				}
			}
		}

		/// <summary></summary>
		protected override void CloseWriter()
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
			{
				CloseAndDisposeWriterNotSynchronized();
			}
		}

		/// <summary></summary>
		private void CloseAndDisposeWriterNotSynchronized()
		{
			switch (this.fileType)
			{
				case FileType.Xml:
				{
					if (this.xmlWriter != null)
					{
						if (!this.xmlWriter.IsDisposed)
						{
							this.xmlWriter.Close();
							this.xmlWriter.Dispose();
						}

						this.xmlWriter = null;
					}

					break;
				}

				case FileType.Rtf:
				{
					if (this.rtfWriter != null)
					{
						if (!this.rtfWriter.IsDisposed)
						{
							this.rtfWriter.Close();
							this.rtfWriter.Dispose();
						}

						this.rtfWriter = null;
					}

					break;
				}

				case FileType.Text:
				default:
				{
					if (this.textWriter != null)
					{
						if (!this.textWriter.IsDisposed)
						{
							this.textWriter.Close();
							this.textWriter.Dispose();
						}

						this.textWriter = null;
					}

					break;
				}
			}
		}

		/// <remarks>
		/// Since neat log uses formatted display lines, only the items that are shown are written to the log.
		/// This differs from raw log, where time stamp, port and direction are always contained in raw items.
		/// </remarks>
		public virtual void WriteLine(Domain.DisplayLine line)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn)
			{
				lock (this.writerSyncObj)
				{
					switch (this.fileType)
					{
						case FileType.Xml:
						{
							if (this.xmlWriter != null)
								this.xmlWriter.WriteLine(line);

							break;
						}

						case FileType.Rtf:
						{
							if (this.rtfWriter != null)
								this.rtfWriter.WriteLine(line);

							break;
						}

						case FileType.Text:
						default:
						{
							if (this.textWriter != null)
								this.textWriter.WriteLine(line);

							break;
						}
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
