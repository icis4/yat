//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	public struct TextLineSendDelay
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public getter/setter is required for default XML serialization/deserialization anyway.")]
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary>Delay in milliseconds.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public getter/setter is required for default XML serialization/deserialization anyway.")]
		[XmlElement("Delay")]
		public int Delay;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public getter/setter is required for default XML serialization/deserialization anyway.")]
		[XmlElement("LineInterval")]
		public int LineInterval;

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
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			TextLineSendDelay other = (TextLineSendDelay)obj;
			return
			(
				(Enabled      == other.Enabled) &&
				(Delay        == other.Delay) &&
				(LineInterval == other.LineInterval)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TextLineSendDelay lhs, TextLineSendDelay rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public struct WaitForResponse
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public getter/setter is required for default XML serialization/deserialization anyway.")]
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary>Wait timeout in milliseconds.</summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Public getter/setter is required for default XML serialization/deserialization anyway.")]
		[XmlElement("Timeout")]
		public int Timeout;

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
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			WaitForResponse other = (WaitForResponse)obj;
			return
			(
				(Enabled == other.Enabled) &&
				(Timeout == other.Timeout)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(WaitForResponse lhs, WaitForResponse rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
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
