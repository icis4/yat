//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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
	public class PredefinedCommandSettings : MKY.Utilities.Settings.Settings, IEquatable<PredefinedCommandSettings>
	{
		/// <summary></summary>
		public const int MaxCommandsPerPage = 12;

		private PredefinedCommandPageCollection _pages;

		/// <summary></summary>
		public PredefinedCommandSettings()
			: base()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public PredefinedCommandSettings(MKY.Utilities.Settings.SettingsType settingsType)
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
			get { return (_pages); }
			set
			{
				if (value != _pages)
				{
					_pages = value;
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
			_pages = new PredefinedCommandPageCollection();
			_pages.Add(new PredefinedCommandPage("Page 1"));
			SetChanged();
		}

		/// <summary></summary>
		/// <param name="selectedPage">Index 0..max-1.</param>
		/// <param name="selectedCommand">Index 0..max-1.</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int selectedPage, int selectedCommand, Command command)
		{
			if ((selectedPage == 0) && (_pages.Count == 0))
				CreateDefaultPage();

			if ((selectedPage >= 0) && (selectedPage < _pages.Count))
			{
				PredefinedCommandPage page = _pages[selectedPage];
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
			if (obj is PredefinedCommandSettings)
				return (Equals((PredefinedCommandSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PredefinedCommandSettings value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
				return (_pages.Equals(value._pages));

			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PredefinedCommandSettings lhs, PredefinedCommandSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(PredefinedCommandSettings lhs, PredefinedCommandSettings rhs)
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
