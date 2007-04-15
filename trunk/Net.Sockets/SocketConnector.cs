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
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HSR.Net.Sockets
{

    /// <summary>
    /// Client socket creator.
    /// </summary>
    internal class SocketConnector : BaseSocketConnectionCreator
    {
        #region Fields

        private Socket FSocket;
		private IPEndPoint FRemoteEndPoint;
		private HostType FHostType;

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
		/// <param name="reconnectAttempts">
		/// Reconnect attempts.
		/// </param>
		/// <param name="reconnectAttemptInterval">
		/// Reconnect attempt interval.
		/// </param>
        /// <param name="localEndPoint">
        /// Local endpoint. if null, will be any address/port.
        /// </param>
        public SocketConnector(BaseSocketConnectionHost host, IPEndPoint remoteEndPoint, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, Cryptography.ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint)
            : base(host, localEndPoint, encryptType, compressionType, cryptoService)
        {
            FReconnectTimer = new Timer(new TimerCallback(ReconnectSocketConnection));
            FRemoteEndPoint = remoteEndPoint;
			FHostType = HostType.TcpClient;

            FReconnectAttempts = reconnectAttempts;
            FReconnectAttemptInterval = reconnectAttemptInterval;

            FReconnectAttempted = -1;
        }

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
		/// <param name="reconnectAttempts">
		/// Reconnect attempts.
		/// </param>
		/// <param name="reconnectAttemptInterval">
		/// Reconnect attempt interval.
		/// </param>
		/// <param name="localEndPoint">
		/// Local endpoint. if null, will be any address/port.
		/// </param>
		/// <param name="hostType">
		/// Host type, e.g. UDP.
		/// </param>
		public SocketConnector(BaseSocketConnectionHost host, IPEndPoint remoteEndPoint, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, Cryptography.ICryptoService cryptoService, int reconnectAttempts, int reconnectAttemptInterval, IPEndPoint localEndPoint, HostType hostType)
			: base(host, localEndPoint, encryptType, compressionType, cryptoService)
		{
			FReconnectTimer = new Timer(new TimerCallback(ReconnectSocketConnection));
			FRemoteEndPoint = remoteEndPoint;
			FHostType = hostType;

			FReconnectAttempts = reconnectAttempts;
			FReconnectAttemptInterval = reconnectAttemptInterval;

			FReconnectAttempted = -1;
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
					if (disposing)
					{
						FSocket.Close(1);
					}
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		#endregion

        #region Methods

        #region Start

        internal override void Start()
        {
			AssertNotDisposed();

			BeginConnect();
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
				switch (FHostType)
				{
					case HostType.Udp:
					{
						FSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
						break;
					}
					default:
					{
						FSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						break;
					}
				}
				FSocket.Bind(LocalEndPoint);
				FSocket.BeginConnect(FRemoteEndPoint, new AsyncCallback(BeginConnectCallback), this);
			}
            catch (Exception exOuter)
            {
				HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				Stop();
                Host.FireOnException(new ExceptionEventArgs(exOuter));
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
			if (!IsDisposed)
			{
				BaseSocketConnection connection = null;
				SocketConnector connector = null;

				try
				{
					connector = (SocketConnector)ar.AsyncState;
					if (!connector.IsDisposed)
					{
						connector.Socket.EndConnect(ar);

						//----- Adjust buffer size!
						connector.Socket.ReceiveBufferSize = Host.SocketBufferSize;
						connector.Socket.SendBufferSize = Host.SocketBufferSize;

						connection = new ClientSocketConnection(Host, this, connector.Socket, this.EncryptionType, this.CompressionType, Host.Header);

						//----- Initialize!
						Host.AddSocketConnection(connection);
						InitializeConnection(connection);
					}
				}
				catch (Exception exOuter)
				{
					HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
					if (!IsDisposed)
					{
						if (connection != null)
						{
							try
							{
								connection.BeginDisconnect(exOuter);
							}
							catch (Exception exInner)
							{
								HSR.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
								Host.FireOnException(new ExceptionEventArgs(exInner));
							}
						}
						else
						{
							Host.FireOnException(new ExceptionEventArgs(exOuter));
							FReconnectAttempted++;
							if (!IsDisposed)
								Reconnect(false);
						}
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
			AssertNotDisposed();

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
					Host.FireOnException(new ExceptionEventArgs(new ReconnectAttemptsException("Max reconnect attempts reached")));
				}
			}
			else
			{
				Stop();
			}
		}

        #endregion

        #region ReconnectSocketConnection

        private void ReconnectSocketConnection(Object stateInfo)
        {
			AssertNotDisposed();

			FReconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
			BeginConnect();
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
