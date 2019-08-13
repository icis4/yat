using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Domain
{
	/// <summary>
	/// Defines a buffered serial interface. The buffers contain raw byte data,
	/// no formatting is done.
	/// </summary>
	public class RawTerminal : IDisposable
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isDisposed = false;

		private Settings.BufferSettings _bufferSettings;
		private RawRepository _txRepository;
		private RawRepository _bidirRepository;
		private RawRepository _rxRepository;

		private Settings.IOSettings _ioSettings;
		private IO.IIOProvider _io;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		public event EventHandler TerminalChanged;
		public event EventHandler TerminalControlChanged;
		public event EventHandler<TerminalErrorEventArgs> TerminalError;

		public event EventHandler<RawElementEventArgs> RawElementSent;
		public event EventHandler<RawElementEventArgs> RawElementReceived;
		public event EventHandler<RepositoryEventArgs> RepositoryCleared;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public RawTerminal(Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings)
		{
			_txRepository    = new RawRepository(bufferSettings.TxBufferSize);
			_bidirRepository = new RawRepository(bufferSettings.BidirBufferSize);
			_rxRepository    = new RawRepository(bufferSettings.RxBufferSize);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(Factory.IOFactory.CreateIO(ioSettings)); // settings are applied by factory
		}

		public RawTerminal(Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings, RawTerminal rhs)
		{
			_txRepository    = new RawRepository(rhs._txRepository);
			_bidirRepository = new RawRepository(rhs._bidirRepository);
			_rxRepository    = new RawRepository(rhs._rxRepository);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(Factory.IOFactory.CreateIO(ioSettings)); // settings are applied by factory
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

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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

		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_io.HasStarted);
			}
		}

		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_io.IsConnected);
			}
		}

		public Domain.IO.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_io);
			}
		}

		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_io.UnderlyingIOInstance);
			}
		}

		#endregion

		#region Methods
		//******************************************************************************************
		// Methods
		//******************************************************************************************

		//------------------------------------------------------------------------------------------
		// Open/Close
		//------------------------------------------------------------------------------------------

		public void Open()
		{
			AssertNotDisposed();
			_io.Start();
		}

		public void Close()
		{
			AssertNotDisposed();
			_io.Stop();
		}

		//------------------------------------------------------------------------------------------
		// Send
		//------------------------------------------------------------------------------------------

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

		new public string ToString()
		{
			AssertNotDisposed();

			return (ToString(""));
		}

		public string ToString(string indent)
		{
			AssertNotDisposed();

			return (indent + "- IOSettings: " + _ioSettings.ToString() + Environment.NewLine +
					indent + "- TxRepository: " + Environment.NewLine + _txRepository.ToString(indent + "- ") +
					indent + "- BidirRepository: " + Environment.NewLine + _bidirRepository.ToString(indent + "- ") +
					indent + "- RxRepository: " + Environment.NewLine + _rxRepository.ToString(indent + "- "));
		}

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
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachBufferSettings(Settings.BufferSettings bufferSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(_bufferSettings, bufferSettings))
				return;

			if (_bufferSettings != null)
				DetachBufferSettings();

			_bufferSettings = bufferSettings;
			_bufferSettings.Changed += new EventHandler<Utilities.Settings.SettingsEventArgs>(_bufferSettings_Changed);
		}

		private void DetachBufferSettings()
		{
			_bufferSettings.Changed -= new EventHandler<Utilities.Settings.SettingsEventArgs>(_bufferSettings_Changed);
			_bufferSettings = null;
		}

		private void ApplyBufferSettings()
		{
			_txRepository.Capacity = _bufferSettings.TxBufferSize;
			_bidirRepository.Capacity = _bufferSettings.BidirBufferSize;
			_rxRepository.Capacity = _bufferSettings.RxBufferSize;
		}

		private void AttachIOSettings(Settings.IOSettings ioSettings)
		{
			if (Settings.IOSettings.ReferenceEquals(_ioSettings, ioSettings))
				return;

			if (_ioSettings != null)
				DetachIOSettings();

			_ioSettings = ioSettings;
			_ioSettings.Changed += new EventHandler<Utilities.Settings.SettingsEventArgs>(_ioSettings_Changed);
		}

		private void DetachIOSettings()
		{
			_ioSettings.Changed -= new EventHandler<Utilities.Settings.SettingsEventArgs>(_ioSettings_Changed);
			_ioSettings = null;
		}

		private void ApplyIOSettings()
		{
			// nothing to do
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void _bufferSettings_Changed(object sender, Utilities.Settings.SettingsEventArgs e)
		{
			ApplyBufferSettings();
		}

		private void _ioSettings_Changed(object sender, Utilities.Settings.SettingsEventArgs e)
		{
			ApplyIOSettings();
		}

		#endregion

		#region IO
		//------------------------------------------------------------------------------------------
		// IO
		//------------------------------------------------------------------------------------------

		private void AttachIO(IO.IIOProvider io)
		{
			if (IO.IIOProvider.ReferenceEquals(_io, io))
				return;

			if (_io != null)
				DetachIO();

			_io = io;
			_io.IOChanged        += new EventHandler(_io_IOChanged);
			_io.IOControlChanged += new EventHandler(_io_IOControlChanged);
			_io.IOError          += new EventHandler<IO.IOErrorEventArgs>(_io_IOError);
			_io.DataReceived     += new EventHandler(_io_DataReceived);
		}

		private void DetachIO()
		{
			_io.IOChanged        -= new EventHandler(_io_IOChanged);
			_io.IOControlChanged -= new EventHandler(_io_IOControlChanged);
			_io.IOError          -= new EventHandler<IO.IOErrorEventArgs>(_io_IOError);
			_io.DataReceived     -= new EventHandler(_io_DataReceived);
			_io = null;
		}

		#endregion

		#region IO Events
		//------------------------------------------------------------------------------------------
		// IO Events
		//------------------------------------------------------------------------------------------

		private void _io_IOChanged(object sender, EventArgs e)
		{
			OnTerminalChanged(new EventArgs());
		}

		private void _io_IOControlChanged(object sender, EventArgs e)
		{
			OnTerminalControlChanged(new EventArgs());
		}

		private void _io_IOError(object sender, IO.IOErrorEventArgs e)
		{
			OnTerminalError(new TerminalErrorEventArgs(e.Message));
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
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnTerminalChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(TerminalChanged, this, e);
		}

		protected virtual void OnTerminalControlChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(TerminalControlChanged, this, e);
		}

		protected virtual void OnTerminalError(TerminalErrorEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<TerminalErrorEventArgs>(TerminalError, this, e);
		}

		protected virtual void OnRawElementSent(RawElementEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<RawElementEventArgs>(RawElementSent, this, e);
		}

		protected virtual void OnRawElementReceived(RawElementEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<RawElementEventArgs>(RawElementReceived, this, e);
		}

		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		#endregion
	}
}