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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	public class AutoActionSettings : AutoTriggerSettings, IEquatable<AutoActionSettings>
	{
		private AutoActionEx action;

		/// <summary></summary>
		public AutoActionSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public AutoActionSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public AutoActionSettings(AutoActionSettings rhs)
			: base(rhs)
		{
			Action = rhs.Action;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Action = AutoAction.None;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// This 'EnumEx' cannot be serialized, thus, the helper below is used for serialization.
		/// Still, this settings object shall provide an 'EnumEx' for full control of the setting.
		/// </remarks>
		[XmlIgnore]
		public AutoActionEx Action
		{
			get { return (this.action); }
			set
			{
				if (this.action != value)
				{
					this.action = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Must be string because an 'EnumEx' cannot be serialized.
		/// Is a string rather than enum same as for <see cref="AutoTriggerSettings.Trigger_ForSerialization"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Action")]
		public string Action_ForSerialization
		{
			get { return (Action); }
			set
			{
				AutoActionEx result;
				if (AutoActionEx.TryParse(value, out result))
					Action = result;
				else
					Action = AutoActionEx.Default; // Silently reset to default, in order to prevent exceptions on changed strings.
			}                                      // Not ideal, but considered good enough. Could be refined by 'intelligent' fallback.
		}

		/// <summary></summary>
		[XmlIgnore]
		public override bool IsActive
		{
			get { return (base.IsActive && Action.IsActive); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool ShallHighlight
		{
			get { return (Action.ShallHighlight); }
		}

		/// <remarks>"FilterOrSuppress" also means "IsReloadable".</remarks>
		[XmlIgnore]
		public virtual bool IsFilterOrSuppress
		{
			get { return ((Action == AutoAction.Filter) || (Action == AutoAction.Suppress)); }
		}

		/// <remarks>"FilterOrSuppress" also means "IsReloadable".</remarks>
		[XmlIgnore]
		public virtual bool IsActiveAsFilterOrSuppress
		{
			get { return (IsActive && IsFilterOrSuppress); }
		}

		/// <remarks>"NeitherFilterNorSuppress" also means "IsNotReloadable".</remarks>
		[XmlIgnore]
		public virtual bool IsNeitherFilterNorSuppress
		{
			get { return ((Action != AutoAction.Filter) && (Action != AutoAction.Suppress)); }
		}

		/// <remarks>"NeitherFilterNorSuppress" also means "IsNotReloadable".</remarks>
		[XmlIgnore]
		public virtual bool IsActiveAsNeitherFilterNorSuppress
		{
			get { return (IsActive && IsNeitherFilterNorSuppress); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsCountRatePlot
		{
			get { return (Action.IsCountRatePlot); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsNotCountRatePlot
		{
			get { return (!Action.IsCountRatePlot); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Resets the automatic action, i.e. reset to <see cref="AutoAction.None"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="AutoTriggerSettings.Trigger"/>
		/// and <see cref="AutoTriggerSettings.TriggerOptions"/>
		/// are not reset. It makes no sense the user has to set the trigger again
		/// and it makes no sense to reset more items than needed.
		/// </remarks>
		public override void Deactivate()
		{
			SuspendChangeEvent();
			try
			{
			////base.Deactivate() shall not be called, trigger shall remain.

				Action = AutoAction.None;
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
				int hashCode = base.GetHashCode(); // Get hash code of base including all settings nodes.

				hashCode = (hashCode * 397) ^ (Action_ForSerialization != null ? Action_ForSerialization.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as AutoActionSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(AutoActionSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare base including all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(Action_ForSerialization, other.Action_ForSerialization)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoActionSettings lhs, AutoActionSettings rhs)
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
		public static bool operator !=(AutoActionSettings lhs, AutoActionSettings rhs)
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
