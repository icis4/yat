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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using MKY;
using MKY.Event;
using MKY.Text;

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
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Terminal : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.TerminalSettings terminalSettings;

		private RawTerminal rawTerminal;

		private DisplayRepository txRepository;
		private DisplayRepository bidirRepository;
		private DisplayRepository rxRepository;
		private object repositorySyncObj = new object();

		private bool eventsSuspendedForReload;

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
			this.txRepository    = new DisplayRepository(settings.Display.TxMaxLineCount);
			this.bidirRepository = new DisplayRepository(settings.Display.BidirMaxLineCount);
			this.rxRepository    = new DisplayRepository(settings.Display.RxMaxLineCount);

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(this.terminalSettings.IO, this.terminalSettings.Buffer));

			this.eventsSuspendedForReload = false;
		}

		/// <summary></summary>
		public Terminal(Settings.TerminalSettings settings, Terminal terminal)
		{
			this.txRepository    = new DisplayRepository(terminal.txRepository);
			this.bidirRepository = new DisplayRepository(terminal.bidirRepository);
			this.rxRepository    = new DisplayRepository(terminal.rxRepository);

			this.txRepository.Capacity    = settings.Display.TxMaxLineCount;
			this.bidirRepository.Capacity = settings.Display.BidirMaxLineCount;
			this.rxRepository.Capacity    = settings.Display.RxMaxLineCount;

			AttachTerminalSettings(settings);
			AttachRawTerminal(new RawTerminal(this.terminalSettings.IO, this.terminalSettings.Buffer, terminal.rawTerminal));

			this.eventsSuspendedForReload = terminal.eventsSuspendedForReload;
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
					if (this.rawTerminal != null)
						this.rawTerminal.Dispose();
				}
				this.isDisposed = true;
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
		public virtual Settings.TerminalSettings TerminalSettings
		{
			get
			{
				AssertNotDisposed();
				return (this.terminalSettings);
			}
			set
			{
				AssertNotDisposed();
				AttachTerminalSettings(value);
				ApplyTerminalSettings();
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.IsStopped);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.IsStarted);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.IsConnected);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.IsOpen);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.IsReadyToSend);
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();
				return (this.rawTerminal.UnderlyingIOInstance);
			}
		}

		/// <summary></summary>
		protected virtual bool IsReloading
		{
			get { return (this.eventsSuspendedForReload); }
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
		public virtual bool Start()
		{
			AssertNotDisposed();
			return (this.rawTerminal.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();
			this.rawTerminal.Stop();
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
			this.rawTerminal.Send(data);
		}

		/// <summary></summary>
		public virtual void Send(string s)
		{
			AssertNotDisposed();

			Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianess);
			foreach (Parser.Result result in p.Parse(s, Parser.ParseMode.All))
			{
				if      (result is Parser.ByteArrayResult)
				{
					this.rawTerminal.Send(((Parser.ByteArrayResult)result).ByteArray);
				}
				else if (result is Parser.KeywordResult)
				{
					ProcessKeywords((Parser.KeywordResult)result);
				}
			}
		}

		/// <summary></summary>
		public virtual void SendLine(string line)
		{
			// Simply send line as string.
			Send(line);
		}

		/// <summary></summary>
		protected virtual void ProcessKeywords(Parser.KeywordResult result)
		{
			switch (((Parser.KeywordResult)result).Keyword)
			{
				case Parser.Keyword.Clear:
				{
					this.ClearRepositories();
					break;
				}

				case Parser.Keyword.Delay:
				{
					OnIOError(new IOErrorEventArgs(IOErrorSeverity.Severe, @"\!(Delay(<TimeSpan>)) is not yet implemented, tracked as feature request #3105478"));
					break;
				}

				case Parser.Keyword.OutputBreakOn:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = true;
					}
					else
					{
						OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("Break is only supported on serial COM ports"));
					}
					break;
				}

				case Parser.Keyword.OutputBreakOff:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.OutputBreak = false;
					}
					else
					{
						OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("Break is only supported on serial COM ports"));
					}
					break;
				}

				case Parser.Keyword.OutputBreakToggle:
				{
					if (this.terminalSettings.IO.IOType == IOType.SerialPort)
					{
						MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)this.UnderlyingIOInstance;
						port.ToggleOutputBreak();
					}
					else
					{
						OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("Break is only supported on serial COM ports"));
					}
					break;
				}

				default:
				{
					// \fixme
					break;
				}
			}
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
				return (ByteToElement(b, d, this.terminalSettings.Display.TxRadix));
			else
				return (ByteToElement(b, d, this.terminalSettings.Display.RxRadix));
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
					if ((b < 0x20) || (b == 0x7F)) // Control chars.
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
					if ((b < 0x20) || (b == 0x7F)) // Control chars.
					{
						if (replaceToAscii)
						{
							text = ByteToAsciiString(b);
						}
						else
						{
							error = true; // Signal error.
							if (d == SerialDirection.Tx)
								text = "Sent";
							else
								text = "Received";
							text += " ASCII control char";
							text += " <" + b.ToString("X2", NumberFormatInfo.InvariantInfo) + "h>";
							text += " cannot be displayed in current settings";
						}
					}
					else if (b == 0x20) // Space.
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							text = Settings.CharReplaceSettings.SpaceReplaceChar;
						else
							text = " ";
					}
					else
					{
						text = ((char)b).ToString();
					}
					break;
				}
				default: throw (new ArgumentOutOfRangeException("r", r, "Invalid radix"));
			}

			if (!error)
			{
				if ((b == 0x09) && !this.terminalSettings.CharReplace.ReplaceTab) // Tab.
				{
					if (d == SerialDirection.Tx)
						return (new DisplayElement.TxData(b, text));
					else
						return (new DisplayElement.RxData(b, text));
				}
				else if ((b < 0x20) || (b == 0x7F)) // Control chars.
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
			if ((b == 0x09) && !this.terminalSettings.CharReplace.ReplaceTab)
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
					if (this.terminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToBinaryString(b) + "b");
					else
						return (ByteEx.ConvertToBinaryString(b));
				}
				case Radix.Oct:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToOctalString(b) + "o");
					else
						return (ByteEx.ConvertToOctalString(b));
				}
				case Radix.Dec:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (b.ToString("D3", NumberFormatInfo.InvariantInfo) + "d");
					else
						return (b.ToString("D3", NumberFormatInfo.InvariantInfo));
				}
				case Radix.Hex:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (b.ToString("X2", NumberFormatInfo.InvariantInfo) + "h");
					else
						return (b.ToString("X2", NumberFormatInfo.InvariantInfo));
				}
				default: throw (new ArgumentOutOfRangeException("r", r, "Invalid radix"));
			}
		}

		/// <summary></summary>
		protected virtual bool ElementsAreSeparate(SerialDirection d)
		{
			if (d == SerialDirection.Tx)
				return (ElementsAreSeparate(this.terminalSettings.Display.TxRadix));
			else
				return (ElementsAreSeparate(this.terminalSettings.Display.RxRadix));
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
				default: throw (new ArgumentOutOfRangeException("r", r, "Invalid radix"));
			}
		}

		/// <summary></summary>
		protected void SuspendEventsForReload()
		{
			this.eventsSuspendedForReload = true;
		}

		/// <summary></summary>
		protected void ResumeEventsAfterReload()
		{
			this.eventsSuspendedForReload = false;
		}

		/// <summary></summary>
		protected virtual void ProcessRawElement(RawElement re, DisplayElementCollection elements, List<DisplayLine> lines)
		{
			DisplayLine dl = new DisplayLine();

			// Line begin and time stamp.
			if (TerminalSettings.Display.ShowTimeStamp)
			{
				dl.Add(new DisplayElement.TimeStamp(re.Direction, re.TimeStamp));
				dl.Add(new DisplayElement.LeftMargin());
			}

			// Data.
			foreach (byte b in re.Data)
			{
				dl.Add(ByteToElement(b, re.Direction));
			}

			// Line length and end.
			if (TerminalSettings.Display.ShowLength)
			{
				dl.Add(new DisplayElement.RightMargin());
				dl.Add(new DisplayElement.LineLength(re.Direction, 1));
			}
			dl.Add(new DisplayElement.LineBreak(re.Direction));

			// Return elements.
			//
			// \attention:
			// Clone elements because they are needed again a line below.
			elements.AddRange(dl.Clone());
			lines.Add(dl);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawElement(RawElement re)
		{
			// Collection of elements processed, extends over one or multiple lines,
			// depending on the number of bytes in raw element.
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
			this.rawTerminal.ClearRepository(repository);
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

			// Clear display repository.
			ClearMyRepository(RepositoryType.Tx);
			ClearMyRepository(RepositoryType.Bidir);
			ClearMyRepository(RepositoryType.Rx);
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Tx));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Bidir));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Rx));

			// Reload display repository.
			SuspendEventsForReload();
			foreach (RawElement re in this.rawTerminal.RepositoryToElements(RepositoryType.Bidir))
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
			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    this.txRepository.Clear();    break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository.Clear();    break;
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryDataCount(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.DataCount);
					case RepositoryType.Bidir: return (this.bidirRepository.DataCount);
					case RepositoryType.Rx:    return (this.rxRepository.DataCount);
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryLineCount(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.Count);
					case RepositoryType.Bidir: return (this.bidirRepository.Count);
					case RepositoryType.Rx:    return (this.rxRepository.Count);
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual List<DisplayLine> RepositoryToDisplayLines(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.ToLines());
					case RepositoryType.Bidir: return (this.bidirRepository.ToLines());
					case RepositoryType.Rx:    return (this.rxRepository.ToLines());
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository.ToString());
					case RepositoryType.Bidir: return (this.bidirRepository.ToString());
					case RepositoryType.Rx:    return (this.rxRepository.ToString());
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual List<RawElement> RepositoryToRawElements(RepositoryType repository)
		{
			AssertNotDisposed();
			return (this.rawTerminal.RepositoryToElements(repository));
		}

		#endregion

		#region Methods > ToString
		//------------------------------------------------------------------------------------------
		// Methods > ToString
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

			string s = null;
			lock (this.repositorySyncObj)
			{
				s = indent + "- Settings: " + this.terminalSettings + Environment.NewLine +
					indent + "- RawTerminal: "     + Environment.NewLine + this.rawTerminal.ToString(indent + "--") +
					indent + "- TxRepository: "    + Environment.NewLine + this.txRepository.ToString(indent + "--") +
					indent + "- BidirRepository: " + Environment.NewLine + this.bidirRepository.ToString(indent + "--") +
					indent + "- RxRepository: "    + Environment.NewLine + this.rxRepository.ToString(indent + "--");
			}
			return (s);
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository, string indent)
		{
			AssertNotDisposed();
			
			string s = null;
			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    s = this.txRepository.ToString(indent);    break;
					case RepositoryType.Bidir: s = this.bidirRepository.ToString(indent); break;
					case RepositoryType.Rx:    s = this.rxRepository.ToString(indent);    break;
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
			return (s);
		}

		#endregion

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachTerminalSettings(Settings.TerminalSettings terminalSettings)
		{
			if (Settings.TerminalSettings.ReferenceEquals(this.terminalSettings, terminalSettings))
				return;

			if (this.terminalSettings != null)
				DetachTerminalSettings();

			this.terminalSettings = terminalSettings;
			this.terminalSettings.Changed += new EventHandler<MKY.Settings.SettingsEventArgs>(terminalSettings_Changed);
		}

		private void DetachTerminalSettings()
		{
			this.terminalSettings.Changed -= new EventHandler<MKY.Settings.SettingsEventArgs>(terminalSettings_Changed);
			this.terminalSettings = null;
		}

		private void ApplyTerminalSettings()
		{
			ApplyIOSettings();
			ApplyBufferSettings();
			ApplyDisplaySettings();
		}

		private void ApplyIOSettings()
		{
			this.rawTerminal.IOSettings = this.terminalSettings.IO;
		}

		private void ApplyBufferSettings()
		{
			this.rawTerminal.BufferSettings = this.terminalSettings.Buffer;
		}

		private void ApplyDisplaySettings()
		{
			Settings.DisplaySettings s = this.terminalSettings.Display;

			this.txRepository.Capacity    = s.TxMaxLineCount;
			this.bidirRepository.Capacity = s.BidirMaxLineCount;
			this.rxRepository.Capacity    = s.RxMaxLineCount;
		}

		#endregion

		#region Settings Events
		//==========================================================================================
		// Settings Events
		//==========================================================================================
		
		private void terminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// TerminalSettings changed
				ApplyTerminalSettings();
			}
			else
			{
				if (Settings.IOSettings.ReferenceEquals(e.Inner.Source, this.terminalSettings.IO))
				{
					// IOSettings changed
					ApplyIOSettings();
				}
				else if (Settings.BufferSettings.ReferenceEquals(e.Inner.Source, this.terminalSettings.Buffer))
				{
					// BufferSettings changed
					ApplyBufferSettings();
				}
				else if (Settings.DisplaySettings.ReferenceEquals(e.Inner.Source, this.terminalSettings.Display))
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
			this.rawTerminal = rawTerminal;

			this.rawTerminal.IOChanged          += new EventHandler(rawTerminal_IOChanged);
			this.rawTerminal.IOControlChanged   += new EventHandler(rawTerminal_IOControlChanged);
			this.rawTerminal.IOError            += new EventHandler<IOErrorEventArgs>(rawTerminal_IOError);

			this.rawTerminal.RawElementSent     += new EventHandler<RawElementEventArgs>(rawTerminal_RawElementSent);
			this.rawTerminal.RawElementReceived += new EventHandler<RawElementEventArgs>(rawTerminal_RawElementReceived);
			this.rawTerminal.RepositoryCleared  += new EventHandler<RepositoryEventArgs>(rawTerminal_RepositoryCleared);
		}

		#endregion

		#region Raw Terminal Events
		//==========================================================================================
		// Raw Terminal Events
		//==========================================================================================

		private void rawTerminal_IOChanged(object sender, EventArgs e)
		{
			OnIOChanged(e);
		}

		private void rawTerminal_IOControlChanged(object sender, EventArgs e)
		{
			OnIOControlChanged(e);
		}

		private void rawTerminal_IOError(object sender, IOErrorEventArgs e)
		{
			SerialPortErrorEventArgs serialPortErrorEventArgs = (e as SerialPortErrorEventArgs);
			if (serialPortErrorEventArgs != null)
			{
				// Handle serial port errors whenever possible.
				switch (serialPortErrorEventArgs.SerialPortError)
				{
					case System.IO.Ports.SerialError.Frame:    OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("RX FRAMING ERROR"));   break;
					case System.IO.Ports.SerialError.Overrun:  OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("RX BUFFER OVERRUN"));  break;
					case System.IO.Ports.SerialError.RXOver:   OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("RX BUFFER OVERFLOW")); break;
					case System.IO.Ports.SerialError.RXParity: OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error("RX PARITY ERROR"));    break;
					case System.IO.Ports.SerialError.TXFull:   OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.Error("TX BUFFER FULL"));     break;
					default:                                   OnIOError(e); break;
				}
			}
			else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Input))
			{
				OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.Error(e.Message));
			}
			else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Output))
			{
				OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.Error(e.Message));
			}
			else
			{
				OnIOError(e);
			}
		}

		private void rawTerminal_RawElementSent(object sender, RawElementEventArgs e)
		{
			OnRawElementSent(e);
			ProcessAndSignalRawElement(e.Element);
		}

		private void rawTerminal_RawElementReceived(object sender, RawElementEventArgs e)
		{
			OnRawElementReceived(e);
			ProcessAndSignalRawElement(e.Element);
		}

		private void rawTerminal_RepositoryCleared(object sender, RepositoryEventArgs e)
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
				lock (this.repositorySyncObj)
				{
					this.txRepository.Enqueue(elements);
					this.bidirRepository.Enqueue(elements);
				}

				if (!this.eventsSuspendedForReload)
					OnDisplayElementsSent(new DisplayElementsEventArgs(elements));
			}
			else
			{
				lock (this.repositorySyncObj)
				{
					this.bidirRepository.Enqueue(elements);
					this.rxRepository.Enqueue(elements);
				}

				if (!this.eventsSuspendedForReload)
					OnDisplayElementsReceived(new DisplayElementsEventArgs(elements));
			}
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsSent(DisplayElementsEventArgs e)
		{
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsReceived(DisplayElementsEventArgs e)
		{
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<DisplayElementsEventArgs>(DisplayElementsReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesProcessed(SerialDirection direction, List<DisplayLine> lines)
		{
			if (!this.eventsSuspendedForReload)
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
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesReceived(DisplayLinesEventArgs e)
		{
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<DisplayLinesEventArgs>(DisplayLinesReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(RepositoryEventArgs e)
		{
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<RepositoryEventArgs>(RepositoryCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryReloaded(RepositoryEventArgs e)
		{
			if (!this.eventsSuspendedForReload)
				EventHelper.FireSync<RepositoryEventArgs>(RepositoryReloaded, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
