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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace HSR.Net.Sockets
{

    /// <summary>
    /// Base socket connection
    /// </summary>
    public abstract class BaseSocketConnection : IDisposable, ISocketConnection
    {

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool FIsDisposed = false;

        #region Fields

        //----- Socket and Stream items!
        private Socket FSocket;
        private Stream FStream;

        //----- Connection Host!
        private BaseSocketConnectionHost FHost;

        private object FCustomData;
        private byte[] FHeader;
        private string FId;

        //----- Write items!
        private Queue<MessageBuffer> FWriteQueue;
        private bool FWriteQueueHasItems;

        //----- Read items!
        private object FSyncReadCount;
        private int FReadCount;
        private bool FReadCanEnqueue;

        private DateTime FLastAction;

        private ICryptoTransform FDecryptor;
        private ICryptoTransform FEncryptor;

        private Cryptography.EncryptionType FEncryptionType;
		private Compression.CompressionType FCompressionType;

        #endregion

        #region Constructor

        internal BaseSocketConnection(BaseSocketConnectionHost host, Socket socket, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, byte[] header)
        {

            //----- Connection Id!
            FId = Guid.NewGuid().ToString();

            FSocket = socket;

            FEncryptionType = encryptType;
            FCompressionType = compressionType;
            
            FWriteQueue = new Queue<MessageBuffer>();
            FWriteQueueHasItems = false;
            FReadCanEnqueue = true;

            FReadCount = 0;
            FSyncReadCount = new object();

            FHeader = header;
            FHost = host;

            FLastAction = DateTime.Now;

            FCustomData = null;
            FEncryptor = null;
            FDecryptor = null;
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
					FWriteQueue.Clear();

					if (FStream != null)
					{
						FStream.Close();
						FStream.Dispose();
					}
				}
				FIsDisposed = true;
			}
		}

		/// <summary></summary>
		~BaseSocketConnection()
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

        #region Properties

		/// <summary></summary>
		internal Queue<MessageBuffer> WriteQueue
        {
            get { return FWriteQueue; }
        }

		/// <summary></summary>
		internal bool WriteQueueHasItems
        {
            get { return FWriteQueueHasItems; }
            set { FWriteQueueHasItems = value; }
        }

		/// <summary></summary>
		internal bool ReadCanEnqueue
        {
            get { return FReadCanEnqueue; }
            set { FReadCanEnqueue = value; }
        }

		/// <summary></summary>
		internal int ReadCount
        {
            get { return FReadCount; }
            set { FReadCount = value; }
        }

		/// <summary></summary>
		internal object SyncReadCount
        {
            get { return FSyncReadCount; }
        }

		/// <summary></summary>
		internal bool Active
        {
            get { return !FIsDisposed; }
        }

		/// <summary></summary>
		internal ICryptoTransform Encryptor
        {
            get { return FEncryptor; }
            set { FEncryptor = value; }
        }

		/// <summary></summary>
		internal ICryptoTransform Decryptor
        {
            get { return FDecryptor; }
            set { FDecryptor = value; }
        }

		/// <summary></summary>
		internal Stream Stream
        {
            get { return FStream; }
            set { FStream = value; }
        }

		/// <summary></summary>
		internal DateTime LastAction
        {
            get { return FLastAction; }
            set { FLastAction = value; }
        }

		/// <summary></summary>
		internal Socket Socket
        {
            get { return FSocket; }
        }

		/// <summary></summary>
		protected BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

        #endregion

        #region ISocketConnection Members

        #region Properties

		/// <summary></summary>
		public object CustomData
        {
            get { return FCustomData; }
            set { FCustomData = value; }
        }

		/// <summary></summary>
		public byte[] Header
        {
            get { return FHeader; }
            set { FHeader = value; }
        }

		/// <summary></summary>
		public Cryptography.EncryptionType EncryptionType
        {
            get { return FEncryptionType; }
        }

		/// <summary></summary>
		public Compression.CompressionType CompressionType
        {
            get { return FCompressionType; }
        }

		/// <summary></summary>
		public HostType HostType
        {
            get { return FHost.HostType; }
        }

		/// <summary></summary>
		public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)FSocket.LocalEndPoint; }
        }

		/// <summary></summary>
		public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)FSocket.RemoteEndPoint; }
        }

		/// <summary></summary>
		public IntPtr SocketHandle
        {
            get { return FSocket.Handle; }
        }

		/// <summary></summary>
		public string ConnectionId
        {
            get { return FId; }
        }

        #endregion

        #region Abstract Methods

		/// <summary></summary>
		public abstract IClientSocketConnection AsClientConnection();

		/// <summary></summary>
		public abstract IServerSocketConnection AsServerConnection();

        #endregion

        #region Send Methods

		/// <summary></summary>
		public void BeginSend(byte[] buffer)
        {
            FHost.BeginSend(this, buffer);
        }

        #endregion

        #region Receive Methods

		/// <summary></summary>
		public void BeginReceive()
        {
            FHost.BeginReceive(this);
        }

        #endregion

        #region Disconnect Methods

		/// <summary></summary>
		public void BeginDisconnect()
        {
            FHost.BeginDisconnect(this, null);
        }

		/// <summary></summary>
		internal void BeginDisconnect(Exception ex)
        {
            FHost.BeginDisconnect(this, ex);
        }

        #endregion

        #endregion

    }
}
