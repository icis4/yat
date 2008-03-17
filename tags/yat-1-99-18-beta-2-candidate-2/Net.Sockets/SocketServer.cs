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
using System.Threading;
using System.Net;

namespace MKY.Net.Sockets
{

    /// <summary>
    /// Server connection host.
    /// </summary>
    public class SocketServer : BaseSocketConnectionHost
    {

        #region Constructor

		/// <summary></summary>
		public SocketServer(ISocketService socketService)
            : base(HostType.TcpServer, socketService, null, 2048, 8192, 0, 0, 60000, 60000)
        {
            //-----
        }

		/// <summary></summary>
		public SocketServer(ISocketService socketService, byte[] header)
            : base(HostType.TcpServer, socketService, header, 2048, 8192, 0, 0, 60000, 60000)
        {
            //-----
        }

		/// <summary></summary>
		public SocketServer(ISocketService socketService, byte[] header, int socketBufferSize, int messageBufferSize, int minThreads, int maxThreads)
            : base(HostType.TcpServer, socketService, header, socketBufferSize, messageBufferSize, minThreads, maxThreads, 60000, 60000)
        {
            //-----
        }

		/// <summary></summary>
		public SocketServer(ISocketService socketService, byte[] header, int socketBufferSize, int messageBufferSize, int minThreads, int maxThreads, int idleCheckInterval, int idleTimeOutValue)
            : base(HostType.TcpServer, socketService, header, socketBufferSize, messageBufferSize, minThreads, maxThreads, idleCheckInterval, idleTimeOutValue)
        {
            //-----
        }

        #endregion

		#region Disposal

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				try
				{
					// add objects to dispose
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		#endregion

        #region Methods

        #region BeginReconnect

        internal override void BeginReconnect(ClientSocketConnection connection) { }

        #endregion

        #region BeginSendToAll

        internal override void BeginSendToAll(ServerSocketConnection connection, byte[] buffer)
        {
			AssertNotDisposed();

			BaseSocketConnection[] items = GetSocketConnections();

			if (items != null)
			{
				foreach (BaseSocketConnection cnn in items)
				{
					if (connection != cnn)
					{
						BeginSend(cnn, buffer);
					}
				}
			}
		}

        #endregion

        #region BeginSendTo

        internal override void BeginSendTo(BaseSocketConnection connection, byte[] buffer)
        {
			AssertNotDisposed();
			BeginSend(connection, buffer);
		}

        #endregion

        #region GetConnectionById

        internal override BaseSocketConnection GetConnectionById(string connectionId)
        {
			AssertNotDisposed();
            return (GetSocketConnectionById(connectionId));
        }

        #endregion

        #region AddListener

        /// <summary>
        /// Add the server connector (SocketListener).
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void AddListener(IPEndPoint localEndPoint)
        {
            AddListener(localEndPoint, Cryptography.EncryptionType.None, Compression.CompressionType.None, null, 5, 2);
        }

		/// <summary>
		/// Add the server connector (SocketListener).
		/// </summary>
		public void AddListener(IPEndPoint localEndPoint, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, Cryptography.ICryptoService cryptoService)
        {
            AddListener(localEndPoint, encryptType, compressionType, cryptoService, 5, 2);
        }

		/// <summary>
		/// Add the server connector (SocketListener).
		/// </summary>
		public void AddListener(IPEndPoint localEndPoint, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, Cryptography.ICryptoService cryptoService, byte backLog, byte acceptThreads)
        {
			AssertNotDisposed();
			AddCreator(new SocketListener(this, localEndPoint, encryptType, compressionType, cryptoService, backLog, acceptThreads));
		}

        #endregion

        #region Stop

		/// <summary></summary>
        public override void Stop()
        {
			AssertNotDisposed();

			StopCreators();
			StopConnections();

			base.Stop();
        }

        #endregion

        #endregion

    }
}
