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
// YAT Version 2.2.0 Development
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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum TimeStampFormatPreset

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum TimeStampFormatPreset
	{
		None,

		Time,
		TimeWithTimeZone,

		DateAndTime,
		DateAndTimeWithTimeZone,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		DateAndTime_Iso8601,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize this item as a variant of the corresponding previous item.")]
		DateAndTimeWithTimeZone_Iso8601
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum TimeStampFormatPresetEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class TimeStampFormatPresetEx : EnumEx
	{
		#region String Definitions

		private const string None_format      = "";
		private const string None_description = "[No preset selected]";

		/// <remarks>
		/// Output milliseconds for readability, even though last digit only provides limited accuracy.
		/// </remarks>
		private const string Time_format                  = "HH:mm:ss.fff";
		private const string Time_description             = "Local time";
		private const string TimeWithTimeZone_format      = "HH:mm:ss.fffK";
		private const string TimeWithTimeZone_description = "Time with time zone";

		private const string DateAndTime_format                  = "yyyy-MM-dd HH:mm:ss.fff";
		private const string DateAndTime_description             = "Local date and time";
		private const string DateAndTimeWithTimeZone_format      = "yyyy-MM-dd HH:mm:ss.fffK";
		private const string DateAndTimeWithTimeZone_description = "Date and time with time zone";

		private const string DateAndTime_Iso8601_format                  = "s";
		private const string DateAndTime_Iso8601_description             = "Date and time ISO8601";
		private const string DateAndTimeWithTimeZone_Iso8601_format      = "yyyy-MM-ddTHH:mm:ss.fffK";
		private const string DateAndTimeWithTimeZone_Iso8601_description = "Date and time with time zone ISO8601";

		#endregion

		/// <summary>Default is <see cref="TimeStampFormatPreset.Time"/>.</summary>
		public const TimeStampFormatPreset Default = TimeStampFormatPreset.Time;

		/// <summary>Default is <see cref="TimeStampFormatPreset.Time"/>.</summary>
		public const string DefaultFormat = Time_format;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public TimeStampFormatPresetEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public TimeStampFormatPresetEx(TimeStampFormatPreset preset)
			: base(preset)
		{
		}

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToFormat()
		{
			switch ((TimeStampFormatPreset)UnderlyingEnum)
			{
				case TimeStampFormatPreset.None: return (None_format);

				case TimeStampFormatPreset.Time:             return (Time_format);
				case TimeStampFormatPreset.TimeWithTimeZone: return (TimeWithTimeZone_format);

				case TimeStampFormatPreset.DateAndTime:             return (DateAndTime_format);
				case TimeStampFormatPreset.DateAndTimeWithTimeZone: return (DateAndTimeWithTimeZone_format);

				case TimeStampFormatPreset.DateAndTime_Iso8601:             return (DateAndTime_Iso8601_format);
				case TimeStampFormatPreset.DateAndTimeWithTimeZone_Iso8601: return (DateAndTimeWithTimeZone_Iso8601_format);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((TimeStampFormatPreset)UnderlyingEnum)
			{
				case TimeStampFormatPreset.None: return (None_description);

				case TimeStampFormatPreset.Time:             return (Time_description);
				case TimeStampFormatPreset.TimeWithTimeZone: return (TimeWithTimeZone_description);

				case TimeStampFormatPreset.DateAndTime:             return (DateAndTime_description);
				case TimeStampFormatPreset.DateAndTimeWithTimeZone: return (DateAndTimeWithTimeZone_description);

				case TimeStampFormatPreset.DateAndTime_Iso8601:             return (DateAndTime_Iso8601_description);
				case TimeStampFormatPreset.DateAndTimeWithTimeZone_Iso8601: return (DateAndTimeWithTimeZone_Iso8601_description);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToDescription());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static TimeStampFormatPresetEx Parse(string s)
		{
			TimeStampFormatPresetEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid preset string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out TimeStampFormatPresetEx result)
		{
			TimeStampFormatPreset enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new TimeStampFormatPresetEx(enumResult);
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
		public static bool TryParse(string s, out TimeStampFormatPreset result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = TimeStampFormatPreset.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, None_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = TimeStampFormatPreset.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, Time_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Time_description))
			{
				result = TimeStampFormatPreset.Time;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, TimeWithTimeZone_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, TimeWithTimeZone_description))
			{
				result = TimeStampFormatPreset.TimeWithTimeZone;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, DateAndTime_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DateAndTime_description))
			{
				result = TimeStampFormatPreset.DateAndTime;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, DateAndTimeWithTimeZone_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DateAndTimeWithTimeZone_description))
			{
				result = TimeStampFormatPreset.DateAndTimeWithTimeZone;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, DateAndTime_Iso8601_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DateAndTime_Iso8601_description))
			{
				result = TimeStampFormatPreset.DateAndTime_Iso8601;
				return (true);
			}
			else if (StringEx.EqualsOrdinal(          s, DateAndTimeWithTimeZone_Iso8601_format) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DateAndTimeWithTimeZone_Iso8601_description))
			{
				result = TimeStampFormatPreset.DateAndTimeWithTimeZone_Iso8601;
				return (true);
			}
			else // Invalid string!
			{
				result = new TimeStampFormatPresetEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static TimeStampFormatPresetEx[] GetItems()
		{
			var a = new List<TimeStampFormatPresetEx>(7); // Preset the required capacity to improve memory management.

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.None));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.Time));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.TimeWithTimeZone));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.DateAndTime));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.DateAndTimeWithTimeZone));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.DateAndTime_Iso8601));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.DateAndTimeWithTimeZone_Iso8601));

			return (a.ToArray());
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator TimeStampFormatPreset(TimeStampFormatPresetEx preset)
		{
			return ((TimeStampFormatPreset)preset.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator TimeStampFormatPresetEx(TimeStampFormatPreset preset)
		{
			return (new TimeStampFormatPresetEx(preset));
		}

		/// <summary></summary>
		public static implicit operator string(TimeStampFormatPresetEx preset)
		{
			return (preset.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
