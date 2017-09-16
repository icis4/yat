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
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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

	// Enable unhandled exception handling:
////#define RETHROW_UNHANDLED_EXCEPTIONS // Disabled for 'Debug' => only write exception details to debug output.

#else // RELEASE

	// Enable unhandled exception handling:
	#define RETHROW_UNHANDLED_EXCEPTIONS // Enabled for 'Release' => to be handled by the application's unhandled exception handlers,
	                                     //                                        or the debugger in case of running 'Release' in debugger.
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize that this type belongs to the 'EventHelper'.")]
		[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Well, there are exceptions to the rules...")]
		[Flags]
		public enum EventHandlingMode
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
		[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "'RethrowAll' actually means 'DiscardNone' but is more obvious.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize that this type belongs to the 'EventHelper'.")]
		[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Well, there are exceptions to the rules...")]
		[Flags]
		public enum ExceptionHandlingMode
		{
			/// <summary>
			/// All exceptions will be rethrown, i.e. may be handled by
			/// a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'rethrown' is a correct English term.")]
			RethrowAll = 0,

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
			/// All exceptions will be discarded, i.e. will neither be propagated
			/// to a <see cref="AppDomain.UnhandledException"/> nor
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> nor catch-all handler.
			/// </summary>
			DiscardAll = DiscardMainThread | DiscardNonMainThread
		}

		/// <summary></summary>
		public const EventHandlingMode EventHandlingDefault = EventHandlingMode.RaiseAll;

		/// <summary></summary>
	#if (RETHROW_UNHANDLED_EXCEPTIONS)
		public const ExceptionHandlingMode ExceptionHandlingDefault = ExceptionHandlingMode.RethrowAll;
	#else
		public const ExceptionHandlingMode ExceptionHandlingDefault = ExceptionHandlingMode.DiscardAll;
	#endif

		#endregion

		#region Item
		//==========================================================================================
		// Item
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize that this type belongs to the 'EventHelper'.")]
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
			public EventHandlingMode EventHandling { get; set; } = EventHandlingDefault;

			/// <summary></summary>
			public ExceptionHandlingMode ExceptionHandling { get; set; } = ExceptionHandlingDefault;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnMainThread;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnNonMainThread;

			/// <summary>
			/// Initializes a new instance of the <see cref="Item"/> class that is by default configured as follows:
			///  - <see cref="EventHandling"/>:
			///     - <see cref="EventHandlingMode.RaiseAll"/>.
			///  - <see cref="ExceptionHandling"/>:
			///     - <see cref="ExceptionHandlingMode.RethrowAll"/> for "RELEASE" configurations.
			///     - <see cref="ExceptionHandlingMode.DiscardAll"/> for "DEBUG" configurations.
			///    The behavior can be configured by the local "RETHROW_UNHANDLED_EXCEPTIONS" definition.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
			public Item(EventHandlingMode eventHandling = EventHandlingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
				: this(null, eventHandling, exceptionHandling)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Item"/> class that is by default configured as follows:
			///  - <see cref="EventHandling"/>:
			///     - <see cref="EventHandlingMode.RaiseAll"/>.
			///  - <see cref="ExceptionHandling"/>:
			///     - <see cref="ExceptionHandlingMode.RethrowAll"/> for "RELEASE" configurations.
			///     - <see cref="ExceptionHandlingMode.DiscardAll"/> for "DEBUG" configurations.
			///    The behavior can be configured by the local "RETHROW_UNHANDLED_EXCEPTIONS" definition.
			/// </summary>
			/// <remarks>
			/// Could be extended such that <paramref name="owner"/> could also be provided as callback method.
			/// </remarks>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
			public Item(string owner, EventHandlingMode eventHandling = EventHandlingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
			{
				Owner = owner;
				EventHandling = eventHandling;
				ExceptionHandling = exceptionHandling;
			}

			/// <remarks>
			/// Convenience method, identical to setting <see cref="EventHandling"/> to
			/// <see cref="EventHandlingMode.DiscardAll"/>. Useful e.g. when disposing or
			/// closing or shutting down objects or the whole application.
			/// </remarks>
			public virtual void DiscardAllEvents()
			{
				EventHandling = EventHandlingMode.DiscardAll;
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
					return ((EventHandling & EventHandlingMode.DiscardMainThread) != 0);
				else
					return ((EventHandling & EventHandlingMode.DiscardNonMainThread) != 0);
			}

			private bool ExceptionHasToBeDiscarded(bool isMainThread)
			{
				if (isMainThread)
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardMainThread) != 0);
				else
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardNonMainThread) != 0);
			}

			#endregion

			#region Sync Event Invoking
			//======================================================================================
			// Sync Event Invoking
			//======================================================================================

			/// <summary>
			/// Raises event with supplied arguments synchronously. Event is raised safely,
			/// exceptions are caught. If an event sink implements <see cref="ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current
			/// thread.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event raising.")]
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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

			#region Async Event Invoking
			//======================================================================================
			// Async Event Invoking
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
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
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

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
			[OneWay]
			private void InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, object[] args)
			{
				try
				{
					sinkTarget.Invoke(sink, args);
				}
				catch (Exception ex)
				{
					var isMainThread = MainThreadHelper.IsMainThread;
					var discard = ExceptionHasToBeDiscarded(isMainThread);

					var sb = new StringBuilder();

					if (!string.IsNullOrEmpty(Owner))
					{
						sb.Append(typeof(EventHelper).Name);
						sb.Append(" owned by ");
						sb.Append(Owner);
						sb.Append(": ");
					}

					if (isMainThread)
						sb.Append("Exception in event callback synchronized from main thread!");
					else
						sb.Append("Exception in event callback synchronized from non-main thread!");

					if (discard)
						sb.Append(" Exception is being discarded...");
					else
						sb.Append(" Exception will be rethrown...");

					WriteExceptionAndEventToDebugOutput(ex, sink, sb.ToString());

					if (!discard)
					{
						if (isMainThread)
						{
							if (UnhandledExceptionOnMainThread != null)
								RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Rethrow!
						}
						else
						{
							if (UnhandledExceptionOnNonMainThread != null)
								RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnNonMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Rethrow!
						}
					}
				}
			}

			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
			[OneWay]
			private void InvokeOnCurrentThread(Delegate sink, object[] args)
			{
				try
				{
					sink.DynamicInvoke(args);
				}
				catch (Exception ex)
				{
					var isMainThread = MainThreadHelper.IsMainThread;
					var discard = ExceptionHasToBeDiscarded(isMainThread);

					// Note that 'discard' is only evaluated *after* the exception occured. This
					// ensures that exceptions happening inside 'zombie' callbacks, i.e. callbacks
					// whose handlers got detached while the callback was being invoked, can be
					// properly discarded as illustrated below:
					//
					// 1. An event callback gets invoked.
					// 2. The event sink signals the 'EventHelper' to discard exception from now.
					// 3. The event sink gets disposed, resulting in a 'zombie' callback.
					// 4. The 'zombie' throws an exception as it can no longer access the sink.
					// 5. The 'EventHelper' discards the exception.

					var sb = new StringBuilder();

					if (!string.IsNullOrEmpty(Owner))
					{
						sb.Append(typeof(EventHelper).Name);
						sb.Append(" owned by ");
						sb.Append(Owner);
						sb.Append(": ");
					}

					if (isMainThread)
						sb.Append("Exception in synchronous event callback on main thread!");
					else
						sb.Append("Exception in synchronous event callback on non-main thread!");

					if (discard)
						sb.Append(" Exception is being discarded...");
					else
						sb.Append(" Exception will be rethrown...");

					WriteExceptionAndEventToDebugOutput(ex, sink, sb.ToString());

					if (!discard)
					{
						if (isMainThread)
						{
							if (UnhandledExceptionOnMainThread != null)
								RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Rethrow!
						}
						else
						{
							if (UnhandledExceptionOnNonMainThread != null)
								RaiseSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnNonMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Rethrow!
						}
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
					// the 'Elapsed' event callback is dispatched onto a different thread. In that
					// case, typically, 'InvokeSynchronized()' will be used by the event.
					// The issue can also be circumvented by using this helper's additional event
					// 'UnhandledExceptionOnNonMainThread'.
				}
			}

			#endregion

			#region Debug Output
			//======================================================================================
			// Debug Output
			//======================================================================================

			[Conditional("DEBUG")]
			private static void WriteExceptionAndEventToDebugOutput(Exception ex, Delegate sink, string leadMessage = null)
			{
				Diagnostics.DebugEx.WriteException(typeof(EventHelper), ex, leadMessage);
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
		///  - <see cref="EventHandling"/>:
		///     - <see cref="EventHandlingMode.RaiseAll"/>.
		///  - <see cref="ExceptionHandling"/>:
		///     - <see cref="ExceptionHandlingMode.RethrowAll"/> for "RELEASE" configurations.
		///     - <see cref="ExceptionHandlingMode.DiscardAll"/> for "DEBUG" configurations.
		///    The behavior can be configured by the local "RETHROW_UNHANDLED_EXCEPTIONS" definition.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public static Item CreateItem(EventHandlingMode eventHandling = EventHandlingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
		{
			return (CreateItem(typeof(EventHelper).FullName, eventHandling, exceptionHandling));
		}

		/// <remarks>
		/// Creates an <see cref="Item"/> that is by default configured as follows:
		///  - <see cref="EventHandling"/>:
		///     - <see cref="EventHandlingMode.RaiseAll"/>.
		///  - <see cref="ExceptionHandling"/>:
		///     - <see cref="ExceptionHandlingMode.RethrowAll"/> for "RELEASE" configurations.
		///     - <see cref="ExceptionHandlingMode.DiscardAll"/> for "DEBUG" configurations.
		///    The behavior can be configured by the local "RETHROW_UNHANDLED_EXCEPTIONS" definition.
		/// </remarks>
		/// <remarks>
		/// Could be extended such that <paramref name="owner"/> could also be provided as callback method.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public static Item CreateItem(string owner, EventHandlingMode eventHandling = EventHandlingDefault, ExceptionHandlingMode exceptionHandling = ExceptionHandlingDefault)
		{
			return (new Item(owner, eventHandling, exceptionHandling));
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
		public virtual EventHandlingMode EventHandling
		{
			get { return (staticItem.EventHandling); }
			set { staticItem.EventHandling = value;  }
		}

		/// <summary></summary>
		public static ExceptionHandlingMode ExceptionHandling
		{
			get { return (staticItem.ExceptionHandling); }
			set { staticItem.ExceptionHandling = value;  }
		}

		/// <remarks>
		/// Convenience method, identical to setting <see cref="EventHandling"/> to
		/// <see cref="EventHandlingMode.DiscardAll"/>. Useful e.g. when disposing or
		/// closing or shutting down objects or the whole application.
		/// </remarks>
		public virtual void DiscardAllEvents()
		{
			EventHandling = EventHandlingMode.DiscardAll;
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

		#region Sync Event Invoking
		//==========================================================================================
		// Sync Event Invoking
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

		#region Async Event Invoking
		//==========================================================================================
		// Async Event Invoking
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
