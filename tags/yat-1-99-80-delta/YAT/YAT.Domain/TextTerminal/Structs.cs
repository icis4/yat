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
using System.Xml.Serialization;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	public struct TextLineSendDelay : IEquatable<TextLineSendDelay>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary>Delay in milliseconds.</summary>
		[XmlElement("Delay")]
		public int Delay { get; set; }

		/// <summary></summary>
		[XmlElement("LineInterval")]
		public int LineInterval { get; set; }

		/// <summary></summary>
		public TextLineSendDelay(bool enabled, int delay, int lineInterval)
		{
			Enabled = enabled;
			Delay = delay;
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
			if (obj is TextLineSendDelay)
				return (Equals((TextLineSendDelay)obj));
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
		public bool Equals(TextLineSendDelay other)
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
		public static bool operator ==(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct WaitForResponse : IEquatable<WaitForResponse>
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled { get; set; }

		/// <summary>Wait timeout in milliseconds.</summary>
		[XmlElement("Timeout")]
		public int Timeout { get; set; }

		/// <summary></summary>
		public WaitForResponse(bool enabled, int timeout)
		{
			Enabled = enabled;
			Timeout = timeout;
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

				hashCode =                    Enabled.GetHashCode();
				hashCode = (hashCode * 397) ^ Timeout.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is WaitForResponse)
				return (Equals((WaitForResponse)obj));
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
		public bool Equals(WaitForResponse other)
		{
			return
			(
				Enabled.Equals(other.Enabled) &&
				Timeout.Equals(other.Timeout)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(WaitForResponse lhs, WaitForResponse rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have inequality.
		/// </summary>
		public static bool operator !=(WaitForResponse lhs, WaitForResponse rhs)
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
