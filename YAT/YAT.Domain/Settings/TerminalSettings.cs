//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettings : MKY.Settings.SettingsItem
	{
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

			TerminalType = TerminalType.Text;
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
				if (value != this.terminalType)
				{
					this.terminalType = value;
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
					this.io = value;
					DetachNode(this.io);
				}
				else if (this.io == null)
				{
					this.io = value;
					AttachNode(this.io);
				}
				else if (value != this.io)
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
					this.status = value;
					DetachNode(this.status);
				}
				else if (this.status == null)
				{
					this.status = value;
					AttachNode(this.status);
				}
				else if (value != this.status)
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
					this.buffer = value;
					DetachNode(this.buffer);
				}
				else if (this.buffer == null)
				{
					this.buffer = value;
					AttachNode(this.buffer);
				}
				else if (value != this.buffer)
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
					this.display = value;
					DetachNode(this.display);
				}
				else if (this.display == null)
				{
					this.display = value;
					AttachNode(this.display);
				}
				else if (value != this.display)
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
					this.charReplace = value;
					DetachNode(this.charReplace);
				}
				else if (this.charReplace == null)
				{
					this.charReplace = value;
					AttachNode(this.charReplace);
				}
				else if (value != this.charReplace)
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
					this.send = value;
					DetachNode(this.send);
				}
				else if (this.send == null)
				{
					this.send = value;
					AttachNode(this.send);
				}
				else if (value != this.send)
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
					this.textTerminal = value;
					DetachNode(this.textTerminal);
				}
				else if (this.textTerminal == null)
				{
					this.textTerminal = value;
					AttachNode(this.textTerminal);
				}
				else if (value != this.textTerminal)
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
				if (this.binaryTerminal == null)
				{
					this.binaryTerminal = value;
					AttachNode(this.binaryTerminal);
				}
				else if (value != this.binaryTerminal)
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
				base.GetHashCode() ^

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
