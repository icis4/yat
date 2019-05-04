using System;
using System.IO;

namespace MKY.Utilities.IO
{
	/// <summary>
	/// Thread-safe log file.
	/// </summary>
	public class LogFile
	{
		private string _filePath;
		private StreamWriter _writer;

		/// <summary>
		/// Starts Logfile.
		/// </summary>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public LogFile(string filePath, bool append)
		{
			_filePath = filePath;
			_writer = new System.IO.StreamWriter(_filePath, append);
		}

		/// <summary>
		/// Returns complete path of log file.
		/// </summary>
		public String FilePath
		{
			get
			{
				return (_filePath);
			}
		}

		/// <summary>
		/// Returns underlying stream.
		/// </summary>
		public Stream UnderlyingStream
		{
			get
			{
				lock (_writer)
				{
					return (_writer.BaseStream);
				}
			}
		}

		/// <summary>
		/// Writes a line into log file and adds a timestamp.
		/// </summary>
		public void WriteLine(string line)
		{
			DateTime now = DateTime.Now;
			try
			{
				lock (_writer)
				{
					_writer.WriteLine(now.ToString("HH:mm:ss.") + string.Format("{0:000}", now.Millisecond) + "  " + line);
					_writer.Flush();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Closes log file.
		/// </summary>
		public void Close()
		{
			try
			{
				lock (_writer)
				{
					_writer.Close();
				}
			}
			catch
			{
			}
		}
	}
}