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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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

using MKY.Text;

namespace MKY.IO
{
	/// <summary>
	/// Thread-safe log file.
	/// </summary>
	public class LogFile : IDisposable, IDisposableEx
	{
		private string filePath;
		private StreamWriter writer;

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Starts log file.
		/// </summary>
		/// <remarks>
		/// Using UTF-8 encoding with (Windows) or without (Unix, Linux,...) BOM.
		/// </remarks>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public LogFile(string filePath, bool append)
			: this(filePath, append, EncodingEx.EnvironmentRecommendedUTF8Encoding)
		{
		}

		/// <summary>
		/// Starts log file.
		/// </summary>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		/// <param name="encoding">Encoding of log file.</param>
		public LogFile(string filePath, bool append, Encoding encoding)
		{
			this.filePath = filePath;

			var writer = new StreamWriter(this.filePath, append, encoding);
			this.writer = (StreamWriter)TextWriter.Synchronized(writer);
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.writer != null)
						this.writer.Dispose();
				}

				// Set state to disposed:
				this.writer = null;
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
		~LogFile()
		{
			Dispose(false);

			Diagnostics.DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
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
				AssertNotDisposed();

				return (this.writer.BaseStream);
			}
		}

		/// <summary>
		/// Writes a line into log file and adds a time stamp.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public virtual void WriteLine(string line, string timeStampFormat = "yyyy-MM-dd HH:mm:ss.fff")
		{
			AssertNotDisposed();

			var now = DateTime.Now;
			try
			{
				lock (this.writer)
				{	// Output milliseconds for readability, even though last digit only provides limited accuracy.
					this.writer.WriteLine(now.ToString(timeStampFormat, DateTimeFormatInfo.CurrentInfo) + "  " + line);
					this.writer.Flush();
				}
			}
			catch { }
		}

		/// <summary>
		/// Closes log file.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public virtual void Close()
		{
			if (IsDisposed)
				return; // Close() shall be callable on a disposed object.

			try
			{
				lock (this.writer)
				{
					this.writer.Close();
				}
			}
			catch { }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
