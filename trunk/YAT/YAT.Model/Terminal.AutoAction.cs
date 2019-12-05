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
using System.Linq;
using System.Media;
using System.Text;
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
	////private Queue<Action<AutoAction, string, DateTime>> autoActionTasks = new Queue<Action<AutoAction, string, DateTime>>(); activate after having upgraded to .NET 4.0

		private bool autoActionClearRepositoriesOnSubsequentRxIsArmed; // = false;
		private string autoActionClearRepositoriesTriggerText; // = null;
		private DateTime autoActionClearRepositoriesTriggerTimeStamp; // = DateTime.MinValue;
		private object autoActionClearRepositoriesSyncObj = new object();

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void CreateAutoActionHelper()
		{
			UpdateAutoAction(); // Simply forward to general Update() method.
		}

		private void DisposeAutoActionHelper()
		{
			lock (this.autoActionTriggerHelperSyncObj)
				this.autoActionTriggerHelper = null; // Simply delete the reference to the object.
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
								this.autoActionTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern, SettingsRoot.AutoAction.Options.CaseSensitive, SettingsRoot.AutoAction.Options.WholeWord, triggerRegex);
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

		/// <summary>
		/// Evaluates the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements)
		{
			List<Pair<string, DateTime>> triggersDummy;
			EvaluateAutoActionFromElements(elements, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionFromElements(Domain.DisplayElementCollection elements, out List<Pair<string, DateTime>> triggers)
		{
			triggers = new List<Pair<string, DateTime>>(); // No preset needed, the default behavior is good enough.

			EvaluateAndInvokeAutoActionClearRepositoriesOnSubsequentRx();

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
											triggers.Add(new Pair<string, DateTime>(elements.Text, de.TimeStamp));

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
			List<Pair<string, DateTime>> triggersDummy;
			EvaluateAutoActionOtherThanFilterOrSuppressFromLines(lines, out triggersDummy);
		}

		/// <summary>
		/// Evaluates the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		protected virtual void EvaluateAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines, out List<Pair<string, DateTime>> triggers)
		{
			EvaluateAndInvokeAutoActionClearRepositoriesOnSubsequentRx();

			if (SettingsRoot.AutoAction.IsByteSequenceTriggered)
			{
				triggers = null;

				foreach (var dl in lines)
					EvaluateAutoActionFromElements(dl, out triggers);
			}
			else // IsTextTriggered
			{
				triggers = new List<Pair<string, DateTime>>(); // No preset needed, the default behavior is good enough.

				foreach (var dl in lines)
				{
					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							int triggerCount = this.autoActionTriggerHelper.TextTriggerCount(dl.Text);
							if (triggerCount > 0)
							{
								this.autoActionTriggerHelper.Reset(); // Invoke shall happen as short as possible after detection.
								dl.Highlight = true;

								// Signal the trigger(s):
								for (int i = 0; i < triggerCount; i++)
									triggers.Add(new Pair<string, DateTime>(dl.Text, dl.TimeStamp));
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
		/// Evaluates and invokes <see cref="AutoAction.ClearRepositoriesOnSubsequentRx"/>.
		/// </summary>
		protected virtual void EvaluateAndInvokeAutoActionClearRepositoriesOnSubsequentRx()
		{
			lock (this.autoActionClearRepositoriesSyncObj)
			{
				if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
				{
					string triggerText;
					DateTime triggerTimeStamp;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						triggerText      = this.autoActionClearRepositoriesTriggerText;
						triggerTimeStamp = this.autoActionClearRepositoriesTriggerTimeStamp;

						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;
						this.autoActionClearRepositoriesTriggerText            = null;
						this.autoActionClearRepositoriesTriggerTimeStamp       = DateTime.MinValue;
					}
					                       //// ClearRepositories is to be invoked, not ClearRepositoriesOnSubsequentRx!
					InvokeAutoAction(AutoAction.ClearRepositories, triggerText, triggerTimeStamp);
				}
			}
		}

		/// <summary>
		/// Invokes the automatic action on an other than the receive thread.
		/// </summary>
		protected virtual void InvokeAutoAction(string triggerText, DateTime triggerTimeStamp)
		{
			InvokeAutoAction(SettingsRoot.AutoAction.Action, triggerText, triggerTimeStamp);
		}

		/// <summary>
		/// Invokes the automatic action on an other than the receive thread.
		/// </summary>
		protected virtual void InvokeAutoAction(AutoAction action, string triggerText, DateTime triggerTimeStamp)
		{
		////Replace by EnqueueAutoAction(); after having upgraded to .NET 4.0

			var asyncInvoker = new Action<AutoAction, string, DateTime>(PreformAutoAction);
			asyncInvoker.BeginInvoke(action, triggerText, triggerTimeStamp, null, null);
		}

		/// <summary>
		/// Performs the automatic action.
		/// </summary>
		protected virtual void PreformAutoAction(AutoAction action, string triggerText, DateTime triggerTimeStamp)
		{
		////while (this.autoActionTasks.Count > 0) after having upgraded to .NET 4.0
		////{
		////	var task = this.autoActionTasks.Dequeue();
		////	task.Invoke(...);

			// \remind (2019-12-03 / MKY)
			// Until .NET 4.0, there is an (acceptable) limitation with the current implementation:
			// If multiple triggers are detected within the same chunk or close after each other,
			// the sequence of the responses may get mixed up. For 'AutoResponse', this should not
			// be an issue, but for 'AutoAction::MessageBox' this is not "nice".

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

				case AutoAction.Beep:                            SystemSounds.Beep.Play();                                       break;
				case AutoAction.ShowMessageBox:                  RequestAutoActionMessage(triggerText, triggerTimeStamp, count); break;
				case AutoAction.ClearRepositories:               ClearRepositories();                                            break;
				case AutoAction.ClearRepositoriesOnSubsequentRx:
				{
					lock (this.autoActionClearRepositoriesSyncObj)
					{
						this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true;
						this.autoActionClearRepositoriesTriggerText      = triggerText;
						this.autoActionClearRepositoriesTriggerTimeStamp = triggerTimeStamp;
					}
					break;
				}
				case AutoAction.ResetCountAndRate:               ResetIOCountAndRate();          break;
				case AutoAction.SwitchLogOn:                     SwitchLogOn();                  break;
				case AutoAction.SwitchLogOff:                    SwitchLogOff();                 break;
				case AutoAction.StopIO:                          StopIO();                       break;
				case AutoAction.CloseTerminal:                   Close();                        break;
				case AutoAction.ExitApplication:                 OnExitRequest(EventArgs.Empty); break;

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
				case AutoAction.ClearRepositories:
				case AutoAction.ClearRepositoriesOnSubsequentRx:
				case AutoAction.ResetCountAndRate:
				case AutoAction.SwitchLogOn:
				case AutoAction.SwitchLogOff:
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
		protected virtual void RequestAutoActionMessage(string triggerText, DateTime triggerTimeStamp, int totalCount)
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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

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
