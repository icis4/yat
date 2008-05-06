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
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY
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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ALAZ.SystemEx.SocketsEx
{

    /// <summary>
    /// Client socket creator.
    /// </summary>
    internal class SocketConnector : BaseSocketConnectionCreator
    {

        #region Fields

        private Socket FSocket;
        private IPEndPoint FRemoteEndPoint;

        private Timer FReconnectTimer;
        private int FReconnectAttempts;
        private int FReconnectAttemptInterval;

        private int FReconnectAttempted;

        #endregion

        #region Constructor

        /// <summary>
        /// Base SocketConnector creator.
        /// </summary>
        /// <param name="host">
        /// Host.
        /// </param>
        /// <param name="remoteEndPoint">
        /// The remote endpoint to connect.
        /// </param>
        /// <param name="encryptType">
        /// Encrypt type.
        /// </param>
        /// <param name="compressionType">
        /// Compression type.
        /// </param>
        /// <param name="cryptoService">
        /// CryptoService. if null, will not be used.
        /// </param>
        /// <param name="localEndPoint">
        /// Local endpoint. if null, will be any address/port.
        /// </param>
        public SocketConnector(BaseSocketConnectionHost host, IPEndPoint remoteEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint)
            : base(host, localEndPoint, encryptType, compressionType, cryptoService)
        {

            FReconnectTimer = new Timer(new TimerCallback(ReconnectSocketConnection));
            FRemoteEndPoint = remoteEndPoint;

            FReconnectAttempts = reconnectAttempts;
            FReconnectAttemptInterval = reconnectAttemptInterval;

            FReconnectAttempted = -1;

        }

        #endregion

        #region Free

        protected override void Free(bool dispodedByUser)
        {

            if (dispodedByUser)
            {
                FSocket.Close();
            }

            base.Free(dispodedByUser);
        }

        #endregion

        #region Methods

        #region Start

        internal override void Start()
        {

            if (!Disposed)
            {
                BeginConnect();
            }

        }

        #endregion

        #region BeginConnect

        /// <summary>
        /// Begin the connection with host.
        /// </summary>
        internal void BeginConnect()
        {

            try
            {
                FSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                FSocket.Bind(LocalEndPoint);
                FSocket.BeginConnect(FRemoteEndPoint, new AsyncCallback(BeginConnectCallback), this);
            }
            catch (Exception exOut)
            {
                Stop();
                Host.FireOnException(exOut);
            }

        }

        #endregion

        #region BeginConnectCallback

        /// <summary>
        /// Connect callback!
        /// </summary>
        /// <param name="ar"></param>
        internal void BeginConnectCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                SocketConnector connector = null;

                try
                {

                    connector = (SocketConnector)ar.AsyncState;
                    connector.Socket.EndConnect(ar);

                    //----- Adjust buffer size!
                    connector.Socket.ReceiveBufferSize = Host.SocketBufferSize;
                    connector.Socket.SendBufferSize = Host.SocketBufferSize;

                    connection = new ClientSocketConnection(Host, this, connector.Socket, this.EncryptType, this.CompressionType, Host.Header);

                    //----- Initialize!
                    Host.AddSocketConnection(connection);
                    InitializeConnection(connection);

                }
                catch (Exception exOut)
                {

                    if (connection != null)
                    {

                        try
                        {
                            connection.BeginDisconnect(exOut);
                        }
                        catch (Exception exInn)
                        {
                            Host.FireOnException(exInn);
                        }

                    }
                    else
                    {
                        
                        Host.FireOnException(exOut);
                        FReconnectAttempted++;
                        Reconnect(false);

                    }

                }
            }
        }

        #endregion

        #region Stop

        internal override void Stop()
        {
            Dispose();
        }

        #endregion

        #region Reconnect

        internal void Reconnect(bool resetAttempts)
        {
            
            if (!Disposed)
            {

                if (resetAttempts)
                {
                    FReconnectAttempted = 0;
                }

                if (FReconnectAttempts > 0)
                {

                    if (FReconnectAttempted < FReconnectAttempts)
                    {
                        FReconnectTimer.Change(FReconnectAttemptInterval, FReconnectAttemptInterval);
                    }
                    else
                    {
                        Stop();
                        Host.FireOnException(new ReconnectAttemptsException("Max reconnect attempts reached"));
                    }

                }
                else
                {
                    Stop();
                }

            }
        }

        #endregion

        #region ReconnectSocketConnection

        private void ReconnectSocketConnection(Object stateInfo)
        {

            if (!Disposed)
            {
                FReconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                BeginConnect();
            }

        }

        #endregion

        #endregion

        #region Properties

        internal Socket Socket
        {
            get { return FSocket; }
        }

        #endregion

    }

}
