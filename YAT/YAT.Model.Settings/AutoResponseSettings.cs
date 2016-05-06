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
		private Command dedicatedTrigger;
		private AutoResponse responseSelection;
		private Command dedicatedResponse;

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
			DedicatedTrigger  = new Command(rhs.DedicatedTrigger);
			ResponseSelection = rhs.ResponseSelection;
			DedicatedResponse = new Command(rhs.DedicatedResponse);
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
			DedicatedTrigger  = null;
			ResponseSelection = AutoResponse.None;
			DedicatedResponse = null;
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
		[XmlElement("DedicatedTrigger")]
		public Command DedicatedTrigger
		{
			get { return (this.dedicatedTrigger); }
			set
			{
				if (this.dedicatedTrigger != value)
				{
					this.dedicatedTrigger = value;
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
		[XmlElement("DedicatedResponse")]
		public Command DedicatedResponse
		{
			get { return (this.dedicatedResponse); }
			set
			{
				if (this.dedicatedResponse != value)
				{
					this.dedicatedResponse = value;
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

			if (trigger == Trigger.DedicatedCommand)
				DedicatedTrigger = trigger.DedicatedCommand;
			else
				DedicatedTrigger = null;

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromDedicatedTriggerText(string triggerText)
		{
			SuspendChangeEvent();

			TriggerSelection = Trigger.DedicatedCommand;
			DedicatedTrigger = new Command(triggerText);

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromResponse(AutoResponseEx response)
		{
			SuspendChangeEvent();

			ResponseSelection = response;

			if (response == AutoResponse.DedicatedCommand)
				DedicatedResponse = response.DedicatedCommand;
			else
				DedicatedResponse = null;

			UpdateEnabled();

			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void FromDedicatedResponseText(string responseText)
		{
			SuspendChangeEvent();

			ResponseSelection = AutoResponse.DedicatedCommand;
			DedicatedResponse = new Command(responseText);

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
				(DedicatedTrigger  == other.DedicatedTrigger) &&
				(ResponseSelection == other.ResponseSelection) &&
				(DedicatedResponse == other.DedicatedResponse)
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
			int dedicatedTriggerHashCode = 0;
			if (DedicatedTrigger != null) // Command may be 'null'!
				dedicatedTriggerHashCode = DedicatedTrigger.GetHashCode();

			int dedicatedResponseHashCode = 0;
			if (DedicatedResponse != null) // Command may be 'null'!
				dedicatedResponseHashCode = DedicatedResponse.GetHashCode();

			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				Visible          .GetHashCode() ^
				Enabled          .GetHashCode() ^
				TriggerSelection .GetHashCode() ^
				dedicatedTriggerHashCode        ^
				ResponseSelection.GetHashCode() ^
				dedicatedResponseHashCode
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
