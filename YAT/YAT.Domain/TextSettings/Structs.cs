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
using System.Xml.Serialization;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\TextSettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public struct TextLineSendDelaySettingTuple : IEquatable<TextLineSendDelaySettingTuple>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary>Delay in milliseconds.</summary>
		[XmlElement("Delay")]
		public int Delay { get; set; }

		/// <remarks>Interval here means "each n lines".</remarks>
		[XmlElement("LineInterval")]
		public int LineInterval { get; set; }

		/// <summary></summary>
		public TextLineSendDelaySettingTuple(bool enabled, int delay, int lineInterval)
		{
			Enabled      = enabled;
			Delay        = delay;
			LineInterval = lineInterval;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
				int hashCode;

				hashCode =                    Enabled     .GetHashCode();
				hashCode = (hashCode * 397) ^ Delay       .GetHashCode();
				hashCode = (hashCode * 397) ^ LineInterval.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TextLineSendDelaySettingTuple)
				return (Equals((TextLineSendDelaySettingTuple)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextLineSendDelaySettingTuple other)
		{
			return
			(
				Enabled     .Equals(other.Enabled) &&
				Delay       .Equals(other.Delay)   &&
				LineInterval.Equals(other.LineInterval)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(TextLineSendDelaySettingTuple lhs, TextLineSendDelaySettingTuple rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(TextLineSendDelaySettingTuple lhs, TextLineSendDelaySettingTuple rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct TextWaitForResponseSettingTuple : IEquatable<TextWaitForResponseSettingTuple>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary></summary>
		[XmlElement("ResponseLineCount")]
		public int ResponseLineCount { get; set; }

		/// <summary></summary>
		[XmlElement("ClearanceLineCount")]
		public int ClearanceLineCount { get; set; }

		/// <summary>Time-out in milliseconds.</summary>
		[XmlElement("Timeout")]
		public int Timeout { get; set; }

		/// <summary></summary>
		public TextWaitForResponseSettingTuple(bool enabled, int responseLineCount, int clearanceLineCount, int timeout)
		{
			Enabled            = enabled;
			ResponseLineCount  = responseLineCount;
			ClearanceLineCount = clearanceLineCount;
			Timeout            = timeout;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
				int hashCode;

				hashCode =                    Enabled           .GetHashCode();
				hashCode = (hashCode * 397) ^ ResponseLineCount .GetHashCode();
				hashCode = (hashCode * 397) ^ ClearanceLineCount.GetHashCode();
				hashCode = (hashCode * 397) ^ Timeout           .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TextWaitForResponseSettingTuple)
				return (Equals((TextWaitForResponseSettingTuple)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextWaitForResponseSettingTuple other)
		{
			return
			(
				Enabled           .Equals(other.Enabled)            &&
				ResponseLineCount .Equals(other.ResponseLineCount)  &&
				ClearanceLineCount.Equals(other.ClearanceLineCount) &&
				Timeout           .Equals(other.Timeout)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(TextWaitForResponseSettingTuple lhs, TextWaitForResponseSettingTuple rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(TextWaitForResponseSettingTuple lhs, TextWaitForResponseSettingTuple rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
