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
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

using MKY.Recent;

using YAT.Model.Types;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendCommandSettings : MKY.Settings.Settings
	{
		/// <summary></summary>
		public const int MaxRecentCommands = 24;

		private Command command;
		private RecentItemCollection<Command> recentsCommands;

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
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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
		[XmlElement("RecentCommands")]
		public RecentItemCollection<Command> RecentCommands
		{
			get { return (this.recentsCommands); }
			set
			{
				if (value != this.recentsCommands)
				{
					this.recentsCommands = value;
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

			SendCommandSettings other = (SendCommandSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.command         == other.command) &&
				(this.recentsCommands == other.recentsCommands)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.command        .GetHashCode() ^
				this.recentsCommands.GetHashCode()
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
			return (string.Compare(command, EasterEggCommand, StringComparison.OrdinalIgnoreCase) == 0);
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
