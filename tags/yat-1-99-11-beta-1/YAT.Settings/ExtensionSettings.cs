using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Settings
{
	/// <summary>
	/// Defines extension filters for open/save dialogs.
	/// </summary>
	public class ExtensionSettings
	{
		public string AllFilesFilter
		{
			get
			{
				return ("All Files (*.*)|*.*");
			}
		}

		public string TerminalFilesFilter
		{
			get
			{
				return ("Terminal Files (*.yat)|*.yat|" +
						"All Files (*.*)|*.*");
			}
		}

		public string TerminalFilesDefault
		{
			get { return (".yat"); }
		}

		public string TextFilesFilter
		{
			get
			{
				return ("Text Files (*.txt;*.text;*.log)|*.txt;*.text;*.log|" +
						"Rich Text Files (*.rtf)|*.rtf|" +
						"XML Files (*.xml)|*.xml|" +
						"All Files (*.*)|*.*");
			}
		}

		public string[] TextFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".txt");
				l.Add(".text");
				l.Add(".log");
				l.Add(".rtf");
				l.Add(".xml");
				return (l.ToArray());
			}
		}

		public string TextFilesDefault
		{
			get { return (".txt"); }
		}

		public string LogFilesDefault
		{
			get { return (".log"); }
		}

		public string MonitorFilesDefault
		{
			get { return (".rtf"); }
		}

		public bool IsText(string extension)
		{
			switch (extension)
			{
				case ".txt":
				case ".text":
				case ".log":
					return (true);

				default:
					return (false);
			}
		}

		public bool IsRtf(string extension)
		{
			switch (extension)
			{
				case ".rtf":
					return (true);

				default:
					return (false);
			}
		}

		public bool IsXml(string extension)
		{
			switch (extension)
			{
				case ".xml":
					return (true);

				default:
					return (false);
			}
		}

		public string BinaryFilesFilter
		{
			get
			{
				return ("Binary Files (*.dat;*.bin;*.binary)|*.dat;*.bin;*.binary|" +
						"All Files (*.*)|*.*");
			}
		}

		public string[] BinaryFilesWithDot
		{
			get
			{
				List<string> l = new List<string>();
				l.Add(".dat");
				l.Add(".bin");
				l.Add(".binary");
				return (l.ToArray());
			}
		}

		public string BinaryFilesDefault
		{
			get { return (".dat"); }
		}
	}
}
