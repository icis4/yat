/* ====================================================================
 * Copyright (c) 2009 Andre Luis Azevedo (az.andrel@yahoo.com.br)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *    In addition, the source code must keep original namespace names.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution. In addition, the binary form must keep the original 
 *    namespace names and original file name.
 * 
 * 3. The name "ALAZ" or "ALAZ Library" must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * 4. Products derived from this software may not be called "ALAZ" or
 *    "ALAZ Library" nor may "ALAZ" or "ALAZ Library" appear in their 
 *    names without prior written permission of the author.
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
using System.Text;
using System.Text.RegularExpressions;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

  public class ProxyInfo
  {

    #region Fields

    private ProxyType FProxyType;
    private IPEndPoint FProxyEndPoint;
    private NetworkCredential FProxyCredential;
    private SOCKS5Phase FSOCKS5Phase;
    private SOCKS5AuthMode FSOCKS5Authentication;
    private bool FCompleted;

    #endregion

    #region Constructor

    public ProxyInfo(ProxyType proxyType, IPEndPoint proxyEndPoint, NetworkCredential proxyCredential)
    {
      
      FProxyType = proxyType;
      FProxyEndPoint = proxyEndPoint;
      FProxyCredential = proxyCredential;
      FSOCKS5Phase = SOCKS5Phase.spIdle;

    }

    #endregion

    #region Properties

    public NetworkCredential ProxyCredential
    {
      get {return FProxyCredential; }
    }

    public IPEndPoint ProxyEndPoint
    {
      get { return FProxyEndPoint; }
    }

    public ProxyType ProxyType
    {
      get { return FProxyType; }
    }

    internal SOCKS5Phase SOCKS5Phase
    {
      get { return FSOCKS5Phase; }
      set { FSOCKS5Phase = value; }
    }

    internal SOCKS5AuthMode SOCKS5Authentication
    {
      get { return FSOCKS5Authentication;  }
      set { FSOCKS5Authentication = value;  }
    }

    internal bool Completed
    { 
      get { return FCompleted; }
      set  {FCompleted = value; }
    }

    #endregion

  }

}
