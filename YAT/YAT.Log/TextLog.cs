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
		private Encoding encoding;

		private StreamWriter writer;

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, LogFileWriteMode writeMode)
			: base(enabled, filePath, writeMode)
		{
			this.encoding = Encoding.UTF8;
		}

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, LogFileWriteMode writeMode, Encoding encoding)
			: base(enabled, filePath, writeMode)
		{
			this.encoding = encoding;
		}

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, string separator, LogFileWriteMode writeMode, Encoding encoding)
			: base(enabled, filePath, (FileNameSeparator)separator, writeMode)
		{
			this.encoding = encoding;
		}

		/// <summary></summary>
		public TextLog(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode, Encoding encoding)
			: base(enabled, filePath, separator, writeMode)
		{
			this.encoding = encoding;
		}

		/// <summary></summary>
		public void SetSettings(bool enabled, string filePath, LogFileWriteMode writeMode, Encoding encoding)
		{
			if (this.IsEnabled && this.IsOn && (this.encoding != encoding))
			{
				Close();
				this.encoding = encoding;
				base.SetSettings(enabled, filePath, writeMode);
				Open();
			}
			else
			{
				this.encoding = encoding;
			}
		}

		/// <summary></summary>
		public void SetSettings(bool enabled, string filePath, string separator, LogFileWriteMode writeMode, Encoding encoding)
		{
			if (this.IsEnabled && this.IsOn && (this.encoding != encoding))
			{
				Close();
				this.encoding = encoding;
				base.SetSettings(enabled, filePath, separator, writeMode);
				Open();
			}
			else
			{
				this.encoding = encoding;
			}
		}

		/// <summary></summary>
		public void SetSettings(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode, Encoding encoding)
		{
			if (this.IsEnabled && this.IsOn && (this.encoding != encoding))
			{
				Close();
				this.encoding = encoding;
				base.SetSettings(enabled, filePath, separator, writeMode);
				Open();
			}
			else
			{
				this.encoding = encoding;
			}
		}

		/// <summary></summary>
		protected override void OpenWriter(FileStream stream)
		{
			this.writer = new StreamWriter(stream, this.encoding);
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
