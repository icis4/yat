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
// YAT 2.0 Gamma 1' Version 1.99.33
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;

using MKY.IO;

namespace YAT.Log
{
	/// <summary></summary>
	public class Logs : IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		#region Types > Log Base
		//==========================================================================================
		// Types > Log Base
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
		protected abstract class Log : IDisposable
		{
			#region Constants
			//==========================================================================================
			// Constants
			//==========================================================================================

			private const int FlushTimeout = 250;

			#endregion

			#region Fields
			//==========================================================================================
			// Fields
			//==========================================================================================

			private bool isDisposed;

			private bool isEnabled;
			private string file;
			private LogFileWriteMode writeMode;
			private FileNameSeparator separator;

			private FileStream fileStream;
			private bool isStarted;

			private Timer flushTimer;
			private object flushTimerSyncObj = new object();

			#endregion

			#region Object Lifetime
			//==========================================================================================
			// Object Lifetime
			//==========================================================================================

			/// <summary></summary>
			protected Log(bool enabled, string file, LogFileWriteMode writeMode)
			{
				Initialize(enabled, file, writeMode, new FileNameSeparator(""));
			}

			/// <summary></summary>
			protected Log(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				Initialize(enabled, file, writeMode, separator);
			}

			private void Initialize(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				this.isEnabled = enabled;
				this.file = file;
				this.writeMode = writeMode;
				this.separator = separator;
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
			protected virtual bool IsEnabled
			{
				get { return (this.isEnabled); }
			}

			/// <summary></summary>
			public virtual bool IsStarted
			{
				get { return (this.isStarted); }
			}

			#endregion

			#region Methods
			//==========================================================================================
			// Methods
			//==========================================================================================

			/// <summary></summary>
			public virtual void SetSettings(bool enabled, string file, LogFileWriteMode writeMode)
			{
				SetSettings(enabled, file, writeMode, new FileNameSeparator(""));
			}

			/// <summary></summary>
			public virtual void SetSettings(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				if (this.isStarted && (enabled != this.isEnabled))
				{
					if (enabled)
					{
						Initialize(enabled, file, writeMode, separator);
						Open();
					}
					else if (!enabled)
					{
						Close();
						Initialize(enabled, file, writeMode, separator);
					}
				}
				else if (this.isStarted && (file != this.file))
				{
					Close();
					Initialize(enabled, file, writeMode, separator);
					Open();
				}
				else
					Initialize(enabled, file, writeMode, separator);
			}

			/// <summary></summary>
			public virtual void Open()
			{
				if (!this.isEnabled)
					return;

				if (!Directory.Exists(Path.GetDirectoryName(this.file)))
					Directory.CreateDirectory(Path.GetDirectoryName(this.file));

				if (this.writeMode == LogFileWriteMode.Create)
				{
					this.file = FileEx.MakeUniqueFileName(this.file, this.separator.Separator);
					this.fileStream = File.Open(this.file, FileMode.Create, FileAccess.Write, FileShare.Read);
				}
				else if (this.writeMode == LogFileWriteMode.Append)
				{
					if (File.Exists(this.file))
						this.fileStream = File.Open(this.file, FileMode.Append, FileAccess.Write, FileShare.Read);
					else
						this.fileStream = File.Open(this.file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
				}
				else
					throw (new ArgumentException("Write mode not supported!"));

				OpenWriter(this.fileStream);
				this.isStarted = true;
			}

			/// <summary></summary>
			public virtual void Flush()
			{
				if (this.isEnabled && this.isStarted)
				{
					FlushWriter();
				}
			}

			/// <summary></summary>
			public virtual void Truncate()
			{
				if (this.isEnabled && this.isStarted)
				{
					CloseWriter();
					this.fileStream.Close();
					this.fileStream = File.Open(this.file, FileMode.Truncate, FileAccess.Write, FileShare.Read);
					OpenWriter(this.fileStream);
				}
			}

			/// <summary></summary>
			public virtual void Close()
			{
				if (this.isEnabled && this.isStarted)
				{
					CloseWriter();
					this.fileStream.Close();
				}

				this.isStarted = false;
			}

			/// <summary></summary>
			protected abstract void OpenWriter(FileStream stream);

			/// <summary></summary>
			protected abstract void FlushWriter();

			/// <summary></summary>
			protected abstract void CloseWriter();

			/// <summary></summary>
			protected virtual void StartFlushTimer()
			{
				lock (flushTimerSyncObj)
				{
					this.flushTimer = new Timer(new TimerCallback(flushTimer_Timeout), null, FlushTimeout, Timeout.Infinite);
				}
			}

			/// <summary></summary>
			protected virtual void RestartFlushTimer()
			{
				StopFlushTimer();
				StartFlushTimer();
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

		#endregion

		#region Types > Binary Log
		//==========================================================================================
		// Types > Binary Log
		//==========================================================================================

		/// <summary></summary>
		protected class BinaryLog : Log
		{
			private BinaryWriter writer;

			/// <summary></summary>
			public BinaryLog(bool enabled, string file, LogFileWriteMode writeMode)
				: base(enabled, file, writeMode)
			{
			}

			/// <summary></summary>
			public BinaryLog(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
				: base(enabled, file, writeMode, separator)
			{
			}

			/// <summary></summary>
			protected override void OpenWriter(FileStream stream)
			{
				this.writer = new BinaryWriter(stream);
			}

			/// <summary></summary>
			protected override void FlushWriter()
			{
				this.writer.Flush();
			}

			/// <summary></summary>
			protected override void CloseWriter()
			{
				this.writer.Close();
			}

			/// <summary></summary>
			public virtual void WriteByte(byte value)
			{
				if (IsEnabled && IsStarted)
				{
					this.writer.Write(value);
					RestartFlushTimer();
				}
			}

			/// <summary></summary>
			public virtual void WriteBytes(ReadOnlyCollection<byte> values)
			{
				if (IsEnabled && IsStarted)
				{
					byte[] array = new byte[values.Count];
					values.CopyTo(array, 0);
					this.writer.Write(array);
					RestartFlushTimer();
				}
			}
		}

		#endregion

		#region Types > Text Log
		//==========================================================================================
		// Types > Text Log
		//==========================================================================================

		/// <summary></summary>
		protected class TextLog : Log
		{
			private StreamWriter writer;

			/// <summary></summary>
			public TextLog(bool enabled, string file, LogFileWriteMode writeMode)
				: base(enabled, file, writeMode)
			{
			}

			/// <summary></summary>
			public TextLog(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
				: base(enabled, file, writeMode, separator)
			{
			}

			/// <summary></summary>
			protected override void OpenWriter(FileStream stream)
			{
				this.writer = new StreamWriter(stream, Encoding.UTF8);
			}

			/// <summary></summary>
			protected override void FlushWriter()
			{
				this.writer.Flush();
			}

			/// <summary></summary>
			protected override void CloseWriter()
			{
				this.writer.Close();
			}

			/// <summary></summary>
			public virtual void WriteString(string value)
			{
				if (IsEnabled && IsStarted)
				{
					this.writer.Write(value);
					RestartFlushTimer();
				}
			}

			/// <summary></summary>
			public virtual void WriteEol()
			{
				if (IsEnabled && IsStarted)
				{
					this.writer.WriteLine();
					RestartFlushTimer();
				}
			}
		}

		#endregion

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.LogSettings settings;

		private List<Log> logs;
		private List<Log> rawLogs;
		private List<Log> neatLogs;

		private BinaryLog rawTxLog;
		private BinaryLog rawBidirLog;
		private BinaryLog rawRxLog;

		private TextLog neatTxLog;
		private TextLog neatBidirLog;
		private TextLog neatRxLog;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Logs(Settings.LogSettings settings)
		{
			this.settings = settings;

			this.logs = new List<Log>();
			this.rawLogs = new List<Log>();
			this.neatLogs = new List<Log>();

			this.rawLogs.Add(this.rawTxLog = new BinaryLog(this.settings.RawLogTx, this.settings.RawTxFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.rawLogs.Add(this.rawBidirLog = new BinaryLog(this.settings.RawLogBidir, this.settings.RawBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.rawLogs.Add(this.rawRxLog = new BinaryLog(this.settings.RawLogRx, this.settings.RawRxFilePath, this.settings.WriteMode, this.settings.NameSeparator));

			this.neatLogs.Add(this.neatTxLog = new TextLog(this.settings.NeatLogTx, this.settings.NeatTxFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.neatLogs.Add(this.neatBidirLog = new TextLog(this.settings.NeatLogBidir, this.settings.NeatBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator));
			this.neatLogs.Add(this.neatRxLog = new TextLog(this.settings.NeatLogRx, this.settings.NeatRxFilePath, this.settings.WriteMode, this.settings.NameSeparator));

			this.logs.AddRange(this.rawLogs);
			this.logs.AddRange(this.neatLogs);
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
					// In the 'normal' case, End() has already been called.
					End();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Logs()
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
		public virtual Settings.LogSettings Settings
		{
			get { return (this.settings); }
			set
			{
				this.settings = value;

				this.rawTxLog.SetSettings(this.settings.RawLogTx, this.settings.RawTxFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.rawBidirLog.SetSettings(this.settings.RawLogBidir, this.settings.RawBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.rawRxLog.SetSettings(this.settings.RawLogRx, this.settings.RawRxFilePath, this.settings.WriteMode, this.settings.NameSeparator);

				this.neatTxLog.SetSettings(this.settings.NeatLogTx, this.settings.NeatTxFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.neatBidirLog.SetSettings(this.settings.NeatLogBidir, this.settings.NeatBidirFilePath, this.settings.WriteMode, this.settings.NameSeparator);
				this.neatRxLog.SetSettings(this.settings.NeatLogRx, this.settings.NeatRxFilePath, this.settings.WriteMode, this.settings.NameSeparator);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				bool isStarted = false;
				foreach (Log l in this.logs)
					isStarted = isStarted || l.IsStarted;
				return (isStarted);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Begin()
		{
			foreach (Log l in this.logs)
				l.Open();
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			foreach (Log l in this.logs)
				l.Truncate();
		}

		/// <summary></summary>
		public virtual void Flush()
		{
			foreach (Log l in this.logs)
				l.Flush();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End", Justification = "End() as method name is the obvious pair against Begin() and should be OK for other languages, .NET itself uses it in ArgIterator.End().")]
		public virtual void End()
		{
			foreach (Log l in this.logs)
				l.Close();
		}

		/// <summary></summary>
		public virtual void WriteByte(byte value, LogChannel writeChannel)
		{
			((BinaryLog)GetLog(writeChannel)).WriteByte(value);
		}

		/// <summary></summary>
		public virtual void WriteBytes(ReadOnlyCollection<byte> values, LogChannel writeChannel)
		{
			((BinaryLog)GetLog(writeChannel)).WriteBytes(values);
		}

		/// <summary></summary>
		public virtual void WriteString(string value, LogChannel writeChannel)
		{
			((TextLog)GetLog(writeChannel)).WriteString(value);
		}

		/// <summary></summary>
		public virtual void WriteEol(LogChannel writeChannel)
		{
			((TextLog)GetLog(writeChannel)).WriteEol();
		}

		/// <summary></summary>
		private Log GetLog(LogChannel channel)
		{
			return (this.logs[channel.GetHashCode()]);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
