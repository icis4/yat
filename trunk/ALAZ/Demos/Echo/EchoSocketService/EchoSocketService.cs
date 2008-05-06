using System;
using System.Threading;
using System.Text;
using System.Security.Cryptography;

using ALAZ.SystemEx.SocketsEx;

namespace EchoSocketService
{
 
    #region Delegates

    public delegate void OnEventDelegate(string eventMessage);

    #endregion

    public class EchoSocketService : BaseSocketService
    {

        #region Fields

        private OnEventDelegate FOnEventDelegate;

        #endregion

        #region Constructors

        public EchoSocketService()
        {
            FOnEventDelegate = null;
        }

        public EchoSocketService(OnEventDelegate eventDelegate)
        {
            FOnEventDelegate = eventDelegate;
        }

        #endregion

        #region Methods

        private void Event(string eventMessage)
        {

            if (FOnEventDelegate != null)
            {
                FOnEventDelegate.Invoke(eventMessage);
            }

        }

        public byte[] GetMessage(int handle)
        {

            Random r = new Random(handle + DateTime.Now.Millisecond);

            byte[] message = new byte[r.Next(256, 2048)];

            for (int i = 0; i < message.Length; i++)
            {
                message[i] = (byte)r.Next(32, 122);
            }

            return message;

        }

        public override void OnConnected(ConnectionEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("------------------------------------------------" + "\r\n");
            s.Append("Connected - " + e.Connection.ConnectionId + "\r\n");
            s.Append(e.Connection.EncryptType.ToString() + "\r\n");
            s.Append(e.Connection.CompressionType.ToString() + "\r\n");
            s.Append("------------------------------------------------" + "\r\n");

            Event(s.ToString());

            if (e.Connection.HostType == HostType.htServer)
            {
                e.Connection.BeginReceive();
            }
            else
            {
                byte[] b = GetMessage(e.Connection.SocketHandle.ToInt32());
                e.Connection.BeginSend(b);
            }

        }

        public override void OnSent(MessageEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("------------------------------------------------" + "\r\n");
            s.Append("Sent - " + e.Connection.ConnectionId + "\r\n");
            s.Append(Encoding.Default.GetString(e.Buffer) + "\r\n");
            s.Append("------------------------------------------------" + "\r\n");

            Event(s.ToString().Trim());

            if (e.Connection.HostType == HostType.htServer)
            {
                e.Connection.BeginReceive();
            }
            else
            {
                e.Connection.BeginReceive();
            }

        }

        public override void OnReceived(MessageEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("------------------------------------------------" + "\r\n");
            s.Append("Received - " + e.Connection.ConnectionId + "\r\n");
            s.Append(Encoding.Default.GetString(e.Buffer) + "\r\n");
            s.Append("------------------------------------------------" + "\r\n");

            Event(s.ToString());

            Thread.Sleep(new Random(DateTime.Now.Millisecond).Next(500, 1000));

            if (e.Connection.HostType == HostType.htServer)
            {
                byte[] b = e.Buffer;
                e.Connection.BeginSend(b);
            }
            else
            {

                byte[] b = GetMessage(e.Connection.SocketHandle.ToInt32());
                e.Connection.BeginSend(b);
            }

        }

        public override void OnDisconnected(DisconnectedEventArgs e)
        {

            StringBuilder s = new StringBuilder();

            s.Append("------------------------------------------------" + "\r\n");
            s.Append("Disconnected - " + e.Connection.ConnectionId + "\r\n");
            s.Append((e.Exception == null ? "" : "Exception - " + e.Exception.Message) + "\r\n");
            s.Append("------------------------------------------------" + "\r\n");

            Event(s.ToString());

            if (e.Connection.HostType == HostType.htServer)
            {
                //------
            }
            else
            {
                e.Connection.AsClientConnection().BeginReconnect();
            }

        }

        #endregion

    }

}
