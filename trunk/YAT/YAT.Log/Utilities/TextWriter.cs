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
// YAT Version 2.1.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.IO;
using System.Text;

using MKY;

using YAT.Domain;

#endregion

namespace YAT.Log.Utilities
{
	/// <summary>
	/// Utility class providing text writer functionality for YAT.
	/// </summary>
	public class TextWriter : IDisposable, IDisposableEx
	{
		private StreamWriter writer;
		private object writerSyncObj = new object();

		/// <summary></summary>
		public TextWriter(FileStream stream, Encoding encoding)
		{
			this.writer = new StreamWriter(stream, encoding);
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
		~TextWriter()
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
		/// Clears all buffers for the current writer and causes any buffered data to be written
		/// to the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Flush()
		{
			AssertNotDisposed();

			lock (writerSyncObj)
				this.writer.Flush(); // Note that the operating system may delay flushing.
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Close()
		{
			AssertNotDisposed();

			lock (writerSyncObj)
				this.writer.Close();
		}

		/// <summary></summary>
		public virtual void WriteLine(DisplayLine line)
		{
			AssertNotDisposed();

			lock (writerSyncObj)
			{
				foreach (var element in line)
				{
					// Handle line break according to current system:
					if (element is DisplayElement.LineBreak)
						this.writer.WriteLine();
					else if (!string.IsNullOrEmpty(element.Text))
						this.writer.Write(element.Text);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
