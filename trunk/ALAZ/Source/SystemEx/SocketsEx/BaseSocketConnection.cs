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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace ALAZ.SystemEx.SocketsEx
{

    /// <summary>
    /// Base socket connection
    /// </summary>
    public abstract class BaseSocketConnection : BaseClass, ISocketConnection
    {

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

        private EncryptType FEncryptType;
        private CompressionType FCompressionType;

        #endregion

        #region Constructor

        internal BaseSocketConnection(BaseSocketConnectionHost host, Socket socket, EncryptType encryptType, CompressionType compressionType, byte[] header)
        {

            //----- Connection Id!
            FId = Guid.NewGuid().ToString();

            FSocket = socket;

            FEncryptType = encryptType;
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

        #region Free

        protected override void Free(bool dispodedByUser)
        {

            if (dispodedByUser)
            {

                FWriteQueue.Clear();

                if (FStream != null)
                {
                    FStream.Close();
                    FStream.Dispose();
                }

            }

            base.Free(dispodedByUser);

        }

        #endregion

        #region Properties

        internal Queue<MessageBuffer> WriteQueue
        {
            get { return FWriteQueue; }
        }

        internal bool WriteQueueHasItems
        {
            get { return FWriteQueueHasItems; }
            set { FWriteQueueHasItems = value; }
        }

        internal bool ReadCanEnqueue
        {
            get { return FReadCanEnqueue; }
            set { FReadCanEnqueue = value; }
        }

        internal int ReadCount
        {
            get { return FReadCount; }
            set { FReadCount = value; }
        }

        internal object SyncReadCount
        {
            get { return FSyncReadCount; }
        }

        internal bool Active
        {
            get { return !Disposed; }
        }

        internal ICryptoTransform Encryptor
        {
            get { return FEncryptor; }
            set { FEncryptor = value; }
        }

        internal ICryptoTransform Decryptor
        {
            get { return FDecryptor; }
            set { FDecryptor = value; }
        }

        internal Stream Stream
        {
            get { return FStream; }
            set { FStream = value; }
        }

        internal DateTime LastAction
        {
            get { return FLastAction; }
            set { FLastAction = value; }
        }

        internal Socket Socket
        {
            get { return FSocket; }
        }

        protected BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

        #endregion

        #region ISocketConnection Members

        #region Properties

        public object CustomData
        {
            get { return FCustomData; }
            set { FCustomData = value; }
        }

        public byte[] Header
        {
            get { return FHeader; }
            set { FHeader = value; }
        }

        public EncryptType EncryptType
        {
            get { return FEncryptType; }
        }

        public CompressionType CompressionType
        {
            get { return FCompressionType; }
        }

        public HostType HostType
        {
            get { return FHost.HostType; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint)FSocket.LocalEndPoint; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return (IPEndPoint)FSocket.RemoteEndPoint; }
        }

        public IntPtr SocketHandle
        {
            get { return FSocket.Handle; }
        }

        public string ConnectionId
        {
            get { return FId; }
        }

        #endregion

        #region Abstract Methods

        public abstract IClientSocketConnection AsClientConnection();
        public abstract IServerSocketConnection AsServerConnection();

        #endregion

        #region Send Methods

        public void BeginSend(byte[] buffer)
        {
            FHost.BeginSend(this, buffer);
        }

        #endregion

        #region Receive Methods

        public void BeginReceive()
        {
            FHost.BeginReceive(this);
        }

        #endregion

        #region Disconnect Methods

        public void BeginDisconnect()
        {
            FHost.BeginDisconnect(this, null);
        }

        internal void BeginDisconnect(Exception ex)
        {
            FHost.BeginDisconnect(this, ex);
        }

        #endregion

        #endregion

    }

}
