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

using System.IO;
using System.Text;

namespace YAT.Log
{
	/// <summary></summary>
	internal class TextLog : Log
	{
		private StreamWriter writer;

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, LogFileWriteMode writeMode)
			: base(enabled, filePath, writeMode)
		{
		}

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, LogFileWriteMode writeMode, string separator)
			: base(enabled, filePath, writeMode, (FileNameSeparator)separator)
		{
		}

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, LogFileWriteMode writeMode, FileNameSeparator separator)
			: base(enabled, filePath, writeMode, separator)
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
			if (IsEnabled && IsOn)
			{
				this.writer.Write(value);
				RestartFlushTimer();
			}
		}

		/// <summary></summary>
		public virtual void WriteEol()
		{
			if (IsEnabled && IsOn)
			{
				this.writer.WriteLine();
				RestartFlushTimer();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
