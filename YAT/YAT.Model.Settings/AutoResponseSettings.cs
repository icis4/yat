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

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class AutoResponseSettings : MKY.Settings.SettingsItem
	{
		private bool visible;
		private bool enabled;
		private Trigger triggerSelection;
		private Command explicitTrigger;
		private AutoResponse responseSelection;
		private Command explicitResponse;

		/// <summary></summary>
		public AutoResponseSettings()
		{
			SetMyDefaults();
			ClearChanged();
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
			Visible           = rhs.Visible;
			Enabled           = rhs.Enabled;
			TriggerSelection  = rhs.TriggerSelection;
			ExplicitTrigger   = new Command(rhs.ExplicitTrigger);
			ResponseSelection = rhs.ResponseSelection;
			ExplicitResponse  = new Command(rhs.ExplicitResponse);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Visible           = false;
			Enabled           = false;
			TriggerSelection  = Trigger.None;
			ExplicitTrigger   = null;
			ResponseSelection = AutoResponse.None;
			ExplicitResponse  = null;
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
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled
		{
			get { return (this.enabled); }
			set
			{
				if (this.enabled != value)
				{
					this.enabled = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TriggerSelection")]
		public Trigger TriggerSelection
		{
			get { return (this.triggerSelection); }
			set
			{
				if (this.triggerSelection != value)
				{
					this.triggerSelection = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ExplicitTrigger")]
		public Command ExplicitTrigger
		{
			get { return (this.explicitTrigger); }
			set
			{
				if (this.explicitTrigger != value)
				{
					this.explicitTrigger = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ResponseSelection")]
		public AutoResponse ResponseSelection
		{
			get { return (this.responseSelection); }
			set
			{
				if (this.responseSelection != value)
				{
					this.responseSelection = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ExplicitResponse")]
		public Command ExplicitResponse
		{
			get { return (this.explicitResponse); }
			set
			{
				if (this.explicitResponse != value)
				{
					this.explicitResponse = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		private void UpdateEnabled()
		{
			Enabled = ((TriggerSelection != Trigger.None) && (ResponseSelection != AutoResponse.None));
		}

		/// <summary></summary>
		public virtual void FromTrigger(TriggerEx trigger)
		{
			SuspendChangeEvent();

			TriggerSelection = trigger;

			if (trigger == Trigger.Explicit)
				ExplicitTrigger = trigger.Explicit;
			else
				ExplicitTrigger = null;

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromExplicitTriggerText(string triggerText)
		{
			SuspendChangeEvent();

			TriggerSelection = Trigger.Explicit;
			ExplicitTrigger = new Command(triggerText);

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromResponse(AutoResponseEx response)
		{
			SuspendChangeEvent();

			ResponseSelection = response;

			if (response == AutoResponse.Explicit)
				ExplicitResponse = response.Explicit;
			else
				ExplicitResponse = null;

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromExplicitResponseText(string responseText)
		{
			SuspendChangeEvent();

			ResponseSelection = AutoResponse.Explicit;
			ExplicitResponse = new Command(responseText);

			UpdateEnabled();

			ResumeChangeEvent(true);
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

				(Visible           == other.Visible) &&
				(Enabled           == other.Enabled) &&
				(TriggerSelection  == other.TriggerSelection) &&
				(ExplicitTrigger   == other.ExplicitTrigger) &&
				(ResponseSelection == other.ResponseSelection) &&
				(ExplicitResponse  == other.ExplicitResponse)
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

				hashCode = (hashCode * 397) ^  Visible                                   .GetHashCode();
				hashCode = (hashCode * 397) ^  Enabled                                   .GetHashCode();
				hashCode = (hashCode * 397) ^  TriggerSelection                          .GetHashCode();
				hashCode = (hashCode * 397) ^ (ExplicitTrigger != null ? ExplicitTrigger .GetHashCode() : 0); // Command may be 'null'!
				hashCode = (hashCode * 397) ^  ResponseSelection                         .GetHashCode();
				hashCode = (hashCode * 397) ^ (ExplicitResponse!= null ? ExplicitResponse.GetHashCode() : 0); // Command may be 'null'!

				return (hashCode);
			}
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
