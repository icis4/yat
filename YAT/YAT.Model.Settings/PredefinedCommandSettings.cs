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
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
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
			: base()
		{
			SetMyDefaults();
			ClearChanged();
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
				if (value != this.pages)
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

		/// <summary></summary>
		/// <param name="selectedPage">Page index 0..max-1.</param>
		/// <param name="selectedCommand">Command index 0..max-1.</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int selectedPage, int selectedCommand, Command command)
		{
			if ((selectedPage == 0) && (this.pages.Count == 0))
				CreateDefaultPage();

			if ((selectedPage >= 0) && (selectedPage < this.pages.Count))
			{
				PredefinedCommandPage page = this.pages[selectedPage];
				if ((selectedCommand >= 0) && (selectedCommand < MaxCommandsPerPage))
				{
					page.SetCommand(selectedCommand, command);
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
			return
			(
				base.GetHashCode() ^

				Pages.GetHashCode()
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
