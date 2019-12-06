﻿//==================================================================================================
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Threading.Tasks; activate after having upgraded to .NET 4.0
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;

using YAT.Model.Types;
using YAT.Model.Utilities;

#endregion

namespace YAT.Model
{
	/// <remarks>
	/// This partial class implements the automatic response part of <see cref="Terminal"/>.
	/// </remarks>
	public partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int autoResponseCount;
		private AutoTriggerHelper autoResponseTriggerHelper;
		private object autoResponseTriggerHelperSyncObj = new object();
	////private Queue<Action<byte[], string>> autoResponseTasks = new Queue<Action<byte[], string>>(); activate after having upgraded to .NET 4.0

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void CreateAutoResponseHelper()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.
		}

		private void DisposeAutoResponseHelper()
		{
			lock (this.autoResponseTriggerHelperSyncObj)
				this.autoResponseTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Updates the automatic response helper.
		/// </summary>
		protected virtual void UpdateAutoResponse()
		{
			if (SettingsRoot.AutoResponse.IsActive)
			{
				if (SettingsRoot.AutoResponse.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					Command triggerCommand;
					string  triggerTextOrRegexPattern;
					Regex   triggerRegex;

					if (SettingsRoot.TryGetActiveAutoResponseTrigger(out triggerCommand, out triggerTextOrRegexPattern, out triggerRegex))
					{
						if (SettingsRoot.AutoResponse.IsByteSequenceTriggered)
						{
							byte[] triggerSequence;
							if (TryParseCommandToSequence(triggerCommand, out triggerSequence))
							{
								lock (this.autoResponseTriggerHelperSyncObj)
									this.autoResponseTriggerHelper = new AutoTriggerHelper(triggerSequence);
							}
							else
							{
								DeactivateAutoResponse();
								DisposeAutoResponseHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic response trigger! The trigger does not specify valid YAT command text! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
									"To enable again, re-configure the automatic response.",
									"Automatic Response Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning
								);
							}
						}
						else // IsTextTriggered
						{
							lock (this.autoResponseTriggerHelperSyncObj)
								this.autoResponseTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoResponse.Options.CaseSensitive, SettingsRoot.AutoResponse.Options.WholeWord, triggerRegex);
						}
					}
					else if (this.autoIsReady) // See remarks of 'Terminal.NotifyAutoIsReady()' for background.
					{
						DeactivateAutoResponse();
						DisposeAutoResponseHelper();

						OnMessageInputRequest
						(
							"Failed to retrieve the automatic response trigger! The trigger is not available! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic response.",
							"Automatic Response Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
					else // (!this.autoIsReady)
					{
						// Ignore, update will again be invoked after e.g. having loaded linked settings.
						// See remarks of 'Terminal.NotifyAutoIsReady()' for background.
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					DisposeAutoResponseHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoResponseHelper();
			}
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromElements(Domain.DisplayElementCollection elements)
		{
			List<Pair<byte[], string>> triggersDummy;
			EvaluateAutoResponseFromElements(elements, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromElements(Domain.DisplayElementCollection elements, out List<Pair<byte[], string>> triggers)
		{
			triggers = new List<Pair<byte[], string>>(); // No preset needed, the default behavior is good enough.

			foreach (var de in elements)
			{
				if (de.Direction == Domain.Direction.Rx) // By specification only active on receive-path.
				{
					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
						{
							if (de.Origin != null) // Foreach element where origin exists.
							{
								foreach (var origin in de.Origin)
								{
									foreach (var originByte in origin.Value1)
									{
										if (this.autoResponseTriggerHelper.EnqueueAndMatchTrigger(originByte))
										{
											this.autoResponseTriggerHelper.Reset();
											de.Highlight = true;

											// Signal the trigger:
											triggers.Add(new Pair<byte[], string>(this.autoResponseTriggerHelper.TriggerSequence, null));
										}
									}
								}
							}
						}
						else
						{
							break;     // Break the loop if response got disposed in the meantime.
						}              // Though unlikely, it may happen when deactivating response
					} // lock (helper) // while receiving a very large chunk.
				} // if (direction == Rx)
			} // foreach (element)
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromLines(Domain.DisplayLineCollection lines)
		{
			List<Pair<byte[], string>> triggersDummy;
			EvaluateAutoResponseFromLines(lines, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromLines(Domain.DisplayLineCollection lines, out List<Pair<byte[], string>> triggers)
		{
			if (SettingsRoot.AutoResponse.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoResponseFromElements(dl, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Pair<byte[], string>>(); // No preset needed, the default behavior is good enough.

				foreach (var dl in lines)
				{
					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
						{
							int triggerCount = this.autoResponseTriggerHelper.TextTriggerCount(dl.Text);
							if (triggerCount > 0)
							{
								this.autoResponseTriggerHelper.Reset(); // Invoke shall happen as short as possible after detection.
								dl.Highlight = true;

								// Signal the trigger(s):
								for (int i = 0; i < triggerCount; i++)                             // Use for [Trigger] response.
									triggers.Add(new Pair<byte[], string>(null, this.autoResponseTriggerHelper.TriggerTextOrRegexPattern));
							}
						}
						else
						{
							break;     // Break the loop if response got disposed in the meantime.
						}              // Though unlikely, it may happen when deactivating response
					} // lock (helper) // while processing many lines, e.g. on reload.
				} // foreach (line)
			} // IsTextTriggered
		}

		/// <summary>
		/// Invokes sending of the automatic response on an other than the receive thread.
		/// </summary>
		protected virtual void InvokeAutoResponse(byte[] triggerSequence, string triggerText)
		{
		////Replace by EnqueueAutoResponse(); after having upgraded to .NET 4.0

			var asyncInvoker = new Action<byte[], string>(SendAutoResponse);
			asyncInvoker.BeginInvoke(triggerSequence, triggerText, null, null);
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence, string triggerText)
		{
		////while (this.autoResponseTasks.Count > 0) after having upgraded to .NET 4.0
		////{
		////	var task = this.autoResponseTasks.Dequeue();
		////	task.Invoke(...);

			// \remind (2019-12-03 / MKY)
			// Until .NET 4.0, there is an (acceptable) limitation with the current implementation:
			// If multiple triggers are detected within the same chunk or close after each other,
			// the sequence of the responses may get mixed up. For 'AutoResponse', this should not
			// be an issue, but for 'AutoAction::MessageBox' this is not "nice".

			int count = Interlocked.Increment(ref this.autoResponseCount); // Incrementing before invoking to have the effective count updated when sending.
			OnAutoResponseCountChanged(new EventArgs<int>(count));

			AutoResponseEx response = SettingsRoot.AutoResponse.Response;
			int page = SettingsRoot.Predefined.SelectedPageId;
			switch ((AutoResponse)response)
			{
				case AutoResponse.None:
					// Nothing to do.
					break;

				case AutoResponse.Trigger:             SendAutoResponseTrigger(triggerSequence, triggerText); break;
				case AutoResponse.PredefinedCommand1:  SendPredefined(page, 1);  break;
				case AutoResponse.PredefinedCommand2:  SendPredefined(page, 2);  break;
				case AutoResponse.PredefinedCommand3:  SendPredefined(page, 3);  break;
				case AutoResponse.PredefinedCommand4:  SendPredefined(page, 4);  break;
				case AutoResponse.PredefinedCommand5:  SendPredefined(page, 5);  break;
				case AutoResponse.PredefinedCommand6:  SendPredefined(page, 6);  break;
				case AutoResponse.PredefinedCommand7:  SendPredefined(page, 7);  break;
				case AutoResponse.PredefinedCommand8:  SendPredefined(page, 8);  break;
				case AutoResponse.PredefinedCommand9:  SendPredefined(page, 9);  break;
				case AutoResponse.PredefinedCommand10: SendPredefined(page, 10); break;
				case AutoResponse.PredefinedCommand11: SendPredefined(page, 11); break;
				case AutoResponse.PredefinedCommand12: SendPredefined(page, 12); break;
				case AutoResponse.SendText:            SendText();               break;
				case AutoResponse.SendFile:            SendFile();               break;

				case AutoResponse.Explicit:
					SendCommand(new Command(response)); // No explicit default radix available (yet).
					break;

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + response + "' is an automatic response that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void SendAutoResponseTrigger(byte[] triggerSequence, string triggerText)
		{
			if (!ArrayEx.IsNullOrEmpty(triggerSequence))                  // If sequence is given, use that, no matter
				this.terminal.Send(ToSequenceWithTxEol(triggerSequence)); // whether 'UseText' is enabled or not.
			else
				this.terminal.SendTextLine(triggerText);
		}

		/// <summary>
		/// Helper method to get the byte sequence including EOL.
		/// </summary>
		protected virtual byte[] ToSequenceWithTxEol(byte[] sequence)
		{
			var l = new List<byte>(sequence);

			if (SettingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Add Tx EOL:
				var txEolSequence = textTerminal.TxEolSequence;
				l.AddRange(txEolSequence);
			}

			return (l.ToArray());
		}

		/// <summary>
		/// Helper method to get the byte sequence from a display line.
		/// </summary>
		protected virtual byte[] ToOriginWithoutRxEol(Domain.DisplayLine dl)
		{
			var l = new List<byte>(dl.ElementsToOrigin());

			if (SettingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Remove Rx EOL:
				if (SettingsRoot.TextTerminal.ShowEol)
				{
					var rxEolSequence = textTerminal.RxEolSequence;
					l.RemoveRange((l.Count - rxEolSequence.Length), rxEolSequence.Length);
				}
			}

			return (l.ToArray());
		}

		/// <summary>
		/// Gets the automatic response count.
		/// </summary>
		public virtual int AutoResponseCount
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.autoResponseCount);
			}
		}

		/// <summary>
		/// Resets the automatic response count.
		/// </summary>
		public virtual void ResetAutoResponseCount()
		{
			AssertNotDisposed();

			int count = Interlocked.Exchange(ref this.autoResponseCount, 0);
			OnAutoResponseCountChanged(new EventArgs<int>(count));
		}

		/// <summary>
		/// Deactivates the automatic response.
		/// </summary>
		public virtual void DeactivateAutoResponse()
		{
			AssertNotDisposed();

			SettingsRoot.AutoResponse.Deactivate();
			ResetAutoResponseCount();
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnAutoResponseCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================