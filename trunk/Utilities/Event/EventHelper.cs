using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.IO;

namespace MKY.Utilities.Event
{
	/// <summary>
	/// Events helper methods to defensivly publish events.
	/// </summary>
	public static class EventHelper
	{
		#region Sync Event Invoking
		//------------------------------------------------------------------------------------------
		// Sync Event Invoking
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise the event is
		/// invoked on the current thread.
		/// </summary>
		public static void FireSync(Delegate eventDelegate, params object[] args)
		{
			if (eventDelegate == null)
				return;

			// invoke event in a safe way
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
				#if (DEBUG) // invoke event directly so exceptions can be debugged where they happen
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
				#else // NON-DEBUG: invoke event the safe way
					InvokeOnSameThread(sink, args);
				#endif
				}
			}
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise the event is
		/// invoked on the current thread.
		/// </summary>
		public static void FireSync<TEventArgs>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			if (eventDelegate == null)
				return;

			// invoke event in a safe way
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
				#if (DEBUG) // invoke event directly so exceptions can be debugged where they happen
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
				#else // NON-DEBUG: invoke event the safe way
					InvokeOnSameThread(sink, args);
				#endif
				}
			}
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise the event is
		/// invoked on the current thread.
		/// </summary>
		public static void FireSync<TEventArgs, TEventHandler>(Delegate eventDelegate, params object[] args)
			where TEventArgs : EventArgs
		{
			if (eventDelegate == null)
				return;

			// events of type "event MyEventHandler MyEvent" are always fired the non-debug way
			Delegate[] sinks = eventDelegate.GetInvocationList();
			foreach (Delegate sink in sinks)
			{
				ISynchronizeInvoke sinkTarget = sink.Target as ISynchronizeInvoke;
				if ((sinkTarget != null) && (sinkTarget.InvokeRequired))
					InvokeSynchronized(sinkTarget, sink, args);
				else
					InvokeOnSameThread(sink, args);
			}
		}

		#endregion

		#region Async Event Invoking
		//------------------------------------------------------------------------------------------
		// Async Event Invoking
		//------------------------------------------------------------------------------------------

		private delegate void AsyncInvokeDelegate(Delegate eventDelegate, object[] args);

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise the event is
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
					AsyncInvokeDelegate asyncInvoker = new AsyncInvokeDelegate(InvokeOnSameThread); ;
					asyncInvoker.BeginInvoke(sink, args, null, null);
				}
			}
		}

		#endregion

		#region Safe Invoke Synchronized To ISynchronizeInvoke
		//------------------------------------------------------------------------------------------
		// Safe Invoke Synchronized To ISynchronizeInvoke
		//------------------------------------------------------------------------------------------

		[OneWay]
		private static void InvokeSynchronized(ISynchronizeInvoke sinkTarget, Delegate sink, object[] args)
		{
			try
			{
				sinkTarget.Invoke(sink, args);
			}
		#if (DEBUG) // output as much data as possible for debugging support
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
		#else // NON-DEBUG, discard exception
			catch
			{
			}
		#endif
		}

		#endregion

		#region Safe Invoke On Same Thread
		//------------------------------------------------------------------------------------------
		// Safe Invoke On Same Thread
		//------------------------------------------------------------------------------------------

		[OneWay]
		private static void InvokeOnSameThread(Delegate sink, object[] args)
		{
			try
			{
				sink.DynamicInvoke(args);
			}
		#if (DEBUG) // output as much data as possible for debugging support
			catch (Exception ex)
			{
				WriteExceptionToDebugOutput(ex, sink);
			}
		#else // NON-DEBUG, discard exception
			catch
			{
			}
		#endif
		}

		#endregion

		#region Debug Output
		//------------------------------------------------------------------------------------------
		// Debug Output
		//------------------------------------------------------------------------------------------

	#if (DEBUG)

		private static void WriteExceptionToDebugOutput(Exception ex, Delegate sink)
		{
			Diagnostics.XDebug.WriteException(typeof(EventHelper), ex);
			Debug.Indent();
			{
				Debug.Write("Event: ");
				Debug.Write(sink.Method.Name);
				Debug.Write(" in ");
				Debug.WriteLine(sink.Target.GetType().ToString());
			}
			Debug.Unindent();
		}

	#endif

		#endregion
	}
}
