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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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

using MKY.Contracts;

#endregion

namespace MKY.IO.Ports
{
	/// <remarks>
	/// \remind (2019-11-10 / MKY)
	/// Instances of this container class shall be treated as immutable objects. However, it is not
	/// possible to assign <see cref="ImmutableObjectAttribute"/>/<see cref="ImmutableContractAttribute"/>
	/// because XML default serialization requires public setters. Split into mutable settings tuple
	/// and immutable runtime container should be done.
	/// </remarks>
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
		public const int LastStandardPortNumber = 65536;

		/// <summary></summary>
		public const int StandardPortCount = (LastStandardPortNumber - FirstStandardPortNumber + 1);

		/// <summary></summary>
		public const int FirstTypicalStandardPortNumber = FirstStandardPortNumber;

		/// <summary></summary>
		public const int LastTypicalStandardPortNumber = 256;

		/// <summary></summary>
		public const int TypicalStandardPortCount = (LastTypicalStandardPortNumber - FirstTypicalStandardPortNumber + 1);

		/// <summary>
		/// First standard port name as string.
		/// </summary>
		/// <remarks>
		/// Can be used as default string on attributes such as <see cref="DefaultValueAttribute"/>,
		/// must therefore be a constant (and not a readonly).
		/// </remarks>
		public const string FirstStandardPortName = "COM1";

		/// <summary></summary>
		public const string InUseTextDefault = "(in use)";

		/// <summary></summary>
		public const string SeparatorDefault = " - ";

		private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		/// <summary></summary>
		public static readonly Regex StandardPortNameRegex = new Regex(StandardPortNamePrefix + @"(?<portNumber>\d+)", Options);

		/// <summary></summary>
		public static readonly Regex StandardPortNameWithParenthesesRegex = new Regex(@"\(" + StandardPortNamePrefix + @"(?<portNumber>\d+)\)", Options);

		/// <summary></summary>
		public static readonly Regex StandardPortNameOnlyRegex = new Regex(@"^" + StandardPortNamePrefix + @"(?<portNumber>\d+)$", Options);

		/// <summary></summary>
		public static readonly Regex UserPortNameRegex = new Regex(@"(?<portName>\w+)\x20?", Options);

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
		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="SerialPortId"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static SerialPortId FirstStandardPort
		{
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
				l.FillWithAvailablePorts(false); // Explicitly not getting captions, thus faster.

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
		private string inUseText = InUseTextDefault;

		private string separator = SeparatorDefault;

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
		public SerialPortId()
		{
			// Defaults only.
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
			this.name               = rhs.name;
			this.standardPortNumber = rhs.standardPortNumber;

			this.caption              = rhs.caption;
			this.hasCaptionFromSystem = rhs.hasCaptionFromSystem;

			this.isInUse   = rhs.isInUse;
			this.inUseText = rhs.inUseText;

			this.separator = rhs.separator;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Port name, e.g. "COM1".
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
		/// Port number, e.g. 1 of "COM1".
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="value"/> is not a standard port number.
		/// </exception>
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
						"Standard port numbers are " + FirstStandardPortNumber + " to " + LastStandardPortNumber + "!"
					)); // Do not append 'MessageHelper.InvalidExecutionPreamble' as caller could rely on this exception text.
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
		/// Port caption, e.g. "Serial On USB Port" of "COM1 - Serial On USB Port".
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
			set { this.hasCaptionFromSystem = value;  }
		}

		/// <summary>
		/// Indicates whether port is currently in use.
		/// </summary>
		[XmlIgnore]
		public virtual bool IsInUse
		{
			get { return (this.isInUse); }
			set { this.isInUse = value;  }
		}

		/// <summary>
		/// The text which is shown when port is currently in use, e.g. "(in use)" of "COM1 - (in use)".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(InUseTextDefault)]
		public virtual string InUseText
		{
			get { return (this.inUseText); }
			set { this.inUseText = value;  }
		}

		/// <summary>
		/// The separator, e.g. " - " of "COM1 - Serial On USB Port".
		/// </summary>
		[XmlIgnore]
		[DefaultValue(SeparatorDefault)]
		public virtual string Separator
		{
			get
			{
				if (string.IsNullOrEmpty(this.separator))
					return (SeparatorDefault);
				else
					return (this.separator);
			}
			set
			{
				this.separator = value;
			}
		}

		/// <summary>
		/// Queries WMI (Windows Management Instrumentation) trying to retrieve the caption
		/// that is associated with the serial port.
		/// </summary>
		/// <remarks>
		/// Query is never done automatically because it takes quite some time.
		/// </remarks>
		public virtual void GetCaptionFromSystem()
		{
			Dictionary<string, string> captions = SerialPortSearcher.GetCaptionsFromSystem();

			if (captions.ContainsKey(this.name))
				SetCaptionFromSystem(captions[this.name]);
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
		/// <param name="caption">The caption to set.</param>
		public virtual void SetCaptionFromSystem(string caption)
		{
			this.caption = caption;
			this.hasCaptionFromSystem = true;
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Corresponds to calling <see cref="ToString(bool, bool)"/> with
		/// 'appendCaption' = <c>true</c> and 'appendInUseText' = <c>true</c>.
		/// </remarks>
		public override string ToString()
		{
			return (ToString(true, true));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Corresponds to calling <see cref="ToString(bool, bool)"/> with
		/// 'appendCaption' = <c>true</c> and 'appendInUseText' = <c>false</c>.
		/// </remarks>
		public virtual string ToNameAndCaptionString()
		{
			return (ToString(true, false));
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
			var sb = new StringBuilder(Name); // "COM10"

			if (appendCaption && (!string.IsNullOrEmpty(Caption)))
			{
				sb.Append(Separator);         // "COM10 - "
				sb.Append(Caption);           // "COM10 - Serial On USB Port"
			}

			if (appendInUseText && IsInUse && (!string.IsNullOrEmpty(InUseText)))
			{
				sb.Append(Separator);         // "COM10 - Serial On USB Port - "
				sb.Append(InUseText);         // "COM10 - Serial On USB Port - (in use)"
			}

			return (sb.ToString());
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
			unchecked
			{
				if (Name != null)
					return (Name.GetHashCode());
				else
					return (base.GetHashCode());
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialPortId));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortId other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				EqualsName(other.Name) // Only 'Name' is relevant. Other properties are for convenience only.
			);
		}

		/// <summary>
		/// Determines whether this instance's and the specified object's name have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool EqualsName(string otherName)
		{
			return (StringEx.EqualsOrdinalIgnoreCase(Name, otherName));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialPortId lhs, SerialPortId rhs)
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
		public static bool operator !=(SerialPortId lhs, SerialPortId rhs)
		{
			return (!(lhs == rhs));
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
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s))
			{
				result = null;
				return (false);
			}

			// e.g. "COM1"
			if (TryParseStandardPortName(s, out result))
				return (true);

			// e.g. "ABC"
			var m = UserPortNameRegex.Match(s);
			if (m.Success)
			{
				string portName = m.Value;
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
				int portNumber;  // m.Value is e.g. "COM2 thus [1] is "2".
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
				int portNumber;  // m.Value is e.g. "(COM2) thus [1] is "2".
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
				int portNumber;  // m.Value is e.g. "COM2 thus [1] is "2".
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
			return ((standardPortNumber >= FirstStandardPortNumber) && (standardPortNumber <= LastStandardPortNumber));
		}

		/// <summary></summary>
		public static bool IsTypicalStandardPortNumber(int standardPortNumber)
		{
			return ((standardPortNumber >= FirstTypicalStandardPortNumber) && (standardPortNumber <= LastTypicalStandardPortNumber));
		}

		/// <summary></summary>
		public static string StandardPortNumberToString(int standardPortNumber)
		{
			return (StandardPortNamePrefix + standardPortNumber.ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			var other = (obj as SerialPortId);
			if (other != null)
			{
				if (IsStandardPort && other.IsStandardPort)
					return (StandardPortNumber.CompareTo(other.StandardPortNumber));
				else
					return (StringEx.CompareOrdinalIgnoreCase(Name, other.Name));
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify a 'SerialPortId!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as SerialPortId);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
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
			this.portList.FillWithTypicalStandardPorts();
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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
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
