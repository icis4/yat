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

using System.Threading;
using System.Xml.Serialization;
using System.Security.Cryptography;

using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;

using System.Diagnostics;

namespace HSR.Net.Sockets
{

    /// <summary>
    /// The connection host.
    /// </summary>
    public abstract class BaseSocketConnectionHost : IDisposable
    {

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool FIsDisposed = false;

		#region Fields

        private HostType FHostType;

        //----- Enumerates the connections and creators!
        private Dictionary<string, BaseSocketConnection> FSocketConnections;
		private List<BaseSocketConnectionCreator> FSocketCreators;

        //----- The Socket Service.
        private ISocketService FSocketService;

        private ManualResetEvent FWaitCreatorsDisposing;
        private ManualResetEvent FWaitConnectionsDisposing;
        private ManualResetEvent FWaitThreadsDisposing;

        private Timer FIdleTimer;
        private int FIdleCheckInterval;
        private int FIdleTimeOutValue;

        private byte[] FHeader;
        private int FMessageBufferSize;
        private int FSocketBufferSize;

        private Utilities.HostThreadPool FThreadPool;

        private event EventHandler<ExceptionEventArgs> FOnExceptionEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Base creator for BaseSocketConnectionHost.
        /// </summary>
        /// <param name="hostType">
        /// Host type.
        /// </param>
        /// <param name="socketService">
        /// Socket service.
        /// </param>
        /// <param name="header">
        /// Header byte array.
        /// </param>
        /// <param name="socketBufferSize">
        /// Socket buffer size.
        /// </param>
        /// <param name="messageBufferSize">
        /// Max message buffer size.
        /// </param>
        /// <param name="minThreads">
        /// Min threads in thread pool. if 0, the .Net thread pool will be used.
        /// </param>
        /// <param name="maxThreads">
        /// Max threads in thread pool. 
        /// </param>
        /// <param name="idleCheckInterval">
        /// Idle check interval timeout.
        /// </param>
        /// <param name="idleTimeOutValue">
        /// Idle connection timeout.
        /// </param>
        public BaseSocketConnectionHost(HostType hostType, ISocketService socketService, byte[] header, int socketBufferSize, int messageBufferSize, int minThreads, int maxThreads, int idleCheckInterval, int idleTimeOutValue)
        {

            FHostType = hostType;

            FSocketConnections = new Dictionary<string, BaseSocketConnection>();
            FSocketCreators = new List<BaseSocketConnectionCreator>();
            FSocketService = socketService;

            FWaitCreatorsDisposing = new ManualResetEvent(false);
            FWaitConnectionsDisposing = new ManualResetEvent(false);
            FWaitThreadsDisposing = new ManualResetEvent(false);

            FIdleTimer = new Timer(new TimerCallback(CheckSocketConnections));

            FIdleCheckInterval = idleCheckInterval;
            FIdleTimeOutValue = idleTimeOutValue;

            FHeader = header;
            FMessageBufferSize = messageBufferSize;
            FSocketBufferSize = socketBufferSize;

            if (minThreads > 0)
            {
                FThreadPool = new Utilities.HostThreadPool(this, minThreads, maxThreads, idleCheckInterval, idleTimeOutValue);
            }

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
					FIdleTimer.Dispose();

					FWaitCreatorsDisposing.Close();
					FWaitConnectionsDisposing.Close();

					foreach (BaseSocketConnection connection in FSocketConnections.Values)
					{
						connection.Dispose();
					}
					foreach (BaseSocketConnectionCreator creator in FSocketCreators)
					{
						creator.Stop();
						creator.Dispose();
					}

					FSocketConnections.Clear();
					FSocketCreators.Clear();
				}
				FIsDisposed = true;
			}
		}

		/// <summary></summary>
		~BaseSocketConnectionHost()
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

        /// <summary>
        /// Starts the base host.
        /// </summary>
        public void Start()
        {
			AssertNotDisposed();

			if (FThreadPool != null)
			{
				FThreadPool.Start();
			}
			foreach (BaseSocketConnectionCreator creator in FSocketCreators)
			{
				creator.Start();
			}
			FIdleTimer.Change(FIdleTimeOutValue, FIdleTimeOutValue);
		}

        #endregion

        #region Stop

        /// <summary>
        /// Stop the base host.
        /// </summary>
        public virtual void Stop()
        {
			AssertNotDisposed();

			if (FThreadPool != null)
			{
				FThreadPool.Stop();
			}
		}

        #endregion

        #region StopCreators

        /// <summary>
        /// Stop the host creators.
        /// </summary>
        protected void StopCreators()
        {
            //----- Stop Creators!
            BaseSocketConnectionCreator[] creators = GetSocketCreators();

            if (creators != null)
            {
                foreach (BaseSocketConnectionCreator creator in creators)
                {
                    try
                    {
                        creator.Stop();
                        RemoveCreator(creator);
                    }
                    catch (Exception ex)
					{
						HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, ex);
					}
                }

				if (GetSocketCreators().Length > 0)
                    FWaitCreatorsDisposing.WaitOne(30000, false);
            }
        }

        #endregion

        #region StopConnections

		/// <summary></summary>
        protected void StopConnections()
        {
			AssertNotDisposed();

			//----- Stop Connections!
			BaseSocketConnection[] connections = GetSocketConnections();

			if (connections != null)
			{
				foreach (BaseSocketConnection connection in connections)
				{
					try
					{
						connection.BeginDisconnect();
					}
					catch (Exception ex)
					{
						HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, ex);
					}
				}

				// ?!? / 2007-03-22 / Matthias Klaey
				// Why wait on thread here? Start() is non-blocking!
				// If Stop() is called from a GUI thread and the GUI is attached to
				//   the Disconnected event, a dead-lock happens:
				//   - The GUI thread is blocked here
				//   - FireOnDisconnected is blocked when trying to synchronize
				//     Invoke() onto the GUI thread

				//if (GetSocketConnections().Length > 0)
				//	FWaitConnectionsDisposing.WaitOne(3000000, false);
			}
		}

        #endregion

        #region Fire Methods

        #region FireOnConnected

        internal void FireOnConnected(BaseSocketConnection connection)
        {
            ConnectionEventArgs e = new ConnectionEventArgs(connection);

            if (FThreadPool != null)
                FThreadPool.Enqueue(new WaitCallback(OnConnectedCallback), e);
            else
                OnConnectedCallback(e);
        }

        private void OnConnectedCallback(object state)
        {
            FSocketService.OnConnected((ConnectionEventArgs)state);
            state = null;
        }

        #endregion

        #region FireOnSent

        private void FireOnSent(BaseSocketConnection connection, byte[] buffer)
        {
            MessageEventArgs e = new MessageEventArgs(connection, buffer);

            if (FThreadPool != null)
            {
                FThreadPool.Enqueue(new WaitCallback(OnSentCallback), e);
            }
            else
            {
                OnSentCallback(e);
            }
        }

        private void OnSentCallback(object state)
        {
            
            MessageEventArgs e = (MessageEventArgs)state;
            FSocketService.OnSent(e);

            e.Buffer = null;
            state = null;

        }

        #endregion

        #region FireOnReceived

        private void FireOnReceived(BaseSocketConnection connection, byte[] buffer, bool readCanEnqueue)
        {
        
            MessageEventArgs e = new MessageEventArgs(connection, buffer);
            e.CustomData = readCanEnqueue;

            if (FThreadPool != null)
            {
                FThreadPool.Enqueue(new WaitCallback(OnReceivedCallback), e);
            }
            else
            {
                OnReceivedCallback(e);
            }

        }

        private void OnReceivedCallback(object state)
        {

            MessageEventArgs e = (MessageEventArgs)state;
            BaseSocketConnection connection = (BaseSocketConnection) e.Connection;
            bool readCanEnqueue = (bool)e.CustomData;

            if (!readCanEnqueue)
            {

                lock (connection.SyncReadCount)
                {
                    connection.ReadCanEnqueue = false;
                }
            
            }

            FSocketService.OnReceived(e);
            e.Buffer = null;
            state = null;

            if (!readCanEnqueue)
            {

                lock (connection.SyncReadCount)
                {
                    connection.ReadCanEnqueue = true;
                }

            }
            
        }

        #endregion

        #region FireOnDisconnected

        private void FireOnDisconnected(DisconnectedEventArgs e)
        {

            if (FThreadPool != null)
            {
                FThreadPool.Enqueue(new WaitCallback(OnDisconnectedCallback), e);
            }
            else
            {
                OnDisconnectedCallback(e);
            }

        }

        private void OnDisconnectedCallback(object state)
        {

            DisconnectedEventArgs e = (DisconnectedEventArgs)state;
            BaseSocketConnection connection = (BaseSocketConnection)e.Connection;

            FSocketService.OnDisconnected(e);

            lock (connection)
            {
                RemoveSocketConnection(connection);
                connection.Dispose();
            }

            state = null;

        }

        #endregion

        #region FireOnException

		internal void FireOnException(ExceptionEventArgs args)
        {
            if (FOnExceptionEvent != null)
            {
                FOnExceptionEvent(this, args);
            }
        }

        #endregion

        #endregion

        #region Begin Methods

        #region BeginSend

        /// <summary>
        /// Begin send the data.
        /// </summary>
        internal void BeginSend(BaseSocketConnection connection, byte[] buffer)
        {
			AssertNotDisposed();

			try
			{
				if (connection.Active)
				{
					if (buffer.Length > FMessageBufferSize)
					{
						throw new MessageLengthException("Message length is greater than Host maximum message length.");
					}

					connection.LastAction = DateTime.Now;

					//----- Get the packet message!
					MessageBuffer writeMessage = MessageBuffer.GetPacketMessage(connection, ref buffer);

					lock (connection.WriteQueue)
					{

						if (connection.WriteQueueHasItems)
						{
							//----- If the connection is sending, enqueue the message!
							connection.WriteQueue.Enqueue(writeMessage);
						}
						else
						{

							//----- If the connection is not sending, send the message!
							connection.WriteQueueHasItems = true;

							if (connection.Stream != null)
							{
								//----- Ssl!
								connection.Stream.BeginWrite(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, new AsyncCallback(BeginSendCallback), new CallbackData(connection, writeMessage));
							}
							else
							{
								//----- Socket!
							connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), new CallbackData(connection, writeMessage));
							}
						}
					}
				}
			}
			catch (Exception exOuter)
			{
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				try
				{
					connection.BeginDisconnect(exOuter);
				}
				catch (Exception exInner)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
					FireOnException(new ExceptionEventArgs(exInner));
				}
			}
		}

        #endregion

        #region BeginSendCallback

        /// <summary>
        /// Send Callback.
        /// </summary>
        private void BeginSendCallback(IAsyncResult ar)
        {
			BaseSocketConnection connection = null;
			MessageBuffer writeMessage = null;
			byte[] sent = null;

			try
			{

				CallbackData callbackData = (CallbackData)ar.AsyncState;

				writeMessage = callbackData.Buffer;
				connection = callbackData.Connection;

				if (connection.Active)
				{
					if (connection.Stream != null)
					{
						//----- Ssl!
						connection.Stream.EndWrite(ar);

						sent = new byte[writeMessage.RawBuffer.Length];
						Array.Copy(writeMessage.RawBuffer, 0, sent, 0, writeMessage.RawBuffer.Length);
						FireOnSent(connection, sent);
					}
					else
					{
						//----- Socket!
						int writeBytes = connection.Socket.EndSend(ar);

						if (writeBytes < writeMessage.PacketBuffer.Length)
						{
							//----- Continue to send until all bytes are sent!
							writeMessage.PacketOffSet += writeBytes;
							connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), callbackData);
						}
						else
						{

							sent = new byte[writeMessage.RawBuffer.Length];
							Array.Copy(writeMessage.RawBuffer, 0, sent, 0, writeMessage.RawBuffer.Length);
							FireOnSent(connection, sent);
						}
					}

					//----- Check Queue!
					lock (connection.WriteQueue)
					{
						if (connection.WriteQueue.Count > 0)
						{
							//----- If has items, send it!
							MessageBuffer dequeueWriteMessage = connection.WriteQueue.Dequeue();

							if (connection.Stream != null)
							{
								//----- Ssl!
								connection.Stream.BeginWrite(dequeueWriteMessage.PacketBuffer, dequeueWriteMessage.PacketOffSet, dequeueWriteMessage.PacketRemaining, new AsyncCallback(BeginSendCallback), new CallbackData(connection, dequeueWriteMessage));
							}
							else
							{
								//----- Socket!
								connection.Socket.BeginSend(dequeueWriteMessage.PacketBuffer, dequeueWriteMessage.PacketOffSet, dequeueWriteMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginSendCallback), new CallbackData(connection, dequeueWriteMessage));
							}
						}
						else
						{
							connection.WriteQueueHasItems = false;
						}
					}
				}
			}
			catch (Exception exOuter)
			{
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				try
				{
					connection.BeginDisconnect(exOuter);
				}
				catch (Exception exInner)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
					FireOnException(new ExceptionEventArgs(exInner));
				}
			}
		}

        #endregion

        #region BeginReceive

        /// <summary>
        /// Receive data from connection.
        /// </summary>
        internal void BeginReceive(BaseSocketConnection connection)
        {
			AssertNotDisposed();

			try
			{
				if (connection.Active)
				{
					lock (connection.SyncReadCount)
					{
						if (connection.ReadCanEnqueue)
						{
							if (connection.ReadCount == 0)
							{
								//----- if the connection is not receiving, start the receive!
								MessageBuffer readMessage = new MessageBuffer(FSocketBufferSize);

								if (connection.Stream != null)
								{
									//----- Ssl!
									connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), new CallbackData(connection, readMessage));
								}
								else
								{
									//----- Socket!
									connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), new CallbackData(connection, readMessage));
								}
							}
							//----- Increase the read count!
							connection.ReadCount++;
						}
					}
				}
			}
			catch (Exception exOuter)
			{
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				try
				{
					connection.BeginDisconnect(exOuter);
				}
				catch (Exception exInner)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
					FireOnException(new ExceptionEventArgs(exInner));
				}
			}
		}

        #endregion

        #region BeginReadCallback

        private void BeginReadCallback(IAsyncResult ar)
        {
			if (!IsDisposed)
			{
				BaseSocketConnection connection = null;
				MessageBuffer readMessage = null;
				byte[] received = null;

				try
				{
					CallbackData callbackData = (CallbackData)ar.AsyncState;

					connection = callbackData.Connection;
					readMessage = callbackData.Buffer;

					if (connection.Active)
					{
						int readBytes = 0;

						if (connection.Stream != null)
						{
							//----- Ssl!
							readBytes = connection.Stream.EndRead(ar);
						}
						else
						{
							//----- Socket!
							readBytes = connection.Socket.EndReceive(ar);
						}

						if (readBytes > 0)
						{
							//----- Has bytes!
							byte[] rawBuffer = null;
							byte[] connectionHeader = connection.Header;

							readMessage.PacketOffSet += readBytes;

							if ((connectionHeader != null) && (connectionHeader.Length > 0))
							{

								//----- Message with header!
								int headerSize = connectionHeader.Length + 2;

								bool readPacket = false;
								bool readSocket = false;

								do
								{

									connection.LastAction = DateTime.Now;

									if (readMessage.PacketOffSet > headerSize)
									{

										//----- Has Header!
										for (int i = 0; i < connectionHeader.Length; i++)
										{
											if (connectionHeader[i] != readMessage.PacketBuffer[i])
											{
												//----- Bad Header!
												throw new BadHeaderException("Message header is different from Host header.");
											}

										}

										//----- Get Length!
										int messageLength = (readMessage.PacketBuffer[connectionHeader.Length] << 8) + readMessage.PacketBuffer[connectionHeader.Length + 1];

										if (messageLength > FMessageBufferSize)
										{
											throw new MessageLengthException("Message length is greater than Host maximum message length.");
										}

										//----- Check Length!
										if (messageLength == readMessage.PacketOffSet)
										{

											//----- Equal -> Get rawBuffer!
											rawBuffer = readMessage.GetRawBuffer(messageLength, headerSize);

											readPacket = false;
											readSocket = false;

										}
										else
										{

											if (messageLength < readMessage.PacketOffSet)
											{

												//----- Less -> Get rawBuffer and fire event!
												rawBuffer = readMessage.GetRawBuffer(messageLength, headerSize);

												//----- Decrypt!
												rawBuffer = Cryptography.Utilities.DecryptData(connection, ref rawBuffer, FMessageBufferSize);

												readPacket = true;
												readSocket = false;

												received = new byte[rawBuffer.Length];
												Array.Copy(rawBuffer, 0, received, 0, rawBuffer.Length);
												FireOnReceived(connection, received, false);

											}
											else
											{

												if (messageLength > readMessage.PacketOffSet)
												{

													//----- Greater -> Read Socket!
													if (messageLength > readMessage.PacketLength)
													{
														readMessage.Resize(messageLength);
													}

													readPacket = false;
													readSocket = true;

												}

											}

										}

									}
									else
									{

										if (readMessage.PacketRemaining < headerSize)
										{
											//----- Adjust room for more! 
											readMessage.Resize(readMessage.PacketLength + headerSize);
										}

										readPacket = false;
										readSocket = true;

									}

								} while (readPacket);

								if (readSocket)
								{

									//----- Read More!
									if (connection.Stream != null)
									{
										//----- Ssl!
										connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), callbackData);
									}
									else
									{
										//----- Socket!
										connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), callbackData);
									}

								}

							}
							else
							{

								//----- Message with no header!
								rawBuffer = readMessage.GetRawBuffer(readBytes, 0);

							}

							if (rawBuffer != null)
							{

								//----- Decrypt!
								rawBuffer = Cryptography.Utilities.DecryptData(connection, ref rawBuffer, FMessageBufferSize);

								//----- Fire Event!
								received = new byte[rawBuffer.Length];
								Array.Copy(rawBuffer, 0, received, 0, rawBuffer.Length);
								FireOnReceived(connection, received, true);

								readMessage.Resize(FSocketBufferSize);

								//----- Check Queue!
								lock (connection.SyncReadCount)
								{

									connection.ReadCount--;

									if (connection.ReadCount > 0)
									{

										//----- if the read queue has items, start to receive!
										if (connection.Stream != null)
										{
											//----- Ssl!
											connection.Stream.BeginRead(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, new AsyncCallback(BeginReadCallback), callbackData);
										}
										else
										{
											//----- Socket!
											connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(BeginReadCallback), callbackData);
										}
									}
								}
							}
						}
						else
						{
							//----- Is has no data to read then the connection has been terminated!
							connection.BeginDisconnect();
						}
					}
				}
				catch (Exception exOuter)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
					try
					{
						connection.BeginDisconnect(exOuter);
					}
					catch (Exception exInner)
					{
						HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
						FireOnException(new ExceptionEventArgs(exInner));
					}
				}
			}
		}

        #endregion

        #region BeginDisconnect

        /// <summary>
        /// Begin disconnect the connection
        /// </summary>
        internal void BeginDisconnect(BaseSocketConnection connection, Exception exception)
        {
			AssertNotDisposed();

			DisconnectedEventArgs e = new DisconnectedEventArgs(connection, exception);

			try
			{
				if (connection.Active)
				{
					connection.Socket.BeginDisconnect(false, new AsyncCallback(BeginDisconnectCallback), e);
				}
			}
			catch (Exception exOuter)
			{
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				try
				{
					FireOnDisconnected(e);
				}
				catch (Exception exInner)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
					FireOnException(new ExceptionEventArgs(exInner));
				}
			}
		}

        #endregion

        #region BeginDisconnectCallback

        /// <summary>
        /// Disconnect callback.
        /// </summary>
        private void BeginDisconnectCallback(IAsyncResult ar)
        {
			BaseSocketConnection connection = null;
			DisconnectedEventArgs e = null;

			try
			{
				e = (DisconnectedEventArgs)ar.AsyncState;
				connection = (BaseSocketConnection)e.Connection;

				if (connection.Active)
				{
					connection.Socket.EndDisconnect(ar);
					if (!IsDisposed)
						FireOnDisconnected(e);
				}
			}
			catch (Exception exOuter)
			{
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				if (!IsDisposed)
				{
					try
					{
						FireOnDisconnected(e);
					}
					catch (Exception exInner)
					{
						HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
						FireOnException(new ExceptionEventArgs(exInner));
					}
				}
			}
		}

        #endregion

        #region Abstract Methods

        internal abstract void BeginReconnect(ClientSocketConnection connection);
        internal abstract void BeginSendToAll(ServerSocketConnection connection, byte[] buffer);
        internal abstract void BeginSendTo(BaseSocketConnection connectionTo, byte[] buffer);
        internal abstract BaseSocketConnection GetConnectionById(string connectionId);

        #endregion

        #endregion

        #region Connection Methods

        #region AddSocketConnection

        internal void AddSocketConnection(BaseSocketConnection socketConnection)
        {
			AssertNotDisposed();

			lock (FSocketConnections)
			{
				FSocketConnections.Add(socketConnection.ConnectionId, socketConnection);
			}
		}

        #endregion

        #region RemoveSocketConnection

        internal void RemoveSocketConnection(BaseSocketConnection socketConnection)
        {
			AssertNotDisposed();

			lock (FSocketConnections)
			{
				FSocketConnections.Remove(socketConnection.ConnectionId);
				if (FSocketConnections.Count <= 0)
				{
					FWaitConnectionsDisposing.Set();
				}
			}
		}

        #endregion

        #region GetSocketConnections

        internal BaseSocketConnection[] GetSocketConnections()
        {
			AssertNotDisposed();

            BaseSocketConnection[] items = null;
			lock (FSocketConnections)
			{
				items = new BaseSocketConnection[FSocketConnections.Count];
				FSocketConnections.Values.CopyTo(items, 0);
			}
            return items;
        }

        #endregion

        #region GetSocketConnectionById

        internal BaseSocketConnection GetSocketConnectionById(string connectionId)
        {
			AssertNotDisposed();

            BaseSocketConnection item = null;
			lock (FSocketConnections)
			{
				item = FSocketConnections[connectionId];
			}
            return item;
        }

        #endregion

        #region CheckSocketConnections

        private void CheckSocketConnections(Object stateInfo)
        {

            //----- Disable timer event!
            FIdleTimer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {

                //----- Get connections!
                BaseSocketConnection[] items = GetSocketConnections();

                if (items != null)
                {

                    foreach (BaseSocketConnection cnn in items)
                    {

                        try
                        {

                            if (cnn != null)
                            {

                                //----- Check the idle timeout!
                                if (DateTime.Now > (cnn.LastAction.AddMilliseconds(FIdleTimeOutValue)))
                                {
                                    cnn.BeginDisconnect();
                                }

                            }

                        }
                        catch (Exception ex)
                        {
							HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, ex);
						}
                    }

                }
            }
            finally
            {
                //----- Restart the timer event!
                FIdleTimer.Change(FIdleCheckInterval, FIdleCheckInterval);
            }

        }

        #endregion

        #endregion

        #region Creators Methods

        #region AddCreator

		/// <summary></summary>
        protected void AddCreator(BaseSocketConnectionCreator creator)
        {
			AssertNotDisposed();

			lock (FSocketCreators)
			{
				FSocketCreators.Add(creator);
			}
		}

        #endregion

        #region RemoveCreator

		/// <summary></summary>
		protected void RemoveCreator(BaseSocketConnectionCreator creator)
        {
			AssertNotDisposed();
		
			lock (FSocketCreators)
			{
				FSocketCreators.Remove(creator);
				if (FSocketCreators.Count <= 0)
				{
					FWaitCreatorsDisposing.Set();
				}
			}
		}

        #endregion

        #region GetSocketCreators

		/// <summary></summary>
		public BaseSocketConnectionCreator[] GetSocketCreators()
        {
			AssertNotDisposed();

            BaseSocketConnectionCreator[] items = null;
			lock (FSocketCreators)
			{
				items = new BaseSocketConnectionCreator[FSocketCreators.Count];
				FSocketCreators.CopyTo(items, 0);
			}
			return items;
        }

        #endregion

        #endregion

        #endregion

        #region Properties

		/// <summary></summary>
		internal int SocketBufferSize
        {
            get { return FSocketBufferSize; }
        }

		/// <summary></summary>
		internal byte[] Header
        {
            get { return FHeader; }
        }

		/// <summary></summary>
        protected ISocketService SocketService
        {
            get { return FSocketService; }
        }

		/// <summary></summary>
		protected Timer CheckTimeOutTimer
        {
            get { return CheckTimeOutTimer; }
        }

		/// <summary></summary>
		public int IdleCheckInterval
        {
            get { return FIdleCheckInterval; }
        }

		/// <summary></summary>
		public int IdleTimeOutValue
        {
            get { return FIdleTimeOutValue; }
        }

		/// <summary></summary>
		public HostType HostType
        {
            get { return FHostType; }
        }

		/// <summary></summary>
		public event EventHandler<ExceptionEventArgs> OnException
        {
            add { FOnExceptionEvent += value; }
            remove { FOnExceptionEvent -= value; }
        }

        #endregion

    }

}
