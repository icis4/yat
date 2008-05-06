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
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ALAZ.SystemEx.SocketsEx
{

    //----- SocketsEx declarations!

    #region Exceptions

    /// <summary>
    /// Max reconnect attempts reached.
    /// </summary>
    public class ReconnectAttemptsException : Exception
    {
        public ReconnectAttemptsException(string message) : base(message) { }
    }

    /// <summary>
    /// Bad Header.
    /// </summary>
    public class BadHeaderException : Exception
    {
        public BadHeaderException(string message): base(message) { }
    }

    /// <summary>
    /// Message length is greater than the maximum value.
    /// </summary>
    public class MessageLengthException : Exception
    {
        public MessageLengthException(string message): base(message) { }
    }

    /// <summary>
    /// Symmetric authentication failure.
    /// </summary>
    public class SymmetricAuthenticationException: Exception
    {
        public SymmetricAuthenticationException(string message) : base(message) { } 
    }

    /// <summary>
    /// SSL authentication failure.
    /// </summary>
    public class SSLAuthenticationException : Exception
    {
        public SSLAuthenticationException(string message) : base(message) { }
    }

    #endregion 

    #region Delegates

    /// <summary>
    /// Exception delegate.
    /// </summary>
    /// <param name="ex"> 
    /// Exception raised.
    /// </param>
    public delegate void OnExceptionDelegate(Exception ex);

    #endregion

    #region Structures

    #region AuthMessage

    public struct AuthMessage
    {
        public byte[] SessionKey;
        public byte[] SessionIV;
        public byte[] SourceKey;
        public byte[] Sign;
    }

    #endregion

    #endregion

    #region Enums

    #region HostType

    /// <summary>
    /// Defines the host type.
    /// </summary>
    public enum HostType
    {
        htServer,
        htClient
    }

    #endregion

    #region EncryptType

    /// <summary>
    /// Defines the encrypt method used.
    /// </summary>
    public enum EncryptType
    {
        etNone,
        etBase64,
        etTripleDES,
        etRijndael,
        etSSL,
    }

    #endregion

    #region CompressionType

    /// <summary>
    /// Defines the compression method used.
    /// </summary>
    public enum CompressionType
    {
        ctNone,
        ctGZIP
    }

    #endregion

    #endregion

    #region Interfaces

    #region SocketConnection

    #region ISocketConnection

    /// <summary>
    /// Common connection properties and methods.
    /// </summary>
    public interface ISocketConnection
    {

        /// <summary>
        /// Connection custom data.
        /// </summary>
        object CustomData
        {
            get;
            set;
        }

        /// <summary>
        /// Connection service header.
        /// </summary>
        byte[] Header
        {
            get;
            set;
        }

        /// <summary>
        /// Connection Session Id (GUID).
        /// </summary>
        string ConnectionId
        {
            get;
        }

        /// <summary>
        /// Handle of the OS Socket.
        /// </summary>
        IntPtr SocketHandle
        {
            get;
        }

        /// <summary>
        /// Local socket endpoint.
        /// </summary>
        IPEndPoint LocalEndPoint
        {
            get;
        }

        /// <summary>
        /// Remote socket endpoint.
        /// </summary>
        IPEndPoint RemoteEndPoint
        {
            get;
        }

        /// <summary>
        /// Connection encrypt type.
        /// </summary>
        EncryptType EncryptType
        {
            get;
        }

        /// <summary>
        /// Connection compression type.
        /// </summary>
        CompressionType CompressionType
        {
            get;
        }

        /// <summary>
        /// Connection host type.
        /// </summary>
        HostType HostType
        {
            get;
        }

        /// <summary>
        /// Represents the connection as a IClientSocketConnection.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        IClientSocketConnection AsClientConnection();

        /// <summary>
        /// Represents the connection as a IServerSocketConnection.
        /// </summary>
        /// <returns></returns>
        IServerSocketConnection AsServerConnection();

        /// <summary>
        /// Begin send data.
        /// </summary>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        void BeginSend(byte[] buffer);

        /// <summary>
        /// Begin receive the data.
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// Begin disconnect the connection.
        /// </summary>
        void BeginDisconnect();

    }

    #endregion

    #region IClientSocketConnection

    /// <summary>
    /// Client connection methods.
    /// </summary>
    public interface IClientSocketConnection: ISocketConnection
    {

        /// <summary>
        /// Begin reconnect the connection.
        /// </summary>
        /// <param name="sleepTimeOutValue"></param>
        void BeginReconnect();
    }

    #endregion

    #region IServerSocketConnection

    /// <summary>
    /// Server connection methods.
    /// </summary>
    public interface IServerSocketConnection: ISocketConnection
    {

        /// <summary>
        /// Begin send data to all server connections.
        /// </summary>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        void BeginSendToAll(byte[] buffer);

        /// <summary>
        /// Begin send data to the connection.
        /// </summary>
        /// <param name="connection">
        /// The connection that the data will be sent.
        /// </param>
        /// <param name="buffer">
        /// Data to be sent.
        /// </param>
        void BeginSendTo(ISocketConnection connection, byte[] buffer);


        /// <summary>
        /// Get the connection from the connectionId
        /// </summary>
        /// <param name="connectionId">
        /// The connectionId.
        /// </param>
        /// <returns>
        /// ISocketConnection to use.
        /// </returns>
        ISocketConnection GetConnectionById(string connectionId);
    
    }

    #endregion

    #endregion

    #region ISocketService

    /// <summary>
    /// Socket service methods.
    /// </summary>
    public interface ISocketService
    {
        /// <summary>
        /// Fired when connected.
        /// </summary>
        /// <param name="e">
        /// Information about the connection.
        /// </param>
        void OnConnected(ConnectionEventArgs e);

        /// <summary>
        /// Fired when data arrives.
        /// </summary>
        /// <param name="e">
        /// Information about the Message.
        /// </param>
        void OnReceived(MessageEventArgs e);

        /// <summary>
        /// Fired when data is sent.
        /// </summary>
        /// <param name="e">
        /// Information about the Message.
        /// </param>
        void OnSent(MessageEventArgs e);

        /// <summary>
        /// Fired when disconnected.
        /// </summary>
        /// <param name="e">
        /// Information about the connection.
        /// </param>
        void OnDisconnected(DisconnectedEventArgs e);

    }

    #endregion

    #region ICryptoService

    /// <summary>
    /// Crypto service methods.
    /// </summary>
    public interface ICryptoService
    {
        
        /// <summary>
        /// Fired when symmetric encryption is used.
        /// </summary>
        /// <param name="serverKey">
        /// The RSA provider used to encrypt symmetric IV and Key.
        /// </param>
        void OnSymmetricAuthenticate(HostType hostType, out RSACryptoServiceProvider serverKey);

        /// <summary>
        /// Fired when SSL encryption is used in client host.
        /// </summary>
        /// <param name="ServerName">
        /// The host name in certificate.
        /// </param>
        /// <param name="certs">
        /// The certification collection to be used (null if not using client certification).
        /// </param>
        /// <param name="checkRevocation">
        /// Indicates if the certificated must be checked for revocation.
        /// </param>
        void OnSSLClientAuthenticate(out string ServerName, ref X509Certificate2Collection certs, ref bool checkRevocation);

        /// <summary>
        /// Fired when SSL encryption is used in server host.
        /// </summary>
        /// <param name="certificate">
        /// The certificate to be used.
        /// </param>
        /// <param name="clientAuthenticate">
        /// Indicates if client connection will be authenticated (uses certificate).
        /// </param>
        /// <param name="checkRevocation">
        /// Indicates if the certificated must be checked for revocation.
        /// </param>
        void OnSSLServerAuthenticate(out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation);

    }

    #endregion

    #endregion

    #region Classes

    #region BaseSocketService

    /// <summary>
    /// Base class for ISocketServive. Use it overriding the virtual methods.
    /// </summary>
    public abstract class BaseSocketService : ISocketService
    {

        #region ISocketService Members

        public virtual void OnConnected(ConnectionEventArgs e) { }
        public virtual void OnSent(MessageEventArgs e) { }
        public virtual void OnReceived(MessageEventArgs e) { }
        public virtual void OnDisconnected(DisconnectedEventArgs e) { }

        #endregion

    }

    #endregion

    #region BaseCreatorService

    /// <summary>
    /// Base class for ICryptoServive. Use it overriding the virtual methods.
    /// </summary>
    public abstract class BaseCryptoService : ICryptoService
    {

        #region ICryptoService Members

        public virtual void OnSymmetricAuthenticate(HostType hostType, out RSACryptoServiceProvider serverKey)
        {
            serverKey = new RSACryptoServiceProvider();
            serverKey.Clear();
        }

        public virtual void OnSSLClientAuthenticate(out string ServerName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {
            ServerName = String.Empty;

        }
        public virtual void OnSSLServerAuthenticate(out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation)
        {
            certificate = new X509Certificate2();
            clientAuthenticate = true;
        }

        #endregion

    }

    #endregion

    #endregion

}