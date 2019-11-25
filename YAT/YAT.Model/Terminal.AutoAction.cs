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
					if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
					{
						byte[] triggerSequence;
						if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoActionTrigger, out triggerSequence))
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
							string triggerText;
							if (TryValidateCommandForTriggerText(this.settingsRoot.ActiveAutoActionTrigger, out triggerText))
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new AutoTriggerHelper(triggerText);
							}
							else
							{
								DeactivateAutoAction();
								DisposeAutoActionHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic action trigger! The trigger is not a single line command! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
									"To enable again, re-configure the automatic action.",
									"Automatic Action Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning
								);
							}
						}
						else // IsRegexTriggered
						{
							string triggerRegexPattern;
							Regex triggerRegex;
							if (TryCreateTriggerRegexFromCommand(this.settingsRoot.ActiveAutoActionTrigger, out triggerRegexPattern, out triggerRegex))
							{
								lock (this.autoActionTriggerHelperSyncObj)
									this.autoActionTriggerHelper = new AutoTriggerHelper(triggerRegexPattern, triggerRegex);
							}
							else
							{
								DeactivateAutoAction();
								DisposeAutoActionHelper();

								OnMessageInputRequest
								(
									"Failed to parse the automatic action trigger! The trigger does not specify a valid regular expression! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
									"To enable again, re-configure the automatic action.",
									"Automatic Action Error",
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning
								);
							}
						}
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
		/// Sends the automatic response.
		/// </summary>
		protected virtual void EvaluateAutoAction(Domain.DisplayElementCollection elements)
		{
			if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
			{
				this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;

				byte[] triggerSequence;
				string triggerText;

				lock (this.autoActionTriggerHelperSyncObj)
				{
					triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					triggerText     = this.autoActionTriggerHelper.TriggerText;
				}
				                       //// ClearRepositories is to be invoked, not ClearRepositoriesOnSubsequentRx!
				InvokeAutoAction(AutoAction.ClearRepositories, triggerSequence, triggerText, elements.TimeStamp);
			}

			int triggerCount = 0;

			if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
			{
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
											triggerCount++;
											de.Highlight = true;
										}
									}
								}
							}
						}
						else
						{
							break; // Break the loop if action got disposed in the meantime.
						}          // Though unlikely, it may happen when deactivating action
					}              // while receiving a very large chunk.
				}
			}
			else // IsTextOrRegexTriggered
			{
				var dl = (elements as Domain.DisplayLine);
				if (dl != null)
				{
					if (this.settingsRoot.AutoAction.IsTextTriggered)
					{
						lock (this.autoActionTriggerHelperSyncObj)
						{
							triggerCount = StringEx.ContainingCount(dl.Text, this.autoActionTriggerHelper.TriggerText);
							if (triggerCount > 0)
								this.autoActionTriggerHelper.EffectiveTriggerTextLine = dl.Text;
						}
					}
					else // IsRegexTriggered
					{
						lock (this.autoActionTriggerHelperSyncObj)
						{
							var m = this.autoActionTriggerHelper.TriggerRegex.Matches(dl.Text);
							triggerCount = m.Count;

							if (triggerCount > 0)
								this.autoActionTriggerHelper.EffectiveTriggerTextLine = dl.Text;
						}
					}
				}
				else
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A line must be provided for text or regex trigger evaluation!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			if ((triggerCount > 0) && (HasActionToInvoke(this.settingsRoot.AutoAction.Action)))
			{
				byte[] triggerSequence;
				string triggerText;

				lock (this.autoActionTriggerHelperSyncObj)
				{
					triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					triggerText     = this.autoActionTriggerHelper.TriggerText;
				}

				InvokeAutoAction(this.settingsRoot.AutoAction.Action, triggerSequence, triggerText, elements.TimeStamp);
			}
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
				case AutoAction.ClearRepositoriesOnSubsequentRx: this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true; break;
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
