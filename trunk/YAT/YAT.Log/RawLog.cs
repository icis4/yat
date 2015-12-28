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
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.ObjectModel;
using System.IO;

namespace YAT.Log
{
	/// <summary></summary>
	internal class RawLog : Log
	{
		private BinaryWriter writer;
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
			if (this.writer != null)
				this.writer.Close();

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		protected override void OpenWriter(FileStream stream)
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
				this.writer = new BinaryWriter(stream);
		}

		/// <summary></summary>
		protected override void FlushWriter()
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
				this.writer.Flush();
		}

		/// <summary></summary>
		protected override void CloseWriter()
		{
			AssertNotDisposed();

			lock (this.writerSyncObj)
				this.writer.Close();
		}

		/// <summary></summary>
		public virtual void WriteByte(byte value)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn)
			{
				lock (this.writerSyncObj)
					this.writer.Write(value);

				TriggerFlushTimer();
			}
		}

		/// <summary></summary>
		public virtual void WriteBytes(ReadOnlyCollection<byte> values)
		{
			AssertNotDisposed();

			if (IsEnabled && IsOn)
			{
				byte[] array = new byte[values.Count];
				values.CopyTo(array, 0);

				lock (this.writerSyncObj)
					this.writer.Write(array);

				TriggerFlushTimer();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
