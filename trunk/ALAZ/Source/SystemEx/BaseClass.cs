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

namespace ALAZ.SystemEx
{

    /// <summary>
    /// Base class for disposable objects.
    /// </summary>
    public abstract class BaseClass : IDisposable
    {

        #region Fields

        private bool FDisposed = false;

        #endregion

        #region Methods

        /// <summary>
        /// This method is called when object is being disposed. Override this method to free resources.
        /// </summary>
        /// <param name="dispodedByUser">
        /// Indicates if was fired by user or GC.
        /// if disposedByUser = true you can dispose unmanaged and managed resources. if false, only unmanaged resources can be disposed.
        /// </param>
        protected virtual void Free(bool dispodedByUser)
        {
            //-----
        }

        /// <summary>
        /// Free the object.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (FDisposed == false)
                {
                    Free(true);
                    FDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates is object is already disposed.
        /// </summary>
        protected bool Disposed
        {
            get
            {
                
                lock (this)
                {
                    return FDisposed;
                }
            }
        }

        #endregion

        #region Free

        /// <summary>
        /// Destructor. (Finalize)
        /// </summary>
        ~BaseClass()
        {
            Free(false);
        }

        #endregion

    }

}