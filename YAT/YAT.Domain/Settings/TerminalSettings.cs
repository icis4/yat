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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Xml.Serialization;

using MKY.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const TerminalType TerminalTypeDefault = TerminalType.Text;

		private TerminalType terminalType;

		// Type independent settings.
		private IOSettings io;
		private StatusSettings status;
		private BufferSettings buffer;
		private DisplaySettings display;
		private CharReplaceSettings charReplace;
		private CharHideSettings charHide;
		private SendSettings send;

		// Type dependent settings.
		private TextTerminalSettings textTerminal;
		private BinaryTerminalSettings binaryTerminal;

		/// <summary></summary>
		public TerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
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

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TerminalType = TerminalTypeDefault;
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

					// Set terminal type dependent settings:

					bool isBinary = ((TerminalTypeEx)value).IsBinary;

					if (Display != null) {
						Display.TxRadix = (isBinary ? Radix.Hex : Radix.String);
						Display.RxRadix = (isBinary ? Radix.Hex : Radix.String);
					}

					if (CharReplace != null) {
						CharReplace.ReplaceControlChars = (!isBinary);
					}

					SetChanged();
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.io = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.status = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.buffer = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.display = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.charReplace = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.charHide = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.send = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.textTerminal = value;
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
					AttachOrReplaceOrDetachNode(oldNode, value);
					this.binaryTerminal = value;
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

			TerminalSettings other = (TerminalSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(TerminalType == other.TerminalType)
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

				hashCode = (hashCode * 397) ^ TerminalType.GetHashCode();

				return (hashCode);
			}
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
