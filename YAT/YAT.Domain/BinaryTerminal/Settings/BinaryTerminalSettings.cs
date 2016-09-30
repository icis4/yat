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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryTerminalSettings : MKY.Settings.SettingsItem
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool separateTxRxDisplay;
		private BinaryDisplaySettings txDisplay;
		private BinaryDisplaySettings rxDisplay;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public BinaryTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			TxDisplay = new BinaryDisplaySettings(SettingsType);
			RxDisplay = new BinaryDisplaySettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public BinaryTerminalSettings(BinaryTerminalSettings rhs)
			: base(rhs)
		{
			SeparateTxRxDisplay = rhs.SeparateTxRxDisplay;
			TxDisplay = new BinaryDisplaySettings(rhs.TxDisplay);
			RxDisplay = new BinaryDisplaySettings(rhs.RxDisplay);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxDisplay = false;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxDisplay")]
		public virtual bool SeparateTxRxDisplay
		{
			get { return (this.separateTxRxDisplay); }
			set
			{
				if (this.separateTxRxDisplay != value)
				{
					this.separateTxRxDisplay = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxDisplay")]
		public virtual BinaryDisplaySettings TxDisplay
		{
			get { return (this.txDisplay); }
			set
			{
				if (this.txDisplay != value)
				{
					var oldNode = this.txDisplay;
					this.txDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxDisplay")]
		public virtual BinaryDisplaySettings RxDisplay
		{
			get
			{
				if (this.separateTxRxDisplay)
				{
					return (this.rxDisplay);
				}
				else // Rx redirects to Tx:
				{
					return (this.txDisplay);
				}
			}
			set
			{
				if (this.separateTxRxDisplay)
				{
					if (this.rxDisplay != value)
					{
						var oldNode = this.rxDisplay;
						this.rxDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

						AttachOrReplaceOrDetachNode(oldNode, value);
					}
				}
				else // Rx redirects to Tx:
				{
					if (this.txDisplay != value)
					{
						var oldNode = this.txDisplay;
						this.txDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

						AttachOrReplaceOrDetachNode(oldNode, value);
					}
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

			BinaryTerminalSettings other = (BinaryTerminalSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(SeparateTxRxDisplay == other.SeparateTxRxDisplay)
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

				hashCode = (hashCode * 397) ^ SeparateTxRxDisplay.GetHashCode();

				return (hashCode);
			}
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityAnalysis for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
