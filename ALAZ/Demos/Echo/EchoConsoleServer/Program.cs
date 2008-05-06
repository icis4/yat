using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using EchoSocketService;
using ALAZ.SystemEx.SocketsEx;

namespace Main
{

    class MainClass
    {

        [STAThread]
        static void Main(string[] args)
        {

            //----- CspParameters used in CryptService.
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            RSACryptoServiceProvider serverKey = new RSACryptoServiceProvider(param);

            OnEventDelegate FEvent = new OnEventDelegate(Event);

            SocketServer echoServer = new SocketServer(new EchoSocketService.EchoSocketService(FEvent), new byte[] { 0xFF, 0xFE, 0xFD });
            echoServer.OnException += new OnExceptionDelegate(echoServer_OnException);

            echoServer.AddListener(new IPEndPoint(IPAddress.Any, 8090), EncryptType.etNone, CompressionType.ctGZIP, null, 50, 10);
            echoServer.AddListener(new IPEndPoint(IPAddress.Any, 8091), EncryptType.etBase64, CompressionType.ctNone, null, 50, 10);

            echoServer.Start();
 
            Console.WriteLine("Started!");
            Console.WriteLine("----------------------");

            Console.ReadLine();

            echoServer.Stop();

            Console.WriteLine("Stopped!");
            Console.WriteLine("----------------------");
            Console.ReadLine();

            echoServer.Dispose();

        }

        static void echoServer_OnException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Service Exception! - " + ex.Message);
            Console.WriteLine("------------------------------------------------");
            Console.ResetColor();
        }

        static void Event(string eventMessage)
        {
            if (eventMessage.Contains("Exception"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(eventMessage);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(eventMessage);
            }

        }

    }

}
