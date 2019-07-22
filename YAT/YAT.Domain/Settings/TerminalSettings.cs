﻿//==================================================================================================
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
using System.Text;
using System.Xml.Serialization;

using MKY.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class TerminalSettings : MKY.Settings.SettingsItem, IEquatable<TerminalSettings>
	{
		/// <summary></summary>
		public const TerminalType TerminalTypeDefault = TerminalType.Text;

		private TerminalType terminalType;

		// Type independent settings:
		private IOSettings io;
		private StatusSettings status;
		private BufferSettings buffer;
		private DisplaySettings display;
		private CharReplaceSettings charReplace;
		private CharHideSettings charHide;
		private SendSettings send;

		// Type dependent settings:
		private TextTerminalSettings textTerminal;
		private BinaryTerminalSettings binaryTerminal;

		/// <summary></summary>
		public TerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			IO          = new IOSettings(settingsType);
			Status      = new StatusSettings(settingsType);
			Buffer      = new BufferSettings(settingsType);
			Display     = new DisplaySettings(settingsType);
			CharReplace = new CharReplaceSettings(settingsType);
			CharHide    = new CharHideSettings(settingsType);
			Send        = new SendSettings(settingsType);

			TextTerminal   = new TextTerminalSettings(settingsType);
			BinaryTerminal = new BinaryTerminalSettings(settingsType);

			UpdateTerminalTypeDependentDefaults(); // Force update *after* all settings
			UpdateIOTypeDependentSettings();       // objects got created.

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalSettings(TerminalSettings rhs)
			: base(rhs)
		{
			TerminalType   = rhs.TerminalType;

			IO             = new IOSettings(rhs.IO);
			Status         = new StatusSettings(rhs.Status);
			Buffer         = new BufferSettings(rhs.Buffer);
			Display        = new DisplaySettings(rhs.Display);
			CharReplace    = new CharReplaceSettings(rhs.CharReplace);
			CharHide       = new CharHideSettings(rhs.CharHide);
			Send           = new SendSettings(rhs.Send);

			TextTerminal   = new TextTerminalSettings(rhs.TextTerminal);
			BinaryTerminal = new BinaryTerminalSettings(rhs.BinaryTerminal);

			UpdateTerminalTypeDependentDefaults(); // Force update *after* all settings
			UpdateIOTypeDependentSettings();       // objects got created.

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalType = TerminalTypeDefault;

			UpdateTerminalTypeDependentDefaults(); // Force update in case type already
			UpdateIOTypeDependentSettings();       // was 'Text' before.
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TerminalType")]
		public TerminalType TerminalType
		{
			get { return (this.terminalType); }
			set
			{
				if (this.terminalType != value)
				{
					this.terminalType = value;

					UpdateTerminalTypeDependentDefaults();
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IO")]
		public IOSettings IO
		{
			get { return (this.io); }
			set
			{
				if (this.io != value)
				{
					var oldNode = this.io;
					this.io = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Status")]
		public StatusSettings Status
		{
			get { return (this.status); }
			set
			{
				if (this.status != value)
				{
					var oldNode = this.status;
					this.status = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Buffer")]
		public BufferSettings Buffer
		{
			get { return (this.buffer); }
			set
			{
				if (this.buffer != value)
				{
					var oldNode = this.buffer;
					this.buffer = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Display")]
		public DisplaySettings Display
		{
			get { return (this.display); }
			set
			{
				if (this.display != value)
				{
					var oldNode = this.display;
					this.display = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharReplace")]
		public CharReplaceSettings CharReplace
		{
			get { return (this.charReplace); }
			set
			{
				if (this.charReplace != value)
				{
					var oldNode = this.charReplace;
					this.charReplace = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharHide")]
		public CharHideSettings CharHide
		{
			get { return (this.charHide); }
			set
			{
				if (this.charHide != value)
				{
					var oldNode = this.charHide;
					this.charHide = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Send")]
		public SendSettings Send
		{
			get { return (this.send); }
			set
			{
				if (this.send != value)
				{
					var oldNode = this.send;
					this.send = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TextTerminal")]
		public TextTerminalSettings TextTerminal
		{
			get { return (this.textTerminal); }
			set
			{
				if (this.textTerminal != value)
				{
					var oldNode = this.textTerminal;
					this.textTerminal = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BinaryTerminal")]
		public BinaryTerminalSettings BinaryTerminal
		{
			get { return (this.binaryTerminal); }
			set
			{
				if (this.binaryTerminal != value)
				{
					var oldNode = this.binaryTerminal;
					this.binaryTerminal = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		#endregion

		#region Property Combinations
		//------------------------------------------------------------------------------------------
		// Property Combinations
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// For text terminals, hide 0xFF is only supported if encoding is single byte.
		/// For binary terminals, hide 0xFF is always supported.
		/// </summary>
		[XmlIgnore]
		public bool SupportsHide0xFF
		{
			get
			{
				if (((TerminalTypeEx)TerminalType).IsText)
				{
					Encoding e = (EncodingEx)TextTerminal.Encoding;
					return (e.IsSingleByte);
				}
				else
				{
					return (true);
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// \remind (2019-04-27 / MKY)
		/// Not a 'nice' solution, but it works... At least if nobody forgets to call this method...
		/// </remarks>
		public virtual void UpdateTerminalTypeDependentDefaults()
		{
			UpdateTerminalTypeDependentDefaults(((TerminalTypeEx)TerminalType).IsBinary);
		}

		/// <remarks>
		/// \remind (2019-04-27 / MKY)
		/// Not a 'nice' solution, but it works... At least if nobody forgets to call this method...
		/// </remarks>
		public virtual void UpdateTerminalTypeDependentDefaults(bool isBinary)
		{
			if (Display != null)
			{
				Display.TxRadix = (isBinary ? DisplaySettings.RadixBinaryDefault : DisplaySettings.RadixTextDefault);
				Display.RxRadix = (isBinary ? DisplaySettings.RadixBinaryDefault : DisplaySettings.RadixTextDefault);

				Display.LengthSelection = (isBinary ? DisplaySettings.LengthSelectionBinaryDefault : DisplaySettings.LengthSelectionTextDefault);
			}

			if (CharReplace != null)
			{
				CharReplace.ReplaceControlChars = (isBinary ? CharReplaceSettings.ReplaceControlCharsBinaryDefault : CharReplaceSettings.ReplaceControlCharsTextDefault);
			}
		}

		/// <remarks>
		/// \remind (2018-02-23 / MKY)
		/// Not a 'nice' solution, but it works... At least if nobody forgets to call this method...
		/// </remarks>
		public virtual void UpdateIOTypeDependentSettings()
		{
			bool isUdpSocket = false;

			if (IO != null)
				isUdpSocket = ((IOTypeEx)IO.IOType).IsUdpSocket;

			UpdateIOTypeDependentSettings(isUdpSocket);
		}

		/// <remarks>
		/// \remind (2018-02-23 / MKY)
		/// Not a 'nice' solution, but it works... At least if nobody forgets to call this method...
		/// </remarks>
		public virtual void UpdateIOTypeDependentSettings(bool isUdpSocket)
		{
			// Attention:
			// When changing code below,...
			// ...code of property further below has to be adapted accordingly.
			// ...messages in terminalSelection_IOTypeChanged() of View.Forms.TerminalSettings have to be adapted accordingly.

			if (Display != null)
			{
				Display.ChunkLineBreakEnabled = isUdpSocket;
			}

			if (TextTerminal != null)
			{
				TextTerminal.TxEol = (isUdpSocket ? ((EolEx)Eol.None).ToSequenceString() : TextTerminalSettings.EolDefault);
				TextTerminal.RxEol = (isUdpSocket ? ((EolEx)Eol.None).ToSequenceString() : TextTerminalSettings.EolDefault);
				TextTerminal.SeparateTxRxEol = false;
			}

			SetMyChanged();
		}

		/// <remarks>
		/// \remind (2018-02-23 / MKY)
		/// Not a 'nice' solution, but it works... At least if nobody forgets to call this method...
		/// </remarks>
		public virtual bool IOTypeDependentSettingsWereDefaults(bool isUdpSocket)
		{
			// Attention:
			// When changing code below,...
			// ...code of method above has to be adapted accordingly.
			// ...messages in terminalSelection_IOTypeChanged() of View.Forms.TerminalSettings have to be adapted accordingly.

			bool areDefaults = true;

			areDefaults &= (Display.ChunkLineBreakEnabled == isUdpSocket);

			areDefaults &= (TextTerminal.TxEol == (isUdpSocket ? ((EolEx)Eol.None).ToSequenceString() : TextTerminalSettings.EolDefault));
			areDefaults &= (TextTerminal.RxEol == (isUdpSocket ? ((EolEx)Eol.None).ToSequenceString() : TextTerminalSettings.EolDefault));
			areDefaults &= (TextTerminal.SeparateTxRxEol == false);

			return (areDefaults);
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

				hashCode = (hashCode * 397) ^ TerminalType.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TerminalSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TerminalSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				TerminalType.Equals(other.TerminalType)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalSettings lhs, TerminalSettings rhs)
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
		public static bool operator !=(TerminalSettings lhs, TerminalSettings rhs)
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
