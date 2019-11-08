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
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

using YAT.Application.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="BinaryTerminal"/>.
	/// </remarks>
	public partial class BinaryTerminal
	{
		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Send Data
		//------------------------------------------------------------------------------------------
		// Send Data
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override void SendFileLine(string dataLine, Radix defaultRadix)
		{
			// AssertNotDisposed() is called by DoSendData().

			var parseMode = TerminalSettings.Send.File.ToParseMode();

			DoSendData(new TextDataSendItem(dataLine, defaultRadix, parseMode, SendMode.File, true));
		}

		/// <remarks>Shall not be called if keywords are disabled.</remarks>
		protected override void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Eol:
				case Parser.Keyword.NoEol:
				{
					// Add space if necessary:
					if (ElementsAreSeparate(IODirection.Tx))
					{
						if (this.txLineState.Elements.ByteCount > 0)
							OnDisplayElementAdded(IODirection.Tx, new DisplayElement.DataSpace());
					}

					string info = ((Parser.KeywordEx)(result.Keyword)) + " keyword is not supported for binary terminals";
					OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, info));
					break;
				}

				default:
				{
					base.ProcessInLineKeywords(result);
					break;
				}
			}
		}

		#endregion

		#region Send File
		//------------------------------------------------------------------------------------------
		// Send File
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// The 'Send*File' methods use the 'Send*Data' methods for sending of packets/lines.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected override void ProcessSendFileItem(FileSendItem item)
		{
			try
			{
				if (ExtensionHelper.IsXmlFile(item.FilePath))
				{
					ProcessSendXmlFileItem(item);
				}
				else if (ExtensionHelper.IsTextFile(item.FilePath))
				{
					ProcessSendTextFileItem(item);
				}
				else // By default treat as binary file:
				{
					using (FileStream fs = File.OpenRead(item.FilePath))
					{
						long remaining = fs.Length;
						while (remaining > 0)
						{
							byte[] a = new byte[1024]; // 1 KB chunks.
							int n = fs.Read(a, 0, a.Length);
							Array.Resize<byte>(ref a, n);
							Send(a);
							remaining -= n;

							if (BreakSendFile)
							{
								OnIOChanged(EventArgs.Empty); // Raise the event to indicate that sending is no longer ongoing.
								break;
							}

							Thread.Sleep(TimeSpan.Zero); // Yield to other threads to e.g. allow refreshing of view.
						}
					}

					// Note that 'item.DefaultRadix' is not used for sending binary files.
					// This fact is considered in 'View.Controls.SendFile.SetRecentAndCommandControls()'.
					// Changes in behavior above will have to be adapted in that control method as well.
				}
			}
			catch (Exception ex)
			{
				OnDisplayElementAdded(IODirection.Tx, new DisplayElement.ErrorInfo(Direction.Tx, @"Error reading file """ + item.FilePath + @""": " + ex.Message));
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
