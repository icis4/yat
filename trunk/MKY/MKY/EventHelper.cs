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
// MKY Development Version 1.0.18
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
		/// Flags to configure the exception handling mode of the <see cref="EventHelper"/>.
		/// </summary>
		[Flags]
		public enum ExceptionHandlingMode
		{
			/// <summary>
			/// All exceptions will be re-thrown, i.e. may be handled by
			/// a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			RethrowAll = 0,

			/// <summary>
			/// Exceptions on the main thread will be discarded, i.e. will *not* be propagated to
			/// a <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// Exceptions on all other threads will be re-thrown, i.e. may be handled by
			/// a <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="AppDomain.UnhandledException"/> handler.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			DiscardMainThreadExceptions = 1,

			/// <summary>
			/// Exceptions on all non-main threads will be discarded, i.e. will *not* be propagated
			/// to a <see cref="AppDomain.UnhandledException"/> or
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> or
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> handler.
			/// Exceptions on the main thread will be re-thrown, i.e. may be handled by
			/// a <see cref="EventHelper.UnhandledExceptionOnMainThread"/> or catch-all handler.
			/// </summary>
			/// <remarks>
			/// The main thread can be set by <see cref="MainThreadHelper.SetCurrentThread"/>.
			/// </remarks>
			DiscardNonMainThreadExceptions = 2,

			/// <summary>
			/// All exceptions will be discarded, i.e. will neither be propagated
			/// to a <see cref="AppDomain.UnhandledException"/> nor
			/// <see cref="System.Windows.Forms.Application.ThreadException"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnNonMainThread"/> nor
			/// <see cref="EventHelper.UnhandledExceptionOnMainThread"/> nor catch-all handler.
			/// </summary>
			DiscardAll = DiscardMainThreadExceptions | DiscardNonMainThreadExceptions
		}

		#endregion

		#region Item
		//==========================================================================================
		// Item
		//==========================================================================================

		/// <summary></summary>
		public class Item
		{
			#region Field/Event/Lifetime/Property
			//======================================================================================
			// Field/Event/Lifetime/Property
			//======================================================================================

			private ExceptionHandlingMode exceptionHandling; // = RethrowAllExceptions;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnMainThread;

			/// <summary></summary>
			public event EventHandler<UnhandledExceptionEventArgs> UnhandledExceptionOnNonMainThread;

			/// <summary></summary>
			public Item(ExceptionHandlingMode exceptionHandling = ExceptionHandlingMode.RethrowAll)
			{
				this.exceptionHandling = exceptionHandling;
			}

			/// <summary></summary>
			public ExceptionHandlingMode ExceptionHandling
			{
				get { return (this.exceptionHandling); }
				set { this.exceptionHandling = value;  }
			}

			#endregion

			#region Sync Event Invoking
			//======================================================================================
			// Sync Event Invoking
			//======================================================================================

			/// <summary>
			/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
			public void FireSync(Delegate eventDelegate, params object[] args)
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
						InvokeSynchronized(sinkTarget, sink, args);
					else
						InvokeOnCurrentThread(sink, args);
				}
			}

			/// <summary>
			/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
			/// </summary>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
			public void FireSync<TEventArgs>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
				{
					var sinkTarget = (sink.Target as ISynchronizeInvoke);
					if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
						InvokeSynchronized(sinkTarget, sink, args);
					else
						InvokeOnCurrentThread(sink, args);
				}
			}

			/// <summary>
			/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
			/// </summary>
			/// <remarks>
			/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
			/// </remarks>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			public void FireSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
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
			/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
			/// thread pool.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			public void FireAsync(Delegate eventDelegate, params object[] args)
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
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
			/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
			/// thread pool.
			/// </summary>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
			public void FireAsync<TEventArgs>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
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
			/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
			/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
			/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
			/// thread pool.
			/// </summary>
			/// <remarks>
			/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
			/// </remarks>
			/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
			/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
			[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
			[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
			public void FireAsync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
				where TEventArgs : EventArgs
			{
				if (eventDelegate == null)
					return;

				Delegate[] sinks = eventDelegate.GetInvocationList();
				foreach (Delegate sink in sinks)
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
					var isMainThread= MainThreadHelper.IsMainThread;
					var discard = EvaluateDiscard(isMainThread);

					var sb = new StringBuilder();

					if (isMainThread)
						sb.Append("Exception in event callback synchronized from main thread!");
					else
						sb.Append("Exception in event callback synchronized from non-main thread!");

					if (discard)
						sb.Append(" Exception is being discarded...");
					else
						sb.Append(" Exception will be re-thrown...");

					WriteExceptionAndEventToDebugOutput(ex, sink, sb.ToString());

					if (!discard)
					{
						if (isMainThread)
						{
							if (UnhandledExceptionOnMainThread != null)
								FireSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Re-throw!
						}
						else
						{
							if (UnhandledExceptionOnNonMainThread != null)
								FireSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnNonMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Re-throw!
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
					var isMainThread= MainThreadHelper.IsMainThread;
					var discard = EvaluateDiscard(isMainThread);

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

					if (isMainThread)
						sb.Append("Exception in synchronous event callback on main thread!");
					else
						sb.Append("Exception in synchronous event callback on non-main thread!");

					if (discard)
						sb.Append(" Exception is being discarded...");
					else
						sb.Append(" Exception will be re-thrown...");

					WriteExceptionAndEventToDebugOutput(ex, sink, sb.ToString());

					if (!discard)
					{
						if (isMainThread)
						{
							if (UnhandledExceptionOnMainThread != null)
								FireSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Re-throw!
						}
						else
						{
							if (UnhandledExceptionOnNonMainThread != null)
								FireSync<UnhandledExceptionEventArgs>(UnhandledExceptionOnNonMainThread, this, new UnhandledExceptionEventArgs(ex, false));
							else
								throw; // Re-throw!
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
					// As a consequence, an exception re-thrown above will not be propagated to the
					// application's 'UnhandledException' handler, but rather be discarded by the
					// timer's 'Elapsed' event. Pretty particular, positively spoken...
					// The issue doesn't happen if the timer's 'SynchronizationObject' is set, i.e.
					// the 'Elapsed' event callback is dispatched onto a different thread. In that
					// case, typically, 'InvokeSynchronized()' will be used by the event.
					// The issue can also be circumvented by using this helper's additional event
					// 'UnhandledExceptionOnNonMainThread'.
				}
			}

			private bool EvaluateDiscard(bool isMainThread)
			{
				if (isMainThread)
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardMainThreadExceptions) != 0);
				else
					return ((ExceptionHandling & ExceptionHandlingMode.DiscardNonMainThreadExceptions) != 0);
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

		#region Field/Event/Lifetime/Property
		//==========================================================================================
		// Field/Event/Lifetime/Property
		//==========================================================================================

	#if (DEBUG)
		private static Item staticItem = new Item(ExceptionHandlingMode.DiscardAll); // For 'Debug' => write exception details to debug output.
	#else // RELEASE
		private static Item staticItem = new Item(ExceptionHandlingMode.RethrowAll); // For 'Release' => to be handled by the application's unhandled exception handlers,
	#endif // DEBUG|RELEASE                                                          //                                or the debugger in case of running 'Release' in debugger.

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
		public static ExceptionHandlingMode ExceptionHandling
		{
			get { return (staticItem.ExceptionHandling); }
			set { staticItem.ExceptionHandling = value;  }
		}

		#endregion

		#region Sync Event Invoking
		//==========================================================================================
		// Sync Event Invoking
		//==========================================================================================

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireSync(Delegate eventDelegate, params object[] args)
		{
			staticItem.FireSync(eventDelegate, args);
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireSync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.FireSync<TEventArgs>(eventDelegate, args);
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
		/// </summary>
		/// <remarks>
		/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
		/// </remarks>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.FireSync<TEventArgs, TEventHandler>(eventDelegate, args);
		}

		#endregion

		#region Async Event Invoking
		//==========================================================================================
		// Async Event Invoking
		//==========================================================================================

		/// <summary>
		/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
		/// thread pool.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireAsync(Delegate eventDelegate, params object[] args)
		{
			staticItem.FireAsync(eventDelegate, args);
		}

		/// <summary>
		/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
		/// thread pool.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireAsync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.FireAsync<TEventArgs>(eventDelegate, args);
		}

		/// <summary>
		/// Fires event with supplied arguments asynchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on a thread from the
		/// thread pool.
		/// </summary>
		/// <remarks>
		/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
		/// </remarks>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		/// <typeparam name="TEventHandler">The type of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Haven't found any alternative way to implement a generic event helper.")]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "It is the nature of this event helper to provide methods for event firing.")]
		public static void FireAsync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			staticItem.FireAsync<TEventArgs, TEventHandler>(eventDelegate, args);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
