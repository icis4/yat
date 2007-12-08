using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace MKY.IO.Ports
{
	/// <summary>
	/// SerialPortId
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
		public const string StandardPortPrefix = "COM";
		/// <summary></summary>
		public const int StandardFirstPort = 1;
		/// <summary></summary>
		public const int StandardLastPort = 256;

		/// <summary></summary>
		public const int DefaultPortNumber = 1;
		/// <summary></summary>
		public const string DefaultPortName = "COM1";
		/// <summary></summary>
		public readonly static SerialPortId DefaultPort = new SerialPortId(DefaultPortNumber);

		/// <summary></summary>
		public const string DefaultInUseText = "(in use)";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int _number = DefaultPortNumber;

		private bool _isInUse = false;
		private string _inUseText = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SerialPortId()
		{
			_number = DefaultPortNumber;
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
			if (!(_number >= StandardFirstPort))
				throw (new ArgumentOutOfRangeException("SerialPortId.Number", _number, "ASSERT(Number >= StandardFirstPort)"));
			if (!(_number <= StandardLastPort))
				throw (new ArgumentOutOfRangeException("SerialPortId.Number", _number, "ASSERT(Number <= StandardLastPort)"));
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
		/// Port name (e.g. COM1).
		/// </summary>
		[XmlElement("Name")]
		public string Name
		{
			get { return (StandardPortPrefix + _number.ToString()); }
			set
			{
				_number = Parse(value);
				CheckPort();
			}
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
		/// The text which is shown when port is currently in use, e.g. "COM1 (in use)".
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
			// ensure that object.operator!=() is called
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
			if (IsInUse)
				return (Name + " " + InUseText);
			else
				return (Name);
		}

		/// <summary>
		/// Parses s for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortId Parse(string s)
		{
			// e.g. "COM1" / "COM1 Bluetooth" / "Bluetooth COM1"
			if (s.StartsWith(StandardPortPrefix))
				s.Remove(0, StandardPortPrefix.Length);

			// e.g. "1" / "1 Bluetooth" / "Bluetooth COM1"
			try
			{
				int portNumber = int.Parse(s);
				return (new SerialPortId(portNumber));
			}
			catch
			{
			}

			// e.g. "1 Bluetooth" / "Bluetooth COM1"
			bool gathering = false;
			StringWriter writer = new StringWriter();
			foreach (char c in s.ToCharArray())
			{
				if (char.IsDigit(c))
				{
					gathering = true;
					writer.Write(c);
				}
				else if (gathering)
				{
					break;
				}
			}
			return (new SerialPortId(int.Parse(writer.ToString())));
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
