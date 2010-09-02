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
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<BinaryTerminalSettings>
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
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public BinaryTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			TxDisplay = new BinaryDisplaySettings(SettingsType);
			RxDisplay = new BinaryDisplaySettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public BinaryTerminalSettings(BinaryTerminalSettings rhs)
			: base(rhs)
		{
			this.separateTxRxDisplay = rhs.SeparateTxRxDisplay;
			TxDisplay = new BinaryDisplaySettings(rhs.TxDisplay);
			RxDisplay = new BinaryDisplaySettings(rhs.RxDisplay);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
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
				if (value != this.separateTxRxDisplay)
				{
					this.separateTxRxDisplay = value;
					SetChanged();
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
				if (this.txDisplay == null)
				{
					this.txDisplay = value;
					AttachNode(this.txDisplay);
				}
				else if (value != this.txDisplay)
				{
					BinaryDisplaySettings old = this.txDisplay;
					this.txDisplay = value;
					ReplaceNode(old, this.txDisplay);
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
					return (this.rxDisplay);
				else
					return (this.txDisplay);
			}
			set
			{
				if (this.rxDisplay == null)
				{
					this.rxDisplay = value;
					AttachNode(this.rxDisplay);
				}
				else if (value != this.rxDisplay)
				{
					BinaryDisplaySettings old = this.rxDisplay;
					this.rxDisplay = value;
					ReplaceNode(old, this.rxDisplay);
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
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			BinaryTerminalSettings casted = obj as BinaryTerminalSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(BinaryTerminalSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.
				(this.separateTxRxDisplay == other.separateTxRxDisplay)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators
		//==========================================================================================
		// Comparison Operators
		//==========================================================================================

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryTerminalSettings lhs, BinaryTerminalSettings rhs)
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
		public static bool operator !=(BinaryTerminalSettings lhs, BinaryTerminalSettings rhs)
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
