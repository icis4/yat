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

namespace MKY.Net.Sockets.Compression
{
	#region Enum CompressionType

	/// <summary></summary>
	public enum CompressionType
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		GZip
	}

	#endregion

	/// <summary>
	/// Extended enum XCompressionType.
	/// </summary>
	[Serializable]
	class XCompressionType : XEnum
	{
		#region String Definitions

		private const string None_string = "None";
		private const string GZip_string = "GZip";

		#endregion

		/// <summary>Default is <see cref="CompressionType.None"/></summary>
		public XCompressionType()
			: base(CompressionType.None)
		{
		}

		/// <summary></summary>
		protected XCompressionType(CompressionType type)
			: base(type)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((CompressionType)UnderlyingEnum)
			{
				case CompressionType.None: return (None_string);
				case CompressionType.GZip: return (GZip_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XCompressionType[] GetItems()
		{
			List<XCompressionType> a = new List<XCompressionType>();
			a.Add(new XCompressionType(CompressionType.None));
			a.Add(new XCompressionType(CompressionType.GZip));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XCompressionType Parse(string type)
		{
			XCompressionType result;

			if (TryParse(type, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("type", type, "Invalid type"));
		}

		/// <summary></summary>
		public static bool TryParse(string type, out XCompressionType result)
		{
			if      (string.Compare(type, None_string, true) == 0)
			{
				result = new XCompressionType(CompressionType.None);
				return (true);
			}
			else if (string.Compare(type, GZip_string, true) == 0)
			{
				result = new XCompressionType(CompressionType.GZip);
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
		public static implicit operator CompressionType(XCompressionType type)
		{
			return ((CompressionType)type.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XCompressionType(CompressionType type)
		{
			return (new XCompressionType(type));
		}

		/// <summary></summary>
		public static implicit operator int(XCompressionType type)
		{
			return (type.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XCompressionType(int type)
		{
			return (new XCompressionType((CompressionType)type));
		}

		/// <summary></summary>
		public static implicit operator string(XCompressionType type)
		{
			return (type.ToString());
		}

		/// <summary></summary>
		public static implicit operator XCompressionType(string type)
		{
			return (Parse(type));
		}

		#endregion
	}
}
