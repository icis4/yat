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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY.Net;

#endregion

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	public class SocketSettings : Settings.SettingsItem, IEquatable<SocketSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="IPHostEx"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static IPHostEx RemoteHostDefault
		{
			get { return (new IPHostEx(IPHost.Localhost)); }
		}

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static IPNetworkInterfaceDescriptorPair LocalInterfaceDefault
		{
			get { return (new IPNetworkInterfaceEx(IPNetworkInterface.Any).ToDescriptorPair()); }
		}

		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="IPFilterEx"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static IPFilterEx LocalFilterDefault
		{
			get { return (new IPFilterEx(IPFilter.Any)); }
		}

		/// <summary></summary>
		public const int RemotePortDefault = 10000;

		/// <summary></summary>
		public const int LocalPortDefault = 10000;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static IntervalSettingTuple TcpClientAutoReconnectDefault
		{
			get { return (new IntervalSettingTuple(true, 500)); }
		}

		/// <summary></summary>
		public const int TcpClientAutoReconnectMinInterval = 100;

		/// <summary></summary>
		public const UdpServerSendMode UdpServerSendModeDefault = UdpServerSendMode.MostRecent;

		private const string Undefined = "(undefined)";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SocketType type;

		private IPHostEx remoteHost;
		private int remoteTcpPort;
		private int remoteUdpPort;

		private IPNetworkInterfaceDescriptorPair localInterface;
		private IPFilterEx localFilter;
		private int localTcpPort;
		private int localUdpPort;

		private IntervalSettingTuple tcpClientAutoReconnect;
		private UdpServerSendMode udpServerSendMode;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SocketSettings()
			: this(Settings.SettingsType.Explicit)
		{
		}

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SocketSettings(Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SocketSettings(SocketType type)
			: this(type, RemoteHostDefault)
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort = RemotePortDefault, int remoteUdpPort = RemotePortDefault)
			: this(type, remoteHost, remoteTcpPort, remoteUdpPort, LocalInterfaceDefault)
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort, int remoteUdpPort, IPNetworkInterfaceDescriptorPair localInterface)
			: this(type, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, LocalFilterDefault)
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort, int remoteUdpPort, IPNetworkInterfaceDescriptorPair localInterface, string localFilter, int localTcpPort = LocalPortDefault, int localUdpPort = LocalPortDefault)
			: this(type, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort, localUdpPort, TcpClientAutoReconnectDefault)
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public SocketSettings(SocketType type, string remoteHost, int remoteTcpPort, int remoteUdpPort, IPNetworkInterfaceDescriptorPair localInterface, string localFilter, int localTcpPort, int localUdpPort, IntervalSettingTuple tcpClientAutoReconnect, UdpServerSendMode udpServerSendMode = UdpServerSendModeDefault)
		{
			Type           = type;

			RemoteHost     = remoteHost;
			RemoteTcpPort  = remoteTcpPort;
			RemoteUdpPort  = remoteUdpPort;

			LocalInterface = localInterface;
			LocalFilter    = localFilter;
			LocalTcpPort   = localTcpPort;
			LocalUdpPort   = localUdpPort;

			TcpClientAutoReconnect = tcpClientAutoReconnect;
			UdpServerSendMode      = udpServerSendMode;

			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SocketSettings(SocketSettings rhs)
			: base(rhs)
		{
			Type           = rhs.Type;

			RemoteHost     = rhs.RemoteHost;
			RemoteTcpPort  = rhs.RemoteTcpPort;
			RemoteUdpPort  = rhs.RemoteUdpPort;

			LocalInterface = rhs.LocalInterface;
			LocalFilter    = rhs.LocalFilter;
			LocalTcpPort   = rhs.LocalTcpPort;
			LocalUdpPort   = rhs.LocalUdpPort;

			TcpClientAutoReconnect = rhs.TcpClientAutoReconnect;
			UdpServerSendMode      = rhs.UdpServerSendMode;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Type           = SocketType.TcpAutoSocket;

			RemoteHost     = RemoteHostDefault;
			RemoteTcpPort  = RemotePortDefault;
			RemoteUdpPort  = RemotePortDefault;

			LocalInterface = LocalInterfaceDefault;
			LocalFilter    = LocalFilterDefault;
			LocalTcpPort   = LocalPortDefault;
			LocalUdpPort   = LocalPortDefault;

			TcpClientAutoReconnect = TcpClientAutoReconnectDefault;
			UdpServerSendMode      = UdpServerSendModeDefault;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// Named 'Type' even though same as <see cref="System.Type"/>. But using this property
		/// as <code>Socket.Type</code> is more intuitive than <code>Socket.SocketType</code>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Well, there are other types than object types...")]
		[XmlElement("Type")]
		public virtual SocketType Type
		{
			get { return (this.type); }
			set
			{
				if (this.type != value)
				{
					this.type = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPHostEx RemoteHost
		{
			get { return (this.remoteHost); }
			set
			{
				if (this.remoteHost != value)
				{
					this.remoteHost = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("RemoteHost")]
		public virtual string RemoteHost_ForSerialization
		{
			get { return (RemoteHost.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { RemoteHost = value;                    }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int RemotePort
		{
			get
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						return (RemoteTcpPort);

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						return (RemoteUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						RemoteTcpPort = value;
						break;

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						RemoteUdpPort = value;
						break;

					default:
						break; // Do nothing.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteTcpPort")]
		public virtual int RemoteTcpPort
		{
			get { return (this.remoteTcpPort); }
			set
			{
				if (this.remoteTcpPort != value)
				{
					this.remoteTcpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RemoteUdpPort")]
		public virtual int RemoteUdpPort
		{
			get { return (this.remoteUdpPort); }
			set
			{
				if (this.remoteUdpPort != value)
				{
					this.remoteUdpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string RemoteEndPointString
		{
			get { return (this.remoteHost.ToEndpointAddressString() + ":" + RemotePort); }
		}

		/// <summary></summary>
		[XmlElement("LocalInterface")]
		public virtual IPNetworkInterfaceDescriptorPair LocalInterface
		{
			get { return (this.localInterface); }
			set
			{
				if (this.localInterface != value)
				{
					this.localInterface = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public virtual IPFilterEx LocalFilter
		{
			get { return (this.localFilter); }
			set
			{
				if (this.localFilter != value)
				{
					this.localFilter = value;
					SetMyChanged();

					// Do not try to resolve the IP address as this may take quite some time!
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("LocalFilter")]
		public virtual string LocalFilter_ForSerialization
		{
			get { return (LocalFilter.ToCompactString()); } // Use compact string represenation, only taking host name or address into account!
			set { LocalFilter = value;                    }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int LocalPort
		{
			get
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						return (LocalTcpPort);

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						return (LocalUdpPort);

					default:
						return (0);
				}
			}
			set
			{
				switch (this.type)
				{
					case SocketType.TcpClient:
					case SocketType.TcpServer:
					case SocketType.TcpAutoSocket:
						LocalTcpPort = value;
						break;

					case SocketType.UdpClient:
					case SocketType.UdpServer:
					case SocketType.UdpPairSocket:
						LocalUdpPort = value;
						break;

					default:
						break; // Do nothing.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalTcpPort")]
		public virtual int LocalTcpPort
		{
			get { return (this.localTcpPort); }
			set
			{
				if (this.localTcpPort != value)
				{
					this.localTcpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LocalUdpPort")]
		public virtual int LocalUdpPort
		{
			get { return (this.localUdpPort); }
			set
			{
				if (this.localUdpPort != value)
				{
					this.localUdpPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TcpClientAutoReconnect")]
		public virtual IntervalSettingTuple TcpClientAutoReconnect
		{
			get { return (this.tcpClientAutoReconnect); }
			set
			{
				if (this.tcpClientAutoReconnect != value)
				{
					this.tcpClientAutoReconnect = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UdpServerSendMode")]
		public virtual UdpServerSendMode UdpServerSendMode
		{
			get { return (this.udpServerSendMode); }
			set
			{
				if (this.udpServerSendMode != value)
				{
					this.udpServerSendMode = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				((SocketTypeEx)Type)   + ", " +
				RemoteHost             + ", " +
				RemoteTcpPort          + ", " +
				RemoteUdpPort          + ", " +
				LocalInterface         + ", " +
				LocalFilter            + ", " +
				LocalTcpPort           + ", " +
				LocalUdpPort           + ", " +
				TcpClientAutoReconnect
			);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Naming according to System.Net.EndPoint.")]
		public virtual string ToShortEndPointString()
		{
			switch (type)
			{
				case SocketType.TcpClient:     return (                                         this.remoteHost.ToEndpointAddressString() + ":" + this.remoteTcpPort);
				case SocketType.TcpServer:     return ("Server:"  + this.localTcpPort                                                                         );
				case SocketType.TcpAutoSocket: return ("Server:"  + this.localTcpPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remoteTcpPort);
				case SocketType.UdpClient:     return (                                         this.remoteHost.ToEndpointAddressString() + ":" + this.remoteUdpPort);
				case SocketType.UdpServer:     return ("Receive:" + this.localUdpPort                                                                         );
				case SocketType.UdpPairSocket: return ("Receive:" + this.localUdpPort + " / " + this.remoteHost.ToEndpointAddressString() + ":" + this.remoteUdpPort);

				default:                       return (Undefined);
			}
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^  Type                                                               .GetHashCode();
				hashCode = (hashCode * 397) ^ ( RemoteHost_ForSerialization != null ?  RemoteHost_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  RemoteTcpPort;
				hashCode = (hashCode * 397) ^  RemoteUdpPort;
				hashCode = (hashCode * 397) ^  LocalInterface                                                     .GetHashCode();
				hashCode = (hashCode * 397) ^ (LocalFilter_ForSerialization != null ? LocalFilter_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  LocalTcpPort;
				hashCode = (hashCode * 397) ^  LocalUdpPort;
				hashCode = (hashCode * 397) ^  TcpClientAutoReconnect                                             .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SocketSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SocketSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				Type                                                   .Equals(other.Type)                         &&
				StringEx.EqualsOrdinalIgnoreCase( RemoteHost_ForSerialization, other.RemoteHost_ForSerialization)  &&
				RemoteTcpPort                                          .Equals(other.RemoteTcpPort)                &&
				RemoteUdpPort                                          .Equals(other.RemoteUdpPort)                &&
				LocalInterface                                         .Equals(other.LocalInterface)               &&
				StringEx.EqualsOrdinalIgnoreCase(LocalFilter_ForSerialization, other.LocalFilter_ForSerialization) &&
				LocalTcpPort                                           .Equals(other.LocalTcpPort)                 &&
				LocalUdpPort                                           .Equals(other.LocalUdpPort)                 &&
				TcpClientAutoReconnect                                 .Equals(other.TcpClientAutoReconnect)       &&
				UdpServerSendMode                                      .Equals(other.UdpServerSendMode)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SocketSettings lhs, SocketSettings rhs)
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
		public static bool operator !=(SocketSettings lhs, SocketSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <summary>
		/// Parses <paramref name="s"/> for socket settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SocketSettings Parse(string s)
		{
			SocketSettings result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify valid socket settings."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for socket settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SocketSettings settings)
		{
			var delimiters = " ,;|"; // '/' is not a valid delimiter as host address likely contains a '/'.
			var sa = s.Trim().Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (sa.Length > 0)
			{
				SocketType socketType;
				if (SocketTypeEx.TryParse(sa[0], out socketType))
				{
					if (sa.Length > 1)
					{
						string remoteHost = sa[1].Trim();

						if (sa.Length > 2)
						{
							int remoteTcpPort;
							if (int.TryParse(sa[2], out remoteTcpPort))
							{
								if (sa.Length > 3)
								{
									int remoteUdpPort;
									if (int.TryParse(sa[3], out remoteUdpPort))
									{
										if (sa.Length > 5)
										{
											var localInterface = new IPNetworkInterfaceDescriptorPair(sa[4].Trim(), sa[5].Trim());

											if (sa.Length > 6)
											{
												var localFilter = sa[6].Trim();

												if (sa.Length > 7)
												{
													int localTcpPort;
													if (int.TryParse(sa[7], out localTcpPort))
													{
														if (sa.Length > 8)
														{
															int localUdpPort;
															if (int.TryParse(sa[8], out localUdpPort))
															{
																if (sa.Length > 10)
																{
																	bool arEnabled;
																	int arInterval;
																	if (bool.TryParse(sa[9], out arEnabled) &&
																	    int.TryParse(sa[10], out arInterval))
																	{
																		var autoRetry = new IntervalSettingTuple(arEnabled, arInterval);

																		if (sa.Length > 11)
																		{
																			int smValue;
																			if (int.TryParse(sa[11], out smValue))
																			{
																				var sendMode = (UdpServerSendModeEx)smValue;

																				settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort, localUdpPort, autoRetry, sendMode);
																				return (true);
																			}
																		}
																		else
																		{
																			settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort, localUdpPort, autoRetry);
																			return (true);
																		}
																	}
																}
																else
																{
																	settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort, localUdpPort);
																	return (true);
																}
															}
														}
														else
														{
															settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter, localTcpPort);
															return (true);
														}
													}
												}
												else
												{
													settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface, localFilter);
													return (true);
												}
											}
											else
											{
												settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort, localInterface);
												return (true);
											}
										}
										else
										{
											settings = new SocketSettings(socketType, remoteHost, remoteTcpPort, remoteUdpPort);
											return (true);
										}
									}
								}
								else
								{
									settings = new SocketSettings(socketType, remoteHost, remoteTcpPort);
									return (true);
								}
							}
						}
						else
						{
							settings = new SocketSettings(socketType, remoteHost);
							return (true);
						}
					}
					else
					{
						settings = new SocketSettings(socketType);
						return (true);
					}
				}
			}

			settings = null;
			return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
