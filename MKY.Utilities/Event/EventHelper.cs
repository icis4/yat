// Choose whether exceptions should be handled or execution immediately stopped
#define HANDLE_EXCEPTIONS
//#define BREAK_EXCEPTIONS

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
		#region Unhandled Exception Callback
		//==========================================================================================
		// Unhandled Exception Callback
		//==========================================================================================

		/// <summary>
		/// Unhandled exception callback delegate.
		/// </summary>
		public delegate void UnhandledExceptionCallback(Exception ex);

		private static UnhandledExceptionCallback _unhandledExceptionCallback = null;
		private static ISynchronizeInvoke _unhandledExceptionCallbackTarget = null;

		/// <summary>
		/// Installs a callback that is called on unhandled exceptions.
		/// </summary>
		public static void InstallUnhandledExceptionCallback(UnhandledExceptionCallback callback)
		{
			InstallUnhandledExceptionCallback(callback, null);
		}

		/// <summary>
		/// Installs a callback that is called on unhandled exceptions.
		/// </summary>
		public static void InstallUnhandledExceptionCallback(UnhandledExceptionCallback callback, ISynchronizeInvoke target)
		{
			_unhandledExceptionCallback = callback;
			_unhandledExceptionCallbackTarget = target;
		}

		/// <summary>
		/// Installs the callback that is called on unhandled exceptions.
		/// </summary>
		public static void UninstallUnhandledExceptionCallback()
		{
			_unhandledExceptionCallback = null;
			_unhandledExceptionCallbackTarget = null;
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
				#if (DEBUG && HANDLE_EXCEPTIONS) // invoke event directly so exceptions can be debugged where they happen
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
				#elif (DEBUG && BREAK_EXCEPTIONS) // invoke event directly so exceptions can be debugged where they happen
					EventHandler castedSink = (EventHandler)sink;
					object sender = args[0];
					EventArgs eventArgs = (EventArgs)args[1];
					castedSink(sender, eventArgs);
				#else // NON-DEBUG: invoke event the safe way
					InvokeOnCurrentThread(sink, args);
				#endif
				}
			}
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is
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
				#if (DEBUG && HANDLE_EXCEPTIONS) // invoke event directly so exceptions can be debugged where they happen
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
				#elif (DEBUG && BREAK_EXCEPTIONS) // invoke event directly so exceptions can be debugged where they happen
					EventHandler<TEventArgs> castedSink = (EventHandler<TEventArgs>)sink;
					object sender = args[0];
					TEventArgs eventArgs = (TEventArgs)args[1];
					castedSink(sender, eventArgs);
				#else // NON-DEBUG: invoke event the safe way
					InvokeOnCurrentThread(sink, args);
				#endif
				}
			}
		}

		/// <summary>
		/// Fires event with supplied arguments synchronously. Event is
		/// fired safely, exceptions are caught. If an event sink implements
		/// <see cref="System.ComponentModel.ISynchronizeInvoke"/>,
		/// the event is invoked on that thread. Otherwise, the event is
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
			#if (DEBUG) // output as much data as possible for debugging support
				WriteExceptionToDebugOutput(ex, sink);
			#else // NON-DEBUG, forward or discard exception
				if (_unhandledExceptionCallback != null)
				{
					try
					{
						if ((_unhandledExceptionCallbackTarget != null) && (_unhandledExceptionCallbackTarget.InvokeRequired))
							_unhandledExceptionCallbackTarget.Invoke(_unhandledExceptionCallback, new object[] { ex });
						else
							_unhandledExceptionCallback.Invoke(ex, null, null);
					}
					catch { }
				}
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
				if (_unhandledExceptionCallback != null)
				{
					try
					{
						if ((_unhandledExceptionCallbackTarget != null) && (_unhandledExceptionCallbackTarget.InvokeRequired))
							_unhandledExceptionCallbackTarget.Invoke(_unhandledExceptionCallback, new object[] { ex });
						else
							_unhandledExceptionCallback.Invoke(ex, null, null);
					}
					catch { }
				}
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
