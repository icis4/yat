using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

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

	public class Logs
	{
		protected abstract class Log
		{
			private bool _enabled;
			private string _file;
			private LogFileWriteMode _writeMode;
			private FileNameSeparator _separator;

			private FileStream _fileStream;
			private bool _open = false;

			private const int _FLUSH_TIMEOUT = 250;
			private Timer _flushTimer;

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

			~Log()
			{
				Close();
			}

			protected bool IsEnabled
			{
				get { return (_enabled); }
			}

			public bool IsOpen
			{
				get { return (_open); }
			}

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
					_file = Utilities.IO.XFile.MakeUniqueFileName(_file, _separator.Separator);
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
				_flushTimer = new Timer(timerDelegate, null, _FLUSH_TIMEOUT, System.Threading.Timeout.Infinite);
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
		}

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
	}
}
