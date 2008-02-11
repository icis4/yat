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

using MKY.Utilities.Types;

namespace MKY.Net.Sockets.Cryptography
{
	#region Enum EncryptionType

	/// <summary></summary>
	public enum EncryptionType
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		Base64,
		/// <summary></summary>
		TripleDES,
		/// <summary></summary>
		Rijndael,
		/// <summary></summary>
		SSL
	}

	#endregion

	/// <summary>
	/// Extended enum XEncryptionType.
	/// </summary>
	[Serializable]
	class XEncryptionType : XEnum
	{
		#region String Definitions

		private const string None_string      = "None";
		private const string Base64_string    = "Base 64";
		private const string TripleDES_string = "Triple DES";
		private const string Rijndael_string  = "AES (Rijndael)";
		private const string SSL_string       = "SSL";

		#endregion

		/// <summary>Default is <see cref="EncryptionType.None"/></summary>
		public XEncryptionType()
			: base(EncryptionType.None)
		{
		}

		/// <summary></summary>
		protected XEncryptionType(EncryptionType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((EncryptionType)UnderlyingEnum)
			{
				case EncryptionType.None:      return (None_string);
				case EncryptionType.Base64:    return (Base64_string);
				case EncryptionType.TripleDES: return (TripleDES_string);
				case EncryptionType.Rijndael:  return (Rijndael_string);
				case EncryptionType.SSL:       return (SSL_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XEncryptionType[] GetItems()
		{
			List<XEncryptionType> a = new List<XEncryptionType>();
			a.Add(new XEncryptionType(EncryptionType.None));
			a.Add(new XEncryptionType(EncryptionType.Base64));
			a.Add(new XEncryptionType(EncryptionType.TripleDES));
			a.Add(new XEncryptionType(EncryptionType.Rijndael));
			a.Add(new XEncryptionType(EncryptionType.SSL));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XEncryptionType Parse(string type)
		{
			XEncryptionType result;

			if (TryParse(type, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException(type));
		}

		/// <summary></summary>
		public static bool TryParse(string type, out XEncryptionType result)
		{
			if      (string.Compare(type, None_string, true) == 0)
			{
				result = new XEncryptionType(EncryptionType.None);
				return (true);
			}
			else if (string.Compare(type, Base64_string, true) == 0)
			{
				result = new XEncryptionType(EncryptionType.Base64);
				return (true);
			}
			else if (string.Compare(type, TripleDES_string, true) == 0)
			{
				result = new XEncryptionType(EncryptionType.TripleDES);
				return (true);
			}
			else if (string.Compare(type, Rijndael_string, true) == 0)
			{
				result = new XEncryptionType(EncryptionType.Rijndael);
				return (true);
			}
			else if (string.Compare(type, SSL_string, true) == 0)
			{
				result = new XEncryptionType(EncryptionType.SSL);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator EncryptionType(XEncryptionType type)
		{
			return ((EncryptionType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XEncryptionType(EncryptionType type)
		{
			return (new XEncryptionType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XEncryptionType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XEncryptionType(int type)
		{
			return (new XEncryptionType((EncryptionType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XEncryptionType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XEncryptionType(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}
