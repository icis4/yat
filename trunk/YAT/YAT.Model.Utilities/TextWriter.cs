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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.IO;
using System.Text;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Utility class providing text writer functionality for YAT.
	/// </summary>
	public class TextWriter : IDisposable
	{
		private bool isDisposed;

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
				if (this.writer != null) {
					this.writer.Close();
					this.writer.Dispose();
				}

				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~TextWriter()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

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
				foreach (DisplayElement element in line)
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
