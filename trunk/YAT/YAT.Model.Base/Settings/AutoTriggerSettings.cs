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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class AutoTriggerSettings : MKY.Settings.SettingsItem, IEquatable<AutoTriggerSettings>
	{
		private AutoTriggerEx trigger;
		private AutoTriggerOptions triggerOptions;

		/// <summary></summary>
		public AutoTriggerSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public AutoTriggerSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public AutoTriggerSettings(AutoTriggerSettings rhs)
			: base(rhs)
		{
			Trigger        = rhs.Trigger;
			TriggerOptions = rhs.TriggerOptions;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Trigger        = (AutoTriggerEx)AutoTrigger.None;
			TriggerOptions = new AutoTriggerOptions(false, true, false, false);
		}                                                   // Same as .NET regex, which are "case-sensitive by default".
		                                                    // Also same as byte sequence based triggers, which are case-sensitive by nature.
		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
		/// Must be string because of <see cref="AutoTrigger.Explicit"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Trigger")]
		public string Trigger_ForSerialization
		{
			get { return (Trigger); }
			set
			{
				AutoTriggerEx result;
				if (AutoTriggerEx.TryParse(value, out result))
					Trigger = result;
				else
					Trigger = new AutoTriggerEx(); // Silently reset to default, in order to prevent exceptions on changed strings.
			}                                      // Not ideal, but considered good enough. Could be refined by 'intelligent' fallback.
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsActive
		{
			get { return (Trigger.IsActive); }
		}

		/// <summary></summary>
		[XmlElement("TriggerOptions")]
		public virtual AutoTriggerOptions TriggerOptions
		{
			get { return (this.triggerOptions); }
			set
			{
				if (this.triggerOptions != value)
				{
					this.triggerOptions = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>Convenience property, same as <code>!<see cref="IsTextTriggered"/></code>.</remarks>
		[XmlIgnore]
		public virtual bool IsByteSequenceTriggered
		{
			get { return (!IsTextTriggered); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsTextTriggered
		{
			get { return (Trigger.TextIsSupported && TriggerOptions.UseText); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Resets the automatic Action, i.e. trigger and Action are reset to 'None'.
		/// </summary>
		public virtual void Deactivate()
		{
			SuspendChangeEvent();
			try
			{
				Trigger = (AutoTriggerEx)AutoTrigger.None;
			}
			finally
			{
				ResumeChangeEvent(true); // Force event.
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

				hashCode = (hashCode * 397) ^ (Trigger_ForSerialization != null ? Trigger_ForSerialization.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^                                     TriggerOptions          .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as AutoTriggerSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(AutoTriggerSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(Trigger_ForSerialization, other.Trigger_ForSerialization) &&
				TriggerOptions.Equals(                                     other.TriggerOptions)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoTriggerSettings lhs, AutoTriggerSettings rhs)
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
		public static bool operator !=(AutoTriggerSettings lhs, AutoTriggerSettings rhs)
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
