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
// YAT 2.0 Gamma 2'' Version 1.99.52
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
using System.IO;

using YAT.Application.Utilities;
using YAT.Model.Utilities;

namespace YAT.Log
{
	/// <summary></summary>
	internal class RawLog : Log
	{
		/// <summary></summary>
		protected enum FileType
		{
			Xml,
			Binary
		}

		private FileType fileType;

		private XmlWriterRaw xmlWriter;
		private BinaryWriter binaryWriter;
		private object writerSyncObj = new object();

		/// <summary></summary>
		public RawLog(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode)
			: base(enabled, makeFilePath, writeMode)
		{
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
				case FileType.Binary:
				default:				if (this.binaryWriter != null)	this.binaryWriter.Close();	break;
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		protected override void MakeFilePath()
		{
			base.MakeFilePath();
			this.fileType = ToFileType(FilePath);
		}

		/// <summary></summary>
		protected static FileType ToFileType(string filePath)
		{
			if (ExtensionHelper.IsXmlFile(filePath))
				return (FileType.Xml);
			else
				return (FileType.Binary);
		}

		/// <summary></summary>
		protected override void OpenWriter(FileStream stream)
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
			{
				switch (this.fileType)
				{
					case FileType.Xml:		this.xmlWriter = new XmlWriterRaw(stream, true, FilePath);		break;
					case FileType.Binary:
					default:				this.binaryWriter = new BinaryWriter(stream);					break;
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
					case FileType.Binary:
					default:				this.binaryWriter.Flush();	break;
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
					case FileType.Binary:
					default:				this.binaryWriter.Close();	this.binaryWriter = null;	break;
				}
			}
		}

		/// <summary></summary>
		public virtual void Write(Domain.RawChunk chunk)
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
							this.xmlWriter.WriteLine(chunk);
							break;
						}

						case FileType.Binary:
						default:
						{
							this.binaryWriter.Write(chunk.Data);
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
