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
// MKY Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#endregion

namespace MKY.IO.Usb
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
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
		public static readonly string FirstVendorIdString  = FirstVendorId .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string LastVendorIdString   = LastVendorId  .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string FirstProductIdString = FirstProductId.ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string LastProductIdString  = LastProductId .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public const int DefaultVendorId = FirstVendorId;

		/// <summary></summary>
		public const int DefaultProductId = FirstProductId;

		/// <remarks><![CDATA["VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"]]></remarks>
		public static readonly Regex VendorIdRegex = new Regex(@"VID[^0-9a-fA-F](?<vendorId>[0-9a-fA-F]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <remarks><![CDATA["VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"]]></remarks>
		public static readonly Regex ProductIdRegex = new Regex(@"PID[^0-9a-fA-F](?<productId>[0-9a-fA-F]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary></summary>
		public const string DefaultSerial = "";

		/// <summary></summary>
		public const string DefaultSeparator = " - ";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string path;

		private int vendorId  = DefaultVendorId;
		private int productId = DefaultProductId;

		private string manufacturer;
		private string product;
		private string serial = DefaultSerial; // Required for XML serialization.

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DeviceInfo()
		{
			Initialize("");
		}

		/// <summary></summary>
		public DeviceInfo(string path)
		{
			int vendorId, productId;
			string manufacturer, product, serial;
			if (Device.GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial))
				Initialize(path, vendorId, productId, manufacturer, product, serial);
			else
				Initialize(path); // Initialize this info based on the available information only.
		}

		/// <summary></summary>
		public DeviceInfo(int vendorId, int productId)
		{
			string path, manufacturer, product, serial;
			if (Device.GetDeviceInfoFromVidAndPid(vendorId, productId, out path, out manufacturer, out product, out serial))
				Initialize(path, vendorId, productId, manufacturer, product, serial);
			else
				Initialize(vendorId, productId); // Initialize this info based on the available information only.
		}

		/// <summary></summary>
		public DeviceInfo(int vendorId, int productId, string serial)
		{
			string path, manufacturer, product;
			if (Device.GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serial, out path, out manufacturer, out product))
				Initialize(path, vendorId, productId, manufacturer, product, serial);
			else
				Initialize(vendorId, productId, serial); // Initialize this info based on the available information only.
		}

		/// <summary></summary>
		public DeviceInfo(string path, int vendorId, int productId, string manufacturer, string product, string serial)
		{
			Initialize(path, vendorId, productId, manufacturer, product, serial);
		}

		private void Initialize(string path)
		{
			Initialize(path, DefaultVendorId, DefaultProductId, "", "", "");
		}

		private void Initialize(int vendorId, int productId)
		{
			Initialize(vendorId, productId, "");
		}

		private void Initialize(int vendorId, int productId, string serial)
		{
			Initialize("", vendorId, productId, "", "", serial);
		}

		private void Initialize(string path, int vendorId, int productId, string manufacturer, string product, string serial)
		{
			if ((vendorId  < FirstVendorId)  || (vendorId  > LastVendorId))
				throw (new ArgumentOutOfRangeException("vendorId",  vendorId,  "Invalid vendor ID."));
			if ((productId < FirstProductId) || (productId > LastProductId))
				throw (new ArgumentOutOfRangeException("productId", productId, "Invalid product ID."));

			this.path = path;

			this.vendorId  = vendorId;
			this.productId = productId;

			this.manufacturer = manufacturer;
			this.product      = product;
			this.serial       = serial;
		}

		/// <summary></summary>
		public DeviceInfo(DeviceInfo rhs)
		{
			this.path = rhs.path;

			this.vendorId  = rhs.vendorId;
			this.productId = rhs.productId;

			this.manufacturer = rhs.manufacturer;
			this.product      = rhs.product;
			this.serial       = rhs.serial;
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
			get { return (VendorId.ToString("X4", CultureInfo.InvariantCulture)); }
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
			get { return (ProductId.ToString("X4", CultureInfo.InvariantCulture)); }
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

		/// <remarks>
		/// Keeping "SerialNumber" for the XML element for backward-compatibility to older versions.
		/// Note that the USB standard also uses the term "SerialNumber" which not really makes
		/// sense as the value can be any unicode (UTF-8 ??) encoded string.
		/// </remarks>
		[XmlElement("SerialNumber")]
		public virtual string Serial
		{
			get { return (this.serial); }
			set { this.serial = value;  }
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
			if (this.path.Length > 0)
			{
				return (Device.GetDeviceInfoFromPath(this.path, out this.vendorId, out this.productId, out this.manufacturer, out this.product, out this.serial));
			}
			else if ((this.vendorId != 0) && (this.productId != 0))
			{
				if (this.serial.Length > 0)
					return (Device.GetDeviceInfoFromVidAndPidAndSerial(this.vendorId, this.productId, this.serial, out this.path, out this.manufacturer, out this.product));
				else
					return (Device.GetDeviceInfoFromVidAndPid(this.vendorId, this.productId, out this.path, out this.manufacturer, out this.product, out this.serial));
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
			return (Equals(obj as DeviceInfo));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(DeviceInfo other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			// Do not care about path, the path is likely to be system dependent.

			return
			(
				(VendorId     == other.VendorId) &&
				(ProductId    == other.ProductId) &&
				(Serial == other.Serial)
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
			if (this.path != null)
				return (this.path.GetHashCode());
			else
				return (base.GetHashCode());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (ToString(true, true));
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToString(bool appendIds, bool appendInUseText)
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Manufacturer))
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

			if (!string.IsNullOrEmpty(Product))
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

			if (!string.IsNullOrEmpty(Serial))
			{
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) Product (PID:1234) "

				sb.Append(Serial);               // "Company (VID:0ABC) Product (PID:1234) 000123A"
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToShortString()
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Product))
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
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToLongString()
		{
			StringBuilder sb = new StringBuilder(ToString());
			if (!string.IsNullOrEmpty(Path))
			{
				sb.Append(" at ");
				sb.Append(Path);
			}

			return (sb.ToString());
		}

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <summary>
		/// Parses <paramref name="s"/> for VID / PID and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static DeviceInfo Parse(string s)
		{
			DeviceInfo result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify a valid USB device ID."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for VID / PID and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out DeviceInfo result)
		{
			s = s.Trim();

			// e.g. "VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"
			Match m = VendorIdRegex.Match(s);
			if (m.Success)
			{
				int vendorId;
				if (int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out vendorId))
				{
					m = ProductIdRegex.Match(s);
					if (m.Success)
					{
						int productId;
						if (int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out productId))
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

		#endregion

		#endregion

		#region IComparable Members

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			DeviceInfo other = obj as DeviceInfo;
			if (other != null)
			{
				if      (VendorId != other.VendorId)
					return (VendorId.CompareTo(other.VendorId));
				else if (ProductId != other.ProductId)
					return (ProductId.CompareTo(other.ProductId));
				else
					return (StringEx.CompareOrdinalIgnoreCase(Serial, other.Serial)); // Case-insensitive (i.e. Windows behaviour).
			}
			else
			{
				throw (new ArgumentException(obj.ToString() + " is not a 'UsbDeviceId'!"));
			}
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			DeviceInfo casted = objA as DeviceInfo;
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
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
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (!(lhs == rhs));
		}

		/// <summary></summary>
		public static bool operator <(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (Compare(lhs, rhs) >= 0);
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
