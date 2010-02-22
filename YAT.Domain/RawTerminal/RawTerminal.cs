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

using System;
using System.Collections.Generic;
using System.Text;

using MKY.Utilities.Event;
using MKY.IO.Serial;

using YAT.Domain;

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

		private bool _isDisposed;

		private Settings.BufferSettings _bufferSettings;
		private RawRepository _txRepository;
		private RawRepository _bidirRepository;
		private RawRepository _rxRepository;

		private Settings.IOSettings _ioSettings;
		private IIOProvider _io;

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
			_txRepository    = new RawRepository(bufferSettings.TxBufferSize);
			_bidirRepository = new RawRepository(bufferSettings.BidirBufferSize);
			_rxRepository    = new RawRepository(bufferSettings.RxBufferSize);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(IOFactory.CreateIO(ioSettings)); // settings are applied by factory
		}

		/// <summary></summary>
		public RawTerminal(Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings, RawTerminal rhs)
		{
			_txRepository    = new RawRepository(rhs._txRepository);
			_bidirRepository = new RawRepository(rhs._bidirRepository);
			_rxRepository    = new RawRepository(rhs._rxRepository);

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
			if (!_isDisposed)
			{
				if (disposing)
				{
					if (_io != null)
						_io.Dispose();
				}
				_isDisposed = true;
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
		public Settings.BufferSettings BufferSettings
		{
			get
			{
				AssertNotDisposed();
				return (_bufferSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachBufferSettings(value);
				ApplyBufferSettings();
			}
		}

		/// <summary></summary>
		public Settings.IOSettings IOSettings
		{
			get
			{
				AssertNotDisposed();
				return (_ioSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachIOSettings(value);
				ApplyIOSettings();
			}
		}

		/// <summary></summary>
		public bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (_io.IsStarted);
			}
		}

		/// <summary></summary>
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_io.IsConnected);
			}
		}

		/// <summary></summary>
		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_io.IsOpen);
			}
		}

		/// <summary></summary>
		public IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_io);
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_io.UnderlyingIOInstance);
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
		public void Start()
		{
			AssertNotDisposed();
			_io.Start();
		}

		/// <summary></summary>
		public void Stop()
		{
			AssertNotDisposed();
			_io.Stop();
		}

		//------------------------------------------------------------------------------------------
		// Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Send(byte[] data)
		{
			AssertNotDisposed();
			
			_io.Send(data);

			RawElement re = new RawElement(data, SerialDirection.Tx);
			_txRepository.Enqueue(re);
			_bidirRepository.Enqueue(re);
			OnRawElementSent(new RawElementEventArgs(re));
		}

		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public List<RawElement> RepositoryToElements(RepositoryType repository)
		{
			AssertNotDisposed();
			
			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.ToElements());
				case RepositoryType.Bidir: return (_bidirRepository.ToElements());
				case RepositoryType.Rx:    return (_rxRepository.ToElements());
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		/// <summary></summary>
		public void ClearRepository(RepositoryType repository)
		{
			AssertNotDisposed();
			
			switch (repository)
			{
				case RepositoryType.Tx:    _txRepository.Clear();    break;
				case RepositoryType.Bidir: _bidirRepository.Clear(); break;
				case RepositoryType.Rx:    _rxRepository.Clear();    break;
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
			OnRepositoryCleared(new RepositoryEventArgs(repository));
		}

		//------------------------------------------------------------------------------------------
		// ToString
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		new public string ToString()
		{
			AssertNotDisposed();

			return (ToString(""));
		}

		/// <summary></summary>
		public string ToString(string indent)
		{
			AssertNotDisposed();

			return (indent + "- IOSettings: " + _ioSettings + Environment.NewLine +
					indent + "- TxRepository: " + Environment.NewLine + _txRepository.ToString(indent + "- ") +
					indent + "- BidirRepository: " + Environment.NewLine + _bidirRepository.ToString(indent + "- ") +
					indent + "- RxRepository: " + Environment.NewLine + _rxRepository.ToString(indent + "- "));
		}

		/// <summary></summary>
		public string RepositoryToString(RepositoryType repository, string indent)
		{
			AssertNotDisposed();
			
			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.ToString(indent));
				case RepositoryType.Bidir: return (_bidirRepository.ToString(indent));
				case RepositoryType.Rx:    return (_rxRepository.ToString(indent));
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachBufferSettings(Settings.BufferSettings bufferSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(_bufferSettings, bufferSettings))
				return;

			if (_bufferSettings != null)
				DetachBufferSettings();

			_bufferSettings = bufferSettings;
			_bufferSettings.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_bufferSettings_Changed);
		}

		private void DetachBufferSettings()
		{
			_bufferSettings.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_bufferSettings_Changed);
			_bufferSettings = null;
		}

		private void ApplyBufferSettings()
		{
			_txRepository.Capacity    = _bufferSettings.TxBufferSize;
			_bidirRepository.Capacity = _bufferSettings.BidirBufferSize;
			_rxRepository.Capacity    = _bufferSettings.RxBufferSize;
		}

		private void AttachIOSettings(Settings.IOSettings ioSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(_ioSettings, ioSettings))
				return;

			if (_ioSettings != null)
				DetachIOSettings();

			_ioSettings = ioSettings;
			_ioSettings.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_ioSettings_Changed);
		}

		private void DetachIOSettings()
		{
			_ioSettings.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_ioSettings_Changed);
			_ioSettings = null;
		}

		private void ApplyIOSettings()
		{
			// Nothing to do
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================

		private void _bufferSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
		{
			ApplyBufferSettings();
		}

		private void _ioSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
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
			if (IIOProvider.ReferenceEquals(_io, io))
				return;

			if (_io != null)
				DetachIO();

			_io = io;
			_io.IOChanged        += new EventHandler(_io_IOChanged);
			_io.IOControlChanged += new EventHandler(_io_IOControlChanged);
			_io.IORequest        += new EventHandler<MKY.IO.Serial.IORequestEventArgs>(_io_IORequest);
			_io.IOError          += new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(_io_IOError);
			_io.DataReceived     += new EventHandler(_io_DataReceived);
		}

		private void DetachIO()
		{
			_io.IOChanged        -= new EventHandler(_io_IOChanged);
			_io.IOControlChanged -= new EventHandler(_io_IOControlChanged);
			_io.IORequest        -= new EventHandler<MKY.IO.Serial.IORequestEventArgs>(_io_IORequest);
			_io.IOError          -= new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(_io_IOError);
			_io.DataReceived     -= new EventHandler(_io_DataReceived);
			_io = null;
		}

		#endregion

		#region IO Events
		//==========================================================================================
		// IO Events
		//==========================================================================================

		private void _io_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(new EventArgs());
		}

		private void _io_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(new EventArgs());
		}

		private void _io_IORequest(object sender, MKY.IO.Serial.IORequestEventArgs e)
		{
			OnIORequest(new IORequestEventArgs((IORequest)e.Request));
		}

		private void _io_IOError(object sender, MKY.IO.Serial.IOErrorEventArgs e)
		{
			SerialPortIOErrorEventArgs serialPortErrorEventArgs = (e as SerialPortIOErrorEventArgs);
			if (serialPortErrorEventArgs == null)
				OnIOError(new IOErrorEventArgs(e.Message));
			else
				OnIOError(new SerialPortErrorEventArgs(serialPortErrorEventArgs.Message, serialPortErrorEventArgs.SerialPortError));
		}

		private void _io_DataReceived(object sender, EventArgs e)
		{
			byte[] data;
			if (_io.Receive(out data) > 0)
			{
				RawElement re = new RawElement(data, SerialDirection.Rx);
				_rxRepository.Enqueue(re);
				_bidirRepository.Enqueue(re);
				OnRawElementReceived(new RawElementEventArgs(re));
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
