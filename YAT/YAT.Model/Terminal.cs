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
// YAT Version 2.1.1 Development
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;
using MKY.Text;
using MKY.Time;
using MKY.Windows.Forms;

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Application.Utilities;
using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Terminals (.yat) of the YAT application model.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Terminal : IGuidProvider, IDisposable, IDisposableEx
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const string TerminalText = "Terminal";

		/// <summary>
		/// Static counter to number terminals. Counter is incremented before first use, first
		/// terminal therefore is "Terminal1".
		/// </summary>
		private const int SequentialIdCounterDefault = (TerminalIds.FirstSequentialId - 1);

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticSequentialIdCounter = SequentialIdCounterDefault;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <remarks>
		/// 'StyleCop' asks to remove this static constructor. But 'Code Analysis' (FxCop) requires
		/// the suppressions at this static constructor. So what...
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.Maintainability", "SA1409:RemoveUnnecessaryCode", Justification = "See below ;-) But unfortunately it seems that StyleCop doesn't allow a suppression at the constructor itself. So what...")]
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Well, kind of a code analysis deadlock ;-)")]
		static Terminal()
		{
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <remarks>
		/// Needed to test the ID feature of terminals and workspace.
		/// </remarks>
		public static void ResetSequentialIdCounter()
		{
			staticSequentialIdCounter = SequentialIdCounterDefault;
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Workaround to the following issue:
		///
		/// A test (e.g. 'FileHandlingTest') needs to verify the settings files after calling
		/// <see cref="Main.Exit()"/>. But at that moment, the settings have already been disposed
		/// of and can no longer be accessed.
		/// The first approach was to disable disposal in <see cref="Close()"/>. But that leads to
		/// remaining resources, resulting in significant slow-down when exiting NUnit.
		/// The second approach was to retrieve the required information *before* exiting, i.e.
		/// calling <see cref="Main.Exit()"/>. But that doesn't work at all, since auto-save paths
		/// are only evaluated *at* <see cref="Main.Exit()"/>.
		///
		/// This workaround is considered the best option to solve this issue.
		/// </summary>
		/// <remarks>
		/// Note that it is not possible to mark a property with [Conditional("TEST")].
		/// </remarks>
		public bool DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification { get; set; }

		/// <summary>
		/// A dedicated event helper to allow discarding exceptions when object got disposed.
		/// </summary>
		/// <remarks> \remind (2019-08-22 / MKY)
		///
		/// Explicitly setting <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// to handle/workaround a) the issue described in <see cref="Chronometer"/> as well as
		/// to handle/workaround b) the issue described in <see cref="Domain.RawTerminal"/>.
		///
		/// Issue a)
		/// --------
		/// Note that <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/> only
		/// is a must here, <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// in <see cref="Chronometer"/> as well as <see cref="RateProvider"/> do not
		/// handle/workaround the issue, since the disposed target is encountered here. Still,
		/// since all three objects are involved, decided to keep the handling/workaround in all
		/// three locations.
		///
		/// Issue b)
		/// --------
		/// Note that <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/> only
		/// is a must here, <see cref="EventHelper.ExceptionHandlingMode.DiscardDisposedTarget"/>
		/// in <see cref="Domain.Terminal"/> as well as <see cref="Domain.RawTerminal"/> do not
		/// handle/workaround the issue, since the disposed target is encountered here. Still,
		/// since all three objects are involved, decided to keep the handling/workaround in all
		/// three locations.
		///
		/// Thus, to reproduce the issue, simply disable the handling/workaround by replacing the
		/// line further below. Then:
		///  1. Start a new terminal as <see cref="Domain.IOType.TcpServer"/>.
		///  2. Start a new terminal as <see cref="Domain.IOType.TcpClient"/>.
		///  3. [File > Close All Terminals]
		/// With handling/workaround, debug output will show the issue, but execution will not get
		/// halted by the debugger. Without handling/workaround, the exception will get handled by
		/// the debugger.
		///
		/// Note that the issue only happens in case both server and client are in the same instance
		/// of YAT! Also note that the issue will not happen on [File > Exit]! The latter does have
		/// a slightly different calling sequence, e.g. workspace is saved before terminals are
		/// closed, but the root cause that makes this differences is not (yet) understood!
		///
		/// Temporarily disabling this handling/workaround can be useful for debugging, i.e. to
		/// continue program execution even in case of exceptions and let the debugger handle it.
		/// </remarks>
		private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Terminal).FullName, exceptionHandling: EventHelper.ExceptionHandlingMode.DiscardDisposedTarget);
	////private EventHelper.Item eventHelper = EventHelper.CreateItem(typeof(Terminal).FullName); // See remarks above!

		private TerminalStartArgs startArgs;
		private Guid guid;
		private int sequentialId;
		private string sequentialName;
		private string fileName;

		// Settings:
		private DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler;
		private TerminalSettingsRoot settingsRoot;

		// Terminal:
		private Domain.Terminal terminal;

		// Logs:
		private Log.Provider log;

		// AutoAction:
		private int autoActionCount;
		private AutoTriggerHelper autoActionTriggerHelper;
		private object autoActionTriggerHelperSyncObj = new object();
		private bool autoActionClearRepositoriesOnSubsequentRxIsArmed; // = false;

		// AutoResponse:
		private int autoResponseCount;
		private AutoTriggerHelper autoResponseTriggerHelper;
		private object autoResponseTriggerHelperSyncObj = new object();

		// Time status:
		private Chronometer activeConnectChrono;
		private Chronometer totalConnectChrono;

		// Count status:
		private int txByteCount;
		private int rxByteCount;

		private int txLineCount;
	////private int bidirLineCount would technically be possible, but doesn't make much sense.
		private int rxLineCount;

		// Rate status:
		private RateProvider txByteRate;
		private RateProvider rxByteRate;

		private RateProvider txLineRate;
	////private RateProvider bidirLineRate would technically be possible, but doesn't make much sense.
		private RateProvider rxLineRate;

		// Partial commands:
		private string partialCommandLine;

	#if (WITH_SCRIPTING)

		// Scripting:
		private string lastSentMessage; // = null;
		private bool isAutoSocket;      // = false;
		private int receivedXOnOffsetForScripting;  // = 0;
		private int receivedXOffOffsetForScripting; // = 0;
		private object receivedXOnXOffForScriptingSyncObj = new object();

	#endif

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler IOChanged;

		/// <summary></summary>
		public event EventHandler<Domain.IOControlEventArgs> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<TimeSpanEventArgs> IOConnectTimeChanged;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IOCountChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IORateChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		public event EventHandler IORateChanged_Decimated;

		/// <summary></summary>
		public event EventHandler<Domain.IOErrorEventArgs> IOError;

	#if (WITH_SCRIPTING)

		// Note that e.g. a 'SendingText' or 'SendingMessage' event doesn't make sense, as it would
		// contain parsable text that may even include keyword to be processed.

		/// <summary>
		/// Occurs when a packet is being sent in the host application. The event args contain the
		/// binary raw data that is being sent, including control characters, EOL,...
		/// </summary>
		/// <remarks>
		/// Named 'Sending...' rather than '...Sent' since sending is just about to happen and can
		/// be modified using the <see cref="Domain.ModifiablePacketEventArgs.Data"/> property or
		/// even canceled using the <see cref="Domain.ModifiablePacketEventArgs.Cancel"/> property.
		/// This is similar to the behavior of e.g. the 'OnValidating' event of WinForms controls.
		/// </remarks>
		public event EventHandler<Domain.ModifiablePacketEventArgs> SendingPacket;

		/// <summary></summary>
		public event EventHandler<Domain.RawChunkEventArgs> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<Domain.RawChunkEventArgs> RawChunkReceived;

	#endif // WITH_SCRIPTING

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsTxAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsBidirAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsRxAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineTxReplaced"/></remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineTxReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineBidirReplaced"/></remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineBidirReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineRxReplaced"/></remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineRxReplaced;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineTxCleared"/></remarks>
		public event EventHandler CurrentDisplayLineTxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineBidirCleared"/></remarks>
		public event EventHandler CurrentDisplayLineBidirCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		/// <remarks><see cref="Domain.Terminal.CurrentDisplayLineRxCleared"/></remarks>
		public event EventHandler CurrentDisplayLineRxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesTxAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesBidirAdded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesRxAdded;

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Occurs when a packet has been received by the host application. The event args contain
		/// the binary raw data that has been received, including control characters, EOL,...
		/// In contrast, the <see cref="ScriptMessageReceived"/> event args contain the message in
		/// formatted text representation.
		/// </summary>
		public event EventHandler<Domain.PacketEventArgs> ScriptPacketReceived;

		/// <summary>
		/// Occurs when a message has been received by the host application. The event args contain
		/// the message in formatted text representation. For text terminals, the text is composed
		/// of the decoded characters, excluding control characters. For binary terminals, the text
		/// represents the received data in hexadecimal notation.
		/// In contrast, the <see cref="ScriptPacketReceived"/> event args contain the binary raw
		/// data that has been received.
		/// </summary>
		public event EventHandler<Domain.MessageEventArgs> ScriptMessageReceived;

	#endif // WITH_SCRIPTING

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryTxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryBidirCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryRxCleared;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryTxReloaded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryBidirReloaded;

		/// <remarks>Intentionally using separate Tx/Bidir/Rx events: More obvious, ease of use.</remarks>
		public event EventHandler RepositoryRxReloaded;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler ResetStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> SaveCommandPageAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> OpenCommandPageFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> Closed;

		/// <summary></summary>
		public event EventHandler ExitRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Terminal()
			: this(new TerminalSettingsRoot())
		{
		}

		/// <summary></summary>
		public Terminal(TerminalSettingsRoot settings)
			: this(new DocumentSettingsHandler<TerminalSettingsRoot>(settings))
		{
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(new TerminalStartArgs(), settingsHandler)
		{
		}

		/// <summary></summary>
		public Terminal(TerminalStartArgs startArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(startArgs, settingsHandler, Guid.Empty)
		{
		}

		/// <remarks><see cref="Guid.Empty"/> cannot be used as default argument as it is read-only.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Terminal(TerminalStartArgs startArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
		{
			try
			{
				DebugMessage("Creating...");

				this.startArgs = startArgs;

				if (guid != Guid.Empty)
					this.guid = guid;
				else
					this.guid = Guid.NewGuid();

				// Link, override and attach to settings:
				this.settingsHandler = settingsHandler;
				this.settingsRoot = this.settingsHandler.Settings;
				this.settingsRoot.ClearChanged();
				AttachSettingsEventHandlers();

				// Set ID and name(s):
				this.sequentialId = ++staticSequentialIdCounter;
				this.sequentialName = TerminalText + this.sequentialId.ToString(CultureInfo.CurrentCulture);
				if (!this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFilePathIsValid)
					this.fileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				// Create underlying terminal:
				this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
				AttachTerminalEventHandlers();

				// Create log:
				this.log = new Log.Provider(this.settingsRoot.Log, (EncodingEx)this.settingsRoot.TextTerminal.Encoding, this.settingsRoot.Format);

				// Create Auto[Action|Response]:
				CreateAutoActionHelper();
				CreateAutoResponseHelper();

				// Create chronos:
				CreateChronos();

				// Create rates:
				CreateRates();

				DebugMessage("...successfully created.");
			}
			catch (Exception ex)
			{
				DebugMessage("...failed!");
				DebugEx.WriteException(GetType(), ex);

				Dispose(); // Immediately call Dispose() to ensure no zombies remain!
				throw; // Rethrow!
			}
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

				DebugMessage("Disposing...");

				// Dispose of managed resources if requested:
				if (disposing)
				{
					// In the 'normal' case, terminal and log have already been closed, otherwise...

					// ...detach event handlers to ensure that no more events are received...
					DetachTerminalEventHandlers();

					// ...ensure that timed objects are stopped and do not raise events anymore...
					DisposeRates();
					DisposeChronos();
					DisposeAutoActionHelper();
					DisposeAutoResponseHelper();

					// ...close and dispose of terminal and log...
					CloseAndDisposeTerminal();
					DisposeLog();

					// ...and finally dispose of the settings:
					if (!DoNotDisposeOfSettingsBecauseTheyAreRequiredForTestVerification)
					{
						DetachSettingsEventHandlers();
						DisposeSettingsHandler();
					}
				}

				// Set state to disposed:
				IsDisposed = true;

				DebugMessage("...successfully disposed.");
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
		~Terminal()
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

		#region General
		//==========================================================================================
		// General
		//==========================================================================================

		/// <summary></summary>
		public virtual Guid Guid
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.guid);
			}
		}

		/// <summary></summary>
		public virtual int SequentialId
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.sequentialId);
			}
		}

		/// <summary>
		/// The name incrementally assigned terminal name 'Terminal1', 'Terminal2',...
		/// </summary>
		public virtual string SequentialName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.sequentialName);
			}
		}

		/// <summary>
		/// The file name if the user has saved the terminal; otherwise <see cref="string.Empty"/>.
		/// </summary>
		public virtual string FileName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (!string.IsNullOrEmpty(this.fileName))
					return (this.fileName);

				return ("");
			}
		}

		/// <summary>
		/// The optional user defined terminal name; otherwise <see cref="string.Empty"/>.
		/// </summary>
		public virtual string UserName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsRoot != null)
				{
					if (!string.IsNullOrEmpty(this.settingsRoot.UserName))
						return (this.settingsRoot.UserName);
				}

				return ("");
			}
		}

		/// <summary>
		/// The indicated name, i.e. either the <see cref="UserName"/>, <see cref="FileName"/>
		/// or <see cref="SequentialName"/>.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (!string.IsNullOrEmpty(UserName))
					return (UserName);

				if (!string.IsNullOrEmpty(FileName))
					return (FileName);

				return (SequentialName);
			}
		}

		/// <summary></summary>
		public virtual TerminalStartArgs StartArgs
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (this.startArgs);
			}
		}

		/// <summary></summary>
		public virtual Domain.IOType IOType
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsRoot != null)
					return (this.settingsRoot.IOType);
				else
					return (Domain.IOType.Unknown);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsStopped);
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

				if (this.terminal != null)
					return (this.terminal.IsStarted);
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

				if (this.terminal != null)
					return (this.terminal.IsOpen);
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

				if (this.terminal != null)
					return (this.terminal.IsConnected);
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

				if (this.terminal != null)
					return (this.terminal.IsTransmissive);
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

				if (this.terminal != null)
					return (this.terminal.IsReadyToSend);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsBusy
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsBusy);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogIsOn
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.AnyIsOn);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool AllLogsAreOn
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.AllAreOn);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogFileExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.terminal != null)
					return (this.log.FileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual string ShortIOString
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.ToShortIOString());
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.UnderlyingIOInstance);
			}
		}

		/// <summary></summary>
		public virtual string Caption
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (CaptionHelper.Compose(this.settingsHandler, this.settingsRoot, this.terminal, IndicatedName, IsStarted, IsOpen, IsConnected));
			}
		}

		/// <summary></summary>
		public virtual string IOStatusText
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				return (IOStatusHelper.Compose(this.settingsRoot, this.terminal, IsStarted, IsOpen, IsConnected));
			}
		}

		/// <summary>
		/// Returns the <see cref="MKY.IO.Ports.SerialPortId"/> for <see cref="Domain.IOType.SerialPort"/>,
		/// <c>null</c> for all other terminal types.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortId IOSerialPortId
		{
			get
			{
				AssertNotDisposed();

				if (this.settingsRoot != null)
				{
					if (this.settingsRoot.IOType == Domain.IOType.SerialPort)
						return (this.settingsRoot.IO.SerialPort.PortId);
				}

				return (null);
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Gets the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (and not in the overlying
		/// <see cref="ScriptBridge"/>) to keep the line for each terminal.
		/// </remarks>
		public virtual void SetLastSentMessage(string value)
		{
			AssertNotDisposed();

			this.lastSentMessage = value;
		}

		/// <summary>
		/// Gets the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (and not in the overlying
		/// <see cref="ScriptBridge"/>) to keep the line for each terminal.
		/// </remarks>
		public virtual void GetLastSentMessage(out string value)
		{
			AssertNotDisposed();

			value = this.lastSentMessage;
		}

		/// <summary>
		/// Clears the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (and not in the overlying
		/// <see cref="ScriptBridge"/>) to keep the line for each terminal.
		/// </remarks>
		public virtual void ClearLastSentMessage(out string cleared)
		{
			AssertNotDisposed();

			cleared = this.lastSentMessage;

			this.lastSentMessage = null;
		}

		/// <summary>
		/// Gets a value indicating whether the terminal has received a line that is available for scripting.
		/// </summary>
		public virtual bool HasAvailableReceivedMessageForScripting
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.HasAvailableReceivedMessageForScripting);
			}
		}

		/// <summary>
		/// Gets a value indicating the number of received lines that are available for scripting.
		/// </summary>
		public int AvailableReceivedMessageCountForScripting
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.AvailableReceivedMessageCountForScripting);
			}
		}

		/// <summary>
		/// Returns the line that has last been enqueued into the receive queue that is available for scripting.
		/// </summary>
		public virtual void GetLastEnqueuedReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			this.terminal.GetLastEnqueuedReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Clears the last enqueued line that is available for scripting.
		/// </summary>
		public virtual void ClearLastEnqueuedReceivedMessageForScripting(out string cleared)
		{
			AssertNotDisposed();

			this.terminal.ClearLastEnqueuedReceivedMessageForScripting(out cleared);
		}

		/// <summary>
		/// Gets the next received line that is available for scripting and removes it from the queue.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The underlying <see cref="Queue{T}"/> is empty.
		/// </exception>
		public virtual void DequeueNextAvailableReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			this.terminal.DequeueNextAvailableReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Returns the received line that has last been dequeued from the receive queue for scripting.
		/// </summary>
		public virtual void GetLastDequeuedReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			this.terminal.GetLastDequeuedReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Clears the last dequeued line that is available for scripting.
		/// </summary>
		public virtual void ClearLastDequeuedReceivedMessageForScripting(out string cleared)
		{
			AssertNotDisposed();

			this.terminal.ClearLastDequeuedReceivedMessageForScripting(out cleared);
		}

		/// <remarks>
		/// \remind (2018-03-27 / MKY)
		/// 'LastAvailable' only works properly for a terminating number of received messages, but
		/// not for consecutive receiving. This method shall be eliminated as soon as the obsolete
		/// GetLastReceived(), CheckLastReceived() and WaitFor() have been removed.
		/// </remarks>
		public virtual void GetLastAvailableReceivedMessageForScripting(out string value)
		{
			AssertNotDisposed();

			this.terminal.GetLastAvailableReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Cleares all available lines in the receive queue for scripting.
		/// </summary>
		public void ClearAvailableReceivedMessagesForScripting(out string[] clearedLines)
		{
			AssertNotDisposed();

			this.terminal.ClearAvailableReceivedMessagesForScripting(out clearedLines);
		}

		/// <summary>
		/// Determins whether this terminal is using an auto socket.
		/// </summary>
		/// <remarks>
		/// \remind (2019-10-18 / MKY) really needed?
		/// </remarks>
		public virtual bool IsAutoSocket
		{
			get
			{
				AssertNotDisposed();

				return (this.isAutoSocket);
			}
			set
			{
				AssertNotDisposed();

				this.isAutoSocket = value;
			}
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Start
		//==========================================================================================
		// Start
		//==========================================================================================

		/// <summary>
		/// Starts terminal, i.e. starts log and opens I/O.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public virtual bool Start()
		{
			AssertNotDisposed();

			// Switch log on if selected:
			if (this.settingsRoot.LogIsOn)
			{
				if (!SwitchLogOn())
					return (false);
			}

			// Then start terminal if selected:
			if (this.settingsRoot.TerminalIsStarted)
			{
				// Check availability of I/O before starting:
				var result = CheckIOAvailability();
				switch (result)
				{
					case CheckResult.OK:     return (StartIO());
					case CheckResult.Cancel: return (false);
					case CheckResult.Ignore: return (true);

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + result.ToString() + "' is a result that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			return (true);
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > Lifetime
		//------------------------------------------------------------------------------------------
		// Settings > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed += settingsRoot_Changed;
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= settingsRoot_Changed;
		}

		private void DisposeSettingsHandler()
		{
			if (this.settingsHandler != null)
			{
				this.settingsHandler.Dispose();
				this.settingsHandler = null;
			}
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'terminalTypeOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.TerminalType settingsRoot_Changed_terminalTypeOld = Domain.Settings.TerminalSettings.TerminalTypeDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'endianessOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.Endianness settingsRoot_Changed_endiannessOld = Domain.Settings.IOSettings.EndiannessDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'endianessOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int settingsRoot_Changed_encodingOld = Domain.Settings.TextTerminalSettings.EncodingDefault;

		/// <remarks>
		/// Required to solve the issue described in bug #223 "Settings events should state the exact settings diff".
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'sendImmediatelyOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_sendImmediatelyOld = Domain.Settings.SendSettingsText.SendImmediatelyDefault;

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			// Note that view settings are handled in View.Forms.Terminal::settingsRoot_Changed().
			// Below, only those settings that need to be managed by the model are handled.

			if (e.Inner == null)
			{
				// Nothing to do, no need to care about 'ProductVersion' and such.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Explicit))
			{
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Implicit))
			{
				HandleImplicitSettings(e.Inner);
			}
		}

		private void HandleExplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal))
			{
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.PredefinedCommand))
			{
				UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoAction))
			{
				UpdateAutoAction(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoResponse))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Format))
			{
				this.log.NeatFormat = this.settingsRoot.Format;
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Log))
			{
				this.log.Settings = this.settingsRoot.Log;
			}
		}

		private void HandleTerminalSettings(SettingsEventArgs e)
		{
			// \ToDo: ApplySettings should be called here => FR #309.

			if (e.Inner == null)
			{
				if (settingsRoot_Changed_terminalTypeOld != this.settingsRoot.TerminalType) {
					settingsRoot_Changed_terminalTypeOld = this.settingsRoot.TerminalType;

					// Terminal type has changed, recreate Auto[Action|Response]:
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.IO))
			{
				if (settingsRoot_Changed_endiannessOld != this.settingsRoot.Terminal.IO.Endianness) {
					settingsRoot_Changed_endiannessOld = this.settingsRoot.Terminal.IO.Endianness; // Relevant for byte sequence based triggers.

					// Endianness has changed, recreate Auto[Action|Response]:
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.Send))
			{
				if (settingsRoot_Changed_sendImmediatelyOld != this.settingsRoot.Terminal.Send.Text.SendImmediately) {
					settingsRoot_Changed_sendImmediatelyOld = this.settingsRoot.Terminal.Send.Text.SendImmediately;

					// Send immediately has changed, reset the command:
					this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
				}
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal.TextTerminal))
			{
				this.log.TextTerminalEncoding = (EncodingEx)this.settingsRoot.Terminal.TextTerminal.Encoding;

				if (settingsRoot_Changed_encodingOld != this.settingsRoot.Terminal.TextTerminal.Encoding) {
					settingsRoot_Changed_encodingOld = this.settingsRoot.Terminal.TextTerminal.Encoding; // Relevant for byte sequence based triggers.

					// Encoding has changed, recreate Auto[Action|Response]:
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
		}

		private void HandleImplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendText))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendFile))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
		}

		#endregion

		#region Settings > Properties
		//------------------------------------------------------------------------------------------
		// Settings > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual string SettingsFilePath
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFilePath);
				else
					return (null);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsReadOnly
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsReadOnly);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileIsWritable);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileHasAlreadyBeenNormallySaved
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (this.settingsHandler.SettingsFileSuccessfullyLoaded && !this.settingsRoot.AutoSaved);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileNoLongerExists
		{
			get
			{
				// Do not call AssertNotDisposed() in a simple get-property.

				if (this.settingsHandler != null)
					return (SettingsFileHasAlreadyBeenNormallySaved && !SettingsFileExists);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual TerminalSettingsRoot SettingsRoot
		{
			get
			{
				// Do not call AssertNotDisposed() to still allow reading the settings after the
				// terminal has been disposed. This is required for certain test cases.

				return (this.settingsRoot);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has linked settings.
		/// </summary>
		public virtual bool HasLinkedSettings
		{
			get { return (SettingsRoot.HasLinkedSettings); }
		}

		#endregion

		#region Settings > Methods
		//------------------------------------------------------------------------------------------
		// Settings > Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Applies new terminal settings.
		/// </summary>
		/// <remarks>
		/// Using <see cref="TerminalExplicitSettings"/> instead of simply using
		/// <see cref="Domain.Settings.TerminalSettings"/> for two reasons:
		/// <list type="bullet">
		/// <item><description>Handling of <see cref="TerminalExplicitSettings.UserName"/>.</description></item>
		/// <item><description>Prepared for future migration to tree view dialog containing all settings.</description></item>
		/// </list>
		/// </remarks>
		public virtual void ApplyTerminalSettings(TerminalExplicitSettings settings)
		{
			AssertNotDisposed();

			// Attention:
			// Similar code exists in Domain.Terminal.ApplySettings() but without changing the terminal settings (.yat file).
			// Changes here may have to be applied there too.

			if (this.terminal.IsStarted) // Terminal is started, stop and restart it with the new settings:
			{
				// Note that the whole terminal will be recreated. Thus, it must also be recreated if non-IO settings have changed.

				if (StopIO(false))
				{
					this.settingsRoot.SuspendChangeEvent();
					try
					{
						this.settingsRoot.Explicit = settings;

						DetachTerminalEventHandlers();
						using (var oldTerminal = this.terminal)
						{
							this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
						}
						AttachTerminalEventHandlers();
					}
					finally
					{
						this.settingsRoot.ResumeChangeEvent();
					}
					this.terminal.RefreshRepositories();

					if (StartIO(false))
						OnTimedStatusTextRequest("Terminal settings applied.");
					else
						OnFixedStatusTextRequest("Terminal settings applied but terminal could not be started anymore.");
				}
				else
				{
					OnTimedStatusTextRequest("Terminal settings not applied!");
				}
			}
			else // Terminal is stopped, simply set the new settings:
			{
				this.settingsRoot.SuspendChangeEvent();
				try
				{
					this.settingsRoot.Explicit = settings;

					DetachTerminalEventHandlers();
					using (var oldTerminal = this.terminal)
					{
						this.terminal = Domain.TerminalFactory.RecreateTerminal(this.settingsRoot.Explicit.Terminal, oldTerminal);
					}
					AttachTerminalEventHandlers();
				}
				finally
				{
					this.settingsRoot.ResumeChangeEvent();
				}
				this.terminal.RefreshRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Applies new log settings.
		/// </summary>
		public virtual void ApplyLogSettings(Log.Settings.LogSettings settings)
		{
			AssertNotDisposed();

			this.settingsRoot.Log = settings;
			this.log.Settings = this.settingsRoot.Log;
		}

		#endregion

		#endregion

		#region Save
		//==========================================================================================
		// Save
		//==========================================================================================

		/// <summary>
		/// Saves terminal to file, prompts for file if it doesn't exist yet.
		/// </summary>
		public virtual bool Save()
		{
			// AssertNotDisposed() is called by 'Save(...)' below.

			bool isCanceled;                               // Save even if not changed since explicitly requesting saving.
			return (SaveConsiderately(false, true, true, true, false, out isCanceled));
		}

		/// <summary>
		/// Silently tries to save terminal to file, i.e. without any user interaction.
		/// </summary>
		public virtual bool TrySaveConsideratelyWithoutUserInteraction(bool isWorkspaceClose, bool autoSaveIsAllowed)
		{
			bool isCanceled;
			return (SaveConsiderately(isWorkspaceClose, autoSaveIsAllowed, false, false, false, out isCanceled));
		}

		/// <summary>
		/// This method implements the logic that is needed when saving, opposed to the method
		/// <see cref="SaveToFile"/> which just performs the actual save, i.e. file handling.
		/// </summary>
		/// <param name="isWorkspaceClose">Indicates that workspace closes.</param>
		/// <param name="autoSaveIsAllowed">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		/// <param name="saveEvenIfNotChanged">Indicates whether save must happen even if not changed.</param>
		/// <param name="canBeCanceled">Indicates whether save can be canceled.</param>
		/// <param name="isCanceled">Indicates whether save has been canceled.</param>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool SaveConsiderately(bool isWorkspaceClose, bool autoSaveIsAllowed, bool userInteractionIsAllowed, bool saveEvenIfNotChanged, bool canBeCanceled, out bool isCanceled)
		{
			AssertNotDisposed();

			isCanceled = false;

			autoSaveIsAllowed = EvaluateWhetherAutoSaveIsAllowedIndeed(autoSaveIsAllowed);

			// -------------------------------------------------------------------------------------
			// Skip save if there is no reason to save:
			// -------------------------------------------------------------------------------------

			if (ThereIsNoReasonToSave(autoSaveIsAllowed, saveEvenIfNotChanged))
				return (true);

			// -------------------------------------------------------------------------------------
			// Create auto save file path or request manual/normal file path if necessary:
			// -------------------------------------------------------------------------------------

			if (!this.settingsHandler.SettingsFilePathIsValid)
			{
				if (autoSaveIsAllowed) {
					this.settingsHandler.SettingsFilePath = ComposeAbsoluteAutoSaveFilePath();
				}
				else if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, false, out isCanceled));
				}
				else {
					return (false); // Let save fail if the file path is invalid and no user interaction is allowed
				}
			}
			else if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
			{
				// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
				if (userInteractionIsAllowed) {
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, false, out isCanceled));
				}
				else {
					return (false);
				}
			}

			// -------------------------------------------------------------------------------------
			// Handle write-protected or non-existant file:
			// -------------------------------------------------------------------------------------

			// Attention:
			// Similar code exists in TrySaveLinkedCommandPageConsiderately() further below.
			// Changes here may have to be applied there too.

			if (!SettingsFileIsWritable || SettingsFileNoLongerExists)
			{
				if (this.settingsRoot.ExplicitHaveChanged || saveEvenIfNotChanged)
				{
					if (userInteractionIsAllowed) {
						return (RequestRestrictedSaveAsFromUser(isWorkspaceClose, autoSaveIsAllowed, canBeCanceled, out isCanceled));
					}
					else {
						return (false); // Let save of explicit change fail if file is restricted.
					}
				}
				else // ImplicitHaveChanged:
				{
					return (true); // Skip save of implicit change as save is currently not feasible.
				}
			}

			// -------------------------------------------------------------------------------------
			// Potentially save linked settings:
			// -------------------------------------------------------------------------------------

			if (this.settingsRoot.LinkedSettingsHaveChanged) // Not (yet) distinguishing explicit/implicit.
			{
				if (!TrySaveLinkedSettings(userInteractionIsAllowed, canBeCanceled, out isCanceled))
					return (false);
			}

			// -------------------------------------------------------------------------------------
			// Save is feasible:
			// -------------------------------------------------------------------------------------

			return (SaveToFile(autoSaveIsAllowed, null));
		}

		/// <summary></summary>
		protected virtual bool EvaluateWhetherAutoSaveIsAllowedIndeed(bool autoSaveIsAllowed)
		{
			// Do not auto save if file already exists but isn't auto saved:
			if (autoSaveIsAllowed && this.settingsHandler.SettingsFilePathIsValid && !this.settingsRoot.AutoSaved)
				return (false);

			return (autoSaveIsAllowed);
		}

		/// <summary></summary>
		protected virtual bool ThereIsNoReasonToSave(bool autoSaveIsAllowed, bool saveEvenIfNotChanged)
		{
			if (saveEvenIfNotChanged)
				return (false);

			// Ensure that former auto files are 'Saved As' if auto save is no longer allowed:
			if (this.settingsRoot.AutoSaved && !autoSaveIsAllowed)
				return (false);

			// No need to save if settings are up to date:
			return (this.settingsHandler.SettingsFileIsUpToDate && !this.settingsRoot.HaveChanged);
		}

		/// <summary></summary>
		protected virtual string ComposeAbsoluteAutoSaveFilePath()
		{
			var sb = new StringBuilder();

			sb.Append(Application.Settings.GeneralSettings.AutoSaveRoot);
			sb.Append(Path.DirectorySeparatorChar);
			sb.Append(Application.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
			sb.Append(Guid.ToString());
			sb.Append(ExtensionHelper.TerminalExtension);

			return (sb.ToString());
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestNormalSaveAsFromUser(bool isWorkspaceClose, bool autoSaveIsAllowed, out bool isCanceled)
		{
			if (isWorkspaceClose && !autoSaveIsAllowed)
			{
				// Ask user whether to save terminal, to give user to possibility to chose 'No'.
				// This is required since the 'Save File Dialog' only offers [OK] and [Cancel].

				var dr = OnMessageInputRequest
				(
					"Save terminal?",
					IndicatedName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes:
						break; // Continue with save below.

					case DialogResult.No:
						isCanceled = false;
						return (true); // Success, as user explicitly chose 'No'.

					case DialogResult.Cancel:
					default:
						isCanceled = true;
						return (false);
				}
			}

			switch (OnSaveAsFileDialogRequest()) // 'Save File Dialog' offers [OK] and [Cancel].
			{
				case DialogResult.OK:
					isCanceled = false;
					return (true);

				case DialogResult.Cancel:
				default:
					isCanceled = true;
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestRestrictedSaveAsFromUser(bool isWorkspaceClose, bool autoSaveIsAllowed, bool canBeCanceled, out bool isCanceled)
		{
			string reason;
			if      ( SettingsFileNoLongerExists) // Shall be checked first, as that is first thing to verify.
				reason = "The file no longer exists.";
			else if (!SettingsFileIsWritable)
				reason = "The file is write-protected.";
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'SaveAs'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var message = new StringBuilder();
			message.AppendLine("Unable to save");
			message.AppendLine(this.settingsHandler.SettingsFilePath);
			message.AppendLine();
			message.Append(reason + " Would you like to save the file at another location? You may also fix the file and then confirm the current location.");

			var dr = OnMessageInputRequest
			(
				message.ToString(),
				"File Error",
				(canBeCanceled ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo),
				MessageBoxIcon.Question
			);

			switch (dr)
			{
				case DialogResult.Yes:
					isCanceled = false;
					return (RequestNormalSaveAsFromUser(isWorkspaceClose, autoSaveIsAllowed, out isCanceled));

				case DialogResult.No:
					OnTimedStatusTextRequest("Terminal not saved!");
					isCanceled = false;
					return (true);

				case DialogResult.Cancel:
				default:
					// No need for TextRequest("Canceled!") as parent will handle cancel.
					isCanceled = true;
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'dr' = DialogResult.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestRestrictedSaveCommandPageFromUser(string linkFilePathRestricted,
		                                                                out string linkFilePathNewOrConfirmed, out bool doUnlink, out bool isCanceled)
		{
			string reason;
			if      (!File.Exists(linkFilePathRestricted)) // Shall be checked first, as that is first thing to verify.
				reason = "The file no longer exists.";
			else if (!FileEx.IsWritable(linkFilePathRestricted))
				reason = "The file is write-protected.";
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'LinkFilePath'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var message = new StringBuilder();
			message.AppendLine("Unable to save");
			message.AppendLine(linkFilePathRestricted);
			message.AppendLine();
			message.AppendLine(reason);
			message.AppendLine();
			message.Append("Would you like to select another location or fix the file and then confirm the current location [Yes],");
			message.Append(" or clear the link to the file [No]?");

			var dr = OnMessageInputRequest
			(
				message.ToString(),
				"File Error",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Question
			);

			switch (dr)
			{
				case DialogResult.Yes:
					doUnlink = false;
					var drSaveAs = OnSaveCommandPageAsFileDialogRequest(linkFilePathRestricted);
					linkFilePathNewOrConfirmed = drSaveAs.FilePath;
					isCanceled = (drSaveAs.Result == DialogResult.Cancel);
					return (drSaveAs.Result == DialogResult.OK);

				case DialogResult.No:
					doUnlink = true;
					linkFilePathNewOrConfirmed = null;
					isCanceled = false;
					return (false); // = no file path!

				case DialogResult.Cancel:
				default:
					doUnlink = false;
					linkFilePathNewOrConfirmed = null;
					isCanceled = true;
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'dr' = DialogResult.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool RequestRestrictedOpenCommandPageFromUser(string linkFilePathRestricted, bool canBeCanceled,
		                                                                out string linkFilePathNewOrConfirmed, out bool doUnlink, out bool isCanceled)
		{
			string reason;
			if      (!File.Exists(linkFilePathRestricted)) // Must be checked first! A non-existent file cannot be readabled!
				reason = "The file no longer exists.";
			else if (!FileEx.IsReadable(linkFilePathRestricted))
				reason = "The file cannot be read.";
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'LinkFilePath'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			var message = new StringBuilder();
			message.AppendLine("Unable to access");
			message.AppendLine(linkFilePathRestricted);
			message.AppendLine();
			message.AppendLine(reason);
			message.AppendLine();
			message.Append("Would you like to select another location or fix the file and then confirm the current location [Yes],");
			message.Append(" or clear the link to the file [No]?");

			var dr = OnMessageInputRequest
			(
				message.ToString(),
				"File Error",
				(canBeCanceled ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo),
				MessageBoxIcon.Question
			);

			switch (dr)
			{
				case DialogResult.Yes:
					doUnlink = false;
					var drOpen = OnOpenCommandPageFileDialogRequest(linkFilePathRestricted);
					linkFilePathNewOrConfirmed = drOpen.FilePath;
					isCanceled = (drOpen.Result == DialogResult.Cancel);
					return (drOpen.Result == DialogResult.OK);

				case DialogResult.No:
					doUnlink = true;
					linkFilePathNewOrConfirmed = null;
					isCanceled = false;
					return (false); // = no file path!

				case DialogResult.Cancel:
				default:
					doUnlink = false;
					linkFilePathNewOrConfirmed = null;
					isCanceled = true;
					return (false);
			}
		}

		/// <summary>
		/// Saves settings to given file.
		/// </summary>
		public virtual bool SaveAs(string filePath)
		{
			AssertNotDisposed();

			var absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(filePath);

			// Request the deletion of the obsolete auto saved settings file given the new file is different:
			string autoSaveFilePathToDelete = null;
			if (this.settingsRoot.AutoSaved && (!PathEx.Equals(absoluteFilePath, this.settingsHandler.SettingsFilePath)))
				autoSaveFilePathToDelete = this.settingsHandler.SettingsFilePath;

			// Set the new file path...
			this.settingsHandler.SettingsFilePath = absoluteFilePath;

			// ...potentially save linked settings...
			if (this.settingsHandler.Settings.LinkedSettingsHaveChanged) // Not (yet) distinguishing explicit/implicit.
			{
				bool isCanceled;
				if (!TrySaveLinkedSettings(true, true, out isCanceled))
					return (false);
			}

			// ...and save the terminal itself:
			return (SaveToFile(false, autoSaveFilePathToDelete));
		}

		/// <param name="isAutoSave">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="autoSaveFilePathToDelete">
		/// The path to the former auto saved file, it will be deleted if the file can successfully
		/// be stored in the new location.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool SaveToFile(bool isAutoSave, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving terminal...");

			bool success;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				if (!isAutoSave)
					this.fileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				OnSaved(new SavedEventArgs(this.settingsHandler.SettingsFilePath, isAutoSave));
				OnTimedStatusTextRequest("Terminal saved.");

				if (!isAutoSave)
					SetRecent(this.settingsHandler.SettingsFilePath);

				// Try to delete existing auto save file:
				if (!string.IsNullOrEmpty(autoSaveFilePathToDelete))
				{
					// Ensure that this is not the current file!
					if (!PathEx.Equals(autoSaveFilePathToDelete, this.settingsHandler.SettingsFilePath))
						FileEx.TryDelete(autoSaveFilePathToDelete);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Error saving terminal!");

				OnFixedStatusTextRequest("Error saving terminal!");
				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to save terminal file", this.settingsHandler.SettingsFilePath, ex),
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Terminal not saved!");

				success = false;
			}

			return (success);
		}

		/// <remarks>
		/// Linked predefined commands pages shall be saved before the terminal itself is for two reasons:
		///  > Changing links or even unlinking must happen prior to saving the terminal itself.
		///  > Symmetricity with loading, where the terminal itself has to be loaded first.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		protected virtual bool TrySaveLinkedSettings(bool userInteractionIsAllowed, bool canBeCanceled, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in TryLoadLinkedSettings() further below.
			// Changes here may have to be applied there too.

			int successCount = 0;
			int linkedCount = this.settingsHandler.Settings.PredefinedCommand.Pages.LinkedToFilePathCount;
			string pageOrPages = ((linkedCount == 1) ? "page" : "pages");

			OnFixedStatusTextRequest("Saving linked predefined command " + pageOrPages + "...");

			foreach (var linkedPage in (this.settingsHandler.Settings.PredefinedCommand.Pages.Where(p => p.IsLinkedToFilePath)))
			{
				DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler;

				// Load linked page:
				bool hasChanged;
				if (TryLoadLinkedPredefinedCommandPageConsiderately(linkedPage, false, userInteractionIsAllowed, canBeCanceled, out linkedSettingsHandler, out hasChanged, out isCanceled)) {
					if (hasChanged)
						this.settingsHandler.Settings.PredefinedCommand.SetChanged();

					if (!linkedPage.IsLinkedToFilePath) // = is no longer linked, i.e. did get unlinked.
						continue; // with next page.

					// Nothing else to do, successCount++ is only done on successful save.
				}
				else {
					if (isCanceled)
						return (false);
					else
						continue; // with next page.
				}

				// Compare pages, save linked page only if needed:
				var explicitHaveChanged = !linkedSettingsHandler.Settings.Page.EqualsEffectivelyInUse(linkedPage);
				if (explicitHaveChanged)
				{
					if (TrySaveLinkedCommandPageConsiderately(linkedPage, linkedSettingsHandler, userInteractionIsAllowed, out hasChanged, out isCanceled)) {
						if (hasChanged)
							this.settingsHandler.Settings.PredefinedCommand.SetChanged();

						if (linkedPage.IsLinkedToFilePath) // = is still linked, i.e. did not get unlinked.
							successCount++;
						else
							continue; // with next page.
					}
					else {
						if (isCanceled)
							return (false);
						else
							continue; // with next page.
					}
				}
				else
				{
					successCount++;
				}
			} // foreach (linkedPage)

			linkedCount = this.settingsHandler.Settings.PredefinedCommand.Pages.LinkedToFilePathCount; // Update to account for pages that got unlinked.
			pageOrPages = ((linkedCount == 1) ? "page" : "pages");

			if (successCount == linkedCount)
				OnTimedStatusTextRequest("Linked predefined command " + pageOrPages + " saved or are still up-to-date.");
			else if (successCount > 0)
				OnTimedStatusTextRequest("Linked predefined command " + pageOrPages + " partly saved or are still up-to-date.");
			else
				OnFixedStatusTextRequest("Linked predefined command " + pageOrPages + " not saved and are not up-to-date!");

			isCanceled = false;
			return (successCount == linkedCount);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected virtual bool TrySaveLinkedCommandPageConsiderately(PredefinedCommandPage linkedPage,
		                                                             DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler,
		                                                             bool userInteractionIsAllowed,
		                                                             out bool hasChanged, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in SaveConsiderately() further above.
			// Changes here may have to be applied there too.

			// Attention:
			// Similar code exists in TryLoadLinkedPredefinedCommandPageConsiderately() below.
			// Changes here may have to be applied there too.

			if (!FileEx.IsWritable(linkedSettingsHandler.SettingsFilePath) || !File.Exists(linkedSettingsHandler.SettingsFilePath))
			{
				if (userInteractionIsAllowed) {
					string linkFilePathNewOrConfirmed;
					bool doUnlink;
					if (RequestRestrictedSaveCommandPageFromUser(linkedPage.LinkFilePath, out linkFilePathNewOrConfirmed, out doUnlink, out isCanceled)) {
						linkedPage.LinkFilePath = linkFilePathNewOrConfirmed;
						hasChanged = true;
					}
					else if (doUnlink) {
						linkedPage.Unlink();
						hasChanged = true;
						return (true);
					}
					else {
						hasChanged = false;
						return (false); // Let save of explicit change fail if file is restricted and user doesn't fix it.
					}
				}
				else {
					hasChanged = false;
					isCanceled = false;
					return (false); // Let save of explicit change fail if file is restricted.
				}
			}
			else
			{
				hasChanged = false;
				isCanceled = false;
			}

			// Save is feasible, update settings and then save:
			linkedSettingsHandler.Settings.Page.UpdateEffectivelyInUse(linkedPage); // Clone is done by the Update...() method.

			try
			{
				linkedSettingsHandler.Save();
				return (true);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(Terminal), ex, "Error saving linked predefined command page!");

				if (userInteractionIsAllowed)
				{
					OnFixedStatusTextRequest("Error saving linked predefined command page!");
					var dr = OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Unable to save linked predefined command page file", linkedSettingsHandler.SettingsFilePath, ex),
						"Linked File Error",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error
					);
					OnTimedStatusTextRequest("Linked predefined command page not saved!");

					isCanceled = (dr == DialogResult.Cancel);
				}

				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Readability.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected virtual bool TryLoadLinkedPredefinedCommandPageConsiderately(PredefinedCommandPage linkedPage, bool doUpdateLinkedPage,
		                                                                       bool userInteractionIsAllowed, bool canBeCanceled,
		                                                                       out DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler,
		                                                                       out bool hasChanged, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in SaveConsiderately() further above.
			// Changes here may have to be applied there too.

			// Attention:
			// Similar code exists in TrySaveLinkedPredefinedCommandPageConsiderately() further above.
			// Changes here may have to be applied there too.

			var isFirst = true;
			var currentFilePath = linkedPage.LinkFilePath;
			var currentSettingsHandler = new DocumentSettingsHandler<CommandPageSettingsRoot>();

			try
			{
				hasChanged = false;
				isCanceled = false;

				while (!string.IsNullOrEmpty(currentFilePath))
				{
					if (!File.Exists(currentFilePath))
					{
						if (userInteractionIsAllowed) {
							string linkFilePathNewOrConfirmed;
							bool doUnlink;
							if (RequestRestrictedOpenCommandPageFromUser(currentFilePath, canBeCanceled, out linkFilePathNewOrConfirmed, out doUnlink, out isCanceled)) {
								currentFilePath = linkFilePathNewOrConfirmed;

								if (isFirst) {
									isFirst = false;
									linkedPage.LinkFilePath = linkFilePathNewOrConfirmed;
									hasChanged = true;
								}
							}
							else if (doUnlink) {
								linkedPage.Unlink();
								linkedSettingsHandler = null;
								hasChanged = true;
								return (true);
							}
							else {
								linkedSettingsHandler = null;
								return (false); // Let save of explicit change fail if file is restricted and user doesn't fix it.
							}
						}
						else {
							linkedSettingsHandler = null;
							isCanceled = false;
							return (false); // Let save of explicit change fail if file is restricted.
						}
					}

					// Load is feasible:
					currentSettingsHandler.SettingsFilePath = currentFilePath;
					if (currentSettingsHandler.Load())
						currentFilePath = currentSettingsHandler.Settings.Page.LinkFilePath; // Either 'null' or valid.
				}

				linkedSettingsHandler = currentSettingsHandler;

				if (doUpdateLinkedPage)
					linkedPage.UpdateEffectivelyInUse(linkedSettingsHandler.Settings.Page); // Clone is done by the Update...() method.

				return (true);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(Terminal), ex, "Error retrieving linked predefined command page!");

				linkedSettingsHandler = null;
				hasChanged = false;

				if (userInteractionIsAllowed)
				{
					OnFixedStatusTextRequest("Error retrieving linked predefined command page!");
					var dr = OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Unable to retrieve linked predefined command page file", currentFilePath, ex),
						"Linked File Error",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error
					);
					OnTimedStatusTextRequest("Linked predefined command page not retrieved!");

					isCanceled = (dr == DialogResult.Cancel);
				}
				else
				{
					isCanceled = false;
				}

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool TryLoadLinkedSettings(bool userInteractionIsAllowed, bool canBeCanceled, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in TrySaveLinkedCommandPages() further above.
			// Changes here may have to be applied there too.

			int successCount = 0;
			int linkedCount = this.settingsHandler.Settings.PredefinedCommand.Pages.LinkedToFilePathCount;
			string pageOrPages = ((linkedCount == 1) ? "page" : "pages");

			OnFixedStatusTextRequest("Loading linked predefined command " + pageOrPages + "...");

			foreach (var linkedPage in (this.settingsHandler.Settings.PredefinedCommand.Pages.Where(p => p.IsLinkedToFilePath)))
			{
				DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler;

				// Load linked page:
				bool hasChanged;
				if (TryLoadLinkedPredefinedCommandPageConsiderately(linkedPage, true, userInteractionIsAllowed, canBeCanceled, out linkedSettingsHandler, out hasChanged, out isCanceled)) {
					if (hasChanged)
						this.settingsHandler.Settings.PredefinedCommand.SetChanged();

					if (linkedPage.IsLinkedToFilePath) // = is still linked, i.e. did not get unlinked.
						successCount++;
					else
						continue; // with next page.
				}
				else {
					if (isCanceled)
						return (false);
					else
						continue; // with next page.
				}
			} // foreach (linkedPage)

			linkedCount = this.settingsHandler.Settings.PredefinedCommand.Pages.LinkedToFilePathCount; // Update to account for pages that got unlinked.
			pageOrPages = ((linkedCount == 1) ? "page" : "pages");

			if (successCount == linkedCount)
				OnTimedStatusTextRequest("Linked predefined command " + pageOrPages + " loaded.");
			else if (successCount > 0)
				OnTimedStatusTextRequest("Linked predefined command " + pageOrPages + " partly loaded.");
			else
				OnFixedStatusTextRequest("Linked predefined command " + pageOrPages + " not loaded!");

			isCanceled = false;
			return (successCount == linkedCount);
		}

		#endregion

		#region Close
		//==========================================================================================
		// Close
		//==========================================================================================

		/// <summary>
		/// Closes the terminal and prompts if needed if settings have changed.
		/// </summary>
		/// <remarks>
		/// In case of a workspace close, <see cref="CloseConsiderately"/> below must be called
		/// with the first argument set to <c>true</c>.
		///
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminal(s), thus, this method will not try to auto save.
		/// </remarks>
		public virtual bool Close()
		{
			return (CloseConsiderately(false, true, false, true)); // See remarks above.
		}

		/// <summary>
		/// Closes the terminal and tries to auto save if desired.
		/// </summary>
		/// <remarks>
		/// Attention:
		/// This method is needed for MDI applications. In case of MDI parent/application closing,
		/// Close() of the terminal is called before Close() of the workspace. Without taking care
		/// of this, the workspace would be saved after the terminal has already been closed, i.e.
		/// removed from the workspace. Therefore, the terminal has to signal such cases to the
		/// workspace.
		///
		/// Cases (similar to cases in Model.Workspace):
		/// - Workspace close
		///   - auto,   no file,       auto save    => auto save, if it fails => nothing  : (w1a)
		///   - auto,   no file,       no auto save => nothing                            : (w1b)
		///   - auto,   existing file, auto save    => auto save, if it fails => delete   : (w2a)
		///   - auto,   existing file, no auto save => delete                             : (w2b)
		///   - normal, no file                     => N/A (normal files have been saved) : (w3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (w4a)
		///   - normal, existing file, no auto save => question                           : (w4b)
		/// - Terminal close
		///   - auto,   no file                     => nothing                            : (t1)
		///   - auto,   existing file               => delete                             : (t2)
		///   - normal, no file                     => N/A (normal files have been saved) : (t3)
		///   - normal, no file anymore             => question                           :  --
		///   - normal, existing file, auto save    => auto save, if it fails => question : (t4a)
		///   - normal, existing file, no auto save => question                           : (t4b)
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		public virtual bool CloseConsiderately(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			AssertNotDisposed();

			OnFixedStatusTextRequest("Closing terminal...");

			// Keep info of existing former auto file:
			bool formerExistingAutoFileAutoSaved = this.settingsRoot.AutoSaved;
			string formerExistingAutoFilePath = null;
			if (this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFileExists)
				formerExistingAutoFilePath = this.settingsHandler.SettingsFilePath;

			// -------------------------------------------------------------------------------------
			// Evaluate save requirements:
			// -------------------------------------------------------------------------------------

			bool success = false;

			// Do not try to auto save if save is not inteded at all.
			if (autoSaveIsAllowed && !doSave)
				autoSaveIsAllowed = false;

			// Do not neither try to auto save nor manually save if there is no existing file (w1, w3, t1, t3),
			// except in case of w1a, i.e. when the file has never been loaded so far.
			if (autoSaveIsAllowed && !this.settingsHandler.SettingsFileExists)
			{
				if (!isWorkspaceClose || this.settingsHandler.SettingsFileSuccessfullyLoaded)
				{
					doSave = false;
					autoSaveIsAllowed = false;
				}
			}

			if (isWorkspaceClose)
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, save is not required but try to auto save if desired.
					if (autoSaveIsAllowed)
						doSave = true;
					else
						success = true;
				}
				else
				{
					// Explicit have changed, save is required.
				}
			}
			else
			{
				if (!this.settingsRoot.HaveChanged)
				{
					// Nothing has changed, no need to do anything.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else if (!this.settingsRoot.ExplicitHaveChanged)
				{
					// Implicit have changed, but do not try to auto save since user intended to close.
					doSave = false;
					autoSaveIsAllowed = false;
					success = true;
				}
				else
				{
					// Explicit have changed, save is required.
				}
			}

			// -------------------------------------------------------------------------------------
			// Try auto save if allowed:
			// -------------------------------------------------------------------------------------

			if (!success && doSave)
				success = TrySaveConsideratelyWithoutUserInteraction(isWorkspaceClose, autoSaveIsAllowed); // Try auto save.

			// -------------------------------------------------------------------------------------
			// If not successfully saved so far, evaluate next step according to rules above:
			// -------------------------------------------------------------------------------------

			// Normal file (w3, w4, t3, t4):
			if (!success && doSave && !this.settingsRoot.AutoSaved)
			{
				var dr = OnMessageInputRequest
				(
					"Save terminal?",
					IndicatedName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
				);

				switch (dr)
				{
					case DialogResult.Yes: success = Save(); break;
					case DialogResult.No:  success = true;   break;

					case DialogResult.Cancel:
					default:
					{
						OnTimedStatusTextRequest("Canceled, terminal not closed.");
						return (false);
					}
				}
			}

			// Delete existing former auto file which is no longer needed (w2a):
			if (isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && !success)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Delete existing former auto file which is no longer needed (w2b):
			if (isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && autoDeleteIsRequested)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Delete existing former auto file which is no longer needed (t2):
			if (!isWorkspaceClose && formerExistingAutoFileAutoSaved && (formerExistingAutoFilePath != null) && autoDeleteIsRequested)
			{
				FileEx.TryDelete(formerExistingAutoFilePath);
				this.settingsHandler.ResetSettingsFilePath();
				success = true;
			}

			// Write-protected file:
			if (!success && !this.settingsHandler.SettingsFileIsWritable)
			{
				success = true; // Consider it successful if file shall not be saved.
			}

			// No file (w1, t1):
			if (!success && !this.settingsHandler.SettingsFileExists)
			{
				this.settingsHandler.ResetSettingsFilePath();
				success = true; // Consider it successful if there was no file to save.
			}

			// -------------------------------------------------------------------------------------
			// Stop the underlying items:
			// -------------------------------------------------------------------------------------

			if (success && this.terminal.IsStarted)
			{
				success = StopIO(false);
			}

			if (success && this.log.AnyIsOn)
			{
				SwitchLogOff();
			}

			// -------------------------------------------------------------------------------------
			// Finally, cleanup and signal state:
			// -------------------------------------------------------------------------------------

			if (success)
			{
				// Status text request must be before closed event, closed event may close the view:
				OnTimedStatusTextRequest("Terminal successfully closed.");

				// Discard potential exceptions already BEFORE signalling the close! Required to
				// prevent exceptions on still pending asynchronous callbacks trying to synchronize
				// event callbacks onto the terminal form which is going to be closed/disposed by
				// the handler of the 'Closed' event below!
				this.eventHelper.DiscardAllExceptions();

				OnClosed(new ClosedEventArgs(isWorkspaceClose));

				// The terminal shall dispose of itself to free all resources for sure. It must be
				// done AFTER it raised the 'Closed' event and all subscribers of the event may still
				// refer to a non-disposed object. This is especially important, as the order of the
				// subscribers is not fixed, i.e. 'Model.Workspace' may dispose of the terminal
				// before 'View.Terminal' receives the event callback!
				Dispose();

				return (true);
			}
			else
			{
				OnTimedStatusTextRequest("Terminal not closed.");

				return (false);
			}
		}

		#endregion

		#region Recent Files
		//==========================================================================================
		// Recent Files
		//==========================================================================================

		/// <summary>
		/// Update recent entry.
		/// </summary>
		/// <param name="recentFile">Recent file.</param>
		private static void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Add(recentFile);
			ApplicationSettings.LocalUserSettings.RecentFiles.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveLocalUserSettings();
		}

		#endregion

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		#region Terminal > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminal > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        += terminal_IOChanged;
				this.terminal.IOControlChanged += terminal_IOControlChanged;
				this.terminal.IOError          += terminal_IOError;
			#if (WITH_SCRIPTING)
				this.terminal.SendingPacket    += terminal_SendingPacket;
			#endif
				this.terminal.RawChunkSent     += terminal_RawChunkSent;
				this.terminal.RawChunkReceived += terminal_RawChunkReceived;

				this.terminal.DisplayElementsTxAdded          += terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       += terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          += terminal_DisplayElementsRxAdded;
			////this.terminal.CurrentDisplayLineTxChanged     += terminal_CurrentDisplayLineTxChanged;    Not needed (yet).
			////this.terminal.CurrentDisplayLineBidirChanged  += terminal_CurrentDisplayLineBidirChanged; Not needed (yet).
				this.terminal.CurrentDisplayLineRxChanged     += terminal_CurrentDisplayLineRxChanged;
				this.terminal.CurrentDisplayLineTxReplaced    += terminal_CurrentDisplayLineTxReplaced;
				this.terminal.CurrentDisplayLineBidirReplaced += terminal_CurrentDisplayLineBidirReplaced;
				this.terminal.CurrentDisplayLineRxReplaced    += terminal_CurrentDisplayLineRxReplaced;
				this.terminal.CurrentDisplayLineTxCleared     += terminal_CurrentDisplayLineTxCleared;
				this.terminal.CurrentDisplayLineBidirCleared  += terminal_CurrentDisplayLineBidirCleared;
				this.terminal.CurrentDisplayLineRxCleared     += terminal_CurrentDisplayLineRxCleared;
				this.terminal.DisplayLinesTxAdded             += terminal_DisplayLinesTxAdded;
				this.terminal.DisplayLinesBidirAdded          += terminal_DisplayLinesBidirAdded;
				this.terminal.DisplayLinesRxAdded             += terminal_DisplayLinesRxAdded;
			#if (WITH_SCRIPTING)
				this.terminal.ScriptPacketReceived    += terminal_ScriptPacketReceived;
				this.terminal.ScriptMessageReceived   += terminal_ScriptMessageReceived;
			#endif
				this.terminal.RepositoryTxCleared     += terminal_RepositoryTxCleared;
				this.terminal.RepositoryBidirCleared  += terminal_RepositoryBidirCleared;
				this.terminal.RepositoryRxCleared     += terminal_RepositoryRxCleared;
				this.terminal.RepositoryTxReloaded    += terminal_RepositoryTxReloaded;
				this.terminal.RepositoryBidirReloaded += terminal_RepositoryBidirReloaded;
				this.terminal.RepositoryRxReloaded    += terminal_RepositoryRxReloaded;
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged        -= terminal_IOChanged;
				this.terminal.IOControlChanged -= terminal_IOControlChanged;
				this.terminal.IOError          -= terminal_IOError;
			#if (WITH_SCRIPTING)
				this.terminal.SendingPacket    -= terminal_SendingPacket;
			#endif
				this.terminal.RawChunkSent     -= terminal_RawChunkSent;
				this.terminal.RawChunkReceived -= terminal_RawChunkReceived;

				this.terminal.DisplayElementsTxAdded          -= terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       -= terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          -= terminal_DisplayElementsRxAdded;
			////this.terminal.CurrentDisplayLineTxChanged     -= terminal_CurrentDisplayLineTxChanged;    Not needed (yet).
			////this.terminal.CurrentDisplayLineBidirChanged  -= terminal_CurrentDisplayLineBidirChanged; Not needed (yet).
				this.terminal.CurrentDisplayLineRxChanged     -= terminal_CurrentDisplayLineRxChanged;
				this.terminal.CurrentDisplayLineTxReplaced    -= terminal_CurrentDisplayLineTxReplaced;
				this.terminal.CurrentDisplayLineBidirReplaced -= terminal_CurrentDisplayLineBidirReplaced;
				this.terminal.CurrentDisplayLineRxReplaced    -= terminal_CurrentDisplayLineRxReplaced;
				this.terminal.CurrentDisplayLineTxCleared     -= terminal_CurrentDisplayLineTxCleared;
				this.terminal.CurrentDisplayLineBidirCleared  -= terminal_CurrentDisplayLineBidirCleared;
				this.terminal.CurrentDisplayLineRxCleared     -= terminal_CurrentDisplayLineRxCleared;
				this.terminal.DisplayLinesTxAdded             -= terminal_DisplayLinesTxAdded;
				this.terminal.DisplayLinesBidirAdded          -= terminal_DisplayLinesBidirAdded;
				this.terminal.DisplayLinesRxAdded             -= terminal_DisplayLinesRxAdded;
			#if (WITH_SCRIPTING)
				this.terminal.ScriptPacketReceived    -= terminal_ScriptPacketReceived;
				this.terminal.ScriptMessageReceived   -= terminal_ScriptMessageReceived;
			#endif
				this.terminal.RepositoryTxCleared     -= terminal_RepositoryTxCleared;
				this.terminal.RepositoryBidirCleared  -= terminal_RepositoryBidirCleared;
				this.terminal.RepositoryRxCleared     -= terminal_RepositoryRxCleared;
				this.terminal.RepositoryTxReloaded    -= terminal_RepositoryTxReloaded;
				this.terminal.RepositoryBidirReloaded -= terminal_RepositoryBidirReloaded;
				this.terminal.RepositoryRxReloaded    -= terminal_RepositoryRxReloaded;
			}
		}

		private void CloseAndDisposeTerminal()
		{
			if (this.terminal != null)
			{
				this.terminal.Close();
				this.terminal.Dispose();
				this.terminal = null;
			}
		}

		#endregion

		#region Terminal > Auxiliary
		//------------------------------------------------------------------------------------------
		// Terminal > Auxiliary
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOStatusToDisplayLine(DateTime ts)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;

			var virtualLine = new Domain.DisplayLine(5); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			virtualLine.Add(new Domain.DisplayElement.DeviceInfo(IOStatusText, left, right));
			virtualLine.Add(new Domain.DisplayElement.LineBreak());

			return (virtualLine);
		}

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOControlToDisplayLine(DateTime ts, Domain.IOControlEventArgs e, bool includeIOStatusText)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;

			var virtualLine = new Domain.DisplayLine(1 + 2 + 2 + e.Texts.Count + 1); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			if (includeIOStatusText)
			{
				virtualLine.Add(new Domain.DisplayElement.DeviceInfo(IOStatusText, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			var c = new Domain.DisplayElementCollection(e.Texts.Count); // Preset the required capacity to improve memory management.
			foreach (var t in e.Texts)
			{                               // 'IOControlInfo' elements are inline elements, thus neither add info separators nor content spaces inbetween.
				c.Add(new Domain.DisplayElement.IOControlInfo((Domain.Direction)e.Direction, t));
			}
			virtualLine.AddRange(c);

			virtualLine.Add(new Domain.DisplayElement.LineBreak());
			return (virtualLine);
		}

		/// <summary></summary>
		protected virtual Domain.DisplayLine IOErrorToDisplayLine(DateTime ts, Domain.IOErrorEventArgs e, bool includeIOStatusText)
		{
			var sep   = SettingsRoot.Display.InfoSeparatorCache;
			var left  = SettingsRoot.Display.InfoEnclosureLeftCache;
			var right = SettingsRoot.Display.InfoEnclosureRightCache;

			var virtualLine = new Domain.DisplayLine(1 + 2 + 2 + 1 + 1); // Preset the required capacity to improve memory management.
			virtualLine.Add(new Domain.DisplayElement.LineStart());

			if (SettingsRoot.Display.ShowTimeStamp)
			{
				virtualLine.Add(new Domain.DisplayElement.TimeStampInfo(ts, SettingsRoot.Display.TimeStampFormat, SettingsRoot.Display.TimeStampUseUtc, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			if (includeIOStatusText)
			{
				virtualLine.Add(new Domain.DisplayElement.DeviceInfo(IOStatusText, left, right));
				virtualLine.Add(new Domain.DisplayElement.InfoSeparator(sep));
			}

			virtualLine.Add(new Domain.DisplayElement.ErrorInfo((Domain.Direction)e.Direction, e.Message, (e.Severity == Domain.IOErrorSeverity.Acceptable)));

			virtualLine.Add(new Domain.DisplayElement.LineBreak());
			return (virtualLine);
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Local field to maintain connection state in order to be able to detect a change of the
		/// connection state.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool terminal_IOChanged_hasBeenConnected;

		private void terminal_IOChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Log:
			if (this.log.AnyControlIsOn)
				this.log.WriteLine(IOStatusToDisplayLine(DateTime.Now), Log.LogChannel.Control);

			// Forward:
			OnIOChanged(e);

			// Attention, the 'IOChanged' event could trigger close = dispose of terminal!

			bool hasBeenConnected = this.terminal_IOChanged_hasBeenConnected;
			bool isConnectedNow = ((this.terminal != null) ? (this.terminal.IsConnected) : (false));

			if (!IsDisposed)
			{
				if      ( isConnectedNow && !hasBeenConnected)
				{
					var now = DateTime.Now; // Ensure that all use exactly the same instant.

					this.activeConnectChrono.Restart(now); // Restart, i.e. reset and start from zero.
					this.totalConnectChrono.Start(now);    // Start again, i.e. continue at last time.

					this.terminal.InitialTimeStamp = now;       // The initial time stamp is used for
					                                            // time spans. Consequently, the spans
					if (this.settingsRoot.Display.ShowTimeSpan) // will be based on the active connect
						this.terminal.RefreshRepositories();    // time, not the total connect time.
				}
				else if (!isConnectedNow && hasBeenConnected)
				{
					this.activeConnectChrono.Stop();
					this.totalConnectChrono.Stop();
				}
			}

			this.terminal_IOChanged_hasBeenConnected = isConnectedNow;
		}

		private void terminal_IOControlChanged(object sender, Domain.IOControlEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Log:
			if ((e.Texts != null) && (e.Texts.Count > 0))
			{
				if (this.log.AnyControlIsOn)                                   // Status text is always included (so far).
					this.log.WriteLine(IOControlToDisplayLine(e.TimeStamp, e, true), Log.LogChannel.Control);

				if (this.log.AnyNeatIsOn) // Workaround to bug #447 "Acceptable errors (e.g. <RX PARITY ERROR>) and I/O control events (e.g. <RTS=on>) are not contained in neat logs."
				{
					var dl = IOControlToDisplayLine(e.TimeStamp, e, false);
					this.log.WriteLine(dl, Log.LogChannel.NeatTx);
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
					this.log.WriteLine(dl, Log.LogChannel.NeatRx);
				}
			}

			// Forward:
			OnIOControlChanged(e);
		}

		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Log:
			if (this.log.AnyControlIsOn)                                 // Status text is always included (so far).
				this.log.WriteLine(IOErrorToDisplayLine(e.TimeStamp, e, true), Log.LogChannel.Control);

			if (this.log.AnyNeatIsOn) // Workaround to bug #447 "Acceptable errors (e.g. <RX PARITY ERROR>) and I/O control events (e.g. <RTS=on>) are not contained in neat logs."
			{
				var dl = IOErrorToDisplayLine(e.TimeStamp, e, false);
				this.log.WriteLine(dl, Log.LogChannel.NeatTx);
				this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
				this.log.WriteLine(dl, Log.LogChannel.NeatRx);
			}

			// Forward:
			OnIOError(e);
		}

	#if (WITH_SCRIPTING)
		private void terminal_SendingPacket(object sender, Domain.ModifiablePacketEventArgs e)
		{
			OnSendingPacket(e);
		}
	#endif

		/// <remarks>
		/// Interval can be quite long, because...
		/// ...first request will be done immediately.
		/// ...timed requests will be shown for 2 seconds.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly long TimedStatusTextRequestTickInterval = StopwatchEx.TimeToTicks(757); // = prime number around 750 milliseconds.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private long terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private long terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp; // = 0;

		/// <remarks>
		/// \remind (2017-08-27 / MKY) (bug #383 freeze while receiving a lot of fast data)
		/// In case of a lot of fast data, this event is raised very often. This handler itself
		/// raises up to three events, thus leading to a sequence of events. If these events need
		/// to be synchronized onto the main thread, this likely results in poor performance. This
		/// situation is anticipated in two ways:
		///  > The <see cref="TimedStatusTextRequest"/> will only be raised
		///    each <see cref="TimedStatusTextRequestTickInterval"/> milliseconds.
		///  > The <see cref="IOCountChanged_Promptly"/> and <see cref="IORateChanged_Promptly"/> events
		///    will not be used by the terminal form. Instead, the values will synchronously be retrieved
		///    when processing <see cref="DisplayElementsTxAdded"/>, <see cref="DisplayElementsRxAdded"/>,
		///    <see cref="CurrentDisplayLineTxReplaced"/>, <see cref="CurrentDisplayLineRxReplaced"/>,
		///    <see cref="CurrentDisplayLineTxCleared"/>, <see cref="CurrentDisplayLineRxCleared"/>,
		///    <see cref="DisplayLinesTxAdded"/> and <see cref="DisplayLinesRxAdded"/> events.
		///    In addition, the <see cref="IORateChanged_Decimated"/> event is used to get
		///    notified on updates after transmission.
		/// </remarks>
		/// <remarks>
		/// This event is raised when a chunk is sent by the <see cref="UnderlyingIOProvider"/>.
		/// The event is not raised on reloading, reloading is done by the <see cref="Domain.Terminal"/>.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkSent(object sender, EventArgs<Domain.RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Sending...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Count:
			this.txByteCount += e.Value.Content.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.txByteRate.Update(e.Value.Content.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Log:
			if (this.log.AnyRawIsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawTx);
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
			}

		#if (WITH_SCRIPTING)
			OnRawChunkSent(e);
		#endif
		}

		/// <remarks>
		/// \remind (2017-08-27 / MKY) (bug #383 freeze while receiving a lot of fast data)
		/// In case of a lot of fast data, this event is raised very often. This handler itself
		/// raises up to three events, thus leading to a sequence of events. If these events need
		/// to be synchronized onto the main thread, this likely results in poor performance. This
		/// situation is anticipated in two ways:
		///  > The <see cref="TimedStatusTextRequest"/> will only be raised
		///    each <see cref="TimedStatusTextRequestTickInterval"/> milliseconds.
		///  > The <see cref="IOCountChanged_Promptly"/> and <see cref="IORateChanged_Promptly"/> events
		///    will not be used by the terminal form. Instead, the values will synchronously be retrieved
		///    when processing <see cref="DisplayElementsTxAdded"/>, <see cref="DisplayElementsRxAdded"/>,
		///    <see cref="CurrentDisplayLineTxReplaced"/>, <see cref="CurrentDisplayLineRxReplaced"/>,
		///    <see cref="CurrentDisplayLineTxCleared"/>, <see cref="CurrentDisplayLineRxCleared"/>,
		///    <see cref="DisplayLinesTxAdded"/> and <see cref="DisplayLinesRxAdded"/> events.
		///    In addition, the <see cref="IORateChanged_Decimated"/> event is used to get
		///    notified on updates after transmission.
		/// </remarks>
		/// <remarks>
		/// This event is raised when a chunk is received by the <see cref="UnderlyingIOProvider"/>.
		/// The event is not raised on reloading, reloading is done by the <see cref="Domain.Terminal"/>.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RawChunkSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_RawChunkReceived(object sender, EventArgs<Domain.RawChunk> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Receiving...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Rate:
			this.rxByteCount += e.Value.Content.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.rxByteRate.Update(e.Value.Content.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Log:
			if (this.log.AnyRawIsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
				this.log.Write(e.Value, Log.LogChannel.RawRx);
			}

			// AutoAction (by specification only active on receive-path):
			if (this.settingsRoot.AutoAction.IsActive)
			{
				if (this.autoActionClearRepositoriesOnSubsequentRxIsArmed)
				{
					this.autoActionClearRepositoriesOnSubsequentRxIsArmed = false;

					byte[] triggerSequence = null;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
							triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<AutoAction, byte[], DateTime>(terminal_RawChunkReceived_InvokeAutoActionAsync);
					asyncInvoker.BeginInvoke(AutoAction.ClearRepositories, triggerSequence, e.Value.TimeStamp, null, null);
				}                                    // ClearRepositories is to be invoked, not ClearRepositoriesOnSubsequentRx!

				bool isTriggered = false;

				foreach (byte b in e.Value.Content) // Note the feature request #366 related remark in the method header.
				{
					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
						{
							if (this.autoActionTriggerHelper.EnqueueAndMatchTrigger(b))
								isTriggered = true;
						}
						else
						{
							break; // Break the for-loop if action got disposed in the meantime.
						}          // Though unlikely, it may happen when deactivating action
					}              // while receiving a very large chunk.
				}

				if (isTriggered)
				{
					// Invoke sending on different thread than the receive thread:
					byte[] triggerSequence = null;

					lock (this.autoActionTriggerHelperSyncObj)
					{
						if (this.autoActionTriggerHelper != null)
							triggerSequence = this.autoActionTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<AutoAction, byte[], DateTime>(terminal_RawChunkReceived_InvokeAutoActionAsync);
					asyncInvoker.BeginInvoke(this.settingsRoot.AutoAction.Action, triggerSequence, e.Value.TimeStamp, null, null);

					// Mark the received chunk as needed (triggered):
					switch ((AutoAction)this.settingsRoot.AutoAction.Action)
					{
						case AutoAction.Filter:   e.Attribute = Domain.LineChangeAttribute.Filter;    break;
						case AutoAction.Suppress: e.Attribute = Domain.LineChangeAttribute.Suppress;  break;
						default:                  e.Attribute = Domain.LineChangeAttribute.Highlight; break;
					}
				}
				else
				{
					// Mark the received chunk as needed (non-triggered):
					switch ((AutoAction)this.settingsRoot.AutoAction.Action)
					{
						case AutoAction.Filter:   e.Attribute = Domain.LineChangeAttribute.SuppressIfNotFiltered;           break;
						case AutoAction.Suppress: e.Attribute = Domain.LineChangeAttribute.SuppressIfSubsequentlyTriggered; break;
					}
				}
			}

			// AutoResponse (by specification only active on receive-path):
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				bool isTriggered = false;

				foreach (byte b in e.Value.Content) // Note the feature request #366 related remark in the method header.
				{
					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
						{
							if (this.autoResponseTriggerHelper.EnqueueAndMatchTrigger(b))
								isTriggered = true;
						}
						else
						{
							break; // Break the for-loop if response got disposed in the meantime.
						}          // Though unlikely, it may happen when deactivating response
					}              // while receiving a very large chunk.
				}

				if (isTriggered)
				{
					// Invoke sending on different thread than the receive thread:
					byte[] triggerSequence = null;

					lock (this.autoResponseTriggerHelperSyncObj)
					{
						if (this.autoResponseTriggerHelper != null)
							triggerSequence = this.autoResponseTriggerHelper.TriggerSequence;
					}

					var asyncInvoker = new Action<byte[]>(terminal_RawChunkReceived_SendAutoResponseAsync);
					asyncInvoker.BeginInvoke(triggerSequence, null, null);

					// Highlighting is done for all auto responses (so far):
					e.Attribute = Domain.LineChangeAttribute.Highlight;
				}
			}

		#if (WITH_SCRIPTING)
			OnRawChunkReceived(e);
		#endif
		}

		private void terminal_RawChunkReceived_InvokeAutoActionAsync(AutoAction action, byte[] triggerSequence, DateTime ts)
		{
			InvokeAutoAction(action, triggerSequence, ts);
		}

		private void terminal_RawChunkReceived_SendAutoResponseAsync(byte[] triggerSequence)
		{
			SendAutoResponse(triggerSequence);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsTxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnDisplayElementsTxAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsBidirAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnDisplayElementsBidirAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsRxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnDisplayElementsRxAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

	////[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirChanged", Rationale = "The terminal synchronizes display element/line processing.")]
	////[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxChanged", Rationale = "The terminal synchronizes display element/line processing.")]
	////private void terminal_CurrentDisplayLineTxChanged(object sender, Domain.DisplayLineChangeAttributeEventArgs e)
	////{
	////	if (IsDisposed)
	////		return; // Ensure not to handle events during closing anymore.
	////
	////	Not needed (yet).
	////}
	////
	////[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirChanged", Rationale = "The terminal synchronizes display element/line processing.")]
	////[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxChanged", Rationale = "The terminal synchronizes display element/line processing.")]
	////private void terminal_CurrentDisplayLineBidirChanged(object sender, Domain.DisplayLineChangeAttributeEventArgs e)
	////{
	////	if (IsDisposed)
	////		return; // Ensure not to handle events during closing anymore.
	////
	////	Not needed (yet).
	////}

		/// <remarks>
		/// Initially (2019-04..11 / YAT 2.1.0) the trigger detection was implemented per chunk, resulting in:
		///  > If trigger was located in a single chunk, all fine, as long as the chunk did not spread across multiple lines.
		///  > If trigger was spread across multiple chunks, all fine, also as long as the chunks do not spread across multiple lines.
		///  > If there was more than one trigger in a chunk, or last byte of one trigger and another complete one, only a single trigger was detected.
		///  > No way to trigger for text.
		///
		/// Potential approaches to overcome these limitations:
		///  a) Move trigger detection from <see cref="terminal_RawChunkReceived"/> to one of the underlying methods of
		///     <see cref="Domain.Terminal.ProcessAndSignalRawChunk"/>, where the chunks are being processed into lines.
		///  b) Keep trigger detection in model but move it from <see cref="terminal_RawChunkReceived"/> to a new
		///     'CurrentDisplayLineRxChanged' event. That event would have to include the current display line part (for
		///     text triggering) as well as the changed part (for byte sequence triggering).
		///     Advantages of this approach:
		///      > Keep settings complexity in model.
		///      > Keep well-defined interface <see cref="Domain.LineChangeAttribute"/> among model and domain.
		///  c) Completely move handling to model, possible as long as using the retaining approach:
		///      > "Simple" actions implemented in <see cref="terminal_DisplayElementsRxAdded"/> (immediate approach / element update mode).
		///      > Filter/Suppress implemented in <see cref="terminal_DisplayLinesRxAdded"/> (retaining approach / line update mode).
		///     Advantages of this approach:
		///      > Keep <see cref="Domain.Terminal"/> as simple as possible, really.
		///      > No interface among model and domain needed anymore.
		///  d) Refine trigger detection such it detects multiple triggers. This can easily be achieved by upgrading the
		///     'isTriggered' flag to a 'numOfTriggers' value.
		///
		/// There are additional ideas to further let these features evolve:
		///  > Move filter/suppress to separate options:
		///     + They not really are "actions" and are implemented partly differently then the true actions.
		///     + Possibility to filter and suppress in parallel.
		///     + Possibility to filter and suppress Tx as well.
		///     - Further (over)loads the main tool bar.
		///     - Lots of duplicated code.
		///  > Provide n automatic actions:
		///     + Possibility to filter/suppress/other in parallel.
		///     - Further complicates the main tool bar.
		///     - Further complicates usage.
		/// These ideas are technically possible but are considered (2019-11-21..22 / YAT 2.1.1) over the top.
		///  > Some inconsistency for filter/suppress is considered preferable over making things more complicated.
		///  > Limited to Rx is considered sufficient.
		///
		/// Approach c) was chosen (2019-11-21..22 / YAT 2.1.1).
		///
		/// The last state of the initial implementation can be found in SVN revisions #2701..#2707.
		/// </remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirChanged", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxChanged", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxChanged(object sender, Domain.DisplayLineChangeEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// PENDING !!!
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineTxReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineBidirReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineRxReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineTxCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineBidirCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnCurrentDisplayLineRxCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesTxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Count:
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.txLineRate.Update(e.Lines.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Display:
			OnDisplayLinesTxAdded(e);

			// Log:
			if (this.log.NeatTxIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatTx);
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesBidirAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

		////// Count:
		////this.bidirLineCount += e.Lines.Count would technically be possible, but doesn't make much sense.
		////OnIOCountChanged_Promptly(EventArgs.Empty);

		////// Rate:
		////if (this.bidirLineRate.Update(e.Lines.Count)) would technically be possible, but doesn't make much sense.
		////	OnIORateChanged_Promptly(EventArgs.Empty);

			// Display:
			OnDisplayLinesBidirAdded(e);

			// Log:
			if (this.log.NeatBidirIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
			}
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesRxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			// Count:
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.rxLineRate.Update(e.Lines.Count))
				OnIORateChanged_Promptly(EventArgs.Empty);

			// Display:
			OnDisplayLinesRxAdded(e);

			// Log:
			if (this.log.NeatRxIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatRx);
			}

			// AutoAction:
			if (this.settingsRoot.AutoAction.IsActive && (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines)
				{
					InvokeAutoAction(this.settingsRoot.AutoAction.Action, LineWithoutRxEolToOrigin(dl), dl.TimeStamp);
				}

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highligted.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesReceived' event, as the monitors already get updated
				// in the 'ElementsReceived' event above.
			}

			// AutoResponse:
			if (this.settingsRoot.AutoResponse.IsActive && (this.settingsRoot.AutoResponse.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines)
				{
					SendAutoResponse(LineWithoutRxEolToOrigin(dl));
				}

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highligted.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesReceived' event, as the monitors already get updated
				// in the 'ElementsReceived' event above.
			}
		}

		private byte[] LineWithoutRxEolToOrigin(Domain.DisplayLine dl)
		{
			var l = new List<byte>(dl.ElementsToOrigin());

			if (this.settingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Remove Rx EOL:
				if (this.settingsRoot.TextTerminal.ShowEol)
				{
					var rxEolSequence = textTerminal.RxEolSequence;
					l.RemoveRange((l.Count - rxEolSequence.Length), rxEolSequence.Length);
				}
			}

			return (l.ToArray());
		}

	#if (WITH_SCRIPTING)

		private void terminal_ScriptPacketReceived(object sender, Domain.PacketEventArgs e)
		{
			OnScriptPacketReceived(e);
		}

		private void terminal_ScriptMessageReceived(object sender, Domain.MessageEventArgs e)
		{
			OnScriptMessageReceived(e);
		}

	#endif // WITH_SCRIPTING

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryTxCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryBidirCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryRxCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxReloaded(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryTxReloaded(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirReloaded(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryBidirReloaded(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxReloaded(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnRepositoryRxReloaded(e);
		}

		#endregion

		#region Terminal > Check I/O
		//------------------------------------------------------------------------------------------
		// Terminal > Check I/O
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks the terminal's I/O port availability. If I/O port is not available, and settings
		/// allow so, user is asked whether to change to a different I/O port.
		/// </summary>
		/// <remarks>
		/// Note that only the availability of the I/O port is checked, not whether the port is
		/// already in use, because that may take quite some time and thus unnecessarily delay the
		/// open/check/start sequence.
		/// </remarks>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual CheckResult CheckIOAvailability()
		{
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
				{
					var portId = this.settingsRoot.Terminal.IO.SerialPort.PortId;
					if (portId != null)
						return (CheckSerialPortAvailability(portId));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					MKY.Net.IPNetworkInterfaceEx localInterface = this.settingsRoot.Terminal.IO.Socket.LocalInterface;
					if (localInterface != null)
						return (CheckLocalInterfaceAvailability(localInterface));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.UsbSerialHid:
				{
					var deviceInfo = this.settingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo;
					if (deviceInfo != null)
						return (CheckUsbDeviceAvailability(deviceInfo));
					else
						return (CheckResult.Ignore);
				}

				default: // Not (yet) useful for UDP/IP sockets.
				{
					return (CheckResult.OK); // All 'non-handled' cases.
				}
			}
		}

		private CheckResult CheckSerialPortAvailability(MKY.IO.Ports.SerialPortId portId)
		{
			OnFixedStatusTextRequest("Checking availability of " + portId + "...");

			var ports = new MKY.IO.Ports.SerialPortCollection();
			ports.FillWithAvailablePorts(false); // Explicitly not getting captions, thus faster.

			// Attention:
			// Similar code exists in View.Controls.SerialPortSelection.SetPortList().
			// Changes here may have to be applied there too!

			if (ports.Count > 0)
			{
				if (ports.Contains(portId))
				{
					return (CheckResult.OK);
				}
				else
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateSerialPort)
					{
						MKY.IO.Ports.SerialPortId portIdAlternate;
						if (TryGetSerialPortAlternate(ports, out portIdAlternate))
						{
							var dr = ShowSerialPortNotAvailableSwitchQuestionYesNo(portId, portIdAlternate);
							if (dr == DialogResult.Yes)
							{
								this.settingsRoot.Explicit.Terminal.IO.SerialPort.PortId = portIdAlternate;
								ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
							}

							return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
						}
						else
						{
							OnTimedStatusTextRequest(portId + " currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // ports.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateSerialPort)
				{
					var dr = ShowNoSerialPortsStartAnywayQuestionYesNo(portId);
					if (dr == DialogResult.Yes)
					{
						return (CheckResult.OK);
					}
					else
					{
						OnTimedStatusTextRequest("No serial COM ports available");
						return (CheckResult.Cancel);
					}
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		private CheckResult CheckLocalInterfaceAvailability(MKY.Net.IPNetworkInterfaceEx localInterface)
		{
			OnFixedStatusTextRequest("Checking availability of '" + localInterface + "'...");

			var localInterfaces = new MKY.Net.IPNetworkInterfaceCollection();
			localInterfaces.FillWithAvailableLocalInterfaces();

			// Attention:
			// Similar code exists in View.Controls.SocketSelection.SetLocalInterfaceList().
			// Changes here may have to be applied there too!

			if (localInterfaces.Count > 0)
			{
				if (localInterfaces.Contains(localInterface))
				{
					return (CheckResult.OK);
				}
				else if (localInterfaces.ContainsDescription(localInterface))
				{
					// A device with same description is available, use that:
					int sameDescriptionIndex = localInterfaces.FindIndexDescription(localInterface);

					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
					{
						var dr = ShowLocalInterfaceNotAvailableAlternateQuestionYesNo(localInterface, localInterfaces[sameDescriptionIndex]);
						if (dr == DialogResult.Yes)
						{
							this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
							ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
							return (CheckResult.OK);
						}
						else
						{
							OnResetStatusTextRequest();
							return (CheckResult.Cancel);
						}
					}
					else // Silently switch interface:
					{
						this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
						ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
						return (CheckResult.OK);
					}
				}
				else
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
					{
						var dr = ShowLocalInterfaceNotAvailableDefaultQuestionYesNo(localInterface, localInterfaces[0]);
						if (dr == DialogResult.Yes)
						{
							this.settingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[0];
							ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
							return (CheckResult.OK);
						}
						else
						{
							OnTimedStatusTextRequest("'" + localInterface + "' currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // localInterfaces.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateNetworkInterface)
				{
					ShowNoLocalInterfacesMessageOK();
					OnTimedStatusTextRequest("No local network interfaces available");
					return (CheckResult.Cancel);
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		private CheckResult CheckUsbDeviceAvailability(MKY.IO.Usb.HidDeviceInfo deviceInfo)
		{
			OnFixedStatusTextRequest("Checking availability of '" + deviceInfo + "'...");

			var devices = new MKY.IO.Usb.SerialHidDeviceCollection();
			devices.FillWithAvailableDevices(true); // Retrieve strings from devices in order to get serial strings.

			// Attention:
			// Similar code exists in View.Controls.UsbSerialHidDeviceSelection.SetDeviceList().
			// Changes here may have to be applied there too!

			if (devices.Count > 0)
			{
				if      (devices.ContainsVidPidSerialUsage(deviceInfo)) // Full match, taking 'AnyUsage' into account.
				{
					return (CheckResult.OK);
				}
				else if (devices.ContainsVidPidSerial(deviceInfo)) // Mismatch in usage, i.e. device available but usage not.
				{
					// A device with same VID/PID/SNR is available, use that:
					int sameVidPidSerialIndex = devices.FindIndexVidPidSerial(deviceInfo);

					// Inform the user in any case:
					{
						if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice) // Use this setting also for mismatch in usage.
						{
							var dr = ShowUsbSerialHidDeviceUsageNotAvailableAlternateQuestionYesNo(deviceInfo, deviceInfo.UsageString, devices[sameVidPidSerialIndex].UsageString);
							if (dr == DialogResult.Yes)
							{
								this.settingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidSerialIndex];
								ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
							}

							return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
						}
						else
						{
							OnResetStatusTextRequest();
							return (CheckResult.Ignore); // Silently ignore.
						}
					}
				}
				else if (devices.ContainsVidPid(deviceInfo)) // Mismatch in serial, i.e. device type available but not specific device.
				{
					// A device with same VID/PID is available, use that:
					int sameVidPidIndex = devices.FindIndexVidPid(deviceInfo);

					// Inform the user if serial is required:
					if (ApplicationSettings.LocalUserSettings.General.MatchUsbSerial)
					{
						if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
						{
							var dr = ShowUsbSerialHidDeviceSerialNotAvailableAlternateQuestionYesNo(deviceInfo, devices[sameVidPidIndex]);
							if (dr == DialogResult.Yes)
							{
								this.settingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
								ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
							}

							return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
						}
						else
						{
							OnResetStatusTextRequest();
							return (CheckResult.Ignore); // Silently ignore.
						}
					}
					else
					{
						// Clear the 'Changed' flag in case of automatically changing settings:
						bool hadAlreadyBeenChanged = this.settingsRoot.Terminal.IO.UsbSerialHidDevice.HaveChanged;
						this.settingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
						if (!hadAlreadyBeenChanged)
							this.settingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.ClearChanged();

						ApplyTerminalSettings(this.settingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.

						return (CheckResult.OK); // Device may not yet be available but 'AutoOpen'.
					}
				}
				else // no device of same VID/PID
				{
					if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
					{
						var dr = ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestionYesNo(deviceInfo);
						if (dr == DialogResult.Yes)
						{
							return (CheckResult.OK);
						}
						else
						{
							OnTimedStatusTextRequest("'" + deviceInfo + "' currently not available");
							return (CheckResult.Cancel);
						}
					}
					else
					{
						OnResetStatusTextRequest();
						return (CheckResult.Ignore); // Silently ignore.
					}
				}
			}
			else // devices.Count == 0
			{
				if (ApplicationSettings.LocalUserSettings.General.AskForAlternateUsbDevice)
				{
					var dr = ShowNoUsbSerialHidDevicesStartAnywayQuestionYesNo(deviceInfo);
					if (dr == DialogResult.Yes)
					{
						return (CheckResult.OK);
					}
					else
					{
						OnTimedStatusTextRequest("No HID capable USB devices available");
						return (CheckResult.Cancel);
					}
				}
				else
				{
					OnResetStatusTextRequest();
					return (CheckResult.Ignore); // Silently ignore.
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private bool TryGetSerialPortAlternate(MKY.IO.Ports.SerialPortCollection ports, out MKY.IO.Ports.SerialPortId portIdAlternate)
		{
			// If not allowed to detect 'InUse', no reliable alternate can be evaluted:
			if (!ApplicationSettings.LocalUserSettings.General.DetectSerialPortsInUse)
			{
				portIdAlternate = null;
				return (false);
			}

			// If allowed, try to retrieve captions:
			if (ApplicationSettings.LocalUserSettings.General.RetrieveSerialPortCaptions)
			{
				// Done once for all ports because:
				//  > Underlying operation is relatively fast.
				//  > Underlying operation needs to retrieve *all* captions anyway.

				try
				{
					ports.RetrieveCaptions();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex, "Failed to retrieve serial COM port captions!");
				}
			}

			// Select the first available port that is not 'InUse':
			foreach (var port in ports)
			{
				ports.DetectWhetherPortIsInUse(port);

				if (!port.IsInUse)
				{
					portIdAlternate = port;
					return (true);
				}
			}

			// No alternate that is not 'InUse':
			portIdAlternate = null;
			return (false);
		}

		private DialogResult ShowNoSerialPortsStartAnywayQuestionYesNo(string portIdNotAvailable)
		{
			string message =
				"There are currently no serial COM ports available." + Environment.NewLine + Environment.NewLine +
				"Start " + portIdNotAvailable + " anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"No serial COM ports available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowSerialPortNotAvailableSwitchQuestionYesNo(string portIdNotAvailable, string portIdAlternate)
		{
			string message =
				"The previous serial port " + portIdNotAvailable + " is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to " + portIdAlternate + " (first available port that is currently not in use) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch serial COM port?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private void ShowNoLocalInterfacesMessageOK()
		{
			string message =
				"There are currently no local network interfaces available." + Environment.NewLine + Environment.NewLine +
				"Terminal will not be started.";

			OnMessageInputRequest
			(
				message,
				"No interfaces available",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);
		}

		private DialogResult ShowLocalInterfaceNotAvailableDefaultQuestionYesNo(string localInterfaceNotAvailable, string localInterfaceDefaulted)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceDefaulted + "' (default) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowLocalInterfaceNotAvailableAlternateQuestionYesNo(string localInterfaceNotAvailable, string localInterfaceAlternate)
		{
			string message =
				"The previous local network interface '" + localInterfaceNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + localInterfaceAlternate + "' (first available interface with same description) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch interface?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowNoUsbSerialHidDevicesStartAnywayQuestionYesNo(string deviceInfoNotAvailable)
		{
			string message =
				"There are currently no HID capable USB devices available." + Environment.NewLine + Environment.NewLine +
				"Start " + deviceInfoNotAvailable + " anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"No USB HID devices available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceNotAvailableStartAnywayQuestionYesNo(string deviceInfoNotAvailable)
		{
			string message =
				"The previous USB HID device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Start anyway?";

			var dr = OnMessageInputRequest
			(
				message,
				"Previous USB HID device not available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceSerialNotAvailableAlternateQuestionYesNo(string deviceInfoNotAvailable, string deviceInfoAlternate)
		{
			string message =
				"The previous device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + deviceInfoAlternate + "' (first available device with same VID and PID) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch USB HID device?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceUsageNotAvailableAlternateQuestionYesNo(string deviceInfo, string usageNotAvailable, string usageAlternate)
		{
			string message =
				"The previous usage '" + usageNotAvailable + "' of '" + deviceInfo + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + usageAlternate + "' (first available usage on same device) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch USB HID usage?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			return (dr);
		}

		#endregion

		#region Terminal > Start/Stop I/O
		//------------------------------------------------------------------------------------------
		// Terminal > Start/Stop I/O
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual bool StartIO()
		{
			string errorMessage;
			return (StartIO(out errorMessage));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <param name="errorMessage">Message used for scripting.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StartIO(out string errorMessage)
		{
			return (StartIO(true, out errorMessage));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		private bool StartIO(bool saveStatus)
		{
			string errorMessage;
			return (StartIO(saveStatus, out errorMessage));
		}

		/// <summary>
		/// Starts the terminal's I/O instance.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <param name="errorMessage">Message used for scripting.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool StartIO(bool saveStatus, out string errorMessage)
		{
			bool success = false;

			OnFixedStatusTextRequest("Starting terminal...");
			try
			{
				if (this.terminal.Start())
				{
					if (saveStatus)
						this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

					OnTimedStatusTextRequest("Terminal successfully started.");
					errorMessage = null;
					success = true;
				}
				else
				{
					errorMessage = string.Format("Terminal on '{0}' could not be started!", this.terminal.ToShortIOString());
					OnFixedStatusTextRequest(errorMessage);

					if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
					{
						string yatLead, yatText;
						ErrorHelper.MakeStartHint(this.settingsRoot.IOType, out yatLead, out yatText);

						OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage(errorMessage, string.Empty, yatLead, yatText),
							"Terminal Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = "Error on starting terminal!";
				OnFixedStatusTextRequest(errorMessage);

				if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
				{
					string yatLead, yatText;
					ErrorHelper.MakeExceptionHint(this.settingsRoot.IOType, out yatLead, out yatText);

					errorMessage = ErrorHelper.ComposeMessage(errorMessage, ex, yatLead, yatText);
					OnMessageInputRequest
					(
						errorMessage,
						"Terminal Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}

			return (success);
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual bool StopIO()
		{
			string errorMessage;
			return (StopIO(out errorMessage));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <param name="errorMessage">Message used for scripting.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public virtual bool StopIO(out string errorMessage)
		{
			return (StopIO(true, out errorMessage));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		private bool StopIO(bool saveStatus)
		{
			string errorMessage;
			return (StopIO(saveStatus, out errorMessage));
		}

		/// <summary>
		/// Stops the terminal's I/O instance.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <param name="errorMessage">Message used for scripting.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool StopIO(bool saveStatus, out string errorMessage)
		{
			bool success = false;

			OnFixedStatusTextRequest("Stopping terminal...");
			try
			{
				this.terminal.Stop();

				if (saveStatus)
					this.settingsRoot.TerminalIsStarted = this.terminal.IsStarted;

				OnTimedStatusTextRequest("Terminal stopped.");
				errorMessage = null;
				success = true;
			}
			catch (Exception ex)
			{
				errorMessage = "Error on stopping terminal!";
				OnTimedStatusTextRequest(errorMessage);
				errorMessage = "Error on stopping terminal:" + Environment.NewLine + Environment.NewLine + ex.Message;
				OnMessageInputRequest
				(
					errorMessage,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Terminal not stopped!");
			}

			return (success);
		}

		#endregion

		#region Terminal > Send Command
		//------------------------------------------------------------------------------------------
		// Terminal > Send Command
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends given command.
		/// </summary>
		public virtual void SendCommand(Command command)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			if (command.IsText)
				SendText(command);
			else if (command.IsFilePath)
				SendFile(command);
			else
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + command + "' does not specify a known command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "command"));
		}

		#endregion

	#if (WITH_SCRIPTING)

		#region Terminal > Send Raw
		//------------------------------------------------------------------------------------------
		// Terminal > Send Raw
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends the specified raw data.
		/// </summary>
		public virtual void SendRaw(byte[] data)
		{
			OnFixedStatusTextRequest("Sending " + data.Length + " bytes...");
			try
			{
				this.terminal.Send(data);
			}
			catch (IOException ex)
			{
				OnFixedStatusTextRequest("Error sending " + data.Length + " bytes!");

				string text;
				string title;
				ComposeSendRawErrorMessage(out text, out title);
				OnMessageInputRequest
				(
					text + Environment.NewLine + Environment.NewLine +
					"System error message:" + Environment.NewLine +
					ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				OnTimedStatusTextRequest("Data not sent!");
			}
		}

		private void ComposeSendRawErrorMessage(out string text, out string title)
		{
			StringBuilder textBuilder = new StringBuilder();
			StringBuilder titleBuilder = new StringBuilder();

			textBuilder.Append("Unable to write to ");
			switch (this.settingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
					textBuilder.Append("port");
					titleBuilder.Append("Serial Port");
					break;

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				case Domain.IOType.UdpClient:
				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
					textBuilder.Append("socket");
					titleBuilder.Append("Socket");
					break;

				case Domain.IOType.UsbSerialHid:
					textBuilder.Append("device");
					titleBuilder.Append("Device");
					break;

				default:
					throw (new NotImplementedException("I/O type " + this.settingsRoot.IOType + "misses implementation"));
			}
			textBuilder.Append(":");
			titleBuilder.Append(" Error!");

			text = textBuilder.ToString();
			title = titleBuilder.ToString();
		}

		#endregion

	#endif // WITH_SCRIPTING

		#region Terminal > Send Text
		//------------------------------------------------------------------------------------------
		// Terminal > Send Text
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendText()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(this.settingsRoot.SendText.Command);

			// Clear command if desired:
			if (!this.settingsRoot.Send.Text.KeepSendText)
				this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
		}

		/// <summary>
		/// Sends text command given by terminal settings.
		/// </summary>
		public virtual void SendTextWithoutEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendTextWithoutEol(this.settingsRoot.SendText.Command);

			// Clear command if desired:
			if (!this.settingsRoot.Send.Text.KeepSendText)
				this.settingsRoot.SendText.Command = new Command(this.settingsRoot.SendText.Command.DefaultRadix); // Set command to "".
		}

		/// <summary>
		/// Sends partial text EOL.
		/// </summary>
		public virtual void SendPartialTextEol()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(new Command(true, this.settingsRoot.SendText.Command.DefaultRadix));
		}

		/// <summary>
		/// Sends given text.
		/// </summary>
		/// <param name="text">Text to be sent.</param>
		public virtual void SendText(string text)
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendText(new Command(text));
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendText(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendText(command);
			else
				DoSendText(command.ToCommandWithoutDefaultRadix());
		}

		/// <summary>
		/// Sends given text command.
		/// </summary>
		/// <param name="command">Text command to be sent.</param>
		public virtual void SendTextWithoutEol(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendTextWithoutEol(command);
			else
				DoSendTextWithoutEol(command.ToCommandWithoutDefaultRadix());
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendText(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendTextLine(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsMultiLineText)
				{
					this.terminal.SendTextLines(c.MultiLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else if (c.IsPartialTextEol)
				{
					this.terminal.SendTextLine("", Domain.Radix.String);
				}
				else // This indicates an invalid operation, since a command must have been validated before calling this method!
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else // This indicates an invalid operation, since a command must have been validated before calling this method!
			{
			////throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));

				// Temporary workaround until bug #469 has been resolved:
				OnMessageInputRequest
				(
					"Command '" + c + "' does not specify a valid text command!",
					"Invalid Text Command",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
			}
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void DoSendTextWithoutEol(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				if (c.IsSingleLineText)
				{
					if (SendTextSettings.IsEasterEggCommand(c.SingleLineText))
						this.terminal.EnqueueEasterEggMessage();
					else
						this.terminal.SendText(c.SingleLineText, c.DefaultRadix);
				}
				else if (c.IsPartialText)
				{
					this.terminal.SendText(c.PartialText, c.DefaultRadix);

					// Compile the partial command line for later use:
					if (string.IsNullOrEmpty(this.partialCommandLine))
						this.partialCommandLine = string.Copy(c.PartialText);
					else
						this.partialCommandLine += c.PartialText;
				}
				else // Covers 'c.IsMultiLineText' and 'c.IsPartialTextEol' which are invalid 'WithoutEol'.
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
				}

				CloneIntoRecentTextCommandsIfNeeded(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid text command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Includes compiled partial text.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentTextCommandsIfNeeded(Command c)
		{
			if (c.IsSingleLineText || c.IsMultiLineText /* || do not add c.IsPartialText to recents */ || c.IsPartialTextEol)
			{
				// Clone the command for the recent commands collection:
				Command clone;
				if (c.IsSingleLineText || c.IsMultiLineText)
				{
					clone = new Command(c); // 'Normal' case, simply clone the command.
				}
				else if (c.IsPartialTextEol)
				{                                        // Create a single line text command,
					if (this.partialCommandLine != null) // using the previously sent characters:
						clone = new Command(this.partialCommandLine, false, c.DefaultRadix);
					else
						clone = new Command(c); // Keep partial EOL.
				}
				else
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Condition is invalid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				// Reset the partial command line, in any case:
				this.partialCommandLine = null;

				// Clear potential description, as that shall not be considered for recents,
				// e.g. same command with different description shall only be listed once:
				clone.ClearDescription();

				// Put clone into recent history:
				this.settingsRoot.SendText.RecentCommands.Add(new RecentItem<Command>(clone));
				this.settingsRoot.SendText.SetChanged(); // Manual change required because underlying collection is modified.
			}
		}

		#endregion

		#region Terminal > Send File
		//------------------------------------------------------------------------------------------
		// Terminal > Send File
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Sends file given by terminal settings.
		/// </summary>
		public virtual void SendFile()
		{
			// AssertNotDisposed() is called by 'Send...' below.

			SendFile(this.settingsRoot.SendFile.Command);
		}

		/// <summary>
		/// Sends given file.
		/// </summary>
		/// <param name="command">File to be sent.</param>
		public virtual void SendFile(Command command)
		{
			// AssertNotDisposed() is called by 'DoSend...' below.

			if (this.settingsRoot.Terminal.Send.UseExplicitDefaultRadix)
				DoSendFile(command);
			else
				DoSendFile(command.ToCommandWithoutDefaultRadix());
		}

		/// <remarks>
		/// Separate "Do...()" method for obvious handling of 'UseExplicitDefaultRadix'.
		/// </remarks>
		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'symmetricity' is a correct English term.")]
		protected virtual void DoSendFile(Command c)
		{
			AssertNotDisposed();

			if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				string absoluteFilePath = EnvironmentEx.ResolveAbsolutePath(c.FilePath, Path.GetDirectoryName(SettingsFilePath));

				this.terminal.SendFile(absoluteFilePath, c.DefaultRadix);

				CloneIntoRecentFileCommands(c);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a valid file command!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "c"));
			}
		}

		/// <remarks>
		/// Argument of this protected method named "c" for compactness.
		/// </remarks>
		protected virtual void CloneIntoRecentFileCommands(Command c)
		{
			// Clone the command for the recent commands collection:
			var clone = new Command(c);

			// Clear potential description, as that shall not be considered for recents,
			// e.g. same command with different description shall only be listed once:
			clone.ClearDescription();

			// Put clone into recent history:
			this.settingsRoot.SendFile.RecentCommands.Add(new RecentItem<Command>(clone));
			this.settingsRoot.SendFile.SetChanged(); // Manual change required because underlying collection is modified.
		}

		#endregion

		#region Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------
		// Terminal > Send and/or Copy Predefined
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Send requested predefined command.
		/// </summary>
		/// <param name="pageId">Page 1..max.</param>
		/// <param name="commandId">Command 1..max.</param>
		public virtual bool SendPredefined(int pageId, int commandId)
		{
			AssertNotDisposed();

			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(pageId - 1, commandId - 1))
				return (false);

			// Process command:
			var c = this.settingsRoot.PredefinedCommand.Pages[pageId - 1].Commands[commandId - 1];
			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				SendText(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendText.Command = new Command(c); // Clone to ensure decoupling.

				return (true);
			}
			else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				SendFile(c);

				if (this.settingsRoot.Send.CopyPredefined)
					this.settingsRoot.SendFile.Command = new Command(c); // Clone to ensure decoupling.

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Copy the requested predefined command, not taking copy predefined settings
		/// <see cref="Domain.Settings.SendSettings.CopyPredefined"/> into account.
		/// </summary>
		/// <param name="pageId">Page 1..max.</param>
		/// <param name="commandId">Command 1..max.</param>
		public virtual bool CopyPredefined(int pageId, int commandId)
		{
			AssertNotDisposed();

			// Verify arguments:
			if (!this.settingsRoot.PredefinedCommand.ValidateWhetherCommandIsDefined(pageId - 1, commandId - 1))
				return (false);

			// Process command:
			var c = this.settingsRoot.PredefinedCommand.Pages[pageId - 1].Commands[commandId - 1];
			if (c.IsValidText(this.settingsRoot.Terminal.Send.Text.ToParseMode()))
			{
				this.settingsRoot.SendText.Command = new Command(c); // Clone to ensure decoupling.
				return (true);
			}
			else if (c.IsValidFilePath(Path.GetDirectoryName(SettingsFilePath)))
			{
				this.settingsRoot.SendFile.Command = new Command(c); // Clone to ensure decoupling.
				return (true);
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region Terminal > Break/Resume
		//------------------------------------------------------------------------------------------
		// Terminal > Break/Resume
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Breaks all currently ongoing operations in the terminal.
		/// </summary>
		public virtual void Break()
		{
			AssertNotDisposed();

			this.terminal.Break();
		}

		/// <summary>
		/// Resumes all currently suspended operations in the terminal.
		/// </summary>
		public virtual void ResumeBreak()
		{
			AssertNotDisposed();

			this.terminal.ResumeBreak();
		}

		#endregion

		#region Terminal > Repositories
		//------------------------------------------------------------------------------------------
		// Terminal > Repositories
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (!this.terminal.ClearRepository(repositoryType))
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertNotDisposed();

			if (!this.terminal.ClearRepositories())
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of repositories.
		/// </summary>
		public virtual void RefreshRepositories()
		{
			AssertNotDisposed();

			if (!this.terminal.RefreshRepositories())
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of given repository.
		/// </summary>
		public virtual void RefreshRepository(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			if (!this.terminal.RefreshRepository(repositoryType))
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayElementCollection RepositoryToDisplayElements(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.RepositoryToDisplayElements(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayLineCollection RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.RepositoryToDisplayLines(repositoryType));
		}

		/// <summary>
		/// Returns the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual Domain.DisplayLine LastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.LastDisplayLineAuxiliary(repositoryType));
		}

		/// <summary>
		/// Clears the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual void ClearLastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			this.terminal.ClearLastDisplayLineAuxiliary(repositoryType);
		}

		/// <summary>
		/// Returns contents of desired repository as string.
		/// </summary>
		public virtual string RepositoryToExtendedDiagnosticsString(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.RepositoryToExtendedDiagnosticsString(repositoryType));
		}

		#endregion

		#region Terminal > Format
		//------------------------------------------------------------------------------------------
		// Terminal > Format
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Formats the given data into a string, same as done by the monitor view.
		/// </summary>
		public virtual string Format(byte[] data, Domain.IODirection direction)
		{
			AssertNotDisposed();
			return (this.terminal.Format(data, direction));
		}

		/// <summary>
		/// Formats the given data into a string, same as done by the monitor view.
		/// </summary>
		public virtual string Format(byte[] data, Domain.IODirection direction, Domain.Radix radix)
		{
			AssertNotDisposed();
			return (this.terminal.Format(data, direction, radix));
		}

		#endregion

		#region Terminal > Time Status
		//------------------------------------------------------------------------------------------
		// Terminal > Time Status
		//------------------------------------------------------------------------------------------

		private void CreateChronos()
		{
			this.activeConnectChrono = new Chronometer();
			this.activeConnectChrono.Interval = 1000;
		////this.activeConnectChrono.TimeSpanChanged shall not be used, events are invoked by 'totalConnectChrono' below.

			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += totalConnectChrono_TimeSpanChanged;
		}

		private void DisposeChronos()
		{
			if (this.activeConnectChrono != null)
			{
				// No elapsed event, events are invoked by total connect chrono.
				this.activeConnectChrono.Dispose();
				this.activeConnectChrono = null;
			}

			if (this.totalConnectChrono != null)
			{
				this.totalConnectChrono.TimeSpanChanged -= totalConnectChrono_TimeSpanChanged;
				this.totalConnectChrono.Dispose();
				this.totalConnectChrono = null;
			}
		}

		/// <summary></summary>
		public virtual TimeSpan ActiveConnectTime
		{
			get
			{
				AssertNotDisposed();

				return (this.activeConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual TimeSpan TotalConnectTime
		{
			get
			{
				AssertNotDisposed();

				return (this.totalConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetConnectTime(out TimeSpan activeConnectTime, out TimeSpan totalConnectTime)
		{
			AssertNotDisposed();

			activeConnectTime = this.activeConnectChrono.TimeSpan;
			totalConnectTime  = this.totalConnectChrono.TimeSpan;
		}

		/// <summary></summary>
		public virtual void ResetConnectTime()
		{
			AssertNotDisposed();

			var now = DateTime.Now; // Ensure that all use exactly the same instant.

			this.activeConnectChrono.Reset(now);
			this.totalConnectChrono .Reset(now);

			this.terminal.InitialTimeStamp = now;

			if (this.settingsRoot.Display.ShowTimeSpan)
				this.terminal.RefreshRepositories();
		}

		private void totalConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			OnIOConnectTimeChanged(e);
		}

		#endregion

		#region Terminal > Data Status
		//------------------------------------------------------------------------------------------
		// Terminal > Data Status
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// The value corresponds to the byte count of the raw terminal repository. The count of the
		/// formatted terminal repository slightly lags behind. <see cref="GetRepositoryByteCount"/>
		/// may be used to retrieve the formatted terminal byte count.
		/// </remarks>
		public virtual int TxByteCount
		{
			get
			{
				AssertNotDisposed();

				return (this.txByteCount);
			}
		}

		/// <remarks>
		/// The value corresponds to the byte count of the raw terminal repository. The count of the
		/// formatted terminal repository slightly lags behind. <see cref="GetRepositoryByteCount"/>
		/// may be used to retrieve the formatted terminal byte count.
		/// </remarks>
		public virtual int RxByteCount
		{
			get
			{
				AssertNotDisposed();

				return (this.rxByteCount);
			}
		}

		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// <see cref="GetRepositoryLineCount"/> may be used to include incomplete line count.
		/// </remarks>
		public virtual int TxLineCount
		{
			get
			{
				AssertNotDisposed();

				return (this.txLineCount);
			}
		}

	/////// <remarks>
	/////// The value corresponds to the completed line count of the formatted terminal repository.
	/////// <see cref="GetRepositoryLineCount"/> may be used to include incomplete line count.
	/////// </remarks>
	////public virtual int BidirLineCount
	////{
	////	get
	////	{
	////		AssertNotDisposed();
	////
	////		return (this.bidirLineCount) would technically be possible, but doesn't make much sense.
	////	}
	////}

		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// <see cref="GetRepositoryLineCount"/> may be used to include incomplete line count.
		/// </remarks>
		public virtual int RxLineCount
		{
			get
			{
				AssertNotDisposed();

				return (this.rxLineCount);
			}
		}

		/// <remarks>
		/// The value corresponds to the rate of the raw terminal repository.
		/// The rate of the formatted terminal repository slightly lags behind.
		/// </remarks>
		public virtual int TxByteRate
		{
			get
			{
				AssertNotDisposed();

				return (this.txByteRate.RateValue);
			}
		}

		/// <remarks>
		/// The value corresponds to the rate of the raw terminal repository.
		/// The rate of the formatted terminal repository slightly lags behind.
		/// </remarks>
		public virtual int RxByteRate
		{
			get
			{
				AssertNotDisposed();

				return (this.rxByteRate.RateValue);
			}
		}

		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// </remarks>
		public virtual int TxLineRate
		{
			get
			{
				AssertNotDisposed();

				return (this.txLineRate.RateValue);
			}
		}

	/////// <remarks>
	/////// The value corresponds to the completed line count of the formatted terminal repository.
	/////// </remarks>
	////public virtual int BidirLineRate
	////{
	////	get
	////	{
	////		AssertNotDisposed();
	////
	////		return (this.bidirLineRate.RateValue) would technically be possible, but doesn't make much sense.
	////	}
	////}

		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// </remarks>
		public virtual int RxLineRate
		{
			get
			{
				AssertNotDisposed();

				return (this.rxLineRate.RateValue);
			}
		}

		/// <remarks>
		/// See remarks of <see cref="TxByteCount"/> and <see cref="TxLineCount"/>, <see cref="RxByteCount"/> and <see cref="RxLineCount"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetDataCount(out int txByteCount, out int txLineCount, out int rxByteCount, out int rxLineCount)
		{
			AssertNotDisposed();

			txByteCount = this.txByteCount;
			txLineCount = this.txLineCount;

			rxByteCount = this.rxByteCount;
			rxLineCount = this.rxLineCount;
		}

		/// <remarks>
		/// See remarks of <see cref="TxByteRate"/> and <see cref="TxLineRate"/>, <see cref="RxByteRate"/> and <see cref="RxLineRate"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetDataRate(out int txByteRate, out int txLineRate, out int rxByteRate, out int rxLineRate)
		{
			AssertNotDisposed();

			txByteRate = this.txByteRate.RateValue;
			txLineRate = this.txLineRate.RateValue;

			rxByteRate = this.rxByteRate.RateValue;
			rxLineRate = this.rxLineRate.RateValue;
		}

		/// <summary></summary>
		public virtual void ResetIOCountAndRate()
		{
			AssertNotDisposed();

			this.txByteCount    = 0;
			this.rxByteCount    = 0;

			this.txLineCount    = 0;
		////this.bidirLineCount = 0 would technically be possible, but doesn't make much sense.
			this.rxLineCount    = 0;

			OnIOCountChanged_Promptly(EventArgs.Empty);

			this.txByteRate   .Reset();
			this.rxByteRate   .Reset();

			this.txLineRate   .Reset();
		////this.bidirLineRate.Reset() would technically be possible, but doesn't make much sense.
			this.rxLineRate   .Reset();

			OnIORateChanged_Promptly(EventArgs.Empty);
			OnIORateChanged_Decimated(EventArgs.Empty);
		}

		private void CreateRates()
		{
			int rateInterval   = 1000;
			int rateWindow     = 5000;
			int updateInterval =  250;

			this.txByteRate    = new RateProvider(rateInterval, rateWindow, updateInterval);
			this.rxByteRate    = new RateProvider(rateInterval, rateWindow, updateInterval);

			this.txLineRate    = new RateProvider(rateInterval, rateWindow, updateInterval);
		////this.bidirLineRate = new RateProvider(rateInterval, rateWindow, updateInterval) would technically be possible, but doesn't make much sense.
			this.rxLineRate    = new RateProvider(rateInterval, rateWindow, updateInterval);

			this.txByteRate   .Changed += rate_Changed;
			this.rxByteRate   .Changed += rate_Changed;

			this.txLineRate   .Changed += rate_Changed;
		////this.bidirLineRate.Changed += rate_Changed would technically be possible, but doesn't make much sense.
			this.rxLineRate   .Changed += rate_Changed;
		}

		private void DisposeRates()
		{
			if (this.txByteRate != null)
			{
				this.txByteRate.Changed -= rate_Changed;
				this.txByteRate.Dispose();
				this.txByteRate = null;
			}

			if (this.rxByteRate != null)
			{
				this.rxByteRate.Changed -= rate_Changed;
				this.rxByteRate.Dispose();
				this.rxByteRate = null;
			}

			if (this.txLineRate != null)
			{
				this.txLineRate.Changed -= rate_Changed;
				this.txLineRate.Dispose();
				this.txLineRate = null;
			}

		////if (this.bidirLineRate != null) would technically be possible, but doesn't make much sense.
		////{
		////	this.bidirLineRate.Changed -= rate_Changed;
		////	this.bidirLineRate.Dispose();
		////	this.bidirLineRate = null;
		////}

			if (this.rxLineRate != null)
			{
				this.rxLineRate.Changed -= rate_Changed;
				this.rxLineRate.Dispose();
				this.rxLineRate = null;
			}
		}

		private void rate_Changed(object sender, RateEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnIORateChanged_Promptly(e);
			OnIORateChanged_Decimated(e);
		}

		/// <remarks>
		/// The value corresponds to the byte count of the formatted terminal repository.
		/// </remarks>
		public virtual int GetRepositoryByteCount(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.GetRepositoryByteCount(repositoryType));
		}

		/// <remarks>
		/// The value corresponds to the line count of the formatted terminal repository.
		/// </remarks>
		public virtual int GetRepositoryLineCount(Domain.RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.terminal.GetRepositoryLineCount(repositoryType));
		}

		#endregion

		#region Terminal > Flow Control and Break Status
		//------------------------------------------------------------------------------------------
		// Terminal > Flow Control and Break Status
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPins SerialPortControlPins
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SerialPortControlPins);
			}
		}

		/// <summary>
		/// Serial port control pin counts.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPinCount SerialPortControlPinCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SerialPortControlPinCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SentXOnCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.SentXOffCount);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertNotDisposed();

			#if !(WITH_SCRIPTING)
				return (this.terminal.ReceivedXOnCount);
			#else
				lock (this.receivedXOnXOffForScriptingSyncObj)
					return (this.terminal.ReceivedXOnCount - this.receivedXOnOffsetForScripting);
			#endif
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOffCount
		{
			get
			{
				AssertNotDisposed();

			#if !(WITH_SCRIPTING)
				return (this.terminal.ReceivedXOffCount);
			#else
				lock (this.receivedXOnXOffForScriptingSyncObj)
					return (this.terminal.ReceivedXOffCount - this.receivedXOffOffsetForScripting);
			#endif
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		public virtual void ResetReceivedXOnCountForScripting()
		{
			lock (this.receivedXOnXOffForScriptingSyncObj)
			{
				var totalCount = (this.receivedXOnOffsetForScripting + ReceivedXOnCount);
				this.receivedXOnOffsetForScripting = totalCount;
			}
		}

		/// <summary></summary>
		public virtual void ResetReceivedXOffCountForScripting()
		{
			lock (this.receivedXOnXOffForScriptingSyncObj)
			{
				var totalCount = (this.receivedXOnOffsetForScripting + ReceivedXOffCount);
				this.receivedXOffOffsetForScripting = totalCount;
			}
		}

		/// <summary></summary>
		public virtual void ResetReceivedXOnXOffCountForScripting()
		{
			lock (this.receivedXOnXOffForScriptingSyncObj) // Atomic for both counts.
			{
				ResetReceivedXOnCountForScripting();
				ResetReceivedXOffCountForScripting();
			}
		}

	#endif

		/// <summary></summary>
		public virtual void ResetFlowControlCount()
		{
			AssertNotDisposed();

		#if !(WITH_SCRIPTING)
			this.terminal.ResetFlowControlCount();
		#else
			lock (this.receivedXOnXOffForScriptingSyncObj)
			{
				this.terminal.ResetFlowControlCount();

				this.receivedXOnOffsetForScripting = 0;
				this.receivedXOffOffsetForScripting = 0;
			}
		#endif
		}

		/// <summary></summary>
		public virtual int InputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.InputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertNotDisposed();

				return (this.terminal.OutputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual void ResetBreakCount()
		{
			AssertNotDisposed();

			this.terminal.ResetBreakCount();
		}

		#endregion

		#region Terminal > I/O Control
		//------------------------------------------------------------------------------------------
		// Terminal > I/O Control
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Toggles RTS control pin if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		public virtual bool RequestToggleRts()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleRts(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.RtsPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be raised after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be raised asynchronously, accessing the settings here
			//     synchronously is the better option.
			//  3. Here, it is obvious that the user indeed wants to change a pin setting,
			//     and therefore expects the setting to be modified.
		}

		/// <summary>
		/// Toggles DTR control pin if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		public virtual bool RequestToggleDtr()
		{
			AssertNotDisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleDtr(out pinState);
			this.SettingsRoot.IO.SerialPort.Communication.DtrPin = pinState;
			return (isSuccess);

			// Note, this user requested change of the current settings is handled here,
			// and not in the 'terminal_IOControlChanged' event handler, for two reasons:
			//  1. The event will be raised after any change of the control pins, separating
			//     user indended and other events would be cumbersome.
			//  2. The event will be raised asynchronously, accessing the settings here
			//     synchronously is the better option.
			//  3. Here, it is obvious that the user indeed wants to change a pin setting,
			//     and therefore expects the setting to be modified.
		}

		/// <summary>
		/// Toggles the input XOn/XOff state if current flow control settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleInputXOnXOff()
		{
			AssertNotDisposed();

			return (this.terminal.RequestToggleInputXOnXOff());
		}

		/// <summary>
		/// Toggles the output break state if current port settings allow this.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the request has been executed; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool RequestToggleOutputBreak()
		{
			AssertNotDisposed();

			return (this.terminal.RequestToggleOutputBreak());
		}

		#endregion

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		private void DisposeLog()
		{
			if (this.log != null)
			{
				this.log.Dispose();
				this.log = null;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool SwitchLogOn()
		{
			try
			{
				this.log.SwitchOn();
				this.settingsRoot.LogIsOn = true;

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				ErrorHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to switch log on!", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool ClearLog()
		{
			try
			{
				this.log.Clear();

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				ErrorHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to clear log!", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);

				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool SwitchLogOff()
		{
			try
			{
				this.log.SwitchOff();
				this.settingsRoot.LogIsOn = false;

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				ErrorHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					ErrorHelper.ComposeMessage("Unable to switch log off!", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool OpenLogFile()
		{
			int fileCount = 0;

			if (this.log != null)
				fileCount = this.log.FilePaths.Count;

			if (fileCount > 0)
			{
				bool success = true;

				foreach (string filePath in this.log.FilePaths)
				{
					if (this.log.AnyIsOn && ExtensionHelper.IsFileTypeThatCanOnlyBeOpenedWhenCompleted(filePath))
					{
						string message =
							@"Log is still on and """ + Path.GetFileName(filePath) + @""" is incomplete." +
							Environment.NewLine + Environment.NewLine +
							"Shall the log be switched off before opening the file? [recommended]";

						var dr = OnMessageInputRequest
						(
							message,
							"Log File Warning",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Warning
						);

						if (dr == DialogResult.Yes)
						{
							SwitchLogOff();
						}

						// dr == DialogResult.No => Open the incomplete file anyway.

						if (dr == DialogResult.Cancel)
						{
							success = false;
							break; // Cancel all files.
						}
					}

					Exception ex;
					if (!Editor.TryOpenFile(filePath, out ex))
					{
						var dr = OnMessageInputRequest
						(
							ErrorHelper.ComposeMessage("Unable to open log file", filePath, ex),
							"Log File Error",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Error
						);

						if (dr == DialogResult.Cancel)
						{
							success = false;
							break; // Cancel all files.
						}
					}
				}

				return (success);
			}
			else
			{
				OnMessageInputRequest
				(
					"No log file(s) available (yet).",
					"Log File Information",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				return (true);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual bool OpenLogDirectory()
		{
			if (this.log != null)
			{
				// Attention:
				// Similar code exists in View.Forms.Main.OpenDefaultLogDirectory().
				// Changes here may have to be applied there too.

				string rootPath = this.log.Settings.RootPath;

				// Create directory if not existing yet:
				if (!Directory.Exists(Path.GetDirectoryName(rootPath)))
				{
					try
					{
						Directory.CreateDirectory(Path.GetDirectoryName(rootPath));
					}
					catch (Exception exCreate)
					{
						string message = "Unable to create folder." + Environment.NewLine + Environment.NewLine +
						                 "System error message:" + Environment.NewLine + exCreate.Message;

						OnMessageInputRequest
						(
							message,
							"Folder Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
						);

						return (false);
					}
				}

				// Open directory:
				Exception exBrowse;
				if (!DirectoryEx.TryBrowse(rootPath, out exBrowse))
				{
					OnMessageInputRequest
					(
						ErrorHelper.ComposeMessage("Unable to open log folder", rootPath, exBrowse),
						"Log Folder Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);

					return (false);
				}
				else
				{
					return (true);
				}
			}
			else
			{
				OnMessageInputRequest
				(
					"No log folder available (yet).",
					"Log File Information",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				);

				return (true);
			}
		}

		#endregion

		#region Auto
		//==========================================================================================
		// Auto
		//==========================================================================================

		// Note that Auto[Action|Response] is intentionally implemented in 'Model'
		// instead of 'Domain' since Triggers/Patterns/Actions/Responses relate to 'Model' items
		// SendText/SendFile/PredefinedCommand/....
		//
		// If needed one day, trigger evaluation could be moved to 'Domain', same as EOL evaluation.
		// Moving this to 'Domain' would e.g. allow for coloring. However, this would require two or
		// even four more colors (Tx/Rx data/control highlight). This becomes too complicated...

		#region Auto > Action
		//------------------------------------------------------------------------------------------
		// Auto > Action
		//------------------------------------------------------------------------------------------

		private void CreateAutoActionHelper()
		{
			UpdateAutoAction(); // Simply forward to general Update() method.
		}

		private void DisposeAutoActionHelper()
		{
			lock (this.autoActionTriggerHelperSyncObj)
				this.autoActionTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Updates the automatic action helper.
		/// </summary>
		protected virtual void UpdateAutoAction()
		{
			if (this.settingsRoot.AutoAction.IsActive)
			{
				if (this.settingsRoot.AutoAction.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					byte[] triggerSequence;
					if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoActionTrigger, out triggerSequence))
					{
						lock (this.autoActionTriggerHelperSyncObj)
						{
							if (this.autoActionTriggerHelper == null)
								this.autoActionTriggerHelper = new AutoTriggerHelper(triggerSequence);
							else
								this.autoActionTriggerHelper.TriggerSequence = triggerSequence;
						}
					}
					else
					{
						DeactivateAutoAction();
						DisposeAutoActionHelper();

						OnMessageInputRequest
						(
							"Failed to parse the automatic action trigger! Automatic action has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic action.",
							"Automatic Action Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					DisposeAutoActionHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoActionHelper();
			}
		}

		/// <summary>
		/// Invokes the automatic action.
		/// </summary>
		protected virtual void InvokeAutoAction(AutoAction action, byte[] triggerSequence, DateTime ts)
		{
			this.autoActionCount++; // Incrementing before invoking to have the effective count available during invoking.
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));

			switch (action)
			{
				case AutoAction.Highlight:                       /* no additional action */                                    break;
				case AutoAction.Filter:                          /* no additional action */                                    break;
				case AutoAction.Suppress:                        /* no additional action */                                    break;
				case AutoAction.Beep:                            SystemSounds.Beep.Play();                                     break;
				case AutoAction.ShowMessageBox:                  RequestAutoActionMessage(triggerSequence, ts);                break;
				case AutoAction.ClearRepositories:               ClearRepositories();                                          break;
				case AutoAction.ClearRepositoriesOnSubsequentRx: this.autoActionClearRepositoriesOnSubsequentRxIsArmed = true; break;
				case AutoAction.ResetCountAndRate:               ResetIOCountAndRate();                                        break;
				case AutoAction.SwitchLogOn:                     SwitchLogOn();                                                break;
				case AutoAction.SwitchLogOff:                    SwitchLogOff();                                               break;
				case AutoAction.StopIO:                          StopIO();                                                     break;
				case AutoAction.CloseTerminal:                   Close();                                                      break;
				case AutoAction.ExitApplication:                 OnExitRequest(EventArgs.Empty);                               break;

				case AutoAction.None:
				default:
					break;
			}
		}

		/// <summary>
		/// Notifies the user about the action.
		/// </summary>
		protected virtual void RequestAutoActionMessage(byte[] triggerSequence, DateTime ts)
		{
			var sb = new StringBuilder();
			sb.Append(@"Message has been triggered by """);
			sb.Append(this.terminal.Format(triggerSequence, Domain.IODirection.Rx));
			sb.Append(@""" the ");
			sb.Append(this.autoActionCount);
			sb.Append(Int32Ex.ToEnglishSuffix(this.autoActionCount));
			sb.Append(" time at ");
			sb.Append(this.terminal.Format(ts));
			sb.Append(".");

			OnMessageInputRequest
			(
				sb.ToString(),
				IndicatedName + " Automatic Action",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		/// <summary>
		/// Gets the automatic action count.
		/// </summary>
		public virtual int AutoActionCount
		{
			get
			{
				AssertNotDisposed();

				return (this.autoActionCount);
			}
		}

		/// <summary>
		/// Resets the automatic action count.
		/// </summary>
		public virtual void ResetAutoActionCount()
		{
			AssertNotDisposed();

			this.autoActionCount = 0;
			OnAutoActionCountChanged(new EventArgs<int>(this.autoActionCount));
		}

		/// <summary>
		/// Deactivates the automatic action.
		/// </summary>
		public virtual void DeactivateAutoAction()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoAction.Deactivate();
			ResetAutoActionCount();
		}

		#endregion

		#region Auto > Response
		//------------------------------------------------------------------------------------------
		// Auto > Response
		//------------------------------------------------------------------------------------------

		private void CreateAutoResponseHelper()
		{
			UpdateAutoResponse(); // Simply forward to general Update() method.
		}

		private void DisposeAutoResponseHelper()
		{
			lock (this.autoResponseTriggerHelperSyncObj)
				this.autoResponseTriggerHelper = null; // Simply delete the reference to the object.
		}

		/// <summary>
		/// Updates the automatic response helper.
		/// </summary>
		protected virtual void UpdateAutoResponse()
		{
			if (this.settingsRoot.AutoResponse.IsActive)
			{
				if (this.settingsRoot.AutoResponse.Trigger.CommandIsRequired) // = sequence required = helper required.
				{
					byte[] triggerSequence;
					if (TryParseCommandToSequence(this.settingsRoot.ActiveAutoResponseTrigger, out triggerSequence))
					{
						lock (this.autoResponseTriggerHelperSyncObj)
						{
							if (this.autoResponseTriggerHelper == null)
								this.autoResponseTriggerHelper = new AutoTriggerHelper(triggerSequence);
							else
								this.autoResponseTriggerHelper.TriggerSequence = triggerSequence;
						}
					}
					else
					{
						DeactivateAutoResponse();
						DisposeAutoResponseHelper();

						OnMessageInputRequest
						(
							"Failed to parse the automatic response trigger! Automatic response has been disabled!" + Environment.NewLine + Environment.NewLine +
							"To enable again, re-configure the automatic response.",
							"Automatic Response Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
				else // No command required = no sequence required = no helper required.
				{
					DisposeAutoResponseHelper();
				}
			}
			else // Disabled.
			{
				DisposeAutoResponseHelper();
			}
		}

		/// <summary>
		/// Sends the automatic response.
		/// </summary>
		protected virtual void SendAutoResponse(byte[] triggerSequence)
		{
			this.autoResponseCount++; // Incrementing before sending to have the effective count available during sending.
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));

			int page = this.settingsRoot.Predefined.SelectedPageId;
			switch ((AutoResponse)this.settingsRoot.AutoResponse.Response)
			{
				case AutoResponse.Trigger:             SendAutoResponseTrigger(triggerSequence); break;
				case AutoResponse.SendText:            SendText();                               break;
				case AutoResponse.SendFile:            SendFile();                               break;
				case AutoResponse.PredefinedCommand1:  SendPredefined(page, 1);                  break;
				case AutoResponse.PredefinedCommand2:  SendPredefined(page, 2);                  break;
				case AutoResponse.PredefinedCommand3:  SendPredefined(page, 3);                  break;
				case AutoResponse.PredefinedCommand4:  SendPredefined(page, 4);                  break;
				case AutoResponse.PredefinedCommand5:  SendPredefined(page, 5);                  break;
				case AutoResponse.PredefinedCommand6:  SendPredefined(page, 6);                  break;
				case AutoResponse.PredefinedCommand7:  SendPredefined(page, 7);                  break;
				case AutoResponse.PredefinedCommand8:  SendPredefined(page, 8);                  break;
				case AutoResponse.PredefinedCommand9:  SendPredefined(page, 9);                  break;
				case AutoResponse.PredefinedCommand10: SendPredefined(page, 10);                 break;
				case AutoResponse.PredefinedCommand11: SendPredefined(page, 11);                 break;
				case AutoResponse.PredefinedCommand12: SendPredefined(page, 12);                 break;

				case AutoResponse.Explicit:
					SendCommand(new Command(this.settingsRoot.AutoResponse.Response)); // No explicit default radix available (yet).
					break;

				case AutoResponse.None:
				default:
					break;
			}
		}

		/// <summary>
		/// Sends the automatic response trigger.
		/// </summary>
		protected virtual void SendAutoResponseTrigger(byte[] triggerSequence)
		{
			if (triggerSequence != null)
				this.terminal.Send(TriggerSequenceWithTxEol(triggerSequence));
		}

		private byte[] TriggerSequenceWithTxEol(byte[] triggerSequence)
		{
			var l = new List<byte>(triggerSequence);

			if (this.settingsRoot.TerminalType == Domain.TerminalType.Text)
			{
				var textTerminal = (this.terminal as Domain.TextTerminal);

				// Add Tx EOL:
				var txEolSequence = textTerminal.TxEolSequence;
				l.AddRange(txEolSequence);
			}

			return (l.ToArray());
		}

		/// <summary>
		/// Gets the automatic response count.
		/// </summary>
		public virtual int AutoResponseCount
		{
			get
			{
				AssertNotDisposed();

				return (this.autoResponseCount);
			}
		}

		/// <summary>
		/// Resets the automatic response count.
		/// </summary>
		public virtual void ResetAutoResponseCount()
		{
			AssertNotDisposed();

			this.autoResponseCount = 0;
			OnAutoResponseCountChanged(new EventArgs<int>(this.autoResponseCount));
		}

		/// <summary>
		/// Deactivates the automatic response.
		/// </summary>
		public virtual void DeactivateAutoResponse()
		{
			AssertNotDisposed();

			this.settingsRoot.AutoResponse.Deactivate();
			ResetAutoResponseCount();
		}

		#endregion

		#region Auto > Common
		//------------------------------------------------------------------------------------------
		// Auto > Common
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to parse the given command into the corresponding byte sequence, taking the current settings into account.
		/// </summary>
		private bool TryParseCommandToSequence(Command c, out byte[] sequence)
		{
			if ((c != null) && (this.terminal != null))
			{
				if (c.IsSingleLineText)
				{
					byte[] lineResult;
					if (this.terminal.TryParseText(c.SingleLineText, out lineResult, c.DefaultRadix))
					{
						sequence = lineResult;
						return (true);
					}
				}
				else if (c.IsMultiLineText)
				{
					List<byte> commandResult = new List<byte>(256); // Preset the initial capacity to improve memory management; 256 is an arbitrary value.

					foreach (string line in c.MultiLineText)
					{
						byte[] lineResult;
						if (this.terminal.TryParseText(line, out lineResult, c.DefaultRadix))
							commandResult.AddRange(lineResult);
					}

					if (commandResult.Count > 0)
					{
						sequence = commandResult.ToArray();
						return (true);
					}
				}
			}

			sequence = null;
			return (false);
		}

		#endregion

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
		protected virtual void OnIOControlChanged(Domain.IOControlEventArgs e)
		{
			this.eventHelper.RaiseSync(IOControlChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOConnectTimeChanged(TimeSpanEventArgs e)
		{
			this.eventHelper.RaiseSync<TimeSpanEventArgs>(IOConnectTimeChanged, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIOCountChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOCountChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIORateChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IORateChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Clearly indicate that this item is a variant of the according item.")]
		protected virtual void OnIORateChanged_Decimated(EventArgs e)
		{
			this.eventHelper.RaiseSync(IORateChanged_Decimated, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIOError(Domain.IOErrorEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.IOErrorEventArgs>(IOError, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Named 'Sending...' rather than '...Sent' since sending is just about to happen and can
		/// be modified using the <see cref="Domain.ModifiablePacketEventArgs.Data"/> property or
		/// even canceled using the <see cref="Domain.ModifiablePacketEventArgs.Cancel"/> property.
		/// This is similar to the behavior of e.g. the 'OnValidating' event of WinForms controls.
		/// </remarks>
		protected virtual void OnSendingPacket(Domain.ModifiablePacketEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.ModifiablePacketEventArgs>(SendingPacket, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkSent(EventArgs<Domain.RawChunk> e)
		{
			this.eventHelper.RaiseSync<Domain.RawChunkEventArgs>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkReceived(EventArgs<Domain.RawChunk> e)
		{
			this.eventHelper.RaiseSync<Domain.RawChunkEventArgs>(RawChunkReceived, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void OnDisplayElementsTxAdded(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(DisplayElementsTxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsBidirAdded(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(DisplayElementsBidirAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayElementsRxAdded(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(DisplayElementsRxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineTxReplaced(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(CurrentDisplayLineTxReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineBidirReplaced(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(CurrentDisplayLineBidirReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineRxReplaced(Domain.DisplayElementsEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayElementsEventArgs>(CurrentDisplayLineRxReplaced, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineTxCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(CurrentDisplayLineTxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineBidirCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(CurrentDisplayLineBidirCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCurrentDisplayLineRxCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(CurrentDisplayLineRxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesTxAdded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinesEventArgs>(DisplayLinesTxAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesBidirAdded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinesEventArgs>(DisplayLinesBidirAdded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDisplayLinesRxAdded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.DisplayLinesEventArgs>(DisplayLinesRxAdded, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		protected virtual void OnScriptPacketReceived(Domain.PacketEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.PacketEventArgs>(ScriptPacketReceived, this, e);
		}

		/// <summary></summary>
		protected virtual void OnScriptMessageReceived(Domain.MessageEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.PacketEventArgs>(ScriptMessageReceived, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void OnRepositoryTxCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryTxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryBidirCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryBidirCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryRxCleared(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryRxCleared, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryTxReloaded(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryTxReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryBidirReloaded(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryBidirReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryRxReloaded(EventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryRxReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoActionCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoResponseCountChanged(EventArgs<int> e)
		{
			this.eventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged, this, e);
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnFixedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(FixedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnTimedStatusTextRequest(string text)
		{
			DebugMessage(text);
			this.eventHelper.RaiseSync<EventArgs<string>>(TimedStatusTextRequest, this, new EventArgs<string>(text));
		}

		/// <remarks>Not using event args parameter for simplicity.</remarks>
		protected virtual void OnResetStatusTextRequest()
		{
			this.eventHelper.RaiseSync(ResetStatusTextRequest, this, EventArgs.Empty);
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (OnMessageInputRequest(text, caption, buttons, icon, MessageBoxDefaultButton.Button1));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			if (this.startArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary>
		/// Requests to show the 'SaveAs' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual DialogResult OnSaveAsFileDialogRequest()
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new DialogEventArgs();
				this.eventHelper.RaiseSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (e.Result);
			}
			else
			{
				return (DialogResult.None);
			}
		}

		/// <summary>
		/// Requests to show the 'SaveAs' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual FilePathDialogResult OnSaveCommandPageAsFileDialogRequest(string filePathOld)
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(SaveCommandPageAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (new FilePathDialogResult(e.Result, e.FilePathNew));
			}
			else
			{
				return (new FilePathDialogResult(DialogResult.None));
			}
		}

		/// <summary>
		/// Requests to show the 'Open' dialog to let the user chose a file path.
		/// If confirmed, the file will be saved to that path.
		/// </summary>
		protected virtual FilePathDialogResult OnOpenCommandPageFileDialogRequest(string filePathOld)
		{
			if (this.startArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(OpenCommandPageFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "An 'Open' request by terminal '" + Caption + "' was not processed by the application!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				return (new FilePathDialogResult(e.Result, e.FilePathNew));
			}
			else
			{
				return (new FilePathDialogResult(DialogResult.None));
			}
		}

		/// <remarks>Using item instead of <see cref="EventArgs"/> for simplicity.</remarks>
		protected virtual void OnCursorRequest(Cursor cursor)
		{
			this.eventHelper.RaiseSync<EventArgs<Cursor>>(CursorRequest, this, new EventArgs<Cursor>(cursor));
		}

		/// <summary></summary>
		protected virtual void OnCursorReset()
		{
			OnCursorRequest(Cursors.Default);
		}

		/// <summary></summary>
		protected virtual void OnSaved(SavedEventArgs e)
		{
			this.eventHelper.RaiseSync<SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnClosed(ClosedEventArgs e)
		{
			this.eventHelper.RaiseSync<ClosedEventArgs>(Closed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnExitRequest(EventArgs e)
		{
			this.eventHelper.RaiseSync(ExitRequest, this, e);
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
			if (IsDisposed)
				return (base.ToString()); // Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (Caption);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			if ((message == "Sending...") || (message == "Receiving..."))
				return; // Skip messages not useful for debugging.

			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"[" + Guid + "]",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
