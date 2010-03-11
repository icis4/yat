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

		private enum PortState
		{
			Connecting,
			Connected,
			Disconnecting,
			Disconnected,
			WaitingForReconnect,
			Error,
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private Usb.DeviceInfo _deviceId;
		private AutoRetry _autoReconnect;

		private PortState _state = PortState.Disconnected;
		private object _stateSyncObj = new object();

		private Usb.HidDevice _port;
		private object _portSyncObj = new object();

		private Queue<byte> _receiveQueue = new Queue<byte>();

		private System.Timers.Timer _reconnectTimer;

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
		public UsbHidDevice(Usb.DeviceInfo deviceId)
		{
			Initialize(deviceId, new AutoRetry());
		}

		/// <summary></summary>
		public UsbHidDevice(Usb.DeviceInfo deviceId, AutoRetry autoReconnect)
		{
			Initialize(deviceId, autoReconnect);
		}

		private void Initialize(Usb.DeviceInfo deviceId, AutoRetry autoReconnect)
		{
			_deviceId = deviceId;
			_autoReconnect = autoReconnect;
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
					if (_port != null)
					{
						_port.Dispose();
						_port = null;
					}
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
		public Usb.DeviceInfo DeviceId
		{
			get
			{
				AssertNotDisposed();
				return (_deviceId);
			}
		}

		/// <summary></summary>
		public AutoRetry AutoReconnect
		{
			get
			{
				AssertNotDisposed();
				return (_autoReconnect);
			}
		}

		/// <summary></summary>
		public bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				switch (_state)
				{
					case PortState.Connecting:
					case PortState.Connected:
					case PortState.WaitingForReconnect:
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
		public bool IsOpen
		{
			get { return (IsConnected); }
		}

		/// <summary></summary>
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_port != null);
			}
		}

		/// <summary></summary>
		public int BytesAvailable
		{
			get
			{
				AssertNotDisposed();

				int bytesAvailable = 0;
				lock (_receiveQueue)
				{
					bytesAvailable = _receiveQueue.Count;
				}
				return (bytesAvailable);
			}
		}

		private bool AutoReconnectEnabledAndAllowed
		{
			get
			{
				return
					(
						!IsDisposed && IsStarted && !IsOpen &&
						_autoReconnect.Enabled
					);
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_port);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		public void Start()
		{
			// AssertNotDisposed() is called by IsStarted

			if (!IsStarted)
				StartConnection();
		}

		/// <summary></summary>
		public void Stop()
		{
			// AssertNotDisposed() is called by IsStarted

			if (IsStarted)
				StopConnection();
		}

		/// <summary></summary>
		public int Receive(out byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen
			// OnDataReceived has been fired before

			int bytesReceived = 0;
			if (IsOpen)
			{
				lock (_receiveQueue)
				{
					bytesReceived = _receiveQueue.Count;
					data = new byte[bytesReceived];
					for (int i = 0; i < bytesReceived; i++)
						data[i] = _receiveQueue.Dequeue();
				}
			}
			else
			{
				data = new byte[] { };
			}
			return (bytesReceived);
		}

		/// <summary></summary>
		public void Send(byte[] data)
		{
			// AssertNotDisposed() is called by IsOpen

			if (IsOpen)
			{
				/*lock (_portSyncObj)
					_port.SpecifiedDevice.SendData(data);*/
			}

			// OnDataSent will be fired by UsbLibrary.UsbHidPort
		}

		#endregion

		#region State Methods
		//==========================================================================================
		// State Methods
		//==========================================================================================

		private void SetStateAndNotify(PortState state)
		{
#if (DEBUG)
			PortState oldState = _state;
#endif
			lock (_stateSyncObj)
				_state = state;
#if (DEBUG)
			System.Diagnostics.Debug.WriteLine(GetType() + " (" + ToShortPortString() + ")(" + _state + "): State has changed from " + oldState + " to " + _state);
#endif
			OnIOChanged(new EventArgs());
		}

		#endregion

		#region Simple Port Methods
		//==========================================================================================
		// Simple Port Methods
		//==========================================================================================

		private void DisposePort()
		{
			if (_port != null)
			{
				lock (_portSyncObj)
				{
					_port.Dispose();
					_port = null;
				}
			}
		}

		#endregion

		#region Connection Methods
		//==========================================================================================
		// Connection Methods
		//==========================================================================================

		/// <summary></summary>
		private void StartConnection()
		{
			if (_port != null)
				DisposePort();

			SetStateAndNotify(PortState.Connecting);

			lock (_portSyncObj)
			{
				/* _port = new UsbLibrary.UsbHidPort();
				_port.OnDeviceArrived += new EventHandler(_port_OnDeviceArrived);
				_port.OnDeviceRemoved += new EventHandler(_port_OnDeviceRemoved);
				_port.OnSpecifiedDeviceArrived += new EventHandler(_port_OnSpecifiedDeviceArrived);
				_port.OnSpecifiedDeviceRemoved += new EventHandler(_port_OnSpecifiedDeviceRemoved);
				_port.OnDataRecieved += new UsbLibrary.DataRecievedEventHandler(_port_OnDataRecieved);
				_port.OnDataSend += new EventHandler(_port_OnDataSend);

				_port.VendorId = _deviceId.VendorId;
				_port.ProductId = _deviceId.ProductId; */
			}
		}

		private void StopConnection()
		{
			SetStateAndNotify(PortState.Disconnecting);
			DisposePort();
		}

		#endregion

		#region Port Events
		//==========================================================================================
		// Port Events
		//==========================================================================================

		private void _port_OnDeviceArrived(object sender, EventArgs e)
		{
			// Do nothing, dummy event handler required, details see above
		}

		private void _port_OnDeviceRemoved(object sender, EventArgs e)
		{
			// Do nothing, dummy event handler required, details see above
		}

		private void _port_OnSpecifiedDeviceArrived(object sender, EventArgs e)
		{
			SetStateAndNotify(PortState.Connected);
		}

		private void _port_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
		{
			SetStateAndNotify(PortState.Disconnected);
		}

		/// <summary>
		/// Asynchronously invoke incoming events to prevent potential dead-locks if close/dispose
		/// was called from a ISynchronizeInvoke target (i.e. a form) on an event thread.
		/// </summary>
		/* private delegate void _port_DataReceivedDelegate(object sender, UsbLibrary.DataRecievedEventArgs e);
		private object _port_DataReceivedSyncObj = new object();

		private void _port_OnDataRecieved(object sender, UsbLibrary.DataRecievedEventArgs e)
		{
			if (_state == PortState.Connected) // Ensure not to forward any events during closing anymore
			{
				// Immediately read data on this thread
				lock (_receiveQueue)
				{
					foreach (byte b in e.data)
						_receiveQueue.Enqueue(b);
				}

				// Ensure that only one data received event thread is active at the same time.
				// Without this exclusivity, two receive threads could create a race condition.
				if (Monitor.TryEnter(_port_DataReceivedSyncObj))
				{
					try
					{
						_port_DataReceivedDelegate asyncInvoker = new _port_DataReceivedDelegate(_port_DataReceivedAsync);
						asyncInvoker.BeginInvoke(sender, e, null, null);
					}
					finally
					{
						Monitor.Exit(_port_DataReceivedSyncObj);
					}
				}
			}
		} */

		/* private void _port_DataReceivedAsync(object sender, UsbLibrary.DataRecievedEventArgs e)
		{
			// Ensure that only one data received event thread is active at the same time.
			// Without this exclusivity, two receive threads could create a race condition.
			Monitor.Enter(_port_DataReceivedSyncObj);
			try
			{
				// Fire events until there is no more data available. Must be done to ensure
				// that events are fired even for data that was enqueued above while the sync
				// obj was busy.
				while (BytesAvailable > 0)
					OnDataReceived(new EventArgs());
			}
			finally
			{
				Monitor.Exit(_port_DataReceivedSyncObj);
			}
		} */

		private void _port_OnDataSend(object sender, EventArgs e)
		{
			OnDataSent(new EventArgs());
		}

		#endregion

		#region Reopen Timer
		//==========================================================================================
		// Reopen Timer
		//==========================================================================================

		private void StartReconnectTimer()
		{
			if (_reconnectTimer == null)
			{
				_reconnectTimer = new System.Timers.Timer(_autoReconnect.Interval);
				_reconnectTimer.AutoReset = false;
				_reconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(_reconnectTimer_Elapsed);
			}
			_reconnectTimer.Start();
		}

		private void StopAndDisposeReconnectTimer()
		{
			if (_reconnectTimer != null)
			{
				_reconnectTimer.Stop();
				_reconnectTimer.Dispose();
				_reconnectTimer = null;
			}
		}

		private void _reconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (AutoReconnectEnabledAndAllowed)
			{
				try
				{
					StartConnection();
				}
				catch
				{
					StartReconnectTimer();
				}
			}
			else
			{
				StopAndDisposeReconnectTimer();
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
			MKY.Utilities.Unused.PreventCompilerWarning(IOControlChanged);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			MKY.Utilities.Unused.PreventCompilerWarning(IORequest);
			throw (new NotSupportedException("Event not in use"));
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			MKY.Utilities.Unused.PreventCompilerWarning(IOError);
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

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public string ToShortPortString()
		{
			if (_port != null)
				return (_port.VendorId.ToString("X4") + ":" + _port.ProductId.ToString("X4"));
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
