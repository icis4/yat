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
// YAT Version 2.2.0 Development
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Text.RegularExpressions;

using YAT.Model.Types;
//// 'YAT.Model.Utilities' is explicitly used due to ambiguity of 'MessageHelper'.

#endregion

namespace YAT.Model
{
	/// <remarks>
	/// This partial class implements the automatic action part of <see cref="Terminal"/>.
	/// </remarks>
	public partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int autoActionCount;
		private Utilities.AutoTriggerHelper autoActionTriggerHelper;
		private object autoActionTriggerHelperSyncObj = new object();

		private bool autoActionClearRepositoriesOnSubsequentRxIsArmed; // = false;
		private DateTime autoActionClearRepositoriesTriggerTimeStamp; // = DateTime.MinValue;
		private string autoActionClearRepositoriesTriggerText; // = null;
		private MatchCollection autoActionClearRepositoriesTriggerMatches; // = null;
		private CountsRatesTuple autoActionClearRepositoriesDataStatus; // = { 0, 0, 0, 0 };
		private object autoActionClearRepositoriesSyncObj = new object();

		private bool autoActionCloseOrExitHasBeenTriggered; // = false;
		private object autoActionCloseOrExitSyncObj = new object();

		private Queue<Tuple<AutoAction, DateTime, string, MatchCollection, CountsRatesTuple>> autoActionQueue = new Queue<Tuple<AutoAction, DateTime, string, MatchCollection, CountsRatesTuple>>();
		private bool autoActionThreadRunFlag;
		private AutoResetEvent autoActionThreadEvent;
		private Thread autoActionThread;
		private object autoActionThreadSyncObj = new object();

		/// <remarks>Public getter for simplicity (update of corresponding view).</remarks>
		/// <remarks><see cref="AutoActionPlotModelSyncObj"/> must be locked when accessing this property!</remarks>
		public AutoActionPlotModel AutoActionPlotModel { get; protected set; }

		/// <remarks>Public getter for simplicity (update of corresponding view).</remarks>
		/// <remarks>Required for locking when accessing <see cref="AutoActionPlotModel"/>!</remarks>
		public object AutoActionPlotModelSyncObj { get; protected set; } = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		public event EventHandler AutoActionPlotRequest_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		public event EventHandler<EventArgs<int>> AutoActionCountChanged_Promptly;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void CreateAutoAction()
		{
			UpdateAutoAction(); // Simply forward to general Update() method.
		}

		/// <summary>
		/// Updates the automatic action helper.
		/// </summary>
		protected virtual void UpdateAutoAction()
		{
			if (SettingsRoot.AutoAction.IsActive)
			{
				if (SettingsRoot.AutoAction.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					Command triggerCommand;
					string  triggerTextOrRegexPattern;
					Regex   triggerRegex;

					if (SettingsRoot.TryGetActiveAutoActionTrigger(out triggerCommand, out triggerTextOrRegexPattern, out triggerRegex))
					{
						if (SettingsRoot.AutoAction.IsByteSequenceTriggered)
						{
							byte[] triggerSequence;
							if (TryParseCommand(triggerCommand, out triggerSequence))
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new Utilities.AutoTriggerHelper(triggerSequence);
							}
							else
							{
								DeactivateAutoAction();
								DisposeAutoActionHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic action trigger! The trigger does not specify valid " + ApplicationEx.CommonName + " command text! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
									"To enable again, re-configure the automatic action.",
									"Automatic Action Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning
								);
							}
						}
						else // IsTextTriggered
						{
							lock (this.autoActionTriggerHelperSyncObj)
								this.autoActionTriggerHelper = new Utilities.AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoAction.TriggerOptions.CaseSensitive, SettingsRoot.AutoAction.TriggerOptions.WholeWord, triggerRegex);
						}
					}
					else if (this.autoIsReady) // See remarks of 'Terminal.NotifyAutoIsReady()' for background.
					{
						DeactivateAutoAction();
						DisposeAutoActionHelper();

						OnMessageInputRequest
						(
							"Failed to retrieve the automatic action trigger! The trigger is not available! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic action.",
							"Automatic Action Error",
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
					DisposeAutoActionHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoActionHelper();
			}
		}

		private void DisposeAutoActionHelper()
		{
			lock (this.autoActionTriggerHelperSyncObj)
				this.autoActionTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Evaluates the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements, CountsRatesTuple dataStatus, bool shallHighlight)
		{
			List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> triggersDummy;
			EvaluateAutoActionFromElements(elements, dataStatus, shallHighlight, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements, CountsRatesTuple dataStatus, bool shallHighlight, out List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> triggers)
		{
			triggers = new List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>>(); // No preset needed, the default behavior is good enough.

			EvaluateAndEnqueueAutoActionClearRepositoriesOnSubsequentRx();

			foreach (var de in elements)
			{
				lock (this.autoActionTriggerHelperSyncObj)
				{
					if (this.autoActionTriggerHelper != null)
					{
						if (de.Origin != null) // Foreach element where origin exists.
						{
							foreach (var origin in de.Origin)
							{
								foreach (var originByte in origin.Value1)
								{
									if (this.autoActionTriggerHelper.EnqueueAndMatchTrigger(originByte))
									{
										this.autoActionTriggerHelper.Reset();
										de.Highlight = shallHighlight;

										// Signal the trigger:
										triggers.Add(new Tuple<DateTime, string, MatchCollection, CountsRatesTuple>(de.TimeStamp, elements.Text, null, dataStatus));

										// Note that 'elements.Text' is not perfect, as it could only contain parts of
										// the trigger. However, using the trigger sequence formatted with...
										// this.terminal.Format(triggerSequence, Domain.IODirection.Rx)
										// ...in RequestAutoActionMessage() isn't perfect either, as it will *never*
										// contain more than the trigger. Thus preferring 'Elements.Text'.
									}
								}
							}
						}
					}
					else
					{
						break;     // Break the loop if action got disposed in the meantime.
					}              // Though unlikely, it may happen when deactivating action
				} // lock (helper) // while receiving a very large chunk.
			} // foreach (element)
		}

		/// <summary>
		/// Evaluates the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines, CountsRatesTuple dataStatus, bool shallHighlight)
		{
			List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> triggersDummy;
			EvaluateAutoActionOtherThanFilterOrSuppressFromLines(lines, dataStatus, shallHighlight, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void EvaluateAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines, CountsRatesTuple dataStatus, bool shallHighlight, out List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> triggers)
		{
			EvaluateAndEnqueueAutoActionClearRepositoriesOnSubsequentRx();

			if (SettingsRoot.AutoAction.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoActionFromElements(dl, dataStatus, shallHighlight, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>>(); // No preset needed, the default behavior is good enough.

				foreach (var dl in lines)
				{
					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							MatchCollection triggerMatches;
							int triggerCount = this.autoActionTriggerHelper.TextTriggerCount(dl.Text, out triggerMatches);
							if (triggerCount > 0)
							{
								this.autoActionTriggerHelper.Reset(); // Invoke shall happen as short as possible after detection.
								dl.Highlight = shallHighlight;

								// Signal the trigger(s):
								for (int i = 0; i < triggerCount; i++)
									triggers.Add(new Tuple<DateTime, string, MatchCollection, CountsRatesTuple>(dl.TimeStamp, dl.Text, triggerMatches, dataStatus));
							}
						}
						else
						{
							break;     // Break the loop if action got disposed in the meantime.
						}              // Though unlikely, it may happen when deactivating action
					} // lock (helper) // while processing many lines, e.g. on reload.
				} // foreach (line)
			} // IsTextTriggered
		}

		/// <summary>
		/// Processes <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>
		/// are never processed from elements.
		/// </remarks>
		/// <remarks>
		/// <paramref name="lines"/> will be recreated. This is preferred over suppressing hidden
		/// lines at the monitors because other terminal clients would also have to suppress them.
		/// Also, suppressing here reduces the amount of data being forwarded to the monitors.
		/// </remarks>
		protected virtual void ProcessAutoActionFilterAndSuppressFromLines(Domain.DisplayLineCollection lines)
		{
			var linesInitially = lines.Clone(); // Needed because collection will be recreated.
			lines.Clear();

			foreach (var dl in linesInitially)
			{
				if (dl.Direction != Domain.Direction.Tx) // By specification only active on receive-path.
				{
					bool isTriggered = false;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							if (SettingsRoot.AutoAction.IsByteSequenceTriggered)
							{
								foreach (var de in dl)
								{
									if (de.Origin != null) // Foreach element where origin exists.
									{
										foreach (var origin in de.Origin)
										{
											foreach (var originByte in origin.Value1)
											{
												if (this.autoActionTriggerHelper.EnqueueAndMatchTrigger(originByte))
												{
													this.autoActionTriggerHelper.Reset();
													isTriggered = true;
												}
											}
										}
									}
								}
							}
							else // IsTextTriggered
							{
								isTriggered = this.autoActionTriggerHelper.TextTriggerSuccess(dl.Text);
							}
						}
						else
						{
							break;     // Break the loop if action got disposed in the meantime.
						}              // Though unlikely, it may happen when deactivating action
					} // lock (helper) // while processing many lines, e.g. on reload.

					switch ((AutoAction)SettingsRoot.AutoAction.Action)
					{
						case AutoAction.Filter:   if ( isTriggered) { lines.Add(dl); } break;
						case AutoAction.Suppress: if (!isTriggered) { lines.Add(dl); } break;

						default: throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "Only 'Filter' and 'Suppress' are evaluated here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
					}
				} // if (direction != Tx)
			} // foreach (linesInitially)
		}

		/// <summary>
		/// Evaluates <see cref="AutoAction.ClearRepositoriesOnSubsequentRx"/> and enqueues it for invocation.
		/// </summary>
		protected virtual void EvaluateAndEnqueueAutoActionClearRepositoriesOnSubsequentRx()
		{
			lock (this.autoActionClearRepositoriesSyncObj)
			{
				if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
				{
					DateTime triggerTimeStamp;
					string triggerText;
					MatchCollection triggerMatches;
					CountsRatesTuple dataStatus;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						triggerTimeStamp = this.autoActionClearRepositoriesTriggerTimeStamp;
						triggerText      = this.autoActionClearRepositoriesTriggerText;
						triggerMatches   = this.autoActionClearRepositoriesTriggerMatches;
						dataStatus       = this.autoActionClearRepositoriesDataStatus;

						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;
						this.autoActionClearRepositoriesTriggerTimeStamp      = DateTime.MinValue;
						this.autoActionClearRepositoriesTriggerText           = null;
						this.autoActionClearRepositoriesTriggerMatches        = null;
						this.autoActionClearRepositoriesDataStatus            = new CountsRatesTuple();
					}
					                        //// ClearRepositories is to be enqueued, not ClearRepositoriesOnSubsequentRx!
					EnqueueAutoAction(AutoAction.ClearRepositories, triggerTimeStamp, triggerText, triggerMatches, dataStatus);
				}
			}
		}

		/// <summary>
		/// Enqueues the automatic actions for invocation on other than the receive thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Agree, but this is a helper method to simplify enqueuing triggers.")]
		protected virtual void EnqueueAutoActions(List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> triggers)
		{
			foreach (var trigger in triggers)
				EnqueueAutoAction(trigger.Item1, trigger.Item2, trigger.Item3, trigger.Item4);
		}

		/// <summary>
		/// Enqueues the automatic action for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoAction(DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches, CountsRatesTuple dataStatus)
		{
			EnqueueAutoAction(SettingsRoot.AutoAction.Action, triggerTimeStamp, triggerText, triggerMatches, dataStatus);
		}

		/// <summary>
		/// Enqueues the automatic action for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoAction(AutoAction action, DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches, CountsRatesTuple dataStatus)
		{
			lock (this.autoActionQueue) // Lock is required because Queue<T> is not synchronized.
				this.autoActionQueue.Enqueue(new Tuple<AutoAction, DateTime, string, MatchCollection, CountsRatesTuple>(action, triggerTimeStamp, triggerText, triggerMatches, dataStatus));

			SignalAutoActionThreadSafely();
		}

		/// <summary>
		/// Asynchronously invoke the automatic actions on other than the receive thread.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="EnqueueAutoAction(AutoAction, DateTime, string, MatchCollection, CountsRatesTuple)"/> above.
		/// </remarks>
		[SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", Justification = "Project does target .NET 4 but FxCop cannot handle that, project must be upgraded to Visual Studio Code Analysis (FR #231).")]
		private void AutoActionThread()
		{
			DebugThreads("...AutoActionThread() has started.");

			try
			{
				// Outer loop, runs when signaled as well as periodically checking the state:
				while (IsUndisposed && this.autoActionThreadRunFlag) // Check disposal state first!
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
						if (!this.autoActionThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue; // to periodically check state.
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in AutoActionThread()!");
						break;
					}

					// Inner loop, runs as long as there are items in the queue:
					while (IsUndisposed && this.autoActionThreadRunFlag && (this.autoActionQueue.Count > 0)) // Check disposal state first!
					{                                                   // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue,
						// since it is likely that more triggers are to be enqueued.
						Thread.Sleep(TimeSpan.Zero); // 'TimeSpan.Zero' = 100% CPU is OK as processing shall happen as fast as possible.

						Tuple<AutoAction, DateTime, string, MatchCollection, CountsRatesTuple>[] pendingItems;
						lock (this.autoActionQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.autoActionQueue.ToArray();
							this.autoActionQueue.Clear();
						}

						foreach (var item in pendingItems)
						{
							PerformAutoAction(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
						}
					} // Inner loop
				} // Outer loop
			}
			catch (ThreadAbortException ex)
			{
				DebugEx.WriteException(GetType(), ex, "AutoActionThread() has been aborted! Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Should only happen when failing to 'friendly' join the thread on stopping!
				// Don't try to set and notify a state change, or even restart the terminal!

				// But reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}

			DebugThreads("AutoActionThread() has terminated.");
		}

		/// <summary>
		/// Performs the automatic action.
		/// </summary>
		protected virtual void PerformAutoAction(AutoAction action, DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches, CountsRatesTuple dataStatus)
		{
			int count = Interlocked.Increment(ref this.autoActionCount); // Incrementing before invoking to have the effective count updated during action.
			OnAutoActionCountChanged_Promptly(new EventArgs<int>(count));

			// Attention:
			// Same switch/case exists in HasActionToInvoke() below.
			// Changes here must be applied there too.

			switch (action)
			{
				case AutoAction.None:
					// Nothing to do.
					break;

				case AutoAction.Highlight:
				case AutoAction.Filter:
				case AutoAction.Suppress:
					// No additional action.
					break;

				case AutoAction.Beep:                SystemSounds.Beep.Play();                                                    break;
				case AutoAction.ShowMessageBox:      RequestAutoActionMessage(triggerTimeStamp,  triggerText, count);             break;

				case AutoAction.PlotByteCountRate:   RequestAutoActionPlot(action, triggerTimeStamp, null,           dataStatus); break;
				case AutoAction.PlotLineCountRate:   RequestAutoActionPlot(action, triggerTimeStamp, null,           dataStatus); break;
				case AutoAction.LineChartIndex:      RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;
				case AutoAction.LineChartTime:       RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;
				case AutoAction.LineChartTimeStamp:  RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;
				case AutoAction.ScatterPlot:         RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;
				case AutoAction.HistogramHorizontal: RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;
				case AutoAction.HistogramVertical:   RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches, dataStatus); break;

				case AutoAction.ClearRepositories:   ClearRepositories();                                                         break;

				case AutoAction.ClearRepositoriesOnSubsequentRx:
				{
					lock (this.autoActionClearRepositoriesSyncObj)
					{
						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true;
						this.autoActionClearRepositoriesTriggerTimeStamp      = triggerTimeStamp;
						this.autoActionClearRepositoriesTriggerText           = triggerText;
						this.autoActionClearRepositoriesTriggerMatches        = triggerMatches;
						this.autoActionClearRepositoriesDataStatus            = dataStatus;
					}
					break;
				}

				case AutoAction.ResetCountAndRate: ResetCountAndRate(); break;
				case AutoAction.SwitchLogOn:       SwitchLogOn();       break;
				case AutoAction.SwitchLogOff:      SwitchLogOff();      break;
				case AutoAction.ToggleLogOnOrOff:  ToggleLogOnOrOff();  break;
				case AutoAction.Stop:              Stop();              break;

				case AutoAction.CloseTerminal:
				{
					lock (this.autoActionCloseOrExitSyncObj)
					{
						if (!this.autoActionCloseOrExitHasBeenTriggered)
						{
							this.autoActionCloseOrExitHasBeenTriggered = true;
							Close();
						}
					}
					break;
				}

				case AutoAction.ExitApplication:
				{
					lock (this.autoActionCloseOrExitSyncObj)
					{
						if (!this.autoActionCloseOrExitHasBeenTriggered)
						{
							this.autoActionCloseOrExitHasBeenTriggered = true;
							OnExitRequest(EventArgs.Empty);
						}
					}
					break;
				}

				default:
				{
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "'" + action + "' is an automatic action that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary>
		/// Determines whether the given action has a true action to invoke.
		/// </summary>
		protected virtual bool HasActionToInvoke(AutoAction action)
		{
			// Attention:
			// Same switch/case exists in PerformAutoAction() above.
			// Changes here must be applied there too.

			switch (action)
			{
				case AutoAction.None:
					return (false);

				case AutoAction.Highlight:
				case AutoAction.Filter:
				case AutoAction.Suppress:
					return (false);

				case AutoAction.Beep:
				case AutoAction.ShowMessageBox:
				case AutoAction.PlotByteCountRate:
				case AutoAction.PlotLineCountRate:
				case AutoAction.LineChartIndex:
				case AutoAction.LineChartTime:
				case AutoAction.LineChartTimeStamp:
				case AutoAction.ScatterPlot:
				case AutoAction.HistogramHorizontal:
				case AutoAction.HistogramVertical:
				case AutoAction.ClearRepositories:
				case AutoAction.ClearRepositoriesOnSubsequentRx:
				case AutoAction.ResetCountAndRate:
				case AutoAction.SwitchLogOn:
				case AutoAction.SwitchLogOff:
				case AutoAction.ToggleLogOnOrOff:
				case AutoAction.Stop:
				case AutoAction.CloseTerminal:
				case AutoAction.ExitApplication:
					return (true);

				default:
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "'" + action + "' is an automatic action that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Notifies the user about the action.
		/// </summary>
		protected virtual void RequestAutoActionMessage(DateTime triggerTimeStamp, string triggerText, int totalCount)
		{
			var sb = new StringBuilder();

			sb.Append(@"Message has been triggered by """);
			sb.Append(triggerText);
			sb.Append(@""" at ");
			sb.Append(this.terminal.Format(triggerTimeStamp));
			sb.AppendLine(".");

			sb.Append("Message has been triggered the ");
			sb.Append(totalCount);
			sb.Append(Int32Ex.ToEnglishSuffix(totalCount));
			sb.Append(" time.");

			OnMessageInputRequest
			(
				sb.ToString(),
				IndicatedName + " Automatic Action",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		/// <summary>
		/// Requests the desired chart/plot.
		/// </summary>
		protected virtual void RequestAutoActionPlot(AutoAction plotAction, DateTime triggerTimeStamp, MatchCollection triggerMatches, CountsRatesTuple dataStatus)
		{
			AutoActionPlotItem pi;
			string messageOnFailure;
			if (TryCreateAutoActionPlotItem(plotAction, triggerTimeStamp, triggerMatches, dataStatus, out pi, out messageOnFailure))
			{
				lock (AutoActionPlotModelSyncObj)
				{
					if (AutoActionPlotModel == null)
						AutoActionPlotModel = new AutoActionPlotModel();

					var txColor = SettingsRoot.Format.TxDataFormat.Color;
					var rxColor = SettingsRoot.Format.RxDataFormat.Color;
					AutoActionPlotModel.AddItem(pi, txColor, rxColor);
				}

				OnAutoActionPlotRequest_Promptly(EventArgs.Empty);
			}
			else
			{
				this.terminal.InlineErrorMessage(Domain.Direction.Rx, messageOnFailure, true);
			}
		}

		/// <summary>
		/// Requests the desired chart/plot.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool TryCreateAutoActionPlotItem(AutoAction plotAction, DateTime triggerTimeStamp, MatchCollection triggerMatches, CountsRatesTuple dataStatus, out AutoActionPlotItem pi, out string messageOnFailure)
		{
			switch (plotAction)
			{
				case AutoAction.PlotByteCountRate:             CreateCountRatePlotItem(  plotAction, triggerTimeStamp,                 dataStatus, out pi);    messageOnFailure = null; return (true);
				case AutoAction.PlotLineCountRate:             CreateCountRatePlotItem(  plotAction, triggerTimeStamp,                 dataStatus, out pi);    messageOnFailure = null; return (true);
				case AutoAction.LineChartIndex:                CreateYPlotItem(          plotAction,                   triggerMatches,             out pi);    messageOnFailure = null; return (true);
				case AutoAction.LineChartTime:      return (TryCreateTimeXYPlotItem(     plotAction,                   triggerMatches,             out pi, out messageOnFailure));
				case AutoAction.LineChartTimeStamp:            CreateTimeStampXYPlotItem(plotAction, triggerTimeStamp, triggerMatches,             out pi);    messageOnFailure = null; return (true);
				case AutoAction.ScatterPlot:                   CreateXYPlotItem(         plotAction,                   triggerMatches,             out pi);    messageOnFailure = null; return (true);
				case AutoAction.HistogramHorizontal:           CreateYPlotItem(          plotAction,                   triggerMatches,             out pi);    messageOnFailure = null; return (true);
				case AutoAction.HistogramVertical:             CreateYPlotItem(          plotAction,                   triggerMatches,             out pi);    messageOnFailure = null; return (true);

				default: throw (new ArgumentOutOfRangeException("plot", plotAction, MKY.MessageHelper.InvalidExecutionPreamble + "'" + plotAction.ToString() + "' is a plot type that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void CreateCountRatePlotItem(AutoAction plotAction, DateTime triggerTimeStamp, CountsRatesTuple dataStatus, out AutoActionPlotItem pi)
		{
			string label;
			int txCount, txRate, rxCount, rxRate;
			switch (plotAction)
			{
				case (AutoAction.PlotByteCountRate): label = "bytes"; txCount = dataStatus.Counts.TxBytes; txRate = dataStatus.Rates.TxBytes; rxCount = dataStatus.Counts.RxBytes; rxRate = dataStatus.Rates.RxBytes; break;
				case (AutoAction.PlotLineCountRate): label = "lines"; txCount = dataStatus.Counts.TxLines; txRate = dataStatus.Rates.TxLines; rxCount = dataStatus.Counts.RxLines; rxRate = dataStatus.Rates.RxLines; break;

				default: throw (new ArgumentOutOfRangeException("plotAction", plotAction, MKY.MessageHelper.InvalidExecutionPreamble + "'" + plotAction + "' is a plot action that is not (yet) supported for counts/rates!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
			}

			var xValue = new Tuple<string, double>("Time Stamp", triggerTimeStamp.ToOADate());
			var yValues = new List<Tuple<string, double>>(4); // Preset the required capacity to improve memory management.
			yValues.Add(new Tuple<string, double>("Tx number of " + label,       txCount));
			yValues.Add(new Tuple<string, double>("Tx " + label + " per second", txRate));
			yValues.Add(new Tuple<string, double>("Rx number of " + label,       rxCount));
			yValues.Add(new Tuple<string, double>("Rx " + label + " per second", rxRate));
			pi = new AutoActionPlotItem(plotAction, xValue, yValues.ToArray());
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void CreateYPlotItem(AutoAction plotAction, MatchCollection triggerMatches, out AutoActionPlotItem pi)
		{
			var captures = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);
			var yValues = ConvertToPlotValues(captures);
			pi = new AutoActionPlotItem(plotAction, null, yValues);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void CreateXYPlotItem(AutoAction plotAction, MatchCollection triggerMatches, out AutoActionPlotItem pi)
		{
			var captures = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);
			var values = ConvertToPlotValues(captures);
			var xValue = values[0];
			var yLength = (values.Length - 1);
			var yValues = new Tuple<string, double>[yLength];
			Array.Copy(values, 1, yValues, 0, yLength);
			pi = new AutoActionPlotItem(plotAction, xValue, yValues);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool TryCreateTimeXYPlotItem(AutoAction plotAction, MatchCollection triggerMatches, out AutoActionPlotItem pi, out string messageOnFailure)
		{
			var captures = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);
			Tuple<string, double> xTimeValue;
			if (!TryConvertToPlotTime(captures[0], out xTimeValue, out messageOnFailure)) {
				pi = null;
				return (false);
			}
			var values = ConvertToPlotValues(captures);
			var yLength = (values.Length - 1);
			var yValues = new Tuple<string, double>[yLength];
			Array.Copy(values, 1, yValues, 0, yLength);
			pi = new AutoActionPlotItem(plotAction, xTimeValue, yValues);
			messageOnFailure = null;
			return (true);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void CreateTimeStampXYPlotItem(AutoAction plotAction, DateTime triggerTimeStamp, MatchCollection triggerMatches, out AutoActionPlotItem pi)
		{
			var xValue = new Tuple<string, double>("Time Stamp", triggerTimeStamp.ToOADate());
			var captures = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);
			var yValues = ConvertToPlotValues(captures);
			pi = new AutoActionPlotItem(plotAction, xValue, yValues);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool TryConvertToPlotTime(string capture, out Tuple<string, double> timeValue, out string messageOnFailure)
		{
			// Try DateTime:
			{
				DateTime result;
				if (DateTime.TryParse(capture, out result)) // Always name series, even if legend is disabled
				{                                           // currently, as user can enable it at any time.
					timeValue = new Tuple<string, double>("Time Captures as OLE Automation Date", result.ToOADate());
					messageOnFailure = null;
					return (true);
				}
			}

			// Try double:
			{
				double result;
				if (double.TryParse(capture, out result))
				{
					timeValue = new Tuple<string, double>("Numeric Captures as OLE Automation Date", result);
					messageOnFailure = null;
					return (true);
				}
			}

			timeValue = null;
			messageOnFailure = string.Format(CultureInfo.CurrentCulture, "The first capture {0} could not be converted into a date/time value!", capture);
			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		protected virtual Tuple<string, double>[] ConvertToPlotValues(string[] captures)
		{
			var yValues = new List<Tuple<string, double>>(); // No preset needed, the default behavior is good enough.

			foreach (var s in captures)
			{
				// Try double:
				{
					double result;
					if (double.TryParse(s, out result))       // Always name series, even if legend is disabled
					{                                         // currently, as user can enable it at any time.
						yValues.Add(new Tuple<string, double>("Numeric Captures", result));
						continue;
					}
				}

				// Try DateTime:
				{
					DateTime result;
					if (DateTime.TryParse(s, out result))
					{
						yValues.Add(new Tuple<string, double>("Time Captures as OLE Automation Date", result.ToOADate()));
						continue;
					}
				}

				// Use string:
				{
					long hash = 0;
					foreach (char c in s)
						hash += c;

					yValues.Add(new Tuple<string, double>("Sum of Character Codes", hash));
				}
			}

			return (yValues.ToArray());
		}

		/// <summary>
		/// Gets the automatic action count.
		/// </summary>
		public virtual int AutoActionCount
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.autoActionCount);
			}
		}

		/// <summary>
		/// Resets the automatic action count.
		/// </summary>
		public virtual void ResetAutoActionCount()
		{
			AssertUndisposed();

			int count = Interlocked.Exchange(ref this.autoActionCount, 0);
			OnAutoActionCountChanged_Promptly(new EventArgs<int>(count));
		}

		/// <summary>
		/// Deactivates the automatic action.
		/// </summary>
		public virtual void DeactivateAutoAction()
		{
			AssertUndisposed();

			SettingsRoot.AutoAction.Deactivate();
			ResetAutoActionCount();
		}

		#endregion

		#region Thread
		//------------------------------------------------------------------------------------------
		// Thread
		//------------------------------------------------------------------------------------------

		private void StartAutoActionThread()
		{
			lock (this.autoActionThreadSyncObj)
			{
				if (this.autoActionThread == null)
				{
					DebugThreads("AutoActionThread() gets started...");

					this.autoActionThreadRunFlag = true;
					this.autoActionThreadEvent = new AutoResetEvent(false);
					this.autoActionThread = new Thread(new ThreadStart(AutoActionThread));
					this.autoActionThread.Name = "Terminal [" + Guid + "] Auto Action Thread";
					this.autoActionThread.Start();
				}
			#if (DEBUG)
				else
				{
					DebugThreads("AutoActionThread() does not get started as it already exists.");
				}
			#endif
			}
		}

		/// <remarks>
		/// Using 'Stop' instead of 'Terminate' to emphasize graceful termination, i.e. trying
		/// to join first, then abort if not successfully joined.
		/// </remarks>
		private void StopAutoActionThread()
		{
			lock (this.autoActionThreadSyncObj)
			{
				if (this.autoActionThread != null)
				{
					DebugThreads("AutoActionThread() gets stopped...");

					this.autoActionThreadRunFlag = false;

					// Ensure that send thread has stopped after the stop request:
					try
					{
						Debug.Assert(this.autoActionThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId, "Attention: Tried to join itself!");

						bool isAborting = false;
						int accumulatedTimeout = 0;
						int interval = 0; // Use a relatively short random interval to trigger the thread:
						while (!this.autoActionThread.Join(interval = staticRandom.Next(5, 20)))
						{
							SignalAutoActionThreadSafely();

							accumulatedTimeout += interval;
							if (accumulatedTimeout >= ThreadWaitTimeout)
							{
								DebugThreads("...failed! Aborting...");
								DebugThreads("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.autoActionThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.autoActionThread = null;
				}
			#if (DEBUG)
				else // (this.autoActionThread == null)
				{
					DebugThreads("...not necessary as it doesn't exist anymore.");
				}
			#endif

				if (this.autoActionThreadEvent != null)
				{
					try     { this.autoActionThreadEvent.Close(); }
					finally { this.autoActionThreadEvent = null; }
				}
			} // lock (autoActionThreadSyncObj)
		}

		/// <remarks>
		/// Especially useful during potentially dangerous creation and disposal sequence.
		/// </remarks>
		private void SignalAutoActionThreadSafely()
		{
			try
			{
				if (this.autoActionThreadEvent != null)
					this.autoActionThreadEvent.Set();
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
		protected virtual void OnAutoActionPlotRequest_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync<EventArgs>(AutoActionPlotRequest_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		protected virtual void OnAutoActionCountChanged_Promptly(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged_Promptly, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
