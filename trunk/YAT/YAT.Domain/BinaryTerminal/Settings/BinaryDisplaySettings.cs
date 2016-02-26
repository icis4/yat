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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class BinaryDisplaySettings : MKY.Settings.SettingsItem
	{
		private BinaryLengthLineBreak   lengthLineBreak;
		private BinarySequenceLineBreak sequenceLineBreakBefore;
		private BinarySequenceLineBreak sequenceLineBreakAfter;
		private BinaryTimedLineBreak    timedLineBreak;

		/// <summary></summary>
		public BinaryDisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
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

			LengthLineBreak         = new BinaryLengthLineBreak(false, 16);
			SequenceLineBreakBefore = new BinarySequenceLineBreak(false, @"ABC");
			SequenceLineBreakAfter  = new BinarySequenceLineBreak(false, @"\h(00)");
			TimedLineBreak          = new BinaryTimedLineBreak(false, 500);
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

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

			BinaryDisplaySettings other = (BinaryDisplaySettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(LengthLineBreak         == other.LengthLineBreak) &&
				(SequenceLineBreakBefore == other.SequenceLineBreakBefore) &&
				(SequenceLineBreakAfter  == other.SequenceLineBreakAfter) &&
				(TimedLineBreak          == other.TimedLineBreak)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				LengthLineBreak        .GetHashCode() ^
				SequenceLineBreakBefore.GetHashCode() ^
				SequenceLineBreakAfter .GetHashCode() ^
				TimedLineBreak         .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
