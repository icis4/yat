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
using System.Security;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ALAZ.SystemEx.SocketsEx
{

    /// <summary>
    /// Connection creator using in BaseSocketConnectionHost.
    /// </summary>
    public abstract class BaseSocketConnectionCreator : BaseClass
    {

        #region Fields

        //----- Local endpoint of creator!
        private IPEndPoint FLocalEndPoint;

        //----- Host!
        private BaseSocketConnectionHost FHost;

        private EncryptType FEncryptType;
        private CompressionType FCompressionType;

        private ICryptoService FCryptoService;

        //----- Sign Message!
        private byte[] signMessage = new byte[] 
                                    { 0xD8, 0xC0, 0x81, 0x40, 0xC3, 
                                        0x03, 0xC9, 0x09, 0x80, 0xC8, 
                                        0xD8, 0x1F, 0x38, 0xB9, 0x8A, 
                                        0x4F, 0x8D, 0x59, 0xE3, 0x92,
                                        0x20, 0x3F, 0xCA, 0x91, 0xCD,
                                        0x39, 0x7A, 0x2E, 0x5B, 0xA8,
                                        0xB7, 0x59 };

        #endregion

        #region Constructor

        public BaseSocketConnectionCreator(BaseSocketConnectionHost host, IPEndPoint localEndPoint, EncryptType encryptType, CompressionType compressionType, ICryptoService cryptoService)
        {

            FHost = host;
            FLocalEndPoint = localEndPoint;
            FCompressionType = compressionType;
            FEncryptType = encryptType;

            FCryptoService = cryptoService;

        }

        #endregion

        #region Methods

        #region InitializeConnection

        /// <summary>
        /// Initializes the connection with encryption.
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void InitializeConnection(BaseSocketConnection connection)
        {

            if (!Disposed)
            {

                if (FCryptoService != null)
                {

                    //----- None!
                    if (connection.EncryptType == EncryptType.etNone || connection.EncryptType == EncryptType.etBase64)
                    {
                        FHost.FireOnConnected(connection);
                    }

                    //----- Symmetric!
                    if (connection.EncryptType == EncryptType.etRijndael || connection.EncryptType == EncryptType.etTripleDES)
                    {

                        if (FHost.HostType == HostType.htClient)
                        {

                            //----- Get RSA provider!
                            RSACryptoServiceProvider serverPublicKey;
                            RSACryptoServiceProvider clientPrivateKey = new RSACryptoServiceProvider();

                            FCryptoService.OnSymmetricAuthenticate(FHost.HostType, out serverPublicKey);

                            //----- Generates symmetric algoritm!
                            SymmetricAlgorithm sa = CryptUtils.CreateSymmetricAlgoritm(connection.EncryptType);
                            sa.GenerateIV();
                            sa.GenerateKey();

                            //----- Adjust connection cryptors!
                            connection.Encryptor = sa.CreateEncryptor();
                            connection.Decryptor = sa.CreateDecryptor();

                            //----- Create authenticate structure!
                            AuthMessage am = new AuthMessage();
                            am.SessionIV = serverPublicKey.Encrypt(sa.IV, false);
                            am.SessionKey = serverPublicKey.Encrypt(sa.Key, false);
                            am.SourceKey = CryptUtils.EncryptDataForAuthenticate(sa, Encoding.UTF8.GetBytes(clientPrivateKey.ToXmlString(false)), PaddingMode.ISO10126);

                            //----- Sign message with am.SourceKey, am.SessionKey and signMessage!
                            //----- Need to use PaddingMode.PKCS7 in sign!
                            MemoryStream m = new MemoryStream();
                            m.Write(am.SourceKey, 0, am.SourceKey.Length);
                            m.Write(am.SessionKey, 0, am.SessionKey.Length);
                            m.Write(signMessage, 0, signMessage.Length);
                            
                            am.Sign = clientPrivateKey.SignData(CryptUtils.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider());

                            //----- Serialize authentication message!
                            XmlSerializer xml = new XmlSerializer(typeof(AuthMessage));
                            m.SetLength(0);
                            xml.Serialize(m, am);

                            //----- Send structure!
                            MessageBuffer mb = new MessageBuffer(0);
                            mb.PacketBuffer = Encoding.Default.GetBytes(Convert.ToBase64String(m.ToArray()));
                            connection.Socket.BeginSend(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionSendCallback), new CallbackData(connection, mb));

                            m.Dispose();
                            am.SessionIV.Initialize();
                            am.SessionKey.Initialize();
                            serverPublicKey.Clear();
                            clientPrivateKey.Clear();

                        }
                        else
                        {

                            //----- Create empty authenticate structure!
                            MessageBuffer mb = new MessageBuffer(8192);

                            //----- Start receive structure!
                            connection.Socket.BeginReceive(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionReceiveCallback), new CallbackData(connection, mb));

                        }

                    }

                    //----- Asymmetric!
                    if (connection.EncryptType == EncryptType.etSSL)
                    {

                        if (FHost.HostType == HostType.htClient)
                        {

                            //----- Get SSL items!
                            X509Certificate2Collection certs = null;
                            string serverName = null;
                            bool checkRevocation = true;

                            FCryptoService.OnSSLClientAuthenticate(out serverName, ref certs, ref checkRevocation);

                            //----- Authneticate SSL!
                            SslStream ssl = new SslStream(new NetworkStream(connection.Socket));

                            if (certs == null)
                            {
                                ssl.BeginAuthenticateAsClient(serverName, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htClient));
                            }
                            else
                            {
                                ssl.BeginAuthenticateAsClient(serverName, certs, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htClient));

                            }

                        }
                        else
                        {

                            //----- Get SSL items!
                            X509Certificate2 cert = null;
                            bool clientAuthenticate = false;
                            bool checkRevocation = true;

                            FCryptoService.OnSSLServerAuthenticate(out cert, out clientAuthenticate, ref checkRevocation);

                            //----- Authneticate SSL!
                            SslStream ssl = new SslStream(new NetworkStream(connection.Socket));
                            ssl.BeginAuthenticateAsServer(cert, clientAuthenticate, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.htServer));

                        }

                    }

                }
                else
                {
                    //----- No encryption - Authenticate!
                    FHost.FireOnConnected(connection);
                }

            }
        }

        #endregion

        #region InitializeConnectionSendCallback

        private void InitializeConnectionSendCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                MessageBuffer writeMessage = null;

                try
                {

                    CallbackData callbackData = (CallbackData)ar.AsyncState;

                    connection = callbackData.Connection;
                    writeMessage = callbackData.Buffer;

                    if (connection.Active)
                    {

                        //----- Socket!
                        int writeBytes = connection.Socket.EndSend(ar);

                        if (writeBytes < writeMessage.PacketBuffer.Length)
                        {
                            //----- Continue to send until all bytes are sent!
                            writeMessage.PacketOffSet += writeBytes;
                            connection.Socket.BeginSend(writeMessage.PacketBuffer, writeMessage.PacketOffSet, writeMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionSendCallback), callbackData);
                        }
                        else
                        {
                            FHost.FireOnConnected(connection);
                        }

                    }

                }
                catch (Exception exOut)
                {

                    try
                    {
                        connection.BeginDisconnect(exOut);
                    }
                    catch (Exception exInn)
                    {
                        FHost.FireOnException(exInn); ;
                    }

                }
            }

        }

        #endregion

        #region InitializeConnectionReceiveCallback

        private void InitializeConnectionReceiveCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                MessageBuffer readMessage = null;

                try
                {

                    CallbackData callbackData = (CallbackData)ar.AsyncState;

                    connection = callbackData.Connection;
                    readMessage = callbackData.Buffer;

                    if (connection.Active)
                    {

                        bool readSocket = true;
                        int readBytes = connection.Socket.EndReceive(ar);

                        if (readBytes > 0)
                        {

                            readMessage.PacketOffSet += readBytes;
                            byte[] message = null;

                            try
                            {
                                message = Convert.FromBase64String(Encoding.Default.GetString(readMessage.PacketBuffer, 0, readMessage.PacketOffSet));
                            }
                            catch (FormatException)
                            {
                                //----- Base64 transformation error!
                            }

                            if ((message != null) && (Encoding.Default.GetString(message).Contains("</AuthMessage>")))
                            {

                                //----- Get RSA provider!
                                RSACryptoServiceProvider serverPrivateKey;
                                RSACryptoServiceProvider clientPublicKey = new RSACryptoServiceProvider();

                                FCryptoService.OnSymmetricAuthenticate(FHost.HostType, out serverPrivateKey);

                                //----- Deserialize authentication message!
                                MemoryStream m = new MemoryStream();
                                m.Write(message, 0, message.Length);
                                m.Position = 0;

                                XmlSerializer xml = new XmlSerializer(typeof(AuthMessage));
                                AuthMessage am = (AuthMessage)xml.Deserialize(m);

                                //----- Generates symmetric algoritm!
                                SymmetricAlgorithm sa = CryptUtils.CreateSymmetricAlgoritm(connection.EncryptType);
                                sa.Key = serverPrivateKey.Decrypt(am.SessionKey, false);
                                sa.IV = serverPrivateKey.Decrypt(am.SessionIV, false);

                                //----- Adjust connection cryptors!
                                connection.Encryptor = sa.CreateEncryptor();
                                connection.Decryptor = sa.CreateDecryptor();

                                //----- Verify sign!
                                clientPublicKey.FromXmlString(Encoding.UTF8.GetString(CryptUtils.DecryptDataForAuthenticate(sa, am.SourceKey, PaddingMode.ISO10126)));

                                m.SetLength(0);
                                m.Write(am.SourceKey, 0, am.SourceKey.Length);
                                m.Write(am.SessionKey, 0, am.SessionKey.Length);
                                m.Write(signMessage, 0, signMessage.Length);

                                if (!clientPublicKey.VerifyData(CryptUtils.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider(), am.Sign))
                                {
                                    throw new SymmetricAuthenticationException("Symmetric sign error.");
                                }
 
                                readSocket = false;


                                m.Dispose();
                                am.SessionIV.Initialize();
                                am.SessionKey.Initialize();
                                serverPrivateKey.Clear();
                                clientPublicKey.Clear();

                                FHost.FireOnConnected(connection);

                            }

                            if (readSocket)
                            {
                                connection.Socket.BeginReceive(readMessage.PacketBuffer, readMessage.PacketOffSet, readMessage.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionReceiveCallback), callbackData);
                            }

                        }
                        else
                        {
                            throw new SymmetricAuthenticationException("Symmetric authentication error.");
                        }

                    }

                }
                catch (Exception exOut)
                {

                    try
                    {
                        connection.BeginDisconnect(exOut);
                    }
                    catch (Exception exInn)
                    {
                        FHost.FireOnException(exInn); ;
                    }

                }

            }

        }

        #endregion

        #region SslAuthenticateCallback

        private void SslAuthenticateCallback(IAsyncResult ar)
        {

            if (!Disposed)
            {

                BaseSocketConnection connection = null;
                SslStream stream = null;

                try
                {

                    AuthenticateCallbackData callbackData = (AuthenticateCallbackData)ar.AsyncState;

                    connection = callbackData.Connection;
                    stream = callbackData.Stream;

                    if (connection.Active)
                    {

                        if (callbackData.HostType == HostType.htClient)
                        {
                            stream.EndAuthenticateAsClient(ar);
                        }
                        else
                        {
                            stream.EndAuthenticateAsServer(ar);
                        }

                        if (!(stream.IsSigned && stream.IsEncrypted))
                        {
                            throw new SSLAuthenticationException("Ssl authenticate is not signed or not encrypted.");
                        }

                        connection.Stream = stream;

                        FHost.FireOnConnected(connection);

                    }

                }
                catch (Exception exOut)
                {

                    try
                    {
                        connection.BeginDisconnect(exOut);
                    }
                    catch (Exception exInn)
                    {
                        FHost.FireOnException(exInn); ;
                    }

                }

            }

        }

        #endregion

        #region Abstract Methods

        internal abstract void Start();
        internal abstract void Stop();

        #endregion

        #endregion

        #region Properties

        internal BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

        protected ICryptoService CryptoService
        {
            get { return FCryptoService; }
        }

        public EncryptType EncryptType
        {
            get { return FEncryptType; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return FLocalEndPoint; }
        }

        public CompressionType CompressionType
        {
            get { return FCompressionType; }
        }

        #endregion

    }

}
