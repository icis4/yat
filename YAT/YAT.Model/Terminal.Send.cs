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
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Model.Settings;
using YAT.Model.Types;

#endregion

namespace YAT.Model
{
	/// <remarks>
	/// This partial class implements the send part of <see cref="Terminal"/>.
	/// </remarks>
	public partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Partial commands:
		private string partialCommandLine;

		/// <summary>
		/// Synchronization object for sending.
		/// </summary>
		protected object SendSyncObj { get; } = new object();

		#endregion

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		#region Terminal > Send Command
		//------------------------------------------------------------------------------------------
		// Terminal > Send Command
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends given command.
		/// </summary>
		public virtual void SendCommand(Command command)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				if (command.IsText)
					SendText(command);
				else if (command.IsFilePath)
					SendFile(command);
				else
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + command + "' does not specify a known command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "command"));
			}
		}

		#endregion

	#if (WITH_SCRIPTING)

		#region Terminal > Send Raw
		//------------------------------------------------------------------------------------------
		// Terminal > Send Raw
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends the specified raw data.
		/// </summary>
		public virtual void SendRaw(byte[] data)
		{
			AssertNotDisposed();

			lock (SendSyncObj)
			{
				OnFixedStatusTextRequest("Sending " + data.Length + " bytes...");
				try
				{
					this.terminal.Send(data);
				}
				catch (IOException ex)
				{
					OnFixedStatusTextRequest("Error sending " + data.Length + " bytes!");

					string text;
					string title;
					ComposeSendRawErrorMessage(out text, out title);
					OnMessageInputRequest
					(
						text + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine +
						ex.Message,
						title,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					OnTimedStatusTextRequest("Data not sent!");
				}
			}
		}

		private void ComposeSendRawErrorMessage(out string text, out string title)
		{
			StringBuilder textBuilder = new StringBuilder();
			StringBuilder titleBuilder = new StringBuilder();

			textBuilder.Append("Unable to write to ");
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
					textBuilder.Append("port");
					titleBuilder.Append("Serial Port");
					break;

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.UdpClient:
				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
					textBuilder.Append("socket");
					titleBuilder.Append("Socket");
					break;

				case Domain.IOType.UsbSerialHid:
					textBuilder.Append("device");
					titleBuilder.Append("Device");
					break;

				default:
					throw (new NotImplementedException("I/O type " + this.settingsRoot.IOType + "misses implementation"));
			}
			textBuilder.Append(":");
			titleBuilder.Append(" Error!");

			text = textBuilder.ToString();
			title = titleBuilder.ToString();
		}

		#endregion

	#endif // WITH_SCRIPTING

		#region Terminal > Send Text
		//------------------------------------------------------------------------------------------
		// Terminal > Send Text
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendText()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				SendText(this.settingsRoot.SendText.Command);

				// Clear command if desired:
				if (!this.settingsRoot.Send.Text.KeepSendText)
					this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
			}
		}

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendTextWithoutEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				SendTextWithoutEol(this.settingsRoot.SendText.Command);

				// Clear command if desired:
				if (!this.settingsRoot.Send.Text.KeepSendText)
					this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
			}
		}

		/// <summary>
		/// Sends partial text EOL.
		/// </summary>
		public virtual void SendPartialTextEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				SendText(new Command(true, this.settingsRoot.SendText.Command.DefaultRadix));
			}
		}

		/// <summary>
		/// Sends given text.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public virtual void SendText(string text)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				SendText(new Command(text));
			}
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendText(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			lock (SendSyncObj)
			{
				if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
					DoSendText(command);
				else
					DoSendText(command.ToCommandWithoutDefaultRadix());
			}
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendTextWithoutEol(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			lock (SendSyncObj)
			{
				if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
					DoSendTextWithoutEol(command);
				else
					DoSendTextWithoutEol(command.ToCommandWithoutDefaultRadix());
			}
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendText(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendTextLine(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsMultiLineText)
				{
					this.terminal.SendTextLines(c.MultiLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else if (c.IsPartialTextEol)
				{
					this.terminal.SendTextLine("", Domain.Radix.String);
				}
				else // This indicates an invalid operation, since a command must have been validated before calling this method!
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else // This indicates an invalid operation, since a command must have been validated before calling this method!
			{
			////throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));

				// Temporary workaround until bug #469 has been resolved:
				OnMessageInputRequest
				(
					"Command '" + c + "' does not specify a valid text command!",
					"Invalid Text Command",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
			}
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendTextWithoutEol(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendText(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else // Covers 'c.IsMultiLineText' and 'c.IsPartialTextEol' which are invalid 'WithoutEol'.
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Includes compiled partial text.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentTextCommandsIfNeeded(Command c)
		{
			if (c.IsSingleLineText || c.IsMultiLineText /* || do not add c.IsPartialText to recents */ || c.IsPartialTextEol)
			{
				// Clone the command for the recent commands collection:
				Command clone;
				if (c.IsSingleLineText || c.IsMultiLineText)
				{
					clone = new Command(c); // 'Normal' case, simply clone the command.
				}
				else if (c.IsPartialTextEol)
				{                                        // Create a single line text command,
					if (this.partialCommandLine != null) // using the previously sent characters:
						clone = new Command(this.partialCommandLine, false, c.DefaultRadix);
					else
						clone = new Command(c); // Keep partial EOL.
				}
				else
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Condition is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				// Reset the partial command line, in any case:
				this.partialCommandLine = null;

				// Clear potential description, as that shall not be considered for recents,
				// e.g. same command with different description shall only be listed once:
				clone.ClearDescription();

				// Put clone into recent history:
				this.settingsRoot.SendText.RecentCommands.Add(new RecentItem<Command>(clone));
				this.settingsRoot.SendText.SetChanged(); // Manual change required because underlying collection is modified.
			}
		}

		#endregion

		#region Terminal > Send File
		//------------------------------------------------------------------------------------------
		// Terminal > Send File
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends file given by terminal settings.
		/// </summary>
		public virtual void SendFile()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			lock (SendSyncObj)
			{
				SendFile(this.settingsRoot.SendFile.Command);
			}
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="command">File to be sent.</param>
		public virtual void SendFile(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			lock (SendSyncObj)
			{
				if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
					DoSendFile(command);
				else
					DoSendFile(command.ToCommandWithoutDefaultRadix());
			}
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'symmetricity' is a correct English term.")]
		protected virtual void DoSendFile(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				string absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(c.FilePath, Path.GetDirectoryName(SettingsFilePath));

				this.terminal.SendFile(absoluteFilePath, c.DefaultRadix);

				CloneIntoRecentFileCommands(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid file command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentFileCommands(Command c)
		{
			// Clone the command for the recent commands collection:
			var clone = new Command(c);

			// Clear potential description, as that shall not be considered for recents,
			// e.g. same command with different description shall only be listed once:
			clone.ClearDescription();

			// Put clone into recent history:
			this.settingsRoot.SendFile.RecentCommands.Add(new RecentItem<Command>(clone));
			this.settingsRoot.SendFile.SetChanged(); // Manual change required because underlying collection is modified.
		}

		#endregion

		#region Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------
		// Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Send requested predefined command.
		/// </summary>
		/// <param name="pageId">Page 1..max.</param>
		/// <param name="commandId">Command 1..max.</param>
		public virtual bool SendPredefined(int pageId, int commandId)
		{
			AssertNotDisposed();

			lock (SendSyncObj)
			{
				// Verify arguments:
				if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(pageId - 1, commandId - 1))
					return (false);

				// Process command:
				var c = this.settingsRoot.PredefinedCommand.Pages[pageId - 1].Commands[commandId - 1];
				if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
				{
					SendText(c);

					if (this.settingsRoot.Send.CopyPredefined)
						this.settingsRoot.SendText.Command = new Command(c); // Clone to ensure decoupling.

					return (true);
				}
				else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
				{
					SendFile(c);

					if (this.settingsRoot.Send.CopyPredefined)
						this.settingsRoot.SendFile.Command = new Command(c); // Clone to ensure decoupling.

					return (true);
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary>
		/// Copy the requested predefined command, not taking copy predefined settings
		/// <see cref="Domain.Settings.SendSettings.CopyPredefined"/> into account.
		/// </summary>
		/// <param name="pageId">Page 1..max.</param>
		/// <param name="commandId">Command 1..max.</param>
		public virtual bool CopyPredefined(int pageId, int commandId)
		{
			AssertNotDisposed();

			lock (SendSyncObj)
			{
				// Verify arguments:
				if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(pageId - 1, commandId - 1))
					return (false);

				// Process command:
				var c = this.settingsRoot.PredefinedCommand.Pages[pageId - 1].Commands[commandId - 1];
				if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
				{
					this.settingsRoot.SendText.Command = new Command(c); // Clone to ensure decoupling.
					return (true);
				}
				else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
				{
					this.settingsRoot.SendFile.Command = new Command(c); // Clone to ensure decoupling.
					return (true);
				}
				else
				{
					return (false);
				}
			}
		}

		#endregion

		#region Terminal > Break/Resume
		//------------------------------------------------------------------------------------------
		// Terminal > Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			AssertNotDisposed();

			this.terminal.Break();
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			AssertNotDisposed();

			this.terminal.ResumeBreak();
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
