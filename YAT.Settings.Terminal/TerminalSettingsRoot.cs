using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	[Serializable]
	[XmlRoot("Settings")]
	public class TerminalSettingsRoot : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettingsRoot>
	{
		private string _productVersion = System.Windows.Forms.Application.ProductVersion;
		private bool _autoSaved = false;
		private ExplicitSettings _explicit;
		private ImplicitSettings _implicit;

		public TerminalSettingsRoot()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			Explicit = new ExplicitSettings();
			Implicit = new ImplicitSettings();
			ClearChanged();
		}

		public TerminalSettingsRoot(TerminalSettingsRoot rhs)
			: base(rhs)
		{
			Explicit = new ExplicitSettings(rhs.Explicit);
			Implicit = new ImplicitSettings(rhs.Implicit);
			ClearChanged();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("FileType")]
		public string FileType
		{
			get { return ("YAT terminal settings"); }
			set { } // do nothing
		}

		[XmlElement("Warning")]
		public string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // do nothing
		}

		[XmlElement("ProductVersion")]
		public string ProductVersion
		{
			get { return (_productVersion); }
			set { } // do nothing
		}

		[XmlElement("Saved")]
		public SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // do nothing
		}

		[XmlElement("AutoSaved")]
		public bool AutoSaved
		{
			get { return (_autoSaved); }
			set
			{
				if (_autoSaved != value)
				{
					_autoSaved = value;
					// do not set changed;
				}
			}
		}

		[XmlElement("Explicit")]
		public ExplicitSettings Explicit
		{
			get { return (_explicit); }
			set
			{
				if (_explicit == null)
				{
					_explicit = value;
					AttachNode(_explicit);
				}
				else if (_explicit != value)
				{
					ExplicitSettings old = _explicit;
					_explicit = value;
					ReplaceNode(old, _explicit);
				}
			}
		}

		[XmlElement("Implicit")]
		public ImplicitSettings Implicit
		{
			get { return (_implicit); }
			set
			{
				if (_implicit == null)
				{
					_implicit = value;
					AttachNode(_implicit);
				}
				else if (_implicit != value)
				{
					ImplicitSettings old = _implicit;
					_implicit = value;
					ReplaceNode(old, _implicit);
				}
			}
		}

		#endregion

		#region Property Shortcuts
		//------------------------------------------------------------------------------------------
		// Property Shortcuts
		//------------------------------------------------------------------------------------------

		[XmlIgnore]
		public bool TerminalIsOpen
		{
			get { return (_implicit.TerminalIsOpen); }
			set { _implicit.TerminalIsOpen = value; }
		}

		[XmlIgnore]
		public bool LogIsOpen
		{
			get { return (_implicit.LogIsOpen); }
			set { _implicit.LogIsOpen = value; }
		}

		[XmlIgnore]
		public Domain.TerminalType TerminalType
		{
			get { return (_explicit.Terminal.TerminalType); }
			set { _explicit.Terminal.TerminalType = value; }
		}

		[XmlIgnore]
		public Domain.Settings.TerminalSettings Terminal
		{
			get { return (_explicit.Terminal); }
			set { _explicit.Terminal = value; }
		}

		[XmlIgnore]
		public Domain.Settings.IOSettings IO
		{
			get { return (_explicit.Terminal.IO); }
			set { _explicit.Terminal.IO = value; }
		}

		[XmlIgnore]
		public Domain.IOType IOType
		{
			get { return (_explicit.Terminal.IO.IOType); }
			set { _explicit.Terminal.IO.IOType = value; }
		}

		[XmlIgnore]
		public MKY.IO.Ports.SerialPortId SerialPortId
		{
			get { return (_explicit.Terminal.IO.SerialPort.PortId); }
			set { _explicit.Terminal.IO.SerialPort.PortId = value; }
		}

		[XmlIgnore]
		public int SocketLocalPort
		{
			get { return (_explicit.Terminal.IO.Socket.LocalPort); }
			set { _explicit.Terminal.IO.Socket.LocalPort = value; }
		}

		[XmlIgnore]
		public string SocketRemoteHostNameOrAddress
		{
			get { return (_explicit.Terminal.IO.Socket.RemoteHostNameOrAddress); }
			set { _explicit.Terminal.IO.Socket.RemoteHostNameOrAddress = value; }
		}

		[XmlIgnore]
		public int SocketRemotePort
		{
			get { return (_explicit.Terminal.IO.Socket.RemotePort); }
			set { _explicit.Terminal.IO.Socket.RemotePort = value; }
		}

		[XmlIgnore]
		public Domain.Settings.BufferSettings Buffer
		{
			get { return (_explicit.Terminal.Buffer); }
			set { _explicit.Terminal.Buffer = value; }
		}

		[XmlIgnore]
		public Domain.Settings.DisplaySettings Display
		{
			get { return (_explicit.Terminal.Display); }
			set { _explicit.Terminal.Display = value; }
		}

		[XmlIgnore]
		public Domain.Settings.TransmitSettings Transmit
		{
			get { return (_explicit.Terminal.Transmit); }
			set { _explicit.Terminal.Transmit = value; }
		}

		[XmlIgnore]
		public Domain.Settings.TextTerminalSettings TextTerminal
		{
			get { return (_explicit.Terminal.TextTerminal); }
			set { _explicit.Terminal.TextTerminal = value; }
		}

		[XmlIgnore]
		public Domain.Settings.BinaryTerminalSettings BinaryTerminal
		{
			get { return (_explicit.Terminal.BinaryTerminal); }
			set { _explicit.Terminal.BinaryTerminal = value; }
		}

		[XmlIgnore]
		public Gui.Settings.SendCommandSettings SendCommand
		{
			get { return (_implicit.SendCommand); }
			set { _implicit.SendCommand = value; }
		}

		[XmlIgnore]
		public Gui.Settings.SendFileSettings SendFile
		{
			get { return (_implicit.SendFile); }
			set { _implicit.SendFile = value; }
		}

		[XmlIgnore]
		public Gui.Settings.PredefinedSettings Predefined
		{
			get { return (_implicit.Predefined); }
			set { _implicit.Predefined = value; }
		}

		[XmlIgnore]
		public Gui.Settings.WindowSettings Window
		{
			get { return (_implicit.Window); }
			set { _implicit.Window = value; }
		}

		[XmlIgnore]
		public Gui.Settings.LayoutSettings Layout
		{
			get { return (_implicit.Layout); }
			set { _implicit.Layout = value; }
		}

		[XmlIgnore]
		public Gui.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (_explicit.PredefinedCommand); }
			set { _explicit.PredefinedCommand = value; }
		}

		[XmlIgnore]
		public Gui.Settings.FormatSettings Format
		{
			get { return (_explicit.Format); }
			set { _explicit.Format = value; }
		}

		[XmlIgnore]
		public Log.Settings.LogSettings Log
		{
			get { return (_explicit.Log); }
			set { _explicit.Log = value; }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TerminalSettingsRoot)
				return (Equals((TerminalSettingsRoot)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TerminalSettingsRoot value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_productVersion.Equals(value._productVersion) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // compares all settings nodes
					);
				// do not compare AutoSaved
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalSettingsRoot lhs, TerminalSettingsRoot rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TerminalSettingsRoot lhs, TerminalSettingsRoot rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}