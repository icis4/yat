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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace MKY.IO
{
	/// <summary>
	/// Thread-safe log file.
	/// </summary>
	public class LogFile : IDisposable
	{
		private bool isDisposed;

		private string filePath;
		private StreamWriter writer;

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Starts Logfile.
		/// </summary>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public LogFile(string filePath, bool append)
		{
			this.filePath = filePath;
			this.writer = new StreamWriter(this.filePath, append, Encoding.UTF8);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if (this.writer != null)
						this.writer.Dispose();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~LogFile()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		/// <summary>
		/// Returns complete path of log file.
		/// </summary>
		public string FilePath
		{
			get
			{
				AssertNotDisposed();
				return (this.filePath);
			}
		}

		/// <summary>
		/// Returns underlying stream.
		/// </summary>
		public Stream UnderlyingStream
		{
			get
			{
				AssertNotDisposed();
				return (this.writer.BaseStream);
			}
		}

		/// <summary>
		/// Writes a line into log file and adds a timestamp.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual void WriteLine(string line)
		{
			AssertNotDisposed();

			DateTime now = DateTime.Now;
			try
			{
				lock (this.writer)
				{
					this.writer.WriteLine(now.ToString("HH:mm:ss.", DateTimeFormatInfo.InvariantInfo) + string.Format(DateTimeFormatInfo.InvariantInfo, "{0:000}", now.Millisecond) + "  " + line);
					this.writer.Flush();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Closes log file.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public virtual void Close()
		{
			AssertNotDisposed();

			try
			{
				lock (this.writer)
				{
					this.writer.Close();
				}
			}
			catch
			{
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
