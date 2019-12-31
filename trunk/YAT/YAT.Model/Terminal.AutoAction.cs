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
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Text;
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
	/// This partial class implements the automatic action part of <see cref="Terminal"/>.
	/// </remarks>
	public partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int autoActionCount;
		private AutoTriggerHelper autoActionTriggerHelper;
		private object autoActionTriggerHelperSyncObj = new object();

		private bool autoActionClearRepositoriesOnSubsequentRxIsArmed; // = false;
		private DateTime autoActionClearRepositoriesTriggerTimeStamp; // = DateTime.MinValue;
		private string autoActionClearRepositoriesTriggerText; // = null;
		private MatchCollection autoActionClearRepositoriesTriggerMatches; // = null;
		private object autoActionClearRepositoriesSyncObj = new object();

		private bool autoActionCloseOrExitHasBeenTriggered; // = false;
		private object autoActionCloseOrExitSyncObj = new object();

		private Queue<Quadruple<AutoAction, DateTime, string, MatchCollection>> autoActionQueue = new Queue<Quadruple<AutoAction, DateTime, string, MatchCollection>>();
		private bool autoActionThreadRunFlag;
		private AutoResetEvent autoActionThreadEvent;
		private Thread autoActionThread;
		private object autoActionThreadSyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<AutoActionPlotEventArgs> AutoActionPlotRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void CreateAutoAction()
		{
			UpdateAutoAction(); // Simply forward to general Update() method.

			CreateAndStartAutoActionThread();
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
							if (TryParseCommandToSequence(triggerCommand, out triggerSequence))
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new AutoTriggerHelper(triggerSequence);
							}
							else
							{
								DeactivateAutoAction();
								DisposeAutoActionHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic action trigger! The trigger does not specify valid YAT command text! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
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
								this.autoActionTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoAction.TriggerOptions.CaseSensitive, SettingsRoot.AutoAction.TriggerOptions.WholeWord, triggerRegex);
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
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements)
		{
			List<Triple<DateTime, string, MatchCollection>> triggersDummy;
			EvaluateAutoActionFromElements(elements, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements, out List<Triple<DateTime, string, MatchCollection>> triggers)
		{
			triggers = new List<Triple<DateTime, string, MatchCollection>>(); // No preset needed, the default behavior is good enough.

			EvaluateAndEnqueueAutoActionClearRepositoriesOnSubsequentRx();

			foreach (var de in elements)
			{
				if (de.Direction == Domain.Direction.Rx) // By specification only active on receive-path.
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
											de.Highlight = true;

											// Signal the trigger:
											triggers.Add(new Triple<DateTime, string, MatchCollection>(de.TimeStamp, elements.Text, null));

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
				} // if (direction == Rx)
			} // foreach (element)
		}

		/// <summary>
		/// Evaluates the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines)
		{
			List<Triple<DateTime, string, MatchCollection>> triggersDummy;
			EvaluateAutoActionOtherThanFilterOrSuppressFromLines(lines, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines, out List<Triple<DateTime, string, MatchCollection>> triggers)
		{
			EvaluateAndEnqueueAutoActionClearRepositoriesOnSubsequentRx();

			if (SettingsRoot.AutoAction.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoActionFromElements(dl, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Triple<DateTime, string, MatchCollection>>(); // No preset needed, the default behavior is good enough.

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
								dl.Highlight = true;

								// Signal the trigger(s):
								for (int i = 0; i < triggerCount; i++)
									triggers.Add(new Triple<DateTime, string, MatchCollection>(dl.TimeStamp, dl.Text, triggerMatches));
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

						default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Only 'Filter' and 'Suppress' are evaluated here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

					lock (this.autoActionTriggerHelperSyncObj)
					{
						triggerTimeStamp = this.autoActionClearRepositoriesTriggerTimeStamp;
						triggerText      = this.autoActionClearRepositoriesTriggerText;
						triggerMatches   = this.autoActionClearRepositoriesTriggerMatches;

						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;
						this.autoActionClearRepositoriesTriggerTimeStamp      = DateTime.MinValue;
						this.autoActionClearRepositoriesTriggerText           = null;
						this.autoActionClearRepositoriesTriggerMatches        = null;
					}
					                        //// ClearRepositories is to be enqueued, not ClearRepositoriesOnSubsequentRx!
					EnqueueAutoAction(AutoAction.ClearRepositories, triggerTimeStamp, triggerText, triggerMatches);
				}
			}
		}

		/// <summary>
		/// Enqueues the automatic actions for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoActions(List<Triple<DateTime, string, MatchCollection>> triggers)
		{
			foreach (var trigger in triggers)
				EnqueueAutoAction(trigger.Value1, trigger.Value2, trigger.Value3);
		}

		/// <summary>
		/// Enqueues the automatic action for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoAction(DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches)
		{
			EnqueueAutoAction(SettingsRoot.AutoAction.Action, triggerTimeStamp, triggerText, triggerMatches);
		}

		/// <summary>
		/// Enqueues the automatic action for invocation on other than the receive thread.
		/// </summary>
		protected virtual void EnqueueAutoAction(AutoAction action, DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches)
		{
			lock (this.autoActionQueue) // Lock is required because Queue<T> is not synchronized.
				this.autoActionQueue.Enqueue(new Quadruple<AutoAction, DateTime, string, MatchCollection>(action, triggerTimeStamp, triggerText, triggerMatches));

			SignalAutoActionThreadSafely();
		}

		/// <summary>
		/// Asynchronously invoke the automatic actions on other than the receive thread.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="EnqueueAutoAction(AutoAction, DateTime, string, MatchCollection)"/> above.
		/// </remarks>
		private void AutoActionThread()
		{
			DebugThreadState("AutoActionThread() has started.");

			try
			{
				// Outer loop, processes data after a signal has been received:
				while (!IsDisposed && this.autoActionThreadRunFlag) // Check 'IsDisposed' first!
				{
					try
					{
						// WaitOne() will wait forever if the underlying I/O provider has crashed, or
						// if the overlying client isn't able or forgets to call Stop() or Dispose().
						// Therefore, only wait for a certain period and then poll the run flag again.
						// The period can be quite long, as an event trigger will immediately resume.
						if (!this.autoActionThreadEvent.WaitOne(staticRandom.Next(50, 200)))
							continue;
					}
					catch (AbandonedMutexException ex)
					{
						// The mutex should never be abandoned, but in case it nevertheless happens,
						// at least output a debug message and gracefully exit the thread.
						DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in AutoActionThread()!");
						break;
					}

					// Inner loop, runs as long as there is data in the send queue.
					// Ensure not to send and forward events during closing anymore. Check 'IsDisposed' first!
					while (!IsDisposed && this.autoActionThreadRunFlag && IsReadyToSend && (this.autoActionQueue.Count > 0))
					{                                                                   // No lock required, just checking for empty.
						// Initially, yield to other threads before starting to read the queue,
						// since it is likely that more triggers are to be enqueued.
						Thread.Sleep(TimeSpan.Zero);

						Quadruple<AutoAction, DateTime, string, MatchCollection>[] pendingItems;
						lock (this.autoActionQueue) // Lock is required because Queue<T> is not synchronized.
						{
							pendingItems = this.autoActionQueue.ToArray();
							this.autoActionQueue.Clear();
						}

						foreach (var item in pendingItems)
						{
							PreformAutoAction(item.Value1, item.Value2, item.Value3, item.Value4);
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

			DebugThreadState("AutoActionThread() has terminated.");
		}

		/// <summary>
		/// Performs the automatic action.
		/// </summary>
		protected virtual void PreformAutoAction(AutoAction action, DateTime triggerTimeStamp, string triggerText, MatchCollection triggerMatches)
		{
			int count = Interlocked.Increment(ref this.autoActionCount); // Incrementing before invoking to have the effective count updated during action.
			OnAutoActionCountChanged(new EventArgs<int>(count));

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

				case AutoAction.Beep:               SystemSounds.Beep.Play();                                        break;
				case AutoAction.ShowMessageBox:     RequestAutoActionMessage(triggerTimeStamp,  triggerText, count); break;

				case AutoAction.LineChartIndex:     RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches); break;
				case AutoAction.LineChartTimeStamp: RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches); break;
				case AutoAction.ScatterPlotXY:      RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches); break;
				case AutoAction.ScatterPlotTime:    RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches); break;
				case AutoAction.Histogram:          RequestAutoActionPlot(action, triggerTimeStamp, triggerMatches); break;

				case AutoAction.ClearRepositories:  ClearRepositories();                                             break;

				case AutoAction.ClearRepositoriesOnSubsequentRx:
				{
					lock (this.autoActionClearRepositoriesSyncObj)
					{
						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true;
						this.autoActionClearRepositoriesTriggerTimeStamp = triggerTimeStamp;
						this.autoActionClearRepositoriesTriggerText      = triggerText;
						this.autoActionClearRepositoriesTriggerMatches   = triggerMatches;
					}
					break;
				}

				case AutoAction.ResetCountAndRate: ResetIOCountAndRate(); break;
				case AutoAction.SwitchLogOn:       SwitchLogOn();         break;
				case AutoAction.SwitchLogOff:      SwitchLogOff();        break;
				case AutoAction.ToggleLogOnOrOff:  ToggleLogOnOrOff();    break;
				case AutoAction.StopIO:            StopIO();              break;
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
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + action + "' is an automatic action that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Determines wether the given action has a true action to invoke.
		/// </summary>
		protected virtual bool HasActionToInvoke(AutoAction action)
		{
			// Attention:
			// Same switch/case exists in PreformAutoAction() above.
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
				case AutoAction.LineChartIndex:
				case AutoAction.LineChartTimeStamp:
				case AutoAction.ScatterPlotXY:
				case AutoAction.ScatterPlotTime:
				case AutoAction.Histogram:
				case AutoAction.ClearRepositories:
				case AutoAction.ClearRepositoriesOnSubsequentRx:
				case AutoAction.ResetCountAndRate:
				case AutoAction.SwitchLogOn:
				case AutoAction.SwitchLogOff:
				case AutoAction.ToggleLogOnOrOff:
				case AutoAction.StopIO:
				case AutoAction.CloseTerminal:
				case AutoAction.ExitApplication:
					return (true);

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + action + "' is an automatic action that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		protected virtual void RequestAutoActionPlot(AutoAction plotAction, DateTime triggerTimeStamp, MatchCollection triggerMatches)
		{
			AutoActionPlotItem pi;
			string errorMessage;
			if (TryCreateAutoActionPlotItem(plotAction, triggerTimeStamp, triggerMatches, out pi, out errorMessage))
				OnAutoActionPlotRequest(new AutoActionPlotEventArgs(CaptionHelper.ComposeInvariant(IndicatedName, "Plot"), pi));
			else
				this.terminal.InlineErrorMessage(Domain.Direction.Rx, errorMessage, true);
		}

		/// <summary>
		/// Requests the desired chart/plot.
		/// </summary>
		protected virtual bool TryCreateAutoActionPlotItem(AutoAction plotAction, DateTime triggerTimeStamp, MatchCollection triggerMatches, out AutoActionPlotItem pi, out string errorMessage)
		{
			switch (plotAction)
			{
				case AutoAction.LineChartIndex: return (TryCreateLineChartIndexItem(plotAction,                   triggerMatches, out pi, out errorMessage));
//				case AutoAction.LineChartTime:  return (TryCreateLineChartTimeItem( plotAction, triggerTimeStamp, triggerMatches, out pi, out errorMessage));
// PENDING		case AutoAction.ScatterPlot:    return (TryCreateScatterPlotItem(   plotAction,                   triggerMatches, out pi, out errorMessage));
// PENDING		case AutoAction.ScatterPlot:    return (TryCreateScatterPlotItem(   plotAction,                   triggerMatches, out pi, out errorMessage));
//				case AutoAction.Histogram:      return (TryCreateHistogramItem(     plotAction,                   triggerMatches, out pi, out errorMessage));

// PENDING	Textli eimal formuliere, dänn ToolTip & MsgBox
// Histogram: Each capture

				default: throw (new ArgumentOutOfRangeException("plot", plotAction, MessageHelper.InvalidExecutionPreamble + "'" + plotAction.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Requests the desired chart/plot.
		/// </summary>
		protected virtual bool TryCreateLineChartIndexItem(AutoAction plotAction, MatchCollection triggerMatches, out AutoActionPlotItem pi, out string errorMessage)
		{
			var captures = MatchCollectionEx.UnfoldCapturesToStringArray(triggerMatches);
			var values = ConvertCapturesToPlotValues(AutoAction.LineChartIndex, captures);
			pi = new ValueCollectionAutoActionPlotItem(plotAction, values);
			errorMessage = null;
			return (true);
		}

		/// <summary>
		/// Requests the desired chart/plot.
		/// </summary>
		protected virtual Tuple<string, double>[] ConvertCapturesToPlotValues(AutoAction plotAction, string[] captures)
		{
			var yValues = new List<Tuple<string, double>>(); // No preset needed, the default behavior is good enough.

			foreach (var s in captures)
			{
				// Try double:
				{
					double result;
					if (double.TryParse(s, out result))       // Always name series, even if legend is disabled
					{                                         // currently, as user can enable it at any time.
						yValues.Add(new Tuple<string, double>("numeric values", result));
						continue;
					}
				}

				// Try DateTime:
				{
					DateTime result;
					if (DateTime.TryParse(s, out result))
					{
						yValues.Add(new Tuple<string, double>("time", result.ToOADate()));
						continue;
					}
				}

				// Use string:
				{
					long hash = 0;
					foreach (char c in s)
						hash += c;

					yValues.Add(new Tuple<string, double>("sum of character codes", hash));
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
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.autoActionCount);
			}
		}

		/// <summary>
		/// Resets the automatic action count.
		/// </summary>
		public virtual void ResetAutoActionCount()
		{
			AssertNotDisposed();

			int count = Interlocked.Exchange(ref this.autoActionCount, 0);
			OnAutoActionCountChanged(new EventArgs<int>(count));
		}

		/// <summary>
		/// Deactivates the automatic action.
		/// </summary>
		public virtual void DeactivateAutoAction()
		{
			AssertNotDisposed();

			SettingsRoot.AutoAction.Deactivate();
			ResetAutoActionCount();
		}

		#endregion

		#region Thread
		//------------------------------------------------------------------------------------------
		// Thread
		//------------------------------------------------------------------------------------------

		private void CreateAndStartAutoActionThread()
		{
			lock (this.autoActionThreadSyncObj)
			{
				DebugThreadState("AutoActionThread() gets created...");

				if (this.autoActionThread == null)
				{
					this.autoActionThreadRunFlag = true;
					this.autoActionThreadEvent = new AutoResetEvent(false);
					this.autoActionThread = new Thread(new ThreadStart(AutoActionThread));
					this.autoActionThread.Name = "Terminal [" + Guid + "] Auto Action Thread";
					this.autoActionThread.Start();

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
		private void StopAutoActionThread()
		{
			lock (this.autoActionThreadSyncObj)
			{
				if (this.autoActionThread != null)
				{
					DebugThreadState("AutoActionThread() gets stopped...");

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
								DebugThreadState("...failed! Aborting...");
								DebugThreadState("(Abort is likely required due to failed synchronization back the calling thread, which is typically the main thread.)");

								isAborting = true;       // Thread.Abort() must not be used whenever possible!
								this.autoActionThread.Abort(); // This is only the fall-back in case joining fails for too long.
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

					this.autoActionThread = null;
				}
			#if (DEBUG)
				else // (this.autoActionThread == null)
				{
					DebugThreadState("...not necessary as it doesn't exist anymore.");
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
		protected virtual void OnAutoActionPlotRequest(AutoActionPlotEventArgs e)
		{
			this.eventHelper.RaiseSync<AutoActionPlotEventArgs>(AutoActionPlotRequest, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoActionCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
