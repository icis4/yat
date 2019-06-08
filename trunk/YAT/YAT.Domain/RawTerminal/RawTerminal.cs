//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY;
using MKY.Diagnostics;
using MKY.IO.Serial;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines a buffered serial interface. The buffers contain raw byte content,
	/// no formatting is done.
	/// </summary>
	public class RawTerminal : IDisposable, IDisposableEx
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string Undefined = "<Undefined>";

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

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		/// <remarks>
		/// Explicitly setting <see cref="EventHelper.DisposedTargetExceptionMode.Discard"/> to
		/// prevent the following issue:
		/// 
		/// <![CDATA[
		/// System.Reflection.TargetInvocationException was unhandled by user code
		///   Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///   Source=mscorlib
		///   StackTrace:
		///        bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///        bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///        bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///        bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///        bei System.Runtime.Remoting.Messaging.StackBuilderSink._PrivateProcessMessage(IntPtr md, Object[] args, Object server, Int32 methodPtr, Boolean fExecuteInContext, Object[]& outArgs)
		///        bei System.Runtime.Remoting.Messaging.StackBuilderSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		///   InnerException: 
		///        Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///        Source=mscorlib
		///        StackTrace:
		///             bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///             bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///             bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///             bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///             bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 370.
		///             bei YAT.Domain.RawTerminal.OnIOChanged(EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\RawTerminal\RawTerminal.cs:Zeile 792.
		///             bei YAT.Domain.RawTerminal.io_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\RawTerminal\RawTerminal.cs:Zeile 689.
		///        InnerException: 
		///             Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///             Source=mscorlib
		///             StackTrace:
		///                  bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///                  bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///                  bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///                  bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///                  bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 370.
		///                  bei YAT.Domain.Terminal.OnIOChanged(EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\Terminal\Terminal.cs:Zeile 3890.
		///                  bei YAT.Domain.Terminal.rawTerminal_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\Terminal\Terminal.cs:Zeile 3767.
		///             InnerException: 
		///                  Message=Invoke oder BeginInvoke kann für ein Steuerelement erst aufgerufen werden, wenn das Fensterhandle erstellt wurde.
		///                  Source=System.Windows.Forms
		///                  StackTrace:
		///                       bei System.Windows.Forms.Control.WaitForWaitHandle(WaitHandle waitHandle)
		///                       bei System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous)
		///                       bei System.Windows.Forms.Control.Invoke(Delegate method, Object[] args)
		///                       bei MKY.EventHelper.Item.InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 567.
		///                       bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 368.
		///                       bei YAT.Model.Terminal.OnIOChanged(EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 5246.
		///                       bei YAT.Model.Terminal.terminal_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 2479.
		/// ]]>
		/// 
		/// The terminals get properly closed, but apparently there may still be pending
		/// asynchronuos 'zombie' callback that later throw an exception. No true solution
		/// has been found.
		/// </remarks>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RawTerminal).FullName, disposedTargetException: EventHelper.DisposedTargetExceptionMode.Discard);

		private Settings.BufferSettings bufferSettings;

		private RawRepository txRepository;
		private RawRepository bidirRepository;
		private RawRepository rxRepository;
		private object repositorySyncObj = new object();

		private Settings.IOSettings ioSettings;
		private IIOProvider io;
		private object ioDataSyncObj = new object();

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
		public event EventHandler<EventArgs<RawChunk>> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> RawChunkReceived;

		/// <summary></summary>
		public event EventHandler<EventArgs<RepositoryType>> RepositoryCleared;

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
			AttachIO(IOFactory.CreateIO(ioSettings)); // Settings are applied by the factory.
		}

		/// <summary></summary>
		public RawTerminal(RawTerminal rhs, Settings.IOSettings ioSettings, Settings.BufferSettings bufferSettings)
		{
			this.txRepository    = new RawRepository(rhs.txRepository);
			this.bidirRepository = new RawRepository(rhs.bidirRepository);
			this.rxRepository    = new RawRepository(rhs.rxRepository);

			AttachBufferSettings(bufferSettings);

			AttachIOSettings(ioSettings);
			AttachIO(IOFactory.CreateIO(ioSettings)); // Settings are applied by the factory.
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				DebugEventManagement.DebugWriteAllEventRemains(this);
				this.eventHelper.DiscardAllEventsAndExceptions();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, I/O will already have been stopped in Stop()...
					if (this.io != null)
						this.io.Stop();

					// ...and objects will already have been detached and disposed of in Close():
					DetachAndDisposeIO();
					DetachIOSettings();
					DetachBufferSettings();
					DisposeRepositories();
				}

				// Set state to disposed:
				this.io = null;
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~RawTerminal()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
		public virtual bool IsTransmissive
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.io != null)
					return (this.io.IsTransmissive);
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
		// Start/Stop/Close
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool Start()
		{
			AssertNotDisposed();

			return (this.io.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertNotDisposed();

			this.io.Stop();
		}

		/// <summary></summary>
		public virtual void Close()
		{
			AssertNotDisposed();

			DetachAndDisposeIO();
			DetachIOSettings();
			DetachBufferSettings();
			DisposeRepositories();
		}

		//------------------------------------------------------------------------------------------
		// Send
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void Send(byte[] data)
		{
			AssertNotDisposed();

			if (IsTransmissive)
			{
				if (!this.io.Send(data))
				{
					string message;
					if (data.Length <= 1)
						message = data.Length + " byte could not be sent as the underlying I/O instance is not ready!"; // Using "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
					else
						message = data.Length + " bytes could not be sent as the underlying I/O instance is not ready!"; // Using "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.

					OnIOError(new IOErrorEventArgs(IOErrorSeverity.Severe, IODirection.Tx, message));
				}
			}
			else
			{
				string message;
				if (data.Length <= 1)
					message = data.Length + " byte not sent anymore as terminal has been closed."; // Using "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.
				else
					message = data.Length + " bytes not sent anymore as terminal has been closed."; // Using "byte" instead of "octet" as that is more common, and .NET uses "byte" as well.

				OnIOError(new IOErrorEventArgs(IOErrorSeverity.Acceptable, IODirection.Tx, message));
			}
		}

		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual List<RawChunk> RepositoryToChunks(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			List<RawChunk> l = null;
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:      /* Nothing to do. */             break;

					case RepositoryType.Tx:    l = this.txRepository   .ToChunks(); break;
					case RepositoryType.Bidir: l = this.bidirRepository.ToChunks(); break;
					case RepositoryType.Rx:    l = this.rxRepository   .ToChunks(); break;

					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			return (l);
		}

		/// <remarks>
		/// \todo:
		/// Currently, all repositories are cleared in any case. That is because repositories are
		/// always reloaded from bidir. Without clearing all, contents reappear after a change to
		/// the settings, e.g. switching radix.
		/// </remarks>
		public virtual void ClearRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
			////\todo:
			////switch (repositoryType)
			////{
			////	case RepositoryType.None:      /* Nothing to do. */      break;
			////
			////	case RepositoryType.Tx:    this.txRepository   .Clear(); break;
			////	case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
			////	case RepositoryType.Rx:    this.rxRepository   .Clear(); break;
			////
			////	default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			////}

				this.txRepository.Clear();
				this.bidirRepository.Clear();
				this.rxRepository.Clear();
			}

			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Tx));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Bidir));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Rx));
		}

		/// <remarks>
		/// \todo: See <see cref="ClearRepository"/> above.
		/// </remarks>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();

			/* \todo:
			call ClearRepository(RepositoryType.Tx)
			call ClearRepository(RepositoryType.Bidir)
			call ClearRepository(RepositoryType.Rx)
			*/

			lock (this.repositorySyncObj)
			{
				this.txRepository.Clear();
				this.bidirRepository.Clear();
				this.rxRepository.Clear();
			}

			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Tx));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Bidir));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Rx));
		}

		/// <summary></summary>
		public virtual string RepositoryToDiagnosticsString(RepositoryType repositoryType, string indent)
		{
			AssertNotDisposed();

			string s = null;
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.None:        /* Nothing to do. */                            break;

					case RepositoryType.Tx:    s = this.txRepository   .ToDiagnosticsString(indent); break;
					case RepositoryType.Bidir: s = this.bidirRepository.ToDiagnosticsString(indent); break;
					case RepositoryType.Rx:    s = this.rxRepository   .ToDiagnosticsString(indent); break;

					default: throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			return (s);
		}

		private void DisposeRepositories()
		{
			lock (this.repositorySyncObj)
			{
				if (this.txRepository != null)
				{
					this.txRepository.Clear();
					this.txRepository = null;
				}

				if (this.bidirRepository != null)
				{
					this.bidirRepository.Clear();
					this.bidirRepository = null;
				}

				if (this.rxRepository != null)
				{
					this.rxRepository.Clear();
					this.rxRepository = null;
				}
			}
		}

		#endregion

		#region Settings Methods
		//==========================================================================================
		// Settings Methods
		//==========================================================================================

		private void AttachBufferSettings(Settings.BufferSettings bufferSettings)
		{
			if (ReferenceEquals(this.bufferSettings, bufferSettings))
				return;

			if (this.bufferSettings != null)
				DetachBufferSettings();

			this.bufferSettings = bufferSettings;
			this.bufferSettings.Changed += bufferSettings_Changed;
		}

		private void DetachBufferSettings()
		{
			if (this.bufferSettings != null)
			{
				this.bufferSettings.Changed -= bufferSettings_Changed;
				this.bufferSettings = null;
			}
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
			if (ReferenceEquals(this.ioSettings, ioSettings))
				return;

			if (this.ioSettings != null)
				DetachIOSettings();

			this.ioSettings = ioSettings;
			this.ioSettings.Changed += ioSettings_Changed;
		}

		private void DetachIOSettings()
		{
			if (this.ioSettings != null)
			{
				this.ioSettings.Changed -= ioSettings_Changed;
				this.ioSettings = null;
			}
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

		#region I/O Methods
		//==========================================================================================
		// I/O Methods
		//==========================================================================================

		private void AttachIO(IIOProvider io)
		{
			this.io = io;
			{
				this.io.IOChanged        += io_IOChanged;
				this.io.IOControlChanged += io_IOControlChanged;
				this.io.IOError          += io_IOError;
				this.io.DataReceived     += io_DataReceived;
				this.io.DataSent         += io_DataSent;
			}
		}

		private void DetachAndDisposeIO()
		{
			if (this.io != null)
			{
				this.io.IOChanged        -= io_IOChanged;
				this.io.IOControlChanged -= io_IOControlChanged;
				this.io.IOError          -= io_IOError;
				this.io.DataReceived     -= io_DataReceived;
				this.io.DataSent         -= io_DataSent;

				this.io.Dispose();
				this.io = null;
			}
		}

		#endregion

		#region I/O Events
		//==========================================================================================
		// I/O Events
		//==========================================================================================

		private void io_IOChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIOChanged(EventArgs.Empty);
		}

		private void io_IOControlChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIOControlChanged(EventArgs.Empty);
		}

		private void io_IOError(object sender, MKY.IO.Serial.IOErrorEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			var spe = (e as MKY.IO.Serial.SerialPortErrorEventArgs);
			if (spe == null)
				OnIOError(new IOErrorEventArgs((IOErrorSeverity)e.Severity, (IODirection)e.Direction, e.Message, e.TimeStamp));
			else
				OnIOError(new SerialPortErrorEventArgs((IOErrorSeverity)spe.Severity, (IODirection)spe.Direction, spe.Message, spe.SerialPortError, spe.TimeStamp));
		}

		/// <remarks>
		/// Note that this I/O event has a calling contract of:
		///   [CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		/// Therefore, no additional synchronization to prevent race condition is required here.
		/// </remarks>
		private void io_DataReceived(object sender, DataReceivedEventArgs e)
		{
			// Synchronize the underlying Tx and Rx callbacks to prevent mix-ups. But attention,
			// do not simply lock() the 'ioDataSyncObj'. Instead, try to get the lock periodically,
			// but quit = discard the event as soon as the object got disposed of:

			while (!IsDisposed) // Ensure not to handle events during closing anymore.
			{
				if (Monitor.TryEnter(this.ioDataSyncObj, staticRandom.Next(50, 200)))
				{
					try
					{
						var re = new RawChunk(e.Data, e.TimeStamp, e.PortStamp, IODirection.Rx);
						lock (this.repositorySyncObj)
						{
							this.rxRepository   .Enqueue(re); // 'RawChunk' object is immutable, subsequent use is OK.
							this.bidirRepository.Enqueue(re); // 'RawChunk' object is immutable, subsequent use is OK.
						}
						OnRawChunkReceived(new EventArgs<RawChunk>(re)); // 'RawChunk' object is immutable, subsequent use is OK.
					}
					finally
					{
						Monitor.Exit(this.ioDataSyncObj);
					}

					break; // Successfully entered the lock and processed the event => break the while-loop.
				} // Monitor.TryEnter()
			} // while (!IsDisposed)
		}

		/// <remarks>
		/// Note that this I/O event has a calling contract of:
		///   [CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true)]
		/// Therefore, no additional synchronization to prevent race condition is required here.
		/// </remarks>
		private void io_DataSent(object sender, DataSentEventArgs e)
		{
			// Synchronize the underlying Tx and Rx callbacks to prevent mix-ups. But attention,
			// do not simply lock() the 'ioDataSyncObj'. Instead, try to get the lock periodically,
			// but quit = discard the event as soon as the object got disposed of:

			while (!IsDisposed) // Ensure not to handle events during closing anymore.
			{
				if (Monitor.TryEnter(this.ioDataSyncObj, staticRandom.Next(50, 200)))
				{
					try
					{
						var re = new RawChunk(e.Data, e.TimeStamp, e.PortStamp, IODirection.Tx);
						lock (this.repositorySyncObj)
						{
							this.txRepository   .Enqueue(re); // 'RawChunk' object is immutable, subsequent use is OK.
							this.bidirRepository.Enqueue(re); // 'RawChunk' object is immutable, subsequent use is OK.
						}
						OnRawChunkSent(new EventArgs<RawChunk>(re)); // 'RawChunk' object is immutable, subsequent use is OK.
					}
					finally
					{
						Monitor.Exit(this.ioDataSyncObj);
					}

					break; // Successfully entered the lock and processed the event => break the while-loop.
				} // Monitor.TryEnter()
			} // while (!IsDisposed)
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkSent(EventArgs<RawChunk> e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<EventArgs<RawChunk>>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkReceived(EventArgs<RawChunk> e)
		{
			if (IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<EventArgs<RawChunk>>(RawChunkReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<RepositoryType> e)
		{
			this.eventHelper.RaiseSync<EventArgs<RepositoryType>>(RepositoryCleared, this, e);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// See below why AssertNotDisposed() is not called on such basic method!

			return (ToDiagnosticsString("")); // No 'real' ToString() method required yet.
		}

		/// <summary></summary>
		public virtual string ToDiagnosticsString(string indent)
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			var sb = new StringBuilder();
			lock (this.repositorySyncObj)
			{
				sb.AppendLine(indent + "> TxRepository: ");
				sb.Append(this.txRepository.ToDiagnosticsString(indent + "   "));    // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> BidirRepository: ");
				sb.Append(this.bidirRepository.ToDiagnosticsString(indent + "   ")); // Repository will add 'NewLine'.

				sb.AppendLine(indent + "> RxRepository: ");
				sb.Append(this.rxRepository.ToDiagnosticsString(indent + "   "));    // Repository will add 'NewLine'.

				sb.Append(indent + "> I/O: " + this.io.ToString());
			}
			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortIOString()
		{
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method!

			if      (this.io != null)
				return (this.io.ToString());
			else if (this.ioSettings != null)
				return (this.ioSettings.ToShortIOString());
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
