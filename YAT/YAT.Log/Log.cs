//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using MKY;
using MKY.IO;

namespace YAT.Log
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	internal abstract class Log : IDisposable
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		// Flushing is a time-intensive operation, it may take up to 10 ms!
		private const int FlushTimeoutMin =  750;
		private const int FlushTimeoutMax = 1250;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private bool isEnabled;
		private string filePath;
		private FileNameSeparator separator;
		private LogFileWriteMode writeMode;

		private bool isOn;
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
		protected Log(bool enabled, string filePath, LogFileWriteMode writeMode)
			: this(enabled, filePath, "", writeMode)
		{
		}

		/// <summary></summary>
		protected Log(bool enabled, string filePath, string separator, LogFileWriteMode writeMode)
			: this(enabled, filePath, (FileNameSeparator)separator, writeMode)
		{
		}

		/// <summary></summary>
		protected Log(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode)
		{
			Initialize(enabled, filePath, separator, writeMode);
		}

		private void Initialize(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode)
		{
			this.isEnabled = enabled;
			this.filePath  = filePath;
			this.separator = separator;
			this.writeMode = writeMode;

			this.flushTimerRandom = new Random(RandomEx.NextPseudoRandomSeed());
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
					StopFlushTimer();

					// In the 'normal' case, Close() has already been called.
					Close();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Log()
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

		/// <summary></summary>
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
		public virtual void SetSettings(bool enabled, string filePath, LogFileWriteMode writeMode)
		{
			SetSettings(enabled, filePath, "", writeMode);
		}

		/// <summary></summary>
		public virtual void SetSettings(bool enabled, string filePath, string separator, LogFileWriteMode writeMode)
		{
			SetSettings(enabled, filePath, (FileNameSeparator)separator, writeMode);
		}

		/// <summary></summary>
		public virtual void SetSettings(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode)
		{
			if (this.isOn)
			{
				if (this.isEnabled != enabled)
				{
					if (enabled)
					{
						Initialize(enabled, filePath, separator, writeMode);
						Open();
					}
					else
					{
						Close();
						Initialize(enabled, filePath, separator, writeMode);
					}
				}
				else if ((this.filePath != filePath) || (this.writeMode != writeMode) || (this.separator != separator))
				{
					Close();
					Initialize(enabled, filePath, separator, writeMode);
					Open();
				}
			}
			else
			{
				Initialize(enabled, filePath, separator, writeMode);
			}
		}

		/// <summary></summary>
		public virtual void Open()
		{
			if (!this.isEnabled)
				return;

			if (!Directory.Exists(Path.GetDirectoryName(this.filePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(this.filePath));

			if (this.writeMode == LogFileWriteMode.Create)
			{
				this.filePath = FileEx.MakeUniqueFileName(this.filePath, this.separator.Separator);
				this.fileStream = File.Open(this.filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
			}
			else if (this.writeMode == LogFileWriteMode.Append)
			{
				if (File.Exists(this.filePath))
					this.fileStream = File.Open(this.filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
				else
					this.fileStream = File.Open(this.filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
			}
			else
			{
				throw (new ArgumentException("Write mode not supported!"));
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
		public virtual void Truncate()
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
			if (this.isEnabled && this.isOn)
			{
				CloseWriter();
				this.fileStream.Close();
			}

			this.isOn = false;
		}

		/// <summary></summary>
		protected abstract void OpenWriter(FileStream stream);

		/// <summary></summary>
		protected abstract void FlushWriter();

		/// <summary></summary>
		protected abstract void CloseWriter();

		/// <summary>
		/// Triggers the flush timer. After triggering, <see cref="Flush"/> will be called within
		/// no more than <see cref="FlushTimeoutMax"/> milliseconds.
		/// </summary>
		/// <remarks>
		/// Unfortunately, neither <see cref="Stream"/> nor any derived class implements some kind
		/// of intelligent flushing except for <see cref="StreamWriter.AutoFlush"/>. However, that
		/// is very inefficient. Therefore, a timer is used to flush the stream regularly. This
		/// ensures that a log file is up to date within a reasonable time, while being performant
		/// at the same time.
		/// </remarks>
		protected virtual void TriggerFlushTimer()
		{
			lock (flushTimerSyncObj)
			{
				if (this.flushTimer == null)
				{
					this.flushTimer = new Timer(new TimerCallback(flushTimer_Timeout), null, flushTimerRandom.Next(FlushTimeoutMin, FlushTimeoutMax), Timeout.Infinite);
				}
			}
		}

		/// <summary></summary>
		protected virtual void StopFlushTimer()
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
		private void flushTimer_Timeout(object obj)
		{
			StopFlushTimer();
			Flush();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
