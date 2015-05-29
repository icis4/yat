//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1 Version 1.99.32
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class EolCommentSettings : MKY.Settings.SettingsItem
	{
		private bool skipComment;
		private bool skipWhiteSpace;
		private List<string> indicators;

		/// <summary></summary>
		public EolCommentSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public EolCommentSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public EolCommentSettings(EolCommentSettings rhs)
			: base(rhs)
		{
			SkipComment    = rhs.SkipComment;
			SkipWhiteSpace = rhs.SkipWhiteSpace;
			Indicators     = new List<string>(rhs.Indicators);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SkipComment    = false;
			SkipWhiteSpace = true;
			Indicators     = new List<string>();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SkipComment")]
		public virtual bool SkipComment
		{
			get { return (this.skipComment); }
			set
			{
				if (this.skipComment != value)
				{
					this.skipComment = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SkipWhiteSpace")]
		public virtual bool SkipWhiteSpace
		{
			get { return (this.skipWhiteSpace); }
			set
			{
				if (this.skipWhiteSpace != value)
				{
					this.skipWhiteSpace = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Indicators")]
		public virtual List<string> Indicators
		{
			get { return (this.indicators); }
			set
			{
				if (this.indicators != value)
				{
					this.indicators = value;
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

			EolCommentSettings other = (EolCommentSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(SkipComment    == other.SkipComment) &&
				(SkipWhiteSpace == other.SkipWhiteSpace) &&
				(Indicators     == other.Indicators)
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

				SkipComment   .GetHashCode() ^
				SkipWhiteSpace.GetHashCode() ^
				Indicators    .GetHashCode()
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
