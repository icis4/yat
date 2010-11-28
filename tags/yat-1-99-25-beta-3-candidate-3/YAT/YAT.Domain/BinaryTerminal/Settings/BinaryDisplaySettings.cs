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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
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
	public class BinaryDisplaySettings : MKY.Settings.Settings
	{
		private BinaryLengthLineBreak lengthLineBreak;
		private BinarySequenceLineBreak sequenceLineBreak;
		private BinaryTimedLineBreak timedLineBreak;

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

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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

				(this.lengthLineBreak   == other.lengthLineBreak) &&
				(this.sequenceLineBreak == other.sequenceLineBreak) &&
				(this.timedLineBreak    == other.timedLineBreak)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.lengthLineBreak  .GetHashCode() ^
				this.sequenceLineBreak.GetHashCode() ^
				this.timedLineBreak   .GetHashCode()
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
