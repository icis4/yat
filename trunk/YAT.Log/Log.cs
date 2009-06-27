//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using MKY.Utilities.IO;

namespace YAT.Log
{
	/// <summary></summary>
	public enum LogStreams
	{
		/// <summary></summary>
		RawTx = 0,
		/// <summary></summary>
		RawBidir = 1,
		/// <summary></summary>
		RawRx = 2,

		/// <summary></summary>
		NeatTx = 3,
		/// <summary></summary>
		NeatBidir = 4,
		/// <summary></summary>
		NeatRx = 5
	}

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
		protected abstract class Log : IDisposable
		{
			#region Constants
			//==========================================================================================
			// Constants
			//==========================================================================================

			private const int _FlushTimeout = 250;

			#endregion

			#region Fields
			//==========================================================================================
			// Fields
			//==========================================================================================

			private bool _isDisposed;

			private bool _isEnabled = false;
			private string _file;
			private LogFileWriteMode _writeMode;
			private FileNameSeparator _separator;

			private FileStream _fileStream;
			private bool _isStarted = false;

			private Timer _flushTimer;

			#endregion

			#region Object Lifetime
			//==========================================================================================
			// Object Lifetime
			//==========================================================================================

			/// <summary></summary>
			public Log(bool enabled, string file, LogFileWriteMode writeMode)
			{
				Initialize(enabled, file, writeMode, new FileNameSeparator(""));
			}

			/// <summary></summary>
			public Log(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				Initialize(enabled, file, writeMode, separator);
			}

			private void Initialize(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				_isEnabled = enabled;
				_file = file;
				_writeMode = writeMode;
				_separator = separator;
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
				if (!_isDisposed)
				{
					if (disposing)
					{
						Close();
					}
					_isDisposed = true;
				}
			}

			/// <summary></summary>
			~Log()
			{
				Dispose(false);
			}

			/// <summary></summary>
			protected bool IsDisposed
			{
				get { return (_isDisposed); }
			}

			/// <summary></summary>
			protected void AssertNotDisposed()
			{
				if (_isDisposed)
					throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
			}

			#endregion

			#endregion

			#region Properties
			//==========================================================================================
			// Properties
			//==========================================================================================

			/// <summary></summary>
			protected bool IsEnabled
			{
				get { return (_isEnabled); }
			}

			/// <summary></summary>
			public bool IsStarted
			{
				get { return (_isStarted); }
			}

			#endregion

			#region Methods
			//==========================================================================================
			// Methods
			//==========================================================================================

			/// <summary></summary>
			public void SetSettings(bool enabled, string file, LogFileWriteMode writeMode)
			{
				SetSettings(enabled, file, writeMode, new FileNameSeparator(""));
			}

			/// <summary></summary>
			public void SetSettings(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				if (_isStarted && (enabled != _isEnabled))
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
				else if (_isStarted && (file != _file))
				{
					Close();
					Initialize(enabled, file, writeMode, separator);
					Open();
				}
				else
					Initialize(enabled, file, writeMode, separator);
			}

			/// <summary></summary>
			public void Open()
			{
				if (!_isEnabled)
					return;

				if (!Directory.Exists(Path.GetDirectoryName(_file)))
					Directory.CreateDirectory(Path.GetDirectoryName(_file));

				if (_writeMode == LogFileWriteMode.Create)
				{
					_file = XFile.MakeUniqueFileName(_file, _separator.Separator);
					_fileStream = File.Open(_file, FileMode.Create, FileAccess.Write, FileShare.Read);
				}
				else if (_writeMode == LogFileWriteMode.Append)
				{
					if (File.Exists(_file))
						_fileStream = File.Open(_file, FileMode.Append, FileAccess.Write, FileShare.Read);
					else
						_fileStream = File.Open(_file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
				}
				else
					throw (new ArgumentException("Write mode not supported"));

				OpenWriter(_fileStream);
				_isStarted = true;
			}

			/// <summary></summary>
			public void Flush()
			{
				if (_isEnabled && _isStarted)
				{
					FlushWriter();
				}
			}

			/// <summary></summary>
			public void Truncate()
			{
				if (_isEnabled && _isStarted)
				{
					CloseWriter();
					_fileStream.Close();
					_fileStream = File.Open(_file, FileMode.Truncate, FileAccess.Write, FileShare.Read);
					OpenWriter(_fileStream);
				}
			}

			/// <summary></summary>
			public void Close()
			{
				if (_isEnabled && _isStarted)
				{
					CloseWriter();
					_fileStream.Close();
				}
				_isStarted = false;
			}

			/// <summary></summary>
			protected abstract void OpenWriter(FileStream stream);
			/// <summary></summary>
			protected abstract void FlushWriter();
			/// <summary></summary>
			protected abstract void CloseWriter();

			/// <summary></summary>
			protected void StartFlushTimer()
			{
				TimerCallback timerDelegate = new TimerCallback(_flushTimer_Timeout);
				_flushTimer = new Timer(timerDelegate, null, _FlushTimeout, System.Threading.Timeout.Infinite);
			}

			/// <summary></summary>
			protected void RestartFlushTimer()
			{
				StopFlushTimer();
				StartFlushTimer();
			}

			/// <summary></summary>
			protected void StopFlushTimer()
			{
				_flushTimer = null; ;
			}

			/// <summary></summary>
			private void _flushTimer_Timeout(object obj)
			{
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
			private BinaryWriter _writer;

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
				_writer = new BinaryWriter(stream);
			}

			/// <summary></summary>
			protected override void FlushWriter()
			{
				_writer.Flush();
			}

			/// <summary></summary>
			protected override void CloseWriter()
			{
				_writer.Close();
			}

			/// <summary></summary>
			public void WriteByte(byte value)
			{
				if (IsEnabled && IsStarted)
				{
					_writer.Write(value);
					RestartFlushTimer();
				}
			}

			/// <summary></summary>
			public void WriteBytes(byte[] array)
			{
				if (IsEnabled && IsStarted)
				{
					_writer.Write(array);
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
			private StreamWriter _writer;

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
				_writer = new StreamWriter(stream);
			}

			/// <summary></summary>
			protected override void FlushWriter()
			{
				_writer.Flush();
			}

			/// <summary></summary>
			protected override void CloseWriter()
			{
				_writer.Close();
			}

			/// <summary></summary>
			public void WriteString(string value)
			{
				if (IsEnabled && IsStarted)
				{
					_writer.Write(value);
					RestartFlushTimer();
				}
			}

			/// <summary></summary>
			public void WriteEol()
			{
				if (IsEnabled && IsStarted)
				{
					_writer.WriteLine();
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

		private bool _isDisposed;

		private Settings.LogSettings _settings;

		private List<Log> _logs;
		private List<Log> _rawLogs;
		private List<Log> _neatLogs;

		private BinaryLog _rawTxLog;
		private BinaryLog _rawBidirLog;
		private BinaryLog _rawRxLog;

		private TextLog _neatTxLog;
		private TextLog _neatBidirLog;
		private TextLog _neatRxLog;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Logs(Settings.LogSettings settings)
		{
			_settings = settings;

			_logs = new List<Log>();
			_rawLogs = new List<Log>();
			_neatLogs = new List<Log>();

			_rawLogs.Add(_rawTxLog = new BinaryLog(_settings.RawLogTx, _settings.RawTxFilePath, _settings.WriteMode, _settings.NameSeparator));
			_rawLogs.Add(_rawBidirLog = new BinaryLog(_settings.RawLogBidir, _settings.RawBidirFilePath, _settings.WriteMode, _settings.NameSeparator));
			_rawLogs.Add(_rawRxLog = new BinaryLog(_settings.RawLogRx, _settings.RawRxFilePath, _settings.WriteMode, _settings.NameSeparator));

			_neatLogs.Add(_neatTxLog = new TextLog(_settings.NeatLogTx, _settings.NeatTxFilePath, _settings.WriteMode, _settings.NameSeparator));
			_neatLogs.Add(_neatBidirLog = new TextLog(_settings.NeatLogBidir, _settings.NeatBidirFilePath, _settings.WriteMode, _settings.NameSeparator));
			_neatLogs.Add(_neatRxLog = new TextLog(_settings.NeatLogRx, _settings.NeatRxFilePath, _settings.WriteMode, _settings.NameSeparator));

			_logs.AddRange(_rawLogs);
			_logs.AddRange(_neatLogs);
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
			if (!_isDisposed)
			{
				if (disposing)
				{
					End();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Logs()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public Settings.LogSettings Settings
		{
			get { return (_settings); }
			set
			{
				_settings = value;

				_rawTxLog.SetSettings(_settings.RawLogTx, _settings.RawTxFilePath, _settings.WriteMode, _settings.NameSeparator);
				_rawBidirLog.SetSettings(_settings.RawLogBidir, _settings.RawBidirFilePath, _settings.WriteMode, _settings.NameSeparator);
				_rawRxLog.SetSettings(_settings.RawLogRx, _settings.RawRxFilePath, _settings.WriteMode, _settings.NameSeparator);

				_neatTxLog.SetSettings(_settings.NeatLogTx, _settings.NeatTxFilePath, _settings.WriteMode, _settings.NameSeparator);
				_neatBidirLog.SetSettings(_settings.NeatLogBidir, _settings.NeatBidirFilePath, _settings.WriteMode, _settings.NameSeparator);
				_neatRxLog.SetSettings(_settings.NeatLogRx, _settings.NeatRxFilePath, _settings.WriteMode, _settings.NameSeparator);
			}
		}

		/// <summary></summary>
		public bool IsStarted
		{
			get
			{
				bool isStarted = false;
				foreach (Log l in _logs)
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
		public void Begin()
		{
			foreach (Log l in _logs)
				l.Open();
		}

		/// <summary></summary>
		public void Clear()
		{
			foreach (Log l in _logs)
				l.Truncate();
		}

		/// <summary></summary>
		public void Flush()
		{
			foreach (Log l in _logs)
				l.Flush();
		}

		/// <summary></summary>
		public void End()
		{
			foreach (Log l in _logs)
				l.Close();
		}

		/// <summary></summary>
		public void WriteByte(byte value, LogStreams writeStream)
		{
			((BinaryLog)GetLog(writeStream)).WriteByte(value);
		}

		/// <summary></summary>
		public void WriteBytes(byte[] array, LogStreams writeStream)
		{
			((BinaryLog)GetLog(writeStream)).WriteBytes(array);
		}

		/// <summary></summary>
		public void WriteString(string value, LogStreams writeStream)
		{
			((TextLog)GetLog(writeStream)).WriteString(value);
		}

		/// <summary></summary>
		public void WriteEol(LogStreams writeStream)
		{
			((TextLog)GetLog(writeStream)).WriteEol();
		}

		/// <summary></summary>
		private Log GetLog(LogStreams stream)
		{
			return (_logs[stream.GetHashCode()]);
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
