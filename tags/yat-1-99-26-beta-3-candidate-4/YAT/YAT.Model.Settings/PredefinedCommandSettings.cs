//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandSettings : MKY.Settings.Settings
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
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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
			Pages = new PredefinedCommandPageCollection();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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
				(this.pages == other.pages)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^
				this.pages.GetHashCode()
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
