//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	[Serializable]
	public class DeviceInfo : IEquatable<DeviceInfo>, IComparable
	{
		#region Public Constants
		//==========================================================================================
		// Public Constants
		//==========================================================================================

		/// <summary></summary>
		public const int FirstVendorId  = 0x0000;

		/// <summary></summary>
		public const int LastVendorId   = 0xFFFF;

		/// <summary></summary>
		public const int FirstProductId = 0x0000;

		/// <summary></summary>
		public const int LastProductId  = 0xFFFF;

		/// <summary></summary>
		public const int DefaultVendorId = 0;

		/// <summary></summary>
		public const int DefaultProductId = 0;

		/// <summary></summary>
		public const string DefaultSeparator = " - ";

		/// <summary></summary>
		public static readonly Regex VendorIdRegex;

		/// <summary></summary>
		public static readonly Regex ProductIdRegex;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string path;

		private int vendorId = DefaultVendorId;
		private int productId = DefaultProductId;

		private string manufacturer;
		private string product;
		private string serialNumber;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		static DeviceInfo()
		{
			// "VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"
			VendorIdRegex  = new Regex(@"VID[^0-9a-fA-F](?<vendorId>[0-9a-fA-F]+)",  RegexOptions.IgnoreCase | RegexOptions.Compiled);
			ProductIdRegex = new Regex(@"PID[^0-9a-fA-F](?<productId>[0-9a-fA-F]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DeviceInfo()
		{
		}

		/// <summary></summary>
		public DeviceInfo(int vendorId, int productId)
			: this(vendorId, productId, "")
		{
			string systemPath, manufacturer, product, serialNumber;
			Device.GetDeviceInfoFromVidAndPid(vendorId, productId, out systemPath, out manufacturer, out product, out serialNumber);
			Initialize(systemPath, vendorId, productId, manufacturer, product, serialNumber);
		}

		/// <summary></summary>
		public DeviceInfo(int vendorId, int productId, string serialNumber)
		{
			string systemPath, manufacturer, product;
			Device.GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serialNumber, out systemPath, out manufacturer, out product);
			Initialize(systemPath, vendorId, productId, manufacturer, product, serialNumber);
		}

		/// <summary></summary>
		public DeviceInfo(string systemPath, int vendorId, int productId, string manufacturer, string product, string serialNumber)
		{
			Initialize(systemPath, vendorId, productId, manufacturer, product, serialNumber);
		}

		private void Initialize(string systemPath, int vendorId, int productId, string manufacturer, string product, string serialNumber)
		{
			if ((vendorId  < FirstVendorId)  || (vendorId  > LastVendorId))
				throw (new ArgumentOutOfRangeException("vendorId",  vendorId,  "Invalid vendor ID"));
			if ((productId < FirstProductId) || (productId > LastProductId))
				throw (new ArgumentOutOfRangeException("productId", productId, "Invalid product ID"));

			this.path = systemPath;

			this.vendorId  = vendorId;
			this.productId = productId;

			this.manufacturer = manufacturer;
			this.product      = product;
			this.serialNumber = serialNumber;
		}

		/// <summary></summary>
		public DeviceInfo(DeviceInfo rhs)
		{
			this.path = rhs.path;

			this.vendorId  = rhs.vendorId;
			this.productId = rhs.productId;

			this.manufacturer = rhs.manufacturer;
			this.product      = rhs.product;
			this.serialNumber = rhs.serialNumber;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual string Path
		{
			get { return (this.path); }
			set { this.path = value;  }
		}

		/// <summary></summary>
		[XmlElement("VendorId")]
		public virtual int VendorId
		{
			get { return (this.vendorId); }
			set { this.vendorId = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string VendorIdString
		{
			get { return (VendorId.ToString("X4")); }
		}

		/// <summary></summary>
		[XmlElement("ProductId")]
		public virtual int ProductId
		{
			get { return (this.productId); }
			set { this.productId = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string ProductIdString
		{
			get { return (ProductId.ToString("X4")); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string Manufacturer
		{
			get { return (this.manufacturer); }
			set { this.manufacturer = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string Product
		{
			get { return (this.product); }
			set { this.product = value;  }
		}

		/// <summary></summary>
		[XmlElement("SerialNumber")]
		public virtual string SerialNumber
		{
			get { return (this.serialNumber); }
			set { this.serialNumber = value;  }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Queries the USB device for user readable strings like vendor or product name.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual bool TryValidate()
		{
			if      (this.path.Length > 0)
			{
				return (Device.GetDeviceInfoFromPath(this.path, out this.vendorId, out this.productId, out this.manufacturer, out this.product, out this.serialNumber));
			}
			else if ((this.vendorId != 0) && (this.productId != 0))
			{
				if (this.serialNumber.Length > 0)
					return (Device.GetDeviceInfoFromVidAndPidAndSerial(this.vendorId, this.productId, this.serialNumber, out this.path, out this.manufacturer, out this.product));
				else
					return (Device.GetDeviceInfoFromVidAndPid(this.vendorId, this.productId, out this.path, out this.manufacturer, out this.product, out this.serialNumber));
			}
			else
			{
				return (false);
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			DeviceInfo casted = obj as DeviceInfo;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(DeviceInfo casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return (this.path == casted.path);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (ToString(true, true));
		}

		/// <summary></summary>
		public virtual string ToString(bool appendIds, bool appendInUseText)
		{
			StringBuilder sb = new StringBuilder();

			if (Manufacturer.Length > 0)
			{
				sb.Append(Manufacturer);         // "Company"
			}

			if (appendIds)
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company "

				sb.Append("(VID:");
				sb.Append(VendorIdString);       // "Company (VID:0ABC)"
				sb.Append(")");
			}

			if (Product.Length > 0)
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) "

				sb.Append(Product);              // "Company (VID:0ABC) Product"
			}

			if (appendIds)
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) Product "

				sb.Append("(PID:");
				sb.Append(ProductIdString);      // "Company (VID:0ABC) Product (PID:1234)"
				sb.Append(")");
			}

			if (SerialNumber.Length > 0)
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) Product (PID:1234) "

				sb.Append(SerialNumber);         // "Company (VID:0ABC) Product (PID:1234) 000123A"
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			StringBuilder sb = new StringBuilder();

			if (Product.Length > 0)
			{
				sb.Append(Product);              // "Product"
			}
			else
			{
				sb.Append("(PID:");
				sb.Append(ProductIdString);      // "(PID:1234)"
				sb.Append(")");
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Parses s for the first integer number and returns the corresponding device.
		/// </summary>
		public static DeviceInfo Parse(string s)
		{
			DeviceInfo result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(s + " does not specify a valid USB device ID"));
		}

		/// <summary>
		/// Tries to parse s for the first integer number and returns the corresponding device.
		/// </summary>
		public static bool TryParse(string s, out DeviceInfo result)
		{
			Match m;

			// e.g. "VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"
			m = VendorIdRegex.Match(s);
			if (m.Success)
			{
				int vendorId;
				if (int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, null, out vendorId))
				{
					m = ProductIdRegex.Match(s);
					if (m.Success)
					{
						int productId;
						if (int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, null, out productId))
						{
							result = new DeviceInfo(vendorId, productId);
							return (true);
						}
					}
				}
			}

			result = null;
			return (false);
		}

		#endregion

		#region IComparable Members

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return (1);
			if (obj is DeviceInfo)
			{
				DeviceInfo id = (DeviceInfo)obj;
				if      (VendorId != id.VendorId)
					return (VendorId.CompareTo(id.VendorId));
				else if (ProductId != id.ProductId)
					return (ProductId.CompareTo(id.ProductId));
				else
					return (SerialNumber.CompareTo(id.SerialNumber));
			}
			throw (new ArgumentException("Object is not a UsbDeviceId entry"));
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is DeviceInfo)
			{
				DeviceInfo casted = (DeviceInfo)objA;
				return (casted.CompareTo(objB));
			}
			return (-1);
		}

		#endregion

		#region Comparison Operators
		//==========================================================================================
		// Comparison Operators
		//==========================================================================================

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(DeviceInfo lhs, DeviceInfo rhs)
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
		public static bool operator !=(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator string(DeviceInfo id)
		{
			return (id.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
