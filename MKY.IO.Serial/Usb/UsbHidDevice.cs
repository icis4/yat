//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

using MKY.Utilities.Event;

#endregion

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\USB for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class UsbHidDevice : IIOProvider, IDisposable
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Reset,
			Connected,
			Disconnected,
			Opened,
			WaitingForReopen,
			Error,
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private State state = State.Reset;
		private object stateSyncObj = new object();

		private UsbHidDeviceSettings settings;
		private Usb.SerialHidDevice device;
		private object deviceSyncObj = new object();

		private System.Timers.Timer reopenTimer;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IORequestEventArgs> IORequest;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler DataReceived;

		/// <summary></summary>
		public event EventHandler DataSent;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public UsbHidDevice(UsbHidDeviceSettings settings)
		{
			this.settings = settings;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					StopAndDisposeReopenTimer();
					CloseAndDisposeDevice();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~UsbHidDevice()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual UsbHidDeviceSettings Settings
		{
			get
			{
				AssertNotDisposed();
				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (this.state)
				{
					case State.Connected:
					case State.Disconnected:
					case State.Opened:
					case State.WaitingForReopen:
					{
						return (true);
					}
					default:
					{
						return (false);
					}
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();

				if (this.device != null)
					return (this.device.IsConnected);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				if (this.device != null)
					return (this.device.IsOpen);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				if (this.device != null)
					return (this.device.BytesAvailable);

				return (0);
			}
		}

		private bool AutoReopenEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						this.settings.AutoReopen.Enabled
					);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.device);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool Start()
		{
			// AssertNotDisposed() is called by IsStarted

			if (!IsStarted)
				return (TryCreateAndOpenDevice());

			return (true);
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by IsStarted

			if (IsStarted)
				CloseAndDisposeDevice();
		}

		/// <summary></summary>
		public virtual int Receive(out byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen
			// OnDataReceived has been fired before

			int bytesReceived = 0;
			if (IsOpen)
			{
				lock (this.deviceSyncObj)
					bytesReceived = this.device.Receive(out data);
			}
			else
			{
				data = new byte[] { };
			}
			return (bytesReceived);
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen

			if (IsOpen)
			{
				lock (this.deviceSyncObj)
					this.device.Send(data);
			}

			// OnDataSent will be fired by Usb.HidDevice
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private void SetStateAndNotify(State state)
		{
#if (DEBUG)
			State oldState = this.state;
#endif
			lock (this.stateSyncObj)
				this.state = state;
#if (DEBUG)
			System.Diagnostics.Debug.WriteLine(GetType() + " (" + ToShortString() + ")(" + this.state + "): State has changed from " + oldState + " to " + this.state + ".");
#endif
			OnIOChanged(new EventArgs());
		}

		#endregion

		#region Simple Device Methods
		//==========================================================================================
		// Simple Device Methods
		//==========================================================================================

		private void CreateDevice()
		{
			if (this.device != null)
				CloseAndDisposeDevice();

			lock (this.deviceSyncObj)
			{
				this.device = new Usb.SerialHidDevice(this.settings.DeviceInfo);
				this.device.Connected    += new EventHandler(this.device_Connected);
				this.device.Disconnected += new EventHandler(this.device_Disconnected);
				this.device.DataReceived += new EventHandler(this.device_DataReceived);
				this.device.DataSent     += new EventHandler(this.device_DataSent);
				this.device.Error        += new EventHandler<Usb.ErrorEventArgs>(this.device_Error);
			}
		}

		private void OpenDevice()
		{
			if (!this.device.IsOpen)
			{
				lock (this.deviceSyncObj)
					this.device.Open();
			}
		}

		private void CloseAndDisposeDevice()
		{
			if (this.device != null)
			{
				try
				{
					lock (this.deviceSyncObj)
					{
						if (this.device.IsOpen)
							this.device.Close();

						this.device.Dispose();
						this.device = null;
					}
				}
				catch { }
			}
		}

		#endregion

		#region Device Methods
		//==========================================================================================
		// Device Methods
		//==========================================================================================

		/// <summary></summary>
		private bool TryCreateAndOpenDevice()
		{
			try
			{
				CreateDevice();
				if (IsConnected)
				{
					SetStateAndNotify(State.Connected);

					OpenDevice();
					if (IsOpen)
					{
						SetStateAndNotify(State.Opened);
					}
				}
				return (true);
			}
			catch
			{
				CloseAndDisposeDevice();
				return (false);
			}
		}

		/// <summary></summary>
		private void ResetDevice()
		{
			StopAndDisposeReopenTimer();
			CloseAndDisposeDevice();
			SetStateAndNotify(State.Reset);
		}

		/// <summary></summary>
		private void ResetDeviceAndStartReopenTimer()
		{
			ResetDevice();
			StartReopenTimer();
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		private void device_Connected(object sender, EventArgs e)
		{
			SetStateAndNotify(State.Connected);
		}

		private void device_Disconnected(object sender, EventArgs e)
		{
			SetStateAndNotify(State.Disconnected);
		}

		private void device_DataReceived(object sender, EventArgs e)
		{
			OnDataReceived(e);
		}

		private void device_DataSent(object sender, EventArgs e)
		{
			OnDataSent(e);
		}

		private void device_Error(object sender, Usb.ErrorEventArgs e)
		{
			OnIOError(new IOErrorEventArgs(e.Message));
		}

		#endregion

		#region Reopen Timer
		//==========================================================================================
		// Reopen Timer
		//==========================================================================================

		private void StartReopenTimer()
		{
			if (this.reopenTimer == null)
			{
				this.reopenTimer = new System.Timers.Timer(this.settings.AutoReopen.Interval);
				this.reopenTimer.AutoReset = false;
				this.reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.reopenTimer_Elapsed);
			}
			this.reopenTimer.Start();
		}

		private void StopAndDisposeReopenTimer()
		{
			if (this.reopenTimer != null)
			{
				this.reopenTimer.Stop();
				this.reopenTimer.Dispose();
				this.reopenTimer = null;
			}
		}

		private void reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReopenEnabledAndAllowed)
			{
				try
				{
					TryCreateAndOpenDevice();
				}
				catch
				{
					StartReopenTimer();
				}
			}
			else
			{
				StopAndDisposeReopenTimer();
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			EventHelper.FireSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			Utilities.Unused.PreventCompilerWarning(IOControlChanged);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnDataReceived(EventArgs e)
		{
			EventHelper.FireSync(DataReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDataSent(EventArgs e)
		{
			EventHelper.FireSync(DataSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			Utilities.Unused.PreventCompilerWarning(IORequest);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			if (this.device != null)
				return (this.device.ToString());
			else
				return ("<Undefined>");
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			if (this.device != null)
				return (this.device.ToShortString());
			else
				return ("<Undefined>");
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
