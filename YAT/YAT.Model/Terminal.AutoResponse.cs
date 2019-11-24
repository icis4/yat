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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;
using MKY.Text;
using MKY.Time;
using MKY.Windows.Forms;

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;

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
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				if (this.settingsRoot.AutoResponse.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
					{
						byte[] triggerSequence;
						if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoResponseTrigger, out triggerSequence))
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
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void EvaluateAutoResponse(Domain.DisplayElementsEventArgs e)
		{
			int triggerCount = 0;

			foreach (var de in e.Elements)
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

			if (triggerCount > 0)
			{
				// Invoke sending on other than the receive thread:
				byte[] triggerSequence = null;

				lock (this.autoResponseTriggerHelperSyncObj)
				{
					if (this.autoResponseTriggerHelper != null)
						triggerSequence = this.autoResponseTriggerHelper.TriggerSequence;
				}

				var asyncInvoker = new Action<byte[]>(SendAutoResponse);
				asyncInvoker.BeginInvoke(triggerSequence, null, null);
			}
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence)
		{
			this.autoResponseCount++; // Incrementing before sending to have the effective count available during sending.
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));

			int page = this.settingsRoot.Predefined.SelectedPageId;
			switch ((AutoResponse)this.settingsRoot.AutoResponse.Response)
			{
				case AutoResponse.Trigger:             SendAutoResponseTrigger(triggerSequence); break;
				case AutoResponse.SendText:            SendText();                               break;
				case AutoResponse.SendFile:            SendFile();                               break;
				case AutoResponse.PredefinedCommand1:  SendPredefined(page, 1);                  break;
				case AutoResponse.PredefinedCommand2:  SendPredefined(page, 2);                  break;
				case AutoResponse.PredefinedCommand3:  SendPredefined(page, 3);                  break;
				case AutoResponse.PredefinedCommand4:  SendPredefined(page, 4);                  break;
				case AutoResponse.PredefinedCommand5:  SendPredefined(page, 5);                  break;
				case AutoResponse.PredefinedCommand6:  SendPredefined(page, 6);                  break;
				case AutoResponse.PredefinedCommand7:  SendPredefined(page, 7);                  break;
				case AutoResponse.PredefinedCommand8:  SendPredefined(page, 8);                  break;
				case AutoResponse.PredefinedCommand9:  SendPredefined(page, 9);                  break;
				case AutoResponse.PredefinedCommand10: SendPredefined(page, 10);                 break;
				case AutoResponse.PredefinedCommand11: SendPredefined(page, 11);                 break;
				case AutoResponse.PredefinedCommand12: SendPredefined(page, 12);                 break;

				case AutoResponse.Explicit:
					SendCommand(new Command(this.settingsRoot.AutoResponse.Response)); // No explicit default radix available (yet).
					break;

				case AutoResponse.None:
				default:
					break;
			}
		}

		/// <summary>
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void SendAutoResponseTrigger(byte[] triggerSequence)
		{
			if (triggerSequence != null)
				this.terminal.Send(TriggerSequenceWithTxEol(triggerSequence));
		}

		private byte[] TriggerSequenceWithTxEol(byte[] triggerSequence)
		{
			var l = new List<byte>(triggerSequence);

			if (this.settingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Add Tx EOL:
				var txEolSequence = textTerminal.TxEolSequence;
				l.AddRange(txEolSequence);
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

			this.autoResponseCount = 0;
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));
		}

		/// <summary>
		/// Deactivates the automatic response.
		/// </summary>
		public virtual void DeactivateAutoResponse()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoResponse.Deactivate();
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
