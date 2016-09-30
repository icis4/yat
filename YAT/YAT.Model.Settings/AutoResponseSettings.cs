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

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class AutoResponseSettings : MKY.Settings.SettingsItem
	{
		private bool visible;
		private AutoTriggerEx trigger;
		private AutoResponseEx response;

		/// <summary></summary>
		public AutoResponseSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public AutoResponseSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public AutoResponseSettings(AutoResponseSettings rhs)
			: base(rhs)
		{
			Visible  = rhs.Visible;
			Trigger  = rhs.Trigger;
			Response = rhs.Response;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Visible  = false;
			Trigger  = AutoTrigger.None;
			Response = AutoResponse.None;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Visible")]
		public bool Visible
		{
			get { return (this.visible); }
			set
			{
				if (this.visible != value)
				{
					this.visible = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public AutoTriggerEx Trigger
		{
			get { return (this.trigger); }
			set
			{
				if (this.trigger != value)
				{
					this.trigger = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Trigger")]
		public virtual string Trigger_ForSerialization
		{
			get { return (Trigger); }
			set { Trigger = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool TriggerIsActive
		{
			get { return (Trigger != AutoTrigger.None); }
		}

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public AutoResponseEx Response
		{
			get { return (this.response); }
			set
			{
				if (this.response != value)
				{
					this.response = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Response")]
		public virtual string Response_ForSerialization
		{
			get { return (Response); }
			set { Response = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool ResponseIsActive
		{
			get { return (Response != AutoResponse.None); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsActive
		{
			get { return (TriggerIsActive && ResponseIsActive); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Resets the automatic response, i.e. trigger and response are reset to 'None'.
		/// </summary>
		public void Deactivate()
		{
			SuspendChangeEvent();

			Trigger  = AutoTrigger.None;
			Response = AutoResponse.None;

			ResumeChangeEvent(true); // Force event.
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

			AutoResponseSettings other = (AutoResponseSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(Visible == other.Visible) &&

				StringEx.EqualsOrdinalIgnoreCase(Trigger_ForSerialization,  other.Trigger_ForSerialization) &&
				StringEx.EqualsOrdinalIgnoreCase(Response_ForSerialization, other.Response_ForSerialization)
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ Visible.GetHashCode();

				hashCode = (hashCode * 397) ^ (Trigger_ForSerialization  != null ? Trigger_ForSerialization .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Response_ForSerialization != null ? Response_ForSerialization.GetHashCode() : 0);

				return (hashCode);
			}
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityAnalysis for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
