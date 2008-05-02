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
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MKY.Net.Sockets.Cryptography
{
    /// <summary>
    /// Crypto service methods.
    /// </summary>
    public interface ICryptoService
    {
        
        /// <summary>
        /// Fired when symmetric encryption is used.
        /// </summary>
		/// <param name="hostType">
		/// Type of host.
		/// </param>
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
}