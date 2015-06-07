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
// MKY Development Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#endregion

namespace MKY.IO.Ports
{
	/// <summary></summary>
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
		/// First standard port name as string.
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
		public static readonly Regex StandardPortNumberRegex = new Regex
			(
			@"(?<portNumber>\d+)",
			RegexOptions.Compiled
			);

		/// <summary></summary>
		public static readonly Regex StandardPortNameRegex = new Regex
			(
			StandardPortNamePrefix + @"(?<portNumber>\d+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled
			);

		/// <summary></summary>
		public static readonly Regex StandardPortNameWithParenthesesRegex = new Regex
			(
			@"\(" + StandardPortNamePrefix + @"(?<portNumber>\d+)\)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled
			);

		/// <summary></summary>
		public static readonly Regex StandardPortNameOnlyRegex = new Regex
			(
			@"^" + StandardPortNamePrefix + @"(?<portNumber>\d+)$",
			RegexOptions.IgnoreCase | RegexOptions.Compiled
			);

		/// <summary></summary>
		public static readonly Regex UserPortNameRegex = new Regex
			(
			@"(?<portName>\w+)\x20?",
			RegexOptions.Compiled
			);

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary>
		/// First standard port.
		/// </summary>
		/// <remarks>
		/// This property can be used as default port. Using <see cref="FirstAvailablePort"/>
		/// below is way worse performing since it needs to search for available ports. Searching
		/// for available ports takes quite some time, especially if checking whether the ports
		/// are in use.
		/// </remarks>
		public static SerialPortId FirstStandardPort
		{
			// Must be implemented as property that creates a new id object on each call to
			// ensure that there aren't multiple clients referencing (and modifying) the same
			// id object.
			get { return (new SerialPortId(FirstStandardPortNumber)); }
		}

		/// <summary>
		/// Returns the first port available, usually "COM1".
		/// Returns <c>null</c> if no ports are available.
		/// </summary>
		public static SerialPortId FirstAvailablePort
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

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string name = FirstStandardPortName;
		private int standardPortNumber = FirstStandardPortNumber;

		private string caption;
		private bool hasCaptionFromSystem;

		private bool isInUse;
		private string inUseText = "";

		private string separator = DefaultSeparator;

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
		public SerialPortId(string name)
		{
			Name = name;
		}

		/// <summary></summary>
		public SerialPortId(int standardPortNumber)
		{
			StandardPortNumber = standardPortNumber;
		}

		/// <summary></summary>
		public SerialPortId(SerialPortId rhs)
		{
			this.name = rhs.name;
			this.standardPortNumber = rhs.standardPortNumber;

			this.caption = rhs.caption;
			this.hasCaptionFromSystem = rhs.hasCaptionFromSystem;

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
		/// Port name (e.g. "COM1").
		/// </summary>
		[XmlElement("Name")]
		public virtual string Name
		{
			get { return (this.name); }
			set
			{
				if (this.name != value)
				{
					SerialPortId id;
					if (TryParseStandardPortName(value, out id))
					{
						this.standardPortNumber = id.StandardPortNumber;
						this.name = id.Name;
					}
					else
					{
						this.standardPortNumber = 0;
						this.name = value;
					}
				}
			}
		}

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
					throw (new ArgumentOutOfRangeException
					(
						"value",
						value,
						"Standard port numbers are " + FirstStandardPortNumber + " to " + LastStandardPortNumber + "."
					));
				}
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
		/// Port caption (e.g. "Serial On USB Port").
		/// </summary>
		[XmlIgnore]
		public virtual string Caption
		{
			get { return (this.caption); }
			set { this.caption = value;  }
		}

		/// <summary>
		/// Indicates whether port has retrieved caption from system.
		/// </summary>
		[XmlIgnore]
		public virtual bool HasCaptionFromSystem
		{
			get { return (this.hasCaptionFromSystem); }
			set { this.hasCaptionFromSystem = value; }
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
				if (string.IsNullOrEmpty(this.inUseText))
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
				if (this.separator.Length == 0)
					return (DefaultSeparator);
				else
					return (this.separator);
			}
			set
			{
				this.separator = value;
			}
		}

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve the description
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual void GetDescriptionFromSystem()
		{
			Dictionary<string, string> descriptions = SerialPortSearcher.GetCaptionsFromSystem();

			if (descriptions.ContainsKey(this.name))
				SetCaptionFromSystem(descriptions[this.name]);
		}

		/// <summary>
		/// Set the caption which was retrieved from the system.
		/// </summary>
		/// <remarks>
		/// Can be used to set the descriptions of multiple ID with a single call to
		/// <see cref="SerialPortSearcher.GetCaptionsFromSystem()"/>. A single call is much
		/// faster than calling <see cref="SerialPortSearcher.GetCaptionsFromSystem()"/> for
		/// each ID.
		/// </remarks>
		/// <param name="caption">The description to set.</param>
		public virtual void SetCaptionFromSystem(string caption)
		{
			this.caption = caption;
			this.hasCaptionFromSystem = true;
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialPortId));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortId other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return (Equals(other.Name));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(string otherName)
		{
			// Only field 'name' is relevant. Other properties are for convenience only.
			return (StringEx.EqualsOrdinalIgnoreCase(Name, otherName));
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
			if (Name != null)
				return (Name.GetHashCode());
			else
				return (base.GetHashCode());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToString(true, true));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual string ToString(bool appendCaption, bool appendInUseText)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Name);                     // "COM10"

			if (appendCaption && !string.IsNullOrEmpty(Caption))
			{
				sb.Append(Separator);            // "COM10 - "
				sb.Append(Caption);              // "COM10 - Serial On USB Port"
			}

			if (appendInUseText && IsInUse)
			{
				sb.Append(Separator);            // "COM10 - Serial On USB Port - "
				sb.Append(InUseText);            // "COM10 - Serial On USB Port - (in use)"
			}

			return (sb.ToString());
		}

		#endregion

		#region Parse/From
		//==========================================================================================
		// Parse/From
		//==========================================================================================

		/// <summary>
		/// Parses <paramref name="s"/> for the first integer number and returns the corresponding port ID.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialPortId Parse(string s)
		{
			SerialPortId result;
			if      (TryParseStandardPortName(s, out result))
				return (result);
			else if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify a valid serial port ID."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for the first integer number and returns the corresponding port ID.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialPortId result)
		{
			s = s.Trim();

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
		/// Parses <paramref name="s"/> for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortId ParseStandardPortName(string s)
		{
			SerialPortId result;
			if (TryParseStandardPortName(s, out result))
				return (result);
			else
				throw (new FormatException(s + " does not specify a valid serial port ID."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for the first integer number and returns the corresponding port.
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

		/// <summary>
		/// Tries to create a <see cref="SerialPortId"/> object from the given port number.
		/// </summary>
		public static bool TryFrom(int portNumber, out SerialPortId result)
		{
			if (IsStandardPortNumber(portNumber))
			{
				result = new SerialPortId(portNumber);
				return (true);
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
			return (StandardPortNamePrefix + standardPortNumber.ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region IComparable Members

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			SerialPortId other = obj as SerialPortId;
			if (other != null)
			{
				if (IsStandardPort && other.IsStandardPort)
					return (StandardPortNumber.CompareTo(other.StandardPortNumber));
				else
					return (StringEx.CompareOrdinalIgnoreCase(Name, other.Name));
			}
			else
			{
				throw (new ArgumentException(obj.ToString() + " does not specify a 'SerialPortId!"));
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

			SerialPortId casted = objA as SerialPortId;
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
		public static bool operator ==(SerialPortId lhs, SerialPortId rhs)
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
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
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
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is int)
			{
				try   { SerialPortId port = new SerialPortId((int)value);    UnusedLocal.PreventAnalysisWarning(port); return (true); }
				catch { return (false); }
			}
			if (value is string)
			{
				try   { SerialPortId port = new SerialPortId((string)value); UnusedLocal.PreventAnalysisWarning(port); return (true); }
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
