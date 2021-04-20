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
using System.Xml.Serialization;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\TextSettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <remarks>
	/// Named 'SendSettingsFile' instead of 'SendFileSettings' to prevent XML invalid operation
	/// exception "type 'A.X' and 'B.X' use the XML type name 'X' in namespace" when reflecting the
	/// settings.
	/// </remarks>
	[Serializable]
	public class SendSettingsFile : MKY.Settings.SettingsItem, IEquatable<SendSettingsFile>
	{
		/// <summary></summary>
		public const bool SkipEmptyLinesDefault = false;

		/// <summary></summary>
		public const bool EnableEscapesDefault = false;

		private bool skipEmptyLines;
		private bool enableEscapes;

		/// <summary></summary>
		public SendSettingsFile()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SendSettingsFile(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendSettingsFile(SendSettingsFile rhs)
			: base(rhs)
		{
			SkipEmptyLines = rhs.SkipEmptyLines;
			EnableEscapes  = rhs.enableEscapes;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SkipEmptyLines = SkipEmptyLinesDefault;
			EnableEscapes  = EnableEscapesDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SkipEmptyLines")]
		public virtual bool SkipEmptyLines
		{
			get { return (this.skipEmptyLines); }
			set
			{
				if (this.skipEmptyLines != value)
				{
					this.skipEmptyLines = value;
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

				hashCode = (hashCode * 397) ^ SkipEmptyLines.GetHashCode();
				hashCode = (hashCode * 397) ^ EnableEscapes .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SendSettingsFile));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendSettingsFile other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				SkipEmptyLines.Equals(other.SkipEmptyLines) &&
				EnableEscapes .Equals(other.EnableEscapes)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SendSettingsFile lhs, SendSettingsFile rhs)
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
		public static bool operator !=(SendSettingsFile lhs, SendSettingsFile rhs)
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
