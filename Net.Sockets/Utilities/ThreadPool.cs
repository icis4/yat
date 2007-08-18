/* ====================================================================
 * Copyright (c) 2006 Andre Luis Azevedo (az.andrel@yahoo.com.br)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer. 
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution.
 * 
 * 3. The name "ALAZ Library" must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * 4. Products derived from this software may not be called "ALAZ" nor 
 *    may "ALAZ Library" appear in their names without prior written 
 *    permission of the author.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace MKY.Net.Sockets.Utilities
{

    /// <summary>
    /// Callback items for HostThreadPool.
    /// </summary>
    internal class WaitCallbackItem
    {

        #region Fields

        private WaitCallback FWaitCallback;
        private ConnectionEventArgs FEventArgs;

        #endregion

        #region Constructor

        public WaitCallbackItem(WaitCallback waitCallback, ConnectionEventArgs e)
        {
            FWaitCallback = waitCallback;
            FEventArgs = e;
        }

        #endregion

        #region Properties

        public WaitCallback WaitCallBack
        {
            get { return FWaitCallback; }
        }

        public ConnectionEventArgs EventArgs
        {
            get { return FEventArgs; }
        }

        #endregion

    }

    /// <summary>
    /// Host thread pool for connection events.
    /// </summary>
    public class HostThreadPool : IDisposable
    {

		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		private bool FIsDisposed = false;

		#region Fields

        private object FSyncThreads;
        private int FNumThreads;
        private int FActiveThreads;

        private int FMaxThreads;
        private int FMinThreads;
        private BaseSocketConnectionHost FHost;

        private List<Thread> FThreads;
        private Queue<WaitCallbackItem> FCallbackItems;

        private bool FActive;
        private object FSyncActive;

        #endregion

        #region Constructor

		/// <summary></summary>
		public HostThreadPool(BaseSocketConnectionHost host, int minThreads, int maxThreads, int idleCheckInterval, int idleTimeOutValue)
        {

            FHost = host;

            FMaxThreads = maxThreads;
            FMinThreads = minThreads;

            FActiveThreads = 0;
            FNumThreads = 0;
            FSyncThreads = new object();

            FThreads = new List<Thread>(minThreads);
            FCallbackItems = new Queue<WaitCallbackItem>(50);

            FActive = false;
            FSyncActive = new object();

        
        }

        #endregion

		#region Disposal

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!FIsDisposed)
			{
				if (disposing)
				{
					FCallbackItems.Clear();
				}
				FIsDisposed = true;
			}
		}

		/// <summary></summary>
		~HostThreadPool()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (FIsDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (FIsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

        #region Methods

        #region Start

		/// <summary></summary>
		public void Start()
        {
			AssertNotDisposed();

			Active = true;
			for (int i = 0; i < FMinThreads; i++)
			{
				AddThread();
			}
		}

        #endregion

        #region AddThread

        private void AddThread()
        {

            Thread t = new Thread(Execute);
            t.IsBackground = true;
            t.Priority = ThreadPriority.Normal;

            FThreads.Add(t);
            FNumThreads++;

            t.Start();

        }

        #endregion

        #region Stop

		/// <summary></summary>
		public void Stop()
        {
			AssertNotDisposed();

			Active = false;
			lock (FCallbackItems)
			{
				Monitor.PulseAll(FCallbackItems);
			}
			lock (FThreads)
			{
				if (FThreads.Count > 0)
				{
					for (int i = 0; i < FThreads.Count; i++)
					{
						FThreads[i].Join(5000);
					}
				}
			}
		}

        #endregion

        #region Execute

        private void Execute()
        {
            WaitCallbackItem item = null;

            while (Active)
            {
                item = null;

                lock (FCallbackItems)
                {
                    if (FCallbackItems.Count > 0)
                    {
                        //----- if has items, dequeue!
                        item = FCallbackItems.Dequeue();
                    }
                    else
                    {
                        //----- if has no items, wait!
                        Monitor.Wait(FCallbackItems);
                    }

                }

                if (item != null)
                {
                    lock(FSyncThreads)
                    {

                        //---- Increment active threads!
                        FActiveThreads++;

                        if (FActiveThreads < FMaxThreads)
                        {

                            if (FActiveThreads == FNumThreads && FNumThreads < FMaxThreads)
                            {
                                //---- Add thread!
                                AddThread();
                            }
                        }
                    }

                    BaseSocketConnection connection = (BaseSocketConnection) item.EventArgs.Connection;

                    try
                    {
                        //----- Execute the callback method!
                        item.WaitCallBack(item.EventArgs);
                    }
                    catch (Exception exOuter)
                    {
						MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
						if (item.EventArgs is DisconnectedEventArgs)
                        {
                            //----- Disconnecting!
                            FHost.FireOnException(new ExceptionEventArgs(exOuter));
                        }
                        else
                        {
                            try
                            {
                                connection.BeginDisconnect(exOuter);
                            }
                            catch (Exception exInner)
                            {
								MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
								FHost.FireOnException(new ExceptionEventArgs(exInner));
                            }
                        }
                    }

                    lock (FSyncThreads)
                    {
                        //----- Decrement active threads!
                        FActiveThreads--;
                    }
                }
            }

            lock (FSyncThreads)
            {
                FNumThreads--;
            }
        }

        #endregion

        #region Enqueue

		/// <summary></summary>
		public void Enqueue(WaitCallback waitCallback, ConnectionEventArgs e)
        {

            lock (FCallbackItems)
            {
                FCallbackItems.Enqueue(new WaitCallbackItem(waitCallback, e));
                Monitor.Pulse(FCallbackItems);
            }
        
        }

        #endregion

        #endregion

        #region Properties

		/// <summary></summary>
        public bool Active
        {
            set
            {
                lock (FSyncActive)
                {
                    FActive = value;
                }
            }

            get
            {
                bool result = false;
                lock (FSyncActive)
                {
                    result = FActive;
                }                    
                return result;
            }
        }

        #endregion

    }

}
