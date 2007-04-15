using System;
using System.IO;

namespace HSR.Utilities.IO
{
	/// <summary>
	/// Thread-safe log file.
	/// </summary>
	public class Logfile
	{
		private string _path;
		private StreamWriter _writer;

		/// <summary>
		/// Starts Logfile.
		/// </summary>
		/// <param name="path">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public Logfile(string path, bool append)
		{
			_path = path;
			_writer = new System.IO.StreamWriter(path, append);
		}

		/// <summary>
		/// Returns complete path of log file.
		/// </summary>
		public String Path
		{
			get
			{
				return (_path);
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
