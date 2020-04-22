//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using YAT.Application.Utilities;
using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="BinaryTerminal"/>.
	/// </remarks>
	public partial class BinaryTerminal
	{
		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected override void ProcessInLineKeywords(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, Parser.KeywordResult result, Queue<byte> conflateDataQueue, ref bool doBreakSend)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				case Parser.Keyword.NoEol:
				{
					string message = ((Parser.KeywordEx)(result.Keyword)) + " keyword is not supported for binary terminals!";
					InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, message));
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(sendingIsBusyChangedEventHelper, result, conflateDataQueue, ref doBreakSend);
					break;
				}
			}
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void DoSendFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					DoSendXmlFileItem(sendingIsBusyChangedEventHelper, item);
				}
				else if (ExtensionHelper.IsTextFile(item.FilePath))
				{
					DoSendTextFileItem(sendingIsBusyChangedEventHelper, item);
				}
				else // By default treat as binary file:
				{
					DoSendBinaryFileItem(sendingIsBusyChangedEventHelper, item);
				}
			}
			catch (Exception ex)
			{
				InlineDisplayElement(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
		}

		/// <remarks>
		/// <paramref name="sendingIsBusyChangedEventHelper"/> is located first as needed down the call chain.
		/// </remarks>
		protected virtual void DoSendBinaryFileItem(SendingIsBusyChangedEventHelper sendingIsBusyChangedEventHelper, FileSendItem item)
		{
			using (FileStream fs = File.OpenRead(item.FilePath))
			{
				long remaining = fs.Length;
				while (remaining > 0)
				{
					byte[] chunk = new byte[1024]; // 1 KB chunks.
					int n = fs.Read(chunk, 0, chunk.Length);
					Array.Resize<byte>(ref chunk, n);

					DoSendRawData(sendingIsBusyChangedEventHelper, chunk);

					remaining -= n;

					if (DoBreak)
						break;

					Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
				}
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
