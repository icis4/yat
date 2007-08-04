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

namespace HSR.Net.Sockets
{

    /// <summary>
    /// Contains original buffer and the read/send buffer and offset.
    /// </summary>
    internal class MessageBuffer
    {

        #region Fields

        private byte[] FRawBuffer;
        private byte[] FPacketBuffer;
        private int FPacketOffSet;

        #endregion

        #region Constructor

        public MessageBuffer(int bufferSize)
        {
            
            FPacketBuffer = null;

            if (bufferSize > 0)
            {
                FPacketBuffer = new byte[bufferSize];
            }

            FPacketOffSet = 0;
            FRawBuffer = null;

        }

        public MessageBuffer(ref byte[] rawBuffer, ref byte[] packetBuffer)
        {
            FRawBuffer = rawBuffer;
            FPacketBuffer = packetBuffer;
            FPacketOffSet = 0;
        }

        #endregion

        #region Methods

        #region GetPacketMessage

        /// <summary>
        /// Gets a packet message!
        /// </summary>
        /// <param name="connection">
        /// Socket connection.
        /// </param>
        /// <param name="buffer">
        /// Data.
        /// </param>
        public static MessageBuffer GetPacketMessage(BaseSocketConnection connection, ref byte[] buffer)
        {

            byte[] workBuffer = null;

			workBuffer = Cryptography.Utilities.EncryptData(connection, buffer);

            if (connection.Header != null && connection.Header.Length >= 0)
            {
                //----- Need header!
                int headerSize = connection.Header.Length + 2;
                byte[] result = new byte[workBuffer.Length + headerSize];

                int messageLength = result.Length;

                //----- Header!
                for (int i = 0; i < connection.Header.Length; i++)
                {
                    result[i] = connection.Header[i];
                }

                //----- Length!
                result[connection.Header.Length] = Convert.ToByte((messageLength & 0xFF00) >> 8);
                result[connection.Header.Length + 1] = Convert.ToByte(messageLength & 0xFF);

                Array.Copy(workBuffer, 0, result, headerSize, workBuffer.Length);

                return new MessageBuffer(ref buffer, ref result);

            }
            else
            {
                //----- No header!
                return new MessageBuffer(ref buffer, ref workBuffer);
            }

        }

        #endregion

        #region GetRawBuffer

        /// <summary>
        /// Get the buffer from packet message!
        /// </summary>
        /// <param name="messageLength">
        /// Message offset.
        /// </param>
        /// <param name="headerSize">
        /// Service header size.
        /// </param>
        public byte[] GetRawBuffer(int messageLength, int headerSize)
        {

            //----- Get Raw Buffer!
            byte[] result = null;

            result = new byte[messageLength - headerSize];
            Array.Copy(FPacketBuffer, headerSize, result, 0, result.Length);

            //----- Adjust Packet Buffer!
            byte[] packetBuffer = new byte[FPacketBuffer.Length - messageLength];
            Array.Copy(FPacketBuffer, messageLength, packetBuffer, 0, packetBuffer.Length);

            FPacketBuffer = packetBuffer;
            FPacketOffSet = FPacketOffSet - messageLength;

            return result;

        }

        #endregion

        #region Resize

        /// <summary>
        /// Resize the buffer.
        /// </summary>
        /// <param name="newLength">
        /// The new length of buffer.
        /// </param>
        public void Resize(int newLength)
        {
            Array.Resize(ref FPacketBuffer, newLength);
        }

        #endregion

        #endregion

        #region Properties

        public byte[] RawBuffer
        {
            get { return FRawBuffer; }
            set { FRawBuffer = value; }
        }

        public byte[] PacketBuffer
        {
            get { return FPacketBuffer; }
            set { FPacketBuffer = value; }
        }

        public int PacketOffSet
        {
            get { return FPacketOffSet; }
            set { FPacketOffSet = value; }
        }

        public int PacketRemaining
        {
            get { return FPacketBuffer.Length - FPacketOffSet; }
        }

        public int PacketLength
        {
            get { return FPacketBuffer.Length; }
        }

        #endregion

    }

}
