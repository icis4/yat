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

using System.Collections.ObjectModel;
using System.IO;

namespace YAT.Log
{
	/// <summary></summary>
	internal class BinaryLog : Log
	{
		private BinaryWriter writer;

		/// <summary></summary>
		public BinaryLog(bool enabled, string filePath, LogFileWriteMode writeMode)
			: base(enabled, filePath, writeMode)
		{
		}

		/// <summary></summary>
		public BinaryLog(bool enabled, string filePath, string separator, LogFileWriteMode writeMode)
			: base(enabled, filePath, (FileNameSeparator)separator, writeMode)
		{
		}

		/// <summary></summary>
		public BinaryLog(bool enabled, string filePath, FileNameSeparator separator, LogFileWriteMode writeMode)
			: base(enabled, filePath, separator, writeMode)
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
			if (IsEnabled && IsOn)
			{
				this.writer.Write(value);
				TriggerFlushTimer();
			}
		}

		/// <summary></summary>
		public virtual void WriteBytes(ReadOnlyCollection<byte> values)
		{
			if (IsEnabled && IsOn)
			{
				byte[] array = new byte[values.Count];
				values.CopyTo(array, 0);
				this.writer.Write(array);
				TriggerFlushTimer();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
