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
using System.Text;
using System.Threading;
using System.ComponentModel;

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

		private bool _isDisposed;

		private State _state = State.Reset;
		private object _stateSyncObj = new object();

		private UsbHidDeviceSettings _settings;
		private Usb.SerialHidDevice _device;
		private object _deviceSyncObj = new object();

		private System.Timers.Timer _reopenTimer;

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
			_settings = settings;
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
			if (!_isDisposed)
			{
				if (disposing)
				{
					StopAndDisposeReopenTimer();
					CloseAndDisposeDevice();
				}
				_isDisposed = true;
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
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
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
				return (_settings);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
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

				if (_device != null)
					return (_device.IsConnected);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();

				if (_device != null)
					return (_device.IsOpen);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				if (_device != null)
					return (_device.BytesAvailable);

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
						_settings.AutoReopen.Enabled
					);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_device);
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
				lock (_deviceSyncObj)
					bytesReceived = _device.Receive(out data);
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
				lock (_deviceSyncObj)
					_device.Send(data);
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
			State oldState = _state;
#endif
			lock (_stateSyncObj)
				_state = state;
#if (DEBUG)
			System.Diagnostics.Debug.WriteLine(GetType() + " (" + ToShortString() + ")(" + _state + "): State has changed from " + oldState + " to " + _state + ".");
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
			if (_device != null)
				CloseAndDisposeDevice();

			lock (_deviceSyncObj)
			{
				_device = new Usb.SerialHidDevice(_settings.DeviceInfo);
				_device.Connected    += new EventHandler(_device_Connected);
				_device.Disconnected += new EventHandler(_device_Disconnected);
				_device.DataReceived += new EventHandler(_device_DataReceived);
				_device.DataSent     += new EventHandler(_device_DataSent);
				_device.Error        += new EventHandler<Usb.ErrorEventArgs>(_device_Error);
			}
		}

		private void OpenDevice()
		{
			if (!_device.IsOpen)
			{
				lock (_deviceSyncObj)
					_device.Open();
			}
		}

		private void CloseAndDisposeDevice()
		{
			if (_device != null)
			{
				try
				{
					lock (_deviceSyncObj)
					{
						if (_device.IsOpen)
							_device.Close();

						_device.Dispose();
						_device = null;
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

		private void _device_Connected(object sender, EventArgs e)
		{
			SetStateAndNotify(State.Connected);
		}

		private void _device_Disconnected(object sender, EventArgs e)
		{
			SetStateAndNotify(State.Disconnected);
		}

		private void _device_DataReceived(object sender, EventArgs e)
		{
			OnDataReceived(e);
		}

		private void _device_DataSent(object sender, EventArgs e)
		{
			OnDataSent(e);
		}

		private void _device_Error(object sender, Usb.ErrorEventArgs e)
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
			if (_reopenTimer == null)
			{
				_reopenTimer = new System.Timers.Timer(_settings.AutoReopen.Interval);
				_reopenTimer.AutoReset = false;
				_reopenTimer.Elapsed += new System.Timers.ElapsedEventHandler(_reopenTimer_Elapsed);
			}
			_reopenTimer.Start();
		}

		private void StopAndDisposeReopenTimer()
		{
			if (_reopenTimer != null)
			{
				_reopenTimer.Stop();
				_reopenTimer.Dispose();
				_reopenTimer = null;
			}
		}

		private void _reopenTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
			if (_device != null)
				return (_device.ToString());
			else
				return ("<Undefined>");
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			if (_device != null)
				return (_device.ToShortString());
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
