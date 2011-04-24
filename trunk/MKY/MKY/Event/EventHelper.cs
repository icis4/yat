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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Choose whether exceptions should be handled or execution immediately stopped:
	// - Uncomment to handle exceptions
	// - Comment out to break exceptions
	#define DEBUG_HANDLE_EXCEPTIONS

	#if (!DEBUG_HANDLE_EXCEPTIONS) // Break exceptions is mutual exclusive against handle exceptions.
		#define DEBUG_BREAK_EXCEPTIONS
	#endif

#endif

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
using System.Threading;

#endregion

namespace MKY.Event
{
	/// <summary>
	/// Events helper methods to defensivly publish events.
	/// </summary>
	public static class EventHelper
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		public class UnhandledExceptionEventArgs : EventArgs
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public fields are straight-forward for event args.")]
			public readonly Exception UnhandledException;

			/// <summary></summary>
			public UnhandledExceptionEventArgs(Exception unhandledException)
			{
				UnhandledException = unhandledException;
			}
		}

		#endregion

		#region Static Events
		//==========================================================================================
		// Static Events
		//==========================================================================================

#if (!DEBUG)

		/// <summary>
		/// Event on unhandled exceptions. An application can install an event handler that handles
		/// the unhandled exceptions.
		/// </summary>
		public static event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

#endif

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

#if (!DEBUG)

		/// <summary>
		/// Semaphore to temporarily suspend the unhandled exceptions event.
		/// </summary>
		private static int SuspendUnhandledExceptionEventSemaphore;

#endif

		/// <remarks>
		/// \fixme:
		/// Not an optimal solution. Added to fix an issue with asynchronous timers in
		/// <see cref="MKY.Time.Chronometer"/>. Should be replaced by a real solution.
		/// </remarks>
		public static void SuspendUnhandledException()
		{
#if (!DEBUG)
			Interlocked.Increment(ref SuspendUnhandledExceptionEventSemaphore);
#endif
		}

		/// <remarks>
		/// \fixme:
		/// Not an optimal solution. Added to fix an issue with asynchronous timers in
		/// <see cref="MKY.Time.Chronometer"/>. Should be replaced by a real solution.
		/// </remarks>
		public static void ResumeUnhandledException()
		{
#if (!DEBUG)
			Interlocked.Decrement(ref SuspendUnhandledExceptionEventSemaphore);
#endif
		}

		#endregion

		#region Sync Event Invoking
		//==========================================================================================
		// Sync Event Invoking
		//==========================================================================================

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is
		/// invoked on the current thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static void FireSync(Delegate eventDelegate, params object[] args)
		{
			if (eventDelegate == null)
				return;

			// Invoke event in a safe way
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
				{
					InvokeSynchronized(sinkTarget, sink, args);
				}
				else
				{
				#if (DEBUG_HANDLE_EXCEPTIONS) // Invoke event savely to ensure that program execution continues.
					EventHandler castedSink = (EventHandler)sink;
					object sender = args[0];
					EventArgs eventArgs = (EventArgs)args[1];
					try
					{
						castedSink(sender, eventArgs);
					}
					catch (Exception ex)
					{
						WriteExceptionToDebugOutput(ex, sink);
					}
				#elif (DEBUG_BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen.
					EventHandler castedSink = (EventHandler)sink;
					object sender = args[0];
					EventArgs eventArgs = (EventArgs)args[1];
					castedSink(sender, eventArgs);
				#else // NON-DEBUG: Invoke event the safe way
					InvokeOnCurrentThread(sink, args);
				#endif
				}
			}
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is fired safely, exceptions are
		/// caught. If an event sink implements <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is invoked on the current thread.
		/// </summary>
		/// <typeparam name="TEventArgs">The type of the EventArgs of the requested event.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		public static void FireSync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			if (eventDelegate == null)
				return;

			// Invoke event in a safe way.
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
				{
					InvokeSynchronized(sinkTarget, sink, args);
				}
				else
				{
				#if (DEBUG_HANDLE_EXCEPTIONS) // Invoke event savely to ensure that program execution continues.
					EventHandler<TEventArgs> castedSink = (EventHandler<TEventArgs>)sink;
					object sender = args[0];
					TEventArgs eventArgs = (TEventArgs)args[1];
					try
					{
						castedSink(sender, eventArgs);
					}
					catch (Exception ex)
					{
						WriteExceptionToDebugOutput(ex, sink);
					}
				#elif (DEBUG_BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen.
					EventHandler<TEventArgs> castedSink = (EventHandler<TEventArgs>)sink;
					object sender = args[0];
					TEventArgs eventArgs = (TEventArgs)args[1];
					castedSink(sender, eventArgs);
				#else // NON-DEBUG: Invoke event the safe way.
					InvokeOnCurrentThread(sink, args);
				#endif
				}
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
		public static void FireSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			if (eventDelegate == null)
				return;

			// Events of type "event MyEventHandler MyEvent" are always fired the non-debug way.
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
					InvokeSynchronized(sinkTarget, sink, args);
				else
					InvokeOnCurrentThread(sink, args);
			}
		}

		#endregion

		#region Async Event Invoking
		//==========================================================================================
		// Async Event Invoking
		//==========================================================================================

		private delegate void AsyncInvokeDelegate(Delegate eventDelegate, object[] args);

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is
		/// invoked on a thread from the thread pool.
		/// </summary>
		public static void FireAsync(Delegate eventDelegate, params object[] args)
		{
			if (eventDelegate == null)
				return;

			// Invoke event in a safe way for DEBUG and NON-DEBUG.
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if (sinkTarget != null)          // No need to check for InvokeRequired,
				{                                //   async always requires invoke.
					sinkTarget.BeginInvoke(sink, args);
				}
				else
				{
					AsyncInvokeDelegate asyncInvoker = new AsyncInvokeDelegate(InvokeOnCurrentThread);
					asyncInvoker.BeginInvoke(sink, args, null, null);
				}
			}
		}

		#endregion

		#region Safe Invoke Synchronized To ISynchronizeInvoke
		//==========================================================================================
		// Safe Invoke Synchronized To ISynchronizeInvoke
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		[OneWay]
		private static void InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, object[] args)
		{
		#if (DEBUG_HANDLE_EXCEPTIONS) // Invoke event savely to ensure that program execution continues.
			try
			{
				sinkTarget.Invoke(sink, args);
			}
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
		#elif (DEBUG_BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen.
			sinkTarget.Invoke(sink, args);
		#else // NON-DEBUG: Forward or discard exception.
			try
			{
				sinkTarget.Invoke(sink, args);
			}
			catch (Exception ex)
			{
				if (SuspendUnhandledExceptionEventSemaphore <= 0)
				{
					UnhandledExceptionEventArgs e = new UnhandledExceptionEventArgs(ex);
					FireSync<UnhandledExceptionEventArgs>(UnhandledException, typeof(EventHelper), e);
				}
			}
		#endif
		}

		#endregion

		#region Safe Invoke On Current Thread
		//==========================================================================================
		// Safe Invoke On Current Thread
		//==========================================================================================

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		[OneWay]
		private static void InvokeOnCurrentThread(Delegate sink, object[] args)
		{
		#if (DEBUG_HANDLE_EXCEPTIONS) // Invoke event savely to ensure that program execution continues.
			try
			{
				sink.DynamicInvoke(args);
			}
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
		#elif (DEBUG_BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen.
			sink.DynamicInvoke(args);
		#else // NON-DEBUG: Forward or discard exception.
			try
			{
				sink.DynamicInvoke(args);
			}
			catch (Exception ex)
			{
				if (SuspendUnhandledExceptionEventSemaphore <= 0)
				{
					UnhandledExceptionEventArgs e = new UnhandledExceptionEventArgs(ex);
					FireSync<UnhandledExceptionEventArgs>(UnhandledException, typeof(EventHelper), e);
				}
			}
		#endif
		}

		#endregion

		#region Debug Output
		//==========================================================================================
		// Debug Output
		//==========================================================================================

#if (DEBUG)

		private static void WriteExceptionToDebugOutput(Exception ex, Delegate sink)
		{
			Diagnostics.DebugEx.WriteException(typeof(EventHelper), ex);
			Debug.Indent();
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Event: ");
				sb.Append(sink.Method.Name);
				sb.Append(" in ");
				sb.Append(sink.Target.GetType().ToString());
				sb.Append(".");
				Debug.WriteLine(sb.ToString());
			}
			Debug.Unindent();
		}

#endif // DEBUG

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
