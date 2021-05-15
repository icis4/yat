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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Domain.Parser
{
	#region Enum Keyword

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Keyword
	{
		None,

		Clear,
		Delay,
		LineDelay,
		LineInterval,
	////Repeat is yet pending (FR #13) and requires parser support for strings (FR #404).
		LineRepeat,
		TimeStamp,
		Eol,
		NoEol,
		Port,
		PortSettings,
		Baud,
		DataBits,
		Parity,
		StopBits,
		FlowControl,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		RtsOn,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		RtsOff,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rts", Justification = "'RTS' is a common term for serial ports.")]
		RtsToggle,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		DtrOn,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		DtrOff,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "'DTR' is a common term for serial ports.")]
		DtrToggle,
		OutputBreakOn,
		OutputBreakOff,
		OutputBreakToggle,
		FramingErrorsOn,
		FramingErrorsOff,
		FramingErrorsRestore,
		ReportId,

		/// <summary>
		/// A special keyword For Internal Testing (= FIT).
		/// </summary>
		/// <remarks>
		/// Prepended "ZZZ_" to even more obviously indicate internal usage.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "See remarks above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ZZZ", Justification = "See remarks above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FIT", Justification = "See summary above.")]
		ZZZ_FIT
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum KeywordEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class KeywordEx : EnumEx
	{
		#region String Definitions

		private const string Clear_string                = "Clear";
		private const string Delay_string                = "Delay";
		private const string LineDelay_string            = "LineDelay";
		private const string LineInterval_string         = "LineInterval";
		private const string LineRepeat_string           = "LineRepeat";
		private const string TimeStamp_string            = "TimeStamp";
		private const string Eol_string                  = "EOL";
		private const string NoEol_string                = "NoEOL";
		private const string Port_string                 = "Port";
		private const string PortSettings_string         = "PortSettings";
		private const string Baud_string                 = "Baud";
		private const string StopBits_string             = "StopBits";
		private const string Parity_string               = "Parity";
		private const string DataBits_string             = "DataBits";
		private const string FlowControl_string          = "FlowControl";
		private const string RtsOn_string                = "RTSOn";
		private const string RtsOff_string               = "RTSOff";
		private const string RtsToggle_string            = "RTSToggle";
		private const string DtrOn_string                = "DTROn";
		private const string DtrOff_string               = "DTROff";
		private const string DtrToggle_string            = "DTRToggle";
		private const string OutputBreakOn_string        = "OutputBreakOn";
		private const string OutputBreakOff_string       = "OutputBreakOff";
		private const string OutputBreakToggle_string    = "OutputBreakToggle";
		private const string FramingErrorsOn_string      = "FramingErrorsOn";
		private const string FramingErrorsOff_string     = "FramingErrorsOff";
		private const string FramingErrorsRestore_string = "FramingErrorsRestore";
		private const string ReportId_string             = "ReportID"; // "ID" instead of "Id" for better readability.

		private const string ZZZ_FIT_string = "ZZZ_FIT"; // = for internal testing.

		#endregion

		/// <summary>Default is <see cref="Keyword.None"/>.</summary>
		public const Keyword Default = Keyword.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public KeywordEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public KeywordEx(Keyword keyword)
			: base(keyword)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:                return (Clear_string);
				case Keyword.Delay:                return (Delay_string);
				case Keyword.LineDelay:            return (LineDelay_string);
				case Keyword.LineInterval:         return (LineInterval_string);
			////case Keyword.Repeat:               return (Repeat_string); is yet pending (FR #13) and requires parser support for strings (FR #404).
				case Keyword.LineRepeat:           return (LineRepeat_string);
				case Keyword.TimeStamp:            return (TimeStamp_string);
				case Keyword.Eol:                  return (Eol_string);
				case Keyword.NoEol:                return (NoEol_string);
				case Keyword.Port:                 return (Port_string);
				case Keyword.PortSettings:         return (PortSettings_string);
				case Keyword.Baud:                 return (Baud_string);
				case Keyword.DataBits:             return (DataBits_string);
				case Keyword.Parity:               return (Parity_string);
				case Keyword.StopBits:             return (StopBits_string);
				case Keyword.FlowControl:          return (FlowControl_string);
				case Keyword.RtsOn:                return (RtsOn_string);
				case Keyword.RtsOff:               return (RtsOff_string);
				case Keyword.RtsToggle:            return (RtsToggle_string);
				case Keyword.DtrOn:                return (DtrOn_string);
				case Keyword.DtrOff:               return (DtrOff_string);
				case Keyword.DtrToggle:            return (DtrToggle_string);
				case Keyword.OutputBreakOn:        return (OutputBreakOn_string);
				case Keyword.OutputBreakOff:       return (OutputBreakOff_string);
				case Keyword.OutputBreakToggle:    return (OutputBreakToggle_string);
				case Keyword.FramingErrorsOn:      return (FramingErrorsOn_string);
				case Keyword.FramingErrorsOff:     return (FramingErrorsOff_string);
				case Keyword.FramingErrorsRestore: return (FramingErrorsRestore_string);
				case Keyword.ReportId:             return (ReportId_string);

				case Keyword.ZZZ_FIT: return (ZZZ_FIT_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems/Args/Validation
		//==========================================================================================
		// GetItems/Args/Validation
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static KeywordEx[] GetItems()
		{
			var a = new List<KeywordEx>(22); // Preset the required capacity to improve memory management.

			// Do not add 'None'.

			a.Add(new KeywordEx(Keyword.Clear));
			a.Add(new KeywordEx(Keyword.Delay));
			a.Add(new KeywordEx(Keyword.LineDelay));
			a.Add(new KeywordEx(Keyword.LineInterval));
		////a.Add(new KeywordEx(Keyword.Repeat)); is yet pending (FR #13) and requires parser support for strings (FR #404).
			a.Add(new KeywordEx(Keyword.LineRepeat));
			a.Add(new KeywordEx(Keyword.TimeStamp));
			a.Add(new KeywordEx(Keyword.Eol));
			a.Add(new KeywordEx(Keyword.NoEol));
			a.Add(new KeywordEx(Keyword.Port));
			a.Add(new KeywordEx(Keyword.PortSettings));
			a.Add(new KeywordEx(Keyword.Baud));
			a.Add(new KeywordEx(Keyword.DataBits));
			a.Add(new KeywordEx(Keyword.Parity));
			a.Add(new KeywordEx(Keyword.StopBits));
			a.Add(new KeywordEx(Keyword.FlowControl));
			a.Add(new KeywordEx(Keyword.RtsOn));
			a.Add(new KeywordEx(Keyword.RtsOff));
			a.Add(new KeywordEx(Keyword.RtsToggle));
			a.Add(new KeywordEx(Keyword.DtrOn));
			a.Add(new KeywordEx(Keyword.DtrOff));
			a.Add(new KeywordEx(Keyword.DtrToggle));
			a.Add(new KeywordEx(Keyword.OutputBreakOn));
			a.Add(new KeywordEx(Keyword.OutputBreakOff));
			a.Add(new KeywordEx(Keyword.OutputBreakToggle));
			a.Add(new KeywordEx(Keyword.FramingErrorsOn));
			a.Add(new KeywordEx(Keyword.FramingErrorsOff));
			a.Add(new KeywordEx(Keyword.FramingErrorsRestore));
			a.Add(new KeywordEx(Keyword.ReportId));

			// Do not add 'ZZZ_FIT' (= for internal testing).

			return (a.ToArray());
		}

		/// <summary>
		/// Gets the maximum arguments count for this keyword.
		/// </summary>
		/// <remarks>
		/// Method instead of property for orthogonality with <see cref="GetItems()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "See remarks.")]
		public virtual int GetMaxArgsCount()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay:        return (1);
				case Keyword.LineDelay:    return (1);
				case Keyword.LineInterval: return (1);
			////case Keyword.Repeat:       return (3); is yet pending (FR #13) and requires parser support for strings (FR #404).
				case Keyword.LineRepeat:   return (1);
			////case Keyword.TimeStamp:    return (1); with argument is yet pending (FR #400) and requires parser support for strings (FR #404).
				case Keyword.Port:         return (1);
				case Keyword.PortSettings: return (5);
				case Keyword.Baud:         return (1);
				case Keyword.DataBits:     return (1);
				case Keyword.Parity:       return (1);
				case Keyword.StopBits:     return (1);
				case Keyword.FlowControl:  return (1);
				case Keyword.ReportId:     return (1);

				case Keyword.ZZZ_FIT:      return (3); // = for internal testing.

				default: return (0);
			}
		}

		/// <summary>
		/// Validates the given value for the given argument index of this keyword.
		/// </summary>
		public virtual bool Validate(int argIndex, int argValue)
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay:        return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:    return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval: return (argValue >= 1); // Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
			////case Keyword.Repeat:       return (ValidateRepeatArg(argIndex, argValue)); is yet pending (FR #13) and requires parser support for strings (FR #404).
				                                                 //// Attention, a similar validation exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineRepeat:   return ((argValue >= 1) || (argValue == Settings.SendSettings.LineRepeatInfinite));
			////case Keyword.TimeStamp:    return (ValidateTimeStampArg(argIndex, argValue)); with argument is yet pending (FR #400) and requires parser support for strings (FR #404).
				case Keyword.Port:         return (MKY.IO.Ports.SerialPortId.IsStandardPortNumber(argValue));
				case Keyword.PortSettings: return (MKY.IO.Ports.BaudRateEx.IsPotentiallyValid(argValue) || MKY.IO.Ports.DataBitsEx.IsDefined(argValue) || MKY.IO.Ports.ParityEx.IsDefined(argValue) || MKY.IO.Ports.StopBitsEx.IsDefined(argValue) || MKY.IO.Serial.SerialPort.SerialFlowControlEx.IsDefined(argValue) );
				                                                 //// Attention, a similar validation exists in 'View.Controls.SerialPortSettings'. Changes here may have to be applied there too.
				case Keyword.Baud:         return (MKY.IO.Ports.BaudRateEx.IsPotentiallyValid(argValue));
				case Keyword.DataBits:     return (MKY.IO.Ports.DataBitsEx                     .IsDefined(argValue));
				case Keyword.Parity:       return (MKY.IO.Ports.ParityEx                       .IsDefined(argValue));
				case Keyword.StopBits:     return (MKY.IO.Ports.StopBitsEx                     .IsDefined(argValue));
				case Keyword.FlowControl:  return (MKY.IO.Serial.SerialPort.SerialFlowControlEx.IsDefined(argValue));
				                                                 //// Attention, a similar validation exists in 'View.Controls.UsbSerialHidDeviceSettings'. Changes here may have to be applied there too.
				case Keyword.ReportId:     return ((argValue >= 0) && (argValue <= 255));

				case Keyword.ZZZ_FIT: // = for internal testing.
				{
					switch (argIndex)
					{
						case 0: return (true);
						case 1: return (true);
						case 2: return (true);

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support more than 3 arguments!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Gets the validation fragment for the given argument index of this keyword.
		/// </summary>
		public virtual string GetValidationFragment(int argIndex)
		{
			string noArgSupportedMessage = "[" + MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support arguments! " + MessageHelper.SubmitBug + "]";

			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Clear:                return (noArgSupportedMessage);
				case Keyword.Delay:                return ("an integer value of 1 or more specifying the delay in milliseconds");    // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineDelay:            return ("an integer value of 1 or more specifying the delay in milliseconds");    // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.LineInterval:         return ("an integer value of 1 or more specifying the interval in milliseconds"); // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
			////case Keyword.Repeat:               return (RepeatArgMessage); is yet pending (FR #13) and requires parser support for strings (FR #404).
				case Keyword.LineRepeat:           return ("an integer value of 1 or more indicating the number of repetitions, or " + Settings.SendSettings.LineRepeatInfinite + " for infinite repetitions");
				case Keyword.TimeStamp:            return (noArgSupportedMessage);                                                   // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.Eol:                  return (noArgSupportedMessage);
				case Keyword.NoEol:                return (noArgSupportedMessage);
				case Keyword.Port:                 return ("an integer value within " + MKY.IO.Ports.SerialPortId.FirstStandardPortNumber + ".." + MKY.IO.Ports.SerialPortId.LastStandardPortNumber + " specifying the number of the serial COM port");
				case Keyword.PortSettings:         return ("one or more integer values specifying the settings, separated by ',' (comma) or ';' (semicolon) or '|' (pipe)");
				case Keyword.Baud:                 return ("an integer value of 1 or more specifying the number of bits per second");
				case Keyword.DataBits:             return ("an integer value of 4 to 8"); // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.Parity:               return ("an integer value where 0 = None, 1 = Odd, 2 = Even, 3 = Mark, 4 = Space");
				case Keyword.StopBits:             return ("an integer value 0, 1 or 2"); // Attention, a similar message exists in 'View.Forms.AdvancedTerminalSettings'. Changes here may have to be applied there too.
				case Keyword.FlowControl:          return ("an integer value where 0 = None, 1 = Software, 2 = Hardware, 3 = Combined, 4 = Manual Hardware, 5 = Manual Software, 6 = Manual Combined, 7 = RS-485");
				case Keyword.RtsOn:                return (noArgSupportedMessage);
				case Keyword.RtsOff:               return (noArgSupportedMessage);
				case Keyword.RtsToggle:            return (noArgSupportedMessage);
				case Keyword.DtrOn:                return (noArgSupportedMessage);
				case Keyword.DtrOff:               return (noArgSupportedMessage);
				case Keyword.DtrToggle:            return (noArgSupportedMessage);
				case Keyword.OutputBreakOn:        return (noArgSupportedMessage);
				case Keyword.OutputBreakOff:       return (noArgSupportedMessage);
				case Keyword.OutputBreakToggle:    return (noArgSupportedMessage);
				case Keyword.FramingErrorsOn:      return (noArgSupportedMessage);
				case Keyword.FramingErrorsOff:     return (noArgSupportedMessage);
				case Keyword.FramingErrorsRestore: return (noArgSupportedMessage);
				case Keyword.ReportId:             return ("an integer value within 0..255 specifying the report ID"); // Attention, a similar message exists in 'View.Controls.UsbSerialHidDeviceSettings'. Changes here may have to be applied there too.

				case Keyword.ZZZ_FIT: // = for internal testing.
				{
					switch (argIndex)
					{
						case 0: return ("an integer value");
						case 1: return ("an integer value");
						case 2: return ("an integer value");

						default: return ("[" + MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' doesn't support more than 3 arguments! " + MessageHelper.SubmitBug + "]");
					}
				}

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static KeywordEx Parse(string s)
		{
			KeywordEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid keyword! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out KeywordEx result)
		{
			Keyword enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new KeywordEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out Keyword result)
		{
			if (s != null)
				s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Clear_string))
			{
				result = Keyword.Clear;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Delay_string))
			{
				result = Keyword.Delay;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineDelay_string))
			{
				result = Keyword.LineDelay;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineInterval_string))
			{
				result = Keyword.LineInterval;
				return (true);
			}
		////else if (StringEx.EqualsOrdinalIgnoreCase(s, Repeat_string)) is yet pending (FR #13) and requires parser support for strings (FR #404).
		////{
		////	result = Keyword.Repeat;
		////	return (true);
		////}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineRepeat_string))
			{
				result = Keyword.LineRepeat;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TimeStamp_string))
			{
				result = Keyword.TimeStamp;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Eol_string))
			{
				result = Keyword.Eol;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, NoEol_string))
			{
				result = Keyword.NoEol;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Port_string))
			{
				result = Keyword.Port;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, PortSettings_string))
			{
				result = Keyword.PortSettings;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Baud_string))
			{
				result = Keyword.Baud;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DataBits_string))
			{
				result = Keyword.DataBits;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Parity_string))
			{
				result = Keyword.Parity;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, StopBits_string))
			{
				result = Keyword.StopBits;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, FlowControl_string))
			{
				result = Keyword.FlowControl;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, RtsOn_string))
			{
				result = Keyword.RtsOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, RtsOff_string))
			{
				result = Keyword.RtsOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, RtsToggle_string))
			{
				result = Keyword.RtsToggle;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DtrOn_string))
			{
				result = Keyword.DtrOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DtrOff_string))
			{
				result = Keyword.DtrOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DtrToggle_string))
			{
				result = Keyword.DtrToggle;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOn_string))
			{
				result = Keyword.OutputBreakOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakOff_string))
			{
				result = Keyword.OutputBreakOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, OutputBreakToggle_string))
			{
				result = Keyword.OutputBreakToggle;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, FramingErrorsOn_string))
			{
				result = Keyword.FramingErrorsOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, FramingErrorsOff_string))
			{
				result = Keyword.FramingErrorsOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, FramingErrorsRestore_string))
			{
				result = Keyword.FramingErrorsRestore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ReportId_string))
			{
				result = Keyword.ReportId;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ZZZ_FIT_string))
			{
				result = Keyword.ZZZ_FIT;
				return (true);
			}
			else // Invalid string!
			{
				result = new KeywordEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator Keyword(KeywordEx keyword)
		{
			return ((Keyword)keyword.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(Keyword keyword)
		{
			return (new KeywordEx(keyword));
		}

		/// <summary></summary>
		public static implicit operator int(KeywordEx keyword)
		{
			return (keyword.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(int keyword)
		{
			return (new KeywordEx((Keyword)keyword));
		}

		/// <summary></summary>
		public static implicit operator string(KeywordEx keyword)
		{
			return (keyword.ToString());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(string keyword)
		{
			return (Parse(keyword));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
