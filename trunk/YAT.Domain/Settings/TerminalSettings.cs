//==================================================================================================
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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettings>
	{
		private TerminalType terminalType;

		// type independent settings
		private IOSettings io;
		private BufferSettings buffer;
		private DisplaySettings display;
		private CharReplaceSettings charReplace;
		private SendSettings send;

		// type dependent settings
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
		public TerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			IO = new IOSettings(SettingsType);
			Buffer = new BufferSettings(SettingsType);
			Display = new DisplaySettings(SettingsType);
			CharReplace = new CharReplaceSettings(SettingsType);
			Send = new SendSettings(SettingsType);

			TextTerminal = new TextTerminalSettings(SettingsType);
			BinaryTerminal = new BinaryTerminalSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TerminalSettings(TerminalSettings rhs)
			: base(rhs)
		{
			this.terminalType = rhs.TerminalType;

			IO = new IOSettings(rhs.IO);
			Buffer = new BufferSettings(rhs.Buffer);
			Display = new DisplaySettings(rhs.Display);
			CharReplace = new CharReplaceSettings(rhs.CharReplace);
			Send = new SendSettings(rhs.Send);

			TextTerminal = new TextTerminalSettings(rhs.TextTerminal);
			BinaryTerminal = new BinaryTerminalSettings(rhs.BinaryTerminal);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
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
				if (this.io == null)
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
		[XmlElement("Buffer")]
		public BufferSettings Buffer
		{
			get { return (this.buffer); }
			set
			{
				if (this.buffer == null)
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
				if (this.display == null)
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
				if (this.charReplace == null)
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
				if (this.send == null)
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
				if (this.textTerminal == null)
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
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			TerminalSettings casted = obj as TerminalSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TerminalSettings casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)casted) && // Compare all settings nodes.
				(this.terminalType == casted.terminalType)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalSettings lhs, TerminalSettings rhs)
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
