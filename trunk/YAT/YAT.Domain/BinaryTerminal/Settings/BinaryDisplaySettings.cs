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
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryDisplaySettings : MKY.Settings.SettingsItem, IEquatable<BinaryDisplaySettings>
	{
		private BinaryLengthLineBreak   lengthLineBreak;
		private BinarySequenceLineBreak sequenceLineBreakBefore;
		private BinarySequenceLineBreak sequenceLineBreakAfter;
		private BinaryTimedLineBreak    timedLineBreak;

		/// <summary></summary>
		public BinaryDisplaySettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public BinaryDisplaySettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public BinaryDisplaySettings(BinaryDisplaySettings rhs)
			: base(rhs)
		{
			LengthLineBreak         = rhs.LengthLineBreak;
			SequenceLineBreakBefore = rhs.SequenceLineBreakBefore;
			SequenceLineBreakAfter  = rhs.SequenceLineBreakAfter;
			TimedLineBreak          = rhs.TimedLineBreak;
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			LengthLineBreak         = new BinaryLengthLineBreak  (true, 16); // Enabled to prevent too long display lines.
			SequenceLineBreakBefore = new BinarySequenceLineBreak(false, @"ABC");
			SequenceLineBreakAfter  = new BinarySequenceLineBreak(false, @"\h(00)");
			TimedLineBreak          = new BinaryTimedLineBreak   (false, 500);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("LengthLineBreak")]
		public BinaryLengthLineBreak LengthLineBreak
		{
			get { return (this.lengthLineBreak); }
			set
			{
				if (this.lengthLineBreak != value)
				{
					this.lengthLineBreak = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SequenceLineBreakBefore")]
		public BinarySequenceLineBreak SequenceLineBreakBefore
		{
			get { return (this.sequenceLineBreakBefore); }
			set
			{
				if (this.sequenceLineBreakBefore != value)
				{
					this.sequenceLineBreakBefore = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SequenceLineBreakAfter")]
		public BinarySequenceLineBreak SequenceLineBreakAfter
		{
			get { return (this.sequenceLineBreakAfter); }
			set
			{
				if (this.sequenceLineBreakAfter != value)
				{
					this.sequenceLineBreakAfter = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimedLineBreak")]
		public BinaryTimedLineBreak TimedLineBreak
		{
			get { return (this.timedLineBreak); }
			set
			{
				if (this.timedLineBreak != value)
				{
					this.timedLineBreak = value;
					SetMyChanged();
				}
			}
		}

		#endregion

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
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ LengthLineBreak        .GetHashCode();
				hashCode = (hashCode * 397) ^ SequenceLineBreakBefore.GetHashCode();
				hashCode = (hashCode * 397) ^ SequenceLineBreakAfter .GetHashCode();
				hashCode = (hashCode * 397) ^ TimedLineBreak         .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as BinaryDisplaySettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(BinaryDisplaySettings other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (ReferenceEquals(this, other))
				return (true);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				LengthLineBreak        .Equals(other.LengthLineBreak)         &&
				SequenceLineBreakBefore.Equals(other.SequenceLineBreakBefore) &&
				SequenceLineBreakAfter .Equals(other.SequenceLineBreakAfter)  &&
				TimedLineBreak         .Equals(other.TimedLineBreak)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryDisplaySettings lhs, BinaryDisplaySettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BinaryDisplaySettings lhs, BinaryDisplaySettings rhs)
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
