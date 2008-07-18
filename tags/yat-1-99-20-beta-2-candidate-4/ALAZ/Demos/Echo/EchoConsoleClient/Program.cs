using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Security.Cryptography.X509Certificates;
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

            Console.SetWindowSize(60, 25);

            EncryptType et = EncryptType.etNone;
            CompressionType ct = CompressionType.ctGZIP;
            int port = 8090;
            int connections = 10;

            if (args.Length >= 1)
            {
                port = Convert.ToInt32(args[0]);
            }

            if (args.Length >= 2)
            {
                et = (EncryptType) Enum.Parse(typeof(EncryptType), args[1], true);
            }

            if (args.Length >= 3)
            {
                ct = (CompressionType) Enum.Parse(typeof(CompressionType), args[2], true);
            }

            OnEventDelegate FEvent = new OnEventDelegate(Event);

            SocketClient echoClient = new SocketClient(new EchoSocketService.EchoSocketService(FEvent), new byte[] { 0xFF, 0xFE, 0xFD });
            echoClient.OnException += new OnExceptionDelegate(echoClient_OnException);

            for (int i = 0; i < connections; i++)
            {
                echoClient.AddConnector(new IPEndPoint(IPAddress.Loopback, port), et, ct, new EchoCryptService.EchoCryptService());
            }

            Console.Title = "EchoConsoleClient / " + connections.ToString() + " Connections / " + Enum.GetName(typeof(EncryptType), et) + " / " + Enum.GetName(typeof(CompressionType), ct);
            echoClient.Start();

            Console.WriteLine("Started!");
            Console.WriteLine("----------------------");

            Console.ReadLine();

            echoClient.Stop();

            Console.WriteLine("Stopped!");
            Console.WriteLine("----------------------");
            Console.ReadLine();

            echoClient.Dispose();

        }

        static void echoClient_OnException(Exception ex)
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
