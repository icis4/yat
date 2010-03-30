//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Settings.Terminal
{
	[Serializable]
	[XmlRoot("Settings")]
	public class TerminalSettingsRoot : MKY.Utilities.Settings.Settings, MKY.Utilities.Xml.IAlternateXmlElementProvider, IEquatable<TerminalSettingsRoot>
	{
		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		/// <remarks>
		/// \remind Matthias Klaey 2008-06-07 (2 hours to the first Euro2008 game :-)
		/// Instead of this approach, an [AlternateXmlElementAttribute] based approach should be tried
		/// in a future version. Such approach would be benefitial in terms of modularity because the
		/// XML path wouldn't need to be considered, i.e. name changes in the path could be handled.
		/// That is not the case currently.
		/// </remarks>
		private static readonly MKY.Utilities.Xml.AlternateXmlElement[] _AlternateXmlElements =
			{
				new MKY.Utilities.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "SerialPort", "Communication" }, "FlowControl",       new string[] { "Handshake" } ),
				new MKY.Utilities.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                  }, "TerminalIsStarted", new string[] { "TerminalIsOpen" } ),
				new MKY.Utilities.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                  }, "LogIsStarted",      new string[] { "LogIsOpen" } ),
			};

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
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>File type is a kind of title, therefore capital 'T' and 'S'.</remarks>
		[XmlElement("FileType")]
		public virtual string FileType
		{
			get { return ("YAT Terminal Settings"); }
			set { } // do nothing
		}

		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // do nothing
		}

		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (_productVersion); }
			set { } // do nothing
		}

		[XmlElement("Saved")]
		public virtual SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // Do nothing.
		}

		[XmlElement("AutoSaved")]
		public virtual bool AutoSaved
		{
			get { return (_autoSaved); }
			set
			{
				if (value != _autoSaved)
				{
					_autoSaved = value;
					// Do not set changed.
				}
			}
		}

		[XmlElement("Explicit")]
		public virtual ExplicitSettings Explicit
		{
			get { return (_explicit); }
			set
			{
				if (_explicit == null)
				{
					_explicit = value;
					AttachNode(_explicit);
				}
				else if (value != _explicit)
				{
					ExplicitSettings old = _explicit;
					_explicit = value;
					ReplaceNode(old, _explicit);
				}
			}
		}

		[XmlElement("Implicit")]
		public virtual ImplicitSettings Implicit
		{
			get { return (_implicit); }
			set
			{
				if (_implicit == null)
				{
					_implicit = value;
					AttachNode(_implicit);
				}
				else if (value != _implicit)
				{
					ImplicitSettings old = _implicit;
					_implicit = value;
					ReplaceNode(old, _implicit);
				}
			}
		}

		[XmlIgnore]
		public virtual MKY.Utilities.Xml.AlternateXmlElement[] AlternateXmlElements
		{
			get { return (_AlternateXmlElements); }
		}

		#endregion

		#region Property Shortcuts
		//------------------------------------------------------------------------------------------
		// Property Shortcuts
		//------------------------------------------------------------------------------------------

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual bool TerminalIsStarted
		{
			get { return (_implicit.TerminalIsStarted); }
			set { _implicit.TerminalIsStarted = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual bool LogIsStarted
		{
			get { return (_implicit.LogIsStarted); }
			set { _implicit.LogIsStarted = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.TerminalType TerminalType
		{
			get { return (_explicit.Terminal.TerminalType); }
			set { _explicit.Terminal.TerminalType = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.TerminalSettings Terminal
		{
			get { return (_explicit.Terminal); }
			set { _explicit.Terminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.IOSettings IO
		{
			get { return (_explicit.Terminal.IO); }
			set { _explicit.Terminal.IO = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.IOType IOType
		{
			get { return (_explicit.Terminal.IO.IOType); }
			set { _explicit.Terminal.IO.IOType = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual MKY.IO.Ports.SerialPortId SerialPortId
		{
			get { return (_explicit.Terminal.IO.SerialPort.PortId); }
			set { _explicit.Terminal.IO.SerialPort.PortId = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual int SocketLocalPort
		{
			get { return (_explicit.Terminal.IO.Socket.LocalPort); }
			set { _explicit.Terminal.IO.Socket.LocalPort = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual string SocketRemoteHost
		{
			get { return (_explicit.Terminal.IO.Socket.RemoteHost); }
			set { _explicit.Terminal.IO.Socket.RemoteHost = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual int SocketRemotePort
		{
			get { return (_explicit.Terminal.IO.Socket.RemotePort); }
			set { _explicit.Terminal.IO.Socket.RemotePort = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.BufferSettings Buffer
		{
			get { return (_explicit.Terminal.Buffer); }
			set { _explicit.Terminal.Buffer = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.DisplaySettings Display
		{
			get { return (_explicit.Terminal.Display); }
			set { _explicit.Terminal.Display = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.CharReplaceSettings CharReplace
		{
			get { return (_explicit.Terminal.CharReplace); }
			set { _explicit.Terminal.CharReplace = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.SendSettings Send
		{
			get { return (_explicit.Terminal.Send); }
			set { _explicit.Terminal.Send = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.TextTerminalSettings TextTerminal
		{
			get { return (_explicit.Terminal.TextTerminal); }
			set { _explicit.Terminal.TextTerminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.BinaryTerminalSettings BinaryTerminal
		{
			get { return (_explicit.Terminal.BinaryTerminal); }
			set { _explicit.Terminal.BinaryTerminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.SendCommandSettings SendCommand
		{
			get { return (_implicit.SendCommand); }
			set { _implicit.SendCommand = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.SendFileSettings SendFile
		{
			get { return (_implicit.SendFile); }
			set { _implicit.SendFile = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.PredefinedSettings Predefined
		{
			get { return (_implicit.Predefined); }
			set { _implicit.Predefined = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.WindowSettings Window
		{
			get { return (_implicit.Window); }
			set { _implicit.Window = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.LayoutSettings Layout
		{
			get { return (_implicit.Layout); }
			set { _implicit.Layout = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (_explicit.PredefinedCommand); }
			set { _explicit.PredefinedCommand = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.FormatSettings Format
		{
			get { return (_explicit.Format); }
			set { _explicit.Format = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Log.Settings.LogSettings Log
		{
			get { return (_explicit.Log); }
			set { _explicit.Log = value;  }
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
			// Ensure that object.operator!=() is called
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
