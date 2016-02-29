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
using System.Xml.Serialization;

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
		private SendSettings send;

		// Type dependent settings.
		private TextTerminalSettings textTerminal;
		private BinaryTerminalSettings binaryTerminal;

		/// <summary></summary>
		public TerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public TerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			IO          = new IOSettings(SettingsType);
			Status      = new StatusSettings(SettingsType);
			Buffer      = new BufferSettings(SettingsType);
			Display     = new DisplaySettings(SettingsType);
			CharReplace = new CharReplaceSettings(SettingsType);
			Send        = new SendSettings(SettingsType);

			TextTerminal   = new TextTerminalSettings(SettingsType);
			BinaryTerminal = new BinaryTerminalSettings(SettingsType);
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

					if (Display != null) {
						Display.TxRadix = (value == TerminalType.Binary ? Radix.Hex : Radix.String);
						Display.RxRadix = (value == TerminalType.Binary ? Radix.Hex : Radix.String);
					}

					if (CharReplace != null) {
						CharReplace.ReplaceControlChars = (value != TerminalType.Binary);
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
				if (value == null)
				{
					DetachNode(this.io);
					this.io = null;
				}
				else if (this.io == null)
				{
					this.io = value;
					AttachNode(this.io);
				}
				else if (this.io != value)
				{
					IOSettings old = this.io;
					this.io = value;
					ReplaceNode(old, this.io);
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
				if (value == null)
				{
					DetachNode(this.status);
					this.status = null;
				}
				else if (this.status == null)
				{
					this.status = value;
					AttachNode(this.status);
				}
				else if (this.status != value)
				{
					StatusSettings old = this.status;
					this.status = value;
					ReplaceNode(old, this.status);
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
				if (value == null)
				{
					DetachNode(this.buffer);
					this.buffer = null;
				}
				else if (this.buffer == null)
				{
					this.buffer = value;
					AttachNode(this.buffer);
				}
				else if (this.buffer != value)
				{
					BufferSettings old = this.buffer;
					this.buffer = value;
					ReplaceNode(old, this.buffer);
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
				if (value == null)
				{
					DetachNode(this.display);
					this.display = null;
				}
				else if (this.display == null)
				{
					this.display = value;
					AttachNode(this.display);
				}
				else if (this.display != value)
				{
					DisplaySettings old = this.display;
					this.display = value;
					ReplaceNode(old, this.display);
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
				if (value == null)
				{
					DetachNode(this.charReplace);
					this.charReplace = null;
				}
				else if (this.charReplace == null)
				{
					this.charReplace = value;
					AttachNode(this.charReplace);
				}
				else if (this.charReplace != value)
				{
					CharReplaceSettings old = this.charReplace;
					this.charReplace = value;
					ReplaceNode(old, this.charReplace);
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
				if (value == null)
				{
					DetachNode(this.send);
					this.send = null;
				}
				else if (this.send == null)
				{
					this.send = value;
					AttachNode(this.send);
				}
				else if (this.send != value)
				{
					SendSettings old = this.send;
					this.send = value;
					ReplaceNode(old, this.send);
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
				if (value == null)
				{
					DetachNode(this.textTerminal);
					this.textTerminal = null;
				}
				else if (this.textTerminal == null)
				{
					this.textTerminal = value;
					AttachNode(this.textTerminal);
				}
				else if (this.textTerminal != value)
				{
					TextTerminalSettings old = this.textTerminal;
					this.textTerminal = value;
					ReplaceNode(old, this.textTerminal);
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
				if (value == null)
				{
					DetachNode(this.binaryTerminal);
					this.binaryTerminal = null;
				}
				else if (this.binaryTerminal == null)
				{
					this.binaryTerminal = value;
					AttachNode(this.binaryTerminal);
				}
				else if (this.binaryTerminal != value)
				{
					BinaryTerminalSettings old = this.binaryTerminal;
					this.binaryTerminal = value;
					ReplaceNode(old, this.binaryTerminal);
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
			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				TerminalType.GetHashCode()
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
