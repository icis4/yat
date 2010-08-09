//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.IO;

namespace MKY.Utilities.IO
{
	/// <summary>
	/// Thread-safe log file.
	/// </summary>
	public class LogFile
	{
		private string filePath;
		private StreamWriter writer;

		/// <summary>
		/// Starts Logfile.
		/// </summary>
		/// <param name="filePath">Path of log file.</param>
		/// <param name="append">true to append to file, false to replace file.</param>
		public LogFile(string filePath, bool append)
		{
			this.filePath = filePath;
			this.writer = new System.IO.StreamWriter(this.filePath, append);
		}

		/// <summary>
		/// Returns complete path of log file.
		/// </summary>
		public string FilePath
		{
			get { return (this.filePath); }
		}

		/// <summary>
		/// Returns underlying stream.
		/// </summary>
		public Stream UnderlyingStream
		{
			get { return (this.writer.BaseStream); }
		}

		/// <summary>
		/// Writes a line into log file and adds a timestamp.
		/// </summary>
		public virtual void WriteLine(string line)
		{
			DateTime now = DateTime.Now;
			try
			{
				lock (this.writer)
				{
					this.writer.WriteLine(now.ToString("HH:mm:ss.") + string.Format("{0:000}", now.Millisecond) + "  " + line);
					this.writer.Flush();
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Closes log file.
		/// </summary>
		public virtual void Close()
		{
			try
			{
				lock (this.writer)
				{
					this.writer.Close();
				}
			}
			catch
			{
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
