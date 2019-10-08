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
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Ports.Test
{
	#region Port
	//==============================================================================================
	// Port
	//==============================================================================================

	/// <summary></summary>
	public class SerialPortConfigurationElement : ConfigurationElement
	{
		private const string DefaultPort = "COM1";

		/// <summary></summary>
		public SerialPortConfigurationElement()
			: this(DefaultPort)
		{
		}

		/// <summary></summary>
		public SerialPortConfigurationElement(string serialPort)
		{
			this["Port"] = serialPort;
		}

		/// <summary></summary>
		[ConfigurationProperty("Port", DefaultValue = DefaultPort, IsRequired = true, IsKey = true)]
		public string Port
		{
			get { return ((string)this["Port"]); }
			set { this["Port"] = value;          }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Inheriting from 'ConfigurationElementCollection'.")]
	[ConfigurationCollection(typeof(SerialPortConfigurationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public class SerialPortConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary></summary>
		public SerialPortConfigurationElementCollection()
		{
		}

		/// <summary></summary>
		protected override ConfigurationElement CreateNewElement()
		{
			return (new SerialPortConfigurationElement());
		}

		/// <summary></summary>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return (((SerialPortConfigurationElement)element).Port);
		}

		/// <summary></summary>
		public void Add(string serialPort)
		{
			BaseAdd(new SerialPortConfigurationElement(serialPort));
		}

		/// <summary></summary>
		public SerialPortConfigurationElement this[int index]
		{
			get { return ((SerialPortConfigurationElement)BaseGet(index)); }
		}
	}

	#endregion

	#region PortPair
	//==============================================================================================
	// PortPair
	//==============================================================================================

	/// <summary></summary>
	public class SerialPortPairConfigurationElement : ConfigurationElement
	{
		private const string DefaultPortA = "COM1";
		private const string DefaultPortB = "COM2";

		/// <summary></summary>
		public SerialPortPairConfigurationElement()
			: this(DefaultPortA, DefaultPortB)
		{
		}

		/// <summary></summary>
		public SerialPortPairConfigurationElement(string portA, string portB)
		{
			this["PortA"] = portA;
			this["PortB"] = portB;
		}

		/// <summary></summary>
		[ConfigurationProperty("PortA", DefaultValue = DefaultPortA, IsRequired = true, IsKey = true)]
		public string PortA
		{
			get { return ((string)this["PortA"]); }
			set { this["PortA"] = value;          }
		}

		/// <summary></summary>
		[ConfigurationProperty("PortB", DefaultValue = DefaultPortB, IsRequired = true, IsKey = true)]
		public string PortB
		{
			get { return ((string)this["PortB"]); }
			set { this["PortB"] = value;          }
		}
	}

	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Inheriting from 'ConfigurationElementCollection'.")]
	[ConfigurationCollection(typeof(SerialPortPairConfigurationElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public class SerialPortPairConfigurationElementCollection : ConfigurationElementCollection
	{
		/// <summary></summary>
		public SerialPortPairConfigurationElementCollection()
		{
		}

		/// <summary></summary>
		protected override ConfigurationElement CreateNewElement()
		{
			return (new SerialPortPairConfigurationElement());
		}

		/// <summary></summary>
		protected override object GetElementKey(ConfigurationElement element)
		{
			SerialPortPairConfigurationElement casted = (SerialPortPairConfigurationElement)element;

			string strA = casted.PortA;
			string strB = casted.PortB;

			return (strA + " " + strB);
		}

		/// <summary></summary>
		public void Add(string portA, string portB)
		{
			BaseAdd(new SerialPortPairConfigurationElement(portA, portB));
		}

		/// <summary></summary>
		public SerialPortPairConfigurationElement this[int index]
		{
			get { return ((SerialPortPairConfigurationElement)BaseGet(index)); }
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
