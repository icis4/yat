//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryDisplaySettings : MKY.Utilities.Settings.Settings, IEquatable<BinaryDisplaySettings>
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
		public BinaryDisplaySettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			this.lengthLineBreak   = rhs.LengthLineBreak;
			this.sequenceLineBreak = rhs.SequenceLineBreak;
			this.timedLineBreak    = rhs.TimedLineBreak;
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
			if (obj == null)
				return (false);

			BinaryDisplaySettings casted = obj as BinaryDisplaySettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(BinaryDisplaySettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.

				(this.lengthLineBreak   == casted.lengthLineBreak) &&
				(this.sequenceLineBreak == casted.sequenceLineBreak) &&
				(this.timedLineBreak    == casted.timedLineBreak)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryDisplaySettings lhs, BinaryDisplaySettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
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
