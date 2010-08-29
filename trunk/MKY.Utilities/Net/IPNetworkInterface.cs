//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;

using MKY.Utilities.Types;

namespace MKY.Utilities.Net
{
	#region Enum IPNetworkInterfaceType

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IPNetworkInterfaceType
	{
		Any,
		IPv4Any,
		IPv4Loopback,
		IPv6Any,
		IPv6Loopback,
		Other,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum IPNetworkInterface.
	/// </summary>
	public class IPNetworkInterface : XEnum
	{
		#region String Definitions

		private const string Any_string          = "any";
		private const string Any_stringNice      = "<Any>";
		private const string IPv4Any_string      = "IPv4 any";
		private const string IPv4Loopback_string = "IPv4 loopback";
		private const string IPv6Any_string      = "IPv6 any";
		private const string IPv6Loopback_string = "IPv6 loopback";

		#endregion

		private IPAddress otherAddress = IPAddress.None;
		private string otherDescription = "";

		/// <summary>Default is <see cref="IPNetworkInterfaceType.Any"/>.</summary>
		public IPNetworkInterface()
			: base(IPNetworkInterfaceType.Any)
		{
		}

		/// <summary></summary>
		public IPNetworkInterface(IPNetworkInterfaceType networkInterface)
			: base(networkInterface)
		{
		}

		/// <summary></summary>
		public IPNetworkInterface(IPAddress address, string description)
		{
			if      (address == IPAddress.Any)          { SetUnderlyingEnum(IPNetworkInterfaceType.Any);          this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Any)          { SetUnderlyingEnum(IPNetworkInterfaceType.IPv4Any);      this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.Loopback)     { SetUnderlyingEnum(IPNetworkInterfaceType.IPv4Loopback); this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Any)      { SetUnderlyingEnum(IPNetworkInterfaceType.IPv6Any);      this.otherAddress = IPAddress.None; }
			else if (address == IPAddress.IPv6Loopback) { SetUnderlyingEnum(IPNetworkInterfaceType.IPv6Loopback); this.otherAddress = IPAddress.None; }
			else                                        { SetUnderlyingEnum(IPNetworkInterfaceType.Other);        this.otherAddress = address;        }

			this.otherDescription = description;
		}

		#region Properties

		/// <summary></summary>
		public IPAddress IPAddress
		{
			get
			{
				switch ((IPNetworkInterfaceType)UnderlyingEnum)
				{
					case IPNetworkInterfaceType.Any:          return (IPAddress.Any);
					case IPNetworkInterfaceType.IPv4Any:      return (IPAddress.Any);
					case IPNetworkInterfaceType.IPv4Loopback: return (IPAddress.Loopback);
					case IPNetworkInterfaceType.IPv6Any:      return (IPAddress.IPv6Any);
					case IPNetworkInterfaceType.IPv6Loopback: return (IPAddress.IPv6Loopback);
					case IPNetworkInterfaceType.Other:        return (this.otherAddress);
				}
				throw (new NotImplementedException(UnderlyingEnum.ToString()));
			}
		}

		#endregion

		#region Object Members
		//------------------------------------------------------------------------------------------
		// Object Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			IPNetworkInterface casted = obj as IPNetworkInterface;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(IPNetworkInterface casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			if ((IPNetworkInterfaceType)UnderlyingEnum == IPNetworkInterfaceType.Other)
			{
				return
				(
					base.Equals((XEnum)casted) &&

					(this.otherAddress     == casted.otherAddress) &&
					(this.otherDescription == casted.otherDescription)
				);
			}
			else
			{
				return (base.Equals((XEnum)casted));
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			switch ((IPNetworkInterfaceType)UnderlyingEnum)
			{
				case IPNetworkInterfaceType.Any:          return (Any_stringNice);
				case IPNetworkInterfaceType.IPv4Any:      return (IPv4Any_string      + " (" + IPAddress.Any + ")");
				case IPNetworkInterfaceType.IPv4Loopback: return (IPv4Loopback_string + " (" + IPAddress.Loopback + ")");
				case IPNetworkInterfaceType.IPv6Any:      return (IPv6Any_string      + " (" + IPAddress.IPv6Any + ")");
				case IPNetworkInterfaceType.IPv6Loopback: return (IPv6Loopback_string + " (" + IPAddress.IPv6Loopback + ")");
				case IPNetworkInterfaceType.Other:
				{
					if (this.otherDescription.Length > 0)
					{
						if (this.otherAddress != IPAddress.None)
							return (this.otherDescription + " (" + this.otherAddress + ")");
						else
							return (this.otherDescription);
					}
					else
					{
						if (this.otherAddress != IPAddress.None)
							return (this.otherAddress.ToString());
						else
							throw (new InvalidOperationException("IP address and interface description or both are undefined"));
					}
				}
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static IPNetworkInterface[] GetItems()
		{
			List<IPNetworkInterface> a = new List<IPNetworkInterface>();
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv4Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv4Loopback));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv6Any));
			a.Add(new IPNetworkInterface(IPNetworkInterfaceType.IPv6Loopback));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static IPNetworkInterface Parse(string networkInterface)
		{
			IPNetworkInterface result;

			if (TryParse(networkInterface, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("networkInterface", networkInterface, "Invalid network interface."));
		}

		/// <summary></summary>
		public static bool TryParse(string networkInterface, out IPNetworkInterface result)
		{
			if     ((string.Compare(networkInterface, Any_string, true) == 0) ||
					(string.Compare(networkInterface, Any_stringNice, true) == 0))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv4Any_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv4Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv4Loopback_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv4Loopback);
				return (true);
			}
			else if (networkInterface.Contains(IPv6Any_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv6Any);
				return (true);
			}
			else if (networkInterface.Contains(IPv6Loopback_string))
			{
				result = new IPNetworkInterface(IPNetworkInterfaceType.IPv6Loopback);
				return (true);
			}
			else
			{
				IPAddress address;
				if (IPAddress.TryParse(networkInterface, out address))
					result = new IPNetworkInterface(address, "");
				else
					result = new IPNetworkInterface(IPAddress.None, networkInterface);

				return (true);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator IPNetworkInterfaceType(IPNetworkInterface networkInterface)
		{
			return ((IPNetworkInterfaceType)networkInterface.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterface(IPNetworkInterfaceType networkInterface)
		{
			return (new IPNetworkInterface(networkInterface));
		}

		/// <summary></summary>
		public static implicit operator string(IPNetworkInterface networkInterface)
		{
			return (networkInterface.ToString());
		}

		/// <summary></summary>
		public static implicit operator IPNetworkInterface(string networkInterface)
		{
			return (Parse(networkInterface));
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IPNetworkInterface lhs, IPNetworkInterface rhs)
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
		public static bool operator !=(IPNetworkInterface lhs, IPNetworkInterface rhs)
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
