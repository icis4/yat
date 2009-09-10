//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MKY.Utilities;
using MKY.Utilities.Event;
using MKY.Utilities.Types;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Terminal with byte/string functionality and settings.
	/// </summary>
	/// <remarks>
	/// Terminal and its specializations <see cref="TextTerminal"/> and <see cref="BinaryTerminal"/>
	/// implement the method pattern. Terminal provides general processing and formatting functions,
	/// its specializations add additional functionality.
	/// </remarks>
	public class Terminal : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private Settings.TerminalSettings _terminalSettings;

		private RawTerminal _rawTerminal;

		private DisplayRepository _txRepository;
		private DisplayRepository _bidirRepository;
		private DisplayRepository _rxRepository;

		private bool _eventsSuspendedForReload = false;

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
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsSent;
		/// <summary></summary>
		public event EventHandler<DisplayElementsEventArgs> DisplayElementsReceived;
		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesSent;
		/// <summary></summary>
		public event EventHandler<DisplayLinesEventArgs> DisplayLinesReceived;
		/// <summary></summary>
		public event EventHandler<RepositoryEventArgs> RepositoryCleared;
		/// <summary></summary>
		public event EventHandler<RepositoryEventArgs> RepositoryReloaded;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Terminal(Settings.TerminalSettings settings)
		{
			_txRepository    = new DisplayRepository(settings.Display.TxMaxLineCount);
			_bidirRepository = new DisplayRepository(settings.Display.BidirMaxLineCount);
			_rxRepository    = new DisplayRepository(settings.Display.RxMaxLineCount);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(_terminalSettings.IO, _terminalSettings.Buffer));

			_eventsSuspendedForReload = false;
		}

		/// <summary></summary>
		public Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			_txRepository    = new DisplayRepository(terminal._txRepository);
			_bidirRepository = new DisplayRepository(terminal._bidirRepository);
			_rxRepository    = new DisplayRepository(terminal._rxRepository);

			_txRepository.Capacity    = settings.Display.TxMaxLineCount;
			_bidirRepository.Capacity = settings.Display.BidirMaxLineCount;
			_rxRepository.Capacity    = settings.Display.RxMaxLineCount;

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

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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

		/// <summary></summary>
		public bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.IsStarted);
			}
		}

		/// <summary></summary>
		public bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.IsConnected);
			}
		}

		/// <summary></summary>
		public bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.IsOpen);
			}
		}

		/// <summary></summary>
		public MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (_rawTerminal.UnderlyingIOInstance);
			}
		}

		/// <summary></summary>
		protected RawTerminal RawTerminal
		{
			get { return (_rawTerminal); }
		}

		/// <summary></summary>
		protected DisplayRepository TxRepository
		{
			get { return (_txRepository); }
		}

		/// <summary></summary>
		protected DisplayRepository BidirRepository
		{
			get { return (_bidirRepository); }
		}

		/// <summary></summary>
		protected DisplayRepository RxRepository
		{
			get { return (_rxRepository); }
		}

		/// <summary></summary>
		protected bool Reload
		{
			get { return (_eventsSuspendedForReload); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > Start/Stop
		//------------------------------------------------------------------------------------------
		// Methods > Start/Stop
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Start()
		{
			AssertNotDisposed();
			_rawTerminal.Start();
		}

		/// <summary></summary>
		public virtual void Stop()
		{
			AssertNotDisposed();
			_rawTerminal.Stop();
		}

		#endregion

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();
			_rawTerminal.Send(data);
		}

		/// <summary></summary>
		public virtual void Send(string s)
		{
			AssertNotDisposed();

			Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianess);
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

		/// <summary></summary>
		public virtual void SendLine(string line)
		{
			// Simply send line as string
			Send(line);
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual DisplayElement ByteToElement(byte b, SerialDirection d)
		{
			if (d == SerialDirection.Tx)
				return (ByteToElement(b, d, _terminalSettings.Display.TxRadix));
			else
				return (ByteToElement(b, d, _terminalSettings.Display.RxRadix));
		}

		/// <summary></summary>
		protected virtual DisplayElement ByteToElement(byte b, SerialDirection d, Radix r)
		{
			bool replaceToAscii = ((TerminalSettings.CharReplace.ReplaceControlChars) &&
								   (TerminalSettings.CharReplace.ControlCharRadix == ControlCharRadix.AsciiMnemonic));
			bool error = false;
			string text = "";

			switch (r)
			{
				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				{
					if ((b < 0x20) || (b == 0x7F)) // Control chars
					{
						if (replaceToAscii)
							text = ByteToAsciiString(b);
						else
							text = ByteToNumericRadixString(b, r);
					}
					else
					{
						text = ByteToNumericRadixString(b, r);
					}
					break;
				}
				case Radix.Char:
				case Radix.String:
				{
					if ((b < 0x20) || (b == 0x7F)) // Control chars
					{
						if (replaceToAscii)
						{
							text = ByteToAsciiString(b);
						}
						else
						{
							error = true; // Signal error
							if (d == SerialDirection.Tx)
								text = "Sent";
							else
								text = "Received";
							text += " ASCII control char";
							text += " <" + b.ToString("X2") + "h>";
							text += " cannot be displayed in current settings";
						}
					}
					else if (b == 0x20) // Space
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							text = "␣";
						else
							text = " ";
					}
					else
					{
						text = ((char)b).ToString();
					}
					break;
				}
				default: throw (new NotImplementedException("Invalid radix"));
			}

			if (!error)
			{
				if ((b == 0x09) && !_terminalSettings.CharReplace.ReplaceTab) // Tab
				{
					if (d == SerialDirection.Tx)
						return (new DisplayElement.TxData(b, text));
					else
						return (new DisplayElement.RxData(b, text));
				}
				else if ((b < 0x20) || (b == 0x7F)) // Control chars
				{
					if (d == SerialDirection.Tx)
						return (new DisplayElement.TxControl(b, text));
					else
						return (new DisplayElement.RxControl(b, text));
				}
				else
				{
					if (d == SerialDirection.Tx)
						return (new DisplayElement.TxData(b, text));
					else
						return (new DisplayElement.RxData(b, text));
				}
			}
			else
			{
				return (new DisplayElement.Error(d, text));
			}
		}

		/// <summary></summary>
		protected virtual string ByteToAsciiString(byte b)
		{
			if ((b == 0x09) && !_terminalSettings.CharReplace.ReplaceTab)
				return ("\t");
			else
				return ("<" + Ascii.ConvertToMnemonic(b) + ">");
		}

		/// <summary></summary>
		protected virtual string ByteToNumericRadixString(byte b, Radix r)
		{
			switch (r)
			{
				case Radix.Bin:
				{
					if (_terminalSettings.Display.ShowRadix)
						return (XByte.ConvertToBinaryString(b) + "b");
					else
						return (XByte.ConvertToBinaryString(b));
				}
				case Radix.Oct:
				{
					if (_terminalSettings.Display.ShowRadix)
						return (XByte.ConvertToOctalString(b) + "o");
					else
						return (XByte.ConvertToOctalString(b));
				}
				case Radix.Dec:
				{
					if (_terminalSettings.Display.ShowRadix)
						return (b.ToString("D3") + "d");
					else
						return (b.ToString("D3"));
				}
				case Radix.Hex:
				{
					if (_terminalSettings.Display.ShowRadix)
						return (b.ToString("X2") + "h");
					else
						return (b.ToString("X2"));
				}
				default: throw (new NotImplementedException("Invalid radix"));
			}
		}

		/// <summary></summary>
		protected virtual bool ElementsAreSeparate(SerialDirection d)
		{
			if (d == SerialDirection.Tx)
				return (ElementsAreSeparate(_terminalSettings.Display.TxRadix));
			else
				return (ElementsAreSeparate(_terminalSettings.Display.RxRadix));
		}

		/// <summary></summary>
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

		/// <summary></summary>
		protected void SuspendEventsForReload()
		{
			_eventsSuspendedForReload = true;
		}

		/// <summary></summary>
		protected void ResumeEventsAfterReload()
		{
			_eventsSuspendedForReload = false;
		}

		/// <summary></summary>
		protected virtual void ProcessRawElement(RawElement re, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			DisplayLine dl = new DisplayLine();

			// Line begin and time stamp
			if (_terminalSettings.Display.ShowTimeStamp)
			{
				dl.Add(new DisplayElement.TimeStamp(re.Direction, re.TimeStamp));
				dl.Add(new DisplayElement.LeftMargin());
			}

			// Data
			foreach (byte b in re.Data)
			{
				dl.Add(ByteToElement(b, re.Direction));
			}

			// Line length and end
			if (_terminalSettings.Display.ShowLength)
			{
				dl.Add(new DisplayElement.RightMargin());
				dl.Add(new DisplayElement.LineLength(re.Direction, 1));
			}
			dl.Add(new DisplayElement.LineBreak(re.Direction));

			// Return elements
			// Attention: Clone elements because they are needed again below
			elements.AddRange(dl.Clone());
			lines.Add(dl);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawElement(RawElement re)
		{
			// Collection of elements processed, extends over one or multiple lines, depending on
			// the number of bytes in raw element
			DisplayElementCollection elements = new DisplayElementCollection();
			List<DisplayLine> lines = new List<DisplayLine>();

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

		/// <summary></summary>
		public virtual void ClearRepository(RepositoryType repository)
		{
			AssertNotDisposed();
			_rawTerminal.ClearRepository(repository);
		}

		/// <summary></summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();

			ClearRepository(RepositoryType.Tx);
			ClearRepository(RepositoryType.Bidir);
			ClearRepository(RepositoryType.Rx);
		}

		/// <summary></summary>
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

		/// <summary></summary>
		protected virtual void ClearMyRepository(RepositoryType repository)
		{
			switch (repository)
			{
				case RepositoryType.Tx:    _txRepository.Clear();    break;
				case RepositoryType.Bidir: _bidirRepository.Clear(); break;
				case RepositoryType.Rx:    _rxRepository.Clear();    break;
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		/// <summary></summary>
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

		/// <summary></summary>
		public int GetRepositoryLineCount(RepositoryType repository)
		{
			AssertNotDisposed();

			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.Count);
				case RepositoryType.Bidir: return (_bidirRepository.Count);
				case RepositoryType.Rx:    return (_rxRepository.Count);
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		/// <summary></summary>
		public virtual List<DisplayLine> RepositoryToDisplayLines(RepositoryType repository)
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

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository)
		{
			AssertNotDisposed();

			switch (repository)
			{
				case RepositoryType.Tx:    return (_txRepository.ToString());
				case RepositoryType.Bidir: return (_bidirRepository.ToString());
				case RepositoryType.Rx:    return (_rxRepository.ToString());
				default: throw (new NotImplementedException("Unknown RepositoryType"));
			}
		}

		/// <summary></summary>
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

		/// <summary></summary>
		new public virtual string ToString()
		{
			AssertNotDisposed();
			
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			AssertNotDisposed();
			
			return (indent + "- Settings: " + _terminalSettings + Environment.NewLine +
					indent + "- RawTerminal: "     + Environment.NewLine + _rawTerminal.ToString(indent + "--") +
					indent + "- TxRepository: "    + Environment.NewLine + _txRepository.ToString(indent + "--") +
					indent + "- BidirRepository: " + Environment.NewLine + _bidirRepository.ToString(indent + "--") +
					indent + "- RxRepository: "    + Environment.NewLine  + _rxRepository.ToString(indent + "--"));
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

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachTerminalSettings(Settings.TerminalSettings terminalSettings)
		{
			if (Settings.TerminalSettings.ReferenceEquals(_terminalSettings, terminalSettings))
				return;

			if (_terminalSettings != null)
				DetachTerminalSettings();

			_terminalSettings = terminalSettings;
			_terminalSettings.Changed += new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_terminalSettings_Changed);
		}

		private void DetachTerminalSettings()
		{
			_terminalSettings.Changed -= new EventHandler<MKY.Utilities.Settings.SettingsEventArgs>(_terminalSettings_Changed);
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

			_txRepository.Capacity    = s.TxMaxLineCount;
			_bidirRepository.Capacity = s.BidirMaxLineCount;
			_rxRepository.Capacity    = s.RxMaxLineCount;
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================
		
		private void _terminalSettings_Changed(object sender, MKY.Utilities.Settings.SettingsEventArgs e)
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
		//==========================================================================================
		// Raw Terminal
		//==========================================================================================

		private void AttachRawTerminal(RawTerminal rawTerminal)
		{
			_rawTerminal = rawTerminal;

			_rawTerminal.IOChanged          += new EventHandler(_rawTerminal_IOChanged);
			_rawTerminal.IOControlChanged   += new EventHandler(_rawTerminal_IOControlChanged);
			_rawTerminal.IORequest          += new EventHandler<IORequestEventArgs>(_rawTerminal_IORequest);
			_rawTerminal.IOError            += new EventHandler<IOErrorEventArgs>(_rawTerminal_IOError);

			_rawTerminal.RawElementSent     += new EventHandler<RawElementEventArgs>(_rawTerminal_RawElementSent);
			_rawTerminal.RawElementReceived += new EventHandler<RawElementEventArgs>(_rawTerminal_RawElementReceived);
			_rawTerminal.RepositoryCleared  += new EventHandler<RepositoryEventArgs>(_rawTerminal_RepositoryCleared);
		}

		#endregion

		#region Raw Terminal Events
		//==========================================================================================
		// Raw Terminal Events
		//==========================================================================================

		private void _rawTerminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void _rawTerminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void _rawTerminal_IORequest(object sender, IORequestEventArgs e)
		{
			OnIORequest(e);
		}

		private void _rawTerminal_IOError(object sender, IOErrorEventArgs e)
		{
			SerialPortErrorEventArgs serialPortErrorEventArgs = (e as SerialPortErrorEventArgs);
			if (serialPortErrorEventArgs == null)
			{
				OnIOError(e);
			}
			else
			{
				// handle serial port errors whenever possible
				switch (serialPortErrorEventArgs.SerialPortError)
				{
					case System.IO.Ports.SerialError.Frame:    OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("<FRAMING ERROR>"));   break;
					case System.IO.Ports.SerialError.Overrun:  OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("<BUFFER OVERRUN>"));  break;
					case System.IO.Ports.SerialError.RXOver:   OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("<BUFFER OVERFLOW>")); break;
					case System.IO.Ports.SerialError.RXParity: OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("<PARITY ERROR>"));    break;
					case System.IO.Ports.SerialError.TXFull:   OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.Error("<BUFFER FULL>"));     break;
					default:                                   OnIOError(e); break;
				}
			}
		}

		private void _rawTerminal_RawElementSent(object sender, RawElementEventArgs e)
		{
			OnRawElementSent(e);
			ProcessAndSignalRawElement(e.Element);
		}

		private void _rawTerminal_RawElementReceived(object sender, RawElementEventArgs e)
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
		protected virtual void OnDisplayElementProcessed(SerialDirection direction, DisplayElement element)
		{
			DisplayElementCollection elements = new DisplayElementCollection();
			elements.Add(element);
			OnDisplayElementsProcessed(direction, elements);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsProcessed(SerialDirection direction, DisplayElementCollection elements)
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

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesProcessed(SerialDirection direction, List<DisplayLine> lines)
		{
			if (!_eventsSuspendedForReload)
			{
				if (direction == SerialDirection.Tx)
					OnDisplayLinesSent(new DisplayLinesEventArgs(lines));
				else
					OnDisplayLinesReceived(new DisplayLinesEventArgs(lines));
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesSent(DisplayLinesEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(RepositoryEventArgs e)
		{
			if (!_eventsSuspendedForReload)
				EventHelper.FireSync<RepositoryEventArgs>(RepositoryReloaded, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
