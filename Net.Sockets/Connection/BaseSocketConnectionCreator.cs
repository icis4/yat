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
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MKY.Net.Sockets
{
	/// <summary></summary>
	public struct AuthMessage
	{
		/// <summary></summary>
		public byte[] SessionKey;

		/// <summary></summary>
		public byte[] SessionIV;

		/// <summary></summary>
		public byte[] SourceKey;

		/// <summary></summary>
		public byte[] Sign;
	}

    /// <summary>
    /// Connection creator using in BaseSocketConnectionHost.
    /// </summary>
    public abstract class BaseSocketConnectionCreator : IDisposable
    {
		#region Fields

		private bool FIsDisposed = false;

		//----- Local endpoint of creator!
        private IPEndPoint FLocalEndPoint;

        //----- Host!
        private BaseSocketConnectionHost FHost;

        private Cryptography.EncryptionType FEncryptionType;
        private Compression.CompressionType FCompressionType;

		private Cryptography.ICryptoService FCryptoService;

        //----- Sign Message!
        private readonly byte[] FSignMessage = new byte[]
			{
				0xD8, 0xC0, 0x81, 0x40, 0xC3, 0x03, 0xC9, 0x09,
				0x80, 0xC8, 0xD8, 0x1F, 0x38, 0xB9, 0x8A, 0x4F,
				0x8D, 0x59, 0xE3, 0x92, 0x20, 0x3F, 0xCA, 0x91,
				0xCD, 0x39, 0x7A, 0x2E, 0x5B, 0xA8, 0xB7, 0x59,
			};

        #endregion

        #region Constructor

		/// <summary></summary>
		public BaseSocketConnectionCreator(BaseSocketConnectionHost host, IPEndPoint localEndPoint, Cryptography.EncryptionType encryptType, Compression.CompressionType compressionType, Cryptography.ICryptoService cryptoService)
        {
            FHost = host;
            FLocalEndPoint = localEndPoint;
            FCompressionType = compressionType;
            FEncryptionType = encryptType;

            FCryptoService = cryptoService;
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
					// add objects to dispose
				}
				FIsDisposed = true;
			}
		}

		/// <summary></summary>
		~BaseSocketConnectionCreator()
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

        #region InitializeConnection

        /// <summary>
        /// Initializes the connection with encryption.
        /// </summary>
        /// <param name="connection"></param>
        protected virtual void InitializeConnection(BaseSocketConnection connection)
        {
			AssertNotDisposed();

			if (FCryptoService != null)
			{

				//----- None!
				if (connection.EncryptionType == Cryptography.EncryptionType.None || connection.EncryptionType == Cryptography.EncryptionType.Base64)
				{
					FHost.FireOnConnected(connection);
				}

				//----- Symmetric!
				if (connection.EncryptionType == Cryptography.EncryptionType.Rijndael || connection.EncryptionType == Cryptography.EncryptionType.TripleDES)
				{
					switch (FHost.HostType)
					{
						case HostType.TcpClient:
						{
							//----- Get RSA provider!
							RSACryptoServiceProvider serverPublicKey;
							RSACryptoServiceProvider clientPrivateKey = new RSACryptoServiceProvider();

							FCryptoService.OnSymmetricAuthenticate(FHost.HostType, out serverPublicKey);

							//----- Generates symmetric algoritm!
							SymmetricAlgorithm sa = Cryptography.Utilities.CreateSymmetricAlgoritm(connection.EncryptionType);
							sa.GenerateIV();
							sa.GenerateKey();

							//----- Adjust connection cryptors!
							connection.Encryptor = sa.CreateEncryptor();
							connection.Decryptor = sa.CreateDecryptor();

							//----- Create authenticate structure!
							AuthMessage am = new AuthMessage();
							am.SessionIV = serverPublicKey.Encrypt(sa.IV, false);
							am.SessionKey = serverPublicKey.Encrypt(sa.Key, false);
							am.SourceKey = Cryptography.Utilities.EncryptDataForAuthenticate(sa, Encoding.UTF8.GetBytes(clientPrivateKey.ToXmlString(false)), PaddingMode.ISO10126);

							//----- Sign message with am.SourceKey, am.SessionKey and FSignMessage!
							//----- Need to use PaddingMode.PKCS7 in sign!
							MemoryStream m = new MemoryStream();
							m.Write(am.SourceKey, 0, am.SourceKey.Length);
							m.Write(am.SessionKey, 0, am.SessionKey.Length);
							m.Write(FSignMessage, 0, FSignMessage.Length);

							am.Sign = clientPrivateKey.SignData(Cryptography.Utilities.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider());

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

							break;
						}

						case HostType.TcpServer:
						case HostType.Udp:
						{
							//----- Create empty authenticate structure!
							MessageBuffer mb = new MessageBuffer(8192);

							//----- Start receive structure!
							connection.Socket.BeginReceive(mb.PacketBuffer, mb.PacketOffSet, mb.PacketRemaining, SocketFlags.None, new AsyncCallback(InitializeConnectionReceiveCallback), new CallbackData(connection, mb));

							break;
						}
					}
				}

				//----- Asymmetric!
				if (connection.EncryptionType == Cryptography.EncryptionType.SSL)
				{
					switch (FHost.HostType)
					{
						case HostType.TcpClient:
						{
							//----- Get SSL items!
							X509Certificate2Collection certs = null;
							string serverName = null;
							bool checkRevocation = true;

							FCryptoService.OnSSLClientAuthenticate(out serverName, ref certs, ref checkRevocation);

							//----- Authneticate SSL!
							SslStream ssl = new SslStream(new NetworkStream(connection.Socket));

							if (certs == null)
								ssl.BeginAuthenticateAsClient(serverName, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.TcpClient));
							else
								ssl.BeginAuthenticateAsClient(serverName, certs, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.TcpClient));

							break;
						}

						case HostType.TcpServer:
						{
							//----- Get SSL items!
							X509Certificate2 cert = null;
							bool clientAuthenticate = false;
							bool checkRevocation = true;

							FCryptoService.OnSSLServerAuthenticate(out cert, out clientAuthenticate, ref checkRevocation);

							//----- Authneticate SSL!
							SslStream ssl = new SslStream(new NetworkStream(connection.Socket));
							ssl.BeginAuthenticateAsServer(cert, clientAuthenticate, System.Security.Authentication.SslProtocols.Default, checkRevocation, new AsyncCallback(SslAuthenticateCallback), new AuthenticateCallbackData(connection, ssl, HostType.TcpServer));

							break;
						}
					}
				}
			}
			else
			{
				//----- No encryption - Authenticate!
				FHost.FireOnConnected(connection);
			}
		}

        #endregion

        #region InitializeConnectionSendCallback

        private void InitializeConnectionSendCallback(IAsyncResult ar)
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

					if (!IsDisposed)
					{
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
			}
			catch (Exception exOuter)
			{
				MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				if (!IsDisposed)
				{
					try
					{
						connection.BeginDisconnect(exOuter);
					}
					catch (Exception exInner)
					{
						MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
						FHost.FireOnException(new ExceptionEventArgs(exInner));
					}
				}
			}
		}

        #endregion

        #region InitializeConnectionReceiveCallback

        private void InitializeConnectionReceiveCallback(IAsyncResult ar)
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

					if (!IsDisposed)
					{
						if (readBytes > 0)
						{
							readMessage.PacketOffSet += readBytes;
							byte[] message = null;

							try
							{
								message = Convert.FromBase64String(Encoding.Default.GetString(readMessage.PacketBuffer, 0, readMessage.PacketOffSet));
							}
							catch (FormatException ex)
							{
								MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, ex);
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
								SymmetricAlgorithm sa = Cryptography.Utilities.CreateSymmetricAlgoritm(connection.EncryptionType);
								sa.Key = serverPrivateKey.Decrypt(am.SessionKey, false);
								sa.IV = serverPrivateKey.Decrypt(am.SessionIV, false);

								//----- Adjust connection cryptors!
								connection.Encryptor = sa.CreateEncryptor();
								connection.Decryptor = sa.CreateDecryptor();

								//----- Verify sign!
								clientPublicKey.FromXmlString(Encoding.UTF8.GetString(Cryptography.Utilities.DecryptDataForAuthenticate(sa, am.SourceKey, PaddingMode.ISO10126)));

								m.SetLength(0);
								m.Write(am.SourceKey, 0, am.SourceKey.Length);
								m.Write(am.SessionKey, 0, am.SessionKey.Length);
								m.Write(FSignMessage, 0, FSignMessage.Length);

								if (!clientPublicKey.VerifyData(Cryptography.Utilities.EncryptDataForAuthenticate(sa, m.ToArray(), PaddingMode.PKCS7), new SHA1CryptoServiceProvider(), am.Sign))
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
			}
			catch (Exception exOuter)
			{
				MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				if (!IsDisposed)
				{
					try
					{
						connection.BeginDisconnect(exOuter);
					}
					catch (Exception exInner)
					{
						MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
						FHost.FireOnException(new ExceptionEventArgs(exInner));
					}
				}
			}
		}

        #endregion

        #region SslAuthenticateCallback

        private void SslAuthenticateCallback(IAsyncResult ar)
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
					switch (callbackData.HostType)
					{
						case HostType.TcpClient:
							stream.EndAuthenticateAsClient(ar);
							break;

						case HostType.TcpServer:
							stream.EndAuthenticateAsServer(ar);
							break;
					}

					if (!(stream.IsSigned && stream.IsEncrypted))
						throw new SSLAuthenticationException("Ssl authenticate is not signed or not encrypted.");

					connection.Stream = stream;

					FHost.FireOnConnected(connection);
				}
			}
			catch (Exception exOuter)
			{
				MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exOuter);
				try
				{
					connection.BeginDisconnect(exOuter);
				}
				catch (Exception exInner)
				{
					MKY.Utilities.Diagnostics.DebugOutput.WriteException(this, exInner);
					FHost.FireOnException(new ExceptionEventArgs(exInner));
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

		/// <summary></summary>
        internal BaseSocketConnectionHost Host
        {
            get { return FHost; }
        }

		/// <summary></summary>
		protected Cryptography.ICryptoService CryptoService
        {
            get { return FCryptoService; }
        }

		/// <summary></summary>
		public Cryptography.EncryptionType EncryptionType
        {
            get { return FEncryptionType; }
        }

		/// <summary></summary>
		public IPEndPoint LocalEndPoint
        {
            get { return FLocalEndPoint; }
        }

		/// <summary></summary>
		public Compression.CompressionType CompressionType
        {
            get { return FCompressionType; }
        }

        #endregion

    }

}
