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
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
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
		public const string DefaultInUseText = "(in use)";

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

		private string _path;

		private int _vendorId = DefaultVendorId;
		private int _productId = DefaultProductId;

		private string _manufacturer;
		private string _product;
		private string _serialNumber;

		private bool _isInUse;
		private string _inUseText;

		private string _separator;

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

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// For USB, no device can be consiered to be the default device. Therefore, this method
		/// always returns <c>null</c>.
		/// </summary>
		/// <remarks>
		/// Do not create a new USB device collection here. Filling the collection with all
		/// available devices takes some time which makes little sense for USB as described above.
		/// </remarks>
		public static DeviceInfo GetDefaultDevice(DeviceClass deviceClass)
		{
			return (null);
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

			_path = systemPath;

			_vendorId = vendorId;
			_productId = productId;

			_manufacturer = manufacturer;
			_product = product;
			_serialNumber = serialNumber;
		}

		/// <summary></summary>
		public DeviceInfo(DeviceInfo rhs)
		{
			_path = rhs._path;

			_vendorId = rhs._vendorId;
			_productId = rhs._productId;

			_manufacturer = rhs._manufacturer;
			_product = rhs._product;
			_serialNumber = rhs._serialNumber;

			_isInUse = rhs._isInUse;
			_inUseText = rhs._inUseText;

			_separator = rhs._separator;
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
			get { return (_path); }
			set { _path = value;  }
		}

		/// <summary></summary>
		[XmlElement("VendorId")]
		public virtual int VendorId
		{
			get { return (_vendorId); }
			set
			{
				if (_vendorId != value)
					_vendorId = value;
			}
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
			get { return (_productId); }
			set
			{
				if (_productId != value)
					_productId = value;
			}
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
			get { return (_manufacturer); }
			set { _manufacturer = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string Product
		{
			get { return (_product); }
			set { _product = value;  }
		}

		/// <summary></summary>
		public virtual string SerialNumber
		{
			get { return (_serialNumber); }
			set { _serialNumber = value;  }
		}

		/// <summary>
		/// Indicates whether device is currently in use.
		/// </summary>
		[XmlIgnore]
		public virtual bool IsInUse
		{
			get { return (_isInUse); }
			set { _isInUse = value; }
		}

		/// <summary>
		/// The text which is shown when device is currently in use,
		/// e.g. "Company (VID:0ABC) Product (PID:1234) 000123A - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultInUseText)]
		public virtual string InUseText
		{
			get
			{
				if (_inUseText == "")
					return (DefaultInUseText);
				else
					return (_inUseText);
			}
			set
			{
				_inUseText = value;
			}
		}

		/// <summary>
		/// The separator,
		/// e.g. "Company (VID:0ABC) Product (PID:1234) 000123A - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultSeparator)]
		public virtual string Separator
		{
			get
			{
				if (_separator == "")
					return (DefaultSeparator);
				else
					return (_separator);
			}
			set
			{
				_separator = value;
			}
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
			if      (_path != "")
			{
				return (Device.GetDeviceInfoFromPath(_path, out _vendorId, out _productId, out _manufacturer, out _product, out _serialNumber));
			}
			else if ((_vendorId != 0) && (_productId != 0))
			{
				if (_serialNumber != "")
					return (Device.GetDeviceInfoFromVidAndPidAndSerial(_vendorId, _productId, _serialNumber, out _path, out _manufacturer, out _product));
				else
					return (Device.GetDeviceInfoFromVidAndPid(_vendorId, _productId, out _path, out _manufacturer, out _product, out _serialNumber));
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
			if (obj is DeviceInfo)
				return (Equals((DeviceInfo)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(DeviceInfo value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
				return (_path.Equals(value._path));

			return (false);
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

			if (Manufacturer != "")
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

			if (Product != "")
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

			if (SerialNumber != "")
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) Product (PID:1234) "

				sb.Append(SerialNumber);         // "Company (VID:0ABC) Product (PID:1234) 000123A"
			}

			if (appendInUseText && IsInUse)
			{
				sb.Append(Separator);            // "Company (VID:0ABC) Product (PID:1234) 000123A - "
				sb.Append(InUseText);            // "Company (VID:0ABC) Product (PID:1234) 000123A - (in use)"
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		public virtual string ToShortString()
		{
			StringBuilder sb = new StringBuilder();

			if (Product != "")
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
