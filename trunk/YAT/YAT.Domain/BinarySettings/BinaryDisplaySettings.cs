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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\BinarySettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryDisplaySettings : MKY.Settings.SettingsItem, IEquatable<BinaryDisplaySettings>
	{
		/// <summary></summary>
		public static readonly bool ChunkLineBreakEnabledDefault = true;

		/// <summary></summary>
		public static readonly LengthSettingTuple LengthLineBreakDefault = new LengthSettingTuple(false, 16);

		/// <summary></summary>
		public static readonly BinarySequenceSettingTuple SequenceLineBreakBeforeDefault = new BinarySequenceSettingTuple(false, @"ABC");

		/// <summary></summary>
		public static readonly BinarySequenceSettingTuple SequenceLineBreakAfterDefault = new BinarySequenceSettingTuple(false, @"\h(00)");

		/// <summary></summary>
		public static readonly TimeoutSettingTuple TimedLineBreakDefault = new TimeoutSettingTuple(false, 500);

		private bool                       chunkLineBreakEnabled;
		private LengthSettingTuple         lengthLineBreak;
		private BinarySequenceSettingTuple sequenceLineBreakBefore;
		private BinarySequenceSettingTuple sequenceLineBreakAfter;
		private TimeoutSettingTuple        timedLineBreak;

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
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public BinaryDisplaySettings(BinaryDisplaySettings rhs)
			: base(rhs)
		{
			ChunkLineBreakEnabled   = rhs.ChunkLineBreakEnabled;
			LengthLineBreak         = rhs.LengthLineBreak;
			SequenceLineBreakBefore = rhs.SequenceLineBreakBefore;
			SequenceLineBreakAfter  = rhs.SequenceLineBreakAfter;
			TimedLineBreak          = rhs.TimedLineBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ChunkLineBreakEnabled   = ChunkLineBreakEnabledDefault;
			LengthLineBreak         = LengthLineBreakDefault;
			SequenceLineBreakBefore = SequenceLineBreakBeforeDefault;
			SequenceLineBreakAfter  = SequenceLineBreakAfterDefault;
			TimedLineBreak          = TimedLineBreakDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("ChunkLineBreakEnabled")]
		public bool ChunkLineBreakEnabled
		{
			get { return (this.chunkLineBreakEnabled); }
			set
			{
				if (this.chunkLineBreakEnabled != value)
				{
					this.chunkLineBreakEnabled = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LengthLineBreak")]
		public LengthSettingTuple LengthLineBreak
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
		public BinarySequenceSettingTuple SequenceLineBreakBefore
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
		public BinarySequenceSettingTuple SequenceLineBreakAfter
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
		public TimeoutSettingTuple TimedLineBreak
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

				hashCode = (hashCode * 397) ^ ChunkLineBreakEnabled  .GetHashCode();
				hashCode = (hashCode * 397) ^ LengthLineBreak        .GetHashCode();
				hashCode = (hashCode * 397) ^ SequenceLineBreakBefore.GetHashCode();
				hashCode = (hashCode * 397) ^ SequenceLineBreakAfter .GetHashCode();
				hashCode = (hashCode * 397) ^ TimedLineBreak         .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as BinaryDisplaySettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(BinaryDisplaySettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ChunkLineBreakEnabled  .Equals(other.ChunkLineBreakEnabled)   &&
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

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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
