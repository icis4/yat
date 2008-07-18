using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Security.Cryptography;

using ALAZ.SystemEx.SocketsEx;

namespace EchoFormServer
{
    public partial class frmEchoServer : EchoFormTemplate.frmEchoForm
    {
        
        private SocketServer FEchoServer;

        public frmEchoServer()
        {
            InitializeComponent();
        }

        private void frmEchoServer_Load(object sender, EventArgs e)
        {

            //----- CspParameters used in CryptService.
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            RSACryptoServiceProvider serverKey = new RSACryptoServiceProvider(param);

            FEchoServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent), new byte[] { 0xFF, 0xFE, 0xFD });
            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8090), EncryptType.etNone, CompressionType.ctGZIP, null, 50, 10);
            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8091), EncryptType.etBase64, CompressionType.ctNone, null, 50, 10);
            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8092), EncryptType.etRijndael, CompressionType.ctGZIP, new EchoCryptService.EchoCryptService(), 50, 10);
            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8093), EncryptType.etTripleDES, CompressionType.ctNone, new EchoCryptService.EchoCryptService(), 50, 10);
            FEchoServer.AddListener(new IPEndPoint(IPAddress.Any, 8094), EncryptType.etSSL, CompressionType.ctGZIP, new EchoCryptService.EchoCryptService(), 50, 10);

            FEchoServer.OnException += new OnExceptionDelegate(OnException);

        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            FEchoServer.Start();
            Event("Started!");
            Event("---------------------------------");

        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            FEchoServer.Stop();
            Event("Stopped!");
            Event("---------------------------------");

        }
    }
}

