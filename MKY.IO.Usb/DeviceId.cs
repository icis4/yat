//==================================================================================================
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
    public class DeviceId : IEquatable<DeviceId>, IComparable
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

		private int _vendorId = DefaultVendorId;
		private int _productId = DefaultProductId;

        private string _manufacturerName = "";
        private string _productName = "";
        private string _serialNumber = "";

        private bool _isInUse = false;
        private string _inUseText = "";

        private string _separator = "";

		#endregion

		#region Static Object Lifetime
		//==========================================================================================
		// Static Object Lifetime
		//==========================================================================================

		/// <summary></summary>
        static DeviceId()
		{
            // "VID:0ABC / PID:1234" or "vid_0ABC & pid_1234"
            VendorIdRegex  = new Regex(@"VID[^0-9a-fA-F](?<vendorId>[0-9a-fA-F]+)",  RegexOptions.IgnoreCase | RegexOptions.Compiled);
            ProductIdRegex = new Regex(@"PID[^0-9a-fA-F](?<productId>[0-9a-fA-F]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary>
		/// Returns default device on system. Default is the first device available.
        /// Returns <c>null</c> if no devices are available.
		/// </summary>
        public static DeviceId GetDefaultDevice(DeviceClass deviceClass)
		{
            DeviceCollection l = new DeviceCollection(deviceClass);
            l.FillWithAvailableDevices();

            if (l.Count > 0)
                return (new DeviceId(l[0]));
            else
                return (null);
        }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DeviceId()
		{
		}

        /// <summary></summary>
        public DeviceId(int vendorId, int productId)
        {
            if ((vendorId  < FirstVendorId)  || (vendorId  > LastVendorId))
                throw (new ArgumentOutOfRangeException("vendorId",  vendorId,  "Invalid vendor ID"));
            if ((productId < FirstProductId) || (productId > LastProductId))
                throw (new ArgumentOutOfRangeException("productId", productId, "Invalid product ID"));

            _vendorId = vendorId;
            _productId = productId;
        }

        /// <summary></summary>
        public DeviceId(int vendorId, int productId, string serialNumber)
        {
            if ((vendorId  < FirstVendorId)  || (vendorId  > LastVendorId))
                throw (new ArgumentOutOfRangeException("vendorId",  vendorId,  "Invalid vendor ID"));
            if ((productId < FirstProductId) || (productId > LastProductId))
                throw (new ArgumentOutOfRangeException("productId", productId, "Invalid product ID"));

            _vendorId = vendorId;
            _productId = productId;
            _serialNumber = serialNumber;
        }

        /// <summary></summary>
        public DeviceId(DeviceId rhs)
		{
            _vendorId = rhs._vendorId;
            _productId = rhs._productId;

            _manufacturerName = rhs._manufacturerName;
            _productName = rhs._productName;
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
        [XmlElement("VendorId")]
        public int VendorId
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
        public string VendorIdString
        {
            get { return (_vendorId.ToString("X4")); }
        }

        /// <summary></summary>
        [XmlElement("ProductId")]
        public int ProductId
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
        public string ProductIdString
        {
            get { return (_productId.ToString("X4")); }
        }

        /// <summary></summary>
        [XmlIgnore]
        public string ManufacturerName
        {
            get { return (_manufacturerName); }
            set { _manufacturerName = value;  }
        }

        /// <summary></summary>
        [XmlIgnore]
        public string ProductName
        {
            get { return (_productName); }
            set { _productName = value;  }
        }

        /// <summary></summary>
        [XmlIgnore]
        public string SerialNumber
        {
            get { return (_serialNumber); }
            set { _serialNumber = value;  }
        }

        /// <summary>
        /// Indicates whether device is currently in use.
        /// </summary>
        [XmlIgnore]
        public bool IsInUse
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
        public string InUseText
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
        public string Separator
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
        public void GetInformationFromDevice()
        {
            // \todo
            //Dictionary<string, string> descriptions = SerialPortSearcher.GetDescriptionsFromSystem();

            /*if (descriptions.ContainsKey(_name))
            {
                Description = descriptions[_name];
                _hasDescriptonFromSystem = true;
            }*/
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
            if (obj is DeviceId)
                return (Equals((DeviceId)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
        public bool Equals(DeviceId value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
                    _vendorId.Equals(value._vendorId) &&
                    _productId.Equals(value._productId)
					);
			}
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
        public string ToString(bool appendIds, bool appendInUseText)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ManufacturerName);         // "Company"

            if (appendIds)
            {
                sb.Append(" (VID:");
                sb.Append(VendorIdString);       // "Company (VID:0ABC)"
                sb.Append(")");
            }

            sb.Append(" ");
            sb.Append(ProductName);              // "Company (VID:0ABC) Product"

            if (appendIds)
            {
                sb.Append(" (PID:");
                sb.Append(ProductIdString);      // "Company (VID:0ABC) Product (PID:1234)"
                sb.Append(")");
            }

            sb.Append(" ");
            sb.Append(SerialNumber);             // "Company (VID:0ABC) Product (PID:1234) 000123A"

            if (appendInUseText && IsInUse)
            {
                sb.Append(Separator);            // "Company (VID:0ABC) Product (PID:1234) 000123A - "
                sb.Append(InUseText);            // "Company (VID:0ABC) Product (PID:1234) 000123A - (in use)"
            }

            return (sb.ToString());
        }

        /// <summary>
        /// Parses s for the first integer number and returns the corresponding device.
        /// </summary>
        public static DeviceId Parse(string s)
        {
            DeviceId result;
            if (TryParse(s, out result))
                return (result);
            else
                throw (new FormatException(s + " does not specify a valid USB device ID"));
        }

        /// <summary>
        /// Tries to parse s for the first integer number and returns the corresponding device.
        /// </summary>
        public static bool TryParse(string s, out DeviceId result)
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
                            result = new DeviceId(vendorId, productId);
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
        public int CompareTo(object obj)
        {
            if (obj == null) return (1);
            if (obj is DeviceId)
            {
                DeviceId id = (DeviceId)obj;
                if (VendorId != id.VendorId)
                    return (VendorId.CompareTo(id.VendorId));
                else
                    return (ProductId.CompareTo(id.ProductId));
            }
            throw (new ArgumentException("Object is not a UsbDeviceId entry"));
        }

        #endregion

        #region Comparison Methods

        /// <summary></summary>
        public static int Compare(object objA, object objB)
        {
            if (ReferenceEquals(objA, objB)) return (0);
            if (objA is DeviceId)
            {
                DeviceId casted = (DeviceId)objA;
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
        public static bool operator ==(DeviceId lhs, DeviceId rhs)
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
        public static bool operator !=(DeviceId lhs, DeviceId rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

        #region Conversion Operators
        //==========================================================================================
        // Conversion Operators
        //==========================================================================================

        /// <summary></summary>
        public static implicit operator string(DeviceId id)
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
