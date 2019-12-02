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
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MKY;

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
		private string autoActionClearRepositoriesTriggerText; // = null;
		private DateTime autoActionClearRepositoriesTriggerTimeStamp; // = DateTime.MinValue;

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
			if (this.settingsRoot.AutoAction.IsActive)
			{
				if (this.settingsRoot.AutoAction.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					Command triggerCommand;
					string  triggerTextOrRegexPattern;
					Regex   triggerRegex;

					if (this.settingsRoot.TryGetActiveAutoActionTrigger(out triggerCommand, out triggerTextOrRegexPattern, out triggerRegex))
					{
						if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
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
						else // IsTextOrRegexTriggered
						{
							if (this.settingsRoot.AutoAction.IsTextTriggered)
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern);
							}
							else // IsRegexTriggered
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new AutoTriggerHelper(triggerTextOrRegexPattern, triggerRegex);
							}
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
		/// Processes <see cref="AutoAction.ClearRepositoriesOnSubsequentRx"/>.
		/// </summary>
		protected virtual void ProcessAutoActionClearRepositoriesOnSubsequentRx()
		{
			if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
			{
				byte[] triggerSequence;
				string triggerText;
				DateTime triggerTimeStamp;

				lock (this.autoActionTriggerHelperSyncObj)
				{
					triggerSequence  = this.autoActionTriggerHelper.TriggerSequence;
					triggerText      = this.autoActionClearRepositoriesTriggerText;
					triggerTimeStamp = this.autoActionClearRepositoriesTriggerTimeStamp;

					this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;
					this.autoActionClearRepositoriesTriggerText = null;
					this.autoActionClearRepositoriesTriggerTimeStamp = DateTime.MinValue;
				}
				                       //// ClearRepositories is to be invoked, not ClearRepositoriesOnSubsequentRx!
				InvokeAutoAction(AutoAction.ClearRepositories, triggerSequence, triggerText, triggerTimeStamp);
			}
		}

		/// <summary>
		/// Processes the automatic action.
		/// </summary>
		/// <remarks>
		/// Automatic actions from elements always are non-reloadable.
		/// </remarks>
		protected virtual void ProcessAutoActionFromElements(Domain.DisplayElementCollection elements)
		{
			ProcessAutoActionClearRepositoriesOnSubsequentRx();

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
										de.Highlight = true;

										// Invoke shall happen as short as possible after detection:
										InvokeAutoResponse(this.autoActionTriggerHelper.TriggerSequence, null);
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
		/// Processes the automatic actions other than <see cref="AutoAction.Filter"/> and <see cref="AutoAction.Suppress"/>.
		/// </summary>
		/// <remarks>
		/// Automatic actions from lines may be reloadable.
		/// </remarks>
		protected virtual void ProcessAutoActionOtherThanFilterOrSuppressFromLines(Domain.DisplayLineCollection lines)
		{
			ProcessAutoActionClearRepositoriesOnSubsequentRx();

			if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
			{
				foreach (var dl in lines)
					ProcessAutoActionFromElements(dl);
			}
			else // IsTextOrRegexTriggered
			{
				foreach (var dl in lines)
				{
					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							int triggerCount = 0;

							if (this.settingsRoot.AutoAction.IsTextTriggered)
								triggerCount = StringEx.ContainingCount(dl.Text, this.autoActionTriggerHelper.TriggerText);
							else                          // IsRegexTriggered
								triggerCount = this.autoActionTriggerHelper.TriggerRegex.Matches(dl.Text).Count;

							if (triggerCount > 0)
							{
								this.autoActionTriggerHelper.Reset(); // Invoke shall happen as short as possible after detection.
								dl.Highlight = true;

								for (int i = 0; i < triggerCount; i++)
									InvokeAutoAction(this.settingsRoot.AutoAction.Action, null, dl.Text, dl.TimeStamp);
							}
						}
						else
						{
							break;     // Break the loop if action got disposed in the meantime.
						}              // Though unlikely, it may happen when deactivating action
					} // lock (helper) // while processing many lines, e.g. on reload.
				} // foreach (line)
			} // IsTextOrRegexTriggered
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
			                                    // Limit processing to pure 'Rx' lines. Required for 'Bidir' monitor!
			foreach (var dl in linesInitially.Where(dl => dl.Direction == Domain.Direction.Rx))
			{
				bool isTriggered = false;

				lock (this.autoActionTriggerHelperSyncObj)
				{
					if (this.autoActionTriggerHelper != null)
					{
						if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
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
						else // IsTextOrRegexTriggered
						{
							if (this.settingsRoot.AutoAction.IsTextTriggered)
								isTriggered = dl.Text.Contains(this.autoActionTriggerHelper.TriggerText);
							else                          // IsRegexTriggered
								isTriggered = this.autoActionTriggerHelper.TriggerRegex.Match(dl.Text).Success;
						}
					}
					else
					{
						break;     // Break the loop if action got disposed in the meantime.
					}              // Though unlikely, it may happen when deactivating action
				} // lock (helper) // while processing many lines, e.g. on reload.

				switch ((AutoAction)this.settingsRoot.AutoAction.Action)
				{
					case AutoAction.Filter:   if ( isTriggered) { lines.Add(dl); } break;
					case AutoAction.Suppress: if (!isTriggered) { lines.Add(dl); } break;

					default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Only 'Filter' and 'Suppress' are evaluated here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			} // foreach (lineCloned)
		}

		/// <summary>
		/// Invokes the automatic action on an other than the receive thread.
		/// </summary>
		protected virtual void InvokeAutoAction(AutoAction action, byte[] triggerSequence, string triggerText, DateTime timeStamp)
		{
			var asyncInvoker = new Action<AutoAction, byte[], string, DateTime>(PreformAutoAction);
			asyncInvoker.BeginInvoke(action, triggerSequence, triggerText, timeStamp, null, null);
		}

		/// <summary>
		/// Performs the automatic action.
		/// </summary>
		protected virtual void PreformAutoAction(AutoAction action, byte[] triggerSequence, string triggerText, DateTime ts)
		{
			this.autoActionCount++; // Incrementing before invoking to have the effective count available during invoking.
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));

			// Attention:
			// Same switch/case exists in HasActionToInvoke() below.
			// Changes here must be applied there too.

			switch (action)
			{
				case AutoAction.Beep:                            SystemSounds.Beep.Play();                                     break;
				case AutoAction.ShowMessageBox:                  RequestAutoActionMessage(triggerSequence, triggerText, ts);   break;
				case AutoAction.ClearRepositories:               ClearRepositories();                                          break;
				case AutoAction.ClearRepositoriesOnSubsequentRx: this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true;
				                                                 this.autoActionClearRepositoriesTriggerText = triggerText;
				                                                 this.autoActionClearRepositoriesTriggerTimeStamp = ts;        break;
				case AutoAction.ResetCountAndRate:               ResetIOCountAndRate();                                        break;
				case AutoAction.SwitchLogOn:                     SwitchLogOn();                                                break;
				case AutoAction.SwitchLogOff:                    SwitchLogOff();                                               break;
				case AutoAction.StopIO:                          StopIO();                                                     break;
				case AutoAction.CloseTerminal:                   Close();                                                      break;
				case AutoAction.ExitApplication:                 OnExitRequest(EventArgs.Empty);                               break;

				case AutoAction.Highlight:
				case AutoAction.Filter:
				case AutoAction.Suppress:
					// No additional action.
					break;

				case AutoAction.None:
				default:
					// Nothing to do.
					break;
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

				case AutoAction.Highlight:
				case AutoAction.Filter:
				case AutoAction.Suppress:
					return (false);

				case AutoAction.None:
				default:
					return (false);
			}
		}

		/// <summary>
		/// Notifies the user about the action.
		/// </summary>
		protected virtual void RequestAutoActionMessage(byte[] triggerSequence, string triggerText, DateTime ts)
		{
			var sb = new StringBuilder();
			sb.Append(@"Message has been triggered by """);

			if (!ArrayEx.IsNullOrEmpty(triggerSequence))
				sb.Append(this.terminal.Format(triggerSequence, Domain.IODirection.Rx));
			else if (!string.IsNullOrEmpty(triggerText))
				sb.Append(triggerText);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Either byte sequence or text must be defined!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			sb.Append(@""" the ");
			sb.Append(this.autoActionCount);
			sb.Append(Int32Ex.ToEnglishSuffix(this.autoActionCount));
			sb.Append(" time at ");
			sb.Append(this.terminal.Format(ts));
			sb.Append(".");

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

			this.autoActionCount = 0;
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));
		}

		/// <summary>
		/// Deactivates the automatic action.
		/// </summary>
		public virtual void DeactivateAutoAction()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoAction.Deactivate();
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
