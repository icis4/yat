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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		/// Starts log file.
		/// </summary>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public LogFile(string filePath, bool append)
		{
			this.filePath = filePath;

			StreamWriter writer = new StreamWriter(this.filePath, append, Encoding.UTF8);
			this.writer = (StreamWriter)TextWriter.Synchronized(writer);
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.writer != null)
						this.writer.Dispose();
				}

				// Set state to disposed:
				this.writer = null;
				this.isDisposed = true;
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
		~LogFile()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

#endif // DEBUG

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
				// Do not call AssertNotDisposed() in a simple get-property.
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
				// Do not call AssertNotDisposed() in a simple get-property.
				return (this.writer.BaseStream);
			}
		}

		/// <summary>
		/// Writes a line into log file and adds a time stamp.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual void WriteLine(string line)
		{
			AssertNotDisposed();

			DateTime now = DateTime.Now;
			try
			{
				lock (this.writer)
				{	// Output milliseconds for readability, even though last digit only provides limited accuracy.
					this.writer.WriteLine(now.ToString("yyyy-MM-dd HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo) + "  " + line);
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
