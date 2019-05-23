﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
using System.Globalization;
using System.Text;
using System.Threading;

using MKY;
using MKY.Diagnostics;
using MKY.Text;

#endregion

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
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string RxFramingErrorString        = "RX FRAMING ERROR";
		private const string RxBufferOverrunErrorString  = "RX BUFFER OVERRUN";
		private const string RxBufferOverflowErrorString = "RX BUFFER OVERFLOW";
		private const string RxParityErrorString         = "RX PARITY ERROR";
		private const string TxBufferFullErrorString     = "TX BUFFER FULL";

		private const string Undefined = "<Undefined>";

		#endregion

		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string SerialPortHelp =
			@"For serial ports (COM), if one of the following error conditions occurs, the according error indication will be shown in the terminal window:" + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxFramingErrorString + ">" + Environment.NewLine +
			@"An input framing error occurs when the last bit received is not a stop bit. This may occur due to a timing error. You will most commonly encounter a framing error when the speed at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxBufferOverrunErrorString + ">" + Environment.NewLine +
			@"An input overrun error occurs when the input gets out of synch. The next character will be lost and the input will be re-synch'd." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxBufferOverflowErrorString + ">" + Environment.NewLine +
			@"An input overflow occurs when there is no more space in the input buffer, i.e. the serial driver, the operating system or YAT doesn't manage to process the incoming data fast enough." + Environment.NewLine +
			Environment.NewLine +
			@"<" + RxParityErrorString + ">" + Environment.NewLine +
			@"An input parity error occurs when a parity check is enabled but the parity bit mismatches. You will most commonly encounter a parity error when the parity setting at which the data is being sent is different to that of what you have YAT set to receive it at." + Environment.NewLine +
			Environment.NewLine +
			@"<" + TxBufferFullErrorString + ">" + Environment.NewLine +
			@"An output buffer full error occurs when there is no more space in the output buffer, i.e. the serial driver, the operating system or YAT doesn't manage to send the data fast enough.";

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Random staticRandom = new Random(RandomEx.NextPseudoRandomSeed());

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Settings.TerminalSettings terminalSettings;

		private RawTerminal rawTerminal;

		private Queue<SendItem> sendQueue = new Queue<SendItem>();

		private bool sendThreadRunFlag;
		private AutoResetEvent sendThreadEvent;
		private Thread sendThread;

		private object repositorySyncObj = new object();
		private DisplayRepository txRepository;
		private DisplayRepository bidirRepository;
		private DisplayRepository rxRepository;

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

		////this.eventsSuspendedForReload = false;

			CreateAndStartSendThread();
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

			CreateAndStartSendThread();
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, the terminal will already have been stopped in Stop().
					if (this.rawTerminal != null)
						this.rawTerminal.Dispose();

					// In the 'normal' case, the send thread will already have been stopped in Close().
					StopSendThread();

					if (this.sendThreadEvent != null)
						this.sendThreadEvent.Close();
				}

				// Set state to disposed:
				this.rawTerminal = null;
				this.sendThreadEvent = null;
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~Terminal()
		{
			Dispose(false);
		}

		/// <summary></summary>
		public bool IsDisposed
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

		#region Send Thread
		//------------------------------------------------------------------------------------------
		// Send Thread
		//------------------------------------------------------------------------------------------

		private void CreateAndStartSendThread()
		{
			// Ensure that thread has stopped after the last stop request:
			int timeoutCounter = 0;
			while (this.sendThread != null)
			{
				Thread.Sleep(1);

				if (++timeoutCounter >= 3000)
					throw (new TimeoutException("Thread hasn't properly stopped"));
			}

			// Do not yet enforce that thread events have been disposed because that may result in
			// deadlock. Further investigation is required in order to further improve the behaviour
			// on Stop()/Dispose().

			// Start thread:
			this.sendThreadRunFlag = true;
			this.sendThreadEvent = new AutoResetEvent(false);
			this.sendThread = new Thread(new ThreadStart(SendThread));
			this.sendThread.Start();
		}

		private void StopSendThread()
		{
			this.sendThreadRunFlag = false;

			// Ensure that thread has stopped after the stop request:
			int timeoutCounter = 0;
			while (this.sendThread != null)
			{
				this.sendThreadEvent.Set();

				Thread.Sleep(1);

				if (++timeoutCounter >= 3000)
					throw (new TimeoutException("Thread hasn't properly stopped"));
			}
		}

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

		#region Methods > Start/Stop/Close
		//------------------------------------------------------------------------------------------
		// Methods > Start/Stop/Close
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			// Do not clear the send queue, it already got cleared when stopping. This setup
			// potentially allows to call Send() and buffer data before starting the terminal.

			return (this.rawTerminal.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			lock (this.sendQueue)
			{
				this.sendQueue.Clear();
			}

			this.rawTerminal.Stop();
		}

		/// <summary></summary>
		public virtual void Close()
		{
			AssertNotDisposed();

			StopSendThread();
		}

		#endregion

		#region Methods > Send
		//------------------------------------------------------------------------------------------
		// Methods > Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			Send(new RawSendItem(data));
		}

		/// <summary></summary>
		public virtual void Send(string data)
		{
			Send(new ParsableSendItem(data));
		}

		/// <summary></summary>
		public virtual void SendLine(string data)
		{
			Send(new ParsableSendItem(data, true));
		}

		/// <remarks>
		/// This method shall not be overridden. All send items shall be enqueued using this
		/// method, but inheriting terminals can override <see cref="ProcessSendItem"/> instead.
		/// </remarks>
		protected void Send(SendItem item)
		{
			AssertNotDisposed();

			lock (this.sendQueue)
			{
				this.sendQueue.Enqueue(item);
			}

			// Signal send thread:
			this.sendThreadEvent.Set();
		}

		/// <summary>
		/// Asynchronously manage outgoing send requests to ensure that send events are not
		/// invoked on the same thread that triggered the send operation.
		/// Also, the mechanism implemented below reduces the amount of events that are propagated
		/// to the main application. Small chunks of sent data would generate many events in
		/// <see cref="Send(SendItem)"/>. However, since <see cref="OnDisplayElementProcessed"/>
		/// synchronously invokes the event, it will take some time until the send queue is checked
		/// again. During this time, no more new events are invoked, instead, outgoing data is
		/// buffered.
		/// </summary>
		/// <remarks>
		/// Will be signaled by <see cref="Send(SendItem)"/> method above.
		/// </remarks>
		private void SendThread()
		{
			WriteDebugMessageLine("SendThread() has started.");

			// Outer loop, requires another signal.
			while (this.sendThreadRunFlag && !IsDisposed)
			{
				try
				{
					// WaitOne() might wait forever in case the underlying I/O provider crashes,
					// or if the overlying client isn't able or forgets to call Stop() or Dispose(),
					// therefore, only wait for a certain period and then poll the run flag again.
					if (!this.sendThreadEvent.WaitOne(staticRandom.Next(50, 200)))
						continue;
				}
				catch (AbandonedMutexException ex)
				{
					// The mutex should never be abandoned, but in case it nevertheless happens,
					// at least output a debug message and gracefully exit the thread.
					DebugEx.WriteException(GetType(), ex, "An 'AbandonedMutexException' occurred in SendThread()");
					break;
				}

				// Inner loop, runs as long as there is data in the send queue.
				// Ensure not to forward any events during closing anymore.
				while (this.sendThreadRunFlag && IsReadyToSend && !IsDisposed)
				{
					SendItem[] pendingItems;
					lock (this.sendQueue)
					{
						if (this.sendQueue.Count <= 0)
							break; // Let other threads do their job and wait until signaled again.

						pendingItems = this.sendQueue.ToArray();
						this.sendQueue.Clear();
					}

					foreach (SendItem si in pendingItems)
						ProcessSendItem(si);
				}
			}

			this.sendThread = null;

			// Do not Close() and de-reference the corresponding event as it may be Set() again
			// right now by another thread, e.g. during closing.

			WriteDebugMessageLine("SendThread() has terminated.");
		}

		/// <summary></summary>
		protected virtual void ProcessSendItem(SendItem item)
		{
			RawSendItem rsi = item as RawSendItem;
			if (rsi != null)
			{
				ProcessRawSendItem(rsi);
			}
			else
			{
				ParsableSendItem psi = item as ParsableSendItem;
				if (psi != null)
					ProcessParsableSendItem(psi);
				else
					throw (new InvalidOperationException("Invalid send item type " + item.GetType()));
			}
		}

		/// <summary></summary>
		protected virtual void ProcessRawSendItem(RawSendItem item)
		{
			// Nothing to further process, simply forward:
			ForwardDataToRawTerminal(item.Data);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
		protected virtual void ProcessParsableSendItem(ParsableSendItem item)
		{
			bool lineDelay = false;

			Parser.Parser p = new Parser.Parser(TerminalSettings.IO.Endianness);
			foreach (Parser.Result r in p.Parse(item.Data, Parser.Modes.All))
			{
				Parser.ByteArrayResult bar = r as Parser.ByteArrayResult;
				if (bar != null)
				{
					ForwardDataToRawTerminal(bar.ByteArray);
				}
				else
				{
					Parser.KeywordResult kr = r as Parser.KeywordResult;
					if (kr != null)
					{
						switch (kr.Keyword)
						{
							// Process end-of-line keywords:
							case Parser.Keyword.LineDelay:
							{
								lineDelay = true;
								break;
							}

							// Process in-line keywords:
							default:
							{
								ProcessInLineKeywords(kr);
								break;
							}
						}
					}
				}
			}

			// Finalize the line.
			if (lineDelay)
				Thread.Sleep(TerminalSettings.Send.DefaultLineDelay);
		}

		/// <summary></summary>
		protected virtual void ProcessInLineKeywords(Parser.KeywordResult result)
		{
			switch (result.Keyword)
			{
				case Parser.Keyword.Clear:
				{
					this.ClearRepositories();
					break;
				}

				case Parser.Keyword.Delay:
				{
					Thread.Sleep(this.terminalSettings.Send.DefaultDelay);
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
						OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError("Break is only supported on serial COM ports"));
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
						OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError("Break is only supported on serial COM ports"));
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
						OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError("Break is only supported on serial COM ports"));
					}
					break;
				}

				default:
				{
					// Add space if necessary.
					if (ElementsAreSeparate(SerialDirection.Tx))
						OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.Space());

					OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError((Parser.KeywordEx)(((Parser.KeywordResult)result).Keyword) + " keyword is not yet supported"));
					break;
				}
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private member 'rawTerminal'.
		/// </remarks>
		protected void ForwardDataToRawTerminal(byte[] data)
		{
			this.rawTerminal.Send(data);
		}

		/// <summary></summary>
		public void ManuallyEnqueueRawOutgoingDataWithoutSendingIt(byte[] data)
		{
			AssertNotDisposed();

			this.rawTerminal.ManuallyEnqueueRawOutgoingDataWithoutSendingIt(data);
		}

		#endregion

		#region Methods > Element Processing
		//------------------------------------------------------------------------------------------
		// Methods > Element Processing
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, SerialDirection d)
		{
			if (d == SerialDirection.Tx)
				return (ByteToElement(b, d, this.terminalSettings.Display.TxRadix));
			else
				return (ByteToElement(b, d, this.terminalSettings.Display.RxRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
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
							text += " <" + b.ToString("X2", CultureInfo.InvariantCulture) + "h>";
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
				return (new DisplayElement.IOError(d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToAsciiString(byte b)
		{
			if ((b == 0x09) && !this.terminalSettings.CharReplace.ReplaceTab)
				return ("\t");
			else
				return ("<" + Ascii.ConvertToMnemonic(b) + ">");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
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
						return (b.ToString("D3", CultureInfo.InvariantCulture) + "d");
					else
						return (b.ToString("D3", CultureInfo.InvariantCulture));
				}
				case Radix.Hex:
				{
					if (this.terminalSettings.Display.ShowRadix)
						return (b.ToString("X2", CultureInfo.InvariantCulture) + "h");
					else
						return (b.ToString("X2", CultureInfo.InvariantCulture));
				}
				default: throw (new ArgumentOutOfRangeException("r", r, "Invalid radix"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(SerialDirection d)
		{
			if (d == SerialDirection.Tx)
				return (ElementsAreSeparate(this.terminalSettings.Display.TxRadix));
			else
				return (ElementsAreSeparate(this.terminalSettings.Display.RxRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
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

			elements.AddRange(dl.Clone()); // Clone elements because they are needed again a line below.
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

			// Clear repository:
			ClearMyRepository(RepositoryType.Tx);
			ClearMyRepository(RepositoryType.Bidir);
			ClearMyRepository(RepositoryType.Rx);
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Tx));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Bidir));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Rx));

			// Reload repository:
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
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;
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
					case RepositoryType.Tx:    return (this.txRepository   .DataCount);
					case RepositoryType.Bidir: return (this.bidirRepository.DataCount);
					case RepositoryType.Rx:    return (this.rxRepository   .DataCount);
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
					case RepositoryType.Tx:    return (this.txRepository   .Count);
					case RepositoryType.Bidir: return (this.bidirRepository.Count);
					case RepositoryType.Rx:    return (this.rxRepository   .Count);
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual List<DisplayElement> RepositoryToDisplayElements(RepositoryType repository)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToElements());
					case RepositoryType.Bidir: return (this.bidirRepository.ToElements());
					case RepositoryType.Rx:    return (this.rxRepository   .ToElements());
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
					case RepositoryType.Tx:    return (this.txRepository.   ToLines());
					case RepositoryType.Bidir: return (this.bidirRepository.ToLines());
					case RepositoryType.Rx:    return (this.rxRepository   .ToLines());
					default: throw (new ArgumentOutOfRangeException("repository", repository, "Invalid repository type"));
				}
			}
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository)
		{
			return (RepositoryToString(repository, ""));
		}

		/// <summary></summary>
		public virtual string RepositoryToString(RepositoryType repository, string indent)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repository)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToString(indent));
					case RepositoryType.Bidir: return (this.bidirRepository.ToString(indent));
					case RepositoryType.Rx:    return (this.rxRepository   .ToString(indent));
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

			ReloadRepositories();
		}

		private void ApplyDisplaySettings()
		{
			this.txRepository.Capacity    = this.terminalSettings.Display.TxMaxLineCount;
			this.bidirRepository.Capacity = this.terminalSettings.Display.BidirMaxLineCount;
			this.rxRepository.Capacity    = this.terminalSettings.Display.RxMaxLineCount;

			ReloadRepositories();
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
					case System.IO.Ports.SerialError.Frame:    OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.IOError(RxFramingErrorString));        break;
					case System.IO.Ports.SerialError.Overrun:  OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.IOError(RxBufferOverrunErrorString));  break;
					case System.IO.Ports.SerialError.RXOver:   OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.IOError(RxBufferOverflowErrorString)); break;
					case System.IO.Ports.SerialError.RXParity: OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.IOError(RxParityErrorString));         break;
					case System.IO.Ports.SerialError.TXFull:   OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError(TxBufferFullErrorString));     break;
					default:                                   OnIOError(e); break;
				}
			}
			else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Input))
			{
				OnDisplayElementProcessed(SerialDirection.Rx, new DisplayElement.IOError(e.Message));
			}
			else if ((e.Severity == IOErrorSeverity.Acceptable) && (e.Direction == IODirection.Output))
			{
				OnDisplayElementProcessed(SerialDirection.Tx, new DisplayElement.IOError(e.Message));
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
			elements.Add(element); // No clone needed as the element must be created when calling this event method.
			OnDisplayElementsProcessed(direction, elements);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsProcessed(SerialDirection direction, DisplayElementCollection elements)
		{
			if (direction == SerialDirection.Tx)
			{
				lock (this.repositorySyncObj)
				{
					this.txRepository   .Enqueue(elements.Clone()); // Clone elements because they are needed again below.
					this.bidirRepository.Enqueue(elements.Clone()); // Clone elements because they are needed again below.
				}

				if (!this.eventsSuspendedForReload)
					OnDisplayElementsSent(new DisplayElementsEventArgs(elements)); // No clone needed as the elements must be created when calling this event method.
			}
			else
			{
				lock (this.repositorySyncObj)
				{
					this.bidirRepository.Enqueue(elements.Clone()); // Clone elements because they are needed again below.
					this.rxRepository   .Enqueue(elements.Clone()); // Clone elements because they are needed again below.
				}

				if (!this.eventsSuspendedForReload)
					OnDisplayElementsReceived(new DisplayElementsEventArgs(elements)); // No clone needed as the elements must be created when calling this event method.
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

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			AssertNotDisposed();

			StringBuilder sb = new StringBuilder();
			lock (this.repositorySyncObj)
			{
				sb.AppendLine(indent + "> Settings: " + this.terminalSettings);

				sb.AppendLine(indent + "> RawTerminal: ");
				sb.AppendLine(this.rawTerminal.ToString(indent + "   "));

				sb.AppendLine(indent + "> TxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> BidirRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> RxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.
			}
			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortIOString()
		{
			if (this.rawTerminal != null)
				return (this.rawTerminal.ToShortIOString());
			else
				return (Undefined);
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
		{
			Debug.WriteLine(GetType() + " '" + ToShortIOString() + "': " + message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================