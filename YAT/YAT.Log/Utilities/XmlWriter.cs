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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

using MKY.Xml.Schema;

using YAT.Domain;
using YAT.Domain.Utilities;

#endregion

namespace YAT.Log.Utilities
{
	/// <summary>
	/// Utility class providing XML writer functionality for YAT.
	/// </summary>
	public abstract class XmlWriter : IDisposable
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
		protected XmlWriter(FileStream stream, string initialLineAfterHeader)
			: this(stream, new string[] { initialLineAfterHeader })
		{
		}

		/// <summary></summary>
		protected XmlWriter(FileStream stream, IEnumerable<string> initialLinesAfterHeader)
		{
			// Create the stream writer and add the XML header:
			this.writer = new StreamWriter(stream, Encoding.UTF8);
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

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources:
				if (disposing)
				{
					if (this.writer != null)
						this.writer.Dispose();

					if (this.indentSyncObj != null)
						this.indentSyncObj.Dispose();
				}

				// Set state to disposed:
				this.writer = null;
				this.indentSyncObj = null;
				IsDisposed = true;
			}
		}

	#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~XmlWriter()
		{
			Dispose(false);

			MKY.Diagnostics.DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
				AssertNotDisposed();

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
				AssertNotDisposed();

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
				AssertNotDisposed();

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
				AssertNotDisposed();

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
			AssertNotDisposed();

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
			AssertNotDisposed();

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
			AssertNotDisposed();

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
			AssertNotDisposed();

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
			AssertNotDisposed();

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
		public XmlWriterRaw(FileStream stream, bool addSchema, string schemaFilePath)
			: base(stream, InitialLineAfterHeader)
		{
			Indent();

			if (addSchema)
			{
				Type type = typeof(List<XmlTransferRawLine>);
				string directory = Path.GetDirectoryName(schemaFilePath);
				string fileName = Path.GetFileNameWithoutExtension(schemaFilePath);
				XmlSchemaEx.ToFile(type, directory, fileName);
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// Nothing to dispose of (yet).
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public void WriteLine(RawChunk chunk)
		{
			WriteLine(new XmlTransferRawLine(chunk.TimeStamp, chunk.PortStamp, (Direction)chunk.Direction, chunk.Content));
		}

		private void WriteLine(XmlTransferRawLine transferLine)
		{
			// AssertNotDisposed() is called by WriteLine() further below.

			// Example (without indentation):
			// <XmlTransferRawLine TimeStamp="2001-12-23T12:34:56.789-01:00" Port="COM1" Direction="Tx" DataAsBase64="QUJDRA==" />

			// Neither outputting time span nor time delta since that can be calculated from time stamp.

			// To recreate this example and validate the schema implemented below:
			//  1. Enable raw export in View.Forms.Terminal.SaveMonitor().
			//  2. Send some data
			//  3. Select some lines in the monitor.
			//  4. Select 'Save To File...' and select .xml as file extension.

			var sb = new StringBuilder();
			sb.Append(@"<XmlTransferRawLine");
			sb.Append(@" TimeStamp=""");
			sb.Append(transferLine.TimeStamp.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
			sb.Append(@"T"); // Note that time stamp format is fixed to standard XML format.
			sb.Append(transferLine.TimeStamp.ToString("HH:mm:ss.fffzzz", CultureInfo.InvariantCulture));
			sb.Append(@""" Port=""");
			sb.Append(transferLine.Port);
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
	/// Utility class providing neat XML writer functionality for YAT.
	/// </summary>
	public class XmlWriterNeat : XmlWriter
	{
		private const string InitialLineAfterHeader = "<ArrayOfXmlTransferNeatLine " + Schema + ">";
		private const string FinalLine              = "</ArrayOfXmlTransferNeatLine>";

		/// <summary></summary>
		public XmlWriterNeat(FileStream stream, bool addSchema, string schemaFilePath)
			: base(stream, InitialLineAfterHeader)
		{
			Indent();

			if (addSchema)
			{
				Type type = typeof(List<XmlTransferNeatLine>);
				string directory = Path.GetDirectoryName(schemaFilePath);
				string fileName = Path.GetFileNameWithoutExtension(schemaFilePath);
				XmlSchemaEx.ToFile(type, directory, fileName);
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					// Nothing to dispose of (yet).
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public void WriteLine(DisplayLine displayLine)
		{
			XmlTransferNeatLine transferLine;
			XmlWriterHelperNeat.ConvertLine(displayLine, out transferLine);
			WriteLine(transferLine);
		}

		private void WriteLine(XmlTransferNeatLine transferLine)
		{
			// AssertNotDisposed() is called by WriteLine() further below.

			// Example (without indentation):
			// <XmlTransferNeatLine TimeStamp="2001-12-23T12:34:56.789-01:00" Port="COM1" Direction="Tx" Text="ABCD" ErrorText="" Length="4" />

			// Neither outputting time span nor time delta since that can be calculated from time stamp.

			// To recreate this example and validate the schema implemented below:
			//  1. Send some data
			//  2. Select some lines in the monitor.
			//  3. Select 'Save To File...' and select .xml as file extension.

			var sb = new StringBuilder();
			sb.Append(@"<XmlTransferNeatLine");
			sb.Append(@" TimeStamp=""");
			sb.Append(transferLine.TimeStamp.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
			sb.Append(@"T"); // Note that time stamp format is fixed to standard XML format.
			sb.Append(transferLine.TimeStamp.ToString("HH:mm:ss.fffzzz", CultureInfo.InvariantCulture));
			sb.Append(@""" Port=""");
			sb.Append(transferLine.Port);
			sb.Append(@""" Direction=""");
			sb.Append(transferLine.Direction.ToString()); // Default is "G".
			sb.Append(@""" Text=""");
			sb.Append(transferLine.Text);
			sb.Append(@""" ErrorText=""");
			sb.Append(transferLine.ErrorText);
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
