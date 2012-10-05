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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY.Event;
using MKY.IO.Serial;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines a buffered serial interface. The buffers contain raw byte data,
	/// no formatting is done.
	/// </summary>
	public class RawTerminal : IDisposable
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.BufferSettings bufferSettings;

		private RawRepository txRepository;
		private RawRepository bidirRepository;
		private RawRepository rxRepository;
		private object repositorySyncObj = new object();

		private Settings.IOSettings ioSettings;
		private IIOProvider io;

		private Thread sendThread;
		private AutoResetEvent sendThreadEvent;
		private bool sendThreadSyncFlag;

		private Thread receiveThread;
		private AutoResetEvent receiveThreadEvent;
		private bool receiveThreadSyncFlag;

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
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<RawElementEventArgs> RawElementSent;

		/// <summary></summary>
		public event EventHandler<RawElementEventArgs> RawElementReceived;

		/// <summary></summary>
		public event EventHandler<RepositoryEventArgs> RepositoryCleared;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RawTerminal(Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings)
		{
			this.txRepository    = new RawRepository(bufferSettings.TxBufferSize);
			this.bidirRepository = new RawRepository(bufferSettings.BidirBufferSize);
			this.rxRepository    = new RawRepository(bufferSettings.RxBufferSize);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(IOFactory.CreateIO(ioSettings)); // settings are applied by factory
		}

		/// <summary></summary>
		public RawTerminal(Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings, RawTerminal rhs)
		{
			this.txRepository    = new RawRepository(rhs.txRepository);
			this.bidirRepository = new RawRepository(rhs.bidirRepository);
			this.rxRepository    = new RawRepository(rhs.rxRepository);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(IOFactory.CreateIO(ioSettings)); // settings are applied by factory
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
					Stop();

					if (this.io != null)
					{
						this.io.Dispose();
						this.io = null;
					}
				}
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~RawTerminal()
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
		public virtual Settings.BufferSettings BufferSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.bufferSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachBufferSettings(value);
				ApplyBufferSettings();
			}
		}

		/// <summary></summary>
		public virtual Settings.IOSettings IOSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.ioSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachIOSettings(value);
				ApplyIOSettings();
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();
				return (this.io.IsStopped);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (this.io.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (this.io.IsOpen);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (this.io.IsConnected);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				AssertNotDisposed();
				return (this.io.IsReadyToSend);
			}
		}

		/// <summary></summary>
		public virtual IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (this.io);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.io.UnderlyingIOInstance);
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		//------------------------------------------------------------------------------------------
		// Start/Stop
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			StartThreads();
			return (this.io.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			RequestStopThreads();
			this.io.Stop();
		}

		//------------------------------------------------------------------------------------------
		// Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsReadyToSend)
				this.io.Send(data);
		}

		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual List<RawElement> RepositoryToElements(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			List<RawElement> l = null;
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    l = this.txRepository.ToElements();    break;
					case RepositoryType.Bidir: l = this.bidirRepository.ToElements(); break;
					case RepositoryType.Rx:    l = this.rxRepository.ToElements();    break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
			}
			return (l);
		}

		/// <remarks>
		/// \todo:
		/// Currently, all respositories are cleared in any case. That is because repositories are
		/// always reloaded from bidir. Without clearing all, contents reappear after a change to
		/// the settings, e.g. switching radix. Issue is described in bug #3111508.
		/// </remarks>
		public virtual void ClearRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				/* \todo:
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository.Clear();    break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository.Clear();    break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
				OnRepositoryCleared(new RepositoryEventArgs(repositoryType));*/

				switch (repositoryType)
				{
					case RepositoryType.Tx:
					case RepositoryType.Bidir:
					case RepositoryType.Rx:
					{
						this.txRepository.Clear();
						this.bidirRepository.Clear();
						this.rxRepository.Clear();

						OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Tx));
						OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Bidir));
						OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Rx));

						break;
					}
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
			} // lock (this.repositorySyncObj)
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repositoryType, string indent)
		{
			AssertNotDisposed();

			string s = null;
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    s = this.txRepository.ToString(indent);    break;
					case RepositoryType.Bidir: s = this.bidirRepository.ToString(indent); break;
					case RepositoryType.Rx:    s = this.rxRepository.ToString(indent);    break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
			}
			return (s);
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachBufferSettings(Settings.BufferSettings bufferSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(this.bufferSettings, bufferSettings))
				return;

			if (this.bufferSettings != null)
				DetachBufferSettings();

			this.bufferSettings = bufferSettings;
			this.bufferSettings.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(bufferSettings_Changed);
		}

		private void DetachBufferSettings()
		{
			this.bufferSettings.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(bufferSettings_Changed);
			this.bufferSettings = null;
		}

		private void ApplyBufferSettings()
		{
			lock (this.repositorySyncObj)
			{
				this.txRepository.Capacity    = this.bufferSettings.TxBufferSize;
				this.bidirRepository.Capacity = this.bufferSettings.BidirBufferSize;
				this.rxRepository.Capacity    = this.bufferSettings.RxBufferSize;
			}
		}

		private void AttachIOSettings(Settings.IOSettings ioSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(this.ioSettings, ioSettings))
				return;

			if (this.ioSettings != null)
				DetachIOSettings();

			this.ioSettings = ioSettings;
			this.ioSettings.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(ioSettings_Changed);
		}

		private void DetachIOSettings()
		{
			this.ioSettings.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(ioSettings_Changed);
			this.ioSettings = null;
		}

		private void ApplyIOSettings()
		{
			// Nothing to do.
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================

		private void bufferSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyBufferSettings();
		}

		private void ioSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyIOSettings();
		}

		#endregion

		#region I/O
		//==========================================================================================
		// I/O
		//==========================================================================================

		/// <remarks>
		/// The DataSent event is not used as it doesn't tell what data was sent, and is therefore
		/// of little use.
		/// </remarks>
		private void AttachIO(IIOProvider io)
		{
			if (IIOProvider.ReferenceEquals(this.io, io))
				return;

			if (this.io != null)
				DetachIO();

			this.io = io;
			this.io.IOChanged        += new EventHandler(io_IOChanged);
			this.io.IOControlChanged += new EventHandler(io_IOControlChanged);
			this.io.DataReceived     += new EventHandler(io_DataReceived);
			this.io.IOError          += new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
		}

		/// <remarks>
		/// The DataSent event is not used as it doesn't tell what data was sent, and is therefore
		/// of little use.
		/// </remarks>
		private void DetachIO()
		{
			this.io.IOChanged        -= new EventHandler(io_IOChanged);
			this.io.IOControlChanged -= new EventHandler(io_IOControlChanged);
			this.io.DataReceived     -= new EventHandler(io_DataReceived);
			this.io.IOError          -= new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
			this.io = null;
		}

		#endregion

		#region I/O Events
		//==========================================================================================
		// I/O Events
		//==========================================================================================

		private void io_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(new EventArgs());
		}

		private void io_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(new EventArgs());
		}

		/// <remarks>
		/// The DataSent event is not used as it doesn't tell what data was sent, and is therefore
		/// of little use.
		/// </remarks>
		private void io_DataReceived(object sender, EventArgs e)
		{
			byte[] data;
			if (this.io.Receive(out data) > 0)
			{
				RawElement re = new RawElement(data, SerialDirection.Rx);
				this.rxRepository.Enqueue(re);
				this.bidirRepository.Enqueue(re);
				OnRawElementReceived(new RawElementEventArgs(re));
			}
		}

		private void io_IOError(object sender, MKY.IO.Serial.IOErrorEventArgs e)
		{
			SerialPortIOErrorEventArgs serialPortErrorEventArgs = (e as SerialPortIOErrorEventArgs);
			if (serialPortErrorEventArgs == null)
				OnIOError(new IOErrorEventArgs((IOErrorSeverity)e.Severity, (IODirection)e.Direction, e.Message));
			else
				OnIOError(new SerialPortErrorEventArgs(serialPortErrorEventArgs.Message, serialPortErrorEventArgs.SerialPortError));
		}

		#endregion

		#region I/O Threads
		//==========================================================================================
		// I/O Threads
		//==========================================================================================

		private void StartThreads()
		{
			// Ensure that threads have stopped after the last stop request.
			while ((this.receiveThread != null) && (this.sendThread != null))
				Thread.Sleep(1);

			this.receiveThreadSyncFlag = true;
			this.receiveThreadEvent = new AutoResetEvent(false);
			this.receiveThread = new Thread(new ThreadStart(ReceiveThread));
			this.receiveThread.Start();

			this.sendThreadSyncFlag = true;
			this.sendThreadEvent = new AutoResetEvent(false);
			this.sendThread = new Thread(new ThreadStart(SendEventThread));
			this.sendThread.Start();
		}

		/// <remarks>
		/// Just signal the threads, they will stop soon. Do not wait (i.e. Join()) them, this
		/// method could have been called from a thread that also has to handle the receive
		/// events (e.g. the application main thread). Waiting here would lead to deadlocks.
		/// </remarks>
		private void RequestStopThreads()
		{
			this.receiveThreadSyncFlag = false;
			this.receiveThreadEvent.Set();

			this.sendThreadSyncFlag = false;
			this.sendThreadEvent.Set();
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
			EventHelper.FireSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			EventHelper.FireSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawElementSent(RawElementEventArgs e)
		{
			EventHelper.FireSync<RawElementEventArgs>(RawElementSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawElementReceived(RawElementEventArgs e)
		{
			EventHelper.FireSync<RawElementEventArgs>(RawElementReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();

			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			AssertNotDisposed();

			StringBuilder sb = new StringBuilder();
			lock (this.repositorySyncObj)
			{
				sb.AppendLine(indent + "- TxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "--")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "- BidirRepository: ");
				sb.Append(this.bidirRepository.ToString(indent + "--")); // Repository will add 'NewLine'.
				
				sb.AppendLine(indent + "- RxRepository: ");
				sb.Append(this.rxRepository.ToString(indent + "--")); // Repository will add 'NewLine'.
				
				sb.Append(indent + "- I/O: " + this.io.ToString());
			}
			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToIOString()
		{
			if (this.io != null)
				return (this.io.ToString());
			else
				return (Undefined);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
