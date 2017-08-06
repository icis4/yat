//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

using YAT.Application.Utilities;

#endregion

namespace YAT.Settings.Terminal
{
	/// <summary></summary>
	[XmlRoot("Settings")]
	public class TerminalSettingsRoot : MKY.Settings.SettingsItem, IEquatable<TerminalSettingsRoot>, MKY.Xml.IAlternateXmlElementProvider
	{
		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string settingsVersion = "1.5.1";

		/// <remarks>Is basically constant, but must be a variable for automatic XML serialization.</remarks>
		private string productVersion = ApplicationEx.ProductVersion;

		private bool autoSaved;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "'explicit' is a key word.")]
		private ExplicitSettings explicit_;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "'implicit' is a key word.")]
		private ImplicitSettings implicit_;

		/// <summary></summary>
		public TerminalSettingsRoot()
			: base(MKY.Settings.SettingsType.Explicit)
		{
			Explicit = new ExplicitSettings(MKY.Settings.SettingsType.Explicit);
			Implicit = new ImplicitSettings(MKY.Settings.SettingsType.Implicit);

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
			get { return (ApplicationEx.ProductName + " Terminal Settings"); }
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
				if (this.explicit_ != value)
				{
					var oldNode = this.explicit_;
					this.explicit_ = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
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
				if (this.implicit_ != value)
				{
					var oldNode = this.implicit_;
					this.implicit_ = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		#region Properties > Shortcuts
		//------------------------------------------------------------------------------------------
		// Properties > Shortcuts
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
			get { return (this.explicit_.LogIsOn); }
			set { this.explicit_.LogIsOn = value;  }
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
		public virtual Domain.Settings.CharHideSettings CharHide
		{
			get { return (this.explicit_.Terminal.CharHide); }
			set { this.explicit_.Terminal.CharHide = value;  }
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
		public virtual Model.Settings.PredefinedCommandSettings PredefinedCommand
		{
			get { return (this.explicit_.PredefinedCommand); }
			set { this.explicit_.PredefinedCommand = value;  }
		}

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.AutoResponseSettings AutoResponse
		{
			get { return (this.explicit_.AutoResponse); }
			set { this.explicit_.AutoResponse = value;  }
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

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public virtual Model.Settings.SendTextSettings SendText
		{
			get { return (this.implicit_.SendText); }
			set { this.implicit_.SendText = value;  }
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
		public virtual Model.Settings.ViewSettings View
		{
			get { return (this.implicit_.View); }
			set { this.implicit_.View = value;  }
		}

		#endregion

		#region Properties > Combinations
		//------------------------------------------------------------------------------------------
		// Properties > Combinations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// The currently valid response items usable for automatic response.
		/// </summary>
		public Model.Types.AutoTriggerEx[] GetValidAutoResponseTriggerItems()
		{
			Model.Types.AutoTriggerEx[] triggers = Model.Types.AutoTriggerEx.GetAllItems();
			List<Model.Types.AutoTriggerEx> a = new List<Model.Types.AutoTriggerEx>(triggers.Length); // Preset the required capacity to improve memory management.

			foreach (Model.Types.AutoTriggerEx trigger in triggers)
			{
				switch ((Model.Types.AutoTrigger)trigger)
				{
					case Model.Types.AutoTrigger.PredefinedCommand1:
					case Model.Types.AutoTrigger.PredefinedCommand2:
					case Model.Types.AutoTrigger.PredefinedCommand3:
					case Model.Types.AutoTrigger.PredefinedCommand4:
					case Model.Types.AutoTrigger.PredefinedCommand5:
					case Model.Types.AutoTrigger.PredefinedCommand6:
					case Model.Types.AutoTrigger.PredefinedCommand7:
					case Model.Types.AutoTrigger.PredefinedCommand8:
					case Model.Types.AutoTrigger.PredefinedCommand9:
					case Model.Types.AutoTrigger.PredefinedCommand10:
					case Model.Types.AutoTrigger.PredefinedCommand11:
					case Model.Types.AutoTrigger.PredefinedCommand12:
					{
						int pageId = Predefined.SelectedPage;
						int commandId = trigger.ToPredefinedCommandId();
						if (commandId != Model.Types.AutoTriggerEx.InvalidPredefinedCommandId)
						{
							Model.Types.Command c = PredefinedCommand.GetCommand(pageId - 1, commandId - 1);
							if ((c != null) && (c.IsValid))
								a.Add(trigger);
						}

						break;
					}

					case Model.Types.AutoTrigger.Explicit:
					{
						Model.Types.Command c = new Model.Types.Command(AutoResponse.Trigger); // No explicit default radix available (yet).
						if (c.IsValid)
							a.Add(trigger);

						break;
					}

					case Model.Types.AutoTrigger.AnyLine:
					case Model.Types.AutoTrigger.None:
					default:
					{
						a.Add(trigger); // Always add these fixed responses.
						break;
					}
				}
			}

			return (a.ToArray());
		}

		/// <summary>
		/// The currently valid response items usable for automatic response.
		/// </summary>
		public Model.Types.AutoResponseEx[] GetValidAutoResponseResponseItems()
		{
			Model.Types.AutoResponseEx[] responses = Model.Types.AutoResponseEx.GetAllItems();
			List<Model.Types.AutoResponseEx> a = new List<Model.Types.AutoResponseEx>(responses.Length); // Preset the required capacity to improve memory management.

			foreach (Model.Types.AutoResponseEx response in responses)
			{
				switch ((Model.Types.AutoResponse)response)
				{
					case Model.Types.AutoResponse.PredefinedCommand1:
					case Model.Types.AutoResponse.PredefinedCommand2:
					case Model.Types.AutoResponse.PredefinedCommand3:
					case Model.Types.AutoResponse.PredefinedCommand4:
					case Model.Types.AutoResponse.PredefinedCommand5:
					case Model.Types.AutoResponse.PredefinedCommand6:
					case Model.Types.AutoResponse.PredefinedCommand7:
					case Model.Types.AutoResponse.PredefinedCommand8:
					case Model.Types.AutoResponse.PredefinedCommand9:
					case Model.Types.AutoResponse.PredefinedCommand10:
					case Model.Types.AutoResponse.PredefinedCommand11:
					case Model.Types.AutoResponse.PredefinedCommand12:
					{
						int pageId = Predefined.SelectedPage;
						int commandId = response.ToPredefinedCommandId();
						if (commandId != Model.Types.AutoResponseEx.InvalidPredefinedCommandId)
						{
							Model.Types.Command c = this.explicit_.PredefinedCommand.GetCommand(pageId - 1, commandId - 1);
							if ((c != null) && (c.IsValid))
								a.Add(response);
						}

						break;
					}

					case Model.Types.AutoResponse.SendText:
					{
						Model.Types.Command c = SendText.Command;
						if ((c != null) && (c.IsValid))
							a.Add(response);

						break;
					}

					case Model.Types.AutoResponse.SendFile:
					{
						Model.Types.Command c = SendFile.Command;
						if ((c != null) && (c.IsValid))
							a.Add(response);

						break;
					}

					case Model.Types.AutoResponse.Explicit:
					{
						Model.Types.Command c = new Model.Types.Command(AutoResponse.Response); // No explicit default radix available (yet).
						if (c.IsValid)
							a.Add(response);

						break;
					}

					case Model.Types.AutoResponse.None:
					default:
					{
						a.Add(response); // Always add these fixed responses.
						break;
					}
				}
			}

			return (a.ToArray());
		}

		/// <summary>
		/// The currently active response used for automatic response.
		/// </summary>
		[XmlIgnore]
		public virtual Model.Types.Command ActiveAutoResponseTrigger
		{
			get
			{
				Model.Types.Command response = null;

				if (this.AutoResponse.TriggerIsActive)
				{
					switch ((Model.Types.AutoTrigger)this.AutoResponse.Trigger)
					{
						case Model.Types.AutoTrigger.PredefinedCommand1:
						case Model.Types.AutoTrigger.PredefinedCommand2:
						case Model.Types.AutoTrigger.PredefinedCommand3:
						case Model.Types.AutoTrigger.PredefinedCommand4:
						case Model.Types.AutoTrigger.PredefinedCommand5:
						case Model.Types.AutoTrigger.PredefinedCommand6:
						case Model.Types.AutoTrigger.PredefinedCommand7:
						case Model.Types.AutoTrigger.PredefinedCommand8:
						case Model.Types.AutoTrigger.PredefinedCommand9:
						case Model.Types.AutoTrigger.PredefinedCommand10:
						case Model.Types.AutoTrigger.PredefinedCommand11:
						case Model.Types.AutoTrigger.PredefinedCommand12:
						{
							int pageId = Predefined.SelectedPage;
							int commandId = AutoResponse.Trigger.ToPredefinedCommandId();
							if (commandId != Model.Types.AutoTriggerEx.InvalidPredefinedCommandId)
							{
								Model.Types.Command c = this.explicit_.PredefinedCommand.GetCommand(pageId - 1, commandId - 1);
								if ((c != null) && (c.IsValid))
									response = c;
							}

							break;
						}

						case Model.Types.AutoTrigger.Explicit:
						{
							Model.Types.Command c = new Model.Types.Command(AutoResponse.Trigger); // No explicit default radix available (yet).
							if (c.IsValid)
								response = c;

							break;
						}

						case Model.Types.AutoTrigger.AnyLine:
						case Model.Types.AutoTrigger.None:
						default:
						{
							break;
						}
					}
				}

				return (response);
			}
		}

		#endregion

		#endregion

		#region Alternate Elements
		//==========================================================================================
		// Alternate Elements
		//==========================================================================================

		/// <summary>
		/// Alternate XML elements for backward compatibility with old settings.
		/// </summary>
		/// <remarks>
		/// \remind (2008-06-07 / MKY) (2 hours to the first Euro2008 game :-)
		/// Instead of this approach, an [AlternateXmlElementAttribute] based approach should be tried
		/// in a future version. Such approach would be beneficial in terms of modularity because the
		/// XML path wouldn't need to be considered, i.e. changes in the path could be handled. This is
		/// not the case currently.
		/// \remind (2011-10-09 / MKY) (no Euro2012 games with Switzerland :-(
		/// Cannot alternate 'Display.ShowConnectTime|ShowCounters' to 'Status.ShowConnectTime|ShowCountAndRate'
		/// due to limitation described above.
		/// \remind (2012-10-29 / MKY)
		/// Attention, the solution above is OK for the give use case, however, it wouldn't allow to
		/// alternate the depth of the path as well. Such alternate is required for the commented
		/// case with 'EolComment' below.
		/// \remind (2016-04-05 / MKY)
		/// Ideally, simple alternate elements should be definable right at the element. Example:
		/// "SocketSettings.HostType" got simplified to "SocketSettings.Type"
		/// The alternate name (i.e. the old name) should be definable in 'SocketSettings'.
		/// </remarks>
		private static readonly MKY.Xml.AlternateXmlElement[] StaticAlternateXmlElements =
		{                                                // XML path:                                                                                    local name of XML element:                                               alternate local name(s), i.e. former name(s) of XML element:
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO"                                     }, "Endianness",                                             new string[] { "Endianess" } ),
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO"                                     }, new string[] { "SerialPort", "IndicateBreakStates" },     new string[] { "IndicateSerialPortBreakStates" } ),     => Should be moved, but doesn't work because new name is at a deeper level. Should be solved using XML transformation. */
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO"                                     }, new string[] { "SerialPort", "OutputBreakIsModifiable" }, new string[] { "SerialPortOutputBreakIsModifiable" } ), => Should be moved, but doesn't work because new name is at a deeper level. Should be solved using XML transformation. */
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "SerialPort", "Communication"      }, "FlowControl",                                            new string[] { "Handshake" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "SerialPort"                       }, "OutputBufferSize",                                       new string[] { "LimitOutputBuffer" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "Socket"                           }, "Type",                                                   new string[] { "HostType" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "Socket"                           }, "RemoteTcpPort",                                          new string[] { "RemotePort" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "UsbSerialHidDevice"               }, "RxFilterUsage",                                          new string[] { "RxIdUsage" } ),
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "IO", "UsbSerialHidDevice", "DeviceInfo" }, "SerialString",                                           new string[] { "SerialNumber" } ), => Should be renamed, but doesn't work because it is part of a serializable object. Should be solved using XML transformation. */
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "TextTerminal"                           }, new string[] { "EolComment", "Indicators" },              new string[] { "EolCommentIndicators" } ), => Should be moved, but doesn't work because new name is at a deeper level. Should be solved using XML transformation. */
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "BinaryTerminal", "TxDisplay"            }, "SequenceLineBreakAfter",                                 new string[] { "SequenceLineBreak" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "Display"                                }, "ShowBufferLineNumbers",                                  new string[] { "ShowLineNumbers" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "Display"                                }, "ShowTime",                                               new string[] { "ShowTimeStamp" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "Display"                                }, "MaxLineCount",                                           new string[] { "TxMaxLineCount" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Terminal", "Send"                                   }, "DisableEscapes",                                         new string[] { "DisableKeywords" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Format"                                             }, "TimeFormat",                                             new string[] { "TimeStampFormat" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Log"                                                }, "FolderFormat",                                           new string[] { "SubdirectoriesFormat" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Explicit", "Log"                                                }, "FolderChannel",                                          new string[] { "SubdirectoriesChannel" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "TerminalIsStarted",                                      new string[] { "TerminalIsOpen" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "LogIsOn",                                                new string[] { "LogIsOpen" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "LogIsOn",                                                new string[] { "LogIsStarted" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit"                                                       }, "SendText",                                               new string[] { "SendCommand" } ),
			new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings", "Implicit", "Layout"                                             }, "SendTextPanelIsVisible",                                 new string[] { "SendCommandPanelIsVisible" } )
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings"                                                                   }, new string[] { "Implicit", "TerminalIsStarted" },         new string[] { "Explicit", "TerminalIsStarted" } ), => Should be moved, but doesn't work because names are at a deeper level. Should be solved using XML transformation. */
		/*	new MKY.Xml.AlternateXmlElement(new string[] { "#document", "Settings"                                                                   }, new string[] { "Explicit", "LogIsOn" },                   new string[] { "Implicit", "LogIsOn" } ),           => Should be moved, but doesn't work because names are at a deeper level. Should be solved using XML transformation. */
		};

		/// <summary></summary>
		[XmlIgnore]
		public virtual IEnumerable<MKY.Xml.AlternateXmlElement> AlternateXmlElements
		{
			get { return (StaticAlternateXmlElements); }
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (ProductVersion != null ? ProductVersion.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TerminalSettingsRoot));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TerminalSettingsRoot other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(ProductVersion, other.ProductVersion)
				//// Do not consider AutoSaved.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalSettingsRoot lhs, TerminalSettingsRoot rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
