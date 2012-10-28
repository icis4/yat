//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryDisplaySettings : MKY.Settings.SettingsItem
	{
		private BinaryLengthLineBreak   lengthLineBreak;
		private BinarySequenceLineBreak sequenceLineBreak;
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
			LengthLineBreak   = rhs.LengthLineBreak;
			SequenceLineBreak = rhs.SequenceLineBreak;
			TimedLineBreak    = rhs.TimedLineBreak;
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			LengthLineBreak   = new BinaryLengthLineBreak(false, 16);
			SequenceLineBreak = new BinarySequenceLineBreak(false, @"\h(00)");
			TimedLineBreak    = new BinaryTimedLineBreak(false, 500);
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
				if (value != this.lengthLineBreak)
				{
					this.lengthLineBreak = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SequenceLineBreak")]
		public BinarySequenceLineBreak SequenceLineBreak
		{
			get { return (this.sequenceLineBreak); }
			set
			{
				if (value != this.sequenceLineBreak)
				{
					this.sequenceLineBreak = value;
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
				if (value != this.timedLineBreak)
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

				(LengthLineBreak   == other.LengthLineBreak) &&
				(SequenceLineBreak == other.SequenceLineBreak) &&
				(TimedLineBreak    == other.TimedLineBreak)
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
				base.GetHashCode() ^

				LengthLineBreak  .GetHashCode() ^
				SequenceLineBreak.GetHashCode() ^
				TimedLineBreak   .GetHashCode()
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
