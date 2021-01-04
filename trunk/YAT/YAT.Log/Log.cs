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
// YAT Version 2.2.0 Development
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using MKY;
using MKY.IO;

#endregion

namespace YAT.Log
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	internal abstract class Log : DisposableBase
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int FlushTimeoutMin = 1000;
		private const int FlushTimeoutMax = 2500;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isEnabled;
		private Func<string> makeFilePath;
		private LogFileWriteMode writeMode;

		private bool isOn;
		private string filePath;
		private FileStream fileStream;

		private Timer flushTimer;
		private Random flushTimerRandom;
		private object flushTimerSyncObj = new object();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		protected Log(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode)
		{
			this.flushTimerRandom = new Random(RandomEx.NextRandomSeed());

			Initialize(enabled, makeFilePath, writeMode);
		}

		private void Initialize(bool enabled, Func<string> makeFilePath, LogFileWriteMode writeMode)
		{
			this.isEnabled    = enabled;
			this.makeFilePath = makeFilePath;
			this.writeMode    = writeMode;
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
				StopAndDisposeFlushTimer();

				// In the 'normal' case, Close() has already been called.
				Close();
			}
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual bool IsEnabled
		{
			get { return (this.isEnabled); }
		}

		/// <summary></summary>
		public virtual bool FileExists
		{
			get { return (File.Exists(this.filePath)); }
		}

		/// <summary>
		/// The absolute path to the log file.
		/// </summary>
		public virtual string FilePath
		{
			get { return (this.filePath); }
		}

		/// <summary></summary>
		public virtual bool IsOn
		{
			get { return (this.isOn); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void ApplySettings(bool enabled, bool isOn, Func<string> makeFilePath, LogFileWriteMode writeMode)
		{
			if (this.isOn)
			{
				if (this.isEnabled != enabled)
				{
					if (enabled)
					{
						Initialize(enabled, makeFilePath, writeMode);

						if (isOn)
							Open();
					}
					else
					{
						Close();

						Initialize(enabled, makeFilePath, writeMode);
					}
				}
				else if ((this.makeFilePath != makeFilePath) || (this.writeMode != writeMode))
				{
					Close();

					Initialize(enabled, makeFilePath, writeMode);

					if (isOn)
						Open();
				}
			}
			else
			{
				Initialize(enabled, makeFilePath, writeMode);

				if (isOn)
					Open();
			}
		}

		/// <summary></summary>
		protected virtual void MakeFilePath()
		{
			this.filePath = this.makeFilePath.Invoke();
		}

		/// <summary></summary>
		protected virtual void MakeUniqueFilePath()
		{
			MakeFilePath();
			this.filePath = FileEx.GetUniqueFilePath(this.filePath);
		}

		/// <summary></summary>
		public virtual void Open()
		{
			if (!this.isEnabled)
				return;

			switch (this.writeMode)
			{
				case LogFileWriteMode.Create:
				{
					// Make file path now in order to get the time stamp of the open operation:
					MakeUniqueFilePath();

					// Create directory if not existing yet:
					if (!Directory.Exists(Path.GetDirectoryName(this.filePath)))
						Directory.CreateDirectory(Path.GetDirectoryName(this.filePath));

					// Create new file:
					this.fileStream = File.Open(this.filePath, FileMode.Create, FileAccess.Write, FileShare.Read);

					break;
				}

				case LogFileWriteMode.Append:
				{
					if (File.Exists(this.filePath)) // Append to existing file:
					{
						this.fileStream = File.Open(this.filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
					}
					else // Create new file:
					{
						MakeFilePath();

						if (!Directory.Exists(Path.GetDirectoryName(this.filePath)))
							Directory.CreateDirectory(Path.GetDirectoryName(this.filePath));

						this.fileStream = File.Open(this.filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
					}

					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.writeMode + "' is a write mode that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			OpenWriter(this.fileStream);
			this.isOn = true;
		}

		/// <summary></summary>
		public virtual void Flush()
		{
			if (this.isEnabled && this.isOn)
				FlushWriter();
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			if (this.isEnabled && this.isOn)
			{
				CloseWriter();
				this.fileStream.Close();
				this.fileStream = File.Open(this.filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read);
				OpenWriter(this.fileStream);
			}
		}

		/// <summary></summary>
		public virtual void Close()
		{
			if (IsUndisposed) // Close() shall be callable on a disposed object.
			{
				if (this.isEnabled && this.isOn)
				{
					CloseWriter();
					this.fileStream.Close();
				}

				this.isOn = false;
			}
		}

		/// <summary></summary>
		protected abstract void OpenWriter(FileStream stream);

		/// <summary></summary>
		protected abstract void FlushWriter();

		/// <summary></summary>
		protected abstract void CloseWriter();

		/// <summary>
		/// Triggers the flush timer. After triggering, <see cref="Flush"/> will be called within
		/// approx. <see cref="FlushTimeoutMin"/> and <see cref="FlushTimeoutMax"/> milliseconds.
		/// </summary>
		/// <remarks>
		/// This flushing mechanism is needed because neither <see cref="Stream"/> nor any derived
		/// class implements some kind of time-out controlled intelligent flushing. .NET would keep
		/// 4096 bytes buffered. This would lead to:
		/// <list type="bullet">
		/// <item><description>"Slow" data would not be written to the file before 4096 got transmitted.</description></item>
		/// <item><description>Data at the end of a transmission would not be written before stopping log or closing terminal/YAT.</description></item>
		/// </list>
		/// Note that <see cref="StreamWriter.AutoFlush"/> does not solve this issue, that is very
		/// inefficient as it "will flush its buffer to the underlying stream after every call
		/// to StreamWriter.Write()".
		/// </remarks>
		protected virtual void TriggerFlushTimer()
		{
			lock (flushTimerSyncObj)
			{
				if (this.flushTimer == null)
				{
					var callback = new TimerCallback(flushTimer_OneShot_Elapsed);
					var dueTime = flushTimerRandom.Next(FlushTimeoutMin, FlushTimeoutMax);
					var period = Timeout.Infinite; // One-Shot!

					this.flushTimer = new Timer(callback, null, dueTime, period);
				}
				else
				{
					// Do not call flushTimer.Change(dueTime, period) as flush shall always happen some now and then.
				}
			}
		}

		/// <summary></summary>
		protected virtual void StopAndDisposeFlushTimer()
		{
			lock (flushTimerSyncObj)
			{
				if (this.flushTimer != null)
				{
					this.flushTimer.Dispose();
					this.flushTimer = null;
				}
			}
		}

		/// <summary></summary>
		private void flushTimer_OneShot_Elapsed(object obj)
		{
			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			lock (this.flushTimerSyncObj)
			{
				if (this.flushTimer == null) // Handle overdue callbacks:
					return;

				StopAndDisposeFlushTimer(); // A new timer will get created on subsequent triggering.
			}

			// Flushing is a time-intensive operation, it may take up to 10 ms!
			Flush();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
