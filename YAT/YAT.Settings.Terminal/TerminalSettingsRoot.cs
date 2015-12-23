//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using YAT.Utilities;

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	[Serializable]
	[XmlRoot("Settings")]
	public class TerminalSettingsRoot : MKY.Settings.SettingsItem, MKY.Xml.IAlternateXmlElementProvider
	{
		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.4.1";

		/// <remarks>Is basically constant, but must be a normal variable for automatic XML serialization.</remarks>
		private string productVersion = Utilities.ApplicationInfo.ProductVersion;

		private bool autoSaved;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "'explicit' is a key word.")]
		private ExplicitSettings explicit_;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "'implicit' is a key word.")]
		private ImplicitSettings implicit_;

		/// <summary></summary>
		public TerminalSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			Explicit = new ExplicitSettings();
			Implicit = new ImplicitSettings();
			ClearChanged();
		}

		/// <summary></summary>
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
			get { return (ApplicationInfo.ProductName + " Terminal Settings"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("SettingsVersion")]
		public virtual string SettingsVersion
		{
			get { return (this.settingsVersion); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (this.productVersion); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying this file may cause undefined behavior!"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Saved")]
		public virtual SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // Do nothing.
		}

		/// <summary>
		/// Auto save means that the settings have been saved at an automatically chosen location,
		/// without telling the user anything about it.
		/// </summary>
		[XmlElement("AutoSaved")]
		public virtual bool AutoSaved
		{
			get { return (this.autoSaved); }
			set
			{
				if (this.autoSaved != value)
				{
					this.autoSaved = value;

					// Do not set changed.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Explicit")]
		public virtual ExplicitSettings Explicit
		{
			get { return (this.explicit_); }
			set
			{
				if (value == null)
				{
					DetachNode(this.explicit_);
					this.explicit_ = null;
				}
				else if (this.explicit_ == null)
				{
					this.explicit_ = value;
					AttachNode(this.explicit_);
				}
				else if (this.explicit_ != value)
				{
					ExplicitSettings old = this.explicit_;
					this.explicit_ = value;
					ReplaceNode(old, this.explicit_);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Implicit")]
		public virtual ImplicitSettings Implicit
		{
			get { return (this.implicit_); }
			set
			{
				if (value == null)
				{
					DetachNode(this.implicit_);
					this.implicit_ = null;
				}
				else if (this.implicit_ == null)
				{
					this.implicit_ = value;
					AttachNode(this.implicit_);
				}
				else if (this.implicit_ != value)
				{
					ImplicitSettings old = this.implicit_;
					this.implicit_ = value;
					ReplaceNode(old, this.implicit_);
				}
			}
		}

		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		/// <remarks>
		/// \remind (2008-06-07 / mky) (2 hours to the first Euro2008 game :-)
		/// Instead of this approach, an [AlternateXmlElementAttribute] based approach should be tried
		/// in a future version. Such approach would be beneficial in terms of modularity because the
		/// XML path wouldn't need to be considered, i.e. changes in the path could be handled. This is
		/// not the case currently.
		/// \remind (2011-10-09 / mky) (No Euro2012 games with Switzerland :-(
		/// Cannot alternate 'Display.ShowConnectTime|ShowCounters' to 'Status.ShowConnectTime|ShowCountAndRate'
		/// due to limitation described above.
		/// \remind (2012-10-29 / mky)
		/// Attention, the solution above is OK for the give use case, however, it wouldn't allow to
		/// alternate the depth of the path as well. Such alternate is required for the commented
		/// case with 'EolComment' below.
		/// </remarks>
		private static readonly MKY.Xml.AlternateXmlElement[] StaticAlternateXmlElements =
		{
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO"                                     }, "Endianness",                                new string[] { "Endianess" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "SerialPort", "Communication"      }, "FlowControl",                               new string[] { "Handshake" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "Socket"                           }, "RemoteTcpPort",                             new string[] { "RemotePort" } ),
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "UsbSerialHidDevice", "DeviceInfo" }, "SerialString",                              new string[] { "SerialNumber" } ), Should be renamed, but doesn't work because it is part of a serializable object. Should be solved using XML transformation. */
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal"                                           }, "Status",                                    new string[] { "Display" } ), */
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "TextTerminal"                           }, new string[] { "EolComment", "Indicators" }, new string[] { "EolCommentIndicators" } ), */
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "Display"                                }, "ShowTime",                                  new string[] { "ShowTimeStamp" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Format"                                             }, "TimeFormat",                                new string[] { "TimeStampFormat" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Log"                                                }, "FolderFormat",                              new string[] { "SubdirectoriesFormat" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Log"                                                }, "FolderChannel",                             new string[] { "SubdirectoriesChannel" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "TerminalIsStarted",                         new string[] { "TerminalIsOpen" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "LogIsStarted",                              new string[] { "LogIsOpen" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "LogIsOn",                                   new string[] { "LogIsStarted" } ),
		};

		/// <summary></summary>
		[XmlIgnore]
		public virtual MKY.Xml.AlternateXmlElement[] AlternateXmlElements
		{
			get { return (StaticAlternateXmlElements); }
		}

		#endregion

		#region Property Shortcuts
		//------------------------------------------------------------------------------------------
		// Property Shortcuts
		//------------------------------------------------------------------------------------------

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual string UserName
		{
			get { return (this.explicit_.UserName); }
			set { this.explicit_.UserName = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual bool TerminalIsStarted
		{
			get { return (this.implicit_.TerminalIsStarted); }
			set { this.implicit_.TerminalIsStarted = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual bool LogIsOn
		{
			get { return (this.implicit_.LogIsOn); }
			set { this.implicit_.LogIsOn = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.TerminalType TerminalType
		{
			get { return (this.explicit_.Terminal.TerminalType); }
			set { this.explicit_.Terminal.TerminalType = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.TerminalSettings Terminal
		{
			get { return (this.explicit_.Terminal); }
			set { this.explicit_.Terminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.IOSettings IO
		{
			get { return (this.explicit_.Terminal.IO); }
			set { this.explicit_.Terminal.IO = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.IOType IOType
		{
			get { return (this.explicit_.Terminal.IO.IOType); }
			set { this.explicit_.Terminal.IO.IOType = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual MKY.IO.Ports.SerialPortId SerialPortId
		{
			get { return (this.explicit_.Terminal.IO.SerialPort.PortId); }
			set { this.explicit_.Terminal.IO.SerialPort.PortId = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual int SocketLocalPort
		{
			get { return (this.explicit_.Terminal.IO.Socket.LocalPort); }
			set { this.explicit_.Terminal.IO.Socket.LocalPort = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual string SocketRemoteHost
		{
			get { return (this.explicit_.Terminal.IO.Socket.RemoteHost); }
			set { this.explicit_.Terminal.IO.Socket.RemoteHost = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual int SocketRemotePort
		{
			get { return (this.explicit_.Terminal.IO.Socket.RemotePort); }
			set { this.explicit_.Terminal.IO.Socket.RemotePort = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.StatusSettings Status
		{
			get { return (this.explicit_.Terminal.Status); }
			set { this.explicit_.Terminal.Status = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.BufferSettings Buffer
		{
			get { return (this.explicit_.Terminal.Buffer); }
			set { this.explicit_.Terminal.Buffer = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.DisplaySettings Display
		{
			get { return (this.explicit_.Terminal.Display); }
			set { this.explicit_.Terminal.Display = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.CharReplaceSettings CharReplace
		{
			get { return (this.explicit_.Terminal.CharReplace); }
			set { this.explicit_.Terminal.CharReplace = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.SendSettings Send
		{
			get { return (this.explicit_.Terminal.Send); }
			set { this.explicit_.Terminal.Send = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.TextTerminalSettings TextTerminal
		{
			get { return (this.explicit_.Terminal.TextTerminal); }
			set { this.explicit_.Terminal.TextTerminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Domain.Settings.BinaryTerminalSettings BinaryTerminal
		{
			get { return (this.explicit_.Terminal.BinaryTerminal); }
			set { this.explicit_.Terminal.BinaryTerminal = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.SendCommandSettings SendCommand
		{
			get { return (this.implicit_.SendCommand); }
			set { this.implicit_.SendCommand = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.SendFileSettings SendFile
		{
			get { return (this.implicit_.SendFile); }
			set { this.implicit_.SendFile = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.PredefinedSettings Predefined
		{
			get { return (this.implicit_.Predefined); }
			set { this.implicit_.Predefined = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.WindowSettings Window
		{
			get { return (this.implicit_.Window); }
			set { this.implicit_.Window = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.LayoutSettings Layout
		{
			get { return (this.implicit_.Layout); }
			set { this.implicit_.Layout = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (this.explicit_.PredefinedCommand); }
			set { this.explicit_.PredefinedCommand = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.FormatSettings Format
		{
			get { return (this.explicit_.Format); }
			set { this.explicit_.Format = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Log.Settings.LogSettings Log
		{
			get { return (this.explicit_.Log); }
			set { this.explicit_.Log = value;  }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			TerminalSettingsRoot other = (TerminalSettingsRoot)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(ProductVersion == other.ProductVersion)
				//// Do not consider AutoSaved.
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				ProductVersion.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
