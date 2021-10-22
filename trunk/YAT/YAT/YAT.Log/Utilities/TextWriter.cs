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
// YAT Version 2.4.1
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
	public class TextWriter : DisposableBase
	{
		private StreamWriter writer;
		private readonly object writerSyncObj = new object();

		/// <summary></summary>
		public TextWriter(FileStream stream, Encoding encoding)
		{
			this.writer = new StreamWriter(stream, encoding);
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
			}
		}

		#endregion

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written
		/// to the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Flush()
		{
			AssertUndisposed();

			lock (writerSyncObj)
				this.writer.Flush(); // Note that the operating system may delay flushing.
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Close()
		{
			AssertUndisposed();

			lock (writerSyncObj)
				this.writer.Close();
		}

		/// <summary></summary>
		public virtual void WriteLine(DisplayLine line)
		{
			AssertUndisposed();

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
