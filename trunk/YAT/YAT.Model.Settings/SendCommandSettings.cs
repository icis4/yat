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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Recent;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendCommandSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int MaxRecentCommands = 24;

		private Command command;
		private RecentItemCollection<Command> recentCommands;

		/// <summary></summary>
		public SendCommandSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SendCommandSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendCommandSettings(SendCommandSettings rhs)
			: base(rhs)
		{
			Command = new Command(rhs.Command);
			RecentCommands = new RecentItemCollection<Command>(rhs.RecentCommands);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
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
				if (value != this.command)
				{
					this.command = value;
					SetChanged();
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
				if (value != this.recentCommands)
				{
					this.recentCommands = value;
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

			SendCommandSettings other = (SendCommandSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(Command        == other.Command) &&
				(RecentCommands == other.RecentCommands)
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

				Command       .GetHashCode() ^
				RecentCommands.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion

		#region Comparision
		//------------------------------------------------------------------------------------------
		// Comparision ;-)
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "This is the easter egg!")]
		private const string EasterEggCommand = @"\easteregg";

		/// <summary></summary>
		public static bool IsEasterEggCommand(string command)
		{
			return (StringEx.EqualsOrdinalIgnoreCase(command, EasterEggCommand));
		}

		/// <summary></summary>
		public static string EasterEggCommandText
		{
			get
			{
				return (":-)");
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
