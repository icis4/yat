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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Threading;

#endregion

namespace MKY.Event
{
	/// <summary>
	/// Events helper methods to defensivly publish events.
	/// </summary>
	public static class EventHelper
	{
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
			try
			{
				sinkTarget.Invoke(sink, args);
			}
#if (DEBUG) // DEBUG: Output as much data as possible to help debugging.
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
#else // NON-DEBUG: Discard exception.
			catch
			{
			}
#endif // DEBUG
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
			try
			{
				sink.DynamicInvoke(args);
			}
#if (DEBUG) // DEBUG: Output as much data as possible to help debugging.
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
#else // NON-DEBUG: Discard exception.
			catch
			{
			}
#endif // DEBUG
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
