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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using MKY.Contracts;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// This container class holds information about a USB HID device.
	/// </summary>
	/// <remarks>
	/// The information contained is limited to pieces that are needed to lookup a device.
	/// It does not contain the complete set of information given by the HID capabilities.
	/// </remarks>
	/// <remarks>
	/// \remind (2019-11-10 / MKY)
	/// Instances of this container class shall be treated as immutable objects. However, it is not
	/// possible to assign <see cref="ImmutableObjectAttribute"/>/<see cref="ImmutableContractAttribute"/>
	/// because XML default serialization requires public setters. Split into mutable settings tuple
	/// and immutable runtime container should be done.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[Serializable]
	public class HidDeviceInfo : DeviceInfo, IEquatable<HidDeviceInfo>, IComparable
	{
		#region Public Constants
		//==========================================================================================
		// Public Constants
		//==========================================================================================

		/// <summary></summary>
		public const int FirstUsagePage   = 0x0000;

		/// <summary></summary>
		public const int LastUsagePage    = 0xFFFF;

		/// <summary></summary>
		public const int FirstUsageId     = 0x0000;

		/// <summary></summary>
		public const int LastUsageId      = 0xFFFF;

		/// <summary></summary>
		public static readonly string FirstUsagePageString = FirstUsagePage.ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string LastUsagePageString  = LastUsagePage .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string FirstUsageIdString   = FirstUsageId  .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string LastUsageIdString    = LastUsageId   .ToString("X4", CultureInfo.InvariantCulture);

		/// <summary>Special value to indicate that any usage page shall be accepted.</summary>
		public const int AnyUsagePage = -1;

		/// <summary>Special value to indicate that any usage ID shall be accepted.</summary>
		public const int AnyUsageId   = -1;

		/// <summary></summary>
		public static readonly string AnyUsagePageString = AnyUsagePage.ToString(CultureInfo.InvariantCulture);

		/// <summary></summary>
		public static readonly string AnyUsageIdString   = AnyUsageId  .ToString(CultureInfo.InvariantCulture);

	////private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
	////
	/////// <remarks><![CDATA["USAGE:00FF/0001"]]></remarks>
	/////// <remarks><![CDATA["usage_00FF_0001"]]></remarks>
	////public static readonly Regex UsageRegex = new Regex(@"USAGE[^0-9a-fA-F](?<usagePage>[0-9a-fA-F]+)[^0-9a-fA-F](?<usageId>[0-9a-fA-F]+)", Options);
	////
	//// See explanation at TryParseConsiderately() further below.

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int usagePage = AnyUsagePage;
		private int usageId   = AnyUsageId;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <remarks>
		/// \remind (2019-11-10 / MKY)
		/// Parameter-less constructor is required for XML default serialization. Could be removed
		/// after having split into mutable settings tuple and immutable runtime container.
		/// </remarks>
		public HidDeviceInfo()
		{
			Initialize(); // Initialize this info based on defaults only.
		}

		/// <summary></summary>
		public HidDeviceInfo(string path)
		{
			int vendorId, productId;
			string manufacturer, product, serial;
			if (HidDevice.GetDeviceInfoFromPath(path, out vendorId, out productId, out manufacturer, out product, out serial, out usagePage, out usageId))
				Initialize(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId);
			else
				Initialize(path); // Initialize this info based on the available information only.
		}

		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceInfo(int vendorId, int productId, int usagePage = AnyUsagePage, int usageId = AnyUsageId)
		{
			string path, manufacturer, product, serial;
			if (HidDevice.GetDeviceInfoFromVidPidUsage(vendorId, productId, usagePage, usageId, out path, out manufacturer, out product, out serial))
				Initialize(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId);
			else
				Initialize(vendorId, productId, usagePage, usageId); // Initialize this info based on the available information only.
		}

		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceInfo(int vendorId, int productId, string serial, int usagePage = AnyUsagePage, int usageId = AnyUsageId)
		{
			string path, manufacturer, product;
			if (HidDevice.GetDeviceInfoFromVidPidSerialUsage(vendorId, productId, serial, usagePage, usageId, out path, out manufacturer, out product))
				Initialize(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId);
			else
				Initialize(vendorId, productId, serial, usagePage, usageId); // Initialize this info based on the available information only.
		}

		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceInfo(string path, int vendorId, int productId, int usagePage = AnyUsagePage, int usageId = AnyUsageId)
		{
			Initialize(path, vendorId, productId, "", "", "", usagePage, usageId);
		}

		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceInfo(string path, int vendorId, int productId, string manufacturer, string product, string serial, int usagePage = AnyUsagePage, int usageId = AnyUsageId)
		{
			Initialize(path, vendorId, productId, manufacturer, product, serial, usagePage, usageId);
		}

		/// <remarks>Initialize this info based on defaults only.</remarks>
		protected override void Initialize()
		{
			base.Initialize();

			this.usagePage = AnyUsagePage;
			this.usageId   = AnyUsageId;
		}

		/// <remarks>Initialize this info based on the available information only.</remarks>
		protected override void Initialize(string path)
		{
			base.Initialize(path);

			this.usagePage = AnyUsagePage;
			this.usageId   = AnyUsageId;
		}

		/// <remarks>Initialize this info based on the available information only.</remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		private void Initialize(int vendorId, int productId, int usagePage, int usageId)
		{
			if (!IsValidVendorId(usagePage))
				throw (new ArgumentOutOfRangeException("usagePage", usagePage, "'" + usagePage + "' is an invalid usage page!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			if (!IsValidProductId(usageId))
				throw (new ArgumentOutOfRangeException("usageId", usageId, "'" + usageId + "' is an invalid usage ID!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			Initialize(vendorId, productId);

			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <remarks>Initialize this info based on the available information only.</remarks>
		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		private void Initialize(int vendorId, int productId, string serial, int usagePage, int usageId)
		{
			if (!IsValidVendorId(usagePage))
				throw (new ArgumentOutOfRangeException("usagePage", usagePage, "'" + usagePage + "' is an invalid usage page!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			if (!IsValidProductId(usageId))
				throw (new ArgumentOutOfRangeException("usageId", usageId, "'" + usageId + "' is an invalid usage ID!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			Initialize(vendorId, productId, serial);

			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <exception cref="ArgumentOutOfRangeException"> if a value is invalid.</exception>
		private void Initialize(string path, int vendorId, int productId, string manufacturer, string product, string serial, int usagePage, int usageId)
		{
			if (!IsValidVendorId(usagePage))
				throw (new ArgumentOutOfRangeException("usagePage", usagePage, "'" + usagePage + "' is an invalid usage page!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			if (!IsValidProductId(usageId))
				throw (new ArgumentOutOfRangeException("usageId", usageId, "'" + usageId + "' is an invalid usage ID!")); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.

			Initialize(path, vendorId, productId, manufacturer, product, serial);

			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		/// <summary></summary>
		public HidDeviceInfo(HidDeviceInfo rhs)
			: base((DeviceInfo)rhs)
		{
			this.usagePage = rhs.usagePage;
			this.usageId   = rhs.usageId;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public HidDeviceInfo(DeviceInfo rhs, int usagePage = AnyUsagePage, int usageId = AnyUsageId)
			: base(rhs)
		{
			this.usagePage = usagePage;
			this.usageId   = usageId;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// Term "Usage page" is given by https://www.usb.org/sites/default/files/documents/hut1_12v2.pdf section 3.1 [HID Usage Table Conventions].
		/// </remarks>
		[XmlElement("UsagePage")]
		public virtual int UsagePage
		{
			get { return (this.usagePage); }
			set { this.usagePage = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string UsagePageString
		{
			get { return (UsagePage.ToString("X4", CultureInfo.InvariantCulture)); }
		}

		/// <remarks>
		/// Term "Usage ID" is given by https://www.usb.org/sites/default/files/documents/hut1_12v2.pdf section 3.1 [HID Usage Table Conventions].
		/// </remarks>
		[XmlElement("UsageId")]
		public virtual int UsageId
		{
			get { return (this.usageId); }
			set { this.usageId = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string UsageIdString
		{
			get { return (UsageId.ToString("X4", CultureInfo.InvariantCulture)); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Returns whether the given number is a valid usage page.
		/// </summary>
		public static bool IsValidUsagePage(int usagePage)
		{
			return (Int32Ex.IsWithin(usagePage, FirstUsagePage, LastUsagePage));
		}

		/// <summary>
		/// Returns whether the given number is a valid usage page; or <see cref="AnyUsagePage"/>.
		/// </summary>
		public static bool IsValidUsagePageOrAny(int usagePage)
		{
			if (usagePage == AnyUsagePage)
				return (true);
			else
				return (IsValidUsagePage(usagePage));
		}

		/// <summary>
		/// Returns whether the given number is a valid usage ID.
		/// </summary>
		public static bool IsValidUsageId(int usageId)
		{
			return (Int32Ex.IsWithin(usageId, FirstUsageId, LastUsageId));
		}

		/// <summary>
		/// Returns whether the given number is a valid usage ID; or <see cref="AnyUsageId"/>.
		/// </summary>
		public static bool IsValidUsageIdOrAny(int usageId)
		{
			if (usageId == AnyUsageId)
				return (true);
			else
				return (IsValidUsageId(usageId));
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
			return (ToString(true, false)); // Do not append usage by default, irrelevant for most uses.
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
		public override string ToString(bool insertVidPid)
		{
			return (ToString(true, false)); // Do not append usage by default, irrelevant for most uses.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToString(bool insertVidPid, bool appendUsage)
		{
			if (!appendUsage)
			{
				return (base.ToString(insertVidPid));
			}
			else // appendUsage
			{
				var sb = new StringBuilder(base.ToString(insertVidPid));
				                               //// "Company (VID:0ABC) Product (PID:1234) 000123A"
				if (sb.Length > 0)
					sb.Append(" ");              // "Company (VID:0ABC) Product (PID:1234) 000123A "

				sb.Append("(USAGE:");
				sb.Append(UsagePageString);      // "Company (VID:0ABC) Product (PID:1234) 000123A (USAGE:FF00"
				sb.Append("/");
				sb.Append(UsageIdString);        // "Company (VID:0ABC) Product (PID:1234) 000123A (USAGE:FF00/0001)"
				sb.Append(")");

				return (sb.ToString());
			}
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToShortString()
		{
			return (ToShortString(false)); // Do not append usage by default, irrelevant for most uses.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToShortString(bool appendUsage)
		{
			if (!appendUsage)
			{
				return (base.ToShortString());
			}
			else // appendUsage
			{
				var sb = new StringBuilder(base.ToShortString());
				                                   //// "VID:1234 PID:1234"
				if (sb.Length > 0)
					sb.Append(" ");                  // "VID:1234 PID:1234 "

				sb.Append("USAGE:");
				sb.Append(UsagePageString);          // "VID:1234 PID:1234 USAGE:FF00"
				sb.Append("/");
				sb.Append(UsageIdString);            // "VID:1234 PID:1234 USAGE:FF00/0001"

				return (sb.ToString());
			}
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
				int hashCode = base.GetHashCode();

				hashCode = (hashCode * 397) ^ UsagePage;
				hashCode = (hashCode * 397) ^ UsageId;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality,
		/// ignoring <see cref="DeviceInfo.Manufacturer"/> and <see cref="DeviceInfo.Product"/>.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as HidDeviceInfo));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality,
		/// ignoring <see cref="DeviceInfo.Manufacturer"/> and <see cref="DeviceInfo.Product"/>.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(HidDeviceInfo other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (Equals(other.VendorId, other.ProductId, other.Serial, other.UsagePage, other.UsageId));

			// Do not care about path, the path is likely system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified properties have value equality,
		/// ignoring <see cref="DeviceInfo.Manufacturer"/> and <see cref="DeviceInfo.Product"/>.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool Equals(int vendorId, int productId, string serial, int usagePage, int usageId)
		{
			return (EqualsVidPidSerialUsage(vendorId, productId, serial, usagePage, usageId));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality
		/// in respect to <see cref="UsagePage"/> and <see cref="UsageId"/>; taking the special
		/// values <see cref="AnyUsagePage"/> and <see cref="AnyUsageId"/> into account.
		/// </summary>
		public virtual bool EqualsUsageOrAny(HidDeviceInfo other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (EqualsUsageOrAny(other.UsagePage, other.UsageId));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality
		/// in respect to <see cref="UsagePage"/> and <see cref="UsageId"/>, taking the special
		/// values <see cref="AnyUsagePage"/> and <see cref="AnyUsageId"/> into account.
		/// </summary>
		public virtual bool EqualsUsageOrAny(int usagePage, int usageId)
		{
			bool equals = true;

			if ((UsagePage != AnyUsagePage) && (usagePage != AnyUsagePage))
				equals = (equals || UsagePage.Equals(usagePage));

			if ((UsageId != AnyUsageId) && (usageId != AnyUsageId))
				equals = (equals || UsageId.Equals(usageId));

			return (equals);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality,
		/// ignoring <see cref="DeviceInfo.Serial"/>, <see cref="DeviceInfo.Manufacturer"/>
		/// and <see cref="DeviceInfo.Product"/>.
		/// </summary>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only
		/// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
		/// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
		/// </remarks>
		public virtual bool EqualsVidPidUsage(HidDeviceInfo other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (EqualsVidPidUsage(other.VendorId, other.ProductId, other.UsagePage, other.UsageId));

			// Do not care about path, the path is likely system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified properties have value equality,
		/// ignoring <see cref="DeviceInfo.Serial"/>, <see cref="DeviceInfo.Manufacturer"/>
		/// and <see cref="DeviceInfo.Product"/>.
		/// </summary>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only
		/// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
		/// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
		/// </remarks>
		public virtual bool EqualsVidPidUsage(int vendorId, int productId, int usagePage, int usageId)
		{
			return
			(
				EqualsVidPid(vendorId, productId) &&
				EqualsUsageOrAny(usagePage, usageId)
			);

			// Do not care about path, the path is likely system dependent.
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
		/// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
		/// </remarks>
		public virtual bool EqualsVidPidSerialUsage(HidDeviceInfo other)
		{
			return (EqualsVidPidSerialUsage(other.VendorId, other.ProductId, other.Serial, other.UsagePage, other.UsageId));
		}

		/// <summary>
		/// Determines whether this instance and the specified properties have value equality.
		/// </summary>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
		/// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
		/// </remarks>
		public virtual bool EqualsVidPidSerialUsage(int vendorId, int productId, string serial, int usagePage, int usageId)
		{
			return
			(
				EqualsVidPidSerial(vendorId, productId, serial) &&
				EqualsUsageOrAny(usagePage, usageId)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(HidDeviceInfo lhs, HidDeviceInfo rhs)
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
		public static bool operator !=(HidDeviceInfo lhs, HidDeviceInfo rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

	/////// <summary>
	/////// Parses <paramref name="s"/> for VID/PID/SNR/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	////public static HidDeviceInfo Parse(string s)
	////{
	////	HidDeviceInfo result;
	////	if (TryParse(s, out result))
	////		return (result);
	////	else
	////		throw (new FormatException(@"""" + s + @""" does not specify a valid USB HID device ID."));
	////}
	////
	//// See explanation at TryParseConsiderately() further below.

		/// <summary>
		/// Parses <paramref name="s"/> for VID/PID and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>
		/// and <see cref="DeviceInfo.ProductId"/> are considered.
		/// </remarks>
		public static new HidDeviceInfo ParseVidPid(string s)
		{
			HidDeviceInfo result;
			if (TryParseVidPid(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify a valid USB HID device ID."));
		}

		/// <summary>
		/// Parses <paramref name="s"/> for VID/PID/SNR and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/> and <see cref="DeviceInfo.Serial"/> are considered.
		/// </remarks>
		public static new HidDeviceInfo ParseVidPidSerial(string s)
		{
			HidDeviceInfo result;
			if (TryParseVidPidSerial(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify a valid USB HID device ID."));
		}

	/////// <summary>
	/////// Parses <paramref name="s"/> for VID/PID/SNR/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	/////// <remarks>
	/////// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
	/////// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
	/////// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
	/////// </remarks>
	////public static HidDeviceInfo ParseVidPidSerialUsage(string s)
	////{
	////	HidDeviceInfo result;
	////	if (TryParseVidPidSerialUsage(s, out result))
	////		return (result);
	////	else
	////		throw (new FormatException(@"""" + s + @""" does not specify a valid USB HID device ID."));
	////}
	////
	//// See explanation at TryParseConsiderately() further below.
	////
	/////// <summary>
	/////// Parses <paramref name="s"/> for VID/PID/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	/////// <remarks>
	/////// Comprehensibility method, i.e. making obvious that only
	/////// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
	/////// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
	/////// </remarks>
	////public static HidDeviceInfo ParseVidPidUsage(string s)
	////{
	////	HidDeviceInfo result;
	////	if (TryParseVidPidUsage(s, out result))
	////		return (result);
	////	else
	////		throw (new FormatException(@"""" + s + @""" does not specify a valid USB HID device ID."));
	////}

	/////// <summary>
	/////// Tries to parse <paramref name="s"/> for VID/PID/SNR/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	////public static bool TryParse(string s, out HidDeviceInfo result)
	////{
	////	return (TryParseConsiderately(s, true, true, out result));
	////}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for VID/PID and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>
		/// and <see cref="DeviceInfo.ProductId"/> are considered.
		/// </remarks>
		public static bool TryParseVidPid(string s, out HidDeviceInfo result)
		{
		////return (TryParseConsiderately(s, false, false, out result)); See explanation at TryParseConsiderately() further below.

			DeviceInfo di;
			if (TryParseConsiderately(s, false, out di))
			{
				result = new HidDeviceInfo(di);
				return (true);
			}

			result = null;
			return (false);
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for VID/PID/SNR and returns a corresponding device ID object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		/// <remarks>
		/// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
		/// <see cref="DeviceInfo.ProductId"/> and <see cref="DeviceInfo.Serial"/> are considered.
		/// </remarks>
		public static bool TryParseVidPidSerial(string s, out HidDeviceInfo result)
		{
		////return (TryParseConsiderately(s, true, false, out result)); See explanation at TryParseConsiderately() further below.

			DeviceInfo di;
			if (TryParseConsiderately(s, true, out di))
			{
				result = new HidDeviceInfo(di);
				return (true);
			}

			result = null;
			return (false);
		}

	/////// <summary>
	/////// Tries to parse <paramref name="s"/> for VID/PID/SNR/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	/////// <remarks>
	/////// Comprehensibility method, i.e. making obvious that only <see cref="DeviceInfo.VendorId"/>,
	/////// <see cref="DeviceInfo.ProductId"/>, <see cref="DeviceInfo.Serial"/>, and
	/////// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
	/////// </remarks>
	////public static bool TryParseVidPidSerialUsage(string s, out HidDeviceInfo result)
	////{
	////	return (TryParseConsiderately(s, true, true, out result));
	////}
	////
	//// See explanation at TryParseConsiderately() further below.
	////
	/////// <summary>
	/////// Tries to parse <paramref name="s"/> for VID/PID/USAGE and returns a corresponding device ID object.
	/////// </summary>
	/////// <remarks>
	/////// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
	/////// </remarks>
	/////// <remarks>
	/////// Comprehensibility method, i.e. making obvious that only
	/////// <see cref="DeviceInfo.VendorId"/>, <see cref="DeviceInfo.ProductId"/> and
	/////// <see cref="UsagePage"/>, <see cref="UsageId"/> are considered.
	/////// </remarks>
	////public static bool TryParseVidPidUsage(string s, out HidDeviceInfo result)
	////{
	////	return (TryParseConsiderately(s, false, true, out result));
	////}
	////
	//// Parsing usage not implemented (yet), because SNR may be any string, thus usage would have to be located ahead...
	////
	/////// <summary></summary>
	////protected static bool TryParseConsiderately(string s, bool requireSerial, bool requireUsage, out HidDeviceInfo result)
	////{
	////	DeviceInfo di;
	////	if (TryParseConsiderately(s, requireSerial, out di))
	////	{
	////		// e.g. "VID:0ABC PID:1234 SNR:XYZ USAGE:00FF/0001" or "vid_0ABC&pid_1234&snr_xyz&usage_00FF_0001"
	////		var m = UsageRegex.Match(s);
	////		if (m.Success)
	////		{
	////			int usagePage;       // m.Value is e.g. "USAGE:00FF/0001" thus [1] is "00FF".
	////			if (int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usagePage))
	////			{
	////				int usageId;     // m.Value is e.g. "USAGE:00FF/0001" thus [2] is "0001".
	////				if (int.TryParse(m.Groups[2].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out usageId))
	////				{
	////					result = new HidDeviceInfo(di, usagePage, usageId);
	////					return (true);
	////				}
	////			}
	////		}
	////
	////		if (!requireUsage)
	////		{
	////			result = new HidDeviceInfo(di);
	////			return (true);
	////		}
	////	}
	////
	////	result = null;
	////	return (false);
	////}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary></summary>
		public override int CompareTo(object obj)
		{
			var other = (obj as HidDeviceInfo);
			if (other != null)
			{
				if (UsagePage != other.UsagePage)
					return (UsagePage.CompareTo(other.UsagePage));
				else
					return (UsageId.CompareTo(other.UsageId));
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify an '" + typeof(HidDeviceInfo).Name + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static new int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as HidDeviceInfo);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
		}

		/// <summary></summary>
		public static bool operator <(HidDeviceInfo lhs, HidDeviceInfo rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(HidDeviceInfo lhs, HidDeviceInfo rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(HidDeviceInfo lhs, HidDeviceInfo rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(HidDeviceInfo lhs, HidDeviceInfo rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator string(HidDeviceInfo deviceInfo)
		{
			return (deviceInfo.ToString());
		}

		/// <summary></summary>
		public static implicit operator HidDeviceInfo(string s)
		{
			HidDeviceInfo result;
			if (TryParseVidPidSerial(s, out result))
				return (result);
			else
				return (ParseVidPid(s));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
