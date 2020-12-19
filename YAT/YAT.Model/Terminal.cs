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

	// Enable debugging of threads:
////#define DEBUG_THREADS

#endif // DEBUG

#endregion

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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections;
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
using YAT.Model.Types;
//// 'YAT.Model.Utilities' is explicitly used due to ambiguity of 'MessageHelper'.
using YAT.Settings.Application;
using YAT.Settings.Model;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Terminals (.yat) of the YAT application model.
	/// </summary>
	/// <remarks>
	/// This class is implemented using partial classes separating send and automatic functionality.
	/// Using partial classes to ease diffing code of the separated functionality.
	/// </remarks>
	/// <remarks>
	/// \remind (2020-09-16 / MKY while integrating YAT 2.2.0 into Albatros)
	/// Could alternatively be partialized into 'N/A'/.Outgoing/.Incoming.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public partial class Terminal : DisposableBase, IGuidProvider
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

		private const int ThreadWaitTimeout = 500; // Enough time to let the threads join...

		private const int RateIntervalsPerSecond = 4; // 250 ms intervals

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static int staticSequentialIdCounter = SequentialIdCounterDefault;
		private static Random staticRandom = new Random(RandomEx.NextRandomSeed());

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Gets or sets a value indicating whether a script run is currently active,
		/// i.e. whether the terminals shall produce received messages for a script.
		/// </summary>
		/// <remarks>
		/// Implemented as static property for two reasons:
		/// <list type="bullet">
		/// <item><description>State always applies to all terminals.</description></item>
		/// <item><description>State can easier be applied to newly created terminals.</description></item>
		/// </list>
		/// </remarks>
		public static bool ScriptRunIsActive
		{
			get { return (Domain.Terminal.ScriptRunIsActive); }
			set { Domain.Terminal.ScriptRunIsActive = value;  }
		}

	#endif

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
		/// <see cref="Main.Exit_ForTestOnly()"/>. But at that moment, the settings have already
		/// been disposed of and can no longer be accessed.
		/// The first approach was to disable disposal in <see cref="Close()"/>. But that leads to
		/// remaining resources, resulting in significant slow-down when exiting NUnit.
		/// The second approach was to retrieve the required information *before* exiting, i.e.
		/// calling <see cref="Main.Exit_ForTestOnly()"/>. But that doesn't work at all, since
		/// auto-save paths are only evaluated *at* <see cref="Main.Exit_ForTestOnly()"/>.
		///
		/// This workaround is considered the best option to solve this issue.
		/// </summary>
		/// <remarks>
		/// Note that it is not possible to mark a property with [Conditional("TEST")].
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize purpose of this property.")]
		public bool DoNotDetachSettingsBecauseTheyAreRequiredForVerification_ForTestOnly { get; set; }

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

		private TerminalLaunchArgs launchArgs;
		private Guid guid;
		private int sequentialId;
		private string sequentialName;
		private string userFileName;

		// Initiating:
		private bool autoIsReady; // = false;

		// Settings:
		private DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler;
		private TerminalSettingsRoot settingsRoot;

		// Terminal:
		private Domain.Terminal terminal;

		// Logs:
		private Log.Provider log;

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

	////private object countsRatesSyncObj = new object(); \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.

	#if (WITH_SCRIPTING)

		// Scripting:
		private string lastSentMessageByScripting; // = null;
		private object lastSentMessageByScriptingSyncObj = new object();
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
		public event EventHandler<EventArgs<DateTime>> IOChanged;

		/// <summary></summary>
		public event EventHandler<Domain.IOControlEventArgs> IOControlChanged;

		/// <summary></summary>
		public event EventHandler<TimeSpanEventArgs> IOConnectTimeChanged;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		public event EventHandler IOCountChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		public event EventHandler IORateChanged_Promptly;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		public event EventHandler IORateChanged_Decimated;

		/// <summary></summary>
		public event EventHandler<Domain.IOErrorEventArgs> IOError;

	#if (WITH_SCRIPTING)

		// Note that e.g. a 'SendingText' or 'SendingMessage' event doesn't make sense, as it would
		// contain parseable text that may even include keyword to be processed.

		/// <remarks>See <see cref="Domain.Terminal.SendingPacket"/>.</remarks>
		public event EventHandler<Domain.ModifiablePacketEventArgs> SendingPacket;

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		public event EventHandler<EventArgs<bool>> IsSendingChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<bool>> IsSendingForSomeTimeChanged;

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RawChunk>> RawChunkSent;

		/// <summary></summary>
		public event EventHandler<EventArgs<Domain.RawChunk>> RawChunkReceived;

	#endif // WITH_SCRIPTING

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayElementsTxAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsTxAdded;

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayElementsBidirAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsBidirAdded;

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayElementsRxAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> DisplayElementsRxAdded;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineTxReplaced"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineTxReplaced;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineBidirReplaced"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineBidirReplaced;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineRxReplaced"/>.</remarks>
		public event EventHandler<Domain.DisplayElementsEventArgs> CurrentDisplayLineRxReplaced;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineTxCleared"/>.</remarks>
		public event EventHandler CurrentDisplayLineTxCleared;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineBidirCleared"/>.</remarks>
		public event EventHandler CurrentDisplayLineBidirCleared;

		/// <remarks>See <see cref="Domain.Terminal.CurrentDisplayLineRxCleared"/>.</remarks>
		public event EventHandler CurrentDisplayLineRxCleared;

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayLinesTxAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesTxAdded;

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayLinesBidirAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesBidirAdded;

		/// <remarks>Also see <see cref="Domain.Terminal.DisplayLinesRxAdded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> DisplayLinesRxAdded;

	#if (WITH_SCRIPTING)

		/// <remarks>See <see cref="Domain.Terminal.ReceivingPacketForScripting"/>.</remarks>
		public event EventHandler<Domain.ModifiablePacketEventArgs> ReceivingPacketForScripting;

		/// <remarks>See <see cref="Domain.Terminal.MessageReceivedForScripting"/>.</remarks>
		public event EventHandler<Domain.ScriptMessageEventArgs> MessageReceivedForScripting;

	#endif // WITH_SCRIPTING

		/// <remarks>See <see cref="Domain.Terminal.RepositoryTxCleared"/>.</remarks>
		public event EventHandler RepositoryTxCleared;

		/// <remarks>See <see cref="Domain.Terminal.RepositoryBidirCleared"/>.</remarks>
		public event EventHandler RepositoryBidirCleared;

		/// <remarks>See <see cref="Domain.Terminal.RepositoryRxCleared"/>.</remarks>
		public event EventHandler RepositoryRxCleared;

		/// <remarks>See <see cref="Domain.Terminal.RepositoryTxReloaded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> RepositoryTxReloaded;

		/// <remarks>See <see cref="Domain.Terminal.RepositoryBidirReloaded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> RepositoryBidirReloaded;

		/// <remarks>See <see cref="Domain.Terminal.RepositoryRxReloaded"/>.</remarks>
		public event EventHandler<Domain.DisplayLinesEventArgs> RepositoryRxReloaded;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> FixedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<string>> TimedStatusTextRequest;

		/// <summary></summary>
		public event EventHandler ResetStatusTextRequest;

		/// <summary></summary>
		public event EventHandler<EventArgs<Cursor>> CursorRequest;

		/// <summary></summary>
		public event EventHandler<MessageInputEventArgs> MessageInputRequest;

		/// <summary></summary>
		public event EventHandler<DialogEventArgs> SaveAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> SaveCommandPageAsFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<FilePathDialogEventArgs> OpenCommandPageFileDialogRequest;

		/// <summary></summary>
		public event EventHandler<SavedEventArgs> Saved;

		/// <summary></summary>
		public event EventHandler<ClosedEventArgs> Closed;

		/// <summary></summary>
		public event EventHandler<EventArgs<ExitMode>> ExitRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Terminal(TerminalSettingsRoot settings)
			: this(new DocumentSettingsHandler<TerminalSettingsRoot>(settings))
		{
		}

		/// <summary></summary>
		public Terminal(DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(new TerminalLaunchArgs(), settingsHandler)
		{
		}

		/// <summary></summary>
		public Terminal(TerminalLaunchArgs launchArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler)
			: this(launchArgs, settingsHandler, Guid.Empty)
		{
		}

		/// <remarks>See <see cref="Guid.Empty"/> cannot be used as default argument as it is read-only.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid", Justification = "Why not? 'Guid' not only is a type, but also emphasizes a purpose.")]
		public Terminal(TerminalLaunchArgs launchArgs, DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler, Guid guid)
		{
			try
			{
				DebugMessage("Creating...");

				this.launchArgs = launchArgs;

				if (guid != Guid.Empty)
					this.guid = guid;
				else
					this.guid = Guid.NewGuid();

				// Link and attach to settings:
				this.settingsHandler = settingsHandler;
				this.settingsRoot = this.settingsHandler.Settings;
				this.settingsRoot.ClearChanged();
				AttachSettingsEventHandlers();

				// Set ID and name(s):
				this.sequentialId = ++staticSequentialIdCounter;
				this.sequentialName = TerminalText + this.sequentialId.ToString(CultureInfo.CurrentCulture);
				if (!this.settingsRoot.AutoSaved && this.settingsHandler.SettingsFilePathIsValid)
					this.userFileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

				// Create underlying terminal:
				this.terminal = Domain.TerminalFactory.CreateTerminal(this.settingsRoot.Terminal);
				AttachTerminalEventHandlers();

				// Create log:
				this.log = new Log.Provider(this.settingsRoot.Log, (EncodingEx)this.settingsRoot.TextTerminal.Encoding, this.settingsRoot.Format);

				// Create Auto[Action|Response]:
				CreateAutoAction();
				CreateAutoResponse();

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

				// In the 'normal' case, terminal and log have already been closed, otherwise...

				// ...detach event handlers to ensure that no more events are received...
				DetachTerminalEventHandlers();

				// ...ensure that threads are stopped and do not raise events anymore...
				StopAutoActionThread();
				StopAutoResponseThread();

				// ...ensure that timed objects are stopped and do not raise events anymore...
				DisposeRates();
				DisposeChronos();
				DisposeAutoActionHelper();
				DisposeAutoResponseHelper();

				// ...close and dispose of terminal and log...
				CloseAndDisposeTerminal();
				DisposeLog();

				// ...and finally detach the settings:
				if (!DoNotDetachSettingsBecauseTheyAreRequiredForVerification_ForTestOnly)
				{
					DetachSettingsEventHandlers();
					DetachSettingsHandler();
				}

				DebugMessage("...successfully disposed.");
			}

		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
		}

		#endregion

		#endregion

		#region General
		//==========================================================================================
		// General
		//==========================================================================================

		/// <summary></summary>
		public virtual TerminalLaunchArgs LaunchArgs
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.launchArgs);
			}
		}

		/// <summary></summary>
		public virtual Guid Guid
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.guid);
			}
		}

		/// <summary></summary>
		public virtual int SequentialId
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.sequentialName);
			}
		}

		/// <summary>
		/// The file name if the user has saved the terminal; otherwise <see cref="string.Empty"/>.
		/// </summary>
		/// <remarks>
		/// Cached from <see cref="SettingsFilePath"/> for...
		/// ...limiting to user files (i.e. not 'AutoSaved').
		/// ...having to compose the name only once.
		/// </remarks>
		public virtual string UserFileName
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (!string.IsNullOrEmpty(this.userFileName))
					return (this.userFileName);

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (SettingsRoot != null)
				{
					if (!string.IsNullOrEmpty(SettingsRoot.UserName))
						return (SettingsRoot.UserName);
				}

				return ("");
			}
		}

		/// <summary>
		/// The indicated name, i.e. either the <see cref="UserName"/>, <see cref="UserFileName"/>
		/// or <see cref="SequentialName"/>.
		/// </summary>
		public virtual string IndicatedName
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (!string.IsNullOrEmpty(UserName))
					return (UserName);

				if (!string.IsNullOrEmpty(UserFileName))
					return (UserFileName);

				return (SequentialName);
			}
		}

		/// <summary></summary>
		public virtual string Caption
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Utilities.CaptionHelper.ComposeTerminal(this.settingsHandler, this.settingsRoot, this.terminal, IndicatedName, IsStarted, IsOpen, IsConnected));
			}
		}

		/// <summary></summary>
		public virtual string ComposeCaption(string info)
		{
		////AssertUndisposed() shall not be called from this simple get-property-style-method.

			return (Utilities.CaptionHelper.ComposeTerminal(IndicatedName, info));
		}

		/// <summary></summary>
		public virtual Domain.IOType IOType
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (SettingsRoot != null)
					return (SettingsRoot.IOType);
				else
					return (Domain.IOType.Unknown);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsReadyToSend);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsReadyToSendForSomeTime
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsReadyToSendForSomeTime);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool IsSending
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsSending);
				else
					return (false);
			}
		}

		/// <remarks>
		/// Opposed to <see cref="IsSending"/>, this property only becomes <c>true</c> when
		/// sending has been ongoing for more than <see cref="Domain.Utilities.ForSomeTimeEventHelper.Threshold"/>,
		/// or is about to be ongoing for more than <see cref="Domain.Utilities.ForSomeTimeEventHelper.Threshold"/>.
		/// </remarks>
		public virtual bool IsSendingForSomeTime
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				if (this.terminal != null)
					return (this.terminal.IsSendingForSomeTime);
				else
					return (false);
			}
		}

		// \remind (2020-12-07 / MKY)
		// 'IsReceiving' and 'IsReceivingForSomeTime' are yet pending.

		/// <summary></summary>
		public virtual bool LogIsOn
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
				AssertUndisposed();

				return (this.terminal.ToShortIOString());
			}
		}

		/// <summary></summary>
		public virtual MKY.IO.Serial.IIOProvider UnderlyingIOProvider
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.UnderlyingIOProvider);
			}
		}

		/// <summary></summary>
		public virtual object UnderlyingIOInstance
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.UnderlyingIOInstance);
			}
		}

		/// <remarks>Only to be used for testing.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize purpose of this property.")]
		public virtual Domain.Terminal UnderlyingDomain_ForTestOnly
		{
			get
			{
				AssertUndisposed();

				return (this.terminal);
			}
		}

		/// <summary></summary>
		public virtual string IOStatusText
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (Utilities.IOStatusHelper.Compose(this.settingsRoot, this.terminal, IsStarted, IsOpen, IsConnected));
			}
		}

		/// <summary>
		/// Returns the name of the serial COM port for <see cref="Domain.IOType.SerialPort"/>,
		/// <c>null</c> for all other terminal types.
		/// </summary>
		public virtual string IOSerialPortName
		{
			get
			{
				AssertUndisposed();

				if (SettingsRoot != null)
				{
					if (SettingsRoot.IOType == Domain.IOType.SerialPort)
						return (SettingsRoot.IO.SerialPort.PortId);
				}

				return (null);
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Gets the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (rather than in overlying <see cref="ScriptBridge"/>)
		/// to keep the message for each terminal.
		/// </remarks>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void SetLastSentMessageByScripting(string value)
		{
			AssertUndisposed();

			lock (this.lastSentMessageByScriptingSyncObj)
				this.lastSentMessageByScripting = value;
		}

		/// <summary>
		/// Gets the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (rather than in overlying <see cref="ScriptBridge"/>)
		/// to keep the message for each terminal.
		/// </remarks>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void GetLastSentMessageByScripting(out string value)
		{
			AssertUndisposed();

			lock (this.lastSentMessageByScriptingSyncObj)
				value = this.lastSentMessageByScripting;
		}

		/// <summary>
		/// Clears the last sent message.
		/// </summary>
		/// <remarks>
		/// Located here in the underlying terminal (rather than in overlying <see cref="ScriptBridge"/>)
		/// to keep the message for each terminal.
		/// </remarks>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void ClearLastSentMessageByScripting(out string cleared)
		{
			AssertUndisposed();

			lock (this.lastSentMessageByScriptingSyncObj)
			{
				cleared = this.lastSentMessageByScripting;

				this.lastSentMessageByScripting = null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the terminal has received a line that is available for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual bool HasReceivedMessageAvailableForScripting
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.HasReceivedMessageAvailableForScripting);
			}
		}

		/// <summary>
		/// Gets a value indicating the number of received lines that are available for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public int AvailableReceivedMessageCountForScripting
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.AvailableReceivedMessageCountForScripting);
			}
		}

		/// <summary>
		/// Returns the line that has last been enqueued into the receive queue that is available for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void GetLastEnqueuedReceivedMessageForScripting(out Domain.ScriptMessage value)
		{
			AssertUndisposed();

			this.terminal.GetLastEnqueuedReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Clears the last enqueued line that is available for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void ClearLastEnqueuedReceivedMessageForScripting(out Domain.ScriptMessage cleared)
		{
			AssertUndisposed();

			this.terminal.ClearLastEnqueuedReceivedMessageForScripting(out cleared);
		}

		/// <summary>
		/// Gets the next received line that is available for scripting and removes it from the queue.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// The underlying <see cref="Queue{T}"/> is empty.
		/// </exception>
		public virtual void DequeueNextAvailableReceivedMessageForScripting(out Domain.ScriptMessage value, out DateTime dequeueTimeStamp)
		{
			AssertUndisposed();

			this.terminal.DequeueNextAvailableReceivedMessageForScripting(out value, out dequeueTimeStamp);
		}

		/// <summary>
		/// Returns the received line that has last been dequeued from the receive queue for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void GetLastDequeuedReceivedMessageForScripting(out Domain.ScriptMessage value)
		{
			AssertUndisposed();

			this.terminal.GetLastDequeuedReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Clears the last dequeued line that is available for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public virtual void ClearLastDequeuedReceivedMessageForScripting(out Domain.ScriptMessage cleared)
		{
			AssertUndisposed();

			this.terminal.ClearLastDequeuedReceivedMessageForScripting(out cleared);
		}

		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		/// <remarks>
		/// \remind (2018-03-27 / MKY)
		/// 'LastAvailable' only works properly for a terminating number of received messages, but
		/// not for consecutive receiving. This method shall be eliminated as soon as the obsolete
		/// GetLastReceived(), CheckLastReceived() and WaitFor() have been removed.
		/// </remarks>
		[Obsolete("See remarks.")]
		public virtual void GetLastAvailableReceivedMessageForScripting(out Domain.ScriptMessage value)
		{
			AssertUndisposed();

			this.terminal.GetLastAvailableReceivedMessageForScripting(out value);
		}

		/// <summary>
		/// Clears all available lines in the receive queue for scripting.
		/// </summary>
		/// <remarks>
		/// Scripting uses term 'Message' for distinction with term 'Line' which is tied to displaying.
		/// </remarks>
		public void ClearAvailableReceivedMessagesForScripting(out Domain.ScriptMessage[] cleared, out DateTime clearTimeStamp)
		{
			AssertUndisposed();

			this.terminal.ClearAvailableReceivedMessagesForScripting(out cleared, out clearTimeStamp);
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
				AssertUndisposed();

				return (this.isAutoSocket);
			}
			set
			{
				AssertUndisposed();

				this.isAutoSocket = value;
			}
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Launch
		//==========================================================================================
		// Launch
		//==========================================================================================

		/// <summary>
		/// Launches the terminal, i.e. starts log and opens I/O.
		/// </summary>
		/// <remarks>
		/// Using term "launch" rather than "start" for distinction with "start/stop" I/O.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public virtual bool Launch()
		{
			AssertUndisposed();

			DebugMessage("Launching...");

			StartRates();
		////StartChronos() is not needed, chronos will be started/stopped in the terminal_IOChanged event handler.

			StartAutoActionThread();
			StartAutoResponseThread();

			// Switch log on if selected:
			if (SettingsRoot.LogIsOn)
			{
				if (!SwitchLogOn())
					return (false);
			}

			// Then start terminal if selected:
			if (SettingsRoot.TerminalIsStarted)
			{
				// Check availability of I/O before starting:
				var result = CheckIOAvailability();
				switch (result)
				{
					case CheckResult.OK:     return (Start());
					case CheckResult.Cancel: return (false);
					case CheckResult.Ignore: return (true);

					default: throw (new NotSupportedException(MKY.MessageHelper.InvalidExecutionPreamble + "'" + result.ToString() + "' is a result that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
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
			{
				this.settingsRoot.Changed += settingsRoot_Changed;

				// Initialize 'Changed' detectors:
				this.settingsRoot_Changed_terminalTypeOld    = this.settingsRoot.Terminal.TerminalType;
				this.settingsRoot_Changed_endiannessOld      = this.settingsRoot.Terminal.IO.Endianness;
				this.settingsRoot_Changed_sendImmediatelyOld = this.settingsRoot.Terminal.Send.Text.SendImmediately;
				this.settingsRoot_Changed_encodingOld        = this.settingsRoot.Terminal.TextTerminal.Encoding;
			}
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
			{
				this.settingsRoot.Changed -= settingsRoot_Changed;
			}
		}

		private void DetachSettingsHandler()
		{
			if (this.settingsHandler != null)
			{
				this.settingsHandler = null;
			}
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.TerminalType settingsRoot_Changed_terminalTypeOld = Domain.Settings.TerminalSettings.TerminalTypeDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Domain.Endianness settingsRoot_Changed_endiannessOld = Domain.Settings.IOSettings.EndiannessDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int settingsRoot_Changed_encodingOld = Domain.Settings.TextTerminalSettings.EncodingDefault;

		/// <remarks>
		/// Required to solve the issue described in bug #223 "Settings events should state the exact settings diff".
		/// </remarks>
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
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Explicit))
			{
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Implicit))
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
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Terminal))
			{
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.PredefinedCommand))
			{
				UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.AutoAction))
			{
				UpdateAutoAction(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.

				if (SettingsRoot.AutoAction.IsActiveAsFilterOrSuppress)
					RefreshRepositories();
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.AutoResponse))
			{
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Format))
			{
				this.log.NeatFormat = SettingsRoot.Format;
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Log))
			{
				this.log.Settings = SettingsRoot.Log;
			}
		}

		private void HandleTerminalSettings(SettingsEventArgs e)
		{
			// \ToDo: ApplySettings should be called here => FR #309.

			if (e.Inner == null)
			{
				if (this.settingsRoot_Changed_terminalTypeOld != SettingsRoot.TerminalType) {
					this.settingsRoot_Changed_terminalTypeOld = SettingsRoot.TerminalType;

					// Terminal type has changed, recreate Auto[Action|Response]:
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Terminal.IO))
			{
				if (this.settingsRoot_Changed_endiannessOld != SettingsRoot.Terminal.IO.Endianness) {
					this.settingsRoot_Changed_endiannessOld = SettingsRoot.Terminal.IO.Endianness; // Relevant for byte sequence based triggers.

					// Endianness has changed, recreate Auto[Action|Response]:
					UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
					UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				}
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Terminal.Send))
			{
				if (this.settingsRoot_Changed_sendImmediatelyOld != SettingsRoot.Terminal.Send.Text.SendImmediately) {
					this.settingsRoot_Changed_sendImmediatelyOld = SettingsRoot.Terminal.Send.Text.SendImmediately;

					// Send immediately has changed, clear the command:
					this.settingsRoot.SendText.ClearCommand();
				}
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.Terminal.TextTerminal))
			{
				this.log.TextTerminalEncoding = (EncodingEx)SettingsRoot.Terminal.TextTerminal.Encoding;

				if (this.settingsRoot_Changed_encodingOld != SettingsRoot.Terminal.TextTerminal.Encoding) {
					this.settingsRoot_Changed_encodingOld = SettingsRoot.Terminal.TextTerminal.Encoding; // Relevant for byte sequence based triggers.

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
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.SendText))
			{
				UpdateAutoAction();   // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
				UpdateAutoResponse(); // \ToDo: Not a good solution, manually gathering all relevant changes, better solution should be found.
			}
			else if (ReferenceEquals(e.Inner.Source, SettingsRoot.SendFile))
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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
			////AssertUndisposed() shall not be called from this simple get-property.

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
				// Attention:
				// AssertUndisposed() must not be called to still allow reading the settings after
				// the terminal has been disposed. This is required for certain test cases.

				return (this.settingsRoot);
			}
		}

		/// <summary>
		/// Gets a value indicating whether one or more automatic items are active.
		/// </summary>
		public virtual bool AutoIsActive
		{
			get { return (SettingsRoot.AutoIsActive); }
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
		/// Notifies this instance that startup by the parent has completed.
		/// </summary>
		/// <remarks>
		/// \remind (2019-12-02 / MKY)
		///
		/// Background:
		///  1. The workspace will create this terminal.
		///  2. The constructor of this terminal will call CreateAuto[Action|Response]Helper() but
		///     linked settings are not available yet.
		///      => TryGetActiveAutoActionTrigger() will fail in case of linked commands.
		///  3. The workspace will also called TryLoadLinkedSettings().
		///      => TryGetActiveAutoActionTrigger() will succeed.
		///
		/// Not the perfect solution, but good enough for the moment.
		/// </remarks>
		public virtual void NotifyAutoIsReady()
		{
			this.autoIsReady = true;
		}

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
			AssertUndisposed();

			// Attention:
			// Similar code exists in Domain.Terminal.TryChangeSettingsOnTheFly().
			// Changes here may have to be applied there too.

			if (this.terminal.IsStarted) // Terminal is started, stop and restart it with the new settings:
			{
				// Note that the whole terminal will be recreated. Thus, it must also be recreated if non-IO settings have changed.

				if (Stop(false))
				{
					OnFixedStatusTextRequest("Applying settings and refreshing terminal...");

					this.terminal.EmptyRepositories();           // RecreateTerminal() below will take long, emptying makes
					                                           //// it more obvious to the user that something is ongoing,
					System.Windows.Forms.Application.DoEvents(); // but requires yielding to let view be refreshed.

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

					OnFixedStatusTextRequest("...refreshing view..."); // RefreshRepositories() below may also take long, stating makes
					                                                 //// it more obvious to the user that something is ongoing,
					System.Windows.Forms.Application.DoEvents();       // but requires yielding to let view be refreshed.

					this.terminal.RefreshRepositories();

					if (Start(false))
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
				OnFixedStatusTextRequest("Applying settings and refreshing terminal...");

				this.terminal.EmptyRepositories();           // RecreateTerminal() below will take long, emptying makes
				                                           //// it more obvious to the user that something is ongoing,
				System.Windows.Forms.Application.DoEvents(); // but requires yielding to let view be refreshed.

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

				OnFixedStatusTextRequest("...refreshing view..."); // RefreshRepositories() below may also take long, stating makes
				                                                 //// it more obvious to the user that something is ongoing,
				System.Windows.Forms.Application.DoEvents();       // but requires yielding to let view be refreshed.

				this.terminal.RefreshRepositories();

				OnTimedStatusTextRequest("Terminal settings applied.");
			}
		}

		/// <summary>
		/// Applies new log settings.
		/// </summary>
		public virtual void ApplyLogSettings(Log.Settings.LogSettings settings)
		{
			AssertUndisposed();

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
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool Save()
		{
		////AssertUndisposed() is called by 'Save(...)' below.

			bool isCanceled;                               // Save even if not changed since explicitly requesting saving.
			return (SaveWithOptions(false, true, true, true, false, out isCanceled));
		}

		/// <summary>
		/// Silently tries to save terminal to file, i.e. without any user interaction.
		/// </summary>
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool SaveWithOptionsWithoutUserInteraction(bool isWorkspaceClose, bool autoSaveIsAllowed)
		{
			bool isCanceled;
			return (SaveWithOptions(isWorkspaceClose, autoSaveIsAllowed, false, false, false, out isCanceled));
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
		public virtual bool SaveWithOptions(bool isWorkspaceClose, bool autoSaveIsAllowed, bool userInteractionIsAllowed, bool saveEvenIfNotChanged, bool canBeCanceled, out bool isCanceled)
		{
			AssertUndisposed();

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
			// Similar code exists in TrySaveLinkedCommandPageWithOptions() further below.
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

			return (SaveToFile(autoSaveIsAllowed, userInteractionIsAllowed, null));
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
				throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'SaveAs'!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));

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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to deal with.")]
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
				throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'LinkFilePath'!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));

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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to deal with.")]
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
				throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "Invalid reason for requesting restricted 'LinkFilePath'!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));

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
		/// Saves terminal to given file, prompts for file as needed.
		/// </summary>
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool SaveAs(string filePath)
		{
		////AssertUndisposed() is called by 'SaveAsWithOptions(...)' below.

			return (SaveAsWithOptions(filePath, true));
		}

		/// <summary>
		/// Silently tries to save terminal to given file, i.e. without any user interaction.
		/// </summary>
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool SaveAsWithoutUserInteraction(string filePath)
		{
		////AssertUndisposed() is called by 'SaveAsWithOptions(...)' below.

			return (SaveAsWithOptions(filePath, false));
		}

		/// <summary>
		/// This method implements the logic that is needed when saving, opposed to the method
		/// <see cref="SaveToFile"/> which just performs the actual save, i.e. file handling.
		/// </summary>
		public virtual bool SaveAsWithOptions(string filePath, bool userInteractionIsAllowed)
		{
			AssertUndisposed();

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
			return (SaveToFile(false, userInteractionIsAllowed, autoSaveFilePathToDelete));
		}

		/// <param name="isAutoSave">
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </param>
		/// <param name="userInteractionIsAllowed">Indicates whether user interaction is allowed.</param>
		/// <param name="autoSaveFilePathToDelete">
		/// The path to the former auto saved file, it will be deleted if the file can successfully
		/// be stored in the new location.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		protected virtual bool SaveToFile(bool isAutoSave, bool userInteractionIsAllowed, string autoSaveFilePathToDelete)
		{
			OnFixedStatusTextRequest("Saving terminal...");

			bool success;

			try
			{
				this.settingsHandler.Settings.AutoSaved = isAutoSave;
				this.settingsHandler.Save();
				success = true;

				if (!isAutoSave)
					this.userFileName = Path.GetFileName(this.settingsHandler.SettingsFilePath);

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

				if (userInteractionIsAllowed)
				{
					OnFixedStatusTextRequest("Error saving terminal!");
					OnMessageInputRequest
					(
						Utilities.MessageHelper.ComposeMessage("Unable to save terminal file", this.settingsHandler.SettingsFilePath, ex),
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					OnTimedStatusTextRequest("Terminal not saved!");
				}

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
				bool linkFilePathHasChanged;
				if (TryLoadLinkedPredefinedCommandPageWithOptions(linkedPage, false, userInteractionIsAllowed, canBeCanceled, out linkedSettingsHandler, out linkFilePathHasChanged, out isCanceled)) {
					if (linkFilePathHasChanged)
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
					if (TrySaveLinkedCommandPageWithOptions(linkedPage, linkedSettingsHandler, userInteractionIsAllowed, out linkFilePathHasChanged, out isCanceled)) {
						if (linkFilePathHasChanged)
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to deal with.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected virtual bool TrySaveLinkedCommandPageWithOptions(PredefinedCommandPage linkedPage,
		                                                           DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler,
		                                                           bool userInteractionIsAllowed,
		                                                           out bool hasChanged, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in TrySaveWithOptions() further above.
			// Changes here may have to be applied there too.

			// Attention:
			// Similar code exists in TryLoadLinkedPredefinedCommandPageWithOptions() below.
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
						Utilities.MessageHelper.ComposeMessage("Unable to save linked predefined command page file", linkedSettingsHandler.SettingsFilePath, ex),
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to deal with.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to deal with.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "6#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected virtual bool TryLoadLinkedPredefinedCommandPageWithOptions(PredefinedCommandPage linkedPage, bool doUpdateLinkedPage,
		                                                                     bool userInteractionIsAllowed, bool canBeCanceled,
		                                                                     out DocumentSettingsHandler<CommandPageSettingsRoot> linkedSettingsHandler,
		                                                                     out bool linkFilePathHasChanged, out bool isCanceled)
		{
			// Attention:
			// Similar code exists in TrySaveWithOptions() further above.
			// Changes here may have to be applied there too.

			// Attention:
			// Similar code exists in TrySaveLinkedPredefinedCommandPageWithOptions() further above.
			// Changes here may have to be applied there too.

			var isFirst = true;
			var currentFilePath = linkedPage.LinkFilePath;
			var currentSettingsHandler = new DocumentSettingsHandler<CommandPageSettingsRoot>();

			try
			{
				linkFilePathHasChanged = false;
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
									linkFilePathHasChanged = true;
								}
							}
							else if (doUnlink) {
								linkedPage.Unlink();
								linkedSettingsHandler = null;
								linkFilePathHasChanged = true;
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
				linkFilePathHasChanged = false;

				if (userInteractionIsAllowed)
				{
					OnFixedStatusTextRequest("Error retrieving linked predefined command page!");
					var dr = OnMessageInputRequest
					(
						Utilities.MessageHelper.ComposeMessage("Unable to retrieve linked predefined command page file", currentFilePath, ex),
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
				bool linkFilePathHasChanged;
				if (TryLoadLinkedPredefinedCommandPageWithOptions(linkedPage, true, userInteractionIsAllowed, canBeCanceled, out linkedSettingsHandler, out linkFilePathHasChanged, out isCanceled)) {
					this.settingsHandler.Settings.PredefinedCommand.SetChanged(); // Commands have changed in any case.

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
		/// Closes the terminal, prompts if needed if settings have changed.
		/// </summary>
		/// <remarks>
		/// In case of a workspace close, <see cref="CloseWithOptions"/> further below must be
		/// called with the first argument set to <c>true</c>.
		///
		/// In case of intended close of one or all terminals, the user intentionally wants to close
		/// the terminal(s), thus, this method will not try to auto save.
		/// </remarks>
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool Close()
		{
			return (CloseWithOptions(false, true, false, true)); // See remarks above.
		}

		/// <summary>
		/// Silently tries to save terminal to file, i.e. without any user interaction.
		/// </summary>
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool CloseWithOptionsWithoutSave(bool isWorkspaceClose)
		{
			return (CloseWithOptions(isWorkspaceClose, false, false, true));
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
		/// <remarks>
		/// Not named "Try" same as all other "main" methods.
		/// </remarks>
		public virtual bool CloseWithOptions(bool isWorkspaceClose, bool doSave, bool autoSaveIsAllowed, bool autoDeleteIsRequested)
		{
			AssertUndisposed();

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

			// Do neither try to auto save nor manually save if there is no existing file (w1, w3)
			// or (t1, t3), except in case of w1a, i.e. when the file has never been loaded so far.
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
				success = SaveWithOptionsWithoutUserInteraction(isWorkspaceClose, autoSaveIsAllowed); // Try auto save.

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

			if (success)
			{
				StopAutoActionThread();
				StopAutoResponseThread();

				StopRates();
			////StopChronos() is not needed, chronos will be started/stopped in the terminal_IOChanged event handler.
			}

			if (success && this.terminal.IsStarted)
			{
				success = Stop(false);
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

		#region Domain
		//==========================================================================================
		// Domain
		//==========================================================================================

		#region Domain > Lifetime
		//------------------------------------------------------------------------------------------
		// Domain > Lifetime
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

				this.terminal.IsSendingChanged            += terminal_IsSendingChanged;
				this.terminal.IsSendingForSomeTimeChanged += terminal_IsSendingForSomeTimeChanged;

				this.terminal.RawChunkSent     += terminal_RawChunkSent;
				this.terminal.RawChunkReceived += terminal_RawChunkReceived;

				this.terminal.DisplayElementsTxAdded          += terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       += terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          += terminal_DisplayElementsRxAdded;
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
				this.terminal.ReceivingPacketForScripting += terminal_ReceivingPacketForScripting;
				this.terminal.MessageReceivedForScripting += terminal_MessageReceivedForScripting;
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

				this.terminal.IsSendingChanged            -= terminal_IsSendingChanged;
				this.terminal.IsSendingForSomeTimeChanged -= terminal_IsSendingForSomeTimeChanged;

				this.terminal.RawChunkSent     -= terminal_RawChunkSent;
				this.terminal.RawChunkReceived -= terminal_RawChunkReceived;

				this.terminal.DisplayElementsTxAdded          -= terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       -= terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          -= terminal_DisplayElementsRxAdded;
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
				this.terminal.ReceivingPacketForScripting -= terminal_ReceivingPacketForScripting;
				this.terminal.MessageReceivedForScripting -= terminal_MessageReceivedForScripting;
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

		#region Domain > Auxiliary
		//------------------------------------------------------------------------------------------
		// Domain > Auxiliary
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
				c.Add(new Domain.DisplayElement.IOControlInfo(ts, (Domain.Direction)e.Direction, t));
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

			virtualLine.Add(new Domain.DisplayElement.ErrorInfo(ts, (Domain.Direction)e.Direction, e.Message, (e.Severity == Domain.IOErrorSeverity.Acceptable)));

			virtualLine.Add(new Domain.DisplayElement.LineBreak());
			return (virtualLine);
		}

		#endregion

		#region Domain > Event Handlers
		//------------------------------------------------------------------------------------------
		// Domain > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Local field to maintain connection state in order to be able to detect a change of the
		/// connection state.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool terminal_IOChanged_hasBeenConnected;

		private void terminal_IOChanged(object sender, EventArgs<DateTime> e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// Log:
			if (this.log.AnyControlIsOn)
				this.log.WriteLine(IOStatusToDisplayLine(DateTime.Now), Log.LogChannel.Control);

			// Forward:
			OnIOChanged(e);

			bool hasBeenConnected = this.terminal_IOChanged_hasBeenConnected;
			bool isConnectedNow = ((this.terminal != null) ? (this.terminal.IsConnected) : (false));

			if (IsUndisposed) // Attention, the 'OnIOChanged' event raise above could trigger close = disposal of terminal!
			{
				if      ( isConnectedNow && !hasBeenConnected)
				{
					this.activeConnectChrono.Restart(e.Value); // Restart, i.e. reset and start from zero.
					this.totalConnectChrono .Start(  e.Value); // Start again, i.e. continue at last time.

					this.terminal.TimeSpanBase = e.Value;    // The initial time stamp is used for
					                                         // time spans. Consequently, the spans
					if (SettingsRoot.Display.ShowTimeSpan)   // will be based on the active connect
						this.terminal.RefreshRepositories(); // time, not the total connect time.
				}
				else if (!isConnectedNow && hasBeenConnected)
				{
					this.activeConnectChrono.Stop();
					this.totalConnectChrono .Stop();
				}
			}

			this.terminal_IOChanged_hasBeenConnected = isConnectedNow;
		}

		private void terminal_IOControlChanged(object sender, Domain.IOControlEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// Log:
			if (!ICollectionEx.IsNullOrEmpty(e.Texts))
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
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

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

		private void terminal_IsSendingChanged(object sender, EventArgs<bool> e)
		{
			OnIsSendingChanged(e);
		}

		private void terminal_IsSendingForSomeTimeChanged(object sender, EventArgs<bool> e)
		{
			OnIsSendingForSomeTimeChanged(e);
		}

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
		///    will not be used by the view. Instead, the values will synchronously be retrieved when
		///    processing the <see cref="DisplayElementsTxAdded"/>, <see cref="DisplayElementsRxAdded"/>,
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
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Sending...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkSent_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Count/Rate:
			bool rateHasChanged;
		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.txByteCount += e.Value.Content.Count;

				rateHasChanged = this.txByteRate.Update(e.Value.Content.Count, false); // Suppress the 'Changed' event, it will explicitly be raised below,
			}                                                                          // in order to prevent duplicated events via rate_Changed.

			OnIOCountChanged_Promptly(EventArgs.Empty);

			if (rateHasChanged)
			{
				OnIORateChanged_Promptly(EventArgs.Empty);
			}

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
		///    will not be used by the view. Instead, the values will synchronously be retrieved when
		///    processing the <see cref="DisplayElementsTxAdded"/>, <see cref="DisplayElementsRxAdded"/>,
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
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			var currentTickStamp = Stopwatch.GetTimestamp();
			if (currentTickStamp >= this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp)
			{
				OnTimedStatusTextRequest("Receiving...");

				unchecked // Calculate tick stamp of next request:
				{
					this.terminal_RawChunkReceived_nextTimedStatusTextRequestTickStamp = (currentTickStamp + TimedStatusTextRequestTickInterval); // Loop-around is OK.
				}
			}

			// Count/Rate:
			bool rateHasChanged;
		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.rxByteCount += e.Value.Content.Count;

				rateHasChanged = this.rxByteRate.Update(e.Value.Content.Count, false); // Suppress the 'Changed' event, it will explicitly be raised below,
			}                                                                          // in order to prevent duplicated events via rate_Changed.

			OnIOCountChanged_Promptly(EventArgs.Empty);

			if (rateHasChanged)
			{
				OnIORateChanged_Promptly(EventArgs.Empty);
			}

			// Log:
			if (this.log.AnyRawIsOn)
			{
				this.log.Write(e.Value, Log.LogChannel.RawBidir);
				this.log.Write(e.Value, Log.LogChannel.RawRx);
			}

		#if (WITH_SCRIPTING)
			OnRawChunkReceived(e);
		#endif
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsTxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// AutoAction for actions that also apply to Tx (e.g. Plot Count/Rate):
			List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> autoActionTriggers = null; // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoAction.AlsoAppliesToTx)
			{
				EvaluateAutoActionFromElements(e.Elements, DataStatus, SettingsRoot.AutoAction.ShallHighlight, out autoActionTriggers); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			// AutoResponse does never apply to Tx (yet).

			OnDisplayElementsTxAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.

			// AutoAction for actions that also apply to Tx (e.g. Plot Count/Rate):
			if (!ICollectionEx.IsNullOrEmpty(autoActionTriggers))
			{
				EnqueueAutoActions(autoActionTriggers);
			}

			// AutoResponse does never apply to Tx (yet).
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsBidirAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// AutoAction highlighting:                                              // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoAction.IsByteSequenceTriggered && // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			    SettingsRoot.AutoAction.IsNeitherFilterNorSuppress) // Filter/Suppress is limited to be processed in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			{
				EvaluateAutoActionFromElements(e.Elements, DataStatus, SettingsRoot.AutoAction.ShallHighlight); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			// AutoResponse highlighting:                                                // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoResponse.IsByteSequenceTriggered) // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			{
				EvaluateAutoResponseFromElements(e.Elements); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			OnDisplayElementsBidirAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsRxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// AutoAction:
			List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> autoActionTriggers = null; // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoAction.IsByteSequenceTriggered && // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			    SettingsRoot.AutoAction.IsNeitherFilterNorSuppress) // Filter/Suppress is limited to be processed in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			{
				EvaluateAutoActionFromElements(e.Elements, DataStatus, SettingsRoot.AutoAction.ShallHighlight, out autoActionTriggers); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			// AutoResponse:
			List<Tuple<byte[], string, MatchCollection>> autoResponseTriggers = null;   // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoResponse.IsByteSequenceTriggered) // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
			{
				EvaluateAutoResponseFromElements(e.Elements, out autoResponseTriggers); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			OnDisplayElementsRxAdded(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.

			// AutoAction:
			if (!ICollectionEx.IsNullOrEmpty(autoActionTriggers))
			{
				EnqueueAutoActions(autoActionTriggers);
			}

			// AutoResponse:
			if (!ICollectionEx.IsNullOrEmpty(autoResponseTriggers))
			{
				EnqueueAutoResponses(autoResponseTriggers);
			}
		}

		// Initially (2019-04..11 / YAT 2.1.0) the trigger detection was implemented per chunk, resulting in:
		//  > If trigger was located in a single chunk, all fine, as long as the chunk did not spread across multiple lines.
		//  > If trigger was spread across multiple chunks, all fine, also as long as the chunks do not spread across multiple lines.
		//  > If there was more than one trigger in a chunk, or last byte of one trigger and another complete one, only a single trigger was detected.
		//  > No way to trigger for text.
		//
		// Potential approaches to overcome these limitations:
		//  a) Move trigger detection from <see cref="terminal_RawChunkReceived"/> to one of the underlying methods of
		//     <see cref="Domain.Terminal.ProcessAndSignalRawChunk"/>, where the chunks are being processed into lines.
		//  b) Keep trigger detection in model but move it from <see cref="terminal_RawChunkReceived"/> to a new
		//     'CurrentDisplayLineRxChanged' event. That event would have to include the current display line part (for
		//     text triggering) as well as the changed part (for byte sequence triggering).
		//     Advantages of this approach:
		//      > Keep settings complexity in model.
		//      > Keep well-defined interface (former 'LineChunkAttribute') among model and domain.
		//  c) Completely move handling to model, possible as long as using the retaining approach:
		//      > Byte sequence based triggering implemented in <see cref="terminal_DisplayElementsRxAdded"/> (immediate approach / element update mode).
		//      > Text based triggering implemented in <see cref="terminal_DisplayLinesRxAdded"/> (retaining approach / line update mode).
		//     Advantages of this approach:
		//      > Text based triggering possible.
		//      > Keep <see cref="Domain.Terminal"/> as simple as possible, really.
		//      > No additional interface (former 'LineChunkAttribute') among model and domain needed anymore.
		//     Disadvantage of this approach:
		//      > Elements/Lines cannot already be suppressed/filtered by <see cref="Domain.Terminal"/>.
		//
		// In addition:
		//  > Refine trigger detection such it detects multiple triggers. This can easily be achieved by upgrading the
		//    'isTriggered' flag to a 'triggerCount' value.
		//
		// There are additional ideas to further let these features evolve:
		//  > Move filter/suppress to separate options:
		//     + They not really are "actions" and are implemented partly differently then the true actions.
		//     + Possibility to filter and suppress in parallel.
		//     + Possibility to filter and suppress Tx as well.
		//     - Further (over)loads the main tool bar.
		//     - Lots of duplicated code.
		//  > Provide n automatic actions:
		//     + Possibility to filter/suppress/other in parallel.
		//     - Further complicates the main tool bar.
		//     - Further complicates usage.
		// These ideas are technically possible but are considered (2019-11-21..22 / YAT 2.1.1) over the top.
		//  > Some inconsistency for filter/suppress is considered preferable over making things more complicated.
		//  > Limited to Rx is considered sufficient.
		//
		// Approach c) was chosen (2019-11-21..22 / YAT 2.1.1).
		//
		// The last state of the initial implementation can be found in SVN revisions #2701..#2707.

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineTxReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineBidirReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineRxReplaced(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineTxCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineBidirCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnCurrentDisplayLineRxCleared(e);

			// Logging is only triggered by the 'DisplayLines[Tx|Bidir|Rx]Added' events and thus does not need to be handled here.
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesTxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// Count:
			this.txLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.txLineRate.Update(e.Lines.Count, false)) // Suppress the 'Changed' event, it will explicitly be raised below,
				OnIORateChanged_Promptly(EventArgs.Empty);    // in order to prevent duplicated events via rate_Changed.

			// Display:
			OnDisplayLinesTxAdded(e);

			// Log:
			if (this.log.NeatTxIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatTx);
			}

			// AutoAction:
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoAction.AlsoAppliesToTx)
			{
				foreach (var dl in e.Lines)
					EnqueueAutoAction(dl.TimeStamp, dl.Text, null, DataStatus);
			}

			// AutoResponse does never apply to Tx (yet).
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesBidirAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

		////// Count:
		////this.bidirLineCount += e.Lines.Count would technically be possible, but doesn't make much sense.
		////OnIOCountChanged_Promptly(EventArgs.Empty);

		////// Rate:
		////if (this.bidirLineRate.Update(e.Lines.Count)) would technically be possible, but doesn't make much sense.
		////	OnIORateChanged_Promptly(EventArgs.Empty);

			// AutoAction:                                                           // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine))
			{
				if (SettingsRoot.AutoAction.IsTextTriggered && // Byte sequence based triggering is evaluated in terminal_DisplayElements[Bidir|Rx]Added.
				    SettingsRoot.AutoAction.IsNeitherFilterNorSuppress)
				{
					EvaluateAutoActionOtherThanFilterOrSuppressFromLines(e.Lines, DataStatus, SettingsRoot.AutoAction.ShallHighlight); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
				}
				else if (SettingsRoot.AutoAction.IsFilterOrSuppress) // Filter/Suppress incl. 'IsByteSequenceTriggered' is processed here.
				{
					ProcessAutoActionFilterAndSuppressFromLines(e.Lines); // Must be done before forward raising the event, because this method will recreate the display lines.
				}
			}

			// AutoResponse:                                                             // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoResponse.IsTextTriggered) // Byte sequence based triggering is evaluated in terminal_DisplayElements[Bidir|Rx]Added.
			{
				EvaluateAutoResponseFromLines(e.Lines); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			// Display:
			OnDisplayLinesBidirAdded(e);

			// Log:
			if (this.log.NeatBidirIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatBidir);
			}
		}

		/// <remarks>This 'normal' event is not raised during reloading, 'Repository[Rx|Bidir|Tx]Reloaded' events event will be raised after completion.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesRxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// Count:
			this.rxLineCount += e.Lines.Count;
			OnIOCountChanged_Promptly(EventArgs.Empty);

			// Rate:
			if (this.rxLineRate.Update(e.Lines.Count, false)) // Suppress the 'Changed' event, it will explicitly be raised below,
				OnIORateChanged_Promptly(EventArgs.Empty);    // in order to prevent duplicated events via rate_Changed.

			// AutoAction:
			List<Tuple<DateTime, string, MatchCollection, CountsRatesTuple>> autoActionTriggers = null; // See [== AutoTrigger.AnyLine] below.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine))
			{
				if (SettingsRoot.AutoAction.IsTextTriggered && // Byte sequence based triggering is evaluated in terminal_DisplayElements[Bidir|Rx]Added.
				    SettingsRoot.AutoAction.IsNeitherFilterNorSuppress)
				{
					EvaluateAutoActionOtherThanFilterOrSuppressFromLines(e.Lines, DataStatus, SettingsRoot.AutoAction.ShallHighlight, out autoActionTriggers); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
				}
				else if (SettingsRoot.AutoAction.IsFilterOrSuppress) // Filter/Suppress incl. 'IsByteSequenceTriggered' is processed here.
				{
					ProcessAutoActionFilterAndSuppressFromLines(e.Lines); // Must be done before forward raising the event, because this method will recreate the display lines.
				}
			}

			// AutoResponse:
			List<Tuple<byte[], string, MatchCollection>> autoResponseTriggers = null;   // See [== AutoTrigger.AnyLine] below.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoResponse.IsTextTriggered) // Byte sequence based triggering is evaluated in terminal_DisplayElements[Bidir|Rx]Added.
			{
				EvaluateAutoResponseFromLines(e.Lines, out autoResponseTriggers); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			// Display:
			OnDisplayLinesRxAdded(e);

			// Log:
			if (this.log.NeatRxIsOn)
			{
				foreach (var dl in e.Lines)
					this.log.WriteLine(dl, Log.LogChannel.NeatRx);
			}

			// AutoAction:
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines)
					EnqueueAutoAction(dl.TimeStamp, dl.Text, null, DataStatus);

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highlighted.
				//
				// Filtering with [Trigger == AnyLine] is possible but has no effect.
				// Suppressing with [Trigger == AnyLine] is prohibitied.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesRxAdded' event, as the monitors already get updated
				// in the 'ElementsRxAdded' event further above.
			}
			else if (!ICollectionEx.IsNullOrEmpty(autoActionTriggers))
			{
				EnqueueAutoActions(autoActionTriggers);
			}

			// AutoResponse:
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger == AutoTrigger.AnyLine))
			{
				foreach (var dl in e.Lines) // Response must always be based on origin, not text, since text could be formatted
					EnqueueAutoResponse(ToOriginWithoutRxEol(dl), null, null); // not suitable for sending, e.g. in binary radix.

				// Note that trigger line is not highlighted if [Trigger == AnyLine] since that
				// would result in all received lines highlighted.
				//
				// Also note that implementation wouldn't be that simple, since "e.Highlight = true"
				// doesn't help in this 'LinesRxAdded' event, as the monitors already get updated
				// in the 'ElementsRxAdded' event further above.
			}
			else if (!ICollectionEx.IsNullOrEmpty(autoResponseTriggers))
			{
				EnqueueAutoResponses(autoResponseTriggers);
			}
		}

	#if (WITH_SCRIPTING)

		private void terminal_ReceivingPacketForScripting(object sender, Domain.ModifiablePacketEventArgs e)
		{
			OnReceivingPacketForScripting(e);
		}

		private void terminal_MessageReceivedForScripting(object sender, Domain.ScriptMessageEventArgs e)
		{
			OnMessageReceivedForScripting(e);
		}

	#endif // WITH_SCRIPTING

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnRepositoryTxCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnRepositoryBidirCleared(e);
		}

		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxCleared(object sender, EventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnRepositoryRxCleared(e);
		}

		/// <remarks>Separated from <see cref="terminal_DisplayLinesTxAdded"/> for not processing count/rate/log... on reload again.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnRepositoryTxReloaded(e);
		}

		/// <remarks>Separated from <see cref="terminal_DisplayLinesBidirAdded"/> for not processing count/rate/log... on reload again.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// AutoAction:                                                           // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine))
			{
				if (SettingsRoot.AutoAction.IsNeitherFilterNorSuppress) // Highlighting is evaluated here.
				{
					EvaluateAutoActionOtherThanFilterOrSuppressFromLines(e.Lines, DataStatus, SettingsRoot.AutoAction.ShallHighlight); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
				}
				else if (SettingsRoot.AutoAction.IsFilterOrSuppress) // Filter/Suppress is processed here.
				{
					ProcessAutoActionFilterAndSuppressFromLines(e.Lines); // Must be done before forward raising the event, because this method will recreate the display lines.
				}
			}

			// AutoResponse:                                                             // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine))
			{
				EvaluateAutoResponseFromLines(e.Lines); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			OnRepositoryBidirReloaded(e);
		}

		/// <remarks>Separated from <see cref="terminal_DisplayLinesRxAdded"/> for not processing count/rate/log... on reload again.</remarks>
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			// AutoAction:                                                           // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine))
			{
				if (SettingsRoot.AutoAction.IsNeitherFilterNorSuppress) // Highlighting is evaluated here.
				{
					EvaluateAutoActionOtherThanFilterOrSuppressFromLines(e.Lines, DataStatus, SettingsRoot.AutoAction.ShallHighlight); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
				}
				else if (SettingsRoot.AutoAction.IsFilterOrSuppress) // Filter/Suppress is processed here.
				{
					ProcessAutoActionFilterAndSuppressFromLines(e.Lines); // Must be done before forward raising the event, because this method will recreate the display lines.
				}
			}

			// AutoResponse:                                                             // See terminal_DisplayLinesRxAdded for background.
			if (SettingsRoot.AutoResponse.IsActive && (SettingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine))
			{
				EvaluateAutoResponseFromLines(e.Lines); // Must be done before forward raising the event, because this method may activate 'Highlight' on one or multiple elements.
			}

			OnRepositoryRxReloaded(e);
		}

		#endregion

		#region Domain > Check I/O
		//------------------------------------------------------------------------------------------
		// Domain > Check I/O
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
			switch (SettingsRoot.IOType)
			{
				case Domain.IOType.SerialPort:
				{
					var portId = SettingsRoot.Terminal.IO.SerialPort.PortId;
					if (portId != null)
						return (CheckSerialPortAvailability(portId));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					MKY.Net.IPNetworkInterfaceEx localInterface = SettingsRoot.Terminal.IO.Socket.LocalInterface;
					if (localInterface != null)
						return (CheckLocalInterfaceAvailability(localInterface));
					else
						return (CheckResult.Ignore);
				}

				case Domain.IOType.UsbSerialHid:
				{
					var deviceInfo = SettingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo;
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
								SettingsRoot.Explicit.Terminal.IO.SerialPort.PortId = portIdAlternate;
								ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
							SettingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
							ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
						SettingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[sameDescriptionIndex];
						ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
							SettingsRoot.Explicit.Terminal.IO.Socket.LocalInterface = localInterfaces[0];
							ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
								SettingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidSerialIndex];
								ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
								SettingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
								ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.
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
						bool hadAlreadyBeenChanged = SettingsRoot.Terminal.IO.UsbSerialHidDevice.HaveChanged;
						SettingsRoot.Terminal.IO.UsbSerialHidDevice.DeviceInfo = devices[sameVidPidIndex];
						if (!hadAlreadyBeenChanged)
							SettingsRoot.Explicit.Terminal.IO.UsbSerialHidDevice.ClearChanged();

						ApplyTerminalSettings(SettingsRoot.Explicit); // \ToDo: Not a good solution, should be called in HandleTerminalSettings(), but that gets called too often => FR #309.

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
				"Start anyway?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2
			);

			return (dr);
		}

		private DialogResult ShowUsbSerialHidDeviceSerialNotAvailableAlternateQuestionYesNo(string deviceInfoNotAvailable, string deviceInfoAlternate)
		{
			string message =
				"The previous USB HID device '" + deviceInfoNotAvailable + "' is currently not available." + Environment.NewLine + Environment.NewLine +
				"Switch to '" + deviceInfoAlternate + "' (first available device with same VID and PID) instead?";

			var dr = OnMessageInputRequest
			(
				message,
				"Switch device?",
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

		#region Domain > Start/Stop
		//------------------------------------------------------------------------------------------
		// Domain > Start/Stop
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Starts the terminal.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public virtual bool Start()
		{
			string messageOnFailure;
			return (Start(out messageOnFailure));
		}

		/// <summary>
		/// Starts the terminal.
		/// </summary>
		/// <param name="messageOnFailure">Message used for scripting.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool Start(out string messageOnFailure)
		{
			return (Start(true, out messageOnFailure));
		}

		/// <summary>
		/// Starts the terminal.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		private bool Start(bool saveStatus)
		{
			string messageOnFailure;
			return (Start(saveStatus, out messageOnFailure));
		}

		/// <summary>
		/// Starts the terminal.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <param name="messageOnFailure">Message used for scripting.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool Start(bool saveStatus, out string messageOnFailure)
		{
			bool success = false;

			OnFixedStatusTextRequest("Starting...");
			try
			{
				if (this.terminal.Start())
				{
					if (saveStatus)
						SettingsRoot.TerminalIsStarted = this.terminal.IsStarted;

					OnTimedStatusTextRequest("Successfully started.");
					messageOnFailure = null;
					success = true;
				}
				else
				{
					messageOnFailure = string.Format(CultureInfo.CurrentCulture, "Terminal on '{0}' could not be started!", this.terminal.ToShortIOString());
					OnFixedStatusTextRequest(messageOnFailure);

					if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
					{
						string yatLead, yatText;
						MakeStartHint(out yatLead, out yatText);

						OnMessageInputRequest
						(                                            // Needed to disabmbiguate.
							Utilities.MessageHelper.ComposeMessage(messageOnFailure, string.Empty, yatLead, yatText),
							"Terminal Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
						);
					}
				}
			}
			catch (Exception ex)
			{
				messageOnFailure = "Error on starting terminal!";
				OnFixedStatusTextRequest(messageOnFailure);

				if (ApplicationSettings.LocalUserSettings.General.NotifyNonAvailableIO)
				{
					string yatLead, yatText;
					MakeExceptionHint(out yatLead, out yatText);

					messageOnFailure = Utilities.MessageHelper.ComposeMessage(messageOnFailure, ex, yatLead, yatText);
					OnMessageInputRequest
					(
						messageOnFailure,
						"Terminal Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}

			return (success);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		protected virtual void MakeStartHint(out string yatLead, out string yatText)
		{
			var ioType = SettingsRoot.IOType;

			switch (ioType)
			{
				case Domain.IOType.SerialPort:
				{
					Utilities.MessageHelper.MakeSerialPortStartHint(out yatLead, out yatText);
					break;
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.UdpClient:
				{
					Utilities.MessageHelper.MakeIPClientStartHint(out yatLead, out yatText);
					break;
				}

				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					Utilities.MessageHelper.MakeTcpListenerHint(SettingsRoot.IO.Socket.LocalTcpPort, out yatLead, out yatText);
					break;
				}

				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
				{
					Utilities.MessageHelper.MakeUdpListenerHint(SettingsRoot.IO.Socket.LocalUdpPort, out yatLead, out yatText);
					break;
				}

				case Domain.IOType.UsbSerialHid:
				{
					Utilities.MessageHelper.MakeUsbSerialHidStartHint(out yatLead, out yatText);
					break;
				}

				default:
				{
					throw (new NotSupportedException(MKY.MessageHelper.InvalidExecutionPreamble + "'" + ioType + "' is an I/O type that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "yat", Justification = "YAT is YAT, guys!")]
		protected virtual void MakeExceptionHint(out string yatLead, out string yatText)
		{
			var ioType = SettingsRoot.IOType;

			switch (ioType)
			{
				case Domain.IOType.SerialPort:
				{
					Utilities.MessageHelper.MakeSerialPortExceptionHint(out yatLead, out yatText);
					break;
				}

				case Domain.IOType.TcpClient:
				case Domain.IOType.UdpClient:
				{
					Utilities.MessageHelper.MakeIPClientExceptionHint(out yatLead, out yatText);
					break;
				}

				case Domain.IOType.TcpServer:
				case Domain.IOType.TcpAutoSocket:
				{
					Utilities.MessageHelper.MakeTcpListenerHint(SettingsRoot.IO.Socket.LocalTcpPort, out yatLead, out yatText);
					break;
				}

				case Domain.IOType.UdpServer:
				case Domain.IOType.UdpPairSocket:
				{
					Utilities.MessageHelper.MakeUdpListenerHint(SettingsRoot.IO.Socket.LocalUdpPort, out yatLead, out yatText);
					break;
				}

				case Domain.IOType.UsbSerialHid:
				{
					Utilities.MessageHelper.MakeUsbSerialHidExceptionHint(out yatLead, out yatText);
					break;
				}

				default:
				{
					throw (new NotSupportedException(MKY.MessageHelper.InvalidExecutionPreamble + "'" + ioType + "' is an I/O type that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary>
		/// Stops the terminal.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual bool Stop()
		{
			string messageOnFailure;
			return (Stop(out messageOnFailure));
		}

		/// <summary>
		/// Stops the terminal.
		/// </summary>
		/// <param name="messageOnFailure">Message used for scripting.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual bool Stop(out string messageOnFailure)
		{
			return (Stop(true, out messageOnFailure));
		}

		/// <summary>
		/// Stops the terminal.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		private bool Stop(bool saveStatus)
		{
			string messageOnFailure;
			return (Stop(saveStatus, out messageOnFailure));
		}

		/// <summary>
		/// Stops the terminal.
		/// </summary>
		/// <param name="saveStatus">Flag indicating whether status of terminal shall be saved.</param>
		/// <param name="messageOnFailure">Message used for scripting.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		private bool Stop(bool saveStatus, out string messageOnFailure)
		{
			bool success = false;

			OnFixedStatusTextRequest("Stopping terminal...");
			try
			{
				this.terminal.Stop();

				if (saveStatus)
					SettingsRoot.TerminalIsStarted = this.terminal.IsStarted;

				OnTimedStatusTextRequest("Terminal stopped.");
				messageOnFailure = null;
				success = true;
			}
			catch (Exception ex)
			{
				messageOnFailure = "Error on stopping terminal!";
				OnTimedStatusTextRequest(messageOnFailure);
				messageOnFailure = "Error on stopping terminal:" + Environment.NewLine + Environment.NewLine + ex.Message;
				OnMessageInputRequest
				(
					messageOnFailure,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				OnTimedStatusTextRequest("Terminal not stopped!");
			}

			return (success);
		}

		#endregion

		#region Domain > Repositories
		//------------------------------------------------------------------------------------------
		// Domain > Repositories
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Clears given repository.
		/// </summary>
		public virtual void ClearRepository(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Clearing...");

			if (this.terminal.ClearRepository(repositoryType))
				OnResetStatusTextRequest(); // Simply reset, no need for "Cleared" or the like.
			else
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Clears all repositories.
		/// </summary>
		public virtual void ClearRepositories()
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Clearing...");

			if (this.terminal.ClearRepositories())
				OnResetStatusTextRequest(); // Simply reset, no need for "Cleared" or the like.
			else
				OnTimedStatusTextRequest("Clear request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of repositories.
		/// </summary>
		public virtual void RefreshRepositories()
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Refreshing...");

			if (this.terminal.RefreshRepositories())
				OnResetStatusTextRequest(); // Simply reset, no need for "Refreshed" or the like.
			else
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Forces complete refresh of given repository.
		/// </summary>
		public virtual void RefreshRepository(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			OnFixedStatusTextRequest("Refreshing...");

			if (this.terminal.RefreshRepository(repositoryType))
				OnResetStatusTextRequest(); // Simply reset, no need for "Refreshed" or the like.
			else
				OnTimedStatusTextRequest("Refresh request has timed out");
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayElementCollection RepositoryToDisplayElements(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.RepositoryToDisplayElements(repositoryType));
		}

		/// <summary>
		/// Returns contents of desired repository.
		/// </summary>
		public virtual Domain.DisplayLineCollection RepositoryToDisplayLines(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.RepositoryToDisplayLines(repositoryType));
		}

		/// <summary>
		/// Returns the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual Domain.DisplayLine LastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.LastDisplayLineAuxiliary(repositoryType));
		}

		/// <summary>
		/// Clears the last display line of desired repository for auxiliary purposes (e.g. automated testing).
		/// </summary>
		public virtual void ClearLastDisplayLineAuxiliary(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			this.terminal.ClearLastDisplayLineAuxiliary(repositoryType);
		}

		/// <summary>
		/// Returns contents of desired repository as string.
		/// </summary>
		public virtual string RepositoryToExtendedDiagnosticsString(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.RepositoryToExtendedDiagnosticsString(repositoryType));
		}

		#endregion

		#region Domain > Format
		//------------------------------------------------------------------------------------------
		// Domain > Format
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Formats the given data into a string, same as done by the monitor view.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// <see cref="Domain.Settings.DisplaySettings.TxRadix"/> and <see cref="Domain.Settings.DisplaySettings.RxRadix"/> have different values.
		/// </exception>
		public virtual string Format(byte[] data)
		{
			AssertUndisposed();
			return (this.terminal.Format(data));
		}

		/// <summary>
		/// Formats the given data into a string, same as done by the monitor view.
		/// </summary>
		public virtual string Format(byte[] data, Domain.IODirection direction)
		{
			AssertUndisposed();
			return (this.terminal.Format(data, direction));
		}

		/// <summary>
		/// Formats the given data into a string, same as done by the monitor view.
		/// </summary>
		public virtual string Format(byte[] data, Domain.Radix radix)
		{
			AssertUndisposed();
			return (this.terminal.Format(data, radix));
		}

		#endregion

		#region Domain > Change
		//------------------------------------------------------------------------------------------
		// Domain > Change
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// The Tx and Rx sequence(s) have different values.
		/// </exception>
		public virtual void RemoveFraming(byte[] data)
		{
			AssertUndisposed();
			this.terminal.RemoveFraming(data);
		}

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		public virtual void RemoveFraming(byte[] data, Domain.IODirection direction)
		{
			AssertUndisposed();
			this.terminal.RemoveFraming(data, direction);
		}

		#endregion

		#region Domain > Time Status
		//------------------------------------------------------------------------------------------
		// Domain > Time Status
		//------------------------------------------------------------------------------------------

		private void CreateChronos()
		{
			this.activeConnectChrono = new Chronometer();
			this.activeConnectChrono.Interval = 1000;
		////this.activeConnectChrono.TimeSpanChanged shall not be used, events are invoked by 'totalConnectChrono' below.
			this.activeConnectChrono.DiagnosticsName = string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}::ActiveConnect", SequentialId);

			this.totalConnectChrono = new Chronometer();
			this.totalConnectChrono.Interval = 1000;
			this.totalConnectChrono.TimeSpanChanged += totalConnectChrono_TimeSpanChanged;
			this.totalConnectChrono.DiagnosticsName = string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}::TotalConnect", SequentialId);
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
				AssertUndisposed();

				return (this.activeConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		public virtual TimeSpan TotalConnectTime
		{
			get
			{
				AssertUndisposed();

				return (this.totalConnectChrono.TimeSpan);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual void GetConnectTime(out TimeSpan activeConnectTime, out TimeSpan totalConnectTime)
		{
			AssertUndisposed();

			activeConnectTime = this.activeConnectChrono.TimeSpan;
			totalConnectTime  = this.totalConnectChrono.TimeSpan;
		}

		/// <summary></summary>
		public virtual void ResetConnectTime()
		{
			AssertUndisposed();

			var now = DateTime.Now; // Ensure that all use exactly the same instant.

			this.activeConnectChrono.Reset(now);
			this.totalConnectChrono .Reset(now);

			this.terminal.TimeSpanBase = now;

			if (SettingsRoot.Display.ShowTimeSpan)
				this.terminal.RefreshRepositories();
		}

		private void totalConnectChrono_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			OnIOConnectTimeChanged(e);
		}

		#endregion

		#region Domain > Data Status
		//------------------------------------------------------------------------------------------
		// Domain > Data Status
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
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
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
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
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
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
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
	////		AssertUndisposed();
	////
	////		lock (this.countsRatesSyncObj)
	////			return (this.bidirLineCount) would technically be possible, but doesn't make much sense.
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
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
					return (this.rxLineCount);
			}
		}

		/// <summary>
		/// The send rate in bytes per second.
		/// </summary>
		/// <remarks>
		/// The value corresponds to the rate of the raw terminal repository.
		/// The rate of the formatted terminal repository slightly lags behind.
		/// </remarks>
		public virtual int TxByteRate
		{
			get
			{
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
					return (this.txByteRate.RateValue * RateIntervalsPerSecond);
			}
		}

		/// <summary>
		/// The receive rate in bytes per second.
		/// </summary>
		/// <remarks>
		/// The value corresponds to the rate of the raw terminal repository.
		/// The rate of the formatted terminal repository slightly lags behind.
		/// </remarks>
		public virtual int RxByteRate
		{
			get
			{
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
					return (this.rxByteRate.RateValue * RateIntervalsPerSecond);
			}
		}

		/// <summary>
		/// The send rate in lines per second.
		/// </summary>
		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// </remarks>
		public virtual int TxLineRate
		{
			get
			{
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
					return (this.txLineRate.RateValue * RateIntervalsPerSecond);
			}
		}

	/////// <summary>
	/////// The bidir rate in lines per second.
	/////// </summary>
	/////// <remarks>
	/////// The value corresponds to the completed line count of the formatted terminal repository.
	/////// </remarks>
	////public virtual int BidirLineRate
	////{
	////	get
	////	{
	////		AssertUndisposed();
	////
	////		lock (this.countsRatesSyncObj)
	////			return (this.bidirLineRate.RateValue * RateIntervalsPerSecond) would technically be possible, but doesn't make much sense.
	////	}
	////}

		/// <summary>
		/// The receive rate in lines per second.
		/// </summary>
		/// <remarks>
		/// The value corresponds to the completed line count of the formatted terminal repository.
		/// </remarks>
		public virtual int RxLineRate
		{
			get
			{
				AssertUndisposed();

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
					return (this.rxLineRate.RateValue * RateIntervalsPerSecond);
			}
		}

		/// <remarks>
		/// See remarks of <see cref="TxByteCount"/> and <see cref="TxLineCount"/>, <see cref="RxByteCount"/> and <see cref="RxLineCount"/>,
		/// <see cref="TxByteRate"/> and <see cref="TxLineRate"/>, <see cref="RxByteRate"/> and <see cref="RxLineRate"/>.
		/// </remarks>
		public virtual CountsRatesTuple DataStatus
		{
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Improved readability as right underneath.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Improved readability as right underneath.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Improved readability as right underneath.")]
			get
			{
				AssertUndisposed();

				BytesLinesTuple counts, rates;

			////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
				{
					counts = new BytesLinesTuple(this.txByteCount,                                   this.txLineCount,
					                             this.rxByteCount,                                   this.rxLineCount);
					rates  = new BytesLinesTuple(this.txByteRate.RateValue * RateIntervalsPerSecond, this.txLineRate.RateValue * RateIntervalsPerSecond,
					                             this.rxByteRate.RateValue * RateIntervalsPerSecond, this.rxLineRate.RateValue * RateIntervalsPerSecond);
				}

				return (new CountsRatesTuple(counts, rates));
			}
		}

		/// <summary></summary>
		public virtual void ResetCountAndRate()
		{
			AssertUndisposed();

		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.txByteCount    = 0;
				this.rxByteCount    = 0;

				this.txLineCount    = 0;
			////this.bidirLineCount = 0 would technically be possible, but doesn't make much sense.
				this.rxLineCount    = 0;

				this.txByteRate   .Reset(false); // Suppress the 'Changed' event, it will explicitly be raised below,
				this.rxByteRate   .Reset(false); // in order to prevent duplicated events via rate_Changed.

				this.txLineRate   .Reset(false); // See above.
			////this.bidirLineRate.Reset(false) would technically be possible, but doesn't make much sense.
				this.rxLineRate   .Reset(false); // See above.
			}

			OnIOCountChanged_Promptly(EventArgs.Empty);
			OnIORateChanged_Promptly( EventArgs.Empty);
			OnIORateChanged_Decimated(EventArgs.Empty);
		}

		private void CreateRates()
		{
			const int RateInterval   = (1000 / RateIntervalsPerSecond); // Resulting in 250 ms intervals.
			const int RateWindow     =  2000; // Note that rate may drop to 0 during sending of files or even "jump" to high values at the end.
			const int UpdateInterval =   250; // FR #375 "migrate Byte/Line Count/Rate from model to domain" will fix this.

		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.txByteRate    = new RateProvider(RateInterval, RateWindow, UpdateInterval, string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}.TxByte", SequentialId));
				this.rxByteRate    = new RateProvider(RateInterval, RateWindow, UpdateInterval, string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}.RxByte", SequentialId));

				this.txLineRate    = new RateProvider(RateInterval, RateWindow, UpdateInterval, string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}.TxLine", SequentialId));
			////this.bidirLineRate = new RateProvider(RateInterval, RateWindow, UpdateInterval) would technically be possible, but doesn't make much sense.
				this.rxLineRate    = new RateProvider(RateInterval, RateWindow, UpdateInterval, string.Format(CultureInfo.CurrentCulture, "Terminal #{0:D2}.RxLine", SequentialId));

				this.txByteRate   .Changed += rate_Changed;
				this.rxByteRate   .Changed += rate_Changed;

				this.txLineRate   .Changed += rate_Changed;
			////this.bidirLineRate.Changed += rate_Changed would technically be possible, but doesn't make much sense.
				this.rxLineRate   .Changed += rate_Changed;
			}
		}

		private void StartRates()
		{
		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.txByteRate   .Start();
				this.rxByteRate   .Start();

				this.txLineRate   .Start();
			////this.bidirLineRate.Start() would technically be possible, but doesn't make much sense.
				this.rxLineRate   .Start();
			}
		}

		private void StopRates()
		{
		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
			{
				this.txByteRate   .Stop();
				this.rxByteRate   .Stop();

				this.txLineRate   .Stop();
			////this.bidirLineRate.Stop() would technically be possible, but doesn't make much sense.
				this.rxLineRate   .Stop();
			}
		}

		private void DisposeRates()
		{
		////lock (this.countsRatesSyncObj) \remind (MKY / 2020-01-10) doesn't work (yet) as changing rates invokes events leading to synchronization deadlocks.
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
		}

		private void rate_Changed(object sender, RateEventArgs e)
		{
			if (IsInDisposal) // Ensure to not handle event during closing anymore.
				return;

			OnIORateChanged_Promptly(e);
			OnIORateChanged_Decimated(e);

			// Note that this event handler will not be called when the 'Changed' event is locally
			// suppressed, as done at several locations, in order to prevent duplicated events.

			if (SettingsRoot.AutoAction.IsActive && (SettingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine) &&
			    SettingsRoot.AutoAction.IsCountRatePlot)
			{
				EnqueueAutoAction(DateTime.Now, null, null, DataStatus); // Needed to plot gradual decrease of rate.
			}
		}

		/// <remarks>
		/// The value corresponds to the byte count of the formatted terminal repository.
		/// </remarks>
		public virtual int GetRepositoryByteCount(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.GetRepositoryByteCount(repositoryType));
		}

		/// <remarks>
		/// The value corresponds to the line count of the formatted terminal repository.
		/// </remarks>
		public virtual int GetRepositoryLineCount(Domain.RepositoryType repositoryType)
		{
			AssertUndisposed();

			return (this.terminal.GetRepositoryLineCount(repositoryType));
		}

		#endregion

		#region Domain > Flow Control and Break Status
		//------------------------------------------------------------------------------------------
		// Domain > Flow Control and Break Status
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Serial port control pins.
		/// </summary>
		public virtual MKY.IO.Ports.SerialPortControlPins SerialPortControlPins
		{
			get
			{
				AssertUndisposed();

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
				AssertUndisposed();

				return (this.terminal.SerialPortControlPinCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOnCount
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.SentXOnCount);
			}
		}

		/// <summary></summary>
		public virtual int SentXOffCount
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.SentXOffCount);
			}
		}

		/// <summary></summary>
		public virtual int ReceivedXOnCount
		{
			get
			{
				AssertUndisposed();

			#if (!WITH_SCRIPTING)
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
				AssertUndisposed();

			#if (!WITH_SCRIPTING)
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
			AssertUndisposed();

		#if (!WITH_SCRIPTING)
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
				AssertUndisposed();

				return (this.terminal.InputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual int OutputBreakCount
		{
			get
			{
				AssertUndisposed();

				return (this.terminal.OutputBreakCount);
			}
		}

		/// <summary></summary>
		public virtual void ResetBreakCount()
		{
			AssertUndisposed();

			this.terminal.ResetBreakCount();
		}

		#endregion

		#region Domain > I/O Control
		//------------------------------------------------------------------------------------------
		// Domain > I/O Control
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
			AssertUndisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleRts(out pinState);
			SettingsRoot.IO.SerialPort.Communication.RtsPin = pinState;
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
			AssertUndisposed();

			MKY.IO.Serial.SerialPort.SerialControlPinState pinState;
			bool isSuccess = this.terminal.RequestToggleDtr(out pinState);
			SettingsRoot.IO.SerialPort.Communication.DtrPin = pinState;
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
			AssertUndisposed();

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
			AssertUndisposed();

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
				SettingsRoot.LogIsOn = true;

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				Utilities.MessageHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					Utilities.MessageHelper.ComposeMessage("Unable to switch log on!", ex, yatLead, yatText),
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
				Utilities.MessageHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					Utilities.MessageHelper.ComposeMessage("Unable to clear log!", ex, yatLead, yatText),
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
				SettingsRoot.LogIsOn = false;

				return (true);
			}
			catch (Exception ex)
			{
				string yatLead, yatText;
				Utilities.MessageHelper.MakeLogHint(this.log, out yatLead, out yatText);

				OnMessageInputRequest
				(
					Utilities.MessageHelper.ComposeMessage("Unable to switch log off!", ex, yatLead, yatText),
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <summary></summary>
		public virtual bool ToggleLogOnOrOff()
		{
			if (this.log.AnyIsOn)
				return (SwitchLogOff());
			else
				return (SwitchLogOn());
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
							Utilities.MessageHelper.ComposeMessage("Unable to open log file", filePath, ex),
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
						Utilities.MessageHelper.ComposeMessage("Unable to open log folder", rootPath, exBrowse),
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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnIOChanged(EventArgs<DateTime> e)
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
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		protected virtual void OnIOCountChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IOCountChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		protected virtual void OnIORateChanged_Promptly(EventArgs e)
		{
			this.eventHelper.RaiseSync(IORateChanged_Promptly, this, e);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
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
		/// This is similar to the behavior of e.g. the 'Validating' event of WinForms controls.
		/// </remarks>
		/// <remarks>
		/// This event is raised when a packet is send by the <see cref="Domain.Terminal"/>.
		/// This plug-in event is not raised during reloading.
		/// </remarks>
		protected virtual void OnSendingPacket(Domain.ModifiablePacketEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.ModifiablePacketEventArgs>(SendingPacket, this, e);
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void OnIsSendingChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(IsSendingChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnIsSendingForSomeTimeChanged(EventArgs<bool> e)
		{
			this.eventHelper.RaiseSync<EventArgs<bool>>(IsSendingForSomeTimeChanged, this, e);
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		protected virtual void OnRawChunkSent(EventArgs<Domain.RawChunk> e)
		{
			this.eventHelper.RaiseSync<EventArgs<Domain.RawChunk>>(RawChunkSent, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRawChunkReceived(EventArgs<Domain.RawChunk> e)
		{
			this.eventHelper.RaiseSync<EventArgs<Domain.RawChunk>>(RawChunkReceived, this, e);
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
		protected virtual void OnReceivingPacketForScripting(Domain.ModifiablePacketEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.ModifiablePacketEventArgs>(ReceivingPacketForScripting, this, e);
		}

		/// <summary></summary>
		protected virtual void OnMessageReceivedForScripting(Domain.ScriptMessageEventArgs e)
		{
			this.eventHelper.RaiseSync<Domain.ScriptMessageEventArgs>(MessageReceivedForScripting, this, e);
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
		protected virtual void OnRepositoryTxReloaded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryTxReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryBidirReloaded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryBidirReloaded, this, e);
		}

		/// <summary></summary>
		protected virtual void OnRepositoryRxReloaded(Domain.DisplayLinesEventArgs e)
		{
			this.eventHelper.RaiseSync(RepositoryRxReloaded, this, e);
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

		/// <remarks>Using item parameter instead of <see cref="EventArgs"/> for simplicity.</remarks>
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
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (OnMessageInputRequest(text, caption, buttons, icon, MessageBoxDefaultButton.Button1));
		}

		/// <summary></summary>
		protected virtual DialogResult OnMessageInputRequest(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			if (this.launchArgs.Interactive)
			{
				DebugMessage(text);

				OnCursorReset(); // Just in case...

				var e = new MessageInputEventArgs(text, caption, buttons, icon, defaultButton);
				this.eventHelper.RaiseSync<MessageInputEventArgs>(MessageInputRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "A 'Message Input' request by terminal '" + Caption + "' has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				#endif
				}

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
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new DialogEventArgs();
				this.eventHelper.RaiseSync<DialogEventArgs>(SaveAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by terminal '" + Caption + "' has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				#endif
				}

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
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(SaveCommandPageAsFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "A 'Save As' request by terminal '" + Caption + "' has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				#endif
				}

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
			if (this.launchArgs.Interactive)
			{
				OnCursorReset(); // Just in case...

				var e = new FilePathDialogEventArgs(filePathOld);
				this.eventHelper.RaiseSync<FilePathDialogEventArgs>(OpenCommandPageFileDialogRequest, this, e);

				if (e.Result == DialogResult.None) // Ensure that request has been processed by the application (as well as during testing)!
				{
				#if (DEBUG)
					Debugger.Break();
				#else
					throw (new InvalidOperationException(MKY.MessageHelper.InvalidExecutionPreamble + "An 'Open' request by terminal '" + Caption + "' has not been processed by the application!" + Environment.NewLine + Environment.NewLine + MKY.MessageHelper.SubmitBug));
				#endif
				}

				return (new FilePathDialogResult(e.Result, e.FilePathNew));
			}
			else
			{
				return (new FilePathDialogResult(DialogResult.None));
			}
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
		protected virtual void OnExitRequest(EventArgs<ExitMode> e)
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
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (Caption);
			else
				return (base.ToString());
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name 'DebugWriteLine' would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named 'Message' for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. 'Common' for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
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
					"#" + SequentialId.ToString("D2", CultureInfo.CurrentCulture),
					"[" + Guid + "]",
					message
				)
			);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_THREADS")]
		private void DebugThreads(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
