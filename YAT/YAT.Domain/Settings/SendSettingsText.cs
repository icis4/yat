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
// YAT Version 2.2.0 Development
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
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class SendSettingsText : MKY.Settings.SettingsItem, IEquatable<SendSettingsText>
	{
		/// <summary></summary>
		public const bool KeepSendTextDefault = true;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		/// <summary></summary>
		public const bool EnableEscapesDefault = true;

		private bool keepSendText;
		private bool sendImmediately;
		private bool enableEscapes;

		/// <summary></summary>
		public SendSettingsText()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SendSettingsText(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendSettingsText(SendSettingsText rhs)
			: base(rhs)
		{
			KeepSendText    = rhs.KeepSendText;
			SendImmediately = rhs.SendImmediately;
			EnableEscapes   = rhs.enableEscapes;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			KeepSendText    = KeepSendTextDefault;
			SendImmediately = SendImmediatelyDefault;
			EnableEscapes   = EnableEscapesDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("KeepSendText")]
		public virtual bool KeepSendText
		{
			get { return (this.keepSendText); }
			set
			{
				if (this.keepSendText != value)
				{
					this.keepSendText = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendImmediately")]
		public virtual bool SendImmediately
		{
			get { return (this.sendImmediately); }
			set
			{
				if (this.sendImmediately != value)
				{
					this.sendImmediately = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("EnableEscapes")]
		public virtual bool EnableEscapes
		{
			get { return (this.enableEscapes); }
			set
			{
				if (this.enableEscapes != value)
				{
					this.enableEscapes = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual Parser.Mode ToParseMode()
		{
			if (EnableEscapes)
				return (Parser.Mode.AllEscapes);
			else
				return (Parser.Mode.NoEscapes);
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

				hashCode = (hashCode * 397) ^ KeepSendText   .GetHashCode();
				hashCode = (hashCode * 397) ^ SendImmediately.GetHashCode();
				hashCode = (hashCode * 397) ^ EnableEscapes  .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SendSettingsText));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendSettingsText other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				KeepSendText   .Equals(other.KeepSendText)    &&
				SendImmediately.Equals(other.SendImmediately) &&
				EnableEscapes  .Equals(other.EnableEscapes)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SendSettingsText lhs, SendSettingsText rhs)
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
		public static bool operator !=(SendSettingsText lhs, SendSettingsText rhs)
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
