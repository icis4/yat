using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using HSR.Utilities;

namespace HSR.YAT.Domain
{
	/// <summary>
	/// Terminal with byte/string functionality and settings.
	/// </summary>
	public class Terminal : IDisposable
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isDisposed = false;

		private Settings.TerminalSettings _terminalSettings;

		private RawTerminal _rawTerminal;

		private DisplayRepository _txRepository;
		private DisplayRepository _bidirRepository;
		private DisplayRepository _rxRepository;

		private bool _eventsSuspendedForReload = false;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------
		
		public event EventHandler TerminalChanged;
		public event EventHandler TerminalControlChanged;
		public event EventHandler<TerminalErrorEventArgs> TerminalError;

		public event EventHandler<RawElementEventArgs> RawElementSent;
		public event EventHandler<RawElementEventArgs> RawElementReceived;
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsSent;
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsReceived;
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesSent;
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesReceived;
		public event EventHandler<RepositoryEventArgs> RepositoryCleared;
		public event EventHandler<RepositoryEventArgs> RepositoryReloaded;

		//------------------------------------------------------------------------------------------
		// Constructors
		//------------------------------------------------------------------------------------------
		
		public Terminal(Settings.TerminalSettings settings)
		{
			_txRepository    = new DisplayRepository(settings.Buffer.TxBufferSize);
			_bidirRepository = new DisplayRepository(settings.Buffer.BidirBufferSize);
			_rxRepository    = new DisplayRepository(settings.Buffer.RxBufferSize);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(_terminalSettings.IO, _terminalSettings.Buffer));

			_eventsSuspendedForReload = false;
		}

		public Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			_txRepository    = new DisplayRepository(terminal._txRepository);
			_bidirRepository = new DisplayRepository(terminal._bidirRepository);
			_rxRepository    = new DisplayRepository(terminal._rxRepository);

			_txRepository.LineCapacity    = settings.Display.TxMaximalLineCount;
			_bidirRepository.LineCapacity = settings.Display.BidirMaximalLineCount;
			_rxRepository.LineCapacity    = settings.Display.RxMaximalLineCount;

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(_terminalSettings.IO, _terminalSettings.Buffer, terminal._rawTerminal));

			_eventsSuspendedForReload = terminal._eventsSuspendedForReload;
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
					if (_rawTerminal != null)
						_rawTerminal.Dispose();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Terminal()
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

		public Settings.TerminalSettings TerminalSettings
		{
			get
			{
				AssertNotDisposed();
				return (_terminalSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachTerminalSettings(value);
				ApplyTerminalSettings();
			}
		}

		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.IsOpen);
			}
		}

		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.IsConnected);
			}
		}

		public Domain.IO.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.UnderlyingIOProvider);
			}
		}

		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.UnderlyingIOInstance);
			}
		}

		protected RawTerminal RawTerminal
		{
			get { return (_rawTerminal); }
		}

		protected DisplayRepository TxRepository
		{
			get { return (_txRepository); }
		}

		protected DisplayRepository BidirRepository
		{
			get { return (_bidirRepository); }
		}

		protected DisplayRepository RxRepository
		{
			get { return (_rxRepository); }
		}

		protected bool Reload
		{
			get { return (_eventsSuspendedForReload); }
		}

		#endregion

		#region Methods
		//******************************************************************************************
		// Methods
		//******************************************************************************************

		#region Methods > Open/Close
		//------------------------------------------------------------------------------------------
		// Methods > Open/Close
		//------------------------------------------------------------------------------------------
		
		public virtual void Open()
		{
			AssertNotDisposed();
			_rawTerminal.Open();
		}

		public virtual void Close()
		{
			AssertNotDisposed();
			_rawTerminal.Close();
		}

		#endregion

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------
		
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();
			_rawTerminal.Send(data);
		}

		public virtual void Send(string s)
		{
			AssertNotDisposed();

			Parser.Parser p = new Parser.Parser();
			foreach (Parser.Result result in p.Parse(s, Parser.ParseMode.All))
			{
				if (result is Parser.ByteArrayResult)
				{
					_rawTerminal.Send(((Parser.ByteArrayResult)result).ByteArray);
				}
				else if (result is Parser.KeywordResult)
				{
					// \fixme
				}
			}
		}

		public virtual void SendLine(string line)
		{
			// simply send line as string
			Send(line);
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		protected virtual DisplayElement ByteToElement(byte b, SerialDirection d)
		{
			return (ByteToElement(b, d, _terminalSettings.Display.Radix));
		}

		protected virtual DisplayElement ByteToElement(byte b, SerialDirection d, Radix r)
		{
			string data = "";

			switch (r)
			{
				case Radix.Bin:    data += Utilities.Types.XByte.ConvertToBinaryString(b) + "b"; break;
				case Radix.Oct:    data += Utilities.Types.XByte.ConvertToOctalString(b) + "o"; break;
				case Radix.Dec:    data += b.ToString("D3") + "d"; break;
				case Radix.Hex:    data += b.ToString("X2") + "h"; break;
				case Radix.Char:
				case Radix.String:
				{
					if ((b < 0x20) || (b == 0x7F))
						data += "<" + b.ToString("X2") + "h>";
					else
						data += ((char)b).ToString();

					break;
				}
				default: throw (new NotImplementedException("Unknown Radix"));
			}

			if (d == SerialDirection.Tx)
				return (new DisplayElement.TxData(new ElementOrigin(b, d), data));
			else
				return (new DisplayElement.RxData(new ElementOrigin(b, d), data));
		}

		protected virtual bool ElementsAreSeparate()
		{
			return (ElementsAreSeparate(_terminalSettings.Display.Radix));
		}

		protected virtual bool ElementsAreSeparate(Radix r)
		{
			switch (r)
			{
				case Radix.Bin:    return (true);
				case Radix.Oct:    return (true);
				case Radix.Dec:    return (true);
				case Radix.Hex:    return (true);
				case Radix.Char:   return (true);
				case Radix.String: return (false);
				default: throw (new NotImplementedException("Unknown Radix"));
			}
		}

		protected void SuspendEventsForReload()
		{
			_eventsSuspendedForReload = true;
		}

		protected void ResumeEventsAfterReload()
		{
			_eventsSuspendedForReload = false;
		}

		protected virtual void ProcessRawElement(RawElement re, List<DisplayElement> elements, List<List<DisplayElement>> lines)
		{
			List<DisplayElement> l = new List<DisplayElement>();

			// line begin and time stamp
			if (_terminalSettings.Display.ShowTimestamp)
			{
				l.Add(new DisplayElement.TimeStamp(re.TimeStamp));
				l.Add(new DisplayElement.LeftMargin());
			}

			// data
			foreach (byte b in re.Data)
			{
				l.Add(ByteToElement(b, re.Direction));
			}

			// line length and end
			if (_terminalSettings.Display.ShowLength)
			{
				l.Add(new DisplayElement.RightMargin());
				l.Add(new DisplayElement.LineLength(1));
			}
			l.Add(new DisplayElement.LineBreak());

			// return elements
			elements.AddRange(l);
			lines.Add(l);
		}

		protected virtual void ProcessAndSignalRawElement(RawElement re)
		{
			List<DisplayElement> elements = new List<DisplayElement>();
			List<List<DisplayElement>> lines = new List<List<DisplayElement>>();

			ProcessRawElement(re, elements, lines);

			if (elements.Count > 0)
			{
				OnDisplayElementsProcessed(re.Direction, elements);
				if (lines.Count > 0)
				{
					OnDisplayLinesProcessed(re.Direction, lines);
				}
			}
		}

		#endregion

		#region Methods > Repository Access
		//------------------------------------------------------------------------------------------
		// Methods > Repository Access
		//------------------------------------------------------------------------------------------

		public virtual void ClearRepository(RepositoryType repository)
		{
			AssertNotDisposed();
			_rawTerminal.ClearRepository(repository);
		}

		public virtual void ClearRepositories()
		{
			AssertNotDisposed();

			ClearRepository(RepositoryType.Tx);
			ClearRepository(RepositoryType.Bidir);
			ClearRepository(RepositoryType.Rx);
		}

		public virtual void ReloadRepositories()
		{
			AssertNotDisposed();

			// clear display repository
			ClearMyRepository(RepositoryType.Tx);
			ClearMyRepository(RepositoryType.Bidir);
			ClearMyRepository(RepositoryType.Rx);
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Tx));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Bidir));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Rx));

			// reload display repository
			SuspendEventsForReload();
			foreach (RawElement re in _rawTerminal.RepositoryToElements(RepositoryType.Bidir))
			{
				ProcessAndSignalRawElement(re);
			}
			ResumeEventsAfterReload();
			OnRepositoryReloaded(new RepositoryEventArgs(RepositoryType.Tx));
			OnRepositoryReloaded(new RepositoryEventArgs(RepositoryType.Bidir));
			OnRepositoryReloaded(new RepositoryEventArgs(RepositoryType.Rx));
		}

		protected virtual void ClearMyRepository(RepositoryType repository)
		{
			switch (repository)
			{
				case RepositoryType.Tx:    _txRepository.Clear(); break;
				case RepositoryType.Bidir: _bidirRepository.Clear(); break;
				case RepositoryType.Rx:    _rxRepository.Clear(); break;
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		public int GetRepositoryDataCount(RepositoryType repository)
		{
			AssertNotDisposed();

			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.DataCount);
				case RepositoryType.Bidir: return (_bidirRepository.DataCount);
				case RepositoryType.Rx:    return (_rxRepository.DataCount);
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		public int GetRepositoryLineCount(RepositoryType repository)
		{
			AssertNotDisposed();

			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.LineCount);
				case RepositoryType.Bidir: return (_bidirRepository.LineCount);
				case RepositoryType.Rx:    return (_rxRepository.LineCount);
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		public virtual List<DisplayElement> RepositoryToDisplayElements(RepositoryType repository)
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

		public virtual List<List<DisplayElement>> RepositoryToDisplayLines(RepositoryType repository)
		{
			AssertNotDisposed();

			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.ToLines());
				case RepositoryType.Bidir: return (_bidirRepository.ToLines());
				case RepositoryType.Rx:    return (_rxRepository.ToLines());
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		public virtual List<RawElement> RepositoryToRawElements(RepositoryType repository)
		{
			AssertNotDisposed();
			return (_rawTerminal.RepositoryToElements(repository));
		}

		#endregion

		#region Methods > ToString
		//------------------------------------------------------------------------------------------
		// Methods > ToString
		//------------------------------------------------------------------------------------------

		new public virtual string ToString()
		{
			AssertNotDisposed();
			
			return (ToString(""));
		}

		public virtual string ToString(string indent)
		{
			AssertNotDisposed();
			
			return (indent + "- Settings: " + _terminalSettings.ToString() + Environment.NewLine +
					indent + "- RawTerminal: "     + Environment.NewLine + _rawTerminal.ToString(indent + "--") +
					indent + "- TxRepository: "    + Environment.NewLine + _txRepository.ToString(indent + "--") +
					indent + "- BidirRepository: " + Environment.NewLine + _bidirRepository.ToString(indent + "--") +
					indent + "- RxRepository: "    + Environment.NewLine  + _rxRepository.ToString(indent + "--"));
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

		#endregion

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachTerminalSettings(Settings.TerminalSettings terminalSettings)
		{
			if (Settings.TerminalSettings.ReferenceEquals(_terminalSettings, terminalSettings))
				return;

			if (_terminalSettings != null)
				DetachTerminalSettings();

			_terminalSettings = terminalSettings;
			_terminalSettings.Changed += new EventHandler<Utilities.Settings.SettingsEventArgs>(_terminalSettings_Changed);
		}

		private void DetachTerminalSettings()
		{
			_terminalSettings.Changed -= new EventHandler<Utilities.Settings.SettingsEventArgs>(_terminalSettings_Changed);
			_terminalSettings = null;
		}

		private void ApplyTerminalSettings()
		{
			ApplyIOSettings();
			ApplyBufferSettings();
			ApplyDisplaySettings();
		}

		private void ApplyIOSettings()
		{
			_rawTerminal.IOSettings = _terminalSettings.IO;
		}

		private void ApplyBufferSettings()
		{
			_rawTerminal.BufferSettings = _terminalSettings.Buffer;
		}

		private void ApplyDisplaySettings()
		{
			Settings.DisplaySettings s = _terminalSettings.Display;

			_txRepository.LineCapacity    = s.TxMaximalLineCount;
			_bidirRepository.LineCapacity = s.BidirMaximalLineCount;
			_rxRepository.LineCapacity    = s.RxMaximalLineCount;
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------
		protected virtual void _terminalSettings_Changed(object sender, HSR.Utilities.Settings.SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// TerminalSettings changed
				ApplyTerminalSettings();
			}
			else
			{
				if (Settings.IOSettings.ReferenceEquals(e.Inner.Source, _terminalSettings.IO))
				{
					// IOSettings changed
					ApplyIOSettings();
				}
				else if (Settings.BufferSettings.ReferenceEquals(e.Inner.Source, _terminalSettings.Buffer))
				{
					// BufferSettings changed
					ApplyBufferSettings();
				}
				else if (Settings.DisplaySettings.ReferenceEquals(e.Inner.Source, _terminalSettings.Display))
				{
					// DisplaySettings changed
					ApplyDisplaySettings();
				}
			}
		}

		#endregion

		#region Raw Terminal
		//------------------------------------------------------------------------------------------
		// Raw Terminal
		//------------------------------------------------------------------------------------------

		private void AttachRawTerminal(RawTerminal rawTerminal)
		{
			_rawTerminal = rawTerminal;
			_rawTerminal.TerminalChanged        += new EventHandler(_rawTerminal_TerminalChanged);
			_rawTerminal.TerminalControlChanged += new EventHandler(_rawTerminal_TerminalControlChanged);
			_rawTerminal.RawElementSent         += new EventHandler<RawElementEventArgs>(_rawTerminal_RawElementSent);
			_rawTerminal.RawElementReceived     += new EventHandler<RawElementEventArgs>(_rawTerminal_RawElementReceived);
			_rawTerminal.RepositoryCleared      += new EventHandler<RepositoryEventArgs>(_rawTerminal_RepositoryCleared);
			_rawTerminal.TerminalError          += new EventHandler<TerminalErrorEventArgs>(_rawTerminal_TerminalError);
		}

		#endregion

		#region Raw Terminal Events
		//------------------------------------------------------------------------------------------
		// Raw Terminal Events
		//------------------------------------------------------------------------------------------

		private void _rawTerminal_TerminalChanged(object sender, EventArgs e)
		{
			OnTerminalChanged(e);
		}

		private void _rawTerminal_TerminalControlChanged(object sender, EventArgs e)
		{
			OnTerminalControlChanged(e);
		}

		private void _rawTerminal_TerminalError(object sender, TerminalErrorEventArgs e)
		{
			OnTerminalError(e);
		}

		protected virtual void _rawTerminal_RawElementSent(object sender, RawElementEventArgs e)
		{
			OnRawElementSent(e);
			ProcessAndSignalRawElement(e.Element);
		}

		protected virtual void _rawTerminal_RawElementReceived(object sender, RawElementEventArgs e)
		{
			OnRawElementReceived(e);
			ProcessAndSignalRawElement(e.Element);
		}

		private void _rawTerminal_RepositoryCleared(object sender, RepositoryEventArgs e)
		{
			ClearMyRepository(e.Repository);
			OnRepositoryCleared(e);
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

		protected virtual void OnDisplayElementsProcessed(SerialDirection direction, List<DisplayElement> elements)
		{
			if (direction == SerialDirection.Tx)
			{
				_txRepository.Enqueue(elements);
				_bidirRepository.Enqueue(elements);

				if (!_eventsSuspendedForReload)
					OnDisplayElementsSent(new DisplayElementsEventArgs(elements));
			}
			else
			{
				_bidirRepository.Enqueue(elements);
				_rxRepository.Enqueue(elements);

				if (!_eventsSuspendedForReload)
					OnDisplayElementsReceived(new DisplayElementsEventArgs(elements));
			}
		}

		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		protected virtual void OnDisplayLinesProcessed(SerialDirection direction, List<List<DisplayElement>> lines)
		{
			if (!_eventsSuspendedForReload)
			{
				if (direction == SerialDirection.Tx)
					OnDisplayLinesSent(new DisplayLinesEventArgs(lines));
				else
					OnDisplayLinesReceived(new DisplayLinesEventArgs(lines));
			}
		}

		protected virtual void OnDisplayLinesSent(DisplayLinesEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		protected virtual void OnRepositoryReloaded(RepositoryEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				Utilities.Event.EventHelper.FireSync<RepositoryEventArgs>(RepositoryReloaded, this, e);
		}

		#endregion
	}
}
