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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using MKY.Collections;
using MKY.Diagnostics;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\TextSettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TextExclusionSettings : MKY.Settings.SettingsItem, IEquatable<TextExclusionSettings>
	{
		private bool enabled; // = false;
		private List<string> patterns; // = null;

		private int regexesUpdatePatternsHashCode; // = 0;
		private ReadOnlyCollection<Regex> regexes; // = null;

		/// <summary></summary>
		public TextExclusionSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public TextExclusionSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TextExclusionSettings(TextExclusionSettings rhs)
			: base(rhs)
		{
			Enabled  = rhs.Enabled;
			Patterns = new List<string>(rhs.Patterns);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Enabled  = false;
			Patterns = new List<string>();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Enabled")]
		public virtual bool Enabled
		{
			get { return (this.enabled); }
			set
			{
				if (this.enabled != value)
				{
					this.enabled = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Patterns")]
		public virtual List<string> Patterns
		{
			get { return (this.patterns); }
			set
			{
				if (this.patterns != value)
				{
					this.patterns = value; // Attention: The update below only works when the whole collection
					UpdateRegexes();       //            gets replaced, but not if items are added or removed
					SetMyChanged();        //            by Add()/Remove()/Clear()! See below for workaround.
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Regexes", Justification = "What else is the plural of 'Regex'?")]
		protected virtual void UpdateRegexes()
		{
			if (this.patterns != null)
			{
				var l = new List<Regex>(this.patterns.Count); // Preset the required capacity to improve memory management.

				foreach (var pattern in this.patterns)
				{
					try
					{
						l.Add(new Regex(pattern));
					}
					catch (ArgumentException ex)
					{
						TraceEx.WriteException(this.GetType(), ex, string.Format(CultureInfo.CurrentCulture, @"Failed to create regex object for pattern ""{0}""!", pattern));
					}
				}

				this.regexesUpdatePatternsHashCode = IEnumerableEx.ItemsToHashCode(this.patterns); // Workaround for issue described in Patterns{set} above.
				this.regexes = new ReadOnlyCollection<Regex>(l);                                      // Not a 100% solution but close enough to such.
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Regexes", Justification = "What else is the plural of 'Regex'?")]
		[XmlIgnore]
		public virtual ReadOnlyCollection<Regex> Regexes
		{
			get
			{
				if (this.regexesUpdatePatternsHashCode != IEnumerableEx.ItemsToHashCode(this.patterns)) // Workaround for issue described in Patterns{set} above.
					UpdateRegexes();                                                                       // Not a 100% solution but close enough to such.

				return (this.regexes);
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

				hashCode = (hashCode * 397) ^ Enabled .GetHashCode();
				hashCode = (hashCode * 397) ^ Patterns.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TextExclusionSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextExclusionSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				Enabled           .Equals(         other.Enabled) &&
				IEnumerableEx.ItemsEqual(Patterns, other.Patterns)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextExclusionSettings lhs, TextExclusionSettings rhs)
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
		public static bool operator !=(TextExclusionSettings lhs, TextExclusionSettings rhs)
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
