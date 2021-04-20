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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

using MKY.Text;

#endregion

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\BinarySettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryTerminalSettings : MKY.Settings.SettingsItem, IEquatable<BinaryTerminalSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool SeparateTxRxDisplayDefault = false;

		/// <summary>
		/// Binary terminals are (yet) fixed to <see cref="Encoding.Default"/> which is limited to
		/// an ANSI code page, i.e. always a single byte character set (SBCS).
		/// </summary>
		/// <remarks>
		/// Rationale:
		/// <list type="bullet">
		/// <item>
		/// Binary terminals are intended to mainly be used with numeric radices and the encoding is
		/// only needed for parsing and displaying char/string.
		/// </item>
		/// <item>
		/// Non-SBCS are not implemented (yet).
		/// </item>
		/// </list>
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Orthogonality with 'TextTerminalSettings.EncodingDefault' which also is an instance member.")]
		public readonly int EncodingFixed = (EncodingEx)Encoding.Default;

		#endregion

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

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public BinaryTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			TxDisplay = new BinaryDisplaySettings(SettingsType);
			RxDisplay = new BinaryDisplaySettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
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
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxDisplay = SeparateTxRxDisplayDefault;
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
					return (this.rxDisplay);
				else // Rx redirects to Tx:
					return (this.txDisplay);
			}
			set
			{
				if (this.rxDisplay != value)
				{
					var oldNode = this.rxDisplay;
					this.rxDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}

				// Do not redirect on 'set'. this would not be an understandable behavior.
				// It could even confuse the user, e.g. when temporarily separating the settings,
				// and then load them again from XML => temporary settings get lost.
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

				hashCode = (hashCode * 397) ^ SeparateTxRxDisplay.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as BinaryTerminalSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(BinaryTerminalSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				SeparateTxRxDisplay.Equals(other.SeparateTxRxDisplay)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BinaryTerminalSettings lhs, BinaryTerminalSettings rhs)
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
