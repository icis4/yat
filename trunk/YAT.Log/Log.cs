using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using MKY.Utilities.IO;

namespace YAT.Log
{
	public enum LogStreams
	{
		RawTx = 0,
		RawBidir = 1,
		RawRx = 2,

		NeatTx = 3,
		NeatBidir = 4,
		NeatRx = 5
	}

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

			private bool _isDisposed = false;

			private bool _enabled = false;
			private string _file;
			private LogFileWriteMode _writeMode;
			private FileNameSeparator _separator;

			private FileStream _fileStream;
			private bool _open = false;

			private Timer _flushTimer;

			#endregion

			#region Object Lifetime
			//==========================================================================================
			// Object Lifetime
			//==========================================================================================

			public Log(bool enabled, string file, LogFileWriteMode writeMode)
			{
				Initialize(enabled, file, writeMode, new FileNameSeparator(""));
			}

			public Log(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				Initialize(enabled, file, writeMode, separator);
			}

			private void Initialize(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				_enabled = enabled;
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

			protected bool IsEnabled
			{
				get { return (_enabled); }
			}

			public bool IsOpen
			{
				get { return (_open); }
			}

			#endregion

			#region Methods
			//==========================================================================================
			// Methods
			//==========================================================================================

			public void SetSettings(bool enabled, string file, LogFileWriteMode writeMode)
			{
				SetSettings(enabled, file, writeMode, new FileNameSeparator(""));
			}

			public void SetSettings(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
			{
				if (_open && (enabled != _enabled))
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
				else if (_open && (file != _file))
				{
					Close();
					Initialize(enabled, file, writeMode, separator);
					Open();
				}
				else
					Initialize(enabled, file, writeMode, separator);
			}

			public void Open()
			{
				if (!_enabled)
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
				_open = true;
			}

			public void Flush()
			{
				if (_enabled && _open)
				{
					FlushWriter();
				}
			}

			public void Truncate()
			{
				if (_enabled && _open)
				{
					CloseWriter();
					_fileStream.Close();
					_fileStream = File.Open(_file, FileMode.Truncate, FileAccess.Write, FileShare.Read);
					OpenWriter(_fileStream);
				}
			}

			public void Close()
			{
				if (_enabled && _open)
				{
					CloseWriter();
					_fileStream.Close();
				}
				_open = false;
			}

			protected abstract void OpenWriter(FileStream stream);
			protected abstract void FlushWriter();
			protected abstract void CloseWriter();

			protected void StartFlushTimer()
			{
				TimerCallback timerDelegate = new TimerCallback(_flushTimer_Timeout);
				_flushTimer = new Timer(timerDelegate, null, _FlushTimeout, System.Threading.Timeout.Infinite);
			}

			protected void RestartFlushTimer()
			{
				StopFlushTimer();
				StartFlushTimer();
			}

			protected void StopFlushTimer()
			{
				_flushTimer = null; ;
			}

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

		protected class BinaryLog : Log
		{
			private BinaryWriter _writer;

			public BinaryLog(bool enabled, string file, LogFileWriteMode writeMode)
				: base(enabled, file, writeMode)
			{
			}

			public BinaryLog(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
				: base(enabled, file, writeMode, separator)
			{
			}

			protected override void OpenWriter(FileStream stream)
			{
				_writer = new BinaryWriter(stream);
			}

			protected override void FlushWriter()
			{
				_writer.Flush();
			}

			protected override void CloseWriter()
			{
				_writer.Close();
			}

			public void WriteByte(byte value)
			{
				if (IsEnabled && IsOpen)
				{
					_writer.Write(value);
					RestartFlushTimer();
				}
			}

			public void WriteBytes(byte[] array)
			{
				if (IsEnabled && IsOpen)
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

		protected class TextLog : Log
		{
			private StreamWriter _writer;

			public TextLog(bool enabled, string file, LogFileWriteMode writeMode)
				: base(enabled, file, writeMode)
			{
			}

			public TextLog(bool enabled, string file, LogFileWriteMode writeMode, FileNameSeparator separator)
				: base(enabled, file, writeMode, separator)
			{
			}

			protected override void OpenWriter(FileStream stream)
			{
				_writer = new StreamWriter(stream);
			}

			protected override void FlushWriter()
			{
				_writer.Flush();
			}

			protected override void CloseWriter()
			{
				_writer.Close();
			}

			public void WriteString(string value)
			{
				if (IsEnabled && IsOpen)
				{
					_writer.Write(value);
					RestartFlushTimer();
				}
			}

			public void WriteEol()
			{
				if (IsEnabled && IsOpen)
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

		private bool _isDisposed = false;

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

		public bool IsOpen
		{
			get
			{
				bool isOpen = false;
				foreach (Log l in _logs)
					isOpen = isOpen || l.IsOpen;
				return (isOpen);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public void Begin()
		{
			foreach (Log l in _logs)
				l.Open();
		}

		public void Clear()
		{
			foreach (Log l in _logs)
				l.Truncate();
		}

		public void Flush()
		{
			foreach (Log l in _logs)
				l.Flush();
		}

		public void End()
		{
			foreach (Log l in _logs)
				l.Close();
		}

		public void WriteByte(byte value, LogStreams writeStream)
		{
			((BinaryLog)GetLog(writeStream)).WriteByte(value);
		}

		public void WriteBytes(byte[] array, LogStreams writeStream)
		{
			((BinaryLog)GetLog(writeStream)).WriteBytes(array);
		}

		public void WriteString(string value, LogStreams writeStream)
		{
			((TextLog)GetLog(writeStream)).WriteString(value);
		}

		public void WriteEol(LogStreams writeStream)
		{
			((TextLog)GetLog(writeStream)).WriteEol();
		}

		private Log GetLog(LogStreams stream)
		{
			return (_logs[stream.GetHashCode()]);
		}

		#endregion
	}
}
