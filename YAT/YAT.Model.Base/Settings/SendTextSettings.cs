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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;
using MKY.Collections.Specialized;

using YAT.Model.Types;

#endregion

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "This is the easter egg!")]
	public class SendTextSettings : MKY.Settings.SettingsItem, IEquatable<SendTextSettings>
	{
		/// <summary></summary>
		public const int MaxRecentCommands = 24;

		private Command command;
		private RecentItemCollection<Command> recentCommands;

		/// <summary></summary>
		public SendTextSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SendTextSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendTextSettings(SendTextSettings rhs)
			: base(rhs)
		{
			Command = new Command(rhs.Command); // Clone to ensure decoupling.
			RecentCommands = new RecentItemCollection<Command>(rhs.RecentCommands);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Command = new Command();
			RecentCommands = new RecentItemCollection<Command>(MaxRecentCommands);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Command")]
		public Command Command
		{
			get { return (this.command); }
			set
			{
				if (this.command != value)
				{
					this.command = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("RecentCommands")]
		public RecentItemCollection<Command> RecentCommands
		{
			get { return (this.recentCommands); }
			set
			{
				if (this.recentCommands != value)
				{
					this.recentCommands = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Expands the multi-line text of a multi-line command into the corresponding single-line
		/// text, taking "\!(EOL)" as additional EOL separator.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		public virtual void ExpandMultiLineText()
		{
			if (Command != null)
			{
				if (Command.IsMultiLineText)
				{
					var singleLine = new StringBuilder();
					foreach (var line in Command.MultiLineText)
					{
						if (singleLine.Length > 0)
							singleLine.Append(@"\!(EOL)"); // \todo (2017-03-27 / MKY) could be retrieved from 'Domain.Parser' somehow.

						singleLine.Append(line);
					}

					var c = new Command(Command); // Clone to ensure decoupling.
					c.ClearDescription();
					c.SingleLineText = singleLine.ToString();
					Command = c;
				}
			}
		}

		/// <summary>
		/// Clears <see cref="Command"/>.
		/// </summary>
		public virtual void ClearCommand()
		{
			if (Command != null)
				Command = new Command(Command.DefaultRadix);
			else
				Command = new Command();
		}

		/// <summary>
		/// Clears <see cref="RecentCommands"/>.
		/// </summary>
		public virtual void ClearRecentCommands()
		{
			RecentCommands = new RecentItemCollection<Command>(MaxRecentCommands);
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

				hashCode = (hashCode * 397) ^ (Command        != null ? Command       .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RecentCommands != null ? RecentCommands.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SendTextSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendTextSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ObjectEx          .Equals(Command,        other.Command) &&
				IEnumerableEx.ItemsEqual( RecentCommands, other.RecentCommands)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SendTextSettings lhs, SendTextSettings rhs)
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
		public static bool operator !=(SendTextSettings lhs, SendTextSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Special ;-)
		//------------------------------------------------------------------------------------------
		// Special ;-)
		//------------------------------------------------------------------------------------------

		private const string EasterEggCommand = @"E/-\5+|三ⴽ ε99";

		/// <summary></summary>
		public static bool IsEasterEggCommand(string command)
		{
			return (StringEx.EqualsOrdinalIgnoreCase(command, EasterEggCommand));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
