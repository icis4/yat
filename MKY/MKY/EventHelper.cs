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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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

	// Configure unhandled exception handling:
////#define HANDLE_UNHANDLED_EXCEPTIONS  // Disabled for 'Debug' => to be handled by the debugger.
	                                     // Rationale: Debugger shall break at the location of the initial throw.
	                                     // Temporarily enabling the option can be useful to continue program execution even in case of exceptions.
	                                     //  > Exception details will be output to the debug console.
	                                     //  > Exceptions will either be discarded or rethrown, depending on the option below.
////#define RETHROW_UNHANDLED_EXCEPTIONS // Disabled for 'Debug' => see above.

////#define BREAK_ON_EXCEPTION           // Disabled for 'Debug' => debugger shall not break.
	                                     // Rationale: Debugger shall behave in the standard way.
	                                     // Temporarily enabling the option can be useful to analyze the root cause of exceptions.
	                                     //  > Target and event details will be output to the debug console.
	                                     //  > Debugger will break.

////#define BREAK_ON_RETHROW             // Disabled for 'Debug' => debugger shall not break.
	                                     // Rationale: Debugger shall output exception details to the debug console but then continue.
	                                     // Temporarily enabling the option can be useful to analyze the root cause of exceptions.
	                                     //  > Target and event details will be output to the debug console.
	                                     //  > Debugger will break.

////#define BREAK_ON_DISPOSED_TARGET     // Disabled for 'Debug' => debugger shall not break.
	                                     // Rationale: 'DisposedTargetException' has explicitly been set to 'Discard'.
	                                     // Temporarily enabling the option can be useful to analyze the root cause of such disposed target.
	                                     //  > Target and event details will be output to the debug console.
	                                     //  > Debugger will break.

#else // RELEASE

	// Configure unhandled exception handling:
////#define HANDLE_UNHANDLED_EXCEPTIONS // Disabled for 'Release' => to be handled by the application's unhandled exception handlers,
	                                    //                                         or the debugger in case of running 'Release' in debugger.

#endif // DEBUG|RELEASE

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

using MKY.Threading;

#endregion

namespace MKY
{
	/// <summary>
	/// Helper to publish events throughout an application or library. Provides the
	/// <see cref="ExceptionHandling"/> property to configure the runtime behavior.
	/// </summary>
	/// <remarks>
	/// Different patterns to handle events exist, use of such a helper may be questioned given the
	/// simplified event handling features of modern C#. However, there are also a few goodies in
	/// using such helper, especially when it comes to shutdown of a system.
	/// <list type="bullet">
	/// <item>
	/// <description>Preventing <see cref="InvalidAsynchronousStateException"/></description>
	/// <![CDATA["Fehler beim Aufrufen der Methode. Der Zielthread ist nicht mehr vorhanden."]]> can
	/// happen when the sink gets disposed of while an async callback is still pending. Preventing
	/// this without such helper can be fairly tricky, as all calling back instances within a system
	/// would have to be notified about a shutdown and then get polled for completion.
	/// </item>
	/// </list>
	/// </remarks>
	public class EventHelper
	{
		#region Mode
		//==========================================================================================
		// Mode
		//==========================================================================================

		/// <summary>
		/// Flags to configure the event handling mode of the <see cref="EventHelper"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "'RaiseAll' actually means 'DiscardNone' but is more obvious.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Well, there are exceptions to the rules...")]
		[Flags]
		public enum EventRaisingMode
		{
			/// <summary>
			/// All events will be raised.
			/// </summary>
			RaiseAll = 0,

			/// <summary>
			/// Events on the main thread will be discarded.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			DiscardMainThread = 1,

			/// <summary>
			/// Events on all non-main threads will be discarded.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			DiscardNonMainThread = 2,

			/// <summary>
			/// All events will be discarded.
			/// </summary>
			DiscardAll = DiscardMainThread | DiscardNonMainThread
		}

		/// <summary>
		/// Flags to configure the exception handling mode of the <see cref="EventHelper"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Well, there are exceptions to the rules...")]
		[Flags]
		public enum ExceptionHandlingMode
		{
			/// <summary>
			/// All exceptions will be tolerated, i.e. not handled be the helper but rather by
			/// a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			None = 0,

			/// <summary>
			/// Exceptions on the main thread will be discarded, i.e. will *not* be propagated to
			/// a <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// Exceptions on all other threads will be rethrown, i.e. may be handled by
			/// a <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="AppDomain.UnhandledException"/> handler.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'rethrown' is a correct English term.")]
			DiscardMainThread = 1,

			/// <summary>
			/// Exceptions on all non-main threads will be discarded, i.e. will *not* be propagated
			/// to a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> handler.
			/// Exceptions on the main thread will be rethrown, i.e. may be handled by
			/// a <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'rethrown' is a correct English term.")]
			DiscardNonMainThread = 2,

			/// <summary>
			/// Events that got raised on targets even if <see cref="IDisposableEx.IsUndisposed"/>
			/// is not indicated likely result in an <see cref="ObjectDisposedException"/> or even
			/// a <see cref="TargetInvocationException"/>. It is not possible to discard such events
			/// upfront, i.e. not raising them at all, since the target could get disposed AFTER the
			/// check has been done. Therefore this additional option tell the helper whether or not
			/// to discard such exceptions.
			/// </summary>
			DiscardDisposedTarget = 4,

			/// <summary>
			/// All exceptions will be discarded, i.e. will neither be propagated
			/// to a <see cref="AppDomain.UnhandledException"/> nor
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> nor catch-all handler.
			/// </summary>
			DiscardAll = DiscardMainThread | DiscardNonMainThread | DiscardDisposedTarget,

			/// <summary>
			/// All exceptions will be rethrown, i.e. initially handled be the helper including
			/// outputting debug information, but then rethrown and handled by
			/// a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'rethrown' is a correct English term.")]
			RethrowAll = 8
		}

		/// <summary></summary>
		public const EventRaisingMode EventRaisingDefault = EventRaisingMode.RaiseAll;

		/// <summary></summary>
	#if (!HANDLE_UNHANDLED_EXCEPTIONS)
		public const ExceptionHandlingMode ExceptionHandlingDefault = ExceptionHandlingMode.None;
	#else
		#if (!RETHROW_UNHANDLED_EXCEPTIONS)
		public const ExceptionHandlingMode ExceptionHandlingDefault = ExceptionHandlingMode.DiscardAll;
		#else
		public const ExceptionHandlingMode ExceptionHandlingDefault = ExceptionHandlingMode.RethrowAll;
		#endif
	#endif

		#endregion

		#region Item
		//==========================================================================================
		// Item
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		[Serializable]
		public class Item
		{
			#region Fields/Events/Lifetime/Properties/Methods
			//======================================================================================
			// Fields/Events/Lifetime/Properties/Methods
			//======================================================================================

			/// <summary></summary>
			public string Owner { get; protected set; } // = null;

			/// <summary></summary>
			public EventRaisingMode EventRaising { get; set; } = EventRaisingDefault;

			/// <summary></summary>
			public ExceptionHandlingMode ExceptionHandling { get; set; } = ExceptionHandlingDefault;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnMainThread;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnNonMainThread;

			/// <summary>
			/// Initializes a new instance of the <see cref="Item"/> class that is by default configured as follows:
			///  - <see cref="EventRaising"/>: <see cref="EventRaisingMode.RaiseAll"/>.
			///  - <see cref="ExceptionHandling"/>: <see cref="ExceptionHandlingMode.None"/>.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public Item(EventRaisingMode eventRaising = EventRaisingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
				: this(null, eventRaising, exceptionHandling)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Item"/> class that is by default configured as follows:
			///  - <see cref="EventRaising"/>: <see cref="EventRaisingMode.RaiseAll"/>.
			///  - <see cref="ExceptionHandling"/>: <see cref="ExceptionHandlingMode.None"/>.
			/// </summary>
			/// <remarks>
			/// Could be extended such that <paramref name="owner"/> could also be provided as callback method.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public Item(string owner, EventRaisingMode eventRaising = EventRaisingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
			{
				Owner             = owner;
				EventRaising      = eventRaising;
				ExceptionHandling = exceptionHandling;
			}

			/// <remarks>
			/// Convenience method, identical to setting <see cref="EventRaising"/> to
			/// <see cref="EventRaisingMode.DiscardAll"/>. Useful e.g. when disposing or
			/// closing or shutting down objects or the whole application.
			/// </remarks>
			public virtual void DiscardAllEvents()
			{
				EventRaising = EventRaisingMode.DiscardAll;
			}

			/// <remarks>
			/// Convenience method, identical to setting <see cref="ExceptionHandling"/> to
			/// <see cref="ExceptionHandlingMode.DiscardAll"/>. Useful e.g. when disposing or
			/// closing or shutting down objects or the whole application.
			/// </remarks>
			public virtual void DiscardAllExceptions()
			{
				ExceptionHandling = ExceptionHandlingMode.DiscardAll;
			}

			/// <remarks>
			/// Convenience method, identical to calling <see cref="DiscardAllEvents"/> and
			/// <see cref="DiscardAllExceptions"/>. Useful e.g. when disposing or closing or
			/// shutting down objects or the whole application.
			/// </remarks>
			public virtual void DiscardAllEventsAndExceptions()
			{
				DiscardAllEvents();
				DiscardAllExceptions();
			}

			private bool EventHasToBeDiscarded(bool isMainThread)
			{
				if (isMainThread)
					return ((EventRaising & EventRaisingMode.DiscardMainThread) != 0);
				else
					return ((EventRaising & EventRaisingMode.DiscardNonMainThread) != 0);
			}

			private bool ExceptionHasToBeRethrown()
			{
				return ((ExceptionHandling & ExceptionHandlingMode.RethrowAll) != 0);
			}

			private bool ExceptionHasToBeDiscarded(bool isMainThread)
			{
				if (isMainThread)
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardMainThread) != 0);
				else
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardNonMainThread) != 0);
			}

			private bool DisposedTargetExceptionHasToBeDiscarded()
			{
				return ((ExceptionHandling & ExceptionHandlingMode.DiscardDisposedTarget) != 0);
			}

			#endregion

			#region Sync Event Raising
			//======================================================================================
			// Sync Event Raising
			//======================================================================================

			/// <summary>
			/// Raises event with supplied arguments synchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current
			/// thread.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseSync(Delegate eventDelegate, params object[] args)
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
						InvokeSynchronized(sinkTarget, sink, args);
					else
						InvokeOnCurrentThread(sink, args);
				}
			}

			/// <summary>
			/// Raises event with supplied arguments synchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current
			/// thread.
			/// </summary>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseSync<TEventArgs>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
						InvokeSynchronized(sinkTarget, sink, args);
					else
						InvokeOnCurrentThread(sink, args);
				}
			}

			/// <summary>
			/// Raises event with supplied arguments synchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current
			/// thread.
			/// </summary>
			/// <remarks>
			/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
			/// </remarks>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
						InvokeSynchronized(sinkTarget, sink, args);
					else
						InvokeOnCurrentThread(sink, args);
				}
			}

			#endregion

			#region Async Event Raising
			//======================================================================================
			// Async Event Raising
			//======================================================================================

			private delegate void AsyncInvokeDelegate(Delegate eventDelegate, object[] args);

			/// <summary>
			/// Raises event with supplied arguments asynchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread
			/// from the thread pool.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseAsync(Delegate eventDelegate, params object[] args)
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if (sinkTarget != null)          // No need to check for InvokeRequired,
					{                                //   async always requires invoke.
						sinkTarget.BeginInvoke(sink, args);
					}
					else
					{
						var asyncInvoker = new AsyncInvokeDelegate(InvokeOnCurrentThread);
						asyncInvoker.BeginInvoke(sink, args, null, null);
					}
				}
			}

			/// <summary>
			/// Raises event with supplied arguments asynchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread
			/// from the thread pool.
			/// </summary>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseAsync<TEventArgs>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if (sinkTarget != null)          // No need to check for InvokeRequired,
					{                                //   async always requires invoke.
						sinkTarget.BeginInvoke(sink, args);
					}
					else
					{
						var asyncInvoker = new AsyncInvokeDelegate(InvokeOnCurrentThread);
						asyncInvoker.BeginInvoke(sink, args, null, null);
					}
				}
			}

			/// <summary>
			/// Raises event with supplied arguments asynchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread
			/// from the thread pool.
			/// </summary>
			/// <remarks>
			/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
			/// </remarks>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			public virtual void RaiseAsync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				if (EventHasToBeDiscarded(MainThreadHelper.IsMainThread))
					return;

				var sinks = eventDelegate.GetInvocationList();
				foreach (var sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if (sinkTarget != null)          // No need to check for InvokeRequired,
					{                                //   async always requires invoke.
						sinkTarget.BeginInvoke(sink, args);
					}
					else
					{
						var asyncInvoker = new AsyncInvokeDelegate(InvokeOnCurrentThread);
						asyncInvoker.BeginInvoke(sink, args, null, null);
					}
				}
			}

			#endregion

			#region Safe Invoke
			//======================================================================================
			// Safe Invoke
			//======================================================================================

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
			[OneWay]
			private void InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, object[] args)
			{
				try
				{
					if (ExceptionHandling == ExceptionHandlingMode.None)
					{
						sinkTarget.Invoke(sink, args);
					}
					else
					{
						try
						{
							sinkTarget.Invoke(sink, args);
						}
						catch (Exception ex)
						{
							if (HandleExceptionAndEvaluateRethrow(ex, sink, wasSyncInvoke: false))
								throw; // Rethrow!

								// Note that 'discard' vs. 'rethrow' is evaluated AFTER the exception
								// has occurred. This ensures that exceptions happening inside 'zombie'
								// callbacks, i.e. callbacks whose handlers got detached while the
								// callback was being invoked, can be properly discarded as illustrated
								// below:
								//
								// 1. An event callback gets invoked.
								// 2. The event sink signals the 'EventHelper' to discard exception from now.
								// 3. The event sink gets disposed, resulting in a 'zombie' callback.
								// 4. The 'zombie' throws an exception as it can no longer access the sink.
								// 5. The 'EventHelper' discards the exception.
						}
					}
				}
				catch (TargetInvocationException ex)
				{
					if (HandleExceptionAndEvaluateRethrow(ex, sink, wasSyncInvoke: true))
						throw; // Rethrow!

					// Same as above, 'discard' vs. 'rethrow' is evaluated AFTER the exception has
					// occurred.
				}
			}

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
			[OneWay]
			private void InvokeOnCurrentThread(Delegate sink, object[] args)
			{
				try
				{
					if (ExceptionHandling == ExceptionHandlingMode.None)
					{
						sink.DynamicInvoke(args);
					}
					else
					{
						try
						{
							sink.DynamicInvoke(args);
						}
						catch (Exception ex)
						{
							if (HandleExceptionAndEvaluateRethrow(ex, sink, wasSyncInvoke: true))
								throw; // Rethrow!

							// Note that 'discard' vs. 'rethrow' is evaluated AFTER the exception
							// has occurred. This ensures that exceptions happening inside 'zombie'
							// callbacks, i.e. callbacks whose handlers got detached while the
							// callback was being invoked, can be properly discarded as illustrated
							// below:
							//
							// 1. An event callback gets invoked.
							// 2. The event sink signals the 'EventHelper' to discard exception from now.
							// 3. The event sink gets disposed, resulting in a 'zombie' callback.
							// 4. The 'zombie' throws an exception as it can no longer access the sink.
							// 5. The 'EventHelper' discards the exception.
						}
					}
				}
				catch (TargetInvocationException ex)
				{
					if (HandleExceptionAndEvaluateRethrow(ex, sink, wasSyncInvoke: true))
						throw; // Rethrow!

					// Same as above, 'discard' vs. 'rethrow' is evaluated AFTER the exception has
					// occurred.
				}
			}

			private bool HandleExceptionAndEvaluateRethrow(Exception ex, Delegate sink, bool wasSyncInvoke)
			{
				var isMainThread = MainThreadHelper.IsMainThread;

				// Special case 1: Check whether to rethrow in any case:
				if (ExceptionHasToBeRethrown())
				{
					DebugWriteExceptionAndEventToDebugOutput(ex, sink, wasSyncInvoke, isMainThread, false);
				#if (BREAK_ON_RETHROW)
					Debugger.Break(); // OK as conditional and disabled by default.
				#endif
					return (true);
				}

				// Special case 2: Check whether to discard exceptions caused by a disposed target:
				if (DisposedTargetExceptionHasToBeDiscarded())
				{
					var sinkTarget = (sink.Target as IDisposableEx);
					if (((sinkTarget != null) && (!sinkTarget.IsUndisposed)) || (ex is ObjectDisposedException))
					{
						DebugWriteDisposedTargetAndEventToDebugOutput(sink);
					#if (BREAK_ON_DISPOSED_TARGET)
						Debugger.Break(); // OK as conditional and disabled by default.
					#endif
						return (false);
					}
				}

				// Neither special case applies:
				var discard = ExceptionHasToBeDiscarded(isMainThread);
				DebugWriteExceptionAndEventToDebugOutput(ex, sink, wasSyncInvoke, isMainThread, discard);
				if (!discard)
				{
					if (isMainThread)
					{
						if (UnhandledExceptionOnMainThread != null)
							RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnMainThread, this, new UnhandledExceptionEventArgs(ex, false));
						else
							return (true);
					}
					else
					{
						if (UnhandledExceptionOnNonMainThread != null)
							RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnNonMainThread, this, new UnhandledExceptionEventArgs(ex, false));
						else
							return (true);
					}

					// Attention, the 'System.Timers.Timer' class has a very particular behavior
					// that impacts re-throwing above!
					//
					// https://msdn.microsoft.com/en-us/library/system.timers.timer.aspx
					// "The Timer component catches and suppresses all exceptions thrown by event
					//  handlers for the Elapsed event. This behavior is subject to change in future
					//  releases of the .NET Framework."
					//
					// As a consequence, an exception rethrown above will not be propagated to the
					// application's 'UnhandledException' handler, but rather be discarded by the
					// timer's 'Elapsed' event. Pretty particular, positively spoken...
					// The issue doesn't happen if the timer's 'SynchronizationObject' is set, i.e.
					// the 'Elapsed' event callback is dispatched onto the thread of the specified
					// 'SynchronizationObject'. In that case, typically, 'InvokeSynchronized()'
					// will be used by the event. The issue can also be circumvented by using this
					// helper's additional event 'UnhandledExceptionOnNonMainThread'.
				}

				return (false);
			}

			#endregion

			#region Debug Output
			//======================================================================================
			// Debug Output
			//======================================================================================

			/// <summary></summary>
			[Conditional("DEBUG")]
			protected virtual void DebugWriteDisposedTargetAndEventToDebugOutput(Delegate sink)
			{
				if (!string.IsNullOrEmpty(Owner))
				{
					var sb = new StringBuilder();
					sb.Append(typeof(EventHelper).Name);
					sb.Append(" owned by ");
					sb.Append(Owner);
					sb.Append(": Target has already been disposed!");
					Debug.WriteLine(sb.ToString());
				}

				Debug.Indent();
				{
					Debug.WriteLine("Target:");
					Debug.Indent();
					{
						Debug.WriteLine(sink.Target.GetType().ToString());
					}
					Debug.Unindent();
				}
				Debug.Unindent();

				Debug.Indent();
				{
					Debug.WriteLine("Event:");
					Debug.Indent();
					{
						var sb = new StringBuilder();
						sb.Append(sink.Method.Name);
						sb.Append(" in ");
						sb.Append(sink.Target.GetType().ToString());
						sb.Append(".");
						Debug.WriteLine(sb.ToString());
					}
					Debug.Unindent();
				}
				Debug.Unindent();

			#if (BREAK_ON_EXCEPTION)
				Debugger.Break(); // OK as conditional and disabled by default.
			#endif
			}

			/// <summary></summary>
			[Conditional("DEBUG")]
			protected virtual void DebugWriteExceptionAndEventToDebugOutput(Exception ex, Delegate sink, bool wasSyncInvoke, bool isMainThread, bool discard)
			{
				var leadMessage = new StringBuilder();

				if (!string.IsNullOrEmpty(Owner))
				{
					leadMessage.Append(typeof(EventHelper).Name);
					leadMessage.Append(" owned by ");
					leadMessage.Append(Owner);
					leadMessage.Append(": ");
				}

				leadMessage.Append("Exception in ");

				if (wasSyncInvoke)
					leadMessage.Append("synchronous ");

				leadMessage.Append("event callback ");

				if (wasSyncInvoke)
					leadMessage.Append("on ");
				else
					leadMessage.Append("synchronized from ");

				if (isMainThread)
					leadMessage.Append("main thread! ");
				else
					leadMessage.Append("non-main thread! ");

				if (discard)
					leadMessage.Append("Exception is being discarded...");
				else
					leadMessage.Append("Exception will be rethrown...");

				Diagnostics.DebugEx.WriteException(typeof(EventHelper), ex, leadMessage.ToString());

				Debug.Indent();
				{
					Debug.WriteLine("Event:");
					Debug.Indent();
					{
						var sb = new StringBuilder();
						sb.Append(sink.Method.Name);
						sb.Append(" in ");
						sb.Append(sink.Target.GetType().ToString());
						sb.Append(".");
						Debug.WriteLine(sb.ToString());
					}
					Debug.Unindent();
				}
				Debug.Unindent();

			#if (BREAK_ON_EXCEPTION)
				Debugger.Break(); // OK as conditional and disabled by default.
			#endif
			}

			#endregion
		}

		#endregion

		#region Static Item
		//==========================================================================================
		// Static Item
		//==========================================================================================

		#region Fields/Events/Lifetime/Properties/Methods
		//==========================================================================================
		// Fields/Events/Lifetime/Properties/Methods
		//==========================================================================================

		private static Item staticItem = CreateItem();

		/// <remarks>
		/// Creates an <see cref="Item"/> that is by default configured as follows:
		///  - <see cref="EventRaising"/>: <see cref="EventRaisingMode.RaiseAll"/>.
		///  - <see cref="ExceptionHandling"/>: <see cref="ExceptionHandlingMode.None"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static Item CreateItem(EventRaisingMode eventRaising = EventRaisingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
		{
			return (CreateItem(typeof(EventHelper).FullName, eventRaising, exceptionHandling));
		}

		/// <remarks>
		/// Creates an <see cref="Item"/> that is by default configured as follows:
		///  - <see cref="EventRaising"/>: <see cref="EventRaisingMode.RaiseAll"/>.
		///  - <see cref="ExceptionHandling"/>: <see cref="ExceptionHandlingMode.None"/>.
		/// </remarks>
		/// <remarks>
		/// Could be extended such that <paramref name="owner"/> could also be provided as callback method.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static Item CreateItem(string owner, EventRaisingMode eventRaising = EventRaisingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
		{
			return (new Item(owner, eventRaising, exceptionHandling));
		}

		/// <summary></summary>
		public static event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnMainThread
		{
			add    { lock (staticItem) staticItem.UnhandledExceptionOnMainThread += value; }
			remove { lock (staticItem) staticItem.UnhandledExceptionOnMainThread -= value; }
		}

		/// <summary></summary>
		public static event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnNonMainThread
		{
			add    { lock (staticItem) staticItem.UnhandledExceptionOnNonMainThread += value; }
			remove { lock (staticItem) staticItem.UnhandledExceptionOnNonMainThread -= value; }
		}

		/// <summary></summary>
		public virtual EventRaisingMode EventRaising
		{
			get { return (staticItem.EventRaising); }
			set { staticItem.EventRaising = value;  }
		}

		/// <summary></summary>
		public static ExceptionHandlingMode ExceptionHandling
		{
			get { return (staticItem.ExceptionHandling); }
			set { staticItem.ExceptionHandling = value;  }
		}

		/// <remarks>
		/// Convenience method, identical to setting <see cref="EventRaising"/> to
		/// <see cref="EventRaisingMode.DiscardAll"/>. Useful e.g. when disposing or
		/// closing or shutting down objects or the whole application.
		/// </remarks>
		public virtual void DiscardAllEvents()
		{
			EventRaising = EventRaisingMode.DiscardAll;
		}

		/// <remarks>
		/// Convenience method, identical to setting <see cref="ExceptionHandling"/> to
		/// <see cref="ExceptionHandlingMode.DiscardAll"/>. Useful e.g. when disposing or
		/// closing or shutting down objects or the whole application.
		/// </remarks>
		public virtual void DiscardAllExceptions()
		{
			ExceptionHandling = ExceptionHandlingMode.DiscardAll;
		}

		/// <remarks>
		/// Convenience method, identical to calling <see cref="DiscardAllEvents"/> and
		/// <see cref="DiscardAllExceptions"/>. Useful e.g. when disposing or closing or
		/// shutting down objects or the whole application.
		/// </remarks>
		public virtual void DiscardAllEventsAndExceptions()
		{
			DiscardAllEvents();
			DiscardAllExceptions();
		}

		#endregion

		#region Sync Event Raising
		//==========================================================================================
		// Sync Event Raising
		//==========================================================================================

		/// <summary>
		/// Raises event with supplied arguments synchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current
		/// thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseSync(Delegate eventDelegate, params object[] args)
		{
			staticItem.RaiseSync(eventDelegate, args);
		}

		/// <summary>
		/// Raises event with supplied arguments synchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current
		/// thread.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseSync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.RaiseSync<TEventArgs>(eventDelegate, args);
		}

		/// <summary>
		/// Raises event with supplied arguments synchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current
		/// thread.
		/// </summary>
		/// <remarks>
		/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
		/// </remarks>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.RaiseSync<TEventArgs, TEventHandler>(eventDelegate, args);
		}

		#endregion

		#region Async Event Raising
		//==========================================================================================
		// Async Event Raising
		//==========================================================================================

		/// <summary>
		/// Raises event with supplied arguments asynchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from
		/// the thread pool.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseAsync(Delegate eventDelegate, params object[] args)
		{
			staticItem.RaiseAsync(eventDelegate, args);
		}

		/// <summary>
		/// Raises event with supplied arguments asynchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from
		/// the thread pool.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseAsync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.RaiseAsync<TEventArgs>(eventDelegate, args);
		}

		/// <summary>
		/// Raises event with supplied arguments asynchronously. Event is raised safely,
		/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from
		/// the thread pool.
		/// </summary>
		/// <remarks>
		/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
		/// </remarks>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
		public static void RaiseAsync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.RaiseAsync<TEventArgs, TEventHandler>(eventDelegate, args);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
