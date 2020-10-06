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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of receiving:
////#define DEBUG_DATA_RECEIVED

	// Enable debugging of sending:
////#define DEBUG_DATA_SENT

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
//// 'System.Diagnostics' is explicitly used for preventing ambiguity between 'MKY.IO.Serial.DataReceivedEventArgs' and 'System.Diagnostics.DataReceivedEventArgs'.
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

using MKY;
using MKY.Contracts;
using MKY.IO.Serial;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// Defines a buffered serial interface. The buffers contain raw byte content,
	/// no formatting is done.
	/// </summary>
	public class RawTerminal : DisposableBase
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticInstanceCounter;
		private static Random staticRandom = new Random(RandomEx.NextRandomSeed());

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		/// <remarks> \remind (2019-08-22 / MKY)
		///
		/// Explicitly setting <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// to handle/workaround the following issue:
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
		///   InnerException 1:
		///        Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///        Source=mscorlib
		///        StackTrace:
		///             bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///             bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///             bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///             bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///             bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 370.
		///             bei YAT.Domain.RawTerminal.OnIOChanged(EventArgs<DateTime> e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\RawTerminal\RawTerminal.cs:Zeile 792.
		///             bei YAT.Domain.RawTerminal.io_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\RawTerminal\RawTerminal.cs:Zeile 689.
		///        InnerException 2:
		///             Message=Ein Aufrufziel hat einen Ausnahmefehler verursacht.
		///             Source=mscorlib
		///             StackTrace:
		///                  bei System.RuntimeMethodHandle._InvokeMethodFast(Object target, Object[] arguments, SignatureStruct& sig, MethodAttributes methodAttributes, RuntimeTypeHandle typeOwner)
		///                  bei System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
		///                  bei System.Delegate.DynamicInvokeImpl(Object[] args)
		///                  bei MKY.EventHelper.Item.InvokeOnCurrentThread(Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 595.
		///                  bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 370.
		///                  bei YAT.Domain.Terminal.OnIOChanged(EventArgs<DateTime> e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\Terminal\Terminal.cs:Zeile 3890.
		///                  bei YAT.Domain.Terminal.rawTerminal_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Domain\Terminal\Terminal.cs:Zeile 3767.
		///             InnerException 3:
		///                  Message=Invoke oder BeginInvoke kann für ein Steuerelement erst aufgerufen werden, wenn das Fensterhandle erstellt wurde.
		///                  Source=System.Windows.Forms
		///                  StackTrace:
		///                       bei System.Windows.Forms.Control.WaitForWaitHandle(WaitHandle waitHandle)
		///                       bei System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous)
		///                       bei System.Windows.Forms.Control.Invoke(Delegate method, Object[] args)
		///                       bei MKY.EventHelper.Item.InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 567.
		///                       bei MKY.EventHelper.Item.RaiseSync(Delegate eventDelegate, Object[] args) in D:\Workspace\YAT\Trunk\MKY\MKY\EventHelper.cs:Zeile 368.
		///                       bei YAT.Model.Terminal.OnIOChanged(EventArgs<DateTime> e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 5246.
		///                       bei YAT.Model.Terminal.terminal_IOChanged(Object sender, EventArgs e) in D:\Workspace\YAT\Trunk\YAT\YAT.Model\Terminal.cs:Zeile 2479.
		/// ]]>
		///
		/// The terminals get properly closed, but apparently there may still be pending
		/// asynchronous 'zombie' callback that later throw an exception. No feasible solution has
		/// been found, however there is a potential approach to deal with this issue:
		///
		/// This raw terminal could dispose of the underlying <see cref="IIOProvider"/> on each
		/// <see cref="Stop"/>. But of course, the <see cref="IIOProvider"/> would again have to
		/// be created on each <see cref="Start"/>. Or yet another alternative, the raw terminal's
		/// owner could invoke <see cref="Close"/> instead of <see cref="Stop"/>. Migrating to
		/// such alternative approach would require quite some refactoring, thus decided to keep
		/// the <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/> approach for
		/// the moment.
		///
		/// Temporarily disabling this handling/workaround can be useful for debugging, i.e. to
		/// continue program execution even in case of exceptions and let the debugger handle it.
		/// </remarks>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RawTerminal).FullName, exceptionHandling: EventHelper.ExceptionHandlingMode.DiscardDisposedTarget);
	////private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(RawTerminal).FullName); // See remarks above!

		private int instanceId;

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
		public event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<DateTime>> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<IOWarningEventArgs> IOWarning;

		/// <summary></summary>
		public event EventHandler<IOErrorEventArgs> IOError;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> ChunkSent;

		/// <summary></summary>
		public event EventHandler<EventArgs<RawChunk>> ChunkReceived;

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
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

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
			this.instanceId = Interlocked.Increment(ref staticInstanceCounter);

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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				DebugMessage("Disposing...");

				// In the 'normal' case, I/O will already have been stopped in Stop()...
				if (this.io != null) {
					this.io.Stop();
				}

				// ...and objects will already have been detached and disposed of in Close():
				DetachAndDisposeIO();
				DetachIOSettings();
				DetachBufferSettings();
				DisposeRepositories();

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) doesn't need and cannot be called since abstract.
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
				AssertUndisposed();

				return (this.bufferSettings);
			}
			set
			{
				AssertUndisposed();

				AttachBufferSettings(value);
				ApplyBufferSettings();
			}
		}

		/// <summary></summary>
		public virtual Settings.IOSettings IOSettings
		{
			get
			{
				AssertUndisposed();

				return (this.ioSettings);
			}
			set
			{
				AssertUndisposed();

				AttachIOSettings(value);
				ApplyIOSettings();
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
				AssertUndisposed();

				return (this.io);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertUndisposed();

				return (this.io.UnderlyingIOInstance);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		//------------------------------------------------------------------------------------------
		// Start/Stop/Close
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool Start()
		{
			AssertUndisposed();

			return (this.io.Start());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			AssertUndisposed();

			this.io.Stop();
		}

		/// <summary></summary>
		public virtual void Close()
		{
			AssertUndisposed();

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
			AssertUndisposed();

			if (data.Length > 0)
			{
				if (IsTransmissive)
				{
					if (!this.io.Send(data))
					{
						string message;
						if (data.Length <= 1)
							message = data.Length + " byte could not be sent because I/O device is not ready!"; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
						else
							message = data.Length + " bytes could not be sent because I/O device is not ready!"; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.

						OnIOError(new IOErrorEventArgs(IOErrorSeverity.Severe, IODirection.Tx, message));
					}
				}
				else
				{
					string message;
					if (data.Length <= 1)
						message = data.Length + " byte not sent anymore because terminal has been closed."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.
					else
						message = data.Length + " bytes not sent anymore because terminal has been closed."; // Using "byte" rather than "octet" as that is more common, and .NET uses "byte" as well.

					OnIOError(new IOErrorEventArgs(IOErrorSeverity.Acceptable, IODirection.Tx, message));
				}
			}
		}

		/// <summary></summary>
		public virtual int ClearSendBuffer()
		{
			AssertUndisposed();

			return (this.io.ClearSendBuffer());
		}

		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual List<RawChunk> RepositoryToChunks(RepositoryType repositoryType)
		{
			AssertUndisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToChunks());
					case RepositoryType.Bidir: return (this.bidirRepository.ToChunks());
					case RepositoryType.Rx:    return (this.rxRepository   .ToChunks());

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual void ClearRepository(RepositoryType repositoryType)
		{
			AssertUndisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			OnRepositoryCleared(new EventArgs<RepositoryType>(repositoryType));
		}

		/// <summary></summary>
		public virtual void ClearRepositories()
		{
			AssertUndisposed();

			lock (this.repositorySyncObj) // Lock throughout whole transaction, but not for raising the event!
			{
				this.txRepository   .Clear();
				this.bidirRepository.Clear();
				this.rxRepository   .Clear();
			}

			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Tx   ));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Bidir));
			OnRepositoryCleared(new EventArgs<RepositoryType>(RepositoryType.Rx   ));
		}

		/// <summary></summary>
		public virtual string RepositoryToExtendedDiagnosticsString(RepositoryType repositoryType, string indent)
		{
			AssertUndisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToExtendedDiagnosticsString(indent));
					case RepositoryType.Bidir: return (this.bidirRepository.ToExtendedDiagnosticsString(indent));
					case RepositoryType.Rx:    return (this.rxRepository   .ToExtendedDiagnosticsString(indent));

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
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

		#region Settings
		//==========================================================================================
		// Settings
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

		#region I/O
		//==========================================================================================
		// I/O
		//==========================================================================================

		private void AttachIO(IIOProvider io)
		{
			this.io = io;
			{
				this.io.IOChanged        += io_IOChanged;
				this.io.IOControlChanged += io_IOControlChanged;
				this.io.IOWarning        += io_IOWarning;
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
				this.io.IOWarning        -= io_IOWarning;
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

		private void io_IOChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not handle event during closing anymore.
				OnIOChanged(e);
		}

		private void io_IOControlChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not handle event during closing anymore.
				OnIOControlChanged(e);
		}

		private void io_IOWarning(object sender, MKY.IO.Serial.IOWarningEventArgs e)
		{
			DebugMessage("io_IOWarning() = " + e.ToDiagnosticsString());

			if (IsUndisposed) // Ensure to not handle event during closing anymore.
				OnIOWarning(new IOWarningEventArgs((IODirection)e.Direction, e.Message, e.TimeStamp));
		}

		private void io_IOError(object sender, MKY.IO.Serial.IOErrorEventArgs e)
		{
			DebugMessage("io_IOError() = " + e.ToDiagnosticsString());

			if (IsUndisposed) // Ensure to not handle event during closing anymore.
			{
				var spe = (e as MKY.IO.Serial.SerialPortErrorEventArgs);
				if (spe == null)
					OnIOError(new IOErrorEventArgs((IOErrorSeverity)e.Severity, (IODirection)e.Direction, e.Message, e.TimeStamp));
				else
					OnIOError(new SerialPortErrorEventArgs((IOErrorSeverity)spe.Severity, (IODirection)spe.Direction, spe.Message, spe.SerialPortError, spe.TimeStamp));
			}
		}

		/// <remarks>
		/// Synchronization to prevent a race condition is required here, see contract.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "Contract is given by the 'IIOProvider' event.")]
		private void io_DataReceived(object sender, DataReceivedEventArgs e)
		{
			// Synchronize the underlying Tx and Rx callbacks to prevent mix-ups. But attention,
			// do not simply lock() the 'ioDataSyncObj'. Instead, try to get the lock periodically,
			// but quit = discard the event as soon as the object got disposed of:

			while (IsUndisposed) // Ensure to not handle event during closing anymore.
			{
				if (Monitor.TryEnter(this.ioDataSyncObj, staticRandom.Next(50, 200)))
				{
					try
					{
						DebugDataReceived(e.Data.Length);

						var re = new RawChunk(e.Data, e.TimeStamp, e.Device, IODirection.Rx);
						lock (this.repositorySyncObj)
						{
							this.rxRepository   .Enqueue(re); // 'RawChunk' objects are immutable, subsequent use is OK.
							this.bidirRepository.Enqueue(re); // 'RawChunk' objects are immutable, subsequent use is OK.
						}

						OnChunkReceived(new EventArgs<RawChunk>(re)); // 'RawChunk' objects are immutable, subsequent use is OK.
					}
					finally
					{
						Monitor.Exit(this.ioDataSyncObj);
					}

					break; // Successfully entered the lock and processed the event => break the while-loop.
				}
				else // Monitor.TryEnter()
				{
					DebugMessage("io_DataReceived() monitor has timed out, trying again...");
				}
			} // while (IsUndisposed)
		}

		/// <remarks>
		/// Synchronization to prevent a race condition is required here, see contract.
		/// </remarks>
		[CallingContract(IsNeverMainThread = true, IsAlwaysSequential = true, Rationale = "Contract is given by the 'IIOProvider' event.")]
		private void io_DataSent(object sender, DataSentEventArgs e)
		{
			// Synchronize the underlying Tx and Rx callbacks to prevent mix-ups. But attention,
			// do not simply lock() the 'ioDataSyncObj'. Instead, try to get the lock periodically,
			// but quit = discard the event as soon as the object got disposed of:

			while (IsUndisposed) // Ensure to not handle event during closing anymore.
			{
				if (Monitor.TryEnter(this.ioDataSyncObj, staticRandom.Next(50, 200)))
				{
					try
					{
						DebugDataSent(e.Data.Length);

						var re = new RawChunk(e.Data, e.TimeStamp, e.Device, IODirection.Tx);
						lock (this.repositorySyncObj)
						{
							this.txRepository   .Enqueue(re); // 'RawChunk' objects are immutable, subsequent use is OK.
							this.bidirRepository.Enqueue(re); // 'RawChunk' objects are immutable, subsequent use is OK.
						}

						OnChunkSent(new EventArgs<RawChunk>(re)); // 'RawChunk' objects are immutable, subsequent use is OK.
					}
					finally
					{
						Monitor.Exit(this.ioDataSyncObj);
					}

					break; // Successfully entered the lock and processed the event => break the while-loop.
				}
				else // Monitor.TryEnter()
				{
					DebugMessage("io_DataSent() monitor has timed out, trying again...");
				}
			} // while (IsUndisposed)
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync(IOChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOControlChanged(EventArgs<DateTime> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOWarning(IOWarningEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync<IOWarningEventArgs>(IOWarning, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(IOErrorEventArgs e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
				this.eventHelper.RaiseSync<IOErrorEventArgs>(IOError, this, e);
		}

		/// <summary></summary>
		protected virtual void OnChunkSent(EventArgs<RawChunk> e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<EventArgs<RawChunk>>(ChunkSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnChunkReceived(EventArgs<RawChunk> e)
		{
			if (IsUndisposed && IsOpen) // Make sure to propagate event only if active.
				this.eventHelper.RaiseSync<EventArgs<RawChunk>>(ChunkReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryCleared(EventArgs<RepositoryType> e)
		{
			if (IsUndisposed) // Ensure to not propagate event during closing anymore.
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
		////AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.

			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
			{
				var sb = new StringBuilder();
				lock (this.repositorySyncObj)
				{
					sb.AppendLine(indent + "> TxRepository: ");
					sb.Append(this.txRepository.ToExtendedDiagnosticsString(indent + "   "));    // Repository will add 'NewLine'.

					sb.AppendLine(indent + "> BidirRepository: ");
					sb.Append(this.bidirRepository.ToExtendedDiagnosticsString(indent + "   ")); // Repository will add 'NewLine'.

					sb.AppendLine(indent + "> RxRepository: ");
					sb.Append(this.rxRepository.ToExtendedDiagnosticsString(indent + "   "));    // Repository will add 'NewLine'.

					sb.Append(indent + "> I/O: " + this.io.ToString());
				}
				return (sb.ToString());
			}

			return (base.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		public virtual string ToShortIOString()
		{
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
			{
				if      (this.io != null)
					return (this.io.ToString());
				else if (this.ioSettings != null)
					return (this.ioSettings.ToShortIOString());
			}

			return (base.ToString());
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[System.Diagnostics.Conditional("DEBUG")]
		protected virtual void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <summary></summary>
		[System.Diagnostics.Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			System.Diagnostics.Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"#" + this.instanceId.ToString("D2", CultureInfo.CurrentCulture),
					"[" + ToShortIOString() + "]",
					message
				)
			);
		}

	#if (DEBUG_DATA_RECEIVED)
		private int DebugDataReceived_counter; // = 0;
	#endif
	#if (DEBUG_DATA_SENT)
		private int DebugDataSent_counter; // = 0;
	#endif

		/// <remarks>
		/// <c>private</c> because value of <see cref="System.Diagnostics.ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[System.Diagnostics.Conditional("DEBUG_DATA_RECEIVED")]
		private void DebugDataReceived(int count)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) received, ", count);
		#if (DEBUG_DATA_RECEIVED)
			unchecked { DebugDataReceived_counter += count; }
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) in total.", DebugDataReceived_counter);
		#endif
			DebugMessage(sb.ToString());
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="System.Diagnostics.ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[System.Diagnostics.Conditional("DEBUG_DATA_SENT")]
		private void DebugDataSent(int count)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) sent, ", count);
		#if (DEBUG_DATA_SENT)
			unchecked { DebugDataSent_counter += count; }
			sb.AppendFormat(CultureInfo.CurrentCulture, "{0} byte(s) in total.", DebugDataSent_counter);
		#endif
			DebugMessage(sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
