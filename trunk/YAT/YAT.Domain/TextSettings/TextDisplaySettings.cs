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
// Copyright © 2003-2020 Matthias Kläy.
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
	public class TextDisplaySettings : MKY.Settings.SettingsItem, IEquatable<TextDisplaySettings>
	{
		/// <summary></summary>
		public static readonly bool ChunkLineBreakEnabledDefault = false;

		/// <summary></summary>
		public const int LengthLineBreakLengthDefault = 80;

		/// <summary></summary>
		public static readonly LengthSettingTuple LengthLineBreakDefault = new LengthSettingTuple(false, LengthLineBreakLengthDefault);

		/// <summary></summary>
		public const int TimedLineBreakTimeoutDefault = 500;

		/// <summary></summary>
		public static readonly TimeoutSettingTuple TimedLineBreakDefault = new TimeoutSettingTuple(false, TimedLineBreakTimeoutDefault);

		private bool                chunkLineBreakEnabled;
		private LengthSettingTuple  lengthLineBreak;
		private TimeoutSettingTuple timedLineBreak;

		/// <summary></summary>
		public TextDisplaySettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public TextDisplaySettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TextDisplaySettings(TextDisplaySettings rhs)
			: base(rhs)
		{
			ChunkLineBreakEnabled = rhs.ChunkLineBreakEnabled;
			LengthLineBreak       = rhs.LengthLineBreak;
			TimedLineBreak        = rhs.TimedLineBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ChunkLineBreakEnabled = ChunkLineBreakEnabledDefault;
			LengthLineBreak       = LengthLineBreakDefault;
			TimedLineBreak        = TimedLineBreakDefault;
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

				hashCode = (hashCode * 397) ^ ChunkLineBreakEnabled.GetHashCode();
				hashCode = (hashCode * 397) ^ LengthLineBreak      .GetHashCode();
				hashCode = (hashCode * 397) ^ TimedLineBreak       .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TextDisplaySettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextDisplaySettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ChunkLineBreakEnabled.Equals(other.ChunkLineBreakEnabled)  &&
				LengthLineBreak      .Equals(other.LengthLineBreak)        &&
				TimedLineBreak       .Equals(other.TimedLineBreak)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextDisplaySettings lhs, TextDisplaySettings rhs)
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
		public static bool operator !=(TextDisplaySettings lhs, TextDisplaySettings rhs)
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
