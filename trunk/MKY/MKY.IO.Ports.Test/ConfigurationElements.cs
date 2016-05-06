//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Configuration;

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
			string strA = ((SerialPortPairConfigurationElement)element).PortA;
			string strB = ((SerialPortPairConfigurationElement)element).PortB;

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
