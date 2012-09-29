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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool KeepCommandDefault = true;

		/// <summary></summary>
		public const bool CopyPredefinedDefault = false;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		/// <summary></summary>
		public const int DefaultDelayDefault = 100;

		private bool keepCommand;
		private bool copyPredefined;
		private bool sendImmediately;
		private int defaultDelay;

		/// <summary></summary>
		public SendSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SendSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendSettings(SendSettings rhs)
			: base(rhs)
		{
			KeepCommand     = rhs.keepCommand;
			CopyPredefined  = rhs.copyPredefined;
			SendImmediately = rhs.sendImmediately;
			DefaultDelay    = rhs.DefaultDelay;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			KeepCommand     = KeepCommandDefault;
			CopyPredefined  = CopyPredefinedDefault;
			SendImmediately = SendImmediatelyDefault;
			DefaultDelay    = DefaultDelayDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("KeepCommand")]
		public virtual bool KeepCommand
		{
			get { return (this.keepCommand); }
			set
			{
				if (value != this.keepCommand)
				{
					this.keepCommand = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CopyPredefined")]
		public virtual bool CopyPredefined
		{
			get { return (this.copyPredefined); }
			set
			{
				if (value != this.copyPredefined)
				{
					this.copyPredefined = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendImmediately")]
		public virtual bool SendImmediately
		{
			get { return (this.sendImmediately); }
			set
			{
				if (value != this.sendImmediately)
				{
					this.sendImmediately = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultDelay")]
		public virtual int DefaultDelay
		{
			get { return (this.defaultDelay); }
			set
			{
				if (value != this.defaultDelay)
				{
					this.defaultDelay = value;
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

			SendSettings other = (SendSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.keepCommand     == other.keepCommand) &&
				(this.copyPredefined  == other.copyPredefined) &&
				(this.sendImmediately == other.sendImmediately) &&
				(this.defaultDelay    == other.defaultDelay)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.keepCommand    .GetHashCode() ^
				this.copyPredefined .GetHashCode() ^
				this.sendImmediately.GetHashCode() ^
				this.defaultDelay   .GetHashCode()
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
