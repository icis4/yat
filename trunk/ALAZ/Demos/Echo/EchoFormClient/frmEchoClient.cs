using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Net;

using ALAZ.SystemEx.SocketsEx;

namespace EchoFormClient
{
    public partial class frmEchoClient : EchoFormTemplate.frmEchoForm
    {

        private SocketClient FEchoClient;

        public frmEchoClient()
        {
            InitializeComponent();
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            FEchoClient.Start();
            Event("Started!");
            Event("---------------------------------");

        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            FEchoClient.Stop();
            Event("Stopped!");
            Event("---------------------------------");

        }

        private void frmEchoClient_Load(object sender, EventArgs e)
        {
            FEchoClient = new SocketClient(new EchoSocketService.EchoSocketService(FEvent), new byte[] { 0xFF, 0xFE, 0xFD });

            FEchoClient.AddConnector(new IPEndPoint(IPAddress.Loopback, 8090), EncryptType.etNone, CompressionType.ctGZIP, null);
            FEchoClient.AddConnector(new IPEndPoint(IPAddress.Loopback, 8091), EncryptType.etBase64, CompressionType.ctNone, null);

            FEchoClient.OnException += new OnExceptionDelegate(OnException);

        }
    }
}

