//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
		public const string StandardPortNamePrefix = "COM";

		/// <summary></summary>
		public const int FirstStandardPortNumber = 1;
		/// <summary></summary>
		public const int LastStandardPortNumber = 256;
		/// <summary></summary>
		public const int MaxStandardPortNumber = 65536;

		/// <summary>
		/// First port name as string.
		/// </summary>
		/// <remarks>
		/// Can be used as default string on attributes such as <see cref="System.ComponentModel.DefaultValueAttribute"/>.
		/// </remarks>
		public const string FirstStandardPortName = "COM1";

		/// <summary></summary>
		public const string DefaultInUseText = "(in use)";

		/// <summary></summary>
		public const string DefaultSeparator = " - ";

		/// <summary></summary>
		public static readonly Regex StandardPortNumberRegex;
		/// <summary></summary>
		public static readonly Regex StandardPortNameRegex;
		/// <summary></summary>
		public static readonly Regex StandardPortNameWithParenthesesRegex;
		/// <summary></summary>
		public static readonly Regex StandardPortNameOnlyRegex;
		/// <summary></summary>
		public static readonly Regex UserPortNameRegex;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string name = FirstStandardPortName;
		private int standardPortNumber = FirstStandardPortNumber;

		private string description = "";
		private bool hasDescriptonFromSystem = false;

		private bool isInUse = false;
		private string inUseText = "";

		private string separator = DefaultSeparator;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		/// <summary></summary>
		static SerialPortId()
		{
			StandardPortNumberRegex              = new Regex(@"(?<portNumber>\d+)",                                                              RegexOptions.Compiled);
			StandardPortNameRegex                = new Regex(StandardPortNamePrefix + @"(?<portNumber>\d+)",           RegexOptions.IgnoreCase | RegexOptions.Compiled);
			StandardPortNameWithParenthesesRegex = new Regex(@"\(" + StandardPortNamePrefix + @"(?<portNumber>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			StandardPortNameOnlyRegex            = new Regex(@"^" + StandardPortNamePrefix + @"(?<portNumber>\d+)$",   RegexOptions.IgnoreCase | RegexOptions.Compiled);
			UserPortNameRegex                    = new Regex(@"(?<portName>\w+)\x20?",                                                           RegexOptions.Compiled);
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
				SerialPortCollection l = new SerialPortCollection();
				l.FillWithAvailablePorts(false);

				if (l.Count > 0)
					return (new SerialPortId(l[0]));
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
		}

		/// <summary></summary>
		public SerialPortId(int standardPortNumber)
		{
			if (IsStandardPortNumber(standardPortNumber))
			{
				this.name = StandardPortNumberToString(standardPortNumber);
				this.standardPortNumber = standardPortNumber;
			}
			else
			{
				throw (new ArgumentOutOfRangeException
					(
					"standardPortNumber",
					standardPortNumber,
					"Standard port numbers are " + FirstStandardPortNumber + " to " + LastStandardPortNumber
					));
			}
		}

		/// <summary></summary>
		public SerialPortId(string portName)
		{
			SerialPortId id;
			if (TryParseStandardPortName(portName, out id))
			{
				this.name = id.Name;
				this.standardPortNumber = id.StandardPortNumber;
			}
			else
			{
				this.name = portName;
				this.standardPortNumber = 0;
			}
		}

		/// <summary></summary>
		public SerialPortId(SerialPortId rhs)
		{
			this.name = rhs.name;
			this.standardPortNumber = rhs.standardPortNumber;

			this.description = rhs.description;
			this.hasDescriptonFromSystem = rhs.hasDescriptonFromSystem;

			this.isInUse = rhs.isInUse;
			this.inUseText = rhs.inUseText;

			this.separator = rhs.separator;
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
		public virtual int StandardPortNumber
		{
			get { return (this.standardPortNumber); }
			set
			{
				if (IsStandardPortNumber(value))
				{
					this.name = StandardPortNumberToString(value);
					this.standardPortNumber = value;
				}
				else
				{
					this.name = value.ToString();
					this.standardPortNumber = 0;
				}
			}
		}

		/// <summary>
		/// Port name (e.g. "COM1").
		/// </summary>
		[XmlElement("Name")]
		public virtual string Name
		{
			get { return (this.name); }
			set
			{
				this.name = value;

				SerialPortId id;
				if (TryParseStandardPortName(value, out id))
					this.standardPortNumber = id.StandardPortNumber;
				else
					this.standardPortNumber = 0;
			}
		}

		/// <summary>
		/// Returns whether this port ID is a standard port COM1 to COM65536.
		/// </summary>
		[XmlIgnore]
		public virtual bool IsStandardPort
		{
			get { return (IsStandardPortNumber(this.standardPortNumber)); }
		}


		/// <summary>
		/// Port description (e.g. "Serial On USB Port").
		/// </summary>
		[XmlIgnore]
		public virtual string Description
		{
			get { return (this.description); }
			set { this.description = value;  }
		}

		/// <summary>
		/// Indicates whether port has retrieved description from system.
		/// </summary>
		[XmlIgnore]
		public virtual bool HasDescriptionFromSystem
		{
			get { return (this.hasDescriptonFromSystem); }
			set { this.hasDescriptonFromSystem = value; }
		}

		/// <summary>
		/// Indicates whether port is currently in use.
		/// </summary>
		[XmlIgnore]
		public virtual bool IsInUse
		{
			get { return (this.isInUse); }
			set { this.isInUse = value; }
		}

		/// <summary>
		/// The text which is shown when port is currently in use, e.g. "COM1 - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultInUseText)]
		public virtual string InUseText
		{
			get
			{
				if (this.inUseText == "")
					return (DefaultInUseText);
				else
					return (this.inUseText);
			}
			set
			{
				this.inUseText = value;
			}
		}

		/// <summary>
		/// The separator, e.g. "COM1 - Serial On USB Port".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(DefaultSeparator)]
		public virtual string Separator
		{
			get
			{
				if (this.separator == "")
					return (DefaultSeparator);
				else
					return (this.separator);
			}
			set
			{
				this.separator = value;
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve the description
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual void GetDescriptionFromSystem()
		{
			Dictionary<string, string> descriptions = SerialPortSearcher.GetDescriptionsFromSystem();

			if (descriptions.ContainsKey(this.name))
			{
				Description = descriptions[this.name];
				this.hasDescriptonFromSystem = true;
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
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
				return (this.name == value.name);

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
		public virtual string ToString(bool appendDescription, bool appendInUseText)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Name);                     // "COM10"

			if (appendDescription && (Description != null) && (Description != ""))
			{
				sb.Append(Separator);            // "COM10 - "
				sb.Append(Description);          // "COM10 - Serial On USB Port"
			}

			if (appendInUseText && IsInUse)
			{
				sb.Append(Separator);            // "COM10 - Serial On USB Port - "
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
			if      (TryParseStandardPortName(s, out result))
				return (result);
			else if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(s + " does not specify a valid serial port ID"));
		}

		/// <summary>
		/// Tries to parse s for the first integer number and returns the corresponding port.
		/// </summary>
		public static bool TryParse(string s, out SerialPortId result)
		{
			// e.g. "COM1"
			if (TryParseStandardPortName(s, out result))
				return (true);

			// e.g. "ABC"
			Match m = UserPortNameRegex.Match(s);
			if (m.Success)
			{
				string portName = m.Groups[1].Value;
				result = new SerialPortId(portName);
				return (true);
			}

			result = null;
			return (false);
		}

		/// <summary>
		/// Parses s for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortId ParseStandardPortName(string s)
		{
			SerialPortId result;
			if (TryParseStandardPortName(s, out result))
				return (result);
			else
				throw (new FormatException(s + " does not specify a valid serial port ID"));
		}

		/// <summary>
		/// Tries to parse s for the first integer number and returns the corresponding port.
		/// </summary>
		public static bool TryParseStandardPortName(string s, out SerialPortId result)
		{
			Match m;

			// e.g. "COM2"
			m = StandardPortNameOnlyRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if (IsStandardPortNumber(portNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
					}
				}
			}

			// e.g. "Bluetooth Communications Port (COM2)"
			m = StandardPortNameWithParenthesesRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if (IsStandardPortNumber(portNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
					}
				}
			}

			// e.g. "Modem on COM2"
			m = StandardPortNameRegex.Match(s);
			if (m.Success)
			{
				int portNumber;
				if (int.TryParse(m.Groups[1].Value, out portNumber))
				{
					if (IsStandardPortNumber(portNumber))
					{
						result = new SerialPortId(portNumber);
						return (true);
					}
				}
			}

			result = null;
			return (false);
		}

		/// <summary></summary>
		public static bool IsStandardPortNumber(int standardPortNumber)
		{
			return ((standardPortNumber >= FirstStandardPortNumber) && (standardPortNumber <= MaxStandardPortNumber));
		}

		/// <summary></summary>
		public static string StandardPortNumberToString(int standardPortNumber)
		{
			return (StandardPortNamePrefix + standardPortNumber.ToString());
		}

		#endregion

		#region IComparable Members

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return (1);
			if (obj is SerialPortId)
			{
				SerialPortId id = (SerialPortId)obj;
				if (IsStandardPort && id.IsStandardPort)
					return (StandardPortNumber.CompareTo(id.StandardPortNumber));
				else
					return (Name.CompareTo(id.Name));
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
		public static implicit operator string(SerialPortId id)
		{
			return (id.Name);
		}

		/// <summary></summary>
		public static implicit operator SerialPortId(string portName)
		{
			return (new SerialPortId(portName));
		}

		/// <summary></summary>
		public static implicit operator int(SerialPortId id)
		{
			return (id.StandardPortNumber);
		}

		/// <summary></summary>
		public static implicit operator SerialPortId(int standardPortNumber)
		{
			return (new SerialPortId(standardPortNumber));
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
		private SerialPortCollection portList;

		/// <summary></summary>
		public SerialPortIdConverter()
		{
			this.portList = new SerialPortCollection();
			this.portList.FillWithStandardPorts();
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
			return (new StandardValuesCollection(this.portList));
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
			if (value is string) return (SerialPortId.ParseStandardPortName((string)value));
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
			if (destinationType == typeof(int))    return (((SerialPortId)value).StandardPortNumber);
			if (destinationType == typeof(string)) return (((SerialPortId)value).Name);
			if (destinationType == typeof(InstanceDescriptor) && value is SerialPortId)
			{
				SerialPortId port = (SerialPortId)value;
				ConstructorInfo ctor = typeof(SerialPortId).GetConstructor(new Type[] { typeof(int) });
				if (ctor != null)
					return (new InstanceDescriptor(ctor, new object[] { port.StandardPortNumber }));
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
