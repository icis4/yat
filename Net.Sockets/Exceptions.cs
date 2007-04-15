using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.Net.Sockets
{
	/// <summary>
	/// Max reconnect attempts reached.
	/// </summary>
	public class ReconnectAttemptsException : Exception
	{
		/// <summary></summary>
		public ReconnectAttemptsException(string message) : base(message)
		{
		}
	}

	/// <summary>
	/// Bad Header.
	/// </summary>
	public class BadHeaderException : Exception
	{
		/// <summary></summary>
		public BadHeaderException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Message length is greater than the maximum value.
	/// </summary>
	public class MessageLengthException : Exception
	{
		/// <summary></summary>
		public MessageLengthException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// Symmetric authentication failure.
	/// </summary>
	public class SymmetricAuthenticationException : Exception
	{
		/// <summary></summary>
		public SymmetricAuthenticationException(string message)
			: base(message)
		{
		}
	}

	/// <summary>
	/// SSL authentication failure.
	/// </summary>
	public class SSLAuthenticationException : Exception
	{
		/// <summary></summary>
		public SSLAuthenticationException(string message)
			: base(message)
		{
		}
	}
}
