using System;

using YAT.Settings;
using YAT.Domain;
using YAT.Domain.Settings;

namespace YAT.Test
{
	public class Test_RawTerminal
	{
		private static RawTerminal _terminal;
		private static Parser _parser;

		public static void Test()
		{
			_terminal = new RawTerminal("COM1", new PortSettings("PortSettings", SettingsType.Normal), BufferSettings.SizeDefault);
			_terminal.ConnectionChanged += new ConnectionChangedEventHandler(_terminal_ConnectionChanged);
			_terminal.RepositoryCleared += new RepositoryClearedEventHandler(_terminal_RepositoryCleared);
			_terminal.DeviceError += new DeviceErrorEventHandler(_terminal_DeviceError);

			_parser = new Parser();

			_terminal.Open();
			_terminal.Send(_parser.Parse("SI<CR><LF>"));
			System.Threading.Thread.Sleep(5000);
			_terminal.Close();
		}

		private static void _terminal_ConnectionChanged(object sender, ConnectionEventArgs e)
		{
			if (e.Opened)
				Console.WriteLine("Connection opened!");
			else
				Console.WriteLine("Connection closed!");
		}

		private static void _terminal_RepositoryCleared(object sender, RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case RepositoryType.Tx: Console.WriteLine("TxRepository:"); break;
				case RepositoryType.Bidir: Console.WriteLine("BiDirRepository:"); break;
				case RepositoryType.Rx: Console.WriteLine("RxRepository:"); break;
			}
			Console.WriteLine(_terminal.RepositoryToString(e.Repository, ""));
		}

		private static void _terminal_DeviceError(object sender, DeviceErrorEventArgs e)
		{
			Console.WriteLine("Device error!");
		}
	}
}
