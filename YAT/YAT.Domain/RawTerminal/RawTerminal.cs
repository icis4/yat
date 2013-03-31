//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY;
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
				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, Stop() will have already been called...
					Stop();

					// ...and the I/O provider will have stopped as well.
					if (this.io != null)
						this.io.Dispose();
				}

				// Set state to disposed:
				this.io = null;
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~RawTerminal()
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
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsStopped);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsStarted);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsOpen
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsOpen);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsConnected
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsConnected);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSend
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsReadyToSend);
				else
					return (false);
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

			if (IsReadyToSend)
			{
				// Enqueue and signal the send operation BEFORE actually sending the data. This
				// ensures that an outgoing request is shown above the corresponding incoming
				// response in ther terminal monitor. // See comments in AttachIO() for more
				// information.
				RawElement re = new RawElement(data, SerialDirection.Tx);
				lock (this.repositorySyncObj)
				{
					this.txRepository   .Enqueue(re.Clone()); // Clone elementas it is needed again below.
					this.bidirRepository.Enqueue(re.Clone()); // Clone elementas it is needed again below.
				}
				OnRawElementSent(new RawElementEventArgs(re));

				// Then send the data using the active I/O provider:
				this.io.Send(data);
			}
			else
			{
				OnIOError(new IOErrorEventArgs(IOErrorSeverity.Severe, IODirection.Output, "No data can currently be sent!"));
			}
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
					case RepositoryType.Tx:    l = this.txRepository   .ToElements(); break;
					case RepositoryType.Bidir: l = this.bidirRepository.ToElements(); break;
					case RepositoryType.Rx:    l = this.rxRepository   .ToElements(); break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
			}
			return (l);
		}

		/// <remarks>
		/// \todo:
		/// Currently, all repositories are cleared in any case. That is because repositories are
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
					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
				OnRepositoryCleared(new RepositoryEventArgs(repositoryType));*/

				this.txRepository.Clear();
				this.bidirRepository.Clear();
				this.rxRepository.Clear();
			} // lock (this.repositorySyncObj)

			// Do not fire event within lock to prevent deadlocks between Terminal and RawTerminal.
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Tx));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Bidir));
			OnRepositoryCleared(new RepositoryEventArgs(RepositoryType.Rx));
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
					case RepositoryType.Tx:    s = this.txRepository   .ToString(indent); break;
					case RepositoryType.Bidir: s = this.bidirRepository.ToString(indent); break;
					case RepositoryType.Rx:    s = this.rxRepository   .ToString(indent); break;
					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Unknown repository type"));
				}
			}

			return (s);
		}

		/// <summary></summary>
		public void ManuallyEnqueueRawOutgoingDataWithoutSendingIt(byte[] data)
		{
			AssertNotDisposed();

			RawElement re = new RawElement(data, SerialDirection.Tx);
			lock (this.repositorySyncObj)
			{
				this.txRepository.Enqueue(re.Clone());    // Clone elementas it is needed again below.
				this.bidirRepository.Enqueue(re.Clone()); // Clone elementas it is needed again below.
			}
			OnRawElementSent(new RawElementEventArgs(re));
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

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For potential future use.")]
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

		private void AttachIO(IIOProvider io)
		{
			if (IIOProvider.ReferenceEquals(this.io, io))
				return;

			if (this.io != null)
				DetachIO();

			this.io = io;
			this.io.IOChanged        += new EventHandler(io_IOChanged);
			this.io.IOControlChanged += new EventHandler(io_IOControlChanged);
			this.io.IOError          += new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
			this.io.DataReceived     += new EventHandler<DataReceivedEventArgs>(io_DataReceived);

			// The 'DataSent' event is not used because it would not allow to properly synchronize
			// outgoing with incoming data. In most applications that isn't needed, but in case of
			// YAT it is a major feature that outgoing requests and incoming responses are properly
			// displayed line-after-line. Using the 'DataSent' event could lead to data races between
			// the threads which process the outgoing and incoming data events, and thus lead to mixed
			// up displaying of the lines.
			// Instead of the 'DataSent' event, this raw terminal fills outgoing data directly into
			// the Tx and Bidir repositories, before that the data is actually being sent, relying
			// on the underlying I/O provider to eventually send the data.
			// 
			// Additional advantage of this approach:
			//  > Tx data is shown even if XOn/XOff or some other flow control mechanism is active
			//
			// Disadvantage of this approach:
			//  > Time stamp information of the outgoing data is more related to the time when the
			//    user triggered the send operation than to the time when the data actually left
		}

		private void DetachIO()
		{
			this.io.IOChanged        -= new EventHandler(io_IOChanged);
			this.io.IOControlChanged -= new EventHandler(io_IOControlChanged);
			this.io.IOError          -= new EventHandler<MKY.IO.Serial.IOErrorEventArgs>(io_IOError);
			this.io.DataReceived     -= new EventHandler<DataReceivedEventArgs>(io_DataReceived);
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

		private void io_IOError(object sender, MKY.IO.Serial.IOErrorEventArgs e)
		{
			MKY.IO.Serial.SerialPortErrorEventArgs serialPortErrorEventArgs = (e as MKY.IO.Serial.SerialPortErrorEventArgs);
			if (serialPortErrorEventArgs == null)
				OnIOError(new IOErrorEventArgs((IOErrorSeverity)e.Severity, (IODirection)e.Direction, e.Message));
			else
				OnIOError(new SerialPortErrorEventArgs(serialPortErrorEventArgs.Message, serialPortErrorEventArgs.SerialPortError));
		}

		/// <remarks>
		/// Note that this I/O event has a calling contract of:
		///   [CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		/// Therefore, no additional synchronization or locking is required here.
		/// </remarks>
		private void io_DataReceived(object sender, DataReceivedEventArgs e)
		{
			RawElement re = new RawElement(e.Data, SerialDirection.Rx, e.TimeStamp);

			lock (this.repositorySyncObj)
			{
				this.rxRepository   .Enqueue(re.Clone()); // Clone elementas it is needed again below.
				this.bidirRepository.Enqueue(re.Clone()); // Clone elementas it is needed again below.
			}

			// Do not fire event within lock to prevent deadlocks between Terminal and RawTerminal.
			OnRawElementReceived(new RawElementEventArgs(re));
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
				sb.AppendLine(indent + "> TxRepository: ");
				sb.Append(this.txRepository.ToString(indent + "   "));    // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> BidirRepository: ");
				sb.Append(this.bidirRepository.ToString(indent + "   ")); // Repository will add 'NewLine'.
				
				sb.AppendLine(indent + "> RxRepository: ");
				sb.Append(this.rxRepository.ToString(indent + "   "));    // Repository will add 'NewLine'.
				
				sb.Append(indent + "> I/O: " + this.io.ToString());
			}
			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortIOString()
		{
			if      (this.io != null)
				return (this.io.ToString());
			else if (this.ioSettings != null)
				return (this.ioSettings.ToShortIOString());
			else
				return (Undefined);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
