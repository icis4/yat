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
// MKY Version 1.0.29
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

namespace MKY.Media
{
	#region Enum SystemSounds

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "Same name as 'System.Media.SystemSounds'.")]
	public enum SystemSounds
	{
		/// <remarks>No corresponding item in <see cref="System.Media.SystemSounds"/>.</remarks>
		None,

		/// <remarks>Corresponds to <see cref="System.Media.SystemSounds.Asterisk"/>.</remarks>
		Asterisk,

		/// <remarks>Corresponds to <see cref="System.Media.SystemSounds.Beep"/>.</remarks>
		Beep,

		/// <remarks>Corresponds to <see cref="System.Media.SystemSounds.Exclamation"/>.</remarks>
		Exclamation,

		/// <remarks>Corresponds to <see cref="System.Media.SystemSounds.Hand"/>.</remarks>
		Hand,

		/// <remarks>Corresponds to <see cref="System.Media.SystemSounds.Question"/>.</remarks>
		Question
	}

	#endregion

	/// <summary>
	/// Extended enum SystemSoundsEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class SystemSoundsEx : EnumEx
	{
		#region String Definitions

		private const string             None_string = "[None]";
		private static readonly string[] None_stringAlternatives = new string[] { "[N]" };

		private const string Asterisk_string    = "Asterisk";
		private const string Beep_string        = "Beep";
		private const string Exclamation_string = "Exclamation";
		private const string Hand_string        = "Hand";
		private const string Question_string    = "Question";

		#endregion

		/// <summary>Default is <see cref="SystemSounds.None"/>.</summary>
		public const SystemSounds Default = SystemSounds.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SystemSoundsEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SystemSoundsEx(SystemSounds sound)
			: base(sound)
		{
		}

		#region Play
		//==========================================================================================
		// Play
		//==========================================================================================

		/// <summary></summary>
		public virtual void Play()
		{
			var sound = (SystemSounds)UnderlyingEnum;
			if (sound != SystemSounds.None)
				((System.Media.SystemSound)((SystemSoundsEx)sound)).Play();
		}

		#endregion

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
			switch ((SystemSounds)UnderlyingEnum)
			{
				case SystemSounds.None:        return (None_string);
				case SystemSounds.Asterisk:    return (Asterisk_string);
				case SystemSounds.Beep:        return (Beep_string);
				case SystemSounds.Exclamation: return (Exclamation_string);
				case SystemSounds.Hand:        return (Hand_string);
				case SystemSounds.Question:    return (Question_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is a system sound that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
		public static SystemSoundsEx[] GetItems()
		{
			var a = new List<SystemSoundsEx>(6); // Preset the required capacity to improve memory management.

			a.Add(new SystemSoundsEx(SystemSounds.None));
			a.Add(new SystemSoundsEx(SystemSounds.Asterisk));
			a.Add(new SystemSoundsEx(SystemSounds.Beep));
			a.Add(new SystemSoundsEx(SystemSounds.Exclamation));
			a.Add(new SystemSoundsEx(SystemSounds.Hand));
			a.Add(new SystemSoundsEx(SystemSounds.Question));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SystemSoundsEx Parse(string s)
		{
			SystemSoundsEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid system sound string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SystemSoundsEx result)
		{
			SystemSounds enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new SystemSoundsEx(enumResult);
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
		public static bool TryParse(string s, out SystemSounds result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = SystemSounds.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, None_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, None_stringAlternatives))
			{
				result = SystemSounds.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Asterisk_string))
			{
				result = SystemSounds.Asterisk;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Beep_string))
			{
				result = SystemSounds.Beep;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Exclamation_string))
			{
				result = SystemSounds.Exclamation;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Hand_string))
			{
				result = SystemSounds.Hand;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Question_string))
			{
				result = SystemSounds.Question;
				return (true);
			}
			else // Invalid string!
			{
				result = new SystemSoundsEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator SystemSounds(SystemSoundsEx sound)
		{
			return ((SystemSounds)sound.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SystemSoundsEx(SystemSounds sound)
		{
			return (new SystemSoundsEx(sound));
		}

		/// <summary></summary>
		public static implicit operator int(SystemSoundsEx sound)
		{
			return (sound.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SystemSoundsEx(int sound)
		{
			return (new SystemSoundsEx((SystemSounds)sound));
		}

		/// <summary></summary>
		public static implicit operator string(SystemSoundsEx sound)
		{
			return (sound.ToString());
		}

		/// <summary></summary>
		public static implicit operator SystemSoundsEx(string sound)
		{
			return (Parse(sound));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public static implicit operator System.Media.SystemSound(SystemSoundsEx sound)
		{
			switch ((SystemSounds)sound)
			{
				case SystemSounds.None:        return (null);
				case SystemSounds.Asterisk:    return (System.Media.SystemSounds.Asterisk);
				case SystemSounds.Beep:        return (System.Media.SystemSounds.Beep);
				case SystemSounds.Exclamation: return (System.Media.SystemSounds.Exclamation);
				case SystemSounds.Hand:        return (System.Media.SystemSounds.Hand);
				case SystemSounds.Question:    return (System.Media.SystemSounds.Question);

				default: throw (new ArgumentOutOfRangeException("sound", sound, MessageHelper.InvalidExecutionPreamble + "'" + sound.ToString() + "' is a system sound that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public static implicit operator SystemSoundsEx(System.Media.SystemSound sound)
		{
			if      (sound == System.Media.SystemSounds.Asterisk)
				return (SystemSounds.Asterisk);
			else if (sound == System.Media.SystemSounds.Beep)
				return (SystemSounds.Beep);
			else if (sound == System.Media.SystemSounds.Exclamation)
				return (SystemSounds.Exclamation);
			else if (sound == System.Media.SystemSounds.Hand)
				return (SystemSounds.Hand);
			else if (sound == System.Media.SystemSounds.Question)
				return (SystemSounds.Question);
			else
				throw (new ArgumentOutOfRangeException("sound", sound, MessageHelper.InvalidExecutionPreamble + "'" + sound.ToString() + "' is a system sound that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
