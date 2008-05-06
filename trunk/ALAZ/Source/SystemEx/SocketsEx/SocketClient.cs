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

namespace ALAZ.SystemEx.SocketsEx
{

    /// <summary>
    /// Socket client host.
    /// </summary>
    public class SocketClient : BaseSocketConnectionHost
    {

        #region Constructor

        public SocketClient(ISocketService socketService)
            : base(HostType.htClient, socketService, null, 2048, 8192, 0, 0, 60000, 60000)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, byte[] header)
            : base(HostType.htClient, socketService, header, 2048, 8192, 0, 0, 60000, 60000)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, byte[] header, int socketBufferSize, int messageBufferSize, int minThreads, int maxThreads)
            : base(HostType.htClient, socketService, header, socketBufferSize, messageBufferSize, minThreads, maxThreads, 60000, 60000)
        {
            //-----
        }

        public SocketClient(ISocketService socketService, byte[] header, int socketBufferSize, int messageBufferSize, int minThreads, int maxThreads, int idleCheckInterval, int idleTimeOutValue)
            : base(HostType.htClient, socketService, header, socketBufferSize, messageBufferSize, minThreads, maxThreads, idleCheckInterval, idleTimeOutValue)
        {
            //-----
        }

        #endregion

        #region Methods

        #region BeginReconnect

        /// <summary>
        /// Reconnects the connection adjusting the reconnect timer.
        /// </summary>
        /// <param name="connection">
        /// Connection.
        /// </param>
        /// <param name="sleepTimeOutValue">
        /// Sleep timeout before reconnect.
        /// </param>
        internal override void BeginReconnect(ClientSocketConnection connection)
        {
            if (!Disposed)
            {
                connection.Creator.Reconnect(true);
            }

        }

        #endregion

        #region BeginSendToAll

        internal override void BeginSendToAll(ServerSocketConnection connection, byte[] buffer) { }

        #endregion

        #region BeginSendTo

        internal override void BeginSendTo(BaseSocketConnection connectionTo, byte[] buffer) { }

        #endregion

        #region GetConnectionByHandle

        internal override BaseSocketConnection GetConnectionById(string connectionId) { return null; }

        #endregion

        #region AddClient

        /// <summary>
        /// Adds the client connector (SocketConnector).
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        public void AddConnector(IPEndPoint remoteEndPoint)
        {
            AddConnector(remoteEndPoint, EncryptType.etNone, CompressionType.ctNone, null, 0, 0, new IPEndPoint(IPAddress.Any, 0));
        }

        public void AddConnector(IPEndPoint remoteEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService)
        {
            AddConnector(remoteEndPoint, encryptType, compressionType, cryptoService, 0, 0, new IPEndPoint(IPAddress.Any, 0));
        }

        public void AddConnector(IPEndPoint remoteEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval)
        {
            AddCreator(new SocketConnector(this, remoteEndPoint, encryptType, compressionType, cryptoService, reconnectAttempts, reconnectAttemptInterval, new IPEndPoint(IPAddress.Any, 0)));
        }

        public void AddConnector(IPEndPoint remoteEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint)
        {
            if (!Disposed)
            {
                AddCreator(new SocketConnector(this, remoteEndPoint, encryptType, compressionType, cryptoService, reconnectAttempts, reconnectAttemptInterval, localEndPoint));
            }
        }

        #endregion

        #region Stop

        public override void Stop()
        {

            if (!Disposed)
            {

                StopConnections();
                StopCreators();

            }

            base.Stop();

        }

        #endregion

        #endregion

    }

}
