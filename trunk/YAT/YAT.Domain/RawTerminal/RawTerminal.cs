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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.Event;
using MKY.IO.Serial;

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
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.BufferSettings bufferSettings;
		private RawRepository txRepository;
		private RawRepository bidirRepository;
		private RawRepository rxRepository;

		private Settings.IOSettings ioSettings;
		private IIOProvider io;

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
					if (this.io != null)
						this.io.Dispose();
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
			return (this.io.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();
			this.io.Stop();
		}

		//------------------------------------------------------------------------------------------
		// Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();
			
			this.io.Send(data);

			RawElement re = new RawElement(data, SerialDirection.Tx);
			this.txRepository.Enqueue(re);
			this.bidirRepository.Enqueue(re);
			OnRawElementSent(new RawElementEventArgs(re));
		}

		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual List<RawElement> RepositoryToElements(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txRepository.ToElements());
				case RepositoryType.Bidir: return (this.bidirRepository.ToElements());
				case RepositoryType.Rx:    return (this.rxRepository.ToElements());
				default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
			}
		}

		/// <remarks>
		/// \todo
		/// Currently, all respositories are cleared in any case. That is because repositories are
		/// always reloaded from bidir. Without clearing all, contents reappear after a change to
		/// the settings, e.g. switching radix. Issue is described in bug #3111508.
		/// </remarks>
		public virtual void ClearRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			/*switch (repositoryType)
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
		}

		//------------------------------------------------------------------------------------------
		// ToString
		//------------------------------------------------------------------------------------------

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

			return (indent + "- IOSettings: " + this.ioSettings + Environment.NewLine +
					indent + "- TxRepository: " + Environment.NewLine + this.txRepository.ToString(indent + "- ") +
					indent + "- BidirRepository: " + Environment.NewLine + this.bidirRepository.ToString(indent + "- ") +
					indent + "- RxRepository: " + Environment.NewLine + this.rxRepository.ToString(indent + "- "));
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repositoryType, string indent)
		{
			AssertNotDisposed();

			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txRepository.ToString(indent));
				case RepositoryType.Bidir: return (this.bidirRepository.ToString(indent));
				case RepositoryType.Rx:    return (this.rxRepository.ToString(indent));
				default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
			}
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
			this.txRepository.Capacity    = this.bufferSettings.TxBufferSize;
			this.bidirRepository.Capacity = this.bufferSettings.BidirBufferSize;
			this.rxRepository.Capacity    = this.bufferSettings.RxBufferSize;
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

		#region IO
		//==========================================================================================
		// IO
		//==========================================================================================

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
			this.io.IORequest        += new EventHandler<MKY.IO.Serial.IORequestEventArgs>(io_IORequest);
			this.io.IOError          += new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
		}

		private void DetachIO()
		{
			this.io.IOChanged        -= new EventHandler(io_IOChanged);
			this.io.IOControlChanged -= new EventHandler(io_IOControlChanged);
			this.io.DataReceived     -= new EventHandler(io_DataReceived);
			this.io.IORequest        -= new EventHandler<MKY.IO.Serial.IORequestEventArgs>(io_IORequest);
			this.io.IOError          -= new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
			this.io = null;
		}

		#endregion

		#region IO Events
		//==========================================================================================
		// IO Events
		//==========================================================================================

		private void io_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(new EventArgs());
		}

		private void io_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(new EventArgs());
		}

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

		private void io_IORequest(object sender, MKY.IO.Serial.IORequestEventArgs e)
		{
			OnIORequest(new IORequestEventArgs((IORequest)e.Request));
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
		protected virtual void OnIORequest(IORequestEventArgs e)
		{
			EventHelper.FireSync<IORequestEventArgs>(IORequest, this, e);
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
