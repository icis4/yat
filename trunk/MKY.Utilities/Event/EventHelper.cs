//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether exceptions should be handled or execution immediately stopped:
// - Uncomment to handle exceptions
// - Comment out to break exceptions
#define HANDLE_EXCEPTIONS

#if (!HANDLE_EXCEPTIONS) // Break exceptions is mutual exclusive against handle exceptions
	#define BREAK_EXCEPTIONS
#endif

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.IO;

#endregion

namespace MKY.Utilities.Event
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
				#if (DEBUG && HANDLE_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen
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
				#elif (DEBUG && BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen
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
		public static void FireSync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
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
				#if (DEBUG && HANDLE_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen
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
				#elif (DEBUG && BREAK_EXCEPTIONS) // Invoke event directly so exceptions can be debugged where they happen
					EventHandler<TEventArgs> castedSink = (EventHandler<TEventArgs>)sink;
					object sender = args[0];
					TEventArgs eventArgs = (TEventArgs)args[1];
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
		/// <remarks>
		/// This overloaded method is provided for backward compatibility with .NET 1.0/1.1 style events.
		/// </remarks>
		public static void FireSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			if (eventDelegate == null)
				return;

			// Events of type "event MyEventHandler MyEvent" are always fired the non-debug way
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

			// invoke event in a safe way for DEBUG and NON-DEBUG
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if (sinkTarget != null)          // no need to check for InvokeRequired,
				{                                //   async always requires invoke
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

		[OneWay]
		private static void InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, object[] args)
		{
			try
			{
				sinkTarget.Invoke(sink, args);
			}
			catch (Exception ex)
			{
			#if (DEBUG) // Output as much data as possible for debugging support
				WriteExceptionToDebugOutput(ex, sink);
			#else // NON-DEBUG: Forward or discard exception
				UnhandledExceptionEventArgs e = new UnhandledExceptionEventArgs(ex);
				FireSync<UnhandledExceptionEventArgs>(UnhandledException, typeof(EventHelper), e);
			#endif
			}
		}

		#endregion

		#region Safe Invoke On Current Thread
		//==========================================================================================
		// Safe Invoke On Current Thread
		//==========================================================================================

		[OneWay]
		private static void InvokeOnCurrentThread(Delegate sink, object[] args)
		{
			try
			{
				sink.DynamicInvoke(args);
			}
			catch (Exception ex)
			{
			#if (DEBUG) // output as much data as possible for debugging support
				WriteExceptionToDebugOutput(ex, sink);
			#else // NON-DEBUG, forward or discard exception
				UnhandledExceptionEventArgs e = new UnhandledExceptionEventArgs(ex);
				FireSync<UnhandledExceptionEventArgs>(UnhandledException, typeof(EventHelper), e);
			#endif
			}
		}

		#endregion

		#region Debug Output
		//==========================================================================================
		// Debug Output
		//==========================================================================================

	#if (DEBUG)

		private static void WriteExceptionToDebugOutput(Exception ex, Delegate sink)
		{
			Diagnostics.XDebug.WriteException(typeof(EventHelper), ex);
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

	#endif

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
