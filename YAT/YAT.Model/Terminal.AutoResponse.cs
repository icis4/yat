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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.Diagnostics;
using MKY.Text.RegularExpressions;

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

		private Queue<Triple<byte[], string, MatchCollection>> autoResponseQueue = new Queue<Triple<byte[], string, MatchCollection>>();
		private bool autoResponseThreadRunFlag;
		private AutoResetEvent autoResponseThreadEvent;
		private Thread autoResponseThread;
		private object autoResponseThreadSyncObj = new object();

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

		private void CreateAutoResponse()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.

			CreateAndStartAutoResponseThread();
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
								this.autoResponseTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoResponse.TriggerOptions.CaseSensitive, SettingsRoot.AutoResponse.TriggerOptions.WholeWord, triggerRegex);
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

		private void DisposeAutoResponseHelper()
		{
			lock (this.autoResponseTriggerHelperSyncObj)
				this.autoResponseTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromElements(Domain.DisplayElementCollection elements)
		{
			List<Triple<byte[], string, MatchCollection>> triggersDummy;
			EvaluateAutoResponseFromElements(elements, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromElements(Domain.DisplayElementCollection elements, out List<Triple<byte[], string, MatchCollection>> triggers)
		{
			triggers = new List<Triple<byte[], string, MatchCollection>>(); // No preset needed, the default behavior is good enough.

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

											// Signal the trigger:                                   // Always use sequence for [Trigger] response, since always 'IsByteSequenceTriggered' when evaluated here.
											triggers.Add(new Triple<byte[], string, MatchCollection>(this.autoResponseTriggerHelper.TriggerSequence, null, null));
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
			List<Triple<byte[], string, MatchCollection>> triggersDummy;
			EvaluateAutoResponseFromLines(lines, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoResponseFromLines(Domain.DisplayLineCollection lines, out List<Triple<byte[], string, MatchCollection>> triggers)
		{
			if (SettingsRoot.AutoResponse.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoResponseFromElements(dl, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Triple<byte[], string, MatchCollection>>(); // No preset needed, the default behavior is good enough.

				foreach (var dl in lines)
				{
					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
						{
							MatchCollection triggerMatches;
							int triggerCount = this.autoResponseTriggerHelper.TextTriggerCount(dl.Text, out triggerMatches);
							if (triggerCount > 0)
							{
								this.autoResponseTriggerHelper.Reset(); // Invoke shall happen as short as possible after detection.
								dl.Highlight = true;

								// Signal the trigger(s):
								for (int i = 0; i < triggerCount; i++)                             // Always use text for [Trigger] response, since always 'IsTextTriggered' when evaluated here.
									triggers.Add(new Triple<byte[], string, MatchCollection>(null, this.autoResponseTriggerHelper.TriggerTextOrRegexPattern, triggerMatches));
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
		/// Enqueues the automatic responses for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoResponses(List<Triple<byte[], string, MatchCollection>> triggers)
		{
			foreach (var trigger in triggers)
				EnqueueAutoResponse(trigger.Value1, trigger.Value2, trigger.Value3);
		}

		/// <summary>
		/// Enqueues the automatic response for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoResponse(byte[] triggerSequence, string triggerText, MatchCollection triggerMatches)
		{
			lock (this.autoResponseQueue) // Lock is required because Queue<T> is not synchronized.
				this.autoResponseQueue.Enqueue(new Triple<byte[], string, MatchCollection>(triggerSequence, triggerText, triggerMatches));

			SignalAutoResponseThreadSafely();
		}

		/// <summary>
		/// Asynchronously invoke the automatic responses on other than the receive thread.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="EnqueueAutoResponse(byte[], string, MatchCollection)"/> above.
		/// </remarks>
		private void AutoResponseThread()
		{
			DebugThreadState("AutoResponseThread() has started.");

			try
			{
				// Outer loop, processes data after a signal has been received:
				while (!IsDisposed && this.autoResponseThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.autoResponseThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in AutoResponseThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.autoResponseThreadRunFlag && IsReadyToSend && (this.autoResponseQueue.Count > 0))
					{                                                                   // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue,
						// since it is likely that more triggers are to be enqueued.
						Thread.Sleep(TimeSpan.Zero);

						Triple<byte[], string, MatchCollection>[] pendingItems;
						lock (this.autoResponseQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.autoResponseQueue.ToArray();
							this.autoResponseQueue.Clear();
						}

						foreach (var item in pendingItems)
						{
							SendAutoResponse(item.Value1, item.Value2, item.Value3);
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "AutoResponseThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreadState("AutoResponseThread() has terminated.");
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence, string triggerText, MatchCollection triggerMatches)
		{
			int count = Interlocked.Increment(ref this.autoResponseCount); // Incrementing before invoking to have the effective count updated when sending.
			OnAutoResponseCountChanged(new EventArgs<int>(count));

			AutoResponseEx response = SettingsRoot.AutoResponse.Response;
			int page = SettingsRoot.Predefined.SelectedPageId;
			switch ((AutoResponse)response)
			{
				case AutoResponse.None:
				{
					// Nothing to do.
					break;
				}

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
				{
					string textLine = response;

					if (SettingsRoot.AutoResponse.ResponseOptions.EnableReplace)
					{
						if (triggerMatches != null)
						{
							var values = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);

							                 // Negative lookbehind for not matching "\$" nor "$$".
							                           // Replacement '$'.
							                              // Replacement tag ID.
							var pattern = @"(?<![\\$])\$(\d+)";
							var evaluator = new AutoResponseReplaceEvaluator(values);
							textLine = Regex.Replace(textLine, pattern, evaluator.Evaluate);
						}
					}

					SendCommand(new Command(textLine)); // No explicit default radix available (yet).
					break;
				}

				default:
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + response + "' is an automatic response that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
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

		#region Thread
		//------------------------------------------------------------------------------------------
		// Thread
		//------------------------------------------------------------------------------------------

		private void CreateAndStartAutoResponseThread()
		{
			lock (this.autoResponseThreadSyncObj)
			{
				DebugThreadState("AutoResponseThread() gets created...");

				if (this.autoResponseThread == null)
				{
					this.autoResponseThreadRunFlag = true;
					this.autoResponseThreadEvent = new AutoResetEvent(false);
					this.autoResponseThread = new Thread(new ThreadStart(AutoResponseThread));
					this.autoResponseThread.Name = "Terminal [" + Guid + "] Auto Response Thread";
					this.autoResponseThread.Start();

					DebugThreadState("...successfully created.");
				}
			#if (DEBUG)
				else
				{
					DebugThreadState("...failed as it already exists.");
				}
			#endif
			}
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopAutoResponseThread()
		{
			lock (this.autoResponseThreadSyncObj)
			{
				if (this.autoResponseThread != null)
				{
					DebugThreadState("AutoResponseThread() gets stopped...");

					this.autoResponseThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.autoResponseThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.autoResponseThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalAutoResponseThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.autoResponseThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreadState("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreadState("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreadState("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.autoResponseThread = null;
				}
			#if (DEBUG)
				else // (this.autoResponseThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.autoResponseThreadEvent != null)
				{
					try     { this.autoResponseThreadEvent.Close(); }
					finally { this.autoResponseThreadEvent = null; }
				}
			} // lock (autoResponseThreadSyncObj)
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalAutoResponseThreadSafely()
		{
			try
			{
				if (this.autoResponseThreadEvent != null)
					this.autoResponseThreadEvent.Set();
			}
			catch (ObjectDisposedException ex) { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }
			catch (NullReferenceException ex)  { DebugEx.WriteException(GetType(), ex, "Unsafe thread signaling caught"); }

			// Catch 'NullReferenceException' for the unlikely case that the event has just been
			// disposed after the if-check. This way, the event doesn't need to be locked (which
			// is a relatively time-consuming operation). Still keep the if-check for the normal
			// cases.
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
