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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
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

		LocalTime,
		LocalTimeWithTimeZone,
		UtcTime,

		LocalDateAndTime,
		LocalDateAndTimeWithTimeZone,
		UtcDateAndTime,

		LocalDateAndTime_ISO8601,
		LocalDateAndTimeWithTimeZone_ISO8601,
		UtcDateAndTime_ISO8601
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
		private const string LocalTime_format                  = "HH:mm:ss.fff";
		private const string LocalTime_description             = "Local time";
		private const string LocalTimeWithTimeZone_format      = "HH:mm:ss.fffzzz";
		private const string LocalTimeWithTimeZone_description = "Local time with time zone";
		private const string UtcTime_format                    = "HH:mm:ss.fffK";
		private const string UtcTime_description               = "UTC time";

		private const string LocalDateAndTime_format                  = "yyyy-MM-dd HH:mm:ss.fff";
		private const string LocalDateAndTime_description             = "Local date and time";
		private const string LocalDateAndTimeWithTimeZone_format      = "yyyy-MM-dd HH:mm:ss.fffzzz";
		private const string LocalDateAndTimeWithTimeZone_description = "Local date and time with time zone";
		private const string UtcDateAndTime_format                    = "u";
		private const string UtcDateAndTime_description               = "UTC date and time";

		private const string LocalDateAndTime_ISO8601_format                  = "s";
		private const string LocalDateAndTime_ISO8601_description             = "Local date and time ISO8601";
		private const string LocalDateAndTimeWithTimeZone_ISO8601_format      = "yyyy-MM-ddTHH:mm:ss.fffzzz";
		private const string LocalDateAndTimeWithTimeZone_ISO8601_description = "Local date and time with time zone ISO8601";
		private const string UtcDateAndTime_ISO8601_format                    = "yyyy-MM-ddTHH:mm:ss.fffK";
		private const string UtcDateAndTime_ISO8601_description               = "UTC date and time ISO8601";

		#endregion

		/// <summary>Default is <see cref="TimeStampFormatPreset.LocalTime"/>.</summary>
		public const TimeStampFormatPreset Default = TimeStampFormatPreset.LocalTime;

		/// <summary>Default is <see cref="TimeStampFormatPreset.LocalTime"/>.</summary>
		public const string DefaultFormat = LocalTime_format;

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
				case TimeStampFormatPreset.None:                 return (None_format);

				case TimeStampFormatPreset.LocalTime:             return (LocalTime_format);
				case TimeStampFormatPreset.LocalTimeWithTimeZone: return (LocalTimeWithTimeZone_format);
				case TimeStampFormatPreset.UtcTime:               return (UtcTime_format);

				case TimeStampFormatPreset.LocalDateAndTime:             return (LocalDateAndTime_format);
				case TimeStampFormatPreset.LocalDateAndTimeWithTimeZone: return (LocalDateAndTimeWithTimeZone_format);
				case TimeStampFormatPreset.UtcDateAndTime:               return (UtcDateAndTime_format);

				case TimeStampFormatPreset.LocalDateAndTime_ISO8601:             return (LocalDateAndTime_ISO8601_format);
				case TimeStampFormatPreset.LocalDateAndTimeWithTimeZone_ISO8601: return (LocalDateAndTimeWithTimeZone_ISO8601_format);
				case TimeStampFormatPreset.UtcDateAndTime_ISO8601:               return (UtcDateAndTime_ISO8601_format);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((TimeStampFormatPreset)UnderlyingEnum)
			{
				case TimeStampFormatPreset.None:                 return (None_description);

				case TimeStampFormatPreset.LocalTime:             return (LocalTime_description);
				case TimeStampFormatPreset.LocalTimeWithTimeZone: return (LocalTimeWithTimeZone_description);
				case TimeStampFormatPreset.UtcTime:               return (UtcTime_description);

				case TimeStampFormatPreset.LocalDateAndTime:             return (LocalDateAndTime_description);
				case TimeStampFormatPreset.LocalDateAndTimeWithTimeZone: return (LocalDateAndTimeWithTimeZone_description);
				case TimeStampFormatPreset.UtcDateAndTime:               return (UtcDateAndTime_description);

				case TimeStampFormatPreset.LocalDateAndTime_ISO8601:             return (LocalDateAndTime_ISO8601_description);
				case TimeStampFormatPreset.LocalDateAndTimeWithTimeZone_ISO8601: return (LocalDateAndTimeWithTimeZone_ISO8601_description);
				case TimeStampFormatPreset.UtcDateAndTime_ISO8601:               return (UtcDateAndTime_ISO8601_description);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static TimeStampFormatPresetEx[] GetItems()
		{
			List<TimeStampFormatPresetEx> a = new List<TimeStampFormatPresetEx>(10); // Preset the required capacity to improve memory management.

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.None));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalTime));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalTimeWithTimeZone));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.UtcTime));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalDateAndTime));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalDateAndTimeWithTimeZone));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.UtcDateAndTime));

			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalDateAndTime_ISO8601));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.LocalDateAndTimeWithTimeZone_ISO8601));
			a.Add(new TimeStampFormatPresetEx(TimeStampFormatPreset.UtcDateAndTime_ISO8601));

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
