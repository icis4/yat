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
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class TextTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<TextTerminalSettings>
	{
		/// <summary></summary>
		public static readonly string DefaultEol = (string)XEol.Parse(Environment.NewLine);

		/// <summary></summary>
		public static readonly int DefaultEncoding = (XEncoding)(System.Text.Encoding.Default);

		private bool              separateTxRxEol;
		private string            txEol;
		private string            rxEol;
		private int               encoding;
		private bool              showEol;
		private TextLineSendDelay lineSendDelay;
		private WaitForResponse   waitForResponse;
		private CharSubstitution  charSubstitution;

		/// <summary></summary>
		public TextTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public TextTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			this.separateTxRxEol     = rhs.SeparateTxRxEol;
			this.txEol               = rhs.TxEol;
			this.rxEol               = rhs.RxEol;
			this.encoding            = rhs.Encoding;
			this.showEol             = rhs.ShowEol;
			this.lineSendDelay       = rhs.LineSendDelay;
			this.waitForResponse     = rhs.WaitForResponse;
			this.charSubstitution    = rhs.CharSubstitution;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SeparateTxRxEol  = false;
			TxEol            = DefaultEol;
			RxEol            = DefaultEol;
			Encoding         = DefaultEncoding;
			ShowEol          = false;
			LineSendDelay    = new TextLineSendDelay(false, 500, 1);
			WaitForResponse  = new WaitForResponse(false, 500);
			CharSubstitution = CharSubstitution.None;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxEol")]
		public virtual bool SeparateTxRxEol
		{
			get { return (this.separateTxRxEol); }
			set
			{
				if (value != this.separateTxRxEol)
				{
					this.separateTxRxEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxEol")]
		public virtual string TxEol
		{
			get { return (this.txEol); }
			set
			{
				if (value != this.txEol)
				{
					this.txEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxEol")]
		public virtual string RxEol
		{
			get
			{
				if (this.separateTxRxEol)
					return (this.rxEol);
				else
					return (this.txEol);
			}
			set
			{
				if (value != this.rxEol)
				{
					this.rxEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Encoding")]
		public virtual int Encoding
		{
			get { return (this.encoding); }
			set
			{
				if (value != this.encoding)
				{
					this.encoding = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowEol")]
		public virtual bool ShowEol
		{
			get { return (this.showEol); }
			set
			{
				if (value != this.showEol)
				{
					this.showEol = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LineSendDelay")]
		public virtual TextLineSendDelay LineSendDelay
		{
			get { return (this.lineSendDelay); }
			set
			{
				if (value != this.lineSendDelay)
				{
					this.lineSendDelay = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("WaitForResponse")]
		public virtual WaitForResponse WaitForResponse
		{
			get { return (this.waitForResponse); }
			set
			{
				if (value != this.waitForResponse)
				{
					this.waitForResponse = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharSubstitution")]
		public virtual CharSubstitution CharSubstitution
		{
			get { return (this.charSubstitution); }
			set
			{
				if (value != this.charSubstitution)
				{
					this.charSubstitution = value;
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
			if (obj is TextTerminalSettings)
				return (Equals((TextTerminalSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TextTerminalSettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.separateTxRxEol  == value.separateTxRxEol) &&
					(this.txEol            == value.txEol) &&
					(this.rxEol            == value.rxEol) &&
					(this.encoding         == value.encoding) &&
					(this.showEol          == value.showEol) &&
					(this.lineSendDelay    == value.lineSendDelay) &&
					(this.waitForResponse  == value.waitForResponse) &&
					(this.charSubstitution == value.charSubstitution)
					);
			}
			return (false);
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
		public static bool operator ==(TextTerminalSettings lhs, TextTerminalSettings rhs)
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
		public static bool operator !=(TextTerminalSettings lhs, TextTerminalSettings rhs)
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
