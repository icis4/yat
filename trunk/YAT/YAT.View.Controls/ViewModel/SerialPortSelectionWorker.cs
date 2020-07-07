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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.IO.Ports;

// This code is intentionally placed into the YAT.View.Controls namespace even though the file is
// located in YAT.View.Controls.ViewModel for same location as parent control.
namespace YAT.View.Controls
{
	/// <remarks>
	/// Not using <see cref="System.ComponentModel.BackgroundWorker"/> due to its limitations:
	///  - Not possible to abort the thread (required for cases where system calls crash).
	///  - Somewhat cumbersome event/message handling ('UserState').
	/// </remarks>
	internal class SerialPortSelectionWorker
	{
		/// <summary>
		/// A dedicated event helper to allow ignoring the 'ThreadAbortException' when cancelling.
		/// </summary>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(SerialPortSelectionWorker).FullName);

		private SerialPortCollection ports;

		private bool retrieveCaptions;
		private bool detectPortsInUse;
		private InUseInfo activePortInUseInfo;

		private bool cancel; // = false;
		private object cancelSyncObj = new object();

		private bool isBusy = true;

		private DialogResult result;  // = DialogResult.None;
		private Exception exception;  // = null;
		private string exceptionLead; // = null;
		private string exceptionHint; // = null;

		public event EventHandler<EventArgs<string>> Status1Changed;
		public event EventHandler<EventArgs<string>> Status2Changed;
		public event EventHandler<EventArgs<DialogResult>> IsDone;

		public SerialPortSelectionWorker(bool retrieveCaptions, bool detectPortsInUse, InUseInfo activePortInUseInfo)
		{
			this.retrieveCaptions    = retrieveCaptions;
			this.detectPortsInUse    = detectPortsInUse;
			this.activePortInUseInfo = activePortInUseInfo;
		}

		public virtual SerialPortCollection Ports
		{
			get { return (this.ports); }
		}

		public virtual bool IsBusy
		{
			get { return (this.isBusy); }
		}

		public virtual DialogResult Result
		{
			get { return (this.result); }
		}

		public virtual Exception Exception
		{
			get { return (this.exception); }
		}

		public virtual string ExceptionLead
		{
			get { return (this.exceptionLead); }
		}

		public virtual string ExceptionHint
		{
			get { return (this.exceptionHint); }
		}

		public virtual void DoWork()
		{
			try
			{
				this.isBusy = true;
				this.result = DoWorkWithResult();
				this.isBusy = false;

				OnIsDone(new EventArgs<DialogResult>(result));
			}
			catch (ThreadAbortException ex)
			{
				// Will happen when failing to 'friendly' join the thread on cancel.

				DebugEx.WriteException(GetType(), ex, "Worker thread has been aborted. Confirming the abort, i.e. Thread.ResetAbort() will be called...");

				// Reset the abort request, as 'ThreadAbortException' is a special exception
				// that would be rethrown at the end of the catch block otherwise!

				Thread.ResetAbort();
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private DialogResult DoWorkWithResult()
		{
			OnStatus1Changed(new EventArgs<string>("Retrieving available serial COM ports..."));

			this.ports = new SerialPortCollection();
			this.ports.ActivePortInUseInfo = this.activePortInUseInfo;

			try // Exceptions should not happen, but actually sometimes do, e.g. in case of Bluetooth...
			{
				this.ports.FillWithAvailablePorts(false); // Captions will be retrieved further below.
			}
			catch (Exception ex)
			{
				this.ports.FillWithTypicalStandardPorts();

				this.exception = ex;
				this.exceptionLead = "There was an error while retrieving the serial COM ports from the system!";
				this.exceptionHint = "The port list has been defaulted with the typical standard ports.";

				return (DialogResult.Abort);
			}

			if (this.retrieveCaptions && !this.cancel)
			{
				OnStatus1Changed(new EventArgs<string>("Retrieving serial COM port captions from system..."));

				try
				{
					this.ports.RetrieveCaptions();
				}
				catch (Exception ex)
				{
					this.exception = ex;
					this.exceptionLead = "There was an error while retrieving the serial COM port captions from the system!";
					this.exceptionHint = "If the issue cannot be solved, tell YAT to no longer retrieve port captions by going to 'File > Preferences...' and disable 'retrieve port captions from system'.";

					return (DialogResult.Abort);
				}
			}

			if (this.detectPortsInUse && !this.cancel)
			{
				OnStatus1Changed(new EventArgs<string>("Detecting serial COM ports that are in use..."));

				try
				{
					this.ports.DetectPortsThatAreInUse(ports_DetectPortsInUseCallback);
				}
				catch (Exception ex)
				{
					this.exception = ex;
					this.exceptionLead = "There was an error while trying to detect the serial COM ports that are in use!";
					this.exceptionHint = "If the issue cannot be solved, tell YAT to no longer detect ports that are in use by going to 'File > Preferences...' and disable 'detect ports that are in use'.";

					return (DialogResult.Abort);
				}
			}

			bool cancel;
			lock (this.cancelSyncObj)
				cancel = this.cancel;

			if (cancel)
				return (DialogResult.Cancel);
			else
				return (DialogResult.OK);
		}

		public virtual void CancelWork()
		{
			lock (this.cancelSyncObj)
				this.cancel = true;
		}

		/// <summary>
		/// Notifies the worker that a thread abort is about to happen soon.
		/// </summary>
		public virtual void NotifyThreadAbortWillHappen()
		{
			this.eventHelper.DiscardAllExceptions();

			this.ports.NotifyThreadAbortWillHappen();
		}

		private void ports_DetectPortsInUseCallback(object sender, SerialPortChangedAndCancelEventArgs e)
		{
			OnStatus2Changed(new EventArgs<string>("Scanning " + e.Port.ToNameAndCaptionString() + "..."));

			lock (this.cancelSyncObj)
				e.Cancel = this.cancel;
		}

		/// <summary>
		/// Invokes the <see cref="Status1Changed"/> event.
		/// </summary>
		protected virtual void OnStatus1Changed(EventArgs<string> e)
		{
			this.eventHelper.RaiseSync<EventArgs<string>>(Status1Changed, this, e);
		}

		/// <summary>
		/// Invokes the <see cref="Status2Changed"/> event.
		/// </summary>
		protected virtual void OnStatus2Changed(EventArgs<string> e)
		{
			this.eventHelper.RaiseSync<EventArgs<string>>(Status2Changed, this, e);
		}

		/// <summary>
		/// Invokes the <see cref="IsDone"/> event.
		/// </summary>
		protected virtual void OnIsDone(EventArgs<DialogResult> e)
		{
			this.eventHelper.RaiseAsync<EventArgs<DialogResult>>(IsDone, this, e); // Raise async! Worker thread termination must not delayed by a sync callbacks!
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
