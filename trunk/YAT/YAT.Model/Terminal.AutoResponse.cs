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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of trigger:
////#define DEBUG_TRIGGER

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Text.RegularExpressions;

using YAT.Model.Types;
//// "YAT.Model.Utilities" is explicitly used due to ambiguity of "MessageHelper".

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
		private Utilities.AutoTriggerHelper autoResponseTriggerHelper;
		private object autoResponseTriggerHelperSyncObj = new object();

		private Queue<Tuple<byte[], string, MatchCollection>> autoResponseQueue = new Queue<Tuple<byte[], string, MatchCollection>>();
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
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged_Promptly;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void CreateAutoResponse()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.
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
							byte[] triggerByteSequence;
							if (TryParseCommand(triggerCommand, out triggerByteSequence))
							{
								lock (this.autoResponseTriggerHelperSyncObj)
								{
									var createOrRecreate = false;

									if (this.autoResponseTriggerHelper == null)
										createOrRecreate = true;
									else if (!ArrayEx.ValuesEqual(this.autoResponseTriggerHelper.ByteSequence, triggerByteSequence))
										createOrRecreate = true;

									if (createOrRecreate)
										this.autoResponseTriggerHelper = new Utilities.AutoTriggerHelper(triggerByteSequence);

									// The helper must not be recreated if sequence has not changed, as recreation will reset the
									// sequence queues in any case, but a change to [Send Text] will also trigger the update here!
									// And subsequently sending some [Send Text] must only reset the queues if really needed!
								}
							}
							else
							{
								DeactivateAutoResponse();
								DisposeAutoResponseHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic response trigger! The trigger does not specify valid " + ApplicationEx.CommonName + " command text! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
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
								this.autoResponseTriggerHelper = new Utilities.AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoResponse.TriggerOptions.CaseSensitive, SettingsRoot.AutoResponse.TriggerOptions.WholeWord, triggerRegex);
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
		protected virtual void EvaluateAutoResponseFromElements(Domain.RepositoryType repositoryType, Domain.DisplayElementCollection elements)
		{
			List<Tuple<byte[], string, MatchCollection>> triggersDummy;
			EvaluateAutoResponseFromElements(repositoryType, elements, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void EvaluateAutoResponseFromElements(Domain.RepositoryType repositoryType, Domain.DisplayElementCollection elements, out List<Tuple<byte[], string, MatchCollection>> triggers)
		{
			triggers = new List<Tuple<byte[], string, MatchCollection>>(); // No preset needed, default behavior is good enough.

			foreach (var de in elements)
			{
				if (de.Direction == Domain.Direction.Rx) // Trigger by specification is only active on receive-path.
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
										if (this.autoResponseTriggerHelper.EnqueueAndMatchByteSequence(repositoryType, originByte))
										{                                                // Same spacing as "No match." further below.
											DebugAutoResponseTrigger("Evaluated => Match   !");

											this.autoResponseTriggerHelper.ResetByteSequence(repositoryType);
											de.Highlight = true;

											// Signal the trigger:                                   // Always use byte sequence for [Trigger] response, since always 'IsByteSequenceTriggered' when evaluated here.
											triggers.Add(new Tuple<byte[], string, MatchCollection>(this.autoResponseTriggerHelper.ByteSequence, null, null));
										}
										else
										{
											DebugAutoResponseTrigger("Evaluated => No match.");
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
		protected virtual void EvaluateAutoResponseFromLines(Domain.RepositoryType repositoryType, Domain.DisplayLineCollection lines)
		{
			List<Tuple<byte[], string, MatchCollection>> triggersDummy;
			EvaluateAutoResponseFromLines(repositoryType, lines, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic response.
		/// </summary>
		/// <remarks>
		/// Automatic responses always are non-reloadable.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void EvaluateAutoResponseFromLines(Domain.RepositoryType repositoryType, Domain.DisplayLineCollection lines, out List<Tuple<byte[], string, MatchCollection>> triggers)
		{
			if (SettingsRoot.AutoResponse.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoResponseFromElements(repositoryType, dl, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Tuple<byte[], string, MatchCollection>>(); // No preset needed, default behavior is good enough.

				foreach (var dl in lines)
				{
					if (dl.Direction != Domain.Direction.Tx) // Trigger by specification is only active on receive-path, no need to further evaluate Tx-only lines.
					{
						lock (this.autoResponseTriggerHelperSyncObj)
						{
							if (this.autoResponseTriggerHelper != null)
							{
								MatchCollection triggerMatches;    // (includeMetaData ? dl.Text : dl.ContentText) with FR #431.
								int triggerCount = this.autoResponseTriggerHelper.TextTriggerCount(dl.ContentText, out triggerMatches);
								if (triggerCount > 0)
								{
									dl.Highlight = true;

									// Signal the trigger(s):
									for (int i = 0; i < triggerCount; i++)                             // Always use text for [Trigger] response, since always 'IsTextTriggered' when evaluated here.
										triggers.Add(new Tuple<byte[], string, MatchCollection>(null, this.autoResponseTriggerHelper.TextOrRegexPattern, triggerMatches));
								}
							}
							else
							{
								break;     // Break the loop if response got disposed in the meantime.
							}              // Though unlikely, it may happen when deactivating response
						} // lock (helper) // while processing many lines, e.g. on reload.
					} // if (direction != Tx)
				} // foreach (line)
			} // IsTextTriggered
		}

		/// <summary>
		/// Enqueues the automatic responses for invocation on other than the receive thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Agree, but this is a helper method to simplify enqueuing triggers.")]
		protected virtual void EnqueueAutoResponses(List<Tuple<byte[], string, MatchCollection>> triggers)
		{
			foreach (var trigger in triggers)
				EnqueueAutoResponse(trigger.Item1, trigger.Item2, trigger.Item3);
		}

		/// <summary>
		/// Enqueues the automatic response for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoResponse(byte[] triggerSequence, string triggerText, MatchCollection triggerMatches)
		{
			lock (this.autoResponseQueue) // Lock is required because "Queue<T>" is not synchronized.
				this.autoResponseQueue.Enqueue(new Tuple<byte[], string, MatchCollection>(triggerSequence, triggerText, triggerMatches));

			SignalAutoResponseThreadSafely();
		}

		/// <summary>
		/// Asynchronously invoke the automatic responses on other than the receive thread.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="EnqueueAutoResponse(byte[], string, MatchCollection)"/> above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void AutoResponseThread()
		{
			DebugThreads("...AutoResponseThread() has started.");

			try
			{
				// Outer loop, runs when signaled as well as periodically checking the state:
				while (IsUndisposed && this.autoResponseThreadRunFlag) // Check disposal state first!
				{
					// A signal will only be received a while after initialization, thus waiting for
					// signal first. Also, placing this code first in the outer loop makes the logic
					// of the two loops more obvious.
					try
					{
						// WaitOne() would wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the state again.
						// The period can be quite long, as an event signal will immediately resume.
						if (!this.autoResponseThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue; // to periodically check state.
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in AutoResponseThread()!");
						break;
					}

					// Inner loop, runs as long as there are items in the queue:
					while (IsUndisposed && this.autoResponseThreadRunFlag && (this.autoResponseQueue.Count > 0)) // Check disposal state first!
					{                                                     // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue,
						// since it is likely that more triggers are to be enqueued.
						Thread.Sleep(TimeSpan.Zero); // "TimeSpan.Zero" = 100% CPU is OK as processing shall happen as fast as possible.

						Tuple<byte[], string, MatchCollection>[] pendingItems;
						lock (this.autoResponseQueue) // Lock is required because "Queue<T>" is not synchronized.
						{
							pendingItems = this.autoResponseQueue.ToArray();
							this.autoResponseQueue.Clear();
						}

						foreach (var item in pendingItems)
						{
							SendAutoResponse(item.Item1, item.Item2, item.Item3);
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

			DebugThreads("AutoResponseThread() has terminated.");
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence, string triggerText, MatchCollection triggerMatches)
		{
			int count = Interlocked.Increment(ref this.autoResponseCount); // Incrementing before invoking to have the effective count updated when sending.
			OnAutoResponseCountChanged_Promptly(new EventArgs<int>(count));

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
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + response + "' is an automatic response that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary>
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void SendAutoResponseTrigger(byte[] triggerSequence, string triggerText)
		{
			if (!ArrayEx.IsNullOrEmpty(triggerSequence))                     // If sequence is given, use that, no matter
				this.terminal.SendRaw(ToSequenceWithTxEol(triggerSequence)); // whether 'UseText' is enabled or not.
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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.autoResponseCount);
			}
		}

		/// <summary>
		/// Resets the automatic response count.
		/// </summary>
		public virtual void ResetAutoResponseCount()
		{
			AssertUndisposed();

			int count = Interlocked.Exchange(ref this.autoResponseCount, 0);
			OnAutoResponseCountChanged_Promptly(new EventArgs<int>(count));
		}

		/// <summary>
		/// Deactivates the automatic response.
		/// </summary>
		public virtual void DeactivateAutoResponse()
		{
			AssertUndisposed();

			ResetAutoResponseCount(); // Must be done before raising the settings 'Changed' event because the 'terminal_AutoAction/ResponseCountChanged_Promptly' events are not used by the 'View.Forms.Terminal'.
			SettingsRoot.AutoResponse.Deactivate();
		}

		#endregion

		#region Thread
		//------------------------------------------------------------------------------------------
		// Thread
		//------------------------------------------------------------------------------------------

		private void StartAutoResponseThread()
		{
			lock (this.autoResponseThreadSyncObj)
			{
				if (this.autoResponseThread == null)
				{
					DebugThreads("AutoResponseThread() gets started...");

					this.autoResponseThreadRunFlag = true;
					this.autoResponseThreadEvent = new AutoResetEvent(false);
					this.autoResponseThread = new Thread(new ThreadStart(AutoResponseThread));
					this.autoResponseThread.Name = "Terminal [" + Guid + "] Auto Response Thread";
					this.autoResponseThread.Start();
				}
			#if (DEBUG)
				else
				{
					DebugThreads("AutoResponseThread() does not get started as it already exists.");
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
					DebugThreads("AutoResponseThread() gets stopped...");

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
								DebugThreads("...failed! Aborting...");
								DebugThreads("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.autoResponseThread.Abort(); // This is only the fall-back in case joining fails for too long.
								break;
							}

							DebugThreads("...trying to join at " + accumulatedTimeout + " ms...");
						}

						if (!isAborting)
							DebugThreads("...successfully stopped.");
					}
					catch (ThreadStateException)
					{
						// Ignore thread state exceptions such as "Thread has not been started" and
						// "Thread cannot be aborted" as it just needs to be ensured that the thread
						// has or will be terminated for sure.

						DebugThreads("...failed too but will be exectued as soon as the calling thread gets suspended again.");
					}

					this.autoResponseThread = null;
				}
			#if (DEBUG)
				else // (this.autoResponseThread == null)
				{
					DebugThreads("...not necessary as it doesn't exist anymore.");
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
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		protected virtual void OnAutoResponseCountChanged_Promptly(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged_Promptly, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_TRIGGER")]
		private void DebugAutoResponseTrigger(string leadMessage)
		{
			DebugMessage("{0} {1}", leadMessage, this.autoResponseTriggerHelper.ToDiagnosticsString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
