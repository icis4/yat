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
using System.Text;
using System.Net;

namespace HSR.Net.Sockets
{
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
		Cryptography.EncryptionType EncryptionType
		{
			get;
		}

		/// <summary>
		/// Connection compression type.
		/// </summary>
		Compression.CompressionType CompressionType
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
	public interface IClientSocketConnection : ISocketConnection
	{

		/// <summary>
		/// Begin reconnect the connection.
		/// </summary>
		void BeginReconnect();
	}

	#endregion

	#region IServerSocketConnection

	/// <summary>
	/// Server connection methods.
	/// </summary>
	public interface IServerSocketConnection : ISocketConnection
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
}
