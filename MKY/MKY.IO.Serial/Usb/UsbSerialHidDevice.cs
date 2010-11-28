//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Event;

#endregion

// The MKY.IO.Serial namespace combines various serial interface infrastructure. This code is
// intentionally placed into the MKY.IO.Serial namespace even though the file is located in
// MKY.IO.Serial\USB for better separation of the implementation files.
namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class UsbSerialHidDevice : IIOProvider, IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private UsbSerialHidDeviceSettings settings;
		private Usb.SerialHidDevice device;
		private object deviceSyncObj = new object();

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
		public UsbSerialHidDevice(UsbSerialHidDeviceSettings settings)
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
					StopAndDisposeDevice();
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~UsbSerialHidDevice()
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
		public virtual UsbSerialHidDeviceSettings Settings
		{
			get
			{
				AssertNotDisposed();
				return (this.settings);
			}
		}

		/// <summary></summary>
		public virtual Usb.DeviceInfo DeviceInfo
		{
			get
			{
				AssertNotDisposed();

				if (this.device != null)
					return (this.device.Info);
				else if (this.settings != null)
					return (this.settings.DeviceInfo);
				else
					return (null);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				return (!IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (this.device != null);
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
		public virtual bool IsReadyToSend
		{
			get { return (IsOpen); }
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
				return (TryCreateAndStartDevice());

			return (true);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			// AssertNotDisposed() is called by IsStarted.

			if (IsStarted)
				StopAndDisposeDevice();
		}

		/// <summary></summary>
		public virtual int Receive(out byte[] data)
		{
			// OnDataReceived has been fired by device before.

			AssertNotDisposed();
			return (this.device.Receive(out data));
		}

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();
			this.device.Send(data);

			// OnDataSent will be fired by device.
		}

		#endregion

		#region Device Methods
		//==========================================================================================
		// Device Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private bool TryCreateAndStartDevice()
		{
			try
			{
				CreateDevice();
				return (StartDevice());
			}
			catch
			{
				StopAndDisposeDevice();
				return (false);
			}
		}

		private void CreateDevice()
		{
			if (this.device != null)
				StopAndDisposeDevice();

			lock (this.deviceSyncObj)
			{
				// Ensure to create device info from VID/PID/SNR since system path is not saved.
				Usb.DeviceInfo di = this.settings.DeviceInfo;
				this.device = new Usb.SerialHidDevice(di.VendorId, di.ProductId, di.SerialNumber);
				this.device.AutoOpen = this.settings.AutoOpen;

				this.device.Connected    += new EventHandler(device_Connected);
				this.device.Disconnected += new EventHandler(device_Disconnected);
				this.device.Opened       += new EventHandler(device_Opened);
				this.device.Closed       += new EventHandler(device_Closed);
				this.device.DataReceived += new EventHandler(device_DataReceived);
				this.device.DataSent     += new EventHandler(device_DataSent);
				this.device.Error        += new EventHandler<Usb.ErrorEventArgs>(device_Error);
			}
		}

		private bool StartDevice()
		{
			if (this.device != null)
			{
				bool success = this.device.Start();
				OnIOChanged(new EventArgs());
				return (success);
			}
			else
			{
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void StopAndDisposeDevice()
		{
			if (this.device != null)
			{
				try
				{
					lock (this.deviceSyncObj)
					{
						this.device.Dispose();
						this.device = null;
					}
					OnIOChanged(new EventArgs());
				}
				catch { }
			}
		}

		#endregion

		#region Device Events
		//==========================================================================================
		// Device Events
		//==========================================================================================

		private void device_Connected(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Disconnected(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Opened(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void device_Closed(object sender, EventArgs e)
		{
			OnIOChanged(e);
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
			UnusedEvent.PreventCompilerWarning(IOControlChanged);
			throw (new NotSupportedException("Event 'IOControlChanged' is not in use for USB Ser/HID devices"));
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
			UnusedEvent.PreventCompilerWarning<IORequestEventArgs>(IORequest);
			throw (new NotSupportedException("Event 'IORequest' is not in use for USB Ser/HID devices"));
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
			if (DeviceInfo != null)
				return (DeviceInfo.ToString());
			else
				return (base.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			if (DeviceInfo != null)
				return (DeviceInfo.ToShortString());
			else
				return (base.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
