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
using System.Net.Sockets;

namespace MKY.Net.Sockets
{

    /// <summary>
    /// Server connection implementation.
    /// </summary>
    internal class ServerSocketConnection : BaseSocketConnection, IServerSocketConnection
    {

        #region Constructor

        internal ServerSocketConnection(BaseSocketConnectionHost host, Socket socket, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, byte[] header)
            : base(host, socket, encryptType, compressionType, header)
        {
            //-----
        }

        #endregion

        #region ISocketConnection Members

        public override IClientSocketConnection AsClientConnection()
        {
            return null;
        }

        public override IServerSocketConnection AsServerConnection()
        {
            return (this as IServerSocketConnection);
        }

        #endregion

        #region IServerSocketConnection Members

        public void BeginSendToAll(byte[] buffer)
        {
            Host.BeginSendToAll(this, buffer);
        }

        public void BeginSendTo(ISocketConnection connection, byte[] buffer)
        {
            Host.BeginSendTo((BaseSocketConnection)connection, buffer);
        }

        public ISocketConnection GetConnectionById(string connectionId)
        {
            return Host.GetConnectionById(connectionId);
        }

        #endregion

    }

}