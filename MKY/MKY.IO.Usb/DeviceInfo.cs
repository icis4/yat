﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2017 Matthias Kläy.
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

		private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		/// <remarks><![CDATA["VID:0ABC / PID:1234"]]></remarks>
		/// <remarks><![CDATA["VID:0ABC / PID:1234 / SNR:XYZ"]]></remarks>
		/// <remarks><![CDATA["vid_0ABC & pid_1234"]]></remarks>
		/// <remarks><![CDATA["vid_0ABC & pid_1234 & snr_xyz"]]></remarks>
		/// <remarks><![CDATA["Company (VID:0ABC) Product (PID:1234)"]]></remarks>
		/// <remarks><![CDATA["Company (VID:0ABC) Product (PID:1234) XYZ"]]></remarks>
		public static readonly Regex VendorIdRegex = new Regex(@"VID[^0-9a-fA-F](?<vendorId>[0-9a-fA-F]+)", Options);

		/// <remarks><![CDATA["VID:0ABC / PID:1234"]]></remarks>
		/// <remarks><![CDATA["VID:0ABC / PID:1234 / SNR:XYZ"]]></remarks>
		/// <remarks><![CDATA["vid_0ABC & pid_1234"]]></remarks>
		/// <remarks><![CDATA["vid_0ABC & pid_1234 & snr_xyz"]]></remarks>
		/// <remarks><![CDATA["Company (VID:0ABC) Product (PID:1234)"]]></remarks>
		/// <remarks><![CDATA["Company (VID:0ABC) Product (PID:1234) XYZ"]]></remarks>
		public static readonly Regex ProductIdRegex = new Regex(@"PID[^0-9a-fA-F](?<productId>[0-9a-fA-F]+)", Options);

		/// <remarks><![CDATA["VID:0ABC / PID:1234 / SNR:XYZ"]]></remarks>
		/// <remarks><![CDATA["vid_0ABC & pid_1234 & snr_xyz"]]></remarks>
		public static readonly Regex SerialRegexTag = new Regex(@"SNR[^0-9a-fA-F](?<serial>.+)", Options); // SNR may be any character.

		/// <remarks><![CDATA["Company (VID:0ABC) Product (PID:1234) XYZ"]]></remarks>
		public static readonly Regex SerialRegexRemainder = new Regex(@"PID[^0-9a-fA-F][0-9a-fA-F]+.\s?(?<serial>.+)", Options); // Everything following the PID pattern.

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

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		public DeviceInfo(int vendorId, int productId)
		{
			string path, manufacturer, product, serial;
			if (Device.GetDeviceInfoFromVidAndPid(vendorId, productId, out path, out manufacturer, out product, out serial))
				Initialize(path, vendorId, productId, manufacturer, product, serial);
			else
				Initialize(vendorId, productId); // Initialize this info based on the available information only.
		}

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		public DeviceInfo(int vendorId, int productId, string serial)
		{
			string path, manufacturer, product;
			if (Device.GetDeviceInfoFromVidAndPidAndSerial(vendorId, productId, serial, out path, out manufacturer, out product))
				Initialize(path, vendorId, productId, manufacturer, product, serial);
			else
				Initialize(vendorId, productId, serial); // Initialize this info based on the available information only.
		}

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		public DeviceInfo(string path, int vendorId, int productId)
		{
			Initialize(path, vendorId, productId, "", "", "");
		}

		/// <summary></summary>
		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		public DeviceInfo(string path, int vendorId, int productId, string manufacturer, string product, string serial)
		{
			Initialize(path, vendorId, productId, manufacturer, product, serial);
		}

		private void Initialize(string path)
		{
			Initialize(path, DefaultVendorId, DefaultProductId, "", "", "");
		}

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		private void Initialize(int vendorId, int productId)
		{
			Initialize(vendorId, productId, "");
		}

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		private void Initialize(int vendorId, int productId, string serial)
		{
			Initialize("", vendorId, productId, "", "", serial);
		}

		/// <exception cref="ArgumentOutOfRangeException"> if either vendor ID or product ID is an invalid value.</exception>
		private void Initialize(string path, int vendorId, int productId, string manufacturer, string product, string serial)
		{
			if (!IsValidVendorId(vendorId))
				throw (new ArgumentOutOfRangeException("vendorId", vendorId, "'" + vendorId + "' is an invalid vendor ID!")); // Do not append 'MessageHelper.SubmitBug' as caller could rely on this exception text.

			if (!IsValidProductId(productId))
				throw (new ArgumentOutOfRangeException("productId", productId, "'" + productId + "' is an invalid product ID!")); // Do not append 'MessageHelper.SubmitBug' as caller could rely on this exception text.

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
		/// The USB standard uses the term "SerialNumber" which not really makes sense as the value
		/// can contain characters as well.
		/// </remarks>
		/// <remarks>
		/// The USB standard does not seem to specify any restriction on the serial number string.
		/// Thus, it may be any UTF-16 encoded string.
		/// </remarks>
		/// <remarks>
		/// Microsoft further restricts the serial string as documented in <a href="https://msdn.microsoft.com/en-us/library/windows/hardware/dn423379%28v=vs.85%29.aspx#usbsn"/>:
		/// "Plug and Play requires that every byte in a USB serial number be valid. If a single
		///  byte is invalid, Windows discards the serial number and treats the device as if it
		///  had no serial number. The following byte values are invalid for USB serial numbers:
		///  - Values less than 0x20.
		///  - Values greater than 0x7F.
		///  - 0x2C (comma)."
		/// Note that 0x7F (delete) doesn't make much sense either...
		/// </remarks>
		/// <remarks>
		/// Keeping "SerialNumber" for the XML element for backward-compatibility to older versions.
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
		/// Returns whether the given number is a valid vendor ID.
		/// </summary>
		public static bool IsValidVendorId(int vendorId)
		{
			return (Int32Ex.IsWithin(vendorId, FirstVendorId, LastVendorId));
		}

		/// <summary>
		/// Returns whether the given number is a valid product ID.
		/// </summary>
		public static bool IsValidProductId(int productId)
		{
			return (Int32Ex.IsWithin(productId, FirstProductId, LastProductId));
		}

		/// <summary>
		/// Queries the USB device for user readable strings like vendor or product name.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual bool TryValidate()
		{
			if (!string.IsNullOrEmpty(this.path))
			{
				return (Device.GetDeviceInfoFromPath(this.path, out this.vendorId, out this.productId, out this.manufacturer, out this.product, out this.serial));
			}
			else if ((this.vendorId != 0) && (this.productId != 0))
			{
				if (!string.IsNullOrEmpty(this.serial))
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
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToString(true, true));
		}

		#region Object Members > ToString Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > ToString Extensions
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
			var sb = new StringBuilder();

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
			var sb = new StringBuilder();

			sb.Append("VID:");
			sb.Append(ProductIdString);          // "VID:1234"

			sb.Append(" PID:");
			sb.Append(ProductIdString);          // "VID:1234 PID:1234"

			if (!string.IsNullOrEmpty(Serial))
			{
				sb.Append(" SNR:");
				sb.Append(Serial);               // "VID:1234 PID:1234 SNR:000123A"
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
			var sb = new StringBuilder(ToString());
			if (!string.IsNullOrEmpty(Path))
			{
				sb.Append(" at ");
				sb.Append(Path);
			}

			return (sb.ToString());
		}

		#endregion

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
				if (this.path != null)
					return (this.path.GetHashCode());
				else
					return (base.GetHashCode());
			}
		}

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
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (Equals(other.VendorId, other.ProductId, other.Serial));

			// Do not care about path, the path is likely to be system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified properties have value equality.
		/// Note that the serial string is compared case-insensitive, same behavior as Windows.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool Equals(int vendorId, int productId, string serial)
		{
			return
			(
				VendorId .Equals(vendorId)  &&
				ProductId.Equals(productId) &&
				StringEx.EqualsOrdinalIgnoreCase(Serial, serial) // Case-insensitive, same behavior as Windows.
			);

			// Do not care about path, the path is likely to be system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality,
		/// ignoring <see cref="Serial"/>.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		public virtual bool EqualsVidPid(DeviceInfo other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (EqualsVidPid(other.VendorId, other.ProductId));

			// Do not care about path, the path is likely to be system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified properties have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool EqualsVidPid(int vendorId, int productId)
		{
			return
			(
				VendorId .Equals(vendorId) &&
				ProductId.Equals(productId)
			);

			// Do not care about path, the path is likely to be system dependent.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(DeviceInfo lhs, DeviceInfo rhs)
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
		public static bool operator !=(DeviceInfo lhs, DeviceInfo rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		public static DeviceInfo ParseFromVidAndPid(string s)
		{
			DeviceInfo result;
			if (TryParseFromVidAndPid(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify a valid USB device ID."));
		}

		/// <summary>
		/// Parses <paramref name="s"/> for VID / PID / SNR and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		public static DeviceInfo ParseFromVidAndPidAndSerial(string s)
		{
			DeviceInfo result;
			if (TryParseFromVidAndPidAndSerial(s, out result))
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		public static bool TryParseFromVidAndPid(string s, out DeviceInfo result)
		{
			return (TryParse(s, false, out result));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for VID / PID / SNR and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pid", Justification = "'PID' is a common term in USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vid", Justification = "'VID' is a common term in USB.")]
		public static bool TryParseFromVidAndPidAndSerial(string s, out DeviceInfo result)
		{
			return (TryParse(s, true, out result));
		}

		private static bool TryParse(string s, bool expectSerial, out DeviceInfo result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = null;
				return (false);
			}

			// e.g. "VID:0ABC / PID:1234 / SNR:XYZ" or "vid_0ABC & pid_1234 & snr_xyz"
			var m = VendorIdRegex.Match(s);
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
							if (expectSerial)
							{
								m = SerialRegexTag.Match(s); // Try to match against "SNR" tag.
								if (m.Success)
								{
									string serial = m.Groups[1].Value;
									result = new DeviceInfo(vendorId, productId, serial);
									return (true);
								}
								m = SerialRegexRemainder.Match(s); // Try to match against remainder string.
								if (m.Success)
								{
									string serial = m.Groups[1].Value;
									result = new DeviceInfo(vendorId, productId, serial);
									return (true);
								}
							}
							else
							{
								result = new DeviceInfo(vendorId, productId);
								return (true);
							}
						}
					}
				}
			}

			result = null;
			return (false);
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			var other = (obj as DeviceInfo);
			if (other != null)
			{
				if      (VendorId != other.VendorId)
					return (VendorId.CompareTo(other.VendorId));
				else if (ProductId != other.ProductId)
					return (ProductId.CompareTo(other.ProductId));
				else
					return (StringEx.CompareOrdinalIgnoreCase(Serial, other.Serial)); // Case-insensitive, same behavior as Windows.
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify a 'UsbDeviceId'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as DeviceInfo);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
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
		public static implicit operator string(DeviceInfo deviceInfo)
		{
			return (deviceInfo.ToString());
		}

		/// <summary></summary>
		public static implicit operator DeviceInfo(string s)
		{
			DeviceInfo result;
			if (TryParseFromVidAndPidAndSerial(s, out result))
				return (result);
			else
				return (ParseFromVidAndPid(s));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
