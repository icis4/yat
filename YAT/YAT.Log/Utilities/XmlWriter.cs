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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using MKY;
using MKY.IO;
using MKY.Xml.Schema;

using YAT.Domain;
using YAT.Domain.Utilities;

#endregion

namespace YAT.Log.Utilities
{
	/// <summary>
	/// Utility class providing XML writer functionality for YAT.
	/// </summary>
	public abstract class XmlWriter : DisposableBase
	{
		/// <summary></summary>
		protected const string Header = @"<?xml version=""1.0"" encoding=""utf-8""?>";

		/// <summary></summary>
		protected const string Schema = @"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""";

		private StreamWriter writer;
		private object writerSyncObj = new object();

		private int    indentSize = 2;
		private string indentString = new string(' ', 2);
		private int    indentLevel; // = 0;
		private ReaderWriterLockSlim indentSyncObj = new ReaderWriterLockSlim();

		/// <summary></summary>
		protected XmlWriter(FileStream stream, Encoding encoding, string initialLineAfterHeader)
			: this(stream, encoding, new string[] { initialLineAfterHeader })
		{
		}

		/// <summary></summary>
		protected XmlWriter(FileStream stream, Encoding encoding, IEnumerable<string> initialLinesAfterHeader)
		{
			// Create the stream writer and add the XML header:
			this.writer = new StreamWriter(stream, encoding);
			this.writer.WriteLine(Header);

			// Add the initial lines:
			if (initialLinesAfterHeader != null)
			{
				foreach (string line in initialLinesAfterHeader)
					this.writer.WriteLine(line);
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				if (this.writer != null) {
					this.writer.Dispose();
					this.writer = null;
				}

				if (this.indentSyncObj != null) {
					this.indentSyncObj.Dispose();
					this.indentSyncObj = null;
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is two.
		/// </value>
		public int IndentSize
		{
			get
			{
				AssertUndisposed();

				int indentSize;
				this.indentSyncObj.EnterReadLock();
				{
					indentSize = this.indentSize;
				}
				this.indentSyncObj.ExitReadLock();
				return (indentSize);
			}

			set
			{
				AssertUndisposed();

				this.indentSyncObj.EnterWriteLock();
				{
					this.indentSize = value;
					this.indentString = new string(' ', this.indentSize);
				}
				this.indentSyncObj.ExitWriteLock();
			}
		}

		/// <summary>
		/// Gets or sets the indent level.
		/// </summary>
		/// <value>
		/// The indent level. The default is zero.
		/// </value>
		public int IndentLevel
		{
			get
			{
				AssertUndisposed();

				int indentLevel;
				this.indentSyncObj.EnterReadLock();
				{
					indentLevel = this.indentLevel;
				}
				this.indentSyncObj.ExitReadLock();
				return (indentLevel);
			}

			set
			{
				AssertUndisposed();

				this.indentSyncObj.EnterWriteLock();
				{
					this.indentLevel = value;
				}
				this.indentSyncObj.ExitWriteLock();
			}
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public void Indent()
		{
			AssertUndisposed();

			this.indentSyncObj.EnterWriteLock();
			{
				if (this.indentLevel < int.MaxValue)
					this.indentLevel++;
			}
			this.indentSyncObj.ExitWriteLock();
		}

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public void Unindent()
		{
			AssertUndisposed();

			this.indentSyncObj.EnterWriteLock();
			{
				if (this.indentLevel > 0)
					this.indentLevel--;
			}
			this.indentSyncObj.ExitWriteLock();
		}

		/// <param name="content">The line content without indentation spaces.</param>
		protected void WriteLine(string content)
		{
			AssertUndisposed();

			lock (writerSyncObj)
			{
				int    indentLevel;
				string indentString;

				this.indentSyncObj.EnterReadLock();
				{
					indentLevel  = this.indentLevel;
					indentString = this.indentString;
				}
				this.indentSyncObj.ExitReadLock();

				for (int i = 0; i < indentLevel; i++)
					this.writer.Write(indentString);

				this.writer.WriteLine(content);
			}
		}

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written
		/// to the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Flush()
		{
			AssertUndisposed();

			lock (writerSyncObj)
				this.writer.Flush();
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		protected void Close(string finalLine)
		{
			Close(new string[] { finalLine });
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		protected void Close(string[] finalLines)
		{
			AssertUndisposed();

			lock (writerSyncObj)
			{
				// Add the final lines:
				if (finalLines != null)
				{
					int remaining = finalLines.Length;
					foreach (string line in finalLines)
					{
						if (--remaining > 0) // All but the last line.
							this.writer.WriteLine(line);
						else
							this.writer.Write(line);
					}
				}

				// Close the file:
				this.writer.Close();
			}
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public abstract void Close();
	}

	/// <summary>
	/// Utility class providing raw XML writer functionality for YAT.
	/// </summary>
	public class XmlWriterRaw : XmlWriter
	{
		private const string InitialLineAfterHeader = "<ArrayOfXmlTransferRawLine " + Schema + ">";
		private const string FinalLine              = "</ArrayOfXmlTransferRawLine>";

		/// <summary></summary>
		public XmlWriterRaw(FileStream stream, Encoding encoding, bool addSchema, string schemaFilePath)
			: base(stream, encoding, InitialLineAfterHeader)
		{
			Indent();

			if (addSchema)
			{
				Type type = typeof(List<XmlTransferRawLine>);
				string directoryPath = PathEx.GetDirectoryPath(schemaFilePath);
				string fileName = Path.GetFileNameWithoutExtension(schemaFilePath);
				XmlSchemaEx.WriteToFile(type, directoryPath, fileName);
			}
		}

		/// <summary></summary>
		public void WriteLine(RawChunk chunk)
		{
			byte[] copyOfContent = new byte[chunk.Content.Count]; // Copy instead of forwarding 'ReadOnlyCollection' because
			chunk.Content.CopyTo(copyOfContent, 0);               // "XmlTransferRawLine" requires an array for serialization.
			WriteLine(new XmlTransferRawLine(chunk.TimeStamp, chunk.Device, (Direction)chunk.Direction, copyOfContent));
		}

		private void WriteLine(XmlTransferRawLine transferLine)
		{
		////AssertUndisposed() is called by WriteLine() further below.

			// Example (without indentation):
			// <XmlTransferRawLine TimeStamp="2001-12-23T12:34:56.789-01:00" Device="COM1" Direction="Tx" DataAsBase64="QUJDRA==" />

			// Neither outputting time span nor time delta since that can be calculated from time stamp.

			// To recreate this example and validate the schema implemented below:
			//  1. Enable raw export in View.Forms.Terminal.SaveMonitor().
			//  2. Send some data
			//  3. Select some lines in the monitor.
			//  4. Select 'Save To File...' and select .xml as file extension.

			var sb = new StringBuilder();
			sb.Append(@"<XmlTransferRawLine");
			sb.Append(@" TimeStamp=""");
			sb.Append(transferLine.TimeStamp.ToString("yyyy-MM-dd",      CultureInfo.InvariantCulture)); // Should result in same 'DateTimeFormatInfo.CurrentInfo' as format is fixed anyway.
			sb.Append(@"T"); // Note that time stamp format is fixed to standard XML format.
			sb.Append(transferLine.TimeStamp.ToString("HH:mm:ss.fffzzz", CultureInfo.InvariantCulture)); // Should result in same 'DateTimeFormatInfo.CurrentInfo' as format is fixed anyway.
			sb.Append(@""" Device=""");
			sb.Append(transferLine.Device);
			sb.Append(@""" Direction=""");
			sb.Append(transferLine.Direction.ToString()); // Default is "G".
			sb.Append(@""" DataAsBase64=""");
			sb.Append(transferLine.DataAsBase64);
			sb.Append(@""" />");
			WriteLine(sb.ToString());
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public override void Close()
		{
			Unindent();

			Close(FinalLine);
		}
	}

	/// <summary>
	/// Utility class providing text XML writer functionality for YAT.
	/// </summary>
	public class XmlWriterText : XmlWriter
	{
		private const string InitialLineAfterHeader = "<ArrayOfXmlTransferTextLine " + Schema + ">";
		private const string FinalLine              = "</ArrayOfXmlTransferTextLine>";

		/// <summary></summary>
		public XmlWriterText(FileStream stream, Encoding encoding, bool addSchema, string schemaFilePath)
			: base(stream, encoding, InitialLineAfterHeader)
		{
			Indent();

			if (addSchema)
			{
				Type type = typeof(List<XmlTransferTextLine>);
				string directoryPath = PathEx.GetDirectoryPath(schemaFilePath);
				string fileName = Path.GetFileNameWithoutExtension(schemaFilePath);
				XmlSchemaEx.WriteToFile(type, directoryPath, fileName);
			}
		}

		/// <summary></summary>
		public void WriteLine(DisplayLine displayLine)
		{
			XmlTransferTextLine transferLine;
			XmlWriterHelperText.ConvertLine(displayLine, out transferLine);
			WriteLine(transferLine);
		}

		private void WriteLine(XmlTransferTextLine transferLine)
		{
		////AssertUndisposed() is called by WriteLine() further below.

			// Example (without indentation):
			// <XmlTransferTextLine TimeStamp="2001-12-23T12:34:56.789-01:00" Device="COM1" Direction="Tx" Text="ABCD" Length="4" />

			// Neither outputting time span nor time delta since that can be calculated from time stamp.

			// To recreate this example and validate the schema implemented below:
			//  1. Send some data
			//  2. Select some lines in the monitor.
			//  3. Select 'Save To File...' and select .xml as file extension.

			var sb = new StringBuilder();
			sb.Append(@"<XmlTransferTextLine");
			sb.Append(@" TimeStamp=""");
			sb.Append(transferLine.TimeStamp.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
			sb.Append(@"T"); // Note that time stamp format is fixed to standard XML format.
			sb.Append(transferLine.TimeStamp.ToString("HH:mm:ss.fffzzz", CultureInfo.InvariantCulture));
			sb.Append(@""" Device=""");
			sb.Append(transferLine.Device);
			sb.Append(@""" Direction=""");
			sb.Append(transferLine.Direction.ToString()); // Default is "G".
			sb.Append(@""" Text=""");
			sb.Append(transferLine.Text);
			sb.Append(@""" Length=""");
			sb.Append(transferLine.Length.ToString(CultureInfo.InvariantCulture)); // Default is "G".
			sb.Append(@""" />");
			WriteLine(sb.ToString());
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public override void Close()
		{
			Unindent();

			Close(FinalLine);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
