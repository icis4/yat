//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace MKY.IO.Ports
{
	/// <summary>
	/// SerialPortId.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(SerialPortIdConverter))]
	public class SerialPortId : IEquatable<SerialPortId>, IComparable
	{
		#region Public Constants
		//==========================================================================================
		// Public Constants
		//==========================================================================================

		/// <summary></summary>
		public const string PortNamePrefix = "COM";

		/// <summary></summary>
		public const int FirstPortNumber = 1;
		/// <summary></summary>
		public const int LastStandardPortNumber = 256;
		/// <summary></summary>
		public const int MaxPortNumber = 65536;

		/// <summary>
		/// First port name as string.
		/// </summary>
		/// <remarks>
		/// Can be used as default string on attributes such as <see cref="System.ComponentModel.DefaultValueAttribute"/>.
		/// </remarks>
		public const string FirstPortName = "COM1";

		/// <summary></summary>
		public const string DefaultDescriptionSeparator = "-";

		/// <summary></summary>
		public const string DefaultInUseSeparator = "-";
		/// <summary></summary>
		public const string DefaultInUseText = "(in use)";

		/// <summary></summary>
		public static readonly Regex PortNumberRegex;
		/// <summary></summary>
		public static readonly Regex PortNameRegex;
		/// <summary></summary>
		public static readonly Regex PortNameWithParenthesesRegex;
		/// <summary></summary>
		public static readonly Regex PortNameOnlyRegex;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int _number = FirstPortNumber;

		private string _description = null;
		private string _descriptionSeparator = null;
		private bool _hasDescriptonFromSystem = false;

		private bool _isInUse = false;
		private string _inUseText = null;
		private string _inUseSeparator = null;

		#endregion

		#region Static Object Lifetime
		//==========================================================================================
		// Static Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		static SerialPortId()
		{
			PortNumberRegex              = new Regex(@"(?<port>\d+)", RegexOptions.Compiled);
			PortNameRegex                = new Regex(PortNamePrefix + @"(?<port>\d+)",           RegexOptions.IgnoreCase | RegexOptions.Compiled);
			PortNameWithParenthesesRegex = new Regex(@"\(" + PortNamePrefix + @"(?<port>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			PortNameOnlyRegex            = new Regex(@"^" + PortNamePrefix + @"(?<port>\d+)$",   RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary>
		/// Returns default port on system. Default is the first port available, usually "COM1".
		/// Returns <c>null</c> if no ports are available.
		/// </summary>
		public static SerialPortId DefaultPort
		{
			get
			{
				SerialPortList l = new SerialPortList();
				l.FillWithAvailablePorts();

				if (l.Count > 0)
					return (new SerialPortId(l[0].Number));
				else
					return (null);
			}
		}

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPortId()
		{
			_number = FirstPortNumber;
		}

		/// <summary></summary>
		public SerialPortId(int portNumber)
		{
			_number = portNumber;
			CheckPort();
		}

		/// <summary></summary>
		public SerialPortId(string portName)
		{
			_number = Parse(portName);
			CheckPort();
		}

		private void CheckPort()
		{
			if (!(_number >= FirstPortNumber))
				throw (new ArgumentOutOfRangeException("SerialPortId.Number", _number, "ASSERT(Number >= StandardFirstPort)."));
			if (!(_number <= MaxPortNumber))
				throw (new ArgumentOutOfRangeException("SerialPortId.Number", _number, "ASSERT(Number <= MaxLastPort)."));
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Port number (e.g. 1).
		/// </summary>
		[XmlIgnore]
		public int Number
		{
			get { return (_number); }
			set
			{
				_number = value;
				CheckPort();
			}
		}

		/// <summary>
		/// Port name (e.g. "COM1").
		/// </summary>
		[XmlElement("Name")]
		public string Name
		{
			get { return (PortNamePrefix + _number.ToString()); }
			set
			{
				_number = Parse(value);
				CheckPort();
			}
		}

		/// <summary>
		/// Port description (e.g. "Serial On USB Port").
		/// </summary>
		[XmlIgnore]
		public string Description
		{
			get { return (_description); }
			set
			{
				if (value == "")
					_description = null;
				else
					_description = value;
			}
		}

		/// <summary>
		/// The separator which is shown when port is currently in use, e.g. "COM1 - Serial On USB Port".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultDescriptionSeparator)]
		public string DescriptionSeparator
		{
			get
			{
				if (_descriptionSeparator == null)
					return (DefaultDescriptionSeparator);
				else
					return (_descriptionSeparator);
			}
			set
			{
				if (value == "")
					_descriptionSeparator = null;
				else
					_descriptionSeparator = value;
			}
		}

		/// <summary>
		/// Indicates whether port has retrieved description from system.
		/// </summary>
		[XmlIgnore]
		public bool HasDescriptionFromSystem
		{
			get { return (_hasDescriptonFromSystem); }
			set { _hasDescriptonFromSystem = value; }
		}

		/// <summary>
		/// Indicates whether port is currently in use.
		/// </summary>
		[XmlIgnore]
		public bool IsInUse
		{
			get { return (_isInUse); }
			set { _isInUse = value; }
		}

		/// <summary>
		/// The text which is shown when port is currently in use, e.g. "COM1 - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultInUseText)]
		public string InUseText
		{
			get
			{
				if (_inUseText == null)
					return (DefaultInUseText);
				else
					return (_inUseText);
			}
			set
			{
				if (value == "")
					_inUseText = null;
				else
					_inUseText = value;
			}
		}

		/// <summary>
		/// The separator which is shown when port is currently in use, e.g. "COM1 - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultInUseSeparator)]
		public string InUseSeparator
		{
			get
			{
				if (_inUseSeparator == null)
					return (DefaultInUseSeparator);
				else
					return (_inUseSeparator);
			}
			set
			{
				if (value == "")
					_inUseSeparator = null;
				else
					_inUseSeparator = value;
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve to description
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public void GetDescriptionFromSystem()
		{
			Dictionary<int, string> descriptions = SerialPortSearcher.GetDescriptionsFromSystem();

			if (descriptions.ContainsKey(_number))
			{
				Description = descriptions[_number];
				_hasDescriptonFromSystem = true;
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialPortId)
				return (Equals((SerialPortId)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialPortId value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
				return (_number.Equals(value._number));

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
		public string ToString(bool appendDescription, bool appendInUseText)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Name);                     // "COM10"

			if (appendDescription && (Description != null))
			{
				sb.Append(" ");
				sb.Append(DescriptionSeparator); // "COM10 -"
				sb.Append(" ");
				sb.Append(Description);          // "COM10 - Serial On USB Port"
			}

			if (appendInUseText && IsInUse)
			{
				sb.Append(" ");
				sb.Append(InUseSeparator);       // "COM10 - Serial On USB Port -"
				sb.Append(" ");
				sb.Append(InUseText);            // "COM10 - Serial On USB Port - (in use)"
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Parses s for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortId Parse(string s)
		{
			SerialPortId result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(s + " does not specify a valid serial port ID"));
		}

		/// <summary>
		/// Tries to parse s for the first integer number and returns the corresponding port.
		/// </summary>
		public static bool TryParse(string s, out SerialPortId result)
		{
			Match m;

			// e.g. "COM2"
			m = PortNameOnlyRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if ((portNumber >= FirstPortNumber) &&
						(portNumber <= MaxPortNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
					}
				}
			}

			// e.g. "Bluetooth Communications Port (COM2)"
			m = PortNameWithParenthesesRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if ((portNumber >= FirstPortNumber) &&
						(portNumber <= MaxPortNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
					}
				}
			}

			// e.g. "Modem on COM2"
			m = PortNameRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if ((portNumber >= FirstPortNumber) &&
						(portNumber <= MaxPortNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
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
			if (obj is SerialPortId)
			{
				SerialPortId p = (SerialPortId)obj;
				return (Number.CompareTo(p.Number));
			}
			throw (new ArgumentException("Object is not a SerialPortId entry"));
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is SerialPortId)
			{
				SerialPortId casted = (SerialPortId)objA;
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
		public static bool operator ==(SerialPortId lhs, SerialPortId rhs)
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
		public static bool operator !=(SerialPortId lhs, SerialPortId rhs)
		{
			return (!(lhs == rhs));
		}

		/// <summary></summary>
		public static bool operator <(SerialPortId lhs, SerialPortId rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(SerialPortId lhs, SerialPortId rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(SerialPortId lhs, SerialPortId rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(SerialPortId lhs, SerialPortId rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator int(SerialPortId port)
		{
			return (port.Number);
		}

		/// <summary></summary>
		public static implicit operator SerialPortId(int portNumber)
		{
			return (new SerialPortId(portNumber));
		}

		/// <summary></summary>
		public static implicit operator string(SerialPortId port)
		{
			return (port.Name);
		}

		/// <summary></summary>
		public static implicit operator SerialPortId(string portName)
		{
			return (Parse(portName));
		}

		#endregion
	}

	#region Type Converter
	//==========================================================================================
	// Type Converter
	//==========================================================================================

	/// <summary></summary>
	public class SerialPortIdConverter : TypeConverter
	{
		private SerialPortList _portList;

		/// <summary></summary>
		public SerialPortIdConverter()
		{
			_portList = new SerialPortList();
			_portList.FillWithStandardPorts();
		}

		/// <summary>
		/// Indicates this converter provides a list of standard values.
		/// </summary>
		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return (true);
		}

		/// <summary>
		/// Returns a StandardValuesCollection of standard value objects.
		/// </summary>
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return (new StandardValuesCollection(_portList));
		}

		/// <summary>
		/// Indicates list of standard values is exclusive.
		/// </summary>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return (true);
		}

		/// <summary></summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(int))    return (true);
			if (sourceType == typeof(string)) return (true);
			return (base.CanConvertFrom(context, sourceType));
		}

		/// <summary></summary>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is int)    return (new SerialPortId((int)value));
			if (value is string) return (SerialPortId.Parse((string)value));
			return (base.ConvertFrom(context, culture, value));
		}

		/// <summary></summary>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(int))    return (true);
			if (destinationType == typeof(string)) return (true);
			if (destinationType == typeof(InstanceDescriptor))
			{
				return (true);
			}
			return (base.CanConvertTo(context, destinationType));
		}

		/// <summary></summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(int))    return (((SerialPortId)value).Number);
			if (destinationType == typeof(string)) return (((SerialPortId)value).Name);
			if (destinationType == typeof(InstanceDescriptor) && value is SerialPortId)
			{
				SerialPortId port = (SerialPortId)value;
				ConstructorInfo ctor = typeof(SerialPortId).GetConstructor(new Type[] { typeof(int) });
				if (ctor != null)
					return (new InstanceDescriptor(ctor, new object[] { port.Number }));
			}
			return (base.ConvertTo(context, culture, value, destinationType));
		}

		/// <summary></summary>
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is int)
			{
				try   { SerialPortId port = new SerialPortId((int)value);    return (true); }
				catch { return (false); }
			}
			if (value is string)
			{
				try   { SerialPortId port = new SerialPortId((string)value); return (true); }
				catch { return (false); }
			}
			return (base.IsValid(context, value));
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
