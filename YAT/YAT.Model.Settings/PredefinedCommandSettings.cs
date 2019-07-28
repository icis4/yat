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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY.Collections;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class PredefinedCommandSettings : MKY.Settings.SettingsItem, IEquatable<PredefinedCommandSettings>
	{
		/// <summary></summary>
		public const int FirstCommandPerPage = 1;

		/// <summary></summary>
		public const int MaxCommandsPerPage = 12;

		/// <summary></summary>
		public static readonly PredefinedCommandPage DefaultPage = new PredefinedCommandPage("Page 1");

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
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public PredefinedCommandSettings(PredefinedCommandSettings rhs)
			: base(rhs)
		{
			Pages = new PredefinedCommandPageCollection(rhs.Pages);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
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
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public int TotalDefinedCommandCount
		{
			get
			{
				int n = 0;

				foreach (var p in Pages)
					n += p.DefinedCommandCount;

				return (n);
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
			this.pages.Add(DefaultPage);
			SetMyChanged();
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
			var commands = this.pages[pageIndex].Commands;
			if (commands == null)
				return (false);

			// Validate command index:
			if ((commandIndex < 0) || (commandIndex > (commands.Count - 1)))
				return (false);

			// Validate command:
			var command = commands[commandIndex];
			if (command == null)
				return (false);

			return (command.IsDefined);
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
				var page = this.pages[pageIndex];
				if ((commandIndex >= 0) && (commandIndex < MaxCommandsPerPage))
				{
					page.SetCommand(commandIndex, command);
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// Clears the given predefined command.
		/// </summary>
		/// <param name="pageIndex">Page index 0..max-1.</param>
		/// <param name="commandIndex">Command index 0..max-1.</param>
		public virtual void ClearCommand(int pageIndex, int commandIndex)
		{
			if (ValidateWhetherCommandIsDefined(pageIndex, commandIndex))
			{
				this.pages[pageIndex].Commands[commandIndex].Clear();
				SetMyChanged();
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

				hashCode = (hashCode * 397) ^ Pages.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as PredefinedCommandSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(PredefinedCommandSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				IEnumerableEx.ItemsEqual(Pages, other.Pages)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PredefinedCommandSettings lhs, PredefinedCommandSettings rhs)
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
