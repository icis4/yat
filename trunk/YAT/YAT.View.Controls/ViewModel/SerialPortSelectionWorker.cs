//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/YAT.View.Controls/ViewModel/MonitorRenderer.cs $
// $Author: maettu_this $
// $Date: 2016-05-09 13:12:15 +0200 (Mo, 09 Mai 2016) $
// $Revision: 980 $
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY;
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
		private SerialPortCollection ports;

		private bool retrieveCaptions;
		private bool detectPortsInUse;

		private bool cancel; // = false;
		private object cancelSyncObj = new object();

		private bool isBusy = true;

		private DialogResult result; // = DialogResult.None
		private Exception exception; // = null
		private string exceptionLead; // = null
		private string exceptionHint; // = null

		public event EventHandler<EventArgs<string>> Status1Changed;
		public event EventHandler<EventArgs<string>> Status2Changed;
		public event EventHandler<EventArgs<DialogResult>> IsDone;

		public SerialPortSelectionWorker(bool retrieveCaptions = true, bool detectPortsInUse = true)
		{
			this.retrieveCaptions = retrieveCaptions;
			this.detectPortsInUse = detectPortsInUse;
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public virtual void DoWork()
		{
			this.isBusy = true;
			this.result = DoWorkWithResult();
			this.isBusy = false;

			OnIsDone(new EventArgs<DialogResult>(result));
		}

		private DialogResult DoWorkWithResult()
		{
			OnStatus1Changed(new EventArgs<string>("Retrieving available ports..."));

			this.ports = new SerialPortCollection();

			try // Exceptions should not happen, but actually sometimes do, e.g. in case of Bluetooth...
			{
				this.ports.FillWithAvailablePorts(false); // Descriptions will be retrieved further below.
			}
			catch (Exception ex)
			{
				this.ports.FillWithStandardPorts();

				this.exception = ex;
				this.exceptionLead = "There was an error while retrieving the serial COM ports from the system!";
				this.exceptionHint = "The port list has been defaulted with the standard ports.";

				return (DialogResult.Abort);
			}

			if (this.retrieveCaptions && !this.cancel)
			{
				OnStatus1Changed(new EventArgs<string>("Retrieving captions from system..."));

				try
				{
					this.ports.RetrieveCaptions();
				}
				catch (Exception ex)
				{
					this.exception = ex;
					this.exceptionLead = "There was an error while retrieving the port captions from the system!";
					this.exceptionHint = "If the issue cannot be solved, tell YAT to no longer retrieve ports captions by going to 'File > Preferences...' and disable 'retrieve port captions from system'.";

					return (DialogResult.Abort);
				}
			}

			if (this.detectPortsInUse && !this.cancel)
			{
				OnStatus1Changed(new EventArgs<string>("Detecting ports that are in use..."));

				try
				{
					this.ports.DetectPortsInUse(ports_DetectPortsInUseCallback);
				}
				catch (Exception ex)
				{
					this.exception = ex;
					this.exceptionLead = "There was an error while trying to detect the ports that are in use!";
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

		private void ports_DetectPortsInUseCallback(object sender, SerialPortChangedAndCancelEventArgs e)
		{
			OnStatus2Changed(new EventArgs<string>("Scanning " + e.Port + "..."));

			lock (this.cancelSyncObj)
				e.Cancel = this.cancel;
		}

		/// <summary>
		/// Invokes the <see cref="Status1Changed"/> event.
		/// </summary>
		protected virtual void OnStatus1Changed(EventArgs<string> e)
		{
			EventHelper.FireSync<EventArgs<string>>(Status1Changed, this, e);
		}

		/// <summary>
		/// Invokes the <see cref="Status2Changed"/> event.
		/// </summary>
		protected virtual void OnStatus2Changed(EventArgs<string> e)
		{
			EventHelper.FireSync<EventArgs<string>>(Status2Changed, this, e);
		}

		/// <summary>
		/// Invokes the <see cref="IsDone"/> event.
		/// </summary>
		protected virtual void OnIsDone(EventArgs<DialogResult> e)
		{
			EventHelper.FireAsync<EventArgs<DialogResult>>(IsDone, this, e); // Fire async! Worker thread termination must not delayed by sync callbacks!
		}
	}
}

//==================================================================================================
// End of
// $URL: https://svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/YAT.View.Controls/ViewModel/MonitorRenderer.cs $
//==================================================================================================
