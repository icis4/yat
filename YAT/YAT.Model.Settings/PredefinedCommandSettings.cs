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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int MaxCommandsPerPage = 12;

		private PredefinedCommandPageCollection pages;

		/// <summary></summary>
		public PredefinedCommandSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public PredefinedCommandSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public PredefinedCommandSettings(PredefinedCommandSettings rhs)
			: base(rhs)
		{
			Pages = new PredefinedCommandPageCollection(rhs.Pages);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			Pages = new PredefinedCommandPageCollection();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Pages")]
		public PredefinedCommandPageCollection Pages
		{
			get { return (this.pages); }
			set
			{
				if (this.pages != value)
				{
					this.pages = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual void CreateDefaultPage()
		{
			this.pages = new PredefinedCommandPageCollection();
			this.pages.Add(new PredefinedCommandPage("Page 1"));
			SetChanged();
		}

		/// <summary>
		/// Sets the given predefined command.
		/// </summary>
		/// <param name="pageIndex">Page index 0..max-1.</param>
		/// <param name="commandIndex">Command index 0..max-1.</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int pageIndex, int commandIndex, Command command)
		{
			if ((pageIndex == 0) && (this.pages.Count == 0))
				CreateDefaultPage();

			if ((pageIndex >= 0) && (pageIndex < this.pages.Count))
			{
				PredefinedCommandPage page = this.pages[pageIndex];
				if ((commandIndex >= 0) && (commandIndex < MaxCommandsPerPage))
				{
					page.SetCommand(commandIndex, command);
					SetChanged();
				}
			}
		}

		/// <summary>
		/// Gets the given predefined command.
		/// </summary>
		/// <param name="pageIndex">Page index 0..max-1.</param>
		/// <param name="commandIndex">Command index 0..max-1.</param>
		public virtual Command GetCommand(int pageIndex, int commandIndex)
		{
			if (ValidateWhetherCommandIsDefined(pageIndex, commandIndex))
				return (this.pages[pageIndex].Commands[commandIndex]);
			else
				return (null);
		}

		/// <summary>
		/// Validates the given predefined arguments.
		/// </summary>
		/// <param name="pageIndex">Page index 0..max-1.</param>
		/// <param name="commandIndex">Command index 0..max-1.</param>
		public bool ValidateWhetherCommandIsDefined(int pageIndex, int commandIndex)
		{
			// Validate page index:
			if ((pageIndex < 0) || (pageIndex > (this.pages.Count - 1)))
				return (false);

			// Validate page:
			List<Command> commands = this.pages[pageIndex].Commands;
			if (commands == null)
				return (false);

			// Validate command index:
			if ((commandIndex < 0) || (commandIndex > (commands.Count - 1)))
				return (false);

			// Validate command:
			Command command = commands[commandIndex];
			if (command == null)
				return (false);

			return (command.IsDefined);
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

			PredefinedCommandSettings other = (PredefinedCommandSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(Pages == other.Pages)
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

				hashCode = (hashCode * 397) ^ Pages.GetHashCode();

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
