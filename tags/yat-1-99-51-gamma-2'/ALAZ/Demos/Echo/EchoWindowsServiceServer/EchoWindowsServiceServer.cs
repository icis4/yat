using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Net;

using ALAZ.SystemEx.SocketsEx;
using EchoSocketService;

namespace EchoWindowsServiceServer
{
    
    public partial class EchoServer : ServiceBase
    {

        public OnEventDelegate FEvent;
        SocketServer FEchoServer;
        StreamWriter FEchoLog;

        public EchoServer()
        {

            InitializeComponent();
            FEvent = new OnEventDelegate(Event);

            FEchoLog = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\EchoLog.txt");

        }

        private void Event(string eventMessage)
        {

            lock (FEchoLog)
            {
                FEchoLog.Write(eventMessage);
                FEchoLog.Flush();
            }
        }

        protected override void OnStart(string[] args)
        {

            FEchoServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent), new byte[] { 0xFF, 0xFE, 0xFD });
            FEchoServer.OnException += new OnExceptionDelegate(OnException);

            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8087), EncryptType.etSSL, CompressionType.ctGZIP, new EchoCryptService.EchoCryptService(), 50, 5);
            FEchoServer.Start();

        }

        protected override void OnStop()
        {
            
            FEchoServer.Stop();
            FEchoServer.Dispose();

            FEchoLog.Close();

        }

        private void OnException(Exception ex)
        {
            Event("Service Exception! - " + ex.Message);
            Event("------------------------------------------------");
        }

    }

}
