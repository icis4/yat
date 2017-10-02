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
	#region Enum TimeDeltaFormatPreset

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum TimeDeltaFormatPreset
	{
		None,

		Standard
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum TimeDeltaFormatPresetEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class TimeDeltaFormatPresetEx : EnumEx
	{
		#region String Definitions

		private const string None_format      = "";
		private const string None_description = "[No preset selected]";

		/// <remarks>
		/// Output milliseconds for readability, even though last digit only provides limited accuracy.
		/// </remarks>
		private const string Standard_format      = @"[d \days ][h][h\:][m][m\:][s]s\.fff"; // Attention, slightly different than time span!
		private const string Standard_description =  "Standard";

		#endregion

		/// <summary>Default is <see cref="TimeDeltaFormatPreset.Standard"/>.</summary>
		public const TimeDeltaFormatPreset Default = TimeDeltaFormatPreset.Standard;

		/// <summary>Default is <see cref="TimeDeltaFormatPreset.Standard"/>.</summary>
		public const string DefaultFormat = Standard_format;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public TimeDeltaFormatPresetEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public TimeDeltaFormatPresetEx(TimeDeltaFormatPreset preset)
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
			switch ((TimeDeltaFormatPreset)UnderlyingEnum)
			{
				case TimeDeltaFormatPreset.None:     return (None_format);

				case TimeDeltaFormatPreset.Standard: return (Standard_format);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((TimeDeltaFormatPreset)UnderlyingEnum)
			{
				case TimeDeltaFormatPreset.None:     return (None_description);

				case TimeDeltaFormatPreset.Standard: return (Standard_description);

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
		public static TimeDeltaFormatPresetEx[] GetItems()
		{
			List<TimeDeltaFormatPresetEx> a = new List<TimeDeltaFormatPresetEx>(2); // Preset the required capacity to improve memory management.

			a.Add(new TimeDeltaFormatPresetEx(TimeDeltaFormatPreset.None));

			a.Add(new TimeDeltaFormatPresetEx(TimeDeltaFormatPreset.Standard));

			return (a.ToArray());
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator TimeDeltaFormatPreset(TimeDeltaFormatPresetEx preset)
		{
			return ((TimeDeltaFormatPreset)preset.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator TimeDeltaFormatPresetEx(TimeDeltaFormatPreset preset)
		{
			return (new TimeDeltaFormatPresetEx(preset));
		}

		/// <summary></summary>
		public static implicit operator string(TimeDeltaFormatPresetEx preset)
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
