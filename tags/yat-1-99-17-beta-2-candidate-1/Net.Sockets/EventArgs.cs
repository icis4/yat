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

namespace MKY.Net.Sockets
{
	#region ConnectionEventArgs

	/// <summary>
	/// Base event arguments for connection events.
	/// </summary>
	public class ConnectionEventArgs : EventArgs
	{

		#region Fields

		private ISocketConnection FConnection;
		private object FCustomData;

		#endregion

		#region Constructor

		/// <summary></summary>
		public ConnectionEventArgs(ISocketConnection connection)
		{
			FConnection = connection;
		}

		#endregion

		#region Properties

		/// <summary></summary>
		public ISocketConnection Connection
		{
			get { return FConnection; }
		}

		/// <summary></summary>
		internal object CustomData
		{
			get { return FCustomData; }
			set { FCustomData = value; }
		}

		#endregion

	}

	#endregion

	#region DisconnectedEventArgs

	/// <summary>
	/// Disconnect event arguments for disconnected event.
	/// </summary>
	public class DisconnectedEventArgs : ConnectionEventArgs
	{

		#region Fields

		private Exception FException;

		#endregion

		#region Constructor

		/// <summary></summary>
		public DisconnectedEventArgs(ISocketConnection connection, Exception exception)
			: base(connection)
		{
			FException = exception;
		}

		#endregion

		#region Properties

		/// <summary></summary>
		public Exception Exception
		{
			get { return FException; }
		}

		#endregion

	}

	#endregion

	#region MessageEventArgs

	/// <summary>
    /// Message event arguments for message events.
    /// </summary>
	public class MessageEventArgs : ConnectionEventArgs
	{

		#region Fields

		private byte[] FBuffer;

		#endregion

		#region Constructor

		/// <summary></summary>
		public MessageEventArgs(ISocketConnection connection, byte[] buffer)
			: base(connection)
		{
			FBuffer = buffer;
		}

		#endregion

		#region Properties

		/// <summary></summary>
		public byte[] Buffer
		{
			get { return FBuffer; }
			internal set { FBuffer = value; }
		}

		#endregion

	}

	#endregion

	#region ExceptionEventArgs

	/// <summary>
    /// Exception event arguments for exception events.
    /// </summary>
	public class ExceptionEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly Exception Exception;

		/// <summary></summary>
		public ExceptionEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}

	#endregion
}
