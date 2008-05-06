using System;

namespace ALAZ.SystemEx.SocketsEx
{
    
    /// <summary>
    /// Buffer tools.
    /// </summary>
    internal static class BufferUtils
    {

        #region Methods

        #region GetWriteMessage

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

            workBuffer = CryptUtils.EncryptData(connection, buffer);

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

        #endregion

    }

}
